using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.HUD
{
	// Token: 0x020000B4 RID: 180
	public class MoraleWidget : Widget
	{
		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06000936 RID: 2358 RVA: 0x0001A6A4 File Offset: 0x000188A4
		private float MoraleArrowFadeInTime
		{
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06000937 RID: 2359 RVA: 0x0001A6AB File Offset: 0x000188AB
		private float MoraleArrowFadeOutTime
		{
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06000938 RID: 2360 RVA: 0x0001A6B2 File Offset: 0x000188B2
		private float MoraleArrowMoveTime
		{
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x0001A6B9 File Offset: 0x000188B9
		public MoraleWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x0001A6C2 File Offset: 0x000188C2
		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			this._moraleItemWidgets = this.CreateItemWidgets(this.ItemContainer);
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x0001A6DC File Offset: 0x000188DC
		protected override void OnUpdate(float dt)
		{
			if (this._triggerAnimations)
			{
				this._triggerWaitPassedFrames++;
				if (this._triggerWaitPassedFrames >= 2)
				{
					this.HandleAnimation();
					this._triggerAnimations = false;
				}
			}
			if (!this._initialized)
			{
				this.FlowArrowWidget.LeftSideArrow = this.ExtendToLeft;
				this._initialized = true;
			}
		}

		// Token: 0x0600093C RID: 2364 RVA: 0x0001A735 File Offset: 0x00018935
		private void UpdateArrows(int flowLevel)
		{
			if (this.Container == null || this.FlowArrowWidget == null)
			{
				return;
			}
			this.FlowArrowWidget.SetFlowLevel(flowLevel);
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x0001A754 File Offset: 0x00018954
		private void UpdateMoraleMask()
		{
			int num = MathF.Floor((float)this.MoralePercentage / 100f * 10f);
			for (int i = 0; i < this._moraleItemWidgets.Length; i++)
			{
				MoraleWidget.MoraleItemWidget moraleItemWidget = this._moraleItemWidgets[i];
				float num2 = 0f;
				if (i < num)
				{
					num2 = 1f;
					if (!moraleItemWidget.ItemGlowWidget.IsVisible)
					{
						this._triggerAnimations = true;
					}
				}
				else if (i == num)
				{
					float num3 = 10f;
					num2 = ((float)this.MoralePercentage - (float)num * num3) / num3;
				}
				moraleItemWidget.SetFillAmount(num2, 12);
			}
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x0001A7E4 File Offset: 0x000189E4
		private void HandleAnimation()
		{
			string text;
			if (this.MoralePercentage < 20)
			{
				text = "IsCriticalAnim";
			}
			else if (this.IncreaseLevel > 0)
			{
				text = "IncreaseAnim";
			}
			else
			{
				text = "Default";
			}
			if (!string.IsNullOrEmpty(text))
			{
				for (int i = 0; i < this._moraleItemWidgets.Length; i++)
				{
					MoraleWidget.MoraleItemWidget moraleItemWidget = this._moraleItemWidgets[i];
					moraleItemWidget.ItemWidget.SetState(text);
					moraleItemWidget.ItemWidget.BrushRenderer.RestartAnimation();
					moraleItemWidget.ItemGlowWidget.SetState(text);
					moraleItemWidget.ItemGlowWidget.BrushRenderer.RestartAnimation();
				}
				this.FlowArrowWidget.SetState(text);
				this.FlowArrowWidget.BrushRenderer.RestartAnimation();
			}
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x0001A890 File Offset: 0x00018A90
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.UpdateMoraleMask();
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x0001A8A0 File Offset: 0x00018AA0
		private MoraleWidget.MoraleItemWidget[] CreateItemWidgets(Widget containerWidget)
		{
			MoraleWidget.MoraleItemWidget[] array = new MoraleWidget.MoraleItemWidget[10];
			for (int i = 0; i < 10; i++)
			{
				Widget widget = new Widget(base.Context);
				widget.UpdateChildrenStates = true;
				widget.WidthSizePolicy = SizePolicy.Fixed;
				widget.HeightSizePolicy = SizePolicy.Fixed;
				widget.SuggestedWidth = 39f;
				widget.SuggestedHeight = 38f;
				if (this.ExtendToLeft)
				{
					widget.HorizontalAlignment = HorizontalAlignment.Right;
					widget.MarginRight = (float)i * 28f;
				}
				else
				{
					widget.HorizontalAlignment = HorizontalAlignment.Left;
					widget.MarginLeft = (float)i * 28f;
				}
				widget.AddState("IncreaseAnim");
				containerWidget.AddChild(widget);
				Widget widget2 = new Widget(base.Context);
				widget2.ClipContents = true;
				widget2.UpdateChildrenStates = true;
				widget2.WidthSizePolicy = SizePolicy.StretchToParent;
				widget2.HeightSizePolicy = SizePolicy.Fixed;
				widget2.VerticalAlignment = VerticalAlignment.Bottom;
				widget.AddChild(widget2);
				BrushWidget brushWidget = new BrushWidget(base.Context);
				brushWidget.WidthSizePolicy = SizePolicy.Fixed;
				brushWidget.HeightSizePolicy = SizePolicy.Fixed;
				brushWidget.VerticalAlignment = VerticalAlignment.Bottom;
				brushWidget.Brush = this.ItemGlowBrush;
				brushWidget.SuggestedWidth = 39f;
				brushWidget.SuggestedHeight = 38f;
				brushWidget.AddState("IncreaseAnim");
				widget2.AddChild(brushWidget);
				BrushWidget brushWidget2 = new BrushWidget(base.Context);
				brushWidget2.WidthSizePolicy = SizePolicy.StretchToParent;
				brushWidget2.HeightSizePolicy = SizePolicy.StretchToParent;
				brushWidget2.Brush = this.ItemBackgroundBrush;
				widget.AddChild(brushWidget2);
				BrushWidget brushWidget3 = new BrushWidget(base.Context);
				brushWidget3.WidthSizePolicy = SizePolicy.Fixed;
				brushWidget3.HeightSizePolicy = SizePolicy.Fixed;
				brushWidget3.VerticalAlignment = VerticalAlignment.Bottom;
				brushWidget3.Brush = this.ItemBrush;
				brushWidget3.SuggestedWidth = 39f;
				brushWidget3.SuggestedHeight = 38f;
				brushWidget3.AddState("IncreaseAnim");
				widget2.AddChild(brushWidget3);
				array[i] = new MoraleWidget.MoraleItemWidget(widget, widget2, brushWidget3, brushWidget, brushWidget2);
			}
			return array;
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06000941 RID: 2369 RVA: 0x0001AA7A File Offset: 0x00018C7A
		// (set) Token: 0x06000942 RID: 2370 RVA: 0x0001AA82 File Offset: 0x00018C82
		[DataSourceProperty]
		public int IncreaseLevel
		{
			get
			{
				return this._increaseLevel;
			}
			set
			{
				if (this._increaseLevel != value)
				{
					this._increaseLevel = value;
					base.OnPropertyChanged(value, "IncreaseLevel");
					this.UpdateArrows(this._increaseLevel);
					this._triggerAnimations = true;
					this._triggerWaitPassedFrames = 0;
				}
			}
		}

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06000943 RID: 2371 RVA: 0x0001AABA File Offset: 0x00018CBA
		// (set) Token: 0x06000944 RID: 2372 RVA: 0x0001AAC2 File Offset: 0x00018CC2
		[DataSourceProperty]
		public int MoralePercentage
		{
			get
			{
				return this._moralePercentage;
			}
			set
			{
				if (this._moralePercentage != value)
				{
					this._moralePercentage = value;
					base.OnPropertyChanged(value, "MoralePercentage");
					this._triggerAnimations = true;
				}
			}
		}

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06000945 RID: 2373 RVA: 0x0001AAE7 File Offset: 0x00018CE7
		// (set) Token: 0x06000946 RID: 2374 RVA: 0x0001AAEF File Offset: 0x00018CEF
		[DataSourceProperty]
		public Widget Container
		{
			get
			{
				return this._container;
			}
			set
			{
				if (this._container != value)
				{
					this._container = value;
					base.OnPropertyChanged<Widget>(value, "Container");
				}
			}
		}

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06000947 RID: 2375 RVA: 0x0001AB0D File Offset: 0x00018D0D
		// (set) Token: 0x06000948 RID: 2376 RVA: 0x0001AB15 File Offset: 0x00018D15
		[DataSourceProperty]
		public Widget ItemContainer
		{
			get
			{
				return this._itemContainer;
			}
			set
			{
				if (this._itemContainer != value)
				{
					this._itemContainer = value;
					base.OnPropertyChanged<Widget>(value, "ItemContainer");
				}
			}
		}

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06000949 RID: 2377 RVA: 0x0001AB33 File Offset: 0x00018D33
		// (set) Token: 0x0600094A RID: 2378 RVA: 0x0001AB3B File Offset: 0x00018D3B
		[DataSourceProperty]
		public Brush ItemBrush
		{
			get
			{
				return this._itemBrush;
			}
			set
			{
				if (this._itemBrush != value)
				{
					this._itemBrush = value;
					base.OnPropertyChanged<Brush>(value, "ItemBrush");
				}
			}
		}

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x0600094B RID: 2379 RVA: 0x0001AB59 File Offset: 0x00018D59
		// (set) Token: 0x0600094C RID: 2380 RVA: 0x0001AB61 File Offset: 0x00018D61
		[DataSourceProperty]
		public Brush ItemGlowBrush
		{
			get
			{
				return this._itemGlowBrush;
			}
			set
			{
				if (this._itemGlowBrush != value)
				{
					this._itemGlowBrush = value;
					base.OnPropertyChanged<Brush>(value, "ItemGlowBrush");
				}
			}
		}

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x0600094D RID: 2381 RVA: 0x0001AB7F File Offset: 0x00018D7F
		// (set) Token: 0x0600094E RID: 2382 RVA: 0x0001AB87 File Offset: 0x00018D87
		[DataSourceProperty]
		public Brush ItemBackgroundBrush
		{
			get
			{
				return this._itemBackgroundBrush;
			}
			set
			{
				if (this._itemBackgroundBrush != value)
				{
					this._itemBackgroundBrush = value;
					base.OnPropertyChanged<Brush>(value, "ItemBackgroundBrush");
				}
			}
		}

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x0600094F RID: 2383 RVA: 0x0001ABA5 File Offset: 0x00018DA5
		// (set) Token: 0x06000950 RID: 2384 RVA: 0x0001ABB0 File Offset: 0x00018DB0
		[DataSourceProperty]
		public string TeamColorAsStr
		{
			get
			{
				return this._teamColorAsStr;
			}
			set
			{
				if (this._teamColorAsStr != value && value != null)
				{
					this._teamColorAsStr = value;
					base.OnPropertyChanged<string>(value, "TeamColorAsStr");
					this._teamColor = Color.ConvertStringToColor(value);
					foreach (MoraleWidget.MoraleItemWidget moraleItemWidget in this._moraleItemWidgets)
					{
						moraleItemWidget.ItemWidget.Brush.Color = this._teamColor;
						foreach (Style style in moraleItemWidget.ItemWidget.Brush.Styles)
						{
							foreach (StyleLayer styleLayer in style.Layers)
							{
								styleLayer.Color = this._teamColor;
							}
						}
					}
				}
			}
		}

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x06000951 RID: 2385 RVA: 0x0001ACB4 File Offset: 0x00018EB4
		// (set) Token: 0x06000952 RID: 2386 RVA: 0x0001ACBC File Offset: 0x00018EBC
		[DataSourceProperty]
		public string TeamColorAsStrSecondary
		{
			get
			{
				return this._teamColorAsStrSecondary;
			}
			set
			{
				if (this._teamColorAsStrSecondary != value && value != null)
				{
					this._teamColorAsStrSecondary = value;
					base.OnPropertyChanged<string>(value, "TeamColorAsStrSecondary");
					this._teamColorSecondary = Color.ConvertStringToColor(value);
					foreach (MoraleWidget.MoraleItemWidget moraleItemWidget in this._moraleItemWidgets)
					{
						moraleItemWidget.ItemGlowWidget.Brush.Color = this._teamColorSecondary;
						foreach (Style style in moraleItemWidget.ItemGlowWidget.Brush.Styles)
						{
							foreach (StyleLayer styleLayer in style.Layers)
							{
								styleLayer.Color = this._teamColorSecondary;
							}
						}
					}
				}
			}
		}

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x06000953 RID: 2387 RVA: 0x0001ADC0 File Offset: 0x00018FC0
		// (set) Token: 0x06000954 RID: 2388 RVA: 0x0001ADC8 File Offset: 0x00018FC8
		[DataSourceProperty]
		public MoraleArrowBrushWidget FlowArrowWidget
		{
			get
			{
				return this._flowArrowWidget;
			}
			set
			{
				if (this._flowArrowWidget != value)
				{
					this._flowArrowWidget = value;
					base.OnPropertyChanged<MoraleArrowBrushWidget>(value, "FlowArrowWidget");
				}
			}
		}

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x06000955 RID: 2389 RVA: 0x0001ADE6 File Offset: 0x00018FE6
		// (set) Token: 0x06000956 RID: 2390 RVA: 0x0001ADEE File Offset: 0x00018FEE
		[DataSourceProperty]
		public bool ExtendToLeft
		{
			get
			{
				return this._extendToLeft;
			}
			set
			{
				if (this._extendToLeft != value)
				{
					this._extendToLeft = value;
					base.OnPropertyChanged(value, "ExtendToLeft");
				}
			}
		}

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06000957 RID: 2391 RVA: 0x0001AE0C File Offset: 0x0001900C
		// (set) Token: 0x06000958 RID: 2392 RVA: 0x0001AE14 File Offset: 0x00019014
		[DataSourceProperty]
		public bool AreMoralesIndependent
		{
			get
			{
				return this._areMoralesIndependent;
			}
			set
			{
				if (this._areMoralesIndependent != value)
				{
					this._areMoralesIndependent = value;
					base.OnPropertyChanged(value, "AreMoralesIndependent");
				}
			}
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x0001AE34 File Offset: 0x00019034
		private float PingPong(float min, float max, float time)
		{
			float num = max - min;
			bool flag = (int)(time / num) % 2 == 0;
			float num2 = time % num;
			if (!flag)
			{
				return max - num2;
			}
			return num2 + min;
		}

		// Token: 0x04000434 RID: 1076
		private const int ItemCount = 10;

		// Token: 0x04000435 RID: 1077
		private const float ItemDistance = 28f;

		// Token: 0x04000436 RID: 1078
		private const int ItemWidth = 39;

		// Token: 0x04000437 RID: 1079
		private const int ItemHeight = 38;

		// Token: 0x04000438 RID: 1080
		private const int FillMargin = 12;

		// Token: 0x04000439 RID: 1081
		private MoraleWidget.MoraleItemWidget[] _moraleItemWidgets;

		// Token: 0x0400043A RID: 1082
		private bool _triggerAnimations;

		// Token: 0x0400043B RID: 1083
		private bool _initialized;

		// Token: 0x0400043C RID: 1084
		private int _triggerWaitPassedFrames;

		// Token: 0x0400043D RID: 1085
		private Color _teamColor;

		// Token: 0x0400043E RID: 1086
		private Color _teamColorSecondary;

		// Token: 0x0400043F RID: 1087
		private int _increaseLevel;

		// Token: 0x04000440 RID: 1088
		private int _moralePercentage;

		// Token: 0x04000441 RID: 1089
		private Widget _container;

		// Token: 0x04000442 RID: 1090
		private Widget _itemContainer;

		// Token: 0x04000443 RID: 1091
		private Brush _itemBrush;

		// Token: 0x04000444 RID: 1092
		private Brush _itemGlowBrush;

		// Token: 0x04000445 RID: 1093
		private Brush _itemBackgroundBrush;

		// Token: 0x04000446 RID: 1094
		private MoraleArrowBrushWidget _flowArrowWidget;

		// Token: 0x04000447 RID: 1095
		private bool _extendToLeft;

		// Token: 0x04000448 RID: 1096
		private bool _areMoralesIndependent;

		// Token: 0x04000449 RID: 1097
		private string _teamColorAsStr;

		// Token: 0x0400044A RID: 1098
		private string _teamColorAsStrSecondary;

		// Token: 0x0200018F RID: 399
		private class MoraleItemWidget
		{
			// Token: 0x170006A6 RID: 1702
			// (get) Token: 0x06001305 RID: 4869 RVA: 0x000342C0 File Offset: 0x000324C0
			// (set) Token: 0x06001306 RID: 4870 RVA: 0x000342C8 File Offset: 0x000324C8
			public Widget ParentWidget { get; private set; }

			// Token: 0x170006A7 RID: 1703
			// (get) Token: 0x06001307 RID: 4871 RVA: 0x000342D1 File Offset: 0x000324D1
			// (set) Token: 0x06001308 RID: 4872 RVA: 0x000342D9 File Offset: 0x000324D9
			public Widget MaskWidget { get; private set; }

			// Token: 0x170006A8 RID: 1704
			// (get) Token: 0x06001309 RID: 4873 RVA: 0x000342E2 File Offset: 0x000324E2
			// (set) Token: 0x0600130A RID: 4874 RVA: 0x000342EA File Offset: 0x000324EA
			public BrushWidget ItemWidget { get; private set; }

			// Token: 0x170006A9 RID: 1705
			// (get) Token: 0x0600130B RID: 4875 RVA: 0x000342F3 File Offset: 0x000324F3
			// (set) Token: 0x0600130C RID: 4876 RVA: 0x000342FB File Offset: 0x000324FB
			public BrushWidget ItemGlowWidget { get; private set; }

			// Token: 0x170006AA RID: 1706
			// (get) Token: 0x0600130D RID: 4877 RVA: 0x00034304 File Offset: 0x00032504
			// (set) Token: 0x0600130E RID: 4878 RVA: 0x0003430C File Offset: 0x0003250C
			public Widget ItemBackgroundWidget { get; private set; }

			// Token: 0x0600130F RID: 4879 RVA: 0x00034315 File Offset: 0x00032515
			public MoraleItemWidget(Widget parentWidget, Widget maskWidget, BrushWidget itemWidget, BrushWidget itemGlowWidget, Widget itemBackgroundWidget)
			{
				this.ParentWidget = parentWidget;
				this.MaskWidget = maskWidget;
				this.ItemWidget = itemWidget;
				this.ItemGlowWidget = itemGlowWidget;
				this.ItemBackgroundWidget = itemBackgroundWidget;
			}

			// Token: 0x06001310 RID: 4880 RVA: 0x00034344 File Offset: 0x00032544
			public void SetFillAmount(float fill, int fillMargin)
			{
				bool flag = MBMath.ApproximatelyEquals(fill, 0f, 1E-05f);
				bool flag2 = MBMath.ApproximatelyEquals(fill, 1f, 1E-05f);
				if (flag)
				{
					this.MaskWidget.SuggestedHeight = 0f;
				}
				else if (flag2)
				{
					this.MaskWidget.SuggestedHeight = this.ItemWidget.SuggestedHeight;
				}
				else
				{
					int num = MathF.Floor(this.ItemWidget.SuggestedHeight - (float)(fillMargin * 2));
					this.MaskWidget.SuggestedHeight = (float)fillMargin + (float)num * fill;
				}
				this.ItemWidget.IsVisible = !flag;
				this.ItemGlowWidget.IsVisible = flag2;
			}
		}
	}
}
