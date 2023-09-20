using System;
using System.Collections.Generic;
using System.Numerics;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000056 RID: 86
	public abstract class Container : Widget
	{
		// Token: 0x17000193 RID: 403
		// (get) Token: 0x0600056D RID: 1389 RVA: 0x0001780A File Offset: 0x00015A0A
		// (set) Token: 0x0600056E RID: 1390 RVA: 0x00017812 File Offset: 0x00015A12
		public ContainerItemDescription DefaultItemDescription { get; private set; }

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x0600056F RID: 1391
		// (set) Token: 0x06000570 RID: 1392
		public abstract Predicate<Widget> AcceptDropPredicate { get; set; }

		// Token: 0x06000571 RID: 1393
		public abstract Vector2 GetDropGizmoPosition(Vector2 draggedWidgetPosition);

		// Token: 0x06000572 RID: 1394
		public abstract int GetIndexForDrop(Vector2 draggedWidgetPosition);

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000573 RID: 1395 RVA: 0x0001781B File Offset: 0x00015A1B
		// (set) Token: 0x06000574 RID: 1396 RVA: 0x00017838 File Offset: 0x00015A38
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

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000575 RID: 1397
		public abstract bool IsDragHovering { get; }

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000576 RID: 1398 RVA: 0x000178DC File Offset: 0x00015ADC
		// (set) Token: 0x06000577 RID: 1399 RVA: 0x000178E4 File Offset: 0x00015AE4
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

		// Token: 0x06000578 RID: 1400 RVA: 0x000178FC File Offset: 0x00015AFC
		protected Container(UIContext context)
			: base(context)
		{
			this.DefaultItemDescription = new ContainerItemDescription();
			this._itemDescriptions = new List<ContainerItemDescription>();
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x0001795C File Offset: 0x00015B5C
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

		// Token: 0x0600057A RID: 1402 RVA: 0x0001799C File Offset: 0x00015B9C
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

		// Token: 0x0600057B RID: 1403
		public abstract void OnChildSelected(Widget widget);

		// Token: 0x0600057C RID: 1404 RVA: 0x00017A2C File Offset: 0x00015C2C
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

		// Token: 0x0600057D RID: 1405 RVA: 0x00017AA0 File Offset: 0x00015CA0
		protected override void OnChildAdded(Widget child)
		{
			foreach (Action<Widget, Widget> action in this.ItemAddEventHandlers)
			{
				action(this, child);
			}
			base.OnChildAdded(child);
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x00017AFC File Offset: 0x00015CFC
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

		// Token: 0x0600057F RID: 1407 RVA: 0x00017B90 File Offset: 0x00015D90
		protected override void OnAfterChildRemoved(Widget child)
		{
			foreach (Action<Widget> action in this.ItemAfterRemoveEventHandlers)
			{
				action(this);
			}
			base.OnAfterChildRemoved(child);
		}

		// Token: 0x06000580 RID: 1408 RVA: 0x00017BE8 File Offset: 0x00015DE8
		public void AddItemDescription(ContainerItemDescription itemDescription)
		{
			this._itemDescriptions.Add(itemDescription);
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x00017BF8 File Offset: 0x00015DF8
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

		// Token: 0x040002A0 RID: 672
		public List<Action<Widget>> SelectEventHandlers = new List<Action<Widget>>();

		// Token: 0x040002A1 RID: 673
		public List<Action<Widget, Widget>> ItemAddEventHandlers = new List<Action<Widget, Widget>>();

		// Token: 0x040002A2 RID: 674
		public List<Action<Widget, Widget>> ItemRemoveEventHandlers = new List<Action<Widget, Widget>>();

		// Token: 0x040002A3 RID: 675
		public List<Action<Widget>> ItemAfterRemoveEventHandlers = new List<Action<Widget>>();

		// Token: 0x040002A4 RID: 676
		private int _intValue = -1;

		// Token: 0x040002A5 RID: 677
		private bool _currentlyChangingIntValue;

		// Token: 0x040002A6 RID: 678
		public bool ShowSelection;

		// Token: 0x040002A7 RID: 679
		private int _dragHoverInsertionIndex;

		// Token: 0x040002A8 RID: 680
		private List<ContainerItemDescription> _itemDescriptions;
	}
}
