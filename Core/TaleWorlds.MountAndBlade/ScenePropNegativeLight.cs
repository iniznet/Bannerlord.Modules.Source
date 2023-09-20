using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class ScenePropNegativeLight : ScriptComponentBehavior
	{
		protected internal override void OnEditorTick(float dt)
		{
			this.SetMeshParameters();
		}

		private void SetMeshParameters()
		{
			MetaMesh metaMesh = base.GameEntity.GetMetaMesh(0);
			if (metaMesh != null)
			{
				metaMesh.SetVectorArgument(this.Flatness_X, this.Flatness_Y, this.Flatness_Z, this.Alpha);
				if (this.Is_Dark_Light)
				{
					metaMesh.SetVectorArgument2(1f, 0f, 0f, 0f);
					return;
				}
				metaMesh.SetVectorArgument2(0f, 0f, 0f, 0f);
			}
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			this.SetMeshParameters();
		}

		protected internal override bool IsOnlyVisual()
		{
			return true;
		}

		public float Flatness_X;

		public float Flatness_Y;

		public float Flatness_Z;

		public float Alpha = 1f;

		public bool Is_Dark_Light = true;
	}
}
