using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	public class GameText
	{
		public string Id { get; private set; }

		public IEnumerable<GameText.GameTextVariation> Variations
		{
			get
			{
				return this._variationList;
			}
		}

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

		internal GameText()
		{
			this._variationList = new List<GameText.GameTextVariation>();
		}

		internal GameText(string id)
		{
			this.Id = id;
			this._variationList = new List<GameText.GameTextVariation>();
		}

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

		private readonly List<GameText.GameTextVariation> _variationList;

		public struct GameTextVariation
		{
			internal GameTextVariation(string id, TextObject text, List<GameTextManager.ChoiceTag> choiceTags)
			{
				this.Id = id;
				this.Text = text;
				this.Tags = choiceTags.ToArray();
			}

			public readonly string Id;

			public readonly TextObject Text;

			public readonly GameTextManager.ChoiceTag[] Tags;
		}
	}
}
