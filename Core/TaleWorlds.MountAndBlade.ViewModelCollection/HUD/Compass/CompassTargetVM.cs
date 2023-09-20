using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.Compass
{
	public class CompassTargetVM : ViewModel
	{
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

		public virtual void Refresh(float circleX, float x, float distance)
		{
			this.FullPosition = circleX;
			this.Position = x;
			this.Distance = MathF.Round(distance);
		}

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

		private int _distance;

		private string _color;

		private string _color2;

		private ImageIdentifierVM _banner;

		private string _iconType;

		private string _iconSpriteType;

		private string _letterCode;

		private float _position;

		private float _fullPosition;

		private bool _isAttacker;

		private bool _isEnemy;

		private bool _isFlag;
	}
}
