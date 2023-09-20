using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.MountAndBlade.ViewModelCollection.BannerBuilder;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	[GameStateScreen(typeof(BannerBuilderState))]
	public class GauntletBannerBuilderScreen : ScreenBase, IGameStateListener
	{
		public SceneLayer SceneLayer { get; private set; }

		public GauntletBannerBuilderScreen(BannerBuilderState state)
		{
			this._state = state;
			this._character = MBObjectManager.Instance.GetObject<BasicCharacterObject>("main_hero");
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._bannerIconsCategory = spriteData.SpriteCategories["ui_bannericons"];
			this._bannerIconsCategory.Load(resourceContext, uiresourceDepot);
			this._bannerBuilderCategory = spriteData.SpriteCategories["ui_bannerbuilder"];
			this._bannerBuilderCategory.Load(resourceContext, uiresourceDepot);
			this._agentVisuals = new AgentVisuals[2];
			string text = (string.IsNullOrWhiteSpace(this._state.DefaultBannerKey) ? "11.163.166.1528.1528.764.764.1.0.0.133.171.171.483.483.764.764.0.0.0" : this._state.DefaultBannerKey);
			this._dataSource = new BannerBuilderVM(this._character, text, new Action<bool>(this.Exit), new Action(this.Refresh), new Action(this.CopyBannerCode));
			this._gauntletLayer = new GauntletLayer(100, "GauntletLayer", false);
			this._gauntletLayer.IsFocusLayer = true;
			base.AddLayer(this._gauntletLayer);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			ScreenManager.TrySetFocus(this._gauntletLayer);
			this._movie = this._gauntletLayer.LoadMovie("BannerBuilderScreen", this._dataSource);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("FaceGenHotkeyCategory"));
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this.CreateScene();
			base.AddLayer(this.SceneLayer);
			this._checkWhetherAgentVisualIsReady = true;
			this._firstCharacterRender = true;
			this.RefreshShieldAndCharacter();
		}

		private void Refresh()
		{
			this.RefreshShieldAndCharacter();
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._isFinalized)
			{
				return;
			}
			this.HandleUserInput();
			if (this._isFinalized)
			{
				return;
			}
			this.UpdateCamera(dt);
			SceneLayer sceneLayer = this.SceneLayer;
			if (sceneLayer != null && sceneLayer.ReadyToRender())
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}
			Scene scene = this._scene;
			if (scene != null)
			{
				scene.Tick(dt);
			}
			if (this._refreshBannersNextFrame)
			{
				this.UpdateBanners();
				this._refreshBannersNextFrame = false;
			}
			if (this._refreshCharacterAndShieldNextFrame)
			{
				this.RefreshShieldAndCharacterAux();
				this._refreshCharacterAndShieldNextFrame = false;
			}
			if (this._checkWhetherAgentVisualIsReady)
			{
				int num = (this._agentVisualToShowIndex + 1) % 2;
				if (this._agentVisuals[this._agentVisualToShowIndex].GetEntity().CheckResources(this._firstCharacterRender, true))
				{
					this._agentVisuals[num].SetVisible(false);
					this._agentVisuals[this._agentVisualToShowIndex].SetVisible(true);
					this._checkWhetherAgentVisualIsReady = false;
					this._firstCharacterRender = false;
					return;
				}
				if (!this._firstCharacterRender)
				{
					this._agentVisuals[num].SetVisible(true);
				}
				this._agentVisuals[this._agentVisualToShowIndex].SetVisible(false);
			}
		}

		private void CreateScene()
		{
			this._scene = Scene.CreateNewScene(true, true, 2, "mono_renderscene");
			this._scene.SetName("BannerBuilderScreen");
			SceneInitializationData sceneInitializationData = default(SceneInitializationData);
			sceneInitializationData.InitPhysicsWorld = false;
			this._scene.Read("banner_editor_scene", ref sceneInitializationData, "");
			this._scene.SetShadow(true);
			this._scene.DisableStaticShadows(true);
			this._scene.SetDynamicShadowmapCascadesRadiusMultiplier(0.1f);
			this._agentRendererSceneController = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(this._scene, 32);
			float aspectRatio = Screen.AspectRatio;
			GameEntity gameEntity = this._scene.FindEntityWithTag("spawnpoint_player");
			this._characterFrame = gameEntity.GetFrame();
			this._characterFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			this._cameraTargetDistanceAdder = 3.5f;
			this._cameraCurrentDistanceAdder = this._cameraTargetDistanceAdder;
			this._cameraTargetElevationAdder = 1.15f;
			this._cameraCurrentElevationAdder = this._cameraTargetElevationAdder;
			this._camera = Camera.CreateCamera();
			this._camera.SetFovVertical(0.6981317f, aspectRatio, 0.2f, 200f);
			this.SceneLayer = new SceneLayer("SceneLayer", true, true);
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("FaceGenHotkeyCategory"));
			this.SceneLayer.SetScene(this._scene);
			this.UpdateCamera(0f);
			this.SceneLayer.SetSceneUsesShadows(true);
			this.SceneLayer.SceneView.SetResolutionScaling(true);
			int num = -1;
			num &= -5;
			this.SceneLayer.SetPostfxConfigParams(num);
			this.AddCharacterEntities(this._idleAction);
		}

		private void AddCharacterEntities(ActionIndexCache action)
		{
			this._weaponEquipment = new Equipment();
			for (int i = 0; i < 12; i++)
			{
				EquipmentElement equipmentFromSlot = this._character.Equipment.GetEquipmentFromSlot(i);
				ItemObject item = equipmentFromSlot.Item;
				if (((item != null) ? item.PrimaryWeapon : null) == null || (!equipmentFromSlot.Item.PrimaryWeapon.IsShield && !Extensions.HasAllFlags<ItemFlags>(equipmentFromSlot.Item.ItemFlags, 4096)))
				{
					this._weaponEquipment.AddEquipmentToSlotWithoutAgent(i, equipmentFromSlot);
				}
			}
			this._weaponEquipment.AddEquipmentToSlotWithoutAgent(this._dataSource.ShieldSlotIndex, this._dataSource.ShieldRosterElement.EquipmentElement);
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(this._character.Race);
			this._agentVisuals[0] = AgentVisuals.Create(new AgentVisualsData().Equipment(this._weaponEquipment).BodyProperties(this._character.GetBodyProperties(this._weaponEquipment, -1)).Frame(this._characterFrame)
				.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, this._character.IsFemale, "_facegen"))
				.ActionCode(action)
				.Scene(this._scene)
				.Monster(baseMonsterFromRace)
				.SkeletonType(this._character.IsFemale ? 1 : 0)
				.Race(this._character.Race)
				.PrepareImmediately(true)
				.RightWieldedItemIndex(-1)
				.LeftWieldedItemIndex(this._dataSource.ShieldSlotIndex)
				.ClothColor1(this._dataSource.CurrentBanner.GetPrimaryColor())
				.ClothColor2(this._dataSource.CurrentBanner.GetFirstIconColor())
				.Banner(this._dataSource.CurrentBanner)
				.UseMorphAnims(true), "BannerEditorChar", false, false, true);
			this._agentVisuals[0].SetAgentLodZeroOrMaxExternal(true);
			this._agentVisuals[0].Refresh(false, this._agentVisuals[0].GetCopyAgentVisualsData(), true);
			MissionWeapon shieldWeapon = new MissionWeapon(this._dataSource.ShieldRosterElement.EquipmentElement.Item, this._dataSource.ShieldRosterElement.EquipmentElement.ItemModifier, this._dataSource.CurrentBanner);
			Action<Texture> action2 = delegate(Texture tex)
			{
				shieldWeapon.GetWeaponData(false).TableauMaterial.SetTexture(1, tex);
			};
			this._dataSource.CurrentBanner.GetTableauTextureLarge(action2);
			this._agentVisuals[0].SetVisible(false);
			this._agentVisuals[0].GetEntity().CheckResources(true, true);
			this._agentVisuals[1] = AgentVisuals.Create(new AgentVisualsData().Equipment(this._weaponEquipment).BodyProperties(this._character.GetBodyProperties(this._weaponEquipment, -1)).Frame(this._characterFrame)
				.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, this._character.IsFemale, "_facegen"))
				.ActionCode(action)
				.Scene(this._scene)
				.Race(this._character.Race)
				.Monster(baseMonsterFromRace)
				.SkeletonType(this._character.IsFemale ? 1 : 0)
				.PrepareImmediately(true)
				.RightWieldedItemIndex(-1)
				.LeftWieldedItemIndex(this._dataSource.ShieldSlotIndex)
				.Banner(this._dataSource.CurrentBanner)
				.ClothColor1(this._dataSource.CurrentBanner.GetPrimaryColor())
				.ClothColor2(this._dataSource.CurrentBanner.GetFirstIconColor())
				.UseMorphAnims(true), "BannerEditorChar", false, false, true);
			this._agentVisuals[1].SetAgentLodZeroOrMaxExternal(true);
			this._agentVisuals[1].Refresh(false, this._agentVisuals[1].GetCopyAgentVisualsData(), true);
			this._agentVisuals[1].SetVisible(false);
			this._agentVisuals[1].GetEntity().CheckResources(true, true);
			this.UpdateBanners();
		}

		private void UpdateBanners()
		{
			BannerCode currentBannerCode = BannerCode.CreateFrom(this._dataSource.CurrentBanner);
			this._dataSource.CurrentBanner.GetTableauTextureLarge(delegate(Texture resultTexture)
			{
				this.OnNewBannerReadyForBanners(currentBannerCode, resultTexture);
			});
			if (this._previousBannerCode != null)
			{
				TableauCacheManager tableauCacheManager = TableauCacheManager.Current;
				if (tableauCacheManager != null)
				{
					tableauCacheManager.ForceReleaseBanner(this._previousBannerCode, true, true);
				}
				TableauCacheManager tableauCacheManager2 = TableauCacheManager.Current;
				if (tableauCacheManager2 != null)
				{
					tableauCacheManager2.ForceReleaseBanner(this._previousBannerCode, true, false);
				}
			}
			this._previousBannerCode = BannerCode.CreateFrom(this._dataSource.CurrentBanner);
		}

		private void OnNewBannerReadyForBanners(BannerCode bannerCodeOfTexture, Texture newTexture)
		{
			if (!this._isFinalized && this._scene != null && this._currentBannerCode == bannerCodeOfTexture)
			{
				GameEntity gameEntity = this._scene.FindEntityWithTag("banner");
				if (gameEntity != null)
				{
					Mesh firstMesh = gameEntity.GetFirstMesh();
					if (firstMesh != null && this._dataSource.CurrentBanner != null)
					{
						firstMesh.GetMaterial().SetTexture(1, newTexture);
					}
				}
				else
				{
					gameEntity = this._scene.FindEntityWithTag("banner_2");
					Mesh firstMesh2 = gameEntity.GetFirstMesh();
					if (firstMesh2 != null && this._dataSource.CurrentBanner != null)
					{
						firstMesh2.GetMaterial().SetTexture(1, newTexture);
					}
				}
				this._refreshCharacterAndShieldNextFrame = true;
			}
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._bannerIconsCategory.Unload();
			this._bannerBuilderCategory.Unload();
			this._dataSource.OnFinalize();
			this._isFinalized = true;
		}

		private void RefreshShieldAndCharacter()
		{
			this._currentBannerCode = BannerCode.CreateFrom(this._dataSource.CurrentBanner);
			this._dataSource.BannerCodeAsString = this._currentBannerCode.Code;
			this._refreshBannersNextFrame = true;
		}

		private void RefreshShieldAndCharacterAux()
		{
			int agentVisualToShowIndex = this._agentVisualToShowIndex;
			this._agentVisualToShowIndex = (this._agentVisualToShowIndex + 1) % 2;
			AgentVisualsData copyAgentVisualsData = this._agentVisuals[this._agentVisualToShowIndex].GetCopyAgentVisualsData();
			copyAgentVisualsData.Equipment(this._weaponEquipment).RightWieldedItemIndex(-1).LeftWieldedItemIndex(this._dataSource.ShieldSlotIndex)
				.Banner(this._dataSource.CurrentBanner)
				.Frame(this._characterFrame)
				.BodyProperties(this._character.GetBodyProperties(this._weaponEquipment, -1))
				.ClothColor1(this._dataSource.CurrentBanner.GetPrimaryColor())
				.ClothColor2(this._dataSource.CurrentBanner.GetFirstIconColor());
			this._agentVisuals[this._agentVisualToShowIndex].Refresh(false, copyAgentVisualsData, true);
			this._agentVisuals[this._agentVisualToShowIndex].GetEntity().CheckResources(true, true);
			this._agentVisuals[this._agentVisualToShowIndex].GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(0.001f, this._characterFrame, true);
			this._agentVisuals[this._agentVisualToShowIndex].SetVisible(false);
			this._agentVisuals[this._agentVisualToShowIndex].SetVisible(true);
			this._checkWhetherAgentVisualIsReady = true;
		}

		private void HandleUserInput()
		{
			if (this._gauntletLayer.IsFocusedOnInput())
			{
				return;
			}
			if (this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
			{
				this._dataSource.ExecuteDone();
				return;
			}
			if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				this._dataSource.ExecuteCancel();
				return;
			}
			if (this.SceneLayer.Input.IsHotKeyReleased("Ascend") || this.SceneLayer.Input.IsHotKeyReleased("Rotate") || this.SceneLayer.Input.IsHotKeyReleased("Zoom"))
			{
				this._gauntletLayer.InputRestrictions.SetMouseVisibility(true);
			}
			Vec2 vec;
			vec..ctor(-this.SceneLayer.Input.GetMouseMoveX(), -this.SceneLayer.Input.GetMouseMoveY());
			if (this.SceneLayer.Input.IsHotKeyDown("Zoom"))
			{
				this._cameraTargetDistanceAdder = MBMath.ClampFloat(this._cameraTargetDistanceAdder + vec.y * 0.002f, 1.5f, 5f);
				MBWindowManager.DontChangeCursorPos();
				this._gauntletLayer.InputRestrictions.SetMouseVisibility(false);
			}
			if (this.SceneLayer.Input.IsHotKeyDown("Rotate"))
			{
				this._cameraTargetRotation = MBMath.WrapAngle(this._cameraTargetRotation - vec.x * 0.004f);
				MBWindowManager.DontChangeCursorPos();
				this._gauntletLayer.InputRestrictions.SetMouseVisibility(false);
			}
			if (this.SceneLayer.Input.IsHotKeyDown("Ascend"))
			{
				this._cameraTargetElevationAdder = MBMath.ClampFloat(this._cameraTargetElevationAdder - vec.y * 0.002f, 0.5f, 1.9f * this._agentVisuals[0].GetScale());
				MBWindowManager.DontChangeCursorPos();
				this._gauntletLayer.InputRestrictions.SetMouseVisibility(false);
			}
			if (this.SceneLayer.Input.GetDeltaMouseScroll() != 0f)
			{
				this._cameraTargetDistanceAdder = MBMath.ClampFloat(this._cameraTargetDistanceAdder - this.SceneLayer.Input.GetDeltaMouseScroll() * 0.001f, 1.5f, 5f);
			}
			if (Input.DebugInput.IsHotKeyPressed("Copy"))
			{
				this.CopyBannerCode();
			}
			if (Input.DebugInput.IsHotKeyPressed("Duplicate"))
			{
				this._dataSource.ExecuteDuplicateCurrentLayer();
			}
			if (Input.DebugInput.IsHotKeyPressed("Paste"))
			{
				this._dataSource.SetBannerCode(Input.GetClipboardText());
				this.RefreshShieldAndCharacter();
			}
			if (Input.DebugInput.IsKeyPressed(211))
			{
				this._dataSource.DeleteCurrentLayer();
			}
			Vec2 vec2;
			vec2..ctor(0f, 0f);
			if (Input.DebugInput.IsKeyReleased(203))
			{
				vec2.x = -1f;
			}
			else if (Input.DebugInput.IsKeyReleased(205))
			{
				vec2.x = 1f;
			}
			if (Input.DebugInput.IsKeyReleased(208))
			{
				vec2.y = 1f;
			}
			else if (Input.DebugInput.IsKeyReleased(200))
			{
				vec2.y = -1f;
			}
			if (vec2.x != 0f || vec2.y != 0f)
			{
				this._dataSource.TranslateCurrentLayerWith(vec2);
			}
		}

		private void UpdateCamera(float dt)
		{
			this._cameraCurrentRotation += MBMath.WrapAngle(this._cameraTargetRotation - this._cameraCurrentRotation) * MathF.Min(1f, 10f * dt);
			this._cameraCurrentElevationAdder += MBMath.WrapAngle(this._cameraTargetElevationAdder - this._cameraCurrentElevationAdder) * MathF.Min(1f, 10f * dt);
			this._cameraCurrentDistanceAdder += MBMath.WrapAngle(this._cameraTargetDistanceAdder - this._cameraCurrentDistanceAdder) * MathF.Min(1f, 10f * dt);
			MatrixFrame characterFrame = this._characterFrame;
			characterFrame.rotation.RotateAboutUp(this._cameraCurrentRotation);
			characterFrame.origin += this._cameraCurrentElevationAdder * characterFrame.rotation.u + this._cameraCurrentDistanceAdder * characterFrame.rotation.f;
			characterFrame.rotation.RotateAboutSide(-1.5707964f);
			characterFrame.rotation.RotateAboutUp(3.1415927f);
			characterFrame.rotation.RotateAboutForward(0.18849556f);
			this._camera.Frame = characterFrame;
			this.SceneLayer.SetCamera(this._camera);
			SoundManager.SetListenerFrame(characterFrame);
		}

		private void CopyBannerCode()
		{
			Input.SetClipboardText(this._dataSource.GetBannerCode());
			InformationManager.DisplayMessage(new InformationMessage("Banner code copied to the clipboard."));
		}

		public void Exit(bool isCancel)
		{
			MouseManager.ActivateMouseCursor(1);
			GameStateManager.Current.PopState(0);
		}

		void IGameStateListener.OnActivate()
		{
		}

		void IGameStateListener.OnDeactivate()
		{
			this._agentVisuals[0].Reset();
			this._agentVisuals[1].Reset();
			MBAgentRendererSceneController.DestructAgentRendererSceneController(this._scene, this._agentRendererSceneController, false);
			this._agentRendererSceneController = null;
			this._scene = null;
		}

		void IGameStateListener.OnInitialize()
		{
		}

		void IGameStateListener.OnFinalize()
		{
		}

		private BannerBuilderVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private IGauntletMovie _movie;

		private SpriteCategory _bannerIconsCategory;

		private SpriteCategory _bannerBuilderCategory;

		private BannerBuilderState _state;

		private bool _isFinalized;

		private Camera _camera;

		private AgentVisuals[] _agentVisuals;

		private readonly ActionIndexCache _idleAction = ActionIndexCache.Create("act_walk_idle_1h_with_shield_left_stance");

		private Scene _scene;

		private MBAgentRendererSceneController _agentRendererSceneController;

		private MatrixFrame _characterFrame;

		private Equipment _weaponEquipment;

		private BannerCode _currentBannerCode;

		private float _cameraCurrentRotation;

		private float _cameraTargetRotation;

		private float _cameraCurrentDistanceAdder;

		private float _cameraTargetDistanceAdder;

		private float _cameraCurrentElevationAdder;

		private float _cameraTargetElevationAdder;

		private int _agentVisualToShowIndex;

		private bool _refreshCharacterAndShieldNextFrame;

		private bool _refreshBannersNextFrame;

		private bool _checkWhetherAgentVisualIsReady;

		private bool _firstCharacterRender = true;

		private BannerCode _previousBannerCode;

		private BasicCharacterObject _character;

		private const string DefaultBannerKey = "11.163.166.1528.1528.764.764.1.0.0.133.171.171.483.483.764.764.0.0.0";
	}
}
