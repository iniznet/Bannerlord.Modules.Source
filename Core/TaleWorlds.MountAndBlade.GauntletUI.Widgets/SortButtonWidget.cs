using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000038 RID: 56
	public class SortButtonWidget : ButtonWidget
	{
		// Token: 0x06000312 RID: 786 RVA: 0x00009F22 File Offset: 0x00008122
		public SortButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000313 RID: 787 RVA: 0x00009F2C File Offset: 0x0000812C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.SortVisualWidget != null)
			{
				switch (this.SortState)
				{
				case 0:
					this.SortVisualWidget.SetState("Default");
					return;
				case 1:
					this.SortVisualWidget.SetState("Ascending");
					return;
				case 2:
					this.SortVisualWidget.SetState("Descending");
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000314 RID: 788 RVA: 0x00009F94 File Offset: 0x00008194
		// (set) Token: 0x06000315 RID: 789 RVA: 0x00009F9C File Offset: 0x0000819C
		[Editor(false)]
		public int SortState
		{
			get
			{
				return this._sortState;
			}
			set
			{
				if (this._sortState != value)
				{
					this._sortState = value;
					base.OnPropertyChanged(value, "SortState");
				}
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06000316 RID: 790 RVA: 0x00009FBA File Offset: 0x000081BA
		// (set) Token: 0x06000317 RID: 791 RVA: 0x00009FC2 File Offset: 0x000081C2
		[Editor(false)]
		public BrushWidget SortVisualWidget
		{
			get
			{
				return this._sortVisualWidget;
			}
			set
			{
				if (this._sortVisualWidget != value)
				{
					this._sortVisualWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "SortVisualWidget");
				}
			}
		}

		// Token: 0x04000146 RID: 326
		private int _sortState;

		// Token: 0x04000147 RID: 327
		private BrushWidget _sortVisualWidget;
	}
}
