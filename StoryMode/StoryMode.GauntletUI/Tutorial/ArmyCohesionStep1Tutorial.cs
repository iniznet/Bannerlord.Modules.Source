using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200001B RID: 27
	public class ArmyCohesionStep1Tutorial : TutorialItemBase
	{
		// Token: 0x0600007F RID: 127 RVA: 0x00002ED2 File Offset: 0x000010D2
		public ArmyCohesionStep1Tutorial()
		{
			base.Type = "ArmyCohesionStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "ArmyOverlayArmyManagementButton";
			base.MouseRequired = true;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00002EFE File Offset: 0x000010FE
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerArmyNeedsCohesion && this._playerOpenedArmyManagement;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00002F10 File Offset: 0x00001110
		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._playerOpenedArmyManagement = this._playerArmyNeedsCohesion && obj.NewContext == 10;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00002F2D File Offset: 0x0000112D
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00002F30 File Offset: 0x00001130
		public override bool IsConditionsMetForActivation()
		{
			bool playerArmyNeedsCohesion = this._playerArmyNeedsCohesion;
			Army army = MobileParty.MainParty.Army;
			float? num = ((army != null) ? new float?(army.Cohesion) : null);
			float maxCohesionForCohesionTutorial = TutorialHelper.MaxCohesionForCohesionTutorial;
			this._playerArmyNeedsCohesion = playerArmyNeedsCohesion | ((num.GetValueOrDefault() < maxCohesionForCohesionTutorial) & (num != null));
			return TutorialHelper.CurrentContext == 4 && MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty && MobileParty.MainParty.Army.Cohesion < TutorialHelper.MaxCohesionForCohesionTutorial;
		}

		// Token: 0x04000020 RID: 32
		private bool _playerOpenedArmyManagement;

		// Token: 0x04000021 RID: 33
		private bool _playerArmyNeedsCohesion;
	}
}
