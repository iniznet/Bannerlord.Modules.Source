using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	// Token: 0x020000B9 RID: 185
	public class MultiplayerClassLoadoutItemTabControllerButtonWidget : ButtonWidget
	{
		// Token: 0x06000989 RID: 2441 RVA: 0x0001B4B9 File Offset: 0x000196B9
		public MultiplayerClassLoadoutItemTabControllerButtonWidget(UIContext context)
			: base(context)
		{
			this._itemTabs = new List<MultiplayerItemTabButtonWidget>();
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x0001B4D0 File Offset: 0x000196D0
		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			this._itemTabs.Clear();
			for (int i = 0; i < this.ItemTabList.ChildCount; i++)
			{
				MultiplayerItemTabButtonWidget multiplayerItemTabButtonWidget = (MultiplayerItemTabButtonWidget)this.ItemTabList.GetChild(i);
				multiplayerItemTabButtonWidget.boolPropertyChanged += this.TabWidgetPropertyChanged;
				this._itemTabs.Add(multiplayerItemTabButtonWidget);
			}
			this.ItemTabList.OnInitialized += this.ItemTabListInitialized;
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x0001B54C File Offset: 0x0001974C
		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			for (int i = 0; i < this._itemTabs.Count; i++)
			{
				this._itemTabs[i].boolPropertyChanged -= this.TabWidgetPropertyChanged;
			}
			this._itemTabs.Clear();
			this.ItemTabList.OnInitialized -= this.ItemTabListInitialized;
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x0001B5B4 File Offset: 0x000197B4
		protected override void OnUpdate(float dt)
		{
			if (this.CursorWidget == null || float.IsNaN(this._targetPositionXOffset) || this.AnimationSpeed <= 0f || MathF.Abs(this.CursorWidget.PositionXOffset - this._targetPositionXOffset) <= 1E-05f)
			{
				return;
			}
			int num = MathF.Sign(this._targetPositionXOffset - this.CursorWidget.PositionXOffset);
			float num2 = MathF.Min(this.AnimationSpeed * dt, 1f);
			this.CursorWidget.PositionXOffset = MathF.Lerp(this.CursorWidget.PositionXOffset, this._targetPositionXOffset, num2, 1E-05f);
			if ((num < 0 && this.CursorWidget.PositionXOffset < this._targetPositionXOffset) || (num > 0 && this.CursorWidget.PositionXOffset > this._targetPositionXOffset))
			{
				this.CursorWidget.PositionXOffset = this._targetPositionXOffset;
			}
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x0001B693 File Offset: 0x00019893
		private void TabWidgetPropertyChanged(PropertyOwnerObject sender, string propertyName, bool value)
		{
			if (propertyName == "IsSelected" && value)
			{
				this.SelectedTabChanged(null);
			}
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x0001B6AB File Offset: 0x000198AB
		private void ItemTabListInitialized()
		{
			this.SelectedTabChanged(null);
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x0001B6B4 File Offset: 0x000198B4
		private void SelectedTabChanged(Widget widget)
		{
			if (this.CursorWidget == null || this.ItemTabList.IntValue < 0)
			{
				return;
			}
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < this.ItemTabList.ChildCount; i++)
			{
				ButtonWidget buttonWidget = (ButtonWidget)this.ItemTabList.GetChild(i);
				if (buttonWidget.IsVisible)
				{
					num2++;
					if (buttonWidget.IsSelected)
					{
						num = num2 - 1;
					}
				}
			}
			this.CalculateTargetPosition(num, num2);
		}

		// Token: 0x06000990 RID: 2448 RVA: 0x0001B724 File Offset: 0x00019924
		private void CalculateTargetPosition(int selectedIndex, int activeTabCount)
		{
			float num = this.ItemTabList.Size.X / base._scaleToUse;
			float num2 = num / (float)activeTabCount;
			float num3 = (float)selectedIndex * num2 + num2 / 2f;
			this._targetPositionXOffset = num3 - num / 2f;
		}

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06000991 RID: 2449 RVA: 0x0001B76A File Offset: 0x0001996A
		// (set) Token: 0x06000992 RID: 2450 RVA: 0x0001B774 File Offset: 0x00019974
		[DataSourceProperty]
		public MultiplayerClassLoadoutItemTabListPanel ItemTabList
		{
			get
			{
				return this._itemTabList;
			}
			set
			{
				if (value != this._itemTabList)
				{
					MultiplayerClassLoadoutItemTabListPanel itemTabList = this._itemTabList;
					if (itemTabList != null)
					{
						itemTabList.SelectEventHandlers.Remove(new Action<Widget>(this.SelectedTabChanged));
					}
					this._itemTabList = value;
					MultiplayerClassLoadoutItemTabListPanel itemTabList2 = this._itemTabList;
					if (itemTabList2 != null)
					{
						itemTabList2.SelectEventHandlers.Add(new Action<Widget>(this.SelectedTabChanged));
					}
					base.OnPropertyChanged<MultiplayerClassLoadoutItemTabListPanel>(value, "ItemTabList");
				}
			}
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06000993 RID: 2451 RVA: 0x0001B7E2 File Offset: 0x000199E2
		// (set) Token: 0x06000994 RID: 2452 RVA: 0x0001B7EA File Offset: 0x000199EA
		[DataSourceProperty]
		public Widget CursorWidget
		{
			get
			{
				return this._cursorWidget;
			}
			set
			{
				if (value != this._cursorWidget)
				{
					this._cursorWidget = value;
					base.OnPropertyChanged<Widget>(value, "CursorWidget");
				}
			}
		}

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06000995 RID: 2453 RVA: 0x0001B808 File Offset: 0x00019A08
		// (set) Token: 0x06000996 RID: 2454 RVA: 0x0001B810 File Offset: 0x00019A10
		[DataSourceProperty]
		public float AnimationSpeed
		{
			get
			{
				return this._animationSpeed;
			}
			set
			{
				if (value != this._animationSpeed)
				{
					this._animationSpeed = value;
					base.OnPropertyChanged(value, "AnimationSpeed");
				}
			}
		}

		// Token: 0x04000462 RID: 1122
		private readonly List<MultiplayerItemTabButtonWidget> _itemTabs;

		// Token: 0x04000463 RID: 1123
		private float _targetPositionXOffset;

		// Token: 0x04000464 RID: 1124
		private MultiplayerClassLoadoutItemTabListPanel _itemTabList;

		// Token: 0x04000465 RID: 1125
		private Widget _cursorWidget;

		// Token: 0x04000466 RID: 1126
		private float _animationSpeed;
	}
}
