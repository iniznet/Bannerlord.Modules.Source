using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200029F RID: 671
	public class MissionMultiplayerFFA : MissionMultiplayerGameModeBase
	{
		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x060024B8 RID: 9400 RVA: 0x0008834E File Offset: 0x0008654E
		public override bool IsGameModeHidingAllAgentVisuals
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006D4 RID: 1748
		// (get) Token: 0x060024B9 RID: 9401 RVA: 0x00088351 File Offset: 0x00086551
		public override bool IsGameModeUsingOpposingTeams
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060024BA RID: 9402 RVA: 0x00088354 File Offset: 0x00086554
		public override MissionLobbyComponent.MultiplayerGameType GetMissionType()
		{
			return MissionLobbyComponent.MultiplayerGameType.FreeForAll;
		}

		// Token: 0x060024BB RID: 9403 RVA: 0x00088358 File Offset: 0x00086558
		public override void AfterStart()
		{
			BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			Banner banner = new Banner(@object.BannerKey, @object.BackgroundColor1, @object.ForegroundColor1);
			Team team = base.Mission.Teams.Add(BattleSideEnum.Attacker, @object.BackgroundColor1, @object.ForegroundColor1, banner, false, false, true);
			team.SetIsEnemyOf(team, true);
		}

		// Token: 0x060024BC RID: 9404 RVA: 0x000883B8 File Offset: 0x000865B8
		protected override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			networkPeer.AddComponent<FFAMissionRepresentative>();
		}

		// Token: 0x060024BD RID: 9405 RVA: 0x000883C1 File Offset: 0x000865C1
		protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			component.Team = base.Mission.AttackerTeam;
			component.Culture = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
		}
	}
}
