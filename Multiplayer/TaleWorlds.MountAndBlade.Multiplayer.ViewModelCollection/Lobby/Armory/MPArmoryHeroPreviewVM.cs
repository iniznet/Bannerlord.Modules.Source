using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory
{
	public class MPArmoryHeroPreviewVM : ViewModel
	{
		public MPArmoryHeroPreviewVM()
		{
			this.HeroVisual = new CharacterViewModel();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			BasicCharacterObject character = this._character;
			this.ClassName = ((character != null) ? character.Name.ToString() : null) ?? "";
		}

		public void SetCharacter(BasicCharacterObject character, DynamicBodyProperties dynamicBodyProperties, int race, bool isFemale)
		{
			this._character = character;
			this.HeroVisual.FillFrom(character, -1);
			this.HeroVisual.BodyProperties = new BodyProperties(dynamicBodyProperties, character.BodyPropertyRange.BodyPropertyMin.StaticProperties).ToString();
			this.HeroVisual.IsFemale = isFemale;
			this.HeroVisual.Race = race;
			this.ClassName = character.Name.ToString();
		}

		public void SetCharacterClass(BasicCharacterObject classCharacter)
		{
			this._character = classCharacter;
			this._orgEquipmentWithoutPerks = classCharacter.Equipment;
			this.HeroVisual.SetEquipment(this._orgEquipmentWithoutPerks);
			this.HeroVisual.ArmorColor1 = classCharacter.Culture.Color;
			this.HeroVisual.ArmorColor2 = classCharacter.Culture.Color2;
			if (NetworkMain.GameClient.PlayerData != null)
			{
				string text = NetworkMain.GameClient.PlayerData.Sigil;
				if (NetworkMain.GameClient.PlayerData.IsUsingClanSigil && NetworkMain.GameClient.ClanInfo != null)
				{
					text = NetworkMain.GameClient.ClanInfo.Sigil;
				}
				Banner banner = new Banner(text, classCharacter.Culture.BackgroundColor1, classCharacter.Culture.ForegroundColor1);
				this.HeroVisual.BannerCodeText = BannerCode.CreateFrom(banner).Code;
			}
			this.ClassName = classCharacter.Name.ToString();
		}

		public void SetCharacterPerks(List<IReadOnlyPerkObject> selectedPerks)
		{
			Equipment equipment = this._orgEquipmentWithoutPerks.Clone(false);
			MPArmoryVM.ApplyPerkEffectsToEquipment(ref equipment, selectedPerks);
			this.HeroVisual.SetEquipment(equipment);
		}

		[DataSourceProperty]
		public CharacterViewModel HeroVisual
		{
			get
			{
				return this._heroVisual;
			}
			set
			{
				if (value != this._heroVisual)
				{
					this._heroVisual = value;
					base.OnPropertyChangedWithValue<CharacterViewModel>(value, "HeroVisual");
				}
			}
		}

		[DataSourceProperty]
		public string ClassName
		{
			get
			{
				return this._className;
			}
			set
			{
				if (value != this._className)
				{
					this._className = value;
					base.OnPropertyChangedWithValue<string>(value, "ClassName");
				}
			}
		}

		private BasicCharacterObject _character;

		private Equipment _orgEquipmentWithoutPerks;

		private CharacterViewModel _heroVisual;

		private string _className;
	}
}
