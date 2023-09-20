using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x0200001E RID: 30
	public class CustomBattleFactionSelectionVM : ViewModel
	{
		// Token: 0x06000197 RID: 407 RVA: 0x0000A108 File Offset: 0x00008308
		public CustomBattleFactionSelectionVM(Action<BasicCultureObject> onSelectionChanged)
		{
			this._onSelectionChanged = onSelectionChanged;
			this.Factions = new MBBindingList<FactionItemVM>();
			foreach (BasicCultureObject basicCultureObject in CustomBattleData.Factions)
			{
				this.Factions.Add(new FactionItemVM(basicCultureObject, new Action<FactionItemVM>(this.OnFactionSelected)));
			}
			this.SelectedItem = this.Factions[0];
			this.SelectFaction(0);
			this.Factions.ApplyActionOnAllItems(delegate(FactionItemVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000A1C8 File Offset: 0x000083C8
		public override void RefreshValues()
		{
			base.RefreshValues();
			FactionItemVM selectedItem = this.SelectedItem;
			this.SelectedFactionName = ((selectedItem != null) ? selectedItem.Faction.Name.ToString() : null);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000A1F4 File Offset: 0x000083F4
		public void SelectFaction(int index)
		{
			if (index >= 0 && index < this.Factions.Count)
			{
				this.SelectedItem.IsSelected = false;
				this.SelectedItem = this.Factions[index];
				this.SelectedItem.IsSelected = true;
			}
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000A244 File Offset: 0x00008444
		public void ExecuteRandomize()
		{
			int num = MBRandom.RandomInt(this.Factions.Count);
			this.SelectFaction(num);
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000A269 File Offset: 0x00008469
		private void OnFactionSelected(FactionItemVM faction)
		{
			this.SelectedItem = faction;
			this._onSelectionChanged(faction.Faction);
			this.SelectedFactionName = this.SelectedItem.Faction.Name.ToString();
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600019C RID: 412 RVA: 0x0000A29E File Offset: 0x0000849E
		// (set) Token: 0x0600019D RID: 413 RVA: 0x0000A2A6 File Offset: 0x000084A6
		[DataSourceProperty]
		public MBBindingList<FactionItemVM> Factions
		{
			get
			{
				return this._factions;
			}
			set
			{
				if (value != this._factions)
				{
					this._factions = value;
					base.OnPropertyChangedWithValue<MBBindingList<FactionItemVM>>(value, "Factions");
				}
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600019E RID: 414 RVA: 0x0000A2C4 File Offset: 0x000084C4
		// (set) Token: 0x0600019F RID: 415 RVA: 0x0000A2CC File Offset: 0x000084CC
		[DataSourceProperty]
		public string SelectedFactionName
		{
			get
			{
				return this._selectedFactionName;
			}
			set
			{
				if (value != this._selectedFactionName)
				{
					this._selectedFactionName = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectedFactionName");
				}
			}
		}

		// Token: 0x0400010B RID: 267
		public FactionItemVM SelectedItem;

		// Token: 0x0400010C RID: 268
		private Action<BasicCultureObject> _onSelectionChanged;

		// Token: 0x0400010D RID: 269
		private MBBindingList<FactionItemVM> _factions;

		// Token: 0x0400010E RID: 270
		private string _selectedFactionName;
	}
}
