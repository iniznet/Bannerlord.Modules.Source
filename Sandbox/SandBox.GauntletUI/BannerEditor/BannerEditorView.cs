using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection;
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
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.BannerEditor
{
	// Token: 0x0200003E RID: 62
	public class BannerEditorView
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000259 RID: 601 RVA: 0x00011434 File Offset: 0x0000F634
		// (set) Token: 0x0600025A RID: 602 RVA: 0x0001143C File Offset: 0x0000F63C
		public GauntletLayer GauntletLayer { get; private set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600025B RID: 603 RVA: 0x00011445 File Offset: 0x0000F645
		// (set) Token: 0x0600025C RID: 604 RVA: 0x0001144D File Offset: 0x0000F64D
		public BannerEditorVM DataSource { get; private set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600025D RID: 605 RVA: 0x00011456 File Offset: 0x0000F656
		// (set) Token: 0x0600025E RID: 606 RVA: 0x0001145E File Offset: 0x0000F65E
		public Banner Banner { get; private set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600025F RID: 607 RVA: 0x00011467 File Offset: 0x0000F667
		private ItemRosterElement ShieldRosterElement
		{
			get
			{
				return this.DataSource.ShieldRosterElement;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000260 RID: 608 RVA: 0x00011474 File Offset: 0x0000F674
		private int ShieldSlotIndex
		{
			get
			{
				return this.DataSource.ShieldSlotIndex;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000262 RID: 610 RVA: 0x0001148A File Offset: 0x0000F68A
		// (set) Token: 0x06000261 RID: 609 RVA: 0x00011481 File Offset: 0x0000F681
		public SceneLayer SceneLayer { get; private set; }

		// Token: 0x06000263 RID: 611 RVA: 0x00011494 File Offset: 0x0000F694
		public BannerEditorView(BasicCharacterObject character, Banner banner, ControlCharacterCreationStage affirmativeAction, TextObject affirmativeActionText, ControlCharacterCreationStage negativeAction, TextObject negativeActionText, ControlCharacterCreationStage onRefresh = null, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction = null, ControlCharacterCreationStageReturnInt getTotalStageCountAction = null, ControlCharacterCreationStageReturnInt getFurthestIndexAction = null, ControlCharacterCreationStageWithInt goToIndexAction = null)
		{
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._spriteCategory = spriteData.SpriteCategories["ui_bannericons"];
			this._spriteCategory.Load(resourceContext, uiresourceDepot);
			this._character = character;
			this.Banner = banner;
			this._goToIndexAction = goToIndexAction;
			if (getCurrentStageIndexAction == null || getTotalStageCountAction == null || getFurthestIndexAction == null)
			{
				this.DataSource = new BannerEditorVM(this._character, this.Banner, new Action<bool>(this.Exit), new Action(this.RefreshShieldAndCharacter), 0, 0, 0, new Action<int>(this.GoToIndex));
				this.DataSource.Description = new TextObject("{=3ZO5cMLu}Customize your banner's sigil", null).ToString();
				this._isOpenedFromCharacterCreation = true;
			}
			else
			{
				this.DataSource = new BannerEditorVM(this._character, this.Banner, new Action<bool>(this.Exit), new Action(this.RefreshShieldAndCharacter), getCurrentStageIndexAction.Invoke(), getTotalStageCountAction.Invoke(), getFurthestIndexAction.Invoke(), new Action<int>(this.GoToIndex));
				this.DataSource.Description = new TextObject("{=312lNJTM}Customize your personal banner by choosing your clan's sigil", null).ToString();
				this._isOpenedFromCharacterCreation = false;
			}
			this.DataSource.DoneText = affirmativeActionText.ToString();
			this.DataSource.CancelText = negativeActionText.ToString();
			this.GauntletLayer = new GauntletLayer(1, "GauntletLayer", false);
			this._gauntletmovie = this.GauntletLayer.LoadMovie("BannerEditor", this.DataSource);
			this.GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this.GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("FaceGenHotkeyCategory"));
			this.GauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this.GauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this.GauntletLayer);
			this.DataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this.DataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._affirmativeAction = affirmativeAction;
			this._negativeAction = negativeAction;
			this._agentVisuals = new AgentVisuals[2];
			this._currentBannerCode = BannerCode.CreateFrom(this.Banner);
			this.CreateScene();
			MBMusicManagerOld.SetMood(1, 0.5f, true, false);
			Input.ClearKeys();
			this._weaponEquipment.AddEquipmentToSlotWithoutAgent(this.ShieldSlotIndex, this.ShieldRosterElement.EquipmentElement);
			AgentVisualsData copyAgentVisualsData = this._agentVisuals[0].GetCopyAgentVisualsData();
			copyAgentVisualsData.Equipment(this._weaponEquipment).RightWieldedItemIndex(-1).LeftWieldedItemIndex(this.ShieldSlotIndex)
				.Banner(this.Banner)
				.ClothColor1(this.Banner.GetPrimaryColor())
				.ClothColor2(this.Banner.GetFirstIconColor());
			this._agentVisuals[0].Refresh(false, copyAgentVisualsData, true);
			MissionWeapon shieldWeapon = new MissionWeapon(this.ShieldRosterElement.EquipmentElement.Item, this.ShieldRosterElement.EquipmentElement.ItemModifier, this.Banner);
			Action<Texture> action = delegate(Texture tex)
			{
				shieldWeapon.GetWeaponData(false).TableauMaterial.SetTexture(1, tex);
			};
			this.Banner.GetTableauTextureLarge(action);
			this._agentVisuals[0].SetVisible(false);
			this._agentVisuals[0].GetEntity().CheckResources(true, true);
			AgentVisualsData copyAgentVisualsData2 = this._agentVisuals[1].GetCopyAgentVisualsData();
			copyAgentVisualsData2.Equipment(this._weaponEquipment).RightWieldedItemIndex(-1).LeftWieldedItemIndex(this.ShieldSlotIndex)
				.Banner(this.Banner)
				.ClothColor1(this.Banner.GetPrimaryColor())
				.ClothColor2(this.Banner.GetFirstIconColor());
			this._agentVisuals[1].Refresh(false, copyAgentVisualsData2, true);
			this._agentVisuals[1].SetVisible(false);
			this._agentVisuals[1].GetEntity().CheckResources(true, true);
			this._checkWhetherAgentVisualIsReady = true;
			this._firstCharacterRender = true;
		}

		// Token: 0x06000264 RID: 612 RVA: 0x000118C0 File Offset: 0x0000FAC0
		public void OnTick(float dt)
		{
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

		// Token: 0x06000265 RID: 613 RVA: 0x000119D0 File Offset: 0x0000FBD0
		public void OnFinalize()
		{
			if (!this._isOpenedFromCharacterCreation)
			{
				this._spriteCategory.Unload();
			}
			BannerEditorVM dataSource = this.DataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._isFinalized = true;
		}

		// Token: 0x06000266 RID: 614 RVA: 0x000119FD File Offset: 0x0000FBFD
		public void Exit(bool isCancel)
		{
			MouseManager.ActivateMouseCursor(1);
			this._gauntletmovie = null;
			if (isCancel)
			{
				this._negativeAction.Invoke();
				return;
			}
			this.SetMapIconAsDirtyForAllPlayerClanParties();
			this._affirmativeAction.Invoke();
		}

		// Token: 0x06000267 RID: 615 RVA: 0x00011A2C File Offset: 0x0000FC2C
		private void SetMapIconAsDirtyForAllPlayerClanParties()
		{
			foreach (Hero hero in Clan.PlayerClan.Lords)
			{
				foreach (CaravanPartyComponent caravanPartyComponent in hero.OwnedCaravans)
				{
					PartyBase party = caravanPartyComponent.MobileParty.Party;
					if (party != null)
					{
						party.Visuals.SetMapIconAsDirty();
					}
				}
			}
			foreach (Hero hero2 in Clan.PlayerClan.Companions)
			{
				foreach (CaravanPartyComponent caravanPartyComponent2 in hero2.OwnedCaravans)
				{
					PartyBase party2 = caravanPartyComponent2.MobileParty.Party;
					if (party2 != null)
					{
						party2.Visuals.SetMapIconAsDirty();
					}
				}
			}
			foreach (WarPartyComponent warPartyComponent in Clan.PlayerClan.WarPartyComponents)
			{
				PartyBase party3 = warPartyComponent.MobileParty.Party;
				if (party3 != null)
				{
					party3.Visuals.SetMapIconAsDirty();
				}
			}
			foreach (Settlement settlement in Clan.PlayerClan.Settlements)
			{
				if (settlement.IsVillage && settlement.Village.VillagerPartyComponent != null)
				{
					PartyBase party4 = settlement.Village.VillagerPartyComponent.MobileParty.Party;
					if (party4 != null)
					{
						party4.Visuals.SetMapIconAsDirty();
					}
				}
				else if ((settlement.IsCastle || settlement.IsTown) && settlement.Town.GarrisonParty != null)
				{
					PartyBase party5 = settlement.Town.GarrisonParty.Party;
					if (party5 != null)
					{
						party5.Visuals.SetMapIconAsDirty();
					}
				}
			}
		}

		// Token: 0x06000268 RID: 616 RVA: 0x00011C80 File Offset: 0x0000FE80
		private void CreateScene()
		{
			this._scene = Scene.CreateNewScene(true, true, 2, "mono_renderscene");
			this._scene.SetName("MBBannerEditorScreen");
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
			this.AddCharacterEntity(this._idleAction);
		}

		// Token: 0x06000269 RID: 617 RVA: 0x00011E20 File Offset: 0x00010020
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
					goto IL_76;
				}
				ItemObject item2 = equipmentFromSlot.Item;
				if (((item2 != null) ? item2.PrimaryWeapon : null) != null && !equipmentFromSlot.Item.PrimaryWeapon.IsShield && !Extensions.HasAllFlags<ItemFlags>(equipmentFromSlot.Item.ItemFlags, 4096))
				{
					goto IL_76;
				}
				IL_83:
				i++;
				continue;
				IL_76:
				this._weaponEquipment.AddEquipmentToSlotWithoutAgent(i, equipmentFromSlot);
				goto IL_83;
			}
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(this._character.Race);
			this._agentVisuals[0] = AgentVisuals.Create(new AgentVisualsData().Equipment(this._weaponEquipment).BodyProperties(this._character.GetBodyProperties(this._weaponEquipment, -1)).Frame(this._characterFrame)
				.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, this._character.IsFemale, "_facegen"))
				.ActionCode(action)
				.Scene(this._scene)
				.Monster(baseMonsterFromRace)
				.SkeletonType(this._character.IsFemale ? 1 : 0)
				.Race(this._character.Race)
				.PrepareImmediately(true)
				.UseMorphAnims(true), "BannerEditorChar", false, false, true);
			this._agentVisuals[0].SetAgentLodZeroOrMaxExternal(true);
			this._agentVisuals[0].GetEntity().CheckResources(true, true);
			this._agentVisuals[1] = AgentVisuals.Create(new AgentVisualsData().Equipment(this._weaponEquipment).BodyProperties(this._character.GetBodyProperties(this._weaponEquipment, -1)).Frame(this._characterFrame)
				.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, this._character.IsFemale, "_facegen"))
				.ActionCode(action)
				.Scene(this._scene)
				.Race(this._character.Race)
				.Monster(baseMonsterFromRace)
				.SkeletonType(this._character.IsFemale ? 1 : 0)
				.PrepareImmediately(true)
				.UseMorphAnims(true), "BannerEditorChar", false, false, true);
			this._agentVisuals[1].SetAgentLodZeroOrMaxExternal(true);
			this._agentVisuals[1].GetEntity().CheckResources(true, true);
			this.UpdateBanners();
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0001206C File Offset: 0x0001026C
		private void UpdateBanners()
		{
			BannerCode currentBannerCode = BannerCode.CreateFrom(this.Banner);
			this.Banner.GetTableauTextureLarge(delegate(Texture resultTexture)
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
			this._previousBannerCode = BannerCode.CreateFrom(this.Banner);
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00012100 File Offset: 0x00010300
		private void OnNewBannerReadyForBanners(BannerCode bannerCodeOfTexture, Texture newTexture)
		{
			if (!this._isFinalized && this._scene != null && this._currentBannerCode == bannerCodeOfTexture)
			{
				GameEntity gameEntity = this._scene.FindEntityWithTag("banner");
				if (gameEntity != null)
				{
					Mesh firstMesh = gameEntity.GetFirstMesh();
					if (firstMesh != null && this.Banner != null)
					{
						firstMesh.GetMaterial().SetTexture(1, newTexture);
					}
				}
				else
				{
					gameEntity = this._scene.FindEntityWithTag("banner_2");
					Mesh firstMesh2 = gameEntity.GetFirstMesh();
					if (firstMesh2 != null && this.Banner != null)
					{
						firstMesh2.GetMaterial().SetTexture(1, newTexture);
					}
				}
				this._refreshCharacterAndShieldNextFrame = true;
			}
		}

		// Token: 0x0600026C RID: 620 RVA: 0x000121B5 File Offset: 0x000103B5
		private void RefreshShieldAndCharacter()
		{
			this._currentBannerCode = BannerCode.CreateFrom(this.Banner);
			this._refreshBannersNextFrame = true;
		}

		// Token: 0x0600026D RID: 621 RVA: 0x000121D0 File Offset: 0x000103D0
		private void RefreshShieldAndCharacterAux()
		{
			int agentVisualToShowIndex = this._agentVisualToShowIndex;
			this._agentVisualToShowIndex = (this._agentVisualToShowIndex + 1) % 2;
			AgentVisualsData copyAgentVisualsData = this._agentVisuals[this._agentVisualToShowIndex].GetCopyAgentVisualsData();
			copyAgentVisualsData.Equipment(this._weaponEquipment).RightWieldedItemIndex(-1).LeftWieldedItemIndex(this.ShieldSlotIndex)
				.Banner(this.Banner)
				.Frame(this._characterFrame)
				.BodyProperties(this._character.GetBodyProperties(this._weaponEquipment, -1))
				.ClothColor1(this.Banner.GetPrimaryColor())
				.ClothColor2(this.Banner.GetFirstIconColor());
			this._agentVisuals[this._agentVisualToShowIndex].Refresh(false, copyAgentVisualsData, true);
			this._agentVisuals[this._agentVisualToShowIndex].GetEntity().CheckResources(true, true);
			this._agentVisuals[this._agentVisualToShowIndex].GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(0.001f, this._characterFrame, true);
			this._agentVisuals[this._agentVisualToShowIndex].SetVisible(false);
			this._agentVisuals[this._agentVisualToShowIndex].SetVisible(true);
			this._checkWhetherAgentVisualIsReady = true;
		}

		// Token: 0x0600026E RID: 622 RVA: 0x000122F8 File Offset: 0x000104F8
		private void HandleUserInput()
		{
			if (this.GauntletLayer.Input.IsHotKeyReleased("Confirm"))
			{
				this.DataSource.ExecuteDone();
				return;
			}
			if (this.GauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				this.DataSource.ExecuteCancel();
				return;
			}
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
				this._cameraTargetElevationAdder = MBMath.ClampFloat(this._cameraTargetElevationAdder - vec.y * 0.002f, 0.5f, 1.9f * this._agentVisuals[0].GetScale());
				MBWindowManager.DontChangeCursorPos();
				this.GauntletLayer.InputRestrictions.SetMouseVisibility(false);
			}
			if (this.SceneLayer.Input.GetDeltaMouseScroll() != 0f)
			{
				this._cameraTargetDistanceAdder = MBMath.ClampFloat(this._cameraTargetDistanceAdder - this.SceneLayer.Input.GetDeltaMouseScroll() * 0.001f, 1.5f, 5f);
			}
		}

		// Token: 0x0600026F RID: 623 RVA: 0x00012518 File Offset: 0x00010718
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

		// Token: 0x06000270 RID: 624 RVA: 0x0001266F File Offset: 0x0001086F
		public void OnDeactivate()
		{
			this._agentVisuals[0].Reset();
			this._agentVisuals[1].Reset();
			MBAgentRendererSceneController.DestructAgentRendererSceneController(this._scene, this._agentRendererSceneController, false);
			this._agentRendererSceneController = null;
			this._scene = null;
		}

		// Token: 0x06000271 RID: 625 RVA: 0x000126AB File Offset: 0x000108AB
		public void GoToIndex(int index)
		{
			this._goToIndexAction.Invoke(index);
		}

		// Token: 0x04000165 RID: 357
		private IGauntletMovie _gauntletmovie;

		// Token: 0x04000166 RID: 358
		private readonly SpriteCategory _spriteCategory;

		// Token: 0x04000167 RID: 359
		private bool _isFinalized;

		// Token: 0x04000168 RID: 360
		private float _cameraCurrentRotation;

		// Token: 0x04000169 RID: 361
		private float _cameraTargetRotation;

		// Token: 0x0400016A RID: 362
		private float _cameraCurrentDistanceAdder;

		// Token: 0x0400016B RID: 363
		private float _cameraTargetDistanceAdder;

		// Token: 0x0400016C RID: 364
		private float _cameraCurrentElevationAdder;

		// Token: 0x0400016D RID: 365
		private float _cameraTargetElevationAdder;

		// Token: 0x0400016E RID: 366
		private readonly BasicCharacterObject _character;

		// Token: 0x0400016F RID: 367
		private readonly ActionIndexCache _idleAction = ActionIndexCache.Create("act_walk_idle_1h_with_shield_left_stance");

		// Token: 0x04000170 RID: 368
		private Scene _scene;

		// Token: 0x04000171 RID: 369
		private MBAgentRendererSceneController _agentRendererSceneController;

		// Token: 0x04000172 RID: 370
		private AgentVisuals[] _agentVisuals;

		// Token: 0x04000173 RID: 371
		private int _agentVisualToShowIndex;

		// Token: 0x04000174 RID: 372
		private bool _checkWhetherAgentVisualIsReady;

		// Token: 0x04000175 RID: 373
		private bool _firstCharacterRender = true;

		// Token: 0x04000176 RID: 374
		private bool _refreshBannersNextFrame;

		// Token: 0x04000177 RID: 375
		private bool _refreshCharacterAndShieldNextFrame;

		// Token: 0x04000178 RID: 376
		private BannerCode _previousBannerCode;

		// Token: 0x04000179 RID: 377
		private MatrixFrame _characterFrame;

		// Token: 0x0400017A RID: 378
		private Equipment _weaponEquipment;

		// Token: 0x0400017B RID: 379
		private BannerCode _currentBannerCode;

		// Token: 0x0400017C RID: 380
		private Camera _camera;

		// Token: 0x0400017E RID: 382
		private bool _isOpenedFromCharacterCreation;

		// Token: 0x0400017F RID: 383
		private ControlCharacterCreationStage _affirmativeAction;

		// Token: 0x04000180 RID: 384
		private ControlCharacterCreationStage _negativeAction;

		// Token: 0x04000181 RID: 385
		private ControlCharacterCreationStageWithInt _goToIndexAction;
	}
}
