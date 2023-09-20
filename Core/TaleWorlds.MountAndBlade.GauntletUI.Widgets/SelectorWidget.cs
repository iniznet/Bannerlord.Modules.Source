using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class SelectorWidget : Widget
	{
		public SelectorWidget(UIContext context)
			: base(context)
		{
			this._listSelectionHandler = new Action<Widget>(this.OnSelectionChanged);
			this._listItemRemovedHandler = new Action<Widget, Widget>(this.OnListChanged);
			this._listItemAddedHandler = new Action<Widget, Widget>(this.OnListChanged);
		}

		public void OnListChanged(Widget widget)
		{
			this.RefreshSelectedItem();
		}

		public void OnListChanged(Widget parentWidget, Widget addedWidget)
		{
			this.RefreshSelectedItem();
		}

		public void OnSelectionChanged(Widget widget)
		{
			this.CurrentSelectedIndex = this.ListPanelValue;
			this.RefreshSelectedItem();
			base.OnPropertyChanged(this.CurrentSelectedIndex, "CurrentSelectedIndex");
		}

		private void RefreshSelectedItem()
		{
			this.ListPanelValue = this.CurrentSelectedIndex;
		}

		[Editor(false)]
		public int ListPanelValue
		{
			get
			{
				if (this.Container != null)
				{
					return this.Container.IntValue;
				}
				return -1;
			}
			set
			{
				if (this.Container != null && this.Container.IntValue != value)
				{
					this.Container.IntValue = value;
				}
			}
		}

		[Editor(false)]
		public int CurrentSelectedIndex
		{
			get
			{
				return this._currentSelectedIndex;
			}
			set
			{
				if (this._currentSelectedIndex != value && value >= 0)
				{
					this._currentSelectedIndex = value;
					this.RefreshSelectedItem();
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
				if (this._container != null)
				{
					this._container.SelectEventHandlers.Remove(this._listSelectionHandler);
					this._container.ItemAddEventHandlers.Remove(this._listItemAddedHandler);
					this._container.ItemRemoveEventHandlers.Remove(this._listItemRemovedHandler);
				}
				this._container = value;
				if (this._container != null)
				{
					this._container.SelectEventHandlers.Add(this._listSelectionHandler);
					this._container.ItemAddEventHandlers.Add(this._listItemAddedHandler);
					this._container.ItemRemoveEventHandlers.Add(this._listItemRemovedHandler);
				}
				this.RefreshSelectedItem();
			}
		}

		private int _currentSelectedIndex;

		private Action<Widget> _listSelectionHandler;

		private Action<Widget, Widget> _listItemRemovedHandler;

		private Action<Widget, Widget> _listItemAddedHandler;

		private Container _container;
	}
}
