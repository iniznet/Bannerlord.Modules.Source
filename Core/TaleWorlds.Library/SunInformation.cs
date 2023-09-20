using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200000E RID: 14
	public struct SunInformation
	{
		// Token: 0x06000031 RID: 49 RVA: 0x00002788 File Offset: 0x00000988
		public void DeserializeFrom(IReader reader)
		{
			this.Altitude = reader.ReadFloat();
			this.Angle = reader.ReadFloat();
			this.Color = reader.ReadVec3();
			this.Brightness = reader.ReadFloat();
			this.MaxBrightness = reader.ReadFloat();
			this.Size = reader.ReadFloat();
			this.RayStrength = reader.ReadFloat();
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000027EC File Offset: 0x000009EC
		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.Altitude);
			writer.WriteFloat(this.Angle);
			writer.WriteVec3(this.Color);
			writer.WriteFloat(this.Brightness);
			writer.WriteFloat(this.MaxBrightness);
			writer.WriteFloat(this.Size);
			writer.WriteFloat(this.RayStrength);
		}

		// Token: 0x0400002A RID: 42
		public float Altitude;

		// Token: 0x0400002B RID: 43
		public float Angle;

		// Token: 0x0400002C RID: 44
		public Vec3 Color;

		// Token: 0x0400002D RID: 45
		public float Brightness;

		// Token: 0x0400002E RID: 46
		public float MaxBrightness;

		// Token: 0x0400002F RID: 47
		public float Size;

		// Token: 0x04000030 RID: 48
		public float RayStrength;
	}
}
