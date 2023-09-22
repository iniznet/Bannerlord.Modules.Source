using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Helpers;
using SandBox.View.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.View.Scripts;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;

namespace SandBox.View.Map
{
	[GameStateScreen(typeof(MapState))]
	public class MapScreen : ScreenBase, IMapStateHandler, IGameStateListener
	{
		public CampaignMapSiegePrefabEntityCache PrefabEntityCache { get; private set; }

		public MapEncyclopediaView EncyclopediaScreenManager { get; private set; }

		public MapNotificationView MapNotificationView { get; private set; }

		public bool IsInMenu
		{
			get
			{
				return this._menuViewContext != null;
			}
		}

		public bool IsEscapeMenuOpened { get; private set; }

		public PartyVisual CurrentVisualOfTooltip { get; private set; }

		public SceneLayer SceneLayer { get; private set; }

		public IInputContext Input
		{
			get
			{
				return this.SceneLayer.Input;
			}
		}

		public bool IsReady
		{
			get
			{
				return this._isReadyForRender;
			}
		}

		public static MapScreen Instance { get; private set; }

		public bool IsInBattleSimulation { get; private set; }

		public bool IsInTownManagement { get; private set; }

		public bool IsInHideoutTroopManage { get; private set; }

		public bool IsInArmyManagement { get; private set; }

		public bool IsInRecruitment { get; private set; }

		public bool IsBarExtended { get; private set; }

		public bool IsInCampaignOptions { get; private set; }

		public bool IsMarriageOfferPopupActive { get; private set; }

		public bool IsMapCheatsActive { get; private set; }

		public Dictionary<Tuple<Material, BannerCode>, Material> BannerTexturedMaterialCache
		{
			get
			{
				Dictionary<Tuple<Material, BannerCode>, Material> dictionary;
				if ((dictionary = this._bannerTexturedMaterialCache) == null)
				{
					dictionary = (this._bannerTexturedMaterialCache = new Dictionary<Tuple<Material, BannerCode>, Material>());
				}
				return dictionary;
			}
		}

		public bool MapSceneCursorActive
		{
			get
			{
				return this._mapSceneCursorActive;
			}
			set
			{
				if (this._mapSceneCursorActive != value)
				{
					this._mapSceneCursorActive = value;
				}
			}
		}

		public GameEntity ContourMaskEntity { get; private set; }

		public List<Mesh> InactiveLightMeshes { get; private set; }

		public List<Mesh> ActiveLightMeshes { get; private set; }

		public MapScreen(MapState mapState)
		{
			this._mapState = mapState;
			mapState.Handler = this;
			this._periodicCampaignUIEvents = new List<MBCampaignEvent>();
			this.InitializeVisuals();
			CampaignMusicHandler.Create();
			this._mapViews = new ObservableCollection<MapView>();
			this._mapViewsCopyCache = new MapView[0];
			this._mapCameraView = (MapCameraView)this.AddMapView<MapCameraView>(Array.Empty<object>());
			this.MapTracksCampaignBehavior = Campaign.Current.GetCampaignBehavior<IMapTracksCampaignBehavior>();
		}

		public void OnHoverMapEntity(IMapEntity mapEntity)
		{
			uint hashCode = (uint)mapEntity.GetHashCode();
			if (this._tooltipTargetHash != hashCode)
			{
				this._tooltipTargetHash = hashCode;
				this._tooltipTargetObject = null;
				mapEntity.OnHover();
			}
		}

		public void SetupMapTooltipForTrack(Track track)
		{
			if (this._tooltipTargetObject != track)
			{
				this._tooltipTargetObject = track;
				this._tooltipTargetHash = 0U;
				InformationManager.ShowTooltip(typeof(Track), new object[] { track });
			}
		}

		public void RemoveMapTooltip()
		{
			if (this._tooltipTargetObject != null || this._tooltipTargetHash != 0U)
			{
				this._tooltipTargetObject = null;
				this._tooltipTargetHash = 0U;
				MBInformationManager.HideInformations();
			}
		}

		private static void PreloadTextures()
		{
			List<string> list = new List<string>();
			list.Add("gui_map_circle_enemy");
			list.Add("gui_map_circle_enemy_selected");
			list.Add("gui_map_circle_neutral");
			list.Add("gui_map_circle_neutral_selected");
			for (int i = 2; i <= 5; i++)
			{
				list.Add("gui_map_circle_enemy_selected_" + i);
				list.Add("gui_map_circle_neutral_selected_" + i);
			}
			for (int j = 0; j < list.Count; j++)
			{
				Texture.GetFromResource(list[j]).PreloadTexture(false);
			}
			list.Clear();
		}

		private void HandleSiegeEngineHoverEnd()
		{
			if (this._preSelectedSiegeEntityID != UIntPtr.Zero)
			{
				MapScreen.FrameAndVisualOfEngines[this._preSelectedSiegeEntityID].Item2.OnMapHoverSiegeEngineEnd();
				this._preSelectedSiegeEntityID = UIntPtr.Zero;
			}
		}

		private void SetCameraOfSceneLayer()
		{
			this.SceneLayer.SetCamera(this._mapCameraView.Camera);
			Vec3 origin = this._mapCameraView.CameraFrame.origin;
			origin.z = 0f;
			this.SceneLayer.SetFocusedShadowmap(false, ref origin, 0f);
		}

		protected override void OnResume()
		{
			base.OnResume();
			MapScreen.PreloadTextures();
			this._isSoundOn = true;
			this.RestartAmbientSounds();
			if (this._gpuMemoryCleared)
			{
				this._gpuMemoryCleared = false;
			}
			for (int i = this._mapViews.Count - 1; i >= 0; i--)
			{
				this._mapViews[i].OnResume();
			}
			MenuViewContext menuViewContext = this._menuViewContext;
			if (menuViewContext != null)
			{
				menuViewContext.OnResume();
			}
			(Campaign.Current.MapSceneWrapper as MapScene).ValidateAgentVisualsReseted();
		}

		protected override void OnPause()
		{
			base.OnPause();
			MBInformationManager.HideInformations();
			this.PauseAmbientSounds();
			this._isSoundOn = false;
			this._activatedFrameNo = Utilities.EngineFrameNo;
			this.HandleIfSceneIsReady();
			this._conversationOverThisFrame = false;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			this._mapCameraView.OnActivate(this._leftButtonDraggingMode, this._clickedPosition);
			this._activatedFrameNo = Utilities.EngineFrameNo;
			this.HandleIfSceneIsReady();
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(4));
			this.SetCameraOfSceneLayer();
			this.RestartAmbientSounds();
			PartyBase.MainParty.SetVisualAsDirty();
		}

		public void ClearGPUMemory()
		{
			if (true)
			{
				MapScreen.Instance.SceneLayer.ClearRuntimeGPUMemory(true);
			}
			Texture.ReleaseGpuMemories();
			this._gpuMemoryCleared = true;
		}

		protected override void OnDeactivate()
		{
			Game game = Game.Current;
			if (game != null)
			{
				game.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
			}
			this.PauseAmbientSounds();
			MenuViewContext menuViewContext = this._menuViewContext;
			if (menuViewContext != null)
			{
				menuViewContext.StopAllSounds();
			}
			MBInformationManager.HideInformations();
			for (int i = this._mapViews.Count - 1; i >= 0; i--)
			{
				this._mapViews[i].OnDeactivate();
			}
			base.OnDeactivate();
		}

		public override void OnFocusChangeOnGameWindow(bool focusGained)
		{
			base.OnFocusChangeOnGameWindow(focusGained);
			if (!focusGained && BannerlordConfig.StopGameOnFocusLost)
			{
				Func<bool> isAnyInquiryActive = InformationManager.IsAnyInquiryActive;
				if (isAnyInquiryActive != null && !isAnyInquiryActive())
				{
					MapEncyclopediaView encyclopediaScreenManager = this.EncyclopediaScreenManager;
					if (encyclopediaScreenManager == null || !encyclopediaScreenManager.IsEncyclopediaOpen)
					{
						if (this._mapViews.All((MapView m) => m.IsOpeningEscapeMenuOnFocusChangeAllowed()))
						{
							this.OnEscapeMenuToggled(true);
						}
					}
				}
			}
			this._focusLost = !focusGained;
		}

		public MapView AddMapView<T>(params object[] parameters) where T : MapView, new()
		{
			MapView mapView = SandBoxViewCreator.CreateMapView<T>(parameters);
			mapView.MapScreen = this;
			mapView.MapState = this._mapState;
			this._mapViews.Add(mapView);
			mapView.CreateLayout();
			return mapView;
		}

		public T GetMapView<T>() where T : MapView
		{
			foreach (MapView mapView in this._mapViews)
			{
				if (mapView is T)
				{
					return (T)((object)mapView);
				}
			}
			return default(T);
		}

		public void RemoveMapView(MapView mapView)
		{
			mapView.OnFinalize();
			this._mapViews.Remove(mapView);
		}

		public void AddEncounterOverlay(GameOverlays.MenuOverlayType type)
		{
			if (this._encounterOverlay == null)
			{
				this._encounterOverlay = this.AddMapView<MapOverlayView>(new object[] { type });
				for (int i = this._mapViews.Count - 1; i >= 0; i--)
				{
					this._mapViews[i].OnOverlayCreated();
				}
			}
		}

		public void AddArmyOverlay(GameOverlays.MapOverlayType type)
		{
			if (this._armyOverlay == null)
			{
				this._armyOverlay = this.AddMapView<MapOverlayView>(new object[] { type });
				for (int i = this._mapViews.Count - 1; i >= 0; i--)
				{
					this._mapViews[i].OnOverlayCreated();
				}
			}
		}

		public void RemoveEncounterOverlay()
		{
			if (this._encounterOverlay != null)
			{
				this.RemoveMapView(this._encounterOverlay);
				this._encounterOverlay = null;
				for (int i = this._mapViews.Count - 1; i >= 0; i--)
				{
					this._mapViews[i].OnOverlayClosed();
				}
			}
		}

		public void RemoveArmyOverlay()
		{
			if (this._armyOverlay != null)
			{
				this.RemoveMapView(this._armyOverlay);
				this._armyOverlay = null;
				for (int i = this._mapViews.Count - 1; i >= 0; i--)
				{
					this._mapViews[i].OnOverlayClosed();
				}
			}
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			if (MBDebug.TestModeEnabled)
			{
				this.CheckValidityOfItems();
			}
			MapScreen.Instance = this;
			this._mapCameraView.Initialize();
			ViewSubModule.BannerTexturedMaterialCache = this.BannerTexturedMaterialCache;
			this.SceneLayer = new SceneLayer("SceneLayer", true, false);
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("MapHotKeyCategory"));
			base.AddLayer(this.SceneLayer);
			this._mapScene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
			Utilities.SetAllocationAlwaysValidScene(null);
			this.SceneLayer.SetScene(this._mapScene);
			this.SceneLayer.SceneView.SetEnable(false);
			this.SceneLayer.SetSceneUsesShadows(true);
			this.SceneLayer.SetRenderWithPostfx(true);
			this.SceneLayer.SetSceneUsesContour(true);
			this.SceneLayer.SceneView.SetAcceptGlobalDebugRenderObjects(true);
			this.SceneLayer.SceneView.SetResolutionScaling(true);
			this.CollectTickableMapMeshes();
			this.MapNotificationView = this.AddMapView<MapNotificationView>(Array.Empty<object>()) as MapNotificationView;
			this.AddMapView<MapBasicView>(Array.Empty<object>());
			this.AddMapView<MapSettlementNameplateView>(Array.Empty<object>());
			this.AddMapView<MapPartyNameplateView>(Array.Empty<object>());
			this.AddMapView<MapEventVisualsView>(Array.Empty<object>());
			this.AddMapView<MapMobilePartyTrackerView>(Array.Empty<object>());
			this.AddMapView<MapSaveView>(Array.Empty<object>());
			this.AddMapView<MapGamepadEffectsView>(Array.Empty<object>());
			this.EncyclopediaScreenManager = this.AddMapView<MapEncyclopediaView>(Array.Empty<object>()) as MapEncyclopediaView;
			this.AddMapView<MapBarView>(Array.Empty<object>());
			this._mapReadyView = this.AddMapView<MapReadyView>(Array.Empty<object>()) as MapReadyView;
			this._mapReadyView.SetIsMapSceneReady(false);
			this._mouseRay = new Ray(Vec3.Zero, Vec3.Up, float.MaxValue);
			if (PlayerSiege.PlayerSiegeEvent != null)
			{
				if (this != null)
				{
					this.OnPlayerSiegeActivated();
				}
			}
			this.PrefabEntityCache = this.SceneLayer.SceneView.GetScene().GetFirstEntityWithScriptComponent<CampaignMapSiegePrefabEntityCache>().GetFirstScriptOfType<CampaignMapSiegePrefabEntityCache>();
			CampaignEvents.OnSaveOverEvent.AddNonSerializedListener(this, new Action<bool, string>(this.OnSaveOver));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnPartyLeftSettlement));
			CampaignEvents.OnMarriageOfferedToPlayerEvent.AddNonSerializedListener(this, new Action<Hero, Hero>(this.OnMarriageOfferedToPlayer));
			CampaignEvents.OnMarriageOfferCanceledEvent.AddNonSerializedListener(this, new Action<Hero, Hero>(this.OnMarriageOfferCanceled));
			GameEntity firstEntityWithScriptComponent = this._mapScene.GetFirstEntityWithScriptComponent<MapColorGradeManager>();
			if (firstEntityWithScriptComponent != null)
			{
				this._colorGradeManager = firstEntityWithScriptComponent.GetFirstScriptOfType<MapColorGradeManager>();
			}
		}

		private void OnPartyLeftSettlement(MobileParty party, Settlement settlement)
		{
			if (party == MobileParty.MainParty)
			{
				this.UpdateMenuView();
			}
		}

		private void OnSaveOver(bool isSuccessful, string newSaveGameName)
		{
			if (this._exitOnSaveOver)
			{
				if (isSuccessful)
				{
					this.OnExit();
				}
				this._exitOnSaveOver = false;
			}
		}

		private void OnMarriageOfferedToPlayer(Hero suitor, Hero maiden)
		{
			this._marriageOfferPopupView = this.AddMapView<MarriageOfferPopupView>(new object[] { suitor, maiden });
		}

		private void OnMarriageOfferCanceled(Hero suitor, Hero maiden)
		{
			if (this._marriageOfferPopupView != null)
			{
				this.RemoveMapView(this._marriageOfferPopupView);
				this._marriageOfferPopupView = null;
			}
		}

		protected override void OnFinalize()
		{
			for (int i = this._mapViews.Count - 1; i >= 0; i--)
			{
				this._mapViews[i].OnFinalize();
			}
			PartyVisualManager.Current.OnFinalized();
			base.OnFinalize();
			if (this._mapScene != null)
			{
				this._mapScene.ClearAll();
			}
			Common.MemoryCleanupGC(false);
			this._characterBannerMaterialCache.Clear();
			this._characterBannerMaterialCache = null;
			ViewSubModule.BannerTexturedMaterialCache = null;
			MBMusicManager.Current.DeactivateCampaignMode();
			MBMusicManager.Current.OnCampaignMusicHandlerFinalize();
			CampaignEvents.OnSaveOverEvent.ClearListeners(this);
			CampaignEvents.OnSettlementLeftEvent.ClearListeners(this);
			CampaignEvents.OnMarriageOfferedToPlayerEvent.ClearListeners(this);
			CampaignEvents.OnMarriageOfferCanceledEvent.ClearListeners(this);
			this._mapScene = null;
			this._campaign = null;
			this._navigationHandler = null;
			this._mapCameraView = null;
			MapScreen.Instance = null;
		}

		public void OnHourlyTick()
		{
			for (int i = this._mapViews.Count - 1; i >= 0; i--)
			{
				this._mapViews[i].OnHourlyTick();
			}
			Kingdom kingdom = Clan.PlayerClan.Kingdom;
			object obj;
			if (kingdom == null)
			{
				obj = null;
			}
			else
			{
				obj = kingdom.UnresolvedDecisions.FirstOrDefault((KingdomDecision d) => d.NotifyPlayer && d.IsEnforced && d.IsPlayerParticipant && !d.ShouldBeCancelled());
			}
			this._isKingdomDecisionsDirty = obj != null;
		}

		private void OnRenderingStateChanged(bool startedRendering)
		{
			if (startedRendering && this._isSceneViewEnabled && this._conversationDataCache != null)
			{
				Campaign.Current.ConversationManager.Handler = null;
				Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
				this.HandleMapConversationInit(this._conversationDataCache.Item1, this._conversationDataCache.Item2);
				this._conversationDataCache.Item3.ApplyHandlerChangesTo(this._mapConversationView as IConversationStateHandler);
				this._conversationDataCache = null;
			}
		}

		private void ShowNextKingdomDecisionPopup()
		{
			Kingdom kingdom = Clan.PlayerClan.Kingdom;
			KingdomDecision kingdomDecision;
			if (kingdom == null)
			{
				kingdomDecision = null;
			}
			else
			{
				kingdomDecision = kingdom.UnresolvedDecisions.FirstOrDefault((KingdomDecision d) => d.NotifyPlayer && d.IsEnforced && d.IsPlayerParticipant && !d.ShouldBeCancelled());
			}
			KingdomDecision kingdomDecision2 = kingdomDecision;
			if (kingdomDecision2 != null)
			{
				InquiryData inquiryData = new InquiryData(new TextObject("{=A7349NHy}Critical Kingdom Decision", null).ToString(), kingdomDecision2.GetChooseTitle().ToString(), true, false, new TextObject("{=bFzZwwjT}Examine", null).ToString(), "", delegate
				{
					this.OpenKingdom();
				}, null, "", 0f, null, null, null);
				kingdomDecision2.NotifyPlayer = false;
				InformationManager.ShowInquiry(inquiryData, true, false);
				this._isKingdomDecisionsDirty = false;
				return;
			}
			Debug.FailedAssert("There is no dirty decision but still demanded one", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Map\\MapScreen.cs", "ShowNextKingdomDecisionPopup", 760);
		}

		void IMapStateHandler.OnMenuModeTick(float dt)
		{
			for (int i = this._mapViews.Count - 1; i >= 0; i--)
			{
				this._mapViews[i].OnMenuModeTick(dt);
			}
		}

		private void HandleIfBlockerStatesDisabled()
		{
			bool isReadyForRender = this._isReadyForRender;
			bool flag = this.SceneLayer.SceneView.ReadyToRender() && this.SceneLayer.SceneView.CheckSceneReadyToRender();
			bool flag2 = (this._isSceneViewEnabled || this._mapConversationView != null) && flag;
			if (LoadingWindow.IsLoadingWindowActive && flag2)
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}
			this._mapReadyView.SetIsMapSceneReady(flag2);
			this._isReadyForRender = flag2;
			if (isReadyForRender != this._isReadyForRender)
			{
				this.OnRenderingStateChanged(this._isReadyForRender);
			}
		}

		private void CheckCursorState()
		{
			Vec3 zero = Vec3.Zero;
			Vec3 zero2 = Vec3.Zero;
			this.SceneLayer.SceneView.TranslateMouse(ref zero, ref zero2, -1f);
			Vec3 vec = zero;
			Vec3 vec2 = zero2;
			PathFaceRecord nullFaceRecord = PathFaceRecord.NullFaceRecord;
			float num;
			Vec3 vec3;
			this.GetCursorIntersectionPoint(ref vec, ref vec2, out num, out vec3, ref nullFaceRecord, 79617);
			bool flag = Campaign.Current.MapSceneWrapper.AreFacesOnSameIsland(nullFaceRecord, MobileParty.MainParty.CurrentNavigationFace, false);
			this.SceneLayer.ActiveCursor = (flag ? 1 : 10);
		}

		private void HandleIfSceneIsReady()
		{
			int num = Utilities.EngineFrameNo - this._activatedFrameNo;
			bool flag = this._isSceneViewEnabled;
			if (num < 5)
			{
				flag = false;
				MapColorGradeManager colorGradeManager = this._colorGradeManager;
				if (colorGradeManager != null)
				{
					colorGradeManager.ApplyAtmosphere(true);
				}
			}
			else
			{
				int num2 = ((this._mapConversationView != null) ? 1 : 0);
				bool flag2 = ScreenManager.TopScreen == this;
				flag = num2 == 0 && flag2;
			}
			if (flag != this._isSceneViewEnabled)
			{
				this._isSceneViewEnabled = flag;
				this.SceneLayer.SceneView.SetEnable(this._isSceneViewEnabled);
				if (this._isSceneViewEnabled)
				{
					this._mapScene.CheckResources();
					if (this._focusLost && !this.IsEscapeMenuOpened)
					{
						this.OnFocusChangeOnGameWindow(false);
					}
				}
			}
			this.HandleIfBlockerStatesDisabled();
		}

		void IMapStateHandler.StartCameraAnimation(Vec2 targetPosition, float animationStopDuration)
		{
			this._mapCameraView.StartCameraAnimation(targetPosition, animationStopDuration);
		}

		void IMapStateHandler.BeforeTick(float dt)
		{
			this.HandleIfSceneIsReady();
			bool flag = MobileParty.MainParty != null && PartyBase.MainParty.IsValid;
			if (flag && !this._mapCameraView.CameraAnimationInProgress)
			{
				if (!this.IsInMenu && this.SceneLayer.Input.IsHotKeyPressed("MapChangeCursorMode"))
				{
					this._mapSceneCursorWanted = !this._mapSceneCursorWanted;
				}
				if (this.SceneLayer.Input.IsHotKeyPressed("MapClick"))
				{
					this._secondLastPressTime = this._lastPressTime;
					this._lastPressTime = (double)Time.ApplicationTime;
				}
				this._leftButtonDoubleClickOnSceneWidget = false;
				if (this.SceneLayer.Input.IsHotKeyReleased("MapClick"))
				{
					Vec2 mousePositionPixel = this.SceneLayer.Input.GetMousePositionPixel();
					float applicationTime = Time.ApplicationTime;
					this._leftButtonDoubleClickOnSceneWidget = (double)applicationTime - this._lastReleaseTime < 0.30000001192092896 && (double)applicationTime - this._secondLastPressTime < 0.44999998807907104 && mousePositionPixel.Distance(this._oldMousePosition) < 10f;
					if (this._leftButtonDoubleClickOnSceneWidget)
					{
						this._waitForDoubleClickUntilTime = 0f;
					}
					this._oldMousePosition = this.SceneLayer.Input.GetMousePositionPixel();
					this._lastReleaseTime = (double)applicationTime;
				}
				if (this.IsReady)
				{
					this.HandleMouse(dt);
				}
			}
			float deltaMouseScroll = this.SceneLayer.Input.GetDeltaMouseScroll();
			Vec3 zero = Vec3.Zero;
			Vec3 zero2 = Vec3.Zero;
			this.SceneLayer.SceneView.TranslateMouse(ref zero, ref zero2, -1f);
			float gameKeyAxis = this.SceneLayer.Input.GetGameKeyAxis("CameraAxisX");
			float num;
			Vec3 vec;
			bool flag2 = this._mapScene.RayCastForClosestEntityOrTerrain(zero, zero2, ref num, ref vec, 0.01f, 6404041);
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 1f;
			bool flag3 = !TaleWorlds.InputSystem.Input.IsGamepadActive && !this.IsInMenu && ScreenManager.FocusedLayer == this.SceneLayer;
			bool flag4 = TaleWorlds.InputSystem.Input.IsGamepadActive && this.MapSceneCursorActive;
			if (flag3 || flag4)
			{
				if (this.SceneLayer.Input.IsGameKeyDown(54))
				{
					num4 = this._mapCameraView.CameraFastMoveMultiplier;
				}
				num2 = this.SceneLayer.Input.GetGameKeyAxis("MapMovementAxisX") * num4;
				num3 = this.SceneLayer.Input.GetGameKeyAxis("MapMovementAxisY") * num4;
			}
			this._ignoreLeftMouseRelease = false;
			if (this.SceneLayer.Input.IsKeyPressed(224))
			{
				this._clickedPositionPixel = this.SceneLayer.Input.GetMousePositionPixel();
				this._mapScene.RayCastForClosestEntityOrTerrain(this._mouseRay.Origin, this._mouseRay.EndPoint, ref num, ref this._clickedPosition, 0.01f, 6404041);
				if (this.CurrentVisualOfTooltip != null)
				{
					this.RemoveMapTooltip();
				}
				this._leftButtonDraggingMode = false;
			}
			else if (this.SceneLayer.Input.IsKeyDown(224) && !this.SceneLayer.Input.IsKeyReleased(224) && (this.SceneLayer.Input.GetMousePositionPixel().DistanceSquared(this._clickedPositionPixel) > 300f || this._leftButtonDraggingMode) && !this.IsInMenu)
			{
				this._leftButtonDraggingMode = true;
			}
			else if (this._leftButtonDraggingMode)
			{
				this._leftButtonDraggingMode = false;
				this._ignoreLeftMouseRelease = true;
			}
			if (this.SceneLayer.Input.IsKeyDown(226))
			{
				MBWindowManager.DontChangeCursorPos();
			}
			if (this.SceneLayer.Input.IsKeyReleased(224))
			{
				this._clickedPositionPixel = this.SceneLayer.Input.GetMousePositionPixel();
			}
			this.MapSceneCursorActive = !this.SceneLayer.Input.GetIsMouseActive() && !this.IsInMenu && ScreenManager.FocusedLayer == this.SceneLayer && this._mapSceneCursorWanted;
			MapCameraView.InputInformation inputInformation;
			inputInformation.IsMainPartyValid = flag;
			inputInformation.IsMapReady = this.IsReady;
			inputInformation.IsControlDown = this.SceneLayer.Input.IsControlDown();
			inputInformation.IsMouseActive = this.SceneLayer.Input.GetIsMouseActive();
			inputInformation.CheatModeEnabled = Game.Current.CheatMode;
			inputInformation.DeltaMouseScroll = deltaMouseScroll;
			inputInformation.LeftMouseButtonPressed = this.SceneLayer.Input.IsKeyPressed(224);
			inputInformation.LeftMouseButtonDown = this.SceneLayer.Input.IsKeyDown(224);
			inputInformation.LeftMouseButtonReleased = this.SceneLayer.Input.IsKeyReleased(224);
			inputInformation.MiddleMouseButtonDown = this.SceneLayer.Input.IsKeyDown(226);
			inputInformation.RightMouseButtonDown = this.SceneLayer.Input.IsKeyDown(225);
			inputInformation.RotateLeftKeyDown = this.SceneLayer.Input.IsGameKeyDown(57);
			inputInformation.RotateRightKeyDown = this.SceneLayer.Input.IsGameKeyDown(58);
			inputInformation.PartyMoveUpKey = this.SceneLayer.Input.IsGameKeyDown(49);
			inputInformation.PartyMoveDownKey = this.SceneLayer.Input.IsGameKeyDown(50);
			inputInformation.PartyMoveLeftKey = this.SceneLayer.Input.IsGameKeyDown(52);
			inputInformation.PartyMoveRightKey = this.SceneLayer.Input.IsGameKeyDown(51);
			inputInformation.MapZoomIn = this.SceneLayer.Input.GetGameKeyState(55);
			inputInformation.MapZoomOut = this.SceneLayer.Input.GetGameKeyState(56);
			inputInformation.CameraFollowModeKeyPressed = this.SceneLayer.Input.IsGameKeyPressed(63);
			inputInformation.MousePositionPixel = this.SceneLayer.Input.GetMousePositionPixel();
			inputInformation.ClickedPositionPixel = this._clickedPositionPixel;
			inputInformation.ClickedPosition = this._clickedPosition;
			inputInformation.LeftButtonDraggingMode = this._leftButtonDraggingMode;
			inputInformation.IsInMenu = this.IsInMenu;
			inputInformation.WorldMouseNear = zero;
			inputInformation.WorldMouseFar = zero2;
			inputInformation.MouseSensitivity = this.SceneLayer.Input.GetMouseSensitivity();
			inputInformation.MouseMoveX = this.SceneLayer.Input.GetMouseMoveX();
			inputInformation.MouseMoveY = this.SceneLayer.Input.GetMouseMoveY();
			inputInformation.HorizontalCameraInput = gameKeyAxis;
			inputInformation.RayCastForClosestEntityOrTerrainCondition = flag2;
			inputInformation.ProjectedPosition = vec;
			inputInformation.RX = num2;
			inputInformation.RY = num3;
			inputInformation.RS = num4;
			inputInformation.Dt = dt;
			this._mapCameraView.OnBeforeTick(inputInformation);
			this._mapCursor.SetVisible(this.MapSceneCursorActive);
			if (flag && !this._campaign.TimeControlModeLock)
			{
				if (this._mapState.AtMenu)
				{
					if (Campaign.Current.CurrentMenuContext == null)
					{
						goto IL_9AD;
					}
					GameMenu gameMenu = Campaign.Current.CurrentMenuContext.GameMenu;
					if (gameMenu == null || !gameMenu.IsWaitActive)
					{
						goto IL_9AD;
					}
				}
				float applicationTime2 = Time.ApplicationTime;
				if (this.SceneLayer.Input.IsGameKeyPressed(62) && this._timeToggleTimer == 3.4028235E+38f)
				{
					this._timeToggleTimer = applicationTime2;
				}
				if (this.SceneLayer.Input.IsGameKeyPressed(62) && applicationTime2 - this._timeToggleTimer > 0.4f)
				{
					if (this._campaign.TimeControlMode == 3 || this._campaign.TimeControlMode == 1)
					{
						this._campaign.SetTimeSpeed(2);
					}
					else if (this._campaign.TimeControlMode == 4 || this._campaign.TimeControlMode == 2)
					{
						this._campaign.SetTimeSpeed(1);
					}
					else if (this._campaign.TimeControlMode == null)
					{
						this._campaign.SetTimeSpeed(1);
					}
					else if (this._campaign.TimeControlMode == 6)
					{
						this._campaign.SetTimeSpeed(2);
					}
					this._timeToggleTimer = float.MaxValue;
					this._ignoreNextTimeToggle = true;
				}
				else if (this.SceneLayer.Input.IsGameKeyPressed(62))
				{
					if (this._ignoreNextTimeToggle)
					{
						this._ignoreNextTimeToggle = false;
					}
					else
					{
						this._waitForDoubleClickUntilTime = 0f;
						if (this._campaign.TimeControlMode == 2 || this._campaign.TimeControlMode == 1 || ((this._campaign.TimeControlMode == 4 || this._campaign.TimeControlMode == 3) && !this._campaign.IsMainPartyWaiting))
						{
							this._campaign.SetTimeSpeed(0);
						}
						else if (this._campaign.TimeControlMode == null || this._campaign.TimeControlMode == 3)
						{
							this._campaign.SetTimeSpeed(1);
						}
						else if (this._campaign.TimeControlMode == 6 || this._campaign.TimeControlMode == 4)
						{
							this._campaign.SetTimeSpeed(2);
						}
					}
					this._timeToggleTimer = float.MaxValue;
				}
				else if (this.SceneLayer.Input.IsGameKeyPressed(59))
				{
					this._waitForDoubleClickUntilTime = 0f;
					this._campaign.SetTimeSpeed(0);
				}
				else if (this.SceneLayer.Input.IsGameKeyPressed(60))
				{
					this._waitForDoubleClickUntilTime = 0f;
					this._campaign.SetTimeSpeed(1);
				}
				else if (this.SceneLayer.Input.IsGameKeyPressed(61))
				{
					this._waitForDoubleClickUntilTime = 0f;
					this._campaign.SetTimeSpeed(2);
				}
				else if (this.SceneLayer.Input.IsGameKeyPressed(64))
				{
					if (this._campaign.TimeControlMode == 2 || this._campaign.TimeControlMode == 4)
					{
						this._campaign.SetTimeSpeed(0);
					}
					else
					{
						this._campaign.SetTimeSpeed(2);
					}
				}
			}
			IL_9AD:
			if (!flag && this.CurrentVisualOfTooltip != null)
			{
				this.CurrentVisualOfTooltip = null;
				this.RemoveMapTooltip();
			}
			this.SetCameraOfSceneLayer();
			if (!this.SceneLayer.Input.GetIsMouseActive() && Campaign.Current.GameStarted)
			{
				this._mapCursor.BeforeTick(dt);
			}
		}

		void IMapStateHandler.Tick(float dt)
		{
			if (this._mapViewsCopyCache.Length != this._mapViews.Count || !this._mapViewsCopyCache.SequenceEqual(this._mapViews))
			{
				this._mapViewsCopyCache = new MapView[this._mapViews.Count];
				this._mapViews.CopyTo(this._mapViewsCopyCache, 0);
			}
			if (!this.IsInMenu)
			{
				if (this._isKingdomDecisionsDirty)
				{
					this.ShowNextKingdomDecisionPopup();
				}
				else
				{
					if (ViewModel.UIDebugMode && base.DebugInput.IsHotKeyDown("UIExtendedDebugKey") && base.DebugInput.IsHotKeyPressed("MapScreenHotkeyOpenEncyclopedia"))
					{
						this.OpenEncyclopedia();
					}
					bool cheatMode = Game.Current.CheatMode;
					if (cheatMode && base.DebugInput.IsHotKeyPressed("MapScreenHotkeySwitchCampaignTrueSight"))
					{
						this._campaign.TrueSight = !this._campaign.TrueSight;
					}
					if (cheatMode)
					{
						base.DebugInput.IsHotKeyPressed("MapScreenPrintMultiLineText");
					}
					for (int i = this._mapViewsCopyCache.Length - 1; i >= 0; i--)
					{
						if (!this._mapViewsCopyCache[i].IsFinalized)
						{
							this._mapViewsCopyCache[i].OnFrameTick(dt);
						}
					}
				}
			}
			this._conversationOverThisFrame = false;
		}

		void IMapStateHandler.OnIdleTick(float dt)
		{
			this.HandleIfSceneIsReady();
			this.RemoveMapTooltip();
			if (this._mapViewsCopyCache.Length != this._mapViews.Count || !this._mapViewsCopyCache.SequenceEqual(this._mapViews))
			{
				this._mapViewsCopyCache = new MapView[this._mapViews.Count];
				this._mapViews.CopyTo(this._mapViewsCopyCache, 0);
			}
			for (int i = this._mapViewsCopyCache.Length - 1; i >= 0; i--)
			{
				if (!this._mapViewsCopyCache[i].IsFinalized)
				{
					this._mapViewsCopyCache[i].OnIdleTick(dt);
				}
			}
			this._conversationOverThisFrame = false;
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			MBDebug.SetErrorReportScene(this._mapScene);
			this.UpdateMenuView();
			if (this.IsInMenu)
			{
				this._menuViewContext.OnFrameTick(dt);
				if (this.SceneLayer.Input.IsGameKeyPressed(4))
				{
					GameMenuOption leaveMenuOption = Campaign.Current.GameMenuManager.GetLeaveMenuOption(this._menuViewContext.MenuContext);
					if (leaveMenuOption != null)
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						if (this._menuViewContext.MenuContext.GameMenu.IsWaitMenu)
						{
							this._menuViewContext.MenuContext.GameMenu.EndWait();
						}
						leaveMenuOption.RunConsequence(this._menuViewContext.MenuContext);
					}
				}
			}
			else if (Campaign.Current != null && !this.IsInBattleSimulation && !this.IsInArmyManagement && !this.IsMarriageOfferPopupActive && !this.IsMapCheatsActive)
			{
				Kingdom kingdom = Clan.PlayerClan.Kingdom;
				bool flag;
				if (kingdom == null)
				{
					flag = null != null;
				}
				else
				{
					MBReadOnlyList<KingdomDecision> unresolvedDecisions = kingdom.UnresolvedDecisions;
					if (unresolvedDecisions == null)
					{
						flag = null != null;
					}
					else
					{
						flag = unresolvedDecisions.FirstOrDefault((KingdomDecision d) => d.NeedsPlayerResolution && !d.ShouldBeCancelled()) != null;
					}
				}
				if (flag)
				{
					this.OpenKingdom();
				}
			}
			if (this._partyIconNeedsRefreshing)
			{
				this._partyIconNeedsRefreshing = false;
				PartyBase.MainParty.SetVisualAsDirty();
			}
			for (int i = this._mapViews.Count - 1; i >= 0; i--)
			{
				this._mapViews[i].OnMapScreenUpdate(dt);
			}
			this.RefreshMapSiegeOverlayRequired();
			if (PlayerSiege.PlayerSiegeEvent != null && this._playerSiegeMachineSlotMeshesAdded)
			{
				this.TickSiegeMachineCircles();
			}
			this._timeSinceCreation += dt;
		}

		private void UpdateMenuView()
		{
			if (this._latestMenuContext == null && this.IsInMenu)
			{
				this.ExitMenuContext();
				return;
			}
			if ((!this.IsInMenu && this._latestMenuContext != null) || (this.IsInMenu && this._menuViewContext.MenuContext != this._latestMenuContext))
			{
				this.EnterMenuContext(this._latestMenuContext);
			}
		}

		private void EnterMenuContext(MenuContext menuContext)
		{
			this._mapCameraView.SetCameraMode(MapCameraView.CameraFollowMode.FollowParty);
			Campaign.Current.CameraFollowParty = PartyBase.MainParty;
			if (!this.IsInMenu)
			{
				this._menuViewContext = new MenuViewContext(this, menuContext);
			}
			else
			{
				this._menuViewContext.UpdateMenuContext(menuContext);
			}
			this._menuViewContext.OnInitialize();
			this._menuViewContext.OnActivate();
			if (this._mapConversationView != null)
			{
				this._menuViewContext.OnMapConversationActivated();
			}
		}

		private void ExitMenuContext()
		{
			this._menuViewContext.OnGameStateDeactivate();
			this._menuViewContext.OnDeactivate();
			this._menuViewContext.OnFinalize();
			this._menuViewContext = null;
		}

		private void OpenBannerEditorScreen()
		{
			if (Campaign.Current.IsBannerEditorEnabled)
			{
				this._partyIconNeedsRefreshing = true;
				Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<BannerEditorState>(), 0);
			}
		}

		private void OpenFaceGeneratorScreen()
		{
			if (Campaign.Current.IsFaceGenEnabled)
			{
				IFaceGeneratorCustomFilter faceGeneratorFilter = CharacterHelper.GetFaceGeneratorFilter();
				BarberState barberState = Game.Current.GameStateManager.CreateState<BarberState>(new object[]
				{
					Hero.MainHero.CharacterObject,
					faceGeneratorFilter
				});
				GameStateManager.Current.PushState(barberState, 0);
			}
		}

		public void OnExit()
		{
			this._mapCameraView.OnExit();
			MBGameManager.EndGame();
		}

		private void SetMapSiegeOverlayState(bool isActive)
		{
			this._mapCameraView.OnSetMapSiegeOverlayState(isActive, this._mapSiegeOverlayView == null);
			if (this._mapSiegeOverlayView != null && !isActive)
			{
				this.RemoveMapView(this._mapSiegeOverlayView);
				this._mapSiegeOverlayView = null;
				return;
			}
			if (this._mapSiegeOverlayView == null && isActive && PlayerSiege.PlayerSiegeEvent != null)
			{
				this._mapSiegeOverlayView = this.AddMapView<MapSiegeOverlayView>(Array.Empty<object>());
				if (!this._playerSiegeMachineSlotMeshesAdded)
				{
					this.InitializeSiegeCircleVisuals();
					this._playerSiegeMachineSlotMeshesAdded = true;
				}
			}
		}

		private void RefreshMapSiegeOverlayRequired()
		{
			this._mapCameraView.OnRefreshMapSiegeOverlayRequired(this._mapSiegeOverlayView == null);
			if (PlayerSiege.PlayerSiegeEvent == null && this._mapSiegeOverlayView != null)
			{
				this.RemoveMapView(this._mapSiegeOverlayView);
				this._mapSiegeOverlayView = null;
				if (this._playerSiegeMachineSlotMeshesAdded)
				{
					this.RemoveSiegeCircleVisuals();
					this._playerSiegeMachineSlotMeshesAdded = false;
					return;
				}
			}
			else if (PlayerSiege.PlayerSiegeEvent != null && this._mapSiegeOverlayView == null)
			{
				this._mapSiegeOverlayView = this.AddMapView<MapSiegeOverlayView>(Array.Empty<object>());
				if (!this._playerSiegeMachineSlotMeshesAdded)
				{
					this.InitializeSiegeCircleVisuals();
					this._playerSiegeMachineSlotMeshesAdded = true;
				}
			}
		}

		private void OnEscapeMenuToggled(bool isOpened = false)
		{
			this._mapCameraView.OnEscapeMenuToggled(isOpened);
			if (this.IsEscapeMenuOpened == isOpened)
			{
				return;
			}
			this.IsEscapeMenuOpened = isOpened;
			if (isOpened)
			{
				List<EscapeMenuItemVM> escapeMenuItems = this.GetEscapeMenuItems();
				Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
				this._escapeMenuView = this.AddMapView<MapEscapeMenuView>(new object[] { escapeMenuItems });
				return;
			}
			this.RemoveMapView(this._escapeMenuView);
			this._escapeMenuView = null;
			Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
		}

		private void CheckValidityOfItems()
		{
			foreach (ItemObject itemObject in MBObjectManager.Instance.GetObjectTypeList<ItemObject>())
			{
				if (itemObject.IsUsingTeamColor)
				{
					MetaMesh copy = MetaMesh.GetCopy(itemObject.MultiMeshName, false, false);
					for (int i = 0; i < copy.MeshCount; i++)
					{
						Material material = copy.GetMeshAtIndex(i).GetMaterial();
						if (material.Name != "vertex_color_lighting_skinned" && material.Name != "vertex_color_lighting" && material.GetTexture(1) == null)
						{
							MBDebug.ShowWarning("Item object(" + itemObject.Name + ") has 'Using Team Color' flag but does not have a mask texture in diffuse2 slot. ");
							break;
						}
					}
				}
			}
		}

		public void GetCursorIntersectionPoint(ref Vec3 clippedMouseNear, ref Vec3 clippedMouseFar, out float closestDistanceSquared, out Vec3 intersectionPoint, ref PathFaceRecord currentFace, BodyFlags excludedBodyFlags = 79617)
		{
			(clippedMouseFar - clippedMouseNear).Normalize();
			Vec3 vec = clippedMouseFar - clippedMouseNear;
			float num = vec.Normalize();
			this._mouseRay.Reset(clippedMouseNear, vec, num);
			intersectionPoint = Vec3.Zero;
			closestDistanceSquared = 1E+12f;
			float num2;
			Vec3 vec2;
			if (this.SceneLayer.SceneView.RayCastForClosestEntityOrTerrain(clippedMouseNear, clippedMouseFar, ref num2, ref vec2, 0.01f, excludedBodyFlags))
			{
				closestDistanceSquared = num2 * num2;
				intersectionPoint = clippedMouseNear + vec * num2;
			}
			currentFace = Campaign.Current.MapSceneWrapper.GetFaceIndex(intersectionPoint.AsVec2);
		}

		public void FastMoveCameraToPosition(Vec2 target)
		{
			this._mapCameraView.FastMoveCameraToPosition(target, this.IsInMenu);
		}

		private void HandleMouse(float dt)
		{
			if (Campaign.Current.GameStarted)
			{
				Track track = null;
				Vec3 zero = Vec3.Zero;
				Vec3 zero2 = Vec3.Zero;
				this.SceneLayer.SceneView.TranslateMouse(ref zero, ref zero2, -1f);
				Vec3 vec = zero;
				Vec3 vec2 = zero2;
				PathFaceRecord nullFaceRecord = PathFaceRecord.NullFaceRecord;
				this.CheckCursorState();
				float num;
				Vec3 vec3;
				this.GetCursorIntersectionPoint(ref vec, ref vec2, out num, out vec3, ref nullFaceRecord, 79617);
				float num2;
				Vec3 vec4;
				this.GetCursorIntersectionPoint(ref vec, ref vec2, out num2, out vec4, ref nullFaceRecord, 79633);
				int num3 = this._mapScene.SelectEntitiesCollidedWith(ref this._mouseRay, this._intersectionInfos, this._intersectedEntityIDs);
				bool flag = false;
				float num4 = MathF.Sqrt(num) + 1f;
				float num5 = num4;
				PartyVisual partyVisual = null;
				PartyVisual partyVisual2 = null;
				bool flag2 = false;
				for (int i = num3 - 1; i >= 0; i--)
				{
					UIntPtr uintPtr = this._intersectedEntityIDs[i];
					if (uintPtr != UIntPtr.Zero)
					{
						PartyVisual partyVisual3;
						if (MapScreen.VisualsOfEntities.TryGetValue(uintPtr, out partyVisual3) && partyVisual3.IsVisibleOrFadingOut())
						{
							PartyVisual partyVisual4 = partyVisual3;
							Intersection intersection = this._intersectionInfos[i];
							vec3 = zero - intersection.IntersectionPoint;
							float num6 = vec3.Length;
							if (partyVisual4.PartyBase.IsMobile)
							{
								num6 -= 1.5f;
							}
							if (num6 < num5)
							{
								num5 = num6;
								if (!partyVisual4.PartyBase.IsMobile || partyVisual4.PartyBase.MobileParty.AttachedTo == null)
								{
									partyVisual = partyVisual3;
								}
								else
								{
									partyVisual = PartyVisualManager.Current.GetVisualOfParty(partyVisual4.PartyBase.MobileParty.AttachedTo.Party);
								}
								flag = true;
							}
							if (num6 < num4 && (!partyVisual4.PartyBase.IsMobile || (partyVisual4.PartyBase != PartyBase.MainParty && (partyVisual4.PartyBase.MobileParty.AttachedTo == null || partyVisual4.PartyBase.MobileParty.AttachedTo != MobileParty.MainParty))))
							{
								num4 = num6;
								if (partyVisual4.PartyBase.IsMobile && partyVisual4.PartyBase.MobileParty.AttachedTo != null)
								{
									partyVisual2 = PartyVisualManager.Current.GetVisualOfParty(partyVisual4.PartyBase.MobileParty.AttachedTo.Party);
								}
								else
								{
									partyVisual2 = partyVisual4;
								}
							}
						}
						else if (ScreenManager.FirstHitLayer == this.SceneLayer && MapScreen.FrameAndVisualOfEngines.ContainsKey(uintPtr))
						{
							flag2 = true;
							if (this._preSelectedSiegeEntityID != uintPtr)
							{
								Tuple<MatrixFrame, PartyVisual> tuple = MapScreen.FrameAndVisualOfEngines[uintPtr];
								tuple.Item2.OnMapHoverSiegeEngine(tuple.Item1);
								this._preSelectedSiegeEntityID = uintPtr;
							}
						}
					}
				}
				if (!flag2)
				{
					this.HandleSiegeEngineHoverEnd();
				}
				Array.Clear(this._intersectedEntityIDs, 0, num3);
				Array.Clear(this._intersectionInfos, 0, num3);
				if (flag)
				{
					if (this._displayedContextMenuType < 0)
					{
						this.SceneLayer.ActiveCursor = 1;
					}
				}
				else
				{
					track = this._campaign.GetEntityComponent<MapTracksVisual>().GetTrackOnMouse(this._mouseRay, vec4);
				}
				float gameKeyAxis = this.SceneLayer.Input.GetGameKeyAxis("CameraAxisY");
				this._mapCameraView.HandleMouse(this.SceneLayer.Input.IsKeyDown(225), gameKeyAxis, this.SceneLayer.Input.GetMouseMoveY(), dt);
				if (this.SceneLayer.Input.IsKeyDown(225))
				{
					MBWindowManager.DontChangeCursorPos();
				}
				if (ScreenManager.FirstHitLayer == this.SceneLayer && this.SceneLayer.Input.IsHotKeyReleased("MapClick") && !this._leftButtonDraggingMode && !this._ignoreLeftMouseRelease)
				{
					if (this._leftButtonDoubleClickOnSceneWidget)
					{
						this.HandleLeftMouseButtonClick(this._preSelectedSiegeEntityID, this._preVisualOfSelectedEntity, vec4, nullFaceRecord);
					}
					else
					{
						this.HandleLeftMouseButtonClick(this._preSelectedSiegeEntityID, partyVisual2, vec4, nullFaceRecord);
						this._preVisualOfSelectedEntity = partyVisual2;
					}
				}
				if (Campaign.Current.TimeControlMode == 4 && this._waitForDoubleClickUntilTime > 0f && this._waitForDoubleClickUntilTime < Time.ApplicationTime)
				{
					Campaign.Current.TimeControlMode = 3;
					this._waitForDoubleClickUntilTime = 0f;
				}
				if (ScreenManager.FirstHitLayer == this.SceneLayer)
				{
					if (partyVisual != null)
					{
						if (this.CurrentVisualOfTooltip != partyVisual)
						{
							this.RemoveMapTooltip();
						}
						IMapEntity mapEntity = partyVisual.GetMapEntity();
						if (this.SceneLayer.Input.IsGameKeyPressed(66))
						{
							mapEntity.OnOpenEncyclopedia();
							this._mapCursor.SetVisible(false);
						}
						ITrackableCampaignObject trackableCampaignObject;
						if ((trackableCampaignObject = mapEntity as ITrackableCampaignObject) != null && this.SceneLayer.Input.IsGameKeyPressed(65))
						{
							if (Campaign.Current.VisualTrackerManager.CheckTracked(trackableCampaignObject))
							{
								Campaign.Current.VisualTrackerManager.RemoveTrackedObject(trackableCampaignObject, false);
							}
							else
							{
								Campaign.Current.VisualTrackerManager.RegisterObject(trackableCampaignObject);
							}
						}
						this.OnHoverMapEntity(mapEntity);
						this.CurrentVisualOfTooltip = partyVisual;
						return;
					}
					if (track != null)
					{
						this.CurrentVisualOfTooltip = null;
						this.SetupMapTooltipForTrack(track);
						return;
					}
					if (!this.TooltipHandlingDisabled)
					{
						this.CurrentVisualOfTooltip = null;
						this.RemoveMapTooltip();
						return;
					}
				}
				else
				{
					this.CurrentVisualOfTooltip = null;
					this.RemoveMapTooltip();
					this.HandleSiegeEngineHoverEnd();
				}
			}
		}

		private void HandleLeftMouseButtonClick(UIntPtr selectedSiegeEntityID, PartyVisual visualOfSelectedEntity, Vec3 intersectionPoint, PathFaceRecord mouseOverFaceIndex)
		{
			this._mapCameraView.HandleLeftMouseButtonClick(this.SceneLayer.Input.GetIsMouseActive());
			if (!this._mapState.AtMenu)
			{
				if (((visualOfSelectedEntity != null) ? visualOfSelectedEntity.GetMapEntity() : null) != null)
				{
					IMapEntity mapEntity = visualOfSelectedEntity.GetMapEntity();
					if (visualOfSelectedEntity.PartyBase == PartyBase.MainParty)
					{
						MobileParty.MainParty.Ai.SetMoveModeHold();
						return;
					}
					PathFaceRecord faceIndex = Campaign.Current.MapSceneWrapper.GetFaceIndex(mapEntity.InteractionPosition);
					if (this._mapScene.DoesPathExistBetweenFaces(faceIndex.FaceIndex, MobileParty.MainParty.CurrentNavigationFace.FaceIndex, false) && this._mapCameraView.ProcessCameraInput && PartyBase.MainParty.MapEvent == null)
					{
						if (mapEntity.OnMapClick(this.SceneLayer.Input.IsHotKeyDown("MapFollowModifier")))
						{
							if (!this._leftButtonDoubleClickOnSceneWidget && Campaign.Current.TimeControlMode == 4)
							{
								this._waitForDoubleClickUntilTime = Time.ApplicationTime + 0.3f;
								Campaign.Current.TimeControlMode = 4;
							}
							else
							{
								Campaign.Current.TimeControlMode = (this._leftButtonDoubleClickOnSceneWidget ? 4 : 3);
							}
							if (TaleWorlds.InputSystem.Input.IsGamepadActive)
							{
								if (mapEntity.IsMobileEntity)
								{
									if (mapEntity.IsAllyOf(PartyBase.MainParty.MapFaction))
									{
										UISoundsHelper.PlayUISound("event:/ui/campaign/click_party");
									}
									else
									{
										UISoundsHelper.PlayUISound("event:/ui/campaign/click_party_enemy");
									}
								}
								else if (mapEntity.IsAllyOf(PartyBase.MainParty.MapFaction))
								{
									UISoundsHelper.PlayUISound("event:/ui/campaign/click_settlement");
								}
								else
								{
									UISoundsHelper.PlayUISound("event:/ui/campaign/click_settlement_enemy");
								}
							}
						}
						MobileParty.MainParty.Ai.ForceAiNoPathMode = false;
						return;
					}
				}
				else if (mouseOverFaceIndex.IsValid())
				{
					bool flag;
					if (this.Input.IsControlDown() && Game.Current.CheatMode)
					{
						if (MobileParty.MainParty.Army != null)
						{
							foreach (MobileParty mobileParty in MobileParty.MainParty.Army.LeaderParty.AttachedParties)
							{
								mobileParty.Position2D += intersectionPoint.AsVec2 - MobileParty.MainParty.Position2D;
							}
						}
						MobileParty.MainParty.Position2D = intersectionPoint.AsVec2;
						MobileParty.MainParty.Ai.SetMoveModeHold();
						foreach (MobileParty mobileParty2 in MobileParty.All)
						{
							mobileParty2.Party.UpdateVisibilityAndInspected(0f);
						}
						foreach (Settlement settlement in Settlement.All)
						{
							settlement.Party.UpdateVisibilityAndInspected(0f);
						}
						MBDebug.Print(string.Concat(new object[] { "main party cheat move! - ", intersectionPoint.x, " ", intersectionPoint.y }), 0, 12, 17592186044416UL);
						flag = true;
					}
					else
					{
						flag = Campaign.Current.MapSceneWrapper.AreFacesOnSameIsland(mouseOverFaceIndex, MobileParty.MainParty.CurrentNavigationFace, false);
					}
					if (flag && this._mapCameraView.ProcessCameraInput && MobileParty.MainParty.MapEvent == null)
					{
						this._mapState.ProcessTravel(intersectionPoint.AsVec2);
						if (!this._leftButtonDoubleClickOnSceneWidget && Campaign.Current.TimeControlMode == 4)
						{
							this._waitForDoubleClickUntilTime = Time.ApplicationTime + 0.3f;
							Campaign.Current.TimeControlMode = 4;
						}
						else
						{
							Campaign.Current.TimeControlMode = (this._leftButtonDoubleClickOnSceneWidget ? 4 : 3);
						}
					}
					this.OnTerrainClick();
					return;
				}
			}
			else
			{
				if (selectedSiegeEntityID != UIntPtr.Zero)
				{
					Tuple<MatrixFrame, PartyVisual> tuple = MapScreen.FrameAndVisualOfEngines[selectedSiegeEntityID];
					this.OnSiegeEngineFrameClick(tuple.Item1);
					return;
				}
				this.OnTerrainClick();
			}
		}

		private void OnTerrainClick()
		{
			foreach (MapView mapView in this._mapViews)
			{
				mapView.OnMapTerrainClick();
			}
			this._mapCursor.OnMapTerrainClick();
		}

		private void OnSiegeEngineFrameClick(MatrixFrame siegeFrame)
		{
			foreach (MapView mapView in this._mapViews)
			{
				mapView.OnSiegeEngineClick(siegeFrame);
			}
		}

		private void InitializeSiegeCircleVisuals()
		{
			Settlement besiegedSettlement = PlayerSiege.PlayerSiegeEvent.BesiegedSettlement;
			PartyVisual visualOfParty = PartyVisualManager.Current.GetVisualOfParty(besiegedSettlement.Party);
			MapScene mapScene = Campaign.Current.MapSceneWrapper as MapScene;
			MatrixFrame[] array = visualOfParty.GetDefenderRangedSiegeEngineFrames();
			this._defenderMachinesCircleEntities = new GameEntity[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				MatrixFrame matrixFrame = array[i];
				this._defenderMachinesCircleEntities[i] = GameEntity.CreateEmpty(mapScene.Scene, true);
				this._defenderMachinesCircleEntities[i].Name = "dRangedMachineCircle_" + i;
				Decal decal = Decal.CreateDecal(null);
				decal.SetMaterial(Material.GetFromResource(this._defenderRangedMachineDecalMaterialName));
				decal.SetFactor1Linear(this._preperationOrEnemySiegeEngineDecalColor);
				this._defenderMachinesCircleEntities[i].AddComponent(decal);
				MatrixFrame matrixFrame2 = matrixFrame;
				if (this._isNewDecalScaleImplementationEnabled)
				{
					matrixFrame2.Scale(new Vec3(this._defenderMachineCircleDecalScale, this._defenderMachineCircleDecalScale, this._defenderMachineCircleDecalScale, -1f));
				}
				this._defenderMachinesCircleEntities[i].SetGlobalFrame(ref matrixFrame2);
				this._defenderMachinesCircleEntities[i].SetVisibilityExcludeParents(true);
				mapScene.Scene.AddDecalInstance(decal, "editor_set", true);
			}
			array = visualOfParty.GetAttackerBatteringRamSiegeEngineFrames();
			this._attackerRamMachinesCircleEntities = new GameEntity[array.Length];
			for (int j = 0; j < array.Length; j++)
			{
				MatrixFrame matrixFrame3 = array[j];
				this._attackerRamMachinesCircleEntities[j] = GameEntity.CreateEmpty(mapScene.Scene, true);
				this._attackerRamMachinesCircleEntities[j].Name = "InitializeSiegeCircleVisuals";
				this._attackerRamMachinesCircleEntities[j].Name = "aRamMachineCircle_" + j;
				Decal decal2 = Decal.CreateDecal(null);
				decal2.SetMaterial(Material.GetFromResource(this._attackerRamMachineDecalMaterialName));
				decal2.SetFactor1Linear(this._preperationOrEnemySiegeEngineDecalColor);
				this._attackerRamMachinesCircleEntities[j].AddComponent(decal2);
				MatrixFrame matrixFrame4 = matrixFrame3;
				if (this._isNewDecalScaleImplementationEnabled)
				{
					matrixFrame4.Scale(new Vec3(this._attackerMachineDecalScale, this._attackerMachineDecalScale, this._attackerMachineDecalScale, -1f));
				}
				this._attackerRamMachinesCircleEntities[j].SetGlobalFrame(ref matrixFrame4);
				this._attackerRamMachinesCircleEntities[j].SetVisibilityExcludeParents(true);
				mapScene.Scene.AddDecalInstance(decal2, "editor_set", true);
			}
			array = visualOfParty.GetAttackerTowerSiegeEngineFrames();
			this._attackerTowerMachinesCircleEntities = new GameEntity[array.Length];
			for (int k = 0; k < array.Length; k++)
			{
				MatrixFrame matrixFrame5 = array[k];
				this._attackerTowerMachinesCircleEntities[k] = GameEntity.CreateEmpty(mapScene.Scene, true);
				this._attackerTowerMachinesCircleEntities[k].Name = "aTowerMachineCircle_" + k;
				Decal decal3 = Decal.CreateDecal(null);
				decal3.SetMaterial(Material.GetFromResource(this._attackerTowerMachineDecalMaterialName));
				decal3.SetFactor1Linear(this._preperationOrEnemySiegeEngineDecalColor);
				this._attackerTowerMachinesCircleEntities[k].AddComponent(decal3);
				MatrixFrame matrixFrame6 = matrixFrame5;
				if (this._isNewDecalScaleImplementationEnabled)
				{
					matrixFrame6.Scale(new Vec3(this._attackerMachineDecalScale, this._attackerMachineDecalScale, this._attackerMachineDecalScale, -1f));
				}
				this._attackerTowerMachinesCircleEntities[k].SetGlobalFrame(ref matrixFrame6);
				this._attackerTowerMachinesCircleEntities[k].SetVisibilityExcludeParents(true);
				mapScene.Scene.AddDecalInstance(decal3, "editor_set", true);
			}
			array = visualOfParty.GetAttackerRangedSiegeEngineFrames();
			this._attackerRangedMachinesCircleEntities = new GameEntity[array.Length];
			for (int l = 0; l < array.Length; l++)
			{
				MatrixFrame matrixFrame7 = array[l];
				this._attackerRangedMachinesCircleEntities[l] = GameEntity.CreateEmpty(mapScene.Scene, true);
				this._attackerRangedMachinesCircleEntities[l].Name = "aRangedMachineCircle_" + l;
				Decal decal4 = Decal.CreateDecal(null);
				decal4.SetMaterial(Material.GetFromResource(this._emptyAttackerRangedDecalMaterialName));
				decal4.SetFactor1Linear(this._preperationOrEnemySiegeEngineDecalColor);
				this._attackerRangedMachinesCircleEntities[l].AddComponent(decal4);
				MatrixFrame matrixFrame8 = matrixFrame7;
				if (this._isNewDecalScaleImplementationEnabled)
				{
					matrixFrame8.Scale(new Vec3(this._attackerMachineDecalScale, this._attackerMachineDecalScale, this._attackerMachineDecalScale, -1f));
				}
				this._attackerRangedMachinesCircleEntities[l].SetGlobalFrame(ref matrixFrame8);
				this._attackerRangedMachinesCircleEntities[l].SetVisibilityExcludeParents(true);
				mapScene.Scene.AddDecalInstance(decal4, "editor_set", true);
			}
		}

		private void TickSiegeMachineCircles()
		{
			SiegeEvent playerSiegeEvent = PlayerSiege.PlayerSiegeEvent;
			bool flag = playerSiegeEvent != null && playerSiegeEvent.IsPlayerSiegeEvent && Campaign.Current.Models.EncounterModel.GetLeaderOfSiegeEvent(playerSiegeEvent, PlayerSiege.PlayerSide) == Hero.MainHero;
			bool isPreparationComplete = playerSiegeEvent.BesiegerCamp.IsPreparationComplete;
			Settlement besiegedSettlement = playerSiegeEvent.BesiegedSettlement;
			PartyVisual visualOfParty = PartyVisualManager.Current.GetVisualOfParty(besiegedSettlement.Party);
			Tuple<MatrixFrame, PartyVisual> tuple = null;
			if (this._preSelectedSiegeEntityID != UIntPtr.Zero)
			{
				tuple = MapScreen.FrameAndVisualOfEngines[this._preSelectedSiegeEntityID];
			}
			for (int i = 0; i < visualOfParty.GetDefenderRangedSiegeEngineFrames().Length; i++)
			{
				bool flag2 = playerSiegeEvent.GetSiegeEventSide(0).SiegeEngines.DeployedRangedSiegeEngines[i] == null;
				bool flag3 = PlayerSiege.PlayerSide > 0;
				string desiredMaterialName = this.GetDesiredMaterialName(true, false, flag2, false);
				Decal decal = this._defenderMachinesCircleEntities[i].GetComponentAtIndex(0, 7) as Decal;
				Material material = decal.GetMaterial();
				if (((material != null) ? material.Name : null) != desiredMaterialName)
				{
					decal.SetMaterial(Material.GetFromResource(desiredMaterialName));
				}
				bool flag4 = tuple != null && this._defenderMachinesCircleEntities[i].GetGlobalFrame().NearlyEquals(tuple.Item1, 1E-05f);
				uint desiredDecalColor = this.GetDesiredDecalColor(isPreparationComplete, flag4, flag3, flag2, flag);
				if (desiredDecalColor != decal.GetFactor1())
				{
					decal.SetFactor1(desiredDecalColor);
				}
			}
			for (int j = 0; j < visualOfParty.GetAttackerRangedSiegeEngineFrames().Length; j++)
			{
				bool flag5 = playerSiegeEvent.GetSiegeEventSide(1).SiegeEngines.DeployedRangedSiegeEngines[j] == null;
				bool flag6 = PlayerSiege.PlayerSide != 1;
				string desiredMaterialName2 = this.GetDesiredMaterialName(true, true, flag5, false);
				Decal decal2 = this._attackerRangedMachinesCircleEntities[j].GetComponentAtIndex(0, 7) as Decal;
				Material material2 = decal2.GetMaterial();
				if (((material2 != null) ? material2.Name : null) != desiredMaterialName2)
				{
					decal2.SetMaterial(Material.GetFromResource(desiredMaterialName2));
				}
				bool flag7 = tuple != null && this._attackerRangedMachinesCircleEntities[j].GetGlobalFrame().NearlyEquals(tuple.Item1, 1E-05f);
				uint desiredDecalColor2 = this.GetDesiredDecalColor(isPreparationComplete, flag7, flag6, flag5, flag);
				if (desiredDecalColor2 != decal2.GetFactor1())
				{
					decal2.SetFactor1(desiredDecalColor2);
				}
			}
			for (int k = 0; k < visualOfParty.GetAttackerBatteringRamSiegeEngineFrames().Length; k++)
			{
				bool flag8 = playerSiegeEvent.GetSiegeEventSide(1).SiegeEngines.DeployedMeleeSiegeEngines[k] == null;
				bool flag9 = PlayerSiege.PlayerSide != 1;
				string desiredMaterialName3 = this.GetDesiredMaterialName(false, true, flag8, false);
				Decal decal3 = this._attackerRamMachinesCircleEntities[k].GetComponentAtIndex(0, 7) as Decal;
				Material material3 = decal3.GetMaterial();
				if (((material3 != null) ? material3.Name : null) != desiredMaterialName3)
				{
					decal3.SetMaterial(Material.GetFromResource(desiredMaterialName3));
				}
				bool flag10 = tuple != null && this._attackerRamMachinesCircleEntities[k].GetGlobalFrame().NearlyEquals(tuple.Item1, 1E-05f);
				uint desiredDecalColor3 = this.GetDesiredDecalColor(isPreparationComplete, flag10, flag9, flag8, flag);
				if (desiredDecalColor3 != decal3.GetFactor1())
				{
					decal3.SetFactor1(desiredDecalColor3);
				}
			}
			for (int l = 0; l < visualOfParty.GetAttackerTowerSiegeEngineFrames().Length; l++)
			{
				bool flag11 = playerSiegeEvent.GetSiegeEventSide(1).SiegeEngines.DeployedMeleeSiegeEngines[visualOfParty.GetAttackerBatteringRamSiegeEngineFrames().Length + l] == null;
				bool flag12 = PlayerSiege.PlayerSide != 1;
				string desiredMaterialName4 = this.GetDesiredMaterialName(false, true, flag11, true);
				Decal decal4 = this._attackerTowerMachinesCircleEntities[l].GetComponentAtIndex(0, 7) as Decal;
				Material material4 = decal4.GetMaterial();
				if (((material4 != null) ? material4.Name : null) != desiredMaterialName4)
				{
					decal4.SetMaterial(Material.GetFromResource(desiredMaterialName4));
				}
				bool flag13 = tuple != null && this._attackerTowerMachinesCircleEntities[l].GetGlobalFrame().NearlyEquals(tuple.Item1, 1E-05f);
				uint desiredDecalColor4 = this.GetDesiredDecalColor(isPreparationComplete, flag13, flag12, flag11, flag);
				if (desiredDecalColor4 != decal4.GetFactor1())
				{
					decal4.SetFactor1(desiredDecalColor4);
				}
			}
		}

		private uint GetDesiredDecalColor(bool isPrepOver, bool isHovered, bool isEnemy, bool isEmpty, bool isPlayerLeader)
		{
			isPrepOver = true;
			if (!isPrepOver || isEnemy)
			{
				return this._preperationOrEnemySiegeEngineDecalColor;
			}
			if (isHovered && isPlayerLeader)
			{
				return this._hoveredSiegeEngineDecalColor;
			}
			if (!isEmpty)
			{
				return this._withMachineSiegeEngineDecalColor;
			}
			if (isPlayerLeader)
			{
				float num = MathF.PingPong(0f, this._machineDecalAnimLoopTime, this._timeSinceCreation) / this._machineDecalAnimLoopTime;
				Color color = Color.FromUint(this._normalStartSiegeEngineDecalColor);
				Color color2 = Color.FromUint(this._normalEndSiegeEngineDecalColor);
				return Color.Lerp(color, color2, num).ToUnsignedInteger();
			}
			return this._normalStartSiegeEngineDecalColor;
		}

		private string GetDesiredMaterialName(bool isRanged, bool isAttacker, bool isEmpty, bool isTower)
		{
			if (isRanged)
			{
				if (!isAttacker)
				{
					return this._defenderRangedMachineDecalMaterialName;
				}
				return this._attackerRangedMachineDecalMaterialName;
			}
			else
			{
				if (!isTower)
				{
					return this._attackerRamMachineDecalMaterialName;
				}
				return this._attackerTowerMachineDecalMaterialName;
			}
		}

		private void RemoveSiegeCircleVisuals()
		{
			if (this._playerSiegeMachineSlotMeshesAdded)
			{
				MapScene mapScene = Campaign.Current.MapSceneWrapper as MapScene;
				for (int i = 0; i < this._defenderMachinesCircleEntities.Length; i++)
				{
					this._defenderMachinesCircleEntities[i].SetVisibilityExcludeParents(false);
					mapScene.Scene.RemoveEntity(this._defenderMachinesCircleEntities[i], 107);
					this._defenderMachinesCircleEntities[i] = null;
				}
				for (int j = 0; j < this._attackerRamMachinesCircleEntities.Length; j++)
				{
					this._attackerRamMachinesCircleEntities[j].SetVisibilityExcludeParents(false);
					mapScene.Scene.RemoveEntity(this._attackerRamMachinesCircleEntities[j], 108);
					this._attackerRamMachinesCircleEntities[j] = null;
				}
				for (int k = 0; k < this._attackerTowerMachinesCircleEntities.Length; k++)
				{
					this._attackerTowerMachinesCircleEntities[k].SetVisibilityExcludeParents(false);
					mapScene.Scene.RemoveEntity(this._attackerTowerMachinesCircleEntities[k], 109);
					this._attackerTowerMachinesCircleEntities[k] = null;
				}
				for (int l = 0; l < this._attackerRangedMachinesCircleEntities.Length; l++)
				{
					this._attackerRangedMachinesCircleEntities[l].SetVisibilityExcludeParents(false);
					mapScene.Scene.RemoveEntity(this._attackerRangedMachinesCircleEntities[l], 110);
					this._attackerRangedMachinesCircleEntities[l] = null;
				}
				this._playerSiegeMachineSlotMeshesAdded = false;
			}
		}

		void IMapStateHandler.AfterTick(float dt)
		{
			if (ScreenManager.TopScreen == this)
			{
				this.TickVisuals(dt);
				SceneLayer sceneLayer = this.SceneLayer;
				if (sceneLayer != null && sceneLayer.Input.IsGameKeyPressed(53))
				{
					Campaign.Current.SaveHandler.QuickSaveCurrentGame();
				}
			}
			base.DebugInput.IsHotKeyPressed("MapScreenHotkeyShowPos");
		}

		void IMapStateHandler.AfterWaitTick(float dt)
		{
			if (!this.SceneLayer.Input.IsShiftDown() && !this.SceneLayer.Input.IsControlDown())
			{
				bool flag = false;
				if (this.SceneLayer.Input.IsGameKeyPressed(38) && this._navigationHandler.InventoryEnabled)
				{
					this.OpenInventory();
					flag = true;
				}
				else if (this.SceneLayer.Input.IsGameKeyPressed(43) && this._navigationHandler.PartyEnabled)
				{
					this.OpenParty();
					flag = true;
				}
				else if (this.SceneLayer.Input.IsGameKeyPressed(39) && !this.IsInArmyManagement && !this.IsMapCheatsActive)
				{
					this.OpenEncyclopedia();
					flag = true;
				}
				else if (this.SceneLayer.Input.IsGameKeyPressed(36) && !this.IsInArmyManagement && !this.IsMarriageOfferPopupActive && !this.IsMapCheatsActive)
				{
					this.OpenBannerEditorScreen();
					flag = true;
				}
				else if (this.SceneLayer.Input.IsGameKeyPressed(40) && this._navigationHandler.KingdomPermission.IsAuthorized)
				{
					this.OpenKingdom();
					flag = true;
				}
				else if (this.SceneLayer.Input.IsGameKeyPressed(42) && this._navigationHandler.QuestsEnabled)
				{
					this.OpenQuestsScreen();
					flag = true;
				}
				else if (this.SceneLayer.Input.IsGameKeyPressed(41) && this._navigationHandler.ClanPermission.IsAuthorized)
				{
					this.OpenClanScreen();
					flag = true;
				}
				else if (this.SceneLayer.Input.IsGameKeyPressed(37) && this._navigationHandler.CharacterDeveloperEnabled)
				{
					this.OpenCharacterDevelopmentScreen();
					flag = true;
				}
				else if (this.SceneLayer.Input.IsHotKeyReleased("ToggleEscapeMenu"))
				{
					if (!this._mapViews.Any((MapView m) => m.IsEscaped()))
					{
						this.OpenEscapeMenu();
						flag = true;
					}
				}
				else if (this.SceneLayer.Input.IsGameKeyPressed(44))
				{
					this.OpenFaceGeneratorScreen();
					flag = true;
				}
				else if (TaleWorlds.InputSystem.Input.IsGamepadActive)
				{
					this.HandleCheatMenuInput(dt);
				}
				if (flag)
				{
					this._mapCursor.SetVisible(false);
				}
			}
		}

		private void HandleCheatMenuInput(float dt)
		{
			if (!this.IsMapCheatsActive && this.Input.IsKeyDown(248) && this.Input.IsKeyDown(255) && this.Input.IsKeyDown(241))
			{
				this._cheatPressTimer += dt;
				if (this._cheatPressTimer > 0.55f)
				{
					this.OpenGameplayCheats();
				}
				return;
			}
			this._cheatPressTimer = 0f;
		}

		void IMapStateHandler.OnRefreshState()
		{
			if (Game.Current.GameStateManager.ActiveState is MapState)
			{
				if (MobileParty.MainParty.Army != null && this._armyOverlay == null)
				{
					this.AddArmyOverlay(1);
					return;
				}
				if (MobileParty.MainParty.Army == null && this._armyOverlay != null)
				{
					for (int i = this._mapViews.Count - 1; i >= 0; i--)
					{
						this._mapViews[i].OnArmyLeft();
					}
					for (int j = this._mapViews.Count - 1; j >= 0; j--)
					{
						this._mapViews[j].OnDispersePlayerLeadedArmy();
					}
				}
			}
		}

		void IMapStateHandler.OnExitingMenuMode()
		{
			this._latestMenuContext = null;
		}

		void IMapStateHandler.OnEnteringMenuMode(MenuContext menuContext)
		{
			this._latestMenuContext = menuContext;
		}

		void IMapStateHandler.OnMainPartyEncounter()
		{
			for (int i = this._mapViews.Count - 1; i >= 0; i--)
			{
				this._mapViews[i].OnMainPartyEncounter();
			}
		}

		void IMapStateHandler.OnSignalPeriodicEvents()
		{
			this.DeleteMarkedPeriodicEvents();
		}

		void IMapStateHandler.OnBattleSimulationStarted(BattleSimulation battleSimulation)
		{
			this.IsInBattleSimulation = true;
			this._battleSimulationView = this.AddMapView<BattleSimulationMapView>(new object[] { battleSimulation });
		}

		void IMapStateHandler.OnBattleSimulationEnded()
		{
			this.IsInBattleSimulation = false;
			this.RemoveMapView(this._battleSimulationView);
			this._battleSimulationView = null;
		}

		void IMapStateHandler.OnSiegeEngineClick(MatrixFrame siegeEngineFrame)
		{
			this._mapCameraView.SiegeEngineClick(siegeEngineFrame);
		}

		void IGameStateListener.OnInitialize()
		{
		}

		void IMapStateHandler.OnPlayerSiegeActivated()
		{
		}

		void IMapStateHandler.OnPlayerSiegeDeactivated()
		{
		}

		void IMapStateHandler.OnGameplayCheatsEnabled()
		{
			this.OpenGameplayCheats();
		}

		void IGameStateListener.OnActivate()
		{
		}

		void IGameStateListener.OnDeactivate()
		{
		}

		void IMapStateHandler.OnMapConversationStarts(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData)
		{
			if (this._isReadyForRender || this._conversationOverThisFrame)
			{
				this.HandleMapConversationInit(playerCharacterData, conversationPartnerData);
				return;
			}
			MapScreen.TempConversationStateHandler tempConversationStateHandler = new MapScreen.TempConversationStateHandler();
			this._conversationDataCache = new Tuple<ConversationCharacterData, ConversationCharacterData, MapScreen.TempConversationStateHandler>(playerCharacterData, conversationPartnerData, tempConversationStateHandler);
			Campaign.Current.ConversationManager.Handler = tempConversationStateHandler;
			Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
		}

		private void HandleMapConversationInit(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData)
		{
			if (this._mapConversationView == null)
			{
				for (int i = this._mapViews.Count - 1; i >= 0; i--)
				{
					this._mapViews[i].OnMapConversationStart();
				}
			}
			MenuViewContext menuViewContext = this._menuViewContext;
			if (menuViewContext != null)
			{
				menuViewContext.OnMapConversationActivated();
			}
			if (this._mapConversationView == null)
			{
				this._mapConversationView = this.AddMapView<MapConversationView>(new object[] { playerCharacterData, conversationPartnerData });
			}
			else
			{
				for (int j = this._mapViews.Count - 1; j >= 0; j--)
				{
					this._mapViews[j].OnMapConversationUpdate(playerCharacterData, conversationPartnerData);
				}
			}
			this._mapCursor.SetVisible(false);
			this.HandleIfSceneIsReady();
		}

		void IMapStateHandler.OnMapConversationOver()
		{
			this._conversationOverThisFrame = true;
			for (int i = this._mapViews.Count - 1; i >= 0; i--)
			{
				this._mapViews[i].OnMapConversationOver();
			}
			MenuViewContext menuViewContext = this._menuViewContext;
			if (menuViewContext != null)
			{
				menuViewContext.OnMapConversationDeactivated();
			}
			this.HandleMapConversationOver();
			this._activatedFrameNo = Utilities.EngineFrameNo;
			this.HandleIfSceneIsReady();
		}

		private void HandleMapConversationOver()
		{
			if (this._mapConversationView != null)
			{
				this.RemoveMapView(this._mapConversationView);
			}
			this._mapConversationView = null;
		}

		private void InitializeVisuals()
		{
			this.InactiveLightMeshes = new List<Mesh>();
			this.ActiveLightMeshes = new List<Mesh>();
			MapScene mapScene = Campaign.Current.MapSceneWrapper as MapScene;
			this._targetCircleEntitySmall = GameEntity.CreateEmpty(mapScene.Scene, true);
			this._targetCircleEntitySmall.Name = "tCircleSmall";
			this._targetCircleEntityBig = GameEntity.CreateEmpty(mapScene.Scene, true);
			this._targetCircleEntityBig.Name = "tCircleBig";
			this._targetCircleTown = GameEntity.CreateEmpty(mapScene.Scene, true);
			this._targetCircleTown.Name = "tTown";
			this._partyOutlineEntity = GameEntity.CreateEmpty(mapScene.Scene, true);
			this._partyOutlineEntity.Name = "sCircle";
			this._townOutlineEntity = GameEntity.CreateEmpty(mapScene.Scene, true);
			this._townOutlineEntity.Name = "sSettlementOutline";
			this._targetDecalMeshSmall = Decal.CreateDecal(null);
			if (this._targetDecalMeshSmall != null)
			{
				this._settlementOutlineMesh = this._targetDecalMeshSmall.CreateCopy();
				Material fromResource = Material.GetFromResource("decal_city_circle_a");
				if (fromResource != null)
				{
					this._settlementOutlineMesh.SetMaterial(fromResource);
				}
				this._targetTownMesh = this._settlementOutlineMesh.CreateCopy();
				this._targetDecalMeshSmall = this._targetDecalMeshSmall.CreateCopy();
				Material fromResource2 = Material.GetFromResource("map_circle_decal");
				if (fromResource2 != null)
				{
					this._targetDecalMeshSmall.SetMaterial(fromResource2);
				}
				else
				{
					MBDebug.ShowWarning("Material(map_circle_decal) for party circles could not be found.");
				}
				this._targetDecalMeshBig = this._targetDecalMeshSmall.CreateCopy();
				this._partyOutlineMesh = this._targetDecalMeshSmall.CreateCopy();
				mapScene.Scene.AddDecalInstance(this._targetDecalMeshSmall, "editor_set", false);
				mapScene.Scene.AddDecalInstance(this._targetDecalMeshBig, "editor_set", false);
				mapScene.Scene.AddDecalInstance(this._partyOutlineMesh, "editor_set", false);
				mapScene.Scene.AddDecalInstance(this._settlementOutlineMesh, "editor_set", false);
				mapScene.Scene.AddDecalInstance(this._targetTownMesh, "editor_set", false);
				this._targetCircleEntitySmall.AddComponent(this._targetDecalMeshSmall);
				this._targetCircleEntityBig.AddComponent(this._targetDecalMeshBig);
				this._partyOutlineEntity.AddComponent(this._partyOutlineMesh);
				this._townOutlineEntity.AddComponent(this._settlementOutlineMesh);
				this._targetCircleTown.AddComponent(this._targetTownMesh);
			}
			else
			{
				MBDebug.ShowWarning("Mesh(decal_mesh) for party circles could not be found.");
			}
			this._mapCursor.Initialize(this);
			this._campaign = Campaign.Current;
			this._campaign.AddEntityComponent<MapTracksVisual>();
			this._campaign.AddEntityComponent<MapWeatherVisualManager>();
			this._campaign.AddEntityComponent<MapAudioManager>();
			this._campaign.AddEntityComponent<PartyVisualManager>();
			this.ContourMaskEntity = GameEntity.CreateEmpty(mapScene.Scene, true);
			this.ContourMaskEntity.Name = "aContourMask";
		}

		internal void TickCircles(float realDt)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			float num = 0.5f;
			float num2 = 0.5f;
			int num3 = 0;
			int num4 = 0;
			uint num5 = 4293199122U;
			uint num6 = 4293199122U;
			uint num7 = 4293199122U;
			bool flag4 = false;
			bool flag5 = false;
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			PartyBase partyBase = null;
			if (MobileParty.MainParty.Ai.PartyMoveMode == 1 && MobileParty.MainParty.DefaultBehavior != 2 && MobileParty.MainParty.DefaultBehavior != null && !MobileParty.MainParty.Ai.ForceAiNoPathMode && MobileParty.MainParty.MapEvent == null && MobileParty.MainParty.TargetPosition.DistanceSquared(MobileParty.MainParty.Position2D) > 0.01f)
			{
				flag = true;
				flag2 = true;
				num = 0.238846f;
				num2 = 0.278584f;
				num3 = 4;
				num4 = 5;
				num5 = 4293993473U;
				num6 = 4293993473U;
				matrixFrame.origin = new Vec3(MobileParty.MainParty.TargetPosition, 0f, -1f);
				flag5 = true;
			}
			else
			{
				if (MobileParty.MainParty.Ai.PartyMoveMode == 2 && MobileParty.MainParty.Ai.MoveTargetParty != null && MobileParty.MainParty.Ai.MoveTargetParty.IsVisible)
				{
					if (MobileParty.MainParty.Ai.MoveTargetParty.CurrentSettlement == null || MobileParty.MainParty.Ai.MoveTargetParty.CurrentSettlement.IsHideout)
					{
						partyBase = MobileParty.MainParty.Ai.MoveTargetParty.Party;
					}
					else
					{
						partyBase = MobileParty.MainParty.Ai.MoveTargetParty.CurrentSettlement.Party;
					}
				}
				else if (MobileParty.MainParty.DefaultBehavior == 2 && MobileParty.MainParty.TargetSettlement != null)
				{
					partyBase = MobileParty.MainParty.TargetSettlement.Party;
				}
				if (partyBase != null)
				{
					bool flag6 = FactionManager.IsAtWarAgainstFaction(partyBase.MapFaction, Hero.MainHero.MapFaction);
					bool flag7 = FactionManager.IsAlliedWithFaction(partyBase.MapFaction, Hero.MainHero.MapFaction);
					matrixFrame = PartyVisualManager.Current.GetVisualOfParty(partyBase).CircleLocalFrame;
					if (partyBase.IsMobile)
					{
						flag = true;
						num3 = this.GetCircleIndex();
						num5 = (flag6 ? this._enemyPartyDecalColor : (flag7 ? this._allyPartyDecalColor : this._neutralPartyDecalColor));
						num = matrixFrame.rotation.GetScaleVector().x * 1.2f;
					}
					else if (partyBase.IsSettlement && (partyBase.Settlement.IsTown || partyBase.Settlement.IsCastle))
					{
						flag4 = true;
						flag3 = true;
						num7 = (flag6 ? this._enemyPartyDecalColor : (flag7 ? this._allyPartyDecalColor : this._neutralPartyDecalColor));
						num = matrixFrame.rotation.GetScaleVector().x * 1.2f;
					}
					else
					{
						flag = true;
						num3 = 5;
						num5 = (flag6 ? this._enemyPartyDecalColor : (flag7 ? this._allyPartyDecalColor : this._neutralPartyDecalColor));
						num = matrixFrame.rotation.GetScaleVector().x * 1.2f;
					}
					if (!flag4)
					{
						matrixFrame.origin += new Vec3(partyBase.Position2D + (partyBase.IsMobile ? (partyBase.MobileParty.EventPositionAdder + partyBase.MobileParty.ArmyPositionAdder) : Vec2.Zero), 0f, -1f);
					}
				}
			}
			if (flag5)
			{
				float num8 = (this._mapCameraView.CameraDistance + 80f) * (this._mapCameraView.CameraDistance + 80f) / 5000f;
				num8 = MathF.Clamp(num8, 0.2f, 45f);
				num *= num8;
				num2 *= num8;
			}
			if (partyBase == null)
			{
				this._targetCircleRotationStartTime = 0f;
			}
			else if (this._targetCircleRotationStartTime == 0f)
			{
				this._targetCircleRotationStartTime = MBCommon.GetApplicationTime();
			}
			Vec3 normalAt = this._mapScene.GetNormalAt(matrixFrame.origin.AsVec2);
			if (!flag4)
			{
				Vec3 origin = this._targetCircleTown.GetGlobalFrame().origin;
				matrixFrame.origin.z = ((origin.AsVec2 != matrixFrame.origin.AsVec2) ? this._mapScene.GetTerrainHeight(matrixFrame.origin.AsVec2, true) : origin.z);
			}
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = matrixFrame.origin;
			identity.rotation.u = normalAt;
			MatrixFrame matrixFrame2 = identity;
			identity.rotation.ApplyScaleLocal(new Vec3(num, num, num, -1f));
			matrixFrame2.rotation.ApplyScaleLocal(new Vec3(num2, num2, num2, -1f));
			this._targetCircleEntitySmall.SetVisibilityExcludeParents(flag);
			this._targetCircleEntityBig.SetVisibilityExcludeParents(flag2);
			this._targetCircleTown.SetVisibilityExcludeParents(flag3);
			if (flag)
			{
				this._targetDecalMeshSmall.SetVectorArgument(0.166f, 1f, 0.166f * (float)num3, 0f);
				this._targetDecalMeshSmall.SetFactor1Linear(num5);
				this._targetCircleEntitySmall.SetGlobalFrame(ref identity);
			}
			if (flag2)
			{
				this._targetDecalMeshBig.SetVectorArgument(0.166f, 1f, 0.166f * (float)num4, 0f);
				this._targetDecalMeshBig.SetFactor1Linear(num6);
				this._targetCircleEntityBig.SetGlobalFrame(ref matrixFrame2);
			}
			if (flag3)
			{
				this._targetTownMesh.SetVectorArgument(1f, 1f, 0f, 0f);
				this._targetTownMesh.SetFactor1Linear(num7);
				this._targetCircleTown.SetGlobalFrame(ref matrixFrame);
			}
			MatrixFrame matrixFrame3 = MatrixFrame.Identity;
			if (this.CurrentVisualOfTooltip == null || ((partyBase != null) ? partyBase.MapEntity : null) == this.CurrentVisualOfTooltip.GetMapEntity())
			{
				this._townOutlineEntity.SetVisibilityExcludeParents(false);
				this._partyOutlineEntity.SetVisibilityExcludeParents(false);
				return;
			}
			this._mapCursor.OnAnotherEntityHighlighted();
			IMapEntity mapEntity = this.CurrentVisualOfTooltip.GetMapEntity();
			if (mapEntity == null || !mapEntity.ShowCircleAroundEntity)
			{
				this._townOutlineEntity.SetVisibilityExcludeParents(false);
				this._partyOutlineEntity.SetVisibilityExcludeParents(false);
				return;
			}
			bool flag8 = mapEntity.IsEnemyOf(Hero.MainHero.MapFaction);
			bool flag9 = mapEntity.IsAllyOf(Hero.MainHero.MapFaction);
			Settlement settlement;
			flag4 = (settlement = mapEntity as Settlement) != null && settlement.IsFortification;
			Vec3 vec;
			if (flag4)
			{
				vec = this._townOutlineEntity.GetGlobalFrame().origin;
				matrixFrame3 = this.CurrentVisualOfTooltip.CircleLocalFrame;
				if (flag8)
				{
					this._settlementOutlineMesh.SetFactor1Linear(this._enemyPartyDecalColor);
				}
				else if (flag9)
				{
					this._settlementOutlineMesh.SetFactor1Linear(this._allyPartyDecalColor);
				}
				else
				{
					this._settlementOutlineMesh.SetFactor1Linear(this._neutralPartyDecalColor);
				}
			}
			else
			{
				vec = this._partyOutlineEntity.GetGlobalFrame().origin;
				matrixFrame3.origin = this.CurrentVisualOfTooltip.GetVisualPosition() + this.CurrentVisualOfTooltip.CircleLocalFrame.origin;
				matrixFrame3.rotation = this.CurrentVisualOfTooltip.CircleLocalFrame.rotation;
				this._partyOutlineMesh.SetFactor1Linear(flag8 ? this._enemyPartyDecalColor : (flag9 ? this._allyPartyDecalColor : this._neutralPartyDecalColor));
				this._partyOutlineMesh.SetVectorArgument(0.166f, 1f, 0.83f, 0f);
			}
			matrixFrame3.origin.z = ((vec.AsVec2 != matrixFrame3.origin.AsVec2) ? this._mapScene.GetTerrainHeight(matrixFrame3.origin.AsVec2, true) : vec.z);
			if (flag4)
			{
				matrixFrame3.rotation.u = normalAt * matrixFrame3.rotation.u.Length;
				this._townOutlineEntity.SetGlobalFrame(ref matrixFrame3);
				this._townOutlineEntity.SetVisibilityExcludeParents(true);
				this._partyOutlineEntity.SetVisibilityExcludeParents(false);
				return;
			}
			this._partyOutlineEntity.SetGlobalFrame(ref matrixFrame3);
			this._townOutlineEntity.SetVisibilityExcludeParents(false);
			this._partyOutlineEntity.SetVisibilityExcludeParents(true);
		}

		public void SetIsInTownManagement(bool isInTownManagement)
		{
			if (this.IsInTownManagement != isInTownManagement)
			{
				this.IsInTownManagement = isInTownManagement;
			}
		}

		public void SetIsInHideoutTroopManage(bool isInHideoutTroopManage)
		{
			if (this.IsInHideoutTroopManage != isInHideoutTroopManage)
			{
				this.IsInHideoutTroopManage = isInHideoutTroopManage;
			}
		}

		public void SetIsInArmyManagement(bool isInArmyManagement)
		{
			if (this.IsInArmyManagement != isInArmyManagement)
			{
				this.IsInArmyManagement = isInArmyManagement;
				if (!this.IsInArmyManagement)
				{
					MenuViewContext menuViewContext = this._menuViewContext;
					if (menuViewContext == null)
					{
						return;
					}
					menuViewContext.OnResume();
				}
			}
		}

		public void SetIsInRecruitment(bool isInRecruitment)
		{
			if (this.IsInRecruitment != isInRecruitment)
			{
				this.IsInRecruitment = isInRecruitment;
			}
		}

		public void SetIsBarExtended(bool isBarExtended)
		{
			if (this.IsBarExtended != isBarExtended)
			{
				this.IsBarExtended = isBarExtended;
			}
		}

		public void SetIsInCampaignOptions(bool isInCampaignOptions)
		{
			if (this.IsInCampaignOptions != isInCampaignOptions)
			{
				this.IsInCampaignOptions = isInCampaignOptions;
			}
		}

		public void SetIsMarriageOfferPopupActive(bool isMarriageOfferPopupActive)
		{
			if (this.IsMarriageOfferPopupActive != isMarriageOfferPopupActive)
			{
				this.IsMarriageOfferPopupActive = isMarriageOfferPopupActive;
			}
		}

		public void SetIsMapCheatsActive(bool isMapCheatsActive)
		{
			if (this.IsMapCheatsActive != isMapCheatsActive)
			{
				this.IsMapCheatsActive = isMapCheatsActive;
				this._cheatPressTimer = 0f;
			}
		}

		private void TickVisuals(float realDt)
		{
			if (this._campaign.CampaignDt < 1E-05f)
			{
				this.ApplySoundSceneProps(realDt);
			}
			else
			{
				this.ApplySoundSceneProps(this._campaign.CampaignDt);
			}
			this._mapScene.TimeOfDay = CampaignTime.Now.CurrentHourInDay;
			float num;
			float num2;
			Campaign.Current.Models.MapWeatherModel.GetSeasonTimeFactorOfCampaignTime(CampaignTime.Now, ref num, ref num2, false);
			MBMapScene.SetSeasonTimeFactor(this._mapScene, num);
			if (!NativeConfig.DisableSound && ScreenManager.TopScreen is MapScreen)
			{
				this._soundCalculationTime += realDt;
				if (this._isSoundOn)
				{
					this.TickStepSounds();
				}
				if (this._soundCalculationTime > 0.2f)
				{
					this._soundCalculationTime -= 0.2f;
				}
			}
			if (this.IsReady)
			{
				foreach (CampaignEntityComponent campaignEntityComponent in this._campaign.CampaignEntityComponents)
				{
					CampaignEntityVisualComponent campaignEntityVisualComponent = campaignEntityComponent as CampaignEntityVisualComponent;
					if (campaignEntityVisualComponent != null)
					{
						campaignEntityVisualComponent.OnVisualTick(this, realDt, this._campaign.CampaignDt);
					}
				}
			}
			MBMapScene.TickVisuals(this._mapScene, Campaign.CurrentTime % 24f, this._tickedMapMeshes);
			this.TickCircles(realDt);
			MBWindowManager.PreDisplay();
		}

		private void TickStepSounds()
		{
			if (Campaign.Current.CampaignDt > 0f)
			{
				MobileParty mainParty = MobileParty.MainParty;
				float seeingRange = mainParty.SeeingRange;
				LocatableSearchData<MobileParty> locatableSearchData = MobileParty.StartFindingLocatablesAroundPosition(mainParty.Position2D, seeingRange + 25f);
				for (MobileParty mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData); mobileParty != null; mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData))
				{
					if (!mobileParty.IsMilitia && !mobileParty.IsGarrison)
					{
						this.StepSounds(mobileParty);
					}
				}
			}
		}

		private void StepSounds(MobileParty party)
		{
			if (party.IsVisible && party.MemberRoster.TotalManCount > 0)
			{
				PartyVisual visualOfParty = PartyVisualManager.Current.GetVisualOfParty(party.Party);
				if (visualOfParty.HumanAgentVisuals != null)
				{
					TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(party.CurrentNavigationFace);
					AgentVisuals agentVisuals = null;
					int num = 0;
					if (visualOfParty.CaravanMountAgentVisuals != null)
					{
						num = 3;
						agentVisuals = visualOfParty.CaravanMountAgentVisuals;
					}
					else if (visualOfParty.HumanAgentVisuals != null)
					{
						if (visualOfParty.MountAgentVisuals != null)
						{
							num = 1;
							agentVisuals = visualOfParty.MountAgentVisuals;
						}
						else
						{
							num = 0;
							agentVisuals = visualOfParty.HumanAgentVisuals;
						}
					}
					MBMapScene.TickStepSound(this._mapScene, agentVisuals.GetVisuals(), faceTerrainType, num);
				}
			}
		}

		public void SetMouseVisible(bool value)
		{
			this.SceneLayer.InputRestrictions.SetMouseVisibility(value);
		}

		public bool GetMouseVisible()
		{
			return MBMapScene.GetMouseVisible();
		}

		public void RestartAmbientSounds()
		{
			if (this._mapScene != null)
			{
				this._mapScene.ResumeSceneSounds();
			}
		}

		void IGameStateListener.OnFinalize()
		{
		}

		public void PauseAmbientSounds()
		{
			if (this._mapScene != null)
			{
				this._mapScene.PauseSceneSounds();
			}
		}

		public void StopSoundSceneProps()
		{
			if (this._mapScene != null)
			{
				this._mapScene.FinishSceneSounds();
			}
		}

		public void ApplySoundSceneProps(float dt)
		{
		}

		private void CollectTickableMapMeshes()
		{
			this._tickedMapEntities = this._mapScene.FindEntitiesWithTag("ticked_map_entity").ToArray<GameEntity>();
			this._tickedMapMeshes = new Mesh[this._tickedMapEntities.Length];
			for (int i = 0; i < this._tickedMapEntities.Length; i++)
			{
				this._tickedMapMeshes[i] = this._tickedMapEntities[i].GetFirstMesh();
			}
		}

		public void OnPauseTick(float dt)
		{
			this.ApplySoundSceneProps(dt);
		}

		public static Dictionary<UIntPtr, PartyVisual> VisualsOfEntities
		{
			get
			{
				return SandBoxViewSubModule.VisualsOfEntities;
			}
		}

		public MBCampaignEvent CreatePeriodicUIEvent(CampaignTime triggerPeriod, CampaignTime initialWait)
		{
			MBCampaignEvent mbcampaignEvent = new MBCampaignEvent(triggerPeriod, initialWait);
			this._periodicCampaignUIEvents.Add(mbcampaignEvent);
			return mbcampaignEvent;
		}

		internal static Dictionary<UIntPtr, Tuple<MatrixFrame, PartyVisual>> FrameAndVisualOfEngines
		{
			get
			{
				return SandBoxViewSubModule.FrameAndVisualOfEngines;
			}
		}

		private void DeleteMarkedPeriodicEvents()
		{
			for (int i = this._periodicCampaignUIEvents.Count - 1; i >= 0; i--)
			{
				if (this._periodicCampaignUIEvents[i].isEventDeleted)
				{
					this._periodicCampaignUIEvents.RemoveAt(i);
				}
			}
		}

		public void DeletePeriodicUIEvent(MBCampaignEvent campaignEvent)
		{
			campaignEvent.isEventDeleted = true;
		}

		private static float CalculateCameraElevation(float cameraDistance)
		{
			return cameraDistance * 0.5f * 0.015f + 0.35f;
		}

		public void OpenOptions()
		{
			ScreenManager.PushScreen(ViewCreator.CreateOptionsScreen(false));
		}

		public void OpenEncyclopedia()
		{
			Campaign.Current.EncyclopediaManager.GoToLink("LastPage", "");
		}

		public void OpenSaveLoad(bool isSaving)
		{
			ScreenManager.PushScreen(SandBoxViewCreator.CreateSaveLoadScreen(isSaving));
		}

		private void OpenGameplayCheats()
		{
			this._mapCheatsView = this.AddMapView<MapCheatsView>(Array.Empty<object>());
			this.IsMapCheatsActive = true;
		}

		public void CloseGameplayCheats()
		{
			if (this._mapCheatsView != null)
			{
				this.RemoveMapView(this._mapCheatsView);
				return;
			}
			Debug.FailedAssert("Requested remove map cheats but cheats is not enabled", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Map\\MapScreen.cs", "CloseGameplayCheats", 3412);
		}

		public void CloseEscapeMenu()
		{
			this.OnEscapeMenuToggled(false);
		}

		public void OpenEscapeMenu()
		{
			this.OnEscapeMenuToggled(true);
		}

		public void CloseCampaignOptions()
		{
			if (this._campaignOptionsView != null)
			{
				this.RemoveMapView(this._campaignOptionsView);
			}
			this.IsInCampaignOptions = false;
		}

		private List<EscapeMenuItemVM> GetEscapeMenuItems()
		{
			bool isMapConversationActive = this._mapConversationView != null;
			bool isAtSaveLimit = MBSaveLoad.IsMaxNumberOfSavesReached();
			List<EscapeMenuItemVM> list = new List<EscapeMenuItemVM>();
			list.Add(new EscapeMenuItemVM(new TextObject("{=e139gKZc}Return to the Game", null), delegate(object o)
			{
				this.OnEscapeMenuToggled(false);
			}, null, () => new Tuple<bool, TextObject>(false, TextObject.Empty), true));
			list.Add(new EscapeMenuItemVM(new TextObject("{=PXT6aA4J}Campaign Options", null), delegate(object o)
			{
				this._campaignOptionsView = this.AddMapView<MapCampaignOptionsView>(Array.Empty<object>());
				this.IsInCampaignOptions = true;
			}, null, () => new Tuple<bool, TextObject>(false, TextObject.Empty), false));
			list.Add(new EscapeMenuItemVM(new TextObject("{=NqarFr4P}Options", null), delegate(object o)
			{
				this.OnEscapeMenuToggled(false);
				this.OpenOptions();
			}, null, () => new Tuple<bool, TextObject>(false, TextObject.Empty), false));
			list.Add(new EscapeMenuItemVM(new TextObject("{=bV75iwKa}Save", null), delegate(object o)
			{
				this.OnEscapeMenuToggled(false);
				Campaign.Current.SaveHandler.QuickSaveCurrentGame();
			}, null, () => this.GetIsEscapeMenuOptionDisabledReason(isMapConversationActive, false, false), false));
			list.Add(new EscapeMenuItemVM(new TextObject("{=e0KdfaNe}Save As", null), delegate(object o)
			{
				this.OnEscapeMenuToggled(false);
				this.OpenSaveLoad(true);
			}, null, () => this.GetIsEscapeMenuOptionDisabledReason(isMapConversationActive, CampaignOptions.IsIronmanMode, false), false));
			list.Add(new EscapeMenuItemVM(new TextObject("{=9NuttOBC}Load", null), delegate(object o)
			{
				this.OnEscapeMenuToggled(false);
				this.OpenSaveLoad(false);
			}, null, () => this.GetIsEscapeMenuOptionDisabledReason(isMapConversationActive, CampaignOptions.IsIronmanMode, false), false));
			list.Add(new EscapeMenuItemVM(new TextObject("{=AbEh2y8o}Save And Exit", null), delegate(object o)
			{
				Campaign.Current.SaveHandler.QuickSaveCurrentGame();
				this.OnEscapeMenuToggled(false);
				InformationManager.HideInquiry();
				this._exitOnSaveOver = true;
			}, null, () => this.GetIsEscapeMenuOptionDisabledReason(isMapConversationActive, false, isAtSaveLimit), false));
			Action <>9__16;
			list.Add(new EscapeMenuItemVM(new TextObject("{=RamV6yLM}Exit to Main Menu", null), delegate(object o)
			{
				string text = GameTexts.FindText("str_exit", null).ToString();
				string text2 = GameTexts.FindText("str_mission_exit_query", null).ToString();
				bool flag = true;
				bool flag2 = true;
				string text3 = GameTexts.FindText("str_yes", null).ToString();
				string text4 = GameTexts.FindText("str_no", null).ToString();
				Action action = new Action(this.OnExitToMainMenu);
				Action action2;
				if ((action2 = <>9__16) == null)
				{
					action2 = (<>9__16 = delegate
					{
						this.OnEscapeMenuToggled(false);
					});
				}
				InformationManager.ShowInquiry(new InquiryData(text, text2, flag, flag2, text3, text4, action, action2, "", 0f, null, null, null), false, false);
			}, null, () => this.GetIsEscapeMenuOptionDisabledReason(false, CampaignOptions.IsIronmanMode, false), false));
			return list;
		}

		private Tuple<bool, TextObject> GetIsEscapeMenuOptionDisabledReason(bool isMapConversationActive, bool isIronmanMode, bool isAtSaveLimit)
		{
			if (isIronmanMode)
			{
				return new Tuple<bool, TextObject>(true, GameTexts.FindText("str_pause_menu_disabled_hint", "IronmanMode"));
			}
			if (isMapConversationActive)
			{
				return new Tuple<bool, TextObject>(true, GameTexts.FindText("str_pause_menu_disabled_hint", "OngoingConversation"));
			}
			if (isAtSaveLimit)
			{
				return new Tuple<bool, TextObject>(true, GameTexts.FindText("str_pause_menu_disabled_hint", "SaveLimitReached"));
			}
			return new Tuple<bool, TextObject>(false, TextObject.Empty);
		}

		private void OpenParty()
		{
			if (Hero.MainHero.HeroState != 3 && Hero.MainHero != null)
			{
				Hero mainHero = Hero.MainHero;
				if (mainHero != null && !mainHero.IsDead)
				{
					PartyScreenManager.OpenScreenAsNormal();
				}
			}
		}

		public void OpenInventory()
		{
			if (Hero.MainHero != null)
			{
				Hero mainHero = Hero.MainHero;
				if (mainHero != null && !mainHero.IsDead)
				{
					InventoryManager.OpenScreenAsInventory(null);
				}
			}
		}

		private void OpenKingdom()
		{
			if (Hero.MainHero != null)
			{
				Hero mainHero = Hero.MainHero;
				if (mainHero != null && !mainHero.IsDead && Hero.MainHero.MapFaction.IsKingdomFaction)
				{
					KingdomState kingdomState = Game.Current.GameStateManager.CreateState<KingdomState>();
					Game.Current.GameStateManager.PushState(kingdomState, 0);
				}
			}
		}

		private void OnExitToMainMenu()
		{
			this.OnEscapeMenuToggled(false);
			InformationManager.HideInquiry();
			this.OnExit();
		}

		private void OpenQuestsScreen()
		{
			if (Hero.MainHero != null)
			{
				Hero mainHero = Hero.MainHero;
				if (mainHero != null && !mainHero.IsDead)
				{
					Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<QuestsState>(), 0);
				}
			}
		}

		private void OpenClanScreen()
		{
			if (Hero.MainHero != null)
			{
				Hero mainHero = Hero.MainHero;
				if (mainHero != null && !mainHero.IsDead)
				{
					Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<ClanState>(), 0);
				}
			}
		}

		private void OpenCharacterDevelopmentScreen()
		{
			if (Hero.MainHero != null)
			{
				Hero mainHero = Hero.MainHero;
				if (mainHero != null && !mainHero.IsDead)
				{
					Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<CharacterDeveloperState>(), 0);
				}
			}
		}

		public void OpenFacegenScreenAux()
		{
			this.OpenFaceGeneratorScreen();
		}

		private int GetCircleIndex()
		{
			int num = (int)((MBCommon.GetApplicationTime() - this._targetCircleRotationStartTime) / 0.1f) % 10;
			if (num >= 5)
			{
				num = 10 - num - 1;
			}
			return num;
		}

		public void FastMoveCameraToMainParty()
		{
			this._mapCameraView.FastMoveCameraToMainParty();
		}

		public void ResetCamera(bool resetDistance, bool teleportToMainParty)
		{
			this._mapCameraView.ResetCamera(resetDistance, teleportToMainParty);
		}

		public void TeleportCameraToMainParty()
		{
			this._mapCameraView.TeleportCameraToMainParty();
		}

		public bool IsCameraLockedToPlayerParty()
		{
			return this._mapCameraView.IsCameraLockedToPlayerParty();
		}

		private const float DoubleClickTimeLimit = 0.3f;

		private MenuViewContext _menuViewContext;

		private MenuContext _latestMenuContext;

		private bool _partyIconNeedsRefreshing;

		private uint _tooltipTargetHash;

		private object _tooltipTargetObject;

		private readonly ObservableCollection<MapView> _mapViews;

		private MapView[] _mapViewsCopyCache;

		private MapView _encounterOverlay;

		private MapView _armyOverlay;

		private MapReadyView _mapReadyView;

		private MapView _escapeMenuView;

		private MapView _battleSimulationView;

		private MapView _mapSiegeOverlayView;

		private MapView _campaignOptionsView;

		private MapView _mapConversationView;

		private MapView _marriageOfferPopupView;

		private MapView _mapCheatsView;

		public MapCameraView _mapCameraView;

		private MapNavigationHandler _navigationHandler = new MapNavigationHandler();

		private const int _frameDelayAmountForRenderActivation = 5;

		private float _timeSinceCreation;

		private bool _leftButtonDraggingMode;

		private UIntPtr _preSelectedSiegeEntityID;

		private Vec2 _oldMousePosition;

		private Vec2 _clickedPositionPixel;

		private Vec3 _clickedPosition;

		private Ray _mouseRay;

		private PartyVisual _preVisualOfSelectedEntity;

		private int _activatedFrameNo = Utilities.EngineFrameNo;

		public Dictionary<Tuple<Material, BannerCode>, Material> _characterBannerMaterialCache = new Dictionary<Tuple<Material, BannerCode>, Material>();

		private Tuple<ConversationCharacterData, ConversationCharacterData, MapScreen.TempConversationStateHandler> _conversationDataCache;

		private readonly int _displayedContextMenuType = -1;

		private double _lastReleaseTime;

		private double _lastPressTime;

		private double _secondLastPressTime;

		private bool _leftButtonDoubleClickOnSceneWidget;

		private float _waitForDoubleClickUntilTime;

		private float _timeToggleTimer = float.MaxValue;

		private bool _ignoreNextTimeToggle;

		private bool _exitOnSaveOver;

		private Scene _mapScene;

		private Campaign _campaign;

		private readonly MapState _mapState;

		private bool _isSceneViewEnabled;

		private bool _isReadyForRender;

		private bool _gpuMemoryCleared;

		private bool _focusLost;

		private bool _isKingdomDecisionsDirty;

		private bool _conversationOverThisFrame;

		private float _cheatPressTimer;

		private Dictionary<Tuple<Material, BannerCode>, Material> _bannerTexturedMaterialCache;

		private GameEntity _targetCircleEntitySmall;

		private GameEntity _targetCircleEntityBig;

		private GameEntity _targetCircleTown;

		private GameEntity _partyOutlineEntity;

		private GameEntity _townOutlineEntity;

		private Decal _targetDecalMeshSmall;

		private Decal _targetDecalMeshBig;

		private Decal _partyOutlineMesh;

		private Decal _settlementOutlineMesh;

		private Decal _targetTownMesh;

		private float _targetCircleRotationStartTime;

		private MapCursor _mapCursor = new MapCursor();

		private bool _mapSceneCursorWanted = true;

		private bool _mapSceneCursorActive;

		public IMapTracksCampaignBehavior MapTracksCampaignBehavior;

		private bool _isSoundOn = true;

		private float _soundCalculationTime;

		private const float SoundCalculationInterval = 0.2f;

		private uint _enemyPartyDecalColor = 4281663744U;

		private uint _allyPartyDecalColor = 4279308800U;

		private uint _neutralPartyDecalColor = 4294919959U;

		private MapColorGradeManager _colorGradeManager;

		private bool _playerSiegeMachineSlotMeshesAdded;

		private GameEntity[] _defenderMachinesCircleEntities;

		private GameEntity[] _attackerRamMachinesCircleEntities;

		private GameEntity[] _attackerTowerMachinesCircleEntities;

		private GameEntity[] _attackerRangedMachinesCircleEntities;

		private string _emptyAttackerRangedDecalMaterialName = "decal_siege_ranged";

		private string _attackerRamMachineDecalMaterialName = "decal_siege_ram";

		private string _attackerTowerMachineDecalMaterialName = "decal_siege_tower";

		private string _attackerRangedMachineDecalMaterialName = "decal_siege_ranged";

		private string _defenderRangedMachineDecalMaterialName = "decal_defender_ranged_siege";

		private uint _preperationOrEnemySiegeEngineDecalColor = 4287064638U;

		private uint _normalStartSiegeEngineDecalColor = 4278394186U;

		private float _defenderMachineCircleDecalScale = 0.25f;

		private float _attackerMachineDecalScale = 0.38f;

		private bool _isNewDecalScaleImplementationEnabled;

		private uint _normalEndSiegeEngineDecalColor = 4284320212U;

		private uint _hoveredSiegeEngineDecalColor = 4293956364U;

		private uint _withMachineSiegeEngineDecalColor = 4283683126U;

		private float _machineDecalAnimLoopTime = 0.5f;

		public bool TooltipHandlingDisabled;

		private readonly UIntPtr[] _intersectedEntityIDs = new UIntPtr[128];

		private readonly Intersection[] _intersectionInfos = new Intersection[128];

		private GameEntity[] _tickedMapEntities;

		private Mesh[] _tickedMapMeshes;

		private readonly List<MBCampaignEvent> _periodicCampaignUIEvents;

		private bool _ignoreLeftMouseRelease;

		private enum TerrainTypeSoundSlot
		{
			dismounted,
			mounted,
			mounted_slow,
			caravan,
			ambient
		}

		private class TempConversationStateHandler : IConversationStateHandler
		{
			void IConversationStateHandler.ExecuteConversationContinue()
			{
				this._actionQueue.Enqueue(delegate
				{
					IConversationStateHandler tempHandler = this._tempHandler;
					if (tempHandler == null)
					{
						return;
					}
					tempHandler.ExecuteConversationContinue();
				});
			}

			void IConversationStateHandler.OnConversationActivate()
			{
				this._actionQueue.Enqueue(delegate
				{
					IConversationStateHandler tempHandler = this._tempHandler;
					if (tempHandler == null)
					{
						return;
					}
					tempHandler.OnConversationActivate();
				});
			}

			void IConversationStateHandler.OnConversationContinue()
			{
				this._actionQueue.Enqueue(delegate
				{
					IConversationStateHandler tempHandler = this._tempHandler;
					if (tempHandler == null)
					{
						return;
					}
					tempHandler.OnConversationContinue();
				});
			}

			void IConversationStateHandler.OnConversationDeactivate()
			{
				this._actionQueue.Enqueue(delegate
				{
					IConversationStateHandler tempHandler = this._tempHandler;
					if (tempHandler == null)
					{
						return;
					}
					tempHandler.OnConversationDeactivate();
				});
			}

			void IConversationStateHandler.OnConversationInstall()
			{
				this._actionQueue.Enqueue(delegate
				{
					IConversationStateHandler tempHandler = this._tempHandler;
					if (tempHandler == null)
					{
						return;
					}
					tempHandler.OnConversationInstall();
				});
			}

			void IConversationStateHandler.OnConversationUninstall()
			{
				this._actionQueue.Enqueue(delegate
				{
					IConversationStateHandler tempHandler = this._tempHandler;
					if (tempHandler == null)
					{
						return;
					}
					tempHandler.OnConversationUninstall();
				});
			}

			public void ApplyHandlerChangesTo(IConversationStateHandler newHandler)
			{
				this._tempHandler = newHandler;
				while (this._actionQueue.Count > 0)
				{
					Action action = this._actionQueue.Dequeue();
					if (action != null)
					{
						action();
					}
				}
				this._tempHandler = null;
			}

			private Queue<Action> _actionQueue = new Queue<Action>();

			private IConversationStateHandler _tempHandler;
		}
	}
}
