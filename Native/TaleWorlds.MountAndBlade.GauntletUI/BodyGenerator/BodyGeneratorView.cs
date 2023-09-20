﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.ViewModelCollection.FaceGenerator;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator
{
	// Token: 0x02000049 RID: 73
	public class BodyGeneratorView : IFaceGeneratorHandler
	{
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600035E RID: 862 RVA: 0x000131BA File Offset: 0x000113BA
		private IInputContext DebugInput
		{
			get
			{
				return Input.DebugInput;
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600035F RID: 863 RVA: 0x000131C1 File Offset: 0x000113C1
		// (set) Token: 0x06000360 RID: 864 RVA: 0x000131C9 File Offset: 0x000113C9
		public FaceGenVM DataSource { get; private set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000361 RID: 865 RVA: 0x000131D2 File Offset: 0x000113D2
		// (set) Token: 0x06000362 RID: 866 RVA: 0x000131DA File Offset: 0x000113DA
		public GauntletLayer GauntletLayer { get; private set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000363 RID: 867 RVA: 0x000131E3 File Offset: 0x000113E3
		// (set) Token: 0x06000364 RID: 868 RVA: 0x000131EB File Offset: 0x000113EB
		public SceneLayer SceneLayer { get; private set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000365 RID: 869 RVA: 0x000131F4 File Offset: 0x000113F4
		// (set) Token: 0x06000366 RID: 870 RVA: 0x000131FC File Offset: 0x000113FC
		public BodyGenerator BodyGen { get; private set; }

		// Token: 0x06000367 RID: 871 RVA: 0x00013208 File Offset: 0x00011408
		public BodyGeneratorView(ControlCharacterCreationStage affirmativeAction, TextObject affirmativeActionText, ControlCharacterCreationStage negativeAction, TextObject negativeActionText, BasicCharacterObject character, bool openedFromMultiplayer, IFaceGeneratorCustomFilter filter, Equipment dressedEquipment = null, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction = null, ControlCharacterCreationStageReturnInt getTotalStageCountAction = null, ControlCharacterCreationStageReturnInt getFurthestIndexAction = null, ControlCharacterCreationStageWithInt goToIndexAction = null)
		{
			this._affirmativeAction = affirmativeAction;
			this._negativeAction = negativeAction;
			this._getCurrentStageIndexAction = getCurrentStageIndexAction;
			this._getTotalStageCountAction = getTotalStageCountAction;
			this._getFurthestIndexAction = getFurthestIndexAction;
			this._goToIndexAction = goToIndexAction;
			this._openedFromMultiplayer = openedFromMultiplayer;
			this.BodyGen = new BodyGenerator(character);
			this._dressedEquipment = dressedEquipment ?? this.BodyGen.Character.Equipment.Clone(false);
			if (!this._dressedEquipment[4].IsEmpty && this._dressedEquipment[4].Item.IsBannerItem)
			{
				this._dressedEquipment[4] = EquipmentElement.Invalid;
			}
			FaceGenerationParams faceGenerationParams = this.BodyGen.InitBodyGenerator(false);
			faceGenerationParams._useCache = true;
			faceGenerationParams._useGpuMorph = true;
			this.SkeletonType = (this.BodyGen.IsFemale ? 1 : 0);
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._facegenCategory = spriteData.SpriteCategories["ui_facegen"];
			this._facegenCategory.Load(resourceContext, uiresourceDepot);
			this.OpenScene();
			this.AddCharacterEntity();
			bool openedFromMultiplayer2 = this._openedFromMultiplayer;
			if (this._getCurrentStageIndexAction == null || this._getTotalStageCountAction == null || this._getFurthestIndexAction == null)
			{
				this.DataSource = new FaceGenVM(this.BodyGen, this, new Action<float>(this.OnHeightChanged), new Action(this.OnAgeChanged), affirmativeActionText, negativeActionText, 0, 0, 0, new Action<int>(this.GoToIndex), openedFromMultiplayer2, openedFromMultiplayer, filter);
			}
			else
			{
				this.DataSource = new FaceGenVM(this.BodyGen, this, new Action<float>(this.OnHeightChanged), new Action(this.OnAgeChanged), affirmativeActionText, negativeActionText, this._getCurrentStageIndexAction.Invoke(), this._getTotalStageCountAction.Invoke(), this._getFurthestIndexAction.Invoke(), new Action<int>(this.GoToIndex), true, openedFromMultiplayer, filter);
			}
			this.DataSource.SetPreviousTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
			this.DataSource.SetNextTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
			this.DataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this.DataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this.DataSource.AddCameraControlInputKey(HotKeyManager.GetCategory("FaceGenHotkeyCategory").GetGameKey(55));
			this.DataSource.AddCameraControlInputKey(HotKeyManager.GetCategory("FaceGenHotkeyCategory").GetGameKey(56));
			this.DataSource.AddCameraControlInputKey(HotKeyManager.GetCategory("FaceGenHotkeyCategory").RegisteredGameAxisKeys.FirstOrDefault((GameAxisKey x) => x.Id == "CameraAxisX"));
			this.DataSource.AddCameraControlInputKey(HotKeyManager.GetCategory("FaceGenHotkeyCategory").RegisteredGameAxisKeys.FirstOrDefault((GameAxisKey x) => x.Id == "CameraAxisY"));
			this.DataSource.SetFaceGenerationParams(faceGenerationParams);
			this.DataSource.Refresh(true);
			this.GauntletLayer = new GauntletLayer(1, "GauntletLayer", false);
			this.GauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this.GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
			this.GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this.GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("FaceGenHotkeyCategory"));
			this.GauntletLayer.InputRestrictions.SetCanOverrideFocusOnHit(true);
			this.GauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this.GauntletLayer);
			this._viewMovie = this.GauntletLayer.LoadMovie("FaceGen", this.DataSource);
			if (!this._openedFromMultiplayer)
			{
				this._templateBodyProperties = new List<BodyProperties>();
				this._templateBodyProperties.Add(MBObjectManager.Instance.GetObject<BasicCharacterObject>("facgen_template_test_char_0").GetBodyProperties(null, -1));
				this._templateBodyProperties.Add(MBObjectManager.Instance.GetObject<BasicCharacterObject>("facgen_template_test_char_1").GetBodyProperties(null, -1));
				this._templateBodyProperties.Add(MBObjectManager.Instance.GetObject<BasicCharacterObject>("facgen_template_test_char_2").GetBodyProperties(null, -1));
				this._templateBodyProperties.Add(MBObjectManager.Instance.GetObject<BasicCharacterObject>("facgen_template_test_char_3").GetBodyProperties(null, -1));
				this._templateBodyProperties.Add(MBObjectManager.Instance.GetObject<BasicCharacterObject>("facgen_template_test_char_4").GetBodyProperties(null, -1));
				this._templateBodyProperties.Add(MBObjectManager.Instance.GetObject<BasicCharacterObject>("facgen_template_test_char_5").GetBodyProperties(null, -1));
				this._templateBodyProperties.Add(MBObjectManager.Instance.GetObject<BasicCharacterObject>("facgen_template_test_char_6").GetBodyProperties(null, -1));
				this._templateBodyProperties.Add(MBObjectManager.Instance.GetObject<BasicCharacterObject>("facgen_template_test_char_7").GetBodyProperties(null, -1));
				this._templateBodyProperties.Add(MBObjectManager.Instance.GetObject<BasicCharacterObject>("facgen_template_test_char_8").GetBodyProperties(null, -1));
				this._templateBodyProperties.Add(MBObjectManager.Instance.GetObject<BasicCharacterObject>("facgen_template_test_char_9").GetBodyProperties(null, -1));
			}
			this.RefreshCharacterEntity();
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("FaceGenHotkeyCategory"));
			this.DataSource.SelectedGender = (this.BodyGen.IsFemale ? 1 : 0);
		}

		// Token: 0x06000368 RID: 872 RVA: 0x0001380C File Offset: 0x00011A0C
		private void OpenScene()
		{
			this._facegenScene = Scene.CreateNewScene(true, false, 0, "mono_renderscene");
			this._facegenScene.DisableStaticShadows(true);
			SceneInitializationData sceneInitializationData = default(SceneInitializationData);
			sceneInitializationData.InitPhysicsWorld = false;
			this._facegenScene.Read("character_menu_new", ref sceneInitializationData, "");
			this._facegenScene.SetShadow(true);
			this._facegenScene.SetDynamicShadowmapCascadesRadiusMultiplier(0.1f);
			GameEntity gameEntity = this._facegenScene.FindEntityWithName("cradle");
			if (gameEntity != null)
			{
				gameEntity.SetVisibilityExcludeParents(false);
			}
			this._facegenScene.DisableStaticShadows(true);
			this._agentRendererSceneController = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(this._facegenScene, 32);
			this._camera = Camera.CreateCamera();
			this._defaultCameraGlobalFrame = BodyGeneratorView.InitCamera(this._camera, new Vec3(6.45f, 5.15f, 1.75f, -1f));
			this._targetCameraGlobalFrame = this._defaultCameraGlobalFrame;
			this.SceneLayer = new SceneLayer("SceneLayer", true, true);
			this.SceneLayer.IsFocusLayer = true;
			this.SceneLayer.SetScene(this._facegenScene);
			this.SceneLayer.SetCamera(this._camera);
			this.SceneLayer.SetSceneUsesShadows(true);
			this.SceneLayer.SetRenderWithPostfx(true);
			this.SceneLayer.SetPostfxFromConfig();
			this.SceneLayer.SceneView.SetResolutionScaling(true);
			this.SceneLayer.InputRestrictions.SetCanOverrideFocusOnHit(true);
			int num = -1;
			num &= -5;
			this.SceneLayer.SetPostfxConfigParams(num);
			this.SceneLayer.SetPostfxFromConfig();
			this.SceneLayer.SceneView.SetAcceptGlobalDebugRenderObjects(true);
		}

		// Token: 0x06000369 RID: 873 RVA: 0x000139AC File Offset: 0x00011BAC
		private void AddCharacterEntity()
		{
			GameEntity gameEntity = this._facegenScene.FindEntityWithTag("spawnpoint_player_1");
			this._initialCharacterFrame = gameEntity.GetFrame();
			this._initialCharacterFrame.origin.z = 0f;
			this._visualToShow = null;
			this._visualsBeingPrepared = new List<KeyValuePair<AgentVisuals, int>>();
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(this.BodyGen.Race);
			AgentVisualsData agentVisualsData = new AgentVisualsData().UseMorphAnims(true).Equipment(this.BodyGen.Character.Equipment).BodyProperties(this.BodyGen.Character.GetBodyProperties(this.BodyGen.Character.Equipment, -1))
				.Race(this.BodyGen.Race)
				.Frame(this._initialCharacterFrame)
				.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, this.BodyGen.IsFemale, "_facegen"))
				.Scene(this._facegenScene)
				.Monster(baseMonsterFromRace)
				.UseTranslucency(true)
				.UseTesselation(false)
				.PrepareImmediately(true);
			this._nextVisualToShow = AgentVisuals.Create(agentVisualsData, "facegenvisual", false, false, false);
			MBSkeletonExtensions.SetAgentActionChannel(this._nextVisualToShow.GetEntity().Skeleton, 1, this.act_inventory_idle_start_cached, 0f, -0.2f, true);
			this._nextVisualToShow.GetEntity();
			this._nextVisualToShow.SetAgentLodZeroOrMaxExternal(true);
			this._nextVisualToShow.GetEntity().CheckResources(true, true);
			this._nextVisualToShow.SetVisible(false);
			this._visualsBeingPrepared.Add(new KeyValuePair<AgentVisuals, int>(this._nextVisualToShow, 1));
			this.SceneLayer.SetFocusedShadowmap(true, ref this._initialCharacterFrame.origin, 0.59999996f);
		}

		// Token: 0x0600036A RID: 874 RVA: 0x00013B53 File Offset: 0x00011D53
		private void SetNewBodyPropertiesAndBodyGen(BodyProperties bodyProperties)
		{
			this.BodyGen.CurrentBodyProperties = bodyProperties;
			this.RefreshCharacterEntity();
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00013B68 File Offset: 0x00011D68
		public void ResetFaceToDefault()
		{
			MBBodyProperties.ProduceNumericKeyWithDefaultValues(ref this.BodyGen.CurrentBodyProperties, this.BodyGen.Character.Equipment.EarsAreHidden, this.BodyGen.Character.Equipment.MouthIsHidden, this.BodyGen.Race, this.BodyGen.IsFemale ? 1 : 0, (int)this.BodyGen.Character.Age);
			this.RefreshCharacterEntity();
		}

		// Token: 0x0600036C RID: 876 RVA: 0x00013BE2 File Offset: 0x00011DE2
		private void OnHeightChanged(float sliderValue)
		{
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00013BE4 File Offset: 0x00011DE4
		private void OnAgeChanged()
		{
		}

		// Token: 0x0600036E RID: 878 RVA: 0x00013BE6 File Offset: 0x00011DE6
		[CommandLineFunctionality.CommandLineArgumentFunction("show_debug", "facegen")]
		public static string FaceGenShowDebug(List<string> strings)
		{
			FaceGen.ShowDebugValues = !FaceGen.ShowDebugValues;
			return "FaceGen: Show Debug Values are " + (FaceGen.ShowDebugValues ? "enabled" : "disabled");
		}

		// Token: 0x0600036F RID: 879 RVA: 0x00013C12 File Offset: 0x00011E12
		[CommandLineFunctionality.CommandLineArgumentFunction("toggle_update_deform_keys", "facegen")]
		public static string FaceGenUpdateDeformKeys(List<string> strings)
		{
			FaceGen.UpdateDeformKeys = !FaceGen.UpdateDeformKeys;
			return "FaceGen: update deform keys is now " + (FaceGen.UpdateDeformKeys ? "enabled" : "disabled");
		}

		// Token: 0x06000370 RID: 880 RVA: 0x00013C3E File Offset: 0x00011E3E
		public bool ReadyToRender()
		{
			return this.SceneLayer != null && this.SceneLayer.SceneView != null && this.SceneLayer.SceneView.ReadyToRender();
		}

		// Token: 0x06000371 RID: 881 RVA: 0x00013C70 File Offset: 0x00011E70
		public void OnTick(float dt)
		{
			this.DataSource.CharacterGamepadControlsEnabled = Input.IsGamepadActive && this.SceneLayer.IsHitThisFrame;
			this.TickUserInputs(dt);
			if (this.SceneLayer != null && this.SceneLayer.ReadyToRender())
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}
			if (this._refreshCharacterEntityNextFrame)
			{
				this.RefreshCharacterEntityAux();
				this._refreshCharacterEntityNextFrame = false;
			}
			if (this._visualToShow != null)
			{
				Skeleton skeleton = this._visualToShow.GetVisuals().GetSkeleton();
				bool flag = skeleton.GetAnimationParameterAtChannel(1) > 0.6f;
				if (MBSkeletonExtensions.GetActionAtChannel(skeleton, 1) == this.act_command_leftstance_cached && flag)
				{
					MBSkeletonExtensions.SetAgentActionChannel(this._visualToShow.GetEntity().Skeleton, 1, this.act_inventory_idle_cached, 0f, -0.2f, true);
				}
			}
			if (!this._openedFromMultiplayer)
			{
				if (this.DebugInput.IsHotKeyReleased("MbFaceGeneratorScreenHotkeySetFaceKeyMin"))
				{
					this.BodyGen.BodyPropertiesMin = this.BodyGen.CurrentBodyProperties;
				}
				else if (this.DebugInput.IsHotKeyReleased("MbFaceGeneratorScreenHotkeySetFaceKeyMax"))
				{
					this.BodyGen.BodyPropertiesMax = this.BodyGen.CurrentBodyProperties;
				}
				else if (this.DebugInput.IsHotKeyPressed("Reset"))
				{
					string text = "";
					string text2 = "";
					string text3 = "";
					this.BodyGen.CurrentBodyProperties = MBBodyProperties.GetRandomBodyProperties(this.BodyGen.Race, this.BodyGen.IsFemale, this.BodyGen.BodyPropertiesMin, this.BodyGen.BodyPropertiesMax, 0, MBRandom.RandomInt(), text, text2, text3);
					this.SetNewBodyPropertiesAndBodyGen(this.BodyGen.CurrentBodyProperties);
					this.DataSource.SetBodyProperties(this.BodyGen.CurrentBodyProperties, false, 0, -1, false);
					this.DataSource.UpdateFacegen();
				}
			}
			if (this.DebugInput.IsHotKeyReleased("MbFaceGeneratorScreenHotkeySetCurFaceKeyToMin"))
			{
				this.BodyGen.CurrentBodyProperties = this.BodyGen.BodyPropertiesMin;
				this.SetNewBodyPropertiesAndBodyGen(this.BodyGen.BodyPropertiesMin);
				this.DataSource.SetBodyProperties(this.BodyGen.CurrentBodyProperties, false, 0, -1, false);
				this.DataSource.UpdateFacegen();
			}
			else if (this.DebugInput.IsHotKeyReleased("MbFaceGeneratorScreenHotkeySetCurFaceKeyToMax"))
			{
				this.BodyGen.CurrentBodyProperties = this.BodyGen.BodyPropertiesMax;
				this.SetNewBodyPropertiesAndBodyGen(this.BodyGen.BodyPropertiesMax);
				this.DataSource.SetBodyProperties(this.BodyGen.CurrentBodyProperties, false, 0, -1, false);
				this.DataSource.UpdateFacegen();
			}
			if (this.DebugInput.IsHotKeyDown("FaceGeneratorExtendedDebugKey") && this.DebugInput.IsHotKeyDown("MbFaceGeneratorScreenHotkeyResetFaceToDefault"))
			{
				this.ResetFaceToDefault();
				this.DataSource.SetBodyProperties(this.BodyGen.CurrentBodyProperties, false, 0, -1, false);
				this.DataSource.UpdateFacegen();
			}
			Utilities.CheckResourceModifications();
			if (this.DebugInput.IsHotKeyReleased("Refresh"))
			{
				this.RefreshCharacterEntity();
			}
			Scene facegenScene = this._facegenScene;
			if (facegenScene != null)
			{
				facegenScene.Tick(dt);
			}
			if (this._visualToShow != null)
			{
				this._visualToShow.TickVisuals();
			}
			foreach (KeyValuePair<AgentVisuals, int> keyValuePair in this._visualsBeingPrepared)
			{
				keyValuePair.Key.TickVisuals();
			}
			for (int i = 0; i < this._visualsBeingPrepared.Count; i++)
			{
				AgentVisuals key = this._visualsBeingPrepared[i].Key;
				int value = this._visualsBeingPrepared[i].Value;
				key.SetVisible(false);
				if (key.GetEntity().CheckResources(false, true))
				{
					if (value > 0)
					{
						this._visualsBeingPrepared[i] = new KeyValuePair<AgentVisuals, int>(key, value - 1);
					}
					else
					{
						if (key == this._nextVisualToShow)
						{
							if (this._visualToShow != null)
							{
								this._visualToShow.Reset();
							}
							this._visualToShow = key;
							this._visualToShow.SetVisible(true);
							this._nextVisualToShow = null;
							if (this._setMorphAnimNextFrame)
							{
								MBSkeletonExtensions.SetFacialAnimation(this._visualToShow.GetEntity().Skeleton, 0, this._nextMorphAnimToSet, true, this._nextMorphAnimLoopValue);
								this._setMorphAnimNextFrame = false;
							}
						}
						else
						{
							this._visualsBeingPrepared[i].Key.Reset();
						}
						this._visualsBeingPrepared[i] = this._visualsBeingPrepared[this._visualsBeingPrepared.Count - 1];
						this._visualsBeingPrepared.RemoveAt(this._visualsBeingPrepared.Count - 1);
						i--;
					}
				}
			}
			SoundManager.SetListenerFrame(this._camera.Frame);
			this.UpdateCamera(dt);
			this.TickLayerInputs();
		}

		// Token: 0x06000372 RID: 882 RVA: 0x00014150 File Offset: 0x00012350
		public void OnFinalize()
		{
			this._facegenCategory.Unload();
			this.ClearAgentVisuals();
			MBAgentRendererSceneController.DestructAgentRendererSceneController(this._facegenScene, this._agentRendererSceneController, false);
			this._agentRendererSceneController = null;
			this._facegenScene.ClearAll();
			this._facegenScene = null;
			this.SceneLayer.SceneView.SetEnable(false);
			this.SceneLayer.SceneView.ClearAll(true, true);
			FaceGenVM dataSource = this.DataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this.DataSource = null;
		}

		// Token: 0x06000373 RID: 883 RVA: 0x000141D4 File Offset: 0x000123D4
		private void TickLayerInputs()
		{
			if (this.IsHotKeyReleasedOnAnyLayer("Exit"))
			{
				this.Cancel();
				return;
			}
			if (this.IsHotKeyReleasedOnAnyLayer("Confirm"))
			{
				this.Done();
			}
		}

		// Token: 0x06000374 RID: 884 RVA: 0x00014200 File Offset: 0x00012400
		private void TickUserInputs(float dt)
		{
			if (this.SceneLayer.Input.IsHotKeyReleased("Ascend") || this.SceneLayer.Input.IsHotKeyReleased("Rotate") || this.SceneLayer.Input.IsHotKeyReleased("Zoom"))
			{
				this.GauntletLayer.InputRestrictions.SetMouseVisibility(true);
			}
			Vec2 vec;
			vec..ctor(-this.SceneLayer.Input.GetMouseMoveX(), -this.SceneLayer.Input.GetMouseMoveY());
			bool flag = this.SceneLayer.Input.IsHotKeyDown("Zoom");
			float gameKeyState = this.SceneLayer.Input.GetGameKeyState(55);
			float num = this.SceneLayer.Input.GetGameKeyState(56) - gameKeyState;
			float num2 = (flag ? (vec.y * 0.002f) : ((num != 0f) ? (num * 0.02f) : (this.SceneLayer.Input.GetDeltaMouseScroll() * -0.001f)));
			float length = (this._targetCameraGlobalFrame.origin.AsVec2 - this._initialCharacterFrame.origin.AsVec2).Length;
			this._cameraCurrentDistanceAdder = MBMath.ClampFloat(this._cameraCurrentDistanceAdder + num2, 0.3f - length, 3f - length);
			if (flag)
			{
				MBWindowManager.DontChangeCursorPos();
				this.GauntletLayer.InputRestrictions.SetMouseVisibility(false);
			}
			float num3 = this.SceneLayer.Input.GetGameKeyAxis("CameraAxisX");
			if (MathF.Abs(num3) < 0.1f)
			{
				num3 = 0f;
			}
			else
			{
				num3 = (num3 - (float)MathF.Sign(num3) * 0.1f) / 0.9f;
			}
			bool flag2 = this.SceneLayer.Input.IsHotKeyDown("Rotate");
			float num4 = (flag2 ? (vec.x * -0.004f) : (num3 * -0.02f));
			this._characterTargetRotation = MBMath.WrapAngle(this._characterTargetRotation + num4);
			if (flag2)
			{
				MBWindowManager.DontChangeCursorPos();
				this.GauntletLayer.InputRestrictions.SetMouseVisibility(false);
			}
			if (this.SceneLayer.Input.IsHotKeyDown("Ascend"))
			{
				float num5 = ((this._visualToShow != null) ? this._visualToShow.GetScale() : 1f);
				float num6 = this._cameraCurrentElevationAdder - vec.y * 0.002f;
				float num7 = 0.15f - this._targetCameraGlobalFrame.origin.z;
				float num8 = 1.9f * num5 - this._targetCameraGlobalFrame.origin.z;
				this._cameraCurrentElevationAdder = MBMath.ClampFloat(num6, num7, num8);
				MBWindowManager.DontChangeCursorPos();
				this.GauntletLayer.InputRestrictions.SetMouseVisibility(false);
			}
			else if (Input.IsGamepadActive)
			{
				float num9 = -this.SceneLayer.Input.GetGameKeyAxis("CameraAxisY");
				if (MathF.Abs(num9) > 0.1f)
				{
					num9 = (num9 - (float)MathF.Sign(num9) * 0.1f) / 0.9f;
					float num10 = ((this._visualToShow != null) ? this._visualToShow.GetScale() : 1f);
					float num11 = this._cameraCurrentElevationAdder - num9 * 0.01f;
					float num12 = 0.15f - this._targetCameraGlobalFrame.origin.z;
					float num13 = 1.9f * num10 - this._targetCameraGlobalFrame.origin.z;
					this._cameraCurrentElevationAdder = MBMath.ClampFloat(num11, num12, num13);
				}
			}
			if (this.IsHotKeyPressedOnAnyLayer("SwitchToPreviousTab"))
			{
				this.DataSource.SelectPreviousTab();
			}
			else if (this.IsHotKeyPressedOnAnyLayer("SwitchToNextTab"))
			{
				this.DataSource.SelectNextTab();
			}
			if (this.SceneLayer.Input.IsControlDown() || this.GauntletLayer.Input.IsControlDown())
			{
				if (this.IsHotKeyPressedOnAnyLayer("Copy"))
				{
					Input.SetClipboardText(this.BodyGen.CurrentBodyProperties.ToString());
					return;
				}
				if (this.IsHotKeyPressedOnAnyLayer("Paste"))
				{
					BodyProperties bodyProperties;
					if (BodyProperties.FromString(Input.GetClipboardText(), ref bodyProperties))
					{
						this.DataSource.SetBodyProperties(bodyProperties, !FaceGen.ShowDebugValues, 0, -1, true);
						return;
					}
					InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_error", null).ToString(), GameTexts.FindText("str_facegen_error_on_paste", null).ToString(), false, true, "", GameTexts.FindText("str_ok", null).ToString(), null, null, "", 0f, null, null, null), false, false);
				}
			}
		}

		// Token: 0x06000375 RID: 885 RVA: 0x0001467F File Offset: 0x0001287F
		private bool IsHotKeyReleasedOnAnyLayer(string hotkeyName)
		{
			return this.GauntletLayer.Input.IsHotKeyReleased(hotkeyName) || this.SceneLayer.Input.IsHotKeyReleased(hotkeyName);
		}

		// Token: 0x06000376 RID: 886 RVA: 0x000146A7 File Offset: 0x000128A7
		private bool IsHotKeyPressedOnAnyLayer(string hotkeyName)
		{
			return this.GauntletLayer.Input.IsHotKeyPressed(hotkeyName) || this.SceneLayer.Input.IsHotKeyPressed(hotkeyName);
		}

		// Token: 0x06000377 RID: 887 RVA: 0x000146D0 File Offset: 0x000128D0
		private void RefreshCharacterEntityAux()
		{
			SkeletonType skeletonType = this.SkeletonType;
			if (skeletonType < 2)
			{
				skeletonType = (this.BodyGen.IsFemale ? 1 : 0);
			}
			this._currentAgentVisualIndex = (this._currentAgentVisualIndex + 1) % 2;
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(this.BodyGen.Race);
			AgentVisualsData agentVisualsData = new AgentVisualsData().UseMorphAnims(true).Scene(this._facegenScene).Monster(baseMonsterFromRace)
				.UseTranslucency(true)
				.UseTesselation(false)
				.SkeletonType(skeletonType)
				.Equipment(this.IsDressed ? this._dressedEquipment : null)
				.BodyProperties(this.BodyGen.CurrentBodyProperties)
				.Race(this.BodyGen.Race)
				.PrepareImmediately(true);
			AgentVisuals agentVisuals = this._visualToShow ?? this._nextVisualToShow;
			ActionIndexCache actionAtChannel = MBSkeletonExtensions.GetActionAtChannel(agentVisuals.GetEntity().Skeleton, 1);
			float animationParameterAtChannel = agentVisuals.GetVisuals().GetSkeleton().GetAnimationParameterAtChannel(1);
			this._nextVisualToShow = AgentVisuals.Create(agentVisualsData, "facegenvisual", false, false, false);
			this._nextVisualToShow.SetAgentLodZeroOrMax(true);
			MBSkeletonExtensions.SetAgentActionChannel(this._nextVisualToShow.GetEntity().Skeleton, 1, actionAtChannel, animationParameterAtChannel, -0.2f, true);
			this._nextVisualToShow.GetEntity().SetEnforcedMaximumLodLevel(0);
			this._nextVisualToShow.GetEntity().CheckResources(true, true);
			this._nextVisualToShow.SetVisible(false);
			MatrixFrame initialCharacterFrame = this._initialCharacterFrame;
			initialCharacterFrame.rotation.RotateAboutUp(this._characterCurrentRotation);
			initialCharacterFrame.rotation.ApplyScaleLocal(this._nextVisualToShow.GetScale());
			this._nextVisualToShow.GetEntity().SetFrame(ref initialCharacterFrame);
			this._nextVisualToShow.GetVisuals().GetSkeleton().SetAnimationParameterAtChannel(1, animationParameterAtChannel);
			this._nextVisualToShow.GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(0.001f, initialCharacterFrame, true);
			this._nextVisualToShow.SetVisible(false);
			this._visualsBeingPrepared.Add(new KeyValuePair<AgentVisuals, int>(this._nextVisualToShow, 1));
		}

		// Token: 0x06000378 RID: 888 RVA: 0x000148C4 File Offset: 0x00012AC4
		void IFaceGeneratorHandler.MakeVoice(int voiceIndex, float pitch)
		{
			if (this._makeSound)
			{
				AgentVisuals visualToShow = this._visualToShow;
				if (visualToShow == null)
				{
					return;
				}
				visualToShow.MakeRandomVoiceForFacegen();
			}
		}

		// Token: 0x06000379 RID: 889 RVA: 0x000148DE File Offset: 0x00012ADE
		void IFaceGeneratorHandler.RefreshCharacterEntity()
		{
			this._refreshCharacterEntityNextFrame = true;
		}

		// Token: 0x0600037A RID: 890 RVA: 0x000148E7 File Offset: 0x00012AE7
		void IFaceGeneratorHandler.SetFacialAnimation(string faceAnimation, bool loop)
		{
			this._setMorphAnimNextFrame = true;
			this._nextMorphAnimToSet = faceAnimation;
			this._nextMorphAnimLoopValue = loop;
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00014900 File Offset: 0x00012B00
		private void ClearAgentVisuals()
		{
			if (this._visualToShow != null)
			{
				this._visualToShow.Reset();
				this._visualToShow = null;
			}
			foreach (KeyValuePair<AgentVisuals, int> keyValuePair in this._visualsBeingPrepared)
			{
				keyValuePair.Key.Reset();
			}
			this._visualsBeingPrepared.Clear();
		}

		// Token: 0x0600037C RID: 892 RVA: 0x00014980 File Offset: 0x00012B80
		void IFaceGeneratorHandler.Done()
		{
			this.BodyGen.SaveCurrentCharacter();
			this.ClearAgentVisuals();
			if (Mission.Current != null)
			{
				Mission.Current.MainAgent.UpdateBodyProperties(this.BodyGen.CurrentBodyProperties);
				Mission.Current.MainAgent.EquipItemsFromSpawnEquipment(false);
			}
			this._affirmativeAction.Invoke();
		}

		// Token: 0x0600037D RID: 893 RVA: 0x000149DA File Offset: 0x00012BDA
		void IFaceGeneratorHandler.Cancel()
		{
			this._negativeAction.Invoke();
			this.ClearAgentVisuals();
		}

		// Token: 0x0600037E RID: 894 RVA: 0x000149ED File Offset: 0x00012BED
		void IFaceGeneratorHandler.ChangeToFaceCamera()
		{
			this._cameraLookMode = 1;
			this._cameraCurrentElevationAdder = 0f;
			this._cameraCurrentDistanceAdder = 0f;
		}

		// Token: 0x0600037F RID: 895 RVA: 0x00014A0C File Offset: 0x00012C0C
		void IFaceGeneratorHandler.ChangeToEyeCamera()
		{
			this._cameraLookMode = 2;
			this._cameraCurrentElevationAdder = 0f;
			this._cameraCurrentDistanceAdder = 0f;
		}

		// Token: 0x06000380 RID: 896 RVA: 0x00014A2B File Offset: 0x00012C2B
		void IFaceGeneratorHandler.ChangeToNoseCamera()
		{
			this._cameraLookMode = 3;
			this._cameraCurrentElevationAdder = 0f;
			this._cameraCurrentDistanceAdder = 0f;
		}

		// Token: 0x06000381 RID: 897 RVA: 0x00014A4A File Offset: 0x00012C4A
		void IFaceGeneratorHandler.ChangeToMouthCamera()
		{
			this._cameraLookMode = 4;
			this._cameraCurrentElevationAdder = 0f;
			this._cameraCurrentDistanceAdder = 0f;
		}

		// Token: 0x06000382 RID: 898 RVA: 0x00014A69 File Offset: 0x00012C69
		void IFaceGeneratorHandler.ChangeToBodyCamera()
		{
			this._cameraLookMode = 0;
			this._cameraCurrentElevationAdder = 0f;
			this._cameraCurrentDistanceAdder = 0f;
		}

		// Token: 0x06000383 RID: 899 RVA: 0x00014A88 File Offset: 0x00012C88
		void IFaceGeneratorHandler.ChangeToHairCamera()
		{
			this._cameraLookMode = 1;
			this._cameraCurrentElevationAdder = 0f;
			this._cameraCurrentDistanceAdder = 0f;
		}

		// Token: 0x06000384 RID: 900 RVA: 0x00014AA7 File Offset: 0x00012CA7
		void IFaceGeneratorHandler.UndressCharacterEntity()
		{
			this.IsDressed = false;
			this.RefreshCharacterEntity();
		}

		// Token: 0x06000385 RID: 901 RVA: 0x00014AB6 File Offset: 0x00012CB6
		void IFaceGeneratorHandler.DressCharacterEntity()
		{
			this.IsDressed = true;
			this.RefreshCharacterEntity();
		}

		// Token: 0x06000386 RID: 902 RVA: 0x00014AC8 File Offset: 0x00012CC8
		void IFaceGeneratorHandler.DefaultFace()
		{
			FaceGenerationParams faceGenerationParams = this.BodyGen.InitBodyGenerator(false);
			faceGenerationParams._useCache = true;
			faceGenerationParams._useGpuMorph = true;
			MBBodyProperties.TransformFaceKeysToDefaultFace(ref faceGenerationParams);
			this.DataSource.SetFaceGenerationParams(faceGenerationParams);
			this.DataSource.Refresh(true);
		}

		// Token: 0x06000387 RID: 903 RVA: 0x00014B11 File Offset: 0x00012D11
		private void GoToIndex(int index)
		{
			this.BodyGen.SaveCurrentCharacter();
			this.ClearAgentVisuals();
			this._goToIndexAction.Invoke(index);
		}

		// Token: 0x06000388 RID: 904 RVA: 0x00014B30 File Offset: 0x00012D30
		public static MatrixFrame InitCamera(Camera camera, Vec3 cameraPosition)
		{
			camera.SetFovVertical(0.7853982f, Screen.AspectRatio, 0.02f, 200f);
			MatrixFrame matrixFrame = Camera.ConstructCameraFromPositionElevationBearing(cameraPosition, -0.195f, 163.17f);
			camera.Frame = matrixFrame;
			return matrixFrame;
		}

		// Token: 0x06000389 RID: 905 RVA: 0x00014B70 File Offset: 0x00012D70
		private void UpdateCamera(float dt)
		{
			this._characterCurrentRotation += MBMath.WrapAngle(this._characterTargetRotation - this._characterCurrentRotation) * MathF.Min(1f, 20f * dt);
			this._targetCameraGlobalFrame.origin = this._defaultCameraGlobalFrame.origin;
			if (this._visualToShow != null)
			{
				MatrixFrame initialCharacterFrame = this._initialCharacterFrame;
				initialCharacterFrame.rotation.RotateAboutUp(this._characterCurrentRotation);
				initialCharacterFrame.rotation.ApplyScaleLocal(this._visualToShow.GetScale());
				this._visualToShow.GetEntity().SetFrame(ref initialCharacterFrame);
				float z = this._visualToShow.GetGlobalStableEyePoint(true).z;
				float z2 = this._visualToShow.GetGlobalStableNeckPoint(true).z;
				float scale = this._visualToShow.GetScale();
				switch (this._cameraLookMode)
				{
				case 1:
				{
					Vec2 vec;
					vec..ctor(6.45f, 6.75f);
					vec += (vec - this._initialCharacterFrame.origin.AsVec2) * (scale - 1f);
					this._targetCameraGlobalFrame.origin = new Vec3(vec, z + (z - z2) * 0.75f, -1f);
					break;
				}
				case 2:
				{
					Vec2 vec;
					vec..ctor(6.45f, 7f);
					vec += (vec - this._initialCharacterFrame.origin.AsVec2) * (scale - 1f);
					this._targetCameraGlobalFrame.origin = new Vec3(vec, z + (z - z2) * 0.5f, -1f);
					break;
				}
				case 3:
				{
					Vec2 vec;
					vec..ctor(6.45f, 7f);
					vec += (vec - this._initialCharacterFrame.origin.AsVec2) * (scale - 1f);
					this._targetCameraGlobalFrame.origin = new Vec3(vec, z + (z - z2) * 0.25f, -1f);
					break;
				}
				case 4:
				{
					Vec2 vec;
					vec..ctor(6.45f, 7f);
					vec += (vec - this._initialCharacterFrame.origin.AsVec2) * (scale - 1f);
					this._targetCameraGlobalFrame.origin = new Vec3(vec, z - (z - z2) * 0.25f, -1f);
					break;
				}
				}
			}
			Vec2 vec2 = (this._targetCameraGlobalFrame.origin.AsVec2 - this._initialCharacterFrame.origin.AsVec2).Normalized();
			Vec3 origin = this._targetCameraGlobalFrame.origin;
			origin.AsVec2 = this._targetCameraGlobalFrame.origin.AsVec2 + vec2 * this._cameraCurrentDistanceAdder;
			origin.z += this._cameraCurrentElevationAdder;
			this._camera.Frame = new MatrixFrame(this._camera.Frame.rotation, this._camera.Frame.origin * (1f - 10f * dt) + origin * 10f * dt);
			this.SceneLayer.SetCamera(this._camera);
		}

		// Token: 0x040001D3 RID: 467
		private const int ViewOrderPriority = 1;

		// Token: 0x040001D4 RID: 468
		private Scene _facegenScene;

		// Token: 0x040001D5 RID: 469
		private MBAgentRendererSceneController _agentRendererSceneController;

		// Token: 0x040001D6 RID: 470
		private IGauntletMovie _viewMovie;

		// Token: 0x040001DA RID: 474
		private AgentVisuals _visualToShow;

		// Token: 0x040001DB RID: 475
		private List<KeyValuePair<AgentVisuals, int>> _visualsBeingPrepared;

		// Token: 0x040001DC RID: 476
		private readonly bool _openedFromMultiplayer;

		// Token: 0x040001DD RID: 477
		private AgentVisuals _nextVisualToShow;

		// Token: 0x040001DE RID: 478
		private int _currentAgentVisualIndex;

		// Token: 0x040001DF RID: 479
		private bool _refreshCharacterEntityNextFrame;

		// Token: 0x040001E0 RID: 480
		private MatrixFrame _initialCharacterFrame;

		// Token: 0x040001E1 RID: 481
		private bool _setMorphAnimNextFrame;

		// Token: 0x040001E2 RID: 482
		private string _nextMorphAnimToSet = "";

		// Token: 0x040001E3 RID: 483
		private bool _nextMorphAnimLoopValue;

		// Token: 0x040001E4 RID: 484
		private readonly ActionIndexCache act_inventory_idle_cached = ActionIndexCache.Create("act_inventory_idle");

		// Token: 0x040001E5 RID: 485
		private List<BodyProperties> _templateBodyProperties;

		// Token: 0x040001E6 RID: 486
		private readonly ActionIndexCache act_inventory_idle_start_cached = ActionIndexCache.Create("act_inventory_idle_start");

		// Token: 0x040001E7 RID: 487
		private readonly ActionIndexCache act_command_leftstance_cached = ActionIndexCache.Create("act_command_leftstance");

		// Token: 0x040001E9 RID: 489
		private readonly ControlCharacterCreationStage _affirmativeAction;

		// Token: 0x040001EA RID: 490
		private readonly ControlCharacterCreationStage _negativeAction;

		// Token: 0x040001EB RID: 491
		private readonly ControlCharacterCreationStageReturnInt _getTotalStageCountAction;

		// Token: 0x040001EC RID: 492
		private readonly ControlCharacterCreationStageReturnInt _getCurrentStageIndexAction;

		// Token: 0x040001ED RID: 493
		private readonly ControlCharacterCreationStageReturnInt _getFurthestIndexAction;

		// Token: 0x040001EE RID: 494
		private readonly ControlCharacterCreationStageWithInt _goToIndexAction;

		// Token: 0x040001EF RID: 495
		public bool IsDressed;

		// Token: 0x040001F0 RID: 496
		public SkeletonType SkeletonType;

		// Token: 0x040001F1 RID: 497
		private Equipment _dressedEquipment;

		// Token: 0x040001F2 RID: 498
		private bool _makeSound = true;

		// Token: 0x040001F3 RID: 499
		private Camera _camera;

		// Token: 0x040001F4 RID: 500
		private int _cameraLookMode;

		// Token: 0x040001F5 RID: 501
		private MatrixFrame _targetCameraGlobalFrame;

		// Token: 0x040001F6 RID: 502
		private MatrixFrame _defaultCameraGlobalFrame;

		// Token: 0x040001F7 RID: 503
		private float _characterCurrentRotation;

		// Token: 0x040001F8 RID: 504
		private float _characterTargetRotation;

		// Token: 0x040001F9 RID: 505
		private float _cameraCurrentDistanceAdder;

		// Token: 0x040001FA RID: 506
		private float _cameraCurrentElevationAdder;

		// Token: 0x040001FB RID: 507
		private SpriteCategory _facegenCategory;
	}
}
