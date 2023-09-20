using System;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyTroopManagerPopUp
{
	// Token: 0x0200002E RID: 46
	public class PartyRecruitTroopVM : PartyTroopManagerVM
	{
		// Token: 0x06000480 RID: 1152 RVA: 0x00018478 File Offset: 0x00016678
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

		// Token: 0x06000481 RID: 1153 RVA: 0x000184E0 File Offset: 0x000166E0
		public override void RefreshValues()
		{
			base.RefreshValues();
			base.TitleText = new TextObject("{=b8CqpGHx}Recruit Prisoners", null).ToString();
			this.EffectText = new TextObject("{=opVqBNLh}Effect", null).ToString();
			this.RecruitText = new TextObject("{=recruitVerb}Recruit", null).ToString();
			this.RecruitAllText = new TextObject("{=YJaNtktT}Recruit All", null).ToString();
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x0001854C File Offset: 0x0001674C
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

		// Token: 0x06000483 RID: 1155 RVA: 0x000185B9 File Offset: 0x000167B9
		public override void OpenPopUp()
		{
			base.OpenPopUp();
			this.PopulateTroops();
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x000185C7 File Offset: 0x000167C7
		public override void ExecuteDone()
		{
			base.ExecuteDone();
			this._partyVM.OnRecruitPopUpClosed(false);
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x000185DB File Offset: 0x000167DB
		public override void ExecuteCancel()
		{
			base.ShowCancelInquiry(new Action(this.ConfirmCancel));
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x000185F0 File Offset: 0x000167F0
		protected override void ConfirmCancel()
		{
			base.ConfirmCancel();
			this._partyVM.OnRecruitPopUpClosed(true);
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x00018604 File Offset: 0x00016804
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

		// Token: 0x06000488 RID: 1160 RVA: 0x00018680 File Offset: 0x00016880
		public override void ExecuteItemPrimaryAction()
		{
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x00018682 File Offset: 0x00016882
		public override void ExecuteItemSecondaryAction()
		{
			PartyTroopManagerItemVM focusedTroop = base.FocusedTroop;
			if (focusedTroop == null)
			{
				return;
			}
			focusedTroop.PartyCharacter.ExecuteRecruitTroop();
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x0001869C File Offset: 0x0001689C
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

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x0600048B RID: 1163 RVA: 0x000186DD File Offset: 0x000168DD
		// (set) Token: 0x0600048C RID: 1164 RVA: 0x000186E5 File Offset: 0x000168E5
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

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x0600048D RID: 1165 RVA: 0x00018708 File Offset: 0x00016908
		// (set) Token: 0x0600048E RID: 1166 RVA: 0x00018710 File Offset: 0x00016910
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

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x0600048F RID: 1167 RVA: 0x00018733 File Offset: 0x00016933
		// (set) Token: 0x06000490 RID: 1168 RVA: 0x0001873B File Offset: 0x0001693B
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

		// Token: 0x040001F0 RID: 496
		private string _effectText;

		// Token: 0x040001F1 RID: 497
		private string _recruitText;

		// Token: 0x040001F2 RID: 498
		private string _recruitAllText;
	}
}
