using System;

namespace TaleWorlds.Library
{
	public class TextInquiryData
	{
		public TextInquiryData(string titleText, string text, bool isAffirmativeOptionShown, bool isNegativeOptionShown, string affirmativeText, string negativeText, Action<string> affirmativeAction, Action negativeAction, bool shouldInputBeObfuscated = false, Func<string, Tuple<bool, string>> textCondition = null, string soundEventPath = "", string defaultInputText = "")
		{
			this.TitleText = titleText;
			this.Text = text;
			this.IsAffirmativeOptionShown = isAffirmativeOptionShown;
			this.IsNegativeOptionShown = isNegativeOptionShown;
			this.AffirmativeText = affirmativeText;
			this.NegativeText = negativeText;
			this.AffirmativeAction = affirmativeAction;
			this.NegativeAction = negativeAction;
			this.TextCondition = textCondition;
			this.IsInputObfuscated = shouldInputBeObfuscated;
			this.SoundEventPath = soundEventPath;
			this.DefaultInputText = defaultInputText;
		}

		public string TitleText;

		public string Text = "";

		public readonly bool IsAffirmativeOptionShown;

		public readonly bool IsNegativeOptionShown;

		public readonly bool IsInputObfuscated;

		public readonly string AffirmativeText;

		public readonly string NegativeText;

		public readonly string SoundEventPath;

		public readonly string DefaultInputText;

		public readonly Action<string> AffirmativeAction;

		public readonly Action NegativeAction;

		public readonly Func<string, Tuple<bool, string>> TextCondition;
	}
}
