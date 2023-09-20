using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class RaidVillageStep1Tutorial : TutorialItemBase
	{
		public RaidVillageStep1Tutorial()
		{
			base.Type = "RaidVillageStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Top;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._gameMenuChanged;
		}

		public override void OnGameMenuOpened(MenuCallbackArgs obj)
		{
			if (this._villageRaidMenuOpened && obj.MenuContext.GameMenu.StringId != TutorialHelper.ActiveVillageRaidGameMenuID)
			{
				this._gameMenuChanged = true;
			}
		}

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

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override bool IsConditionsMetForActivation()
		{
			this._villageRaidMenuOpened = TutorialHelper.IsActiveVillageRaidGameMenuOpen;
			return this._villageRaidMenuOpened;
		}

		private bool _gameMenuChanged;

		private bool _villageRaidMenuOpened;
	}
}
