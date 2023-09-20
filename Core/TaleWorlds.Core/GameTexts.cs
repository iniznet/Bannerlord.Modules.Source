using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	// Token: 0x02000074 RID: 116
	public static class GameTexts
	{
		// Token: 0x06000758 RID: 1880 RVA: 0x00019604 File Offset: 0x00017804
		public static void Initialize(GameTextManager gameTextManager)
		{
			GameTexts._gameTextManager = gameTextManager;
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x0001960C File Offset: 0x0001780C
		public static TextObject FindText(string id, string variation = null)
		{
			return GameTexts._gameTextManager.FindText(id, variation);
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x0001961A File Offset: 0x0001781A
		public static bool TryGetText(string id, out TextObject textObject, string variation = null)
		{
			return GameTexts._gameTextManager.TryGetText(id, variation, out textObject);
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x00019629 File Offset: 0x00017829
		public static IEnumerable<TextObject> FindAllTextVariations(string id)
		{
			return GameTexts._gameTextManager.FindAllTextVariations(id);
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x00019636 File Offset: 0x00017836
		public static void SetVariable(string variableName, string content)
		{
			MBTextManager.SetTextVariable(variableName, content, false);
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x00019640 File Offset: 0x00017840
		public static void SetVariable(string variableName, float content)
		{
			MBTextManager.SetTextVariable(variableName, content);
		}

		// Token: 0x0600075E RID: 1886 RVA: 0x00019649 File Offset: 0x00017849
		public static void SetVariable(string variableName, int content)
		{
			MBTextManager.SetTextVariable(variableName, content);
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x00019652 File Offset: 0x00017852
		public static void SetVariable(string variableName, TextObject content)
		{
			MBTextManager.SetTextVariable(variableName, content, false);
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x0001965C File Offset: 0x0001785C
		public static void ClearInstance()
		{
			GameTexts._gameTextManager = null;
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x00019664 File Offset: 0x00017864
		public static GameTexts.GameTextHelper AddGameTextWithVariation(string id)
		{
			return new GameTexts.GameTextHelper(id);
		}

		// Token: 0x040003B4 RID: 948
		private static GameTextManager _gameTextManager;

		// Token: 0x02000100 RID: 256
		public class GameTextHelper
		{
			// Token: 0x06000A41 RID: 2625 RVA: 0x000213FB File Offset: 0x0001F5FB
			public GameTextHelper(string id)
			{
				this._id = id;
			}

			// Token: 0x06000A42 RID: 2626 RVA: 0x0002140A File Offset: 0x0001F60A
			public GameTexts.GameTextHelper Variation(string text, params object[] propertiesAndWeights)
			{
				GameTexts._gameTextManager.AddGameText(this._id).AddVariation(text, propertiesAndWeights);
				return this;
			}

			// Token: 0x06000A43 RID: 2627 RVA: 0x00021424 File Offset: 0x0001F624
			public static TextObject MergeTextObjectsWithComma(List<TextObject> textObjects, bool includeAnd)
			{
				return GameTexts.GameTextHelper.MergeTextObjectsWithSymbol(textObjects, new TextObject("{=kfdxjIad}, ", null), includeAnd ? new TextObject("{=eob9goyW} and ", null) : null);
			}

			// Token: 0x06000A44 RID: 2628 RVA: 0x00021448 File Offset: 0x0001F648
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

			// Token: 0x040006D0 RID: 1744
			private string _id;
		}
	}
}
