using System;
using TaleWorlds.Diamond;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public struct PlayerSessionId
	{
		public Guid Guid
		{
			get
			{
				return this._guid;
			}
		}

		public SessionKey SessionKey
		{
			get
			{
				return new SessionKey(this._guid);
			}
		}

		public PlayerSessionId(Guid guid)
		{
			this._guid = guid;
		}

		public PlayerSessionId(SessionKey sessionKey)
		{
			this._guid = sessionKey.Guid;
		}

		public PlayerSessionId(byte[] b)
		{
			this._guid = new Guid(b);
		}

		public PlayerSessionId(string g)
		{
			this._guid = new Guid(g);
		}

		public static PlayerSessionId NewGuid()
		{
			return new PlayerSessionId(Guid.NewGuid());
		}

		public override string ToString()
		{
			return this._guid.ToString();
		}

		public byte[] ToByteArray()
		{
			return this._guid.ToByteArray();
		}

		public static bool operator ==(PlayerSessionId a, PlayerSessionId b)
		{
			return a._guid == b._guid;
		}

		public static bool operator !=(PlayerSessionId a, PlayerSessionId b)
		{
			return a._guid != b._guid;
		}

		public override bool Equals(object o)
		{
			return o != null && o is PlayerSessionId && this._guid.Equals(((PlayerSessionId)o).Guid);
		}

		public override int GetHashCode()
		{
			return this._guid.GetHashCode();
		}

		private Guid _guid;
	}
}
