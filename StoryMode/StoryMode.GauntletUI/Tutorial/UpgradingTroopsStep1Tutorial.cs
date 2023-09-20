using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000003 RID: 3
	public class UpgradingTroopsStep1Tutorial : TutorialItemBase
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002101 File Offset: 0x00000301
		public UpgradingTroopsStep1Tutorial()
		{
			base.Type = "UpgradingTroopsStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "PartyButton";
			base.MouseRequired = true;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000212D File Offset: 0x0000032D
		public override bool IsConditionsMetForCompletion()
		{
			return this._partyScreenOpened || this._playerUpgradedTroop;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000213F File Offset: 0x0000033F
		public override void OnPlayerUpgradeTroop(CharacterObject arg1, CharacterObject arg2, int arg3)
		{
			this._playerUpgradedTroop = true;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002148 File Offset: 0x00000348
		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._partyScreenOpened = obj.NewContext == 1;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002159 File Offset: 0x00000359
		public override bool IsConditionsMetForActivation()
		{
			return Hero.MainHero.Gold >= 100 && TutorialHelper.CurrentContext == 4 && !TutorialHelper.PlayerIsInAnySettlement && TutorialHelper.PlayerIsSafeOnMap && TutorialHelper.PlayerHasAnyUpgradeableTroop;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002186 File Offset: 0x00000386
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x04000002 RID: 2
		private bool _partyScreenOpened;

		// Token: 0x04000003 RID: 3
		private bool _playerUpgradedTroop;
	}
}
