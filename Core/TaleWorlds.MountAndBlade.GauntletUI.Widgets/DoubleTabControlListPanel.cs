using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000016 RID: 22
	public class DoubleTabControlListPanel : ListPanel
	{
		// Token: 0x06000109 RID: 265 RVA: 0x00004BF8 File Offset: 0x00002DF8
		public DoubleTabControlListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00004C01 File Offset: 0x00002E01
		public void OnFirstTabClick(Widget widget)
		{
			if (!this._firstList.IsVisible && this._secondList.IsVisible)
			{
				this._secondList.IsVisible = false;
				this._firstList.IsVisible = true;
			}
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00004C35 File Offset: 0x00002E35
		public void OnSecondTabClick(Widget widget)
		{
			if (this._firstList.IsVisible && !this._secondList.IsVisible)
			{
				this._secondList.IsVisible = true;
				this._firstList.IsVisible = false;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600010C RID: 268 RVA: 0x00004C69 File Offset: 0x00002E69
		// (set) Token: 0x0600010D RID: 269 RVA: 0x00004C74 File Offset: 0x00002E74
		[Editor(false)]
		public ButtonWidget FirstListButton
		{
			get
			{
				return this._firstListButton;
			}
			set
			{
				if (this._firstListButton != value)
				{
					this._firstListButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "FirstListButton");
					if (this.FirstListButton != null && !this.FirstListButton.ClickEventHandlers.Contains(new Action<Widget>(this.OnFirstTabClick)))
					{
						this.FirstListButton.ClickEventHandlers.Add(new Action<Widget>(this.OnFirstTabClick));
					}
				}
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600010E RID: 270 RVA: 0x00004CDF File Offset: 0x00002EDF
		// (set) Token: 0x0600010F RID: 271 RVA: 0x00004CE8 File Offset: 0x00002EE8
		[Editor(false)]
		public ButtonWidget SecondListButton
		{
			get
			{
				return this._secondListButton;
			}
			set
			{
				if (this._secondListButton != value)
				{
					this._secondListButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "SecondListButton");
					if (this.SecondListButton != null && !this.SecondListButton.ClickEventHandlers.Contains(new Action<Widget>(this.OnSecondTabClick)))
					{
						this.SecondListButton.ClickEventHandlers.Add(new Action<Widget>(this.OnSecondTabClick));
					}
				}
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000110 RID: 272 RVA: 0x00004D53 File Offset: 0x00002F53
		// (set) Token: 0x06000111 RID: 273 RVA: 0x00004D5B File Offset: 0x00002F5B
		[Editor(false)]
		public Widget FirstList
		{
			get
			{
				return this._firstList;
			}
			set
			{
				if (this._firstList != value)
				{
					this._firstList = value;
					base.OnPropertyChanged<Widget>(value, "FirstList");
				}
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000112 RID: 274 RVA: 0x00004D79 File Offset: 0x00002F79
		// (set) Token: 0x06000113 RID: 275 RVA: 0x00004D81 File Offset: 0x00002F81
		[Editor(false)]
		public Widget SecondList
		{
			get
			{
				return this._secondList;
			}
			set
			{
				if (this._secondList != value)
				{
					this._secondList = value;
					base.OnPropertyChanged<Widget>(value, "SecondList");
				}
			}
		}

		// Token: 0x04000082 RID: 130
		private ButtonWidget _firstListButton;

		// Token: 0x04000083 RID: 131
		private ButtonWidget _secondListButton;

		// Token: 0x04000084 RID: 132
		private Widget _firstList;

		// Token: 0x04000085 RID: 133
		private Widget _secondList;
	}
}
