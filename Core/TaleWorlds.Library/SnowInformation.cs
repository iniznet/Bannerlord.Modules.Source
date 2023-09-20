using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000010 RID: 16
	public struct SnowInformation
	{
		// Token: 0x06000035 RID: 53 RVA: 0x00002869 File Offset: 0x00000A69
		public void DeserializeFrom(IReader reader)
		{
			this.Density = reader.ReadFloat();
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002877 File Offset: 0x00000A77
		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.Density);
		}

		// Token: 0x04000032 RID: 50
		public float Density;
	}
}
