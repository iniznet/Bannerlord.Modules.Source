using System;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000063 RID: 99
	public struct MemberTypeId
	{
		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060002E7 RID: 743 RVA: 0x0000BEA1 File Offset: 0x0000A0A1
		public short SaveId
		{
			get
			{
				return (short)(this.TypeLevel << 8) + this.LocalSaveId;
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060002E8 RID: 744 RVA: 0x0000BEB4 File Offset: 0x0000A0B4
		public static MemberTypeId Invalid
		{
			get
			{
				return new MemberTypeId(0, -1);
			}
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x0000BEC0 File Offset: 0x0000A0C0
		public override string ToString()
		{
			return string.Concat(new object[] { "(", this.TypeLevel, ",", this.LocalSaveId, ")" });
		}

		// Token: 0x060002EA RID: 746 RVA: 0x0000BF0C File Offset: 0x0000A10C
		public MemberTypeId(byte typeLevel, short localSaveId)
		{
			this.TypeLevel = typeLevel;
			this.LocalSaveId = localSaveId;
		}

		// Token: 0x040000E8 RID: 232
		public byte TypeLevel;

		// Token: 0x040000E9 RID: 233
		public short LocalSaveId;
	}
}
