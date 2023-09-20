using System;
using TaleWorlds.Engine;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	// Token: 0x02000033 RID: 51
	public class SceneEditorLayer : ScreenLayer
	{
		// Token: 0x0600025E RID: 606 RVA: 0x0001636C File Offset: 0x0001456C
		public SceneEditorLayer(string categoryId = "SceneEditorLayer")
			: base(-100, categoryId)
		{
			base.Name = "SceneEditorLayer";
		}

		// Token: 0x0600025F RID: 607 RVA: 0x00016382 File Offset: 0x00014582
		protected override void OnActivate()
		{
			base.OnActivate();
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0001638A File Offset: 0x0001458A
		protected override void Tick(float dt)
		{
			base.Tick(dt);
		}

		// Token: 0x06000261 RID: 609 RVA: 0x00016393 File Offset: 0x00014593
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0001639C File Offset: 0x0001459C
		protected override void RefreshGlobalOrder(ref int currentOrder)
		{
			SceneView editorSceneView = MBEditor.GetEditorSceneView();
			if (editorSceneView != null)
			{
				editorSceneView.SetRenderOrder(currentOrder);
				currentOrder++;
			}
		}
	}
}
