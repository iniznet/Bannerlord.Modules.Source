using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class WindMill : ScriptComponentBehavior
	{
		protected internal override void OnInit()
		{
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		private void Rotate(float dt)
		{
			GameEntity gameEntity = base.GameEntity;
			float num = this.rotationSpeed * 0.001f * dt;
			MatrixFrame frame = gameEntity.GetFrame();
			frame.rotation.RotateAboutForward(num);
			gameEntity.SetFrame(ref frame);
			this.currentRotation += num;
		}

		private static bool IsRotationPhaseInsidePhaseBoundaries(float currentPhase, float startPhase, float endPhase)
		{
			if (endPhase <= startPhase)
			{
				return currentPhase > startPhase;
			}
			return currentPhase > startPhase && currentPhase < endPhase;
		}

		public static int GetIntegerFromStringEnd(string str)
		{
			string text = "";
			for (int i = str.Length - 1; i > -1; i--)
			{
				char c = str[i];
				if (c < '0' || c > '9')
				{
					break;
				}
				text = c.ToString() + text;
			}
			return Convert.ToInt32(text);
		}

		private void DoWaterMillCalculation()
		{
			float num = (float)base.GameEntity.ChildCount;
			if (num > 0f)
			{
				IEnumerable<GameEntity> children = base.GameEntity.GetChildren();
				float num2 = 6.28f / num;
				foreach (GameEntity gameEntity in children)
				{
					int integerFromStringEnd = WindMill.GetIntegerFromStringEnd(gameEntity.Name);
					float num3 = this.currentRotation % 6.28f;
					float num4 = (num2 * (float)integerFromStringEnd + this.waterSplashPhaseOffset) % 6.28f;
					float num5 = (num4 + num2 * this.waterSplashIntervalMultiplier) % 6.28f;
					if (WindMill.IsRotationPhaseInsidePhaseBoundaries(num3, num4, num5))
					{
						gameEntity.ResumeParticleSystem(true);
					}
					else
					{
						gameEntity.PauseParticleSystem(true);
					}
				}
			}
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.TickParallel;
		}

		protected internal override void OnTickParallel(float dt)
		{
			this.Rotate(dt);
			if (this.isWaterMill)
			{
				this.DoWaterMillCalculation();
			}
		}

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.Rotate(dt);
			if (this.isWaterMill)
			{
				this.DoWaterMillCalculation();
			}
		}

		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "testMesh")
			{
				if (this.testMesh != null)
				{
					base.GameEntity.AddMultiMesh(this.testMesh, true);
					return;
				}
			}
			else if (variableName == "testTexture")
			{
				if (this.testTexture != null)
				{
					Material material = base.GameEntity.GetFirstMesh().GetMaterial().CreateCopy();
					material.SetTexture(Material.MBTextureType.DiffuseMap, this.testTexture);
					base.GameEntity.SetMaterialForAllMeshes(material);
					return;
				}
			}
			else
			{
				if (variableName == "testEntity")
				{
					this.testEntity != null;
					return;
				}
				if (variableName == "testButton")
				{
					this.rotationSpeed *= 2f;
				}
			}
		}

		public float rotationSpeed = 100f;

		public float waterSplashPhaseOffset;

		public float waterSplashIntervalMultiplier = 1f;

		public MetaMesh testMesh;

		public Texture testTexture;

		public GameEntity testEntity;

		public bool isWaterMill;

		private float currentRotation;
	}
}
