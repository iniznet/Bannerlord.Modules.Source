using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000053 RID: 83
	public class SandBoxSallyOutMissionController : SallyOutMissionController
	{
		// Token: 0x060003B8 RID: 952 RVA: 0x0001B51C File Offset: 0x0001971C
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._mapEvent = MapEvent.PlayerMapEvent;
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x0001B52F File Offset: 0x0001972F
		protected override void GetInitialTroopCounts(out int besiegedTotalTroopCount, out int besiegerTotalTroopCount)
		{
			besiegedTotalTroopCount = this._mapEvent.GetNumberOfInvolvedMen(0);
			besiegerTotalTroopCount = this._mapEvent.GetNumberOfInvolvedMen(1);
		}

		// Token: 0x040001C2 RID: 450
		private MapEvent _mapEvent;
	}
}
