using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000011 RID: 17
	public class ContainerPageControlWidget : Widget
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x000039A5 File Offset: 0x00001BA5
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x000039AD File Offset: 0x00001BAD
		public int PageCount { get; private set; }

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060000A8 RID: 168 RVA: 0x000039B8 File Offset: 0x00001BB8
		// (remove) Token: 0x060000A9 RID: 169 RVA: 0x000039F0 File Offset: 0x00001BF0
		public event Action OnPageCountChanged;

		// Token: 0x060000AA RID: 170 RVA: 0x00003A28 File Offset: 0x00001C28
		public ContainerPageControlWidget(UIContext context)
			: base(context)
		{
			this._nextPageClickedHandler = new Action<Widget>(this.NextPageClicked);
			this._previousPageClickedHandler = new Action<Widget>(this.PreviousPageClicked);
			this._onContainerChildRemovedHandler = new Action<Widget>(this.OnContainerChildRemoved);
			this._onContainerChildAddedHandler = new Action<Widget, Widget>(this.OnContainerChildAdded);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00003A84 File Offset: 0x00001C84
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._isInitialized)
			{
				int pageCount = this.PageCount;
				this.PageCount = MathF.Ceiling((float)this.Container.ChildCount / (float)this.ItemPerPage);
				if (pageCount != this.PageCount)
				{
					Action onPageCountChanged = this.OnPageCountChanged;
					if (onPageCountChanged != null)
					{
						onPageCountChanged();
					}
				}
				this._currentPageIndex = ((this.PageCount > 1) ? ((int)MathF.Clamp((float)this._currentPageIndex, 0f, (float)(this.PageCount - 1))) : 0);
				this.UpdateControlElements();
				this.UpdateContainerItems();
				this._isInitialized = true;
				this.OnInitialized();
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00003B28 File Offset: 0x00001D28
		private void NextPageClicked(Widget widget)
		{
			int num = this._currentPageIndex + 1;
			if (num >= this.PageCount)
			{
				num = (this.LoopNavigation ? 0 : (this.PageCount - 1));
			}
			if (num != this._currentPageIndex)
			{
				this._currentPageIndex = num;
				this.UpdateContainerItems();
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00003B74 File Offset: 0x00001D74
		private void PreviousPageClicked(Widget widget)
		{
			int num = this._currentPageIndex - 1;
			if (num < 0)
			{
				num = (this.LoopNavigation ? (this.PageCount - 1) : 0);
			}
			if (num != this._currentPageIndex)
			{
				this._currentPageIndex = num;
				this.UpdateContainerItems();
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00003BB8 File Offset: 0x00001DB8
		private void OnContainerChildAdded(Widget parentWidget, Widget addedWidget)
		{
			this._isInitialized = false;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00003BC1 File Offset: 0x00001DC1
		private void OnContainerChildRemoved(Widget widget)
		{
			this._isInitialized = false;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00003BCC File Offset: 0x00001DCC
		private void UpdateContainerItems()
		{
			int childCount = this.Container.ChildCount;
			int num = this._currentPageIndex * this.ItemPerPage;
			int num2 = (this._currentPageIndex + 1) * this.ItemPerPage;
			for (int i = 0; i < childCount; i++)
			{
				this.Container.GetChild(i).IsVisible = i >= num && i < num2;
			}
			this.UpdatePageText();
			this.OnContainerItemsUpdated();
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00003C38 File Offset: 0x00001E38
		private void UpdateControlElements()
		{
			if (this.NextPageButton != null)
			{
				this.NextPageButton.IsVisible = this.PageCount > 1;
			}
			if (this.PreviousPageButton != null)
			{
				this.PreviousPageButton.IsVisible = this.PageCount > 1;
			}
			if (this.PageText != null)
			{
				this.PageText.IsVisible = this.PageCount > 1;
			}
			if (this.PageButtonsContext != null)
			{
				this.PageButtonsContext.IsScopeEnabled = this.PageCount > 1;
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00003CB5 File Offset: 0x00001EB5
		private void UpdatePageText()
		{
			if (this.PageText != null)
			{
				this.PageText.Text = this._currentPageIndex + 1 + "/" + this.PageCount;
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00003CEC File Offset: 0x00001EEC
		protected virtual void OnInitialized()
		{
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00003CEE File Offset: 0x00001EEE
		protected virtual void OnContainerItemsUpdated()
		{
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00003CF0 File Offset: 0x00001EF0
		protected void GoToPage(int index)
		{
			if (index >= 0 && index < this.PageCount && index != this._currentPageIndex)
			{
				this._currentPageIndex = index;
				this.UpdateContainerItems();
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x00003D15 File Offset: 0x00001F15
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x00003D1D File Offset: 0x00001F1D
		public NavigationScopeTargeter PageButtonsContext { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x00003D26 File Offset: 0x00001F26
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x00003D2E File Offset: 0x00001F2E
		[Editor(false)]
		public int ItemPerPage
		{
			get
			{
				return this._itemPerPage;
			}
			set
			{
				if (this._itemPerPage != value)
				{
					this._itemPerPage = value;
					base.OnPropertyChanged(value, "ItemPerPage");
				}
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00003D4C File Offset: 0x00001F4C
		// (set) Token: 0x060000BB RID: 187 RVA: 0x00003D54 File Offset: 0x00001F54
		[Editor(false)]
		public bool LoopNavigation
		{
			get
			{
				return this._loopNavigation;
			}
			set
			{
				if (this._loopNavigation != value)
				{
					this._loopNavigation = value;
					base.OnPropertyChanged(value, "LoopNavigation");
				}
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00003D72 File Offset: 0x00001F72
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00003D7C File Offset: 0x00001F7C
		[Editor(false)]
		public Container Container
		{
			get
			{
				return this._container;
			}
			set
			{
				if (this._container != value)
				{
					Container container = this._container;
					if (container != null)
					{
						container.ItemAddEventHandlers.Remove(this._onContainerChildAddedHandler);
					}
					Container container2 = this._container;
					if (container2 != null)
					{
						container2.ItemAfterRemoveEventHandlers.Remove(this._onContainerChildRemovedHandler);
					}
					this._container = value;
					Container container3 = this._container;
					if (container3 != null)
					{
						container3.ItemAddEventHandlers.Add(this._onContainerChildAddedHandler);
					}
					Container container4 = this._container;
					if (container4 != null)
					{
						container4.ItemAfterRemoveEventHandlers.Add(this._onContainerChildRemovedHandler);
					}
					base.OnPropertyChanged<Container>(value, "Container");
				}
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00003E1A File Offset: 0x0000201A
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00003E24 File Offset: 0x00002024
		[Editor(false)]
		public ButtonWidget NextPageButton
		{
			get
			{
				return this._nextPageButton;
			}
			set
			{
				if (this._nextPageButton != value)
				{
					ButtonWidget nextPageButton = this._nextPageButton;
					if (nextPageButton != null)
					{
						nextPageButton.ClickEventHandlers.Remove(this._nextPageClickedHandler);
					}
					this._nextPageButton = value;
					ButtonWidget nextPageButton2 = this._nextPageButton;
					if (nextPageButton2 != null)
					{
						nextPageButton2.ClickEventHandlers.Add(this._nextPageClickedHandler);
					}
					base.OnPropertyChanged<ButtonWidget>(value, "NextPageButton");
				}
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00003E86 File Offset: 0x00002086
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x00003E90 File Offset: 0x00002090
		[Editor(false)]
		public ButtonWidget PreviousPageButton
		{
			get
			{
				return this._previousPageButton;
			}
			set
			{
				if (this._previousPageButton != value)
				{
					ButtonWidget previousPageButton = this._previousPageButton;
					if (previousPageButton != null)
					{
						previousPageButton.ClickEventHandlers.Remove(this._previousPageClickedHandler);
					}
					this._previousPageButton = value;
					ButtonWidget previousPageButton2 = this._previousPageButton;
					if (previousPageButton2 != null)
					{
						previousPageButton2.ClickEventHandlers.Add(this._previousPageClickedHandler);
					}
					base.OnPropertyChanged<ButtonWidget>(value, "PreviousPageButton");
				}
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x00003EF2 File Offset: 0x000020F2
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x00003EFA File Offset: 0x000020FA
		[Editor(false)]
		public TextWidget PageText
		{
			get
			{
				return this._pageText;
			}
			set
			{
				if (this._pageText != value)
				{
					this._pageText = value;
					base.OnPropertyChanged<TextWidget>(value, "PageText");
				}
			}
		}

		// Token: 0x04000050 RID: 80
		private Action<Widget> _nextPageClickedHandler;

		// Token: 0x04000051 RID: 81
		private Action<Widget> _previousPageClickedHandler;

		// Token: 0x04000052 RID: 82
		private Action<Widget, Widget> _onContainerChildAddedHandler;

		// Token: 0x04000053 RID: 83
		private Action<Widget> _onContainerChildRemovedHandler;

		// Token: 0x04000055 RID: 85
		protected int _currentPageIndex;

		// Token: 0x04000056 RID: 86
		private bool _isInitialized;

		// Token: 0x04000059 RID: 89
		private int _itemPerPage;

		// Token: 0x0400005A RID: 90
		private bool _loopNavigation;

		// Token: 0x0400005B RID: 91
		private Container _container;

		// Token: 0x0400005C RID: 92
		private ButtonWidget _nextPageButton;

		// Token: 0x0400005D RID: 93
		private ButtonWidget _previousPageButton;

		// Token: 0x0400005E RID: 94
		private TextWidget _pageText;
	}
}
