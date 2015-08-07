using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Mapping;
using FluentNHibernate.Mapping.Providers;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

using Poker.NHib.DataAnnotations;
using Poker.NHib;

namespace Poker.NHib.Conventions {

	// override GenericEnumMapper so that enums map to the actual underlying type
	// we assume that all enums should be non-nullable, but if we need nullable enums see http://stackoverflow.com/questions/439003/how-do-you-map-an-enum-as-an-int-value-with-fluent-nhibernate/2716236#2716236
	public class EnumConvention : IUserTypeConvention {
		public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria) {
			criteria.Expect(x => x.Property.PropertyType.IsEnum);
		}

		public void Apply(IPropertyInstance target) {
			target.CustomType(target.Property.PropertyType);
			target.Not.Nullable();
		}
	}

	// tell NH that an Id property is programatically assigned
	public class AssignedIdConvention : IIdConvention, IIdConventionAcceptance {
		public void Accept(IAcceptanceCriteria<IIdentityInspector> criteria) {
			criteria.Expect(c => c.Property.MemberInfo.IsDefined(typeof(AssignedAttribute), false));
		}

		public void Apply(IIdentityInstance instance) {
			instance.GeneratedBy.Assigned();
		}
	}

	// override default string column length
	public class StringLengthAttributeConvention : IPropertyConvention, IPropertyConventionAcceptance {
		public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria) {
			criteria.Expect(c => c.Property.MemberInfo.IsDefined(typeof(StringLengthAttribute), false))
				.Expect(c => c.Property.PropertyType == typeof(string))
				.Expect(c => !c.Property.MemberInfo.IsDefined(typeof(TextAttribute), false));
		}
		public void Apply(IPropertyInstance instance) {
			var attribute = Attribute.GetCustomAttribute(instance.Property.MemberInfo, typeof(StringLengthAttribute)) as StringLengthAttribute;
			if (attribute.MaximumLength != default(int)) instance.Length(attribute.MaximumLength);
		}
	}

	public class RequiredAttributeConvention : IPropertyConvention, IReferenceConvention {
		public void Apply(IManyToOneInstance instance) {
			if (instance.Property.MemberInfo.IsDefined(typeof(RequiredAttribute), false))
				instance.Not.Nullable();
		}
		public void Apply(IPropertyInstance instance) {
			if (instance.Property.MemberInfo.IsDefined(typeof(RequiredAttribute), false))
				instance.Not.Nullable();
		}
	}

	public class IndexedAttributeConvention : AttributePropertyConvention<IndexedAttribute> {
		protected override void Apply(IndexedAttribute attribute, IPropertyInstance instance) {
			instance.Index("i" + instance.EntityType.Name + "_" + instance.Name);
		}
	}

	public class UniqueAttributeConvention : AttributePropertyConvention<UniqueAttribute> {
		protected override void Apply(UniqueAttribute attribute, IPropertyInstance instance) {
			instance.Unique();
		}
	}

	public class TextAttributeConvention : IPropertyConvention, IPropertyConventionAcceptance {
		public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria) {
			criteria.Expect(c => c.Property.MemberInfo.IsDefined(typeof(TextAttribute), false))
				.Expect(c => c.Property.PropertyType == typeof(string));
		}
		public void Apply(IPropertyInstance instance) {
			instance.CustomType("StringClob");
			instance.CustomSqlType("nvarchar(max)");
		}
	}

	public class PrecisionScaleAttributeConvention : IPropertyConvention, IPropertyConventionAcceptance {
		public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria) {
			criteria.Expect(c => c.Property.MemberInfo.IsDefined(typeof(PrecisionScaleAttribute), false))
				.Expect(c => c.Property.PropertyType == typeof(decimal));
		}
		public void Apply(IPropertyInstance instance) {
			var attribute = Attribute.GetCustomAttribute(instance.Property.MemberInfo, typeof(PrecisionScaleAttribute)) as PrecisionScaleAttribute;
			instance.Precision(attribute.Precision);
			instance.Scale(attribute.Scale);
		}
	}

	//// happens after base mapping of relationships
	//public class ManyToManyAttributeConvention : IPropertyConvention, IPropertyConventionAcceptance {
	//	public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria) {
	//		criteria.Expect(c => c.Property.MemberInfo.IsDefined(typeof(ManyToManyAttribute), false));
	//			//.Expect(c => c.Property.PropertyType == typeof(Collection));
	//	}
	//	public void Apply(IPropertyInstance instance) {
	//		int i = 0;
	//	}
	//}

	public class CustomManyToManyTableNameConvention : ManyToManyTableNameConvention {
		protected override string GetUniDirectionalTableName(IManyToManyCollectionInspector collection) {
			return collection.EntityType.Name + "_" + collection.ChildType.Name;
		}

		protected override string GetBiDirectionalTableName(IManyToManyCollectionInspector collection, IManyToManyCollectionInspector otherSide) {
			string co = collection.EntityType.Name + "_" + otherSide.EntityType.Name;
			string oc = otherSide.EntityType.Name + "_" + collection.EntityType.Name;
			if (collection.Member.IsDefined(typeof(ManyToManyAttribute), false)) {
				return collection.TableName == oc ? oc : co; // if tablename was already set (matches inverse name), don't change it
			}
			else {
				return collection.TableName == co ? co : oc;
			}
		}
	}

	public class CustomHasManyConvention : IHasManyConvention {
		public void Apply(IOneToManyCollectionInstance instance) {
            // by definition we are only here if instance is a collection, never on the "many" (reference) side of a OneToMany
            string childName = instance.ChildType.Name;
			string entityName = instance.EntityType.Name;

			// set collection storage type
			Type memberPropertyType = (instance.Member as PropertyInfo).PropertyType;
			if (memberPropertyType.IsGenericType && memberPropertyType.GetGenericTypeDefinition() == typeof(ISet<>)) {
				instance.AsSet();
			}
			else if (instance.Member.IsDefined(typeof(KeepSequenceAttribute), false)) {
				instance.AsList();
				instance.Index.Column("Seq_" + entityName);
			}
			else {
				instance.AsBag();
			}

			// set override cascade (default is CascadeSaveUpdate)
			if (instance.Member.IsDefined(typeof(CascadeAllDeleteOrphansAttribute), false)) instance.Cascade.AllDeleteOrphan();
			else if (instance.Member.IsDefined(typeof(CascadeAllAttribute), false)) instance.Cascade.All();
			else if (instance.Member.IsDefined(typeof(CascadeNoneAttribute), false)) instance.Cascade.None();

			// collection of components - successful mapping assumes simple case, properties only, no references or nested components
			if (instance.ChildType.IsDefined(typeof(ComponentAttribute), false)) {
				instance.Table(entityName + childName); // set the table name for the collection of components

				// get mapping of collection, remove default generic Relationship property, and set CompositeElementMapping property
				var mapping = (CollectionMapping)typeof(OneToManyCollectionInstance).GetField("mapping", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
				mapping.Set<ICollectionRelationshipMapping>(x => x.Relationship, Layer.Defaults, null);
				mapping.Set<CompositeElementMapping>(x => x.CompositeElement, Layer.Defaults, GetCompositeElementCollectionMapping(instance));
			}

			// set inverse if appropriate. If inverse is on the collection side, the other (child, many, reference) side will control the relationship
			if (instance.Member.IsDefined(typeof(InverseAttribute), false)) instance.Inverse(); // if Inverse attribute specified, set it

			if (instance.OtherSide != null && instance.OtherSide.EntityType.Name == instance.ChildType.Name) {
				if (memberPropertyType.IsGenericType) {
					var genericType = memberPropertyType.GetGenericTypeDefinition();
					if (genericType == typeof(IList<>) || genericType == typeof(ISet<>)) {
						if (instance.OtherSide.Property.MemberInfo.IsDefined(typeof(RequiredAttribute))) {
							string genType = new Regex(@"^([\w]+)").Match(genericType.Name).Groups[0].Captures[0].Value;
							throw new PokerException(String.Format("Attribute conflict. The reference on the other side of an {4}<T> collection cannot be required. {4}<{2}> {0}.{1}; [Required] {0} {2}.{3}.", entityName, instance.Member.Name, childName, instance.OtherSide.Property.Name, genType));
						}
					}
				}
			}
		}

		/// <summary>
		/// Returns the <see cref="CompositeElementMapping"/> for a collection of components, suitable for use in (CollectionMapping)mapping.Set&lt;CompositeElementMapping>().
		/// </summary>
		/// <param name="instance">The current <see cref="IOneToManyCollectionInstance"/> instance of a collection of components such as IHasManyConvention:Apply() receives.</param>
		/*	The first step is to create an object of generic type CompositeElementPart<T>(typeof(OneToManyPart<T>)), where T is the type of the component class.
		 *
		 *	Then we must map each property of T into that object. This is done by creating a generic type matching constructor
		 *	MapPropertiesToCompositeElementPart<T>(CompositeElementPart<T>) and instantiating it, which performs the mapping.
		 *
		 *	Finally, retrieve the resultant CompositeElementMapping and return it. This must be done through the ICompositeElementMappingProvider interface
		 *	because GetCompositeElementMapping() is an explicit interface implementation.
		*/
		public CompositeElementMapping GetCompositeElementCollectionMapping(IOneToManyCollectionInstance instance) {
			//var part = new CompositeElementPart<T>(typeof(OneToManyPart<T>));
			Type gtCompositeElementPart = typeof(CompositeElementPart<>).MakeGenericType(instance.ChildType);
			Type gtOneToManyPart = typeof(OneToManyPart<>).MakeGenericType(instance.ChildType);
			var part = Activator.CreateInstance(gtCompositeElementPart, gtOneToManyPart);

			var mapperType = typeof(MapPropertiesToCompositeElementPart<>).MakeGenericType(new Type[] { instance.ChildType });
			Activator.CreateInstance(mapperType, new object[] { part });

			ICompositeElementMappingProvider partI = (ICompositeElementMappingProvider)part;
			return partI.GetCompositeElementMapping();
		}

		//public void Apply(IOneToManyInstance instance) {
		//	string entityName = instance.EntityType.Name;
		//}
		//public void Apply(IManyToOneInstance instance) {
		//	string instanceName = instance.Name;
		//	string entityName = instance.EntityType.Name;
		//}
	}

	/// <summary>
	/// Maps properties of component type T to an instance of CompositeElementPart<T>. The constructor performs the mapping.
	/// </summary>
	/// <typeparam name="T">The component class.</typeparam>
	public class MapPropertiesToCompositeElementPart<T> {
		static Func<CompositeElementPart<T>, object> compiledFunc = null;

		public MapPropertiesToCompositeElementPart(CompositeElementPart<T> part) {
			if (compiledFunc == null) { // create/compile expression tree specific to T if we haven't already
				var instanceParm = Expression.Parameter(typeof(CompositeElementPart<T>), "part"); // create Expression parameter for instance
				// find FluentNHibernate method CompositeElementPart<T>.Map(Expression<Func<T, object>>)
				var method = typeof(CompositeElementPart<T>).GetMethod("Map", new Type[] { typeof(Expression<Func<T, object>>) });

				// setup a call expression for each property in component class: y => y.prop
				var innerParm = Expression.Parameter(typeof(T), "y");
				List<Expression> calls = new List<Expression>();
				foreach (var prop in typeof(T).GetProperties()) {
					var lambdaParm = Expression.Lambda(Expression.Convert(Expression.Property(innerParm, prop.Name), typeof(object)), innerParm);
					var call = Expression.Call(instanceParm, method, lambdaParm);
					calls.Add(call);
				}
				// setup single expression containing all subsidiary calls x => { y => y.prop1, y => y.prop2,... }
				var lambda = Expression.Lambda(Expression.Block(calls), instanceParm);
				compiledFunc = (Func<CompositeElementPart<T>, object>)lambda.Compile();
			}
			compiledFunc(part); // execute compiled function
		}
	}

	public class CustomManyToManyConvention : IHasManyToManyConvention {
		public void Apply(IManyToManyCollectionInstance instance) {
			string childName = instance.ChildType.Name;
			string entityName = instance.EntityType.Name;

			//// handy for debugging
			//var mtm = instance.Member.IsDefined(typeof(ManyToManyAttribute), false);
			//var osmtm = instance.OtherSide == null ? false : instance.OtherSide.Member.IsDefined(typeof(ManyToManyAttribute), false);

			Type mpt = (instance.Member as PropertyInfo).PropertyType;
			Type mpgt = mpt.IsGenericType ? mpt.GetGenericTypeDefinition() : typeof(object);
			// cannot ensure both sides same collection type here because if they aren't, will be two HasMany instead

			// set collection storage type
			if (mpgt == typeof(ISet<>)) {
				instance.AsSet();
				//Console.WriteLine(entityName + ":" + childName + ": AsSet");
			}
				else if (instance.Member.IsDefined(typeof(KeepSequenceAttribute), false)) {
				instance.AsList();
				instance.Index.Column("Seq_" + entityName);
			}
			else {
				instance.AsBag();
			}

			// set cascade
			if (instance.Member.IsDefined(typeof(CascadeAllDeleteOrphansAttribute), false)) instance.Cascade.AllDeleteOrphan();
			else if (instance.Member.IsDefined(typeof(CascadeAllAttribute), false)) instance.Cascade.All();
			else if (instance.Member.IsDefined(typeof(CascadeNoneAttribute), false)) instance.Cascade.None();

			// set unique constraint on the relationship
			if (instance.Member.IsDefined(typeof(UniqueAttribute), false)) {
				string uniqueConstraintName = "uc" + entityName + childName;

				// this block works fine for bidirectional, but fails for unidir
				//foreach (IManyToManyCollectionInstance mmci in (new IManyToManyCollectionInstance[] { instance, instance.OtherSide }).Where(x => x != null)) {
				//	foreach (ColumnInstance ci in mmci.Relationship.Columns) {
				//		var ciMapping = (ColumnMapping)typeof(ColumnInstance).GetField("mapping", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ci);
				//		ciMapping.Set<string>(x => x.UniqueKey, Layer.Defaults, uniqueConstraintName);
				//	}
				//}
				// so instead use key columns (entity) and relationship columns (child). If this becomes problematic for paired many-to-many, then use first block for paired and this for unidirectional.
				foreach (var ci in instance.Key.Columns.Union(instance.Relationship.Columns)) { // columns from both key and relationship. (key.Columns are ColumnInspector; Relationship.Columns are ColumnInstance)
					var ciMapping = (ColumnMapping)ci.GetType().GetField("mapping", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ci);
					ciMapping.Set<string>(x => x.UniqueKey, Layer.Defaults, uniqueConstraintName);
				}
			}

			if (instance.Member.IsDefined(typeof(InverseAttribute), false)) { // if Inverse attribute specified, force this side Inverse, other side not
				instance.Inverse();
				if (instance.OtherSide != null) {
					if (instance.OtherSide.Member.IsDefined(typeof(InverseAttribute), false)) { // test for inverse both sides
						throw new PokerException(String.Format("Attribute conflict. Both sides of a relationship cannot be Inverse. {0}.{1}; {2}.{3}.", entityName, instance.Member.Name, childName, instance.OtherSide.Member.Name));
					}
					var osm = typeof(ManyToManyCollectionInstance).GetField("mapping", BindingFlags.Instance | BindingFlags.NonPublic); // get the FieldInfo for the mapping field
					var osmv = (CollectionMapping)osm.GetValue(instance.OtherSide); // get the mapping field itself
					osmv.Set(a => a.Inverse, Layer.Defaults, false); // set inverse
					osm.SetValue(instance.OtherSide, osmv); // set modified mapping field
				}
			}
			// else Inverse attribute not specified
			else if (instance.OtherSide != null) { // null means unidirectional ManyToMany from Override
				// if inverse already set either side, leave it alone
				// else set either by ManyToManyAttribute or implicit heirarchy ordering of visits here (which suggests that on first visit otherside gets it)
				bool iInverse = ((ICollectionInspector)instance).Inverse;
				bool osInverse = ((ICollectionInspector)instance.OtherSide).Inverse;
				if (!iInverse && !osInverse) {
					var iSide = instance.Member.IsDefined(typeof(ManyToManyAttribute), false) ? instance : instance.OtherSide;
					iSide.Inverse();
				}
			}
		}
	}

	/// <summary>
	/// Allows altering AutoPersistenceModel. Occurs before mappings are generated, but after inline .Override()s.
	/// </summary>
	public class BaseAlteration : IAutoMappingAlteration {
		public void Alter(AutoPersistenceModel model) {
			// get the types that will be mapped
			var sources = (List<ITypeSource>)typeof(AutoPersistenceModel).GetField("sources", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(model);
			var types = sources.SelectMany(a => a.GetTypes());

			// detect and force NH to create a ManyToMany relationship for potentially unidirectional cases
			// get generic properties with ManyToManyAttribute
			var mtms = types.SelectMany(a => a.GetInstanceMembers()).Where(b => b.MemberInfo.IsDefined(typeof(ManyToManyAttribute)) && b.PropertyType.IsGenericType); // need namespace==systems.collections.generic?
			// synthesize a HasManyToMany override for each
			foreach (Member m in mtms) {
				var mapperType = typeof(CreateHasManyToManyOverride<,>).MakeGenericType(new Type[] { m.DeclaringType, m.PropertyType.GenericTypeArguments[0] });
				var instance = Activator.CreateInstance(mapperType, new object[] { model, m });
			}
		}
	}

	/// <summary>
	/// Creates a HasManyToMany Override&lt;TP, TC>. The constructor performs the mapping.
	/// </summary>
	/// <typeparam name="TP">The parent class being overriden.</typeparam>
	/// <typeparam name="TC">The child class with which we have a ManyToMany relationship.</typeparam>
	/* I suspect this needs to be an instance rather than static because the lambda is a callback.
	 */
	public class CreateHasManyToManyOverride<TP, TC> {
		public CreateHasManyToManyOverride(AutoPersistenceModel model, Member m) {
			var parm = Expression.Parameter(typeof(TP), "b");
			var lambda = Expression.Lambda<Func<TP, object>>(Expression.Property(parm, m.Name), parm);
			var qry = model.Override<TP>(a => a.HasManyToMany<TC>(lambda));
		}
	}

	// so far, no necessary/useful opportunity here
	//public class CustomReferenceConvention : IReferenceConvention {
	//	public void Apply(IManyToOneInstance instance) {
	//		//string entityName = instance.EntityType.Name;
	//		if (instance.Property.MemberInfo.IsDefined(typeof(UniqueAttribute))) instance.Unique();
	//	}
	//}
}

