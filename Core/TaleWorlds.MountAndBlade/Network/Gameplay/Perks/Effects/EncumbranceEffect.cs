using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003BF RID: 959
	public class EncumbranceEffect : MPPerkEffect
	{
		// Token: 0x060033AA RID: 13226 RVA: 0x000D6361 File Offset: 0x000D4561
		protected EncumbranceEffect()
		{
		}

		// Token: 0x060033AB RID: 13227 RVA: 0x000D636C File Offset: 0x000D456C
		protected override void Deserialize(XmlNode node)
		{
			string text;
			if (node == null)
			{
				text = null;
			}
			else
			{
				XmlAttributeCollection attributes = node.Attributes;
				if (attributes == null)
				{
					text = null;
				}
				else
				{
					XmlAttribute xmlAttribute = attributes["is_disabled_in_warmup"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			base.IsDisabledInWarmup = ((text2 != null) ? text2.ToLower() : null) == "true";
			string text3;
			if (node == null)
			{
				text3 = null;
			}
			else
			{
				XmlAttributeCollection attributes2 = node.Attributes;
				if (attributes2 == null)
				{
					text3 = null;
				}
				else
				{
					XmlAttribute xmlAttribute2 = attributes2["value"];
					text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				}
			}
			string text4 = text3;
			if (text4 == null || !float.TryParse(text4, out this._value))
			{
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\EncumbranceEffect.cs", "Deserialize", 23);
			}
			string text5;
			if (node == null)
			{
				text5 = null;
			}
			else
			{
				XmlAttributeCollection attributes3 = node.Attributes;
				if (attributes3 == null)
				{
					text5 = null;
				}
				else
				{
					XmlAttribute xmlAttribute3 = attributes3["is_on_body"];
					text5 = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
				}
			}
			string text6 = text5;
			this._isOnBody = ((text6 != null) ? text6.ToLower() : null) == "true";
		}

		// Token: 0x060033AC RID: 13228 RVA: 0x000D6456 File Offset: 0x000D4656
		public override void OnUpdate(Agent agent, bool newState)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			if (agent != null)
			{
				agent.UpdateAgentProperties();
			}
		}

		// Token: 0x060033AD RID: 13229 RVA: 0x000D6477 File Offset: 0x000D4677
		public override float GetEncumbrance(bool isOnBody)
		{
			if (isOnBody != this._isOnBody)
			{
				return 0f;
			}
			return this._value;
		}

		// Token: 0x040015F6 RID: 5622
		protected static string StringType = "Encumbrance";

		// Token: 0x040015F7 RID: 5623
		private bool _isOnBody;

		// Token: 0x040015F8 RID: 5624
		private float _value;
	}
}
