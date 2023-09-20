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
	// Token: 0x02000121 RID: 289
	public class CharacterCreationClanNamingStageVM : CharacterCreationStageBaseVM
	{
		// Token: 0x17000996 RID: 2454
		// (get) Token: 0x06001BF9 RID: 7161 RVA: 0x00064A40 File Offset: 0x00062C40
		// (set) Token: 0x06001BFA RID: 7162 RVA: 0x00064A48 File Offset: 0x00062C48
		public BasicCharacterObject Character { get; private set; }

		// Token: 0x17000997 RID: 2455
		// (get) Token: 0x06001BFB RID: 7163 RVA: 0x00064A51 File Offset: 0x00062C51
		// (set) Token: 0x06001BFC RID: 7164 RVA: 0x00064A59 File Offset: 0x00062C59
		public int ShieldSlotIndex { get; private set; } = 3;

		// Token: 0x17000998 RID: 2456
		// (get) Token: 0x06001BFD RID: 7165 RVA: 0x00064A62 File Offset: 0x00062C62
		// (set) Token: 0x06001BFE RID: 7166 RVA: 0x00064A6A File Offset: 0x00062C6A
		public ItemRosterElement ShieldRosterElement { get; private set; }

		// Token: 0x06001BFF RID: 7167 RVA: 0x00064A74 File Offset: 0x00062C74
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

		// Token: 0x06001C00 RID: 7168 RVA: 0x00064AFC File Offset: 0x00062CFC
		public override void RefreshValues()
		{
			base.RefreshValues();
			base.Title = new TextObject("{=!}Clan Name", null).ToString();
			base.Description = new TextObject("{=JJiKk4ow}Select your family name: ", null).ToString();
			this.BottomHintText = new TextObject("{=dbBAJ8yi}You can change your banner and clan name later on clan screen", null).ToString();
		}

		// Token: 0x06001C01 RID: 7169 RVA: 0x00064B54 File Offset: 0x00062D54
		public override bool CanAdvanceToNextStage()
		{
			Tuple<bool, string> tuple = FactionHelper.IsClanNameApplicable(this.ClanName);
			this.ClanNameNotApplicableReason = tuple.Item2;
			return tuple.Item1;
		}

		// Token: 0x06001C02 RID: 7170 RVA: 0x00064B7F File Offset: 0x00062D7F
		public override void OnNextStage()
		{
			this._affirmativeAction();
		}

		// Token: 0x06001C03 RID: 7171 RVA: 0x00064B8C File Offset: 0x00062D8C
		public override void OnPreviousStage()
		{
			this._negativeAction();
		}

		// Token: 0x06001C04 RID: 7172 RVA: 0x00064B9C File Offset: 0x00062D9C
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

		// Token: 0x06001C05 RID: 7173 RVA: 0x00064C74 File Offset: 0x00062E74
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

		// Token: 0x06001C06 RID: 7174 RVA: 0x00064C9D File Offset: 0x00062E9D
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001C07 RID: 7175 RVA: 0x00064CAC File Offset: 0x00062EAC
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x17000999 RID: 2457
		// (get) Token: 0x06001C08 RID: 7176 RVA: 0x00064CBB File Offset: 0x00062EBB
		// (set) Token: 0x06001C09 RID: 7177 RVA: 0x00064CC3 File Offset: 0x00062EC3
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

		// Token: 0x1700099A RID: 2458
		// (get) Token: 0x06001C0A RID: 7178 RVA: 0x00064CE1 File Offset: 0x00062EE1
		// (set) Token: 0x06001C0B RID: 7179 RVA: 0x00064CE9 File Offset: 0x00062EE9
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

		// Token: 0x1700099B RID: 2459
		// (get) Token: 0x06001C0C RID: 7180 RVA: 0x00064D07 File Offset: 0x00062F07
		// (set) Token: 0x06001C0D RID: 7181 RVA: 0x00064D0F File Offset: 0x00062F0F
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

		// Token: 0x1700099C RID: 2460
		// (get) Token: 0x06001C0E RID: 7182 RVA: 0x00064D3D File Offset: 0x00062F3D
		// (set) Token: 0x06001C0F RID: 7183 RVA: 0x00064D45 File Offset: 0x00062F45
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

		// Token: 0x1700099D RID: 2461
		// (get) Token: 0x06001C10 RID: 7184 RVA: 0x00064D68 File Offset: 0x00062F68
		// (set) Token: 0x06001C11 RID: 7185 RVA: 0x00064D70 File Offset: 0x00062F70
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

		// Token: 0x1700099E RID: 2462
		// (get) Token: 0x06001C12 RID: 7186 RVA: 0x00064D93 File Offset: 0x00062F93
		// (set) Token: 0x06001C13 RID: 7187 RVA: 0x00064D9B File Offset: 0x00062F9B
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

		// Token: 0x04000D3A RID: 3386
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000D3B RID: 3387
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000D3C RID: 3388
		private string _clanName;

		// Token: 0x04000D3D RID: 3389
		private string _clanNameNotApplicableReason;

		// Token: 0x04000D3E RID: 3390
		private string _bottomHintText;

		// Token: 0x04000D3F RID: 3391
		private ImageIdentifierVM _clanBanner;
	}
}
