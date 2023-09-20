using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map
{
	public class MobilePartyTrackerItemWidget : Widget
	{
		public Widget FrameVisualWidget { get; set; }

		public MobilePartyTrackerItemWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.FrameVisualWidget.Sprite = base.Context.SpriteData.GetSprite(this.IsArmy ? "army_track_frame_9" : "party_track_frame_9");
				this._initialized = true;
			}
			this.UpdateScreenPosition();
		}

		private void UpdateScreenPosition()
		{
			this._screenWidth = base.Context.EventManager.PageSize.X;
			this._screenHeight = base.Context.EventManager.PageSize.Y;
			if (!this.IsActive)
			{
				base.IsHidden = true;
				return;
			}
			Vec2 vec = new Vec2(this.Position);
			if (this.IsTracked)
			{
				if (!this.IsBehind && vec.X - base.Size.X / 2f > 0f && vec.x + base.Size.X / 2f < base.Context.EventManager.PageSize.X && vec.y > 0f && vec.y + base.Size.Y < base.Context.EventManager.PageSize.Y)
				{
					base.ScaledPositionXOffset = vec.x - base.Size.X / 2f;
					base.ScaledPositionYOffset = vec.y;
				}
				else
				{
					Vec2 vec2 = new Vec2(base.Context.EventManager.PageSize.X / 2f, base.Context.EventManager.PageSize.Y / 2f);
					vec -= vec2;
					if (this.IsBehind)
					{
						vec *= -1f;
					}
					float num = Mathf.Atan2(vec.y, vec.x) - 1.5707964f;
					float num2 = Mathf.Cos(num);
					float num3 = Mathf.Sin(num);
					float num4 = num2 / num3;
					Vec2 vec3 = vec2 * 1f;
					vec = ((num2 > 0f) ? new Vec2(-vec3.y / num4, vec2.y) : new Vec2(vec3.y / num4, -vec2.y));
					if (vec.x > vec3.x)
					{
						vec = new Vec2(vec3.x, -vec3.x * num4);
					}
					else if (vec.x < -vec3.x)
					{
						vec = new Vec2(-vec3.x, vec3.x * num4);
					}
					vec += vec2;
					base.ScaledPositionXOffset = Mathf.Clamp(vec.x - base.Size.X / 2f, 0f, this._screenWidth - base.Size.X);
					base.ScaledPositionYOffset = Mathf.Clamp(vec.y, 0f, this._screenHeight - (base.Size.Y + 55f));
				}
			}
			else
			{
				base.ScaledPositionXOffset = this.Position.x - base.Size.X / 2f;
				base.ScaledPositionYOffset = this.Position.y;
			}
			base.IsHidden = (!this.IsTracked && this.IsBehind) || base.ScaledPositionXOffset > base.Context.TwoDimensionContext.Width || base.ScaledPositionYOffset > base.Context.TwoDimensionContext.Height || base.ScaledPositionXOffset + base.Size.X < 0f || base.ScaledPositionYOffset + base.Size.Y < 0f;
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
					this._initialized = false;
				}
			}
		}

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

		public bool IsTracked
		{
			get
			{
				return this._isTracked;
			}
			set
			{
				if (this._isTracked != value)
				{
					this._isTracked = value;
					base.OnPropertyChanged(value, "IsTracked");
				}
			}
		}

		private float _screenWidth;

		private float _screenHeight;

		private bool _initialized;

		private Vec2 _position;

		private bool _isActive;

		private bool _isBehind;

		private bool _isArmy;

		private bool _isTracked;

		private bool _shouldShowFullName;
	}
}
