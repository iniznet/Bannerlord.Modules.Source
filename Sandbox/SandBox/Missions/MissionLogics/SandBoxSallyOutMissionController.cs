using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class SandBoxSallyOutMissionController : SallyOutMissionController
	{
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._mapEvent = MapEvent.PlayerMapEvent;
		}

		protected override void GetInitialTroopCounts(out int besiegedTotalTroopCount, out int besiegerTotalTroopCount)
		{
			besiegedTotalTroopCount = this._mapEvent.GetNumberOfInvolvedMen(0);
			besiegerTotalTroopCount = this._mapEvent.GetNumberOfInvolvedMen(1);
		}

		private MapEvent _mapEvent;
	}
}
