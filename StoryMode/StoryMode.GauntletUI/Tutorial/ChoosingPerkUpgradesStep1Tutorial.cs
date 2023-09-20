using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000009 RID: 9
	public class ChoosingPerkUpgradesStep1Tutorial : TutorialItemBase
	{
		// Token: 0x06000025 RID: 37 RVA: 0x000023B3 File Offset: 0x000005B3
		public ChoosingPerkUpgradesStep1Tutorial()
		{
			base.Type = "ChoosingPerkUpgradesStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "CharacterButton";
			base.MouseRequired = true;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000023DF File Offset: 0x000005DF
		public override bool IsConditionsMetForCompletion()
		{
			return this._contextChangedToCharacterScreen;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000023E7 File Offset: 0x000005E7
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000023EA File Offset: 0x000005EA
		public override bool IsConditionsMetForActivation()
		{
			return (TutorialHelper.PlayerIsInAnySettlement || TutorialHelper.PlayerIsSafeOnMap) && Hero.MainHero.HeroDeveloper.GetOneAvailablePerkForEachPerkPair().Count > 1 && TutorialHelper.CurrentContext == 4;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x0000241B File Offset: 0x0000061B
		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._contextChangedToCharacterScreen = obj.NewContext == 3;
		}

		// Token: 0x0400000A RID: 10
		private bool _contextChangedToCharacterScreen;
	}
}
