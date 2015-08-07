/* Available attributes:
 * [CascadeAll]				applies to entity properties and collections; default is CascadeSaveUpdate
 * [CascadeAllDeleteOrphans]	"
 * [CascadeNone]				"
 * [Component]				applies to entity classes; works with properties and collections
 * [ManyToMany]				applies to collections; specifies "first" entity in Table_Name and may specify a unidirectional ManyToMany relationship (lacking an opposite side)
 * [PrecisionScale(p, s)]	applies to decimal properties; (5, 4) => 3.1415
 * [Required]				applies to properties and collections
 * [StringLength(n)]		applies to string properties
 * [Text]					applies to string properties
 * [Unique]					applies to properties and collections
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.NHib.DataAnnotations {
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class KeepSequenceAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Property)]
	public sealed class NotPersistedAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class NotMappedAttribute : Attribute { }


	[AttributeUsage(AttributeTargets.Property)]
	public sealed class AssignedAttribute : Attribute { }


	[AttributeUsage(AttributeTargets.Property)]
	public sealed class RequiredAttribute : Attribute { }


	[AttributeUsage(AttributeTargets.Property)]
	public sealed class StringLengthAttribute : Attribute {
		private int maxLen = 255;

		public StringLengthAttribute(int maxLength) {
			this.maxLen = maxLength;
		}

		public int MaximumLength {
			get { return this.maxLen; }
		}
	}


	[AttributeUsage(AttributeTargets.Property)]
	public sealed class IndexedAttribute : Attribute { }


	[AttributeUsage(AttributeTargets.Property)]
	public sealed class UniqueAttribute : Attribute { }


	[AttributeUsage(AttributeTargets.Property)]
	public sealed class TextAttribute : Attribute { }


	[AttributeUsage(AttributeTargets.Property)]
	public sealed class PrecisionScaleAttribute : Attribute {
		private int precision = 8;
		private int scale = 2;

		public PrecisionScaleAttribute(int precision, int scale) {
			this.precision = precision;
			this.scale = scale;
		}

		public int Precision {
			get { return this.precision; }
		}
		public int Scale {
			get { return this.scale; }
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public sealed class CascadeAllDeleteOrphansAttribute : Attribute { }


	[AttributeUsage(AttributeTargets.Property)]
	public sealed class CascadeAllAttribute : Attribute { }


	[AttributeUsage(AttributeTargets.Property)]
	public sealed class CascadeNoneAttribute : Attribute { }


	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public sealed class ComponentAttribute : Attribute { }


	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ManyToManyAttribute : Attribute { }


	[AttributeUsage(AttributeTargets.Property)]
	public sealed class InverseAttribute : Attribute { }
}
