using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization.Expressions;

namespace TaleWorlds.Localization.TextProcessor
{
	internal class MBTextParser
	{
		internal TextExpression LookAheadFirst
		{
			get
			{
				return this._lookaheadFirst;
			}
		}

		internal TextExpression LookAheadSecond
		{
			get
			{
				return this._lookaheadSecond;
			}
		}

		internal TextExpression LookAheadThird
		{
			get
			{
				return this._lookaheadThird;
			}
		}

		private TextExpression GetSimpleToken(TokenType tokenType, string strValue)
		{
			if (tokenType == TokenType.Text)
			{
				return new SimpleText(strValue);
			}
			if (tokenType == TokenType.Number)
			{
				return new SimpleNumberExpression(strValue);
			}
			if (tokenType == TokenType.Identifier)
			{
				return new VariableExpression(strValue, null);
			}
			if (tokenType == TokenType.LanguageMarker)
			{
				return new LangaugeMarkerExpression(strValue);
			}
			if (tokenType == TokenType.textId)
			{
				return new TextIdExpression(strValue);
			}
			if (tokenType == TokenType.QualifiedIdentifier)
			{
				return new QualifiedIdentifierExpression(strValue);
			}
			if (tokenType == TokenType.ParameterWithAttribute)
			{
				return new ParameterWithAttributeExpression(strValue);
			}
			if (tokenType == TokenType.StartsWith)
			{
				return new StartsWithExpression(strValue);
			}
			return new SimpleToken(tokenType, strValue);
		}

		private void LoadSequenceStack(List<MBTextToken> tokens)
		{
			this._symbolSequence = new Stack<TextExpression>();
			for (int i = tokens.Count - 1; i >= 0; i--)
			{
				TextExpression simpleToken = this.GetSimpleToken(tokens[i].TokenType, tokens[i].Value);
				this._symbolSequence.Push(simpleToken);
			}
		}

		private void PushToken(TextExpression token)
		{
			this._symbolSequence.Push(token);
			this.UpdateLookAheads();
		}

		private void UpdateLookAheads()
		{
			if (this._symbolSequence.Count == 0)
			{
				this._lookaheadFirst = SimpleToken.SequenceTerminator;
			}
			else
			{
				this._lookaheadFirst = this._symbolSequence.Peek();
			}
			if (this._symbolSequence.Count < 2)
			{
				this._lookaheadSecond = SimpleToken.SequenceTerminator;
			}
			else
			{
				TextExpression textExpression = this._symbolSequence.Pop();
				this._lookaheadSecond = this._symbolSequence.Peek();
				this._symbolSequence.Push(textExpression);
			}
			if (this._symbolSequence.Count < 3)
			{
				this._lookaheadThird = SimpleToken.SequenceTerminator;
				return;
			}
			TextExpression textExpression2 = this._symbolSequence.Pop();
			TextExpression textExpression3 = this._symbolSequence.Pop();
			this._lookaheadThird = this._symbolSequence.Peek();
			this._symbolSequence.Push(textExpression3);
			this._symbolSequence.Push(textExpression2);
		}

		private void DiscardToken()
		{
			if (this._symbolSequence.Count > 0)
			{
				this._symbolSequence.Pop();
			}
			this.UpdateLookAheads();
		}

		private void DiscardToken(TokenType tokenType)
		{
			if (this._lookaheadFirst.TokenType != tokenType)
			{
				MBTextManager.ThrowLocalizationError(string.Format("Unxpected token: {1} while expecting: {0}", tokenType.ToString().ToUpper(), this._lookaheadFirst.RawValue));
			}
			this.DiscardToken();
		}

		private void Statements()
		{
			TextExpression rootExpressions = this.GetRootExpressions();
			this._queryModel.AddRootExpression(rootExpressions);
		}

		private bool IsRootExpression(TokenType tokenType)
		{
			return tokenType == TokenType.Text || tokenType == TokenType.SimpleExpression || tokenType == TokenType.ConditionalExpression || tokenType == TokenType.textId || tokenType == TokenType.SelectionExpression || tokenType == TokenType.MultiStatement || tokenType == TokenType.FieldExpression || tokenType == TokenType.LanguageMarker;
		}

		private void GetRootExpressionsImp(List<TextExpression> expList)
		{
			for (;;)
			{
				if (!this.RunRootGrammarRulesExceptCollapse())
				{
					if (!this.IsRootExpression(this.LookAheadFirst.TokenType))
					{
						break;
					}
					TextExpression lookAheadFirst = this.LookAheadFirst;
					this.DiscardToken();
					expList.Add(lookAheadFirst);
				}
			}
		}

		private TextExpression GetRootExpressions()
		{
			List<TextExpression> list = new List<TextExpression>();
			this.GetRootExpressionsImp(list);
			if (list.Count == 0)
			{
				return null;
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			return new MultiStatement(list);
		}

		private bool RunRootGrammarRulesExceptCollapse()
		{
			return this.CheckSimpleStatement() || this.CheckConditionalStatement() || this.CheckSelectionStatement() || this.CheckFieldStatement();
		}

		private bool CollapseStatements()
		{
			if (!this.IsRootExpression(this.LookAheadFirst.TokenType) || this.LookAheadFirst.TokenType == TokenType.MultiStatement)
			{
				return false;
			}
			List<TextExpression> list = new List<TextExpression>();
			TextExpression lookAheadFirst = this.LookAheadFirst;
			this.DiscardToken();
			list.Add(lookAheadFirst);
			bool flag = false;
			while (!flag)
			{
				while (this.RunRootGrammarRulesExceptCollapse())
				{
				}
				if (this.IsRootExpression(this.LookAheadFirst.TokenType))
				{
					TextExpression lookAheadFirst2 = this.LookAheadFirst;
					this.DiscardToken();
					list.Add(lookAheadFirst2);
				}
				else
				{
					flag = true;
				}
			}
			this.PushToken(new MultiStatement(list));
			return true;
		}

		private bool CheckSimpleStatement()
		{
			if (this.LookAheadFirst.TokenType != TokenType.OpenBraces)
			{
				return false;
			}
			this.DiscardToken(TokenType.OpenBraces);
			bool flag = false;
			while (!flag)
			{
				flag = !this.DoExpressionRules();
			}
			TokenType tokenType = this.LookAheadFirst.TokenType;
			if (this.IsArithmeticExpression(tokenType))
			{
				TextExpression textExpression = new SimpleExpression(this.LookAheadFirst);
				this.DiscardToken();
				this.DiscardToken(TokenType.CloseBraces);
				this.PushToken(textExpression);
			}
			else
			{
				this.DiscardToken(TokenType.CloseBraces);
			}
			return true;
		}

		private bool CheckFieldStatement()
		{
			if (this.LookAheadFirst.TokenType != TokenType.FieldStarter)
			{
				return false;
			}
			this.DiscardToken(TokenType.FieldStarter);
			bool flag = false;
			while (!flag)
			{
				flag = !this.DoExpressionRules();
			}
			if (this.LookAheadFirst.TokenType != TokenType.Identifier)
			{
				Debug.FailedAssert("Can not parse the text: " + this.LookAheadFirst, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Localization\\TextProcessor\\MbTextParser.cs", "CheckFieldStatement", 289);
				return false;
			}
			TextExpression lookAheadFirst = this.LookAheadFirst;
			this.DiscardToken(TokenType.Identifier);
			this.DiscardToken(TokenType.CloseBraces);
			TextExpression textExpression = this.GetRootExpressions();
			if (textExpression == null)
			{
				textExpression = new SimpleToken(TokenType.Text, "");
			}
			this.DiscardToken(TokenType.FieldFinalizer);
			FieldExpression fieldExpression = new FieldExpression(lookAheadFirst, textExpression);
			this.PushToken(fieldExpression);
			return true;
		}

		private bool CheckConditionalStatement()
		{
			if (this.LookAheadFirst.TokenType != TokenType.ConditionStarter)
			{
				return false;
			}
			bool flag = false;
			List<TextExpression> list = new List<TextExpression>();
			List<TextExpression> list2 = new List<TextExpression>();
			while (!flag)
			{
				TokenType tokenType = this.LookAheadFirst.TokenType;
				if (this.LookAheadFirst.TokenType == TokenType.ConditionStarter || this.LookAheadFirst.TokenType == TokenType.ConditionFollowUp)
				{
					this.DiscardToken();
					while (this.DoExpressionRules())
					{
					}
					tokenType = this.LookAheadFirst.TokenType;
					if (!this.IsArithmeticExpression(tokenType))
					{
						Debug.FailedAssert("Can not parse the text: " + this.LookAheadFirst, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Localization\\TextProcessor\\MbTextParser.cs", "CheckConditionalStatement", 336);
						return false;
					}
					list.Add(this.LookAheadFirst);
					this.DiscardToken();
					this.DiscardToken(TokenType.CloseBraces);
				}
				else
				{
					if (tokenType != TokenType.ConditionSeperator && tokenType != TokenType.Seperator)
					{
						MBTextManager.ThrowLocalizationError("Can not parse the text: " + this.LookAheadFirst);
						return false;
					}
					this.DiscardToken();
					flag = true;
				}
				TextExpression textExpression = this.GetRootExpressions();
				if (textExpression == null)
				{
					textExpression = new SimpleToken(TokenType.Text, "");
				}
				list2.Add(textExpression);
			}
			while (!flag)
			{
			}
			this.DiscardToken(TokenType.ConditionFinalizer);
			ConditionExpression conditionExpression = new ConditionExpression(list, list2);
			this.PushToken(conditionExpression);
			return true;
		}

		private bool CheckSelectionStatement()
		{
			if (this.LookAheadFirst.TokenType != TokenType.SelectionStarter)
			{
				return false;
			}
			this.DiscardToken(TokenType.SelectionStarter);
			while (this.DoExpressionRules())
			{
			}
			TokenType tokenType = this.LookAheadFirst.TokenType;
			if (!this.IsArithmeticExpression(tokenType))
			{
				Debug.FailedAssert("Can not parse the text: " + this.LookAheadFirst, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Localization\\TextProcessor\\MbTextParser.cs", "CheckSelectionStatement", 382);
				return false;
			}
			TextExpression lookAheadFirst = this.LookAheadFirst;
			this.DiscardToken();
			this.DiscardToken(TokenType.CloseBraces);
			bool flag = false;
			List<TextExpression> list = new List<TextExpression>();
			for (;;)
			{
				TextExpression textExpression = this.GetRootExpressions();
				if (textExpression == null)
				{
					textExpression = new SimpleToken(TokenType.Text, "");
				}
				list.Add(textExpression);
				TokenType tokenType2 = this.LookAheadFirst.TokenType;
				if (tokenType2 == TokenType.SelectionSeperator)
				{
					this.DiscardToken();
				}
				else
				{
					if (tokenType2 != TokenType.SelectionFinalizer)
					{
						break;
					}
					flag = true;
					this.DiscardToken();
				}
				if (flag)
				{
					goto Block_7;
				}
			}
			Debug.FailedAssert("Can not parse the text: " + this.LookAheadFirst, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Localization\\TextProcessor\\MbTextParser.cs", "CheckSelectionStatement", 414);
			return false;
			Block_7:
			SelectionExpression selectionExpression = new SelectionExpression(lookAheadFirst, list);
			this.PushToken(selectionExpression);
			return true;
		}

		private bool DoExpressionRules()
		{
			return this.ConsumeArrayAccessExpression() || this.ConsumeFunction() || this.ConsumeMarkerOccuranceExpression() || this.ConsumeNegativeAritmeticExpression() || this.ConsumeParenthesisExpression() || this.ConsumeInnerAritmeticExpression() || this.ConsumeOuterAritmeticExpression() || this.ConsumeComparisonExpression();
		}

		private bool ConsumeFunction()
		{
			if (this.LookAheadFirst.TokenType != TokenType.FunctionIdentifier)
			{
				return false;
			}
			string text = this.LookAheadFirst.RawValue.Substring(0, this.LookAheadFirst.RawValue.Length - 1);
			this.DiscardToken();
			bool flag = false;
			List<TextExpression> list = new List<TextExpression>();
			while (this.LookAheadFirst.TokenType != TokenType.CloseParenthesis && !flag)
			{
				if (list.Count > 0)
				{
					this.DiscardToken(TokenType.Comma);
				}
				while (this.DoExpressionRules())
				{
				}
				TokenType tokenType = this.LookAheadFirst.TokenType;
				if (!this.IsArithmeticExpression(tokenType))
				{
					Debug.FailedAssert("Can not parse the text: " + this.LookAheadFirst, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Localization\\TextProcessor\\MbTextParser.cs", "ConsumeFunction", 472);
					return false;
				}
				list.Add(this.LookAheadFirst);
				this.DiscardToken();
			}
			this.DiscardToken(TokenType.CloseParenthesis);
			FunctionCall functionCall = new FunctionCall(text, list);
			this.PushToken(functionCall);
			return true;
		}

		private bool ConsumeMarkerOccuranceExpression()
		{
			if (this.LookAheadFirst.TokenType == TokenType.Identifier && this.LookAheadSecond.TokenType == TokenType.MarkerOccuranceIdentifier)
			{
				VariableExpression variableExpression = this.LookAheadFirst as VariableExpression;
				TextExpression lookAheadSecond = this.LookAheadSecond;
				this.DiscardToken();
				this.DiscardToken();
				MarkerOccuranceTextExpression markerOccuranceTextExpression = new MarkerOccuranceTextExpression(lookAheadSecond.RawValue.Substring(2), variableExpression);
				this.PushToken(markerOccuranceTextExpression);
				return true;
			}
			return false;
		}

		private bool ConsumeArrayAccessExpression()
		{
			if (this.LookAheadFirst.TokenType == TokenType.Identifier && this.LookAheadSecond.TokenType == TokenType.OpenBrackets)
			{
				TextExpression lookAheadFirst = this.LookAheadFirst;
				this.DiscardToken();
				this.DiscardToken(TokenType.OpenBrackets);
				while (this.DoExpressionRules())
				{
				}
				TokenType tokenType = this.LookAheadFirst.TokenType;
				if (this.IsArithmeticExpression(tokenType))
				{
					TextExpression lookAheadFirst2 = this.LookAheadFirst;
					this.DiscardToken();
					this.DiscardToken(TokenType.CloseBrackets);
					ArrayReference arrayReference = new ArrayReference(lookAheadFirst.RawValue, lookAheadFirst2);
					this.PushToken(arrayReference);
					return true;
				}
			}
			return false;
		}

		private bool ConsumeNegativeAritmeticExpression()
		{
			if (this.LookAheadFirst.TokenType == TokenType.Minus)
			{
				this.ConsumeAritmeticOperation();
				TokenType tokenType = this.LookAheadFirst.TokenType;
				if (this.IsArithmeticExpression(tokenType))
				{
					ArithmeticExpression arithmeticExpression = new ArithmeticExpression(ArithmeticOperation.Subtract, new SimpleToken(TokenType.Number, "0"), this.LookAheadFirst);
					this.PushToken(arithmeticExpression);
					return true;
				}
			}
			return false;
		}

		private bool ConsumeParenthesisExpression()
		{
			if (this.LookAheadFirst.TokenType != TokenType.OpenParenthesis)
			{
				return false;
			}
			this.DiscardToken(TokenType.OpenParenthesis);
			while (this.DoExpressionRules())
			{
			}
			TokenType tokenType = this.LookAheadFirst.TokenType;
			if (this.IsArithmeticExpression(tokenType))
			{
				ParanthesisExpression paranthesisExpression = new ParanthesisExpression(this.LookAheadFirst);
				this.DiscardToken();
				this.DiscardToken(TokenType.CloseParenthesis);
				this.PushToken(paranthesisExpression);
				return true;
			}
			this.DiscardToken(TokenType.CloseParenthesis);
			return true;
		}

		private bool IsArithmeticExpression(TokenType t)
		{
			return t == TokenType.ArithmeticProduct || t == TokenType.ArithmeticSum || t == TokenType.Identifier || t == TokenType.QualifiedIdentifier || t == TokenType.MarkerOccuranceExpression || t == TokenType.ParameterWithMarkerOccurance || t == TokenType.ParameterWithMultipleMarkerOccurances || t == TokenType.StartsWith || t == TokenType.Number || t == TokenType.ParenthesisExpression || t == TokenType.ComparisonExpression || t == TokenType.FunctionCall || t == TokenType.FunctionParam || t == TokenType.ArrayAccess || t == TokenType.ParameterWithAttribute;
		}

		private bool ConsumeInnerAritmeticExpression()
		{
			TokenType tokenType = this.LookAheadFirst.TokenType;
			TokenType tokenType2 = this.LookAheadSecond.TokenType;
			TokenType tokenType3 = this.LookAheadThird.TokenType;
			if (this.IsArithmeticExpression(tokenType) && (tokenType2 == TokenType.Multiply || tokenType2 == TokenType.Divide))
			{
				TextExpression lookAheadFirst = this.LookAheadFirst;
				this.DiscardToken();
				ArithmeticOperation arithmeticOperation = this.ConsumeAritmeticOperation();
				if (!this.IsArithmeticExpression(this.LookAheadFirst.TokenType))
				{
					while (this.DoExpressionRules())
					{
					}
				}
				TextExpression lookAheadFirst2 = this.LookAheadFirst;
				this.DiscardToken();
				ArithmeticExpression arithmeticExpression = new ArithmeticExpression(arithmeticOperation, lookAheadFirst, lookAheadFirst2);
				this.PushToken(arithmeticExpression);
				return true;
			}
			return false;
		}

		private bool ConsumeOuterAritmeticExpression()
		{
			TokenType tokenType = this.LookAheadFirst.TokenType;
			TokenType tokenType2 = this.LookAheadSecond.TokenType;
			if (this.IsArithmeticExpression(tokenType) && (tokenType2 == TokenType.Plus || tokenType2 == TokenType.Minus))
			{
				TextExpression lookAheadFirst = this.LookAheadFirst;
				this.DiscardToken();
				ArithmeticOperation arithmeticOperation = this.ConsumeAritmeticOperation();
				while (this.DoExpressionRules())
				{
				}
				if (this.IsArithmeticExpression(this.LookAheadFirst.TokenType))
				{
					TextExpression lookAheadFirst2 = this.LookAheadFirst;
					this.DiscardToken();
					ArithmeticExpression arithmeticExpression = new ArithmeticExpression(arithmeticOperation, lookAheadFirst, lookAheadFirst2);
					this.PushToken(arithmeticExpression);
					return true;
				}
				Debug.FailedAssert("Can not parse the text: " + this.LookAheadFirst, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Localization\\TextProcessor\\MbTextParser.cs", "ConsumeOuterAritmeticExpression", 646);
			}
			return false;
		}

		private ArithmeticOperation ConsumeAritmeticOperation()
		{
			ArithmeticOperation arithmeticOperation = ((this.LookAheadFirst.TokenType == TokenType.Plus) ? ArithmeticOperation.Add : ((this.LookAheadFirst.TokenType == TokenType.Minus) ? ArithmeticOperation.Subtract : ((this.LookAheadFirst.TokenType == TokenType.Multiply) ? ArithmeticOperation.Multiply : ((this.LookAheadFirst.TokenType == TokenType.Divide) ? ArithmeticOperation.Divide : ArithmeticOperation.Add))));
			this.DiscardToken();
			return arithmeticOperation;
		}

		private bool ConsumeComparisonExpression()
		{
			TokenType tokenType = this.LookAheadFirst.TokenType;
			TokenType tokenType2 = this.LookAheadSecond.TokenType;
			if (!this.IsArithmeticExpression(tokenType) || !this.IsComparisonOperator(tokenType2))
			{
				return false;
			}
			TextExpression lookAheadFirst = this.LookAheadFirst;
			this.DiscardToken();
			ComparisonOperation comparisonOp = this.GetComparisonOp(tokenType2);
			this.DiscardToken();
			while (this.DoExpressionRules())
			{
			}
			if (!this.IsArithmeticExpression(this.LookAheadFirst.TokenType))
			{
				Debug.FailedAssert("Can not parse the text: " + this.LookAheadFirst, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Localization\\TextProcessor\\MbTextParser.cs", "ConsumeComparisonExpression", 690);
				return false;
			}
			TextExpression lookAheadFirst2 = this.LookAheadFirst;
			this.DiscardToken();
			ComparisonExpression comparisonExpression = new ComparisonExpression(comparisonOp, lookAheadFirst, lookAheadFirst2);
			this.PushToken(comparisonExpression);
			return true;
		}

		private bool IsComparisonOperator(TokenType tokenType)
		{
			return tokenType == TokenType.Equals || tokenType == TokenType.NotEquals || tokenType == TokenType.GreaterThan || tokenType == TokenType.GreaterOrEqual || tokenType == TokenType.GreaterThan || tokenType == TokenType.LessOrEqual || tokenType == TokenType.LessThan;
		}

		private BooleanOperation GetBooleanOp(TokenType tokenType)
		{
			if (tokenType == TokenType.Or)
			{
				return BooleanOperation.Or;
			}
			if (tokenType == TokenType.And)
			{
				return BooleanOperation.And;
			}
			if (tokenType != TokenType.Not)
			{
				return BooleanOperation.And;
			}
			return BooleanOperation.Not;
		}

		private ComparisonOperation GetComparisonOp(TokenType tokenType)
		{
			if (tokenType == TokenType.Equals)
			{
				return ComparisonOperation.Equals;
			}
			if (tokenType == TokenType.NotEquals)
			{
				return ComparisonOperation.NotEquals;
			}
			if (tokenType == TokenType.GreaterThan)
			{
				return ComparisonOperation.GreaterThan;
			}
			if (tokenType == TokenType.GreaterOrEqual)
			{
				return ComparisonOperation.GreaterOrEqual;
			}
			if (tokenType == TokenType.GreaterThan)
			{
				return ComparisonOperation.GreaterThan;
			}
			if (tokenType == TokenType.LessOrEqual)
			{
				return ComparisonOperation.LessOrEqual;
			}
			if (tokenType != TokenType.LessThan)
			{
				return ComparisonOperation.Equals;
			}
			return ComparisonOperation.LessThan;
		}

		private MBTextModel ParseInternal(List<MBTextToken> tokens)
		{
			this.LoadSequenceStack(tokens);
			this.UpdateLookAheads();
			this._queryModel = new MBTextModel();
			this.Statements();
			this.DiscardToken(TokenType.SequenceTerminator);
			return this._queryModel;
		}

		internal static MBTextModel Parse(List<MBTextToken> tokens)
		{
			return new MBTextParser().ParseInternal(tokens);
		}

		private Stack<TextExpression> _symbolSequence;

		private TextExpression _lookaheadFirst;

		private TextExpression _lookaheadSecond;

		private TextExpression _lookaheadThird;

		private MBTextModel _queryModel;
	}
}
