using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003C4 RID: 964
	public class HitpointsEffect : MPOnSpawnPerkEffect
	{
		// Token: 0x060033C1 RID: 13249 RVA: 0x000D69C2 File Offset: 0x000D4BC2
		protected HitpointsEffect()
		{
		}

		// Token: 0x060033C2 RID: 13250 RVA: 0x000D69CC File Offset: 0x000D4BCC
		protected override void Deserialize(XmlNode node)
		{
			base.Deserialize(node);
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
					XmlAttribute xmlAttribute = attributes["value"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			if (text2 == null || !float.TryParse(text2, out this._value))
			{
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\HitpointsEffect.cs", "Deserialize", 20);
			}
		}

		// Token: 0x060033C3 RID: 13251 RVA: 0x000D6A31 File Offset: 0x000D4C31
		public override float GetHitpoints(bool isPlayer)
		{
			if (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Any || (isPlayer ? (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Player) : (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Troops)))
			{
				return this._value;
			}
			return 0f;
		}

		// Token: 0x04001604 RID: 5636
		protected static string StringType = "HitpointsOnSpawn";

		// Token: 0x04001605 RID: 5637
		private float _value;
	}
}
