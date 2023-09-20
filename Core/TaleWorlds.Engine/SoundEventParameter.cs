using System;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x02000088 RID: 136
	[EngineStruct("Managed_sound_event_parameter")]
	public struct SoundEventParameter
	{
		// Token: 0x06000A62 RID: 2658 RVA: 0x0000B62B File Offset: 0x0000982B
		public SoundEventParameter(string paramName, float value)
		{
			this.ParamName = paramName;
			this.Value = value;
		}

		// Token: 0x06000A63 RID: 2659 RVA: 0x0000B63B File Offset: 0x0000983B
		public void Update(string paramName, float value)
		{
			this.ParamName = paramName;
			this.Value = value;
		}

		// Token: 0x040001A7 RID: 423
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		internal string ParamName;

		// Token: 0x040001A8 RID: 424
		internal float Value;
	}
}
