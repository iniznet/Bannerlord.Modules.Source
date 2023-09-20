using System;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	// Token: 0x02000024 RID: 36
	public class PartyCompositionVM : ViewModel
	{
		// Token: 0x060002CD RID: 717 RVA: 0x00012C50 File Offset: 0x00010E50
		public PartyCompositionVM()
		{
			this.InfantryHint = new HintViewModel(new TextObject("{=1Bm1Wk1v}Infantry", null), null);
			this.RangedHint = new HintViewModel(new TextObject("{=bIiBytSB}Archers", null), null);
			this.CavalryHint = new HintViewModel(new TextObject("{=YVGtcLHF}Cavalry", null), null);
			this.HorseArcherHint = new HintViewModel(new TextObject("{=I1CMeL9R}Mounted Archers", null), null);
		}

		// Token: 0x060002CE RID: 718 RVA: 0x00012CC0 File Offset: 0x00010EC0
		public void OnTroopRemoved(FormationClass formationClass, int count)
		{
			if (this.IsInfantry(formationClass))
			{
				this.InfantryCount -= count;
			}
			if (this.IsRanged(formationClass))
			{
				this.RangedCount -= count;
			}
			if (this.IsCavalry(formationClass))
			{
				this.CavalryCount -= count;
			}
			if (this.IsHorseArcher(formationClass))
			{
				this.HorseArcherCount -= count;
			}
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00012D2C File Offset: 0x00010F2C
		public void OnTroopAdded(FormationClass formationClass, int count)
		{
			if (this.IsInfantry(formationClass))
			{
				this.InfantryCount += count;
			}
			if (this.IsRanged(formationClass))
			{
				this.RangedCount += count;
			}
			if (this.IsCavalry(formationClass))
			{
				this.CavalryCount += count;
			}
			if (this.IsHorseArcher(formationClass))
			{
				this.HorseArcherCount += count;
			}
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x00012D98 File Offset: 0x00010F98
		public void RefreshCounts(MBBindingList<PartyCharacterVM> list)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < list.Count; i++)
			{
				TroopRosterElement troop = list[i].Troop;
				FormationClass defaultFormationClass = list[i].Troop.Character.DefaultFormationClass;
				if (this.IsInfantry(defaultFormationClass))
				{
					num += troop.Number;
				}
				if (this.IsRanged(defaultFormationClass))
				{
					num2 += troop.Number;
				}
				if (this.IsCavalry(defaultFormationClass))
				{
					num3 += troop.Number;
				}
				if (this.IsHorseArcher(defaultFormationClass))
				{
					num4 += troop.Number;
				}
			}
			this.InfantryCount = num;
			this.RangedCount = num2;
			this.CavalryCount = num3;
			this.HorseArcherCount = num4;
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00012E59 File Offset: 0x00011059
		private bool IsInfantry(FormationClass formationClass)
		{
			return formationClass == FormationClass.Infantry || formationClass == FormationClass.HeavyInfantry || formationClass == FormationClass.NumberOfDefaultFormations;
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x00012E68 File Offset: 0x00011068
		private bool IsRanged(FormationClass formationClass)
		{
			return formationClass == FormationClass.Ranged;
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x00012E6E File Offset: 0x0001106E
		private bool IsCavalry(FormationClass formationClass)
		{
			return formationClass == FormationClass.Cavalry || formationClass == FormationClass.LightCavalry || formationClass == FormationClass.HeavyCavalry;
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x00012E7E File Offset: 0x0001107E
		private bool IsHorseArcher(FormationClass formationClass)
		{
			return formationClass == FormationClass.HorseArcher;
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x060002D5 RID: 725 RVA: 0x00012E84 File Offset: 0x00011084
		// (set) Token: 0x060002D6 RID: 726 RVA: 0x00012E8C File Offset: 0x0001108C
		[DataSourceProperty]
		public int InfantryCount
		{
			get
			{
				return this._infantryCount;
			}
			set
			{
				if (value != this._infantryCount)
				{
					this._infantryCount = value;
					base.OnPropertyChangedWithValue(value, "InfantryCount");
				}
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x060002D7 RID: 727 RVA: 0x00012EAA File Offset: 0x000110AA
		// (set) Token: 0x060002D8 RID: 728 RVA: 0x00012EB2 File Offset: 0x000110B2
		[DataSourceProperty]
		public int RangedCount
		{
			get
			{
				return this._rangedCount;
			}
			set
			{
				if (value != this._rangedCount)
				{
					this._rangedCount = value;
					base.OnPropertyChangedWithValue(value, "RangedCount");
				}
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x060002D9 RID: 729 RVA: 0x00012ED0 File Offset: 0x000110D0
		// (set) Token: 0x060002DA RID: 730 RVA: 0x00012ED8 File Offset: 0x000110D8
		[DataSourceProperty]
		public int CavalryCount
		{
			get
			{
				return this._cavalryCount;
			}
			set
			{
				if (value != this._cavalryCount)
				{
					this._cavalryCount = value;
					base.OnPropertyChangedWithValue(value, "CavalryCount");
				}
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060002DB RID: 731 RVA: 0x00012EF6 File Offset: 0x000110F6
		// (set) Token: 0x060002DC RID: 732 RVA: 0x00012EFE File Offset: 0x000110FE
		[DataSourceProperty]
		public int HorseArcherCount
		{
			get
			{
				return this._horseArcherCount;
			}
			set
			{
				if (value != this._horseArcherCount)
				{
					this._horseArcherCount = value;
					base.OnPropertyChangedWithValue(value, "HorseArcherCount");
				}
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060002DD RID: 733 RVA: 0x00012F1C File Offset: 0x0001111C
		// (set) Token: 0x060002DE RID: 734 RVA: 0x00012F24 File Offset: 0x00011124
		[DataSourceProperty]
		public HintViewModel InfantryHint
		{
			get
			{
				return this._infantryHint;
			}
			set
			{
				if (value != this._infantryHint)
				{
					this._infantryHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "InfantryHint");
				}
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060002DF RID: 735 RVA: 0x00012F42 File Offset: 0x00011142
		// (set) Token: 0x060002E0 RID: 736 RVA: 0x00012F4A File Offset: 0x0001114A
		[DataSourceProperty]
		public HintViewModel RangedHint
		{
			get
			{
				return this._rangedHint;
			}
			set
			{
				if (value != this._rangedHint)
				{
					this._rangedHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RangedHint");
				}
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x00012F68 File Offset: 0x00011168
		// (set) Token: 0x060002E2 RID: 738 RVA: 0x00012F70 File Offset: 0x00011170
		[DataSourceProperty]
		public HintViewModel CavalryHint
		{
			get
			{
				return this._cavalryHint;
			}
			set
			{
				if (value != this._cavalryHint)
				{
					this._cavalryHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CavalryHint");
				}
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x00012F8E File Offset: 0x0001118E
		// (set) Token: 0x060002E4 RID: 740 RVA: 0x00012F96 File Offset: 0x00011196
		[DataSourceProperty]
		public HintViewModel HorseArcherHint
		{
			get
			{
				return this._horseArcherHint;
			}
			set
			{
				if (value != this._horseArcherHint)
				{
					this._horseArcherHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "HorseArcherHint");
				}
			}
		}

		// Token: 0x04000144 RID: 324
		private int _infantryCount;

		// Token: 0x04000145 RID: 325
		private int _rangedCount;

		// Token: 0x04000146 RID: 326
		private int _cavalryCount;

		// Token: 0x04000147 RID: 327
		private int _horseArcherCount;

		// Token: 0x04000148 RID: 328
		private HintViewModel _infantryHint;

		// Token: 0x04000149 RID: 329
		private HintViewModel _rangedHint;

		// Token: 0x0400014A RID: 330
		private HintViewModel _cavalryHint;

		// Token: 0x0400014B RID: 331
		private HintViewModel _horseArcherHint;
	}
}
