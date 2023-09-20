using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	// Token: 0x020000D6 RID: 214
	public class OrderOfBattleHeroDragWidget : Widget
	{
		// Token: 0x06000AE2 RID: 2786 RVA: 0x0001E3B2 File Offset: 0x0001C5B2
		public OrderOfBattleHeroDragWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x0001E3BC File Offset: 0x0001C5BC
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

		// Token: 0x06000AE4 RID: 2788 RVA: 0x0001E58C File Offset: 0x0001C78C
		private void OnStackCountChanged()
		{
			this._isDirty = true;
		}

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x06000AE5 RID: 2789 RVA: 0x0001E595 File Offset: 0x0001C795
		// (set) Token: 0x06000AE6 RID: 2790 RVA: 0x0001E59D File Offset: 0x0001C79D
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

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x06000AE7 RID: 2791 RVA: 0x0001E5C1 File Offset: 0x0001C7C1
		// (set) Token: 0x06000AE8 RID: 2792 RVA: 0x0001E5C9 File Offset: 0x0001C7C9
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

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x06000AE9 RID: 2793 RVA: 0x0001E5E7 File Offset: 0x0001C7E7
		// (set) Token: 0x06000AEA RID: 2794 RVA: 0x0001E5EF File Offset: 0x0001C7EF
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

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x06000AEB RID: 2795 RVA: 0x0001E60D File Offset: 0x0001C80D
		// (set) Token: 0x06000AEC RID: 2796 RVA: 0x0001E615 File Offset: 0x0001C815
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

		// Token: 0x040004F5 RID: 1269
		private bool _isDirty;

		// Token: 0x040004F6 RID: 1270
		private int _stackCount;

		// Token: 0x040004F7 RID: 1271
		private BrushWidget _stackDragWidget;

		// Token: 0x040004F8 RID: 1272
		private ImageIdentifierWidget _stackThumbnailWidget;

		// Token: 0x040004F9 RID: 1273
		private string _innerBrushName;
	}
}
