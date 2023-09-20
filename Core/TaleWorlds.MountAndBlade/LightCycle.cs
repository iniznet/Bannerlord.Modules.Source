using System;
using System.Collections.Generic;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class LightCycle : ScriptComponentBehavior
	{
		private void SetVisibility()
		{
			Light light = base.GameEntity.GetLight();
			float timeOfDay = base.Scene.TimeOfDay;
			this.visibility = timeOfDay < 6f || timeOfDay > 20f || base.Scene.IsAtmosphereIndoor || this.alwaysBurn;
			if (light != null)
			{
				light.SetVisibility(this.visibility);
			}
			foreach (GameEntity gameEntity in base.GameEntity.GetChildren())
			{
				gameEntity.SetVisibilityExcludeParents(this.visibility);
			}
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			this.SetVisibility();
			if (!this.visibility)
			{
				List<GameEntity> list = new List<GameEntity>();
				base.GameEntity.GetChildrenRecursive(ref list);
				for (int i = list.Count - 1; i >= 0; i--)
				{
					base.Scene.RemoveEntity(list[i], 0);
				}
				base.GameEntity.RemoveScriptComponent(base.ScriptComponent.Pointer, 0);
			}
		}

		protected internal override void OnEditorTick(float dt)
		{
			this.SetVisibility();
		}

		protected internal override bool MovesEntity()
		{
			return false;
		}

		public bool alwaysBurn;

		private bool visibility;
	}
}
