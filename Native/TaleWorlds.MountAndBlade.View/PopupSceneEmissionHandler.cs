using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View
{
	public class PopupSceneEmissionHandler : ScriptComponentBehavior
	{
		protected override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		protected override void OnEditorInit()
		{
			base.OnEditorInit();
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2;
		}

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

		protected override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.OnTick(dt);
		}

		public float startTime;

		public float transitionTime;

		private float timeElapsed;
	}
}
