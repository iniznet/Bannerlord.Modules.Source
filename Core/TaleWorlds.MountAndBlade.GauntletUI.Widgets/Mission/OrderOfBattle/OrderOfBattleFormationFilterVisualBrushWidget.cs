using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	// Token: 0x020000D2 RID: 210
	public class OrderOfBattleFormationFilterVisualBrushWidget : BrushWidget
	{
		// Token: 0x06000AAA RID: 2730 RVA: 0x0001DD10 File Offset: 0x0001BF10
		public OrderOfBattleFormationFilterVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x0001DD1C File Offset: 0x0001BF1C
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

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x06000AAC RID: 2732 RVA: 0x0001DDC9 File Offset: 0x0001BFC9
		// (set) Token: 0x06000AAD RID: 2733 RVA: 0x0001DDD1 File Offset: 0x0001BFD1
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

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x06000AAE RID: 2734 RVA: 0x0001DDFD File Offset: 0x0001BFFD
		// (set) Token: 0x06000AAF RID: 2735 RVA: 0x0001DE05 File Offset: 0x0001C005
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

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x06000AB0 RID: 2736 RVA: 0x0001DE29 File Offset: 0x0001C029
		// (set) Token: 0x06000AB1 RID: 2737 RVA: 0x0001DE31 File Offset: 0x0001C031
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

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x06000AB2 RID: 2738 RVA: 0x0001DE55 File Offset: 0x0001C055
		// (set) Token: 0x06000AB3 RID: 2739 RVA: 0x0001DE5D File Offset: 0x0001C05D
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

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06000AB4 RID: 2740 RVA: 0x0001DE81 File Offset: 0x0001C081
		// (set) Token: 0x06000AB5 RID: 2741 RVA: 0x0001DE89 File Offset: 0x0001C089
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

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x06000AB6 RID: 2742 RVA: 0x0001DEAD File Offset: 0x0001C0AD
		// (set) Token: 0x06000AB7 RID: 2743 RVA: 0x0001DEB5 File Offset: 0x0001C0B5
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

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06000AB8 RID: 2744 RVA: 0x0001DED9 File Offset: 0x0001C0D9
		// (set) Token: 0x06000AB9 RID: 2745 RVA: 0x0001DEE1 File Offset: 0x0001C0E1
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

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06000ABA RID: 2746 RVA: 0x0001DF05 File Offset: 0x0001C105
		// (set) Token: 0x06000ABB RID: 2747 RVA: 0x0001DF0D File Offset: 0x0001C10D
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

		// Token: 0x040004DD RID: 1245
		private bool _hasBaseBrushSet;

		// Token: 0x040004DE RID: 1246
		private int _formationFilter;

		// Token: 0x040004DF RID: 1247
		private Brush _unsetBrush;

		// Token: 0x040004E0 RID: 1248
		private Brush _spearBrush;

		// Token: 0x040004E1 RID: 1249
		private Brush _shieldBrush;

		// Token: 0x040004E2 RID: 1250
		private Brush _thrownBrush;

		// Token: 0x040004E3 RID: 1251
		private Brush _heavyBrush;

		// Token: 0x040004E4 RID: 1252
		private Brush _highTierBrush;

		// Token: 0x040004E5 RID: 1253
		private Brush _lowTierBrush;
	}
}
