using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000347 RID: 839
	public class ScenePropPositiveLight : ScriptComponentBehavior
	{
		// Token: 0x06002CA6 RID: 11430 RVA: 0x000AD237 File Offset: 0x000AB437
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.SetMeshParams();
		}

		// Token: 0x06002CA7 RID: 11431 RVA: 0x000AD246 File Offset: 0x000AB446
		protected internal override void OnInit()
		{
			base.OnInit();
			this.SetMeshParams();
		}

		// Token: 0x06002CA8 RID: 11432 RVA: 0x000AD254 File Offset: 0x000AB454
		private void SetMeshParams()
		{
			MetaMesh metaMesh = base.GameEntity.GetMetaMesh(0);
			if (metaMesh != null)
			{
				uint num = this.CalculateFactor(new Vec3(this.DirectLightRed, this.DirectLightGreen, this.DirectLightBlue, -1f), this.DirectLightIntensity);
				metaMesh.SetFactor1Linear(num);
				uint num2 = this.CalculateFactor(new Vec3(this.AmbientLightRed, this.AmbientLightGreen, this.AmbientLightBlue, -1f), this.AmbientLightIntensity);
				metaMesh.SetFactor2Linear(num2);
				metaMesh.SetVectorArgument(this.Flatness_X, this.Flatness_Y, this.Flatness_Z, 1f);
			}
		}

		// Token: 0x06002CA9 RID: 11433 RVA: 0x000AD2F4 File Offset: 0x000AB4F4
		private uint CalculateFactor(Vec3 color, float alpha)
		{
			float num = 10f;
			byte maxValue = byte.MaxValue;
			alpha = MathF.Min(MathF.Max(0f, alpha), num);
			return ((uint)(alpha / num * (float)maxValue) << 24) + ((uint)(color.x * (float)maxValue) << 16) + ((uint)(color.y * (float)maxValue) << 8) + (uint)(color.z * (float)maxValue);
		}

		// Token: 0x06002CAA RID: 11434 RVA: 0x000AD350 File Offset: 0x000AB550
		protected internal override bool IsOnlyVisual()
		{
			return true;
		}

		// Token: 0x04001116 RID: 4374
		public float Flatness_X;

		// Token: 0x04001117 RID: 4375
		public float Flatness_Y;

		// Token: 0x04001118 RID: 4376
		public float Flatness_Z;

		// Token: 0x04001119 RID: 4377
		public float DirectLightRed = 1f;

		// Token: 0x0400111A RID: 4378
		public float DirectLightGreen = 1f;

		// Token: 0x0400111B RID: 4379
		public float DirectLightBlue = 1f;

		// Token: 0x0400111C RID: 4380
		public float DirectLightIntensity = 1f;

		// Token: 0x0400111D RID: 4381
		public float AmbientLightRed;

		// Token: 0x0400111E RID: 4382
		public float AmbientLightGreen;

		// Token: 0x0400111F RID: 4383
		public float AmbientLightBlue = 1f;

		// Token: 0x04001120 RID: 4384
		public float AmbientLightIntensity = 1f;
	}
}
