using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace SandBox
{
	public class Give10WarhorsesCheat : GameplayCheatItem
	{
		public override void ExecuteCheat()
		{
			ItemObject itemObject = MBObjectManager.Instance.GetObjectTypeList<ItemObject>().FirstOrDefault((ItemObject i) => i.StringId == "war_horse");
			if (itemObject != null)
			{
				PartyBase mainParty = PartyBase.MainParty;
				if (mainParty == null)
				{
					return;
				}
				ItemRoster itemRoster = mainParty.ItemRoster;
				if (itemRoster == null)
				{
					return;
				}
				itemRoster.AddToCounts(itemObject, 10);
			}
		}

		public override TextObject GetName()
		{
			return new TextObject("{=4KvAwfEZ}Give 10 War Horses", null);
		}
	}
}
