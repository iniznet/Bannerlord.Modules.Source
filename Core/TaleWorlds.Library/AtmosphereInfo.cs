using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.Library
{
	// Token: 0x02000017 RID: 23
	[StructLayout(LayoutKind.Sequential)]
	public class AtmosphereInfo
	{
		// Token: 0x06000043 RID: 67 RVA: 0x00002A7D File Offset: 0x00000C7D
		public AtmosphereInfo()
		{
			this.AtmosphereName = "";
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002A90 File Offset: 0x00000C90
		public void DeserializeFrom(IReader reader)
		{
			this.SunInfo.DeserializeFrom(reader);
			this.RainInfo.DeserializeFrom(reader);
			this.SnowInfo.DeserializeFrom(reader);
			this.AmbientInfo.DeserializeFrom(reader);
			this.FogInfo.DeserializeFrom(reader);
			this.SkyInfo.DeserializeFrom(reader);
			this.TimeInfo.DeserializeFrom(reader);
			this.AreaInfo.DeserializeFrom(reader);
			this.PostProInfo.DeserializeFrom(reader);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002B0C File Offset: 0x00000D0C
		public void SerializeTo(IWriter writer)
		{
			this.SunInfo.SerializeTo(writer);
			this.RainInfo.SerializeTo(writer);
			this.SnowInfo.SerializeTo(writer);
			this.AmbientInfo.SerializeTo(writer);
			this.FogInfo.SerializeTo(writer);
			this.SkyInfo.SerializeTo(writer);
			this.TimeInfo.SerializeTo(writer);
			this.AreaInfo.SerializeTo(writer);
			this.PostProInfo.SerializeTo(writer);
		}

		// Token: 0x04000047 RID: 71
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string AtmosphereName;

		// Token: 0x04000048 RID: 72
		public SunInformation SunInfo;

		// Token: 0x04000049 RID: 73
		public RainInformation RainInfo;

		// Token: 0x0400004A RID: 74
		public SnowInformation SnowInfo;

		// Token: 0x0400004B RID: 75
		public AmbientInformation AmbientInfo;

		// Token: 0x0400004C RID: 76
		public FogInformation FogInfo;

		// Token: 0x0400004D RID: 77
		public SkyInformation SkyInfo;

		// Token: 0x0400004E RID: 78
		public TimeInformation TimeInfo;

		// Token: 0x0400004F RID: 79
		public AreaInformation AreaInfo;

		// Token: 0x04000050 RID: 80
		public PostProcessInformation PostProInfo;

		// Token: 0x04000051 RID: 81
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string AtmosphereTypeName;
	}
}
