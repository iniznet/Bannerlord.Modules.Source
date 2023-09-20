using System;

namespace TaleWorlds.Localization.TextProcessor
{
	[Serializable]
	internal class MBTextToken
	{
		internal MBTextToken(TokenType tokenType)
		{
			this.TokenType = tokenType;
			this.Value = string.Empty;
		}

		internal MBTextToken(TokenType tokenType, string value)
		{
			this.TokenType = tokenType;
			this.Value = value;
		}

		internal TokenType TokenType { get; set; }

		public string Value { get; set; }

		public MBTextToken Clone()
		{
			return new MBTextToken(this.TokenType, this.Value);
		}
	}
}
