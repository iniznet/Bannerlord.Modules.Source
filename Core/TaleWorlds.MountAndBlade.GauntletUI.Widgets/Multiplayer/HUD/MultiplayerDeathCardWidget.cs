using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.HUD
{
	public class MultiplayerDeathCardWidget : Widget
	{
		public TextWidget WeaponTextWidget { get; set; }

		public TextWidget TitleTextWidget { get; set; }

		public ScrollingRichTextWidget KillerNameTextWidget { get; set; }

		public Widget KillCountContainer { get; set; }

		public Brush SelfInflictedTitleBrush { get; set; }

		public Brush NormalBrushTitleBrush { get; set; }

		public float FadeInModifier { get; set; } = 2f;

		public float FadeOutModifier { get; set; } = 10f;

		public float StayTime { get; set; } = 7f;

		public MultiplayerDeathCardWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this._initialized = true;
				base.IsEnabled = false;
				this._initAlpha = base.AlphaFactor;
				this.SetGlobalAlphaRecursively(this._targetAlpha);
			}
			if (Math.Abs(base.AlphaFactor - this._targetAlpha) > 1E-45f)
			{
				float num = ((base.AlphaFactor > this._targetAlpha) ? this.FadeOutModifier : this.FadeInModifier);
				float num2 = Mathf.Lerp(base.AlphaFactor, this._targetAlpha, dt * num);
				this.SetGlobalAlphaRecursively(num2);
			}
			if ((this.IsActive && base.AlphaFactor < 1E-45f) || base.Context.EventManager.Time - this._activeTimeStart > this.StayTime)
			{
				this.IsActive = false;
			}
		}

		private void HandleIsActiveToggle(bool isActive)
		{
			this._targetAlpha = (isActive ? 1f : 0f);
			if (isActive)
			{
				this._activeTimeStart = base.Context.EventManager.Time;
			}
			this.KillCountContainer.IsVisible = !this.IsSelfInflicted && this.KillCountsEnabled;
		}

		private void HandleSelfInflictedToggle(bool isSelfInflicted)
		{
			this.TitleTextWidget.IsVisible = true;
			this.TitleTextWidget.Brush = (isSelfInflicted ? this.SelfInflictedTitleBrush : this.NormalBrushTitleBrush);
			this.KillerNameTextWidget.IsVisible = !isSelfInflicted;
			this.WeaponTextWidget.IsVisible = !isSelfInflicted;
			this.KillCountContainer.IsVisible = !this.IsSelfInflicted && this.KillCountsEnabled;
		}

		private void HandleKillCountsEnabledSwitch(bool killCountsEnabled)
		{
			this.KillCountContainer.IsVisible = killCountsEnabled;
		}

		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChanged(value, "IsActive");
					this.HandleIsActiveToggle(value);
				}
			}
		}

		public bool IsSelfInflicted
		{
			get
			{
				return this._isSelfInflicted;
			}
			set
			{
				if (value != this._isSelfInflicted)
				{
					this._isSelfInflicted = value;
					base.OnPropertyChanged(value, "IsSelfInflicted");
					this.HandleSelfInflictedToggle(value);
				}
			}
		}

		public bool KillCountsEnabled
		{
			get
			{
				return this._killCountsEnabled;
			}
			set
			{
				if (value != this._killCountsEnabled)
				{
					this._killCountsEnabled = value;
					base.OnPropertyChanged(value, "KillCountsEnabled");
					this.HandleKillCountsEnabledSwitch(value);
				}
			}
		}

		private float _targetAlpha;

		private float _initAlpha;

		private float _activeTimeStart;

		private bool _initialized;

		private bool _isActive;

		private bool _isSelfInflicted;

		private bool _killCountsEnabled;
	}
}
