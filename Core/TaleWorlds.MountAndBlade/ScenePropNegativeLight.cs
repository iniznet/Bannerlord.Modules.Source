using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000346 RID: 838
	public class ScenePropNegativeLight : ScriptComponentBehavior
	{
		// Token: 0x06002CA2 RID: 11426 RVA: 0x000AD19E File Offset: 0x000AB39E
		protected internal override void OnEditorTick(float dt)
		{
			this.SetMeshParameters();
		}

		// Token: 0x06002CA3 RID: 11427 RVA: 0x000AD1A8 File Offset: 0x000AB3A8
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

		// Token: 0x06002CA4 RID: 11428 RVA: 0x000AD226 File Offset: 0x000AB426
		protected internal override void OnInit()
		{
			base.OnInit();
			this.SetMeshParameters();
		}

		// Token: 0x06002CA5 RID: 11429 RVA: 0x000AD234 File Offset: 0x000AB434
		protected internal override bool IsOnlyVisual()
		{
			return true;
		}

		// Token: 0x04001111 RID: 4369
		public float Flatness_X;

		// Token: 0x04001112 RID: 4370
		public float Flatness_Y;

		// Token: 0x04001113 RID: 4371
		public float Flatness_Z;

		// Token: 0x04001114 RID: 4372
		public float Alpha = 1f;

		// Token: 0x04001115 RID: 4373
		public bool Is_Dark_Light = true;
	}
}
