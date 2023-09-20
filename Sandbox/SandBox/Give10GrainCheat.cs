using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace SandBox
{
	public class Give10GrainCheat : GameplayCheatItem
	{
		public override void ExecuteCheat()
		{
			MBReadOnlyList<ItemObject> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<ItemObject>();
			ItemObject itemObject;
			if (objectTypeList == null)
			{
				itemObject = null;
			}
			else
			{
				itemObject = objectTypeList.FirstOrDefault((ItemObject i) => i.StringId == "grain");
			}
			ItemObject itemObject2 = itemObject;
			if (itemObject2 != null)
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
				itemRoster.AddToCounts(itemObject2, 10);
			}
		}

		public override TextObject GetName()
		{
			return new TextObject("{=Jdc2aaYo}Give 10 Grain", null);
		}
	}
}
