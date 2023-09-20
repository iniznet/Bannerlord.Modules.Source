using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SandBox.BoardGames.MissionLogics;
using SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	public static class SandBoxCheats
	{
		[CommandLineFunctionality.CommandLineArgumentFunction("spawn_new_alley_attack", "campaign")]
		public static string SpawnNewAlleyAttack(List<string> strings)
		{
			AlleyCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<AlleyCampaignBehavior>();
			if (campaignBehavior == null)
			{
				return "Alley Campaign Behavior not found";
			}
			foreach (AlleyCampaignBehavior.PlayerAlleyData playerAlleyData in ((List<AlleyCampaignBehavior.PlayerAlleyData>)typeof(AlleyCampaignBehavior).GetField("_playerOwnedCommonAreaData", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(campaignBehavior)))
			{
				if (!playerAlleyData.IsUnderAttack)
				{
					if (playerAlleyData.Alley.Settlement.Alleys.Any((Alley x) => x.State == 1))
					{
						typeof(AlleyCampaignBehavior).GetMethod("StartNewAlleyAttack", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(campaignBehavior, new object[] { playerAlleyData });
						return "Success";
					}
				}
			}
			return "There is no suitable alley for spawning an alley attack.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("win_board_game", "campaign")]
		public static string WinCurrentGame(List<string> strings)
		{
			Mission mission = Mission.Current;
			MissionBoardGameLogic missionBoardGameLogic = ((mission != null) ? mission.GetMissionBehavior<MissionBoardGameLogic>() : null);
			if (missionBoardGameLogic == null)
			{
				return "There is no board game.";
			}
			missionBoardGameLogic.PlayerOneWon("str_boardgame_victory_message");
			return "Success";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("refresh_battle_scene_index_map", "campaign")]
		public static string RefreshBattleSceneIndexMap(List<string> strings)
		{
			MapScene mapScene = Campaign.Current.MapSceneWrapper as MapScene;
			Type typeFromHandle = typeof(MapScene);
			FieldInfo field = typeFromHandle.GetField("_scene", BindingFlags.Instance | BindingFlags.NonPublic);
			FieldInfo field2 = typeFromHandle.GetField("_battleTerrainIndexMap", BindingFlags.Instance | BindingFlags.NonPublic);
			FieldInfo field3 = typeFromHandle.GetField("_battleTerrainIndexMapWidth", BindingFlags.Instance | BindingFlags.NonPublic);
			FieldInfo field4 = typeFromHandle.GetField("_battleTerrainIndexMapHeight", BindingFlags.Instance | BindingFlags.NonPublic);
			byte[] array = null;
			int num = 0;
			int num2 = 0;
			Scene scene = (Scene)field.GetValue(mapScene);
			MBMapScene.GetBattleSceneIndexMap(scene, ref array, ref num, ref num2);
			field.SetValue(mapScene, scene);
			field2.SetValue(mapScene, array);
			field3.SetValue(mapScene, num);
			field4.SetValue(mapScene, num2);
			return "Success";
		}
	}
}
