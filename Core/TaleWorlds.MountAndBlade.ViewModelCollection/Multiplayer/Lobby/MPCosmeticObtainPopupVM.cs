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
	// Token: 0x02000057 RID: 87
	public class MPCosmeticObtainPopupVM : ViewModel
	{
		// Token: 0x0600074B RID: 1867 RVA: 0x0001D1FC File Offset: 0x0001B3FC
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

		// Token: 0x0600074C RID: 1868 RVA: 0x0001D438 File Offset: 0x0001B638
		public override void RefreshValues()
		{
			this.ObtainDescriptionText = new TextObject("{=7uILxbP5}You will obtain this item", null).ToString();
			this.ContinueText = GameTexts.FindText("str_continue", null).ToString();
			this.NotEnoughLootText = new TextObject("{=FzFqhHKU}Not enough loot", null).ToString();
			this.PreviewAsText = new TextObject("{=V0bpuzV3}Preview as", null).ToString();
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x0001D4A0 File Offset: 0x0001B6A0
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

		// Token: 0x0600074E RID: 1870 RVA: 0x0001D564 File Offset: 0x0001B764
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

		// Token: 0x0600074F RID: 1871 RVA: 0x0001D65C File Offset: 0x0001B85C
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

		// Token: 0x06000750 RID: 1872 RVA: 0x0001D6E4 File Offset: 0x0001B8E4
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

		// Token: 0x06000751 RID: 1873 RVA: 0x0001D71D File Offset: 0x0001B91D
		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x0001D728 File Offset: 0x0001B928
		private void OnCultureSelection(MPCultureItemVM cultureItem)
		{
			ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(this._cultureShieldItemIDs[cultureItem.Culture]);
			Banner banner = Banner.CreateOneColoredBannerWithOneIcon(cultureItem.Culture.ForegroundColor1, cultureItem.Culture.ForegroundColor2, this.SigilItem.IconID);
			this.ItemVisual.FillFrom(new EquipmentElement(@object, null, null, false), BannerCode.CreateFrom(banner).Code);
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06000753 RID: 1875 RVA: 0x0001D797 File Offset: 0x0001B997
		// (set) Token: 0x06000754 RID: 1876 RVA: 0x0001D79F File Offset: 0x0001B99F
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

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x06000755 RID: 1877 RVA: 0x0001D7BD File Offset: 0x0001B9BD
		// (set) Token: 0x06000756 RID: 1878 RVA: 0x0001D7C5 File Offset: 0x0001B9C5
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

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x06000757 RID: 1879 RVA: 0x0001D7E3 File Offset: 0x0001B9E3
		// (set) Token: 0x06000758 RID: 1880 RVA: 0x0001D7EB File Offset: 0x0001B9EB
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

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x06000759 RID: 1881 RVA: 0x0001D809 File Offset: 0x0001BA09
		// (set) Token: 0x0600075A RID: 1882 RVA: 0x0001D811 File Offset: 0x0001BA11
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

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x0600075B RID: 1883 RVA: 0x0001D82F File Offset: 0x0001BA2F
		// (set) Token: 0x0600075C RID: 1884 RVA: 0x0001D837 File Offset: 0x0001BA37
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

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x0600075D RID: 1885 RVA: 0x0001D855 File Offset: 0x0001BA55
		// (set) Token: 0x0600075E RID: 1886 RVA: 0x0001D85D File Offset: 0x0001BA5D
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

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x0600075F RID: 1887 RVA: 0x0001D87B File Offset: 0x0001BA7B
		// (set) Token: 0x06000760 RID: 1888 RVA: 0x0001D883 File Offset: 0x0001BA83
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

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06000761 RID: 1889 RVA: 0x0001D8A6 File Offset: 0x0001BAA6
		// (set) Token: 0x06000762 RID: 1890 RVA: 0x0001D8AE File Offset: 0x0001BAAE
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

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000763 RID: 1891 RVA: 0x0001D8D1 File Offset: 0x0001BAD1
		// (set) Token: 0x06000764 RID: 1892 RVA: 0x0001D8D9 File Offset: 0x0001BAD9
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

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000765 RID: 1893 RVA: 0x0001D8FC File Offset: 0x0001BAFC
		// (set) Token: 0x06000766 RID: 1894 RVA: 0x0001D904 File Offset: 0x0001BB04
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

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06000767 RID: 1895 RVA: 0x0001D927 File Offset: 0x0001BB27
		// (set) Token: 0x06000768 RID: 1896 RVA: 0x0001D92F File Offset: 0x0001BB2F
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

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06000769 RID: 1897 RVA: 0x0001D952 File Offset: 0x0001BB52
		// (set) Token: 0x0600076A RID: 1898 RVA: 0x0001D95A File Offset: 0x0001BB5A
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

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x0600076B RID: 1899 RVA: 0x0001D97D File Offset: 0x0001BB7D
		// (set) Token: 0x0600076C RID: 1900 RVA: 0x0001D985 File Offset: 0x0001BB85
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

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x0600076D RID: 1901 RVA: 0x0001D9A8 File Offset: 0x0001BBA8
		// (set) Token: 0x0600076E RID: 1902 RVA: 0x0001D9B0 File Offset: 0x0001BBB0
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

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x0600076F RID: 1903 RVA: 0x0001D9CE File Offset: 0x0001BBCE
		// (set) Token: 0x06000770 RID: 1904 RVA: 0x0001D9D6 File Offset: 0x0001BBD6
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

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06000771 RID: 1905 RVA: 0x0001D9F4 File Offset: 0x0001BBF4
		// (set) Token: 0x06000772 RID: 1906 RVA: 0x0001D9FC File Offset: 0x0001BBFC
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

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06000773 RID: 1907 RVA: 0x0001DA1A File Offset: 0x0001BC1A
		// (set) Token: 0x06000774 RID: 1908 RVA: 0x0001DA22 File Offset: 0x0001BC22
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

		// Token: 0x040003B5 RID: 949
		private readonly Action<string, int> _onItemObtained;

		// Token: 0x040003B6 RID: 950
		private readonly Func<string> _getContinueKeyText;

		// Token: 0x040003B7 RID: 951
		private string _activeCosmeticID = string.Empty;

		// Token: 0x040003B8 RID: 952
		private readonly Dictionary<BasicCultureObject, string> _cultureShieldItemIDs;

		// Token: 0x040003B9 RID: 953
		private TextObject _currentLootTextObject = new TextObject("{=7vbGaapv}Your Loot: {LOOT}", null);

		// Token: 0x040003BA RID: 954
		private bool _isEnabled;

		// Token: 0x040003BB RID: 955
		private bool _canObtain;

		// Token: 0x040003BC RID: 956
		private bool _isOpenedWithArmoryItem;

		// Token: 0x040003BD RID: 957
		private bool _isOpenedWithSigilItem;

		// Token: 0x040003BE RID: 958
		private bool _isObtainSuccessful;

		// Token: 0x040003BF RID: 959
		private int _obtainState;

		// Token: 0x040003C0 RID: 960
		private string _obtainDescriptionText;

		// Token: 0x040003C1 RID: 961
		private string _continueText;

		// Token: 0x040003C2 RID: 962
		private string _notEnoughLootText;

		// Token: 0x040003C3 RID: 963
		private string _obtainResultText;

		// Token: 0x040003C4 RID: 964
		private string _previewAsText;

		// Token: 0x040003C5 RID: 965
		private string _currentLootText;

		// Token: 0x040003C6 RID: 966
		private string _clickToCloseText;

		// Token: 0x040003C7 RID: 967
		private MPLobbyCosmeticSigilItemVM _sigilItem;

		// Token: 0x040003C8 RID: 968
		private MPArmoryCosmeticItemVM _item;

		// Token: 0x040003C9 RID: 969
		private ItemCollectionElementViewModel _itemVisual;

		// Token: 0x040003CA RID: 970
		private MBBindingList<MPCultureItemVM> _cultures;

		// Token: 0x02000178 RID: 376
		public enum CosmeticObtainState
		{
			// Token: 0x04000CA4 RID: 3236
			Initialized,
			// Token: 0x04000CA5 RID: 3237
			Ongoing,
			// Token: 0x04000CA6 RID: 3238
			FinishedSuccessfully,
			// Token: 0x04000CA7 RID: 3239
			FinishedUnsuccessfully
		}
	}
}
