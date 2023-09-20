using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000016 RID: 22
	public struct PostProcessInformation
	{
		// Token: 0x06000041 RID: 65 RVA: 0x00002A19 File Offset: 0x00000C19
		public void DeserializeFrom(IReader reader)
		{
			this.MinExposure = reader.ReadFloat();
			this.MaxExposure = reader.ReadFloat();
			this.BrightpassThreshold = reader.ReadFloat();
			this.MiddleGray = reader.ReadFloat();
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002A4B File Offset: 0x00000C4B
		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.MinExposure);
			writer.WriteFloat(this.MaxExposure);
			writer.WriteFloat(this.BrightpassThreshold);
			writer.WriteFloat(this.MiddleGray);
		}

		// Token: 0x04000043 RID: 67
		public float MinExposure;

		// Token: 0x04000044 RID: 68
		public float MaxExposure;

		// Token: 0x04000045 RID: 69
		public float BrightpassThreshold;

		// Token: 0x04000046 RID: 70
		public float MiddleGray;
	}
}
