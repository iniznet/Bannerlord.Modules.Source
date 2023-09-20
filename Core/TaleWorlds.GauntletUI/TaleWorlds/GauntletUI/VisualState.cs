using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000030 RID: 48
	public class VisualState
	{
		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000335 RID: 821 RVA: 0x0000E845 File Offset: 0x0000CA45
		// (set) Token: 0x06000336 RID: 822 RVA: 0x0000E84D File Offset: 0x0000CA4D
		public string State { get; private set; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000337 RID: 823 RVA: 0x0000E856 File Offset: 0x0000CA56
		// (set) Token: 0x06000338 RID: 824 RVA: 0x0000E85E File Offset: 0x0000CA5E
		public float TransitionDuration
		{
			get
			{
				return this._transitionDuration;
			}
			set
			{
				this._transitionDuration = value;
				this.GotTransitionDuration = true;
			}
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000339 RID: 825 RVA: 0x0000E86E File Offset: 0x0000CA6E
		// (set) Token: 0x0600033A RID: 826 RVA: 0x0000E876 File Offset: 0x0000CA76
		public float PositionXOffset
		{
			get
			{
				return this._positionXOffset;
			}
			set
			{
				this._positionXOffset = value;
				this.GotPositionXOffset = true;
			}
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x0600033B RID: 827 RVA: 0x0000E886 File Offset: 0x0000CA86
		// (set) Token: 0x0600033C RID: 828 RVA: 0x0000E88E File Offset: 0x0000CA8E
		public float PositionYOffset
		{
			get
			{
				return this._positionYOffset;
			}
			set
			{
				this._positionYOffset = value;
				this.GotPositionYOffset = true;
			}
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x0600033D RID: 829 RVA: 0x0000E89E File Offset: 0x0000CA9E
		// (set) Token: 0x0600033E RID: 830 RVA: 0x0000E8A6 File Offset: 0x0000CAA6
		public float SuggestedWidth
		{
			get
			{
				return this._suggestedWidth;
			}
			set
			{
				this._suggestedWidth = value;
				this.GotSuggestedWidth = true;
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x0600033F RID: 831 RVA: 0x0000E8B6 File Offset: 0x0000CAB6
		// (set) Token: 0x06000340 RID: 832 RVA: 0x0000E8BE File Offset: 0x0000CABE
		public float SuggestedHeight
		{
			get
			{
				return this._suggestedHeight;
			}
			set
			{
				this._suggestedHeight = value;
				this.GotSuggestedHeight = true;
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000341 RID: 833 RVA: 0x0000E8CE File Offset: 0x0000CACE
		// (set) Token: 0x06000342 RID: 834 RVA: 0x0000E8D6 File Offset: 0x0000CAD6
		public float MarginTop
		{
			get
			{
				return this._marginTop;
			}
			set
			{
				this._marginTop = value;
				this.GotMarginTop = true;
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000343 RID: 835 RVA: 0x0000E8E6 File Offset: 0x0000CAE6
		// (set) Token: 0x06000344 RID: 836 RVA: 0x0000E8EE File Offset: 0x0000CAEE
		public float MarginBottom
		{
			get
			{
				return this._marginBottom;
			}
			set
			{
				this._marginBottom = value;
				this.GotMarginBottom = true;
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000345 RID: 837 RVA: 0x0000E8FE File Offset: 0x0000CAFE
		// (set) Token: 0x06000346 RID: 838 RVA: 0x0000E906 File Offset: 0x0000CB06
		public float MarginLeft
		{
			get
			{
				return this._marginLeft;
			}
			set
			{
				this._marginLeft = value;
				this.GotMarginLeft = true;
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000347 RID: 839 RVA: 0x0000E916 File Offset: 0x0000CB16
		// (set) Token: 0x06000348 RID: 840 RVA: 0x0000E91E File Offset: 0x0000CB1E
		public float MarginRight
		{
			get
			{
				return this._marginRight;
			}
			set
			{
				this._marginRight = value;
				this.GotMarginRight = true;
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000349 RID: 841 RVA: 0x0000E92E File Offset: 0x0000CB2E
		// (set) Token: 0x0600034A RID: 842 RVA: 0x0000E936 File Offset: 0x0000CB36
		public bool GotTransitionDuration { get; private set; }

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x0600034B RID: 843 RVA: 0x0000E93F File Offset: 0x0000CB3F
		// (set) Token: 0x0600034C RID: 844 RVA: 0x0000E947 File Offset: 0x0000CB47
		public bool GotPositionXOffset { get; private set; }

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x0600034D RID: 845 RVA: 0x0000E950 File Offset: 0x0000CB50
		// (set) Token: 0x0600034E RID: 846 RVA: 0x0000E958 File Offset: 0x0000CB58
		public bool GotPositionYOffset { get; private set; }

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x0600034F RID: 847 RVA: 0x0000E961 File Offset: 0x0000CB61
		// (set) Token: 0x06000350 RID: 848 RVA: 0x0000E969 File Offset: 0x0000CB69
		public bool GotSuggestedWidth { get; private set; }

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000351 RID: 849 RVA: 0x0000E972 File Offset: 0x0000CB72
		// (set) Token: 0x06000352 RID: 850 RVA: 0x0000E97A File Offset: 0x0000CB7A
		public bool GotSuggestedHeight { get; private set; }

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000353 RID: 851 RVA: 0x0000E983 File Offset: 0x0000CB83
		// (set) Token: 0x06000354 RID: 852 RVA: 0x0000E98B File Offset: 0x0000CB8B
		public bool GotMarginTop { get; private set; }

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000355 RID: 853 RVA: 0x0000E994 File Offset: 0x0000CB94
		// (set) Token: 0x06000356 RID: 854 RVA: 0x0000E99C File Offset: 0x0000CB9C
		public bool GotMarginBottom { get; private set; }

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000357 RID: 855 RVA: 0x0000E9A5 File Offset: 0x0000CBA5
		// (set) Token: 0x06000358 RID: 856 RVA: 0x0000E9AD File Offset: 0x0000CBAD
		public bool GotMarginLeft { get; private set; }

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000359 RID: 857 RVA: 0x0000E9B6 File Offset: 0x0000CBB6
		// (set) Token: 0x0600035A RID: 858 RVA: 0x0000E9BE File Offset: 0x0000CBBE
		public bool GotMarginRight { get; private set; }

		// Token: 0x0600035B RID: 859 RVA: 0x0000E9C7 File Offset: 0x0000CBC7
		public VisualState(string state)
		{
			this.State = state;
		}

		// Token: 0x0600035C RID: 860 RVA: 0x0000E9D8 File Offset: 0x0000CBD8
		public void FillFromWidget(Widget widget)
		{
			this.PositionXOffset = widget.PositionXOffset;
			this.PositionYOffset = widget.PositionYOffset;
			this.SuggestedWidth = widget.SuggestedWidth;
			this.SuggestedHeight = widget.SuggestedHeight;
			this.MarginTop = widget.MarginTop;
			this.MarginBottom = widget.MarginBottom;
			this.MarginLeft = widget.MarginLeft;
			this.MarginRight = widget.MarginRight;
		}

		// Token: 0x040001A5 RID: 421
		private float _transitionDuration;

		// Token: 0x040001A6 RID: 422
		private float _positionXOffset;

		// Token: 0x040001A7 RID: 423
		private float _positionYOffset;

		// Token: 0x040001A8 RID: 424
		private float _suggestedWidth;

		// Token: 0x040001A9 RID: 425
		private float _suggestedHeight;

		// Token: 0x040001AA RID: 426
		private float _marginTop;

		// Token: 0x040001AB RID: 427
		private float _marginBottom;

		// Token: 0x040001AC RID: 428
		private float _marginLeft;

		// Token: 0x040001AD RID: 429
		private float _marginRight;
	}
}
