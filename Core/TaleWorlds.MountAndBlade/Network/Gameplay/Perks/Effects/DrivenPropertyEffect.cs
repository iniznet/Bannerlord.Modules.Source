using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003BD RID: 957
	public class DrivenPropertyEffect : MPPerkEffect
	{
		// Token: 0x060033A1 RID: 13217 RVA: 0x000D6069 File Offset: 0x000D4269
		protected DrivenPropertyEffect()
		{
		}

		// Token: 0x060033A2 RID: 13218 RVA: 0x000D6074 File Offset: 0x000D4274
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
					XmlAttribute xmlAttribute2 = attributes2["is_ratio"];
					text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				}
			}
			string text4 = text3;
			this._isRatio = ((text4 != null) ? text4.ToLower() : null) == "true";
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
					XmlAttribute xmlAttribute3 = attributes3["driven_property"];
					text5 = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
				}
			}
			if (!Enum.TryParse<DrivenProperty>(text5, true, out this._drivenProperty))
			{
				Debug.FailedAssert("provided 'driven_property' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\DrivenPropertyEffect.cs", "Deserialize", 28);
			}
			string text6;
			if (node == null)
			{
				text6 = null;
			}
			else
			{
				XmlAttributeCollection attributes4 = node.Attributes;
				if (attributes4 == null)
				{
					text6 = null;
				}
				else
				{
					XmlAttribute xmlAttribute4 = attributes4["value"];
					text6 = ((xmlAttribute4 != null) ? xmlAttribute4.Value : null);
				}
			}
			string text7 = text6;
			if (text7 == null || !float.TryParse(text7, out this._value))
			{
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\DrivenPropertyEffect.cs", "Deserialize", 35);
			}
		}

		// Token: 0x060033A3 RID: 13219 RVA: 0x000D61AB File Offset: 0x000D43AB
		public override void OnUpdate(Agent agent, bool newState)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			if (agent != null)
			{
				agent.UpdateAgentProperties();
			}
		}

		// Token: 0x060033A4 RID: 13220 RVA: 0x000D61CC File Offset: 0x000D43CC
		public override float GetDrivenPropertyBonus(DrivenProperty drivenProperty, float baseValue)
		{
			if (drivenProperty != this._drivenProperty)
			{
				return 0f;
			}
			if (!this._isRatio)
			{
				return this._value;
			}
			return baseValue * this._value;
		}

		// Token: 0x040015EE RID: 5614
		protected static string StringType = "DrivenProperty";

		// Token: 0x040015EF RID: 5615
		private DrivenProperty _drivenProperty;

		// Token: 0x040015F0 RID: 5616
		private float _value;

		// Token: 0x040015F1 RID: 5617
		private bool _isRatio;
	}
}
