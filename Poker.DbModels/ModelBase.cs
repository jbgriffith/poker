using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Poker.NHib.DataAnnotations;

namespace Poker.DbModels {
	public abstract class ModelBase {
		/// <summary>
		/// Validates that [Required] properties of the calling class have non-null values.
		/// </summary>
		/// <returns>A System.Reflection.PropertyInfo[] containing properties that failed validation.</returns>
		public virtual PropertyInfo[] ValidateRequired() {
			return this.GetType().GetProperties().Where(a => a.IsDefined(typeof(RequiredAttribute), false) && a.GetValue(this) == null).ToArray();
		}
	}

	public abstract class ModelBaseGuid : ModelBase {
		public virtual Guid Id { get; set; }
	}
}
