using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyTroopManagerPopUp
{
	public class PartyUpgradeTroopVM : PartyTroopManagerVM
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			base.TitleText = new TextObject("{=IgoxNz2H}Upgrade Troops", null).ToString();
			this.UpgradeCostText = new TextObject("{=SK8G9QpE}Upgrd. Cost", null).ToString();
			GameTexts.SetVariable("LEFT", new TextObject("{=6bx9IhpD}Upgrades", null).ToString());
			GameTexts.SetVariable("RIGHT", new TextObject("{=guxNZZWh}Requirements", null).ToString());
			this.UpgradesAndRequirementsText = GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString();
		}

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

		public override void OpenPopUp()
		{
			base.OpenPopUp();
			this.PopulateTroops();
			this.UpdateUpgradesOfAllTroops();
		}

		public override void ExecuteDone()
		{
			base.ExecuteDone();
			this._partyVM.OnUpgradePopUpClosed(false);
		}

		public override void ExecuteCancel()
		{
			base.ShowCancelInquiry(new Action(this.ConfirmCancel));
		}

		protected override void ConfirmCancel()
		{
			base.ConfirmCancel();
			this._partyVM.OnUpgradePopUpClosed(true);
		}

		private void UpdateUpgradesOfAllTroops()
		{
			foreach (PartyTroopManagerItemVM partyTroopManagerItemVM in base.Troops)
			{
				partyTroopManagerItemVM.PartyCharacter.InitializeUpgrades();
			}
		}

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

		public override void ExecuteItemPrimaryAction()
		{
			PartyTroopManagerItemVM focusedTroop = base.FocusedTroop;
			PartyCharacterVM partyCharacterVM = ((focusedTroop != null) ? focusedTroop.PartyCharacter : null);
			if (partyCharacterVM != null && partyCharacterVM.Upgrades.Count > 0 && partyCharacterVM.Upgrades[0].IsAvailable)
			{
				partyCharacterVM.Upgrades[0].ExecuteUpgrade();
			}
		}

		public override void ExecuteItemSecondaryAction()
		{
			PartyTroopManagerItemVM focusedTroop = base.FocusedTroop;
			PartyCharacterVM partyCharacterVM = ((focusedTroop != null) ? focusedTroop.PartyCharacter : null);
			if (partyCharacterVM != null && partyCharacterVM.Upgrades.Count > 1 && partyCharacterVM.Upgrades[1].IsAvailable)
			{
				partyCharacterVM.Upgrades[1].ExecuteUpgrade();
			}
		}

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

		private int _disabledTroopsStartIndex = -1;

		private string _upgradeCostText;

		private string _upgradesAndRequirementsText;
	}
}
