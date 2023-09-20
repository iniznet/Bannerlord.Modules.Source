using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;

namespace StoryMode.GameComponents.CampaignBehaviors
{
	public class ThirdPhaseCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
			CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.WeeklyTick));
			CampaignEvents.CanKingdomBeDiscontinuedEvent.AddNonSerializedListener(this, new ReferenceAction<Kingdom, bool>(this.CanKingdomBeDiscontinued));
		}

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

		private void WeeklyTick()
		{
			foreach (Tuple<Kingdom, Kingdom> tuple in new List<Tuple<Kingdom, Kingdom>>(this._warsToEnforcePeaceNextWeek))
			{
				MakePeaceAction.Apply(tuple.Item1, tuple.Item2, 0);
			}
		}

		private void CanKingdomBeDiscontinued(Kingdom kingdom, ref bool result)
		{
			if (StoryModeManager.Current.MainStoryLine.ThirdPhase != null && StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms.Contains(kingdom))
			{
				result = false;
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<Tuple<Kingdom, Kingdom>>>("_warsToEnforcePeaceNextWeek", ref this._warsToEnforcePeaceNextWeek);
		}

		private List<Tuple<Kingdom, Kingdom>> _warsToEnforcePeaceNextWeek = new List<Tuple<Kingdom, Kingdom>>();
	}
}
