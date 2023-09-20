using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000015 RID: 21
	public struct AreaInformation
	{
		// Token: 0x0600003F RID: 63 RVA: 0x000029CD File Offset: 0x00000BCD
		public void DeserializeFrom(IReader reader)
		{
			this.Temperature = reader.ReadFloat();
			this.Humidity = reader.ReadFloat();
			this.AreaType = reader.ReadInt();
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000029F3 File Offset: 0x00000BF3
		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.Temperature);
			writer.WriteFloat(this.Humidity);
			writer.WriteInt(this.AreaType);
		}

		// Token: 0x04000040 RID: 64
		public float Temperature;

		// Token: 0x04000041 RID: 65
		public float Humidity;

		// Token: 0x04000042 RID: 66
		public int AreaType;
	}
}
