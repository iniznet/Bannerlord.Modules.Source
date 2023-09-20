using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	public static class GameTexts
	{
		public static void Initialize(GameTextManager gameTextManager)
		{
			GameTexts._gameTextManager = gameTextManager;
		}

		public static TextObject FindText(string id, string variation = null)
		{
			return GameTexts._gameTextManager.FindText(id, variation);
		}

		public static bool TryGetText(string id, out TextObject textObject, string variation = null)
		{
			return GameTexts._gameTextManager.TryGetText(id, variation, out textObject);
		}

		public static IEnumerable<TextObject> FindAllTextVariations(string id)
		{
			return GameTexts._gameTextManager.FindAllTextVariations(id);
		}

		public static void SetVariable(string variableName, string content)
		{
			MBTextManager.SetTextVariable(variableName, content, false);
		}

		public static void SetVariable(string variableName, float content)
		{
			MBTextManager.SetTextVariable(variableName, content);
		}

		public static void SetVariable(string variableName, int content)
		{
			MBTextManager.SetTextVariable(variableName, content);
		}

		public static void SetVariable(string variableName, TextObject content)
		{
			MBTextManager.SetTextVariable(variableName, content, false);
		}

		public static void ClearInstance()
		{
			GameTexts._gameTextManager = null;
		}

		public static GameTexts.GameTextHelper AddGameTextWithVariation(string id)
		{
			return new GameTexts.GameTextHelper(id);
		}

		private static GameTextManager _gameTextManager;

		public class GameTextHelper
		{
			public GameTextHelper(string id)
			{
				this._id = id;
			}

			public GameTexts.GameTextHelper Variation(string text, params object[] propertiesAndWeights)
			{
				GameTexts._gameTextManager.AddGameText(this._id).AddVariation(text, propertiesAndWeights);
				return this;
			}

			public static TextObject MergeTextObjectsWithComma(List<TextObject> textObjects, bool includeAnd)
			{
				return GameTexts.GameTextHelper.MergeTextObjectsWithSymbol(textObjects, new TextObject("{=kfdxjIad}, ", null), includeAnd ? new TextObject("{=eob9goyW} and ", null) : null);
			}

			public static TextObject MergeTextObjectsWithSymbol(List<TextObject> textObjects, TextObject symbol, TextObject lastSymbol = null)
			{
				int count = textObjects.Count;
				TextObject textObject;
				if (count == 0)
				{
					textObject = TextObject.Empty;
				}
				else if (count == 1)
				{
					textObject = textObjects[0];
				}
				else
				{
					string text = "{=!}";
					for (int i = 0; i < textObjects.Count - 2; i++)
					{
						text = string.Concat(new object[] { text, "{VAR_", i, "}{SYMBOL}" });
					}
					text = string.Concat(new object[]
					{
						text,
						"{VAR_",
						textObjects.Count - 2,
						"}{LAST_SYMBOL}{VAR_",
						textObjects.Count - 1,
						"}"
					});
					textObject = new TextObject(text, null);
					for (int j = 0; j < textObjects.Count; j++)
					{
						textObject.SetTextVariable("VAR_" + j, textObjects[j]);
					}
					textObject.SetTextVariable("SYMBOL", symbol);
					textObject.SetTextVariable("LAST_SYMBOL", lastSymbol ?? symbol);
				}
				return textObject;
			}

			private string _id;
		}
	}
}
