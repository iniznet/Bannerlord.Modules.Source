using System;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	[EngineStruct("Managed_sound_event_parameter")]
	public struct SoundEventParameter
	{
		public SoundEventParameter(string paramName, float value)
		{
			this.ParamName = paramName;
			this.Value = value;
		}

		public void Update(string paramName, float value)
		{
			this.ParamName = paramName;
			this.Value = value;
		}

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		internal string ParamName;

		internal float Value;
	}
}
