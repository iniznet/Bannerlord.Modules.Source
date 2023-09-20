using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor
{
	// Token: 0x0200002C RID: 44
	public abstract class LanguageSpecificTextProcessor
	{
		// Token: 0x06000119 RID: 281
		public abstract void ProcessToken(string sourceText, ref int cursorPos, string token, StringBuilder outputString);

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600011A RID: 282
		public abstract CultureInfo CultureInfoForLanguage { get; }

		// Token: 0x0600011B RID: 283
		public abstract void ClearTemporaryData();

		// Token: 0x0600011C RID: 284 RVA: 0x00005F20 File Offset: 0x00004120
		public LanguageSpecificTextProcessor()
		{
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00005F34 File Offset: 0x00004134
		public string Process(string text)
		{
			if (text == null)
			{
				return null;
			}
			bool flag = false;
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '{')
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return text;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int j = 0;
			while (j < text.Length)
			{
				if (text[j] != '{')
				{
					stringBuilder.Append(text[j]);
					j++;
				}
				else
				{
					string text2 = LanguageSpecificTextProcessor.ReadFirstToken(text, ref j);
					if (LanguageSpecificTextProcessor.IsPostProcessToken(text2))
					{
						this.ProcessTokenInternal(text, ref j, text2, stringBuilder);
					}
				}
			}
			this.ProcessLowerCaseMarkers(stringBuilder);
			return stringBuilder.ToString();
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00005FCC File Offset: 0x000041CC
		private void ProcessTokenInternal(string sourceText, ref int cursorPos, string token, StringBuilder outputString)
		{
			CultureInfo cultureInfoForLanguage = this.CultureInfoForLanguage;
			char c = token[1];
			if (c == '^' && token.Length == 2)
			{
				int num = LanguageSpecificTextProcessor.FindNextLetter(sourceText, cursorPos);
				if (num > cursorPos && num < sourceText.Length)
				{
					outputString.Append(sourceText.Substring(cursorPos, num - cursorPos));
				}
				if (num < sourceText.Length)
				{
					outputString.Append(char.ToUpper(sourceText[num], cultureInfoForLanguage));
					cursorPos = num + 1;
					return;
				}
			}
			else if (c == '_' && token.Length == 2)
			{
				int num2 = LanguageSpecificTextProcessor.FindNextLetter(sourceText, cursorPos);
				if (num2 > cursorPos && num2 < sourceText.Length)
				{
					outputString.Append(sourceText.Substring(cursorPos, num2 - cursorPos));
				}
				if (num2 < sourceText.Length)
				{
					outputString.Append(char.ToLower(sourceText[num2], cultureInfoForLanguage));
					cursorPos = num2 + 1;
					return;
				}
			}
			else
			{
				if (c == '%' && token.Length == 2)
				{
					this._lowerMarkers.Add(outputString.Length - 1);
					return;
				}
				this.ProcessToken(sourceText, ref cursorPos, token, outputString);
			}
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000060D4 File Offset: 0x000042D4
		private void ProcessLowerCaseMarkers(StringBuilder stringBuilder)
		{
			if (this._lowerMarkers.Count > 0)
			{
				for (int i = 0; i < this._lowerMarkers.Count; i += 2)
				{
					int num = this._lowerMarkers[i];
					if (i + 1 < this._lowerMarkers.Count)
					{
						int num2 = this._lowerMarkers[i + 1];
						if (num != num2)
						{
							if (num > stringBuilder.Length)
							{
								num = -1;
							}
							int num3 = Math.Min(num2 - num, stringBuilder.Length - num - 1);
							string text = stringBuilder.ToString(num + 1, num3);
							stringBuilder = stringBuilder.Remove(num + 1, num3).Insert(num + 1, text.ToLower());
						}
					}
					else
					{
						if (num > stringBuilder.Length)
						{
							num = -1;
						}
						if (num + 1 < stringBuilder.Length)
						{
							string text2 = stringBuilder.ToString(num + 1, stringBuilder.Length - num - 1);
							stringBuilder = stringBuilder.Remove(num + 1, stringBuilder.Length - num - 1).Insert(num + 1, text2.ToLower());
						}
					}
				}
				this._lowerMarkers.Clear();
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x000061E4 File Offset: 0x000043E4
		private static int FindNextLetter(string sourceText, int cursorPos)
		{
			int i = cursorPos;
			if (sourceText.Length > i + "<a style=\"Link.".Length && sourceText.Substring(i, "<a style=\"Link.".Length).Equals("<a style=\"Link."))
			{
				i += "<a style=\"Link.".Length;
				while (sourceText[i++] != '>')
				{
				}
			}
			while (i < sourceText.Length)
			{
				if (sourceText[i] == '<')
				{
					i += 2;
				}
				if (char.IsLetter(sourceText, i))
				{
					return i;
				}
				i++;
			}
			return i;
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000626A File Offset: 0x0000446A
		private static bool IsPostProcessToken(string token)
		{
			return token.Length > 1 && token[0] == '.';
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00006284 File Offset: 0x00004484
		private static string ReadFirstToken(string text, ref int i)
		{
			int num = i;
			while (i < text.Length && text[i] != '}')
			{
				i++;
			}
			int num2 = i - num - 1;
			if (i < text.Length)
			{
				i++;
			}
			return text.Substring(num + 1, num2);
		}

		// Token: 0x04000060 RID: 96
		private List<int> _lowerMarkers = new List<int>();
	}
}
