using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.EscapeMenu
{
	// Token: 0x02000134 RID: 308
	public class EscapeMenuButtonWidget : ButtonWidget
	{
		// Token: 0x06001060 RID: 4192 RVA: 0x0002E107 File Offset: 0x0002C307
		public EscapeMenuButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001061 RID: 4193 RVA: 0x0002E110 File Offset: 0x0002C310
		private void PositiveBehavioredStateUpdated()
		{
			if (this.IsPositiveBehaviored)
			{
				base.Brush = this.PositiveBehaviorBrush;
			}
		}

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x06001062 RID: 4194 RVA: 0x0002E126 File Offset: 0x0002C326
		// (set) Token: 0x06001063 RID: 4195 RVA: 0x0002E12E File Offset: 0x0002C32E
		[Editor(false)]
		public bool IsPositiveBehaviored
		{
			get
			{
				return this._isPositiveBehaviored;
			}
			set
			{
				if (this._isPositiveBehaviored != value)
				{
					this._isPositiveBehaviored = value;
					base.OnPropertyChanged(value, "IsPositiveBehaviored");
					this.PositiveBehavioredStateUpdated();
				}
			}
		}

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x06001064 RID: 4196 RVA: 0x0002E152 File Offset: 0x0002C352
		// (set) Token: 0x06001065 RID: 4197 RVA: 0x0002E15A File Offset: 0x0002C35A
		[Editor(false)]
		public Brush PositiveBehaviorBrush
		{
			get
			{
				return this._positiveBehaviorBrush;
			}
			set
			{
				if (this._positiveBehaviorBrush != value)
				{
					this._positiveBehaviorBrush = value;
					base.OnPropertyChanged<Brush>(value, "PositiveBehaviorBrush");
				}
			}
		}

		// Token: 0x0400078D RID: 1933
		private bool _isPositiveBehaviored;

		// Token: 0x0400078E RID: 1934
		private Brush _positiveBehaviorBrush;
	}
}
