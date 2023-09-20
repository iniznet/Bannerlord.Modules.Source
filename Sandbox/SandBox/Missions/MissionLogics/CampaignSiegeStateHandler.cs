using System;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000038 RID: 56
	public class CampaignSiegeStateHandler : MissionLogic
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060002B0 RID: 688 RVA: 0x00011D0B File Offset: 0x0000FF0B
		public bool IsSiege
		{
			get
			{
				return this._mapEvent.IsSiegeAssault;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060002B1 RID: 689 RVA: 0x00011D18 File Offset: 0x0000FF18
		public bool IsSallyOut
		{
			get
			{
				return this._mapEvent.IsSallyOut;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x00011D25 File Offset: 0x0000FF25
		public Settlement Settlement
		{
			get
			{
				return this._mapEvent.MapEventSettlement;
			}
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00011D32 File Offset: 0x0000FF32
		public CampaignSiegeStateHandler()
		{
			this._mapEvent = PlayerEncounter.Battle;
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x00011D45 File Offset: 0x0000FF45
		public override void OnRetreatMission()
		{
			this._isRetreat = true;
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00011D4E File Offset: 0x0000FF4E
		public override void OnMissionResultReady(MissionResult missionResult)
		{
			this._defenderVictory = missionResult.BattleState == 1;
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00011D5F File Offset: 0x0000FF5F
		public override void OnSurrenderMission()
		{
			PlayerEncounter.PlayerSurrender = true;
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00011D67 File Offset: 0x0000FF67
		protected override void OnEndMission()
		{
			if (this.IsSiege && this._mapEvent.PlayerSide == 1 && !this._isRetreat && !this._defenderVictory)
			{
				this.Settlement.SetNextSiegeState();
			}
		}

		// Token: 0x0400014F RID: 335
		private readonly MapEvent _mapEvent;

		// Token: 0x04000150 RID: 336
		private bool _isRetreat;

		// Token: 0x04000151 RID: 337
		private bool _defenderVictory;
	}
}
