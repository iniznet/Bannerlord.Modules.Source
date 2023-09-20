using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x0200004B RID: 75
	public class MissionSettlementPrepareLogic : MissionLogic
	{
		// Token: 0x060003A3 RID: 931 RVA: 0x0001AE24 File Offset: 0x00019024
		public override void AfterStart()
		{
			if (Campaign.Current.GameMode == 1 && Settlement.CurrentSettlement != null && (Settlement.CurrentSettlement.IsTown || Settlement.CurrentSettlement.IsCastle))
			{
				this.OpenGates();
			}
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x0001AE58 File Offset: 0x00019058
		private void OpenGates()
		{
			foreach (CastleGate castleGate in MBExtensions.FindAllWithType<CastleGate>(Mission.Current.ActiveMissionObjects).ToList<CastleGate>())
			{
				castleGate.OpenDoorAndDisableGateForCivilianMission();
			}
		}
	}
}
