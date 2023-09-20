using System;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class CampaignSiegeStateHandler : MissionLogic
	{
		public bool IsSiege
		{
			get
			{
				return this._mapEvent.IsSiegeAssault;
			}
		}

		public bool IsSallyOut
		{
			get
			{
				return this._mapEvent.IsSallyOut;
			}
		}

		public Settlement Settlement
		{
			get
			{
				return this._mapEvent.MapEventSettlement;
			}
		}

		public CampaignSiegeStateHandler()
		{
			this._mapEvent = PlayerEncounter.Battle;
		}

		public override void OnRetreatMission()
		{
			this._isRetreat = true;
		}

		public override void OnMissionResultReady(MissionResult missionResult)
		{
			this._defenderVictory = missionResult.BattleState == 1;
		}

		public override void OnSurrenderMission()
		{
			PlayerEncounter.PlayerSurrender = true;
		}

		protected override void OnEndMission()
		{
			if (this.IsSiege && this._mapEvent.PlayerSide == 1 && !this._isRetreat && !this._defenderVictory)
			{
				this.Settlement.SetNextSiegeState();
			}
		}

		private readonly MapEvent _mapEvent;

		private bool _isRetreat;

		private bool _defenderVictory;
	}
}
