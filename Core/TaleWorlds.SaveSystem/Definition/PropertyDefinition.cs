using System;
using System.Reflection;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	public class PropertyDefinition : MemberDefinition
	{
		public PropertyInfo PropertyInfo { get; private set; }

		public SaveablePropertyAttribute SaveablePropertyAttribute { get; private set; }

		public MethodInfo GetMethod { get; private set; }

		public MethodInfo SetMethod { get; private set; }

		public GetPropertyValueDelegate GetPropertyValueMethod { get; private set; }

		public PropertyDefinition(PropertyInfo propertyInfo, MemberTypeId id)
			: base(propertyInfo, id)
		{
			this.PropertyInfo = propertyInfo;
			this.SaveablePropertyAttribute = propertyInfo.GetCustomAttribute<SaveablePropertyAttribute>();
			this.SetMethod = this.PropertyInfo.GetSetMethod(true);
			if (this.SetMethod == null && this.PropertyInfo.DeclaringType != null)
			{
				PropertyInfo property = this.PropertyInfo.DeclaringType.GetProperty(this.PropertyInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (property != null)
				{
					this.SetMethod = property.GetSetMethod(true);
				}
			}
			if (this.SetMethod == null)
			{
				Debug.FailedAssert(string.Concat(new string[]
				{
					"Property ",
					this.PropertyInfo.Name,
					" at Type ",
					this.PropertyInfo.DeclaringType.FullName,
					" does not have setter method."
				}), "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.SaveSystem\\Definition\\PropertyDefinition.cs", ".ctor", 39);
				throw new Exception(string.Concat(new string[]
				{
					"Property ",
					this.PropertyInfo.Name,
					" at Type ",
					this.PropertyInfo.DeclaringType.FullName,
					" does not have setter method."
				}));
			}
			this.GetMethod = this.PropertyInfo.GetGetMethod(true);
			if (this.GetMethod == null && this.PropertyInfo.DeclaringType != null)
			{
				PropertyInfo property2 = this.PropertyInfo.DeclaringType.GetProperty(this.PropertyInfo.Name);
				if (property2 != null)
				{
					this.GetMethod = property2.GetGetMethod(true);
				}
			}
			if (this.GetMethod == null)
			{
				throw new Exception(string.Concat(new string[]
				{
					"Property ",
					this.PropertyInfo.Name,
					" at Type ",
					this.PropertyInfo.DeclaringType.FullName,
					" does not have getter method."
				}));
			}
		}

		public override Type GetMemberType()
		{
			return this.PropertyInfo.PropertyType;
		}

		public override object GetValue(object target)
		{
			object obj;
			if (this.GetPropertyValueMethod != null)
			{
				obj = this.GetPropertyValueMethod(target);
			}
			else
			{
				obj = this.GetMethod.Invoke(target, new object[0]);
			}
			return obj;
		}

		public void InitializeForAutoGeneration(GetPropertyValueDelegate getPropertyValueMethod)
		{
			this.GetPropertyValueMethod = getPropertyValueMethod;
		}
	}
}
