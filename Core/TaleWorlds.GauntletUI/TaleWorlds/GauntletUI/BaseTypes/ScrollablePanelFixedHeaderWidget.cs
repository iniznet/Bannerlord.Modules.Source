using System;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000064 RID: 100
	public class ScrollablePanelFixedHeaderWidget : Widget
	{
		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x0600066C RID: 1644 RVA: 0x0001CE5B File Offset: 0x0001B05B
		// (set) Token: 0x0600066D RID: 1645 RVA: 0x0001CE63 File Offset: 0x0001B063
		public Widget FixedHeader { get; set; }

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x0600066E RID: 1646 RVA: 0x0001CE6C File Offset: 0x0001B06C
		// (set) Token: 0x0600066F RID: 1647 RVA: 0x0001CE74 File Offset: 0x0001B074
		public float TopOffset { get; set; }

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000670 RID: 1648 RVA: 0x0001CE7D File Offset: 0x0001B07D
		// (set) Token: 0x06000671 RID: 1649 RVA: 0x0001CE85 File Offset: 0x0001B085
		public float BottomOffset { get; set; } = float.MinValue;

		// Token: 0x06000672 RID: 1650 RVA: 0x0001CE8E File Offset: 0x0001B08E
		public ScrollablePanelFixedHeaderWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x0001CEA9 File Offset: 0x0001B0A9
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isDirty)
			{
				base.EventFired("FixedHeaderPropertyChanged", Array.Empty<object>());
				this._isDirty = false;
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000674 RID: 1652 RVA: 0x0001CED1 File Offset: 0x0001B0D1
		// (set) Token: 0x06000675 RID: 1653 RVA: 0x0001CED9 File Offset: 0x0001B0D9
		public float HeaderHeight
		{
			get
			{
				return this._headerHeight;
			}
			set
			{
				if (value != this._headerHeight)
				{
					this._headerHeight = value;
					base.SuggestedHeight = this._headerHeight;
					this._isDirty = true;
				}
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000676 RID: 1654 RVA: 0x0001CEFE File Offset: 0x0001B0FE
		// (set) Token: 0x06000677 RID: 1655 RVA: 0x0001CF06 File Offset: 0x0001B106
		public float AdditionalTopOffset
		{
			get
			{
				return this._additionalTopOffset;
			}
			set
			{
				if (value != this._additionalTopOffset)
				{
					this._additionalTopOffset = value;
					this._isDirty = true;
				}
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000678 RID: 1656 RVA: 0x0001CF1F File Offset: 0x0001B11F
		// (set) Token: 0x06000679 RID: 1657 RVA: 0x0001CF27 File Offset: 0x0001B127
		public float AdditionalBottomOffset
		{
			get
			{
				return this._additionalBottomOffset;
			}
			set
			{
				if (value != this._additionalBottomOffset)
				{
					this._additionalBottomOffset = value;
					this._isDirty = true;
				}
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x0600067A RID: 1658 RVA: 0x0001CF40 File Offset: 0x0001B140
		// (set) Token: 0x0600067B RID: 1659 RVA: 0x0001CF48 File Offset: 0x0001B148
		[Editor(false)]
		public bool IsRelevant
		{
			get
			{
				return this._isRelevant;
			}
			set
			{
				if (value != this._isRelevant)
				{
					this._isRelevant = value;
					base.IsVisible = value;
					this._isDirty = true;
					base.OnPropertyChanged(value, "IsRelevant");
				}
			}
		}

		// Token: 0x0400030D RID: 781
		private bool _isDirty;

		// Token: 0x04000311 RID: 785
		private float _headerHeight;

		// Token: 0x04000312 RID: 786
		private float _additionalTopOffset;

		// Token: 0x04000313 RID: 787
		private float _additionalBottomOffset;

		// Token: 0x04000314 RID: 788
		private bool _isRelevant = true;
	}
}
