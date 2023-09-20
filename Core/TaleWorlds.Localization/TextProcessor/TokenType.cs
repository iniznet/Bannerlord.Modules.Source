using System;

namespace TaleWorlds.Localization.TextProcessor
{
	// Token: 0x0200002F RID: 47
	internal enum TokenType
	{
		// Token: 0x04000066 RID: 102
		NotDefined,
		// Token: 0x04000067 RID: 103
		And,
		// Token: 0x04000068 RID: 104
		Or,
		// Token: 0x04000069 RID: 105
		Not,
		// Token: 0x0400006A RID: 106
		Equals,
		// Token: 0x0400006B RID: 107
		NotEquals,
		// Token: 0x0400006C RID: 108
		GreaterThan,
		// Token: 0x0400006D RID: 109
		LessThan,
		// Token: 0x0400006E RID: 110
		GreaterOrEqual,
		// Token: 0x0400006F RID: 111
		LessOrEqual,
		// Token: 0x04000070 RID: 112
		Comma,
		// Token: 0x04000071 RID: 113
		OpenBraces,
		// Token: 0x04000072 RID: 114
		CloseBraces,
		// Token: 0x04000073 RID: 115
		OpenParenthesis,
		// Token: 0x04000074 RID: 116
		CloseParenthesis,
		// Token: 0x04000075 RID: 117
		OpenBrackets,
		// Token: 0x04000076 RID: 118
		CloseBrackets,
		// Token: 0x04000077 RID: 119
		Number,
		// Token: 0x04000078 RID: 120
		Identifier,
		// Token: 0x04000079 RID: 121
		VariableExpression,
		// Token: 0x0400007A RID: 122
		MarkerOccuranceIdentifier,
		// Token: 0x0400007B RID: 123
		Match,
		// Token: 0x0400007C RID: 124
		ConditionSeperator,
		// Token: 0x0400007D RID: 125
		ConditionFollowUp,
		// Token: 0x0400007E RID: 126
		Seperator,
		// Token: 0x0400007F RID: 127
		ConditionStarter,
		// Token: 0x04000080 RID: 128
		ConditionFinalizer,
		// Token: 0x04000081 RID: 129
		SelectionSeperator,
		// Token: 0x04000082 RID: 130
		SelectionStarter,
		// Token: 0x04000083 RID: 131
		SelectionFinalizer,
		// Token: 0x04000084 RID: 132
		FieldStarter,
		// Token: 0x04000085 RID: 133
		FieldFinalizer,
		// Token: 0x04000086 RID: 134
		SequenceTerminator,
		// Token: 0x04000087 RID: 135
		Text,
		// Token: 0x04000088 RID: 136
		LanguageMarker,
		// Token: 0x04000089 RID: 137
		UnrecognizedTokenError,
		// Token: 0x0400008A RID: 138
		Plus,
		// Token: 0x0400008B RID: 139
		Minus,
		// Token: 0x0400008C RID: 140
		Multiply,
		// Token: 0x0400008D RID: 141
		Divide,
		// Token: 0x0400008E RID: 142
		ArithmeticProduct,
		// Token: 0x0400008F RID: 143
		ArithmeticSum,
		// Token: 0x04000090 RID: 144
		StringExpression,
		// Token: 0x04000091 RID: 145
		SimpleExpression,
		// Token: 0x04000092 RID: 146
		ConditionalExpression,
		// Token: 0x04000093 RID: 147
		SelectionExpression,
		// Token: 0x04000094 RID: 148
		ParenthesisExpression,
		// Token: 0x04000095 RID: 149
		ArrayAccess,
		// Token: 0x04000096 RID: 150
		MultiStatement,
		// Token: 0x04000097 RID: 151
		ComparisonExpression,
		// Token: 0x04000098 RID: 152
		FieldExpression,
		// Token: 0x04000099 RID: 153
		MarkerOccuranceExpression,
		// Token: 0x0400009A RID: 154
		FunctionIdentifier,
		// Token: 0x0400009B RID: 155
		FunctionCall,
		// Token: 0x0400009C RID: 156
		FunctionParam,
		// Token: 0x0400009D RID: 157
		ParameterWithMarkerOccurance,
		// Token: 0x0400009E RID: 158
		ParameterWithMultipleMarkerOccurances,
		// Token: 0x0400009F RID: 159
		QualifiedIdentifier,
		// Token: 0x040000A0 RID: 160
		ParameterWithAttribute,
		// Token: 0x040000A1 RID: 161
		StartsWith,
		// Token: 0x040000A2 RID: 162
		textId
	}
}
