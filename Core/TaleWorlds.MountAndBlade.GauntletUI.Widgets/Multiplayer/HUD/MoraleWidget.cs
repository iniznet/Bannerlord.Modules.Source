using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.HUD
{
	public class MoraleWidget : Widget
	{
		public MoraleWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			this._moraleItemWidgets = this.CreateItemWidgets(this.ItemContainer);
			this.SetItemWidgetColors(this._teamColor);
			this.SetItemGlowWidgetColors(this._teamColorSecondary);
			this.RestartAnimations();
		}

		protected override void OnUpdate(float dt)
		{
			if (this._triggerAnimations)
			{
				if (this._animWaitFrame >= 1)
				{
					this.HandleAnimation();
					this._triggerAnimations = false;
					this._animWaitFrame = 0;
				}
				else
				{
					this._animWaitFrame++;
				}
			}
			if (!this._initialized)
			{
				this.FlowArrowWidget.LeftSideArrow = this.ExtendToLeft;
				this._initialized = true;
			}
		}

		private void RestartAnimations()
		{
			this._triggerAnimations = true;
			this._animWaitFrame = 0;
		}

		private void UpdateArrows(int flowLevel)
		{
			if (this.Container == null || this.FlowArrowWidget == null)
			{
				return;
			}
			this.FlowArrowWidget.SetFlowLevel(flowLevel);
		}

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
						this.RestartAnimations();
					}
				}
				else if (i == num)
				{
					float num3 = 10f;
					num2 = ((float)this.MoralePercentage - (float)num * num3) / num3;
					if (!moraleItemWidget.ItemWidget.IsVisible && !MBMath.ApproximatelyEquals(num2, 0f, 1E-05f))
					{
						this.RestartAnimations();
					}
				}
				moraleItemWidget.SetFillAmount(num2, 12);
			}
		}

		private string GetCurrentStateName()
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
			return text;
		}

		private void HandleAnimation()
		{
			for (int i = 0; i < this._moraleItemWidgets.Length; i++)
			{
				MoraleWidget.MoraleItemWidget moraleItemWidget = this._moraleItemWidgets[i];
				moraleItemWidget.ItemWidget.SetState(this._currentStateName);
				moraleItemWidget.ItemWidget.BrushRenderer.RestartAnimation();
				moraleItemWidget.ItemGlowWidget.SetState(this._currentStateName);
				moraleItemWidget.ItemGlowWidget.BrushRenderer.RestartAnimation();
			}
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.UpdateMoraleMask();
			string currentStateName = this.GetCurrentStateName();
			if (this._currentStateName != currentStateName)
			{
				this._currentStateName = currentStateName;
				this.RestartAnimations();
			}
		}

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
				widget.AddState("IsCriticalAnim");
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
				brushWidget.AddState("IsCriticalAnim");
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
				brushWidget3.AddState("IsCriticalAnim");
				widget2.AddChild(brushWidget3);
				array[i] = new MoraleWidget.MoraleItemWidget(widget, widget2, brushWidget3, brushWidget, brushWidget2);
			}
			return array;
		}

		private void SetItemWidgetColors(Color color)
		{
			if (this._moraleItemWidgets != null)
			{
				foreach (MoraleWidget.MoraleItemWidget moraleItemWidget in this._moraleItemWidgets)
				{
					this.SetSingleItemWidgetColor(moraleItemWidget, color);
				}
			}
		}

		private void SetSingleItemWidgetColor(MoraleWidget.MoraleItemWidget widget, Color color)
		{
			widget.ItemWidget.Brush.Color = color;
			foreach (Style style in widget.ItemWidget.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Color = color;
				}
			}
		}

		private void SetItemGlowWidgetColors(Color color)
		{
			if (this._moraleItemWidgets != null)
			{
				foreach (MoraleWidget.MoraleItemWidget moraleItemWidget in this._moraleItemWidgets)
				{
					this.SetSingleItemGlowWidgetColor(moraleItemWidget, color);
				}
			}
		}

		private void SetSingleItemGlowWidgetColor(MoraleWidget.MoraleItemWidget widget, Color color)
		{
			widget.ItemGlowWidget.Brush.Color = color;
			foreach (Style style in widget.ItemGlowWidget.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Color = color;
				}
			}
		}

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
				}
			}
		}

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
				}
			}
		}

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
					this.SetItemWidgetColors(this._teamColor);
				}
			}
		}

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
					this.SetItemGlowWidgetColors(this._teamColorSecondary);
				}
			}
		}

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

		private const int ItemCount = 10;

		private const float ItemDistance = 28f;

		private const int ItemWidth = 39;

		private const int ItemHeight = 38;

		private const int FillMargin = 12;

		private MoraleWidget.MoraleItemWidget[] _moraleItemWidgets;

		private bool _initialized;

		private bool _triggerAnimations;

		private int _animWaitFrame;

		private string _currentStateName;

		private Color _teamColor;

		private Color _teamColorSecondary;

		private int _increaseLevel;

		private int _moralePercentage;

		private Widget _container;

		private Widget _itemContainer;

		private Brush _itemBrush;

		private Brush _itemGlowBrush;

		private Brush _itemBackgroundBrush;

		private MoraleArrowBrushWidget _flowArrowWidget;

		private bool _extendToLeft;

		private bool _areMoralesIndependent;

		private string _teamColorAsStr;

		private string _teamColorAsStrSecondary;

		private class MoraleItemWidget
		{
			public Widget ParentWidget { get; private set; }

			public Widget MaskWidget { get; private set; }

			public BrushWidget ItemWidget { get; private set; }

			public BrushWidget ItemGlowWidget { get; private set; }

			public Widget ItemBackgroundWidget { get; private set; }

			public MoraleItemWidget(Widget parentWidget, Widget maskWidget, BrushWidget itemWidget, BrushWidget itemGlowWidget, Widget itemBackgroundWidget)
			{
				this.ParentWidget = parentWidget;
				this.MaskWidget = maskWidget;
				this.ItemWidget = itemWidget;
				this.ItemGlowWidget = itemGlowWidget;
				this.ItemBackgroundWidget = itemBackgroundWidget;
			}

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
