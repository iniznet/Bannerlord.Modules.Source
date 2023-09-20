using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TaleWorlds.Library;
using TaleWorlds.Localization.Expressions;

namespace TaleWorlds.Localization.TextProcessor
{
	public class TextProcessingContext
	{
		internal void SetTextVariable(string variableName, TextObject data)
		{
			this._variables[variableName] = data;
		}

		internal TextObject GetRawTextVariable(string variableName, TextObject parent)
		{
			TextObject textObject = null;
			if (parent != null && parent.GetVariableValue(variableName, out textObject))
			{
				return textObject;
			}
			if (!this._variables.ContainsKey(variableName))
			{
				return TextObject.Empty;
			}
			return this._variables[variableName];
		}

		internal MultiStatement GetVariableValue(string variableName, TextObject parent)
		{
			TextObject textObject = null;
			MBTextModel mbtextModel = null;
			if (parent == null || !parent.GetVariableValue(variableName, out textObject))
			{
				this._variables.TryGetValue(variableName, out textObject);
			}
			if (textObject != null)
			{
				mbtextModel = MBTextParser.Parse(MBTextManager.Tokenizer.Tokenize(textObject.ToStringWithoutClear()));
			}
			if (mbtextModel == null)
			{
				return null;
			}
			if (mbtextModel.RootExpressions.Count == 1 && mbtextModel.RootExpressions[0] is MultiStatement)
			{
				return new MultiStatement((mbtextModel.RootExpressions[0] as MultiStatement).SubStatements);
			}
			return new MultiStatement(mbtextModel.RootExpressions);
		}

		internal ValueTuple<TextObject, bool> GetVariableValueAsTextObject(string variableName, TextObject parent)
		{
			TextObject textObject = null;
			if (parent != null)
			{
				if (parent.GetVariableValue(variableName, out textObject))
				{
					return new ValueTuple<TextObject, bool>(textObject, true);
				}
				textObject = TextProcessingContext.FindNestedFieldValue(MBTextManager.GetLocalizedText(parent.Value), variableName, parent);
				if (textObject != null && textObject.Length > 0)
				{
					return new ValueTuple<TextObject, bool>(textObject, true);
				}
			}
			if (textObject != null && textObject.Length != 0)
			{
				return new ValueTuple<TextObject, bool>(textObject, true);
			}
			return new ValueTuple<TextObject, bool>(new TextObject("{=!}ERROR: " + variableName + " variable has not been set before.", null), false);
		}

		internal MultiStatement GetArrayAccess(string variableName, int index)
		{
			string text = variableName + ":" + index;
			TextObject textObject;
			if (this._variables.TryGetValue(text, out textObject))
			{
				return new MultiStatement(MBTextParser.Parse(MBTextManager.Tokenizer.Tokenize(textObject.ToStringWithoutClear())).RootExpressions);
			}
			return null;
		}

		private int CountMarkerOccurancesInString(string searchedIdentifier, TextObject parent)
		{
			Regex regex = new Regex("{." + searchedIdentifier + "}");
			TextObject textObject = parent;
			if (parent.IsLink)
			{
				string key = parent.Attributes.First((KeyValuePair<string, object> x) => !x.Key.Equals("LINK")).Key;
				parent.GetVariableValue(key, out textObject);
			}
			string text = MBTextManager.ProcessWithoutLanguageProcessor(textObject);
			if (regex.IsMatch(text))
			{
				return 1;
			}
			return 0;
		}

		internal string GetParameterWithMarkerOccurance(string token, TextObject parent)
		{
			int num = token.IndexOf('!');
			if (num == -1)
			{
				return "";
			}
			string text = token.Substring(0, num);
			string text2 = token.Substring(num + 2, token.Length - num - 2);
			TextObject functionParamWithoutEvaluate = this.GetFunctionParamWithoutEvaluate(text);
			TextObject textObject;
			if (((((parent != null) ? parent.Attributes : null) != null && parent.TryGetAttributesValue(functionParamWithoutEvaluate.ToStringWithoutClear(), out textObject)) || this._variables.TryGetValue(functionParamWithoutEvaluate.ToStringWithoutClear(), out textObject)) && textObject.Length > 0)
			{
				return this.CountMarkerOccurancesInString(text2, textObject).ToString();
			}
			return "";
		}

		internal string GetParameterWithMarkerOccurances(string token, TextObject parent)
		{
			int num = token.IndexOf('!');
			if (num == -1)
			{
				return "";
			}
			string text = token.Substring(0, num);
			int num2 = token.IndexOf('[') + 1;
			int num3 = token.IndexOf(']');
			string[] array = token.Substring(num2, num3 - num2).Split(new char[] { ',' });
			TextObject functionParamWithoutEvaluate = this.GetFunctionParamWithoutEvaluate(text);
			TextObject textObject;
			if (((((parent != null) ? parent.Attributes : null) != null && parent.TryGetAttributesValue(functionParamWithoutEvaluate.ToStringWithoutClear(), out textObject)) || this._variables.TryGetValue(functionParamWithoutEvaluate.ToStringWithoutClear(), out textObject)) && textObject.Length > 0)
			{
				foreach (string text2 in array)
				{
					int num4 = this.CountMarkerOccurancesInString(text2, textObject);
					if (num4 > 0)
					{
						return num4.ToString();
					}
				}
				return "0";
			}
			return "";
		}

		internal static bool IsDeclaration(string token)
		{
			return token.Length > 1 && token[0] == '@';
		}

		internal static bool IsLinkToken(string token)
		{
			return token == ".link" || token == "LINK";
		}

		internal static bool IsDeclarationFinalizer(string token)
		{
			return token.Length == 2 && (token[0] == '\\' || token[0] == '/') && token[1] == '@';
		}

		private static TextObject FindNestedFieldValue(string text, string identifier, TextObject parent)
		{
			string[] array = identifier.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
			return new TextObject(TextProcessingContext.GetFieldValue(text, array, parent), null);
		}

		[return: TupleElementNames(new string[] { "value", "doesValueExist" })]
		internal ValueTuple<TextObject, bool> GetQualifiedVariableValue(string token, TextObject parent)
		{
			int num = token.IndexOf('.');
			if (num == -1)
			{
				return this.GetVariableValueAsTextObject(token, parent);
			}
			string text = token.Substring(0, num);
			string text2 = token.Substring(num + 1, token.Length - (num + 1));
			TextObject textObject;
			if (((parent != null) ? parent.Attributes : null) != null && parent.TryGetAttributesValue(text, out textObject))
			{
				ValueTuple<TextObject, bool> qualifiedVariableValue = this.GetQualifiedVariableValue(text2, textObject);
				TextObject item = qualifiedVariableValue.Item1;
				bool item2 = qualifiedVariableValue.Item2;
				if (!item.Equals(TextObject.Empty))
				{
					return new ValueTuple<TextObject, bool>(item, item2);
				}
			}
			else
			{
				TextObject textObject2;
				if (this._variables.TryGetValue(text, out textObject2) && textObject2.Length > 0)
				{
					return new ValueTuple<TextObject, bool>(TextProcessingContext.FindNestedFieldValue(MBTextManager.GetLocalizedText(textObject2.Value), text2, textObject2), true);
				}
				foreach (KeyValuePair<string, TextObject> keyValuePair in this._variables)
				{
					if (keyValuePair.Key == text && keyValuePair.Value.Attributes != null)
					{
						foreach (KeyValuePair<string, object> keyValuePair2 in keyValuePair.Value.Attributes)
						{
							if (keyValuePair2.Key == text2)
							{
								return new ValueTuple<TextObject, bool>(TextObject.TryGetOrCreateFromObject(keyValuePair2.Value), true);
							}
						}
					}
				}
			}
			return new ValueTuple<TextObject, bool>(TextObject.Empty, false);
		}

		private static string GetFieldValue(string text, string[] fieldNames, TextObject parent)
		{
			int i = 0;
			int num = 0;
			int num2 = 0;
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "GetFieldValue");
			bool flag = false;
			while (i < text.Length)
			{
				if (text[i] != '{')
				{
					if (num == fieldNames.Length && num2 == num)
					{
						mbstringBuilder.Append(text[i]);
					}
				}
				else
				{
					string text2 = TextProcessingContext.ReadFirstToken(text, ref i);
					if (TextProcessingContext.IsLinkToken(text2))
					{
						flag = true;
					}
					else if (TextProcessingContext.IsDeclarationFinalizer(text2))
					{
						num--;
						if (num2 > num)
						{
							num2 = num;
						}
					}
					else if (TextProcessingContext.IsDeclaration(text2))
					{
						string text3 = text2.Substring(1);
						bool flag2 = num2 == num && num < fieldNames.Length && string.Compare(fieldNames[num], text3, StringComparison.InvariantCultureIgnoreCase) == 0;
						num++;
						if (flag2)
						{
							num2 = num;
						}
					}
					else if (flag)
					{
						TextObject textObject;
						if (parent.Attributes != null && parent.TryGetAttributesValue(text2, out textObject))
						{
							return TextProcessingContext.GetFieldValuesFromLinks(fieldNames, textObject, ref mbstringBuilder);
						}
					}
					else if (num == fieldNames.Length && num2 == num)
					{
						mbstringBuilder.Append<string>("{" + text2 + "}");
					}
				}
				i++;
			}
			return mbstringBuilder.ToStringAndRelease();
		}

		private static string GetFieldValuesFromLinks(string[] fieldNames, TextObject value, ref MBStringBuilder targetString)
		{
			TextObject textObject;
			if (fieldNames.Length == 1 && value.TryGetAttributesValue(fieldNames[0], out textObject))
			{
				targetString.Append<TextObject>(textObject);
				return targetString.ToStringAndRelease();
			}
			targetString.Append<string>(TextProcessingContext.GetFieldValue(MBTextManager.GetLocalizedText(value.Value), fieldNames, null));
			return targetString.ToStringAndRelease();
		}

		internal static string ReadFirstToken(string text, ref int i)
		{
			int num = i;
			while (i < text.Length && text[i] != '}')
			{
				i++;
			}
			int num2 = i - num;
			return text.Substring(num + 1, num2 - 1);
		}

		internal TextObject CallFunction(string functionName, List<TextExpression> functionParams, TextObject parent)
		{
			TextObject[] array = new TextObject[functionParams.Count];
			TextObject[] array2 = new TextObject[functionParams.Count];
			for (int i = 0; i < functionParams.Count; i++)
			{
				array[i] = new TextObject(functionParams[i].EvaluateString(this, parent), null);
				array2[i] = new TextObject(functionParams[i].RawValue, null);
			}
			this._curParams.Push(array);
			this._curParamsWithoutEvaluate.Push(array2);
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			MBTextModel functionBody = this.GetFunctionBody(functionName);
			mbstringBuilder.Initialize(16, "CallFunction");
			if (functionBody != null)
			{
				using (List<TextExpression>.Enumerator enumerator = functionBody.RootExpressions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TextExpression textExpression = enumerator.Current;
						mbstringBuilder.Append<string>(textExpression.EvaluateString(this, parent));
					}
					goto IL_E7;
				}
			}
			if (array.Length != 0)
			{
				mbstringBuilder.Append<TextObject>(array[0]);
			}
			IL_E7:
			string text = mbstringBuilder.ToStringAndRelease();
			this._curParams.Pop();
			return new TextObject(text, null);
		}

		public void SetFunction(string functionName, MBTextModel functionBody)
		{
			this._functions[functionName] = functionBody;
		}

		public void ResetFunctions()
		{
			this._functions.Clear();
		}

		public MBTextModel GetFunctionBody(string functionName)
		{
			MBTextModel mbtextModel;
			this._functions.TryGetValue(functionName, out mbtextModel);
			return mbtextModel;
		}

		public TextObject GetFunctionParam(string rawValue)
		{
			int num;
			if (!int.TryParse(rawValue.Substring(1), out num))
			{
				return TextObject.Empty;
			}
			if (this._curParams.Count > 0 && this._curParams.Peek().Length > num)
			{
				return this._curParams.Peek()[num];
			}
			return new TextObject("Can't find parameter:" + rawValue, null);
		}

		public TextObject GetFunctionParamWithoutEvaluate(string rawValue)
		{
			int num;
			if (!int.TryParse(rawValue.Substring(1), out num))
			{
				return TextObject.Empty;
			}
			if (this._curParamsWithoutEvaluate.Count > 0 && this._curParamsWithoutEvaluate.Peek().Length > num)
			{
				return this._curParamsWithoutEvaluate.Peek()[num];
			}
			return new TextObject("Can't find parameter:" + rawValue, null);
		}

		internal void ClearAll()
		{
			this._variables.Clear();
		}

		private readonly Dictionary<string, TextObject> _variables = new Dictionary<string, TextObject>(new CaseInsensitiveComparer());

		private readonly Dictionary<string, MBTextModel> _functions = new Dictionary<string, MBTextModel>();

		private readonly Stack<TextObject[]> _curParams = new Stack<TextObject[]>();

		private readonly Stack<TextObject[]> _curParamsWithoutEvaluate = new Stack<TextObject[]>();
	}
}
