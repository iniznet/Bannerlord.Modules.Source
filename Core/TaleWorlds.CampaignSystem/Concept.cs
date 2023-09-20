﻿using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000059 RID: 89
	public class Concept : MBObjectBase
	{
		// Token: 0x06000A3A RID: 2618 RVA: 0x00039107 File Offset: 0x00037307
		internal static void AutoGeneratedStaticCollectObjectsConcept(object o, List<object> collectedObjects)
		{
			((Concept)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x00039115 File Offset: 0x00037315
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06000A3C RID: 2620 RVA: 0x0003911E File Offset: 0x0003731E
		// (set) Token: 0x06000A3D RID: 2621 RVA: 0x00039126 File Offset: 0x00037326
		public TextObject Title { get; set; }

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06000A3E RID: 2622 RVA: 0x0003912F File Offset: 0x0003732F
		// (set) Token: 0x06000A3F RID: 2623 RVA: 0x00039137 File Offset: 0x00037337
		public TextObject Description { get; set; }

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06000A40 RID: 2624 RVA: 0x00039140 File Offset: 0x00037340
		// (set) Token: 0x06000A41 RID: 2625 RVA: 0x00039148 File Offset: 0x00037348
		public string FilterGroup { get; set; }

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06000A42 RID: 2626 RVA: 0x00039151 File Offset: 0x00037351
		// (set) Token: 0x06000A43 RID: 2627 RVA: 0x00039159 File Offset: 0x00037359
		public string LinkID { get; private set; }

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06000A44 RID: 2628 RVA: 0x00039162 File Offset: 0x00037362
		public static MBReadOnlyList<Concept> All
		{
			get
			{
				return Campaign.Current.Concepts;
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06000A45 RID: 2629 RVA: 0x0003916E File Offset: 0x0003736E
		public string EncyclopediaLink
		{
			get
			{
				return Campaign.Current.EncyclopediaManager.GetIdentifier(typeof(Concept)) + "-" + base.StringId;
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06000A46 RID: 2630 RVA: 0x00039199 File Offset: 0x00037399
		public TextObject EncyclopediaLinkWithName
		{
			get
			{
				return HyperlinkTexts.GetConceptHyperlinkText(this.EncyclopediaLink, this.Title);
			}
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x000391AC File Offset: 0x000373AC
		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			this.Title = new TextObject(node.Attributes["title"].Value, null);
			this.Description = new TextObject(node.Attributes["text"].Value, null);
			this.FilterGroup = ((node.Attributes["group"] != null) ? node.Attributes["group"].Value : "");
			this.LinkID = node.Attributes["link_id"].Value;
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x00039252 File Offset: 0x00037452
		public static bool IsGroupMember(string groupName, Concept c)
		{
			return c.FilterGroup == groupName;
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x00039260 File Offset: 0x00037460
		public static void SetConceptTextLinks()
		{
			foreach (Concept concept in Concept.All)
			{
				MBTextManager.SetTextVariable(concept.LinkID, concept.EncyclopediaLinkWithName, false);
			}
		}
	}
}
