using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar
{
	public class MapInfoBarWidget : Widget
	{
		public event MapInfoBarWidget.MapBarExtendStateChangeEvent OnMapInfoBarExtendStateChange;

		public MapInfoBarWidget(UIContext context)
			: base(context)
		{
			base.AddState("Disabled");
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.RefreshBarExtendState();
		}

		private void OnExtendButtonClick(Widget widget)
		{
			this.IsInfoBarExtended = !this.IsInfoBarExtended;
			this.RefreshBarExtendState();
		}

		private void RefreshBarExtendState()
		{
			if (this.IsInfoBarExtended && base.CurrentState != "Extended")
			{
				this.SetState("Extended");
				this.RefreshVerticalVisual();
				return;
			}
			if (!this.IsInfoBarExtended && base.CurrentState != "Default")
			{
				this.SetState("Default");
				this.RefreshVerticalVisual();
			}
		}

		private void RefreshVerticalVisual()
		{
			foreach (Style style in this.ExtendButtonWidget.Brush.Styles)
			{
				for (int i = 0; i < style.LayerCount; i++)
				{
					style.GetLayer(i).VerticalFlip = this.IsInfoBarExtended;
				}
			}
		}

		[Editor(false)]
		public ButtonWidget ExtendButtonWidget
		{
			get
			{
				return this._extendButtonWidget;
			}
			set
			{
				if (this._extendButtonWidget != value)
				{
					this._extendButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ExtendButtonWidget");
					if (!this._extendButtonWidget.ClickEventHandlers.Contains(new Action<Widget>(this.OnExtendButtonClick)))
					{
						this._extendButtonWidget.ClickEventHandlers.Add(new Action<Widget>(this.OnExtendButtonClick));
					}
				}
			}
		}

		[Editor(false)]
		public bool IsInfoBarExtended
		{
			get
			{
				return this._isInfoBarExtended;
			}
			set
			{
				if (this._isInfoBarExtended != value)
				{
					this._isInfoBarExtended = value;
					base.OnPropertyChanged(value, "IsInfoBarExtended");
					MapInfoBarWidget.MapBarExtendStateChangeEvent onMapInfoBarExtendStateChange = this.OnMapInfoBarExtendStateChange;
					if (onMapInfoBarExtendStateChange == null)
					{
						return;
					}
					onMapInfoBarExtendStateChange(this.IsInfoBarExtended);
				}
			}
		}

		private ButtonWidget _extendButtonWidget;

		private bool _isInfoBarExtended;

		public delegate void MapBarExtendStateChangeEvent(bool newState);
	}
}
