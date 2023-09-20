using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000036 RID: 54
	public class SettlementStatTextWidget : RichTextWidget
	{
		// Token: 0x06000306 RID: 774 RVA: 0x00009CD1 File Offset: 0x00007ED1
		public SettlementStatTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00009CDC File Offset: 0x00007EDC
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			switch (this._state)
			{
			case SettlementStatTextWidget.State.Idle:
				if (this.IsWarning)
				{
					this.SetState("Warning");
				}
				else
				{
					this.SetState("Default");
				}
				this._state = SettlementStatTextWidget.State.Start;
				return;
			case SettlementStatTextWidget.State.Start:
				this._state = ((base.BrushRenderer.Brush != null) ? SettlementStatTextWidget.State.Playing : SettlementStatTextWidget.State.Start);
				return;
			case SettlementStatTextWidget.State.Playing:
				base.BrushRenderer.RestartAnimation();
				this._state = SettlementStatTextWidget.State.End;
				break;
			case SettlementStatTextWidget.State.End:
				break;
			default:
				return;
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000308 RID: 776 RVA: 0x00009D60 File Offset: 0x00007F60
		// (set) Token: 0x06000309 RID: 777 RVA: 0x00009D68 File Offset: 0x00007F68
		[Editor(false)]
		public bool IsWarning
		{
			get
			{
				return this._isWarning;
			}
			set
			{
				if (value != this._isWarning)
				{
					this._isWarning = value;
					base.OnPropertyChanged(value, "IsWarning");
					this._state = SettlementStatTextWidget.State.Idle;
				}
			}
		}

		// Token: 0x04000140 RID: 320
		private SettlementStatTextWidget.State _state;

		// Token: 0x04000141 RID: 321
		private bool _isWarning;

		// Token: 0x0200017B RID: 379
		public enum State
		{
			// Token: 0x040008BA RID: 2234
			Idle,
			// Token: 0x040008BB RID: 2235
			Start,
			// Token: 0x040008BC RID: 2236
			Playing,
			// Token: 0x040008BD RID: 2237
			End
		}
	}
}
