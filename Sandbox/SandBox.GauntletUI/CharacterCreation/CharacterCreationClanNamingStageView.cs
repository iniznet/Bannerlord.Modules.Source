using System;
using System.Collections.Generic;
using SandBox.View.CharacterCreation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.CharacterCreation
{
	[CharacterCreationStageView(typeof(CharacterCreationClanNamingStage))]
	public class CharacterCreationClanNamingStageView : CharacterCreationStageViewBase
	{
		private ItemRosterElement ShieldRosterElement
		{
			get
			{
				return this._dataSource.ShieldRosterElement;
			}
		}

		private int ShieldSlotIndex
		{
			get
			{
				return this._dataSource.ShieldSlotIndex;
			}
		}

		public SceneLayer SceneLayer { get; private set; }

		public CharacterCreationClanNamingStageView(CharacterCreation characterCreation, ControlCharacterCreationStage affirmativeAction, TextObject affirmativeActionText, ControlCharacterCreationStage negativeAction, TextObject negativeActionText, ControlCharacterCreationStage refreshAction, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction, ControlCharacterCreationStageReturnInt getTotalStageCountAction, ControlCharacterCreationStageReturnInt getFurthestIndexAction, ControlCharacterCreationStageWithInt goToIndexAction)
			: base(affirmativeAction, negativeAction, refreshAction, getCurrentStageIndexAction, getTotalStageCountAction, getFurthestIndexAction, goToIndexAction)
		{
			this._characterCreation = characterCreation;
			this._affirmativeActionText = affirmativeActionText;
			this._negativeActionText = negativeActionText;
			this.GauntletLayer = new GauntletLayer(1, "GauntletLayer", false)
			{
				IsFocusLayer = true
			};
			this.GauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this.GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			ScreenManager.TrySetFocus(this.GauntletLayer);
			this._character = CharacterObject.PlayerCharacter;
			this._banner = Clan.PlayerClan.Banner;
			this._dataSource = new CharacterCreationClanNamingStageVM(this._character, this._banner, this._characterCreation, new Action(this.NextStage), this._affirmativeActionText, new Action(this.PreviousStage), this._negativeActionText, getCurrentStageIndexAction.Invoke(), getTotalStageCountAction.Invoke(), getFurthestIndexAction.Invoke(), new Action<int>(this.GoToIndex));
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._clanNamingStageMovie = this.GauntletLayer.LoadMovie("CharacterCreationClanNamingStage", this._dataSource);
			this.CreateScene();
			this.RefreshCharacterEntity();
		}

		public override void Tick(float dt)
		{
			this.HandleUserInput();
			this.UpdateCamera(dt);
			if (this.SceneLayer != null && this.SceneLayer.ReadyToRender())
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}
			if (this._scene != null)
			{
				this._scene.Tick(dt);
			}
			base.HandleEscapeMenu(this, this.GauntletLayer);
			this.HandleLayerInput();
		}

		private void CreateScene()
		{
			this._scene = Scene.CreateNewScene(true, false, 0, "mono_renderscene");
			this._scene.SetName("MBBannerEditorScreen");
			SceneInitializationData sceneInitializationData;
			sceneInitializationData..ctor(true);
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
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("FaceGenHotkeyCategory"));
			this.SceneLayer.SetScene(this._scene);
			this.UpdateCamera(0f);
			this.SceneLayer.SetSceneUsesShadows(true);
			this.SceneLayer.SceneView.SetResolutionScaling(true);
			int num = -1;
			num &= -5;
			this.SceneLayer.SetPostfxConfigParams(num);
			this.AddCharacterEntity(this._idleAction);
		}

		private void AddCharacterEntity(ActionIndexCache action)
		{
			this._weaponEquipment = new Equipment();
			int i = 0;
			while (i < 12)
			{
				EquipmentElement equipmentFromSlot = this._character.Equipment.GetEquipmentFromSlot(i);
				ItemObject item = equipmentFromSlot.Item;
				if (((item != null) ? item.PrimaryWeapon : null) == null)
				{
					goto IL_5E;
				}
				ItemObject item2 = equipmentFromSlot.Item;
				if (((item2 != null) ? item2.PrimaryWeapon : null) != null && !equipmentFromSlot.Item.PrimaryWeapon.IsShield)
				{
					goto IL_5E;
				}
				IL_6B:
				i++;
				continue;
				IL_5E:
				this._weaponEquipment.AddEquipmentToSlotWithoutAgent(i, equipmentFromSlot);
				goto IL_6B;
			}
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(this._character.Race);
			this._agentVisuals = AgentVisuals.Create(new AgentVisualsData().Equipment(this._weaponEquipment).BodyProperties(this._character.GetBodyProperties(this._weaponEquipment, -1)).Frame(this._characterFrame)
				.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, this._character.IsFemale, "_facegen"))
				.ActionCode(action)
				.Scene(this._scene)
				.Race(this._character.Race)
				.Monster(baseMonsterFromRace)
				.SkeletonType(this._character.IsFemale ? 1 : 0)
				.PrepareImmediately(true)
				.UseMorphAnims(true), "BannerEditorChar", false, false, true);
			this._agentVisuals.SetAgentLodZeroOrMaxExternal(true);
			this.UpdateBanners();
		}

		private void UpdateBanners()
		{
			BannerVisualExtensions.GetTableauTextureLarge(this._banner, new Action<Texture>(this.OnNewBannerReadyForBanners));
		}

		private void OnNewBannerReadyForBanners(Texture newTexture)
		{
			if (this._scene == null)
			{
				return;
			}
			GameEntity gameEntity = this._scene.FindEntityWithTag("banner");
			if (gameEntity == null)
			{
				return;
			}
			Mesh mesh = gameEntity.GetFirstMesh();
			if (mesh != null && this._banner != null)
			{
				mesh.GetMaterial().SetTexture(1, newTexture);
			}
			gameEntity = this._scene.FindEntityWithTag("banner_2");
			if (gameEntity == null)
			{
				return;
			}
			mesh = gameEntity.GetFirstMesh();
			if (mesh != null && this._banner != null)
			{
				mesh.GetMaterial().SetTexture(1, newTexture);
			}
		}

		private void RefreshCharacterEntity()
		{
			this._weaponEquipment.AddEquipmentToSlotWithoutAgent(this.ShieldSlotIndex, this.ShieldRosterElement.EquipmentElement);
			AgentVisualsData copyAgentVisualsData = this._agentVisuals.GetCopyAgentVisualsData();
			copyAgentVisualsData.Equipment(this._weaponEquipment).RightWieldedItemIndex(-1).LeftWieldedItemIndex(this.ShieldSlotIndex)
				.Banner(this._banner)
				.ClothColor1(this._banner.GetPrimaryColor())
				.ClothColor2(this._banner.GetFirstIconColor());
			this._agentVisuals.Refresh(false, copyAgentVisualsData, false);
			MissionWeapon shieldWeapon = new MissionWeapon(this.ShieldRosterElement.EquipmentElement.Item, this.ShieldRosterElement.EquipmentElement.ItemModifier, this._banner);
			Action<Texture> action = delegate(Texture tex)
			{
				shieldWeapon.GetWeaponData(false).TableauMaterial.SetTexture(1, tex);
			};
			BannerVisualExtensions.GetTableauTextureLarge(this._banner, action);
		}

		private void HandleLayerInput()
		{
			if (this.IsGameKeyReleasedInAnyLayer("Exit"))
			{
				this._dataSource.OnPreviousStage();
				return;
			}
			if (this.IsGameKeyReleasedInAnyLayer("Confirm") && this._dataSource.CanAdvance)
			{
				this._dataSource.OnNextStage();
			}
		}

		private void HandleUserInput()
		{
			if (this.SceneLayer.Input.IsHotKeyReleased("Ascend") || this.SceneLayer.Input.IsHotKeyReleased("Rotate") || this.SceneLayer.Input.IsHotKeyReleased("Zoom"))
			{
				this.GauntletLayer.InputRestrictions.SetMouseVisibility(true);
			}
			Vec2 vec;
			vec..ctor(-this.SceneLayer.Input.GetMouseMoveX(), -this.SceneLayer.Input.GetMouseMoveY());
			if (this.SceneLayer.Input.IsHotKeyDown("Zoom"))
			{
				this._cameraTargetDistanceAdder = MBMath.ClampFloat(this._cameraTargetDistanceAdder + vec.y * 0.002f, 1.5f, 5f);
				MBWindowManager.DontChangeCursorPos();
				this.GauntletLayer.InputRestrictions.SetMouseVisibility(false);
			}
			if (this.SceneLayer.Input.IsHotKeyDown("Rotate"))
			{
				this._cameraTargetRotation = MBMath.WrapAngle(this._cameraTargetRotation - vec.x * 0.004f);
				MBWindowManager.DontChangeCursorPos();
				this.GauntletLayer.InputRestrictions.SetMouseVisibility(false);
			}
			if (this.SceneLayer.Input.IsHotKeyDown("Ascend"))
			{
				this._cameraTargetElevationAdder = MBMath.ClampFloat(this._cameraTargetElevationAdder - vec.y * 0.002f, 0.5f, 1.9f * this._agentVisuals.GetScale());
				MBWindowManager.DontChangeCursorPos();
				this.GauntletLayer.InputRestrictions.SetMouseVisibility(false);
			}
			if (this.SceneLayer.Input.GetDeltaMouseScroll() != 0f)
			{
				this._cameraTargetDistanceAdder = MBMath.ClampFloat(this._cameraTargetDistanceAdder - this.SceneLayer.Input.GetDeltaMouseScroll() * 0.001f, 1.5f, 5f);
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
			characterFrame.rotation.RotateAboutForward(-0.18849556f);
			this._camera.Frame = characterFrame;
			this.SceneLayer.SetCamera(this._camera);
			SoundManager.SetListenerFrame(characterFrame);
		}

		public override IEnumerable<ScreenLayer> GetLayers()
		{
			return new List<ScreenLayer> { this.SceneLayer, this.GauntletLayer };
		}

		public override int GetVirtualStageCount()
		{
			return 1;
		}

		public override void NextStage()
		{
			TextObject textObject = new TextObject(this._dataSource.ClanName, null);
			TextObject textObject2 = GameTexts.FindText("str_generic_clan_name", null);
			textObject2.SetTextVariable("CLAN_NAME", textObject);
			Clan.PlayerClan.ChangeClanName(textObject2, textObject2);
			ControlCharacterCreationStage affirmativeAction = this._affirmativeAction;
			if (affirmativeAction == null)
			{
				return;
			}
			affirmativeAction.Invoke();
		}

		public override void PreviousStage()
		{
			ControlCharacterCreationStage negativeAction = this._negativeAction;
			if (negativeAction == null)
			{
				return;
			}
			negativeAction.Invoke();
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			this.SceneLayer.SceneView.SetEnable(false);
			this.SceneLayer.SceneView.ClearAll(true, true);
			this.GauntletLayer = null;
			this.SceneLayer = null;
			CharacterCreationClanNamingStageVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._dataSource = null;
			this._clanNamingStageMovie = null;
			this._agentVisuals.Reset();
			this._agentVisuals = null;
			MBAgentRendererSceneController.DestructAgentRendererSceneController(this._scene, this._agentRendererSceneController, false);
			this._agentRendererSceneController = null;
			this._scene = null;
		}

		public override void LoadEscapeMenuMovie()
		{
			this._escapeMenuDatasource = new EscapeMenuVM(base.GetEscapeMenuItems(this), null);
			this._escapeMenuMovie = this.GauntletLayer.LoadMovie("EscapeMenu", this._escapeMenuDatasource);
		}

		public override void ReleaseEscapeMenuMovie()
		{
			this.GauntletLayer.ReleaseMovie(this._escapeMenuMovie);
			this._escapeMenuDatasource = null;
			this._escapeMenuMovie = null;
		}

		private bool IsGameKeyReleasedInAnyLayer(string hotKeyID)
		{
			bool flag = this.IsReleasedInSceneLayer(hotKeyID);
			bool flag2 = this.IsReleasedInGauntletLayer(hotKeyID);
			return flag || flag2;
		}

		private bool IsReleasedInSceneLayer(string hotKeyID)
		{
			SceneLayer sceneLayer = this.SceneLayer;
			return sceneLayer != null && sceneLayer.Input.IsHotKeyReleased(hotKeyID);
		}

		private bool IsReleasedInGauntletLayer(string hotKeyID)
		{
			GauntletLayer gauntletLayer = this.GauntletLayer;
			return gauntletLayer != null && gauntletLayer.Input.IsHotKeyReleased(hotKeyID);
		}

		private CharacterCreation _characterCreation;

		private GauntletLayer GauntletLayer;

		private CharacterCreationClanNamingStageVM _dataSource;

		private IGauntletMovie _clanNamingStageMovie;

		private TextObject _affirmativeActionText;

		private TextObject _negativeActionText;

		private Banner _banner;

		private float _cameraCurrentRotation;

		private float _cameraTargetRotation;

		private float _cameraCurrentDistanceAdder;

		private float _cameraTargetDistanceAdder;

		private float _cameraCurrentElevationAdder;

		private float _cameraTargetElevationAdder;

		private readonly BasicCharacterObject _character;

		private Scene _scene;

		private readonly ActionIndexCache _idleAction = ActionIndexCache.Create("act_walk_idle_1h_with_shield_left_stance");

		private MBAgentRendererSceneController _agentRendererSceneController;

		private AgentVisuals _agentVisuals;

		private MatrixFrame _characterFrame;

		private Equipment _weaponEquipment;

		private Camera _camera;

		private EscapeMenuVM _escapeMenuDatasource;

		private IGauntletMovie _escapeMenuMovie;
	}
}
