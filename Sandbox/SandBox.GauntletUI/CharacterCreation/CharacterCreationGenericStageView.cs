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
	[CharacterCreationStageView(typeof(CharacterCreationGenericStage))]
	public class CharacterCreationGenericStageView : CharacterCreationStageViewBase
	{
		public SceneLayer CharacterLayer { get; private set; }

		public CharacterCreationGenericStageView(CharacterCreation characterCreation, ControlCharacterCreationStage affirmativeAction, TextObject affirmativeActionText, ControlCharacterCreationStage negativeAction, TextObject negativeActionText, ControlCharacterCreationStage onRefresh, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction, ControlCharacterCreationStageReturnInt getTotalStageCountAction, ControlCharacterCreationStageReturnInt getFurthestIndexAction, ControlCharacterCreationStageWithInt goToIndexAction)
			: base(affirmativeAction, negativeAction, onRefresh, getCurrentStageIndexAction, getTotalStageCountAction, getFurthestIndexAction, goToIndexAction)
		{
			this._characterCreation = characterCreation;
			this._affirmativeActionText = affirmativeActionText;
			this._negativeActionText = negativeActionText;
			this.GauntletLayer = new GauntletLayer(1, "GauntletLayer", false);
			this.GauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this.GauntletLayer.IsFocusLayer = true;
			this.GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			ScreenManager.TrySetFocus(this.GauntletLayer);
			this._dataSource = new CharacterCreationGenericStageVM(this._characterCreation, new Action(this.NextStage), this._affirmativeActionText, new Action(this.PreviousStage), this._negativeActionText, this._stageIndex, getCurrentStageIndexAction.Invoke(), getTotalStageCountAction.Invoke(), getFurthestIndexAction.Invoke(), new Action<int>(this.GoToIndex))
			{
				OnOptionSelection = new Action(this.OnSelectionChanged)
			};
			this.CreateHotKeyVisuals();
			this._movie = this.GauntletLayer.LoadMovie("CharacterCreationGenericStage", this._dataSource);
		}

		public override void SetGenericScene(Scene scene)
		{
			this.OpenScene(scene);
			this.RefreshCharacterEntity();
			this.RefreshMountEntity();
		}

		private void CreateHotKeyVisuals()
		{
			CharacterCreationGenericStageVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			}
			CharacterCreationGenericStageVM dataSource2 = this._dataSource;
			if (dataSource2 == null)
			{
				return;
			}
			dataSource2.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		}

		private void OpenScene(Scene cachedScene)
		{
			this._characterScene = cachedScene;
			this._characterScene.SetShadow(true);
			this._characterScene.SetDynamicShadowmapCascadesRadiusMultiplier(0.1f);
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
			if (!this.CharacterLayer.Input.IsCategoryRegistered(HotKeyManager.GetCategory("FaceGenHotkeyCategory")))
			{
				this.CharacterLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("FaceGenHotkeyCategory"));
			}
			int num = -1;
			num &= -5;
			this.CharacterLayer.SetPostfxConfigParams(num);
			this.CharacterLayer.SetPostfxFromConfig();
			GameEntity gameEntity = this._characterScene.FindEntityWithName("cradle");
			if (gameEntity == null)
			{
				return;
			}
			gameEntity.SetVisibilityExcludeParents(false);
		}

		private void RefreshCharacterEntity()
		{
			List<float> list = new List<float>();
			bool isPlayerAlone = this._characterCreation.IsPlayerAlone;
			bool hasSecondaryCharacter = this._characterCreation.HasSecondaryCharacter;
			if (this._playerOrParentAgentVisuals != null && this._characterCreation.FaceGenChars.Count == 1)
			{
				foreach (AgentVisuals agentVisuals in this._playerOrParentAgentVisuals)
				{
					Skeleton skeleton = agentVisuals.GetVisuals().GetSkeleton();
					list.Add(skeleton.GetAnimationParameterAtChannel(0));
				}
			}
			if (this._playerOrParentAgentVisualsPrevious != null)
			{
				foreach (AgentVisuals agentVisuals2 in this._playerOrParentAgentVisualsPrevious)
				{
					agentVisuals2.Reset();
				}
			}
			this._playerOrParentAgentVisualsPrevious = new List<AgentVisuals>();
			if (this._playerOrParentAgentVisuals != null)
			{
				foreach (AgentVisuals agentVisuals3 in this._playerOrParentAgentVisuals)
				{
					this._playerOrParentAgentVisualsPrevious.Add(agentVisuals3);
				}
			}
			this._checkForVisualVisibility = 1;
			if (this._characterCreation.FaceGenChars.Count > 0)
			{
				this._playerOrParentAgentVisuals = new List<AgentVisuals>();
				int num = this._characterCreation.FaceGenChars.Count;
				int num2 = 0;
				using (List<FaceGenChar>.Enumerator enumerator2 = this._characterCreation.FaceGenChars.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						FaceGenChar faceGenChar = enumerator2.Current;
						string text = (isPlayerAlone ? "spawnpoint_player_1" : "spawnpoint_player_3");
						if (hasSecondaryCharacter)
						{
							if (this._characterCreation.FaceGenChars.ElementAt(num2).ActionName.ToString().Contains("horse"))
							{
								text = "spawnpoint_mount_1";
							}
							else if (num2 == 0)
							{
								text = "spawnpoint_player_brother_stage";
							}
							else if (num2 == 1)
							{
								text = "spawnpoint_brother_brother_stage";
							}
						}
						GameEntity gameEntity = this._characterScene.FindEntityWithTag(text);
						this._initialCharacterFrame = gameEntity.GetFrame();
						this._initialCharacterFrame.origin.z = 0f;
						AgentVisuals agentVisuals4 = AgentVisuals.Create(this.CreateAgentVisual(faceGenChar, this._initialCharacterFrame, isPlayerAlone, (GameStateManager.Current.ActiveState as CharacterCreationState).CurrentCharacterCreationContent.GetSelectedParentType(), num2 == 2), "facegenvisual" + num.ToString(), false, false, false);
						agentVisuals4.SetVisible(false);
						this._playerOrParentAgentVisuals.Add(agentVisuals4);
						this._playerOrParentAgentVisuals[num2].GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(0.001f, this._initialCharacterFrame, true);
						if (isPlayerAlone || hasSecondaryCharacter)
						{
							MBReadOnlyList<FaceGenChar> faceGenChars = this._characterCreation.FaceGenChars;
							ActionIndexCache actionIndexCache = ActionIndexCache.Create((faceGenChars != null) ? faceGenChars.ElementAt(num2).ActionName : null);
							MBSkeletonExtensions.SetAgentActionChannel(this._playerOrParentAgentVisuals[num2].GetVisuals().GetSkeleton(), 0, actionIndexCache, 0f, -0.2f, true);
						}
						if (num2 == 0 && !string.IsNullOrEmpty(this._characterCreation.PrefabId) && GameEntity.Instantiate(this._characterScene, this._characterCreation.PrefabId, true) != null)
						{
							this._playerOrParentAgentVisuals[num2].AddPrefabToAgentVisualBoneByRealBoneIndex(this._characterCreation.PrefabId, this._characterCreation.PrefabBoneUsage);
						}
						this._playerOrParentAgentVisuals[num2].SetAgentLodZeroOrMax(true);
						this._playerOrParentAgentVisuals[num2].GetEntity().SetEnforcedMaximumLodLevel(0);
						this._playerOrParentAgentVisuals[num2].GetEntity().CheckResources(true, true);
						this.CharacterLayer.SetFocusedShadowmap(true, ref this._initialCharacterFrame.origin, 0.59999996f);
						num++;
						num2++;
					}
					return;
				}
			}
			this._playerOrParentAgentVisuals = null;
		}

		private void RefreshMountEntity()
		{
			this.RemoveShownMount();
			if (this._characterCreation.FaceGenMount != null)
			{
				GameEntity gameEntity = this._characterScene.FindEntityWithTag("spawnpoint_mount_1");
				HorseComponent horseComponent = this._characterCreation.FaceGenMount.HorseItem.HorseComponent;
				Monster monster = horseComponent.Monster;
				this._mountEntityToPrepare = GameEntity.CreateEmpty(this._characterScene, true);
				AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(monster, MBGlobals.GetActionSet(horseComponent.Monster.ActionSetCode), 1f, false);
				GameEntityExtensions.CreateSkeletonWithActionSet(this._mountEntityToPrepare, ref animationSystemData);
				ActionIndexCache actionIndexCache = ActionIndexCache.Create(this._characterCreation.FaceGenMount.ActionName);
				MBSkeletonExtensions.SetAgentActionChannel(this._mountEntityToPrepare.Skeleton, 0, actionIndexCache, 0f, -0.2f, true);
				this._mountEntityToPrepare.EntityFlags |= 256;
				MountVisualCreator.AddMountMeshToEntity(this._mountEntityToPrepare, this._characterCreation.FaceGenMount.HorseItem, this._characterCreation.FaceGenMount.HarnessItem, this._characterCreation.FaceGenMount.MountKey.ToString(), null);
				MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
				this._mountEntityToPrepare.SetFrame(ref globalFrame);
				this._mountEntityToPrepare.SetVisibilityExcludeParents(false);
				this._mountEntityToPrepare.SetEnforcedMaximumLodLevel(0);
				this._mountEntityToPrepare.CheckResources(true, false);
			}
		}

		private void RemoveShownMount()
		{
			if (this._mountEntityToShow != null)
			{
				this._mountEntityToShow.Remove(116);
			}
			this._mountEntityToShow = this._mountEntityToPrepare;
			this._mountEntityToPrepare = null;
		}

		private AgentVisualsData CreateAgentVisual(FaceGenChar character, MatrixFrame characterFrame, bool isPlayerEntity, int selectedParentType = 0, bool isChildAgent = false)
		{
			ActionIndexCache actionIndexCache = (isChildAgent ? ActionIndexCache.Create("act_character_creation_toddler_" + selectedParentType) : ActionIndexCache.Create(character.IsFemale ? ("act_character_creation_female_default_" + selectedParentType) : ("act_character_creation_male_default_" + selectedParentType)));
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(character.Race);
			AgentVisualsData agentVisualsData = new AgentVisualsData().UseMorphAnims(true).Equipment(character.Equipment).BodyProperties(character.BodyProperties)
				.Frame(characterFrame)
				.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, character.IsFemale, "_facegen"))
				.ActionCode(actionIndexCache)
				.Scene(this._characterScene)
				.Monster(baseMonsterFromRace)
				.UseTranslucency(true)
				.UseTesselation(true)
				.RightWieldedItemIndex(0)
				.LeftWieldedItemIndex(1)
				.Race(CharacterObject.PlayerCharacter.Race)
				.SkeletonType(character.IsFemale ? 1 : 0);
			CharacterCreationContentBase currentCharacterCreationContent = ((CharacterCreationState)GameStateManager.Current.ActiveState).CurrentCharacterCreationContent;
			if (currentCharacterCreationContent.GetSelectedCulture() != null)
			{
				agentVisualsData.ClothColor1(currentCharacterCreationContent.GetSelectedCulture().Color);
				agentVisualsData.ClothColor2(currentCharacterCreationContent.GetSelectedCulture().Color2);
			}
			if (!isPlayerEntity && !isChildAgent)
			{
				agentVisualsData.Scale(character.IsFemale ? 0.99f : 1f);
			}
			if (!isPlayerEntity && isChildAgent)
			{
				agentVisualsData.Scale(0.5f);
			}
			return agentVisualsData;
		}

		private void OnSelectionChanged()
		{
			this.RefreshCharacterEntity();
			this.RefreshMountEntity();
		}

		public override void Tick(float dt)
		{
			base.Tick(dt);
			base.HandleEscapeMenu(this, this.CharacterLayer);
			Scene characterScene = this._characterScene;
			if (characterScene != null)
			{
				characterScene.Tick(dt);
			}
			foreach (AgentVisuals agentVisuals in this._playerOrParentAgentVisuals)
			{
				agentVisuals.TickVisuals();
			}
			if (this._playerOrParentAgentVisuals != null && this._checkForVisualVisibility > 0)
			{
				bool flag = true;
				using (List<AgentVisuals>.Enumerator enumerator = this._playerOrParentAgentVisuals.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.GetEntity().CheckResources(false, true))
						{
							flag = false;
						}
					}
				}
				if (this._mountEntityToPrepare != null && !this._mountEntityToPrepare.CheckResources(false, true))
				{
					flag = false;
				}
				if (flag)
				{
					this._checkForVisualVisibility--;
					if (this._checkForVisualVisibility == 0)
					{
						foreach (AgentVisuals agentVisuals2 in this._playerOrParentAgentVisuals)
						{
							agentVisuals2.SetVisible(true);
						}
						foreach (AgentVisuals agentVisuals3 in this._playerOrParentAgentVisualsPrevious)
						{
							agentVisuals3.SetVisible(false);
							agentVisuals3.Reset();
						}
						if (this._mountEntityToPrepare != null)
						{
							this._mountEntityToPrepare.SetVisibilityExcludeParents(true);
						}
						if (this._mountEntityToShow != null)
						{
							this._mountEntityToShow.SetVisibilityExcludeParents(false);
							this._characterScene.RemoveEntity(this._mountEntityToShow, 116);
						}
						this._mountEntityToShow = this._mountEntityToPrepare;
						this._mountEntityToPrepare = null;
						this._playerOrParentAgentVisualsPrevious.Clear();
					}
				}
			}
			if (this.CharacterLayer.Input.IsHotKeyReleased("Ascend") || this.CharacterLayer.Input.IsHotKeyReleased("Rotate") || this.CharacterLayer.Input.IsHotKeyReleased("Zoom"))
			{
				this.GauntletLayer.InputRestrictions.SetMouseVisibility(true);
			}
			this.HandleLayerInput();
		}

		private void HandleLayerInput()
		{
			if (this.GauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				UISoundsHelper.PlayUISound("event:/ui/panels/next");
				this._dataSource.OnPreviousStage();
				return;
			}
			if (this.GauntletLayer.Input.IsHotKeyReleased("Confirm") && this._dataSource.CanAdvance)
			{
				UISoundsHelper.PlayUISound("event:/ui/panels/next");
				this._dataSource.OnNextStage();
			}
		}

		public override void NextStage()
		{
			this._stageIndex++;
			if (this._stageIndex < this._characterCreation.CharacterCreationMenuCount)
			{
				if (this._movie != null)
				{
					this.GauntletLayer.ReleaseMovie(this._movie);
					this._movie = null;
				}
				if (this._dataSource != null)
				{
					this._dataSource.OnOptionSelection = null;
				}
				this._dataSource = new CharacterCreationGenericStageVM(this._characterCreation, new Action(this.NextStage), this._affirmativeActionText, new Action(this.PreviousStage), this._negativeActionText, this._stageIndex, this._getCurrentStageIndexAction.Invoke(), this._getTotalStageCountAction.Invoke(), this._getFurthestIndexAction.Invoke(), new Action<int>(this.GoToIndex))
				{
					OnOptionSelection = new Action(this.OnSelectionChanged)
				};
				this.CreateHotKeyVisuals();
				this._movie = this.GauntletLayer.LoadMovie("CharacterCreationGenericStage", this._dataSource);
				this.RefreshCharacterEntity();
				this.RefreshMountEntity();
				return;
			}
			this.RefreshMountEntity();
			this._affirmativeAction.Invoke();
		}

		public override void PreviousStage()
		{
			this._stageIndex--;
			if (this._stageIndex >= 0)
			{
				if (this._movie != null)
				{
					this.GauntletLayer.ReleaseMovie(this._movie);
					this._movie = null;
				}
				if (this._dataSource != null)
				{
					this._dataSource.OnOptionSelection = null;
				}
				this._dataSource = new CharacterCreationGenericStageVM(this._characterCreation, new Action(this.NextStage), this._affirmativeActionText, new Action(this.PreviousStage), this._negativeActionText, this._stageIndex, this._getCurrentStageIndexAction.Invoke(), this._getTotalStageCountAction.Invoke(), this._getFurthestIndexAction.Invoke(), new Action<int>(this.GoToIndex))
				{
					OnOptionSelection = new Action(this.OnSelectionChanged)
				};
				this.CreateHotKeyVisuals();
				this._movie = this.GauntletLayer.LoadMovie("CharacterCreationGenericStage", this._dataSource);
				this.RefreshCharacterEntity();
				this.RefreshMountEntity();
				return;
			}
			this.RefreshMountEntity();
			this._negativeAction.Invoke();
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			if (this._playerOrParentAgentVisuals != null)
			{
				foreach (AgentVisuals agentVisuals in this._playerOrParentAgentVisuals)
				{
					agentVisuals.Reset();
				}
			}
			if (this._playerOrParentAgentVisualsPrevious != null)
			{
				foreach (AgentVisuals agentVisuals2 in this._playerOrParentAgentVisualsPrevious)
				{
					agentVisuals2.Reset();
				}
			}
			this.CharacterLayer.SceneView.SetEnable(false);
			this.CharacterLayer.SceneView.ClearAll(false, false);
			this._playerOrParentAgentVisuals = null;
			this._playerOrParentAgentVisualsPrevious = null;
			this.GauntletLayer = null;
			CharacterCreationGenericStageVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._dataSource = null;
			this.CharacterLayer = null;
			this._characterScene = null;
		}

		public override int GetVirtualStageCount()
		{
			return this._characterCreation.CharacterCreationMenuCount;
		}

		public override IEnumerable<ScreenLayer> GetLayers()
		{
			return new List<ScreenLayer> { this.CharacterLayer, this.GauntletLayer };
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

		protected readonly TextObject _affirmativeActionText;

		protected readonly TextObject _negativeActionText;

		private IGauntletMovie _movie;

		private GauntletLayer GauntletLayer;

		private CharacterCreationGenericStageVM _dataSource;

		private int _stageIndex;

		private readonly ActionIndexCache act_inventory_idle_start = ActionIndexCache.Create("act_inventory_idle_start");

		private readonly ActionIndexCache act_horse_stand_1 = ActionIndexCache.Create("act_horse_stand_1");

		private readonly CharacterCreation _characterCreation;

		private Scene _characterScene;

		private Camera _camera;

		private MatrixFrame _initialCharacterFrame;

		private List<AgentVisuals> _playerOrParentAgentVisuals;

		private List<AgentVisuals> _playerOrParentAgentVisualsPrevious;

		private int _checkForVisualVisibility;

		private GameEntity _mountEntityToPrepare;

		private GameEntity _mountEntityToShow;

		private EscapeMenuVM _escapeMenuDatasource;

		private IGauntletMovie _escapeMenuMovie;
	}
}
