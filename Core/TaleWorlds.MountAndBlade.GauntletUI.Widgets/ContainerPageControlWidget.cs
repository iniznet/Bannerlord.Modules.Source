using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ContainerPageControlWidget : Widget
	{
		public int PageCount { get; private set; }

		public event Action OnPageCountChanged;

		public ContainerPageControlWidget(UIContext context)
			: base(context)
		{
			this._nextPageClickedHandler = new Action<Widget>(this.NextPageClicked);
			this._previousPageClickedHandler = new Action<Widget>(this.PreviousPageClicked);
			this._onContainerChildRemovedHandler = new Action<Widget>(this.OnContainerChildRemoved);
			this._onContainerChildAddedHandler = new Action<Widget, Widget>(this.OnContainerChildAdded);
		}

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

		private void OnContainerChildAdded(Widget parentWidget, Widget addedWidget)
		{
			this._isInitialized = false;
		}

		private void OnContainerChildRemoved(Widget widget)
		{
			this._isInitialized = false;
		}

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

		private void UpdatePageText()
		{
			if (this.PageText != null)
			{
				this.PageText.Text = this._currentPageIndex + 1 + "/" + this.PageCount;
			}
		}

		protected virtual void OnInitialized()
		{
		}

		protected virtual void OnContainerItemsUpdated()
		{
		}

		protected void GoToPage(int index)
		{
			if (index >= 0 && index < this.PageCount && index != this._currentPageIndex)
			{
				this._currentPageIndex = index;
				this.UpdateContainerItems();
			}
		}

		public NavigationScopeTargeter PageButtonsContext { get; set; }

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

		private Action<Widget> _nextPageClickedHandler;

		private Action<Widget> _previousPageClickedHandler;

		private Action<Widget, Widget> _onContainerChildAddedHandler;

		private Action<Widget> _onContainerChildRemovedHandler;

		protected int _currentPageIndex;

		private bool _isInitialized;

		private int _itemPerPage;

		private bool _loopNavigation;

		private Container _container;

		private ButtonWidget _nextPageButton;

		private ButtonWidget _previousPageButton;

		private TextWidget _pageText;
	}
}
