using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000076 RID: 118
	public class DefaultItems
	{
		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06000EB7 RID: 3767 RVA: 0x00044A1B File Offset: 0x00042C1B
		private static DefaultItems Instance
		{
			get
			{
				return Campaign.Current.DefaultItems;
			}
		}

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06000EB8 RID: 3768 RVA: 0x00044A27 File Offset: 0x00042C27
		public static ItemObject Grain
		{
			get
			{
				return DefaultItems.Instance._itemGrain;
			}
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06000EB9 RID: 3769 RVA: 0x00044A33 File Offset: 0x00042C33
		public static ItemObject Meat
		{
			get
			{
				return DefaultItems.Instance._itemMeat;
			}
		}

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06000EBA RID: 3770 RVA: 0x00044A3F File Offset: 0x00042C3F
		public static ItemObject Hides
		{
			get
			{
				return DefaultItems.Instance._itemHides;
			}
		}

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06000EBB RID: 3771 RVA: 0x00044A4B File Offset: 0x00042C4B
		public static ItemObject Tools
		{
			get
			{
				return DefaultItems.Instance._itemTools;
			}
		}

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06000EBC RID: 3772 RVA: 0x00044A57 File Offset: 0x00042C57
		public static ItemObject IronOre
		{
			get
			{
				return DefaultItems.Instance._itemIronOre;
			}
		}

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x06000EBD RID: 3773 RVA: 0x00044A63 File Offset: 0x00042C63
		public static ItemObject HardWood
		{
			get
			{
				return DefaultItems.Instance._itemHardwood;
			}
		}

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x06000EBE RID: 3774 RVA: 0x00044A6F File Offset: 0x00042C6F
		public static ItemObject Charcoal
		{
			get
			{
				return DefaultItems.Instance._itemCharcoal;
			}
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06000EBF RID: 3775 RVA: 0x00044A7B File Offset: 0x00042C7B
		public static ItemObject IronIngot1
		{
			get
			{
				return DefaultItems.Instance._itemIronIngot1;
			}
		}

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06000EC0 RID: 3776 RVA: 0x00044A87 File Offset: 0x00042C87
		public static ItemObject IronIngot2
		{
			get
			{
				return DefaultItems.Instance._itemIronIngot2;
			}
		}

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06000EC1 RID: 3777 RVA: 0x00044A93 File Offset: 0x00042C93
		public static ItemObject IronIngot3
		{
			get
			{
				return DefaultItems.Instance._itemIronIngot3;
			}
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x06000EC2 RID: 3778 RVA: 0x00044A9F File Offset: 0x00042C9F
		public static ItemObject IronIngot4
		{
			get
			{
				return DefaultItems.Instance._itemIronIngot4;
			}
		}

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06000EC3 RID: 3779 RVA: 0x00044AAB File Offset: 0x00042CAB
		public static ItemObject IronIngot5
		{
			get
			{
				return DefaultItems.Instance._itemIronIngot5;
			}
		}

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06000EC4 RID: 3780 RVA: 0x00044AB7 File Offset: 0x00042CB7
		public static ItemObject IronIngot6
		{
			get
			{
				return DefaultItems.Instance._itemIronIngot6;
			}
		}

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06000EC5 RID: 3781 RVA: 0x00044AC3 File Offset: 0x00042CC3
		public static ItemObject Trash
		{
			get
			{
				return DefaultItems.Instance._itemTrash;
			}
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x00044ACF File Offset: 0x00042CCF
		public DefaultItems()
		{
			this.RegisterAll();
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x00044AE0 File Offset: 0x00042CE0
		private void RegisterAll()
		{
			this._itemGrain = this.Create("grain");
			this._itemMeat = this.Create("meat");
			this._itemHides = this.Create("hides");
			this._itemTools = this.Create("tools");
			this._itemIronOre = this.Create("iron");
			this._itemHardwood = this.Create("hardwood");
			this._itemCharcoal = this.Create("charcoal");
			this._itemIronIngot1 = this.Create("ironIngot1");
			this._itemIronIngot2 = this.Create("ironIngot2");
			this._itemIronIngot3 = this.Create("ironIngot3");
			this._itemIronIngot4 = this.Create("ironIngot4");
			this._itemIronIngot5 = this.Create("ironIngot5");
			this._itemIronIngot6 = this.Create("ironIngot6");
			this._itemTrash = this.Create("trash");
			this.InitializeAll();
		}

		// Token: 0x06000EC8 RID: 3784 RVA: 0x00044BE1 File Offset: 0x00042DE1
		private ItemObject Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<ItemObject>(new ItemObject(stringId));
		}

		// Token: 0x06000EC9 RID: 3785 RVA: 0x00044BF8 File Offset: 0x00042DF8
		private void InitializeAll()
		{
			ItemObject.InitializeTradeGood(this._itemGrain, new TextObject("{=Itv3fgJm}Grain{@Plural}loads of grain{\\@}", null), "merchandise_grain", DefaultItemCategories.Grain, 10, 10f, ItemObject.ItemTypeEnum.Goods, true);
			ItemObject.InitializeTradeGood(this._itemMeat, new TextObject("{=LmwhFv5p}Meat{@Plural}loads of meat{\\@}", null), "merchandise_meat", DefaultItemCategories.Meat, 30, 10f, ItemObject.ItemTypeEnum.Goods, true);
			ItemObject.InitializeTradeGood(this._itemHides, new TextObject("{=4kvKQuXM}Hides{@Plural}loads of hide{\\@}", null), "merchandise_hides_b", DefaultItemCategories.Hides, 50, 10f, ItemObject.ItemTypeEnum.Goods, false);
			ItemObject.InitializeTradeGood(this._itemTools, new TextObject("{=n3cjEB0X}Tools{@Plural}loads of tools{\\@}", null), "bd_pickaxe_b", DefaultItemCategories.Tools, 200, 10f, ItemObject.ItemTypeEnum.Goods, false);
			ItemObject.InitializeTradeGood(this._itemIronOre, new TextObject("{=Kw6BkhIf}Iron Ore{@Plural}loads of iron ore{\\@}", null), "iron_ore", DefaultItemCategories.Iron, 50, 10f, ItemObject.ItemTypeEnum.Goods, false);
			ItemObject.InitializeTradeGood(this._itemHardwood, new TextObject("{=ExjMoUiT}Hardwood{@Plural}hardwood logs{\\@}", null), "hardwood", DefaultItemCategories.Wood, 25, 10f, ItemObject.ItemTypeEnum.Goods, false);
			ItemObject.InitializeTradeGood(this._itemCharcoal, new TextObject("{=iQadPYNe}Charcoal{@Plural}loads of charcoal{\\@}", null), "charcoal", DefaultItemCategories.Wood, 50, 5f, ItemObject.ItemTypeEnum.Goods, false);
			ItemObject.InitializeTradeGood(this._itemIronIngot1, new TextObject("{=gOpodlt1}Crude Iron{@Plural}loads of crude iron{\\@}", null), "crude_iron", DefaultItemCategories.Iron, 20, 0.5f, ItemObject.ItemTypeEnum.Goods, false);
			ItemObject.InitializeTradeGood(this._itemIronIngot2, new TextObject("{=7HvtT8bm}Wrought Iron{@Plural}loads of wrought iron{\\@}", null), "wrought_iron", DefaultItemCategories.Iron, 30, 0.5f, ItemObject.ItemTypeEnum.Goods, false);
			ItemObject.InitializeTradeGood(this._itemIronIngot3, new TextObject("{=XHmmbnbB}Iron{@Plural}loads of iron{\\@}", null), "iron_a", DefaultItemCategories.Iron, 60, 0.5f, ItemObject.ItemTypeEnum.Goods, false);
			ItemObject.InitializeTradeGood(this._itemIronIngot4, new TextObject("{=UfuLKuaI}Steel{@Plural}loads of steel{\\@}", null), "steel", DefaultItemCategories.Iron, 100, 0.5f, ItemObject.ItemTypeEnum.Goods, false);
			ItemObject.InitializeTradeGood(this._itemIronIngot5, new TextObject("{=azjMBa86}Fine Steel{@Plural}loads of fine steel{\\@}", null), "fine_steel", DefaultItemCategories.Iron, 160, 0.5f, ItemObject.ItemTypeEnum.Goods, false);
			ItemObject.InitializeTradeGood(this._itemIronIngot6, new TextObject("{=vLVAfcta}Thamaskene Steel{@Plural}loads of thamaskene steel{\\@}", null), "thamaskene_steel", DefaultItemCategories.Iron, 260, 0.5f, ItemObject.ItemTypeEnum.Goods, false);
			ItemObject.InitializeTradeGood(this._itemTrash, new TextObject("{=ZvZN6UkU}Trash Item", null), "iron_ore", DefaultItemCategories.Unassigned, 1, 1f, ItemObject.ItemTypeEnum.Goods, false);
		}

		// Token: 0x040004C2 RID: 1218
		private const float TradeGoodWeight = 10f;

		// Token: 0x040004C3 RID: 1219
		private const float HalfWeight = 5f;

		// Token: 0x040004C4 RID: 1220
		private const float IngotWeight = 0.5f;

		// Token: 0x040004C5 RID: 1221
		private const float TrashWeight = 1f;

		// Token: 0x040004C6 RID: 1222
		private const int IngotValue = 20;

		// Token: 0x040004C7 RID: 1223
		private const int TrashValue = 1;

		// Token: 0x040004C8 RID: 1224
		private ItemObject _itemGrain;

		// Token: 0x040004C9 RID: 1225
		private ItemObject _itemMeat;

		// Token: 0x040004CA RID: 1226
		private ItemObject _itemHides;

		// Token: 0x040004CB RID: 1227
		private ItemObject _itemTools;

		// Token: 0x040004CC RID: 1228
		private ItemObject _itemIronOre;

		// Token: 0x040004CD RID: 1229
		private ItemObject _itemHardwood;

		// Token: 0x040004CE RID: 1230
		private ItemObject _itemCharcoal;

		// Token: 0x040004CF RID: 1231
		private ItemObject _itemIronIngot1;

		// Token: 0x040004D0 RID: 1232
		private ItemObject _itemIronIngot2;

		// Token: 0x040004D1 RID: 1233
		private ItemObject _itemIronIngot3;

		// Token: 0x040004D2 RID: 1234
		private ItemObject _itemIronIngot4;

		// Token: 0x040004D3 RID: 1235
		private ItemObject _itemIronIngot5;

		// Token: 0x040004D4 RID: 1236
		private ItemObject _itemIronIngot6;

		// Token: 0x040004D5 RID: 1237
		private ItemObject _itemTrash;
	}
}
