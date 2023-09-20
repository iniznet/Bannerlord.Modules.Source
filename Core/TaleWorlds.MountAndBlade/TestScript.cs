using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200036E RID: 878
	public class TestScript : ScriptComponentBehavior
	{
		// Token: 0x06002FF7 RID: 12279 RVA: 0x000C4F00 File Offset: 0x000C3100
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

		// Token: 0x06002FF8 RID: 12280 RVA: 0x000C5010 File Offset: 0x000C3210
		private void Rotate(float dt)
		{
			MatrixFrame frame = base.GameEntity.GetFrame();
			frame.rotation.RotateAboutUp(this.rotationSpeed * 0.001f * dt);
			base.GameEntity.SetFrame(ref frame);
			this.currentRotation += this.rotationSpeed * 0.001f * dt;
		}

		// Token: 0x06002FF9 RID: 12281 RVA: 0x000C506B File Offset: 0x000C326B
		private bool isRotationPhaseInsidePhaseBoundries(float currentPhase, float startPhase, float endPhase)
		{
			if (endPhase <= startPhase)
			{
				return currentPhase > startPhase;
			}
			return currentPhase > startPhase && currentPhase < endPhase;
		}

		// Token: 0x06002FFA RID: 12282 RVA: 0x000C5080 File Offset: 0x000C3280
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

		// Token: 0x06002FFB RID: 12283 RVA: 0x000C50CC File Offset: 0x000C32CC
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

		// Token: 0x06002FFC RID: 12284 RVA: 0x000C5198 File Offset: 0x000C3398
		protected internal override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06002FFD RID: 12285 RVA: 0x000C51AC File Offset: 0x000C33AC
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

		// Token: 0x06002FFE RID: 12286 RVA: 0x000C51B6 File Offset: 0x000C33B6
		protected internal override void OnTick(float dt)
		{
			this.Rotate(dt);
			this.Move(dt);
			if (this.isWaterMill)
			{
				this.DoWaterMillCalculation();
			}
		}

		// Token: 0x06002FFF RID: 12287 RVA: 0x000C51D4 File Offset: 0x000C33D4
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

		// Token: 0x040013E6 RID: 5094
		public string testString;

		// Token: 0x040013E7 RID: 5095
		public float rotationSpeed;

		// Token: 0x040013E8 RID: 5096
		public float waterSplashPhaseOffset;

		// Token: 0x040013E9 RID: 5097
		public float waterSplashIntervalMultiplier = 1f;

		// Token: 0x040013EA RID: 5098
		public bool isWaterMill;

		// Token: 0x040013EB RID: 5099
		private float currentRotation;

		// Token: 0x040013EC RID: 5100
		public float MoveAxisX = 1f;

		// Token: 0x040013ED RID: 5101
		public float MoveAxisY;

		// Token: 0x040013EE RID: 5102
		public float MoveAxisZ;

		// Token: 0x040013EF RID: 5103
		public float MoveSpeed = 0.0001f;

		// Token: 0x040013F0 RID: 5104
		public float MoveDistance = 10f;

		// Token: 0x040013F1 RID: 5105
		protected float MoveDirection = 1f;

		// Token: 0x040013F2 RID: 5106
		protected float CurrentDistance;

		// Token: 0x040013F3 RID: 5107
		public GameEntity sideRotatingEntity;

		// Token: 0x040013F4 RID: 5108
		public GameEntity forwardRotatingEntity;
	}
}
