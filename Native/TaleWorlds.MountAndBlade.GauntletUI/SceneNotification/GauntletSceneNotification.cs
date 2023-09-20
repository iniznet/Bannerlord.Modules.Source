using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Scripts;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.SceneNotification
{
	// Token: 0x0200001C RID: 28
	public class GauntletSceneNotification : GlobalLayer
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000100 RID: 256 RVA: 0x0000635B File Offset: 0x0000455B
		// (set) Token: 0x06000101 RID: 257 RVA: 0x00006362 File Offset: 0x00004562
		public static GauntletSceneNotification Current { get; private set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000102 RID: 258 RVA: 0x0000636A File Offset: 0x0000456A
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00006374 File Offset: 0x00004574
		private GauntletSceneNotification()
		{
			this._dataSource = new SceneNotificationVM(new Action(this.OnPositiveAction), new Action(this.CloseNotification), new Func<string>(this.GetContinueKeyText));
			this._notificationQueue = new Queue<ValueTuple<SceneNotificationData, bool>>();
			this._contextProviders = new List<ISceneNotificationContextProvider>();
			this._gauntletLayer = new GauntletLayer(4600, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("SceneNotification", this._dataSource);
			base.Layer = this._gauntletLayer;
			MBInformationManager.OnShowSceneNotification += this.OnShowSceneNotification;
			MBInformationManager.OnHideSceneNotification += this.OnHideSceneNotification;
			MBInformationManager.IsAnySceneNotificationActive += this.IsAnySceneNotifiationActive;
			this._gauntletLayer._gauntletUIContext.EventManager.GainNavigationAfterFrames(2, null);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x0000644F File Offset: 0x0000464F
		private bool IsAnySceneNotifiationActive()
		{
			return this._isActive;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00006457 File Offset: 0x00004657
		public static void Initialize()
		{
			if (GauntletSceneNotification.Current == null)
			{
				GauntletSceneNotification.Current = new GauntletSceneNotification();
				ScreenManager.AddGlobalLayer(GauntletSceneNotification.Current, false);
				ScreenManager.SetSuspendLayer(GauntletSceneNotification.Current.Layer, true);
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00006485 File Offset: 0x00004685
		private void OnHideSceneNotification()
		{
			this.CloseNotification();
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000648D File Offset: 0x0000468D
		private void OnShowSceneNotification(SceneNotificationData campaignNotification)
		{
			this._notificationQueue.Enqueue(new ValueTuple<SceneNotificationData, bool>(campaignNotification, campaignNotification.PauseActiveState));
		}

		// Token: 0x06000108 RID: 264 RVA: 0x000064A8 File Offset: 0x000046A8
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._dataSource != null)
			{
				SceneNotificationVM dataSource = this._dataSource;
				PopupSceneCameraPath cameraPathScript = this._cameraPathScript;
				dataSource.EndProgress = ((cameraPathScript != null) ? cameraPathScript.GetCameraFade() : 0f);
				PopupSceneCameraPath cameraPathScript2 = this._cameraPathScript;
				if (cameraPathScript2 != null)
				{
					cameraPathScript2.SetIsReady(this._dataSource.IsReady);
				}
			}
			Scene scene = this._scene;
			if (scene == null)
			{
				return;
			}
			scene.Tick(dt);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00006512 File Offset: 0x00004712
		protected override void OnLateTick(float dt)
		{
			base.OnLateTick(dt);
			this.QueueTick();
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00006524 File Offset: 0x00004724
		private void QueueTick()
		{
			if (!this._isActive && this._notificationQueue.Count > 0)
			{
				SceneNotificationData.RelevantContextType relevantContext = this._notificationQueue.Peek().Item1.RelevantContext;
				if (this.IsGivenContextApplicableToCurrentContext(relevantContext))
				{
					ValueTuple<SceneNotificationData, bool> valueTuple = this._notificationQueue.Dequeue();
					this.CreateSceneNotification(valueTuple.Item1, valueTuple.Item2);
				}
			}
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00006584 File Offset: 0x00004784
		private void OnPositiveAction()
		{
			PopupSceneCameraPath cameraPathScript = this._cameraPathScript;
			if (cameraPathScript != null)
			{
				cameraPathScript.SetPositiveState();
			}
			foreach (PopupSceneSpawnPoint popupSceneSpawnPoint in this._sceneCharacterScripts)
			{
				popupSceneSpawnPoint.SetPositiveState();
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x000065E8 File Offset: 0x000047E8
		private void OpenScene()
		{
			this._scene = Scene.CreateNewScene(true, true, 0, "mono_renderscene");
			SceneInitializationData sceneInitializationData;
			sceneInitializationData..ctor(true);
			this._scene.Read(this._activeData.SceneID, ref sceneInitializationData, "");
			this._scene.SetClothSimulationState(true);
			this._scene.SetShadow(true);
			this._scene.SetDynamicShadowmapCascadesRadiusMultiplier(0.1f);
			this._agentRendererSceneController = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(this._scene, 32);
			this._agentRendererSceneController.SetEnforcedVisibilityForAllAgents(this._scene);
			this._sceneCharacterScripts = new List<PopupSceneSpawnPoint>();
			this._customPrefabBannerEntities = new Dictionary<string, GameEntity>();
			GameEntity firstEntityWithScriptComponent = this._scene.GetFirstEntityWithScriptComponent<PopupSceneCameraPath>();
			this._cameraPathScript = firstEntityWithScriptComponent.GetFirstScriptOfType<PopupSceneCameraPath>();
			PopupSceneCameraPath cameraPathScript = this._cameraPathScript;
			if (cameraPathScript != null)
			{
				cameraPathScript.Initialize();
			}
			PopupSceneCameraPath cameraPathScript2 = this._cameraPathScript;
			if (cameraPathScript2 != null)
			{
				cameraPathScript2.SetInitialState();
			}
			List<SceneNotificationData.SceneNotificationCharacter> list = this._activeData.GetSceneNotificationCharacters().ToList<SceneNotificationData.SceneNotificationCharacter>();
			List<Banner> list2 = this._activeData.GetBanners().ToList<Banner>();
			if (list != null)
			{
				int num = 1;
				for (int i = 0; i < list.Count; i++)
				{
					SceneNotificationData.SceneNotificationCharacter sceneNotificationCharacter = list[i];
					BasicCharacterObject character = sceneNotificationCharacter.Character;
					if (character == null)
					{
						num++;
					}
					else
					{
						string text = "spawnpoint_player_" + num.ToString();
						GameEntity gameEntity = this._scene.FindEntitiesWithTag(text).ToList<GameEntity>().FirstOrDefault<GameEntity>();
						if (gameEntity == null)
						{
							num++;
						}
						else
						{
							PopupSceneSpawnPoint firstScriptOfType = gameEntity.GetFirstScriptOfType<PopupSceneSpawnPoint>();
							MatrixFrame frame = gameEntity.GetFrame();
							Equipment equipment = character.GetFirstEquipment(false);
							if (sceneNotificationCharacter.OverriddenEquipment != null)
							{
								equipment = sceneNotificationCharacter.OverriddenEquipment;
							}
							else if (sceneNotificationCharacter.UseCivilianEquipment)
							{
								equipment = character.GetFirstEquipment(true);
							}
							BodyProperties bodyProperties = character.GetBodyProperties(character.Equipment, -1);
							if (sceneNotificationCharacter.OverriddenBodyProperties != default(BodyProperties))
							{
								bodyProperties = sceneNotificationCharacter.OverriddenBodyProperties;
							}
							uint num2 = character.Culture.Color;
							uint num3 = character.Culture.Color2;
							if (sceneNotificationCharacter.CustomColor1 != 4294967295U)
							{
								num2 = sceneNotificationCharacter.CustomColor1;
							}
							if (sceneNotificationCharacter.CustomColor2 != 4294967295U)
							{
								num3 = sceneNotificationCharacter.CustomColor2;
							}
							Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(character.Race);
							AgentVisuals agentVisuals = AgentVisuals.Create(new AgentVisualsData().UseMorphAnims(true).Equipment(equipment).Race(character.Race)
								.BodyProperties(bodyProperties)
								.SkeletonType(character.IsFemale ? 1 : 0)
								.Frame(frame)
								.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, character.IsFemale, "_facegen"))
								.Scene(this._scene)
								.Monster(baseMonsterFromRace)
								.PrepareImmediately(true)
								.UseTranslucency(true)
								.UseTesselation(true)
								.ClothColor1(num2)
								.ClothColor2(num3), "notification_agent_visuals_" + num, false, false, false);
							AgentVisuals agentVisuals2 = null;
							if (sceneNotificationCharacter.UseHorse)
							{
								ItemObject item = equipment[10].Item;
								string randomMountKeyString = MountCreationKey.GetRandomMountKeyString(item, character.GetMountKeySeed());
								MBActionSet actionSet = MBGlobals.GetActionSet(item.HorseComponent.Monster.ActionSetCode);
								agentVisuals2 = AgentVisuals.Create(new AgentVisualsData().Equipment(equipment).Frame(frame).ActionSet(actionSet)
									.Scene(this._scene)
									.Monster(item.HorseComponent.Monster)
									.Scale(item.ScaleFactor)
									.PrepareImmediately(true)
									.UseTranslucency(true)
									.UseTesselation(true)
									.MountCreationKey(randomMountKeyString), "notification_mount_visuals_" + num, false, false, false);
							}
							firstScriptOfType.InitializeWithAgentVisuals(agentVisuals, agentVisuals2);
							agentVisuals.SetAgentLodZeroOrMaxExternal(true);
							if (agentVisuals2 != null)
							{
								agentVisuals2.SetAgentLodZeroOrMaxExternal(true);
							}
							firstScriptOfType.SetInitialState();
							this._sceneCharacterScripts.Add(firstScriptOfType);
							if (!string.IsNullOrEmpty(firstScriptOfType.BannerTagToUseForAddedPrefab) && firstScriptOfType.AddedPrefabComponent != null)
							{
								this._customPrefabBannerEntities.Add(firstScriptOfType.BannerTagToUseForAddedPrefab, firstScriptOfType.AddedPrefabComponent.GetEntity());
							}
							num++;
						}
					}
				}
			}
			if (list2 != null)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					Banner banner = list2[j];
					string text2 = "banner_" + (j + 1).ToString();
					GameEntity bannerEntity = this._scene.FindEntityWithTag(text2);
					if (bannerEntity != null)
					{
						((BannerVisual)banner.BannerVisual).GetTableauTextureLarge(delegate(Texture t)
						{
							this.OnBannerTableauRenderDone(bannerEntity, t);
						}, true);
					}
					else
					{
						GameEntity entity;
						if (this._customPrefabBannerEntities.TryGetValue(text2, out entity))
						{
							((BannerVisual)banner.BannerVisual).GetTableauTextureLarge(delegate(Texture t)
							{
								this.OnBannerTableauRenderDone(entity, t);
							}, true);
						}
					}
				}
			}
			this._dataSource.Scene = this._scene;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00006B00 File Offset: 0x00004D00
		private void OnBannerTableauRenderDone(GameEntity bannerEntity, Texture bannerTexture)
		{
			if (bannerEntity != null)
			{
				foreach (Mesh mesh in bannerEntity.GetAllMeshesWithTag("banner_replacement_mesh"))
				{
					this.ApplyBannerTextureToMesh(mesh, bannerTexture);
				}
				Skeleton skeleton = bannerEntity.Skeleton;
				if (((skeleton != null) ? skeleton.GetAllMeshes() : null) != null)
				{
					Skeleton skeleton2 = bannerEntity.Skeleton;
					foreach (Mesh mesh2 in ((skeleton2 != null) ? skeleton2.GetAllMeshes() : null))
					{
						if (mesh2.HasTag("banner_replacement_mesh"))
						{
							this.ApplyBannerTextureToMesh(mesh2, bannerTexture);
						}
					}
				}
			}
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00006BCC File Offset: 0x00004DCC
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

		// Token: 0x0600010F RID: 271 RVA: 0x00006C24 File Offset: 0x00004E24
		private void CreateSceneNotification(SceneNotificationData data, bool pauseGameActiveState)
		{
			if (this._isActive)
			{
				return;
			}
			this._isActive = true;
			this._dataSource.CreateNotification(data);
			ScreenManager.SetSuspendLayer(base.Layer, false);
			base.Layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(base.Layer);
			base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
			this._isLastActiveGameStatePaused = pauseGameActiveState;
			if (this._isLastActiveGameStatePaused)
			{
				GameStateManager.Current.RegisterActiveStateDisableRequest(this);
				MBCommon.PauseGameEngine();
			}
			this._activeData = data;
			this._dataSource.EndProgress = 0f;
			this.OpenScene();
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00006CC0 File Offset: 0x00004EC0
		private void CloseNotification()
		{
			if (!this._isActive)
			{
				return;
			}
			this._dataSource.ForceClose();
			this._isActive = false;
			base.Layer.InputRestrictions.ResetInputRestrictions();
			ScreenManager.SetSuspendLayer(base.Layer, true);
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
			if (this._isLastActiveGameStatePaused)
			{
				GameStateManager.Current.UnregisterActiveStateDisableRequest(this);
				MBCommon.UnPauseGameEngine();
			}
			PopupSceneCameraPath cameraPathScript = this._cameraPathScript;
			if (cameraPathScript != null)
			{
				cameraPathScript.Destroy();
			}
			if (this._sceneCharacterScripts != null)
			{
				foreach (PopupSceneSpawnPoint popupSceneSpawnPoint in this._sceneCharacterScripts)
				{
					popupSceneSpawnPoint.Destroy();
				}
				this._sceneCharacterScripts = null;
			}
			MBAgentRendererSceneController.DestructAgentRendererSceneController(this._scene, this._agentRendererSceneController, false);
			this._activeData = null;
			this._scene = null;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00006DB8 File Offset: 0x00004FB8
		private string GetContinueKeyText()
		{
			if (Input.IsGamepadActive)
			{
				TextObject textObject = Module.CurrentModule.GlobalTextManager.FindText("str_click_to_continue_console", null);
				textObject.SetTextVariable("CONSOLE_KEY_NAME", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("ConversationHotKeyCategory", "ContinueKey")));
				return textObject.ToString();
			}
			return Module.CurrentModule.GlobalTextManager.FindText("str_click_to_continue", null).ToString();
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00006E21 File Offset: 0x00005021
		public void OnFinalize()
		{
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00006E35 File Offset: 0x00005035
		public void RegisterContextProvider(ISceneNotificationContextProvider provider)
		{
			this._contextProviders.Add(provider);
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00006E43 File Offset: 0x00005043
		public bool RemoveContextProvider(ISceneNotificationContextProvider provider)
		{
			return this._contextProviders.Remove(provider);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00006E54 File Offset: 0x00005054
		private bool IsGivenContextApplicableToCurrentContext(SceneNotificationData.RelevantContextType givenContextType)
		{
			if (LoadingWindow.IsLoadingWindowActive)
			{
				return false;
			}
			if (givenContextType == null)
			{
				return true;
			}
			for (int i = 0; i < this._contextProviders.Count; i++)
			{
				ISceneNotificationContextProvider sceneNotificationContextProvider = this._contextProviders[i];
				if (sceneNotificationContextProvider != null && !sceneNotificationContextProvider.IsContextAllowed(givenContextType))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04000095 RID: 149
		private readonly GauntletLayer _gauntletLayer;

		// Token: 0x04000096 RID: 150
		private readonly Queue<ValueTuple<SceneNotificationData, bool>> _notificationQueue;

		// Token: 0x04000097 RID: 151
		private readonly List<ISceneNotificationContextProvider> _contextProviders;

		// Token: 0x04000098 RID: 152
		private SceneNotificationVM _dataSource;

		// Token: 0x04000099 RID: 153
		private SceneNotificationData _activeData;

		// Token: 0x0400009A RID: 154
		private bool _isActive;

		// Token: 0x0400009B RID: 155
		private bool _isLastActiveGameStatePaused;

		// Token: 0x0400009C RID: 156
		private Scene _scene;

		// Token: 0x0400009D RID: 157
		private MBAgentRendererSceneController _agentRendererSceneController;

		// Token: 0x0400009E RID: 158
		private List<PopupSceneSpawnPoint> _sceneCharacterScripts;

		// Token: 0x0400009F RID: 159
		private PopupSceneCameraPath _cameraPathScript;

		// Token: 0x040000A0 RID: 160
		private Dictionary<string, GameEntity> _customPrefabBannerEntities;
	}
}
