using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000020 RID: 32
	public class RaidVillageStep1Tutorial : TutorialItemBase
	{
		// Token: 0x06000098 RID: 152 RVA: 0x000031F5 File Offset: 0x000013F5
		public RaidVillageStep1Tutorial()
		{
			base.Type = "RaidVillageStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Top;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = true;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00003221 File Offset: 0x00001421
		public override bool IsConditionsMetForCompletion()
		{
			return this._gameMenuChanged;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00003229 File Offset: 0x00001429
		public override void OnGameMenuOpened(MenuCallbackArgs obj)
		{
			if (this._villageRaidMenuOpened && obj.MenuContext.GameMenu.StringId != TutorialHelper.ActiveVillageRaidGameMenuID)
			{
				this._gameMenuChanged = true;
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00003258 File Offset: 0x00001458
		public override void OnGameMenuOptionSelected(GameMenuOption obj)
		{
			base.OnGameMenuOptionSelected(obj);
			if (this._villageRaidMenuOpened)
			{
				Campaign campaign = Campaign.Current;
				string text;
				if (campaign == null)
				{
					text = null;
				}
				else
				{
					MenuContext currentMenuContext = campaign.CurrentMenuContext;
					if (currentMenuContext == null)
					{
						text = null;
					}
					else
					{
						GameMenu gameMenu = currentMenuContext.GameMenu;
						text = ((gameMenu != null) ? gameMenu.StringId : null);
					}
				}
				if (text == TutorialHelper.ActiveVillageRaidGameMenuID)
				{
					this._gameMenuChanged = true;
				}
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000032B0 File Offset: 0x000014B0
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000032B3 File Offset: 0x000014B3
		public override bool IsConditionsMetForActivation()
		{
			this._villageRaidMenuOpened = TutorialHelper.IsActiveVillageRaidGameMenuOpen;
			return this._villageRaidMenuOpened;
		}

		// Token: 0x04000026 RID: 38
		private bool _gameMenuChanged;

		// Token: 0x04000027 RID: 39
		private bool _villageRaidMenuOpened;
	}
}
