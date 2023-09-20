using System;
using System.Reflection;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000064 RID: 100
	public class PropertyDefinition : MemberDefinition
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060002EB RID: 747 RVA: 0x0000BF1C File Offset: 0x0000A11C
		// (set) Token: 0x060002EC RID: 748 RVA: 0x0000BF24 File Offset: 0x0000A124
		public PropertyInfo PropertyInfo { get; private set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060002ED RID: 749 RVA: 0x0000BF2D File Offset: 0x0000A12D
		// (set) Token: 0x060002EE RID: 750 RVA: 0x0000BF35 File Offset: 0x0000A135
		public SaveablePropertyAttribute SaveablePropertyAttribute { get; private set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060002EF RID: 751 RVA: 0x0000BF3E File Offset: 0x0000A13E
		// (set) Token: 0x060002F0 RID: 752 RVA: 0x0000BF46 File Offset: 0x0000A146
		public MethodInfo GetMethod { get; private set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060002F1 RID: 753 RVA: 0x0000BF4F File Offset: 0x0000A14F
		// (set) Token: 0x060002F2 RID: 754 RVA: 0x0000BF57 File Offset: 0x0000A157
		public MethodInfo SetMethod { get; private set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060002F3 RID: 755 RVA: 0x0000BF60 File Offset: 0x0000A160
		// (set) Token: 0x060002F4 RID: 756 RVA: 0x0000BF68 File Offset: 0x0000A168
		public GetPropertyValueDelegate GetPropertyValueMethod { get; private set; }

		// Token: 0x060002F5 RID: 757 RVA: 0x0000BF74 File Offset: 0x0000A174
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

		// Token: 0x060002F6 RID: 758 RVA: 0x0000C170 File Offset: 0x0000A370
		public override Type GetMemberType()
		{
			return this.PropertyInfo.PropertyType;
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0000C180 File Offset: 0x0000A380
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

		// Token: 0x060002F8 RID: 760 RVA: 0x0000C1B8 File Offset: 0x0000A3B8
		public void InitializeForAutoGeneration(GetPropertyValueDelegate getPropertyValueMethod)
		{
			this.GetPropertyValueMethod = getPropertyValueMethod;
		}
	}
}
