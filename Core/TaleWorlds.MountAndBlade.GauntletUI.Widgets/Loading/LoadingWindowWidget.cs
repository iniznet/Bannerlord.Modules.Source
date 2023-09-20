using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Loading
{
	public class LoadingWindowWidget : Widget
	{
		public LoadingWindowWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this._edgeOffsetPadding = base.EventManager.PageSize.X / 2.72f;
			if (this.AnimWidget != null && base.IsVisible && this.AnimWidget.IsVisible)
			{
				this.AnimWidget.ScaledPositionXOffset = MathF.PingPong(this._edgeOffsetPadding, base.EventManager.PageSize.X - this.AnimWidget.Size.X - this._edgeOffsetPadding, this._totalDt);
				this._totalDt += dt * 500f;
			}
		}

		private void UpdateStates()
		{
			base.IsVisible = this.IsActive;
			base.IsEnabled = this.IsActive;
			base.ParentWidget.IsVisible = this.IsActive;
			base.ParentWidget.IsEnabled = this.IsActive;
		}

		private void UpdateImage(string imageName)
		{
			Sprite sprite = base.Context.SpriteData.GetSprite(imageName);
			if (sprite == null)
			{
				base.Sprite = base.Context.SpriteData.GetSprite("background_1");
				return;
			}
			base.Sprite = sprite;
		}

		[Editor(false)]
		public Widget AnimWidget
		{
			get
			{
				return this._animWidget;
			}
			set
			{
				if (this._animWidget != value)
				{
					this._animWidget = value;
					base.OnPropertyChanged<Widget>(value, "AnimWidget");
				}
			}
		}

		[Editor(false)]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (this._isActive != value)
				{
					this._isActive = value;
					base.OnPropertyChanged(value, "IsActive");
					this.UpdateStates();
				}
			}
		}

		[Editor(false)]
		public string ImageName
		{
			get
			{
				return this._imageName;
			}
			set
			{
				if (this._imageName != value)
				{
					this._imageName = value;
					base.OnPropertyChanged<string>(value, "ImageName");
					this.UpdateImage(value);
				}
			}
		}

		private const string _defaultBackgroundSpriteData = "background_1";

		private float _edgeOffsetPadding;

		private float _totalDt;

		private Widget _animWidget;

		private bool _isActive;

		private string _imageName;
	}
}
