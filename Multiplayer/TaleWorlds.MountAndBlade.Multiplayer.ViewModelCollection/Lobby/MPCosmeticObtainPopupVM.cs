using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics.CosmeticTypes;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticItem;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby
{
	public class MPCosmeticObtainPopupVM : ViewModel
	{
		public MPCosmeticObtainPopupVM(Action<string, int> onItemObtained, Func<string> getContinueKeyText)
		{
			this._onItemObtained = onItemObtained;
			this._getExitText = getContinueKeyText;
			this._characterEquipments = new List<EquipmentElement>();
			this.ItemVisual = new ItemCollectionElementViewModel();
			MBBindingList<MPCultureItemVM> mbbindingList = new MBBindingList<MPCultureItemVM>();
			mbbindingList.Add(new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("vlandia").StringId, new Action<MPCultureItemVM>(this.OnCultureSelection)));
			mbbindingList.Add(new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("sturgia").StringId, new Action<MPCultureItemVM>(this.OnCultureSelection)));
			mbbindingList.Add(new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("empire").StringId, new Action<MPCultureItemVM>(this.OnCultureSelection)));
			mbbindingList.Add(new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("battania").StringId, new Action<MPCultureItemVM>(this.OnCultureSelection)));
			mbbindingList.Add(new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("khuzait").StringId, new Action<MPCultureItemVM>(this.OnCultureSelection)));
			mbbindingList.Add(new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("aserai").StringId, new Action<MPCultureItemVM>(this.OnCultureSelection)));
			this.Cultures = mbbindingList;
			Dictionary<BasicCultureObject, string> dictionary = new Dictionary<BasicCultureObject, string>();
			BasicCultureObject culture = this.Cultures[0].Culture;
			dictionary[culture] = "mp_tall_heater_shield_light";
			BasicCultureObject culture2 = this.Cultures[1].Culture;
			dictionary[culture2] = "mp_worn_kite_shield";
			BasicCultureObject culture3 = this.Cultures[2].Culture;
			dictionary[culture3] = "mp_leather_bound_kite_shield";
			BasicCultureObject culture4 = this.Cultures[3].Culture;
			dictionary[culture4] = "mp_highland_riders_shield";
			BasicCultureObject culture5 = this.Cultures[4].Culture;
			dictionary[culture5] = "mp_eastern_wicker_shield";
			BasicCultureObject culture6 = this.Cultures[5].Culture;
			dictionary[culture6] = "mp_desert_oval_shield";
			this._cultureShieldItemIDs = dictionary;
			this.CharacterVisual = new CharacterViewModel();
			MPArmoryCosmeticsVM.OnEquipmentRefreshed += this.OnCharacterEquipmentRefreshed;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			this.ContinueText = GameTexts.FindText("str_continue", null).ToString();
			this.NotEnoughLootText = new TextObject("{=FzFqhHKU}Not enough loot", null).ToString();
			this.PreviewAsText = new TextObject("{=V0bpuzV3}Preview as", null).ToString();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			MPArmoryCosmeticsVM.OnEquipmentRefreshed -= this.OnCharacterEquipmentRefreshed;
		}

		private void OnCharacterEquipmentRefreshed(List<EquipmentElement> equipments)
		{
			this._characterEquipments.Clear();
			this._characterEquipments.AddRange(equipments);
		}

		public void OpenWith(MPArmoryCosmeticClothingItemVM item)
		{
			this.OnOpened();
			this.Item = item;
			this.ItemVisual.FillFrom(item.EquipmentElement, "");
			this.ItemVisual.BannerCode = "";
			this.ItemVisual.InitialPanRotation = 0f;
			this.ObtainDescriptionText = new TextObject("{=7uILxbP5}You will obtain this item", null).ToString();
			this._activeCosmeticID = item.CosmeticID;
			this.IsOpenedWithClothingItem = true;
			GameTexts.SetVariable("STR1", GameTexts.FindText("str_continue", null));
			GameTexts.SetVariable("STR2", item.Cost);
			this.ContinueText = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			MPArmoryCosmeticClothingItemVM item2 = this.Item;
			int? num = ((item2 != null) ? new int?(item2.Cost) : null);
			int gold = NetworkMain.GameClient.PlayerData.Gold;
			this.CanObtain = (num.GetValueOrDefault() <= gold) & (num != null);
		}

		public void OpenWith(MPArmoryCosmeticTauntItemVM item, CharacterViewModel sourceCharacter)
		{
			this.OnOpened();
			TauntCosmeticElement tauntCosmeticElement = item.TauntCosmeticElement;
			this.TauntItem = item;
			Equipment equipment = LobbyTauntHelper.PrepareForTaunt(Equipment.CreateFromEquipmentCode(sourceCharacter.EquipmentCode), tauntCosmeticElement, false);
			EquipmentIndex equipmentIndex;
			EquipmentIndex equipmentIndex2;
			bool flag;
			equipment.GetInitialWeaponIndicesToEquip(ref equipmentIndex, ref equipmentIndex2, ref flag, 0);
			this.CharacterVisual.RightHandWieldedEquipmentIndex = equipmentIndex;
			if (!flag)
			{
				this.CharacterVisual.LeftHandWieldedEquipmentIndex = equipmentIndex2;
			}
			this.CharacterVisual.FillFrom(sourceCharacter, -1);
			this.CharacterVisual.SetEquipment(equipment);
			string defaultAction = TauntUsageManager.GetDefaultAction(TauntUsageManager.GetIndexOfAction(tauntCosmeticElement.Id));
			this.CharacterVisual.ExecuteStartCustomAnimation(TauntUsageManager.GetDefaultAction(TauntUsageManager.GetIndexOfAction(tauntCosmeticElement.Id)), true, 0.35f);
			this.AnimationVariationText = this.GetAnimationVariationText(defaultAction);
			this.ItemVisual.BannerCode = "";
			this.ItemVisual.InitialPanRotation = 0f;
			this._activeCosmeticID = tauntCosmeticElement.Id;
			this.IsOpenedWithTauntItem = true;
			this.ObtainDescriptionText = new TextObject("{=6mrCNU5U}You will obtain this taunt", null).ToString();
			GameTexts.SetVariable("STR1", GameTexts.FindText("str_continue", null));
			GameTexts.SetVariable("STR2", item.Cost);
			this.ContinueText = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			MPArmoryCosmeticTauntItemVM tauntItem = this.TauntItem;
			int? num = ((tauntItem != null) ? new int?(tauntItem.Cost) : null);
			int gold = NetworkMain.GameClient.PlayerData.Gold;
			this.CanObtain = (num.GetValueOrDefault() <= gold) & (num != null);
		}

		private string GetAnimationVariationText(string animationName)
		{
			if (animationName.EndsWith("leftstance"))
			{
				return new TextObject("{=8DSymjRe}Left Stance", null).ToString();
			}
			if (animationName.EndsWith("bow"))
			{
				return new TextObject("{=5rj7xQE4}Bow", null).ToString();
			}
			return new TextObject("{=fMSYE6Ii}Default", null).ToString();
		}

		public void ExecuteSelectNextAnimation(int increment)
		{
			MPArmoryCosmeticTauntItemVM tauntItem = this.TauntItem;
			if (((tauntItem != null) ? tauntItem.TauntCosmeticElement : null) == null)
			{
				Debug.FailedAssert("Invalid taunt cosmetic item", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\Lobby\\MPCosmeticObtainPopupVM.cs", "ExecuteSelectNextAnimation", 178);
				return;
			}
			TauntUsageManager.TauntUsageSet usageSet = TauntUsageManager.GetUsageSet(this._tauntItem.TauntCosmeticElement.Id);
			if (usageSet == null)
			{
				Debug.FailedAssert("No usage set for taunt: " + this.TauntItem.TauntCosmeticElement.Id, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\Lobby\\MPCosmeticObtainPopupVM.cs", "ExecuteSelectNextAnimation", 186);
				return;
			}
			MBReadOnlyList<TauntUsageManager.TauntUsage> usages = usageSet.GetUsages();
			if (usages == null || usages.Count == 0)
			{
				Debug.FailedAssert("No usages assigned for taunt usage set: " + this.TauntItem.TauntCosmeticElement.Id, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\Lobby\\MPCosmeticObtainPopupVM.cs", "ExecuteSelectNextAnimation", 194);
				return;
			}
			this._currentTauntUsageIndex += increment;
			if (this._currentTauntUsageIndex >= usages.Count)
			{
				this._currentTauntUsageIndex = 0;
			}
			else if (this._currentTauntUsageIndex < 0)
			{
				this._currentTauntUsageIndex = usages.Count - 1;
			}
			string action = usages[this._currentTauntUsageIndex].GetAction();
			this.CharacterVisual.ExecuteStartCustomAnimation(usages[this._currentTauntUsageIndex].GetAction(), true, 0f);
			this.AnimationVariationText = this.GetAnimationVariationText(action);
		}

		public void OpenWith(MPLobbyCosmeticSigilItemVM sigilItem)
		{
			this.OnOpened();
			this.SigilItem = sigilItem;
			this._activeCosmeticID = sigilItem.CosmeticID;
			this.IsOpenedWithSigilItem = true;
			this.ItemVisual.InitialPanRotation = -3.3f;
			this.ObtainDescriptionText = new TextObject("{=7uILxbP5}You will obtain this item", null).ToString();
			MPCultureItemVM mpcultureItemVM = this.Cultures.FirstOrDefault((MPCultureItemVM c) => c.IsSelected);
			if (mpcultureItemVM != null)
			{
				mpcultureItemVM.IsSelected = false;
			}
			this.Cultures[0].IsSelected = true;
			this.OnCultureSelection(this.Cultures[0]);
			GameTexts.SetVariable("STR1", GameTexts.FindText("str_continue", null));
			GameTexts.SetVariable("STR2", sigilItem.Cost);
			this.ContinueText = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			this.CanObtain = this.SigilItem.Cost <= NetworkMain.GameClient.PlayerData.Gold;
		}

		private void OnOpened()
		{
			this.Item = null;
			this.SigilItem = null;
			this.IsOpenedWithSigilItem = false;
			this.IsOpenedWithClothingItem = false;
			this.IsOpenedWithTauntItem = false;
			this.IsObtainSuccessful = false;
			this.ObtainState = 0;
			this.IsEnabled = true;
			this._currentLootTextObject.SetTextVariable("LOOT", NetworkMain.GameClient.PlayerData.Gold);
			this.CurrentLootText = this._currentLootTextObject.ToString();
			Func<string> getExitText = this._getExitText;
			this.ClickToCloseText = ((getExitText != null) ? getExitText() : null);
		}

		internal async void ExecuteAction()
		{
			if (this.ObtainState == 2 || this.ObtainState == 3)
			{
				this.ExecuteClosePopup();
			}
			else if (this.ObtainState == 0)
			{
				this.ObtainState = 1;
				ValueTuple<bool, int> valueTuple = await NetworkMain.GameClient.BuyCosmetic(this._activeCosmeticID);
				bool item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				this.ContinueText = GameTexts.FindText("str_continue", null).ToString();
				if (item)
				{
					if (this.Item != null)
					{
						this.Item.IsUnlocked = true;
					}
					else if (this.SigilItem != null)
					{
						this.SigilItem.IsUnlocked = true;
					}
					NetworkMain.GameClient.PlayerData.Gold = item2;
					this.ObtainResultText = new TextObject("{=V0k0urbO}Item obtained", null).ToString();
					string text = (this.IsOpenedWithSigilItem ? this.SigilItem.CosmeticID : (this.IsOpenedWithClothingItem ? this.Item.CosmeticID : string.Empty));
					this._onItemObtained(text, item2);
					this.IsObtainSuccessful = true;
					this.ObtainState = 2;
					SoundEvent.PlaySound2D("event:/ui/multiplayer/shop_purchase_complete");
				}
				else
				{
					this.ObtainResultText = new TextObject("{=XtVZe9cC}Item can not be obtained", null).ToString();
					this.IsObtainSuccessful = false;
					this.ObtainState = 3;
				}
			}
		}

		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		private void OnCultureSelection(MPCultureItemVM cultureItem)
		{
			ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(this._cultureShieldItemIDs[cultureItem.Culture]);
			Banner banner = Banner.CreateOneColoredBannerWithOneIcon(cultureItem.Culture.ForegroundColor1, cultureItem.Culture.ForegroundColor2, this.SigilItem.IconID);
			this.ItemVisual.FillFrom(new EquipmentElement(@object, null, null, false), BannerCode.CreateFrom(banner).Code);
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool CanObtain
		{
			get
			{
				return this._canObtain;
			}
			set
			{
				if (value != this._canObtain)
				{
					this._canObtain = value;
					base.OnPropertyChangedWithValue(value, "CanObtain");
				}
			}
		}

		[DataSourceProperty]
		public bool IsOpenedWithClothingItem
		{
			get
			{
				return this._isOpenedWithClothingItem;
			}
			set
			{
				if (value != this._isOpenedWithClothingItem)
				{
					this._isOpenedWithClothingItem = value;
					base.OnPropertyChangedWithValue(value, "IsOpenedWithClothingItem");
				}
			}
		}

		[DataSourceProperty]
		public bool IsOpenedWithSigilItem
		{
			get
			{
				return this._isOpenedWithSigilItem;
			}
			set
			{
				if (value != this._isOpenedWithSigilItem)
				{
					this._isOpenedWithSigilItem = value;
					base.OnPropertyChangedWithValue(value, "IsOpenedWithSigilItem");
				}
			}
		}

		[DataSourceProperty]
		public bool IsOpenedWithTauntItem
		{
			get
			{
				return this._isOpenedWithTauntItem;
			}
			set
			{
				if (value != this._isOpenedWithTauntItem)
				{
					this._isOpenedWithTauntItem = value;
					base.OnPropertyChangedWithValue(value, "IsOpenedWithTauntItem");
				}
			}
		}

		[DataSourceProperty]
		public bool IsObtainSuccessful
		{
			get
			{
				return this._isObtainSuccessful;
			}
			set
			{
				if (value != this._isObtainSuccessful)
				{
					this._isObtainSuccessful = value;
					base.OnPropertyChangedWithValue(value, "IsObtainSuccessful");
				}
			}
		}

		[DataSourceProperty]
		public int ObtainState
		{
			get
			{
				return this._obtainState;
			}
			set
			{
				if (value != this._obtainState)
				{
					this._obtainState = value;
					base.OnPropertyChangedWithValue(value, "ObtainState");
				}
			}
		}

		[DataSourceProperty]
		public string ObtainDescriptionText
		{
			get
			{
				return this._obtainDescriptionText;
			}
			set
			{
				if (value != this._obtainDescriptionText)
				{
					this._obtainDescriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "ObtainDescriptionText");
				}
			}
		}

		[DataSourceProperty]
		public string AnimationVariationText
		{
			get
			{
				return this._animationVariationText;
			}
			set
			{
				if (value != this._animationVariationText)
				{
					this._animationVariationText = value;
					base.OnPropertyChangedWithValue<string>(value, "AnimationVariationText");
				}
			}
		}

		[DataSourceProperty]
		public string ContinueText
		{
			get
			{
				return this._continueText;
			}
			set
			{
				if (value != this._continueText)
				{
					this._continueText = value;
					base.OnPropertyChangedWithValue<string>(value, "ContinueText");
				}
			}
		}

		[DataSourceProperty]
		public string NotEnoughLootText
		{
			get
			{
				return this._notEnoughLootText;
			}
			set
			{
				if (value != this._notEnoughLootText)
				{
					this._notEnoughLootText = value;
					base.OnPropertyChangedWithValue<string>(value, "NotEnoughLootText");
				}
			}
		}

		[DataSourceProperty]
		public string ObtainResultText
		{
			get
			{
				return this._obtainResultText;
			}
			set
			{
				if (value != this._obtainResultText)
				{
					this._obtainResultText = value;
					base.OnPropertyChangedWithValue<string>(value, "ObtainResultText");
				}
			}
		}

		[DataSourceProperty]
		public string PreviewAsText
		{
			get
			{
				return this._previewAsText;
			}
			set
			{
				if (value != this._previewAsText)
				{
					this._previewAsText = value;
					base.OnPropertyChangedWithValue<string>(value, "PreviewAsText");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentLootText
		{
			get
			{
				return this._currentLootText;
			}
			set
			{
				if (value != this._currentLootText)
				{
					this._currentLootText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentLootText");
				}
			}
		}

		[DataSourceProperty]
		public string ClickToCloseText
		{
			get
			{
				return this._clickToCloseText;
			}
			set
			{
				if (value != this._clickToCloseText)
				{
					this._clickToCloseText = value;
					base.OnPropertyChangedWithValue<string>(value, "ClickToCloseText");
				}
			}
		}

		[DataSourceProperty]
		public CharacterViewModel CharacterVisual
		{
			get
			{
				return this._characterVisual;
			}
			set
			{
				if (value != this._characterVisual)
				{
					this._characterVisual = value;
					base.OnPropertyChangedWithValue<CharacterViewModel>(value, "CharacterVisual");
				}
			}
		}

		[DataSourceProperty]
		public MPLobbyCosmeticSigilItemVM SigilItem
		{
			get
			{
				return this._sigilItem;
			}
			set
			{
				if (value != this._sigilItem)
				{
					this._sigilItem = value;
					base.OnPropertyChangedWithValue<MPLobbyCosmeticSigilItemVM>(value, "SigilItem");
				}
			}
		}

		[DataSourceProperty]
		public MPArmoryCosmeticClothingItemVM Item
		{
			get
			{
				return this._item;
			}
			set
			{
				if (value != this._item)
				{
					this._item = value;
					base.OnPropertyChangedWithValue<MPArmoryCosmeticClothingItemVM>(value, "Item");
				}
			}
		}

		[DataSourceProperty]
		public MPArmoryCosmeticTauntItemVM TauntItem
		{
			get
			{
				return this._tauntItem;
			}
			set
			{
				if (value != this._tauntItem)
				{
					this._tauntItem = value;
					base.OnPropertyChangedWithValue<MPArmoryCosmeticTauntItemVM>(value, "TauntItem");
				}
			}
		}

		[DataSourceProperty]
		public ItemCollectionElementViewModel ItemVisual
		{
			get
			{
				return this._itemVisual;
			}
			set
			{
				if (value != this._itemVisual)
				{
					this._itemVisual = value;
					base.OnPropertyChangedWithValue<ItemCollectionElementViewModel>(value, "ItemVisual");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPCultureItemVM> Cultures
		{
			get
			{
				return this._cultures;
			}
			set
			{
				if (value != this._cultures)
				{
					this._cultures = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPCultureItemVM>>(value, "Cultures");
				}
			}
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
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
					base.OnPropertyChanged("DoneInputKey");
				}
			}
		}

		private readonly Action<string, int> _onItemObtained;

		private readonly Func<string> _getExitText;

		private int _currentTauntUsageIndex;

		private string _activeCosmeticID = string.Empty;

		private readonly Dictionary<BasicCultureObject, string> _cultureShieldItemIDs;

		private TextObject _currentLootTextObject = new TextObject("{=7vbGaapv}Your Loot: {LOOT}", null);

		private const string _purchaseCompleteSound = "event:/ui/multiplayer/shop_purchase_complete";

		private List<EquipmentElement> _characterEquipments;

		private bool _isEnabled;

		private bool _canObtain;

		private bool _isOpenedWithClothingItem;

		private bool _isOpenedWithSigilItem;

		private bool _isOpenedWithTauntItem;

		private bool _isObtainSuccessful;

		private int _obtainState;

		private string _animationVariationText;

		private string _obtainDescriptionText;

		private string _continueText;

		private string _notEnoughLootText;

		private string _obtainResultText;

		private string _previewAsText;

		private string _currentLootText;

		private string _clickToCloseText;

		private CharacterViewModel _characterVisual;

		private MPLobbyCosmeticSigilItemVM _sigilItem;

		private MPArmoryCosmeticClothingItemVM _item;

		private MPArmoryCosmeticTauntItemVM _tauntItem;

		private ItemCollectionElementViewModel _itemVisual;

		private MBBindingList<MPCultureItemVM> _cultures;

		private InputKeyItemVM _doneInputKey;

		public enum CosmeticObtainState
		{
			Initialized,
			Ongoing,
			FinishedSuccessfully,
			FinishedUnsuccessfully
		}
	}
}
