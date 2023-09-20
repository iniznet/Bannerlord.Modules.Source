using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	// Token: 0x0200005B RID: 91
	public class PartyQuestProgressWidget : Widget
	{
		// Token: 0x060004B6 RID: 1206 RVA: 0x0000E854 File Offset: 0x0000CA54
		public PartyQuestProgressWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x0000E860 File Offset: 0x0000CA60
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

		// Token: 0x060004B8 RID: 1208 RVA: 0x0000E90C File Offset: 0x0000CB0C
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

		// Token: 0x060004B9 RID: 1209 RVA: 0x0000E9B8 File Offset: 0x0000CBB8
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

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060004BA RID: 1210 RVA: 0x0000EA1C File Offset: 0x0000CC1C
		// (set) Token: 0x060004BB RID: 1211 RVA: 0x0000EA24 File Offset: 0x0000CC24
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

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x060004BC RID: 1212 RVA: 0x0000EA48 File Offset: 0x0000CC48
		// (set) Token: 0x060004BD RID: 1213 RVA: 0x0000EA50 File Offset: 0x0000CC50
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

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x060004BE RID: 1214 RVA: 0x0000EA6E File Offset: 0x0000CC6E
		// (set) Token: 0x060004BF RID: 1215 RVA: 0x0000EA76 File Offset: 0x0000CC76
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

		// Token: 0x0400020D RID: 525
		private int _itemCount;

		// Token: 0x0400020E RID: 526
		private ListPanel _dividerContainer;

		// Token: 0x0400020F RID: 527
		private Brush _dividerBrush;
	}
}
