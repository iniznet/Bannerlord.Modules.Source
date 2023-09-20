using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Encyclopedia.Pages
{
	// Token: 0x02000166 RID: 358
	[EncyclopediaModel(new Type[] { typeof(Concept) })]
	public class DefaultEncyclopediaConceptPage : EncyclopediaPage
	{
		// Token: 0x06001891 RID: 6289 RVA: 0x0007BF9B File Offset: 0x0007A19B
		public DefaultEncyclopediaConceptPage()
		{
			base.HomePageOrderIndex = 600;
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x0007BFAE File Offset: 0x0007A1AE
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

		// Token: 0x06001893 RID: 6291 RVA: 0x0007BFC0 File Offset: 0x0007A1C0
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

		// Token: 0x06001894 RID: 6292 RVA: 0x0007C197 File Offset: 0x0007A397
		protected override IEnumerable<EncyclopediaSortController> InitializeSortControllers()
		{
			return new List<EncyclopediaSortController>();
		}

		// Token: 0x06001895 RID: 6293 RVA: 0x0007C19E File Offset: 0x0007A39E
		public override string GetViewFullyQualifiedName()
		{
			return "EncyclopediaConceptPage";
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x0007C1A5 File Offset: 0x0007A3A5
		public override TextObject GetName()
		{
			return GameTexts.FindText("str_concepts", null);
		}

		// Token: 0x06001897 RID: 6295 RVA: 0x0007C1B2 File Offset: 0x0007A3B2
		public override TextObject GetDescriptionText()
		{
			return GameTexts.FindText("str_concepts_description", null);
		}

		// Token: 0x06001898 RID: 6296 RVA: 0x0007C1BF File Offset: 0x0007A3BF
		public override string GetStringID()
		{
			return "EncyclopediaConcept";
		}

		// Token: 0x06001899 RID: 6297 RVA: 0x0007C1C8 File Offset: 0x0007A3C8
		public override bool IsValidEncyclopediaItem(object o)
		{
			Concept concept = o as Concept;
			return concept != null && concept.Title != null && concept.Description != null;
		}
	}
}
