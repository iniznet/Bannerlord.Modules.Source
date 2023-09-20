using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class ArmyCohesionStep2Tutorial : TutorialItemBase
	{
		public ArmyCohesionStep2Tutorial()
		{
			base.Type = "ArmyCohesionStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "ArmyManagementBoostCohesionButton";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerBoostedCohesion;
		}

		public override void OnArmyCohesionByPlayerBoosted(ArmyCohesionBoostedByPlayerEvent obj)
		{
			this._playerBoostedCohesion = true;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 10;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 10 && Campaign.Current.CurrentMenuContext == null && MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty && MobileParty.MainParty.Army.Cohesion < TutorialHelper.MaxCohesionForCohesionTutorial;
		}

		private bool _playerBoostedCohesion;
	}
}
