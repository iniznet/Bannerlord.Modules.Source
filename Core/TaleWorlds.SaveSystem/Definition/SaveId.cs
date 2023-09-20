using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	public abstract class SaveId
	{
		public abstract string GetStringId();

		public override int GetHashCode()
		{
			return this.GetStringId().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && this.GetStringId() == ((SaveId)obj).GetStringId();
		}

		public abstract void WriteTo(IWriter writer);

		public static SaveId ReadSaveIdFrom(IReader reader)
		{
			byte b = reader.ReadByte();
			SaveId saveId = null;
			if (b == 0)
			{
				saveId = TypeSaveId.ReadFrom(reader);
			}
			else if (b == 1)
			{
				saveId = GenericSaveId.ReadFrom(reader);
			}
			else if (b == 2)
			{
				saveId = ContainerSaveId.ReadFrom(reader);
			}
			return saveId;
		}
	}
}
