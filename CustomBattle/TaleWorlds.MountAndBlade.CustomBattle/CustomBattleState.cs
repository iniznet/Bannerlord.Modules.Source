using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	// Token: 0x02000005 RID: 5
	public class CustomBattleState : GameState
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000016 RID: 22 RVA: 0x0000485C File Offset: 0x00002A5C
		public override bool IsMusicMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00004867 File Offset: 0x00002A67
		protected override void OnInitialize()
		{
			base.OnInitialize();
			CustomBattleHelper.AssertMissingTroopsForDebug();
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00004874 File Offset: 0x00002A74
		[CommandLineFunctionality.CommandLineArgumentFunction("enable_custom_record", "replay_mission")]
		public static string EnableRecordMission(List<string> strings)
		{
			if (!(GameStateManager.Current.ActiveState is CustomBattleState))
			{
				return "Mission recording for custom battle can only be enabled while in custom battle screen.";
			}
			MissionState.RecordMission = true;
			return "Mission recording activated.";
		}
	}
}
