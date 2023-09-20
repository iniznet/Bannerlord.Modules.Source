using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000012 RID: 18
	public struct SkyInformation
	{
		// Token: 0x06000039 RID: 57 RVA: 0x000028E9 File Offset: 0x00000AE9
		public void DeserializeFrom(IReader reader)
		{
			this.Brightness = reader.ReadFloat();
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000028F7 File Offset: 0x00000AF7
		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.Brightness);
		}

		// Token: 0x04000037 RID: 55
		public float Brightness;
	}
}
