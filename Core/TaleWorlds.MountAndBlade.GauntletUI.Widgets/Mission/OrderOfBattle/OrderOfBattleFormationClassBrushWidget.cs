using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	// Token: 0x020000CF RID: 207
	public class OrderOfBattleFormationClassBrushWidget : BrushWidget
	{
		// Token: 0x06000A88 RID: 2696 RVA: 0x0001D997 File Offset: 0x0001BB97
		public OrderOfBattleFormationClassBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000A89 RID: 2697 RVA: 0x0001D9A0 File Offset: 0x0001BBA0
		private void SetBaseBrush()
		{
			switch (this.FormationClass)
			{
			case 0:
				base.Brush = this.UnsetBrush;
				break;
			case 1:
				base.Brush = this.InfantryBrush;
				break;
			case 2:
				base.Brush = this.RangedBrush;
				break;
			case 3:
				base.Brush = this.CavalryBrush;
				break;
			case 4:
				base.Brush = this.HorseArcherBrush;
				break;
			case 5:
				base.Brush = this.InfantryAndRangedBrush;
				break;
			case 6:
				base.Brush = this.CavalryAndHorseArcherBrush;
				break;
			default:
				base.Brush = this.UnsetBrush;
				break;
			}
			this._hasBaseBrushSet = true;
			this.SetColor();
		}

		// Token: 0x06000A8A RID: 2698 RVA: 0x0001DA53 File Offset: 0x0001BC53
		private void SetColor()
		{
			if (this.IsErrored)
			{
				base.Brush.Color = this.ErroredColor;
			}
		}

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06000A8B RID: 2699 RVA: 0x0001DA6E File Offset: 0x0001BC6E
		// (set) Token: 0x06000A8C RID: 2700 RVA: 0x0001DA76 File Offset: 0x0001BC76
		[Editor(false)]
		public int FormationClass
		{
			get
			{
				return this._formationClass;
			}
			set
			{
				if (value != this._formationClass || !this._hasBaseBrushSet)
				{
					this._formationClass = value;
					base.OnPropertyChanged(value, "FormationClass");
					this.SetBaseBrush();
				}
			}
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06000A8D RID: 2701 RVA: 0x0001DAA2 File Offset: 0x0001BCA2
		// (set) Token: 0x06000A8E RID: 2702 RVA: 0x0001DAAA File Offset: 0x0001BCAA
		[Editor(false)]
		public Color ErroredColor
		{
			get
			{
				return this._erroredColor;
			}
			set
			{
				if (value != this._erroredColor)
				{
					this._erroredColor = value;
					base.OnPropertyChanged(value, "ErroredColor");
				}
			}
		}

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x06000A8F RID: 2703 RVA: 0x0001DACD File Offset: 0x0001BCCD
		// (set) Token: 0x06000A90 RID: 2704 RVA: 0x0001DAD5 File Offset: 0x0001BCD5
		[Editor(false)]
		public bool IsErrored
		{
			get
			{
				return this._isErrored;
			}
			set
			{
				if (value != this._isErrored)
				{
					this._isErrored = value;
					base.OnPropertyChanged(value, "IsErrored");
					this.SetColor();
				}
			}
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x06000A91 RID: 2705 RVA: 0x0001DAF9 File Offset: 0x0001BCF9
		// (set) Token: 0x06000A92 RID: 2706 RVA: 0x0001DB01 File Offset: 0x0001BD01
		[Editor(false)]
		public Brush UnsetBrush
		{
			get
			{
				return this._unsetBrush;
			}
			set
			{
				if (value != this._unsetBrush)
				{
					this._unsetBrush = value;
					base.OnPropertyChanged<Brush>(value, "UnsetBrush");
					this.SetBaseBrush();
				}
			}
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06000A93 RID: 2707 RVA: 0x0001DB25 File Offset: 0x0001BD25
		// (set) Token: 0x06000A94 RID: 2708 RVA: 0x0001DB2D File Offset: 0x0001BD2D
		[Editor(false)]
		public Brush InfantryBrush
		{
			get
			{
				return this._infantryBrush;
			}
			set
			{
				if (value != this._infantryBrush)
				{
					this._infantryBrush = value;
					base.OnPropertyChanged<Brush>(value, "InfantryBrush");
					this.SetBaseBrush();
				}
			}
		}

		// Token: 0x170003BB RID: 955
		// (get) Token: 0x06000A95 RID: 2709 RVA: 0x0001DB51 File Offset: 0x0001BD51
		// (set) Token: 0x06000A96 RID: 2710 RVA: 0x0001DB59 File Offset: 0x0001BD59
		[Editor(false)]
		public Brush RangedBrush
		{
			get
			{
				return this._rangedBrush;
			}
			set
			{
				if (value != this._rangedBrush)
				{
					this._rangedBrush = value;
					base.OnPropertyChanged<Brush>(value, "RangedBrush");
					this.SetBaseBrush();
				}
			}
		}

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x06000A97 RID: 2711 RVA: 0x0001DB7D File Offset: 0x0001BD7D
		// (set) Token: 0x06000A98 RID: 2712 RVA: 0x0001DB85 File Offset: 0x0001BD85
		[Editor(false)]
		public Brush CavalryBrush
		{
			get
			{
				return this._cavalryBrush;
			}
			set
			{
				if (value != this._cavalryBrush)
				{
					this._cavalryBrush = value;
					base.OnPropertyChanged<Brush>(value, "CavalryBrush");
					this.SetBaseBrush();
				}
			}
		}

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x06000A99 RID: 2713 RVA: 0x0001DBA9 File Offset: 0x0001BDA9
		// (set) Token: 0x06000A9A RID: 2714 RVA: 0x0001DBB1 File Offset: 0x0001BDB1
		[Editor(false)]
		public Brush HorseArcherBrush
		{
			get
			{
				return this._horseArcherBrush;
			}
			set
			{
				if (value != this._horseArcherBrush)
				{
					this._horseArcherBrush = value;
					base.OnPropertyChanged<Brush>(value, "HorseArcherBrush");
					this.SetBaseBrush();
				}
			}
		}

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x06000A9B RID: 2715 RVA: 0x0001DBD5 File Offset: 0x0001BDD5
		// (set) Token: 0x06000A9C RID: 2716 RVA: 0x0001DBDD File Offset: 0x0001BDDD
		[Editor(false)]
		public Brush InfantryAndRangedBrush
		{
			get
			{
				return this._infantryAndRangedBrush;
			}
			set
			{
				if (value != this._infantryAndRangedBrush)
				{
					this._infantryAndRangedBrush = value;
					base.OnPropertyChanged<Brush>(value, "InfantryAndRangedBrush");
					this.SetBaseBrush();
				}
			}
		}

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x06000A9D RID: 2717 RVA: 0x0001DC01 File Offset: 0x0001BE01
		// (set) Token: 0x06000A9E RID: 2718 RVA: 0x0001DC09 File Offset: 0x0001BE09
		[Editor(false)]
		public Brush CavalryAndHorseArcherBrush
		{
			get
			{
				return this._cavalryAndHorseArcherBrush;
			}
			set
			{
				if (value != this._cavalryAndHorseArcherBrush)
				{
					this._cavalryAndHorseArcherBrush = value;
					base.OnPropertyChanged<Brush>(value, "CavalryAndHorseArcherBrush");
					this.SetBaseBrush();
				}
			}
		}

		// Token: 0x040004CD RID: 1229
		private bool _hasBaseBrushSet;

		// Token: 0x040004CE RID: 1230
		private int _formationClass;

		// Token: 0x040004CF RID: 1231
		private Color _erroredColor;

		// Token: 0x040004D0 RID: 1232
		private bool _isErrored;

		// Token: 0x040004D1 RID: 1233
		private Brush _unsetBrush;

		// Token: 0x040004D2 RID: 1234
		private Brush _infantryBrush;

		// Token: 0x040004D3 RID: 1235
		private Brush _rangedBrush;

		// Token: 0x040004D4 RID: 1236
		private Brush _cavalryBrush;

		// Token: 0x040004D5 RID: 1237
		private Brush _horseArcherBrush;

		// Token: 0x040004D6 RID: 1238
		private Brush _infantryAndRangedBrush;

		// Token: 0x040004D7 RID: 1239
		private Brush _cavalryAndHorseArcherBrush;
	}
}
