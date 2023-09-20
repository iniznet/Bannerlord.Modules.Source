using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;

namespace StoryMode.GameComponents.CampaignBehaviors
{
	// Token: 0x02000050 RID: 80
	public class ThirdPhaseCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000450 RID: 1104 RVA: 0x0001A26B File Offset: 0x0001846B
		public override void RegisterEvents()
		{
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
			CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.WeeklyTick));
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x0001A29C File Offset: 0x0001849C
		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
		{
			Kingdom kingdom;
			Kingdom kingdom2;
			if ((kingdom = faction1 as Kingdom) != null && (kingdom2 = faction2 as Kingdom) != null && StoryModeManager.Current.MainStoryLine.ThirdPhase != null)
			{
				MBReadOnlyList<Kingdom> oppositionKingdoms = StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms;
				MBReadOnlyList<Kingdom> allyKingdoms = StoryModeManager.Current.MainStoryLine.ThirdPhase.AllyKingdoms;
				if ((oppositionKingdoms.IndexOf(kingdom) >= 0 && oppositionKingdoms.IndexOf(kingdom2) >= 0) || (allyKingdoms.IndexOf(kingdom) >= 0 && allyKingdoms.IndexOf(kingdom2) >= 0))
				{
					this._warsToEnforcePeaceNextWeek.Add(new Tuple<Kingdom, Kingdom>(kingdom, kingdom2));
				}
			}
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x0001A334 File Offset: 0x00018534
		private void WeeklyTick()
		{
			foreach (Tuple<Kingdom, Kingdom> tuple in new List<Tuple<Kingdom, Kingdom>>(this._warsToEnforcePeaceNextWeek))
			{
				MakePeaceAction.Apply(tuple.Item1, tuple.Item2, 0);
			}
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x0001A398 File Offset: 0x00018598
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<Tuple<Kingdom, Kingdom>>>("_warsToEnforcePeaceNextWeek", ref this._warsToEnforcePeaceNextWeek);
		}

		// Token: 0x040001C3 RID: 451
		private List<Tuple<Kingdom, Kingdom>> _warsToEnforcePeaceNextWeek = new List<Tuple<Kingdom, Kingdom>>();
	}
}
