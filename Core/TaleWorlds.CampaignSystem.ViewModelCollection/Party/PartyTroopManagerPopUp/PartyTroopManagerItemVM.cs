using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyTroopManagerPopUp
{
	// Token: 0x0200002F RID: 47
	public class PartyTroopManagerItemVM : ViewModel
	{
		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000491 RID: 1169 RVA: 0x0001875E File Offset: 0x0001695E
		// (set) Token: 0x06000492 RID: 1170 RVA: 0x00018766 File Offset: 0x00016966
		public Action<PartyTroopManagerItemVM> SetFocused { get; private set; }

		// Token: 0x06000493 RID: 1171 RVA: 0x0001876F File Offset: 0x0001696F
		public PartyTroopManagerItemVM(PartyCharacterVM baseTroop, Action<PartyTroopManagerItemVM> setFocused)
		{
			this.PartyCharacter = baseTroop;
			this.SetFocused = setFocused;
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x00018785 File Offset: 0x00016985
		public void ExecuteSetFocused()
		{
			if (this.PartyCharacter.Character != null)
			{
				Action<PartyTroopManagerItemVM> setFocused = this.SetFocused;
				if (setFocused != null)
				{
					setFocused(this);
				}
				this.IsFocused = true;
			}
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x000187AD File Offset: 0x000169AD
		public void ExecuteSetUnfocused()
		{
			Action<PartyTroopManagerItemVM> setFocused = this.SetFocused;
			if (setFocused != null)
			{
				setFocused(null);
			}
			this.IsFocused = false;
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x000187C8 File Offset: 0x000169C8
		public void ExecuteOpenTroopEncyclopedia()
		{
			this.PartyCharacter.ExecuteOpenTroopEncyclopedia();
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000497 RID: 1175 RVA: 0x000187D5 File Offset: 0x000169D5
		// (set) Token: 0x06000498 RID: 1176 RVA: 0x000187DD File Offset: 0x000169DD
		[DataSourceProperty]
		public bool IsFocused
		{
			get
			{
				return this._isFocused;
			}
			set
			{
				if (value != this._isFocused)
				{
					this._isFocused = value;
					base.OnPropertyChangedWithValue(value, "IsFocused");
				}
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000499 RID: 1177 RVA: 0x000187FB File Offset: 0x000169FB
		// (set) Token: 0x0600049A RID: 1178 RVA: 0x00018803 File Offset: 0x00016A03
		[DataSourceProperty]
		public PartyCharacterVM PartyCharacter
		{
			get
			{
				return this._partyCharacter;
			}
			set
			{
				if (value != this._partyCharacter)
				{
					this._partyCharacter = value;
					base.OnPropertyChangedWithValue<PartyCharacterVM>(value, "PartyCharacter");
				}
			}
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x0600049B RID: 1179 RVA: 0x00018821 File Offset: 0x00016A21
		// (set) Token: 0x0600049C RID: 1180 RVA: 0x0001882E File Offset: 0x00016A2E
		[DataSourceProperty]
		public bool IsTroopUpgradable
		{
			get
			{
				return this.PartyCharacter.IsTroopUpgradable;
			}
			set
			{
				if (value != this.PartyCharacter.IsTroopUpgradable)
				{
					this.PartyCharacter.IsTroopUpgradable = value;
					base.OnPropertyChangedWithValue(value, "IsTroopUpgradable");
				}
			}
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x0600049D RID: 1181 RVA: 0x00018856 File Offset: 0x00016A56
		// (set) Token: 0x0600049E RID: 1182 RVA: 0x00018863 File Offset: 0x00016A63
		[DataSourceProperty]
		public bool IsTroopRecruitable
		{
			get
			{
				return this.PartyCharacter.IsTroopRecruitable;
			}
			set
			{
				if (value != this.PartyCharacter.IsTroopRecruitable)
				{
					this.PartyCharacter.IsTroopRecruitable = value;
					base.OnPropertyChangedWithValue(value, "IsTroopRecruitable");
				}
			}
		}

		// Token: 0x040001F4 RID: 500
		private bool _isFocused;

		// Token: 0x040001F5 RID: 501
		private PartyCharacterVM _partyCharacter;
	}
}
