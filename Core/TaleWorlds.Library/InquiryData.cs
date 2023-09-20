using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000039 RID: 57
	public class InquiryData
	{
		// Token: 0x060001D0 RID: 464 RVA: 0x00006898 File Offset: 0x00004A98
		public InquiryData(string titleText, string text, bool isAffirmativeOptionShown, bool isNegativeOptionShown, string affirmativeText, string negativeText, Action affirmativeAction, Action negativeAction, string soundEventPath = "", float expireTime = 0f, Action timeoutAction = null, Func<ValueTuple<bool, string>> isAffirmativeOptionEnabled = null, Func<ValueTuple<bool, string>> isNegativeOptionEnabled = null)
		{
			this.TitleText = titleText;
			this.Text = text;
			this.IsAffirmativeOptionShown = isAffirmativeOptionShown;
			this.IsNegativeOptionShown = isNegativeOptionShown;
			this.GetIsAffirmativeOptionEnabled = isAffirmativeOptionEnabled;
			this.GetIsNegativeOptionEnabled = isNegativeOptionEnabled;
			this.AffirmativeText = affirmativeText;
			this.NegativeText = negativeText;
			this.AffirmativeAction = affirmativeAction;
			this.NegativeAction = negativeAction;
			this.SoundEventPath = soundEventPath;
			this.ExpireTime = expireTime;
			this.TimeoutAction = timeoutAction;
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00006910 File Offset: 0x00004B10
		public void SetText(string text)
		{
			this.Text = text;
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00006919 File Offset: 0x00004B19
		public void SetTitleText(string titleText)
		{
			this.TitleText = titleText;
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00006922 File Offset: 0x00004B22
		public void SetAffirmativeAction(Action newAffirmativeAction)
		{
			this.AffirmativeAction = newAffirmativeAction;
		}

		// Token: 0x040000A2 RID: 162
		public string TitleText;

		// Token: 0x040000A3 RID: 163
		public string Text;

		// Token: 0x040000A4 RID: 164
		public readonly float ExpireTime;

		// Token: 0x040000A5 RID: 165
		public readonly bool IsAffirmativeOptionShown;

		// Token: 0x040000A6 RID: 166
		public readonly bool IsNegativeOptionShown;

		// Token: 0x040000A7 RID: 167
		public readonly string AffirmativeText;

		// Token: 0x040000A8 RID: 168
		public readonly string NegativeText;

		// Token: 0x040000A9 RID: 169
		public readonly string SoundEventPath;

		// Token: 0x040000AA RID: 170
		public Action AffirmativeAction;

		// Token: 0x040000AB RID: 171
		public readonly Action NegativeAction;

		// Token: 0x040000AC RID: 172
		public readonly Action TimeoutAction;

		// Token: 0x040000AD RID: 173
		public readonly Func<ValueTuple<bool, string>> GetIsAffirmativeOptionEnabled;

		// Token: 0x040000AE RID: 174
		public readonly Func<ValueTuple<bool, string>> GetIsNegativeOptionEnabled;
	}
}
