using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class Markable : ScriptComponentBehavior
	{
		private bool MarkerActive
		{
			get
			{
				return this._markerActive;
			}
			set
			{
				if (this._markerActive != value)
				{
					this._markerActive = value;
					base.SetScriptComponentToTick(this.GetTickRequirement());
				}
			}
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			this._marker = GameEntity.Instantiate(Mission.Current.Scene, "highlight_beam", base.GameEntity.GetGlobalFrame());
			this.DeactivateMarker();
			this._destructibleComponent = base.GameEntity.GetFirstScriptOfType<DestructableComponent>();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (this.MarkerActive)
			{
				return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
			}
			return base.GetTickRequirement();
		}

		protected internal override void OnTick(float dt)
		{
			if (this.MarkerActive)
			{
				if (this._destructibleComponent != null && this._destructibleComponent.IsDestroyed)
				{
					if (this._markerVisible)
					{
						this.DisableMarkerActivation();
						return;
					}
				}
				else if (this._markerVisible)
				{
					if (Mission.Current.CurrentTime - this._markerEventBeginningTime > this._markerActiveDuration)
					{
						this.DeactivateMarker();
						return;
					}
				}
				else if (!this._markerVisible && Mission.Current.CurrentTime - this._markerEventBeginningTime > this._markerPassiveDuration)
				{
					this.ActivateMarkerFor(this._markerActiveDuration, this._markerPassiveDuration);
				}
			}
		}

		public void DisableMarkerActivation()
		{
			this.MarkerActive = false;
			this.DeactivateMarker();
		}

		public void ActivateMarkerFor(float activeSeconds, float passiveSeconds)
		{
			if (this._destructibleComponent == null || !this._destructibleComponent.IsDestroyed)
			{
				this.MarkerActive = true;
				this._markerVisible = true;
				this._markerEventBeginningTime = Mission.Current.CurrentTime;
				this._markerActiveDuration = activeSeconds;
				this._markerPassiveDuration = passiveSeconds;
				this._marker.SetVisibilityExcludeParents(true);
				this._marker.BurstEntityParticle(true);
			}
		}

		private void DeactivateMarker()
		{
			this._markerVisible = false;
			this._marker.SetVisibilityExcludeParents(false);
			this._markerEventBeginningTime = Mission.Current.CurrentTime;
		}

		public void ResetPassiveDurationTimer()
		{
			if (!this._markerVisible && this.MarkerActive)
			{
				this._markerEventBeginningTime = Mission.Current.CurrentTime;
			}
		}

		public string MarkerPrefabName = "highlight_beam";

		private GameEntity _marker;

		private DestructableComponent _destructibleComponent;

		private bool _markerActive;

		private bool _markerVisible;

		private float _markerEventBeginningTime;

		private float _markerActiveDuration;

		private float _markerPassiveDuration;
	}
}
