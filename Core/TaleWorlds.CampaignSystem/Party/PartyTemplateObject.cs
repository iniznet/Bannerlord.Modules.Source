﻿using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Party
{
	// Token: 0x0200029E RID: 670
	public sealed class PartyTemplateObject : MBObjectBase
	{
		// Token: 0x0600241D RID: 9245 RVA: 0x00099429 File Offset: 0x00097629
		internal static void AutoGeneratedStaticCollectObjectsPartyTemplateObject(object o, List<object> collectedObjects)
		{
			((PartyTemplateObject)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x0600241E RID: 9246 RVA: 0x00099437 File Offset: 0x00097637
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06002420 RID: 9248 RVA: 0x00099448 File Offset: 0x00097648
		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			this.Stacks = new MBList<PartyTemplateStack>();
			base.Deserialize(objectManager, node);
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "stacks")
				{
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						if (xmlNode2.Name == "PartyTemplateStack")
						{
							PartyTemplateStack partyTemplateStack = new PartyTemplateStack((CharacterObject)objectManager.ReadObjectReferenceFromXml("troop", typeof(CharacterObject), xmlNode2), Convert.ToInt32(xmlNode2.Attributes["min_value"].Value), Convert.ToInt32(xmlNode2.Attributes["max_value"].Value));
							this.Stacks.Add(partyTemplateStack);
						}
					}
				}
			}
		}

		// Token: 0x04000B11 RID: 2833
		public MBList<PartyTemplateStack> Stacks;
	}
}
