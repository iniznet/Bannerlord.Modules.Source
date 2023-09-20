using System;
using System.Collections.Generic;
using System.Numerics;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public abstract class Container : Widget
	{
		public ContainerItemDescription DefaultItemDescription { get; private set; }

		public abstract Predicate<Widget> AcceptDropPredicate { get; set; }

		public abstract Vector2 GetDropGizmoPosition(Vector2 draggedWidgetPosition);

		public abstract int GetIndexForDrop(Vector2 draggedWidgetPosition);

		public int IntValue
		{
			get
			{
				if (this._intValue >= base.ChildCount)
				{
					this._intValue = -1;
				}
				return this._intValue;
			}
			set
			{
				if (!this._currentlyChangingIntValue)
				{
					this._currentlyChangingIntValue = true;
					if (value != this._intValue && value < base.ChildCount)
					{
						this._intValue = value;
						this.UpdateSelected();
						foreach (Action<Widget> action in this.SelectEventHandlers)
						{
							action(this);
						}
						base.EventFired("SelectedItemChange", Array.Empty<object>());
						base.OnPropertyChanged(value, "IntValue");
					}
					this._currentlyChangingIntValue = false;
				}
			}
		}

		public abstract bool IsDragHovering { get; }

		public int DragHoverInsertionIndex
		{
			get
			{
				return this._dragHoverInsertionIndex;
			}
			set
			{
				if (this._dragHoverInsertionIndex != value)
				{
					this._dragHoverInsertionIndex = value;
					base.SetMeasureAndLayoutDirty();
				}
			}
		}

		protected Container(UIContext context)
			: base(context)
		{
			this.DefaultItemDescription = new ContainerItemDescription();
			this._itemDescriptions = new List<ContainerItemDescription>();
		}

		private void UpdateSelected()
		{
			for (int i = 0; i < base.ChildCount; i++)
			{
				ButtonWidget buttonWidget = base.GetChild(i) as ButtonWidget;
				if (buttonWidget != null)
				{
					bool flag = i == this.IntValue;
					buttonWidget.IsSelected = flag;
				}
			}
		}

		protected internal override bool OnDrop()
		{
			if (base.AcceptDrop)
			{
				bool flag = true;
				if (this.AcceptDropHandler != null)
				{
					flag = this.AcceptDropHandler(this, base.EventManager.DraggedWidget);
				}
				if (flag)
				{
					Widget widget = base.EventManager.ReleaseDraggedWidget();
					int indexForDrop = this.GetIndexForDrop(base.EventManager.DraggedWidgetPosition);
					if (!base.DropEventHandledManually)
					{
						widget.ParentWidget = this;
						widget.SetSiblingIndex(indexForDrop, false);
					}
					base.EventFired("Drop", new object[] { widget, indexForDrop });
					return true;
				}
			}
			return false;
		}

		public abstract void OnChildSelected(Widget widget);

		public ContainerItemDescription GetItemDescription(string id, int index)
		{
			bool flag = !string.IsNullOrEmpty(id);
			ContainerItemDescription containerItemDescription = null;
			ContainerItemDescription containerItemDescription2 = null;
			for (int i = 0; i < this._itemDescriptions.Count; i++)
			{
				ContainerItemDescription containerItemDescription3 = this._itemDescriptions[i];
				if (flag && containerItemDescription3.WidgetId == id)
				{
					containerItemDescription = containerItemDescription3;
				}
				if (index == containerItemDescription3.WidgetIndex)
				{
					containerItemDescription2 = containerItemDescription3;
				}
			}
			ContainerItemDescription containerItemDescription4;
			if ((containerItemDescription4 = containerItemDescription) == null)
			{
				containerItemDescription4 = containerItemDescription2 ?? this.DefaultItemDescription;
			}
			return containerItemDescription4;
		}

		protected override void OnChildAdded(Widget child)
		{
			foreach (Action<Widget, Widget> action in this.ItemAddEventHandlers)
			{
				action(this, child);
			}
			base.OnChildAdded(child);
		}

		protected override void OnChildRemoved(Widget child)
		{
			int childIndex = base.GetChildIndex(child);
			if (base.ChildCount == 1)
			{
				this.IntValue = -1;
			}
			else if (childIndex < this.IntValue && this.IntValue > 0)
			{
				this.IntValue--;
			}
			foreach (Action<Widget, Widget> action in this.ItemRemoveEventHandlers)
			{
				action(this, child);
			}
			base.OnChildRemoved(child);
		}

		protected override void OnAfterChildRemoved(Widget child)
		{
			foreach (Action<Widget> action in this.ItemAfterRemoveEventHandlers)
			{
				action(this);
			}
			base.OnAfterChildRemoved(child);
		}

		public void AddItemDescription(ContainerItemDescription itemDescription)
		{
			this._itemDescriptions.Add(itemDescription);
		}

		public ScrollablePanel FindParentPanel()
		{
			for (Widget widget = base.ParentWidget; widget != null; widget = widget.ParentWidget)
			{
				ScrollablePanel scrollablePanel;
				if ((scrollablePanel = widget as ScrollablePanel) != null)
				{
					return scrollablePanel;
				}
			}
			return null;
		}

		public List<Action<Widget>> SelectEventHandlers = new List<Action<Widget>>();

		public List<Action<Widget, Widget>> ItemAddEventHandlers = new List<Action<Widget, Widget>>();

		public List<Action<Widget, Widget>> ItemRemoveEventHandlers = new List<Action<Widget, Widget>>();

		public List<Action<Widget>> ItemAfterRemoveEventHandlers = new List<Action<Widget>>();

		private int _intValue = -1;

		private bool _currentlyChangingIntValue;

		public bool ShowSelection;

		private int _dragHoverInsertionIndex;

		private List<ContainerItemDescription> _itemDescriptions;
	}
}
