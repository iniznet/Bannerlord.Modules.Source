using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public class MissionMultiplayerFFA : MissionMultiplayerGameModeBase
	{
		public override bool IsGameModeHidingAllAgentVisuals
		{
			get
			{
				return true;
			}
		}

		public override bool IsGameModeUsingOpposingTeams
		{
			get
			{
				return false;
			}
		}

		public override MissionLobbyComponent.MultiplayerGameType GetMissionType()
		{
			return MissionLobbyComponent.MultiplayerGameType.FreeForAll;
		}

		public override void AfterStart()
		{
			BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			Banner banner = new Banner(@object.BannerKey, @object.BackgroundColor1, @object.ForegroundColor1);
			Team team = base.Mission.Teams.Add(BattleSideEnum.Attacker, @object.BackgroundColor1, @object.ForegroundColor1, banner, false, false, true);
			team.SetIsEnemyOf(team, true);
		}

		protected override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			networkPeer.AddComponent<FFAMissionRepresentative>();
		}

		protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			component.Team = base.Mission.AttackerTeam;
			component.Culture = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
		}
	}
}
