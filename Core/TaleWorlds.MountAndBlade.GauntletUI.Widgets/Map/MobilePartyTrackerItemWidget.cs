using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map
{
	// Token: 0x020000FA RID: 250
	public class MobilePartyTrackerItemWidget : Widget
	{
		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x06000CEA RID: 3306 RVA: 0x00024224 File Offset: 0x00022424
		// (set) Token: 0x06000CEB RID: 3307 RVA: 0x0002422C File Offset: 0x0002242C
		public Widget FrameVisualWidget { get; set; }

		// Token: 0x06000CEC RID: 3308 RVA: 0x00024235 File Offset: 0x00022435
		public MobilePartyTrackerItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000CED RID: 3309 RVA: 0x00024240 File Offset: 0x00022440
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

		// Token: 0x06000CEE RID: 3310 RVA: 0x00024298 File Offset: 0x00022498
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

		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x06000CEF RID: 3311 RVA: 0x000245FF File Offset: 0x000227FF
		// (set) Token: 0x06000CF0 RID: 3312 RVA: 0x00024607 File Offset: 0x00022807
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

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x06000CF1 RID: 3313 RVA: 0x0002462A File Offset: 0x0002282A
		// (set) Token: 0x06000CF2 RID: 3314 RVA: 0x00024632 File Offset: 0x00022832
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

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x06000CF3 RID: 3315 RVA: 0x00024650 File Offset: 0x00022850
		// (set) Token: 0x06000CF4 RID: 3316 RVA: 0x00024658 File Offset: 0x00022858
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

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x06000CF5 RID: 3317 RVA: 0x0002467D File Offset: 0x0002287D
		// (set) Token: 0x06000CF6 RID: 3318 RVA: 0x00024685 File Offset: 0x00022885
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

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06000CF7 RID: 3319 RVA: 0x000246A3 File Offset: 0x000228A3
		// (set) Token: 0x06000CF8 RID: 3320 RVA: 0x000246AB File Offset: 0x000228AB
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

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x06000CF9 RID: 3321 RVA: 0x000246C9 File Offset: 0x000228C9
		// (set) Token: 0x06000CFA RID: 3322 RVA: 0x000246D1 File Offset: 0x000228D1
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

		// Token: 0x040005FC RID: 1532
		private float _screenWidth;

		// Token: 0x040005FD RID: 1533
		private float _screenHeight;

		// Token: 0x040005FE RID: 1534
		private bool _initialized;

		// Token: 0x040005FF RID: 1535
		private Vec2 _position;

		// Token: 0x04000600 RID: 1536
		private bool _isActive;

		// Token: 0x04000601 RID: 1537
		private bool _isBehind;

		// Token: 0x04000602 RID: 1538
		private bool _isArmy;

		// Token: 0x04000603 RID: 1539
		private bool _isTracked;

		// Token: 0x04000604 RID: 1540
		private bool _shouldShowFullName;
	}
}
