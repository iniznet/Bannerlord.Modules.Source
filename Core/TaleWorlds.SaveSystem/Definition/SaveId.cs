using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000067 RID: 103
	public abstract class SaveId
	{
		// Token: 0x0600031B RID: 795
		public abstract string GetStringId();

		// Token: 0x0600031C RID: 796 RVA: 0x0000DF80 File Offset: 0x0000C180
		public override int GetHashCode()
		{
			return this.GetStringId().GetHashCode();
		}

		// Token: 0x0600031D RID: 797 RVA: 0x0000DF8D File Offset: 0x0000C18D
		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && this.GetStringId() == ((SaveId)obj).GetStringId();
		}

		// Token: 0x0600031E RID: 798
		public abstract void WriteTo(IWriter writer);

		// Token: 0x0600031F RID: 799 RVA: 0x0000DFC0 File Offset: 0x0000C1C0
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
