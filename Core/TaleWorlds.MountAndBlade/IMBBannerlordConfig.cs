﻿using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[ScriptingInterfaceBase]
	internal interface IMBBannerlordConfig
	{
		[EngineMethod("validate_options", false)]
		void ValidateOptions();
	}
}
