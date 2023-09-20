using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000011 RID: 17
	public struct AmbientInformation
	{
		// Token: 0x06000037 RID: 55 RVA: 0x00002885 File Offset: 0x00000A85
		public void DeserializeFrom(IReader reader)
		{
			this.EnvironmentMultiplier = reader.ReadFloat();
			this.AmbientColor = reader.ReadVec3();
			this.MieScatterStrength = reader.ReadFloat();
			this.RayleighConstant = reader.ReadFloat();
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000028B7 File Offset: 0x00000AB7
		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.EnvironmentMultiplier);
			writer.WriteVec3(this.AmbientColor);
			writer.WriteFloat(this.MieScatterStrength);
			writer.WriteFloat(this.RayleighConstant);
		}

		// Token: 0x04000033 RID: 51
		public float EnvironmentMultiplier;

		// Token: 0x04000034 RID: 52
		public Vec3 AmbientColor;

		// Token: 0x04000035 RID: 53
		public float MieScatterStrength;

		// Token: 0x04000036 RID: 54
		public float RayleighConstant;
	}
}
