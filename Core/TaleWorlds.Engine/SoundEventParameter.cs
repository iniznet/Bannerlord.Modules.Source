using System;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	[EngineStruct("Managed_sound_event_parameter", false)]
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

		[CustomEngineStructMemberData("str_id")]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		internal string ParamName;

		internal float Value;
	}
}
