using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class WaveFloater : ScriptComponentBehavior
	{
		private float ConvertToRadians(float angle)
		{
			return 0.017453292f * angle;
		}

		private void SetMatrix()
		{
			this.resetMF = base.GameEntity.GetGlobalFrame();
		}

		private void ResetMatrix()
		{
			base.GameEntity.SetGlobalFrame(this.resetMF);
		}

		private void CalculateAxis()
		{
			this.axis = new Vec3(Convert.ToSingle(this.oscillateAtX), Convert.ToSingle(this.oscillateAtY), Convert.ToSingle(this.oscillateAtZ), -1f);
			base.GameEntity.GetGlobalFrame().TransformToParent(this.axis);
			this.axis.Normalize();
		}

		private float CalculateSpeed(float fq, float maxVal, bool angular)
		{
			if (!angular)
			{
				return maxVal * 2f * fq * 1f;
			}
			return maxVal * 3.1415927f / 90f * fq * 1f;
		}

		private void CalculateOscilations()
		{
			this.ResetMatrix();
			this.oscillationStart = base.GameEntity.GetGlobalFrame();
			this.oscillationEnd = base.GameEntity.GetGlobalFrame();
			this.oscillationStart.rotation.RotateAboutAnArbitraryVector(this.axis, -this.ConvertToRadians(this.maxOscillationAngle));
			this.oscillationEnd.rotation.RotateAboutAnArbitraryVector(this.axis, this.ConvertToRadians(this.maxOscillationAngle));
		}

		private void Oscillate()
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			globalFrame.rotation = Mat3.Lerp(this.oscillationStart.rotation, this.oscillationEnd.rotation, MathF.Clamp(this.oscillationPercentage, 0f, 1f));
			base.GameEntity.SetGlobalFrame(globalFrame);
			this.oscillationPercentage = (1f + MathF.Cos(this.oscillationSpeed * 1f * this.et)) / 2f;
		}

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

		protected internal override void OnSceneSave(string saveFolder)
		{
			base.OnSceneSave(saveFolder);
			this.ResetMatrix();
		}

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

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

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

		public SimpleButton largeObject;

		public SimpleButton smallObject;

		public bool oscillateAtX;

		public bool oscillateAtY;

		public bool oscillateAtZ;

		public float oscillationFrequency = 1f;

		public float maxOscillationAngle = 10f;

		public bool bounceX;

		public float bounceXFrequency = 14f;

		public float maxBounceXDistance = 0.3f;

		public bool bounceY;

		public float bounceYFrequency = 14f;

		public float maxBounceYDistance = 0.3f;

		public bool bounceZ;

		public float bounceZFrequency = 14f;

		public float maxBounceZDistance = 0.3f;

		private Vec3 axis;

		private float oscillationSpeed = 1f;

		private float oscillationPercentage = 0.5f;

		private MatrixFrame resetMF;

		private MatrixFrame oscillationStart;

		private MatrixFrame oscillationEnd;

		private bool oscillate;

		private float bounceXSpeed = 1f;

		private float bounceXPercentage = 0.5f;

		private MatrixFrame bounceXStart;

		private MatrixFrame bounceXEnd;

		private float bounceYSpeed = 1f;

		private float bounceYPercentage = 0.5f;

		private MatrixFrame bounceYStart;

		private MatrixFrame bounceYEnd;

		private float bounceZSpeed = 1f;

		private float bounceZPercentage = 0.5f;

		private MatrixFrame bounceZStart;

		private MatrixFrame bounceZEnd;

		private bool bounce;

		private float et;

		private const float SPEED_MODIFIER = 1f;
	}
}
