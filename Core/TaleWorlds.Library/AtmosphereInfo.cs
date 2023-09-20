using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.Library
{
	public struct AtmosphereInfo
	{
		public bool IsValid
		{
			get
			{
				return !string.IsNullOrEmpty(this.AtmosphereName);
			}
		}

		public static AtmosphereInfo GetInvalidAtmosphereInfo()
		{
			return new AtmosphereInfo
			{
				AtmosphereName = ""
			};
		}

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

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string AtmosphereName;

		public SunInformation SunInfo;

		public RainInformation RainInfo;

		public SnowInformation SnowInfo;

		public AmbientInformation AmbientInfo;

		public FogInformation FogInfo;

		public SkyInformation SkyInfo;

		public TimeInformation TimeInfo;

		public AreaInformation AreaInfo;

		public PostProcessInformation PostProInfo;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string InterpolatedAtmosphereName;
	}
}
