using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyTroopManagerPopUp
{
	public class PartyTroopManagerItemVM : ViewModel
	{
		public Action<PartyTroopManagerItemVM> SetFocused { get; private set; }

		public PartyTroopManagerItemVM(PartyCharacterVM baseTroop, Action<PartyTroopManagerItemVM> setFocused)
		{
			this.PartyCharacter = baseTroop;
			this.SetFocused = setFocused;
		}

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

		public void ExecuteSetUnfocused()
		{
			Action<PartyTroopManagerItemVM> setFocused = this.SetFocused;
			if (setFocused != null)
			{
				setFocused(null);
			}
			this.IsFocused = false;
		}

		public void ExecuteOpenTroopEncyclopedia()
		{
			this.PartyCharacter.ExecuteOpenTroopEncyclopedia();
		}

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

		private bool _isFocused;

		private PartyCharacterVM _partyCharacter;
	}
}
