using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	// Token: 0x02000072 RID: 114
	public class GameText
	{
		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x0001902C File Offset: 0x0001722C
		// (set) Token: 0x06000747 RID: 1863 RVA: 0x00019034 File Offset: 0x00017234
		public string Id { get; private set; }

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06000748 RID: 1864 RVA: 0x0001903D File Offset: 0x0001723D
		public IEnumerable<GameText.GameTextVariation> Variations
		{
			get
			{
				return this._variationList;
			}
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06000749 RID: 1865 RVA: 0x00019045 File Offset: 0x00017245
		public TextObject DefaultText
		{
			get
			{
				if (this._variationList != null && this._variationList.Count > 0)
				{
					return this._variationList[0].Text;
				}
				return null;
			}
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x00019070 File Offset: 0x00017270
		internal GameText()
		{
			this._variationList = new List<GameText.GameTextVariation>();
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x00019083 File Offset: 0x00017283
		internal GameText(string id)
		{
			this.Id = id;
			this._variationList = new List<GameText.GameTextVariation>();
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x000190A0 File Offset: 0x000172A0
		internal TextObject GetVariation(string variationId)
		{
			foreach (GameText.GameTextVariation gameTextVariation in this._variationList)
			{
				if (gameTextVariation.Id.Equals(variationId))
				{
					return gameTextVariation.Text;
				}
			}
			return null;
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x00019108 File Offset: 0x00017308
		public void AddVariationWithId(string variationId, TextObject text, List<GameTextManager.ChoiceTag> choiceTags)
		{
			foreach (GameText.GameTextVariation gameTextVariation in this._variationList)
			{
				if (gameTextVariation.Id.Equals(variationId) && gameTextVariation.Text.ToString().Equals(text.ToString()))
				{
					return;
				}
			}
			this._variationList.Add(new GameText.GameTextVariation(variationId, text, choiceTags));
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x00019190 File Offset: 0x00017390
		public void AddVariation(string text, params object[] propertiesAndWeights)
		{
			List<GameTextManager.ChoiceTag> list = new List<GameTextManager.ChoiceTag>();
			for (int i = 0; i < propertiesAndWeights.Length; i += 2)
			{
				string text2 = (string)propertiesAndWeights[i];
				int num = Convert.ToInt32(propertiesAndWeights[i + 1]);
				list.Add(new GameTextManager.ChoiceTag(text2, num));
			}
			this.AddVariationWithId("", new TextObject(text, null), list);
		}

		// Token: 0x040003B2 RID: 946
		private readonly List<GameText.GameTextVariation> _variationList;

		// Token: 0x020000FD RID: 253
		public struct GameTextVariation
		{
			// Token: 0x06000A30 RID: 2608 RVA: 0x000211DF File Offset: 0x0001F3DF
			internal GameTextVariation(string id, TextObject text, List<GameTextManager.ChoiceTag> choiceTags)
			{
				this.Id = id;
				this.Text = text;
				this.Tags = choiceTags.ToArray();
			}

			// Token: 0x040006C3 RID: 1731
			public readonly string Id;

			// Token: 0x040006C4 RID: 1732
			public readonly TextObject Text;

			// Token: 0x040006C5 RID: 1733
			public readonly GameTextManager.ChoiceTag[] Tags;
		}
	}
}
