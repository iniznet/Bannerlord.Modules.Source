using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ModuleManager;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	[GameStateScreen(typeof(EditorState))]
	public class SceneEditorScreen : ScreenBase, IGameStateListener
	{
		public SceneEditorScreen(EditorState editorState)
		{
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			SceneEditorLayer sceneEditorLayer = new SceneEditorLayer("SceneEditorLayer");
			sceneEditorLayer.InputRestrictions.SetInputRestrictions(true, 0);
			base.AddLayer(sceneEditorLayer);
			ManagedParameters.Instance.Initialize(ModuleHelper.GetXmlPath("Native", "managed_core_parameters"));
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			MouseManager.ActivateMouseCursor(0);
			MBEditor.ActivateSceneEditorPresentation();
		}

		protected override void OnDeactivate()
		{
			MBEditor.DeactivateSceneEditorPresentation();
			MouseManager.ActivateMouseCursor(1);
			base.OnDeactivate();
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			MBEditor.TickSceneEditorPresentation(dt);
		}

		void IGameStateListener.OnActivate()
		{
		}

		void IGameStateListener.OnDeactivate()
		{
		}

		void IGameStateListener.OnInitialize()
		{
		}

		void IGameStateListener.OnFinalize()
		{
		}
	}
}
