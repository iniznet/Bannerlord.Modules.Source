using System;

namespace JetBrains.Annotations
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class LocalizationRequiredAttribute : Attribute
	{
		public LocalizationRequiredAttribute(bool required)
		{
			this.Required = required;
		}

		[UsedImplicitly]
		public bool Required { get; set; }

		public override bool Equals(object obj)
		{
			LocalizationRequiredAttribute localizationRequiredAttribute = obj as LocalizationRequiredAttribute;
			return localizationRequiredAttribute != null && localizationRequiredAttribute.Required == this.Required;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
