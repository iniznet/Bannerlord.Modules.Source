using System;
using SandBox.ViewModelCollection.MapSiege;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class BombardmentStep1Tutorial : TutorialItemBase
	{
		public BombardmentStep1Tutorial()
		{
			base.Type = "BombardmentStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerSelectedSiegeEngine || this._isGameMenuChangedAfterActivation;
		}

		public override void OnPlayerStartEngineConstruction(PlayerStartEngineConstructionEvent obj)
		{
			this._playerSelectedSiegeEngine = true;
		}

		public override void OnGameMenuOptionSelected(GameMenuOption obj)
		{
			base.OnGameMenuOptionSelected(obj);
			if (this._isActivated)
			{
				this._isGameMenuChangedAfterActivation = true;
			}
		}

		public override void OnGameMenuOpened(MenuCallbackArgs obj)
		{
			base.OnGameMenuOpened(obj);
			if (this._isActivated)
			{
				this._isGameMenuChangedAfterActivation = true;
			}
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override bool IsConditionsMetForActivation()
		{
			MenuContext currentMenuContext = Campaign.Current.CurrentMenuContext;
			bool flag5;
			if (((currentMenuContext != null) ? currentMenuContext.GameMenu.StringId : null) == "menu_siege_strategies")
			{
				SiegeEvent playerSiegeEvent = PlayerSiege.PlayerSiegeEvent;
				bool flag;
				if (playerSiegeEvent == null)
				{
					flag = false;
				}
				else
				{
					SiegeEvent.SiegeEnginesContainer siegeEngines = playerSiegeEvent.GetSiegeEventSide(PlayerSiege.PlayerSide).SiegeEngines;
					bool? flag2;
					if (siegeEngines == null)
					{
						flag2 = null;
					}
					else
					{
						SiegeEvent.SiegeEngineConstructionProgress siegePreparations = siegeEngines.SiegePreparations;
						flag2 = ((siegePreparations != null) ? new bool?(siegePreparations.IsConstructed) : null);
					}
					bool? flag3 = flag2;
					bool flag4 = true;
					flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
				}
				if (flag)
				{
					flag5 = TutorialHelper.CurrentContext == 4;
					goto IL_92;
				}
			}
			flag5 = false;
			IL_92:
			this._isActivated = flag5;
			return this._isActivated;
		}

		private bool _playerSelectedSiegeEngine;

		private bool _isGameMenuChangedAfterActivation;

		private bool _isActivated;
	}
}
