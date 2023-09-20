using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000010 RID: 16
	public class GettingCompanionsStep3Tutorial : TutorialItemBase
	{
		// Token: 0x06000048 RID: 72 RVA: 0x000027BE File Offset: 0x000009BE
		public GettingCompanionsStep3Tutorial()
		{
			base.Type = "GettingCompanionsStep3";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "OverlayTalkButton";
			base.MouseRequired = true;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000027EA File Offset: 0x000009EA
		public override bool IsConditionsMetForCompletion()
		{
			return this._startedTalkingWithCompanion;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000027F2 File Offset: 0x000009F2
		public override void OnPlayerStartTalkFromMenuOverlay(Hero hero)
		{
			this._startedTalkingWithCompanion = hero.IsWanderer && !hero.IsPlayerCompanion;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x0000280E File Offset: 0x00000A0E
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002814 File Offset: 0x00000A14
		public override bool IsConditionsMetForActivation()
		{
			LocationComplex locationComplex = LocationComplex.Current;
			Location location = ((locationComplex != null) ? locationComplex.GetLocationWithId("tavern") : null);
			if (TutorialHelper.PlayerIsInNonEnemyTown && TutorialHelper.CurrentContext == 4 && TutorialHelper.BackStreetMenuIsOpen && TutorialHelper.IsCharacterPopUpWindowOpen && Clan.PlayerClan.Companions.Count == 0 && Clan.PlayerClan.CompanionLimit > 0)
			{
				bool? flag = TutorialHelper.IsThereAvailableCompanionInLocation(location);
				bool flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					return Hero.MainHero.Gold > TutorialHelper.MinimumGoldForCompanion;
				}
			}
			return false;
		}

		// Token: 0x04000011 RID: 17
		private bool _startedTalkingWithCompanion;
	}
}
