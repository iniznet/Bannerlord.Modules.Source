using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public class GameTextManager
	{
		public GameTextManager()
		{
			this._gameTexts = new Dictionary<string, GameText>();
		}

		public GameText GetGameText(string id)
		{
			GameText gameText;
			if (this._gameTexts.TryGetValue(id, out gameText))
			{
				return gameText;
			}
			return null;
		}

		public GameText AddGameText(string id)
		{
			GameText gameText;
			if (!this._gameTexts.TryGetValue(id, out gameText))
			{
				gameText = new GameText(id);
				this._gameTexts.Add(gameText.Id, gameText);
			}
			return gameText;
		}

		public bool TryGetText(string id, string variation, out TextObject text)
		{
			text = null;
			GameText gameText;
			this._gameTexts.TryGetValue(id, out gameText);
			if (gameText != null)
			{
				if (variation == null)
				{
					text = gameText.DefaultText;
				}
				else
				{
					text = gameText.GetVariation(variation);
				}
				if (text != null)
				{
					text = text.CopyTextObject();
					text.AddIDToValue(id);
					return true;
				}
			}
			return false;
		}

		public TextObject FindText(string id, string variation = null)
		{
			TextObject textObject;
			if (this.TryGetText(id, variation, out textObject))
			{
				return textObject;
			}
			TextObject textObject2;
			if (variation == null)
			{
				textObject2 = new TextObject("{=!}ERROR: Text with id " + id + " doesn't exist!", null);
			}
			else
			{
				textObject2 = new TextObject("{=!}ERROR: Text with id " + id + " doesn't exist! Variation: " + variation, null);
			}
			return textObject2;
		}

		public IEnumerable<TextObject> FindAllTextVariations(string id)
		{
			GameText gameText;
			this._gameTexts.TryGetValue(id, out gameText);
			if (gameText != null)
			{
				foreach (GameText.GameTextVariation gameTextVariation in gameText.Variations)
				{
					yield return gameTextVariation.Text;
				}
				IEnumerator<GameText.GameTextVariation> enumerator = null;
			}
			yield break;
			yield break;
		}

		public void LoadGameTexts()
		{
			Game game = Game.Current;
			bool flag = false;
			string text = "";
			if (game != null)
			{
				flag = game.GameType.IsDevelopment;
				text = game.GameType.GetType().Name;
			}
			XmlDocument mergedXmlForManaged = MBObjectManager.GetMergedXmlForManaged("GameText", false, flag, text);
			try
			{
				this.LoadFromXML(mergedXmlForManaged);
			}
			catch (Exception ex)
			{
				Debug.ShowError("Could not load merged xml file correctly: GameText Error: " + ex.Message);
			}
		}

		public void LoadDefaultTexts()
		{
			string text = ModuleHelper.GetModuleFullPath("Native") + "ModuleData/global_strings.xml";
			Debug.Print("opening " + text, 0, Debug.DebugColor.White, 17592186044416UL);
			XmlDocument xmlDocument = new XmlDocument();
			StreamReader streamReader = new StreamReader(text);
			string text2 = streamReader.ReadToEnd();
			xmlDocument.LoadXml(text2);
			streamReader.Close();
			this.LoadFromXML(xmlDocument);
		}

		private void LoadFromXML(XmlDocument doc)
		{
			XmlNode xmlNode = null;
			for (int i = 0; i < doc.ChildNodes.Count; i++)
			{
				XmlNode xmlNode2 = doc.ChildNodes[i];
				if (xmlNode2.NodeType != XmlNodeType.Comment && xmlNode2.Name == "strings" && xmlNode2.ChildNodes.Count > 0)
				{
					xmlNode = xmlNode2.ChildNodes[0];
					IL_1F8:
					while (xmlNode != null)
					{
						if (xmlNode.Name == "string" && xmlNode.NodeType != XmlNodeType.Comment)
						{
							if (xmlNode.Attributes == null)
							{
								throw new TWXmlLoadException("Node attributes are null.");
							}
							string[] array = xmlNode.Attributes["id"].Value.Split(new char[] { '.' });
							string text = array[0];
							GameText gameText = this.AddGameText(text);
							string text2 = "";
							if (array.Length > 1)
							{
								text2 = array[1];
							}
							TextObject textObject = new TextObject(xmlNode.Attributes["text"].Value, null);
							List<GameTextManager.ChoiceTag> list = new List<GameTextManager.ChoiceTag>();
							foreach (object obj in xmlNode.ChildNodes)
							{
								XmlNode xmlNode3 = (XmlNode)obj;
								if (xmlNode3.Name == "tags")
								{
									XmlNodeList childNodes = xmlNode3.ChildNodes;
									for (int j = 0; j < childNodes.Count; j++)
									{
										XmlAttributeCollection attributes = childNodes[j].Attributes;
										if (attributes != null)
										{
											int num = 1;
											if (attributes["weight"] != null)
											{
												int.TryParse(attributes["weight"].Value, out num);
											}
											GameTextManager.ChoiceTag choiceTag = new GameTextManager.ChoiceTag(attributes["tag_name"].Value, num);
											list.Add(choiceTag);
										}
									}
								}
							}
							textObject.CacheTokens();
							gameText.AddVariationWithId(text2, textObject, list);
						}
						xmlNode = xmlNode.NextSibling;
					}
					return;
				}
			}
			goto IL_1F8;
		}

		private readonly Dictionary<string, GameText> _gameTexts;

		public struct ChoiceTag
		{
			public string TagName { get; private set; }

			public uint Weight { get; private set; }

			public bool IsTagReversed { get; private set; }

			public ChoiceTag(string tagName, int weight)
			{
				this = default(GameTextManager.ChoiceTag);
				this.TagName = tagName;
				this.Weight = (uint)MathF.Abs(weight);
				this.IsTagReversed = weight < 0;
			}
		}
	}
}
