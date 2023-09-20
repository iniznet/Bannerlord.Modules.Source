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
	// Token: 0x02000073 RID: 115
	public class GameTextManager
	{
		// Token: 0x0600074F RID: 1871 RVA: 0x000191E5 File Offset: 0x000173E5
		public GameTextManager()
		{
			this._gameTexts = new Dictionary<string, GameText>();
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x000191F8 File Offset: 0x000173F8
		public GameText GetGameText(string id)
		{
			GameText gameText;
			if (this._gameTexts.TryGetValue(id, out gameText))
			{
				return gameText;
			}
			return null;
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x00019218 File Offset: 0x00017418
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

		// Token: 0x06000752 RID: 1874 RVA: 0x00019250 File Offset: 0x00017450
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

		// Token: 0x06000753 RID: 1875 RVA: 0x000192A0 File Offset: 0x000174A0
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

		// Token: 0x06000754 RID: 1876 RVA: 0x000192F0 File Offset: 0x000174F0
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

		// Token: 0x06000755 RID: 1877 RVA: 0x00019308 File Offset: 0x00017508
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

		// Token: 0x06000756 RID: 1878 RVA: 0x00019384 File Offset: 0x00017584
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

		// Token: 0x06000757 RID: 1879 RVA: 0x000193E8 File Offset: 0x000175E8
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

		// Token: 0x040003B3 RID: 947
		private readonly Dictionary<string, GameText> _gameTexts;

		// Token: 0x020000FE RID: 254
		public struct ChoiceTag
		{
			// Token: 0x17000352 RID: 850
			// (get) Token: 0x06000A31 RID: 2609 RVA: 0x000211FB File Offset: 0x0001F3FB
			// (set) Token: 0x06000A32 RID: 2610 RVA: 0x00021203 File Offset: 0x0001F403
			public string TagName { get; private set; }

			// Token: 0x17000353 RID: 851
			// (get) Token: 0x06000A33 RID: 2611 RVA: 0x0002120C File Offset: 0x0001F40C
			// (set) Token: 0x06000A34 RID: 2612 RVA: 0x00021214 File Offset: 0x0001F414
			public uint Weight { get; private set; }

			// Token: 0x17000354 RID: 852
			// (get) Token: 0x06000A35 RID: 2613 RVA: 0x0002121D File Offset: 0x0001F41D
			// (set) Token: 0x06000A36 RID: 2614 RVA: 0x00021225 File Offset: 0x0001F425
			public bool IsTagReversed { get; private set; }

			// Token: 0x06000A37 RID: 2615 RVA: 0x0002122E File Offset: 0x0001F42E
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
