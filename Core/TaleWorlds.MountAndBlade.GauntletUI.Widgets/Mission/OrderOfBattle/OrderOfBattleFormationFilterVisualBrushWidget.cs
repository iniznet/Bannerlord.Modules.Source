using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	public class OrderOfBattleFormationFilterVisualBrushWidget : BrushWidget
	{
		public OrderOfBattleFormationFilterVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		private void SetBaseBrush()
		{
			switch (this.FormationFilter)
			{
			case 0:
				base.Brush = this.UnsetBrush;
				break;
			case 1:
				base.Brush = this.ShieldBrush;
				break;
			case 2:
				base.Brush = this.SpearBrush;
				break;
			case 3:
				base.Brush = this.ThrownBrush;
				break;
			case 4:
				base.Brush = this.HeavyBrush;
				break;
			case 5:
				base.Brush = this.HighTierBrush;
				break;
			case 6:
				base.Brush = this.LowTierBrush;
				break;
			default:
				base.Brush = this.UnsetBrush;
				break;
			}
			this._hasBaseBrushSet = true;
		}

		[Editor(false)]
		public int FormationFilter
		{
			get
			{
				return this._formationFilter;
			}
			set
			{
				if (value != this._formationFilter || !this._hasBaseBrushSet)
				{
					this._formationFilter = value;
					base.OnPropertyChanged(value, "FormationFilter");
					this.SetBaseBrush();
				}
			}
		}

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

		[Editor(false)]
		public Brush SpearBrush
		{
			get
			{
				return this._spearBrush;
			}
			set
			{
				if (value != this._spearBrush)
				{
					this._spearBrush = value;
					base.OnPropertyChanged<Brush>(value, "SpearBrush");
					this.SetBaseBrush();
				}
			}
		}

		[Editor(false)]
		public Brush ShieldBrush
		{
			get
			{
				return this._shieldBrush;
			}
			set
			{
				if (value != this._shieldBrush)
				{
					this._shieldBrush = value;
					base.OnPropertyChanged<Brush>(value, "ShieldBrush");
					this.SetBaseBrush();
				}
			}
		}

		[Editor(false)]
		public Brush ThrownBrush
		{
			get
			{
				return this._thrownBrush;
			}
			set
			{
				if (value != this._thrownBrush)
				{
					this._thrownBrush = value;
					base.OnPropertyChanged<Brush>(value, "ThrownBrush");
					this.SetBaseBrush();
				}
			}
		}

		[Editor(false)]
		public Brush HeavyBrush
		{
			get
			{
				return this._heavyBrush;
			}
			set
			{
				if (value != this._heavyBrush)
				{
					this._heavyBrush = value;
					base.OnPropertyChanged<Brush>(value, "HeavyBrush");
					this.SetBaseBrush();
				}
			}
		}

		[Editor(false)]
		public Brush HighTierBrush
		{
			get
			{
				return this._highTierBrush;
			}
			set
			{
				if (value != this._highTierBrush)
				{
					this._highTierBrush = value;
					base.OnPropertyChanged<Brush>(value, "HighTierBrush");
					this.SetBaseBrush();
				}
			}
		}

		[Editor(false)]
		public Brush LowTierBrush
		{
			get
			{
				return this._lowTierBrush;
			}
			set
			{
				if (value != this._lowTierBrush)
				{
					this._lowTierBrush = value;
					base.OnPropertyChanged<Brush>(value, "LowTierBrush");
					this.SetBaseBrush();
				}
			}
		}

		private bool _hasBaseBrushSet;

		private int _formationFilter;

		private Brush _unsetBrush;

		private Brush _spearBrush;

		private Brush _shieldBrush;

		private Brush _thrownBrush;

		private Brush _heavyBrush;

		private Brush _highTierBrush;

		private Brush _lowTierBrush;
	}
}
