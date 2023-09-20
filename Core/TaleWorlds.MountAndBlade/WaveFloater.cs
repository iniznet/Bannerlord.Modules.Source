using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000387 RID: 903
	public class WaveFloater : ScriptComponentBehavior
	{
		// Token: 0x06003196 RID: 12694 RVA: 0x000CD52A File Offset: 0x000CB72A
		private float ConvertToRadians(float angle)
		{
			return 0.017453292f * angle;
		}

		// Token: 0x06003197 RID: 12695 RVA: 0x000CD533 File Offset: 0x000CB733
		private void SetMatrix()
		{
			this.resetMF = base.GameEntity.GetGlobalFrame();
		}

		// Token: 0x06003198 RID: 12696 RVA: 0x000CD546 File Offset: 0x000CB746
		private void ResetMatrix()
		{
			base.GameEntity.SetGlobalFrame(this.resetMF);
		}

		// Token: 0x06003199 RID: 12697 RVA: 0x000CD55C File Offset: 0x000CB75C
		private void CalculateAxis()
		{
			this.axis = new Vec3(Convert.ToSingle(this.oscillateAtX), Convert.ToSingle(this.oscillateAtY), Convert.ToSingle(this.oscillateAtZ), -1f);
			base.GameEntity.GetGlobalFrame().TransformToParent(this.axis);
			this.axis.Normalize();
		}

		// Token: 0x0600319A RID: 12698 RVA: 0x000CD5C0 File Offset: 0x000CB7C0
		private float CalculateSpeed(float fq, float maxVal, bool angular)
		{
			if (!angular)
			{
				return maxVal * 2f * fq * 1f;
			}
			return maxVal * 3.1415927f / 90f * fq * 1f;
		}

		// Token: 0x0600319B RID: 12699 RVA: 0x000CD5EC File Offset: 0x000CB7EC
		private void CalculateOscilations()
		{
			this.ResetMatrix();
			this.oscillationStart = base.GameEntity.GetGlobalFrame();
			this.oscillationEnd = base.GameEntity.GetGlobalFrame();
			this.oscillationStart.rotation.RotateAboutAnArbitraryVector(this.axis, -this.ConvertToRadians(this.maxOscillationAngle));
			this.oscillationEnd.rotation.RotateAboutAnArbitraryVector(this.axis, this.ConvertToRadians(this.maxOscillationAngle));
		}

		// Token: 0x0600319C RID: 12700 RVA: 0x000CD668 File Offset: 0x000CB868
		private void Oscillate()
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			globalFrame.rotation = Mat3.Lerp(this.oscillationStart.rotation, this.oscillationEnd.rotation, MathF.Clamp(this.oscillationPercentage, 0f, 1f));
			base.GameEntity.SetGlobalFrame(globalFrame);
			this.oscillationPercentage = (1f + MathF.Cos(this.oscillationSpeed * 1f * this.et)) / 2f;
		}

		// Token: 0x0600319D RID: 12701 RVA: 0x000CD6F0 File Offset: 0x000CB8F0
		private void CalculateBounces()
		{
			this.ResetMatrix();
			this.bounceXStart = base.GameEntity.GetGlobalFrame();
			this.bounceXEnd = base.GameEntity.GetGlobalFrame();
			this.bounceYStart = base.GameEntity.GetGlobalFrame();
			this.bounceYEnd = base.GameEntity.GetGlobalFrame();
			this.bounceZStart = base.GameEntity.GetGlobalFrame();
			this.bounceZEnd = base.GameEntity.GetGlobalFrame();
			this.bounceXStart.origin.x = this.bounceXStart.origin.x + this.maxBounceXDistance;
			this.bounceXEnd.origin.x = this.bounceXEnd.origin.x - this.maxBounceXDistance;
			this.bounceYStart.origin.y = this.bounceYStart.origin.y + this.maxBounceYDistance;
			this.bounceYEnd.origin.y = this.bounceYEnd.origin.y - this.maxBounceYDistance;
			this.bounceZStart.origin.z = this.bounceZStart.origin.z + this.maxBounceZDistance;
			this.bounceZEnd.origin.z = this.bounceZEnd.origin.z - this.maxBounceZDistance;
		}

		// Token: 0x0600319E RID: 12702 RVA: 0x000CD808 File Offset: 0x000CBA08
		private void Bounce()
		{
			if (this.bounceX)
			{
				MatrixFrame matrixFrame = base.GameEntity.GetGlobalFrame();
				matrixFrame.origin.x = Vec3.Lerp(this.bounceXStart.origin, this.bounceXEnd.origin, MathF.Clamp(this.bounceXPercentage, 0f, 1f)).x;
				base.GameEntity.SetGlobalFrame(matrixFrame);
				this.bounceXPercentage = (1f + MathF.Sin(this.bounceXSpeed * 1f * this.et)) / 2f;
			}
			if (this.bounceY)
			{
				MatrixFrame matrixFrame = base.GameEntity.GetGlobalFrame();
				matrixFrame.origin.y = Vec3.Lerp(this.bounceYStart.origin, this.bounceYEnd.origin, MathF.Clamp(this.bounceYPercentage, 0f, 1f)).y;
				base.GameEntity.SetGlobalFrame(matrixFrame);
				this.bounceYPercentage = (1f + MathF.Cos(this.bounceYSpeed * 1f * this.et)) / 2f;
			}
			if (this.bounceZ)
			{
				MatrixFrame matrixFrame = base.GameEntity.GetGlobalFrame();
				matrixFrame.origin.z = Vec3.Lerp(this.bounceZStart.origin, this.bounceZEnd.origin, MathF.Clamp(this.bounceZPercentage, 0f, 1f)).z;
				base.GameEntity.SetGlobalFrame(matrixFrame);
				this.bounceZPercentage = (1f + MathF.Cos(this.bounceZSpeed * 1f * this.et)) / 2f;
			}
		}

		// Token: 0x0600319F RID: 12703 RVA: 0x000CD9C4 File Offset: 0x000CBBC4
		protected internal override void OnInit()
		{
			base.OnInit();
			this.SetMatrix();
			this.oscillate = this.oscillateAtX || this.oscillateAtY || this.oscillateAtZ;
			this.bounce = this.bounceX || this.bounceY || this.bounceZ;
			this.oscillationSpeed = this.CalculateSpeed(this.oscillationFrequency, this.maxOscillationAngle, true);
			this.bounceXSpeed = this.CalculateSpeed(this.bounceXFrequency, this.maxBounceXDistance, false);
			this.bounceYSpeed = this.CalculateSpeed(this.bounceYFrequency, this.maxBounceYDistance, false);
			this.bounceZSpeed = this.CalculateSpeed(this.bounceZFrequency, this.maxBounceZDistance, false);
			this.CalculateBounces();
			this.CalculateAxis();
			this.CalculateOscilations();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x060031A0 RID: 12704 RVA: 0x000CDAA0 File Offset: 0x000CBCA0
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this.SetMatrix();
			this.oscillate = this.oscillateAtX || this.oscillateAtY || this.oscillateAtZ;
			this.bounce = this.bounceX || this.bounceY || this.bounceZ;
			this.oscillationSpeed = this.CalculateSpeed(this.oscillationFrequency, this.maxOscillationAngle, true);
			this.bounceXSpeed = this.CalculateSpeed(this.bounceXFrequency, this.maxBounceXDistance, false);
			this.bounceYSpeed = this.CalculateSpeed(this.bounceYFrequency, this.maxBounceYDistance, false);
			this.bounceZSpeed = this.CalculateSpeed(this.bounceZFrequency, this.maxBounceZDistance, false);
			this.CalculateBounces();
			this.CalculateAxis();
			this.CalculateOscilations();
		}

		// Token: 0x060031A1 RID: 12705 RVA: 0x000CDB6D File Offset: 0x000CBD6D
		protected internal override void OnSceneSave(string saveFolder)
		{
			base.OnSceneSave(saveFolder);
			this.ResetMatrix();
		}

		// Token: 0x060031A2 RID: 12706 RVA: 0x000CDB7C File Offset: 0x000CBD7C
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.et += dt;
			if (this.oscillate)
			{
				this.Oscillate();
			}
			if (this.bounce)
			{
				this.Bounce();
			}
		}

		// Token: 0x060031A3 RID: 12707 RVA: 0x000CDBAF File Offset: 0x000CBDAF
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

		// Token: 0x060031A4 RID: 12708 RVA: 0x000CDBB9 File Offset: 0x000CBDB9
		protected internal override void OnTick(float dt)
		{
			this.et += dt;
			if (this.oscillate)
			{
				this.Oscillate();
			}
			if (this.bounce)
			{
				this.Bounce();
			}
		}

		// Token: 0x060031A5 RID: 12709 RVA: 0x000CDBE8 File Offset: 0x000CBDE8
		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "largeObject")
			{
				this.ResetMatrix();
				this.oscillateAtX = true;
				this.oscillateAtY = true;
				this.oscillationFrequency = 1.5f;
				this.maxOscillationAngle = 7.4f;
				this.bounceX = true;
				this.bounceXFrequency = 2f;
				this.maxBounceXDistance = 0.1f;
				this.bounceY = true;
				this.bounceYFrequency = 0.2f;
				this.maxBounceYDistance = 0.5f;
				this.bounceZ = true;
				this.bounceZFrequency = 0.6f;
				this.maxBounceZDistance = 0.22f;
				this.CalculateAxis();
				this.oscillationSpeed = this.CalculateSpeed(this.oscillationFrequency, this.maxOscillationAngle, true);
				this.bounceXSpeed = this.CalculateSpeed(this.bounceXFrequency, this.maxBounceXDistance, false);
				this.bounceYSpeed = this.CalculateSpeed(this.bounceYFrequency, this.maxBounceYDistance, false);
				this.bounceZSpeed = this.CalculateSpeed(this.bounceZFrequency, this.maxBounceZDistance, false);
				this.CalculateOscilations();
				this.CalculateOscilations();
				this.oscillate = true;
				this.bounce = true;
				return;
			}
			if (variableName == "smallObject")
			{
				this.ResetMatrix();
				this.oscillateAtX = true;
				this.oscillateAtY = true;
				this.oscillateAtZ = true;
				this.oscillationFrequency = 1f;
				this.maxOscillationAngle = 11f;
				this.bounceX = true;
				this.bounceXFrequency = 1.5f;
				this.maxBounceXDistance = 0.3f;
				this.bounceY = true;
				this.bounceYFrequency = 1.5f;
				this.maxBounceYDistance = 0.2f;
				this.bounceZ = true;
				this.bounceZFrequency = 1f;
				this.maxBounceZDistance = 0.1f;
				this.CalculateAxis();
				this.oscillationSpeed = this.CalculateSpeed(this.oscillationFrequency, this.maxOscillationAngle, true);
				this.bounceXSpeed = this.CalculateSpeed(this.bounceXFrequency, this.maxBounceXDistance, false);
				this.bounceYSpeed = this.CalculateSpeed(this.bounceYFrequency, this.maxBounceYDistance, false);
				this.bounceZSpeed = this.CalculateSpeed(this.bounceZFrequency, this.maxBounceZDistance, false);
				this.CalculateOscilations();
				this.CalculateOscilations();
				this.oscillate = true;
				this.bounce = true;
				return;
			}
			if (variableName == "oscillateAtX" || variableName == "oscillateAtY" || variableName == "oscillateAtZ")
			{
				if (this.oscillateAtX || this.oscillateAtY || this.oscillateAtZ)
				{
					if (!this.oscillate)
					{
						if (!this.bounce)
						{
							this.SetMatrix();
						}
						this.oscillate = true;
					}
				}
				else
				{
					this.oscillate = false;
					if (!this.bounce)
					{
						this.ResetMatrix();
					}
				}
				this.CalculateAxis();
				this.CalculateOscilations();
				return;
			}
			if (variableName == "oscillationFrequency")
			{
				this.oscillationSpeed = this.CalculateSpeed(this.oscillationFrequency, this.maxOscillationAngle, true);
				return;
			}
			if (variableName == "maxOscillationAngle")
			{
				this.maxOscillationAngle = MathF.Clamp(this.maxOscillationAngle, 0f, 90f);
				this.oscillationSpeed = this.CalculateSpeed(this.oscillationFrequency, this.maxOscillationAngle, true);
				this.CalculateOscilations();
				return;
			}
			if (variableName == "bounceX" || variableName == "bounceY" || variableName == "bounceZ")
			{
				if (this.bounceX || this.bounceY || this.bounceZ)
				{
					if (!this.bounce)
					{
						if (!this.oscillate)
						{
							this.SetMatrix();
						}
						this.bounce = true;
					}
				}
				else
				{
					this.bounce = false;
					if (!this.oscillate)
					{
						this.ResetMatrix();
					}
				}
				this.CalculateBounces();
				return;
			}
			if (variableName == "bounceXFrequency")
			{
				this.bounceXSpeed = this.CalculateSpeed(this.bounceXFrequency, this.maxBounceXDistance, false);
				return;
			}
			if (variableName == "bounceYFrequency")
			{
				this.bounceYSpeed = this.CalculateSpeed(this.bounceYFrequency, this.maxBounceYDistance, false);
				return;
			}
			if (variableName == "bounceZFrequency")
			{
				this.bounceZSpeed = this.CalculateSpeed(this.bounceZFrequency, this.maxBounceZDistance, false);
				return;
			}
			if (variableName == "maxBounceXDistance")
			{
				this.bounceXSpeed = this.CalculateSpeed(this.bounceXFrequency, this.maxBounceXDistance, false);
				this.CalculateBounces();
				return;
			}
			if (variableName == "maxBounceYDistance")
			{
				this.bounceYSpeed = this.CalculateSpeed(this.bounceYFrequency, this.maxBounceYDistance, false);
				this.CalculateBounces();
				return;
			}
			if (variableName == "maxBounceZDistance")
			{
				this.bounceZSpeed = this.CalculateSpeed(this.bounceZFrequency, this.maxBounceZDistance, false);
				this.CalculateBounces();
			}
		}

		// Token: 0x040014B3 RID: 5299
		public SimpleButton largeObject;

		// Token: 0x040014B4 RID: 5300
		public SimpleButton smallObject;

		// Token: 0x040014B5 RID: 5301
		public bool oscillateAtX;

		// Token: 0x040014B6 RID: 5302
		public bool oscillateAtY;

		// Token: 0x040014B7 RID: 5303
		public bool oscillateAtZ;

		// Token: 0x040014B8 RID: 5304
		public float oscillationFrequency = 1f;

		// Token: 0x040014B9 RID: 5305
		public float maxOscillationAngle = 10f;

		// Token: 0x040014BA RID: 5306
		public bool bounceX;

		// Token: 0x040014BB RID: 5307
		public float bounceXFrequency = 14f;

		// Token: 0x040014BC RID: 5308
		public float maxBounceXDistance = 0.3f;

		// Token: 0x040014BD RID: 5309
		public bool bounceY;

		// Token: 0x040014BE RID: 5310
		public float bounceYFrequency = 14f;

		// Token: 0x040014BF RID: 5311
		public float maxBounceYDistance = 0.3f;

		// Token: 0x040014C0 RID: 5312
		public bool bounceZ;

		// Token: 0x040014C1 RID: 5313
		public float bounceZFrequency = 14f;

		// Token: 0x040014C2 RID: 5314
		public float maxBounceZDistance = 0.3f;

		// Token: 0x040014C3 RID: 5315
		private Vec3 axis;

		// Token: 0x040014C4 RID: 5316
		private float oscillationSpeed = 1f;

		// Token: 0x040014C5 RID: 5317
		private float oscillationPercentage = 0.5f;

		// Token: 0x040014C6 RID: 5318
		private MatrixFrame resetMF;

		// Token: 0x040014C7 RID: 5319
		private MatrixFrame oscillationStart;

		// Token: 0x040014C8 RID: 5320
		private MatrixFrame oscillationEnd;

		// Token: 0x040014C9 RID: 5321
		private bool oscillate;

		// Token: 0x040014CA RID: 5322
		private float bounceXSpeed = 1f;

		// Token: 0x040014CB RID: 5323
		private float bounceXPercentage = 0.5f;

		// Token: 0x040014CC RID: 5324
		private MatrixFrame bounceXStart;

		// Token: 0x040014CD RID: 5325
		private MatrixFrame bounceXEnd;

		// Token: 0x040014CE RID: 5326
		private float bounceYSpeed = 1f;

		// Token: 0x040014CF RID: 5327
		private float bounceYPercentage = 0.5f;

		// Token: 0x040014D0 RID: 5328
		private MatrixFrame bounceYStart;

		// Token: 0x040014D1 RID: 5329
		private MatrixFrame bounceYEnd;

		// Token: 0x040014D2 RID: 5330
		private float bounceZSpeed = 1f;

		// Token: 0x040014D3 RID: 5331
		private float bounceZPercentage = 0.5f;

		// Token: 0x040014D4 RID: 5332
		private MatrixFrame bounceZStart;

		// Token: 0x040014D5 RID: 5333
		private MatrixFrame bounceZEnd;

		// Token: 0x040014D6 RID: 5334
		private bool bounce;

		// Token: 0x040014D7 RID: 5335
		private float et;

		// Token: 0x040014D8 RID: 5336
		private const float SPEED_MODIFIER = 1f;
	}
}
