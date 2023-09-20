using System;

namespace TaleWorlds.Library
{
	public struct AmbientInformation
	{
		public void DeserializeFrom(IReader reader)
		{
			this.EnvironmentMultiplier = reader.ReadFloat();
			this.AmbientColor = reader.ReadVec3();
			this.MieScatterStrength = reader.ReadFloat();
			this.RayleighConstant = reader.ReadFloat();
		}

		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.EnvironmentMultiplier);
			writer.WriteVec3(this.AmbientColor);
			writer.WriteFloat(this.MieScatterStrength);
			writer.WriteFloat(this.RayleighConstant);
		}

		public float EnvironmentMultiplier;

		public Vec3 AmbientColor;

		public float MieScatterStrength;

		public float RayleighConstant;
	}
}
