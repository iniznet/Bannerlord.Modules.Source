using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	public class WeaponAttributeVM : ViewModel
	{
		public DamageTypes DamageType { get; }

		public CraftingTemplate.CraftingStatTypes AttributeType { get; }

		public float AttributeValue { get; }

		public WeaponAttributeVM(CraftingTemplate.CraftingStatTypes type, DamageTypes damageType, string attributeName, float attributeValue)
		{
			this.AttributeType = type;
			this.DamageType = damageType;
			this.AttributeValue = attributeValue;
			string text = ((this.AttributeValue > 100f) ? attributeValue.ToString("F0") : attributeValue.ToString("F1"));
			string text2 = "<span style=\"Value\">" + text + "</span>";
			TextObject textObject = new TextObject("{=!}{ATTR_NAME}{ATTR_VALUE_RTT}", null);
			textObject.SetTextVariable("ATTR_NAME", attributeName);
			textObject.SetTextVariable("ATTR_VALUE_RTT", text2);
			this.AttributeFieldText = textObject.ToString();
		}

		[DataSourceProperty]
		public string AttributeFieldText
		{
			get
			{
				return this._attributeFieldText;
			}
			set
			{
				if (value != this._attributeFieldText)
				{
					this._attributeFieldText = value;
					base.OnPropertyChangedWithValue<string>(value, "AttributeFieldText");
				}
			}
		}

		private string _attributeFieldText;
	}
}
