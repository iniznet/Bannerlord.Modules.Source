﻿using System;
using TaleWorlds.Engine;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	public class SceneEditorLayer : ScreenLayer
	{
		public SceneEditorLayer(string categoryId = "SceneEditorLayer")
			: base(-100, categoryId)
		{
			base.Name = "SceneEditorLayer";
		}

		protected override void OnActivate()
		{
			base.OnActivate();
		}

		protected override void Tick(float dt)
		{
			base.Tick(dt);
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
		}

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
