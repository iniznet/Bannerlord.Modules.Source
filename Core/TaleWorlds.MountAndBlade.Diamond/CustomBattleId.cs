using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public struct CustomBattleId
	{
		public Guid Guid
		{
			get
			{
				return this._guid;
			}
		}

		public CustomBattleId(Guid guid)
		{
			this._guid = guid;
		}

		public CustomBattleId(byte[] b)
		{
			this._guid = new Guid(b);
		}

		public CustomBattleId(string g)
		{
			this._guid = new Guid(g);
		}

		public static CustomBattleId NewGuid()
		{
			return new CustomBattleId(Guid.NewGuid());
		}

		public override string ToString()
		{
			return this._guid.ToString();
		}

		public byte[] ToByteArray()
		{
			return this._guid.ToByteArray();
		}

		public static bool operator ==(CustomBattleId a, CustomBattleId b)
		{
			return a._guid == b._guid;
		}

		public static bool operator !=(CustomBattleId a, CustomBattleId b)
		{
			return a._guid != b._guid;
		}

		public override bool Equals(object o)
		{
			return o != null && o is CustomBattleId && this._guid.Equals(((CustomBattleId)o).Guid);
		}

		public override int GetHashCode()
		{
			return this._guid.GetHashCode();
		}

		private Guid _guid;
	}
}
