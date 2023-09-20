using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000354 RID: 852
	public class Markable : ScriptComponentBehavior
	{
		// Token: 0x1700083C RID: 2108
		// (get) Token: 0x06002DDC RID: 11740 RVA: 0x000B566C File Offset: 0x000B386C
		// (set) Token: 0x06002DDD RID: 11741 RVA: 0x000B5674 File Offset: 0x000B3874
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

		// Token: 0x06002DDE RID: 11742 RVA: 0x000B5694 File Offset: 0x000B3894
		protected internal override void OnInit()
		{
			base.OnInit();
			this._marker = GameEntity.Instantiate(Mission.Current.Scene, "highlight_beam", base.GameEntity.GetGlobalFrame());
			this.DeactivateMarker();
			this._destructibleComponent = base.GameEntity.GetFirstScriptOfType<DestructableComponent>();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06002DDF RID: 11743 RVA: 0x000B56EF File Offset: 0x000B38EF
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (this.MarkerActive)
			{
				return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06002DE0 RID: 11744 RVA: 0x000B5708 File Offset: 0x000B3908
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

		// Token: 0x06002DE1 RID: 11745 RVA: 0x000B579C File Offset: 0x000B399C
		public void DisableMarkerActivation()
		{
			this.MarkerActive = false;
			this.DeactivateMarker();
		}

		// Token: 0x06002DE2 RID: 11746 RVA: 0x000B57AC File Offset: 0x000B39AC
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

		// Token: 0x06002DE3 RID: 11747 RVA: 0x000B5812 File Offset: 0x000B3A12
		private void DeactivateMarker()
		{
			this._markerVisible = false;
			this._marker.SetVisibilityExcludeParents(false);
			this._markerEventBeginningTime = Mission.Current.CurrentTime;
		}

		// Token: 0x06002DE4 RID: 11748 RVA: 0x000B5837 File Offset: 0x000B3A37
		public void ResetPassiveDurationTimer()
		{
			if (!this._markerVisible && this.MarkerActive)
			{
				this._markerEventBeginningTime = Mission.Current.CurrentTime;
			}
		}

		// Token: 0x04001235 RID: 4661
		public string MarkerPrefabName = "highlight_beam";

		// Token: 0x04001236 RID: 4662
		private GameEntity _marker;

		// Token: 0x04001237 RID: 4663
		private DestructableComponent _destructibleComponent;

		// Token: 0x04001238 RID: 4664
		private bool _markerActive;

		// Token: 0x04001239 RID: 4665
		private bool _markerVisible;

		// Token: 0x0400123A RID: 4666
		private float _markerEventBeginningTime;

		// Token: 0x0400123B RID: 4667
		private float _markerActiveDuration;

		// Token: 0x0400123C RID: 4668
		private float _markerPassiveDuration;
	}
}
