using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace SandBox
{
	public class UnlockAllCraftingRecipesCheat : GameplayCheatItem
	{
		public override void ExecuteCheat()
		{
			CraftingCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<CraftingCampaignBehavior>();
			if (campaignBehavior == null)
			{
				return;
			}
			Type typeFromHandle = typeof(CraftingCampaignBehavior);
			FieldInfo field = typeFromHandle.GetField("_openedPartsDictionary", BindingFlags.Instance | BindingFlags.NonPublic);
			FieldInfo field2 = typeFromHandle.GetField("_openNewPartXpDictionary", BindingFlags.Instance | BindingFlags.NonPublic);
			Dictionary<CraftingTemplate, List<CraftingPiece>> dictionary = (Dictionary<CraftingTemplate, List<CraftingPiece>>)field.GetValue(campaignBehavior);
			Dictionary<CraftingTemplate, float> dictionary2 = (Dictionary<CraftingTemplate, float>)field2.GetValue(campaignBehavior);
			MethodInfo method = typeFromHandle.GetMethod("OpenPart", BindingFlags.Instance | BindingFlags.NonPublic);
			foreach (CraftingTemplate craftingTemplate in CraftingTemplate.All)
			{
				if (!dictionary.ContainsKey(craftingTemplate))
				{
					dictionary.Add(craftingTemplate, new List<CraftingPiece>());
				}
				if (!dictionary2.ContainsKey(craftingTemplate))
				{
					dictionary2.Add(craftingTemplate, 0f);
				}
				foreach (CraftingPiece craftingPiece in craftingTemplate.Pieces)
				{
					object[] array = new object[] { craftingPiece, craftingTemplate, false };
					method.Invoke(campaignBehavior, array);
				}
			}
			field.SetValue(campaignBehavior, dictionary);
			field2.SetValue(campaignBehavior, dictionary2);
		}

		public override TextObject GetName()
		{
			return new TextObject("{=pGfDkbBE}Unlock All Crafting Recipes", null);
		}
	}
}
