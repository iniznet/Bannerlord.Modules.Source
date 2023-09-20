using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200001C RID: 28
	public class ArmyCohesionStep2Tutorial : TutorialItemBase
	{
		// Token: 0x06000084 RID: 132 RVA: 0x00002FC8 File Offset: 0x000011C8
		public ArmyCohesionStep2Tutorial()
		{
			base.Type = "ArmyCohesionStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "ArmyManagementBoostCohesionButton";
			base.MouseRequired = true;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00002FF4 File Offset: 0x000011F4
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerBoostedCohesion;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00002FFC File Offset: 0x000011FC
		public override void OnArmyCohesionByPlayerBoosted(ArmyCohesionBoostedByPlayerEvent obj)
		{
			this._playerBoostedCohesion = true;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00003005 File Offset: 0x00001205
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 10;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000300C File Offset: 0x0000120C
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 10 && Campaign.Current.CurrentMenuContext == null && MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty && MobileParty.MainParty.Army.Cohesion < TutorialHelper.MaxCohesionForCohesionTutorial;
		}

		// Token: 0x04000022 RID: 34
		private bool _playerBoostedCohesion;
	}
}
