using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class ArmyCohesionStep1Tutorial : TutorialItemBase
	{
		public ArmyCohesionStep1Tutorial()
		{
			base.Type = "ArmyCohesionStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "ArmyOverlayArmyManagementButton";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerArmyNeedsCohesion && this._playerOpenedArmyManagement;
		}

		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._playerOpenedArmyManagement = this._playerArmyNeedsCohesion && obj.NewContext == 10;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override bool IsConditionsMetForActivation()
		{
			bool playerArmyNeedsCohesion = this._playerArmyNeedsCohesion;
			Army army = MobileParty.MainParty.Army;
			float? num = ((army != null) ? new float?(army.Cohesion) : null);
			float maxCohesionForCohesionTutorial = TutorialHelper.MaxCohesionForCohesionTutorial;
			this._playerArmyNeedsCohesion = playerArmyNeedsCohesion | ((num.GetValueOrDefault() < maxCohesionForCohesionTutorial) & (num != null));
			return TutorialHelper.CurrentContext == 4 && MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty && MobileParty.MainParty.Army.Cohesion < TutorialHelper.MaxCohesionForCohesionTutorial;
		}

		private bool _playerOpenedArmyManagement;

		private bool _playerArmyNeedsCohesion;
	}
}
