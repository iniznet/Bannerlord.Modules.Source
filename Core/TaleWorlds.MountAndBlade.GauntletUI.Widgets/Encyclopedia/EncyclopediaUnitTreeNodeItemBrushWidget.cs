using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	public class EncyclopediaUnitTreeNodeItemBrushWidget : BrushWidget
	{
		public EncyclopediaUnitTreeNodeItemBrushWidget(UIContext context)
			: base(context)
		{
			this._listItemAddedHandler = new Action<Widget, Widget>(this.OnListItemAdded);
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isLinesDirty)
			{
				if (this.ChildContainer.ChildCount == this.LineContainer.ChildCount)
				{
					float num = base.GlobalPosition.X + base.Size.X * 0.5f;
					for (int i = 0; i < this.ChildContainer.ChildCount; i++)
					{
						Widget child = this.ChildContainer.GetChild(i);
						Widget child2 = this.LineContainer.GetChild(i);
						float num2 = child.GlobalPosition.X + child.Size.X * 0.5f;
						bool flag = num > num2;
						child2.SetState(flag ? "Left" : "Right");
						float num3 = MathF.Abs(num - num2);
						child2.ScaledSuggestedWidth = num3;
						child2.ScaledPositionXOffset = (num3 * 0.5f + 5f * base._scaleToUse) * (float)(flag ? (-1) : 1);
					}
				}
				this._isLinesDirty = false;
			}
		}

		public void OnListItemAdded(Widget parentWidget, Widget addedWidget)
		{
			Widget widget = this.CreateLineWidget();
			if (this.ChildContainer.ChildCount == 1)
			{
				widget.SetState("Straight");
				return;
			}
			this._isLinesDirty = true;
		}

		private Widget CreateLineWidget()
		{
			BrushWidget brushWidget = new BrushWidget(base.Context)
			{
				WidthSizePolicy = SizePolicy.Fixed,
				HeightSizePolicy = SizePolicy.StretchToParent,
				Brush = this.LineBrush
			};
			brushWidget.SuggestedWidth = (float)brushWidget.ReadOnlyBrush.Sprite.Width;
			brushWidget.SuggestedHeight = (float)brushWidget.ReadOnlyBrush.Sprite.Height;
			brushWidget.HorizontalAlignment = HorizontalAlignment.Center;
			brushWidget.AddState("Left");
			brushWidget.AddState("Right");
			brushWidget.AddState("Straight");
			this.LineContainer.AddChild(brushWidget);
			return brushWidget;
		}

		[Editor(false)]
		public bool IsAlternativeUpgrade
		{
			get
			{
				return this._isAlternativeUpgrade;
			}
			set
			{
				if (value != this._isAlternativeUpgrade)
				{
					this._isAlternativeUpgrade = value;
					base.OnPropertyChanged(value, "IsAlternativeUpgrade");
				}
			}
		}

		[Editor(false)]
		public ListPanel ChildContainer
		{
			get
			{
				return this._childContainer;
			}
			set
			{
				if (this._childContainer != value)
				{
					ListPanel childContainer = this._childContainer;
					if (childContainer != null)
					{
						childContainer.ItemAddEventHandlers.Remove(this._listItemAddedHandler);
					}
					this._childContainer = value;
					base.OnPropertyChanged<ListPanel>(value, "ChildContainer");
					ListPanel childContainer2 = this._childContainer;
					if (childContainer2 == null)
					{
						return;
					}
					childContainer2.ItemAddEventHandlers.Add(this._listItemAddedHandler);
				}
			}
		}

		[Editor(false)]
		public Widget LineContainer
		{
			get
			{
				return this._lineContainer;
			}
			set
			{
				if (this._lineContainer != value)
				{
					this._lineContainer = value;
					base.OnPropertyChanged<Widget>(value, "LineContainer");
				}
			}
		}

		[Editor(false)]
		public Brush LineBrush
		{
			get
			{
				return this._lineBrush;
			}
			set
			{
				if (this._lineBrush != value)
				{
					this._lineBrush = value;
					base.OnPropertyChanged<Brush>(value, "LineBrush");
				}
			}
		}

		[Editor(false)]
		public Brush AlternateLineBrush
		{
			get
			{
				return this._alternateLineBrush;
			}
			set
			{
				if (this._alternateLineBrush != value)
				{
					this._alternateLineBrush = value;
					base.OnPropertyChanged<Brush>(value, "AlternateLineBrush");
				}
			}
		}

		private Action<Widget, Widget> _listItemAddedHandler;

		private bool _isLinesDirty;

		private bool _isAlternativeUpgrade;

		private ListPanel _childContainer;

		private Widget _lineContainer;

		private Brush _lineBrush;

		private Brush _alternateLineBrush;
	}
}
