using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TaleWorlds.Library
{
	public class ProfanityChecker
	{
		public ProfanityChecker(string[] profanityList, string[] allowList)
		{
			this.ProfanityList = profanityList;
			this.AllowList = allowList;
			for (int i = 0; i < this.ProfanityList.Length; i++)
			{
				this.ProfanityList[i] = this.ProfanityList[i].ToLower();
			}
			for (int j = 0; j < this.AllowList.Length; j++)
			{
				this.AllowList[j] = this.AllowList[j].ToLower();
			}
		}

		public bool IsProfane(string word)
		{
			if (string.IsNullOrEmpty(word) || word.Length == 0)
			{
				return false;
			}
			word = word.ToLower();
			return !this.AllowList.Contains(word) && this.ProfanityList.Contains(word);
		}

		public bool ContainsProfanity(string text, ProfanityChecker.ProfanityChechkerType checkType)
		{
			if (string.IsNullOrEmpty(text) || text.Length == 0)
			{
				return false;
			}
			List<string> list = new List<string>();
			foreach (string text2 in this.ProfanityList)
			{
				if (text.Length >= text2.Length)
				{
					list.Add(text2);
				}
			}
			if (list.Count == 0)
			{
				return false;
			}
			text = text.ToLower();
			if (checkType == ProfanityChecker.ProfanityChechkerType.FalsePositive)
			{
				using (IEnumerator enumerator = new Regex(string.Format("(?:{0})", string.Join("|", list).Replace("$", "\\$"), RegexOptions.IgnoreCase)).Matches(text).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						if (!this.AllowList.Contains(obj))
						{
							return true;
						}
					}
					return false;
				}
			}
			if (checkType == ProfanityChecker.ProfanityChechkerType.FalseNegative)
			{
				foreach (object obj2 in new Regex("\\w(?<!\\d)[\\w'-]*", RegexOptions.IgnoreCase).Matches(text))
				{
					string text3 = obj2.ToString();
					if (this.ProfanityList.Contains(text3) && !this.AllowList.Contains(text3))
					{
						return true;
					}
				}
			}
			return false;
		}

		public string CensorText(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = text.ToLower();
				StringBuilder stringBuilder = new StringBuilder(text);
				string[] array = text.Split(new char[] { ' ' });
				for (int i = 0; i < array.Length; i++)
				{
					string text3 = array[i].ToLower();
					foreach (string text4 in this.ProfanityList)
					{
						string text5 = text3;
						while (text3.Contains(text4) && !this.AllowList.Contains(text3))
						{
							string text6 = stringBuilder.ToString().ToLower();
							int num = text6.IndexOf(text4, StringComparison.Ordinal);
							if (num < 0)
							{
								num = text2.IndexOf(text4, StringComparison.Ordinal);
								text6.Substring(num, text4.Length);
							}
							int num2 = text3.IndexOf(text4, StringComparison.Ordinal);
							text3 = text3.Remove(num2, text4.Length);
							for (int k = num; k < num + text4.Length; k++)
							{
								stringBuilder[k] = '*';
							}
						}
						text3 = text5;
					}
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		private readonly string[] ProfanityList;

		private readonly string[] AllowList;

		public enum ProfanityChechkerType
		{
			FalsePositive,
			FalseNegative
		}
	}
}
