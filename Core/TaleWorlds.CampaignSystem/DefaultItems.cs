using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	public class DefaultItems
	{
		private static DefaultItems Instance
		{
			get
			{
				return Campaign.Current.DefaultItems;
			}
		}

		public static ItemObject Grain
		{
			get
			{
				return DefaultItems.Instance._itemGrain;
			}
		}

		public static ItemObject Meat
		{
			get
			{
				return DefaultItems.Instance._itemMeat;
			}
		}

		public static ItemObject Hides
		{
			get
			{
				return DefaultItems.Instance._itemHides;
			}
		}

		public static ItemObject Tools
		{
			get
			{
				return DefaultItems.Instance._itemTools;
			}
		}

		public static ItemObject IronOre
		{
			get
			{
				return DefaultItems.Instance._itemIronOre;
			}
		}

		public static ItemObject HardWood
		{
			get
			{
				return DefaultItems.Instance._itemHardwood;
			}
		}

		public static ItemObject Charcoal
		{
			get
			{
				return DefaultItems.Instance._itemCharcoal;
			}
		}

		public static ItemObject IronIngot1
		{
			get
			{
				return DefaultItems.Instance._itemIronIngot1;
			}
		}

		public static ItemObject IronIngot2
		{
			get
			{
				return DefaultItems.Instance._itemIronIngot2;
			}
		}

		public static ItemObject IronIngot3
		{
			get
			{
				return DefaultItems.Instance._itemIronIngot3;
			}
		}

		public static ItemObject IronIngot4
		{
			get
			{
				return DefaultItems.Instance._itemIronIngot4;
			}
		}

		public static ItemObject IronIngot5
		{
			get
			{
				return DefaultItems.Instance._itemIronIngot5;
			}
		}

		public static ItemObject IronIngot6
		{
			get
			{
				return DefaultItems.Instance._itemIronIngot6;
			}
		}

		public static ItemObject Trash
		{
			get
			{
				return DefaultItems.Instance._itemTrash;
			}
		}

		public DefaultItems()
		{
			this.RegisterAll();
		}

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

		private ItemObject Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<ItemObject>(new ItemObject(stringId));
		}

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

		private const float TradeGoodWeight = 10f;

		private const float HalfWeight = 5f;

		private const float IngotWeight = 0.5f;

		private const float TrashWeight = 1f;

		private const int IngotValue = 20;

		private const int TrashValue = 1;

		private ItemObject _itemGrain;

		private ItemObject _itemMeat;

		private ItemObject _itemHides;

		private ItemObject _itemTools;

		private ItemObject _itemIronOre;

		private ItemObject _itemHardwood;

		private ItemObject _itemCharcoal;

		private ItemObject _itemIronIngot1;

		private ItemObject _itemIronIngot2;

		private ItemObject _itemIronIngot3;

		private ItemObject _itemIronIngot4;

		private ItemObject _itemIronIngot5;

		private ItemObject _itemIronIngot6;

		private ItemObject _itemTrash;
	}
}
