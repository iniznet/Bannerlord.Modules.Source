using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200019E RID: 414
	[ScriptingInterfaceBase]
	internal interface IMBTeam
	{
		// Token: 0x060016E7 RID: 5863
		[EngineMethod("is_enemy", false)]
		bool IsEnemy(UIntPtr missionPointer, int teamIndex, int otherTeamIndex);

		// Token: 0x060016E8 RID: 5864
		[EngineMethod("set_is_enemy", false)]
		void SetIsEnemy(UIntPtr missionPointer, int teamIndex, int otherTeamIndex, bool isEnemy);
	}
}
