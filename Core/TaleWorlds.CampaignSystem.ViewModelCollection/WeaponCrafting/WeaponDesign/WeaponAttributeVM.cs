using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000E0 RID: 224
	public class WeaponAttributeVM : ViewModel
	{
		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x060014CB RID: 5323 RVA: 0x0004E3FF File Offset: 0x0004C5FF
		public DamageTypes DamageType { get; }

		// Token: 0x17000700 RID: 1792
		// (get) Token: 0x060014CC RID: 5324 RVA: 0x0004E407 File Offset: 0x0004C607
		public CraftingTemplate.CraftingStatTypes AttributeType { get; }

		// Token: 0x17000701 RID: 1793
		// (get) Token: 0x060014CD RID: 5325 RVA: 0x0004E40F File Offset: 0x0004C60F
		public float AttributeValue { get; }

		// Token: 0x060014CE RID: 5326 RVA: 0x0004E418 File Offset: 0x0004C618
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

		// Token: 0x17000702 RID: 1794
		// (get) Token: 0x060014CF RID: 5327 RVA: 0x0004E4AC File Offset: 0x0004C6AC
		// (set) Token: 0x060014D0 RID: 5328 RVA: 0x0004E4B4 File Offset: 0x0004C6B4
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

		// Token: 0x040009B5 RID: 2485
		private string _attributeFieldText;
	}
}
