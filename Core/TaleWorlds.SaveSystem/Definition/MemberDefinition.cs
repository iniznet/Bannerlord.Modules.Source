using System;
using System.Reflection;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000062 RID: 98
	public abstract class MemberDefinition
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x0000BE69 File Offset: 0x0000A069
		// (set) Token: 0x060002E1 RID: 737 RVA: 0x0000BE71 File Offset: 0x0000A071
		public MemberTypeId Id { get; private set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x0000BE7A File Offset: 0x0000A07A
		// (set) Token: 0x060002E3 RID: 739 RVA: 0x0000BE82 File Offset: 0x0000A082
		public MemberInfo MemberInfo { get; private set; }

		// Token: 0x060002E4 RID: 740 RVA: 0x0000BE8B File Offset: 0x0000A08B
		protected MemberDefinition(MemberInfo memberInfo, MemberTypeId id)
		{
			this.MemberInfo = memberInfo;
			this.Id = id;
		}

		// Token: 0x060002E5 RID: 741
		public abstract Type GetMemberType();

		// Token: 0x060002E6 RID: 742
		public abstract object GetValue(object target);
	}
}
