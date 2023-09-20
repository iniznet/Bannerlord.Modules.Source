using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	// Token: 0x02000044 RID: 68
	public class TutorialScreenWidget : Widget
	{
		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000389 RID: 905 RVA: 0x0000B964 File Offset: 0x00009B64
		// (set) Token: 0x0600038A RID: 906 RVA: 0x0000B96C File Offset: 0x00009B6C
		public TutorialPanelImageWidget LeftItem { get; set; }

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x0600038B RID: 907 RVA: 0x0000B975 File Offset: 0x00009B75
		// (set) Token: 0x0600038C RID: 908 RVA: 0x0000B97D File Offset: 0x00009B7D
		public TutorialPanelImageWidget RightItem { get; set; }

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x0600038D RID: 909 RVA: 0x0000B986 File Offset: 0x00009B86
		// (set) Token: 0x0600038E RID: 910 RVA: 0x0000B98E File Offset: 0x00009B8E
		public TutorialPanelImageWidget BottomItem { get; set; }

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x0600038F RID: 911 RVA: 0x0000B997 File Offset: 0x00009B97
		// (set) Token: 0x06000390 RID: 912 RVA: 0x0000B99F File Offset: 0x00009B9F
		public TutorialPanelImageWidget TopItem { get; set; }

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000391 RID: 913 RVA: 0x0000B9A8 File Offset: 0x00009BA8
		// (set) Token: 0x06000392 RID: 914 RVA: 0x0000B9B0 File Offset: 0x00009BB0
		public TutorialPanelImageWidget LeftTopItem { get; set; }

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000393 RID: 915 RVA: 0x0000B9B9 File Offset: 0x00009BB9
		// (set) Token: 0x06000394 RID: 916 RVA: 0x0000B9C1 File Offset: 0x00009BC1
		public TutorialPanelImageWidget RightTopItem { get; set; }

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000395 RID: 917 RVA: 0x0000B9CA File Offset: 0x00009BCA
		// (set) Token: 0x06000396 RID: 918 RVA: 0x0000B9D2 File Offset: 0x00009BD2
		public TutorialPanelImageWidget LeftBottomItem { get; set; }

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000397 RID: 919 RVA: 0x0000B9DB File Offset: 0x00009BDB
		// (set) Token: 0x06000398 RID: 920 RVA: 0x0000B9E3 File Offset: 0x00009BE3
		public TutorialPanelImageWidget RightBottomItem { get; set; }

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000399 RID: 921 RVA: 0x0000B9EC File Offset: 0x00009BEC
		// (set) Token: 0x0600039A RID: 922 RVA: 0x0000B9F4 File Offset: 0x00009BF4
		public TutorialPanelImageWidget CenterItem { get; set; }

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x0600039B RID: 923 RVA: 0x0000B9FD File Offset: 0x00009BFD
		// (set) Token: 0x0600039C RID: 924 RVA: 0x0000BA05 File Offset: 0x00009C05
		public TutorialArrowWidget ArrowWidget { get; set; }

		// Token: 0x0600039D RID: 925 RVA: 0x0000BA0E File Offset: 0x00009C0E
		public TutorialScreenWidget(UIContext context)
			: base(context)
		{
			EventManager.UIEventManager.RegisterEvent<TutorialHighlightItemBrushWidget.HighlightElementToggledEvent>(new Action<TutorialHighlightItemBrushWidget.HighlightElementToggledEvent>(this.OnHighlightElementToggleEvent));
		}

		// Token: 0x0600039E RID: 926 RVA: 0x0000BA30 File Offset: 0x00009C30
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initalized)
			{
				this.LeftItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.RightItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.BottomItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.TopItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.LeftTopItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.RightTopItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.LeftBottomItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.RightBottomItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.CenterItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this._initalized = true;
			}
			if (this._currentActiveHighligtFrame != null && this._currentActivePanelItem != null)
			{
				Tuple<Widget, Widget> leftAndRightElements = this.GetLeftAndRightElements();
				Tuple<Widget, Widget> topAndBottomElements = this.GetTopAndBottomElements();
				float num = leftAndRightElements.Item1.GlobalPosition.X + leftAndRightElements.Item1.Size.X;
				float x = leftAndRightElements.Item2.GlobalPosition.X;
				float y = topAndBottomElements.Item1.GlobalPosition.Y;
				float y2 = topAndBottomElements.Item2.GlobalPosition.Y;
				float num2 = MathF.Abs(num - x);
				float num3 = MathF.Abs(y - y2);
				this.ArrowWidget.ScaledPositionXOffset = num;
				this.ArrowWidget.ScaledPositionYOffset = y;
				this.ArrowWidget.SetArrowProperties(num2, num3, this.GetIsArrowDirectionIsDownwards(), this.GetIsArrowDirectionIsTowardsRight());
				this.ArrowWidget.IsVisible = true;
				return;
			}
			this.ArrowWidget.IsVisible = false;
		}

		// Token: 0x0600039F RID: 927 RVA: 0x0000BC00 File Offset: 0x00009E00
		private bool GetIsArrowDirectionIsDownwards()
		{
			if (this._currentActiveHighligtFrame.GlobalPosition.Y < this._currentActivePanelItem.GlobalPosition.Y)
			{
				return this._currentActiveHighligtFrame.GlobalPosition.X < this._currentActivePanelItem.GlobalPosition.X;
			}
			return this._currentActivePanelItem.GlobalPosition.X < this._currentActiveHighligtFrame.GlobalPosition.X;
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x0000BC74 File Offset: 0x00009E74
		private bool GetIsArrowDirectionIsTowardsRight()
		{
			return this._currentActiveHighligtFrame.GlobalPosition.X > this._currentActivePanelItem.GlobalPosition.X;
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x0000BC98 File Offset: 0x00009E98
		private Tuple<Widget, Widget> GetLeftAndRightElements()
		{
			if (this._currentActiveHighligtFrame.GlobalPosition.X < this._currentActivePanelItem.GlobalPosition.X)
			{
				return new Tuple<Widget, Widget>(this._currentActiveHighligtFrame, this._currentActivePanelItem);
			}
			return new Tuple<Widget, Widget>(this._currentActivePanelItem, this._currentActiveHighligtFrame);
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x0000BCEC File Offset: 0x00009EEC
		private Tuple<Widget, Widget> GetTopAndBottomElements()
		{
			if (this._currentActiveHighligtFrame.GlobalPosition.Y < this._currentActivePanelItem.GlobalPosition.Y)
			{
				return new Tuple<Widget, Widget>(this._currentActiveHighligtFrame, this._currentActivePanelItem);
			}
			return new Tuple<Widget, Widget>(this._currentActivePanelItem, this._currentActiveHighligtFrame);
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x0000BD3E File Offset: 0x00009F3E
		private void OnTutorialItemPropertyChanged(PropertyOwnerObject widget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsDisabled")
			{
				if (propertyValue)
				{
					this._currentActivePanelItem = null;
					this.ArrowWidget.ResetFade();
					return;
				}
				this._currentActivePanelItem = widget as TutorialPanelImageWidget;
				this.ArrowWidget.DisableFade();
			}
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x0000BD7A File Offset: 0x00009F7A
		private void OnHighlightElementToggleEvent(TutorialHighlightItemBrushWidget.HighlightElementToggledEvent obj)
		{
			if (obj.IsEnabled)
			{
				this._currentActiveHighligtFrame = obj.HighlightFrameWidget;
				this.ArrowWidget.ResetFade();
				return;
			}
			this.ArrowWidget.DisableFade();
			this._currentActiveHighligtFrame = null;
		}

		// Token: 0x0400018A RID: 394
		private bool _initalized;

		// Token: 0x0400018B RID: 395
		private TutorialHighlightItemBrushWidget _currentActiveHighligtFrame;

		// Token: 0x0400018C RID: 396
		private TutorialPanelImageWidget _currentActivePanelItem;
	}
}
