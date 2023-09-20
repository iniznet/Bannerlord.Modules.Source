using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200000F RID: 15
	public struct RainInformation
	{
		// Token: 0x06000033 RID: 51 RVA: 0x0000284D File Offset: 0x00000A4D
		public void DeserializeFrom(IReader reader)
		{
			this.Density = reader.ReadFloat();
		}

		// Token: 0x06000034 RID: 52 RVA: 0x0000285B File Offset: 0x00000A5B
		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.Density);
		}

		// Token: 0x04000031 RID: 49
		public float Density;
	}
}
