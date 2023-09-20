using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Nameplate
{
	public class PartyNameplateWidget : Widget
	{
		public PartyNameplateWidget(UIContext context)
			: base(context)
		{
		}

		private float _animSpeedModifier
		{
			get
			{
				return 8f;
			}
		}

		private int _armyFontSizeOffset
		{
			get
			{
				return 10;
			}
		}

		public Widget HeadGroupWidget { get; set; }

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isFirstFrame)
			{
				this.NameplateFullNameTextWidget.Brush.GlobalAlphaFactor = 0f;
				this.NameplateTextWidget.Brush.GlobalAlphaFactor = 0f;
				this.NameplateExtraInfoTextWidget.Brush.GlobalAlphaFactor = 0f;
				this.PartyBannerWidget.Brush.GlobalAlphaFactor = 0f;
				this.SpeedTextWidget.AlphaFactor = 0f;
				this._defaultNameplateFontSize = this.NameplateTextWidget.ReadOnlyBrush.FontSize;
				this._isFirstFrame = false;
			}
			int num = (this.IsArmy ? (this._defaultNameplateFontSize + this._armyFontSizeOffset) : this._defaultNameplateFontSize);
			if (this.NameplateTextWidget.Brush.FontSize != num)
			{
				this.NameplateTextWidget.Brush.FontSize = num;
			}
			this.UpdateNameplatesScreenPosition();
			this.UpdateNameplatesVisibility(dt);
			this.UpdateTutorialStatus();
		}

		private void UpdateNameplatesVisibility(float dt)
		{
			float num = 0f;
			float num2;
			if (this.IsMainParty)
			{
				this._latestIsOutside = this.IsNameplateOutsideScreen();
				this.MainPartyArrowWidget.IsVisible = this._latestIsOutside;
				this.NameplateTextWidget.IsVisible = !this._latestIsOutside && !this.IsInArmy && !this.IsPrisoner && !this.IsInSettlement;
				this.NameplateFullNameTextWidget.IsVisible = !this._latestIsOutside && !this.IsInArmy && !this.IsPrisoner && !this.IsInSettlement;
				this.SpeedTextWidget.IsVisible = !this._latestIsOutside && !this.IsInArmy && !this.IsPrisoner && !this.IsInSettlement;
				this.SpeedIconWidget.IsVisible = !this._latestIsOutside && !this.IsInArmy && !this.IsPrisoner && !this.IsInSettlement;
				this.TrackerFrame.IsVisible = this._latestIsOutside;
				num2 = (float)((!this._latestIsOutside && !this.IsInArmy && !this.IsPrisoner && !this.IsInSettlement) ? 1 : 0);
				this.PartyBannerWidget.IsVisible = !this._latestIsOutside && !this.IsInArmy && !this.IsPrisoner && !this.IsInSettlement;
				this.NameplateExtraInfoTextWidget.IsVisible = !this._latestIsOutside && !this.IsInArmy && !this.IsPrisoner && !this.IsInSettlement;
				base.IsEnabled = this._latestIsOutside;
			}
			else
			{
				this.MainPartyArrowWidget.IsVisible = false;
				this.NameplateTextWidget.IsVisible = true;
				this.NameplateFullNameTextWidget.IsVisible = true;
				this.SpeedTextWidget.IsVisible = true;
				this.SpeedIconWidget.IsVisible = true;
				this.TrackerFrame.IsVisible = false;
				this.PartyBannerWidget.IsVisible = true;
				num2 = 1f;
				base.IsEnabled = false;
			}
			if (!this.IsVisibleOnMap && !this.IsMainParty)
			{
				this.NameplateTextWidget.IsVisible = false;
				this.NameplateFullNameTextWidget.IsVisible = false;
				this.SpeedTextWidget.IsVisible = false;
				this.SpeedIconWidget.IsVisible = false;
				num2 = 0f;
			}
			else
			{
				this._initialDelayAmount -= dt;
				if (this._initialDelayAmount <= 0f)
				{
					num = (float)(this.ShouldShowFullName ? 1 : 0);
				}
				else
				{
					num = 1f;
				}
			}
			this.NameplateTextWidget.Brush.GlobalAlphaFactor = this.LocalLerp(this.NameplateTextWidget.ReadOnlyBrush.GlobalAlphaFactor, num2, dt * this._animSpeedModifier);
			this.NameplateFullNameTextWidget.Brush.GlobalAlphaFactor = this.LocalLerp(this.NameplateFullNameTextWidget.ReadOnlyBrush.GlobalAlphaFactor, num, dt * this._animSpeedModifier);
			this.SpeedTextWidget.Brush.GlobalAlphaFactor = this.LocalLerp(this.SpeedTextWidget.ReadOnlyBrush.GlobalAlphaFactor, num, dt * this._animSpeedModifier);
			this.SpeedIconWidget.AlphaFactor = this.LocalLerp(this.SpeedIconWidget.AlphaFactor, num, dt * this._animSpeedModifier);
			this.NameplateExtraInfoTextWidget.Brush.GlobalAlphaFactor = this.LocalLerp(this.NameplateExtraInfoTextWidget.ReadOnlyBrush.GlobalAlphaFactor, (float)(this.ShouldShowFullName ? 1 : 0), dt * this._animSpeedModifier);
			this.PartyBannerWidget.Brush.GlobalAlphaFactor = this.LocalLerp(this.PartyBannerWidget.ReadOnlyBrush.GlobalAlphaFactor, num2, dt * this._animSpeedModifier);
		}

		private void UpdateNameplatesScreenPosition()
		{
			this._screenWidth = base.Context.EventManager.PageSize.X;
			this._screenHeight = base.Context.EventManager.PageSize.Y;
			if (!this.IsVisibleOnMap && !this.IsMainParty)
			{
				base.IsHidden = true;
				return;
			}
			if (this.IsMainParty)
			{
				if (!this.IsBehind && this.Position.x + base.Size.X <= this._screenWidth && this.Position.y - base.Size.Y <= this._screenHeight && this.Position.x >= 0f && this.Position.y >= 0f)
				{
					Widget headGroupWidget = this.HeadGroupWidget;
					float num = ((headGroupWidget != null) ? headGroupWidget.Size.Y : 0f);
					this.NameplateLayoutListPanel.ScaledPositionYOffset = this.Position.y - this.HeadPosition.y + num;
					if (this.IsHigh)
					{
						base.ScaledPositionXOffset = MathF.Clamp(this.HeadPosition.x - base.Size.X / 2f, 0f, this._screenWidth - base.Size.X);
					}
					else
					{
						base.ScaledPositionXOffset = MathF.Clamp(this.HeadPosition.x - base.Size.X / 2f, 0f, this._screenWidth - base.Size.X);
					}
					base.ScaledPositionYOffset = this.HeadPosition.y - num;
				}
				else
				{
					Vec2 vec = new Vec2(base.Context.EventManager.PageSize.X / 2f, base.Context.EventManager.PageSize.Y / 2f);
					Vec2 vec2 = this.HeadPosition;
					vec2 -= vec;
					if (this.IsBehind)
					{
						vec2 *= -1f;
					}
					float num2 = Mathf.Atan2(vec2.y, vec2.x) - 1.5707964f;
					float num3 = Mathf.Cos(num2);
					float num4 = Mathf.Sin(num2);
					vec2 = vec + new Vec2(num4 * 150f, num3 * 150f);
					float num5 = num3 / num4;
					Vec2 vec3 = vec * 1f;
					vec2 = ((num3 > 0f) ? new Vec2(-vec3.y / num5, vec.y) : new Vec2(vec3.y / num5, -vec.y));
					if (vec2.x > vec3.x)
					{
						vec2 = new Vec2(vec3.x, -vec3.x * num5);
					}
					else if (vec2.x < -vec3.x)
					{
						vec2 = new Vec2(-vec3.x, vec3.x * num5);
					}
					vec2 += vec;
					base.ScaledPositionXOffset = MathF.Clamp(vec2.x - base.Size.X / 2f, 0f, this._screenWidth - base.Size.X);
					base.ScaledPositionYOffset = MathF.Clamp(vec2.y - base.Size.Y / 2f, 0f, this._screenHeight - base.Size.Y);
				}
			}
			else
			{
				Widget headGroupWidget2 = this.HeadGroupWidget;
				float num6 = ((headGroupWidget2 != null) ? headGroupWidget2.Size.Y : 0f);
				this.NameplateLayoutListPanel.ScaledPositionYOffset = this.Position.y - this.HeadPosition.y + num6;
				base.ScaledPositionXOffset = this.HeadPosition.x - base.Size.X / 2f;
				base.ScaledPositionYOffset = this.HeadPosition.y - num6;
				base.IsHidden = base.ScaledPositionXOffset > base.Context.TwoDimensionContext.Width || base.ScaledPositionYOffset > base.Context.TwoDimensionContext.Height || base.ScaledPositionXOffset + base.Size.X < 0f || base.ScaledPositionYOffset + base.Size.Y < 0f;
			}
			this.NameplateLayoutListPanel.PositionXOffset = (base.Size.X / 2f - this.PartyBannerWidget.Size.X) * base._inverseScaleToUse;
		}

		private void UpdateTutorialStatus()
		{
			if (this._tutorialAnimState == PartyNameplateWidget.TutorialAnimState.Start)
			{
				this._tutorialAnimState = PartyNameplateWidget.TutorialAnimState.FirstFrame;
			}
			else
			{
				PartyNameplateWidget.TutorialAnimState tutorialAnimState = this._tutorialAnimState;
			}
			if (this.IsTargetedByTutorial)
			{
				this.SetState("Default");
				return;
			}
			this.SetState("Disabled");
		}

		private bool IsNameplateOutsideScreen()
		{
			return this.Position.x + base.Size.X > this._screenWidth || this.Position.y - base.Size.Y > this._screenHeight || this.Position.x < 0f || this.Position.y < 0f || this.IsBehind || this.IsHigh;
		}

		private float LocalLerp(float start, float end, float delta)
		{
			if (Math.Abs(start - end) > 1E-45f)
			{
				return (end - start) * delta + start;
			}
			return end;
		}

		public ListPanel NameplateLayoutListPanel
		{
			get
			{
				return this._nameplateLayoutListPanel;
			}
			set
			{
				if (this._nameplateLayoutListPanel != value)
				{
					this._nameplateLayoutListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "NameplateLayoutListPanel");
				}
			}
		}

		public MaskedTextureWidget PartyBannerWidget
		{
			get
			{
				return this._partyBannerWidget;
			}
			set
			{
				if (this._partyBannerWidget != value)
				{
					this._partyBannerWidget = value;
					base.OnPropertyChanged<MaskedTextureWidget>(value, "PartyBannerWidget");
				}
			}
		}

		public Widget TrackerFrame
		{
			get
			{
				return this._trackerFrame;
			}
			set
			{
				if (this._trackerFrame != value)
				{
					this._trackerFrame = value;
					base.OnPropertyChanged<Widget>(value, "TrackerFrame");
				}
			}
		}

		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (this._position != value)
				{
					this._position = value;
					base.OnPropertyChanged(value, "Position");
				}
			}
		}

		public Vec2 HeadPosition
		{
			get
			{
				return this._headPosition;
			}
			set
			{
				if (this._headPosition != value)
				{
					this._headPosition = value;
					base.OnPropertyChanged(value, "HeadPosition");
				}
			}
		}

		public bool ShouldShowFullName
		{
			get
			{
				return this._shouldShowFullName;
			}
			set
			{
				if (this._shouldShowFullName != value)
				{
					this._shouldShowFullName = value;
					base.OnPropertyChanged(value, "ShouldShowFullName");
				}
			}
		}

		public bool IsTargetedByTutorial
		{
			get
			{
				return this._isTargetedByTutorial;
			}
			set
			{
				if (this._isTargetedByTutorial != value)
				{
					this._isTargetedByTutorial = value;
					base.OnPropertyChanged(value, "IsTargetedByTutorial");
					this._tutorialAnimState = PartyNameplateWidget.TutorialAnimState.Start;
				}
			}
		}

		public bool IsInArmy
		{
			get
			{
				return this._isInArmy;
			}
			set
			{
				if (this._isInArmy != value)
				{
					this._isInArmy = value;
					base.OnPropertyChanged(value, "IsInArmy");
				}
			}
		}

		public bool IsInSettlement
		{
			get
			{
				return this._isInSettlement;
			}
			set
			{
				if (this._isInSettlement != value)
				{
					this._isInSettlement = value;
					base.OnPropertyChanged(value, "IsInSettlement");
				}
			}
		}

		public bool IsArmy
		{
			get
			{
				return this._isArmy;
			}
			set
			{
				if (this._isArmy != value)
				{
					this._isArmy = value;
					base.OnPropertyChanged(value, "IsArmy");
				}
			}
		}

		public bool IsVisibleOnMap
		{
			get
			{
				return this._isVisibleOnMap;
			}
			set
			{
				if (this._isVisibleOnMap != value)
				{
					this._isVisibleOnMap = value;
					base.OnPropertyChanged(value, "IsVisibleOnMap");
				}
			}
		}

		public bool IsMainParty
		{
			get
			{
				return this._isMainParty;
			}
			set
			{
				if (this._isMainParty != value)
				{
					this._isMainParty = value;
					base.OnPropertyChanged(value, "IsMainParty");
				}
			}
		}

		public bool IsInside
		{
			get
			{
				return this._isInside;
			}
			set
			{
				if (this._isInside != value)
				{
					this._isInside = value;
					base.OnPropertyChanged(value, "IsInside");
				}
			}
		}

		public bool IsHigh
		{
			get
			{
				return this._isHigh;
			}
			set
			{
				if (this._isHigh != value)
				{
					this._isHigh = value;
					base.OnPropertyChanged(value, "IsHigh");
				}
			}
		}

		public bool IsBehind
		{
			get
			{
				return this._isBehind;
			}
			set
			{
				if (this._isBehind != value)
				{
					this._isBehind = value;
					base.OnPropertyChanged(value, "IsBehind");
				}
			}
		}

		public bool IsPrisoner
		{
			get
			{
				return this._isPrisoner;
			}
			set
			{
				if (this._isPrisoner != value)
				{
					this._isPrisoner = value;
					base.OnPropertyChanged(value, "IsPrisoner");
				}
			}
		}

		public TextWidget NameplateTextWidget
		{
			get
			{
				return this._nameplateTextWidget;
			}
			set
			{
				if (this._nameplateTextWidget != value)
				{
					this._nameplateTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "NameplateTextWidget");
				}
			}
		}

		public TextWidget NameplateExtraInfoTextWidget
		{
			get
			{
				return this._nameplateExtraInfoTextWidget;
			}
			set
			{
				if (this._nameplateExtraInfoTextWidget != value)
				{
					this._nameplateExtraInfoTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "NameplateExtraInfoTextWidget");
				}
			}
		}

		public TextWidget NameplateFullNameTextWidget
		{
			get
			{
				return this._nameplateFullNameTextWidget;
			}
			set
			{
				if (this._nameplateFullNameTextWidget != value)
				{
					this._nameplateFullNameTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "NameplateFullNameTextWidget");
				}
			}
		}

		public TextWidget SpeedTextWidget
		{
			get
			{
				return this._speedTextWidget;
			}
			set
			{
				if (this._speedTextWidget != value)
				{
					this._speedTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "SpeedTextWidget");
				}
			}
		}

		public Widget SpeedIconWidget
		{
			get
			{
				return this._speedIconWidget;
			}
			set
			{
				if (value != this._speedIconWidget)
				{
					this._speedIconWidget = value;
					base.OnPropertyChanged<Widget>(value, "SpeedIconWidget");
				}
			}
		}

		public Widget MainPartyArrowWidget
		{
			get
			{
				return this._mainPartyArrowWidget;
			}
			set
			{
				if (this._mainPartyArrowWidget != value)
				{
					this._mainPartyArrowWidget = value;
					base.OnPropertyChanged<Widget>(value, "MainPartyArrowWidget");
				}
			}
		}

		private bool _isFirstFrame = true;

		private float _screenWidth;

		private float _screenHeight;

		private bool _latestIsOutside;

		private float _initialDelayAmount = 2f;

		private int _defaultNameplateFontSize;

		private PartyNameplateWidget.TutorialAnimState _tutorialAnimState;

		private Vec2 _position;

		private Vec2 _headPosition;

		private TextWidget _nameplateTextWidget;

		private TextWidget _nameplateFullNameTextWidget;

		private TextWidget _speedTextWidget;

		private Widget _speedIconWidget;

		private TextWidget _nameplateExtraInfoTextWidget;

		private Widget _trackerFrame;

		private Widget _mainPartyArrowWidget;

		private ListPanel _nameplateLayoutListPanel;

		private MaskedTextureWidget _partyBannerWidget;

		private bool _isVisibleOnMap;

		private bool _isMainParty;

		private bool _isInside;

		private bool _isBehind;

		private bool _isHigh;

		private bool _isInArmy;

		private bool _isInSettlement;

		private bool _isArmy;

		private bool _isTargetedByTutorial;

		private bool _shouldShowFullName;

		private bool _isPrisoner;

		public enum TutorialAnimState
		{
			Idle,
			Start,
			FirstFrame,
			Playing
		}
	}
}
