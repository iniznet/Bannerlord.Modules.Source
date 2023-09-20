using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Localization
{
	// Token: 0x02000006 RID: 6
	internal class LocalizedText
	{
		// Token: 0x06000042 RID: 66 RVA: 0x00002BFC File Offset: 0x00000DFC
		public LocalizedText()
		{
			this._localizedTextDictionary = new Dictionary<string, string>();
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002C0F File Offset: 0x00000E0F
		public void AddTranslation(string language, string translation)
		{
			if (!this._localizedTextDictionary.ContainsKey(language))
			{
				this._localizedTextDictionary.Add(language, translation);
				return;
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002C30 File Offset: 0x00000E30
		public string GetTranslatedText(string languageId)
		{
			string text;
			if (this._localizedTextDictionary.TryGetValue(languageId, out text))
			{
				return text;
			}
			if (this._localizedTextDictionary.TryGetValue("English", out text))
			{
				return text;
			}
			return null;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002C68 File Offset: 0x00000E68
		public bool CheckValidity(string id, out string errorLine)
		{
			errorLine = null;
			bool flag = false;
			foreach (KeyValuePair<string, string> keyValuePair in this._localizedTextDictionary)
			{
				string value = keyValuePair.Value;
				int num = 0;
				int num2 = 0;
				foreach (char c in value)
				{
					if (c == '{')
					{
						num++;
					}
					else if (c == '}')
					{
						num2++;
					}
				}
				int num3 = 0;
				int num4 = 0;
				string text2 = value;
				for (;;)
				{
					int num5 = text2.IndexOf("{?");
					if (num5 == -1)
					{
						break;
					}
					num5 = MathF.Min(num5 + 1, text2.Length - 1);
					text2 = text2.Substring(num5);
					if (text2.Length > 2 && text2[1] != '}')
					{
						num3++;
					}
				}
				string text3 = value;
				for (;;)
				{
					int num6 = text3.IndexOf("{\\?}");
					if (num6 == -1)
					{
						break;
					}
					num4++;
					num6 = MathF.Min(num6 + 1, value.Length - 1);
					text3 = text3.Substring(num6);
				}
				if (num != num2)
				{
					errorLine = string.Format("{0} | {1}\n", id, value);
					flag = true;
				}
				else if (num3 != num4)
				{
					errorLine = string.Format("{0} | {1}\n", id, value);
					flag = true;
				}
				else if (!flag)
				{
					try
					{
						MBTextManager.ProcessTextToString(new TextObject("{=" + id + "}" + LocalizedTextManager.GetTranslatedText(MBTextManager.ActiveTextLanguage, id), null), true);
					}
					catch
					{
						errorLine = string.Format("{0} | {1}\n", id, value);
					}
				}
			}
			return flag;
		}

		// Token: 0x04000010 RID: 16
		private readonly Dictionary<string, string> _localizedTextDictionary;
	}
}
