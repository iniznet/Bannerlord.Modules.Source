using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000107 RID: 263
	[Serializable]
	public struct CustomBattleId
	{
		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x060004C0 RID: 1216 RVA: 0x00006FEB File Offset: 0x000051EB
		public Guid Guid
		{
			get
			{
				return this._guid;
			}
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x00006FF3 File Offset: 0x000051F3
		public CustomBattleId(Guid guid)
		{
			this._guid = guid;
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x00006FFC File Offset: 0x000051FC
		public CustomBattleId(byte[] b)
		{
			this._guid = new Guid(b);
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x0000700A File Offset: 0x0000520A
		public CustomBattleId(string g)
		{
			this._guid = new Guid(g);
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x00007018 File Offset: 0x00005218
		public static CustomBattleId NewGuid()
		{
			return new CustomBattleId(Guid.NewGuid());
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x00007024 File Offset: 0x00005224
		public override string ToString()
		{
			return this._guid.ToString();
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x00007037 File Offset: 0x00005237
		public byte[] ToByteArray()
		{
			return this._guid.ToByteArray();
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x00007044 File Offset: 0x00005244
		public static bool operator ==(CustomBattleId a, CustomBattleId b)
		{
			return a._guid == b._guid;
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x00007057 File Offset: 0x00005257
		public static bool operator !=(CustomBattleId a, CustomBattleId b)
		{
			return a._guid != b._guid;
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x0000706C File Offset: 0x0000526C
		public override bool Equals(object o)
		{
			return o != null && o is CustomBattleId && this._guid.Equals(((CustomBattleId)o).Guid);
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x0000709F File Offset: 0x0000529F
		public override int GetHashCode()
		{
			return this._guid.GetHashCode();
		}

		// Token: 0x0400021E RID: 542
		private Guid _guid;
	}
}
