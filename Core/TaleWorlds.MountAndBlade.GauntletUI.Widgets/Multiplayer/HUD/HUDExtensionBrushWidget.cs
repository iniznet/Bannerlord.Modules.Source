using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.HUD
{
	public class HUDExtensionBrushWidget : BrushWidget
	{
		public float AlphaChangeDuration { get; set; } = 0.15f;

		public float OrderEnabledAlpha { get; set; } = 0.3f;

		public HUDExtensionBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._currentAlpha - this._targetAlpha > 1E-45f)
			{
				if (this._alphaChangeTimeElapsed < this.AlphaChangeDuration)
				{
					this._currentAlpha = MathF.Lerp(this._initialAlpha, this._targetAlpha, this._alphaChangeTimeElapsed / this.AlphaChangeDuration, 1E-05f);
					this.SetGlobalAlphaRecursively(this._currentAlpha);
					this._alphaChangeTimeElapsed += dt;
					return;
				}
			}
			else if (this._currentAlpha != this._targetAlpha)
			{
				this._currentAlpha = this._targetAlpha;
				this.SetGlobalAlphaRecursively(this._targetAlpha);
			}
		}

		private void OnIsOrderEnabledChanged()
		{
			this._alphaChangeTimeElapsed = 0f;
			this._targetAlpha = (this.IsOrderActive ? this.OrderEnabledAlpha : 1f);
			this._initialAlpha = this._currentAlpha;
		}

		[Editor(false)]
		public bool IsOrderActive
		{
			get
			{
				return this._isOrderActive;
			}
			set
			{
				if (this._isOrderActive != value)
				{
					this._isOrderActive = value;
					base.OnPropertyChanged(value, "IsOrderActive");
					this.OnIsOrderEnabledChanged();
				}
			}
		}

		private float _alphaChangeTimeElapsed;

		private float _initialAlpha = 1f;

		private float _targetAlpha = 1f;

		private float _currentAlpha = 1f;

		private bool _isOrderActive;
	}
}
