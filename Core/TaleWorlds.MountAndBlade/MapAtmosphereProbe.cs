using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200033E RID: 830
	public class MapAtmosphereProbe : ScriptComponentBehavior
	{
		// Token: 0x06002C69 RID: 11369 RVA: 0x000AC45C File Offset: 0x000AA65C
		public float GetInfluenceAmount(Vec3 worldPosition)
		{
			return MBMath.SmoothStep(this.minRadius, this.maxRadius, worldPosition.Distance(base.GameEntity.GetGlobalFrame().origin));
		}

		// Token: 0x06002C6A RID: 11370 RVA: 0x000AC488 File Offset: 0x000AA688
		public MapAtmosphereProbe()
		{
			this.hideAllProbes = MapAtmosphereProbe.hideAllProbesStatic;
			if (MBEditor.IsEditModeOn)
			{
				this.innerSphereMesh = MetaMesh.GetCopy("physics_sphere_detailed", true, false);
				this.outerSphereMesh = MetaMesh.GetCopy("physics_sphere_detailed", true, false);
				this.innerSphereMesh.SetMaterial(Material.GetFromResource("light_radius_visualizer"));
				this.outerSphereMesh.SetMaterial(Material.GetFromResource("light_radius_visualizer"));
			}
		}

		// Token: 0x06002C6B RID: 11371 RVA: 0x000AC520 File Offset: 0x000AA720
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			if (this.visualizeRadius && !MapAtmosphereProbe.hideAllProbesStatic)
			{
				uint num = 16711680U;
				uint num2 = 720640U;
				if (MBEditor.IsEntitySelected(base.GameEntity))
				{
					num |= 2147483648U;
					num2 |= 2147483648U;
				}
				else
				{
					num |= 1073741824U;
					num2 |= 1073741824U;
				}
				this.innerSphereMesh.SetFactor1(num);
				this.outerSphereMesh.SetFactor1(num2);
				MatrixFrame matrixFrame;
				matrixFrame.origin = base.GameEntity.GetGlobalFrame().origin;
				matrixFrame.rotation = Mat3.Identity;
				matrixFrame.rotation.ApplyScaleLocal(this.minRadius);
				MatrixFrame matrixFrame2;
				matrixFrame2.origin = base.GameEntity.GetGlobalFrame().origin;
				matrixFrame2.rotation = Mat3.Identity;
				matrixFrame2.rotation.ApplyScaleLocal(this.maxRadius);
				this.innerSphereMesh.SetVectorArgument(this.minRadius, this.maxRadius, 0f, 0f);
				this.outerSphereMesh.SetVectorArgument(this.minRadius, this.maxRadius, 0f, 0f);
				MBEditor.RenderEditorMesh(this.innerSphereMesh, matrixFrame);
				MBEditor.RenderEditorMesh(this.outerSphereMesh, matrixFrame2);
			}
		}

		// Token: 0x06002C6C RID: 11372 RVA: 0x000AC660 File Offset: 0x000AA860
		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "minRadius")
			{
				this.minRadius = MBMath.ClampFloat(this.minRadius, 0.1f, this.maxRadius);
			}
			if (variableName == "maxRadius")
			{
				this.maxRadius = MBMath.ClampFloat(this.maxRadius, this.minRadius, float.MaxValue);
			}
			if (variableName == "hideAllProbes")
			{
				MapAtmosphereProbe.hideAllProbesStatic = this.hideAllProbes;
			}
		}

		// Token: 0x040010E9 RID: 4329
		public bool visualizeRadius = true;

		// Token: 0x040010EA RID: 4330
		public bool hideAllProbes = true;

		// Token: 0x040010EB RID: 4331
		public static bool hideAllProbesStatic = true;

		// Token: 0x040010EC RID: 4332
		public float minRadius = 1f;

		// Token: 0x040010ED RID: 4333
		public float maxRadius = 2f;

		// Token: 0x040010EE RID: 4334
		public float rainDensity;

		// Token: 0x040010EF RID: 4335
		public float temperature;

		// Token: 0x040010F0 RID: 4336
		public string atmosphereType;

		// Token: 0x040010F1 RID: 4337
		public string colorGrade;

		// Token: 0x040010F2 RID: 4338
		private MetaMesh innerSphereMesh;

		// Token: 0x040010F3 RID: 4339
		private MetaMesh outerSphereMesh;
	}
}
