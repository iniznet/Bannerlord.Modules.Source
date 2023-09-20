using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	public class PartyQuestProgressWidget : Widget
	{
		public PartyQuestProgressWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateDividers()
		{
			if (this.DividerContainer == null || this.DividerBrush == null)
			{
				return;
			}
			int itemCount = this.ItemCount;
			if (this.DividerContainer.ChildCount > itemCount)
			{
				int num = this.DividerContainer.ChildCount - itemCount;
				for (int i = 0; i < num; i++)
				{
					this.DividerContainer.RemoveChild(this.DividerContainer.GetChild(i));
				}
			}
			else if (itemCount > this.DividerContainer.ChildCount)
			{
				int num2 = itemCount - this.DividerContainer.ChildCount;
				for (int j = 0; j < num2; j++)
				{
					this.DividerContainer.AddChild(this.CreateDivider());
				}
			}
			this.UpdateDividerPositions();
		}

		private Widget CreateDivider()
		{
			Widget widget = new Widget(base.Context);
			widget.WidthSizePolicy = SizePolicy.StretchToParent;
			widget.HeightSizePolicy = SizePolicy.StretchToParent;
			BrushWidget brushWidget = new BrushWidget(base.Context);
			brushWidget.WidthSizePolicy = SizePolicy.Fixed;
			brushWidget.HeightSizePolicy = SizePolicy.Fixed;
			brushWidget.Brush = this.DividerBrush;
			brushWidget.SuggestedWidth = (float)brushWidget.ReadOnlyBrush.Sprite.Width;
			brushWidget.SuggestedHeight = (float)brushWidget.ReadOnlyBrush.Sprite.Height;
			brushWidget.HorizontalAlignment = HorizontalAlignment.Right;
			brushWidget.VerticalAlignment = VerticalAlignment.Center;
			brushWidget.PositionXOffset = (float)brushWidget.ReadOnlyBrush.Sprite.Width * 0.5f;
			widget.AddChild(brushWidget);
			return widget;
		}

		private void UpdateDividerPositions()
		{
			int childCount = this.DividerContainer.ChildCount;
			float num = this.DividerContainer.Size.X / (float)(childCount + 1);
			for (int i = 0; i < childCount; i++)
			{
				Widget child = this.DividerContainer.GetChild(i);
				child.PositionXOffset = (float)i * num - child.Size.X / 2f;
			}
		}

		[Editor(false)]
		public int ItemCount
		{
			get
			{
				return this._itemCount;
			}
			set
			{
				if (this._itemCount != value)
				{
					this._itemCount = value;
					base.OnPropertyChanged(value, "ItemCount");
					this.UpdateDividers();
				}
			}
		}

		[Editor(false)]
		public ListPanel DividerContainer
		{
			get
			{
				return this._dividerContainer;
			}
			set
			{
				if (this._dividerContainer != value)
				{
					this._dividerContainer = value;
					base.OnPropertyChanged<ListPanel>(value, "DividerContainer");
				}
			}
		}

		[Editor(false)]
		public Brush DividerBrush
		{
			get
			{
				return this._dividerBrush;
			}
			set
			{
				if (this._dividerBrush != value)
				{
					this._dividerBrush = value;
					base.OnPropertyChanged<Brush>(value, "DividerBrush");
				}
			}
		}

		private int _itemCount;

		private ListPanel _dividerContainer;

		private Brush _dividerBrush;
	}
}
