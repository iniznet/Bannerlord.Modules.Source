using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	// Token: 0x02000012 RID: 18
	public class ValueBasedVisibilityWidget : Widget
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000105 RID: 261 RVA: 0x000063DD File Offset: 0x000045DD
		// (set) Token: 0x06000106 RID: 262 RVA: 0x000063E5 File Offset: 0x000045E5
		public ValueBasedVisibilityWidget.WatchTypes WatchType { get; set; }

		// Token: 0x06000107 RID: 263 RVA: 0x000063EE File Offset: 0x000045EE
		public ValueBasedVisibilityWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000108 RID: 264 RVA: 0x00006409 File Offset: 0x00004609
		// (set) Token: 0x06000109 RID: 265 RVA: 0x00006414 File Offset: 0x00004614
		[Editor(false)]
		public int IndexToWatch
		{
			get
			{
				return this._indexToWatch;
			}
			set
			{
				if (this._indexToWatch != value)
				{
					this._indexToWatch = value;
					base.OnPropertyChanged(value, "IndexToWatch");
					switch (this.WatchType)
					{
					case ValueBasedVisibilityWidget.WatchTypes.Equal:
						base.IsVisible = value == this.IndexToBeVisible;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.BiggerThan:
						base.IsVisible = value > this.IndexToBeVisible;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.BiggerThanEqual:
						base.IsVisible = value >= this.IndexToBeVisible;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.LessThan:
						base.IsVisible = value < this.IndexToBeVisible;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.LessThanEqual:
						base.IsVisible = value <= this.IndexToBeVisible;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.NotEqual:
						base.IsVisible = value != this.IndexToBeVisible;
						break;
					default:
						return;
					}
				}
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600010A RID: 266 RVA: 0x000064CE File Offset: 0x000046CE
		// (set) Token: 0x0600010B RID: 267 RVA: 0x000064D8 File Offset: 0x000046D8
		[Editor(false)]
		public float IndexToWatchFloat
		{
			get
			{
				return this._indexToWatchFloat;
			}
			set
			{
				if (this._indexToWatchFloat != value)
				{
					this._indexToWatchFloat = value;
					base.OnPropertyChanged(value, "IndexToWatchFloat");
					switch (this.WatchType)
					{
					case ValueBasedVisibilityWidget.WatchTypes.Equal:
						base.IsVisible = value == this.IndexToBeVisibleFloat;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.BiggerThan:
						base.IsVisible = value > this.IndexToBeVisibleFloat;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.BiggerThanEqual:
						base.IsVisible = value >= this.IndexToBeVisibleFloat;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.LessThan:
						base.IsVisible = value < this.IndexToBeVisibleFloat;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.LessThanEqual:
						base.IsVisible = value <= this.IndexToBeVisibleFloat;
						return;
					case ValueBasedVisibilityWidget.WatchTypes.NotEqual:
						base.IsVisible = value != this.IndexToBeVisibleFloat;
						break;
					default:
						return;
					}
				}
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x0600010C RID: 268 RVA: 0x00006592 File Offset: 0x00004792
		// (set) Token: 0x0600010D RID: 269 RVA: 0x0000659A File Offset: 0x0000479A
		[Editor(false)]
		public int IndexToBeVisible
		{
			get
			{
				return this._indexToBeVisible;
			}
			set
			{
				if (this._indexToBeVisible != value)
				{
					this._indexToBeVisible = value;
					base.OnPropertyChanged(value, "IndexToBeVisible");
				}
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600010E RID: 270 RVA: 0x000065B8 File Offset: 0x000047B8
		// (set) Token: 0x0600010F RID: 271 RVA: 0x000065C0 File Offset: 0x000047C0
		[Editor(false)]
		public float IndexToBeVisibleFloat
		{
			get
			{
				return this._indexToBeVisibleFloat;
			}
			set
			{
				if (this._indexToBeVisibleFloat != value)
				{
					this._indexToBeVisibleFloat = value;
					base.OnPropertyChanged(value, "IndexToBeVisibleFloat");
				}
			}
		}

		// Token: 0x0400007A RID: 122
		private int _indexToBeVisible;

		// Token: 0x0400007B RID: 123
		private int _indexToWatch = -1;

		// Token: 0x0400007C RID: 124
		private float _indexToBeVisibleFloat;

		// Token: 0x0400007D RID: 125
		private float _indexToWatchFloat = -1f;

		// Token: 0x02000019 RID: 25
		public enum WatchTypes
		{
			// Token: 0x040000A9 RID: 169
			Equal,
			// Token: 0x040000AA RID: 170
			BiggerThan,
			// Token: 0x040000AB RID: 171
			BiggerThanEqual,
			// Token: 0x040000AC RID: 172
			LessThan,
			// Token: 0x040000AD RID: 173
			LessThanEqual,
			// Token: 0x040000AE RID: 174
			NotEqual
		}
	}
}
