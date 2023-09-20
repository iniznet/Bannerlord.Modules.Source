using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	public class CharacterCreationClanNamingStageVM : CharacterCreationStageBaseVM
	{
		public BasicCharacterObject Character { get; private set; }

		public int ShieldSlotIndex { get; private set; } = 3;

		public ItemRosterElement ShieldRosterElement { get; private set; }

		public CharacterCreationClanNamingStageVM(BasicCharacterObject character, Banner banner, CharacterCreation characterCreation, Action affirmativeAction, TextObject affirmativeActionText, Action negativeAction, TextObject negativeActionText, int currentStageIndex, int totalStagesCount, int furthestIndex, Action<int> goToIndex)
			: base(characterCreation, affirmativeAction, affirmativeActionText, negativeAction, negativeActionText, currentStageIndex, totalStagesCount, furthestIndex, goToIndex)
		{
			this.Character = character;
			this.ClanName = Hero.MainHero.Clan.Name.ToString();
			ItemObject itemObject = this.FindShield();
			this.ShieldRosterElement = new ItemRosterElement(itemObject, 1, null);
			this.ClanBanner = new ImageIdentifierVM(BannerCode.CreateFrom(Hero.MainHero.Clan.Banner), true);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			base.Title = new TextObject("{=wNUcqcJP}Clan Name", null).ToString();
			base.Description = new TextObject("{=JJiKk4ow}Select your family name: ", null).ToString();
			this.BottomHintText = new TextObject("{=dbBAJ8yi}You can change your banner and clan name later on clan screen", null).ToString();
		}

		public override bool CanAdvanceToNextStage()
		{
			Tuple<bool, string> tuple = FactionHelper.IsClanNameApplicable(this.ClanName);
			this.ClanNameNotApplicableReason = tuple.Item2;
			return tuple.Item1;
		}

		public override void OnNextStage()
		{
			this._affirmativeAction();
		}

		public override void OnPreviousStage()
		{
			this._negativeAction();
		}

		private ItemObject FindShield()
		{
			for (int i = 0; i < 4; i++)
			{
				EquipmentElement equipmentFromSlot = this.Character.Equipment.GetEquipmentFromSlot((EquipmentIndex)i);
				ItemObject item = equipmentFromSlot.Item;
				if (((item != null) ? item.PrimaryWeapon : null) != null && equipmentFromSlot.Item.PrimaryWeapon.IsShield && equipmentFromSlot.Item.IsUsingTableau)
				{
					return equipmentFromSlot.Item;
				}
			}
			foreach (ItemObject itemObject in Game.Current.ObjectManager.GetObjectTypeList<ItemObject>())
			{
				if (itemObject.PrimaryWeapon != null && itemObject.PrimaryWeapon.IsShield && itemObject.IsUsingTableau)
				{
					return itemObject;
				}
			}
			return null;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey == null)
			{
				return;
			}
			doneInputKey.OnFinalize();
		}

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		[DataSourceProperty]
		public string ClanName
		{
			get
			{
				return this._clanName;
			}
			set
			{
				if (value != this._clanName)
				{
					this._clanName = value;
					base.OnPropertyChangedWithValue<string>(value, "ClanName");
					base.OnPropertyChanged("CanAdvance");
				}
			}
		}

		[DataSourceProperty]
		public string ClanNameNotApplicableReason
		{
			get
			{
				return this._clanNameNotApplicableReason;
			}
			set
			{
				if (value != this._clanNameNotApplicableReason)
				{
					this._clanNameNotApplicableReason = value;
					base.OnPropertyChangedWithValue<string>(value, "ClanNameNotApplicableReason");
				}
			}
		}

		[DataSourceProperty]
		public string BottomHintText
		{
			get
			{
				return this._bottomHintText;
			}
			set
			{
				if (value != this._bottomHintText)
				{
					this._bottomHintText = value;
					base.OnPropertyChangedWithValue<string>(value, "BottomHintText");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM ClanBanner
		{
			get
			{
				return this._clanBanner;
			}
			set
			{
				if (value != this._clanBanner)
				{
					this._clanBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ClanBanner");
				}
			}
		}

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private string _clanName;

		private string _clanNameNotApplicableReason;

		private string _bottomHintText;

		private ImageIdentifierVM _clanBanner;
	}
}
