using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	public class KingdomDecisionPopupWidget : Widget
	{
		public int DelayAfterKingsDecision { get; set; } = 5;

		public KingdomDecisionPopupWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._kingDecisionDoneTime != -1f && base.EventManager.Time - this._kingDecisionDoneTime > (float)this.DelayAfterKingsDecision)
			{
				this.ExecuteFinalDone();
			}
		}

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

		private float _kingDecisionDoneTime = -1f;

		private bool _isKingsDecisionDone;
	}
}
