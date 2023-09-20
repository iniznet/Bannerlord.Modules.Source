using System;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x020000F7 RID: 247
	public readonly struct ClanCardSelectionItemPropertyInfo
	{
		// Token: 0x06001752 RID: 5970 RVA: 0x000563F4 File Offset: 0x000545F4
		public ClanCardSelectionItemPropertyInfo(TextObject title, TextObject value)
		{
			this.Title = title;
			this.Value = value;
		}

		// Token: 0x06001753 RID: 5971 RVA: 0x00056414 File Offset: 0x00054614
		public ClanCardSelectionItemPropertyInfo(TextObject value)
		{
			this.Title = null;
			this.Value = value;
		}

		// Token: 0x06001754 RID: 5972 RVA: 0x00056431 File Offset: 0x00054631
		public static TextObject CreateLabeledValueText(TextObject label, TextObject value)
		{
			TextObject textObject = new TextObject("{=!}<span style=\"Label\">{LABEL}</span>: {VALUE}", null);
			textObject.SetTextVariable("LABEL", label);
			textObject.SetTextVariable("VALUE", value);
			return textObject;
		}

		// Token: 0x06001755 RID: 5973 RVA: 0x00056458 File Offset: 0x00054658
		public static TextObject CreateActionGoldChangeText(int goldChange)
		{
			if (goldChange != 0)
			{
				bool flag = goldChange > 0;
				string text = (flag ? "PositiveChange" : "NegativeChange");
				TextObject textObject = (flag ? new TextObject("{=8N1EdPB3}You will earn {GOLD}{GOLD_ICON}", null) : new TextObject("{=kjaACKUq}This action will cost {GOLD}{GOLD_ICON}", null));
				textObject.SetTextVariable("GOLD", string.Format("<span style=\"{0}\">{1}</span>", text, Math.Abs(goldChange)));
				textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
				return textObject;
			}
			return TextObject.Empty;
		}

		// Token: 0x04000AEF RID: 2799
		public readonly TextObject Title;

		// Token: 0x04000AF0 RID: 2800
		public readonly TextObject Value;
	}
}
