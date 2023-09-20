using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Barter
{
	// Token: 0x02000170 RID: 368
	public class BarterTupleItemButtonWidget : ButtonWidget
	{
		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x060012C8 RID: 4808 RVA: 0x00033FB0 File Offset: 0x000321B0
		// (set) Token: 0x060012C9 RID: 4809 RVA: 0x00033FB8 File Offset: 0x000321B8
		public ListPanel SliderParentList { get; set; }

		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x060012CA RID: 4810 RVA: 0x00033FC1 File Offset: 0x000321C1
		// (set) Token: 0x060012CB RID: 4811 RVA: 0x00033FC9 File Offset: 0x000321C9
		public TextWidget CountText { get; set; }

		// Token: 0x060012CC RID: 4812 RVA: 0x00033FD2 File Offset: 0x000321D2
		public BarterTupleItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060012CD RID: 4813 RVA: 0x00033FDB File Offset: 0x000321DB
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.Refresh();
				this._initialized = true;
			}
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x00033FFC File Offset: 0x000321FC
		private void Refresh()
		{
			this.SliderParentList.IsVisible = this.IsMultiple && this.IsOffered;
			this.CountText.IsHidden = this.IsMultiple && this.IsOffered;
			base.IsSelected = this.IsOffered;
			base.DoNotAcceptEvents = this.IsOffered;
		}

		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x060012CF RID: 4815 RVA: 0x00034059 File Offset: 0x00032259
		// (set) Token: 0x060012D0 RID: 4816 RVA: 0x00034061 File Offset: 0x00032261
		[Editor(false)]
		public bool IsMultiple
		{
			get
			{
				return this._isMultiple;
			}
			set
			{
				if (this._isMultiple != value)
				{
					this._isMultiple = value;
					base.OnPropertyChanged(value, "IsMultiple");
					this.Refresh();
				}
			}
		}

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x060012D1 RID: 4817 RVA: 0x00034085 File Offset: 0x00032285
		// (set) Token: 0x060012D2 RID: 4818 RVA: 0x0003408D File Offset: 0x0003228D
		[Editor(false)]
		public bool IsOffered
		{
			get
			{
				return this._isOffered;
			}
			set
			{
				if (this._isOffered != value)
				{
					this._isOffered = value;
					base.OnPropertyChanged(value, "IsOffered");
					this.Refresh();
				}
			}
		}

		// Token: 0x0400089B RID: 2203
		private bool _initialized;

		// Token: 0x0400089C RID: 2204
		private bool _isMultiple;

		// Token: 0x0400089D RID: 2205
		private bool _isOffered;
	}
}
