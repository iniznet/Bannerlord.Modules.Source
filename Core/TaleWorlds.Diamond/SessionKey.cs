using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond
{
	// Token: 0x0200001D RID: 29
	[DataContract]
	[Serializable]
	public struct SessionKey
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00002B7B File Offset: 0x00000D7B
		public Guid Guid
		{
			get
			{
				return this._guid;
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00002B83 File Offset: 0x00000D83
		public SessionKey(Guid guid)
		{
			this._guid = guid;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00002B8C File Offset: 0x00000D8C
		public SessionKey(byte[] b)
		{
			this._guid = new Guid(b);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00002B9A File Offset: 0x00000D9A
		public SessionKey(string g)
		{
			this._guid = new Guid(g);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00002BA8 File Offset: 0x00000DA8
		public static SessionKey NewGuid()
		{
			return new SessionKey(Guid.NewGuid());
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00002BB4 File Offset: 0x00000DB4
		public override string ToString()
		{
			return this._guid.ToString();
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00002BD8 File Offset: 0x00000DD8
		public byte[] ToByteArray()
		{
			return this._guid.ToByteArray();
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00002BF3 File Offset: 0x00000DF3
		public static bool operator ==(SessionKey a, SessionKey b)
		{
			return a._guid == b._guid;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00002C06 File Offset: 0x00000E06
		public static bool operator !=(SessionKey a, SessionKey b)
		{
			return a._guid != b._guid;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00002C1C File Offset: 0x00000E1C
		public override bool Equals(object o)
		{
			if (o != null && o is SessionKey)
			{
				SessionKey sessionKey = (SessionKey)o;
				return this._guid.Equals(sessionKey.Guid);
			}
			return false;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00002C54 File Offset: 0x00000E54
		public override int GetHashCode()
		{
			return this._guid.GetHashCode();
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00002C75 File Offset: 0x00000E75
		public static SessionKey Empty
		{
			get
			{
				return new SessionKey(Guid.Empty);
			}
		}

		// Token: 0x04000023 RID: 35
		[DataMember]
		private readonly Guid _guid;
	}
}
