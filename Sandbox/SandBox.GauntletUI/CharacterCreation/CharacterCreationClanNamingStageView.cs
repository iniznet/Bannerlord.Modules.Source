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
	// Token: 0x02000038 RID: 56
	[CharacterCreationStageView(typeof(CharacterCreationClanNamingStage))]
	public class CharacterCreationClanNamingStageView : CharacterCreationStageViewBase
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060001F4 RID: 500 RVA: 0x0000DF44 File Offset: 0x0000C144
		private ItemRosterElement ShieldRosterElement
		{
			get
			{
				return this._dataSource.ShieldRosterElement;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060001F5 RID: 501 RVA: 0x0000DF51 File Offset: 0x0000C151
		private int ShieldSlotIndex
		{
			get
			{
				return this._dataSource.ShieldSlotIndex;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x0000DF67 File Offset: 0x0000C167
		// (set) Token: 0x060001F6 RID: 502 RVA: 0x0000DF5E File Offset: 0x0000C15E
		public SceneLayer SceneLayer { get; private set; }

		// Token: 0x060001F8 RID: 504 RVA: 0x0000DF70 File Offset: 0x0000C170
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

		// Token: 0x060001F9 RID: 505 RVA: 0x0000E0EC File Offset: 0x0000C2EC
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

		// Token: 0x060001FA RID: 506 RVA: 0x0000E150 File Offset: 0x0000C350
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

		// Token: 0x060001FB RID: 507 RVA: 0x0000E300 File Offset: 0x0000C500
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

		// Token: 0x060001FC RID: 508 RVA: 0x0000E44D File Offset: 0x0000C64D
		private void UpdateBanners()
		{
			this._banner.GetTableauTextureLarge(new Action<Texture>(this.OnNewBannerReadyForBanners));
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000E468 File Offset: 0x0000C668
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

		// Token: 0x060001FE RID: 510 RVA: 0x0000E504 File Offset: 0x0000C704
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
			this._banner.GetTableauTextureLarge(action);
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000E5EE File Offset: 0x0000C7EE
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

		// Token: 0x06000200 RID: 512 RVA: 0x0000E630 File Offset: 0x0000C830
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

		// Token: 0x06000201 RID: 513 RVA: 0x0000E808 File Offset: 0x0000CA08
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

		// Token: 0x06000202 RID: 514 RVA: 0x0000E95F File Offset: 0x0000CB5F
		public override IEnumerable<ScreenLayer> GetLayers()
		{
			return new List<ScreenLayer> { this.SceneLayer, this.GauntletLayer };
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000E97E File Offset: 0x0000CB7E
		public override int GetVirtualStageCount()
		{
			return 1;
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000E984 File Offset: 0x0000CB84
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

		// Token: 0x06000205 RID: 517 RVA: 0x0000E9D8 File Offset: 0x0000CBD8
		public override void PreviousStage()
		{
			ControlCharacterCreationStage negativeAction = this._negativeAction;
			if (negativeAction == null)
			{
				return;
			}
			negativeAction.Invoke();
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000E9EC File Offset: 0x0000CBEC
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

		// Token: 0x06000207 RID: 519 RVA: 0x0000EA81 File Offset: 0x0000CC81
		public override void LoadEscapeMenuMovie()
		{
			this._escapeMenuDatasource = new EscapeMenuVM(base.GetEscapeMenuItems(this), null);
			this._escapeMenuMovie = this.GauntletLayer.LoadMovie("EscapeMenu", this._escapeMenuDatasource);
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000EAB2 File Offset: 0x0000CCB2
		public override void ReleaseEscapeMenuMovie()
		{
			this.GauntletLayer.ReleaseMovie(this._escapeMenuMovie);
			this._escapeMenuDatasource = null;
			this._escapeMenuMovie = null;
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000EAD4 File Offset: 0x0000CCD4
		private bool IsGameKeyReleasedInAnyLayer(string hotKeyID)
		{
			bool flag = this.IsReleasedInSceneLayer(hotKeyID);
			bool flag2 = this.IsReleasedInGauntletLayer(hotKeyID);
			return flag || flag2;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000EAF2 File Offset: 0x0000CCF2
		private bool IsReleasedInSceneLayer(string hotKeyID)
		{
			SceneLayer sceneLayer = this.SceneLayer;
			return sceneLayer != null && sceneLayer.Input.IsHotKeyReleased(hotKeyID);
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000EB0B File Offset: 0x0000CD0B
		private bool IsReleasedInGauntletLayer(string hotKeyID)
		{
			GauntletLayer gauntletLayer = this.GauntletLayer;
			return gauntletLayer != null && gauntletLayer.Input.IsHotKeyReleased(hotKeyID);
		}

		// Token: 0x04000109 RID: 265
		private CharacterCreation _characterCreation;

		// Token: 0x0400010A RID: 266
		private GauntletLayer GauntletLayer;

		// Token: 0x0400010B RID: 267
		private CharacterCreationClanNamingStageVM _dataSource;

		// Token: 0x0400010C RID: 268
		private IGauntletMovie _clanNamingStageMovie;

		// Token: 0x0400010D RID: 269
		private TextObject _affirmativeActionText;

		// Token: 0x0400010E RID: 270
		private TextObject _negativeActionText;

		// Token: 0x0400010F RID: 271
		private Banner _banner;

		// Token: 0x04000110 RID: 272
		private float _cameraCurrentRotation;

		// Token: 0x04000111 RID: 273
		private float _cameraTargetRotation;

		// Token: 0x04000112 RID: 274
		private float _cameraCurrentDistanceAdder;

		// Token: 0x04000113 RID: 275
		private float _cameraTargetDistanceAdder;

		// Token: 0x04000114 RID: 276
		private float _cameraCurrentElevationAdder;

		// Token: 0x04000115 RID: 277
		private float _cameraTargetElevationAdder;

		// Token: 0x04000116 RID: 278
		private readonly BasicCharacterObject _character;

		// Token: 0x04000117 RID: 279
		private Scene _scene;

		// Token: 0x04000118 RID: 280
		private readonly ActionIndexCache _idleAction = ActionIndexCache.Create("act_walk_idle_1h_with_shield_left_stance");

		// Token: 0x04000119 RID: 281
		private MBAgentRendererSceneController _agentRendererSceneController;

		// Token: 0x0400011A RID: 282
		private AgentVisuals _agentVisuals;

		// Token: 0x0400011B RID: 283
		private MatrixFrame _characterFrame;

		// Token: 0x0400011C RID: 284
		private Equipment _weaponEquipment;

		// Token: 0x0400011D RID: 285
		private Camera _camera;

		// Token: 0x0400011F RID: 287
		private EscapeMenuVM _escapeMenuDatasource;

		// Token: 0x04000120 RID: 288
		private IGauntletMovie _escapeMenuMovie;
	}
}
