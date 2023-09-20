using System;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI
{
	[GameStateScreen(typeof(ClanState))]
	public class GauntletClanScreen : ScreenBase, IGameStateListener
	{
		public GauntletClanScreen(ClanState clanState)
		{
			this._clanState = clanState;
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			LoadingWindow.DisableGlobalLoadingWindow();
			this._dataSource.CanSwitchTabs = !Input.IsGamepadActive || (!InformationManager.GetIsAnyTooltipActiveAndExtended() && this._gauntletLayer.IsHitThisFrame);
			ClanManagementVM dataSource = this._dataSource;
			bool flag;
			if (dataSource == null)
			{
				flag = false;
			}
			else
			{
				ClanCardSelectionPopupVM cardSelectionPopup = dataSource.CardSelectionPopup;
				bool? flag2 = ((cardSelectionPopup != null) ? new bool?(cardSelectionPopup.IsVisible) : null);
				bool flag3 = true;
				flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
			}
			if (flag)
			{
				if (this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
				{
					this._dataSource.CardSelectionPopup.ExecuteDone();
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
				{
					this._dataSource.CardSelectionPopup.ExecuteCancel();
					return;
				}
			}
			else
			{
				if (this._gauntletLayer.Input.IsHotKeyReleased("Exit") || this._gauntletLayer.Input.IsGameKeyPressed(41) || this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
				{
					this.CloseClanScreen();
					return;
				}
				if (this._dataSource.CanSwitchTabs)
				{
					if (this._gauntletLayer.Input.IsHotKeyReleased("SwitchToPreviousTab"))
					{
						this._dataSource.SelectPreviousCategory();
						return;
					}
					if (this._gauntletLayer.Input.IsHotKeyReleased("SwitchToNextTab"))
					{
						this._dataSource.SelectNextCategory();
					}
				}
			}
		}

		private void OpenPartyScreenForNewClanParty(Hero hero)
		{
			this._isCreatingPartyWithMembers = true;
			PartyScreenManager.OpenScreenAsCreateClanPartyForHero(hero, null, null);
		}

		private void OpenBannerEditorWithPlayerClan()
		{
			Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<BannerEditorState>(), 0);
		}

		void IGameStateListener.OnActivate()
		{
			base.OnActivate();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._clanCategory = spriteData.SpriteCategories["ui_clan"];
			this._clanCategory.Load(resourceContext, uiresourceDepot);
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", true);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._gauntletLayer);
			base.AddLayer(this._gauntletLayer);
			this._dataSource = new ClanManagementVM(new Action(this.CloseClanScreen), new Action<Hero>(this.ShowHeroOnMap), new Action<Hero>(this.OpenPartyScreenForNewClanParty), new Action(this.OpenBannerEditorWithPlayerClan));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetPreviousTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
			this._dataSource.SetNextTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
			if (this._isCreatingPartyWithMembers)
			{
				this._dataSource.SelectParty(PartyBase.MainParty);
				this._isCreatingPartyWithMembers = false;
			}
			else if (this._clanState.InitialSelectedHero != null)
			{
				this._dataSource.SelectHero(this._clanState.InitialSelectedHero);
			}
			else if (this._clanState.InitialSelectedParty != null)
			{
				this._dataSource.SelectParty(this._clanState.InitialSelectedParty);
				if (this._clanState.InitialSelectedParty.LeaderHero == null)
				{
					ClanPartiesVM clanParties = this._dataSource.ClanParties;
					bool flag;
					if (clanParties == null)
					{
						flag = false;
					}
					else
					{
						ClanPartyItemVM currentSelectedParty = clanParties.CurrentSelectedParty;
						bool? flag2 = ((currentSelectedParty != null) ? new bool?(currentSelectedParty.IsChangeLeaderEnabled) : null);
						bool flag3 = true;
						flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
					}
					if (flag)
					{
						this._dataSource.ClanParties.OnShowChangeLeaderPopup();
					}
				}
			}
			else if (this._clanState.InitialSelectedSettlement != null)
			{
				this._dataSource.SelectSettlement(this._clanState.InitialSelectedSettlement);
			}
			else if (this._clanState.InitialSelectedWorkshop != null)
			{
				this._dataSource.SelectWorkshop(this._clanState.InitialSelectedWorkshop);
			}
			else if (this._clanState.InitialSelectedAlley != null)
			{
				this._dataSource.SelectAlley(this._clanState.InitialSelectedAlley);
			}
			this._gauntletLayer.LoadMovie("ClanScreen", this._dataSource);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(6));
			SoundEvent.PlaySound2D("event:/ui/panels/panel_clan_open");
			this._gauntletLayer._gauntletUIContext.EventManager.GainNavigationAfterFrames(2, null);
		}

		private void ShowHeroOnMap(Hero hero)
		{
			Vec2 asVec = hero.GetPosition().AsVec2;
			this.CloseClanScreen();
			MapScreen.Instance.FastMoveCameraToPosition(asVec);
		}

		void IGameStateListener.OnDeactivate()
		{
			base.OnDeactivate();
			base.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
		}

		void IGameStateListener.OnInitialize()
		{
		}

		void IGameStateListener.OnFinalize()
		{
			this._clanCategory.Unload();
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer = null;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			ClanManagementVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.UpdateBannerVisuals();
			}
			ClanManagementVM dataSource2 = this._dataSource;
			if (dataSource2 == null)
			{
				return;
			}
			dataSource2.RefreshCategoryValues();
		}

		private void CloseClanScreen()
		{
			Game.Current.GameStateManager.PopState(0);
		}

		private const string _panelOpenSound = "event:/ui/panels/panel_clan_open";

		private ClanManagementVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private SpriteCategory _clanCategory;

		private readonly ClanState _clanState;

		private bool _isCreatingPartyWithMembers;
	}
}
