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
	// Token: 0x02000021 RID: 33
	public class BombardmentStep1Tutorial : TutorialItemBase
	{
		// Token: 0x0600009E RID: 158 RVA: 0x000032C6 File Offset: 0x000014C6
		public BombardmentStep1Tutorial()
		{
			base.Type = "BombardmentStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = true;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000032F2 File Offset: 0x000014F2
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerSelectedSiegeEngine || this._isGameMenuChangedAfterActivation;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00003304 File Offset: 0x00001504
		public override void OnPlayerStartEngineConstruction(PlayerStartEngineConstructionEvent obj)
		{
			this._playerSelectedSiegeEngine = true;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x0000330D File Offset: 0x0000150D
		public override void OnGameMenuOptionSelected(GameMenuOption obj)
		{
			base.OnGameMenuOptionSelected(obj);
			if (this._isActivated)
			{
				this._isGameMenuChangedAfterActivation = true;
			}
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00003325 File Offset: 0x00001525
		public override void OnGameMenuOpened(MenuCallbackArgs obj)
		{
			base.OnGameMenuOpened(obj);
			if (this._isActivated)
			{
				this._isGameMenuChangedAfterActivation = true;
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x0000333D File Offset: 0x0000153D
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00003340 File Offset: 0x00001540
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

		// Token: 0x04000028 RID: 40
		private bool _playerSelectedSiegeEngine;

		// Token: 0x04000029 RID: 41
		private bool _isGameMenuChangedAfterActivation;

		// Token: 0x0400002A RID: 42
		private bool _isActivated;
	}
}
