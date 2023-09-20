using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class TestScript : ScriptComponentBehavior
	{
		private void Move(float dt)
		{
			if (MathF.Abs(this.MoveDistance) < 1E-05f)
			{
				return;
			}
			Vec3 vec = new Vec3(this.MoveAxisX, this.MoveAxisY, this.MoveAxisZ, -1f);
			vec.Normalize();
			Vec3 vec2 = vec * this.MoveSpeed * this.MoveDirection;
			float num = vec2.Length * this.MoveDirection;
			if (this.CurrentDistance + num <= -this.MoveDistance)
			{
				this.MoveDirection = 1f;
				num *= -1f;
				vec2 *= -1f;
			}
			else if (this.CurrentDistance + num >= this.MoveDistance)
			{
				this.MoveDirection = -1f;
				num *= -1f;
				vec2 *= -1f;
			}
			this.CurrentDistance += num;
			MatrixFrame frame = base.GameEntity.GetFrame();
			frame.origin += vec2;
			base.GameEntity.SetFrame(ref frame);
		}

		private void Rotate(float dt)
		{
			MatrixFrame frame = base.GameEntity.GetFrame();
			frame.rotation.RotateAboutUp(this.rotationSpeed * 0.001f * dt);
			base.GameEntity.SetFrame(ref frame);
			this.currentRotation += this.rotationSpeed * 0.001f * dt;
		}

		private bool isRotationPhaseInsidePhaseBoundries(float currentPhase, float startPhase, float endPhase)
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
					int integerFromStringEnd = TestScript.GetIntegerFromStringEnd(gameEntity.Name);
					float num3 = this.currentRotation % 6.28f;
					float num4 = (num2 * (float)integerFromStringEnd + this.waterSplashPhaseOffset) % 6.28f;
					float num5 = (num4 + num2 * this.waterSplashIntervalMultiplier) % 6.28f;
					if (this.isRotationPhaseInsidePhaseBoundries(num3, num4, num5))
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

		protected internal override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

		protected internal override void OnTick(float dt)
		{
			this.Rotate(dt);
			this.Move(dt);
			if (this.isWaterMill)
			{
				this.DoWaterMillCalculation();
			}
		}

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.Rotate(dt);
			this.Move(dt);
			if (this.isWaterMill)
			{
				this.DoWaterMillCalculation();
			}
			if (this.sideRotatingEntity != null)
			{
				MatrixFrame frame = this.sideRotatingEntity.GetFrame();
				frame.rotation.RotateAboutSide(this.rotationSpeed * 0.01f * dt);
				this.sideRotatingEntity.SetFrame(ref frame);
			}
			if (this.forwardRotatingEntity != null)
			{
				MatrixFrame frame2 = this.forwardRotatingEntity.GetFrame();
				frame2.rotation.RotateAboutSide(this.rotationSpeed * 0.005f * dt);
				this.forwardRotatingEntity.SetFrame(ref frame2);
			}
		}

		public string testString;

		public float rotationSpeed;

		public float waterSplashPhaseOffset;

		public float waterSplashIntervalMultiplier = 1f;

		public bool isWaterMill;

		private float currentRotation;

		public float MoveAxisX = 1f;

		public float MoveAxisY;

		public float MoveAxisZ;

		public float MoveSpeed = 0.0001f;

		public float MoveDistance = 10f;

		protected float MoveDirection = 1f;

		protected float CurrentDistance;

		public GameEntity sideRotatingEntity;

		public GameEntity forwardRotatingEntity;
	}
}
