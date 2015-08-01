using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;

using Poker.NHib.Conventions;
using Poker.NHib.DataAnnotations;
using Poker.Utils;


namespace Poker.NHib {
	/// <summary>
	/// Configures NHibernate automapping. Accepts connectionName, mapping from namespaces of specified Types, and an auxiliary ShouldMap() function.
	/// Provides ConnectionString and SessionFactory.
	/// </summary>
	public class NHConfiguration : DefaultAutomappingConfiguration {
		private const string defaultConnName = "PokerDb";
		// use "Namespace.Type, Assembly" to describe defaults so that Poker.DBModel is not required unlesss we actually use the default
		private string[] defaultMapFromAssemblyOf = new string[] { "Poker.DbModels.Card, Poker.DbModels" };

		public string ConnectionName { get; private set; }
		public string ConnectionString { get; private set; }
		public bool DbDropCreate { get; private set; }
		public bool DbSchemaUpdate { get; private set; }
		public Assembly[] Assemblies { get; private set; }
		protected Func<Type, bool> AuxShouldMap { get; set; }

		/// <summary>
		/// Returns the singleton session factory for this NHConfiguration.
		/// </summary>
		public ISessionFactory SessionFactory { // lazy creation of ISessionFactory
			get {
				if (_SessionFactory == null) {
					if (DbDropCreate) DbUtils.DropCreateDB(ConnectionString);
					_SessionFactory = FluentConfiguration
						.ExposeConfiguration(ExecuteSchemaActions)
						.BuildSessionFactory();
				}
				return _SessionFactory;
			}
		}
		private ISessionFactory _SessionFactory;

		/// <summary>
		/// Returns the singleton FluentConfiguration for this NHConfiguration.
		/// </summary>
		private FluentConfiguration FluentConfiguration { // lazy NH mapping
			get {
				if (_FluentConfiguration == null) {
					AutoPersistenceModel apm = AutoMap.Assemblies(this, this.Assemblies.Distinct());
					_FluentConfiguration = Fluently.Configure()
						//.Diagnostics(d => { d.Enable(); d.OutputToConsole(); })
						.Database(MsSqlConfiguration.MsSql2008.ConnectionString(this.ConnectionString).AdoNetBatchSize(batchSize > 0 ? batchSize : 0).UseReflectionOptimizer())
                        .Mappings(m => m.AutoMappings.Add(SetConventions(apm)));
				}
				return _FluentConfiguration;
			}
		}
		private FluentConfiguration _FluentConfiguration;

		private string[] mapNamespaces;
		const int batchSize = 0; //100;


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="connectionName">Name of connection in web.config and/or vsOverride.config. Defaults to "tce_db".</param>
		/// <param name="dbDropCreate">When true, will drop/create db on creation of session factory. Unsafe for multiple instances.</param>
		/// <param name="dbSchemaUpdate">When true, runs SchemaUpdate() against the db.</param>
		/// <param name="mapFromAssembliesOfType">Types used to determine Assemblies and namespaces from which to map entities. Defaults to TCE.DBModel.Question</param>
		/// <param name="auxShouldMap">Auxiliary Function&lt;Type, bool> ShouldMap(), logically ANDed with base::ShouldMap(). Defaults to { return true; }</param>
		public NHConfiguration(
			string connectionName = defaultConnName,
			bool dbDropCreate = false,
			bool dbSchemaUpdate = false,
			IEnumerable<Type> mapFromAssembliesOfType = null,
			Func<Type, bool> auxShouldMap = null
		) {
			// Note that if the assembly specified in defaultMapFromAssemblyOf is not loaded and mapFromAssembliesOfType is not specified,
			// the next line will result in a zero-length array and nothing will be mapped. This should fit the intent of such a case.
			if (mapFromAssembliesOfType == null) mapFromAssembliesOfType = defaultMapFromAssemblyOf.Select(a => Type.GetType(a)).Where(b => b != null).ToArray();
			mapNamespaces = mapFromAssembliesOfType.Select(t => t.Namespace).ToArray();

			ConnectionName = connectionName;
			DbDropCreate = dbDropCreate;
			DbSchemaUpdate = dbSchemaUpdate;
			Assemblies = mapFromAssembliesOfType.Select(a => a.Assembly).ToArray();
			AuxShouldMap = (auxShouldMap == null) ? (f) => { return true; } : auxShouldMap;

			ConfigUtils.OverrideConnectionStrings(); // adds to or replaces connection strings in ConfigurationManager
			var css = System.Configuration.ConfigurationManager.ConnectionStrings.Cast<System.Configuration.ConnectionStringSettings>().FirstOrDefault(a => a.Name == ConnectionName);
			if (css != null) 
				ConnectionString = css.ConnectionString;
			else 
				throw new PokerException(String.Format("ConnectionString named '{0}' not found.", ConnectionName));
		}

		/// <summary>
		/// Export the schema specified by this NHConfiguration to a file.
		/// </summary>
		/// <param name="path">The full path of the file to create. Will overwrite an existing file.</param>
		public void ExportSchemaToFile(string path) {
			FluentConfiguration.ExposeConfiguration(cfg => { new SchemaExport(cfg).SetOutputFile(path).Create(false, false); });
		}

		/// <summary>
		/// Specifies per Type what should be mapped. Intended for use by NH Automapping.
		/// </summary>
		/// <param name="type">The Type to test.</param>
		/// <returns>True/False</returns>
		public override bool ShouldMap(Type type) {
			if (type.ContainsGenericParameters) return false;
			if (!mapNamespaces.Contains(type.Namespace)) return false;
			if (type.GetCustomAttributes<NotMappedAttribute>(false).Count() > 0) return false;
			bool rv = AuxShouldMap(type) ? base.ShouldMap(type) : false;
			return rv;
		}

		/// <summary>
		/// Uses [Component] to flag component classes. Intended for use by NH Automapping.
		/// </summary>
		/// <param name="type">The type to test and mark as a component if appropriate.</param>
		/// <returns>True/False</returns>
		public override bool IsComponent(Type type) {
			bool rv = type.IsDefined(typeof(ComponentAttribute), true);
			return rv;
		}

		/// <summary>
		/// Sets column name of components within a class to "componentClassName_propertyName". Intended for use by NH Automapping.
		/// </summary>
		/// <param name="member">The property member.</param>
		/// <returns>The column name.</returns>
		public override string GetComponentColumnPrefix(Member member) {
			return member.Name + "_";
		}

		/// <summary>
		/// set automapping conventions and alterations
		/// </summary>
		/// <param name="apm">The AutoPersistenceModel to modify.</param>
		/// <returns></returns>
		private AutoPersistenceModel SetConventions(AutoPersistenceModel apm) {
            return apm
                .Alterations(a => a.Add<BaseAlteration>())
                .OverrideAll(c => { c.IgnoreProperties(p => p.MemberInfo.GetCustomAttributes<NotPersistedAttribute>(false).Count() > 0); })

                // provided helper conventions - put before custom conventions so things like keynames are completed by the time we get to custom conventions
                .Conventions.Add(
                    //StringMaxLengthConvention(),
                    PrimaryKey.Name.Is(a => "Id"),
                    ForeignKey.EndsWith("Id"),
                    //AutoImport.Never(), // requires namespace.class; only needed if dup class names in diff namespaces
                    //DefaultAccess.Field(), // default is "property"
                    DefaultCascade.SaveUpdate(),
                    DefaultLazy.Always()
                //Cache.Is(a => a.ReadOnly())
                //DynamicInsert.AlwaysTrue(),
                //DynamicUpdate.AlwaysTrue(),
                //OptimisticLock.Is(a => a.Dirty()), // for DynamicInsert/Update; version recommended
                )
                // custom conventions
                .Conventions.Add<EnumConvention>()
                .Conventions.Add<AssignedIdConvention>()
                .Conventions.Add<StringLengthAttributeConvention>()
                .Conventions.Add<RequiredAttributeConvention>()
                .Conventions.Add<IndexedAttributeConvention>()
                .Conventions.Add<UniqueAttributeConvention>()
                .Conventions.Add<TextAttributeConvention>()
                .Conventions.Add<PrecisionScaleAttributeConvention>()
                //.Conventions.Add<ManyToManyAttributeConvention>()
                .Conventions.Add<CustomManyToManyTableNameConvention>()
                .Conventions.Add<CustomHasManyConvention>()
                .Conventions.Add<CustomManyToManyConvention>();
		}

		private void ExecuteSchemaActions(Configuration cfg) {
			if (this.DbSchemaUpdate) {
				new SchemaUpdate(cfg).Execute(false, true);
			}
			try {
				new SchemaValidator(cfg).Validate();
			}
			catch (Exception e) {
				PokerException ex = new PokerException("NH schema validation failed. Application schema does not match database schema.", e);
				throw (ex);
			}
		}

	}

	[Serializable]
	public class PokerException : Exception {
		public PokerException() { }
		public PokerException(string message) : base(message) { }
		public PokerException(string message, Exception innerException) : base(message, innerException) { }
	}

}
