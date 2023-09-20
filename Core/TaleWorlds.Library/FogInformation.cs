using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000013 RID: 19
	public struct FogInformation
	{
		// Token: 0x0600003B RID: 59 RVA: 0x00002905 File Offset: 0x00000B05
		public void DeserializeFrom(IReader reader)
		{
			this.Density = reader.ReadFloat();
			this.Color = reader.ReadVec3();
			this.Falloff = reader.ReadFloat();
		}

		// Token: 0x0600003C RID: 60 RVA: 0x0000292B File Offset: 0x00000B2B
		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.Density);
			writer.WriteVec3(this.Color);
			writer.WriteFloat(this.Falloff);
		}

		// Token: 0x04000038 RID: 56
		public float Density;

		// Token: 0x04000039 RID: 57
		public Vec3 Color;

		// Token: 0x0400003A RID: 58
		public float Falloff;
	}
}
