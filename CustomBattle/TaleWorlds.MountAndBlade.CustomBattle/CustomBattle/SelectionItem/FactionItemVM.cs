using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000020 RID: 32
	public class FactionItemVM : ViewModel
	{
		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x0000A31A File Offset: 0x0000851A
		// (set) Token: 0x060001A4 RID: 420 RVA: 0x0000A322 File Offset: 0x00008522
		public BasicCultureObject Faction { get; private set; }

		// Token: 0x060001A5 RID: 421 RVA: 0x0000A32C File Offset: 0x0000852C
		public FactionItemVM(BasicCultureObject faction, Action<FactionItemVM> onSelected)
		{
			this.Faction = faction;
			this._onSelected = onSelected;
			this.CultureCode = faction.GetCultureCode().ToString().ToLower();
			this.Hint = new HintViewModel(faction.Name, null);
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x0000A37E File Offset: 0x0000857E
		// (set) Token: 0x060001A7 RID: 423 RVA: 0x0000A386 File Offset: 0x00008586
		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x0000A3A4 File Offset: 0x000085A4
		// (set) Token: 0x060001A9 RID: 425 RVA: 0x0000A3AC File Offset: 0x000085AC
		[DataSourceProperty]
		public string CultureCode
		{
			get
			{
				return this._cultureCode;
			}
			set
			{
				if (value != this._cultureCode)
				{
					this._cultureCode = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureCode");
				}
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060001AA RID: 426 RVA: 0x0000A3CF File Offset: 0x000085CF
		// (set) Token: 0x060001AB RID: 427 RVA: 0x0000A3D7 File Offset: 0x000085D7
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
					if (value)
					{
						this._onSelected(this);
					}
				}
			}
		}

		// Token: 0x04000111 RID: 273
		private Action<FactionItemVM> _onSelected;

		// Token: 0x04000112 RID: 274
		private HintViewModel _hint;

		// Token: 0x04000113 RID: 275
		private string _cultureCode;

		// Token: 0x04000114 RID: 276
		private bool _isSelected;
	}
}
