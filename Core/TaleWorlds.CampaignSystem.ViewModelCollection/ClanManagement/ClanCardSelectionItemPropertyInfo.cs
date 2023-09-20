using System;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	public readonly struct ClanCardSelectionItemPropertyInfo
	{
		public ClanCardSelectionItemPropertyInfo(TextObject title, TextObject value)
		{
			this.Title = title;
			this.Value = value;
		}

		public ClanCardSelectionItemPropertyInfo(TextObject value)
		{
			this.Title = null;
			this.Value = value;
		}

		public static TextObject CreateLabeledValueText(TextObject label, TextObject value)
		{
			TextObject textObject = new TextObject("{=!}<span style=\"Label\">{LABEL}</span>: {VALUE}", null);
			textObject.SetTextVariable("LABEL", label);
			textObject.SetTextVariable("VALUE", value);
			return textObject;
		}

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

		public readonly TextObject Title;

		public readonly TextObject Value;
	}
}
