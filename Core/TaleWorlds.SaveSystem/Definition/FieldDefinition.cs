using System;
using System.Reflection;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x0200005E RID: 94
	public class FieldDefinition : MemberDefinition
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060002CA RID: 714 RVA: 0x0000BC11 File Offset: 0x00009E11
		// (set) Token: 0x060002CB RID: 715 RVA: 0x0000BC19 File Offset: 0x00009E19
		public FieldInfo FieldInfo { get; private set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060002CC RID: 716 RVA: 0x0000BC22 File Offset: 0x00009E22
		// (set) Token: 0x060002CD RID: 717 RVA: 0x0000BC2A File Offset: 0x00009E2A
		public SaveableFieldAttribute SaveableFieldAttribute { get; private set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060002CE RID: 718 RVA: 0x0000BC33 File Offset: 0x00009E33
		// (set) Token: 0x060002CF RID: 719 RVA: 0x0000BC3B File Offset: 0x00009E3B
		public GetFieldValueDelegate GetFieldValueMethod { get; private set; }

		// Token: 0x060002D0 RID: 720 RVA: 0x0000BC44 File Offset: 0x00009E44
		public FieldDefinition(FieldInfo fieldInfo, MemberTypeId id)
			: base(fieldInfo, id)
		{
			this.FieldInfo = fieldInfo;
			this.SaveableFieldAttribute = fieldInfo.GetCustomAttribute<SaveableFieldAttribute>();
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0000BC61 File Offset: 0x00009E61
		public override Type GetMemberType()
		{
			return this.FieldInfo.FieldType;
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0000BC70 File Offset: 0x00009E70
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

		// Token: 0x060002D3 RID: 723 RVA: 0x0000BCA2 File Offset: 0x00009EA2
		public void InitializeForAutoGeneration(GetFieldValueDelegate getFieldValueMethod)
		{
			this.GetFieldValueMethod = getFieldValueMethod;
		}
	}
}
