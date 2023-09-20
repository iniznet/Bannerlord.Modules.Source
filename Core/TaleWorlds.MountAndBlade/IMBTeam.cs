﻿using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[ScriptingInterfaceBase]
	internal interface IMBTeam
	{
		[EngineMethod("is_enemy", false)]
		bool IsEnemy(UIntPtr missionPointer, int teamIndex, int otherTeamIndex);

		[EngineMethod("set_is_enemy", false)]
		void SetIsEnemy(UIntPtr missionPointer, int teamIndex, int otherTeamIndex, bool isEnemy);
	}
}
