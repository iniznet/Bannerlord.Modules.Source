using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000005 RID: 5
	public class UpgradingTroopsStep3Tutorial : TutorialItemBase
	{
		// Token: 0x06000011 RID: 17 RVA: 0x00002219 File Offset: 0x00000419
		public UpgradingTroopsStep3Tutorial()
		{
			base.Type = "UpgradingTroopsStep3";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "UpgradeButton";
			base.MouseRequired = true;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002245 File Offset: 0x00000445
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerUpgradedTroop;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000224D File Offset: 0x0000044D
		public override void OnPlayerUpgradeTroop(CharacterObject arg1, CharacterObject arg2, int arg3)
		{
			this._playerUpgradedTroop = true;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002256 File Offset: 0x00000456
		public override bool IsConditionsMetForActivation()
		{
			if (Hero.MainHero.Gold > 100 && TutorialHelper.CurrentContext == 1)
			{
				PartyScreenManager instance = PartyScreenManager.Instance;
				if (instance != null && instance.CurrentMode <= 0)
				{
					return TutorialHelper.PlayerHasAnyUpgradeableTroop;
				}
			}
			return false;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000228B File Offset: 0x0000048B
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 1;
		}

		// Token: 0x04000006 RID: 6
		private bool _playerUpgradedTroop;
	}
}
