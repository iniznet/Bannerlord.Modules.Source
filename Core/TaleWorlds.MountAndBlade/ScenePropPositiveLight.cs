using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class ScenePropPositiveLight : ScriptComponentBehavior
	{
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.SetMeshParams();
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			this.SetMeshParams();
		}

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

		private uint CalculateFactor(Vec3 color, float alpha)
		{
			float num = 10f;
			byte maxValue = byte.MaxValue;
			alpha = MathF.Min(MathF.Max(0f, alpha), num);
			return ((uint)(alpha / num * (float)maxValue) << 24) + ((uint)(color.x * (float)maxValue) << 16) + ((uint)(color.y * (float)maxValue) << 8) + (uint)(color.z * (float)maxValue);
		}

		protected internal override bool IsOnlyVisual()
		{
			return true;
		}

		public float Flatness_X;

		public float Flatness_Y;

		public float Flatness_Z;

		public float DirectLightRed = 1f;

		public float DirectLightGreen = 1f;

		public float DirectLightBlue = 1f;

		public float DirectLightIntensity = 1f;

		public float AmbientLightRed;

		public float AmbientLightGreen;

		public float AmbientLightBlue = 1f;

		public float AmbientLightIntensity = 1f;
	}
}
