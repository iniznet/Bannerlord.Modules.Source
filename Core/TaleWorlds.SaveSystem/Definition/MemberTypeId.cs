using System;

namespace TaleWorlds.SaveSystem.Definition
{
	public struct MemberTypeId
	{
		public short SaveId
		{
			get
			{
				return (short)(this.TypeLevel << 8) + this.LocalSaveId;
			}
		}

		public static MemberTypeId Invalid
		{
			get
			{
				return new MemberTypeId(0, -1);
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[] { "(", this.TypeLevel, ",", this.LocalSaveId, ")" });
		}

		public MemberTypeId(byte typeLevel, short localSaveId)
		{
			this.TypeLevel = typeLevel;
			this.LocalSaveId = localSaveId;
		}

		public byte TypeLevel;

		public short LocalSaveId;
	}
}
