using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using TaleWorlds.CampaignSystem.Issues;
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
	// Token: 0x02000050 RID: 80
	[GameStateScreen(typeof(MapState))]
	public class MapScreen : ScreenBase, IMapStateHandler, IGameStateListener
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060002AA RID: 682 RVA: 0x000168C0 File Offset: 0x00014AC0
		// (set) Token: 0x060002AB RID: 683 RVA: 0x000168C8 File Offset: 0x00014AC8
		public CampaignMapSiegePrefabEntityCache PrefabEntityCache { get; private set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060002AC RID: 684 RVA: 0x000168D1 File Offset: 0x00014AD1
		// (set) Token: 0x060002AD RID: 685 RVA: 0x000168D9 File Offset: 0x00014AD9
		public MapEncyclopediaView EncyclopediaScreenManager { get; private set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060002AE RID: 686 RVA: 0x000168E2 File Offset: 0x00014AE2
		// (set) Token: 0x060002AF RID: 687 RVA: 0x000168EA File Offset: 0x00014AEA
		public MapNotificationView MapNotificationView { get; private set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060002B0 RID: 688 RVA: 0x000168F3 File Offset: 0x00014AF3
		public bool IsInMenu
		{
			get
			{
				return this._menuViewContext != null;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060002B1 RID: 689 RVA: 0x000168FE File Offset: 0x00014AFE
		// (set) Token: 0x060002B2 RID: 690 RVA: 0x00016906 File Offset: 0x00014B06
		public bool IsEscapeMenuOpened { get; private set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060002B3 RID: 691 RVA: 0x0001690F File Offset: 0x00014B0F
		// (set) Token: 0x060002B4 RID: 692 RVA: 0x00016917 File Offset: 0x00014B17
		public IPartyVisual CurrentVisualOfTooltip { get; private set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060002B5 RID: 693 RVA: 0x00016920 File Offset: 0x00014B20
		// (set) Token: 0x060002B6 RID: 694 RVA: 0x00016928 File Offset: 0x00014B28
		public SceneLayer SceneLayer { get; private set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060002B7 RID: 695 RVA: 0x00016931 File Offset: 0x00014B31
		public IInputContext Input
		{
			get
			{
				return this.SceneLayer.Input;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x0001693E File Offset: 0x00014B3E
		public bool IsReady
		{
			get
			{
				return this._isReadyForRender;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060002B9 RID: 697 RVA: 0x00016946 File Offset: 0x00014B46
		// (set) Token: 0x060002BA RID: 698 RVA: 0x0001694D File Offset: 0x00014B4D
		public static MapScreen Instance { get; private set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060002BB RID: 699 RVA: 0x00016955 File Offset: 0x00014B55
		// (set) Token: 0x060002BC RID: 700 RVA: 0x0001695D File Offset: 0x00014B5D
		public bool IsInBattleSimulation { get; private set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060002BD RID: 701 RVA: 0x00016966 File Offset: 0x00014B66
		// (set) Token: 0x060002BE RID: 702 RVA: 0x0001696E File Offset: 0x00014B6E
		public bool IsInTownManagement { get; private set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060002BF RID: 703 RVA: 0x00016977 File Offset: 0x00014B77
		// (set) Token: 0x060002C0 RID: 704 RVA: 0x0001697F File Offset: 0x00014B7F
		public bool IsInHideoutTroopManage { get; private set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060002C1 RID: 705 RVA: 0x00016988 File Offset: 0x00014B88
		// (set) Token: 0x060002C2 RID: 706 RVA: 0x00016990 File Offset: 0x00014B90
		public bool IsInArmyManagement { get; private set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060002C3 RID: 707 RVA: 0x00016999 File Offset: 0x00014B99
		// (set) Token: 0x060002C4 RID: 708 RVA: 0x000169A1 File Offset: 0x00014BA1
		public bool IsInRecruitment { get; private set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060002C5 RID: 709 RVA: 0x000169AA File Offset: 0x00014BAA
		// (set) Token: 0x060002C6 RID: 710 RVA: 0x000169B2 File Offset: 0x00014BB2
		public bool IsBarExtended { get; private set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060002C7 RID: 711 RVA: 0x000169BB File Offset: 0x00014BBB
		// (set) Token: 0x060002C8 RID: 712 RVA: 0x000169C3 File Offset: 0x00014BC3
		public bool IsInCampaignOptions { get; private set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060002C9 RID: 713 RVA: 0x000169CC File Offset: 0x00014BCC
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

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060002CA RID: 714 RVA: 0x000169F1 File Offset: 0x00014BF1
		// (set) Token: 0x060002CB RID: 715 RVA: 0x000169F9 File Offset: 0x00014BF9
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

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060002CC RID: 716 RVA: 0x00016A0B File Offset: 0x00014C0B
		// (set) Token: 0x060002CD RID: 717 RVA: 0x00016A13 File Offset: 0x00014C13
		public GameEntity ContourMaskEntity { get; private set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060002CE RID: 718 RVA: 0x00016A1C File Offset: 0x00014C1C
		// (set) Token: 0x060002CF RID: 719 RVA: 0x00016A24 File Offset: 0x00014C24
		public List<Mesh> InactiveLightMeshes { get; private set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x00016A2D File Offset: 0x00014C2D
		// (set) Token: 0x060002D1 RID: 721 RVA: 0x00016A35 File Offset: 0x00014C35
		public List<Mesh> ActiveLightMeshes { get; private set; }

		// Token: 0x060002D2 RID: 722 RVA: 0x00016A40 File Offset: 0x00014C40
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

		// Token: 0x060002D3 RID: 723 RVA: 0x00016BD0 File Offset: 0x00014DD0
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

		// Token: 0x060002D4 RID: 724 RVA: 0x00016C01 File Offset: 0x00014E01
		public void SetupMapTooltipForTrack(Track track)
		{
			if (this._tooltipTargetObject != track)
			{
				this._tooltipTargetObject = track;
				this._tooltipTargetHash = 0U;
				InformationManager.ShowTooltip(typeof(Track), new object[] { track });
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x00016C33 File Offset: 0x00014E33
		public void RemoveMapTooltip()
		{
			if (this._tooltipTargetObject != null || this._tooltipTargetHash != 0U)
			{
				this._tooltipTargetObject = null;
				this._tooltipTargetHash = 0U;
				MBInformationManager.HideInformations();
			}
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00016C58 File Offset: 0x00014E58
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

		// Token: 0x060002D7 RID: 727 RVA: 0x00016CF8 File Offset: 0x00014EF8
		private void HandleSiegeEngineHoverEnd()
		{
			if (this._preSelectedSiegeEntityID != UIntPtr.Zero)
			{
				MapScreen.FrameAndVisualOfEngines[this._preSelectedSiegeEntityID].Item2.OnMapHoverSiegeEngineEnd();
				this._preSelectedSiegeEntityID = UIntPtr.Zero;
			}
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00016D34 File Offset: 0x00014F34
		private void SetCameraOfSceneLayer()
		{
			this.SceneLayer.SetCamera(this._mapCameraView.Camera);
			Vec3 origin = this._mapCameraView.CameraFrame.origin;
			origin.z = 0f;
			this.SceneLayer.SetFocusedShadowmap(false, ref origin, 0f);
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00016D88 File Offset: 0x00014F88
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

		// Token: 0x060002DA RID: 730 RVA: 0x00016E0A File Offset: 0x0001500A
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

		// Token: 0x060002DB RID: 731 RVA: 0x00016E3C File Offset: 0x0001503C
		private void UpdateSoundParametersOfMainMap()
		{
			int num = 0;
			switch (CampaignTime.Now.GetSeasonOfYear)
			{
			case 0:
				num = 3;
				break;
			case 1:
				num = 0;
				break;
			case 2:
				num = 1;
				break;
			case 3:
				num = 2;
				break;
			}
			Campaign.Current.MapSceneWrapper.SetSoundParameters(CampaignTime.Now.CurrentHourInDay, num, this._mapCameraView.CameraFrame.origin.z);
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00016EB4 File Offset: 0x000150B4
		protected override void OnActivate()
		{
			base.OnActivate();
			this._mapCameraView.OnActivate(this._leftButtonDraggingMode, this._clickedPosition);
			this._activatedFrameNo = Utilities.EngineFrameNo;
			this.HandleIfSceneIsReady();
			this.UpdateSoundParametersOfMainMap();
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(4));
			this.SetCameraOfSceneLayer();
			this.RestartAmbientSounds();
			IPartyVisual visuals = PartyBase.MainParty.Visuals;
			if (visuals == null)
			{
				return;
			}
			visuals.SetMapIconAsDirty();
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00016F2A File Offset: 0x0001512A
		public void ClearGPUMemory()
		{
			if (true)
			{
				MapScreen.Instance.SceneLayer.ClearRuntimeGPUMemory(true);
			}
			Texture.ReleaseGpuMemories();
			this._gpuMemoryCleared = true;
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00016F4C File Offset: 0x0001514C
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

		// Token: 0x060002DF RID: 735 RVA: 0x00016FC0 File Offset: 0x000151C0
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
						this.OnEscapeMenuToggled(true);
					}
				}
			}
			this._focusLost = !focusGained;
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0001701C File Offset: 0x0001521C
		public MapView AddMapView<T>(params object[] parameters) where T : MapView, new()
		{
			MapView mapView = SandBoxViewCreator.CreateMapView<T>(parameters);
			mapView.MapScreen = this;
			mapView.MapState = this._mapState;
			this._mapViews.Add(mapView);
			mapView.CreateLayout();
			return mapView;
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x00017058 File Offset: 0x00015258
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

		// Token: 0x060002E2 RID: 738 RVA: 0x000170BC File Offset: 0x000152BC
		public void RemoveMapView(MapView mapView)
		{
			mapView.OnFinalize();
			this._mapViews.Remove(mapView);
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x000170D4 File Offset: 0x000152D4
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

		// Token: 0x060002E4 RID: 740 RVA: 0x00017130 File Offset: 0x00015330
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

		// Token: 0x060002E5 RID: 741 RVA: 0x0001718C File Offset: 0x0001538C
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

		// Token: 0x060002E6 RID: 742 RVA: 0x000171E0 File Offset: 0x000153E0
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

		// Token: 0x060002E7 RID: 743 RVA: 0x00017234 File Offset: 0x00015434
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
			GameEntity firstEntityWithScriptComponent = this._mapScene.GetFirstEntityWithScriptComponent<MapColorGradeManager>();
			if (firstEntityWithScriptComponent != null)
			{
				this._colorGradeManager = firstEntityWithScriptComponent.GetFirstScriptOfType<MapColorGradeManager>();
			}
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x000174AB File Offset: 0x000156AB
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

		// Token: 0x060002E9 RID: 745 RVA: 0x000174C8 File Offset: 0x000156C8
		protected override void OnFinalize()
		{
			for (int i = this._mapViews.Count - 1; i >= 0; i--)
			{
				this._mapViews[i].OnFinalize();
			}
			foreach (Settlement settlement in Settlement.All)
			{
				settlement.Party.Visuals.ReleaseResources();
			}
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
			this._mapScene = null;
			this._campaign = null;
			this._navigationHandler = null;
			this._mapCameraView = null;
			MapScreen.Instance = null;
		}

		// Token: 0x060002EA RID: 746 RVA: 0x000175C8 File Offset: 0x000157C8
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

		// Token: 0x060002EB RID: 747 RVA: 0x00017644 File Offset: 0x00015844
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

		// Token: 0x060002EC RID: 748 RVA: 0x000176C4 File Offset: 0x000158C4
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

		// Token: 0x060002ED RID: 749 RVA: 0x00017790 File Offset: 0x00015990
		void IMapStateHandler.OnMenuModeTick(float dt)
		{
			for (int i = this._mapViews.Count - 1; i >= 0; i--)
			{
				this._mapViews[i].OnMenuModeTick(dt);
			}
		}

		// Token: 0x060002EE RID: 750 RVA: 0x000177C8 File Offset: 0x000159C8
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

		// Token: 0x060002EF RID: 751 RVA: 0x00017850 File Offset: 0x00015A50
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

		// Token: 0x060002F0 RID: 752 RVA: 0x000178D4 File Offset: 0x00015AD4
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
						this.OnEscapeMenuToggled(true);
					}
				}
			}
			this.HandleIfBlockerStatesDisabled();
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x0001797B File Offset: 0x00015B7B
		void IMapStateHandler.StartCameraAnimation(Vec2 targetPosition, float animationStopDuration)
		{
			this._mapCameraView.StartCameraAnimation(targetPosition, animationStopDuration);
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x0001798C File Offset: 0x00015B8C
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
				num2 = this.SceneLayer.Input.GetGameKeyAxis("MovementAxisX") * num4;
				num3 = this.SceneLayer.Input.GetGameKeyAxis("MovementAxisY") * num4;
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

		// Token: 0x060002F3 RID: 755 RVA: 0x00018390 File Offset: 0x00016590
		void IMapStateHandler.Tick(float dt)
		{
			this.UpdateSoundParametersOfMainMap();
			if (this._mapViewsCopyCache.Length != this._mapViews.Count)
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

		// Token: 0x060002F4 RID: 756 RVA: 0x000184B4 File Offset: 0x000166B4
		void IMapStateHandler.OnIdleTick(float dt)
		{
			this.HandleIfSceneIsReady();
			this.RemoveMapTooltip();
			for (int i = this._mapViews.Count - 1; i >= 0; i--)
			{
				this._mapViews[i].OnIdleTick(dt);
			}
			this._conversationOverThisFrame = false;
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00018500 File Offset: 0x00016700
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
						if (this._menuViewContext.MenuContext.GameMenu.IsWaitMenu)
						{
							this._menuViewContext.MenuContext.GameMenu.EndWait();
						}
						leaveMenuOption.RunConsequence(this._menuViewContext.MenuContext);
					}
				}
			}
			else if (Campaign.Current != null && !this.IsInBattleSimulation && !this.IsInArmyManagement)
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
				PartyBase.MainParty.Visuals.SetMapIconAsDirty();
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

		// Token: 0x060002F6 RID: 758 RVA: 0x00018684 File Offset: 0x00016884
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

		// Token: 0x060002F7 RID: 759 RVA: 0x000186E0 File Offset: 0x000168E0
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

		// Token: 0x060002F8 RID: 760 RVA: 0x00018754 File Offset: 0x00016954
		private void ExitMenuContext()
		{
			this._menuViewContext.OnGameStateDeactivate();
			this._menuViewContext.OnDeactivate();
			this._menuViewContext.OnFinalize();
			this._menuViewContext = null;
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0001877E File Offset: 0x0001697E
		private void OpenBannerEditorScreen()
		{
			if (Campaign.Current.IsBannerEditorEnabled)
			{
				this._partyIconNeedsRefreshing = true;
				Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<BannerEditorState>(), 0);
			}
		}

		// Token: 0x060002FA RID: 762 RVA: 0x000187B4 File Offset: 0x000169B4
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

		// Token: 0x060002FB RID: 763 RVA: 0x00018806 File Offset: 0x00016A06
		public void OnExit()
		{
			this._mapCameraView.OnExit();
			MBGameManager.EndGame();
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00018818 File Offset: 0x00016A18
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

		// Token: 0x060002FD RID: 765 RVA: 0x00018894 File Offset: 0x00016A94
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

		// Token: 0x060002FE RID: 766 RVA: 0x00018924 File Offset: 0x00016B24
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

		// Token: 0x060002FF RID: 767 RVA: 0x000189A4 File Offset: 0x00016BA4
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

		// Token: 0x06000300 RID: 768 RVA: 0x00018A84 File Offset: 0x00016C84
		public void FastMoveCameraToPosition(Vec2 target)
		{
			this._mapCameraView.FastMoveCameraToPosition(target, this.IsInMenu);
		}

		// Token: 0x06000301 RID: 769 RVA: 0x00018A98 File Offset: 0x00016C98
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

		// Token: 0x06000302 RID: 770 RVA: 0x00018B64 File Offset: 0x00016D64
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
				IPartyVisual partyVisual = null;
				IPartyVisual partyVisual2 = null;
				bool flag2 = false;
				for (int i = num3 - 1; i >= 0; i--)
				{
					UIntPtr uintPtr = this._intersectedEntityIDs[i];
					if (uintPtr != UIntPtr.Zero)
					{
						IPartyVisual partyVisual3;
						if (MapScreen.VisualsOfEntities.TryGetValue(uintPtr, out partyVisual3) && partyVisual3.IsVisibleOrFadingOut())
						{
							IMapEntity mapEntity = partyVisual3.GetMapEntity();
							Intersection intersection = this._intersectionInfos[i];
							vec3 = zero - intersection.IntersectionPoint;
							float num6 = vec3.Length;
							if (mapEntity.IsMobileEntity)
							{
								num6 -= 1.5f;
							}
							if (num6 < num5)
							{
								num5 = num6;
								if (!mapEntity.IsMobileEntity || mapEntity.AttachedEntity == null)
								{
									partyVisual = partyVisual3;
								}
								else
								{
									partyVisual = mapEntity.AttachedEntity.PartyVisual;
								}
								flag = true;
							}
							if (num6 < num4 && (!mapEntity.IsMobileEntity || (!mapEntity.IsMainEntity() && (mapEntity.AttachedEntity == null || !mapEntity.AttachedEntity.IsMainEntity()))))
							{
								num4 = num6;
								IMapEntity attachedEntity = mapEntity.AttachedEntity;
								partyVisual2 = ((attachedEntity != null) ? attachedEntity.PartyVisual : null) ?? mapEntity.PartyVisual;
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
						IMapEntity mapEntity2 = partyVisual.GetMapEntity();
						if (this.SceneLayer.Input.IsGameKeyPressed(66))
						{
							mapEntity2.OnOpenEncyclopedia();
							this._mapCursor.SetVisible(false);
						}
						ITrackableCampaignObject trackableCampaignObject;
						if ((trackableCampaignObject = mapEntity2 as ITrackableCampaignObject) != null && this.SceneLayer.Input.IsGameKeyPressed(65))
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
						this.OnHoverMapEntity(mapEntity2);
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

		// Token: 0x06000303 RID: 771 RVA: 0x00019004 File Offset: 0x00017204
		private void HandleLeftMouseButtonClick(UIntPtr selectedSiegeEntityID, IPartyVisual visualOfSelectedEntity, Vec3 intersectionPoint, PathFaceRecord mouseOverFaceIndex)
		{
			this._mapCameraView.HandleLeftMouseButtonClick(this.SceneLayer.Input.GetIsMouseActive());
			IMapEntity mapEntity = ((visualOfSelectedEntity != null) ? visualOfSelectedEntity.GetMapEntity() : null);
			if (!this._mapState.AtMenu)
			{
				if (mapEntity != null)
				{
					IMapEntity mapEntity2 = visualOfSelectedEntity.GetMapEntity();
					if (mapEntity2.IsMainEntity())
					{
						MobileParty.MainParty.Ai.SetMoveModeHold();
						return;
					}
					PathFaceRecord faceIndex = Campaign.Current.MapSceneWrapper.GetFaceIndex(mapEntity2.InteractionPosition);
					if (this._mapScene.DoesPathExistBetweenFaces(faceIndex.FaceIndex, MobileParty.MainParty.CurrentNavigationFace.FaceIndex, false) && this._mapCameraView.ProcessCameraInput && PartyBase.MainParty.MapEvent == null)
					{
						if (mapEntity2.OnMapClick(this.SceneLayer.Input.IsHotKeyDown("MapFollowModifier")))
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
							mobileParty2.Party.UpdateVisibilityAndInspected(0f, false);
						}
						foreach (Settlement settlement in Settlement.All)
						{
							settlement.Party.UpdateVisibilityAndInspected(0f, false);
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

		// Token: 0x06000304 RID: 772 RVA: 0x000193BC File Offset: 0x000175BC
		private void OnTerrainClick()
		{
			foreach (MapView mapView in this._mapViews)
			{
				mapView.OnMapTerrainClick();
			}
			this._mapCursor.OnMapTerrainClick();
		}

		// Token: 0x06000305 RID: 773 RVA: 0x00019414 File Offset: 0x00017614
		private void OnSiegeEngineFrameClick(MatrixFrame siegeFrame)
		{
			foreach (MapView mapView in this._mapViews)
			{
				mapView.OnSiegeEngineClick(siegeFrame);
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00019460 File Offset: 0x00017660
		private void InitializeSiegeCircleVisuals()
		{
			IPartyVisual visuals = PlayerSiege.PlayerSiegeEvent.BesiegedSettlement.Party.Visuals;
			MapScene mapScene = Campaign.Current.MapSceneWrapper as MapScene;
			this._defenderMachinesCircleEntities = new GameEntity[visuals.GetDefenderSiegeEngineFrameCount()];
			for (int i = 0; i < visuals.GetDefenderSiegeEngineFrameCount(); i++)
			{
				MatrixFrame defenderSiegeEngineFrameAtIndex = visuals.GetDefenderSiegeEngineFrameAtIndex(i);
				this._defenderMachinesCircleEntities[i] = GameEntity.CreateEmpty(mapScene.Scene, true);
				this._defenderMachinesCircleEntities[i].Name = "dRangedMachineCircle_" + i;
				Decal decal = Decal.CreateDecal(null);
				decal.SetMaterial(Material.GetFromResource(this._defenderRangedMachineDecalMaterialName));
				decal.SetFactor1Linear(this._preperationOrEnemySiegeEngineDecalColor);
				this._defenderMachinesCircleEntities[i].AddComponent(decal);
				MatrixFrame matrixFrame = defenderSiegeEngineFrameAtIndex;
				if (this._isNewDecalScaleImplementationEnabled)
				{
					matrixFrame.Scale(new Vec3(this._defenderMachineCircleDecalScale, this._defenderMachineCircleDecalScale, this._defenderMachineCircleDecalScale, -1f));
				}
				this._defenderMachinesCircleEntities[i].SetGlobalFrame(ref matrixFrame);
				this._defenderMachinesCircleEntities[i].SetVisibilityExcludeParents(true);
				mapScene.Scene.AddDecalInstance(decal, "editor_set", true);
			}
			this._attackerRamMachinesCircleEntities = new GameEntity[visuals.GetAttackerBatteringRamSiegeEngineFrameCount()];
			for (int j = 0; j < visuals.GetAttackerBatteringRamSiegeEngineFrameCount(); j++)
			{
				MatrixFrame attackerBatteringRamSiegeEngineFrameAtIndex = visuals.GetAttackerBatteringRamSiegeEngineFrameAtIndex(j);
				this._attackerRamMachinesCircleEntities[j] = GameEntity.CreateEmpty(mapScene.Scene, true);
				this._attackerRamMachinesCircleEntities[j].Name = "InitializeSiegeCircleVisuals";
				this._attackerRamMachinesCircleEntities[j].Name = "aRamMachineCircle_" + j;
				Decal decal2 = Decal.CreateDecal(null);
				decal2.SetMaterial(Material.GetFromResource(this._attackerRamMachineDecalMaterialName));
				decal2.SetFactor1Linear(this._preperationOrEnemySiegeEngineDecalColor);
				this._attackerRamMachinesCircleEntities[j].AddComponent(decal2);
				MatrixFrame matrixFrame2 = attackerBatteringRamSiegeEngineFrameAtIndex;
				if (this._isNewDecalScaleImplementationEnabled)
				{
					matrixFrame2.Scale(new Vec3(this._attackerMachineDecalScale, this._attackerMachineDecalScale, this._attackerMachineDecalScale, -1f));
				}
				this._attackerRamMachinesCircleEntities[j].SetGlobalFrame(ref matrixFrame2);
				this._attackerRamMachinesCircleEntities[j].SetVisibilityExcludeParents(true);
				mapScene.Scene.AddDecalInstance(decal2, "editor_set", true);
			}
			this._attackerTowerMachinesCircleEntities = new GameEntity[visuals.GetAttackerTowerSiegeEngineFrameCount()];
			for (int k = 0; k < visuals.GetAttackerTowerSiegeEngineFrameCount(); k++)
			{
				MatrixFrame attackerTowerSiegeEngineFrameAtIndex = visuals.GetAttackerTowerSiegeEngineFrameAtIndex(k);
				this._attackerTowerMachinesCircleEntities[k] = GameEntity.CreateEmpty(mapScene.Scene, true);
				this._attackerTowerMachinesCircleEntities[k].Name = "aTowerMachineCircle_" + k;
				Decal decal3 = Decal.CreateDecal(null);
				decal3.SetMaterial(Material.GetFromResource(this._attackerTowerMachineDecalMaterialName));
				decal3.SetFactor1Linear(this._preperationOrEnemySiegeEngineDecalColor);
				this._attackerTowerMachinesCircleEntities[k].AddComponent(decal3);
				MatrixFrame matrixFrame3 = attackerTowerSiegeEngineFrameAtIndex;
				if (this._isNewDecalScaleImplementationEnabled)
				{
					matrixFrame3.Scale(new Vec3(this._attackerMachineDecalScale, this._attackerMachineDecalScale, this._attackerMachineDecalScale, -1f));
				}
				this._attackerTowerMachinesCircleEntities[k].SetGlobalFrame(ref matrixFrame3);
				this._attackerTowerMachinesCircleEntities[k].SetVisibilityExcludeParents(true);
				mapScene.Scene.AddDecalInstance(decal3, "editor_set", true);
			}
			this._attackerRangedMachinesCircleEntities = new GameEntity[visuals.GetAttackerRangedSiegeEngineFrameCount()];
			for (int l = 0; l < visuals.GetAttackerRangedSiegeEngineFrameCount(); l++)
			{
				MatrixFrame attackerRangedSiegeEngineFrameAtIndex = visuals.GetAttackerRangedSiegeEngineFrameAtIndex(l);
				this._attackerRangedMachinesCircleEntities[l] = GameEntity.CreateEmpty(mapScene.Scene, true);
				this._attackerRangedMachinesCircleEntities[l].Name = "aRangedMachineCircle_" + l;
				Decal decal4 = Decal.CreateDecal(null);
				decal4.SetMaterial(Material.GetFromResource(this._emptyAttackerRangedDecalMaterialName));
				decal4.SetFactor1Linear(this._preperationOrEnemySiegeEngineDecalColor);
				this._attackerRangedMachinesCircleEntities[l].AddComponent(decal4);
				MatrixFrame matrixFrame4 = attackerRangedSiegeEngineFrameAtIndex;
				if (this._isNewDecalScaleImplementationEnabled)
				{
					matrixFrame4.Scale(new Vec3(this._attackerMachineDecalScale, this._attackerMachineDecalScale, this._attackerMachineDecalScale, -1f));
				}
				this._attackerRangedMachinesCircleEntities[l].SetGlobalFrame(ref matrixFrame4);
				this._attackerRangedMachinesCircleEntities[l].SetVisibilityExcludeParents(true);
				mapScene.Scene.AddDecalInstance(decal4, "editor_set", true);
			}
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00019890 File Offset: 0x00017A90
		private void TickSiegeMachineCircles()
		{
			SiegeEvent playerSiegeEvent = PlayerSiege.PlayerSiegeEvent;
			bool flag;
			if (playerSiegeEvent.IsPlayerSiegeEvent)
			{
				BesiegerCamp besiegerCamp = playerSiegeEvent.BesiegerCamp;
				if (besiegerCamp == null)
				{
					flag = false;
				}
				else
				{
					MobileParty besiegerParty = besiegerCamp.BesiegerParty;
					bool? flag2 = ((besiegerParty != null) ? new bool?(besiegerParty.IsMainParty) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
			}
			else
			{
				flag = false;
			}
			bool flag4 = flag;
			bool isPreparationComplete = playerSiegeEvent.BesiegerCamp.IsPreparationComplete;
			IPartyVisual visuals = playerSiegeEvent.BesiegedSettlement.Party.Visuals;
			Tuple<MatrixFrame, PartyVisual> tuple = null;
			if (this._preSelectedSiegeEntityID != UIntPtr.Zero)
			{
				tuple = MapScreen.FrameAndVisualOfEngines[this._preSelectedSiegeEntityID];
			}
			for (int i = 0; i < visuals.GetDefenderSiegeEngineFrameCount(); i++)
			{
				bool flag5 = playerSiegeEvent.GetSiegeEventSide(0).SiegeEngines.DeployedRangedSiegeEngines[i] == null;
				bool flag6 = PlayerSiege.PlayerSide > 0;
				string desiredMaterialName = this.GetDesiredMaterialName(true, false, flag5, false);
				Decal decal = this._defenderMachinesCircleEntities[i].GetComponentAtIndex(0, 7) as Decal;
				Material material = decal.GetMaterial();
				if (((material != null) ? material.Name : null) != desiredMaterialName)
				{
					decal.SetMaterial(Material.GetFromResource(desiredMaterialName));
				}
				bool flag7 = tuple != null && this._defenderMachinesCircleEntities[i].GetGlobalFrame().NearlyEquals(tuple.Item1, 1E-05f);
				uint desiredDecalColor = this.GetDesiredDecalColor(isPreparationComplete, flag7, flag6, flag5, flag4);
				if (desiredDecalColor != decal.GetFactor1())
				{
					decal.SetFactor1(desiredDecalColor);
				}
			}
			for (int j = 0; j < visuals.GetAttackerRangedSiegeEngineFrameCount(); j++)
			{
				bool flag8 = playerSiegeEvent.GetSiegeEventSide(1).SiegeEngines.DeployedRangedSiegeEngines[j] == null;
				bool flag9 = PlayerSiege.PlayerSide != 1;
				string desiredMaterialName2 = this.GetDesiredMaterialName(true, true, flag8, false);
				Decal decal2 = this._attackerRangedMachinesCircleEntities[j].GetComponentAtIndex(0, 7) as Decal;
				Material material2 = decal2.GetMaterial();
				if (((material2 != null) ? material2.Name : null) != desiredMaterialName2)
				{
					decal2.SetMaterial(Material.GetFromResource(desiredMaterialName2));
				}
				bool flag10 = tuple != null && this._attackerRangedMachinesCircleEntities[j].GetGlobalFrame().NearlyEquals(tuple.Item1, 1E-05f);
				uint desiredDecalColor2 = this.GetDesiredDecalColor(isPreparationComplete, flag10, flag9, flag8, flag4);
				if (desiredDecalColor2 != decal2.GetFactor1())
				{
					decal2.SetFactor1(desiredDecalColor2);
				}
			}
			for (int k = 0; k < visuals.GetAttackerBatteringRamSiegeEngineFrameCount(); k++)
			{
				bool flag11 = playerSiegeEvent.GetSiegeEventSide(1).SiegeEngines.DeployedMeleeSiegeEngines[k] == null;
				bool flag12 = PlayerSiege.PlayerSide != 1;
				string desiredMaterialName3 = this.GetDesiredMaterialName(false, true, flag11, false);
				Decal decal3 = this._attackerRamMachinesCircleEntities[k].GetComponentAtIndex(0, 7) as Decal;
				Material material3 = decal3.GetMaterial();
				if (((material3 != null) ? material3.Name : null) != desiredMaterialName3)
				{
					decal3.SetMaterial(Material.GetFromResource(desiredMaterialName3));
				}
				bool flag13 = tuple != null && this._attackerRamMachinesCircleEntities[k].GetGlobalFrame().NearlyEquals(tuple.Item1, 1E-05f);
				uint desiredDecalColor3 = this.GetDesiredDecalColor(isPreparationComplete, flag13, flag12, flag11, flag4);
				if (desiredDecalColor3 != decal3.GetFactor1())
				{
					decal3.SetFactor1(desiredDecalColor3);
				}
			}
			for (int l = 0; l < visuals.GetAttackerTowerSiegeEngineFrameCount(); l++)
			{
				bool flag14 = playerSiegeEvent.GetSiegeEventSide(1).SiegeEngines.DeployedMeleeSiegeEngines[visuals.GetAttackerBatteringRamSiegeEngineFrameCount() + l] == null;
				bool flag15 = PlayerSiege.PlayerSide != 1;
				string desiredMaterialName4 = this.GetDesiredMaterialName(false, true, flag14, true);
				Decal decal4 = this._attackerTowerMachinesCircleEntities[l].GetComponentAtIndex(0, 7) as Decal;
				Material material4 = decal4.GetMaterial();
				if (((material4 != null) ? material4.Name : null) != desiredMaterialName4)
				{
					decal4.SetMaterial(Material.GetFromResource(desiredMaterialName4));
				}
				bool flag16 = tuple != null && this._attackerTowerMachinesCircleEntities[l].GetGlobalFrame().NearlyEquals(tuple.Item1, 1E-05f);
				uint desiredDecalColor4 = this.GetDesiredDecalColor(isPreparationComplete, flag16, flag15, flag14, flag4);
				if (desiredDecalColor4 != decal4.GetFactor1())
				{
					decal4.SetFactor1(desiredDecalColor4);
				}
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00019CB8 File Offset: 0x00017EB8
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

		// Token: 0x06000309 RID: 777 RVA: 0x00019D3C File Offset: 0x00017F3C
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

		// Token: 0x0600030A RID: 778 RVA: 0x00019D64 File Offset: 0x00017F64
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

		// Token: 0x0600030B RID: 779 RVA: 0x00019E98 File Offset: 0x00018098
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

		// Token: 0x0600030C RID: 780 RVA: 0x00019EF0 File Offset: 0x000180F0
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
				else if (this.SceneLayer.Input.IsGameKeyPressed(39) && !this.IsInArmyManagement)
				{
					this.OpenEncyclopedia();
					flag = true;
				}
				else if (this.SceneLayer.Input.IsGameKeyPressed(36) && !this.IsInArmyManagement)
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
				if (flag)
				{
					this._mapCursor.SetVisible(false);
				}
			}
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0001A114 File Offset: 0x00018314
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

		// Token: 0x0600030E RID: 782 RVA: 0x0001A1BC File Offset: 0x000183BC
		void IMapStateHandler.OnExitingMenuMode()
		{
			this._latestMenuContext = null;
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0001A1C5 File Offset: 0x000183C5
		void IMapStateHandler.OnEnteringMenuMode(MenuContext menuContext)
		{
			this._latestMenuContext = menuContext;
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0001A1D0 File Offset: 0x000183D0
		void IMapStateHandler.OnMainPartyEncounter()
		{
			for (int i = this._mapViews.Count - 1; i >= 0; i--)
			{
				this._mapViews[i].OnMainPartyEncounter();
			}
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0001A206 File Offset: 0x00018406
		void IMapStateHandler.OnSignalPeriodicEvents()
		{
			this.DeleteMarkedPeriodicEvents();
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0001A20E File Offset: 0x0001840E
		void IMapStateHandler.OnBattleSimulationStarted(BattleSimulation battleSimulation)
		{
			this.IsInBattleSimulation = true;
			this._battleSimulationView = this.AddMapView<BattleSimulationMapView>(new object[] { battleSimulation });
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0001A22D File Offset: 0x0001842D
		void IMapStateHandler.OnBattleSimulationEnded()
		{
			this.IsInBattleSimulation = false;
			this.RemoveMapView(this._battleSimulationView);
			this._battleSimulationView = null;
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0001A249 File Offset: 0x00018449
		void IMapStateHandler.OnSiegeEngineClick(MatrixFrame siegeEngineFrame)
		{
			this._mapCameraView.SiegeEngineClick(siegeEngineFrame);
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0001A257 File Offset: 0x00018457
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0001A259 File Offset: 0x00018459
		void IMapStateHandler.OnPlayerSiegeActivated()
		{
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0001A25B File Offset: 0x0001845B
		void IMapStateHandler.OnPlayerSiegeDeactivated()
		{
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0001A25D File Offset: 0x0001845D
		void IGameStateListener.OnActivate()
		{
		}

		// Token: 0x06000319 RID: 793 RVA: 0x0001A25F File Offset: 0x0001845F
		void IGameStateListener.OnDeactivate()
		{
		}

		// Token: 0x0600031A RID: 794 RVA: 0x0001A264 File Offset: 0x00018464
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

		// Token: 0x0600031B RID: 795 RVA: 0x0001A2C0 File Offset: 0x000184C0
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

		// Token: 0x0600031C RID: 796 RVA: 0x0001A37C File Offset: 0x0001857C
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

		// Token: 0x0600031D RID: 797 RVA: 0x0001A3E1 File Offset: 0x000185E1
		private void HandleMapConversationOver()
		{
			if (this._mapConversationView != null)
			{
				this.RemoveMapView(this._mapConversationView);
			}
			this._mapConversationView = null;
		}

		// Token: 0x0600031E RID: 798 RVA: 0x0001A400 File Offset: 0x00018600
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
			this.ContourMaskEntity = GameEntity.CreateEmpty(mapScene.Scene, true);
			this.ContourMaskEntity.Name = "aContourMask";
		}

		// Token: 0x0600031F RID: 799 RVA: 0x0001A6B4 File Offset: 0x000188B4
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
					matrixFrame = partyBase.Visuals.CircleLocalFrame;
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
				matrixFrame3.origin = this.CurrentVisualOfTooltip.GetGlobalFrame().origin + this.CurrentVisualOfTooltip.CircleLocalFrame.origin;
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

		// Token: 0x06000320 RID: 800 RVA: 0x0001AEE4 File Offset: 0x000190E4
		public void SetIsInTownManagement(bool isInTownManagement)
		{
			if (this.IsInTownManagement != isInTownManagement)
			{
				this.IsInTownManagement = isInTownManagement;
			}
		}

		// Token: 0x06000321 RID: 801 RVA: 0x0001AEF6 File Offset: 0x000190F6
		public void SetIsInHideoutTroopManage(bool isInHideoutTroopManage)
		{
			if (this.IsInHideoutTroopManage != isInHideoutTroopManage)
			{
				this.IsInHideoutTroopManage = isInHideoutTroopManage;
			}
		}

		// Token: 0x06000322 RID: 802 RVA: 0x0001AF08 File Offset: 0x00019108
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

		// Token: 0x06000323 RID: 803 RVA: 0x0001AF32 File Offset: 0x00019132
		public void SetIsInRecruitment(bool isInRecruitment)
		{
			if (this.IsInRecruitment != isInRecruitment)
			{
				this.IsInRecruitment = isInRecruitment;
			}
		}

		// Token: 0x06000324 RID: 804 RVA: 0x0001AF44 File Offset: 0x00019144
		public void SetIsBarExtended(bool isBarExtended)
		{
			if (this.IsBarExtended != isBarExtended)
			{
				this.IsBarExtended = isBarExtended;
			}
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0001AF56 File Offset: 0x00019156
		public void SetIsInCampaignOptions(bool isInCampaignOptions)
		{
			if (this.IsInCampaignOptions != isInCampaignOptions)
			{
				this.IsInCampaignOptions = isInCampaignOptions;
			}
		}

		// Token: 0x06000326 RID: 806 RVA: 0x0001AF68 File Offset: 0x00019168
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
			MBMapScene.SetSeasonTimeFactor(this._mapScene, Campaign.Current.Models.MapWeatherModel.GetSeasonTimeFactor());
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

		// Token: 0x06000327 RID: 807 RVA: 0x0001B0B8 File Offset: 0x000192B8
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

		// Token: 0x06000328 RID: 808 RVA: 0x0001B120 File Offset: 0x00019320
		private void StepSounds(MobileParty party)
		{
			if (party.IsVisible && ((PartyVisual)party.Party.Visuals).HumanAgentVisuals != null && party.MemberRoster.TotalManCount > 0)
			{
				TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(party.CurrentNavigationFace);
				AgentVisuals agentVisuals = null;
				int num = 0;
				if (((PartyVisual)party.Party.Visuals).CaravanMountAgentVisuals != null)
				{
					num = 3;
					agentVisuals = ((PartyVisual)party.Party.Visuals).CaravanMountAgentVisuals;
				}
				else if (((PartyVisual)party.Party.Visuals).HumanAgentVisuals != null)
				{
					if (((PartyVisual)party.Party.Visuals).MountAgentVisuals != null)
					{
						num = 1;
						agentVisuals = ((PartyVisual)party.Party.Visuals).MountAgentVisuals;
					}
					else
					{
						num = 0;
						agentVisuals = ((PartyVisual)party.Party.Visuals).HumanAgentVisuals;
					}
				}
				MBMapScene.TickStepSound(this._mapScene, agentVisuals.GetVisuals(), faceTerrainType, num);
			}
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0001B221 File Offset: 0x00019421
		public void SetMouseVisible(bool value)
		{
			this.SceneLayer.InputRestrictions.SetMouseVisibility(value);
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0001B234 File Offset: 0x00019434
		public bool GetMouseVisible()
		{
			return MBMapScene.GetMouseVisible();
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0001B23B File Offset: 0x0001943B
		public void RestartAmbientSounds()
		{
			if (this._mapScene != null)
			{
				this._mapScene.ResumeSceneSounds();
			}
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0001B256 File Offset: 0x00019456
		void IGameStateListener.OnFinalize()
		{
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0001B258 File Offset: 0x00019458
		public void PauseAmbientSounds()
		{
			if (this._mapScene != null)
			{
				this._mapScene.PauseSceneSounds();
			}
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0001B273 File Offset: 0x00019473
		public void StopSoundSceneProps()
		{
			if (this._mapScene != null)
			{
				this._mapScene.FinishSceneSounds();
			}
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0001B28E File Offset: 0x0001948E
		public void ApplySoundSceneProps(float dt)
		{
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0001B290 File Offset: 0x00019490
		private void CollectTickableMapMeshes()
		{
			this._tickedMapEntities = this._mapScene.FindEntitiesWithTag("ticked_map_entity").ToArray<GameEntity>();
			this._tickedMapMeshes = new Mesh[this._tickedMapEntities.Length];
			for (int i = 0; i < this._tickedMapEntities.Length; i++)
			{
				this._tickedMapMeshes[i] = this._tickedMapEntities[i].GetFirstMesh();
			}
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0001B2F3 File Offset: 0x000194F3
		public void OnPauseTick(float dt)
		{
			this.ApplySoundSceneProps(dt);
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000332 RID: 818 RVA: 0x0001B2FC File Offset: 0x000194FC
		public static Dictionary<UIntPtr, IPartyVisual> VisualsOfEntities
		{
			get
			{
				return SandBoxViewSubModule.VisualsOfEntities;
			}
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0001B304 File Offset: 0x00019504
		public MBCampaignEvent CreatePeriodicUIEvent(CampaignTime triggerPeriod, CampaignTime initialWait)
		{
			MBCampaignEvent mbcampaignEvent = new MBCampaignEvent(triggerPeriod, initialWait);
			this._periodicCampaignUIEvents.Add(mbcampaignEvent);
			return mbcampaignEvent;
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000334 RID: 820 RVA: 0x0001B326 File Offset: 0x00019526
		internal static Dictionary<UIntPtr, Tuple<MatrixFrame, PartyVisual>> FrameAndVisualOfEngines
		{
			get
			{
				return SandBoxViewSubModule.FrameAndVisualOfEngines;
			}
		}

		// Token: 0x06000335 RID: 821 RVA: 0x0001B32D File Offset: 0x0001952D
		[CommandLineFunctionality.CommandLineArgumentFunction("disable_core_game", "debug")]
		public static string DisableCoreGame(List<string> parameters)
		{
			Utilities.DisableCoreGame();
			return "Done";
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0001B339 File Offset: 0x00019539
		[CommandLineFunctionality.CommandLineArgumentFunction("gather_core_game_references", "items")]
		public static string GatherCoreGameReferences(List<string> parameters)
		{
			Utilities.GatherCoreGameReferences("SandBoxCore:battle_terrain_029#Native:main_menu_a#Native:scn_soldier#Native:inventory_character_scene#Native:scn_item_tableau");
			return "Done";
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0001B34C File Offset: 0x0001954C
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

		// Token: 0x06000338 RID: 824 RVA: 0x0001B390 File Offset: 0x00019590
		public void DeletePeriodicUIEvent(MBCampaignEvent campaignEvent)
		{
			campaignEvent.isEventDeleted = true;
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0001B399 File Offset: 0x00019599
		private static float CalculateCameraElevation(float cameraDistance)
		{
			return cameraDistance * 0.5f * 0.015f + 0.35f;
		}

		// Token: 0x0600033A RID: 826 RVA: 0x0001B3AE File Offset: 0x000195AE
		public void OpenOptions()
		{
			ScreenManager.PushScreen(ViewCreator.CreateOptionsScreen(false));
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0001B3BB File Offset: 0x000195BB
		public void OpenEncyclopedia()
		{
			Campaign.Current.EncyclopediaManager.GoToLink("LastPage", "");
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0001B3D6 File Offset: 0x000195D6
		[CommandLineFunctionality.CommandLineArgumentFunction("export_nav_mesh_face_marks", "items")]
		public static string ExportNavMeshFaceMarks(List<string> strings)
		{
			if (strings.Count == 0)
			{
				return "Parameter missing!";
			}
			return Utilities.ExportNavMeshFaceMarks(strings[0]);
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0001B3F2 File Offset: 0x000195F2
		[CommandLineFunctionality.CommandLineArgumentFunction("find_meshes_without_lods", "items")]
		public static string FindMeshesWithoutLods(List<string> strings)
		{
			if (strings.Count != 1)
			{
				return "find_meshes_without_lods takes the module name as the only argument!";
			}
			Utilities.FindMeshesWithoutLods(strings[0]);
			return "Processing Asynchronously...";
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0001B414 File Offset: 0x00019614
		public void OpenSaveLoad(bool isSaving)
		{
			ScreenManager.PushScreen(SandBoxViewCreator.CreateSaveLoadScreen(isSaving));
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0001B421 File Offset: 0x00019621
		[CommandLineFunctionality.CommandLineArgumentFunction("take_ss_from_top", "items")]
		public static string TakeSSFromTop(List<string> strings)
		{
			return Utilities.TakeSSFromTop("");
		}

		// Token: 0x06000340 RID: 832 RVA: 0x0001B42D File Offset: 0x0001962D
		public void CloseEscapeMenu()
		{
			this.OnEscapeMenuToggled(false);
		}

		// Token: 0x06000341 RID: 833 RVA: 0x0001B438 File Offset: 0x00019638
		[CommandLineFunctionality.CommandLineArgumentFunction("print_all_items_with_generated_lods", "items")]
		public static string PrintAllItemsWithLod(List<string> strings)
		{
			HashSet<string> hashSet = new HashSet<string>();
			if (strings.Count == 2)
			{
				string text = strings[1];
				if (File.Exists(text))
				{
					foreach (string text2 in File.ReadAllLines(text))
					{
						if (!hashSet.Contains(text2))
						{
							hashSet.Add(text2);
						}
					}
				}
			}
			if (Game.Current == null || Campaign.Current == null)
			{
				return "Campaign was not started.";
			}
			if (strings.Count == 0 || (strings.Count == 1 && strings[0] != "all" && strings[0] != "armor" && strings[0] != "crafting"))
			{
				return "Enter any filter (all, armor, crafting)";
			}
			string text3 = "";
			HashSet<string> hashSet2 = new HashSet<string>();
			if (strings[0] == "all")
			{
				List<MetaMesh> list = new List<MetaMesh>();
				MetaMesh.GetAllMultiMeshes(ref list);
				using (List<MetaMesh>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MetaMesh metaMesh = enumerator.Current;
						bool flag = metaMesh.HasAnyGeneratedLods();
						string name = metaMesh.GetName();
						if (!hashSet2.Contains(name) && !hashSet.Contains(name) && flag)
						{
							text3 += name;
							text3 += "\n";
							hashSet2.Add(name);
						}
					}
					return text3;
				}
			}
			if (strings[0] == "armor")
			{
				using (List<ItemObject>.Enumerator enumerator2 = Game.Current.ObjectManager.GetObjectTypeList<ItemObject>().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ItemObject itemObject = enumerator2.Current;
						MetaMesh copy = MetaMesh.GetCopy(itemObject.MultiMeshName, false, true);
						if (!(copy == null))
						{
							bool flag2 = copy.HasAnyGeneratedLods();
							if (!hashSet2.Contains(itemObject.MultiMeshName) && !hashSet.Contains(itemObject.MultiMeshName) && flag2)
							{
								text3 += itemObject.MultiMeshName;
								text3 += "\n";
								hashSet2.Add(itemObject.MultiMeshName);
							}
						}
					}
					return text3;
				}
			}
			if (strings[0] == "crafting")
			{
				foreach (CraftingPiece craftingPiece in Game.Current.ObjectManager.GetObjectTypeList<CraftingPiece>())
				{
					MetaMesh copy2 = MetaMesh.GetCopy(craftingPiece.MeshName, false, true);
					if (!(copy2 == null))
					{
						bool flag3 = copy2.HasAnyGeneratedLods();
						if (!hashSet2.Contains(craftingPiece.MeshName) && !hashSet.Contains(craftingPiece.MeshName) && flag3)
						{
							text3 += craftingPiece.MeshName;
							text3 += "\n";
							hashSet2.Add(craftingPiece.MeshName);
						}
					}
				}
			}
			return text3;
		}

		// Token: 0x06000342 RID: 834 RVA: 0x0001B75C File Offset: 0x0001995C
		public void OpenEscapeMenu()
		{
			this.OnEscapeMenuToggled(true);
		}

		// Token: 0x06000343 RID: 835 RVA: 0x0001B768 File Offset: 0x00019968
		[CommandLineFunctionality.CommandLineArgumentFunction("illumination", "global")]
		private static string TryGlobalIllumination(List<string> values)
		{
			string text = "";
			foreach (Settlement settlement in MBObjectManager.Instance.GetObjectTypeList<Settlement>())
			{
				if (settlement.Culture != null)
				{
					string[] array = new string[5];
					array[0] = text;
					int num = 1;
					Vec2 vec = settlement.Position2D;
					array[num] = vec.x.ToString();
					array[2] = ",";
					int num2 = 3;
					vec = settlement.Position2D;
					array[num2] = vec.y.ToString();
					array[4] = ",";
					text = string.Concat(array);
					text += settlement.MapFaction.Color;
					text += "-";
				}
			}
			MBMapScene.GetGlobalIlluminationOfString((ScreenManager.TopScreen as MapScreen)._mapScene, text);
			return "";
		}

		// Token: 0x06000344 RID: 836 RVA: 0x0001B854 File Offset: 0x00019A54
		[CommandLineFunctionality.CommandLineArgumentFunction("print_all_items_without_lod", "items")]
		public static string PrintAllItemsWithoutLod(List<string> strings)
		{
			HashSet<string> hashSet = new HashSet<string>();
			if (strings.Count == 2)
			{
				string text = strings[1];
				if (File.Exists(text))
				{
					foreach (string text2 in File.ReadAllLines(text))
					{
						if (!hashSet.Contains(text2))
						{
							hashSet.Add(text2);
						}
					}
				}
			}
			if (Game.Current == null || Campaign.Current == null)
			{
				return "Campaign was not started.";
			}
			if (strings.Count == 0 || (strings.Count == 1 && strings[0] != "all" && strings[0] != "armor" && strings[0] != "crafting"))
			{
				return "Enter any filter (all, armor, crafting)";
			}
			string text3 = "";
			HashSet<string> hashSet2 = new HashSet<string>();
			if (strings[0] == "all")
			{
				HashSet<string> hashSet3 = new HashSet<string>();
				Module.GetItemMeshNames(hashSet3);
				using (HashSet<string>.Enumerator enumerator = hashSet3.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text4 = enumerator.Current;
						MetaMesh multiMesh = MetaMesh.GetMultiMesh(text4);
						if (!(multiMesh == null))
						{
							bool flag = multiMesh.HasAnyLods();
							if (hashSet3.Contains(text4) && !hashSet2.Contains(text4) && !hashSet.Contains(text4) && !flag)
							{
								text3 += text4;
								text3 += "\n";
								hashSet2.Add(text4);
							}
						}
					}
					return text3;
				}
			}
			if (strings[0] == "armor")
			{
				using (List<ItemObject>.Enumerator enumerator2 = Game.Current.ObjectManager.GetObjectTypeList<ItemObject>().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ItemObject itemObject = enumerator2.Current;
						MetaMesh copy = MetaMesh.GetCopy(itemObject.MultiMeshName, false, true);
						if (!(copy == null))
						{
							bool flag2 = copy.HasAnyLods();
							if (!hashSet2.Contains(itemObject.MultiMeshName) && !hashSet.Contains(itemObject.MultiMeshName) && !flag2)
							{
								text3 += itemObject.MultiMeshName;
								text3 += "\n";
								hashSet2.Add(itemObject.MultiMeshName);
							}
						}
					}
					return text3;
				}
			}
			if (strings[0] == "crafting")
			{
				foreach (CraftingPiece craftingPiece in Game.Current.ObjectManager.GetObjectTypeList<CraftingPiece>())
				{
					MetaMesh copy2 = MetaMesh.GetCopy(craftingPiece.MeshName, false, true);
					if (!(copy2 == null))
					{
						bool flag3 = copy2.HasAnyLods();
						if (!hashSet2.Contains(craftingPiece.MeshName) && !hashSet.Contains(craftingPiece.MeshName) && !flag3)
						{
							text3 += craftingPiece.MeshName;
							text3 += "\n";
							hashSet2.Add(craftingPiece.MeshName);
						}
					}
				}
			}
			return text3;
		}

		// Token: 0x06000345 RID: 837 RVA: 0x0001BB7C File Offset: 0x00019D7C
		[CommandLineFunctionality.CommandLineArgumentFunction("print_all_items_without_generated_lod", "items")]
		public static string PrintAllItemsWithoutGeneratedLod(List<string> strings)
		{
			HashSet<string> hashSet = new HashSet<string>();
			if (strings.Count == 2)
			{
				string text = strings[1];
				if (File.Exists(text))
				{
					foreach (string text2 in File.ReadAllLines(text))
					{
						if (!hashSet.Contains(text2))
						{
							hashSet.Add(text2);
						}
					}
				}
			}
			if (Game.Current == null || Campaign.Current == null)
			{
				return "Campaign was not started.";
			}
			if (strings.Count == 0 || (strings.Count == 1 && strings[0] != "all" && strings[0] != "armor" && strings[0] != "crafting"))
			{
				return "Enter any filter (all, armor, crafting)";
			}
			string text3 = "";
			HashSet<string> hashSet2 = new HashSet<string>();
			if (strings[0] == "all")
			{
				List<MetaMesh> list = new List<MetaMesh>();
				MetaMesh.GetAllMultiMeshes(ref list);
				Module.GetItemMeshNames(new HashSet<string>());
				using (List<MetaMesh>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MetaMesh metaMesh = enumerator.Current;
						bool flag = metaMesh.HasAnyGeneratedLods();
						string name = metaMesh.GetName();
						if (!hashSet2.Contains(name) && !hashSet.Contains(name) && !flag)
						{
							text3 += name;
							text3 += "\n";
							hashSet2.Add(name);
						}
					}
					return text3;
				}
			}
			if (strings[0] == "armor")
			{
				using (List<ItemObject>.Enumerator enumerator2 = Game.Current.ObjectManager.GetObjectTypeList<ItemObject>().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ItemObject itemObject = enumerator2.Current;
						MetaMesh copy = MetaMesh.GetCopy(itemObject.MultiMeshName, false, true);
						if (!(copy == null))
						{
							bool flag2 = copy.HasAnyGeneratedLods();
							if (!hashSet2.Contains(itemObject.MultiMeshName) && !hashSet.Contains(itemObject.MultiMeshName) && !flag2)
							{
								text3 += itemObject.MultiMeshName;
								text3 += "\n";
								hashSet2.Add(itemObject.MultiMeshName);
							}
						}
					}
					return text3;
				}
			}
			if (strings[0] == "crafting")
			{
				foreach (CraftingPiece craftingPiece in Game.Current.ObjectManager.GetObjectTypeList<CraftingPiece>())
				{
					MetaMesh copy2 = MetaMesh.GetCopy(craftingPiece.MeshName, false, true);
					if (!(copy2 == null))
					{
						bool flag3 = copy2.HasAnyGeneratedLods();
						if (!hashSet2.Contains(craftingPiece.MeshName) && !hashSet.Contains(craftingPiece.MeshName) && !flag3)
						{
							text3 += craftingPiece.MeshName;
							text3 += "\n";
							hashSet2.Add(craftingPiece.MeshName);
						}
					}
				}
			}
			return text3;
		}

		// Token: 0x06000346 RID: 838 RVA: 0x0001BE94 File Offset: 0x0001A094
		[CommandLineFunctionality.CommandLineArgumentFunction("print_faulty_materials", "items")]
		public static string PrintFaultyMaterials(List<string> strings)
		{
			if (Game.Current == null || Campaign.Current == null)
			{
				return "Campaign was not started.";
			}
			if (strings.Count == 0 || (strings.Count == 1 && strings[0] != "all" && strings[0] != "armor" && strings[0] != "crafting"))
			{
				return "Enter any filter (all, armor, crafting)";
			}
			List<string> list = new List<string>();
			list.Add("use_detailnormalmap");
			list.Add("alpha_test");
			list.Add("use_parallaxmapping");
			list.Add("use_tesselation");
			list.Add("self_illumination");
			list.Add("alignment_deformation_with_offset");
			list.Add("alpha_blend");
			list.Add("two_sided");
			list.Add("multipass_alpha");
			list.Add("needs_forward_rendering");
			list.Add("alpha_sort");
			Dictionary<string, HashSet<string>> dictionary = new Dictionary<string, HashSet<string>>();
			for (int i = 0; i < list.Count; i++)
			{
				dictionary[list[i]] = new HashSet<string>();
			}
			List<Material> list2 = new List<Material>();
			if (strings[0] == "all")
			{
				List<MetaMesh> list3 = new List<MetaMesh>();
				MetaMesh.GetAllMultiMeshes(ref list3);
				using (List<MetaMesh>.Enumerator enumerator = list3.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MetaMesh metaMesh = enumerator.Current;
						for (int j = 0; j < metaMesh.MeshCount; j++)
						{
							Material material = metaMesh.GetMeshAtIndex(j).GetMaterial();
							list2.Add(material);
						}
					}
					goto IL_2BE;
				}
			}
			if (strings[0] == "armor")
			{
				using (List<ItemObject>.Enumerator enumerator2 = Game.Current.ObjectManager.GetObjectTypeList<ItemObject>().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ItemObject itemObject = enumerator2.Current;
						MetaMesh copy = MetaMesh.GetCopy(itemObject.MultiMeshName, false, true);
						if (!(copy == null))
						{
							for (int k = 0; k < copy.MeshCount; k++)
							{
								Material material2 = copy.GetMeshAtIndex(k).GetMaterial();
								list2.Add(material2);
							}
						}
					}
					goto IL_2BE;
				}
			}
			if (strings[0] == "crafting")
			{
				foreach (CraftingPiece craftingPiece in Game.Current.ObjectManager.GetObjectTypeList<CraftingPiece>())
				{
					MetaMesh copy2 = MetaMesh.GetCopy(craftingPiece.MeshName, false, true);
					if (!(copy2 == null))
					{
						for (int l = 0; l < copy2.MeshCount; l++)
						{
							Material material3 = copy2.GetMeshAtIndex(l).GetMaterial();
							list2.Add(material3);
						}
					}
				}
			}
			IL_2BE:
			foreach (Material material4 in list2)
			{
				foreach (string text in list)
				{
					if (text == "alpha_blend")
					{
						Material.MBAlphaBlendMode alphaBlendMode = material4.GetAlphaBlendMode();
						if (alphaBlendMode == 2 || alphaBlendMode == 1 || alphaBlendMode == 4 || alphaBlendMode == 7)
						{
							dictionary[text].Add(material4.Name);
						}
					}
					else if (text == "two_sided")
					{
						if (Extensions.HasAnyFlag<MaterialFlags>(material4.Flags, 32))
						{
							dictionary[text].Add(material4.Name);
						}
					}
					else if (text == "multipass_alpha")
					{
						if (Extensions.HasAnyFlag<MaterialFlags>(material4.Flags, 262144))
						{
							dictionary[text].Add(material4.Name);
						}
					}
					else if (text == "needs_forward_rendering")
					{
						if (Extensions.HasAnyFlag<MaterialFlags>(material4.Flags, 1048576))
						{
							dictionary[text].Add(material4.Name);
						}
					}
					else if (text == "alpha_sort")
					{
						if (Extensions.HasAnyFlag<MaterialFlags>(material4.Flags, 64))
						{
							dictionary[text].Add(material4.Name);
						}
					}
					else
					{
						ulong materialShaderFlagMask = material4.GetShader().GetMaterialShaderFlagMask(text, false);
						if ((material4.GetShaderFlags() & materialShaderFlagMask) != 0UL)
						{
							dictionary[text].Add(material4.Name);
						}
					}
				}
			}
			string text2 = "";
			foreach (string text3 in list)
			{
				HashSet<string> hashSet = dictionary[text3];
				text2 += "--------------------------------\n";
				text2 = string.Concat(new object[] { text2, "!!", text3, " ", hashSet.Count, " --------------------\n" });
				foreach (string text4 in hashSet)
				{
					text2 = text2 + text4 + "\n";
				}
				text2 += "--------------------------------\n";
			}
			return text2;
		}

		// Token: 0x06000347 RID: 839 RVA: 0x0001C4A4 File Offset: 0x0001A6A4
		public void CloseCampaignOptions()
		{
			if (this._campaignOptionsView != null)
			{
				this.RemoveMapView(this._campaignOptionsView);
			}
			this.IsInCampaignOptions = false;
		}

		// Token: 0x06000348 RID: 840 RVA: 0x0001C4C4 File Offset: 0x0001A6C4
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

		// Token: 0x06000349 RID: 841 RVA: 0x0001C6B8 File Offset: 0x0001A8B8
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

		// Token: 0x0600034A RID: 842 RVA: 0x0001C71B File Offset: 0x0001A91B
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

		// Token: 0x0600034B RID: 843 RVA: 0x0001C74C File Offset: 0x0001A94C
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

		// Token: 0x0600034C RID: 844 RVA: 0x0001C774 File Offset: 0x0001A974
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

		// Token: 0x0600034D RID: 845 RVA: 0x0001C7D0 File Offset: 0x0001A9D0
		private void OnExitToMainMenu()
		{
			this.OnEscapeMenuToggled(false);
			InformationManager.HideInquiry();
			this.OnExit();
		}

		// Token: 0x0600034E RID: 846 RVA: 0x0001C7E4 File Offset: 0x0001A9E4
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

		// Token: 0x0600034F RID: 847 RVA: 0x0001C822 File Offset: 0x0001AA22
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

		// Token: 0x06000350 RID: 848 RVA: 0x0001C860 File Offset: 0x0001AA60
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

		// Token: 0x06000351 RID: 849 RVA: 0x0001C89E File Offset: 0x0001AA9E
		public void OpenFacegenScreenAux()
		{
			this.OpenFaceGeneratorScreen();
		}

		// Token: 0x06000352 RID: 850 RVA: 0x0001C8A8 File Offset: 0x0001AAA8
		private int GetCircleIndex()
		{
			int num = (int)((MBCommon.GetApplicationTime() - this._targetCircleRotationStartTime) / 0.1f) % 10;
			if (num >= 5)
			{
				num = 10 - num - 1;
			}
			return num;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0001C8D8 File Offset: 0x0001AAD8
		[CommandLineFunctionality.CommandLineArgumentFunction("export_issue_statistics", "campaign")]
		public static string ExportIssueStatistics(List<string> strings)
		{
			if (Campaign.Current == null)
			{
				return "Campaign was not started.";
			}
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			string text = "IssueStats";
			int num = (int)Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow;
			PlatformDirectoryPath statisticsPath = FilePaths.StatisticsPath;
			PlatformFilePath platformFilePath;
			platformFilePath..ctor(statisticsPath, string.Concat(new object[] { text, "_", num, ".txt" }));
			if (CampaignCheats.CheckHelp(strings))
			{
				string text2 = "";
				text2 = string.Concat(new object[] { text2, "Writes current issue statistics to ", platformFilePath, "\n" });
				return text2 + "File format: " + text + "_<CampaignElapsedTimeInDays>.txt";
			}
			MemoryStream memoryStream = new MemoryStream();
			Dictionary<Type, int> dictionary = new Dictionary<Type, int>();
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			Dictionary<Type, IssueBase.IssueFrequency> dictionary2 = new Dictionary<Type, IssueBase.IssueFrequency>();
			foreach (KeyValuePair<Hero, IssueBase> keyValuePair in Campaign.Current.IssueManager.Issues)
			{
				Type type = keyValuePair.Value.GetType();
				if (!dictionary.ContainsKey(type))
				{
					dictionary.Add(type, 1);
					dictionary2.Add(type, keyValuePair.Value.GetFrequency());
				}
				else
				{
					dictionary[type]++;
				}
				num2++;
				if (keyValuePair.Key.IsLord)
				{
					num4++;
				}
				else
				{
					num3++;
				}
			}
			StreamWriter streamWriter = new StreamWriter(memoryStream);
			streamWriter.WriteLine("Time (Days): " + num);
			streamWriter.WriteLine("Total Issue Count : " + num2);
			streamWriter.WriteLine("Noble Issues Count : " + num4);
			streamWriter.WriteLine("Notable Issues Count : " + num3 + "\n");
			streamWriter.WriteLine("Unique Issue Types : " + dictionary.Count + "\n");
			foreach (KeyValuePair<Type, int> keyValuePair2 in dictionary.OrderBy((KeyValuePair<Type, int> p) => p.Key.Name.ToString().GetHashCode()))
			{
				string text3 = keyValuePair2.Key.ToString();
				text3 = text3.Substring(text3.LastIndexOf('+') + 1);
				string text4 = string.Concat(keyValuePair2.Value);
				string text5 = string.Format("{0:0.000}", (float)keyValuePair2.Value / (float)num2);
				streamWriter.WriteLine(string.Concat(new string[]
				{
					text3,
					"\t",
					text4,
					"\t",
					text5,
					"\t",
					dictionary2[keyValuePair2.Key].ToString()
				}));
			}
			int num5 = 10;
			int[] array = new int[10];
			foreach (Settlement settlement in Settlement.All.Where((Settlement s) => s.IsTown))
			{
				int num6 = MBMath.ClampInt((int)settlement.Town.Security, 0, 99) / num5;
				array[num6]++;
			}
			streamWriter.WriteLine("\nTown Security Histogram (#Bins: 10, Range: 0-100) : ");
			string text6 = "[";
			for (int i = 0; i < array.Length; i++)
			{
				text6 = string.Concat(new object[]
				{
					text6,
					i * num5,
					"-",
					(i + 1) * num5,
					":",
					array[i],
					(i < array.Length - 1) ? ", " : ""
				});
			}
			text6 += "]";
			streamWriter.WriteLine(text6);
			streamWriter.Close();
			byte[] array2 = memoryStream.ToArray();
			FileHelper.SaveFile(platformFilePath, array2);
			return "Issue statistics successfully exported to " + platformFilePath;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0001CD78 File Offset: 0x0001AF78
		[CommandLineFunctionality.CommandLineArgumentFunction("open_retirement_screen", "gameover")]
		public static string OpenGameOverRetirement(List<string> strings)
		{
			GameOverState gameOverState = Game.Current.GameStateManager.CreateState<GameOverState>(new object[] { 0 });
			Game.Current.GameStateManager.PushState(gameOverState, 0);
			return "DONE";
		}

		// Token: 0x06000355 RID: 853 RVA: 0x0001CDBC File Offset: 0x0001AFBC
		[CommandLineFunctionality.CommandLineArgumentFunction("open_victory_screen", "gameover")]
		public static string OpenGameOverVictory(List<string> strings)
		{
			GameOverState gameOverState = Game.Current.GameStateManager.CreateState<GameOverState>(new object[] { 2 });
			Game.Current.GameStateManager.PushState(gameOverState, 0);
			return "DONE";
		}

		// Token: 0x06000356 RID: 854 RVA: 0x0001CDFE File Offset: 0x0001AFFE
		public void FastMoveCameraToMainParty()
		{
			this._mapCameraView.FastMoveCameraToMainParty();
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0001CE0C File Offset: 0x0001B00C
		[CommandLineFunctionality.CommandLineArgumentFunction("open_clan_destroyed_screen", "gameover")]
		public static string OpenGameOverDestroyed(List<string> strings)
		{
			GameOverState gameOverState = Game.Current.GameStateManager.CreateState<GameOverState>(new object[] { 1 });
			Game.Current.GameStateManager.PushState(gameOverState, 0);
			return "DONE";
		}

		// Token: 0x06000358 RID: 856 RVA: 0x0001CE4E File Offset: 0x0001B04E
		[CommandLineFunctionality.CommandLineArgumentFunction("remove_all_circle_notifications", "campaign")]
		public static string ClearAllCircleNotifications(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			MapScreen.Instance.MapNotificationView.ResetNotifications();
			return "Cleared";
		}

		// Token: 0x06000359 RID: 857 RVA: 0x0001CE76 File Offset: 0x0001B076
		public bool IsCameraLockedToPlayerParty()
		{
			return this._mapCameraView.IsCameraLockedToPlayerParty();
		}

		// Token: 0x0600035A RID: 858 RVA: 0x0001CE83 File Offset: 0x0001B083
		public void ResetCamera(bool resetDistance, bool teleportToMainParty)
		{
			this._mapCameraView.ResetCamera(resetDistance, teleportToMainParty);
		}

		// Token: 0x0600035B RID: 859 RVA: 0x0001CE92 File Offset: 0x0001B092
		public void TeleportCameraToMainParty()
		{
			this._mapCameraView.TeleportCameraToMainParty();
		}

		// Token: 0x0400014E RID: 334
		private const float DoubleClickTimeLimit = 0.3f;

		// Token: 0x04000152 RID: 338
		private MenuViewContext _menuViewContext;

		// Token: 0x04000153 RID: 339
		private MenuContext _latestMenuContext;

		// Token: 0x04000154 RID: 340
		private bool _partyIconNeedsRefreshing;

		// Token: 0x04000155 RID: 341
		private uint _tooltipTargetHash;

		// Token: 0x04000156 RID: 342
		private object _tooltipTargetObject;

		// Token: 0x04000157 RID: 343
		private readonly ObservableCollection<MapView> _mapViews;

		// Token: 0x04000158 RID: 344
		private MapView[] _mapViewsCopyCache;

		// Token: 0x04000159 RID: 345
		private MapView _encounterOverlay;

		// Token: 0x0400015A RID: 346
		private MapView _armyOverlay;

		// Token: 0x0400015B RID: 347
		private MapReadyView _mapReadyView;

		// Token: 0x0400015C RID: 348
		private MapView _escapeMenuView;

		// Token: 0x0400015D RID: 349
		private MapView _battleSimulationView;

		// Token: 0x0400015E RID: 350
		private MapView _mapSiegeOverlayView;

		// Token: 0x0400015F RID: 351
		private MapView _campaignOptionsView;

		// Token: 0x04000160 RID: 352
		private MapView _mapConversationView;

		// Token: 0x04000161 RID: 353
		public MapCameraView _mapCameraView;

		// Token: 0x04000162 RID: 354
		private MapNavigationHandler _navigationHandler = new MapNavigationHandler();

		// Token: 0x04000165 RID: 357
		private const int _frameDelayAmountForRenderActivation = 5;

		// Token: 0x04000166 RID: 358
		private float _timeSinceCreation;

		// Token: 0x04000167 RID: 359
		private bool _leftButtonDraggingMode;

		// Token: 0x04000168 RID: 360
		private UIntPtr _preSelectedSiegeEntityID;

		// Token: 0x04000169 RID: 361
		private Vec2 _oldMousePosition;

		// Token: 0x0400016A RID: 362
		private Vec2 _clickedPositionPixel;

		// Token: 0x0400016B RID: 363
		private Vec3 _clickedPosition;

		// Token: 0x0400016C RID: 364
		private Ray _mouseRay;

		// Token: 0x0400016D RID: 365
		private IPartyVisual _preVisualOfSelectedEntity;

		// Token: 0x0400016E RID: 366
		private int _activatedFrameNo = Utilities.EngineFrameNo;

		// Token: 0x0400016F RID: 367
		public Dictionary<Tuple<Material, BannerCode>, Material> _characterBannerMaterialCache = new Dictionary<Tuple<Material, BannerCode>, Material>();

		// Token: 0x04000170 RID: 368
		private Tuple<ConversationCharacterData, ConversationCharacterData, MapScreen.TempConversationStateHandler> _conversationDataCache;

		// Token: 0x04000171 RID: 369
		private readonly int _displayedContextMenuType = -1;

		// Token: 0x04000172 RID: 370
		private double _lastReleaseTime;

		// Token: 0x04000173 RID: 371
		private double _lastPressTime;

		// Token: 0x04000174 RID: 372
		private double _secondLastPressTime;

		// Token: 0x04000175 RID: 373
		private bool _leftButtonDoubleClickOnSceneWidget;

		// Token: 0x04000176 RID: 374
		private float _waitForDoubleClickUntilTime;

		// Token: 0x04000177 RID: 375
		private float _timeToggleTimer = float.MaxValue;

		// Token: 0x04000178 RID: 376
		private bool _ignoreNextTimeToggle;

		// Token: 0x04000179 RID: 377
		private bool _exitOnSaveOver;

		// Token: 0x0400017B RID: 379
		private Scene _mapScene;

		// Token: 0x0400017C RID: 380
		private Campaign _campaign;

		// Token: 0x0400017D RID: 381
		private readonly MapState _mapState;

		// Token: 0x0400017E RID: 382
		private bool _isSceneViewEnabled;

		// Token: 0x0400017F RID: 383
		private bool _isReadyForRender;

		// Token: 0x04000180 RID: 384
		private bool _gpuMemoryCleared;

		// Token: 0x04000181 RID: 385
		private bool _focusLost;

		// Token: 0x04000183 RID: 387
		private bool _isKingdomDecisionsDirty;

		// Token: 0x04000184 RID: 388
		private bool _conversationOverThisFrame;

		// Token: 0x0400018C RID: 396
		private Dictionary<Tuple<Material, BannerCode>, Material> _bannerTexturedMaterialCache;

		// Token: 0x0400018D RID: 397
		private GameEntity _targetCircleEntitySmall;

		// Token: 0x0400018E RID: 398
		private GameEntity _targetCircleEntityBig;

		// Token: 0x0400018F RID: 399
		private GameEntity _targetCircleTown;

		// Token: 0x04000190 RID: 400
		private GameEntity _partyOutlineEntity;

		// Token: 0x04000191 RID: 401
		private GameEntity _townOutlineEntity;

		// Token: 0x04000192 RID: 402
		private Decal _targetDecalMeshSmall;

		// Token: 0x04000193 RID: 403
		private Decal _targetDecalMeshBig;

		// Token: 0x04000194 RID: 404
		private Decal _partyOutlineMesh;

		// Token: 0x04000195 RID: 405
		private Decal _settlementOutlineMesh;

		// Token: 0x04000196 RID: 406
		private Decal _targetTownMesh;

		// Token: 0x04000197 RID: 407
		private float _targetCircleRotationStartTime;

		// Token: 0x04000198 RID: 408
		private MapCursor _mapCursor = new MapCursor();

		// Token: 0x04000199 RID: 409
		private bool _mapSceneCursorWanted = true;

		// Token: 0x0400019A RID: 410
		private bool _mapSceneCursorActive;

		// Token: 0x0400019E RID: 414
		public IMapTracksCampaignBehavior MapTracksCampaignBehavior;

		// Token: 0x0400019F RID: 415
		private bool _isSoundOn = true;

		// Token: 0x040001A0 RID: 416
		private float _soundCalculationTime;

		// Token: 0x040001A1 RID: 417
		private const float SoundCalculationInterval = 0.2f;

		// Token: 0x040001A2 RID: 418
		private uint _enemyPartyDecalColor = 4281663744U;

		// Token: 0x040001A3 RID: 419
		private uint _allyPartyDecalColor = 4279308800U;

		// Token: 0x040001A4 RID: 420
		private uint _neutralPartyDecalColor = 4294919959U;

		// Token: 0x040001A5 RID: 421
		private MapColorGradeManager _colorGradeManager;

		// Token: 0x040001A6 RID: 422
		private bool _playerSiegeMachineSlotMeshesAdded;

		// Token: 0x040001A7 RID: 423
		private GameEntity[] _defenderMachinesCircleEntities;

		// Token: 0x040001A8 RID: 424
		private GameEntity[] _attackerRamMachinesCircleEntities;

		// Token: 0x040001A9 RID: 425
		private GameEntity[] _attackerTowerMachinesCircleEntities;

		// Token: 0x040001AA RID: 426
		private GameEntity[] _attackerRangedMachinesCircleEntities;

		// Token: 0x040001AB RID: 427
		private string _emptyAttackerRangedDecalMaterialName = "decal_siege_ranged";

		// Token: 0x040001AC RID: 428
		private string _attackerRamMachineDecalMaterialName = "decal_siege_ram";

		// Token: 0x040001AD RID: 429
		private string _attackerTowerMachineDecalMaterialName = "decal_siege_tower";

		// Token: 0x040001AE RID: 430
		private string _attackerRangedMachineDecalMaterialName = "decal_siege_ranged";

		// Token: 0x040001AF RID: 431
		private string _defenderRangedMachineDecalMaterialName = "decal_defender_ranged_siege";

		// Token: 0x040001B0 RID: 432
		private uint _preperationOrEnemySiegeEngineDecalColor = 4287064638U;

		// Token: 0x040001B1 RID: 433
		private uint _normalStartSiegeEngineDecalColor = 4278394186U;

		// Token: 0x040001B2 RID: 434
		private float _defenderMachineCircleDecalScale = 0.25f;

		// Token: 0x040001B3 RID: 435
		private float _attackerMachineDecalScale = 0.38f;

		// Token: 0x040001B4 RID: 436
		private bool _isNewDecalScaleImplementationEnabled;

		// Token: 0x040001B5 RID: 437
		private uint _normalEndSiegeEngineDecalColor = 4284320212U;

		// Token: 0x040001B6 RID: 438
		private uint _hoveredSiegeEngineDecalColor = 4293956364U;

		// Token: 0x040001B7 RID: 439
		private uint _withMachineSiegeEngineDecalColor = 4283683126U;

		// Token: 0x040001B8 RID: 440
		private float _machineDecalAnimLoopTime = 0.5f;

		// Token: 0x040001B9 RID: 441
		public bool TooltipHandlingDisabled;

		// Token: 0x040001BA RID: 442
		private readonly UIntPtr[] _intersectedEntityIDs = new UIntPtr[128];

		// Token: 0x040001BB RID: 443
		private readonly Intersection[] _intersectionInfos = new Intersection[128];

		// Token: 0x040001BC RID: 444
		private GameEntity[] _tickedMapEntities;

		// Token: 0x040001BD RID: 445
		private Mesh[] _tickedMapMeshes;

		// Token: 0x040001BE RID: 446
		private readonly List<MBCampaignEvent> _periodicCampaignUIEvents;

		// Token: 0x040001BF RID: 447
		private bool _ignoreLeftMouseRelease;

		// Token: 0x0200008E RID: 142
		private enum TerrainTypeSoundSlot
		{
			// Token: 0x040002F0 RID: 752
			dismounted,
			// Token: 0x040002F1 RID: 753
			mounted,
			// Token: 0x040002F2 RID: 754
			mounted_slow,
			// Token: 0x040002F3 RID: 755
			caravan,
			// Token: 0x040002F4 RID: 756
			ambient
		}

		// Token: 0x0200008F RID: 143
		private class TempConversationStateHandler : IConversationStateHandler
		{
			// Token: 0x06000461 RID: 1121 RVA: 0x00022EEB File Offset: 0x000210EB
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

			// Token: 0x06000462 RID: 1122 RVA: 0x00022F04 File Offset: 0x00021104
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

			// Token: 0x06000463 RID: 1123 RVA: 0x00022F1D File Offset: 0x0002111D
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

			// Token: 0x06000464 RID: 1124 RVA: 0x00022F36 File Offset: 0x00021136
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

			// Token: 0x06000465 RID: 1125 RVA: 0x00022F4F File Offset: 0x0002114F
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

			// Token: 0x06000466 RID: 1126 RVA: 0x00022F68 File Offset: 0x00021168
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

			// Token: 0x06000467 RID: 1127 RVA: 0x00022F81 File Offset: 0x00021181
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

			// Token: 0x040002F5 RID: 757
			private Queue<Action> _actionQueue = new Queue<Action>();

			// Token: 0x040002F6 RID: 758
			private IConversationStateHandler _tempHandler;
		}
	}
}
