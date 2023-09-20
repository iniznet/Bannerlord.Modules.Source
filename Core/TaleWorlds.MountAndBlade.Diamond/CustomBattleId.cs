using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public struct CustomBattleId
	{
		public Guid Guid { get; private set; }

		public CustomBattleId(Guid guid)
		{
			this.Guid = guid;
		}

		public static CustomBattleId NewGuid()
		{
			return new CustomBattleId(Guid.NewGuid());
		}

		public override string ToString()
		{
			return this.Guid.ToString();
		}

		public byte[] ToByteArray()
		{
			return this.Guid.ToByteArray();
		}

		public static bool operator ==(CustomBattleId a, CustomBattleId b)
		{
			return a.Guid == b.Guid;
		}

		public static bool operator !=(CustomBattleId a, CustomBattleId b)
		{
			return a.Guid != b.Guid;
		}

		public override bool Equals(object o)
		{
			if (o != null && o is CustomBattleId)
			{
				CustomBattleId customBattleId = (CustomBattleId)o;
				return this.Guid.Equals(customBattleId.Guid);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.Guid.GetHashCode();
		}
	}
}
