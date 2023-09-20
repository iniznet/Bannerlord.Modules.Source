using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	public class EncyclopediaListWidget : Widget
	{
		public EncyclopediaListWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._isListSizeInitialized && this.ItemListScroll != null && this.ItemListScroll.Size.Y != 0f)
			{
				this._isListSizeInitialized = true;
				this._isDirty = true;
			}
			if (this._isDirty)
			{
				this._isDirty = false;
				this.UpdateScrollPosition();
			}
		}

		private void UpdateScrollPosition()
		{
			if (!string.IsNullOrEmpty(this.LastSelectedItemId) && this.ItemList != null && this.ItemListScroll != null)
			{
				Widget widget = this.ItemList.AllChildren.FirstOrDefault(delegate(Widget x)
				{
					EncyclopediaListItemButtonWidget encyclopediaListItemButtonWidget;
					return (encyclopediaListItemButtonWidget = x as EncyclopediaListItemButtonWidget) != null && encyclopediaListItemButtonWidget.ListItemId == this.LastSelectedItemId;
				});
				if (widget != null && widget.IsVisible)
				{
					float num = widget.ScaledSuggestedHeight + widget.ScaledMarginTop + widget.ScaledMarginBottom - 2f * base._scaleToUse;
					int visibleSiblingIndex = widget.GetVisibleSiblingIndex();
					float num2 = num * (float)visibleSiblingIndex - this.ItemListScroll.Size.Y / 2f;
					this.ItemListScroll.SetValueForced(num2);
				}
			}
		}

		private void OnListItemAdded(Widget widget, string eventName, object[] eventArgs)
		{
			if (eventName == "ItemAdd" && eventArgs.Length != 0 && eventArgs[0] is EncyclopediaListItemButtonWidget)
			{
				this._isDirty = true;
			}
		}

		[Editor(false)]
		public string LastSelectedItemId
		{
			get
			{
				return this._lastSelectedItemId;
			}
			set
			{
				if (this._lastSelectedItemId != value)
				{
					this._lastSelectedItemId = value;
					base.OnPropertyChanged<string>(value, "LastSelectedItemId");
					this._isDirty = true;
				}
			}
		}

		public ListPanel ItemList
		{
			get
			{
				return this._itemList;
			}
			set
			{
				if (this._itemList != value)
				{
					this._itemList = value;
					this._isDirty = true;
					this._itemList.EventFire += this.OnListItemAdded;
				}
			}
		}

		public ScrollbarWidget ItemListScroll
		{
			get
			{
				return this._itemListScroll;
			}
			set
			{
				if (this._itemListScroll != value)
				{
					this._itemListScroll = value;
					this._isDirty = true;
				}
			}
		}

		private bool _isDirty;

		private bool _isListSizeInitialized;

		private string _lastSelectedItemId;

		private ListPanel _itemList;

		private ScrollbarWidget _itemListScroll;
	}
}
