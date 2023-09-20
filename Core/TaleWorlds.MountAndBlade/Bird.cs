using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class Bird : MissionObject
	{
		private Bird.State GetState()
		{
			if (this.CanFly && !this._canLand)
			{
				return Bird.State.Airborne;
			}
			return Bird.State.Perched;
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			base.GameEntity.SetAnimationSoundActivation(true);
			base.GameEntity.Skeleton.SetAnimationAtChannel("anim_bird_idle", 0, 1f, -1f, 0f);
			base.GameEntity.Skeleton.SetAnimationParameterAtChannel(0, MBRandom.RandomFloat * 0.5f);
			this._kmPerHour = 4f;
			this._state = this.GetState();
			if (this._timer == null)
			{
				this._timer = new BasicMissionTimer();
			}
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			base.GameEntity.Skeleton.SetAnimationAtChannel("anim_bird_idle", 0, 1f, -1f, 0f);
			base.GameEntity.Skeleton.SetAnimationParameterAtChannel(0, 0f);
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

		protected internal override void OnTick(float dt)
		{
			switch (this._state)
			{
			case Bird.State.TakingOff:
				this.ApplyDisplacement(dt);
				if (base.GameEntity.Skeleton.GetAnimationParameterAtChannel(0) > 0.99f)
				{
					base.GameEntity.Skeleton.SetAnimationAtChannel("anim_bird_cycle", 0, 1f, -1f, 0f);
					this._timer.Reset();
					this.SetDisplacement();
					this._state = Bird.State.Airborne;
					return;
				}
				break;
			case Bird.State.Airborne:
				if (this._timer.ElapsedTime <= 5f)
				{
					this.ApplyDisplacement(dt);
					return;
				}
				if (this._canLand)
				{
					base.GameEntity.Skeleton.SetAnimationAtChannel("anim_bird_landing", 0, 1f, -1f, 0f);
					this._timer.Reset();
					this._state = Bird.State.Landing;
					this.SetDisplacement();
					return;
				}
				base.GameEntity.SetVisibilityExcludeParents(false);
				return;
			case Bird.State.Landing:
				this.ApplyDisplacement(dt);
				if (base.GameEntity.Skeleton.GetAnimationParameterAtChannel(0) > 0.99f)
				{
					base.GameEntity.Skeleton.SetAnimationAtChannel("anim_bird_idle", 0, 1f, -1f, 0f);
					this._timer.Reset();
					this._state = Bird.State.Perched;
					return;
				}
				break;
			case Bird.State.Perched:
				if (this.CanFly && this._timer.ElapsedTime > 5f + MBRandom.RandomFloat * 13f && base.GameEntity.Skeleton.GetAnimationParameterAtChannel(0) > 0.99f)
				{
					base.GameEntity.Skeleton.SetAnimationAtChannel("anim_bird_flying", 0, 1f, -1f, 0f);
					this._timer.Reset();
					this._state = Bird.State.TakingOff;
				}
				break;
			default:
				return;
			}
		}

		private void ApplyDisplacement(float dt)
		{
			float num = this._kmPerHour * dt;
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			MatrixFrame matrixFrame = globalFrame;
			Vec3 f = globalFrame.rotation.f;
			Vec3 u = globalFrame.rotation.u;
			f.Normalize();
			u.Normalize();
			if (this._state == Bird.State.TakingOff)
			{
				matrixFrame.origin = matrixFrame.origin - f * 0.30769232f + u * 0.1f;
			}
			else if (this._state == Bird.State.Airborne)
			{
				globalFrame.origin -= f * num;
				matrixFrame.origin -= f * num;
			}
			else if (this._state == Bird.State.Landing)
			{
				matrixFrame.origin = matrixFrame.origin - f * 0.30769232f - u * 0.1f;
			}
			base.GameEntity.SetGlobalFrame(globalFrame);
		}

		private void SetDisplacement()
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			Vec3 f = globalFrame.rotation.f;
			Vec3 u = globalFrame.rotation.u;
			f.Normalize();
			u.Normalize();
			if (this._state == Bird.State.TakingOff)
			{
				globalFrame.origin -= f * 20f - u * 6.5f;
			}
			else if (this._state == Bird.State.Landing)
			{
				globalFrame.origin -= f * 20f + u * 6.5f;
			}
			base.GameEntity.SetGlobalFrame(globalFrame);
		}

		private const float Speed = 14400f;

		private const string IdleAnimation = "anim_bird_idle";

		private const string LandingAnimation = "anim_bird_landing";

		private const string TakingOffAnimation = "anim_bird_flying";

		private const string FlyCycleAnimation = "anim_bird_cycle";

		public bool CanFly;

		private float _kmPerHour;

		private Bird.State _state;

		private BasicMissionTimer _timer;

		private bool _canLand = true;

		private enum State
		{
			TakingOff,
			Airborne,
			Landing,
			Perched
		}
	}
}
