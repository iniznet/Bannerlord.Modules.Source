using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond
{
	[DataContract]
	[Serializable]
	public struct SessionKey
	{
		public Guid Guid
		{
			get
			{
				return this._guid;
			}
		}

		public SessionKey(Guid guid)
		{
			this._guid = guid;
		}

		public SessionKey(byte[] b)
		{
			this._guid = new Guid(b);
		}

		public SessionKey(string g)
		{
			this._guid = new Guid(g);
		}

		public static SessionKey NewGuid()
		{
			return new SessionKey(Guid.NewGuid());
		}

		public override string ToString()
		{
			return this._guid.ToString();
		}

		public byte[] ToByteArray()
		{
			return this._guid.ToByteArray();
		}

		public static bool operator ==(SessionKey a, SessionKey b)
		{
			return a._guid == b._guid;
		}

		public static bool operator !=(SessionKey a, SessionKey b)
		{
			return a._guid != b._guid;
		}

		public override bool Equals(object o)
		{
			if (o != null && o is SessionKey)
			{
				SessionKey sessionKey = (SessionKey)o;
				return this._guid.Equals(sessionKey.Guid);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this._guid.GetHashCode();
		}

		public static SessionKey Empty
		{
			get
			{
				return new SessionKey(Guid.Empty);
			}
		}

		[DataMember]
		private readonly Guid _guid;
	}
}
