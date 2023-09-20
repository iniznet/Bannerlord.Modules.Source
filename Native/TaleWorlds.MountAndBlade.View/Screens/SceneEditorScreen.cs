using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ModuleManager;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	// Token: 0x02000032 RID: 50
	[GameStateScreen(typeof(EditorState))]
	public class SceneEditorScreen : ScreenBase, IGameStateListener
	{
		// Token: 0x06000255 RID: 597 RVA: 0x000162D9 File Offset: 0x000144D9
		public SceneEditorScreen(EditorState editorState)
		{
		}

		// Token: 0x06000256 RID: 598 RVA: 0x000162E4 File Offset: 0x000144E4
		protected override void OnInitialize()
		{
			base.OnInitialize();
			SceneEditorLayer sceneEditorLayer = new SceneEditorLayer("SceneEditorLayer");
			sceneEditorLayer.InputRestrictions.SetInputRestrictions(true, 0);
			base.AddLayer(sceneEditorLayer);
			ManagedParameters.Instance.Initialize(ModuleHelper.GetXmlPath("Native", "managed_core_parameters"));
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0001632F File Offset: 0x0001452F
		protected override void OnActivate()
		{
			base.OnActivate();
			MouseManager.ActivateMouseCursor(0);
			MBEditor.ActivateSceneEditorPresentation();
		}

		// Token: 0x06000258 RID: 600 RVA: 0x00016342 File Offset: 0x00014542
		protected override void OnDeactivate()
		{
			MBEditor.DeactivateSceneEditorPresentation();
			MouseManager.ActivateMouseCursor(1);
			base.OnDeactivate();
		}

		// Token: 0x06000259 RID: 601 RVA: 0x00016355 File Offset: 0x00014555
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			MBEditor.TickSceneEditorPresentation(dt);
		}

		// Token: 0x0600025A RID: 602 RVA: 0x00016364 File Offset: 0x00014564
		void IGameStateListener.OnActivate()
		{
		}

		// Token: 0x0600025B RID: 603 RVA: 0x00016366 File Offset: 0x00014566
		void IGameStateListener.OnDeactivate()
		{
		}

		// Token: 0x0600025C RID: 604 RVA: 0x00016368 File Offset: 0x00014568
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0001636A File Offset: 0x0001456A
		void IGameStateListener.OnFinalize()
		{
		}
	}
}
