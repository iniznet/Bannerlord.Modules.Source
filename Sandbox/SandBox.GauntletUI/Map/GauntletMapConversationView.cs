using System;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.BarterSystem;
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
	// Token: 0x02000026 RID: 38
	[OverrideView(typeof(MapConversationView))]
	public class GauntletMapConversationView : MapView, IConversationStateHandler
	{
		// Token: 0x0600015B RID: 347 RVA: 0x0000AADC File Offset: 0x00008CDC
		public GauntletMapConversationView(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData)
		{
			float num = CampaignTime.Now.CurrentHourInDay * 1f;
			bool isSnowTerrainInPos = Campaign.Current.Models.MapWeatherModel.GetIsSnowTerrainInPos(MobileParty.MainParty.GetPosition());
			bool flag = false;
			if (conversationPartnerData.Character.HeroObject != null)
			{
				LocationComplex locationComplex = LocationComplex.Current;
				string text;
				if (locationComplex == null)
				{
					text = null;
				}
				else
				{
					Location locationOfCharacter = locationComplex.GetLocationOfCharacter(conversationPartnerData.Character.HeroObject);
					text = ((locationOfCharacter != null) ? locationOfCharacter.StringId : null);
				}
				string text2 = text;
				flag = Hero.MainHero.CurrentSettlement != null && (text2 == "lordshall" || text2 == "tavern");
			}
			this._tableauData = MapConversationTableauData.CreateFrom(playerCharacterData, conversationPartnerData, Campaign.Current.MapSceneWrapper.GetFaceTerrainType(MobileParty.MainParty.CurrentNavigationFace), num, isSnowTerrainInPos, Hero.MainHero.CurrentSettlement, flag);
			this._barter = Campaign.Current.BarterManager;
			BarterManager barter = this._barter;
			barter.BarterBegin = (BarterManager.BarterBeginEventDelegate)Delegate.Combine(barter.BarterBegin, new BarterManager.BarterBeginEventDelegate(this.OnBarterBegin));
			BarterManager barter2 = this._barter;
			barter2.Closed = (BarterManager.BarterCloseEventDelegate)Delegate.Combine(barter2.Closed, new BarterManager.BarterCloseEventDelegate(this.OnBarterClosed));
		}

		// Token: 0x0600015C RID: 348 RVA: 0x0000AC20 File Offset: 0x00008E20
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

		// Token: 0x0600015D RID: 349 RVA: 0x0000AC70 File Offset: 0x00008E70
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

		// Token: 0x0600015E RID: 350 RVA: 0x0000AD4C File Offset: 0x00008F4C
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

		// Token: 0x0600015F RID: 351 RVA: 0x0000ADB4 File Offset: 0x00008FB4
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

		// Token: 0x06000160 RID: 352 RVA: 0x0000AE23 File Offset: 0x00009023
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this.Tick();
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000AE32 File Offset: 0x00009032
		protected override void OnIdleTick(float dt)
		{
			base.OnIdleTick(dt);
			this.Tick();
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000AE41 File Offset: 0x00009041
		protected override bool IsEscaped()
		{
			return !this._isConversationInstalled;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000AE4C File Offset: 0x0000904C
		private void Tick()
		{
			if (this._enableDelayFrameCurrentCount != 0)
			{
				this._enableDelayFrameCurrentCount--;
				if (this._enableDelayFrameCurrentCount == 0)
				{
					this.SetEnable(true);
				}
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
					this._barterDataSource.ExecuteCancel();
				}
				else
				{
					if (this.IsReleasedInGauntletLayer("Confirm"))
					{
						BarterVM barterDataSource = this._barterDataSource;
						if (barterDataSource != null && !barterDataSource.IsOfferDisabled)
						{
							this._barterDataSource.ExecuteOffer();
							goto IL_17C;
						}
					}
					if (this.IsReleasedInGauntletLayer("Reset"))
					{
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
			IL_17C:
			BarterItemVM.IsFiveStackModifierActive = this.IsDownInGauntletLayer("FiveStackModifier");
			BarterItemVM.IsEntireStackModifierActive = this.IsDownInGauntletLayer("EntireStackModifier");
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0000AFF5 File Offset: 0x000091F5
		private void SetEnable(bool isEnabled)
		{
			if (this._isEnabled != isEnabled)
			{
				this._layerAsGauntletLayer._twoDimensionView.SetEnable(isEnabled);
				this._dataSource.IsTableauEnabled = isEnabled;
				this._isEnabled = isEnabled;
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0000B024 File Offset: 0x00009224
		protected override void OnMenuModeTick(float dt)
		{
			base.OnMenuModeTick(dt);
			this.Tick();
		}

		// Token: 0x06000166 RID: 358 RVA: 0x0000B034 File Offset: 0x00009234
		protected override void OnMapConversationUpdate(ConversationCharacterData playerConversationData, ConversationCharacterData partnerConversationData)
		{
			base.OnMapConversationUpdate(playerConversationData, partnerConversationData);
			float num = CampaignTime.Now.CurrentHourInDay * 1f;
			bool isSnowTerrainInPos = Campaign.Current.Models.MapWeatherModel.GetIsSnowTerrainInPos(MobileParty.MainParty.GetPosition());
			bool flag = false;
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
				flag = Hero.MainHero.CurrentSettlement != null && (text2 == "lordshall" || text2 == "tavern");
			}
			MapConversationTableauData mapConversationTableauData = MapConversationTableauData.CreateFrom(playerConversationData, partnerConversationData, Campaign.Current.MapSceneWrapper.GetFaceTerrainType(MobileParty.MainParty.CurrentNavigationFace), num, isSnowTerrainInPos, Hero.MainHero.CurrentSettlement, flag);
			if (!GauntletMapConversationView.IsSame(mapConversationTableauData, this._tableauData))
			{
				this._dataSource.TableauData = mapConversationTableauData;
			}
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0000B12B File Offset: 0x0000932B
		private bool IsReleasedInGauntletLayer(string hotKeyID)
		{
			ScreenLayer layer = base.Layer;
			return layer != null && layer.Input.IsHotKeyReleased(hotKeyID);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000B144 File Offset: 0x00009344
		private bool IsDownInGauntletLayer(string hotKeyID)
		{
			ScreenLayer layer = base.Layer;
			return layer != null && layer.Input.IsHotKeyDown(hotKeyID);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000B15D File Offset: 0x0000935D
		private void OnClose()
		{
			MapState mapState = Game.Current.GameStateManager.LastOrDefault<MapState>();
			if (mapState == null)
			{
				return;
			}
			mapState.OnMapConversationOver();
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000B178 File Offset: 0x00009378
		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._dataSource.OnFinalize();
			this._dataSource = null;
			base.MapScreen.RemoveLayer(base.Layer);
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
			BarterManager barter = this._barter;
			barter.BarterBegin = (BarterManager.BarterBeginEventDelegate)Delegate.Remove(barter.BarterBegin, new BarterManager.BarterBeginEventDelegate(this.OnBarterBegin));
			BarterManager barter2 = this._barter;
			barter2.Closed = (BarterManager.BarterCloseEventDelegate)Delegate.Remove(barter2.Closed, new BarterManager.BarterCloseEventDelegate(this.OnBarterClosed));
			base.Layer = null;
			this._barterMovie = null;
			this._dataSource = null;
			BarterVM barterDataSource = this._barterDataSource;
			if (barterDataSource != null)
			{
				barterDataSource.OnFinalize();
			}
			this._barterDataSource = null;
			SpriteCategory conversationCategory = this._conversationCategory;
			if (conversationCategory != null)
			{
				conversationCategory.Unload();
			}
			Campaign.Current.ConversationManager.Handler = null;
			Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000B274 File Offset: 0x00009474
		private string GetContinueKeyText()
		{
			if (Input.IsGamepadActive)
			{
				GameTexts.SetVariable("CONSOLE_KEY_NAME", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("ConversationHotKeyCategory", "ContinueKey")));
				return GameTexts.FindText("str_click_to_continue_console", null).ToString();
			}
			return GameTexts.FindText("str_click_to_continue", null).ToString();
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000B2C8 File Offset: 0x000094C8
		void IConversationStateHandler.OnConversationInstall()
		{
			if (!this._isConversationInstalled)
			{
				this._dataSource = new MapConversationVM(new Action(this.OnContinue), new Func<string>(this.GetContinueKeyText));
				base.Layer = new GauntletLayer(205, "GauntletLayer", false);
				this._layerAsGauntletLayer = base.Layer as GauntletLayer;
				this._layerAsGauntletLayer.LoadMovie("MapConversation", this._dataSource);
				base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
				base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
				base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
				base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
				base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ConversationHotKeyCategory"));
				this._dataSource.TableauData = this._tableauData;
				base.MapScreen.AddLayer(base.Layer);
				base.Layer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(base.Layer);
				this._isConversationInstalled = true;
				this._layerAsGauntletLayer._gauntletUIContext.EventManager.GainNavigationAfterFrames(2, null);
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000B412 File Offset: 0x00009612
		void IConversationStateHandler.OnConversationActivate()
		{
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000B414 File Offset: 0x00009614
		void IConversationStateHandler.OnConversationUninstall()
		{
			if (this._isConversationInstalled)
			{
				this.OnClose();
				this._isConversationInstalled = false;
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000B42B File Offset: 0x0000962B
		void IConversationStateHandler.OnConversationDeactivate()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000B432 File Offset: 0x00009632
		void IConversationStateHandler.OnConversationContinue()
		{
			this._dataSource.DialogController.OnConversationContinue();
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000B444 File Offset: 0x00009644
		void IConversationStateHandler.ExecuteConversationContinue()
		{
			this._dataSource.DialogController.ExecuteContinue();
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000B458 File Offset: 0x00009658
		private static bool IsSame(MapConversationTableauData first, MapConversationTableauData second)
		{
			return first != null && second != null && (GauntletMapConversationView.IsSame(first.PlayerCharacterData, second.PlayerCharacterData) && GauntletMapConversationView.IsSame(first.ConversationPartnerData, second.ConversationPartnerData) && first.ConversationTerrainType == second.ConversationTerrainType && first.IsCurrentTerrainUnderSnow == second.IsCurrentTerrainUnderSnow) && first.TimeOfDay == second.TimeOfDay;
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000B4C0 File Offset: 0x000096C0
		private static bool IsSame(ConversationCharacterData first, ConversationCharacterData second)
		{
			return first.Character == second.Character && first.NoHorse == second.NoHorse && first.NoWeapon == second.NoWeapon && first.Party == second.Party && first.SpawnedAfterFight == second.SpawnedAfterFight && first.IsCivilianEquipmentRequiredForLeader == second.IsCivilianEquipmentRequiredForLeader;
		}

		// Token: 0x040000B2 RID: 178
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x040000B3 RID: 179
		private MapConversationVM _dataSource;

		// Token: 0x040000B4 RID: 180
		private SpriteCategory _conversationCategory;

		// Token: 0x040000B5 RID: 181
		private MapConversationTableauData _tableauData;

		// Token: 0x040000B6 RID: 182
		private bool _isBarterActive;

		// Token: 0x040000B7 RID: 183
		private bool _isConversationInstalled;

		// Token: 0x040000B8 RID: 184
		private const int _enableAfterFrameCount = 5;

		// Token: 0x040000B9 RID: 185
		private int _enableDelayFrameCurrentCount;

		// Token: 0x040000BA RID: 186
		private bool _isEnabled = true;

		// Token: 0x040000BB RID: 187
		private BarterManager _barter;

		// Token: 0x040000BC RID: 188
		private SpriteCategory _barterCategory;

		// Token: 0x040000BD RID: 189
		private BarterVM _barterDataSource;

		// Token: 0x040000BE RID: 190
		private IGauntletMovie _barterMovie;
	}
}
