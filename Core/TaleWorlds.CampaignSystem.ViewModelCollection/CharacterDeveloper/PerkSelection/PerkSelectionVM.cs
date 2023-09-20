using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkSelection
{
	// Token: 0x0200011E RID: 286
	public class PerkSelectionVM : ViewModel
	{
		// Token: 0x06001BE7 RID: 7143 RVA: 0x000646F9 File Offset: 0x000628F9
		public PerkSelectionVM(IHeroDeveloper developer, Action<SkillObject> refreshPerksOf, Action onPerkSelection)
		{
			this._developer = developer;
			this._refreshPerksOf = refreshPerksOf;
			this._onPerkSelection = onPerkSelection;
			this._selectedPerks = new List<PerkObject>();
			this.AvailablePerks = new MBBindingList<PerkSelectionItemVM>();
			this.IsActive = false;
		}

		// Token: 0x06001BE8 RID: 7144 RVA: 0x00064734 File Offset: 0x00062934
		public void SetCurrentSelectionPerk(PerkVM perk)
		{
			if (this.AvailablePerks.Count > 0 || this.IsActive)
			{
				this.ExecuteDeactivate();
			}
			this.AvailablePerks.Clear();
			this._currentInitialPerk = perk;
			this.AvailablePerks.Add(new PerkSelectionItemVM(perk.Perk, new Action<PerkSelectionItemVM>(this.OnSelectPerk)));
			if (perk.AlternativeType == 2)
			{
				this.AvailablePerks.Insert(0, new PerkSelectionItemVM(perk.Perk.AlternativePerk, new Action<PerkSelectionItemVM>(this.OnSelectPerk)));
			}
			else if (perk.AlternativeType == 1)
			{
				this.AvailablePerks.Add(new PerkSelectionItemVM(perk.Perk.AlternativePerk, new Action<PerkSelectionItemVM>(this.OnSelectPerk)));
			}
			perk.IsInSelection = true;
			this.IsActive = true;
		}

		// Token: 0x06001BE9 RID: 7145 RVA: 0x00064804 File Offset: 0x00062A04
		private void OnSelectPerk(PerkSelectionItemVM selectedPerk)
		{
			this._selectedPerks.Add(selectedPerk.Perk);
			this._refreshPerksOf(selectedPerk.Perk.Skill);
			this._currentInitialPerk.IsInSelection = false;
			this.IsActive = false;
			Game.Current.EventManager.TriggerEvent<PerkSelectedByPlayerEvent>(new PerkSelectedByPlayerEvent(selectedPerk.Perk));
			Action onPerkSelection = this._onPerkSelection;
			if (onPerkSelection == null)
			{
				return;
			}
			onPerkSelection();
		}

		// Token: 0x06001BEA RID: 7146 RVA: 0x00064878 File Offset: 0x00062A78
		public void ResetSelectedPerks()
		{
			foreach (PerkObject perkObject in this._selectedPerks)
			{
				this._refreshPerksOf(perkObject.Skill);
			}
			this._selectedPerks.Clear();
		}

		// Token: 0x06001BEB RID: 7147 RVA: 0x000648E0 File Offset: 0x00062AE0
		public void ApplySelectedPerks()
		{
			foreach (PerkObject perkObject in this._selectedPerks.ToList<PerkObject>())
			{
				this._developer.AddPerk(perkObject);
				this._selectedPerks.Remove(perkObject);
			}
		}

		// Token: 0x06001BEC RID: 7148 RVA: 0x0006494C File Offset: 0x00062B4C
		public bool IsPerkSelected(PerkObject perk)
		{
			return this._selectedPerks.Contains(perk);
		}

		// Token: 0x06001BED RID: 7149 RVA: 0x0006495A File Offset: 0x00062B5A
		public bool IsAnyPerkSelected()
		{
			return this._selectedPerks.Count > 0;
		}

		// Token: 0x06001BEE RID: 7150 RVA: 0x0006496A File Offset: 0x00062B6A
		public void ExecuteDeactivate()
		{
			this.IsActive = false;
			this._refreshPerksOf(this._currentInitialPerk.Perk.Skill);
			this._currentInitialPerk.IsInSelection = false;
		}

		// Token: 0x17000992 RID: 2450
		// (get) Token: 0x06001BEF RID: 7151 RVA: 0x0006499A File Offset: 0x00062B9A
		// (set) Token: 0x06001BF0 RID: 7152 RVA: 0x000649A2 File Offset: 0x00062BA2
		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChangedWithValue(value, "IsActive");
					Game.Current.EventManager.TriggerEvent<PerkSelectionToggleEvent>(new PerkSelectionToggleEvent(this.IsActive));
				}
			}
		}

		// Token: 0x17000993 RID: 2451
		// (get) Token: 0x06001BF1 RID: 7153 RVA: 0x000649DA File Offset: 0x00062BDA
		// (set) Token: 0x06001BF2 RID: 7154 RVA: 0x000649E2 File Offset: 0x00062BE2
		[DataSourceProperty]
		public MBBindingList<PerkSelectionItemVM> AvailablePerks
		{
			get
			{
				return this._availablePerks;
			}
			set
			{
				if (value != this._availablePerks)
				{
					this._availablePerks = value;
					base.OnPropertyChangedWithValue<MBBindingList<PerkSelectionItemVM>>(value, "AvailablePerks");
				}
			}
		}

		// Token: 0x04000D2E RID: 3374
		private readonly IHeroDeveloper _developer;

		// Token: 0x04000D2F RID: 3375
		private readonly List<PerkObject> _selectedPerks;

		// Token: 0x04000D30 RID: 3376
		private readonly Action<SkillObject> _refreshPerksOf;

		// Token: 0x04000D31 RID: 3377
		private readonly Action _onPerkSelection;

		// Token: 0x04000D32 RID: 3378
		private PerkVM _currentInitialPerk;

		// Token: 0x04000D33 RID: 3379
		private bool _isActive;

		// Token: 0x04000D34 RID: 3380
		private MBBindingList<PerkSelectionItemVM> _availablePerks;
	}
}
