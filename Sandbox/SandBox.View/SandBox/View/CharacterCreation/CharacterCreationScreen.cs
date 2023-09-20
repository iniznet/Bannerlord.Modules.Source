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
	[GameStateScreen(typeof(CharacterCreationState))]
	public class CharacterCreationScreen : ScreenBase, ICharacterCreationStateHandler, IGameStateListener
	{
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

		private void CreateGenericScene()
		{
			this._genericScene = Scene.CreateNewScene(true, false, 0, "mono_renderscene");
			SceneInitializationData sceneInitializationData = default(SceneInitializationData);
			sceneInitializationData.InitPhysicsWorld = false;
			this._genericScene.Read("character_menu_new", ref sceneInitializationData, "");
			this._agentRendererSceneController = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(this._genericScene, 32);
		}

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

		void ICharacterCreationStateHandler.OnCharacterCreationFinalized()
		{
			LoadingWindow.EnableGlobalLoadingWindow();
		}

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

		void IGameStateListener.OnActivate()
		{
			base.OnActivate();
		}

		void IGameStateListener.OnDeactivate()
		{
			base.OnDeactivate();
		}

		void IGameStateListener.OnInitialize()
		{
			base.OnInitialize();
		}

		void IGameStateListener.OnFinalize()
		{
			base.OnFinalize();
			this.StopSound();
			MBAgentRendererSceneController.DestructAgentRendererSceneController(this._genericScene, this._agentRendererSceneController, false);
			this._agentRendererSceneController = null;
			this._genericScene.ClearAll();
			this._genericScene = null;
		}

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

		private const string CultureParameterId = "MissionCulture";

		private readonly CharacterCreationState _characterCreationStateState;

		private IEnumerable<ScreenLayer> _shownLayers;

		private CharacterCreationStageViewBase _currentStageView;

		private readonly Dictionary<Type, Type> _stageViews;

		private SoundEvent _cultureAmbientSoundEvent;

		private Scene _genericScene;

		private MBAgentRendererSceneController _agentRendererSceneController;
	}
}
