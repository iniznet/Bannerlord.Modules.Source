using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.Tableaus
{
	public class CharacterTableau
	{
		public Texture Texture { get; private set; }

		public bool IsRunningCustomAnimation
		{
			get
			{
				return this._customAnimation != null || this._customAnimationStartScheduled;
			}
		}

		public bool ShouldLoopCustomAnimation { get; set; }

		public float CustomAnimationWaitDuration { get; set; }

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

		public CharacterTableau()
		{
			this._leftHandEquipmentIndex = -1;
			this._rightHandEquipmentIndex = -1;
			this._isVisualsDirty = false;
			this._equipment = new Equipment();
			this.SetEnabled(true);
			this.FirstTimeInit();
		}

		public void OnTick(float dt)
		{
			if (this._customAnimationStartScheduled)
			{
				this.StartCustomAnimation();
			}
			if (this._customAnimation != null && this._characterActionSet.IsValid)
			{
				this._customAnimationTimer += dt;
				float actionAnimationDuration = MBActionSet.GetActionAnimationDuration(this._characterActionSet, this._customAnimation);
				if (this._customAnimationTimer > actionAnimationDuration)
				{
					if (this._customAnimationTimer > actionAnimationDuration + this.CustomAnimationWaitDuration)
					{
						if (this.ShouldLoopCustomAnimation)
						{
							this.StartCustomAnimation();
						}
						else
						{
							this.StopCustomAnimationIfCantContinue();
						}
					}
					else
					{
						AgentVisuals agentVisuals = this._agentVisuals;
						if (agentVisuals != null)
						{
							agentVisuals.SetAction(this.GetIdleAction(), 0f, true);
						}
					}
				}
			}
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
				AgentVisuals agentVisuals2 = this._agentVisuals;
				if (agentVisuals2 != null)
				{
					agentVisuals2.TickVisuals();
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
				AgentVisuals agentVisuals3 = this._agentVisuals;
				if (agentVisuals3 != null)
				{
					agentVisuals3.SetVisible(this._bodyProperties != BodyProperties.Default);
				}
			}
			if (this._isEquipmentIndicesDirty)
			{
				this._agentVisuals.GetVisuals().SetWieldedWeaponIndices(this._rightHandEquipmentIndex, this._leftHandEquipmentIndex);
				this._isEquipmentIndicesDirty = false;
			}
		}

		public float GetCustomAnimationProgressRatio()
		{
			if (!(this._customAnimation != null) || !this._characterActionSet.IsValid)
			{
				return -1f;
			}
			float actionAnimationDuration = MBActionSet.GetActionAnimationDuration(this._characterActionSet, this._customAnimation);
			if (actionAnimationDuration == 0f)
			{
				return -1f;
			}
			return this._customAnimationTimer / actionAnimationDuration;
		}

		private void StopCustomAnimationIfCantContinue()
		{
			bool flag = false;
			if (this._agentVisuals != null && this._customAnimation != null && this._customAnimation.Index >= 0)
			{
				ActionIndexValueCache actionAnimationContinueToAction = MBActionSet.GetActionAnimationContinueToAction(this._characterActionSet, ActionIndexValueCache.Create(this._customAnimation));
				if (actionAnimationContinueToAction.Index >= 0)
				{
					this._customAnimationName = actionAnimationContinueToAction.Name;
					this.StartCustomAnimation();
					flag = true;
				}
			}
			if (!flag)
			{
				this.StopCustomAnimation();
				this._customAnimationTimer = -1f;
			}
		}

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

		public void SetLeftHandWieldedEquipmentIndex(int index)
		{
			this._leftHandEquipmentIndex = index;
			this._isEquipmentIndicesDirty = true;
		}

		public void SetRightHandWieldedEquipmentIndex(int index)
		{
			this._rightHandEquipmentIndex = index;
			this._isEquipmentIndicesDirty = true;
		}

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
				this.RenderScale = NativeOptions.GetConfig(25) / 100f;
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

		public void SetCharStringID(string charStringId)
		{
			if (this._charStringId != charStringId)
			{
				this._charStringId = charStringId;
			}
		}

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

		public void SetStanceIndex(int index)
		{
			this._stanceIndex = index;
			this._isVisualsDirty = true;
		}

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

		private void ForceRefresh()
		{
			int stanceIndex = this._stanceIndex;
			this._stanceIndex = 0;
			this.SetStanceIndex(stanceIndex);
		}

		public void SetIsFemale(bool isFemale)
		{
			this._isFemale = isFemale;
			this._isVisualsDirty = true;
		}

		public void SetIsBannerShownInBackground(bool isBannerShownInBackground)
		{
			this._isBannerShownInBackground = isBannerShownInBackground;
			this._isVisualsDirty = true;
		}

		public void SetRace(int race)
		{
			this._race = race;
			this._isVisualsDirty = true;
		}

		public void SetIdleAction(string idleAction)
		{
			this._idleAction = ActionIndexCache.Create(idleAction);
			this._isVisualsDirty = true;
		}

		public void SetCustomAnimation(string animation)
		{
			this._customAnimationName = animation;
		}

		public void StartCustomAnimation()
		{
			if (this._isVisualsDirty || this._agentVisuals == null || string.IsNullOrEmpty(this._customAnimationName))
			{
				this._customAnimationStartScheduled = true;
				return;
			}
			this.StopCustomAnimation();
			this._customAnimation = ActionIndexCache.Create(this._customAnimationName);
			if (this._customAnimation.Index >= 0)
			{
				this._agentVisuals.SetAction(this._customAnimation, 0f, true);
				this._customAnimationStartScheduled = false;
				this._customAnimationTimer = 0f;
				return;
			}
			Debug.FailedAssert("Invalid custom animation in character tableau: " + this._customAnimationName, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.View\\Tableaus\\CharacterTableau.cs", "StartCustomAnimation", 598);
		}

		public void StopCustomAnimation()
		{
			if (this._agentVisuals != null && this._customAnimation != null)
			{
				if (MBActionSet.GetActionAnimationContinueToAction(this._characterActionSet, ActionIndexValueCache.Create(this._customAnimation)).Index < 0)
				{
					this._agentVisuals.SetAction(this.GetIdleAction(), 0f, true);
				}
				this._customAnimation = null;
			}
		}

		public void SetIdleFaceAnim(string idleFaceAnim)
		{
			if (!string.IsNullOrEmpty(idleFaceAnim))
			{
				this._idleFaceAnim = idleFaceAnim;
				this._isVisualsDirty = true;
			}
		}

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

		public void SetIsEquipmentAnimActive(bool value)
		{
			this._isEquipmentAnimActive = value;
		}

		public void SetMountCreationKey(string value)
		{
			if (this._mountCreationKey != value)
			{
				this._mountCreationKey = value;
				this._isVisualsDirty = true;
			}
		}

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

		public void SetArmorColor1(uint clothColor1)
		{
			if (this._clothColor1 != clothColor1)
			{
				this._clothColor1 = clothColor1;
				this._isVisualsDirty = true;
			}
		}

		public void SetArmorColor2(uint clothColor2)
		{
			if (this._clothColor2 != clothColor2)
			{
				this._clothColor2 = clothColor2;
				this._isVisualsDirty = true;
			}
		}

		private ActionIndexCache GetIdleAction()
		{
			return this._idleAction ?? CharacterTableau.act_inventory_idle_start;
		}

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
				this._characterActionSet = MBGlobals.GetActionSetWithSuffix(copyAgentVisualsData.MonsterData, this._isFemale, "_warrior");
				copyAgentVisualsData.BodyProperties(this._bodyProperties).SkeletonType(this._isFemale ? 1 : 0).Frame(matrixFrame)
					.ActionSet(this._characterActionSet)
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

		public void RotateCharacter(bool value)
		{
			this._isRotatingCharacter = value;
		}

		public void TriggerCharacterMountPlacesSwap()
		{
			this._mainCharacterRotation = 0f;
			this._isCharacterMountPlacesSwapped = !this._isCharacterMountPlacesSwapped;
			this._isVisualsDirty = true;
		}

		public void OnCharacterTableauMouseMove(int mouseMoveX)
		{
			this.UpdateCharacterRotation(mouseMoveX);
		}

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

		private void InitializeAgentVisuals()
		{
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(this._race);
			this._characterActionSet = MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, this._isFemale, "_warrior");
			this._oldAgentVisuals = AgentVisuals.Create(new AgentVisualsData().Banner(this._banner).Equipment(this._equipment).BodyProperties(this._bodyProperties)
				.Race(this._race)
				.Frame(this._initialSpawnFrame)
				.UseMorphAnims(true)
				.ActionSet(this._characterActionSet)
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
				.ActionSet(this._characterActionSet)
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

		private bool _isFinalized;

		private MatrixFrame _mountSpawnPoint;

		private MatrixFrame _bannerSpawnPoint;

		private float _animationFrequencyThreshold = 2.5f;

		private MatrixFrame _initialSpawnFrame;

		private MatrixFrame _characterMountPositionFrame;

		private MatrixFrame _mountCharacterPositionFrame;

		private AgentVisuals _agentVisuals;

		private AgentVisuals _mountVisuals;

		private int _agentVisualLoadingCounter;

		private int _mountVisualLoadingCounter;

		private AgentVisuals _oldAgentVisuals;

		private AgentVisuals _oldMountVisuals;

		private int _initialLoadingCounter;

		private ActionIndexCache _idleAction;

		private string _idleFaceAnim;

		private Scene _tableauScene;

		private MBAgentRendererSceneController _agentRendererSceneController;

		private Camera _continuousRenderCamera;

		private float _cameraRatio;

		private MatrixFrame _camPos;

		private MatrixFrame _camPosGatheredFromScene;

		private string _charStringId;

		private int _tableauSizeX;

		private int _tableauSizeY;

		private uint _clothColor1 = new Color(1f, 1f, 1f, 1f).ToUnsignedInteger();

		private uint _clothColor2 = new Color(1f, 1f, 1f, 1f).ToUnsignedInteger();

		private bool _isRotatingCharacter;

		private bool _isCharacterMountPlacesSwapped;

		private string _mountCreationKey = "";

		private string _equipmentCode = "";

		private bool _isEquipmentAnimActive;

		private float _animationGap;

		private float _mainCharacterRotation;

		private bool _isEnabled;

		private float RenderScale = 1f;

		private float _customRenderScale = 1f;

		private int _latestWidth = -1;

		private int _latestHeight = -1;

		private string _bodyPropertiesCode;

		private BodyProperties _bodyProperties = BodyProperties.Default;

		private bool _isFemale;

		private CharacterViewModel.StanceTypes _stanceIndex;

		private Equipment _equipment;

		private Banner _banner;

		private int _race;

		private bool _isBannerShownInBackground;

		private ItemObject _bannerItem;

		private GameEntity _bannerEntity;

		private static readonly ActionIndexCache act_cheer_1 = ActionIndexCache.Create("act_arena_winner_1");

		private static readonly ActionIndexCache act_inventory_idle_start = ActionIndexCache.Create("act_inventory_idle_start");

		private static readonly ActionIndexCache act_inventory_glove_equip = ActionIndexCache.Create("act_inventory_glove_equip");

		private static readonly ActionIndexCache act_inventory_cloth_equip = ActionIndexCache.Create("act_inventory_cloth_equip");

		private static readonly ActionIndexCache act_horse_stand = ActionIndexCache.Create("act_inventory_idle_start");

		private static readonly ActionIndexCache act_camel_stand = ActionIndexCache.Create("act_inventory_idle_start");

		private int _leftHandEquipmentIndex;

		private int _rightHandEquipmentIndex;

		private bool _isEquipmentIndicesDirty;

		private bool _customAnimationStartScheduled;

		private float _customAnimationTimer;

		private string _customAnimationName;

		private ActionIndexCache _customAnimation;

		private MBActionSet _characterActionSet;

		private bool _isVisualsDirty;

		private Equipment _oldEquipment;
	}
}
