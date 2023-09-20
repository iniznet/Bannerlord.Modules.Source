using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.Tableaus
{
	// Token: 0x02000021 RID: 33
	public class CharacterTableau
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000118 RID: 280 RVA: 0x000093E0 File Offset: 0x000075E0
		// (set) Token: 0x06000119 RID: 281 RVA: 0x000093E8 File Offset: 0x000075E8
		public Texture Texture { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600011A RID: 282 RVA: 0x000093F1 File Offset: 0x000075F1
		private TableauView View
		{
			get
			{
				if (this.Texture != null)
				{
					return this.Texture.TableauView;
				}
				return null;
			}
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00009410 File Offset: 0x00007610
		public CharacterTableau()
		{
			this._isVisualsDirty = false;
			this._equipment = new Equipment();
			this.SetEnabled(true);
			this.FirstTimeInit();
		}

		// Token: 0x0600011C RID: 284 RVA: 0x000094E0 File Offset: 0x000076E0
		public void OnTick(float dt)
		{
			if (this._isEnabled && this._isRotatingCharacter)
			{
				this.UpdateCharacterRotation((int)Input.MouseMoveX);
			}
			if (this._animationFrequencyThreshold > this._animationGap)
			{
				this._animationGap += dt;
			}
			if (this._isEnabled)
			{
				AgentVisuals agentVisuals = this._agentVisuals;
				if (agentVisuals != null)
				{
					agentVisuals.TickVisuals();
				}
				AgentVisuals oldAgentVisuals = this._oldAgentVisuals;
				if (oldAgentVisuals != null)
				{
					oldAgentVisuals.TickVisuals();
				}
				AgentVisuals mountVisuals = this._mountVisuals;
				if (mountVisuals != null)
				{
					mountVisuals.TickVisuals();
				}
				AgentVisuals oldMountVisuals = this._oldMountVisuals;
				if (oldMountVisuals != null)
				{
					oldMountVisuals.TickVisuals();
				}
			}
			if (this.View != null)
			{
				if (this._continuousRenderCamera == null)
				{
					this._continuousRenderCamera = Camera.CreateCamera();
				}
				this.View.SetDoNotRenderThisFrame(false);
			}
			if (this._isVisualsDirty)
			{
				this.RefreshCharacterTableau(this._oldEquipment);
				this._oldEquipment = null;
				this._isVisualsDirty = false;
			}
			if (this._agentVisualLoadingCounter > 0 && this._agentVisuals.GetEntity().CheckResources(true, true))
			{
				this._agentVisualLoadingCounter--;
			}
			if (this._mountVisualLoadingCounter > 0 && this._mountVisuals.GetEntity().CheckResources(true, true))
			{
				this._mountVisualLoadingCounter--;
			}
			if (this._mountVisualLoadingCounter == 0 && this._agentVisualLoadingCounter == 0)
			{
				AgentVisuals oldMountVisuals2 = this._oldMountVisuals;
				if (oldMountVisuals2 != null)
				{
					oldMountVisuals2.SetVisible(false);
				}
				AgentVisuals mountVisuals2 = this._mountVisuals;
				if (mountVisuals2 != null)
				{
					mountVisuals2.SetVisible(this._bodyProperties != BodyProperties.Default);
				}
				AgentVisuals oldAgentVisuals2 = this._oldAgentVisuals;
				if (oldAgentVisuals2 != null)
				{
					oldAgentVisuals2.SetVisible(false);
				}
				AgentVisuals agentVisuals2 = this._agentVisuals;
				if (agentVisuals2 == null)
				{
					return;
				}
				agentVisuals2.SetVisible(this._bodyProperties != BodyProperties.Default);
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00009691 File Offset: 0x00007891
		private void SetEnabled(bool enabled)
		{
			this._isEnabled = enabled;
			TableauView view = this.View;
			if (view == null)
			{
				return;
			}
			view.SetEnable(this._isEnabled);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x000096B0 File Offset: 0x000078B0
		public void SetTargetSize(int width, int height)
		{
			this._isRotatingCharacter = false;
			this._latestWidth = width;
			this._latestHeight = height;
			if (width <= 0 || height <= 0)
			{
				this._tableauSizeX = 10;
				this._tableauSizeY = 10;
			}
			else
			{
				this.RenderScale = NativeOptions.GetConfig(21) / 100f;
				this._tableauSizeX = (int)((float)width * this._customRenderScale * this.RenderScale);
				this._tableauSizeY = (int)((float)height * this._customRenderScale * this.RenderScale);
			}
			this._cameraRatio = (float)this._tableauSizeX / (float)this._tableauSizeY;
			TableauView view = this.View;
			if (view != null)
			{
				view.SetEnable(false);
			}
			TableauView view2 = this.View;
			if (view2 != null)
			{
				view2.AddClearTask(true);
			}
			Texture texture = this.Texture;
			if (texture != null)
			{
				texture.ReleaseNextFrame();
			}
			this.Texture = TableauView.AddTableau("CharacterTableau", new RenderTargetComponent.TextureUpdateEventHandler(this.CharacterTableauContinuousRenderFunction), this._tableauScene, this._tableauSizeX, this._tableauSizeY);
			this.Texture.TableauView.SetSceneUsesContour(false);
			this.Texture.TableauView.SetFocusedShadowmap(true, ref this._initialSpawnFrame.origin, 2.55f);
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000097D7 File Offset: 0x000079D7
		public void SetCharStringID(string charStringId)
		{
			if (this._charStringId != charStringId)
			{
				this._charStringId = charStringId;
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x000097F0 File Offset: 0x000079F0
		public void OnFinalize()
		{
			if (this._continuousRenderCamera != null)
			{
				this._continuousRenderCamera.ReleaseCameraEntity();
				this._continuousRenderCamera = null;
			}
			AgentVisuals agentVisuals = this._agentVisuals;
			if (agentVisuals != null)
			{
				agentVisuals.ResetNextFrame();
			}
			this._agentVisuals = null;
			AgentVisuals mountVisuals = this._mountVisuals;
			if (mountVisuals != null)
			{
				mountVisuals.ResetNextFrame();
			}
			this._mountVisuals = null;
			AgentVisuals oldAgentVisuals = this._oldAgentVisuals;
			if (oldAgentVisuals != null)
			{
				oldAgentVisuals.ResetNextFrame();
			}
			this._oldAgentVisuals = null;
			AgentVisuals oldMountVisuals = this._oldMountVisuals;
			if (oldMountVisuals != null)
			{
				oldMountVisuals.ResetNextFrame();
			}
			this._oldMountVisuals = null;
			TableauView view = this.View;
			if (view != null)
			{
				view.SetEnable(false);
			}
			if (this._tableauScene != null)
			{
				if (this._bannerEntity != null)
				{
					this._tableauScene.RemoveEntity(this._bannerEntity, 0);
					this._bannerEntity = null;
				}
				if (this._agentRendererSceneController != null)
				{
					if (view != null)
					{
						view.SetEnable(false);
					}
					if (view != null)
					{
						view.AddClearTask(false);
					}
					MBAgentRendererSceneController.DestructAgentRendererSceneController(this._tableauScene, this._agentRendererSceneController, false);
					this._agentRendererSceneController = null;
					this._tableauScene.ManualInvalidate();
					this._tableauScene = null;
				}
				else
				{
					TableauCacheManager.Current.ReturnCachedInventoryTableauScene();
					TableauCacheManager.Current.ReturnCachedInventoryTableauScene();
					if (view != null)
					{
						view.AddClearTask(true);
					}
					this._tableauScene = null;
				}
			}
			Texture texture = this.Texture;
			if (texture != null)
			{
				texture.ReleaseNextFrame();
			}
			this.Texture = null;
			this._isFinalized = true;
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00009954 File Offset: 0x00007B54
		public void SetBodyProperties(string bodyPropertiesCode)
		{
			if (this._bodyPropertiesCode != bodyPropertiesCode)
			{
				this._bodyPropertiesCode = bodyPropertiesCode;
				BodyProperties bodyProperties;
				if (!string.IsNullOrEmpty(bodyPropertiesCode) && BodyProperties.FromString(bodyPropertiesCode, ref bodyProperties))
				{
					this._bodyProperties = bodyProperties;
				}
				else
				{
					this._bodyProperties = BodyProperties.Default;
				}
				this._isVisualsDirty = true;
			}
		}

		// Token: 0x06000122 RID: 290 RVA: 0x000099A3 File Offset: 0x00007BA3
		public void SetStanceIndex(int index)
		{
			this._stanceIndex = index;
			this._isVisualsDirty = true;
		}

		// Token: 0x06000123 RID: 291 RVA: 0x000099B3 File Offset: 0x00007BB3
		public void SetCustomRenderScale(float value)
		{
			if (!MBMath.ApproximatelyEqualsTo(this._customRenderScale, value, 1E-05f))
			{
				this._customRenderScale = value;
				if (this._latestWidth != -1 && this._latestHeight != -1)
				{
					this.SetTargetSize(this._latestWidth, this._latestHeight);
				}
			}
		}

		// Token: 0x06000124 RID: 292 RVA: 0x000099F4 File Offset: 0x00007BF4
		private void AdjustCharacterForStanceIndex()
		{
			switch (this._stanceIndex)
			{
			case 0:
			{
				AgentVisuals agentVisuals = this._agentVisuals;
				if (agentVisuals != null)
				{
					agentVisuals.SetAction(this.GetIdleAction(), 0f, true);
				}
				AgentVisuals oldAgentVisuals = this._oldAgentVisuals;
				if (oldAgentVisuals != null)
				{
					oldAgentVisuals.SetAction(this.GetIdleAction(), 0f, true);
				}
				break;
			}
			case 1:
			{
				this._camPos = this._camPosGatheredFromScene;
				this._camPos.Elevate(-2f);
				this._camPos.Advance(0.5f);
				AgentVisuals agentVisuals2 = this._agentVisuals;
				if (agentVisuals2 != null)
				{
					agentVisuals2.SetAction(this.GetIdleAction(), 0f, true);
				}
				AgentVisuals oldAgentVisuals2 = this._oldAgentVisuals;
				if (oldAgentVisuals2 != null)
				{
					oldAgentVisuals2.SetAction(this.GetIdleAction(), 0f, true);
				}
				break;
			}
			case 2:
			case 4:
				if (this._agentVisuals != null)
				{
					this._camPos = this._camPosGatheredFromScene;
					if (this._equipment[10].Item != null)
					{
						this._camPos.Advance(0.5f);
						this._agentVisuals.SetAction(MBSkeletonExtensions.GetActionAtChannel(this._mountVisuals.GetEntity().Skeleton, 0), this._mountVisuals.GetEntity().Skeleton.GetAnimationParameterAtChannel(0), true);
						this._oldAgentVisuals.SetAction(MBSkeletonExtensions.GetActionAtChannel(this._mountVisuals.GetEntity().Skeleton, 0), this._mountVisuals.GetEntity().Skeleton.GetAnimationParameterAtChannel(0), true);
					}
					else
					{
						this._camPos.Elevate(-2f);
						this._camPos.Advance(0.5f);
						this._agentVisuals.SetAction(this.GetIdleAction(), 0f, true);
						this._oldAgentVisuals.SetAction(this.GetIdleAction(), 0f, true);
					}
				}
				break;
			case 3:
			{
				AgentVisuals agentVisuals3 = this._agentVisuals;
				if (agentVisuals3 != null)
				{
					agentVisuals3.SetAction(CharacterTableau.act_cheer_1, 0f, true);
				}
				AgentVisuals oldAgentVisuals3 = this._oldAgentVisuals;
				if (oldAgentVisuals3 != null)
				{
					oldAgentVisuals3.SetAction(CharacterTableau.act_cheer_1, 0f, true);
				}
				break;
			}
			}
			if (this._agentVisuals != null)
			{
				GameEntity entity = this._agentVisuals.GetEntity();
				Skeleton skeleton = entity.Skeleton;
				skeleton.TickAnimations(0.01f, this._agentVisuals.GetVisuals().GetGlobalFrame(), true);
				if (!string.IsNullOrEmpty(this._idleFaceAnim))
				{
					MBSkeletonExtensions.SetFacialAnimation(skeleton, 1, this._idleFaceAnim, false, true);
				}
				entity.ManualInvalidate();
				skeleton.ManualInvalidate();
			}
			if (this._oldAgentVisuals != null)
			{
				GameEntity entity2 = this._oldAgentVisuals.GetEntity();
				Skeleton skeleton2 = entity2.Skeleton;
				skeleton2.TickAnimations(0.01f, this._oldAgentVisuals.GetVisuals().GetGlobalFrame(), true);
				if (!string.IsNullOrEmpty(this._idleFaceAnim))
				{
					MBSkeletonExtensions.SetFacialAnimation(skeleton2, 1, this._idleFaceAnim, false, true);
				}
				entity2.ManualInvalidate();
				skeleton2.ManualInvalidate();
			}
			if (this._mountVisuals != null)
			{
				GameEntity entity3 = this._mountVisuals.GetEntity();
				Skeleton skeleton3 = entity3.Skeleton;
				skeleton3.TickAnimations(0.01f, this._mountVisuals.GetVisuals().GetGlobalFrame(), true);
				if (!string.IsNullOrEmpty(this._idleFaceAnim))
				{
					MBSkeletonExtensions.SetFacialAnimation(skeleton3, 1, this._idleFaceAnim, false, true);
				}
				entity3.ManualInvalidate();
				skeleton3.ManualInvalidate();
			}
			if (this._oldMountVisuals != null)
			{
				GameEntity entity4 = this._oldMountVisuals.GetEntity();
				Skeleton skeleton4 = entity4.Skeleton;
				skeleton4.TickAnimations(0.01f, this._oldMountVisuals.GetVisuals().GetGlobalFrame(), true);
				entity4.ManualInvalidate();
				skeleton4.ManualInvalidate();
			}
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00009D74 File Offset: 0x00007F74
		private void ForceRefresh()
		{
			int stanceIndex = this._stanceIndex;
			this._stanceIndex = 0;
			this.SetStanceIndex(stanceIndex);
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00009D96 File Offset: 0x00007F96
		public void SetIsFemale(bool isFemale)
		{
			this._isFemale = isFemale;
			this._isVisualsDirty = true;
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00009DA6 File Offset: 0x00007FA6
		public void SetIsBannerShownInBackground(bool isBannerShownInBackground)
		{
			this._isBannerShownInBackground = isBannerShownInBackground;
			this._isVisualsDirty = true;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00009DB6 File Offset: 0x00007FB6
		public void SetRace(int race)
		{
			this._race = race;
			this._isVisualsDirty = true;
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00009DC6 File Offset: 0x00007FC6
		public void SetIdleAction(string idleAction)
		{
			if (!string.IsNullOrEmpty(idleAction))
			{
				this._idleAction = ActionIndexCache.Create(idleAction);
				this._isVisualsDirty = true;
			}
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00009DE3 File Offset: 0x00007FE3
		public void SetIdleFaceAnim(string idleFaceAnim)
		{
			if (!string.IsNullOrEmpty(idleFaceAnim))
			{
				this._idleFaceAnim = idleFaceAnim;
				this._isVisualsDirty = true;
			}
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00009DFC File Offset: 0x00007FFC
		public void SetEquipmentCode(string equipmentCode)
		{
			if (this._equipmentCode != equipmentCode && !string.IsNullOrEmpty(equipmentCode))
			{
				this._oldEquipment = Equipment.CreateFromEquipmentCode(this._equipmentCode);
				this._equipmentCode = equipmentCode;
				this._equipment = Equipment.CreateFromEquipmentCode(equipmentCode);
				this._bannerItem = this.GetAndRemoveBannerFromEquipment(ref this._equipment);
				this._isVisualsDirty = true;
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00009E5C File Offset: 0x0000805C
		public void SetIsEquipmentAnimActive(bool value)
		{
			this._isEquipmentAnimActive = value;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00009E65 File Offset: 0x00008065
		public void SetMountCreationKey(string value)
		{
			if (this._mountCreationKey != value)
			{
				this._mountCreationKey = value;
				this._isVisualsDirty = true;
			}
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00009E83 File Offset: 0x00008083
		public void SetBannerCode(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this._banner = null;
			}
			else
			{
				this._banner = BannerCode.CreateFrom(value).CalculateBanner();
			}
			this._isVisualsDirty = true;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00009EAE File Offset: 0x000080AE
		public void SetArmorColor1(uint clothColor1)
		{
			if (this._clothColor1 != clothColor1)
			{
				this._clothColor1 = clothColor1;
				this._isVisualsDirty = true;
			}
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00009EC7 File Offset: 0x000080C7
		public void SetArmorColor2(uint clothColor2)
		{
			if (this._clothColor2 != clothColor2)
			{
				this._clothColor2 = clothColor2;
				this._isVisualsDirty = true;
			}
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00009EE0 File Offset: 0x000080E0
		private ActionIndexCache GetIdleAction()
		{
			return this._idleAction ?? CharacterTableau.act_inventory_idle_start;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00009EF4 File Offset: 0x000080F4
		private void RefreshCharacterTableau(Equipment oldEquipment = null)
		{
			this.UpdateMount(this._stanceIndex == 4);
			this.UpdateBannerItem();
			if (this._mountVisuals == null && this._isCharacterMountPlacesSwapped)
			{
				this._isCharacterMountPlacesSwapped = false;
				this._mainCharacterRotation = 0f;
			}
			if (this._agentVisuals != null)
			{
				bool visibilityExcludeParents = this._oldAgentVisuals.GetEntity().GetVisibilityExcludeParents();
				AgentVisuals agentVisuals = this._agentVisuals;
				this._agentVisuals = this._oldAgentVisuals;
				this._oldAgentVisuals = agentVisuals;
				this._agentVisualLoadingCounter = 1;
				AgentVisualsData copyAgentVisualsData = this._agentVisuals.GetCopyAgentVisualsData();
				MatrixFrame matrixFrame = (this._isCharacterMountPlacesSwapped ? this._characterMountPositionFrame : this._initialSpawnFrame);
				if (!this._isCharacterMountPlacesSwapped)
				{
					matrixFrame.rotation.RotateAboutUp(this._mainCharacterRotation);
				}
				copyAgentVisualsData.BodyProperties(this._bodyProperties).SkeletonType(this._isFemale ? 1 : 0).Frame(matrixFrame)
					.ActionSet(MBGlobals.GetActionSetWithSuffix(copyAgentVisualsData.MonsterData, this._isFemale, "_warrior"))
					.Equipment(this._equipment)
					.Banner(this._banner)
					.UseMorphAnims(true)
					.ClothColor1(this._clothColor1)
					.ClothColor2(this._clothColor2)
					.Race(this._race);
				if (this._initialLoadingCounter > 0)
				{
					this._initialLoadingCounter--;
				}
				this._agentVisuals.Refresh(false, copyAgentVisualsData, false);
				this._agentVisuals.SetVisible(false);
				if (this._initialLoadingCounter == 0)
				{
					this._oldAgentVisuals.SetVisible(visibilityExcludeParents);
				}
				if (oldEquipment != null && this._animationFrequencyThreshold <= this._animationGap && this._isEquipmentAnimActive)
				{
					if (this._equipment[8].Item != null && oldEquipment[8].Item != this._equipment[8].Item)
					{
						MBSkeletonExtensions.SetAgentActionChannel(this._agentVisuals.GetVisuals().GetSkeleton(), 0, CharacterTableau.act_inventory_glove_equip, 0f, -0.2f, true);
						this._animationGap = 0f;
					}
					else if (this._equipment[6].Item != null && oldEquipment[6].Item != this._equipment[6].Item)
					{
						MBSkeletonExtensions.SetAgentActionChannel(this._agentVisuals.GetVisuals().GetSkeleton(), 0, CharacterTableau.act_inventory_cloth_equip, 0f, -0.2f, true);
						this._animationGap = 0f;
					}
				}
				this._agentVisuals.GetEntity().CheckResources(true, true);
			}
			this.AdjustCharacterForStanceIndex();
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000A18E File Offset: 0x0000838E
		public void RotateCharacter(bool value)
		{
			this._isRotatingCharacter = value;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000A197 File Offset: 0x00008397
		public void TriggerCharacterMountPlacesSwap()
		{
			this._mainCharacterRotation = 0f;
			this._isCharacterMountPlacesSwapped = !this._isCharacterMountPlacesSwapped;
			this._isVisualsDirty = true;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000A1BA File Offset: 0x000083BA
		public void OnCharacterTableauMouseMove(int mouseMoveX)
		{
			this.UpdateCharacterRotation(mouseMoveX);
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000A1C4 File Offset: 0x000083C4
		private void UpdateCharacterRotation(int mouseMoveX)
		{
			if (this._agentVisuals != null)
			{
				float num = (float)mouseMoveX * 0.005f;
				this._mainCharacterRotation += num;
				if (this._isCharacterMountPlacesSwapped)
				{
					MatrixFrame frame = this._mountVisuals.GetEntity().GetFrame();
					frame.rotation.RotateAboutUp(num);
					this._mountVisuals.GetEntity().SetFrame(ref frame);
					return;
				}
				MatrixFrame frame2 = this._agentVisuals.GetEntity().GetFrame();
				frame2.rotation.RotateAboutUp(num);
				this._agentVisuals.GetEntity().SetFrame(ref frame2);
			}
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000A25C File Offset: 0x0000845C
		private void FirstTimeInit()
		{
			if (this._equipment != null)
			{
				if (this._tableauScene == null)
				{
					if (TableauCacheManager.Current.IsCachedInventoryTableauSceneUsed())
					{
						this._tableauScene = Scene.CreateNewScene(true, false, 0, "mono_renderscene");
						this._tableauScene.SetName("CharacterTableau");
						this._tableauScene.DisableStaticShadows(true);
						this._tableauScene.SetClothSimulationState(true);
						this._agentRendererSceneController = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(this._tableauScene, 32);
						SceneInitializationData sceneInitializationData;
						sceneInitializationData..ctor(true);
						sceneInitializationData.InitPhysicsWorld = false;
						sceneInitializationData.DoNotUseLoadingScreen = true;
						this._tableauScene.Read("inventory_character_scene", ref sceneInitializationData, "");
					}
					else
					{
						this._tableauScene = TableauCacheManager.Current.GetCachedInventoryTableauScene();
					}
					this._tableauScene.SetShadow(true);
					this._tableauScene.SetClothSimulationState(true);
					this._camPos = (this._camPosGatheredFromScene = TableauCacheManager.Current.InventorySceneCameraFrame);
					this._mountSpawnPoint = this._tableauScene.FindEntityWithTag("horse_inv").GetGlobalFrame();
					this._bannerSpawnPoint = this._tableauScene.FindEntityWithTag("banner_inv").GetGlobalFrame();
					this._initialSpawnFrame = this._tableauScene.FindEntityWithTag("agent_inv").GetGlobalFrame();
					this._characterMountPositionFrame = new MatrixFrame(this._initialSpawnFrame.rotation, this._mountSpawnPoint.origin);
					this._characterMountPositionFrame.Strafe(-0.25f);
					this._mountCharacterPositionFrame = new MatrixFrame(this._mountSpawnPoint.rotation, this._initialSpawnFrame.origin);
					this._mountCharacterPositionFrame.Strafe(0.25f);
					if (this._agentRendererSceneController != null)
					{
						this._tableauScene.RemoveEntity(this._tableauScene.FindEntityWithTag("agent_inv"), 99);
						this._tableauScene.RemoveEntity(this._tableauScene.FindEntityWithTag("horse_inv"), 100);
						this._tableauScene.RemoveEntity(this._tableauScene.FindEntityWithTag("banner_inv"), 101);
					}
				}
				this.InitializeAgentVisuals();
				this._isVisualsDirty = true;
			}
		}

		// Token: 0x06000138 RID: 312 RVA: 0x0000A474 File Offset: 0x00008674
		private void InitializeAgentVisuals()
		{
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(this._race);
			this._oldAgentVisuals = AgentVisuals.Create(new AgentVisualsData().Banner(this._banner).Equipment(this._equipment).BodyProperties(this._bodyProperties)
				.Race(this._race)
				.Frame(this._initialSpawnFrame)
				.UseMorphAnims(true)
				.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, this._isFemale, "_warrior"))
				.ActionCode(this.GetIdleAction())
				.Scene(this._tableauScene)
				.Monster(baseMonsterFromRace)
				.PrepareImmediately(false)
				.SkeletonType(this._isFemale ? 1 : 0)
				.ClothColor1(this._clothColor1)
				.ClothColor2(this._clothColor2)
				.CharacterObjectStringId(this._charStringId), "CharacterTableau", false, false, false);
			this._oldAgentVisuals.SetAgentLodZeroOrMaxExternal(true);
			this._oldAgentVisuals.SetVisible(false);
			this._agentVisuals = AgentVisuals.Create(new AgentVisualsData().Banner(this._banner).Equipment(this._equipment).BodyProperties(this._bodyProperties)
				.Race(this._race)
				.Frame(this._initialSpawnFrame)
				.UseMorphAnims(true)
				.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, this._isFemale, "_warrior"))
				.ActionCode(this.GetIdleAction())
				.Scene(this._tableauScene)
				.Monster(baseMonsterFromRace)
				.PrepareImmediately(false)
				.SkeletonType(this._isFemale ? 1 : 0)
				.ClothColor1(this._clothColor1)
				.ClothColor2(this._clothColor2)
				.CharacterObjectStringId(this._charStringId), "CharacterTableau", false, false, false);
			this._agentVisuals.SetAgentLodZeroOrMaxExternal(true);
			this._agentVisuals.SetVisible(false);
			this._initialLoadingCounter = 2;
			if (!string.IsNullOrEmpty(this._idleFaceAnim))
			{
				MBSkeletonExtensions.SetFacialAnimation(this._agentVisuals.GetVisuals().GetSkeleton(), 1, this._idleFaceAnim, false, true);
				MBSkeletonExtensions.SetFacialAnimation(this._oldAgentVisuals.GetVisuals().GetSkeleton(), 1, this._idleFaceAnim, false, true);
			}
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000A68C File Offset: 0x0000888C
		private void UpdateMount(bool isRiderAgentMounted = false)
		{
			ItemObject item = this._equipment[10].Item;
			if (((item != null) ? item.HorseComponent : null) != null)
			{
				ItemObject item2 = this._equipment[10].Item;
				Monster monster = item2.HorseComponent.Monster;
				Equipment equipment = new Equipment();
				equipment[10] = this._equipment[10];
				equipment[11] = this._equipment[11];
				Equipment equipment2 = equipment;
				MatrixFrame matrixFrame = (this._isCharacterMountPlacesSwapped ? this._mountCharacterPositionFrame : this._mountSpawnPoint);
				if (this._isCharacterMountPlacesSwapped)
				{
					matrixFrame.rotation.RotateAboutUp(this._mainCharacterRotation);
				}
				if (this._oldMountVisuals != null)
				{
					this._oldMountVisuals.ResetNextFrame();
				}
				this._oldMountVisuals = this._mountVisuals;
				this._mountVisualLoadingCounter = 3;
				AgentVisualsData agentVisualsData = new AgentVisualsData();
				agentVisualsData.Banner(this._banner).Equipment(equipment2).Frame(matrixFrame)
					.Scale(item2.ScaleFactor)
					.ActionSet(MBGlobals.GetActionSet(monster.ActionSetCode))
					.ActionCode(isRiderAgentMounted ? ((monster.MonsterUsage == "camel") ? CharacterTableau.act_camel_stand : CharacterTableau.act_horse_stand) : this.GetIdleAction())
					.Scene(this._tableauScene)
					.Monster(monster)
					.PrepareImmediately(false)
					.ClothColor1(this._clothColor1)
					.ClothColor2(this._clothColor2)
					.MountCreationKey(this._mountCreationKey);
				this._mountVisuals = AgentVisuals.Create(agentVisualsData, "MountTableau", false, false, false);
				this._mountVisuals.SetAgentLodZeroOrMaxExternal(true);
				this._mountVisuals.SetVisible(false);
				this._mountVisuals.GetEntity().CheckResources(true, true);
				return;
			}
			if (this._mountVisuals != null)
			{
				this._mountVisuals.Reset();
				this._mountVisuals = null;
				this._mountVisualLoadingCounter = 0;
			}
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000A870 File Offset: 0x00008A70
		private void UpdateBannerItem()
		{
			if (this._bannerEntity != null)
			{
				this._tableauScene.RemoveEntity(this._bannerEntity, 0);
				this._bannerEntity = null;
			}
			if (this._isBannerShownInBackground && this._bannerItem != null)
			{
				this._bannerEntity = GameEntity.CreateEmpty(this._tableauScene, true);
				this._bannerEntity.SetFrame(ref this._bannerSpawnPoint);
				this._bannerEntity.AddMultiMesh(this._bannerItem.GetMultiMeshCopy(), true);
				if (this._banner != null)
				{
					this._banner.GetTableauTextureLarge(delegate(Texture t)
					{
						this.OnBannerTableauRenderDone(t);
					});
				}
			}
		}

		// Token: 0x0600013B RID: 315 RVA: 0x0000A910 File Offset: 0x00008B10
		private void OnBannerTableauRenderDone(Texture newTexture)
		{
			if (this._isFinalized)
			{
				return;
			}
			if (this._bannerEntity == null)
			{
				return;
			}
			foreach (Mesh mesh in this._bannerEntity.GetAllMeshesWithTag("banner_replacement_mesh"))
			{
				this.ApplyBannerTextureToMesh(mesh, newTexture);
			}
			Skeleton skeleton = this._bannerEntity.Skeleton;
			if (((skeleton != null) ? skeleton.GetAllMeshes() : null) != null)
			{
				Skeleton skeleton2 = this._bannerEntity.Skeleton;
				foreach (Mesh mesh2 in ((skeleton2 != null) ? skeleton2.GetAllMeshes() : null))
				{
					if (mesh2.HasTag("banner_replacement_mesh"))
					{
						this.ApplyBannerTextureToMesh(mesh2, newTexture);
					}
				}
			}
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000A9F4 File Offset: 0x00008BF4
		private void ApplyBannerTextureToMesh(Mesh bannerMesh, Texture bannerTexture)
		{
			if (bannerMesh != null)
			{
				Material material = bannerMesh.GetMaterial().CreateCopy();
				material.SetTexture(1, bannerTexture);
				uint num = (uint)material.GetShader().GetMaterialShaderFlagMask("use_tableau_blending", true);
				ulong shaderFlags = material.GetShaderFlags();
				material.SetShaderFlags(shaderFlags | (ulong)num);
				bannerMesh.SetMaterial(material);
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000AA4C File Offset: 0x00008C4C
		private ItemObject GetAndRemoveBannerFromEquipment(ref Equipment equipment)
		{
			ItemObject itemObject = null;
			ItemObject item = equipment[4].Item;
			if (item != null && item.IsBannerItem)
			{
				itemObject = equipment[4].Item;
				equipment[4] = EquipmentElement.Invalid;
			}
			return itemObject;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000AA98 File Offset: 0x00008C98
		internal void CharacterTableauContinuousRenderFunction(Texture sender, EventArgs e)
		{
			Scene scene = (Scene)sender.UserData;
			TableauView tableauView = sender.TableauView;
			if (scene == null)
			{
				tableauView.SetContinuousRendering(false);
				tableauView.SetDeleteAfterRendering(true);
				return;
			}
			scene.EnsurePostfxSystem();
			scene.SetDofMode(false);
			scene.SetMotionBlurMode(false);
			scene.SetBloom(true);
			scene.SetDynamicShadowmapCascadesRadiusMultiplier(0.31f);
			tableauView.SetRenderWithPostfx(true);
			float cameraRatio = this._cameraRatio;
			MatrixFrame camPos = this._camPos;
			if (this._continuousRenderCamera != null)
			{
				Camera continuousRenderCamera = this._continuousRenderCamera;
				this._continuousRenderCamera = null;
				continuousRenderCamera.SetFovVertical(0.7853982f, cameraRatio, 0.2f, 200f);
				continuousRenderCamera.Frame = camPos;
				tableauView.SetCamera(continuousRenderCamera);
				tableauView.SetScene(scene);
				tableauView.SetSceneUsesSkybox(false);
				tableauView.SetDeleteAfterRendering(false);
				tableauView.SetContinuousRendering(true);
				tableauView.SetDoNotRenderThisFrame(true);
				tableauView.SetClearColor(0U);
				tableauView.SetFocusedShadowmap(true, ref this._initialSpawnFrame.origin, 1.55f);
			}
		}

		// Token: 0x04000089 RID: 137
		private bool _isFinalized;

		// Token: 0x0400008A RID: 138
		private MatrixFrame _mountSpawnPoint;

		// Token: 0x0400008B RID: 139
		private MatrixFrame _bannerSpawnPoint;

		// Token: 0x0400008C RID: 140
		private float _animationFrequencyThreshold = 2.5f;

		// Token: 0x0400008D RID: 141
		private MatrixFrame _initialSpawnFrame;

		// Token: 0x0400008E RID: 142
		private MatrixFrame _characterMountPositionFrame;

		// Token: 0x0400008F RID: 143
		private MatrixFrame _mountCharacterPositionFrame;

		// Token: 0x04000090 RID: 144
		private AgentVisuals _agentVisuals;

		// Token: 0x04000091 RID: 145
		private AgentVisuals _mountVisuals;

		// Token: 0x04000092 RID: 146
		private int _agentVisualLoadingCounter;

		// Token: 0x04000093 RID: 147
		private int _mountVisualLoadingCounter;

		// Token: 0x04000094 RID: 148
		private AgentVisuals _oldAgentVisuals;

		// Token: 0x04000095 RID: 149
		private AgentVisuals _oldMountVisuals;

		// Token: 0x04000096 RID: 150
		private int _initialLoadingCounter;

		// Token: 0x04000097 RID: 151
		private ActionIndexCache _idleAction;

		// Token: 0x04000098 RID: 152
		private string _idleFaceAnim;

		// Token: 0x04000099 RID: 153
		private Scene _tableauScene;

		// Token: 0x0400009A RID: 154
		private MBAgentRendererSceneController _agentRendererSceneController;

		// Token: 0x0400009B RID: 155
		private Camera _continuousRenderCamera;

		// Token: 0x0400009C RID: 156
		private float _cameraRatio;

		// Token: 0x0400009D RID: 157
		private MatrixFrame _camPos;

		// Token: 0x0400009E RID: 158
		private MatrixFrame _camPosGatheredFromScene;

		// Token: 0x0400009F RID: 159
		private string _charStringId;

		// Token: 0x040000A0 RID: 160
		private int _tableauSizeX;

		// Token: 0x040000A1 RID: 161
		private int _tableauSizeY;

		// Token: 0x040000A2 RID: 162
		private uint _clothColor1 = new Color(1f, 1f, 1f, 1f).ToUnsignedInteger();

		// Token: 0x040000A3 RID: 163
		private uint _clothColor2 = new Color(1f, 1f, 1f, 1f).ToUnsignedInteger();

		// Token: 0x040000A4 RID: 164
		private bool _isRotatingCharacter;

		// Token: 0x040000A5 RID: 165
		private bool _isCharacterMountPlacesSwapped;

		// Token: 0x040000A6 RID: 166
		private string _mountCreationKey = "";

		// Token: 0x040000A7 RID: 167
		private string _equipmentCode = "";

		// Token: 0x040000A8 RID: 168
		private bool _isEquipmentAnimActive;

		// Token: 0x040000A9 RID: 169
		private float _animationGap;

		// Token: 0x040000AA RID: 170
		private float _mainCharacterRotation;

		// Token: 0x040000AB RID: 171
		private bool _isEnabled;

		// Token: 0x040000AC RID: 172
		private float RenderScale = 1f;

		// Token: 0x040000AD RID: 173
		private float _customRenderScale = 1f;

		// Token: 0x040000AE RID: 174
		private int _latestWidth = -1;

		// Token: 0x040000AF RID: 175
		private int _latestHeight = -1;

		// Token: 0x040000B0 RID: 176
		private string _bodyPropertiesCode;

		// Token: 0x040000B1 RID: 177
		private BodyProperties _bodyProperties = BodyProperties.Default;

		// Token: 0x040000B2 RID: 178
		private bool _isFemale;

		// Token: 0x040000B3 RID: 179
		private CharacterViewModel.StanceTypes _stanceIndex;

		// Token: 0x040000B4 RID: 180
		private Equipment _equipment;

		// Token: 0x040000B5 RID: 181
		private Banner _banner;

		// Token: 0x040000B6 RID: 182
		private int _race;

		// Token: 0x040000B7 RID: 183
		private bool _isBannerShownInBackground;

		// Token: 0x040000B8 RID: 184
		private ItemObject _bannerItem;

		// Token: 0x040000B9 RID: 185
		private GameEntity _bannerEntity;

		// Token: 0x040000BA RID: 186
		private static readonly ActionIndexCache act_cheer_1 = ActionIndexCache.Create("act_arena_winner_1");

		// Token: 0x040000BB RID: 187
		private static readonly ActionIndexCache act_inventory_idle_start = ActionIndexCache.Create("act_inventory_idle_start");

		// Token: 0x040000BC RID: 188
		private static readonly ActionIndexCache act_inventory_glove_equip = ActionIndexCache.Create("act_inventory_glove_equip");

		// Token: 0x040000BD RID: 189
		private static readonly ActionIndexCache act_inventory_cloth_equip = ActionIndexCache.Create("act_inventory_cloth_equip");

		// Token: 0x040000BE RID: 190
		private static readonly ActionIndexCache act_horse_stand = ActionIndexCache.Create("act_inventory_idle_start");

		// Token: 0x040000BF RID: 191
		private static readonly ActionIndexCache act_camel_stand = ActionIndexCache.Create("act_inventory_idle_start");

		// Token: 0x040000C0 RID: 192
		private bool _isVisualsDirty;

		// Token: 0x040000C1 RID: 193
		private Equipment _oldEquipment;
	}
}
