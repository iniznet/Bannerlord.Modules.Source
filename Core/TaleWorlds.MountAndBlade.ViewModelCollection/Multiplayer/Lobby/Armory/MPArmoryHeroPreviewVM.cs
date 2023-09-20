using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Armory
{
	// Token: 0x020000A8 RID: 168
	public class MPArmoryHeroPreviewVM : ViewModel
	{
		// Token: 0x06001026 RID: 4134 RVA: 0x00035AA8 File Offset: 0x00033CA8
		public MPArmoryHeroPreviewVM()
		{
			this.HeroVisual = new CharacterViewModel();
		}

		// Token: 0x06001027 RID: 4135 RVA: 0x00035ABB File Offset: 0x00033CBB
		public override void RefreshValues()
		{
			base.RefreshValues();
			BasicCharacterObject character = this._character;
			this.ClassName = ((character != null) ? character.Name.ToString() : null) ?? "";
		}

		// Token: 0x06001028 RID: 4136 RVA: 0x00035AEC File Offset: 0x00033CEC
		public void SetCharacter(BasicCharacterObject character, DynamicBodyProperties dynamicBodyProperties, int race, bool isFemale)
		{
			this._character = character;
			this.HeroVisual.FillFrom(character, -1);
			this.HeroVisual.BodyProperties = new BodyProperties(dynamicBodyProperties, character.BodyPropertyRange.BodyPropertyMin.StaticProperties).ToString();
			this.HeroVisual.IsFemale = isFemale;
			this.HeroVisual.Race = race;
			this.ClassName = character.Name.ToString();
		}

		// Token: 0x06001029 RID: 4137 RVA: 0x00035B6C File Offset: 0x00033D6C
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

		// Token: 0x0600102A RID: 4138 RVA: 0x00035C58 File Offset: 0x00033E58
		public void SetCharacterPerks(List<IReadOnlyPerkObject> selectedPerks)
		{
			Equipment equipment = this._orgEquipmentWithoutPerks.Clone(false);
			MPArmoryVM.ApplyPerkEffectsToEquipment(ref equipment, selectedPerks);
			this.HeroVisual.SetEquipment(equipment);
		}

		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x0600102B RID: 4139 RVA: 0x00035C86 File Offset: 0x00033E86
		// (set) Token: 0x0600102C RID: 4140 RVA: 0x00035C8E File Offset: 0x00033E8E
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

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x0600102D RID: 4141 RVA: 0x00035CAC File Offset: 0x00033EAC
		// (set) Token: 0x0600102E RID: 4142 RVA: 0x00035CB4 File Offset: 0x00033EB4
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

		// Token: 0x040007A5 RID: 1957
		private BasicCharacterObject _character;

		// Token: 0x040007A6 RID: 1958
		private Equipment _orgEquipmentWithoutPerks;

		// Token: 0x040007A7 RID: 1959
		private CharacterViewModel _heroVisual;

		// Token: 0x040007A8 RID: 1960
		private string _className;
	}
}
