using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Armory;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	public class MPCosmeticObtainPopupVM : ViewModel
	{
		public MPCosmeticObtainPopupVM(Action<string, int> onItemObtained, Func<string> getContinueKeyText)
		{
			this._onItemObtained = onItemObtained;
			this._getContinueKeyText = getContinueKeyText;
			this.ItemVisual = new ItemCollectionElementViewModel();
			this.Cultures = new MBBindingList<MPCultureItemVM>
			{
				new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("vlandia").StringId, new Action<MPCultureItemVM>(this.OnCultureSelection)),
				new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("sturgia").StringId, new Action<MPCultureItemVM>(this.OnCultureSelection)),
				new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("empire").StringId, new Action<MPCultureItemVM>(this.OnCultureSelection)),
				new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("battania").StringId, new Action<MPCultureItemVM>(this.OnCultureSelection)),
				new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("khuzait").StringId, new Action<MPCultureItemVM>(this.OnCultureSelection)),
				new MPCultureItemVM(Game.Current.ObjectManager.GetObject<BasicCultureObject>("aserai").StringId, new Action<MPCultureItemVM>(this.OnCultureSelection))
			};
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
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			this.ObtainDescriptionText = new TextObject("{=7uILxbP5}You will obtain this item", null).ToString();
			this.ContinueText = GameTexts.FindText("str_continue", null).ToString();
			this.NotEnoughLootText = new TextObject("{=FzFqhHKU}Not enough loot", null).ToString();
			this.PreviewAsText = new TextObject("{=V0bpuzV3}Preview as", null).ToString();
		}

		public void OpenWith(MPArmoryCosmeticItemVM item)
		{
			this.OnOpened();
			this.Item = item;
			this.ItemVisual.FillFrom(item.EquipmentElement, "");
			this.ItemVisual.BannerCode = "";
			this._activeCosmeticID = item.CosmeticID;
			this.IsOpenedWithArmoryItem = true;
			this.ItemVisual.InitialPanRotation = 0f;
			GameTexts.SetVariable("STR1", GameTexts.FindText("str_continue", null));
			GameTexts.SetVariable("STR2", item.Cost);
			this.ContinueText = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			this.CanObtain = this.Item.Cost <= NetworkMain.GameClient.PlayerData.Gold;
		}

		public void OpenWith(MPLobbyCosmeticSigilItemVM sigilItem)
		{
			this.OnOpened();
			this.SigilItem = sigilItem;
			this._activeCosmeticID = sigilItem.CosmeticID;
			this.IsOpenedWithSigilItem = true;
			this.ItemVisual.InitialPanRotation = -3.3f;
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
			this.IsOpenedWithArmoryItem = false;
			this.IsObtainSuccessful = false;
			this.ObtainState = 0;
			this.IsEnabled = true;
			this._currentLootTextObject.SetTextVariable("LOOT", NetworkMain.GameClient.PlayerData.Gold);
			this.CurrentLootText = this._currentLootTextObject.ToString();
			Func<string> getContinueKeyText = this._getContinueKeyText;
			this.ClickToCloseText = ((getContinueKeyText != null) ? getContinueKeyText() : null);
		}

		private async void ExecuteAction()
		{
			if (this.ObtainState == 2 || this.ObtainState == 3)
			{
				this.ExecuteClosePopup();
			}
			else
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
					string text = (this.IsOpenedWithSigilItem ? this.SigilItem.CosmeticID : (this.IsOpenedWithArmoryItem ? this.Item.CosmeticID : string.Empty));
					this._onItemObtained(text, item2);
					this.IsObtainSuccessful = true;
					this.ObtainState = 2;
					SoundEvent.PlaySound2D("event:/ui/multiplayer/purchase_success");
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
		public bool IsOpenedWithArmoryItem
		{
			get
			{
				return this._isOpenedWithArmoryItem;
			}
			set
			{
				if (value != this._isOpenedWithArmoryItem)
				{
					this._isOpenedWithArmoryItem = value;
					base.OnPropertyChangedWithValue(value, "IsOpenedWithArmoryItem");
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
		public MPArmoryCosmeticItemVM Item
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
					base.OnPropertyChangedWithValue<MPArmoryCosmeticItemVM>(value, "Item");
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

		private readonly Action<string, int> _onItemObtained;

		private readonly Func<string> _getContinueKeyText;

		private string _activeCosmeticID = string.Empty;

		private readonly Dictionary<BasicCultureObject, string> _cultureShieldItemIDs;

		private TextObject _currentLootTextObject = new TextObject("{=7vbGaapv}Your Loot: {LOOT}", null);

		private bool _isEnabled;

		private bool _canObtain;

		private bool _isOpenedWithArmoryItem;

		private bool _isOpenedWithSigilItem;

		private bool _isObtainSuccessful;

		private int _obtainState;

		private string _obtainDescriptionText;

		private string _continueText;

		private string _notEnoughLootText;

		private string _obtainResultText;

		private string _previewAsText;

		private string _currentLootText;

		private string _clickToCloseText;

		private MPLobbyCosmeticSigilItemVM _sigilItem;

		private MPArmoryCosmeticItemVM _item;

		private ItemCollectionElementViewModel _itemVisual;

		private MBBindingList<MPCultureItemVM> _cultures;

		public enum CosmeticObtainState
		{
			Initialized,
			Ongoing,
			FinishedSuccessfully,
			FinishedUnsuccessfully
		}
	}
}
