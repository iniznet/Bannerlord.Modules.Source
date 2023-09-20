using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200003A RID: 58
	public class TextInquiryData
	{
		// Token: 0x060001D4 RID: 468 RVA: 0x0000692C File Offset: 0x00004B2C
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

		// Token: 0x040000AF RID: 175
		public string TitleText;

		// Token: 0x040000B0 RID: 176
		public string Text = "";

		// Token: 0x040000B1 RID: 177
		public readonly bool IsAffirmativeOptionShown;

		// Token: 0x040000B2 RID: 178
		public readonly bool IsNegativeOptionShown;

		// Token: 0x040000B3 RID: 179
		public readonly bool IsInputObfuscated;

		// Token: 0x040000B4 RID: 180
		public readonly string AffirmativeText;

		// Token: 0x040000B5 RID: 181
		public readonly string NegativeText;

		// Token: 0x040000B6 RID: 182
		public readonly string SoundEventPath;

		// Token: 0x040000B7 RID: 183
		public readonly string DefaultInputText;

		// Token: 0x040000B8 RID: 184
		public readonly Action<string> AffirmativeAction;

		// Token: 0x040000B9 RID: 185
		public readonly Action NegativeAction;

		// Token: 0x040000BA RID: 186
		public readonly Func<string, Tuple<bool, string>> TextCondition;
	}
}
