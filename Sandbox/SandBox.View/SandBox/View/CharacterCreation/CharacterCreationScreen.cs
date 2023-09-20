using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace SandBox.View.CharacterCreation
{
	// Token: 0x0200005B RID: 91
	[GameStateScreen(typeof(CharacterCreationState))]
	public class CharacterCreationScreen : ScreenBase, ICharacterCreationStateHandler, IGameStateListener
	{
		// Token: 0x060003EF RID: 1007 RVA: 0x00022208 File Offset: 0x00020408
		public CharacterCreationScreen(CharacterCreationState characterCreationState)
		{
			this._characterCreationStateState = characterCreationState;
			characterCreationState.Handler = this;
			this._stageViews = new Dictionary<Type, Type>();
			Assembly[] viewAssemblies = this.GetViewAssemblies();
			foreach (Type type in this.CollectUnorderedStages(viewAssemblies))
			{
				CharacterCreationStageViewAttribute characterCreationStageViewAttribute;
				if (typeof(CharacterCreationStageViewBase).IsAssignableFrom(type) && (characterCreationStageViewAttribute = type.GetCustomAttributes(typeof(CharacterCreationStageViewAttribute), true).FirstOrDefault<object>() as CharacterCreationStageViewAttribute) != null)
				{
					if (this._stageViews.ContainsKey(characterCreationStageViewAttribute.StageType))
					{
						this._stageViews[characterCreationStageViewAttribute.StageType] = type;
					}
					else
					{
						this._stageViews.Add(characterCreationStageViewAttribute.StageType, type);
					}
				}
			}
			this._cultureAmbientSoundEvent = SoundEvent.CreateEventFromString("event:/mission/ambient/special/charactercreation", null);
			this._cultureAmbientSoundEvent.Play();
			this.CreateGenericScene();
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x00022304 File Offset: 0x00020504
		private void CreateGenericScene()
		{
			this._genericScene = Scene.CreateNewScene(true, false, 0, "mono_renderscene");
			SceneInitializationData sceneInitializationData = default(SceneInitializationData);
			sceneInitializationData.InitPhysicsWorld = false;
			this._genericScene.Read("character_menu_new", ref sceneInitializationData, "");
			this._agentRendererSceneController = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(this._genericScene, 32);
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x0002235E File Offset: 0x0002055E
		private void StopSound()
		{
			SoundManager.SetGlobalParameter("MissionCulture", 0f);
			SoundEvent cultureAmbientSoundEvent = this._cultureAmbientSoundEvent;
			if (cultureAmbientSoundEvent != null)
			{
				cultureAmbientSoundEvent.Stop();
			}
			this._cultureAmbientSoundEvent = null;
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x00022387 File Offset: 0x00020587
		void ICharacterCreationStateHandler.OnCharacterCreationFinalized()
		{
			LoadingWindow.EnableGlobalLoadingWindow();
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x00022390 File Offset: 0x00020590
		void ICharacterCreationStateHandler.OnRefresh()
		{
			if (this._shownLayers != null)
			{
				foreach (ScreenLayer screenLayer in this._shownLayers.ToArray<ScreenLayer>())
				{
					base.RemoveLayer(screenLayer);
				}
			}
			if (this._currentStageView != null)
			{
				this._shownLayers = this._currentStageView.GetLayers();
				if (this._shownLayers != null)
				{
					foreach (ScreenLayer screenLayer2 in this._shownLayers.ToArray<ScreenLayer>())
					{
						base.AddLayer(screenLayer2);
					}
				}
			}
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x00022410 File Offset: 0x00020610
		void ICharacterCreationStateHandler.OnStageCreated(CharacterCreationStageBase stage)
		{
			Type type;
			if (this._stageViews.TryGetValue(stage.GetType(), out type))
			{
				this._currentStageView = Activator.CreateInstance(type, new object[]
				{
					this._characterCreationStateState.CharacterCreation,
					new ControlCharacterCreationStage(this._characterCreationStateState.NextStage),
					new TextObject("{=Rvr1bcu8}Next", null),
					new ControlCharacterCreationStage(this._characterCreationStateState.PreviousStage),
					new TextObject("{=WXAaWZVf}Previous", null),
					new ControlCharacterCreationStage(this._characterCreationStateState.Refresh),
					new ControlCharacterCreationStageReturnInt(this._characterCreationStateState.GetIndexOfCurrentStage),
					new ControlCharacterCreationStageReturnInt(this._characterCreationStateState.GetTotalStagesCount),
					new ControlCharacterCreationStageReturnInt(this._characterCreationStateState.GetFurthestIndex),
					new ControlCharacterCreationStageWithInt(this._characterCreationStateState.GoToStage)
				}) as CharacterCreationStageViewBase;
				stage.Listener = this._currentStageView;
				this._currentStageView.SetGenericScene(this._genericScene);
				return;
			}
			this._currentStageView = null;
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x00022529 File Offset: 0x00020729
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (LoadingWindow.IsLoadingWindowActive)
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}
			CharacterCreationStageViewBase currentStageView = this._currentStageView;
			if (currentStageView == null)
			{
				return;
			}
			currentStageView.Tick(dt);
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x0002254F File Offset: 0x0002074F
		void IGameStateListener.OnActivate()
		{
			base.OnActivate();
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x00022557 File Offset: 0x00020757
		void IGameStateListener.OnDeactivate()
		{
			base.OnDeactivate();
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x0002255F File Offset: 0x0002075F
		void IGameStateListener.OnInitialize()
		{
			base.OnInitialize();
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x00022567 File Offset: 0x00020767
		void IGameStateListener.OnFinalize()
		{
			base.OnFinalize();
			this.StopSound();
			MBAgentRendererSceneController.DestructAgentRendererSceneController(this._genericScene, this._agentRendererSceneController, false);
			this._agentRendererSceneController = null;
			this._genericScene.ClearAll();
			this._genericScene = null;
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x000225A0 File Offset: 0x000207A0
		private IEnumerable<Type> CollectUnorderedStages(Assembly[] assemblies)
		{
			Assembly[] array = assemblies;
			for (int i = 0; i < array.Length; i++)
			{
				Type[] types = array[i].GetTypes();
				foreach (Type type in types)
				{
					if (typeof(CharacterCreationStageViewBase).IsAssignableFrom(type) && type.GetCustomAttributes(typeof(CharacterCreationStageViewAttribute), true).FirstOrDefault<object>() is CharacterCreationStageViewAttribute)
					{
						yield return type;
					}
				}
				Type[] array2 = null;
			}
			array = null;
			yield break;
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x000225B0 File Offset: 0x000207B0
		private Assembly[] GetViewAssemblies()
		{
			List<Assembly> list = new List<Assembly>();
			Assembly assembly = typeof(CharacterCreationStageViewAttribute).Assembly;
			foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
			{
				AssemblyName[] referencedAssemblies = assembly2.GetReferencedAssemblies();
				for (int j = 0; j < referencedAssemblies.Length; j++)
				{
					if (referencedAssemblies[j].ToString() == assembly.GetName().ToString())
					{
						list.Add(assembly2);
						break;
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x0400021A RID: 538
		private const string CultureParameterId = "MissionCulture";

		// Token: 0x0400021B RID: 539
		private readonly CharacterCreationState _characterCreationStateState;

		// Token: 0x0400021C RID: 540
		private IEnumerable<ScreenLayer> _shownLayers;

		// Token: 0x0400021D RID: 541
		private CharacterCreationStageViewBase _currentStageView;

		// Token: 0x0400021E RID: 542
		private readonly Dictionary<Type, Type> _stageViews;

		// Token: 0x0400021F RID: 543
		private SoundEvent _cultureAmbientSoundEvent;

		// Token: 0x04000220 RID: 544
		private Scene _genericScene;

		// Token: 0x04000221 RID: 545
		private MBAgentRendererSceneController _agentRendererSceneController;
	}
}
