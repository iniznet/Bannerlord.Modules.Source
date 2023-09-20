using System;

namespace TaleWorlds.Library
{
	public class InquiryData
	{
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

		public void SetText(string text)
		{
			this.Text = text;
		}

		public void SetTitleText(string titleText)
		{
			this.TitleText = titleText;
		}

		public void SetAffirmativeAction(Action newAffirmativeAction)
		{
			this.AffirmativeAction = newAffirmativeAction;
		}

		public string TitleText;

		public string Text;

		public readonly float ExpireTime;

		public readonly bool IsAffirmativeOptionShown;

		public readonly bool IsNegativeOptionShown;

		public readonly string AffirmativeText;

		public readonly string NegativeText;

		public readonly string SoundEventPath;

		public Action AffirmativeAction;

		public readonly Action NegativeAction;

		public readonly Action TimeoutAction;

		public readonly Func<ValueTuple<bool, string>> GetIsAffirmativeOptionEnabled;

		public readonly Func<ValueTuple<bool, string>> GetIsNegativeOptionEnabled;
	}
}
