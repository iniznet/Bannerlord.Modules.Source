using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	public class MultiplayerClassLoadoutItemTabControllerButtonWidget : ButtonWidget
	{
		public MultiplayerClassLoadoutItemTabControllerButtonWidget(UIContext context)
			: base(context)
		{
			this._itemTabs = new List<MultiplayerItemTabButtonWidget>();
		}

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

		private void TabWidgetPropertyChanged(PropertyOwnerObject sender, string propertyName, bool value)
		{
			if (propertyName == "IsSelected" && value)
			{
				this.SelectedTabChanged(null);
			}
		}

		private void ItemTabListInitialized()
		{
			this.SelectedTabChanged(null);
		}

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

		private void CalculateTargetPosition(int selectedIndex, int activeTabCount)
		{
			float num = this.ItemTabList.Size.X / base._scaleToUse;
			float num2 = num / (float)activeTabCount;
			float num3 = (float)selectedIndex * num2 + num2 / 2f;
			this._targetPositionXOffset = num3 - num / 2f;
		}

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

		private readonly List<MultiplayerItemTabButtonWidget> _itemTabs;

		private float _targetPositionXOffset;

		private MultiplayerClassLoadoutItemTabListPanel _itemTabList;

		private Widget _cursorWidget;

		private float _animationSpeed;
	}
}
