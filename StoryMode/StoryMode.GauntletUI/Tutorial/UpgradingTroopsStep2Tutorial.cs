using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000004 RID: 4
	public class UpgradingTroopsStep2Tutorial : TutorialItemBase
	{
		// Token: 0x0600000B RID: 11 RVA: 0x00002189 File Offset: 0x00000389
		public UpgradingTroopsStep2Tutorial()
		{
			base.Type = "UpgradingTroopsStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Left;
			base.HighlightedVisualElementID = "UpgradePopupButton";
			base.MouseRequired = true;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000021B5 File Offset: 0x000003B5
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerUpgradedTroop || this._playerOpenedUpgradePopup;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000021C7 File Offset: 0x000003C7
		public override void OnPlayerToggledUpgradePopup(PlayerToggledUpgradePopupEvent obj)
		{
			if (obj.IsOpened)
			{
				this._playerOpenedUpgradePopup = true;
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000021D8 File Offset: 0x000003D8
		public override void OnPlayerUpgradeTroop(CharacterObject arg1, CharacterObject arg2, int arg3)
		{
			this._playerUpgradedTroop = true;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000021E1 File Offset: 0x000003E1
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

		// Token: 0x06000010 RID: 16 RVA: 0x00002216 File Offset: 0x00000416
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 1;
		}

		// Token: 0x04000004 RID: 4
		private bool _playerUpgradedTroop;

		// Token: 0x04000005 RID: 5
		private bool _playerOpenedUpgradePopup;
	}
}
