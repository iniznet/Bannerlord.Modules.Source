using System;
using System.Collections.Generic;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200033D RID: 829
	public class LightCycle : ScriptComponentBehavior
	{
		// Token: 0x06002C64 RID: 11364 RVA: 0x000AC328 File Offset: 0x000AA528
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

		// Token: 0x06002C65 RID: 11365 RVA: 0x000AC3D8 File Offset: 0x000AA5D8
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

		// Token: 0x06002C66 RID: 11366 RVA: 0x000AC449 File Offset: 0x000AA649
		protected internal override void OnEditorTick(float dt)
		{
			this.SetVisibility();
		}

		// Token: 0x06002C67 RID: 11367 RVA: 0x000AC451 File Offset: 0x000AA651
		protected internal override bool MovesEntity()
		{
			return false;
		}

		// Token: 0x040010E7 RID: 4327
		public bool alwaysBurn;

		// Token: 0x040010E8 RID: 4328
		private bool visibility;
	}
}
