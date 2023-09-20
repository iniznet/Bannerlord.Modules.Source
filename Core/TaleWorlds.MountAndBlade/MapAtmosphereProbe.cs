using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class MapAtmosphereProbe : ScriptComponentBehavior
	{
		public float GetInfluenceAmount(Vec3 worldPosition)
		{
			return MBMath.SmoothStep(this.minRadius, this.maxRadius, worldPosition.Distance(base.GameEntity.GetGlobalFrame().origin));
		}

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

		public bool visualizeRadius = true;

		public bool hideAllProbes = true;

		public static bool hideAllProbesStatic = true;

		public float minRadius = 1f;

		public float maxRadius = 2f;

		public float rainDensity;

		public float temperature;

		public string atmosphereType;

		public string colorGrade;

		private MetaMesh innerSphereMesh;

		private MetaMesh outerSphereMesh;
	}
}
