using System;
using TaleWorlds.Diamond;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200013A RID: 314
	[Serializable]
	public struct PlayerSessionId
	{
		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06000775 RID: 1909 RVA: 0x0000C232 File Offset: 0x0000A432
		public Guid Guid
		{
			get
			{
				return this._guid;
			}
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06000776 RID: 1910 RVA: 0x0000C23A File Offset: 0x0000A43A
		public SessionKey SessionKey
		{
			get
			{
				return new SessionKey(this._guid);
			}
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x0000C247 File Offset: 0x0000A447
		public PlayerSessionId(Guid guid)
		{
			this._guid = guid;
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x0000C250 File Offset: 0x0000A450
		public PlayerSessionId(SessionKey sessionKey)
		{
			this._guid = sessionKey.Guid;
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x0000C25F File Offset: 0x0000A45F
		public PlayerSessionId(byte[] b)
		{
			this._guid = new Guid(b);
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x0000C26D File Offset: 0x0000A46D
		public PlayerSessionId(string g)
		{
			this._guid = new Guid(g);
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x0000C27B File Offset: 0x0000A47B
		public static PlayerSessionId NewGuid()
		{
			return new PlayerSessionId(Guid.NewGuid());
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x0000C287 File Offset: 0x0000A487
		public override string ToString()
		{
			return this._guid.ToString();
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x0000C29A File Offset: 0x0000A49A
		public byte[] ToByteArray()
		{
			return this._guid.ToByteArray();
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x0000C2A7 File Offset: 0x0000A4A7
		public static bool operator ==(PlayerSessionId a, PlayerSessionId b)
		{
			return a._guid == b._guid;
		}

		// Token: 0x0600077F RID: 1919 RVA: 0x0000C2BA File Offset: 0x0000A4BA
		public static bool operator !=(PlayerSessionId a, PlayerSessionId b)
		{
			return a._guid != b._guid;
		}

		// Token: 0x06000780 RID: 1920 RVA: 0x0000C2D0 File Offset: 0x0000A4D0
		public override bool Equals(object o)
		{
			return o != null && o is PlayerSessionId && this._guid.Equals(((PlayerSessionId)o).Guid);
		}

		// Token: 0x06000781 RID: 1921 RVA: 0x0000C303 File Offset: 0x0000A503
		public override int GetHashCode()
		{
			return this._guid.GetHashCode();
		}

		// Token: 0x04000373 RID: 883
		private Guid _guid;
	}
}
