using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	// Token: 0x02000116 RID: 278
	public class KingdomDecisionPopupWidget : Widget
	{
		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x06000E24 RID: 3620 RVA: 0x000276D1 File Offset: 0x000258D1
		// (set) Token: 0x06000E25 RID: 3621 RVA: 0x000276D9 File Offset: 0x000258D9
		public int DelayAfterKingsDecision { get; set; } = 5;

		// Token: 0x06000E26 RID: 3622 RVA: 0x000276E2 File Offset: 0x000258E2
		public KingdomDecisionPopupWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000E27 RID: 3623 RVA: 0x000276FD File Offset: 0x000258FD
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._kingDecisionDoneTime != -1f && base.EventManager.Time - this._kingDecisionDoneTime > (float)this.DelayAfterKingsDecision)
			{
				this.ExecuteFinalDone();
			}
		}

		// Token: 0x06000E28 RID: 3624 RVA: 0x00027734 File Offset: 0x00025934
		private void ExecuteFinalDone()
		{
			base.EventFired("FinalDone", Array.Empty<object>());
			this._kingDecisionDoneTime = -1f;
			using (IEnumerator<Widget> enumerator = base.AllChildren.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KingdomDecisionOptionWidget kingdomDecisionOptionWidget;
					if ((kingdomDecisionOptionWidget = enumerator.Current as KingdomDecisionOptionWidget) != null)
					{
						kingdomDecisionOptionWidget.OnFinalDone();
					}
				}
			}
		}

		// Token: 0x06000E29 RID: 3625 RVA: 0x000277A4 File Offset: 0x000259A4
		private void OnKingsDecisionDone()
		{
			this._kingDecisionDoneTime = base.EventManager.Time;
			using (IEnumerator<Widget> enumerator = base.AllChildren.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KingdomDecisionOptionWidget kingdomDecisionOptionWidget;
					if ((kingdomDecisionOptionWidget = enumerator.Current as KingdomDecisionOptionWidget) != null)
					{
						kingdomDecisionOptionWidget.OnKingsDecisionDone();
					}
				}
			}
		}

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06000E2A RID: 3626 RVA: 0x0002780C File Offset: 0x00025A0C
		// (set) Token: 0x06000E2B RID: 3627 RVA: 0x00027814 File Offset: 0x00025A14
		[Editor(false)]
		public bool IsKingsDecisionDone
		{
			get
			{
				return this._isKingsDecisionDone;
			}
			set
			{
				if (this._isKingsDecisionDone != value)
				{
					this._isKingsDecisionDone = value;
					base.OnPropertyChanged(value, "IsKingsDecisionDone");
					if (value)
					{
						this.OnKingsDecisionDone();
					}
				}
			}
		}

		// Token: 0x04000683 RID: 1667
		private float _kingDecisionDoneTime = -1f;

		// Token: 0x04000684 RID: 1668
		private bool _isKingsDecisionDone;
	}
}
