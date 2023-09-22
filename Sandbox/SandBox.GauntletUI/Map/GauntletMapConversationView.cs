using System;
using System.Collections.Generic;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.ViewModelCollection.Barter;
using TaleWorlds.CampaignSystem.ViewModelCollection.Conversation;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapConversation;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.Map
{
	[OverrideView(typeof(MapConversationView))]
	public class GauntletMapConversationView : MapConversationView, IConversationStateHandler
	{
		public GauntletMapConversationView(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData)
		{
			this._conversationStateQueue = new Queue<GauntletMapConversationView.ConversationStates>();
			this._playerCharacterData = new ConversationCharacterData?(playerCharacterData);
			this._conversationPartnerData = new ConversationCharacterData?(conversationPartnerData);
			this._barter = Campaign.Current.BarterManager;
			BarterManager barter = this._barter;
			barter.BarterBegin = (BarterManager.BarterBeginEventDelegate)Delegate.Combine(barter.BarterBegin, new BarterManager.BarterBeginEventDelegate(this.OnBarterBegin));
			BarterManager barter2 = this._barter;
			barter2.Closed = (BarterManager.BarterCloseEventDelegate)Delegate.Combine(barter2.Closed, new BarterManager.BarterCloseEventDelegate(this.OnBarterClosed));
		}

		private void OnBarterClosed()
		{
			this._layerAsGauntletLayer.ReleaseMovie(this._barterMovie);
			this._barterCategory.Unload();
			this._barterDataSource = null;
			this._isBarterActive = false;
			this._dataSource.IsBarterActive = false;
			BarterItemVM.IsFiveStackModifierActive = false;
			BarterItemVM.IsEntireStackModifierActive = false;
		}

		private void OnBarterBegin(BarterData args)
		{
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._barterCategory = spriteData.SpriteCategories["ui_barter"];
			this._barterCategory.Load(resourceContext, uiresourceDepot);
			this._barterDataSource = new BarterVM(args);
			this._barterDataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			this._barterDataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._barterDataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._barterMovie = this._layerAsGauntletLayer.LoadMovie("BarterScreen", this._barterDataSource);
			this._isBarterActive = true;
			this._dataSource.IsBarterActive = true;
		}

		protected override void CreateLayout()
		{
			base.CreateLayout();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._conversationCategory = spriteData.SpriteCategories["ui_conversation"];
			this._conversationCategory.Load(resourceContext, uiresourceDepot);
			Campaign.Current.ConversationManager.Handler = this;
			Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
		}

		private void OnContinue()
		{
			MapConversationVM dataSource = this._dataSource;
			bool flag;
			if (dataSource == null)
			{
				flag = false;
			}
			else
			{
				MissionConversationVM dialogController = dataSource.DialogController;
				int? num = ((dialogController != null) ? new int?(dialogController.AnswerList.Count) : null);
				int num2 = 0;
				flag = (num.GetValueOrDefault() <= num2) & (num != null);
			}
			if (flag && !this._isBarterActive)
			{
				this._dataSource.DialogController.ExecuteContinue();
			}
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this.Tick();
		}

		protected override void OnIdleTick(float dt)
		{
			base.OnIdleTick(dt);
			this.Tick();
		}

		protected override bool IsEscaped()
		{
			return !this._isConversationInstalled;
		}

		protected override bool IsOpeningEscapeMenuOnFocusChangeAllowed()
		{
			return true;
		}

		private void Tick()
		{
			if (this._conversationStateQueue.Count > 0)
			{
				GauntletMapConversationView.ConversationStates conversationStates = this._conversationStateQueue.Dequeue();
				this.ProcessConversationState(conversationStates);
			}
			if (this._isConversationInstalled && ScreenManager.TopScreen == base.MapScreen && ScreenManager.FocusedLayer != base.Layer)
			{
				ScreenManager.TrySetFocus(base.Layer);
			}
			MapConversationVM dataSource = this._dataSource;
			bool flag;
			if (dataSource == null)
			{
				flag = false;
			}
			else
			{
				MissionConversationVM dialogController = dataSource.DialogController;
				int? num = ((dialogController != null) ? new int?(dialogController.AnswerList.Count) : null);
				int num2 = 0;
				flag = (num.GetValueOrDefault() <= num2) & (num != null);
			}
			if (flag && !this._isBarterActive && this.IsReleasedInGauntletLayer("ContinueKey"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				MapConversationVM dataSource2 = this._dataSource;
				if (dataSource2 != null)
				{
					MissionConversationVM dialogController2 = dataSource2.DialogController;
					if (dialogController2 != null)
					{
						dialogController2.ExecuteContinue();
					}
				}
			}
			if (this._barterDataSource != null)
			{
				if (this.IsReleasedInGauntletLayer("Exit"))
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._barterDataSource.ExecuteCancel();
				}
				else
				{
					if (this.IsReleasedInGauntletLayer("Confirm"))
					{
						BarterVM barterDataSource = this._barterDataSource;
						if (barterDataSource != null && !barterDataSource.IsOfferDisabled)
						{
							UISoundsHelper.PlayUISound("event:/ui/default");
							this._barterDataSource.ExecuteOffer();
							goto IL_1A3;
						}
					}
					if (this.IsReleasedInGauntletLayer("Reset"))
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._barterDataSource.ExecuteReset();
					}
				}
			}
			else if (this.IsReleasedInGauntletLayer("ToggleEscapeMenu"))
			{
				MapScreen mapScreen = base.MapScreen;
				if (mapScreen != null && mapScreen.IsEscapeMenuOpened)
				{
					base.MapScreen.CloseEscapeMenu();
				}
				else
				{
					MapScreen mapScreen2 = base.MapScreen;
					if (mapScreen2 != null)
					{
						mapScreen2.OpenEscapeMenu();
					}
				}
			}
			IL_1A3:
			BarterItemVM.IsFiveStackModifierActive = this.IsDownInGauntletLayer("FiveStackModifier");
			BarterItemVM.IsEntireStackModifierActive = this.IsDownInGauntletLayer("EntireStackModifier");
		}

		protected override void OnMenuModeTick(float dt)
		{
			base.OnMenuModeTick(dt);
			this.Tick();
		}

		protected override void OnMapConversationUpdate(ConversationCharacterData playerConversationData, ConversationCharacterData partnerConversationData)
		{
			base.OnMapConversationUpdate(playerConversationData, partnerConversationData);
			float num = CampaignTime.Now.CurrentHourInDay * 1f;
			MapWeatherModel.WeatherEvent weatherEventInPosition = Campaign.Current.Models.MapWeatherModel.GetWeatherEventInPosition(MobileParty.MainParty.Position2D);
			bool flag = weatherEventInPosition == 3 || weatherEventInPosition == 4;
			bool flag2 = false;
			if (partnerConversationData.Character.HeroObject != null)
			{
				LocationComplex locationComplex = LocationComplex.Current;
				string text;
				if (locationComplex == null)
				{
					text = null;
				}
				else
				{
					Location locationOfCharacter = locationComplex.GetLocationOfCharacter(partnerConversationData.Character.HeroObject);
					text = ((locationOfCharacter != null) ? locationOfCharacter.StringId : null);
				}
				string text2 = text;
				flag2 = Hero.MainHero.CurrentSettlement != null && (text2 == "lordshall" || text2 == "tavern");
			}
			MapConversationTableauData mapConversationTableauData = MapConversationTableauData.CreateFrom(playerConversationData, partnerConversationData, Campaign.Current.MapSceneWrapper.GetFaceTerrainType(MobileParty.MainParty.CurrentNavigationFace), num, flag, Hero.MainHero.CurrentSettlement, flag2, weatherEventInPosition == 2, weatherEventInPosition == 4);
			if (!GauntletMapConversationView.IsSame(mapConversationTableauData, this._tableauData))
			{
				this._dataSource.TableauData = mapConversationTableauData;
			}
		}

		private bool IsReleasedInGauntletLayer(string hotKeyID)
		{
			ScreenLayer layer = base.Layer;
			return layer != null && layer.Input.IsHotKeyReleased(hotKeyID);
		}

		private bool IsDownInGauntletLayer(string hotKeyID)
		{
			ScreenLayer layer = base.Layer;
			return layer != null && layer.Input.IsHotKeyDown(hotKeyID);
		}

		private void OnClose()
		{
			MapState mapState = Game.Current.GameStateManager.LastOrDefault<MapState>();
			if (mapState == null)
			{
				return;
			}
			mapState.OnMapConversationOver();
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
			BarterManager barter = this._barter;
			barter.BarterBegin = (BarterManager.BarterBeginEventDelegate)Delegate.Remove(barter.BarterBegin, new BarterManager.BarterBeginEventDelegate(this.OnBarterBegin));
			BarterManager barter2 = this._barter;
			barter2.Closed = (BarterManager.BarterCloseEventDelegate)Delegate.Remove(barter2.Closed, new BarterManager.BarterCloseEventDelegate(this.OnBarterClosed));
			this._dataSource.OnFinalize();
			BarterVM barterDataSource = this._barterDataSource;
			if (barterDataSource != null)
			{
				barterDataSource.OnFinalize();
			}
			base.MapScreen.RemoveLayer(base.Layer);
			SpriteCategory conversationCategory = this._conversationCategory;
			if (conversationCategory != null)
			{
				conversationCategory.Unload();
			}
			base.Layer = null;
			this._barterMovie = null;
			this._dataSource = null;
			this._barterDataSource = null;
			Campaign.Current.ConversationManager.Handler = null;
			Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
		}

		private string GetContinueKeyText()
		{
			if (Input.IsGamepadActive)
			{
				GameTexts.SetVariable("CONSOLE_KEY_NAME", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("ConversationHotKeyCategory", "ContinueKey")));
				return GameTexts.FindText("str_click_to_continue_console", null).ToString();
			}
			return GameTexts.FindText("str_click_to_continue", null).ToString();
		}

		private void ProcessConversationState(GauntletMapConversationView.ConversationStates state)
		{
			switch (state)
			{
			case GauntletMapConversationView.ConversationStates.OnInstall:
				this.CreateConversationTableau();
				return;
			case GauntletMapConversationView.ConversationStates.OnUninstall:
				this.UninstallConversation();
				return;
			case GauntletMapConversationView.ConversationStates.OnActivate:
				break;
			case GauntletMapConversationView.ConversationStates.OnDeactivate:
				MBInformationManager.HideInformations();
				return;
			case GauntletMapConversationView.ConversationStates.OnContinue:
				this._dataSource.DialogController.OnConversationContinue();
				return;
			case GauntletMapConversationView.ConversationStates.ExecuteContinue:
				this._dataSource.DialogController.ExecuteContinue();
				break;
			default:
				return;
			}
		}

		private void CreateConversationTableau()
		{
			float num = CampaignTime.Now.CurrentHourInDay * 1f;
			MapWeatherModel.WeatherEvent weatherEventInPosition = Campaign.Current.Models.MapWeatherModel.GetWeatherEventInPosition(MobileParty.MainParty.Position2D);
			bool flag = weatherEventInPosition == 3 || weatherEventInPosition == 4;
			bool flag2 = false;
			if (this._conversationPartnerData.Value.Character.HeroObject != null)
			{
				LocationComplex locationComplex = LocationComplex.Current;
				string text;
				if (locationComplex == null)
				{
					text = null;
				}
				else
				{
					Location locationOfCharacter = locationComplex.GetLocationOfCharacter(this._conversationPartnerData.Value.Character.HeroObject);
					text = ((locationOfCharacter != null) ? locationOfCharacter.StringId : null);
				}
				string text2 = text;
				flag2 = Hero.MainHero.CurrentSettlement != null && (text2 == "lordshall" || text2 == "tavern");
			}
			this._tableauData = MapConversationTableauData.CreateFrom(this._playerCharacterData.Value, this._conversationPartnerData.Value, Campaign.Current.MapSceneWrapper.GetFaceTerrainType(MobileParty.MainParty.CurrentNavigationFace), num, flag, Hero.MainHero.CurrentSettlement, flag2, weatherEventInPosition == 2, weatherEventInPosition == 4);
			this._dataSource.TableauData = this._tableauData;
			this._dataSource.IsTableauEnabled = true;
			this._layerAsGauntletLayer._gauntletUIContext.EventManager.GainNavigationAfterFrames(1, null);
		}

		private void UninstallConversation()
		{
			if (this._isConversationInstalled)
			{
				this.OnClose();
				this._isConversationInstalled = false;
			}
		}

		void IConversationStateHandler.OnConversationInstall()
		{
			if (!this._isConversationInstalled)
			{
				base.CreateConversationMission();
				this._dataSource = new MapConversationVM(new Action(this.OnContinue), new Func<string>(this.GetContinueKeyText));
				base.Layer = new GauntletLayer(205, "GauntletLayer", false);
				this._layerAsGauntletLayer = base.Layer as GauntletLayer;
				this._layerAsGauntletLayer.LoadMovie("MapConversation", this._dataSource);
				base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
				base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
				base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
				base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
				base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ConversationHotKeyCategory"));
				base.MapScreen.AddLayer(base.Layer);
				base.Layer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(base.Layer);
				this._conversationStateQueue.Enqueue(GauntletMapConversationView.ConversationStates.OnInstall);
				this._isConversationInstalled = true;
			}
		}

		void IConversationStateHandler.OnConversationUninstall()
		{
			this._conversationStateQueue.Enqueue(GauntletMapConversationView.ConversationStates.OnUninstall);
		}

		void IConversationStateHandler.OnConversationActivate()
		{
			if (this._conversationStateQueue.Count > 0)
			{
				this._conversationStateQueue.Enqueue(GauntletMapConversationView.ConversationStates.OnActivate);
				return;
			}
			this.ProcessConversationState(GauntletMapConversationView.ConversationStates.OnActivate);
		}

		void IConversationStateHandler.OnConversationDeactivate()
		{
			if (this._conversationStateQueue.Count > 0)
			{
				this._conversationStateQueue.Enqueue(GauntletMapConversationView.ConversationStates.OnDeactivate);
				return;
			}
			this.ProcessConversationState(GauntletMapConversationView.ConversationStates.OnDeactivate);
		}

		void IConversationStateHandler.OnConversationContinue()
		{
			if (this._conversationStateQueue.Count > 0)
			{
				this._conversationStateQueue.Enqueue(GauntletMapConversationView.ConversationStates.OnContinue);
				return;
			}
			this.ProcessConversationState(GauntletMapConversationView.ConversationStates.OnContinue);
		}

		void IConversationStateHandler.ExecuteConversationContinue()
		{
			if (this._conversationStateQueue.Count > 0)
			{
				this._conversationStateQueue.Enqueue(GauntletMapConversationView.ConversationStates.ExecuteContinue);
				return;
			}
			this.ProcessConversationState(GauntletMapConversationView.ConversationStates.ExecuteContinue);
		}

		private static bool IsSame(MapConversationTableauData first, MapConversationTableauData second)
		{
			return first != null && second != null && (GauntletMapConversationView.IsSame(first.PlayerCharacterData, second.PlayerCharacterData) && GauntletMapConversationView.IsSame(first.ConversationPartnerData, second.ConversationPartnerData) && first.ConversationTerrainType == second.ConversationTerrainType && first.IsCurrentTerrainUnderSnow == second.IsCurrentTerrainUnderSnow) && first.TimeOfDay == second.TimeOfDay;
		}

		private static bool IsSame(ConversationCharacterData first, ConversationCharacterData second)
		{
			return first.Character == second.Character && first.NoHorse == second.NoHorse && first.NoWeapon == second.NoWeapon && first.Party == second.Party && first.SpawnedAfterFight == second.SpawnedAfterFight && first.IsCivilianEquipmentRequiredForLeader == second.IsCivilianEquipmentRequiredForLeader;
		}

		private GauntletLayer _layerAsGauntletLayer;

		private MapConversationVM _dataSource;

		private SpriteCategory _conversationCategory;

		private MapConversationTableauData _tableauData;

		private bool _isBarterActive;

		private Queue<GauntletMapConversationView.ConversationStates> _conversationStateQueue;

		private ConversationCharacterData? _playerCharacterData;

		private ConversationCharacterData? _conversationPartnerData;

		private bool _isConversationInstalled;

		private BarterManager _barter;

		private SpriteCategory _barterCategory;

		private BarterVM _barterDataSource;

		private IGauntletMovie _barterMovie;

		private enum ConversationStates
		{
			OnInstall,
			OnUninstall,
			OnActivate,
			OnDeactivate,
			OnContinue,
			ExecuteContinue
		}
	}
}
