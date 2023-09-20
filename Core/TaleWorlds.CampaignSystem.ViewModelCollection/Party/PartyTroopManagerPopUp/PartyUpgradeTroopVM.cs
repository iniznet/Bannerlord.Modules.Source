using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyTroopManagerPopUp
{
	// Token: 0x02000031 RID: 49
	public class PartyUpgradeTroopVM : PartyTroopManagerVM
	{
		// Token: 0x060004D5 RID: 1237 RVA: 0x0001900C File Offset: 0x0001720C
		public PartyUpgradeTroopVM(PartyVM partyVM)
			: base(partyVM)
		{
			this.RefreshValues();
			base.IsUpgradePopUp = true;
			this._openButtonEnabledHint = new TextObject("{=hRSezxnT}Some of your troops are ready to upgrade.", null);
			this._openButtonNoTroopsHint = new TextObject("{=fpE7BQ7f}You don't have any upgradable troops.", null);
			this._openButtonIrrelevantScreenHint = new TextObject("{=mdvnjI72}Troops are not upgradable in this screen.", null);
			this._openButtonUpgradesDisabledHint = new TextObject("{=R4rTlKMU}Troop upgrades are currently disabled.", null);
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x00019078 File Offset: 0x00017278
		public override void RefreshValues()
		{
			base.RefreshValues();
			base.TitleText = new TextObject("{=IgoxNz2H}Upgrade Troops", null).ToString();
			this.UpgradeCostText = new TextObject("{=SK8G9QpE}Upgrd. Cost", null).ToString();
			GameTexts.SetVariable("LEFT", new TextObject("{=6bx9IhpD}Upgrades", null).ToString());
			GameTexts.SetVariable("RIGHT", new TextObject("{=guxNZZWh}Requirements", null).ToString());
			this.UpgradesAndRequirementsText = GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString();
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x00019104 File Offset: 0x00017304
		public void OnRanOutTroop(PartyCharacterVM troop)
		{
			if (!base.IsOpen)
			{
				return;
			}
			PartyTroopManagerItemVM partyTroopManagerItemVM = base.Troops.FirstOrDefault((PartyTroopManagerItemVM x) => x.PartyCharacter == troop);
			base.Troops.Remove(partyTroopManagerItemVM);
			this._disabledTroopsStartIndex--;
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x0001915C File Offset: 0x0001735C
		public void OnTroopUpgraded()
		{
			if (!base.IsOpen)
			{
				return;
			}
			this._hasMadeChanges = true;
			for (int i = 0; i < this._disabledTroopsStartIndex; i++)
			{
				if (base.Troops[i].PartyCharacter.NumOfReadyToUpgradeTroops <= 0)
				{
					this._disabledTroopsStartIndex--;
					base.Troops.RemoveAt(i);
					i--;
				}
				else if (base.Troops[i].PartyCharacter.NumOfUpgradeableTroops <= 0)
				{
					this._disabledTroopsStartIndex--;
					PartyTroopManagerItemVM partyTroopManagerItemVM = base.Troops[i];
					base.Troops.RemoveAt(i);
					base.Troops.Insert(this._disabledTroopsStartIndex, partyTroopManagerItemVM);
					i--;
				}
			}
			base.UpdateLabels();
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x00019225 File Offset: 0x00017425
		public override void OpenPopUp()
		{
			base.OpenPopUp();
			this.PopulateTroops();
			this.UpdateUpgradesOfAllTroops();
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x00019239 File Offset: 0x00017439
		public override void ExecuteDone()
		{
			base.ExecuteDone();
			this._partyVM.OnUpgradePopUpClosed(false);
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x0001924D File Offset: 0x0001744D
		public override void ExecuteCancel()
		{
			base.ShowCancelInquiry(new Action(this.ConfirmCancel));
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x00019262 File Offset: 0x00017462
		protected override void ConfirmCancel()
		{
			base.ConfirmCancel();
			this._partyVM.OnUpgradePopUpClosed(true);
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x00019278 File Offset: 0x00017478
		private void UpdateUpgradesOfAllTroops()
		{
			foreach (PartyTroopManagerItemVM partyTroopManagerItemVM in base.Troops)
			{
				partyTroopManagerItemVM.PartyCharacter.InitializeUpgrades();
			}
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x000192C8 File Offset: 0x000174C8
		private void PopulateTroops()
		{
			base.Troops = new MBBindingList<PartyTroopManagerItemVM>();
			this._disabledTroopsStartIndex = 0;
			foreach (PartyCharacterVM partyCharacterVM in this._partyVM.MainPartyTroops)
			{
				if (partyCharacterVM.IsTroopUpgradable)
				{
					base.Troops.Insert(this._disabledTroopsStartIndex, new PartyTroopManagerItemVM(partyCharacterVM, new Action<PartyTroopManagerItemVM>(base.SetFocusedCharacter)));
					this._disabledTroopsStartIndex++;
				}
				else if (partyCharacterVM.NumOfReadyToUpgradeTroops > 0)
				{
					base.Troops.Add(new PartyTroopManagerItemVM(partyCharacterVM, new Action<PartyTroopManagerItemVM>(base.SetFocusedCharacter)));
				}
			}
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x00019388 File Offset: 0x00017588
		public override void ExecuteItemPrimaryAction()
		{
			PartyTroopManagerItemVM focusedTroop = base.FocusedTroop;
			PartyCharacterVM partyCharacterVM = ((focusedTroop != null) ? focusedTroop.PartyCharacter : null);
			if (partyCharacterVM != null && partyCharacterVM.Upgrades.Count > 0 && partyCharacterVM.Upgrades[0].IsAvailable)
			{
				partyCharacterVM.Upgrades[0].ExecuteUpgrade();
			}
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x000193E0 File Offset: 0x000175E0
		public override void ExecuteItemSecondaryAction()
		{
			PartyTroopManagerItemVM focusedTroop = base.FocusedTroop;
			PartyCharacterVM partyCharacterVM = ((focusedTroop != null) ? focusedTroop.PartyCharacter : null);
			if (partyCharacterVM != null && partyCharacterVM.Upgrades.Count > 1 && partyCharacterVM.Upgrades[1].IsAvailable)
			{
				partyCharacterVM.Upgrades[1].ExecuteUpgrade();
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x060004E1 RID: 1249 RVA: 0x00019435 File Offset: 0x00017635
		// (set) Token: 0x060004E2 RID: 1250 RVA: 0x0001943D File Offset: 0x0001763D
		[DataSourceProperty]
		public string UpgradeCostText
		{
			get
			{
				return this._upgradeCostText;
			}
			set
			{
				if (value != this._upgradeCostText)
				{
					this._upgradeCostText = value;
					base.OnPropertyChangedWithValue<string>(value, "UpgradeCostText");
				}
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x060004E3 RID: 1251 RVA: 0x00019460 File Offset: 0x00017660
		// (set) Token: 0x060004E4 RID: 1252 RVA: 0x00019468 File Offset: 0x00017668
		[DataSourceProperty]
		public string UpgradesAndRequirementsText
		{
			get
			{
				return this._upgradesAndRequirementsText;
			}
			set
			{
				if (value != this._upgradesAndRequirementsText)
				{
					this._upgradesAndRequirementsText = value;
					base.OnPropertyChangedWithValue<string>(value, "UpgradesAndRequirementsText");
				}
			}
		}

		// Token: 0x04000212 RID: 530
		private int _disabledTroopsStartIndex = -1;

		// Token: 0x04000213 RID: 531
		private string _upgradeCostText;

		// Token: 0x04000214 RID: 532
		private string _upgradesAndRequirementsText;
	}
}
