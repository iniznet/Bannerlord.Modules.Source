using System;
using System.Text.RegularExpressions;

namespace TaleWorlds.Localization.TextProcessor
{
	// Token: 0x0200002A RID: 42
	internal class TokenDefinition
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000111 RID: 273 RVA: 0x00005DDE File Offset: 0x00003FDE
		// (set) Token: 0x06000112 RID: 274 RVA: 0x00005DE6 File Offset: 0x00003FE6
		public TokenType TokenType { get; private set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000113 RID: 275 RVA: 0x00005DEF File Offset: 0x00003FEF
		// (set) Token: 0x06000114 RID: 276 RVA: 0x00005DF7 File Offset: 0x00003FF7
		public int Precedence { get; private set; }

		// Token: 0x06000115 RID: 277 RVA: 0x00005E00 File Offset: 0x00004000
		public TokenDefinition(TokenType tokenType, string regexPattern, int precedence)
		{
			this._regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			this.TokenType = tokenType;
			this.Precedence = precedence;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00005E24 File Offset: 0x00004024
		internal Match CheckMatch(string str, int beginIndex)
		{
			beginIndex = this.SkipWhiteSpace(str, beginIndex);
			Match match = this._regex.Match(str, beginIndex);
			if (match.Success && match.Index == beginIndex)
			{
				return match;
			}
			return null;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00005E60 File Offset: 0x00004060
		private int SkipWhiteSpace(string str, int beginIndex)
		{
			int num = beginIndex;
			int length = str.Length;
			while (num < length && char.IsWhiteSpace(str[num]))
			{
				num++;
			}
			return num;
		}

		// Token: 0x0400005D RID: 93
		private readonly Regex _regex;
	}
}
