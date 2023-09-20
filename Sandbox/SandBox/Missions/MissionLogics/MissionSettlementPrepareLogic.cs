using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class MissionSettlementPrepareLogic : MissionLogic
	{
		public override void AfterStart()
		{
			if (Campaign.Current.GameMode == 1 && Settlement.CurrentSettlement != null && (Settlement.CurrentSettlement.IsTown || Settlement.CurrentSettlement.IsCastle))
			{
				this.OpenGates();
			}
		}

		private void OpenGates()
		{
			foreach (CastleGate castleGate in MBExtensions.FindAllWithType<CastleGate>(Mission.Current.ActiveMissionObjects).ToList<CastleGate>())
			{
				castleGate.OpenDoorAndDisableGateForCivilianMission();
			}
		}
	}
}
