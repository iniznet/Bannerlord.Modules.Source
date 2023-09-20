using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.Compass
{
	// Token: 0x020000F1 RID: 241
	public class CompassTargetVM : ViewModel
	{
		// Token: 0x0600154B RID: 5451 RVA: 0x000453D8 File Offset: 0x000435D8
		public CompassTargetVM(TargetIconType iconType, uint color, uint color2, BannerCode bannercode, bool isAttacker, bool isAlly)
		{
			this.IconType = iconType.ToString();
			this.LetterCode = this.GetLetterCode(iconType);
			this.RefreshColor(color, color2);
			this.IsFlag = iconType >= TargetIconType.Flag_A && iconType <= TargetIconType.Flag_I;
			this.IsAttacker = isAttacker;
			this.IsEnemy = !isAlly;
			if (bannercode == null)
			{
				this.Banner = new ImageIdentifierVM(ImageIdentifierType.Null);
				return;
			}
			this.Banner = new ImageIdentifierVM(bannercode, false);
		}

		// Token: 0x0600154C RID: 5452 RVA: 0x00045464 File Offset: 0x00043664
		private string GetLetterCode(TargetIconType iconType)
		{
			switch (iconType)
			{
			case TargetIconType.Flag_A:
				return "A";
			case TargetIconType.Flag_B:
				return "B";
			case TargetIconType.Flag_C:
				return "C";
			case TargetIconType.Flag_D:
				return "D";
			case TargetIconType.Flag_E:
				return "E";
			case TargetIconType.Flag_F:
				return "F";
			case TargetIconType.Flag_G:
				return "G";
			case TargetIconType.Flag_H:
				return "H";
			case TargetIconType.Flag_I:
				return "I";
			default:
				return "";
			}
		}

		// Token: 0x0600154D RID: 5453 RVA: 0x000454DC File Offset: 0x000436DC
		public void RefreshColor(uint color, uint color2)
		{
			if (color != 0U)
			{
				string text = color.ToString("X");
				char c = text[0];
				char c2 = text[1];
				text = text.Remove(0, 2);
				text = text.Add(c.ToString() + c2.ToString(), false);
				this.Color = "#" + text;
			}
			else
			{
				this.Color = "#FFFFFFFF";
			}
			if (color2 != 0U)
			{
				string text2 = color2.ToString("X");
				char c3 = text2[0];
				char c4 = text2[1];
				text2 = text2.Remove(0, 2);
				text2 = text2.Add(c3.ToString() + c4.ToString(), false);
				this.Color2 = "#" + text2;
				return;
			}
			this.Color2 = "#FFFFFFFF";
		}

		// Token: 0x0600154E RID: 5454 RVA: 0x000455AE File Offset: 0x000437AE
		public virtual void Refresh(float circleX, float x, float distance)
		{
			this.FullPosition = circleX;
			this.Position = x;
			this.Distance = MathF.Round(distance);
		}

		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x0600154F RID: 5455 RVA: 0x000455CA File Offset: 0x000437CA
		// (set) Token: 0x06001550 RID: 5456 RVA: 0x000455D4 File Offset: 0x000437D4
		[DataSourceProperty]
		public ImageIdentifierVM Banner
		{
			get
			{
				return this._banner;
			}
			set
			{
				if (value != this._banner && (value == null || this._banner == null || this._banner.Id != value.Id))
				{
					this._banner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Banner");
				}
			}
		}

		// Token: 0x17000700 RID: 1792
		// (get) Token: 0x06001551 RID: 5457 RVA: 0x00045620 File Offset: 0x00043820
		// (set) Token: 0x06001552 RID: 5458 RVA: 0x00045628 File Offset: 0x00043828
		[DataSourceProperty]
		public bool IsFlag
		{
			get
			{
				return this._isFlag;
			}
			set
			{
				if (value != this._isFlag)
				{
					this._isFlag = value;
					base.OnPropertyChangedWithValue(value, "IsFlag");
				}
			}
		}

		// Token: 0x17000701 RID: 1793
		// (get) Token: 0x06001553 RID: 5459 RVA: 0x00045646 File Offset: 0x00043846
		// (set) Token: 0x06001554 RID: 5460 RVA: 0x0004564E File Offset: 0x0004384E
		[DataSourceProperty]
		public int Distance
		{
			get
			{
				return this._distance;
			}
			set
			{
				if (value != this._distance)
				{
					this._distance = value;
					base.OnPropertyChangedWithValue(value, "Distance");
				}
			}
		}

		// Token: 0x17000702 RID: 1794
		// (get) Token: 0x06001555 RID: 5461 RVA: 0x0004566C File Offset: 0x0004386C
		// (set) Token: 0x06001556 RID: 5462 RVA: 0x00045674 File Offset: 0x00043874
		[DataSourceProperty]
		public string Color2
		{
			get
			{
				return this._color2;
			}
			set
			{
				if (value != this._color2)
				{
					this._color2 = value;
					base.OnPropertyChangedWithValue<string>(value, "Color2");
				}
			}
		}

		// Token: 0x17000703 RID: 1795
		// (get) Token: 0x06001557 RID: 5463 RVA: 0x00045697 File Offset: 0x00043897
		// (set) Token: 0x06001558 RID: 5464 RVA: 0x0004569F File Offset: 0x0004389F
		[DataSourceProperty]
		public string Color
		{
			get
			{
				return this._color;
			}
			set
			{
				if (value != this._color)
				{
					this._color = value;
					base.OnPropertyChangedWithValue<string>(value, "Color");
				}
			}
		}

		// Token: 0x17000704 RID: 1796
		// (get) Token: 0x06001559 RID: 5465 RVA: 0x000456C2 File Offset: 0x000438C2
		// (set) Token: 0x0600155A RID: 5466 RVA: 0x000456CA File Offset: 0x000438CA
		[DataSourceProperty]
		public string IconType
		{
			get
			{
				return this._iconType;
			}
			set
			{
				if (value != this._iconType)
				{
					this._iconType = value;
					base.OnPropertyChangedWithValue<string>(value, "IconType");
					this.IconSpriteType = value;
				}
			}
		}

		// Token: 0x17000705 RID: 1797
		// (get) Token: 0x0600155B RID: 5467 RVA: 0x000456F4 File Offset: 0x000438F4
		// (set) Token: 0x0600155C RID: 5468 RVA: 0x000456FC File Offset: 0x000438FC
		[DataSourceProperty]
		public string IconSpriteType
		{
			get
			{
				return this._iconSpriteType;
			}
			set
			{
				if (value != this._iconSpriteType)
				{
					this._iconSpriteType = value;
					base.OnPropertyChangedWithValue<string>(value, "IconSpriteType");
				}
			}
		}

		// Token: 0x17000706 RID: 1798
		// (get) Token: 0x0600155D RID: 5469 RVA: 0x0004571F File Offset: 0x0004391F
		// (set) Token: 0x0600155E RID: 5470 RVA: 0x00045727 File Offset: 0x00043927
		[DataSourceProperty]
		public string LetterCode
		{
			get
			{
				return this._letterCode;
			}
			set
			{
				if (value != this._letterCode)
				{
					this._letterCode = value;
					base.OnPropertyChangedWithValue<string>(value, "LetterCode");
				}
			}
		}

		// Token: 0x17000707 RID: 1799
		// (get) Token: 0x0600155F RID: 5471 RVA: 0x0004574A File Offset: 0x0004394A
		// (set) Token: 0x06001560 RID: 5472 RVA: 0x00045752 File Offset: 0x00043952
		[DataSourceProperty]
		public float FullPosition
		{
			get
			{
				return this._fullPosition;
			}
			set
			{
				if (MathF.Abs(value - this._fullPosition) > 1E-45f)
				{
					this._fullPosition = value;
					base.OnPropertyChangedWithValue(value, "FullPosition");
				}
			}
		}

		// Token: 0x17000708 RID: 1800
		// (get) Token: 0x06001561 RID: 5473 RVA: 0x0004577B File Offset: 0x0004397B
		// (set) Token: 0x06001562 RID: 5474 RVA: 0x00045783 File Offset: 0x00043983
		[DataSourceProperty]
		public float Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (MathF.Abs(value - this._position) > 1E-45f)
				{
					this._position = value;
					base.OnPropertyChangedWithValue(value, "Position");
				}
			}
		}

		// Token: 0x17000709 RID: 1801
		// (get) Token: 0x06001563 RID: 5475 RVA: 0x000457AC File Offset: 0x000439AC
		// (set) Token: 0x06001564 RID: 5476 RVA: 0x000457B4 File Offset: 0x000439B4
		[DataSourceProperty]
		public bool IsAttacker
		{
			get
			{
				return this._isAttacker;
			}
			set
			{
				if (value != this._isAttacker)
				{
					this._isAttacker = value;
					base.OnPropertyChangedWithValue(value, "IsAttacker");
				}
			}
		}

		// Token: 0x1700070A RID: 1802
		// (get) Token: 0x06001565 RID: 5477 RVA: 0x000457D2 File Offset: 0x000439D2
		// (set) Token: 0x06001566 RID: 5478 RVA: 0x000457DA File Offset: 0x000439DA
		[DataSourceProperty]
		public bool IsEnemy
		{
			get
			{
				return this._isEnemy;
			}
			set
			{
				if (value != this._isEnemy)
				{
					this._isEnemy = value;
					base.OnPropertyChangedWithValue(value, "IsEnemy");
				}
			}
		}

		// Token: 0x04000A31 RID: 2609
		private int _distance;

		// Token: 0x04000A32 RID: 2610
		private string _color;

		// Token: 0x04000A33 RID: 2611
		private string _color2;

		// Token: 0x04000A34 RID: 2612
		private ImageIdentifierVM _banner;

		// Token: 0x04000A35 RID: 2613
		private string _iconType;

		// Token: 0x04000A36 RID: 2614
		private string _iconSpriteType;

		// Token: 0x04000A37 RID: 2615
		private string _letterCode;

		// Token: 0x04000A38 RID: 2616
		private float _position;

		// Token: 0x04000A39 RID: 2617
		private float _fullPosition;

		// Token: 0x04000A3A RID: 2618
		private bool _isAttacker;

		// Token: 0x04000A3B RID: 2619
		private bool _isEnemy;

		// Token: 0x04000A3C RID: 2620
		private bool _isFlag;
	}
}
