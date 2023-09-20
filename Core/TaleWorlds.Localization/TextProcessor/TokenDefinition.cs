using System;
using System.Text.RegularExpressions;

namespace TaleWorlds.Localization.TextProcessor
{
	internal class TokenDefinition
	{
		public TokenType TokenType { get; private set; }

		public int Precedence { get; private set; }

		public TokenDefinition(TokenType tokenType, string regexPattern, int precedence)
		{
			this._regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			this.TokenType = tokenType;
			this.Precedence = precedence;
		}

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

		private readonly Regex _regex;
	}
}
