using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	public class OrderOfBattleHeroDragWidget : Widget
	{
		public OrderOfBattleHeroDragWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!base.IsVisible)
			{
				return;
			}
			if (this._isDirty && this.StackDragWidget != null)
			{
				base.RemoveAllChildren();
				for (int i = 0; i < this.StackCount; i++)
				{
					BrushWidget brushWidget = new BrushWidget(base.Context)
					{
						Brush = this.StackDragWidget.ReadOnlyBrush,
						DoNotAcceptEvents = false,
						SuggestedHeight = this.StackDragWidget.SuggestedHeight,
						SuggestedWidth = this.StackDragWidget.SuggestedWidth,
						ScaledPositionXOffset = (float)(i * 5),
						ScaledPositionYOffset = (float)(i * 5)
					};
					if (i == this.StackCount - 1)
					{
						BrushWidget brushWidget2 = new BrushWidget(brushWidget.Context)
						{
							Brush = base.Context.GetBrush(this.InnerBrushName),
							WidthSizePolicy = SizePolicy.StretchToParent,
							HeightSizePolicy = SizePolicy.StretchToParent,
							MarginBottom = 5f,
							MarginTop = 5f,
							MarginLeft = 5f,
							MarginRight = 5f,
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center
						};
						ImageIdentifierWidget imageIdentifierWidget = new ImageIdentifierWidget(brushWidget.Context)
						{
							WidthSizePolicy = SizePolicy.Fixed,
							HeightSizePolicy = SizePolicy.Fixed,
							SuggestedWidth = this.StackThumbnailWidget.SuggestedWidth,
							SuggestedHeight = this.StackThumbnailWidget.SuggestedHeight,
							MarginTop = this.StackThumbnailWidget.MarginTop,
							MarginLeft = this.StackThumbnailWidget.MarginLeft,
							AdditionalArgs = this.StackThumbnailWidget.AdditionalArgs,
							ImageId = this.StackThumbnailWidget.ImageId,
							ImageTypeCode = this.StackThumbnailWidget.ImageTypeCode
						};
						brushWidget.AddChild(brushWidget2);
						brushWidget.AddChild(imageIdentifierWidget);
					}
					base.AddChild(brushWidget);
				}
				this._isDirty = false;
			}
		}

		private void OnStackCountChanged()
		{
			this._isDirty = true;
		}

		[Editor(false)]
		public int StackCount
		{
			get
			{
				return this._stackCount;
			}
			set
			{
				if (value != this._stackCount)
				{
					this._stackCount = value;
					base.OnPropertyChanged(value, "StackCount");
					this.OnStackCountChanged();
				}
			}
		}

		[Editor(false)]
		public BrushWidget StackDragWidget
		{
			get
			{
				return this._stackDragWidget;
			}
			set
			{
				if (value != this._stackDragWidget)
				{
					this._stackDragWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "StackDragWidget");
				}
			}
		}

		[Editor(false)]
		public ImageIdentifierWidget StackThumbnailWidget
		{
			get
			{
				return this._stackThumbnailWidget;
			}
			set
			{
				if (value != this._stackThumbnailWidget)
				{
					this._stackThumbnailWidget = value;
					base.OnPropertyChanged<ImageIdentifierWidget>(value, "StackThumbnailWidget");
				}
			}
		}

		[Editor(false)]
		public string InnerBrushName
		{
			get
			{
				return this._innerBrushName;
			}
			set
			{
				if (value != this._innerBrushName)
				{
					this._innerBrushName = value;
					base.OnPropertyChanged<string>(value, "InnerBrushName");
				}
			}
		}

		private bool _isDirty;

		private int _stackCount;

		private BrushWidget _stackDragWidget;

		private ImageIdentifierWidget _stackThumbnailWidget;

		private string _innerBrushName;
	}
}
