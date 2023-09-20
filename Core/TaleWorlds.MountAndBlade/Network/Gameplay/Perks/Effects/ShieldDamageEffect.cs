using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003CF RID: 975
	public class ShieldDamageEffect : MPPerkEffect
	{
		// Token: 0x060033F3 RID: 13299 RVA: 0x000D7720 File Offset: 0x000D5920
		protected ShieldDamageEffect()
		{
		}

		// Token: 0x060033F4 RID: 13300 RVA: 0x000D7728 File Offset: 0x000D5928
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\ShieldDamageEffect.cs", "Deserialize", 31);
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
					XmlAttribute xmlAttribute3 = attributes3["block_type"];
					text5 = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
				}
			}
			string text6 = text5;
			this._blockType = ShieldDamageEffect.BlockType.Any;
			if (text6 != null && !Enum.TryParse<ShieldDamageEffect.BlockType>(text6, true, out this._blockType))
			{
				this._blockType = ShieldDamageEffect.BlockType.Any;
				Debug.FailedAssert("provided 'block_type' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\ShieldDamageEffect.cs", "Deserialize", 39);
			}
		}

		// Token: 0x060033F5 RID: 13301 RVA: 0x000D782C File Offset: 0x000D5A2C
		public override float GetShieldDamage(bool isCorrectSideBlock)
		{
			switch (this._blockType)
			{
			case ShieldDamageEffect.BlockType.Any:
				return this._value;
			case ShieldDamageEffect.BlockType.CorrectSide:
				if (!isCorrectSideBlock)
				{
					return 0f;
				}
				return this._value;
			case ShieldDamageEffect.BlockType.WrongSide:
				if (!isCorrectSideBlock)
				{
					return this._value;
				}
				return 0f;
			default:
				return 0f;
			}
		}

		// Token: 0x0400161D RID: 5661
		protected static string StringType = "ShieldDamage";

		// Token: 0x0400161E RID: 5662
		private float _value;

		// Token: 0x0400161F RID: 5663
		private ShieldDamageEffect.BlockType _blockType;

		// Token: 0x020006C5 RID: 1733
		private enum BlockType
		{
			// Token: 0x040022A6 RID: 8870
			Any,
			// Token: 0x040022A7 RID: 8871
			CorrectSide,
			// Token: 0x040022A8 RID: 8872
			WrongSide
		}
	}
}
