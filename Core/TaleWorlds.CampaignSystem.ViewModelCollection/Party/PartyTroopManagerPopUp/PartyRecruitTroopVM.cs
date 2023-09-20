using System;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyTroopManagerPopUp
{
	public class PartyRecruitTroopVM : PartyTroopManagerVM
	{
		public PartyRecruitTroopVM(PartyVM partyVM)
			: base(partyVM)
		{
			this.RefreshValues();
			base.IsUpgradePopUp = false;
			this._openButtonEnabledHint = new TextObject("{=tnbCJyax}Some of your prisoners are recruitable.", null);
			this._openButtonNoTroopsHint = new TextObject("{=1xf8rHLH}You don't have any recruitable prisoners.", null);
			this._openButtonIrrelevantScreenHint = new TextObject("{=zduu7dpz}Prisoners are not recruitable in this screen.", null);
			this._openButtonUpgradesDisabledHint = new TextObject("{=HfsUngkh}Recruitment is currently disabled.", null);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			base.TitleText = new TextObject("{=b8CqpGHx}Recruit Prisoners", null).ToString();
			this.EffectText = new TextObject("{=opVqBNLh}Effect", null).ToString();
			this.RecruitText = new TextObject("{=recruitVerb}Recruit", null).ToString();
			this.RecruitAllText = new TextObject("{=YJaNtktT}Recruit All", null).ToString();
		}

		public void OnTroopRecruited(PartyCharacterVM recruitedCharacter)
		{
			if (!base.IsOpen)
			{
				return;
			}
			this._hasMadeChanges = true;
			PartyTroopManagerItemVM partyTroopManagerItemVM = base.Troops.FirstOrDefault((PartyTroopManagerItemVM x) => x.PartyCharacter == recruitedCharacter);
			recruitedCharacter.UpdateRecruitable();
			if (!recruitedCharacter.IsTroopRecruitable)
			{
				base.Troops.Remove(partyTroopManagerItemVM);
			}
			base.UpdateLabels();
		}

		public override void OpenPopUp()
		{
			base.OpenPopUp();
			this.PopulateTroops();
		}

		public override void ExecuteDone()
		{
			base.ExecuteDone();
			this._partyVM.OnRecruitPopUpClosed(false);
		}

		public override void ExecuteCancel()
		{
			base.ShowCancelInquiry(new Action(this.ConfirmCancel));
		}

		protected override void ConfirmCancel()
		{
			base.ConfirmCancel();
			this._partyVM.OnRecruitPopUpClosed(true);
		}

		private void PopulateTroops()
		{
			base.Troops = new MBBindingList<PartyTroopManagerItemVM>();
			foreach (PartyCharacterVM partyCharacterVM in this._partyVM.MainPartyPrisoners)
			{
				if (partyCharacterVM.IsTroopRecruitable)
				{
					base.Troops.Add(new PartyTroopManagerItemVM(partyCharacterVM, new Action<PartyTroopManagerItemVM>(base.SetFocusedCharacter)));
				}
			}
		}

		public override void ExecuteItemPrimaryAction()
		{
		}

		public override void ExecuteItemSecondaryAction()
		{
			PartyTroopManagerItemVM focusedTroop = base.FocusedTroop;
			if (focusedTroop == null)
			{
				return;
			}
			focusedTroop.PartyCharacter.ExecuteRecruitTroop();
		}

		public void ExecuteRecruitAll()
		{
			for (int i = base.Troops.Count - 1; i >= 0; i--)
			{
				PartyCharacterVM partyCharacter = base.Troops[i].PartyCharacter;
				if (partyCharacter != null)
				{
					partyCharacter.RecruitAll();
				}
			}
		}

		[DataSourceProperty]
		public string EffectText
		{
			get
			{
				return this._effectText;
			}
			set
			{
				if (value != this._effectText)
				{
					this._effectText = value;
					base.OnPropertyChangedWithValue<string>(value, "EffectText");
				}
			}
		}

		[DataSourceProperty]
		public string RecruitText
		{
			get
			{
				return this._recruitText;
			}
			set
			{
				if (value != this._recruitText)
				{
					this._recruitText = value;
					base.OnPropertyChangedWithValue<string>(value, "RecruitText");
				}
			}
		}

		[DataSourceProperty]
		public string RecruitAllText
		{
			get
			{
				return this._recruitAllText;
			}
			set
			{
				if (value != this._recruitAllText)
				{
					this._recruitAllText = value;
					base.OnPropertyChangedWithValue<string>(value, "RecruitAllText");
				}
			}
		}

		private string _effectText;

		private string _recruitText;

		private string _recruitAllText;
	}
}
