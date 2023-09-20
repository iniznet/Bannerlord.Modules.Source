using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	// Token: 0x0200013C RID: 316
	public class EncyclopediaUnitTreeNodeItemBrushWidget : BrushWidget
	{
		// Token: 0x060010A1 RID: 4257 RVA: 0x0002E936 File Offset: 0x0002CB36
		public EncyclopediaUnitTreeNodeItemBrushWidget(UIContext context)
			: base(context)
		{
			this._listItemAddedHandler = new Action<Widget, Widget>(this.OnListItemAdded);
		}

		// Token: 0x060010A2 RID: 4258 RVA: 0x0002E954 File Offset: 0x0002CB54
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

		// Token: 0x060010A3 RID: 4259 RVA: 0x0002EA60 File Offset: 0x0002CC60
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

		// Token: 0x060010A4 RID: 4260 RVA: 0x0002EA98 File Offset: 0x0002CC98
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

		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x060010A5 RID: 4261 RVA: 0x0002EB2E File Offset: 0x0002CD2E
		// (set) Token: 0x060010A6 RID: 4262 RVA: 0x0002EB36 File Offset: 0x0002CD36
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

		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x060010A7 RID: 4263 RVA: 0x0002EB54 File Offset: 0x0002CD54
		// (set) Token: 0x060010A8 RID: 4264 RVA: 0x0002EB5C File Offset: 0x0002CD5C
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

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x060010A9 RID: 4265 RVA: 0x0002EBBD File Offset: 0x0002CDBD
		// (set) Token: 0x060010AA RID: 4266 RVA: 0x0002EBC5 File Offset: 0x0002CDC5
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

		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x060010AB RID: 4267 RVA: 0x0002EBE3 File Offset: 0x0002CDE3
		// (set) Token: 0x060010AC RID: 4268 RVA: 0x0002EBEB File Offset: 0x0002CDEB
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

		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x060010AD RID: 4269 RVA: 0x0002EC09 File Offset: 0x0002CE09
		// (set) Token: 0x060010AE RID: 4270 RVA: 0x0002EC11 File Offset: 0x0002CE11
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

		// Token: 0x040007A3 RID: 1955
		private Action<Widget, Widget> _listItemAddedHandler;

		// Token: 0x040007A4 RID: 1956
		private bool _isLinesDirty;

		// Token: 0x040007A5 RID: 1957
		private bool _isAlternativeUpgrade;

		// Token: 0x040007A6 RID: 1958
		private ListPanel _childContainer;

		// Token: 0x040007A7 RID: 1959
		private Widget _lineContainer;

		// Token: 0x040007A8 RID: 1960
		private Brush _lineBrush;

		// Token: 0x040007A9 RID: 1961
		private Brush _alternateLineBrush;
	}
}
