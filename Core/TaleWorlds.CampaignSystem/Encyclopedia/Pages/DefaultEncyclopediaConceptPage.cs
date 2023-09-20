using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Encyclopedia.Pages
{
	[EncyclopediaModel(new Type[] { typeof(Concept) })]
	public class DefaultEncyclopediaConceptPage : EncyclopediaPage
	{
		public DefaultEncyclopediaConceptPage()
		{
			base.HomePageOrderIndex = 600;
		}

		protected override IEnumerable<EncyclopediaListItem> InitializeListItems()
		{
			foreach (Concept concept in Concept.All)
			{
				yield return new EncyclopediaListItem(concept, concept.Title.ToString(), concept.Description.ToString(), concept.StringId, base.GetIdentifier(typeof(Concept)), true, null);
			}
			List<Concept>.Enumerator enumerator = default(List<Concept>.Enumerator);
			yield break;
			yield break;
		}

		protected override IEnumerable<EncyclopediaFilterGroup> InitializeFilterItems()
		{
			List<EncyclopediaFilterGroup> list = new List<EncyclopediaFilterGroup>();
			List<EncyclopediaFilterItem> list2 = new List<EncyclopediaFilterItem>();
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=uauMia0D} Characters", null), (object c) => Concept.IsGroupMember("Characters", (Concept)c)));
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=cwRkqIt4} Kingdoms", null), (object c) => Concept.IsGroupMember("Kingdoms", (Concept)c)));
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=x6knoNnC} Clans", null), (object c) => Concept.IsGroupMember("Clans", (Concept)c)));
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=GYzkb4iB} Parties", null), (object c) => Concept.IsGroupMember("Parties", (Concept)c)));
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=u6GM5Spa} Armies", null), (object c) => Concept.IsGroupMember("Armies", (Concept)c)));
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=zPYRGJtD} Troops", null), (object c) => Concept.IsGroupMember("Troops", (Concept)c)));
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=3PUkH5Zf} Items", null), (object c) => Concept.IsGroupMember("Items", (Concept)c)));
			list2.Add(new EncyclopediaFilterItem(new TextObject("{=xKVBAL3m} Campaign Issues", null), (object c) => Concept.IsGroupMember("CampaignIssues", (Concept)c)));
			list.Add(new EncyclopediaFilterGroup(list2, new TextObject("{=tBx7XXps}Types", null)));
			return list;
		}

		protected override IEnumerable<EncyclopediaSortController> InitializeSortControllers()
		{
			return new List<EncyclopediaSortController>();
		}

		public override string GetViewFullyQualifiedName()
		{
			return "EncyclopediaConceptPage";
		}

		public override TextObject GetName()
		{
			return GameTexts.FindText("str_concepts", null);
		}

		public override TextObject GetDescriptionText()
		{
			return GameTexts.FindText("str_concepts_description", null);
		}

		public override string GetStringID()
		{
			return "EncyclopediaConcept";
		}

		public override bool IsValidEncyclopediaItem(object o)
		{
			Concept concept = o as Concept;
			return concept != null && concept.Title != null && concept.Description != null;
		}
	}
}
