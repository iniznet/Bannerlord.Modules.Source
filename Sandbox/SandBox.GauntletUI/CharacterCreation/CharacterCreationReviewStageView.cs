using System;
using System.Collections.Generic;
using System.Linq;
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
using TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.CharacterCreation
{
	// Token: 0x0200003D RID: 61
	[CharacterCreationStageView(typeof(CharacterCreationReviewStage))]
	public class CharacterCreationReviewStageView : CharacterCreationStageViewBase
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000247 RID: 583 RVA: 0x00010A78 File Offset: 0x0000EC78
		// (set) Token: 0x06000248 RID: 584 RVA: 0x00010A80 File Offset: 0x0000EC80
		public SceneLayer CharacterLayer { get; private set; }

		// Token: 0x06000249 RID: 585 RVA: 0x00010A8C File Offset: 0x0000EC8C
		public CharacterCreationReviewStageView(CharacterCreation characterCreation, ControlCharacterCreationStage affirmativeAction, TextObject affirmativeActionText, ControlCharacterCreationStage negativeAction, TextObject negativeActionText, ControlCharacterCreationStage onRefresh, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction, ControlCharacterCreationStageReturnInt getTotalStageCountAction, ControlCharacterCreationStageReturnInt getFurthestIndexAction, ControlCharacterCreationStageWithInt goToIndexAction)
			: base(affirmativeAction, negativeAction, onRefresh, getCurrentStageIndexAction, getTotalStageCountAction, getFurthestIndexAction, goToIndexAction)
		{
			this._characterCreation = characterCreation;
			this._affirmativeActionText = new TextObject("{=Rvr1bcu8}Next", null);
			this._negativeActionText = negativeActionText;
			this.GauntletLayer = new GauntletLayer(1, "GauntletLayer", false);
			this.GauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this.GauntletLayer.IsFocusLayer = true;
			this.GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			ScreenManager.TrySetFocus(this.GauntletLayer);
			CharacterCreationContentBase currentCharacterCreationContent = (GameStateManager.Current.ActiveState as CharacterCreationState).CurrentCharacterCreationContent;
			bool flag = currentCharacterCreationContent.CharacterCreationStages.Contains(typeof(CharacterCreationBannerEditorStage)) && currentCharacterCreationContent.CharacterCreationStages.Contains(typeof(CharacterCreationClanNamingStage));
			this._dataSource = new CharacterCreationReviewStageVM(this._characterCreation, new Action(this.NextStage), this._affirmativeActionText, new Action(this.PreviousStage), this._negativeActionText, getCurrentStageIndexAction.Invoke(), getTotalStageCountAction.Invoke(), getFurthestIndexAction.Invoke(), new Action<int>(this.GoToIndex), flag);
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._movie = this.GauntletLayer.LoadMovie("CharacterCreationReviewStage", this._dataSource);
		}

		// Token: 0x0600024A RID: 586 RVA: 0x00010C2A File Offset: 0x0000EE2A
		public override void SetGenericScene(Scene scene)
		{
			this.OpenScene(scene);
			this.AddCharacterEntity();
			this.RefreshMountEntity();
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00010C40 File Offset: 0x0000EE40
		private void OpenScene(Scene cachedScene)
		{
			this._characterScene = cachedScene;
			this._characterScene.SetShadow(true);
			this._characterScene.SetDynamicShadowmapCascadesRadiusMultiplier(0.1f);
			GameEntity gameEntity = this._characterScene.FindEntityWithName("cradle");
			if (gameEntity != null)
			{
				gameEntity.SetVisibilityExcludeParents(false);
			}
			this._characterScene.SetDoNotWaitForLoadingStatesToRender(true);
			this._characterScene.DisableStaticShadows(true);
			this._camera = Camera.CreateCamera();
			BodyGeneratorView.InitCamera(this._camera, this._cameraPosition);
			this.CharacterLayer = new SceneLayer("SceneLayer", false, true);
			this.CharacterLayer.SetScene(this._characterScene);
			this.CharacterLayer.SetCamera(this._camera);
			this.CharacterLayer.SetSceneUsesShadows(true);
			this.CharacterLayer.SetRenderWithPostfx(true);
			this.CharacterLayer.SetPostfxFromConfig();
			this.CharacterLayer.SceneView.SetResolutionScaling(true);
			int num = -1;
			num &= -5;
			this.CharacterLayer.SetPostfxConfigParams(num);
			this.CharacterLayer.SetPostfxFromConfig();
			if (!this.CharacterLayer.Input.IsCategoryRegistered(HotKeyManager.GetCategory("FaceGenHotkeyCategory")))
			{
				this.CharacterLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("FaceGenHotkeyCategory"));
			}
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00010D80 File Offset: 0x0000EF80
		private void AddCharacterEntity()
		{
			GameEntity gameEntity = this._characterScene.FindEntityWithTag("spawnpoint_player_1");
			this._initialCharacterFrame = gameEntity.GetFrame();
			this._initialCharacterFrame.origin.z = 0f;
			ActionIndexCache actionIndexCache = ActionIndexCache.Create("act_childhood_schooled");
			CharacterObject characterObject = Hero.MainHero.CharacterObject;
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(characterObject.Race);
			AgentVisualsData agentVisualsData = new AgentVisualsData().UseMorphAnims(true).Equipment(characterObject.Equipment).BodyProperties(characterObject.GetBodyProperties(characterObject.Equipment, -1))
				.SkeletonType(characterObject.IsFemale ? 1 : 0)
				.Frame(this._initialCharacterFrame)
				.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, characterObject.IsFemale, "_facegen"))
				.ActionCode(actionIndexCache)
				.Scene(this._characterScene)
				.Race(characterObject.Race)
				.Monster(baseMonsterFromRace)
				.UseTranslucency(true)
				.UseTesselation(true);
			CharacterCreationContentBase currentCharacterCreationContent = (GameStateManager.Current.ActiveState as CharacterCreationState).CurrentCharacterCreationContent;
			Banner currentPlayerBanner = currentCharacterCreationContent.GetCurrentPlayerBanner();
			CultureObject selectedCulture = currentCharacterCreationContent.GetSelectedCulture();
			if (currentPlayerBanner != null)
			{
				agentVisualsData.ClothColor1(currentPlayerBanner.GetPrimaryColor());
				agentVisualsData.ClothColor2(currentPlayerBanner.GetFirstIconColor());
			}
			else if (currentCharacterCreationContent.GetSelectedCulture() != null)
			{
				agentVisualsData.ClothColor1(selectedCulture.Color);
				agentVisualsData.ClothColor2(selectedCulture.Color2);
			}
			this._agentVisuals = AgentVisuals.Create(agentVisualsData, "facegenvisual", false, false, true);
			this.CharacterLayer.SetFocusedShadowmap(true, ref this._initialCharacterFrame.origin, 0.59999996f);
		}

		// Token: 0x0600024D RID: 589 RVA: 0x00010F10 File Offset: 0x0000F110
		private void RefreshCharacterEntityFrame()
		{
			if (this._agentVisuals != null)
			{
				MatrixFrame initialCharacterFrame = this._initialCharacterFrame;
				initialCharacterFrame.rotation.RotateAboutUp(this._charRotationAmount);
				initialCharacterFrame.rotation.ApplyScaleLocal(this._agentVisuals.GetScale());
				this._agentVisuals.GetEntity().SetFrame(ref initialCharacterFrame);
			}
		}

		// Token: 0x0600024E RID: 590 RVA: 0x00010F68 File Offset: 0x0000F168
		private void RefreshMountEntity()
		{
			this.RemoveMount();
			if (CharacterObject.PlayerCharacter.HasMount())
			{
				FaceGenMount faceGenMount = new FaceGenMount(MountCreationKey.GetRandomMountKey(CharacterObject.PlayerCharacter.Equipment[10].Item, CharacterObject.PlayerCharacter.GetMountKeySeed()), CharacterObject.PlayerCharacter.Equipment[10].Item, CharacterObject.PlayerCharacter.Equipment[11].Item, "act_inventory_idle_start");
				GameEntity gameEntity = this._characterScene.FindEntityWithTag("spawnpoint_mount_1");
				HorseComponent horseComponent = faceGenMount.HorseItem.HorseComponent;
				Monster monster = horseComponent.Monster;
				this._mountEntity = GameEntity.CreateEmpty(this._characterScene, true);
				AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(monster, MBGlobals.GetActionSet(horseComponent.Monster.ActionSetCode), 1f, false);
				GameEntityExtensions.CreateSkeletonWithActionSet(this._mountEntity, ref animationSystemData);
				MBSkeletonExtensions.SetAgentActionChannel(this._mountEntity.Skeleton, 0, this.act_inventory_idle_start, 0f, -0.2f, true);
				this._mountEntity.EntityFlags |= 256;
				MountVisualCreator.AddMountMeshToEntity(this._mountEntity, faceGenMount.HorseItem, faceGenMount.HarnessItem, faceGenMount.MountKey.ToString(), null);
				MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
				this._mountEntity.SetFrame(ref globalFrame);
				this._agentVisuals.GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(0.001f, this._initialCharacterFrame, true);
			}
		}

		// Token: 0x0600024F RID: 591 RVA: 0x000110DF File Offset: 0x0000F2DF
		private void RemoveMount()
		{
			if (this._mountEntity != null)
			{
				this._mountEntity.Remove(118);
			}
			this._mountEntity = null;
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00011104 File Offset: 0x0000F304
		public override void Tick(float dt)
		{
			base.Tick(dt);
			base.HandleEscapeMenu(this, this.CharacterLayer);
			Scene characterScene = this._characterScene;
			if (characterScene != null)
			{
				characterScene.Tick(dt);
			}
			AgentVisuals agentVisuals = this._agentVisuals;
			if (agentVisuals != null)
			{
				agentVisuals.TickVisuals();
			}
			Vec2 vec;
			vec..ctor(-this.CharacterLayer.Input.GetMouseMoveX(), -this.CharacterLayer.Input.GetMouseMoveY());
			if (this.CharacterLayer.Input.IsHotKeyReleased("Ascend") || this.CharacterLayer.Input.IsHotKeyReleased("Rotate") || this.CharacterLayer.Input.IsHotKeyReleased("Zoom"))
			{
				this.GauntletLayer.InputRestrictions.SetMouseVisibility(true);
			}
			if (this.CharacterLayer.Input.IsHotKeyDown("Rotate"))
			{
				this._charRotationAmount = (this._charRotationAmount - vec.x * 0.5f * dt) % 6.2831855f;
				this.RefreshCharacterEntityFrame();
				MBWindowManager.DontChangeCursorPos();
				this.GauntletLayer.InputRestrictions.SetMouseVisibility(false);
			}
			this.HandleLayerInput();
		}

		// Token: 0x06000251 RID: 593 RVA: 0x00011224 File Offset: 0x0000F424
		private void HandleLayerInput()
		{
			if (this.GauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				this._dataSource.OnPreviousStage();
				return;
			}
			if (this.GauntletLayer.Input.IsHotKeyReleased("Confirm") && this._dataSource.CanAdvance)
			{
				this._dataSource.OnNextStage();
			}
		}

		// Token: 0x06000252 RID: 594 RVA: 0x00011284 File Offset: 0x0000F484
		public override void NextStage()
		{
			TextObject textObject = GameTexts.FindText("str_generic_character_firstname", null);
			textObject.SetTextVariable("CHARACTER_FIRSTNAME", new TextObject(this._dataSource.Name, null));
			TextObject textObject2 = GameTexts.FindText("str_generic_character_name", null);
			textObject2.SetTextVariable("CHARACTER_NAME", new TextObject(this._dataSource.Name, null));
			textObject2.SetTextVariable("CHARACTER_GENDER", Hero.MainHero.IsFemale ? 1 : 0);
			textObject.SetTextVariable("CHARACTER_GENDER", Hero.MainHero.IsFemale ? 1 : 0);
			Hero.MainHero.SetName(textObject2, textObject);
			this.RemoveMount();
			this._affirmativeAction.Invoke();
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00011338 File Offset: 0x0000F538
		protected override void OnFinalize()
		{
			base.OnFinalize();
			this.CharacterLayer.SceneView.SetEnable(false);
			this.CharacterLayer.SceneView.ClearAll(false, false);
			this._agentVisuals.Reset();
			this._agentVisuals = null;
			this.GauntletLayer = null;
			CharacterCreationReviewStageVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._dataSource = null;
			this.CharacterLayer = null;
			this._characterScene = null;
		}

		// Token: 0x06000254 RID: 596 RVA: 0x000113AD File Offset: 0x0000F5AD
		public override int GetVirtualStageCount()
		{
			return 1;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x000113B0 File Offset: 0x0000F5B0
		public override void PreviousStage()
		{
			this.RemoveMount();
			this._negativeAction.Invoke();
		}

		// Token: 0x06000256 RID: 598 RVA: 0x000113C3 File Offset: 0x0000F5C3
		public override IEnumerable<ScreenLayer> GetLayers()
		{
			return new List<ScreenLayer> { this.CharacterLayer, this.GauntletLayer };
		}

		// Token: 0x06000257 RID: 599 RVA: 0x000113E2 File Offset: 0x0000F5E2
		public override void LoadEscapeMenuMovie()
		{
			this._escapeMenuDatasource = new EscapeMenuVM(base.GetEscapeMenuItems(this), null);
			this._escapeMenuMovie = this.GauntletLayer.LoadMovie("EscapeMenu", this._escapeMenuDatasource);
		}

		// Token: 0x06000258 RID: 600 RVA: 0x00011413 File Offset: 0x0000F613
		public override void ReleaseEscapeMenuMovie()
		{
			this.GauntletLayer.ReleaseMovie(this._escapeMenuMovie);
			this._escapeMenuDatasource = null;
			this._escapeMenuMovie = null;
		}

		// Token: 0x04000152 RID: 338
		protected readonly TextObject _affirmativeActionText;

		// Token: 0x04000153 RID: 339
		protected readonly TextObject _negativeActionText;

		// Token: 0x04000154 RID: 340
		private readonly IGauntletMovie _movie;

		// Token: 0x04000155 RID: 341
		private GauntletLayer GauntletLayer;

		// Token: 0x04000156 RID: 342
		private CharacterCreationReviewStageVM _dataSource;

		// Token: 0x04000157 RID: 343
		private readonly ActionIndexCache act_inventory_idle_start = ActionIndexCache.Create("act_inventory_idle_start");

		// Token: 0x04000158 RID: 344
		private readonly CharacterCreation _characterCreation;

		// Token: 0x04000159 RID: 345
		private Scene _characterScene;

		// Token: 0x0400015A RID: 346
		private Camera _camera;

		// Token: 0x0400015B RID: 347
		private MatrixFrame _initialCharacterFrame;

		// Token: 0x0400015C RID: 348
		private AgentVisuals _agentVisuals;

		// Token: 0x0400015D RID: 349
		private GameEntity _mountEntity;

		// Token: 0x0400015E RID: 350
		private float _charRotationAmount;

		// Token: 0x04000160 RID: 352
		private EscapeMenuVM _escapeMenuDatasource;

		// Token: 0x04000161 RID: 353
		private IGauntletMovie _escapeMenuMovie;
	}
}
