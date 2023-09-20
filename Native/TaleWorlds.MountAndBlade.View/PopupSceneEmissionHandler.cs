using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x02000014 RID: 20
	public class PopupSceneEmissionHandler : ScriptComponentBehavior
	{
		// Token: 0x06000087 RID: 135 RVA: 0x00006117 File Offset: 0x00004317
		protected override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000612B File Offset: 0x0000432B
		protected override void OnEditorInit()
		{
			base.OnEditorInit();
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00006133 File Offset: 0x00004333
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00006138 File Offset: 0x00004338
		protected override void OnTick(float dt)
		{
			this.timeElapsed += dt;
			foreach (GameEntity gameEntity in base.GameEntity.GetChildren())
			{
				Mesh firstMesh = gameEntity.GetFirstMesh();
				if (firstMesh != null)
				{
					firstMesh.SetVectorArgument(1f, 0.5f, 1f, MBMath.SmoothStep(this.startTime, this.startTime + this.transitionTime, this.timeElapsed) * 10f);
				}
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000061D8 File Offset: 0x000043D8
		protected override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.OnTick(dt);
		}

		// Token: 0x0400001B RID: 27
		public float startTime;

		// Token: 0x0400001C RID: 28
		public float transitionTime;

		// Token: 0x0400001D RID: 29
		private float timeElapsed;
	}
}
