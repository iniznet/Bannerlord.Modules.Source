using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using TaleWorlds.Library;

namespace TaleWorlds.Localization.TextProcessor.LanguageProcessors
{
	public class RussianTextProcessor : LanguageSpecificTextProcessor
	{
		public override CultureInfo CultureInfoForLanguage
		{
			get
			{
				return RussianTextProcessor.CultureInfo;
			}
		}

		public override void ClearTemporaryData()
		{
			RussianTextProcessor.LinkList.Clear();
			RussianTextProcessor.WordGroups.Clear();
			RussianTextProcessor.WordGroupsNoTags.Clear();
			RussianTextProcessor._curGender = RussianTextProcessor.WordGenderEnum.NoDeclination;
			RussianTextProcessor._doesComeFromWordGroup = false;
		}

		private bool MasculineAnimate
		{
			get
			{
				return RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.MasculineAnimate;
			}
		}

		private bool MasculineInanimate
		{
			get
			{
				return RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.MasculineInanimate;
			}
		}

		private bool Masculine
		{
			get
			{
				return RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.MasculineAnimate || RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.MasculineInanimate;
			}
		}

		private bool FeminineAnimate
		{
			get
			{
				return RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.FeminineAnimate;
			}
		}

		private bool FeminineInanimate
		{
			get
			{
				return RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.FeminineInanimate;
			}
		}

		private bool Feminine
		{
			get
			{
				return RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.FeminineAnimate || RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.FeminineInanimate;
			}
		}

		private bool Neuter
		{
			get
			{
				return RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.NeuterInanimate || RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.NeuterAnimate;
			}
		}

		private bool NeuterInanimate
		{
			get
			{
				return RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.NeuterInanimate;
			}
		}

		private bool NeuterAnimate
		{
			get
			{
				return RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.NeuterAnimate;
			}
		}

		private bool Animate
		{
			get
			{
				return RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.MasculineAnimate || RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.FeminineAnimate || RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.NeuterAnimate;
			}
		}

		[TupleElementNames(new string[] { "wordGroup", "firstMarkerPost" })]
		private static List<ValueTuple<string, int>> WordGroups
		{
			[return: TupleElementNames(new string[] { "wordGroup", "firstMarkerPost" })]
			get
			{
				if (RussianTextProcessor._wordGroups == null)
				{
					RussianTextProcessor._wordGroups = new List<ValueTuple<string, int>>();
				}
				return RussianTextProcessor._wordGroups;
			}
		}

		private static List<string> WordGroupsNoTags
		{
			get
			{
				if (RussianTextProcessor._wordGroupsNoTags == null)
				{
					RussianTextProcessor._wordGroupsNoTags = new List<string>();
				}
				return RussianTextProcessor._wordGroupsNoTags;
			}
		}

		private static List<string> LinkList
		{
			get
			{
				if (RussianTextProcessor._linkList == null)
				{
					RussianTextProcessor._linkList = new List<string>();
				}
				return RussianTextProcessor._linkList;
			}
		}

		private string LinkTag
		{
			get
			{
				return ".link";
			}
		}

		private int LinkTagLength
		{
			get
			{
				return 7;
			}
		}

		private string LinkStarter
		{
			get
			{
				return "<a style=\"Link.";
			}
		}

		private string LinkEnding
		{
			get
			{
				return "</b></a>";
			}
		}

		private int LinkEndingLength
		{
			get
			{
				return 8;
			}
		}

		private static char GetLastCharacter(StringBuilder outputString)
		{
			if (outputString.Length <= 0)
			{
				return '*';
			}
			return outputString[outputString.Length - 1];
		}

		private static string GetEnding(StringBuilder outputString, int numChars)
		{
			numChars = MathF.Min(numChars, outputString.Length);
			return outputString.ToString(outputString.Length - numChars, numChars);
		}

		public override void ProcessToken(string sourceText, ref int cursorPos, string token, StringBuilder outputString)
		{
			bool flag = false;
			if (token == this.LinkTag)
			{
				RussianTextProcessor.LinkList.Add(sourceText.Substring(this.LinkTagLength));
			}
			else if (sourceText.Contains(this.LinkStarter))
			{
				flag = this.IsLink(sourceText, token.Length + 2, cursorPos);
			}
			if (flag)
			{
				cursorPos -= this.LinkEndingLength;
				outputString.Remove(outputString.Length - this.LinkEndingLength, this.LinkEndingLength);
			}
			int num2;
			if (token.EndsWith("Creator"))
			{
				outputString.Append("{" + token.Replace("Creator", "") + "}");
			}
			else if (Array.IndexOf<string>(RussianTextProcessor.GenderTokens.TokenList, token) >= 0)
			{
				if (token == ".MA")
				{
					this.SetMasculineAnimate();
				}
				else if (token == ".MI")
				{
					this.SetMasculineInanimate();
				}
				else if (token == ".FI")
				{
					this.SetFeminineInanimate();
				}
				else if (token == ".FA")
				{
					this.SetFeminineAnimate();
				}
				else if (token == ".NA")
				{
					this.SetNeuterAnimate();
				}
				else if (token == ".NI")
				{
					this.SetNeuterInanimate();
				}
				if (cursorPos == token.Length + 2 && sourceText.IndexOf("{.", cursorPos, StringComparison.InvariantCulture) == -1 && sourceText.IndexOf(' ', cursorPos) == -1)
				{
					RussianTextProcessor.WordGroups.Add(new ValueTuple<string, int>(sourceText + "{.nn}", cursorPos));
					RussianTextProcessor.WordGroupsNoTags.Add(sourceText.Substring(cursorPos));
				}
			}
			else if (token == ".nnp" || token == ".ajp" || token == ".aj" || token == ".nn")
			{
				if (token == ".nnp" || token == ".ajp" || token == ".aj")
				{
					string text;
					int num;
					if (this.IsIrregularWord(sourceText, cursorPos, token, out text, out num))
					{
						outputString.Remove(outputString.Length - num, num);
						outputString.Append(text);
					}
					else if (token == ".nnp")
					{
						this.AddSuffixNounNominativePlural(outputString);
					}
					else if (token == ".ajp")
					{
						this.AddSuffixAdjectiveNominativePlural(outputString);
					}
					else if (token == ".aj")
					{
						this.AddSuffixAdjectiveNominative(outputString);
					}
				}
				RussianTextProcessor._curGender = RussianTextProcessor.WordGenderEnum.NoDeclination;
				this.WordGroupProcessor(sourceText, cursorPos);
			}
			else if (Array.IndexOf<string>(RussianTextProcessor.NounTokens.TokenList, token) >= 0 && (!RussianTextProcessor._doesComeFromWordGroup || (RussianTextProcessor._doesComeFromWordGroup && RussianTextProcessor._curGender == RussianTextProcessor.WordGenderEnum.NoDeclination)) && this.IsWordGroup(token.Length, sourceText, cursorPos, out num2))
			{
				if (num2 >= 0)
				{
					token = "{" + token + "}";
					RussianTextProcessor._curGender = RussianTextProcessor.WordGenderEnum.NoDeclination;
					this.AddSuffixWordGroup(token, num2, outputString);
				}
			}
			else if (RussianTextProcessor._curGender != RussianTextProcessor.WordGenderEnum.NoDeclination)
			{
				string text2;
				int num3;
				if (this.IsIrregularWord(sourceText, cursorPos, token, out text2, out num3))
				{
					outputString.Remove(outputString.Length - num3, num3);
					outputString.Append(text2);
				}
				else if (token == ".p")
				{
					this.AddSuffixNounNominativePlural(outputString);
				}
				else if (token == ".a")
				{
					this.AddSuffixNounAccusative(outputString);
				}
				else if (token == ".ap")
				{
					this.AddSuffixNounAccusativePlural(outputString);
				}
				else if (token == ".g")
				{
					this.AddSuffixNounGenitive(outputString);
				}
				else if (token == ".gp")
				{
					this.AddSuffixNounGenitivePlural(outputString);
				}
				else if (token == ".d")
				{
					this.AddSuffixNounDative(outputString);
				}
				else if (token == ".dp")
				{
					this.AddSuffixNounDativePlural(outputString);
				}
				else if (token == ".l")
				{
					this.AddSuffixNounLocative(outputString);
				}
				else if (token == ".lp")
				{
					this.AddSuffixNounLocativePlural(outputString);
				}
				else if (token == ".i")
				{
					this.AddSuffixNounInstrumental(outputString);
				}
				else if (token == ".ip")
				{
					this.AddSuffixNounInstrumentalPlural(outputString);
				}
				else if (token == ".j")
				{
					this.AddSuffixAdjectiveNominative(outputString);
				}
				else if (token == ".jp")
				{
					this.AddSuffixAdjectiveNominativePlural(outputString);
				}
				else if (token == ".ja")
				{
					this.AddSuffixAdjectiveAccusative(outputString);
				}
				else if (token == ".jap")
				{
					this.AddSuffixAdjectiveAccusativePlural(outputString);
				}
				else if (token == ".jg")
				{
					this.AddSuffixAdjectiveGenitive(outputString);
				}
				else if (token == ".jgp")
				{
					this.AddSuffixAdjectiveGenitivePlural(outputString);
				}
				else if (token == ".jd")
				{
					this.AddSuffixAdjectiveDative(outputString);
				}
				else if (token == ".jdp")
				{
					this.AddSuffixAdjectiveDativePlural(outputString);
				}
				else if (token == ".jl")
				{
					this.AddSuffixAdjectiveLocative(outputString);
				}
				else if (token == ".jlp")
				{
					this.AddSuffixAdjectiveLocativePlural(outputString);
				}
				else if (token == ".ji")
				{
					this.AddSuffixAdjectiveInstrumental(outputString);
				}
				else if (token == ".jip")
				{
					this.AddSuffixAdjectiveInstrumentalPlural(outputString);
				}
				RussianTextProcessor._curGender = RussianTextProcessor.WordGenderEnum.NoDeclination;
			}
			if (flag)
			{
				cursorPos += this.LinkEndingLength;
				outputString.Append(this.LinkEnding);
			}
		}

		private void AddSuffixWordGroup(string token, int wordGroupIndex, StringBuilder outputString)
		{
			bool flag = char.IsUpper(outputString[outputString.Length - RussianTextProcessor.WordGroupsNoTags[wordGroupIndex].Length]);
			string text = RussianTextProcessor.WordGroups[wordGroupIndex].Item1;
			outputString.Remove(outputString.Length - RussianTextProcessor.WordGroupsNoTags[wordGroupIndex].Length, RussianTextProcessor.WordGroupsNoTags[wordGroupIndex].Length);
			text = text.Replace("{.nn}", token);
			if (token.Equals("{.n}"))
			{
				text = text.Replace("{.nnp}", "{.p}");
				text = text.Replace("{.ajp}", "{.jp}");
				text = text.Replace("{.aj}", "{.j}");
			}
			else
			{
				text = text.Replace("{.aj}", token.Insert(2, "j"));
				if (token.Contains("p"))
				{
					text = text.Replace("{.nnp}", token);
					text = text.Replace("{.ajp}", token.Insert(2, "j"));
				}
				else
				{
					text = text.Replace("{.nnp}", token.Insert(3, "p"));
					text = text.Replace("{.ajp}", token.Insert(2, "j").Insert(4, "p"));
				}
			}
			RussianTextProcessor._doesComeFromWordGroup = true;
			string text2 = base.Process(text);
			RussianTextProcessor._doesComeFromWordGroup = false;
			if (flag && char.IsLower(text2[0]))
			{
				outputString.Append(char.ToUpperInvariant(text2[0]));
				outputString.Append(text2.Substring(1));
				return;
			}
			if (!flag && char.IsUpper(text2[0]))
			{
				outputString.Append(char.ToLowerInvariant(text2[0]));
				outputString.Append(text2.Substring(1));
				return;
			}
			outputString.Append(text2);
		}

		private bool IsWordGroup(int tokenLength, string sourceText, int curPos, out int wordGroupIndex)
		{
			int num = 0;
			while (num < RussianTextProcessor.WordGroupsNoTags.Count && curPos - tokenLength - 2 - RussianTextProcessor.WordGroupsNoTags[num].Length >= 0)
			{
				if (sourceText.Substring(curPos - tokenLength - 2 - RussianTextProcessor.WordGroupsNoTags[num].Length, RussianTextProcessor.WordGroupsNoTags[num].Length).Equals(RussianTextProcessor.WordGroupsNoTags[num]))
				{
					wordGroupIndex = num;
					return true;
				}
				num++;
			}
			wordGroupIndex = -1;
			return false;
		}

		private void AddSuffixNounNominativePlural(StringBuilder outputString)
		{
			char c = RussianTextProcessor.GetLastCharacter(outputString);
			if (c == 'а' && !this.Neuter)
			{
				outputString.Remove(outputString.Length - 1, 1);
				c = RussianTextProcessor.GetLastCharacter(outputString);
				if (c == 'г' || c == 'ж' || c == 'к' || c == 'х' || c == 'ч' || c == 'ш' || c == 'щ')
				{
					outputString.Append('и');
					return;
				}
				outputString.Append('ы');
				return;
			}
			else
			{
				if (c == 'я' && !this.Neuter)
				{
					outputString.Remove(outputString.Length - 1, 1).Append('и');
					return;
				}
				if (!this.Neuter)
				{
					if (this.Feminine)
					{
						if (c == 'ь')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('и');
							return;
						}
					}
					else
					{
						if (c == 'ь')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('и');
							return;
						}
						if (c == 'й')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('и');
							return;
						}
						if (c == 'о')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('и');
							return;
						}
						if (c == 'е')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('я');
							return;
						}
						if (c == 'м')
						{
							outputString.Append('а');
							return;
						}
						if (this.MasculineAnimate && RussianTextProcessor.GetEnding(outputString, 2) == "ин")
						{
							outputString.Remove(outputString.Length - 2, 2).Append('е');
							return;
						}
						if (this.IsConsonant(c))
						{
							if (this.Animate && (RussianTextProcessor.GetEnding(outputString, 4) == "енок" || RussianTextProcessor.GetEnding(outputString, 4) == "ёнок"))
							{
								outputString.Remove(outputString.Length - 4, 4).Append("ята");
								return;
							}
							if (this.Animate && RussianTextProcessor.GetEnding(outputString, 4) == "онок")
							{
								outputString.Remove(outputString.Length - 4, 4).Append("ата");
								return;
							}
							if (RussianTextProcessor.GetEnding(outputString, 2) == "ок" && outputString.ToString() != "сок")
							{
								outputString.Remove(outputString.Length - 2, 2).Append("ки");
								return;
							}
							if (RussianTextProcessor.GetEnding(outputString, 2) == "ек" && RussianTextProcessor.GetEnding(outputString, 2) == "ёк")
							{
								if (this.IsConsonant(RussianTextProcessor.GetEnding(outputString, 3)[0]))
								{
									outputString.Remove(outputString.Length - 2, 2).Append("ьки");
									return;
								}
								outputString.Remove(outputString.Length - 2, 2).Append("йки");
								return;
							}
							else
							{
								if (c == 'г' || c == 'ж' || c == 'к' || c == 'х' || c == 'ч' || c == 'ш' || c == 'щ')
								{
									outputString.Append('и');
									return;
								}
								outputString.Append('ы');
							}
						}
					}
					return;
				}
				outputString.Remove(outputString.Length - 1, 1);
				if (c == 'о')
				{
					outputString.Append('а');
					return;
				}
				if (c == 'е')
				{
					c = RussianTextProcessor.GetLastCharacter(outputString);
					if (c == 'ч' || c == 'щ')
					{
						outputString.Append('а');
						return;
					}
					outputString.Append('я');
					return;
				}
				else
				{
					if (c == 'я' && RussianTextProcessor.GetLastCharacter(outputString) == 'м')
					{
						outputString.Append("ена");
						return;
					}
					outputString.Append(c);
					return;
				}
			}
		}

		private void AddSuffixNounGenitive(StringBuilder outputString)
		{
			char c = RussianTextProcessor.GetLastCharacter(outputString);
			if (c == 'а' && !this.Neuter)
			{
				outputString.Remove(outputString.Length - 1, 1);
				c = RussianTextProcessor.GetLastCharacter(outputString);
				if (c == 'ж' || c == 'ш' || c == 'к' || c == 'щ' || c == 'ч' || c == 'г' || c == 'х')
				{
					outputString.Append('и');
					return;
				}
				outputString.Append('ы');
				return;
			}
			else
			{
				if (c == 'я' && !this.Neuter)
				{
					outputString.Remove(outputString.Length - 1, 1).Append('и');
					return;
				}
				if (!this.Neuter)
				{
					if (this.Feminine)
					{
						if (c == 'ь')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('и');
							return;
						}
					}
					else
					{
						if (c == 'ь')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('я');
							return;
						}
						if (c == 'й')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('я');
							return;
						}
						if (c == 'о')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('и');
							return;
						}
						if (c == 'е')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('я');
							return;
						}
						if (c == 'м')
						{
							outputString.Append('а');
							return;
						}
						if (this.MasculineAnimate && RussianTextProcessor.GetEnding(outputString, 2) == "ин")
						{
							outputString.Append('а');
							return;
						}
						if (this.IsConsonant(c))
						{
							if (RussianTextProcessor.GetEnding(outputString, 3) == "нок")
							{
								outputString.Remove(outputString.Length - 2, 2).Append("ка");
								return;
							}
							if (RussianTextProcessor.GetEnding(outputString, 2) == "ок" && outputString.ToString() != "сок")
							{
								outputString.Remove(outputString.Length - 2, 2).Append("ка");
								return;
							}
							if (RussianTextProcessor.GetEnding(outputString, 2) == "ек" && RussianTextProcessor.GetEnding(outputString, 2) == "ёк")
							{
								if (this.IsConsonant(RussianTextProcessor.GetEnding(outputString, 3)[0]))
								{
									outputString.Remove(outputString.Length - 2, 2).Append("ька");
									return;
								}
								outputString.Remove(outputString.Length - 2, 2).Append("йка");
								return;
							}
							else
							{
								outputString.Append('а');
							}
						}
					}
					return;
				}
				outputString.Remove(outputString.Length - 1, 1);
				if (c == 'о')
				{
					outputString.Append('а');
					return;
				}
				if (c == 'е')
				{
					c = RussianTextProcessor.GetLastCharacter(outputString);
					if (c == 'ч' || c == 'щ')
					{
						outputString.Append('а');
						return;
					}
					outputString.Append('я');
					return;
				}
				else
				{
					if (c == 'я' && RussianTextProcessor.GetLastCharacter(outputString) == 'м')
					{
						outputString.Append("ени");
						return;
					}
					outputString.Append(c);
					return;
				}
			}
		}

		private void AddSuffixNounGenitivePlural(StringBuilder outputString)
		{
			char c = RussianTextProcessor.GetLastCharacter(outputString);
			if (c == 'а' && !this.Neuter)
			{
				outputString.Remove(outputString.Length - 1, 1);
				c = RussianTextProcessor.GetLastCharacter(outputString);
				if (this.Masculine)
				{
					if (c == 'ж' || c == 'ш')
					{
						outputString.Append("ей");
						return;
					}
				}
				else if (c == 'к')
				{
					outputString.Remove(outputString.Length - 1, 1);
					c = RussianTextProcessor.GetLastCharacter(outputString);
					if (c == 'ч' || c == 'ц' || c == 'ш' || c == 'щ')
					{
						outputString.Append("ек");
						return;
					}
					if (c == 'ь')
					{
						outputString.Remove(outputString.Length - 1, 1);
						outputString.Append("ек");
						return;
					}
					outputString.Append("ок");
					return;
				}
			}
			else if (c == 'я' && !this.Neuter)
			{
				outputString.Remove(outputString.Length - 1, 1);
				c = RussianTextProcessor.GetLastCharacter(outputString);
				if (this.Masculine)
				{
					outputString.Append('ь');
					return;
				}
				if (c == 'л')
				{
					outputString.Remove(outputString.Length - 1, 1).Append("ель");
					return;
				}
				if (c == 'и')
				{
					outputString.Append('й');
					return;
				}
				outputString.Append('ь');
				return;
			}
			else if (this.Neuter)
			{
				outputString.Remove(outputString.Length - 1, 1);
				if (c == 'о')
				{
					c = RussianTextProcessor.GetLastCharacter(outputString);
					outputString.Remove(outputString.Length - 1, 1);
					if (c != 'н')
					{
						outputString.Append(c).Append("ов");
						return;
					}
					c = RussianTextProcessor.GetLastCharacter(outputString);
					if (c == 'к')
					{
						outputString.Append("он");
						return;
					}
					outputString.Append("ен");
					return;
				}
				else if (c == 'е')
				{
					c = RussianTextProcessor.GetLastCharacter(outputString);
					if (c == 'ь')
					{
						outputString.Remove(outputString.Length - 1, 1);
						outputString.Append("ий");
						return;
					}
					if (c == 'и')
					{
						outputString.Append('й');
						return;
					}
					if (c != 'щ')
					{
						outputString.Append("ей");
						return;
					}
				}
				else
				{
					if (c == 'я' && RussianTextProcessor.GetLastCharacter(outputString) == 'м')
					{
						outputString.Append("ен");
						return;
					}
					outputString.Append(c);
					return;
				}
			}
			else if (this.Feminine)
			{
				if (c == 'ь')
				{
					outputString.Remove(outputString.Length - 1, 1).Append("ей");
					return;
				}
			}
			else
			{
				if (c == 'ь')
				{
					outputString.Remove(outputString.Length - 1, 1).Append("ей");
					return;
				}
				if (c == 'й')
				{
					outputString.Remove(outputString.Length - 1, 1).Append("ев");
					return;
				}
				if (c == 'о')
				{
					outputString.Remove(outputString.Length - 1, 1);
					c = RussianTextProcessor.GetLastCharacter(outputString);
					if (c == 'к')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ек");
						return;
					}
				}
				else
				{
					if (c == 'е')
					{
						outputString.Append('в');
						return;
					}
					if (c == 'м')
					{
						outputString.Append("ов");
						return;
					}
					if (this.MasculineAnimate && RussianTextProcessor.GetEnding(outputString, 2) == "ин")
					{
						outputString.Remove(outputString.Length - 2, 2);
						return;
					}
					if (this.IsConsonant(c))
					{
						if (this.Animate && (RussianTextProcessor.GetEnding(outputString, 4) == "енок" || RussianTextProcessor.GetEnding(outputString, 4) == "ёнок"))
						{
							outputString.Remove(outputString.Length - 4, 4).Append("ят");
							return;
						}
						if (this.Animate && RussianTextProcessor.GetEnding(outputString, 4) == "онок")
						{
							outputString.Remove(outputString.Length - 4, 4).Append("ат");
							return;
						}
						if (RussianTextProcessor.GetEnding(outputString, 2) == "ок" && outputString.ToString() != "сок")
						{
							outputString.Remove(outputString.Length - 2, 2).Append("ков");
							return;
						}
						if (RussianTextProcessor.GetEnding(outputString, 2) == "ек" && RussianTextProcessor.GetEnding(outputString, 2) == "ёк")
						{
							if (this.IsConsonant(RussianTextProcessor.GetEnding(outputString, 3)[0]))
							{
								outputString.Remove(outputString.Length - 2, 2).Append("ьков");
								return;
							}
							outputString.Remove(outputString.Length - 2, 2).Append("йков");
							return;
						}
						else
						{
							if (RussianTextProcessor.GetEnding(outputString, 2) == "нц")
							{
								outputString.Append("ев");
								return;
							}
							if (c == 'ч' || c == 'ц' || c == 'ш' || c == 'щ')
							{
								outputString.Append("ей");
								return;
							}
							outputString.Append("ов");
						}
					}
				}
			}
		}

		private void AddSuffixNounDative(StringBuilder outputString)
		{
			char c = RussianTextProcessor.GetLastCharacter(outputString);
			if (c == 'а' && !this.Neuter)
			{
				outputString.Remove(outputString.Length - 1, 1).Append('е');
				return;
			}
			if (c == 'я' && !this.Neuter)
			{
				outputString.Remove(outputString.Length - 1, 1);
				c = RussianTextProcessor.GetLastCharacter(outputString);
				if (c == 'и')
				{
					outputString.Append('и');
					return;
				}
				outputString.Append('е');
				return;
			}
			else
			{
				if (!this.Neuter)
				{
					if (this.Feminine)
					{
						if (c == 'ь')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('и');
							return;
						}
					}
					else
					{
						if (c == 'ь')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('ю');
							return;
						}
						if (c == 'й')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('ю');
							return;
						}
						if (c == 'о')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('е');
							return;
						}
						if (c == 'е')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('ю');
							return;
						}
						if (c == 'м')
						{
							outputString.Append('у');
							return;
						}
						if (this.MasculineAnimate && RussianTextProcessor.GetEnding(outputString, 2) == "ин")
						{
							outputString.Append('у');
							return;
						}
						if (this.IsConsonant(c))
						{
							if (RussianTextProcessor.GetEnding(outputString, 3) == "нок")
							{
								outputString.Remove(outputString.Length - 2, 2).Append("ку");
								return;
							}
							if (RussianTextProcessor.GetEnding(outputString, 2) == "ок" && outputString.ToString() != "сок")
							{
								outputString.Remove(outputString.Length - 2, 2).Append("ку");
								return;
							}
							if (RussianTextProcessor.GetEnding(outputString, 2) == "ек" && RussianTextProcessor.GetEnding(outputString, 2) == "ёк")
							{
								if (this.IsConsonant(RussianTextProcessor.GetEnding(outputString, 3)[0]))
								{
									outputString.Remove(outputString.Length - 2, 2).Append("ьку");
									return;
								}
								outputString.Remove(outputString.Length - 2, 2).Append("йку");
								return;
							}
							else
							{
								outputString.Append('у');
							}
						}
					}
					return;
				}
				outputString.Remove(outputString.Length - 1, 1);
				if (c == 'о')
				{
					outputString.Append('у');
					return;
				}
				if (c == 'е')
				{
					c = RussianTextProcessor.GetLastCharacter(outputString);
					if (c == 'ч' || c == 'щ')
					{
						outputString.Append('у');
						return;
					}
					outputString.Append('ю');
					return;
				}
				else
				{
					if (c == 'я' && RussianTextProcessor.GetLastCharacter(outputString) == 'м')
					{
						outputString.Append("ени");
						return;
					}
					outputString.Append(c);
					return;
				}
			}
		}

		private void AddSuffixNounDativePlural(StringBuilder outputString)
		{
			char c = RussianTextProcessor.GetLastCharacter(outputString);
			if (c == 'а' && !this.Neuter)
			{
				outputString.Append('м');
				return;
			}
			if (c == 'я' && !this.Neuter)
			{
				outputString.Append('м');
				return;
			}
			if (!this.Neuter)
			{
				if (this.Feminine)
				{
					if (c == 'ь')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ям");
						return;
					}
				}
				else
				{
					if (c == 'ь')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ям");
						return;
					}
					if (c == 'й')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ям");
						return;
					}
					if (c == 'о')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ам");
						return;
					}
					if (c == 'е')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ям");
						return;
					}
					if (c == 'м')
					{
						outputString.Append("ам");
						return;
					}
					if (this.MasculineAnimate && RussianTextProcessor.GetEnding(outputString, 2) == "ин")
					{
						outputString.Remove(outputString.Length - 2, 2).Append("ам");
						return;
					}
					if (this.IsConsonant(c))
					{
						if (this.Animate && (RussianTextProcessor.GetEnding(outputString, 4) == "енок" || RussianTextProcessor.GetEnding(outputString, 4) == "ёнок"))
						{
							outputString.Remove(outputString.Length - 4, 4).Append("ятам");
							return;
						}
						if (this.Animate && RussianTextProcessor.GetEnding(outputString, 4) == "онок")
						{
							outputString.Remove(outputString.Length - 4, 4).Append("атам");
							return;
						}
						if (RussianTextProcessor.GetEnding(outputString, 2) == "ок" && outputString.ToString() != "сок")
						{
							outputString.Remove(outputString.Length - 2, 2).Append("кам");
							return;
						}
						if (RussianTextProcessor.GetEnding(outputString, 2) == "ек" && RussianTextProcessor.GetEnding(outputString, 2) == "ёк")
						{
							if (this.IsConsonant(RussianTextProcessor.GetEnding(outputString, 3)[0]))
							{
								outputString.Remove(outputString.Length - 2, 2).Append("ькам");
								return;
							}
							outputString.Remove(outputString.Length - 2, 2).Append("йкам");
							return;
						}
						else
						{
							outputString.Append("ам");
						}
					}
				}
				return;
			}
			outputString.Remove(outputString.Length - 1, 1);
			if (c == 'о')
			{
				outputString.Append("ам");
				return;
			}
			if (c == 'е')
			{
				c = RussianTextProcessor.GetLastCharacter(outputString);
				if (c == 'ч' || c == 'щ')
				{
					outputString.Append("ам");
					return;
				}
				outputString.Append("ям");
				return;
			}
			else
			{
				if (c == 'я' && RussianTextProcessor.GetLastCharacter(outputString) == 'м')
				{
					outputString.Append("енам");
					return;
				}
				outputString.Append(c);
				return;
			}
		}

		private void AddSuffixNounAccusative(StringBuilder outputString)
		{
			char lastCharacter = RussianTextProcessor.GetLastCharacter(outputString);
			if (lastCharacter == 'а' && !this.Neuter)
			{
				outputString.Remove(outputString.Length - 1, 1).Append('у');
				return;
			}
			if (lastCharacter == 'я' && !this.Neuter)
			{
				outputString.Remove(outputString.Length - 1, 1).Append('ю');
				return;
			}
			if (!this.Neuter && !this.Feminine && this.MasculineAnimate)
			{
				if (RussianTextProcessor.GetEnding(outputString, 3) == "нок")
				{
					outputString.Remove(outputString.Length - 2, 2).Append("ка");
					return;
				}
				if (RussianTextProcessor.GetEnding(outputString, 2) == "ок" && outputString.ToString() != "сок")
				{
					outputString.Remove(outputString.Length - 2, 2).Append("ка");
					return;
				}
				if (RussianTextProcessor.GetEnding(outputString, 2) == "ек" && RussianTextProcessor.GetEnding(outputString, 2) == "ёк")
				{
					if (this.IsConsonant(RussianTextProcessor.GetEnding(outputString, 3)[0]))
					{
						outputString.Remove(outputString.Length - 2, 2).Append("ьки");
						return;
					}
					outputString.Remove(outputString.Length - 2, 2).Append("йки");
					return;
				}
				else
				{
					this.AddSuffixNounGenitive(outputString);
				}
			}
		}

		private void AddSuffixNounAccusativePlural(StringBuilder outputString)
		{
			if (this.Animate)
			{
				this.AddSuffixNounGenitivePlural(outputString);
				return;
			}
			this.AddSuffixNounNominativePlural(outputString);
		}

		private void AddSuffixNounInstrumental(StringBuilder outputString)
		{
			char c = RussianTextProcessor.GetLastCharacter(outputString);
			if (c == 'а' && !this.Neuter)
			{
				outputString.Remove(outputString.Length - 1, 1);
				c = RussianTextProcessor.GetLastCharacter(outputString);
				if (c == 'ж' || c == 'ш' || c == 'щ' || c == 'ч')
				{
					outputString.Append("ей");
					return;
				}
				outputString.Append("ой");
				return;
			}
			else
			{
				if (c == 'я' && !this.Neuter)
				{
					outputString.Remove(outputString.Length - 1, 1).Append("ей");
					return;
				}
				if (this.Neuter)
				{
					if (c == 'о' || c == 'е')
					{
						outputString.Append('м');
						return;
					}
					if (RussianTextProcessor.GetEnding(outputString, 2) == "мя")
					{
						outputString.Remove(outputString.Length - 1, 1).Append("енем");
						return;
					}
				}
				else if (this.Feminine)
				{
					if (c == 'ь')
					{
						outputString.Append('ю');
						return;
					}
				}
				else
				{
					if (c == 'ь')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ем");
						return;
					}
					if (c == 'й')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ем");
						return;
					}
					if (c == 'о')
					{
						outputString.Append('м');
						return;
					}
					if (c == 'е')
					{
						outputString.Append('м');
						return;
					}
					if (c == 'м')
					{
						outputString.Append("ом");
						return;
					}
					if (this.MasculineAnimate && RussianTextProcessor.GetEnding(outputString, 2) == "ин")
					{
						outputString.Append("ом");
						return;
					}
					if (this.IsConsonant(c))
					{
						if (RussianTextProcessor.GetEnding(outputString, 3) == "нок")
						{
							outputString.Remove(outputString.Length - 2, 2).Append("ком");
							return;
						}
						if (RussianTextProcessor.GetEnding(outputString, 2) == "ок" && outputString.ToString() != "сок")
						{
							outputString.Remove(outputString.Length - 2, 2).Append("ком");
							return;
						}
						if (RussianTextProcessor.GetEnding(outputString, 2) == "ек" && RussianTextProcessor.GetEnding(outputString, 2) == "ёк")
						{
							if (this.IsConsonant(RussianTextProcessor.GetEnding(outputString, 3)[0]))
							{
								outputString.Remove(outputString.Length - 2, 2).Append("ьком");
								return;
							}
							outputString.Remove(outputString.Length - 2, 2).Append("йком");
							return;
						}
						else
						{
							outputString.Append("ом");
						}
					}
				}
				return;
			}
		}

		private void AddSuffixNounInstrumentalPlural(StringBuilder outputString)
		{
			char c = RussianTextProcessor.GetLastCharacter(outputString);
			if (c == 'а' && !this.Neuter)
			{
				outputString.Append("ми");
				return;
			}
			if (c == 'я' && !this.Neuter)
			{
				outputString.Append("ми");
				return;
			}
			if (!this.Neuter)
			{
				if (this.Feminine)
				{
					if (c == 'ь')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ями");
						return;
					}
				}
				else
				{
					if (c == 'ь')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ями");
						return;
					}
					if (c == 'й')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ями");
						return;
					}
					if (c == 'о')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ами");
						return;
					}
					if (c == 'е')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ями");
						return;
					}
					if (c == 'м')
					{
						outputString.Append("ами");
						return;
					}
					if (this.MasculineAnimate && RussianTextProcessor.GetEnding(outputString, 2) == "ин")
					{
						outputString.Remove(outputString.Length - 2, 2).Append("ами");
						return;
					}
					if (this.IsConsonant(c))
					{
						if (this.Animate && (RussianTextProcessor.GetEnding(outputString, 4) == "енок" || RussianTextProcessor.GetEnding(outputString, 4) == "ёнок"))
						{
							outputString.Remove(outputString.Length - 4, 4).Append("ятами");
							return;
						}
						if (this.Animate && RussianTextProcessor.GetEnding(outputString, 4) == "онок")
						{
							outputString.Remove(outputString.Length - 4, 4).Append("атами");
							return;
						}
						if (RussianTextProcessor.GetEnding(outputString, 2) == "ок" && outputString.ToString() != "сок")
						{
							outputString.Remove(outputString.Length - 2, 2).Append("ками");
							return;
						}
						if (RussianTextProcessor.GetEnding(outputString, 2) == "ек" && RussianTextProcessor.GetEnding(outputString, 2) == "ёк")
						{
							if (this.IsConsonant(RussianTextProcessor.GetEnding(outputString, 3)[0]))
							{
								outputString.Remove(outputString.Length - 2, 2).Append("ьками");
								return;
							}
							outputString.Remove(outputString.Length - 2, 2).Append("йками");
							return;
						}
						else
						{
							outputString.Append("ами");
						}
					}
				}
				return;
			}
			outputString.Remove(outputString.Length - 1, 1);
			if (c == 'о')
			{
				outputString.Append("ами");
				return;
			}
			if (c == 'е')
			{
				c = RussianTextProcessor.GetLastCharacter(outputString);
				if (c == 'ч' || c == 'щ')
				{
					outputString.Append("ами");
					return;
				}
				outputString.Append("ями");
				return;
			}
			else
			{
				if (c == 'я' && RussianTextProcessor.GetLastCharacter(outputString) == 'м')
				{
					outputString.Append("енами");
					return;
				}
				outputString.Append(c);
				return;
			}
		}

		private void AddSuffixNounLocative(StringBuilder outputString)
		{
			char c = RussianTextProcessor.GetLastCharacter(outputString);
			if (c == 'а' && !this.Neuter)
			{
				outputString.Remove(outputString.Length - 1, 1).Append('е');
				return;
			}
			if (c == 'я' && !this.Neuter)
			{
				outputString.Remove(outputString.Length - 1, 1);
				c = RussianTextProcessor.GetLastCharacter(outputString);
				if (c == 'и')
				{
					outputString.Append('и');
					return;
				}
				outputString.Append('е');
				return;
			}
			else
			{
				if (!this.Neuter)
				{
					if (this.Feminine)
					{
						if (c == 'ь')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('и');
							return;
						}
					}
					else
					{
						if (c == 'ь')
						{
							outputString.Remove(outputString.Length - 1, 1).Append('е');
							return;
						}
						if (c == 'й')
						{
							outputString.Remove(outputString.Length - 1, 1);
							c = RussianTextProcessor.GetLastCharacter(outputString);
							if (c == 'и')
							{
								outputString.Append('и');
								return;
							}
							outputString.Append('е');
							return;
						}
						else
						{
							if (c == 'о')
							{
								outputString.Remove(outputString.Length - 1, 1).Append('е');
								return;
							}
							if (c != 'е')
							{
								if (c == 'м')
								{
									outputString.Append('е');
									return;
								}
								if (this.MasculineAnimate && RussianTextProcessor.GetEnding(outputString, 2) == "ин")
								{
									outputString.Append('е');
									return;
								}
								if (this.IsConsonant(c))
								{
									if (RussianTextProcessor.GetEnding(outputString, 3) == "нок")
									{
										outputString.Remove(outputString.Length - 2, 2).Append("ке");
										return;
									}
									if (RussianTextProcessor.GetEnding(outputString, 2) == "ок" && outputString.ToString() != "сок")
									{
										outputString.Remove(outputString.Length - 2, 2).Append("ке");
										return;
									}
									if (RussianTextProcessor.GetEnding(outputString, 2) == "ек" && RussianTextProcessor.GetEnding(outputString, 2) == "ёк")
									{
										if (this.IsConsonant(RussianTextProcessor.GetEnding(outputString, 3)[0]))
										{
											outputString.Remove(outputString.Length - 2, 2).Append("ьке");
											return;
										}
										outputString.Remove(outputString.Length - 2, 2).Append("йке");
										return;
									}
									else
									{
										outputString.Append('е');
									}
								}
							}
						}
					}
					return;
				}
				outputString.Remove(outputString.Length - 1, 1);
				if (c == 'о')
				{
					outputString.Append('е');
					return;
				}
				if (c == 'е')
				{
					c = RussianTextProcessor.GetLastCharacter(outputString);
					if (c == 'и')
					{
						outputString.Append('и');
						return;
					}
					outputString.Append('е');
					return;
				}
				else
				{
					if (c == 'я' && RussianTextProcessor.GetLastCharacter(outputString) == 'м')
					{
						outputString.Append("ени");
						return;
					}
					outputString.Append(c);
					return;
				}
			}
		}

		private void AddSuffixNounLocativePlural(StringBuilder outputString)
		{
			char c = RussianTextProcessor.GetLastCharacter(outputString);
			if (c == 'а' && !this.Neuter)
			{
				outputString.Append('х');
				return;
			}
			if (c == 'я' && !this.Neuter)
			{
				outputString.Append('х');
				return;
			}
			if (!this.Neuter)
			{
				if (this.Feminine)
				{
					if (c == 'ь')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ях");
						return;
					}
				}
				else
				{
					if (c == 'ь')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ях");
						return;
					}
					if (c == 'й')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ях");
						return;
					}
					if (c == 'о')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ах");
						return;
					}
					if (c == 'е')
					{
						outputString.Remove(outputString.Length - 1, 1).Append("ях");
						return;
					}
					if (c == 'м')
					{
						outputString.Append("ах");
						return;
					}
					if (this.MasculineAnimate && RussianTextProcessor.GetEnding(outputString, 2) == "ин")
					{
						outputString.Remove(outputString.Length - 2, 2).Append("ах");
						return;
					}
					if (this.IsConsonant(c))
					{
						if (this.Animate && (RussianTextProcessor.GetEnding(outputString, 4) == "енок" || RussianTextProcessor.GetEnding(outputString, 4) == "ёнок"))
						{
							outputString.Remove(outputString.Length - 4, 4).Append("ятах");
							return;
						}
						if (this.Animate && RussianTextProcessor.GetEnding(outputString, 4) == "онок")
						{
							outputString.Remove(outputString.Length - 4, 4).Append("атах");
							return;
						}
						if (RussianTextProcessor.GetEnding(outputString, 2) == "ок" && outputString.ToString() != "сок")
						{
							outputString.Remove(outputString.Length - 2, 2).Append("ках");
							return;
						}
						if (RussianTextProcessor.GetEnding(outputString, 2) == "ек" && RussianTextProcessor.GetEnding(outputString, 2) == "ёк")
						{
							if (this.IsConsonant(RussianTextProcessor.GetEnding(outputString, 3)[0]))
							{
								outputString.Remove(outputString.Length - 2, 2).Append("ьках");
								return;
							}
							outputString.Remove(outputString.Length - 2, 2).Append("йках");
							return;
						}
						else
						{
							outputString.Append("ах");
						}
					}
				}
				return;
			}
			outputString.Remove(outputString.Length - 1, 1);
			if (c == 'о')
			{
				outputString.Append("ах");
				return;
			}
			if (c == 'е')
			{
				c = RussianTextProcessor.GetLastCharacter(outputString);
				if (c == 'ч' || c == 'щ')
				{
					outputString.Append("ах");
					return;
				}
				outputString.Append("ях");
				return;
			}
			else
			{
				if (c == 'я' && RussianTextProcessor.GetLastCharacter(outputString) == 'м')
				{
					outputString.Append("енах");
					return;
				}
				outputString.Append(c);
				return;
			}
		}

		private void AddSuffixAdjectiveNominative(StringBuilder outputString)
		{
			string ending = RussianTextProcessor.GetEnding(outputString, 2);
			outputString.Remove(outputString.Length - 2, 2);
			if (ending == "ый")
			{
				if (this.Feminine)
				{
					outputString.Append("ая");
					return;
				}
				if (this.Neuter)
				{
					outputString.Append("ое");
					return;
				}
				outputString.Append("ый");
				return;
			}
			else
			{
				if (!(ending == "ой"))
				{
					if (ending == "ий")
					{
						char lastCharacter = RussianTextProcessor.GetLastCharacter(outputString);
						if (this.Feminine)
						{
							if (this.IsVelarOrSibilant(lastCharacter))
							{
								outputString.Append("ая");
								return;
							}
							outputString.Append("яя");
							return;
						}
						else if (this.Neuter)
						{
							if (this.IsVelarOrSibilant(lastCharacter))
							{
								outputString.Append("ое");
								return;
							}
							outputString.Append("ее");
							return;
						}
						else
						{
							outputString.Append("ий");
						}
					}
					return;
				}
				if (this.Feminine)
				{
					outputString.Append("ая");
					return;
				}
				if (this.Neuter)
				{
					outputString.Append("ое");
					return;
				}
				outputString.Append("ой");
				return;
			}
		}

		private void AddSuffixAdjectiveNominativePlural(StringBuilder outputString)
		{
			string ending = RussianTextProcessor.GetEnding(outputString, 2);
			outputString.Remove(outputString.Length - 2, 2);
			string ending2 = RussianTextProcessor.GetEnding(outputString, 1);
			if (this.IsVelarOrSibilant(ending2[0]) || (ending == "ий" && this.IsSoftStemAdjective(ending2[0])))
			{
				outputString.Append("ие");
				return;
			}
			outputString.Append("ые");
		}

		private void AddSuffixAdjectiveGenitive(StringBuilder outputString)
		{
			string ending = RussianTextProcessor.GetEnding(outputString, 2);
			outputString.Remove(outputString.Length - 2, 2);
			if (!(ending == "ый") && !(ending == "ой"))
			{
				if (ending == "ий")
				{
					char lastCharacter = RussianTextProcessor.GetLastCharacter(outputString);
					if (this.Feminine)
					{
						if (lastCharacter == 'к' || lastCharacter == 'г' || lastCharacter == 'х')
						{
							outputString.Append("ой");
							return;
						}
						outputString.Append("ей");
						return;
					}
					else
					{
						if (lastCharacter == 'к' || lastCharacter == 'г' || lastCharacter == 'х')
						{
							outputString.Append("ого");
							return;
						}
						outputString.Append("его");
					}
				}
				return;
			}
			if (this.Feminine)
			{
				outputString.Append("ой");
				return;
			}
			outputString.Append("ого");
		}

		private void AddSuffixAdjectiveGenitivePlural(StringBuilder outputString)
		{
			string ending = RussianTextProcessor.GetEnding(outputString, 2);
			if (ending == "ый" || ending == "ой")
			{
				outputString.Remove(outputString.Length - 2, 2).Append("ых");
				return;
			}
			if (ending == "ий")
			{
				outputString.Remove(outputString.Length - 2, 2).Append("их");
			}
		}

		private void AddSuffixAdjectiveDative(StringBuilder outputString)
		{
			string ending = RussianTextProcessor.GetEnding(outputString, 2);
			outputString.Remove(outputString.Length - 2, 2);
			if (!(ending == "ый") && !(ending == "ой"))
			{
				if (ending == "ий")
				{
					char lastCharacter = RussianTextProcessor.GetLastCharacter(outputString);
					if (this.Feminine)
					{
						if (lastCharacter == 'к' || lastCharacter == 'г' || lastCharacter == 'х')
						{
							outputString.Append("ой");
							return;
						}
						outputString.Append("ей");
						return;
					}
					else
					{
						if (lastCharacter == 'к' || lastCharacter == 'г' || lastCharacter == 'х')
						{
							outputString.Append("ому");
							return;
						}
						outputString.Append("ему");
					}
				}
				return;
			}
			if (this.Feminine)
			{
				outputString.Append("ой");
				return;
			}
			outputString.Append("ому");
		}

		private void AddSuffixAdjectiveDativePlural(StringBuilder outputString)
		{
			string ending = RussianTextProcessor.GetEnding(outputString, 2);
			if (ending == "ый" || ending == "ой")
			{
				outputString.Remove(outputString.Length - 2, 2).Append("ым");
				return;
			}
			if (ending == "ий")
			{
				outputString.Remove(outputString.Length - 2, 2).Append("им");
			}
		}

		private void AddSuffixAdjectiveAccusative(StringBuilder outputString)
		{
			if (this.Feminine)
			{
				string ending = RussianTextProcessor.GetEnding(outputString, 2);
				outputString.Remove(outputString.Length - 2, 2);
				if (ending == "ый" || ending == "ой")
				{
					outputString.Append("ую");
					return;
				}
				if (ending == "ий")
				{
					char lastCharacter = RussianTextProcessor.GetLastCharacter(outputString);
					if (lastCharacter == 'к' || lastCharacter == 'г' || lastCharacter == 'х')
					{
						outputString.Append("ую");
						return;
					}
					outputString.Append("юю");
					return;
				}
			}
			else
			{
				if (this.MasculineAnimate)
				{
					this.AddSuffixAdjectiveGenitive(outputString);
					return;
				}
				this.AddSuffixAdjectiveNominative(outputString);
			}
		}

		private void AddSuffixAdjectiveAccusativePlural(StringBuilder outputString)
		{
			if (this.Animate)
			{
				this.AddSuffixAdjectiveGenitivePlural(outputString);
				return;
			}
			this.AddSuffixAdjectiveNominativePlural(outputString);
		}

		private void AddSuffixAdjectiveInstrumental(StringBuilder outputString)
		{
			string ending = RussianTextProcessor.GetEnding(outputString, 2);
			outputString.Remove(outputString.Length - 2, 2);
			if (!(ending == "ый") && !(ending == "ой"))
			{
				if (ending == "ий")
				{
					char lastCharacter = RussianTextProcessor.GetLastCharacter(outputString);
					if (this.Feminine)
					{
						if (lastCharacter == 'к' || lastCharacter == 'г' || lastCharacter == 'х')
						{
							outputString.Append("ой");
							return;
						}
						outputString.Append("ей");
						return;
					}
					else
					{
						if (lastCharacter == 'к' || lastCharacter == 'г' || lastCharacter == 'х')
						{
							outputString.Append("им");
							return;
						}
						outputString.Append("им");
					}
				}
				return;
			}
			if (this.Feminine)
			{
				outputString.Append("ой");
				return;
			}
			outputString.Append("ым");
		}

		private void AddSuffixAdjectiveInstrumentalPlural(StringBuilder outputString)
		{
			string ending = RussianTextProcessor.GetEnding(outputString, 2);
			if (ending == "ый" || ending == "ой")
			{
				outputString.Remove(outputString.Length - 2, 2).Append("ыми");
				return;
			}
			if (ending == "ий")
			{
				outputString.Remove(outputString.Length - 2, 2).Append("ими");
			}
		}

		private void AddSuffixAdjectiveLocative(StringBuilder outputString)
		{
			string ending = RussianTextProcessor.GetEnding(outputString, 2);
			outputString.Remove(outputString.Length - 2, 2);
			if (!(ending == "ый") && !(ending == "ой"))
			{
				if (ending == "ий")
				{
					char lastCharacter = RussianTextProcessor.GetLastCharacter(outputString);
					if (this.Feminine)
					{
						if (lastCharacter == 'к' || lastCharacter == 'г' || lastCharacter == 'х')
						{
							outputString.Append("ой");
							return;
						}
						outputString.Append("ей");
						return;
					}
					else
					{
						if (lastCharacter == 'к' || lastCharacter == 'г' || lastCharacter == 'х')
						{
							outputString.Append("ом");
							return;
						}
						outputString.Append("ем");
					}
				}
				return;
			}
			if (this.Feminine)
			{
				outputString.Append("ой");
				return;
			}
			outputString.Append("ом");
		}

		private void AddSuffixAdjectiveLocativePlural(StringBuilder outputString)
		{
			string ending = RussianTextProcessor.GetEnding(outputString, 2);
			if (ending == "ый" || ending == "ой")
			{
				outputString.Remove(outputString.Length - 2, 2).Append("ых");
				return;
			}
			if (ending == "ий")
			{
				outputString.Remove(outputString.Length - 2, 2).Append("их");
			}
		}

		private void SetFeminineAnimate()
		{
			RussianTextProcessor._curGender = RussianTextProcessor.WordGenderEnum.FeminineAnimate;
		}

		private void SetFeminineInanimate()
		{
			RussianTextProcessor._curGender = RussianTextProcessor.WordGenderEnum.FeminineInanimate;
		}

		private void SetNeuterInanimate()
		{
			RussianTextProcessor._curGender = RussianTextProcessor.WordGenderEnum.NeuterInanimate;
		}

		private void SetNeuterAnimate()
		{
			RussianTextProcessor._curGender = RussianTextProcessor.WordGenderEnum.NeuterAnimate;
		}

		private void SetMasculineAnimate()
		{
			RussianTextProcessor._curGender = RussianTextProcessor.WordGenderEnum.MasculineAnimate;
		}

		private void SetMasculineInanimate()
		{
			RussianTextProcessor._curGender = RussianTextProcessor.WordGenderEnum.MasculineInanimate;
		}

		private bool IsRecordedWithPreviousTag(string sourceText, int cursorPos)
		{
			for (int i = 0; i < RussianTextProcessor.WordGroups.Count; i++)
			{
				if (RussianTextProcessor.WordGroups[i].Item1 == sourceText && RussianTextProcessor.WordGroups[i].Item2 != cursorPos)
				{
					return true;
				}
			}
			return false;
		}

		private void WordGroupProcessor(string sourceText, int cursorPos)
		{
			if (!this.IsRecordedWithPreviousTag(sourceText, cursorPos))
			{
				RussianTextProcessor.WordGroups.Add(new ValueTuple<string, int>(sourceText, cursorPos));
				string text = sourceText.Replace("{.nnp}", "{.p}");
				text = text.Replace("{.ajp}", "{.jp}");
				text = text.Replace("{.nn}", "{.n}");
				text = text.Replace("{.aj}", "{.j}");
				RussianTextProcessor._doesComeFromWordGroup = true;
				RussianTextProcessor.WordGroupsNoTags.Add(base.Process(text));
				RussianTextProcessor._doesComeFromWordGroup = false;
			}
		}

		private bool IsLink(string sourceText, int tokenLength, int cursorPos)
		{
			string text = sourceText.Remove(cursorPos - tokenLength);
			for (int i = 0; i < RussianTextProcessor.LinkList.Count; i++)
			{
				if (sourceText.Length >= RussianTextProcessor.LinkList[i].Length && text.EndsWith(RussianTextProcessor.LinkList[i]))
				{
					RussianTextProcessor.LinkList.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		private bool IsIrregularWord(string sourceText, int cursorPos, string token, out string irregularWord, out int lengthOfWordToReplace)
		{
			int num = sourceText.Remove(cursorPos - token.Length - 2).LastIndexOf('}') + 1;
			lengthOfWordToReplace = cursorPos - token.Length - 2 - num;
			irregularWord = "";
			bool flag = false;
			if (lengthOfWordToReplace > 0)
			{
				string text = sourceText.Substring(num, lengthOfWordToReplace);
				flag = char.IsLower(text[0]);
				text = text.ToLowerInvariant();
				if (RussianTextProcessor.exceptions.ContainsKey(text) && RussianTextProcessor.exceptions[text] != null && RussianTextProcessor.exceptions[text].ContainsKey(token))
				{
					irregularWord = RussianTextProcessor.exceptions[text][token];
				}
			}
			if (irregularWord.Length > 0)
			{
				if (flag)
				{
					irregularWord = irregularWord.ToLower();
				}
				return true;
			}
			return false;
		}

		private bool IsVowel(char c)
		{
			return Array.IndexOf<char>(RussianTextProcessor._vowels, c) >= 0;
		}

		private bool IsVelarOrSibilant(char c)
		{
			return Array.IndexOf<char>(RussianTextProcessor._sibilants, c) >= 0 || Array.IndexOf<char>(RussianTextProcessor._velars, c) >= 0;
		}

		private bool IsSoftStemAdjective(char c)
		{
			return c == 'н';
		}

		private bool IsConsonant(char c)
		{
			return "БбВвГгДдЖжЗзЙйКкЛлМмНнПпРрСсТтФфХхЦцЧчШшЩщЬьЪъ".IndexOf(c) >= 0;
		}

		private static string IrregularWordWithCase(string token, RussianTextProcessor.IrregularWord irregularWord)
		{
			if (token == ".j")
			{
				return irregularWord.Nominative;
			}
			if (token == ".jp" || token == ".p" || token == ".nnp" || token == ".ajp")
			{
				return irregularWord.NominativePlural;
			}
			if (token == ".ja" || token == ".a")
			{
				return irregularWord.Accusative;
			}
			if (token == ".jap" || token == ".ap")
			{
				return irregularWord.AccusativePlural;
			}
			if (token == ".jg" || token == ".g")
			{
				return irregularWord.Genitive;
			}
			if (token == ".jgp" || token == ".gp")
			{
				return irregularWord.GenitivePlural;
			}
			if (token == ".jd" || token == ".d")
			{
				return irregularWord.Dative;
			}
			if (token == ".jdp" || token == ".dp")
			{
				return irregularWord.DativePlural;
			}
			if (token == ".jl" || token == ".l")
			{
				return irregularWord.Locative;
			}
			if (token == ".jlp" || token == ".lp")
			{
				return irregularWord.LocativePlural;
			}
			if (token == ".ji" || token == ".i")
			{
				return irregularWord.Instrumental;
			}
			if (token == ".jip" || token == ".ip")
			{
				return irregularWord.InstrumentalPlural;
			}
			return "";
		}

		public string PrepareNounCheckString(string noun)
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "PrepareNounCheckString");
			mbstringBuilder.Append<string>("\"Есть ").Append<string>(base.Process(noun)).Append('"')
				.Append<string>(",\"Нет ")
				.Append<string>(base.Process(noun + "{.g}"))
				.Append('"')
				.Append<string>(",\"Рад ")
				.Append<string>(base.Process(noun + "{.d}"))
				.Append('"')
				.Append<string>(",\"Вижу ")
				.Append<string>(base.Process(noun + "{.a}"))
				.Append('"')
				.Append<string>(",\"Доволен ")
				.Append<string>(base.Process(noun + "{.i}"))
				.Append('"')
				.Append<string>(",\"Думаю о ")
				.Append<string>(base.Process(noun + "{.l}"))
				.Append('"')
				.Append<string>(",\"Есть ")
				.Append<string>(base.Process(noun + "{.p}"))
				.Append('"')
				.Append<string>(",\"Нет ")
				.Append<string>(base.Process(noun + "{.gp}"))
				.Append('"')
				.Append<string>(",\"Рад ")
				.Append<string>(base.Process(noun + "{.dp}"))
				.Append('"')
				.Append<string>(",\"Вижу ")
				.Append<string>(base.Process(noun + "{.ap}"))
				.Append('"')
				.Append<string>(",\"Доволен ")
				.Append<string>(base.Process(noun + "{.ip}"))
				.Append('"')
				.Append<string>(",\"Думаю о ")
				.Append<string>(base.Process(noun + "{.lp}"))
				.Append('"');
			return mbstringBuilder.ToStringAndRelease();
		}

		public string PrepareAdjectiveCheckString(string adj)
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "PrepareAdjectiveCheckString");
			mbstringBuilder.Append<string>("\"Есть ").Append<string>(base.Process("{.MI}" + adj + "{.j} {.MI}меч{.n}")).Append('"')
				.Append<string>(",\"Нет ")
				.Append<string>(base.Process("{.MI}" + adj + "{.jg} {.MI}меч{.g}"))
				.Append('"')
				.Append<string>(",\"Рад ")
				.Append<string>(base.Process("{.MI}" + adj + "{.jd} {.MI}меч{.d}"))
				.Append('"')
				.Append<string>(",\"Вижу ")
				.Append<string>(base.Process("{.MI}" + adj + "{.ja} {.MI}меч{.a}"))
				.Append('"')
				.Append<string>(",\"Доволен ")
				.Append<string>(base.Process("{.MI}" + adj + "{.ji} {.MI}меч{.i}"))
				.Append('"')
				.Append<string>(",\"Думаю о ")
				.Append<string>(base.Process("{.MI}" + adj + "{.jl} {.MI}меч{.l}"))
				.Append('"')
				.Append<string>(",\"Есть ")
				.Append<string>(base.Process("{.MA}" + adj + "{.j} {.MA}юноша{.n}"))
				.Append('"')
				.Append<string>(",\"Вижу ")
				.Append<string>(base.Process("{.MA}" + adj + "{.ja} {.MA}юноша{.a}"))
				.Append('"')
				.Append<string>(",\"Есть ")
				.Append<string>(base.Process("{.FI}" + adj + "{.j} {.FI}доска"))
				.Append('"')
				.Append<string>(",\"Нет ")
				.Append<string>(base.Process("{.FI}" + adj + "{.jg} {.FI}доска{.g}"))
				.Append('"')
				.Append<string>(",\"Рад ")
				.Append<string>(base.Process("{.FI}" + adj + "{.jd} {.FI}доска{.d}"))
				.Append('"')
				.Append<string>(",\"Вижу ")
				.Append<string>(base.Process("{.FI}" + adj + "{.ja} {.FI}доска{.a}"))
				.Append('"')
				.Append<string>(",\"Доволен ")
				.Append<string>(base.Process("{.FI}" + adj + "{.ji} {.FI}доска{.i}"))
				.Append('"')
				.Append<string>(",\"Думаю о ")
				.Append<string>(base.Process("{.FI}" + adj + "{.jl} {.FI}доска{.l}"))
				.Append('"')
				.Append<string>(",\"Есть ")
				.Append<string>(base.Process("{.FA}" + adj + "{.j} {.FA}девушка{.n}"))
				.Append('"')
				.Append<string>(",\"Вижу ")
				.Append<string>(base.Process("{.FA}" + adj + "{.ja} {.FA}девушка{.a}"))
				.Append('"')
				.Append<string>(",\"Есть ")
				.Append<string>(base.Process("{.NI}" + adj + "{.j} {.NI}бревно{.n}"))
				.Append('"')
				.Append<string>(",\"Нет ")
				.Append<string>(base.Process("{.NI}" + adj + "{.jg} {.NI}бревно{.g}"))
				.Append('"')
				.Append<string>(",\"Рад ")
				.Append<string>(base.Process("{.NI}" + adj + "{.jd} {.NI}бревно{.d}"))
				.Append('"')
				.Append<string>(",\"Вижу ")
				.Append<string>(base.Process("{.NI}" + adj + "{.ja} {.NI}бревно{.a}"))
				.Append('"')
				.Append<string>(",\"Доволен ")
				.Append<string>(base.Process("{.NI}" + adj + "{.ji} {.NI}бревно{.i}"))
				.Append('"')
				.Append<string>(",\"Думаю о ")
				.Append<string>(base.Process("{.NI}" + adj + "{.jl} {.NI}бревно{.l}"))
				.Append('"')
				.Append<string>(",\"Есть ")
				.Append<string>(base.Process("{.MI}" + adj + "{.jp} {.MI}меч{.p}"))
				.Append('"')
				.Append<string>(",\"Нет ")
				.Append<string>(base.Process("{.MI}" + adj + "{.jgp} {.MI}меч{.gp}"))
				.Append('"')
				.Append<string>(",\"Рад ")
				.Append<string>(base.Process("{.MI}" + adj + "{.jdp} {.MI}меч{.dp}"))
				.Append('"')
				.Append<string>(",\"Вижу ")
				.Append<string>(base.Process("{.MI}" + adj + "{.jap} {.MI}меч{.ap}"))
				.Append('"')
				.Append<string>(",\"Доволен ")
				.Append<string>(base.Process("{.MI}" + adj + "{.jip} {.MI}меч{.ip}"))
				.Append('"')
				.Append<string>(",\"Думаю о ")
				.Append<string>(base.Process("{.MI}" + adj + "{.jlp} {.MI}меч{.lp}"))
				.Append('"')
				.Append<string>(",\"Есть ")
				.Append<string>(base.Process("{.FA}" + adj + "{.jp} {.FA}девушка{.p}"))
				.Append('"')
				.Append<string>(",\"Вижу ")
				.Append<string>(base.Process("{.FA}" + adj + "{.jap} {.FA}девушка{.ap}"))
				.Append('"');
			return mbstringBuilder.ToStringAndRelease();
		}

		public static string[] GetProcessedNouns(string str, string gender, string[] tokens = null)
		{
			if (tokens == null)
			{
				tokens = new string[]
				{
					".n", ".g", ".d", ".a", ".i", ".l", ".p", ".gp", ".dp", ".ap",
					".ip", ".lp"
				};
			}
			List<string> list = new List<string>();
			RussianTextProcessor russianTextProcessor = new RussianTextProcessor();
			foreach (string text in tokens)
			{
				string text2 = string.Concat(new string[] { "{=!}", gender, str, "{", text, "}" });
				list.Add(russianTextProcessor.Process(text2));
			}
			return list.ToArray();
		}

		private static readonly CultureInfo CultureInfo = new CultureInfo("ru-RU");

		[ThreadStatic]
		private static RussianTextProcessor.WordGenderEnum _curGender;

		[TupleElementNames(new string[] { "wordGroup", "firstMarkerPost" })]
		[ThreadStatic]
		private static List<ValueTuple<string, int>> _wordGroups = new List<ValueTuple<string, int>>();

		[ThreadStatic]
		private static List<string> _wordGroupsNoTags = new List<string>();

		[ThreadStatic]
		private static List<string> _linkList = new List<string>();

		[ThreadStatic]
		private static bool _doesComeFromWordGroup = false;

		private static readonly char[] _vowels = new char[] { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я' };

		private static readonly char[] _sibilants = new char[] { 'ж', 'ч', 'ш', 'щ' };

		private static readonly char[] _velars = new char[] { 'г', 'к', 'х' };

		private static readonly string[] _nounTokens = new string[]
		{
			".n", ".p", ".g", ".gp", ".d", ".dp", ".a", ".ap", ".i", ".ip",
			".l", ".lp"
		};

		private static readonly Dictionary<char, List<RussianTextProcessor.IrregularWord>> _irregularMasculineAnimateDictionary = new Dictionary<char, List<RussianTextProcessor.IrregularWord>> { 
		{
			'A',
			new List<RussianTextProcessor.IrregularWord>
			{
				new RussianTextProcessor.IrregularWord("Alary", "Alary", "Alarego", "Alarego", "Alaremu", "Alaremu", "Alarego", "Alarego", "Alarym", "Alarym", "Alarym", "Alarym")
			}
		} };

		private static readonly Dictionary<char, List<RussianTextProcessor.IrregularWord>> _irregularMasculineInanimateDictionary = new Dictionary<char, List<RussianTextProcessor.IrregularWord>> { 
		{
			'A',
			new List<RussianTextProcessor.IrregularWord>
			{
				new RussianTextProcessor.IrregularWord("Alary", "Alary", "Alarego", "Alarego", "Alaremu", "Alaremu", "Alarego", "Alarego", "Alarym", "Alarym", "Alarym", "Alarym")
			}
		} };

		private static readonly Dictionary<char, List<RussianTextProcessor.IrregularWord>> _irregularFeminineAnimateDictionary = new Dictionary<char, List<RussianTextProcessor.IrregularWord>> { 
		{
			'A',
			new List<RussianTextProcessor.IrregularWord>
			{
				new RussianTextProcessor.IrregularWord("Alary", "Alary", "Alarego", "Alarego", "Alaremu", "Alaremu", "Alarego", "Alarego", "Alarym", "Alarym", "Alarym", "Alarym")
			}
		} };

		private static readonly Dictionary<char, List<RussianTextProcessor.IrregularWord>> _irregularFeminineInanimateDictionary = new Dictionary<char, List<RussianTextProcessor.IrregularWord>> { 
		{
			'A',
			new List<RussianTextProcessor.IrregularWord>
			{
				new RussianTextProcessor.IrregularWord("Alary", "Alary", "Alarego", "Alarego", "Alaremu", "Alaremu", "Alarego", "Alarego", "Alarym", "Alarym", "Alarym", "Alarym")
			}
		} };

		private static readonly Dictionary<char, List<RussianTextProcessor.IrregularWord>> _irregularNeuterAnimateDictionary = new Dictionary<char, List<RussianTextProcessor.IrregularWord>> { 
		{
			'A',
			new List<RussianTextProcessor.IrregularWord>
			{
				new RussianTextProcessor.IrregularWord("Alary", "Alary", "Alarego", "Alarego", "Alaremu", "Alaremu", "Alarego", "Alarego", "Alarym", "Alarym", "Alarym", "Alarym")
			}
		} };

		private static readonly Dictionary<char, List<RussianTextProcessor.IrregularWord>> _irregularNeuterInanimateDictionary = new Dictionary<char, List<RussianTextProcessor.IrregularWord>> { 
		{
			'A',
			new List<RussianTextProcessor.IrregularWord>
			{
				new RussianTextProcessor.IrregularWord("Alary", "Alary", "Alarego", "Alarego", "Alaremu", "Alaremu", "Alarego", "Alarego", "Alarym", "Alarym", "Alarym", "Alarym")
			}
		} };

		private const string Consonants = "БбВвГгДдЖжЗзЙйКкЛлМмНнПпРрСсТтФфХхЦцЧчШшЩщЬьЪъ";

		private static Dictionary<string, Dictionary<string, string>> exceptions = new Dictionary<string, Dictionary<string, string>>
		{
			{
				"стургиец",
				new Dictionary<string, string>
				{
					{ ".g", "стургийца" },
					{ ".d", "стургийцу" },
					{ ".a", "стургийца" },
					{ ".i", "стургийцем" },
					{ ".l", "стургийце" },
					{ ".p", "стургийцы" },
					{ ".gp", "стургийцев" },
					{ ".dp", "стургийцам" },
					{ ".ap", "стургийцев" },
					{ ".ip", "стургийцами" },
					{ ".lp", "стургийцах" }
				}
			},
			{
				"путь",
				new Dictionary<string, string>
				{
					{ ".g", "Пути" },
					{ ".d", "Пути" },
					{ ".a", "Путь" },
					{ ".i", "Путем" },
					{ ".l", "Пути" },
					{ ".p", "Пути" },
					{ ".gp", "Путей" },
					{ ".dp", "Путям" },
					{ ".ap", "Пути" },
					{ ".ip", "Путями" },
					{ ".lp", "Путях" }
				}
			},
			{
				"вилы",
				new Dictionary<string, string>
				{
					{ ".g", "вил" },
					{ ".d", "вилам" },
					{ ".a", "вилы" },
					{ ".i", "вилами" },
					{ ".l", "вилах" },
					{ ".p", "вилы" },
					{ ".gp", "вил" },
					{ ".dp", "вилам" },
					{ ".ap", "вилы" },
					{ ".ip", "вилами" },
					{ ".lp", "вилах" }
				}
			},
			{
				"лес",
				new Dictionary<string, string>
				{
					{ ".g", "Леса" },
					{ ".d", "Лесу" },
					{ ".a", "Лес" },
					{ ".i", "Лесом" },
					{ ".l", "Лесу" },
					{ ".p", "Леса" },
					{ ".gp", "Лесов" },
					{ ".dp", "Лесам" },
					{ ".ap", "Леса" },
					{ ".ip", "Лесами" },
					{ ".lp", "Лесах" }
				}
			},
			{
				"дочь",
				new Dictionary<string, string>
				{
					{ ".g", "Дочери" },
					{ ".d", "Дочери" },
					{ ".a", "Дочь" },
					{ ".i", "Дочерью" },
					{ ".l", "Дочери" },
					{ ".p", "Дочери" },
					{ ".gp", "Дочерей" },
					{ ".dp", "Дочерям" },
					{ ".ap", "Дочерей" },
					{ ".ip", "Дочерями" },
					{ ".lp", "Дочерях" }
				}
			},
			{
				"угол",
				new Dictionary<string, string>
				{
					{ ".g", "Угла" },
					{ ".d", "Углу" },
					{ ".a", "Угол" },
					{ ".i", "Углом" },
					{ ".l", "Углу" },
					{ ".p", "Углы" },
					{ ".gp", "Углов" },
					{ ".dp", "Углам" },
					{ ".ap", "Углы" },
					{ ".ip", "Углами" },
					{ ".lp", "Углах" }
				}
			},
			{
				"козёл",
				new Dictionary<string, string>
				{
					{ ".g", "Козла" },
					{ ".d", "Козлу" },
					{ ".a", "Козла" },
					{ ".i", "Козлом" },
					{ ".l", "Козле" },
					{ ".p", "Козлы" },
					{ ".gp", "Козлов" },
					{ ".dp", "Козлам" },
					{ ".ap", "Козлов" },
					{ ".ip", "Козлами" },
					{ ".lp", "Козлах" }
				}
			},
			{
				"берег",
				new Dictionary<string, string>
				{
					{ ".g", "Берега" },
					{ ".d", "Берегу" },
					{ ".a", "Берег" },
					{ ".i", "Берегом" },
					{ ".l", "Берегу" },
					{ ".p", "Берега" },
					{ ".gp", "Берегов" },
					{ ".dp", "Берегам" },
					{ ".ap", "Берега" },
					{ ".ip", "Берегами" },
					{ ".lp", "Берегах" }
				}
			},
			{
				"стул",
				new Dictionary<string, string>
				{
					{ ".g", "Стула" },
					{ ".d", "Стулу" },
					{ ".a", "Стул" },
					{ ".i", "Стулом" },
					{ ".l", "Стуле" },
					{ ".p", "Стулья" },
					{ ".gp", "Стульев" },
					{ ".dp", "Стульям" },
					{ ".ap", "Стулья" },
					{ ".ip", "Стульями" },
					{ ".lp", "Стульяах" }
				}
			},
			{
				"человек",
				new Dictionary<string, string>
				{
					{ ".g", "Человека" },
					{ ".d", "Человеку" },
					{ ".a", "Человека" },
					{ ".i", "Человеком" },
					{ ".l", "о человеке" },
					{ ".p", "Люди" },
					{ ".gp", "Людей" },
					{ ".dp", "Людям" },
					{ ".ap", "Людей" },
					{ ".ip", "Людьми" },
					{ ".lp", "о людях" }
				}
			},
			{
				"судно",
				new Dictionary<string, string>
				{
					{ ".g", "Судна" },
					{ ".d", "Судну" },
					{ ".a", "Судно" },
					{ ".i", "Судном" },
					{ ".l", "о судне" },
					{ ".p", "Суда" },
					{ ".gp", "Судов" },
					{ ".dp", "Судам" },
					{ ".ap", "Суда" },
					{ ".ip", "Судами" },
					{ ".lp", "о судах" }
				}
			},
			{
				"время",
				new Dictionary<string, string>
				{
					{ ".g", "Времени" },
					{ ".d", "Времени" },
					{ ".a", "Время" },
					{ ".i", "Временем" },
					{ ".l", "о времени" },
					{ ".p", "Времена" },
					{ ".gp", "Времён" },
					{ ".dp", "Временам" },
					{ ".ap", "Времена" },
					{ ".ip", "Временами" },
					{ ".lp", "о временах" }
				}
			},
			{
				"горожанин",
				new Dictionary<string, string>
				{
					{ ".g", "Горожанина" },
					{ ".d", "Горожанину" },
					{ ".a", "Горожанина" },
					{ ".i", "Горожанином" },
					{ ".l", "о горожанине" },
					{ ".p", "Горожане" },
					{ ".gp", "Горожан" },
					{ ".dp", "Горожанам" },
					{ ".ap", "Горожан" },
					{ ".ip", "Горожанами" },
					{ ".lp", "о горожанах" }
				}
			},
			{
				"никто",
				new Dictionary<string, string>
				{
					{ ".g", "Никого" },
					{ ".d", "Никому" },
					{ ".a", "Никого" },
					{ ".i", "Никем" },
					{ ".l", "Ни о ком" },
					{ ".p", "Никто" },
					{ ".gp", "Никого" },
					{ ".dp", "Никому" },
					{ ".ap", "Никого" },
					{ ".ip", "Никем" },
					{ ".lp", "Ни о ком" }
				}
			},
			{
				"ничто",
				new Dictionary<string, string>
				{
					{ ".g", "Ничего" },
					{ ".d", "Ничему" },
					{ ".a", "Ничего" },
					{ ".i", "Ничем" },
					{ ".l", "Ни о чём" },
					{ ".p", "Ничто" },
					{ ".gp", "Ничего" },
					{ ".dp", "Ничему" },
					{ ".ap", "Ничего" },
					{ ".ip", "Ничем" },
					{ ".lp", "Ни о чём" }
				}
			},
			{
				"наш",
				new Dictionary<string, string>
				{
					{ ".g", "Нашего" },
					{ ".d", "Нашему" },
					{ ".a", "Нашего" },
					{ ".i", "Нашим" },
					{ ".l", "о нашем" },
					{ ".p", "Наши" },
					{ ".gp", "Наших" },
					{ ".dp", "Нашим" },
					{ ".ap", "Наших" },
					{ ".ip", "Нашими" },
					{ ".lp", "о наших" }
				}
			},
			{
				"мать",
				new Dictionary<string, string>
				{
					{ ".g", "Матери" },
					{ ".d", "Матери" },
					{ ".a", "Мать" },
					{ ".i", "Матерью" },
					{ ".l", "о матери" },
					{ ".p", "Матери" },
					{ ".gp", "Матерей" },
					{ ".dp", "Матерям" },
					{ ".ap", "Матерей" },
					{ ".ip", "Матерями" },
					{ ".lp", "о матерях" }
				}
			},
			{
				"мастерская",
				new Dictionary<string, string>
				{
					{ ".g", "мастерской" },
					{ ".d", "мастерской" },
					{ ".a", "мастерскую" },
					{ ".i", "мастерской" },
					{ ".l", "мастерской" },
					{ ".p", "мастерские" },
					{ ".gp", "мастерских" },
					{ ".dp", "мастерским" },
					{ ".ap", "мастерские" },
					{ ".ip", "мастерскими" },
					{ ".lp", "мастерских" }
				}
			},
			{
				"медвежья",
				new Dictionary<string, string>
				{
					{ ".g", "медвежьей" },
					{ ".d", "медвежьей" },
					{ ".a", "медвежью" },
					{ ".i", "медвежьей" },
					{ ".l", "медвежьей" }
				}
			},
			{
				"волчья",
				new Dictionary<string, string>
				{
					{ ".g", "волчьей" },
					{ ".d", "волчьей" },
					{ ".a", "волчью" },
					{ ".i", "волчьей" },
					{ ".l", "волчьей" }
				}
			},
			{
				"медвежий",
				new Dictionary<string, string>
				{
					{ ".g", "медвежьего" },
					{ ".d", "медвежьему" },
					{ ".a", "медвежий" },
					{ ".i", "медвежьим" },
					{ ".l", "медвежьем" }
				}
			},
			{
				"волчий",
				new Dictionary<string, string>
				{
					{ ".g", "волчьим" },
					{ ".d", "волчьему" },
					{ ".a", "волчий" },
					{ ".i", "волчьим" },
					{ ".l", "волчьем" }
				}
			}
		};

		private enum WordGenderEnum
		{
			MasculineInanimate,
			MasculineAnimate,
			FeminineInanimate,
			FeminineAnimate,
			NeuterInanimate,
			NeuterAnimate,
			NoDeclination
		}

		private static class NounTokens
		{
			public const string Nominative = ".n";

			public const string NominativePlural = ".p";

			public const string Accusative = ".a";

			public const string Genitive = ".g";

			public const string Instrumental = ".i";

			public const string Locative = ".l";

			public const string Dative = ".d";

			public const string AccusativePlural = ".ap";

			public const string GenitivePlural = ".gp";

			public const string InstrumentalPlural = ".ip";

			public const string LocativePlural = ".lp";

			public const string DativePlural = ".dp";

			public static readonly string[] TokenList = new string[]
			{
				".n", ".p", ".a", ".g", ".i", ".l", ".d", ".ap", ".gp", ".ip",
				".lp", ".dp"
			};
		}

		private static class AdjectiveTokens
		{
			public const string Nominative = ".j";

			public const string NominativePlural = ".jp";

			public const string Accusative = ".ja";

			public const string Genitive = ".jg";

			public const string Instrumental = ".ji";

			public const string Locative = ".jl";

			public const string Dative = ".jd";

			public const string AccusativePlural = ".jap";

			public const string GenitivePlural = ".jgp";

			public const string InstrumentalPlural = ".jip";

			public const string LocativePlural = ".jlp";

			public const string DativePlural = ".jdp";

			public static readonly string[] TokenList = new string[]
			{
				".j", ".jp", ".ja", ".jap", ".jg", ".jgp", ".jd", ".jdp", ".jl", ".jlp",
				".ji", ".jip"
			};
		}

		private static class GenderTokens
		{
			public const string MasculineAnimate = ".MA";

			public const string MasculineInanimate = ".MI";

			public const string FeminineAnimate = ".FA";

			public const string FeminineInanimate = ".FI";

			public const string NeuterAnimate = ".NA";

			public const string NeuterInanimate = ".NI";

			public static readonly string[] TokenList = new string[] { ".MI", ".MA", ".FI", ".FA", ".NI", ".NA" };
		}

		private static class WordGroupTokens
		{
			public const string NounNominativePlural = ".nnp";

			public const string NounNominative = ".nn";

			public const string AdjectiveNominativePlural = ".ajp";

			public const string AdjectiveNominative = ".aj";

			public const string NounNominativePluralWithBrackets = "{.nnp}";

			public const string NounNominativeWithBrackets = "{.nn}";

			public const string AdjectiveNominativePluralWithBrackets = "{.ajp}";

			public const string AdjectiveNominativeWithBrackets = "{.aj}";
		}

		private struct IrregularWord
		{
			public IrregularWord(string nominative, string nominativePlural, string genitive, string genitivePlural, string dative, string dativePlural, string accusative, string accusativePlural, string instrumental, string instrumentalPlural, string locative, string locativePlural)
			{
				this.Nominative = nominative;
				this.NominativePlural = nominativePlural;
				this.Accusative = accusative;
				this.Genitive = genitive;
				this.Instrumental = instrumental;
				this.Locative = locative;
				this.Dative = dative;
				this.AccusativePlural = accusativePlural;
				this.GenitivePlural = genitivePlural;
				this.InstrumentalPlural = instrumentalPlural;
				this.LocativePlural = locativePlural;
				this.DativePlural = dativePlural;
			}

			public readonly string Nominative;

			public readonly string NominativePlural;

			public readonly string Accusative;

			public readonly string Genitive;

			public readonly string Instrumental;

			public readonly string Locative;

			public readonly string Dative;

			public readonly string AccusativePlural;

			public readonly string GenitivePlural;

			public readonly string InstrumentalPlural;

			public readonly string LocativePlural;

			public readonly string DativePlural;
		}
	}
}
