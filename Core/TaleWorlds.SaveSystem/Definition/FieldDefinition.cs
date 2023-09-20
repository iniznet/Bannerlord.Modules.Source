using System;
using System.Reflection;

namespace TaleWorlds.SaveSystem.Definition
{
	public class FieldDefinition : MemberDefinition
	{
		public FieldInfo FieldInfo { get; private set; }

		public SaveableFieldAttribute SaveableFieldAttribute { get; private set; }

		public GetFieldValueDelegate GetFieldValueMethod { get; private set; }

		public FieldDefinition(FieldInfo fieldInfo, MemberTypeId id)
			: base(fieldInfo, id)
		{
			this.FieldInfo = fieldInfo;
			this.SaveableFieldAttribute = fieldInfo.GetCustomAttribute<SaveableFieldAttribute>();
		}

		public override Type GetMemberType()
		{
			return this.FieldInfo.FieldType;
		}

		public override object GetValue(object target)
		{
			object obj;
			if (this.GetFieldValueMethod != null)
			{
				obj = this.GetFieldValueMethod(target);
			}
			else
			{
				obj = this.FieldInfo.GetValue(target);
			}
			return obj;
		}

		public void InitializeForAutoGeneration(GetFieldValueDelegate getFieldValueMethod)
		{
			this.GetFieldValueMethod = getFieldValueMethod;
		}
	}
}
