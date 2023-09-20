using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	// Token: 0x0200013A RID: 314
	public class EncyclopediaListWidget : Widget
	{
		// Token: 0x0600108A RID: 4234 RVA: 0x0002E59E File Offset: 0x0002C79E
		public EncyclopediaListWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600108B RID: 4235 RVA: 0x0002E5A8 File Offset: 0x0002C7A8
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

		// Token: 0x0600108C RID: 4236 RVA: 0x0002E608 File Offset: 0x0002C808
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

		// Token: 0x0600108D RID: 4237 RVA: 0x0002E6AC File Offset: 0x0002C8AC
		private void OnListItemAdded(Widget widget, string eventName, object[] eventArgs)
		{
			if (eventName == "ItemAdd" && eventArgs.Length != 0 && eventArgs[0] is EncyclopediaListItemButtonWidget)
			{
				this._isDirty = true;
			}
		}

		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x0600108E RID: 4238 RVA: 0x0002E6D0 File Offset: 0x0002C8D0
		// (set) Token: 0x0600108F RID: 4239 RVA: 0x0002E6D8 File Offset: 0x0002C8D8
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

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x06001090 RID: 4240 RVA: 0x0002E702 File Offset: 0x0002C902
		// (set) Token: 0x06001091 RID: 4241 RVA: 0x0002E70A File Offset: 0x0002C90A
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

		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x06001092 RID: 4242 RVA: 0x0002E73A File Offset: 0x0002C93A
		// (set) Token: 0x06001093 RID: 4243 RVA: 0x0002E742 File Offset: 0x0002C942
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

		// Token: 0x0400079A RID: 1946
		private bool _isDirty;

		// Token: 0x0400079B RID: 1947
		private bool _isListSizeInitialized;

		// Token: 0x0400079C RID: 1948
		private string _lastSelectedItemId;

		// Token: 0x0400079D RID: 1949
		private ListPanel _itemList;

		// Token: 0x0400079E RID: 1950
		private ScrollbarWidget _itemListScroll;
	}
}
