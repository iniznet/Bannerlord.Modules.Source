using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TaleWorlds.Library;

namespace TaleWorlds.Localization.TextProcessor.LanguageProcessors
{
	public class GermanTextProcessor : LanguageSpecificTextProcessor
	{
		public override CultureInfo CultureInfoForLanguage
		{
			get
			{
				return GermanTextProcessor.CultureInfo;
			}
		}

		public override void ClearTemporaryData()
		{
			GermanTextProcessor.LinkList.Clear();
			GermanTextProcessor.WordGroups.Clear();
			GermanTextProcessor.WordGroupsNoTags.Clear();
			GermanTextProcessor._curGender = GermanTextProcessor.WordGenderEnum.NoDeclination;
			GermanTextProcessor._doesComeFromWordGroup = false;
		}

		private bool Masculine
		{
			get
			{
				return GermanTextProcessor._curGender == GermanTextProcessor.WordGenderEnum.Masculine;
			}
		}

		private bool Feminine
		{
			get
			{
				return GermanTextProcessor._curGender == GermanTextProcessor.WordGenderEnum.Feminine;
			}
		}

		private bool Neuter
		{
			get
			{
				return GermanTextProcessor._curGender == GermanTextProcessor.WordGenderEnum.Neuter;
			}
		}

		private bool Plural
		{
			get
			{
				return GermanTextProcessor._curGender == GermanTextProcessor.WordGenderEnum.Plural;
			}
		}

		[TupleElementNames(new string[] { "wordGroup", "firstMarkerPost" })]
		private static List<ValueTuple<string, int>> WordGroups
		{
			[return: TupleElementNames(new string[] { "wordGroup", "firstMarkerPost" })]
			get
			{
				if (GermanTextProcessor._wordGroups == null)
				{
					GermanTextProcessor._wordGroups = new List<ValueTuple<string, int>>();
				}
				return GermanTextProcessor._wordGroups;
			}
		}

		private static List<string> LinkList
		{
			get
			{
				if (GermanTextProcessor._linkList == null)
				{
					GermanTextProcessor._linkList = new List<string>();
				}
				return GermanTextProcessor._linkList;
			}
		}

		private static List<string> WordGroupsNoTags
		{
			get
			{
				if (GermanTextProcessor._wordGroupsNoTags == null)
				{
					GermanTextProcessor._wordGroupsNoTags = new List<string>();
				}
				return GermanTextProcessor._wordGroupsNoTags;
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

		public override void ProcessToken(string sourceText, ref int cursorPos, string token, StringBuilder outputString)
		{
			bool flag = false;
			if (token == this.LinkTag)
			{
				GermanTextProcessor.LinkList.Add(sourceText.Substring(this.LinkTagLength));
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
			else if (Array.IndexOf<string>(GermanTextProcessor.GenderTokens.TokenList, token) >= 0)
			{
				if (token == ".M")
				{
					this.SetMasculine();
				}
				else if (token == ".F")
				{
					this.SetFeminine();
				}
				else if (token == ".N")
				{
					this.SetNeuter();
				}
				else if (token == ".P")
				{
					this.SetPlural();
				}
				if (cursorPos == token.Length + 2 && sourceText.IndexOf("{.", cursorPos, StringComparison.InvariantCulture) == -1 && sourceText.IndexOf(' ', cursorPos) == -1)
				{
					GermanTextProcessor.WordGroups.Add(new ValueTuple<string, int>(sourceText + "{.nn}", cursorPos));
					GermanTextProcessor.WordGroupsNoTags.Add(sourceText.Substring(cursorPos));
				}
			}
			else if (Array.IndexOf<string>(GermanTextProcessor.WordGroupTokens.TokenList, token) >= 0)
			{
				if (token == ".nnp" || token == ".ajmp" || token == ".ajwp" || token == ".ajsp" || token == ".ajm" || token == ".ajw" || token == ".ajs")
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
					else if (token == ".ajmp")
					{
						this.AddSuffixMixedNominativePlural(outputString);
					}
					else if (token == ".ajwp")
					{
						this.AddSuffixWeakNominativePlural(outputString);
					}
					else if (token == ".ajsp")
					{
						this.AddSuffixStrongNominativePlural(outputString);
					}
					else if (token == ".ajm")
					{
						this.AddSuffixMixedNominative(outputString);
					}
					else if (token == ".ajw")
					{
						this.AddSuffixWeakNominative(outputString);
					}
					else if (token == ".ajs")
					{
						this.AddSuffixStrongNominative(outputString);
					}
				}
				else if (token == ".pnpgroup" || token == ".pngroup")
				{
					this.AddPronounArticle(sourceText, cursorPos, token, ref outputString);
				}
				GermanTextProcessor._curGender = GermanTextProcessor.WordGenderEnum.NoDeclination;
				this.WordGroupProcessor(sourceText, cursorPos);
			}
			else if (Array.IndexOf<string>(GermanTextProcessor.NounTokens.TokenList, token) >= 0 && (!GermanTextProcessor._doesComeFromWordGroup || (GermanTextProcessor._doesComeFromWordGroup && GermanTextProcessor._curGender == GermanTextProcessor.WordGenderEnum.NoDeclination)) && this.IsWordGroup(token.Length, sourceText, cursorPos, out num2))
			{
				if (num2 >= 0)
				{
					token = "{" + token + "}";
					GermanTextProcessor._curGender = GermanTextProcessor.WordGenderEnum.NoDeclination;
					this.AddSuffixWordGroup(token, num2, outputString);
				}
			}
			else if (token == ".o")
			{
				this.HandlePossession(outputString, cursorPos);
			}
			else if (GermanTextProcessor._curGender != GermanTextProcessor.WordGenderEnum.NoDeclination)
			{
				if (this.ShouldDeclineWord(sourceText, cursorPos, token))
				{
					string text2;
					int num3;
					if (this.IsIrregularWord(sourceText, cursorPos, token, out text2, out num3))
					{
						outputString.Remove(outputString.Length - num3, num3);
						outputString.Append(text2);
					}
					else
					{
						uint num4 = <PrivateImplementationDetails>.ComputeStringHash(token);
						if (num4 > 1642453538U)
						{
							if (num4 > 1927659500U)
							{
								if (num4 <= 3294919074U)
								{
									if (num4 <= 2295634285U)
									{
										if (num4 != 2093905499U)
										{
											if (num4 != 2295634285U)
											{
												goto IL_ABA;
											}
											if (!(token == ".wgp"))
											{
												goto IL_ABA;
											}
											goto IL_A90;
										}
										else
										{
											if (!(token == ".wap"))
											{
												goto IL_ABA;
											}
											goto IL_A90;
										}
									}
									else if (num4 != 3194106265U)
									{
										if (num4 != 3262202479U)
										{
											if (num4 != 3294919074U)
											{
												goto IL_ABA;
											}
											if (!(token == ".sdp"))
											{
												goto IL_ABA;
											}
											this.AddSuffixStrongDativePlural(outputString);
											goto IL_ABA;
										}
										else if (!(token == ".sap"))
										{
											goto IL_ABA;
										}
									}
									else
									{
										if (!(token == ".sgp"))
										{
											goto IL_ABA;
										}
										goto IL_A90;
									}
								}
								else if (num4 <= 3363015288U)
								{
									if (num4 != 3362880480U)
									{
										if (num4 != 3363015288U)
										{
											goto IL_ABA;
										}
										if (!(token == ".snp"))
										{
											goto IL_ABA;
										}
									}
									else
									{
										if (!(token == ".mdp"))
										{
											goto IL_ABA;
										}
										goto IL_A90;
									}
								}
								else if (num4 != 3665422075U)
								{
									if (num4 != 3834331098U)
									{
										if (num4 != 3867047693U)
										{
											goto IL_ABA;
										}
										if (!(token == ".map"))
										{
											goto IL_ABA;
										}
										goto IL_A90;
									}
									else
									{
										if (!(token == ".mnp"))
										{
											goto IL_ABA;
										}
										goto IL_A90;
									}
								}
								else
								{
									if (!(token == ".mgp"))
									{
										goto IL_ABA;
									}
									goto IL_A90;
								}
								this.AddSuffixStrongAccusativePlural(outputString);
								goto IL_ABA;
							}
							if (num4 <= 1743119252U)
							{
								if (num4 <= 1675273301U)
								{
									if (num4 != 1662879919U)
									{
										if (num4 != 1675273301U)
										{
											goto IL_ABA;
										}
										if (!(token == ".pd"))
										{
											goto IL_ABA;
										}
										goto IL_AAE;
									}
									else
									{
										if (!(token == ".ma"))
										{
											goto IL_ABA;
										}
										this.AddSuffixMixedAccusative(outputString);
										goto IL_ABA;
									}
								}
								else if (num4 != 1680098823U)
								{
									if (num4 != 1680457860U)
									{
										if (num4 != 1743119252U)
										{
											goto IL_ABA;
										}
										if (!(token == ".wn"))
										{
											goto IL_ABA;
										}
										this.AddSuffixWeakNominative(outputString);
										goto IL_ABA;
									}
									else
									{
										if (!(token == ".pgp"))
										{
											goto IL_ABA;
										}
										goto IL_AAE;
									}
								}
								else
								{
									if (!(token == ".np"))
									{
										goto IL_ABA;
									}
									this.AddSuffixNounNominativePlural(outputString);
									goto IL_ABA;
								}
							}
							else if (num4 <= 1746768014U)
							{
								if (num4 != 1745635181U)
								{
									if (num4 != 1746768014U)
									{
										goto IL_ABA;
									}
									if (!(token == ".mn"))
									{
										goto IL_ABA;
									}
									this.AddSuffixMixedNominative(outputString);
									goto IL_ABA;
								}
								else
								{
									if (!(token == ".dp"))
									{
										goto IL_ABA;
									}
									this.AddSuffixNounDativePlural(outputString);
									goto IL_ABA;
								}
							}
							else if (num4 != 1775939015U)
							{
								if (num4 != 1859563286U)
								{
									if (num4 != 1927659500U)
									{
										goto IL_ABA;
									}
									if (!(token == ".wnp"))
									{
										goto IL_ABA;
									}
								}
								else if (!(token == ".wdp"))
								{
									goto IL_ABA;
								}
							}
							else
							{
								if (!(token == ".pn"))
								{
									goto IL_ABA;
								}
								goto IL_AAE;
							}
							IL_A90:
							this.AddSuffixMixedDativePlural(outputString);
							goto IL_ABA;
						}
						if (num4 <= 1478729074U)
						{
							if (num4 <= 1205647064U)
							{
								if (num4 <= 1104981350U)
								{
									if (num4 != 1088203731U)
									{
										if (num4 != 1104981350U)
										{
											goto IL_ABA;
										}
										if (!(token == ".sd"))
										{
											goto IL_ABA;
										}
										this.AddSuffixStrongDative(outputString);
										goto IL_ABA;
									}
									else
									{
										if (!(token == ".sg"))
										{
											goto IL_ABA;
										}
										this.AddSuffixStrongGenitive(outputString);
										goto IL_ABA;
									}
								}
								else if (num4 != 1188869445U)
								{
									if (num4 != 1205647064U)
									{
										goto IL_ABA;
									}
									if (!(token == ".sn"))
									{
										goto IL_ABA;
									}
									this.AddSuffixStrongNominative(outputString);
									goto IL_ABA;
								}
								else
								{
									if (!(token == ".sa"))
									{
										goto IL_ABA;
									}
									this.AddSuffixStrongAccusative(outputString);
									goto IL_ABA;
								}
							}
							else if (num4 <= 1309564182U)
							{
								if (num4 != 1241467968U)
								{
									if (num4 != 1309564182U)
									{
										goto IL_ABA;
									}
									if (!(token == ".gp"))
									{
										goto IL_ABA;
									}
									this.AddSuffixNounGenitivePlural(outputString);
									goto IL_ABA;
								}
								else
								{
									if (!(token == ".ap"))
									{
										goto IL_ABA;
									}
									this.AddSuffixNounAccusativePlural(outputString);
									goto IL_ABA;
								}
							}
							else if (num4 != 1423190704U)
							{
								if (num4 != 1446012479U)
								{
									if (num4 != 1478729074U)
									{
										goto IL_ABA;
									}
									if (!(token == ".pap"))
									{
										goto IL_ABA;
									}
								}
								else if (!(token == ".pdp"))
								{
									goto IL_ABA;
								}
							}
							else
							{
								if (!(token == ".a"))
								{
									goto IL_ABA;
								}
								this.AddSuffixNounAccusative(outputString);
								goto IL_ABA;
							}
						}
						else if (num4 <= 1591385206U)
						{
							if (num4 <= 1514108693U)
							{
								if (num4 != 1507078799U)
								{
									if (num4 != 1514108693U)
									{
										goto IL_ABA;
									}
									if (!(token == ".pnp"))
									{
										goto IL_ABA;
									}
								}
								else
								{
									if (!(token == ".d"))
									{
										goto IL_ABA;
									}
									this.AddSuffixNounDative(outputString);
									goto IL_ABA;
								}
							}
							else if (num4 != 1523856418U)
							{
								if (num4 != 1578991824U)
								{
									if (num4 != 1591385206U)
									{
										goto IL_ABA;
									}
									if (!(token == ".pa"))
									{
										goto IL_ABA;
									}
								}
								else
								{
									if (!(token == ".md"))
									{
										goto IL_ABA;
									}
									this.AddSuffixMixedDative(outputString);
									goto IL_ABA;
								}
							}
							else
							{
								if (!(token == ".g"))
								{
									goto IL_ABA;
								}
								this.AddSuffixNounGenitive(outputString);
								goto IL_ABA;
							}
						}
						else if (num4 <= 1624940444U)
						{
							if (num4 != 1592120681U)
							{
								if (num4 != 1624940444U)
								{
									goto IL_ABA;
								}
								if (!(token == ".pg"))
								{
									goto IL_ABA;
								}
							}
							else
							{
								if (!(token == ".wa"))
								{
									goto IL_ABA;
								}
								this.AddSuffixWeakAccusative(outputString);
								goto IL_ABA;
							}
						}
						else if (num4 != 1625675919U)
						{
							if (num4 != 1629324681U)
							{
								if (num4 != 1642453538U)
								{
									goto IL_ABA;
								}
								if (!(token == ".wd"))
								{
									goto IL_ABA;
								}
								this.AddSuffixWeakDative(outputString);
								goto IL_ABA;
							}
							else
							{
								if (!(token == ".mg"))
								{
									goto IL_ABA;
								}
								this.AddSuffixMixedGenitive(outputString);
								goto IL_ABA;
							}
						}
						else
						{
							if (!(token == ".wg"))
							{
								goto IL_ABA;
							}
							this.AddSuffixWeakGenitive(outputString);
							goto IL_ABA;
						}
						IL_AAE:
						this.AddPronounArticle(sourceText, cursorPos, token, ref outputString);
					}
				}
				IL_ABA:
				GermanTextProcessor._curGender = GermanTextProcessor.WordGenderEnum.NoDeclination;
			}
			if (flag)
			{
				cursorPos += this.LinkEndingLength;
				outputString.Append(this.LinkEnding);
			}
		}

		private void HandlePossession(StringBuilder outputString, int cursorPos)
		{
			string text = outputString.ToString();
			bool flag = false;
			if (text.Length < cursorPos)
			{
				cursorPos = text.Length;
			}
			if (text.EndsWith("</b></a>"))
			{
				cursorPos -= 8;
				outputString.Remove(outputString.Length - 8, 8);
				flag = true;
			}
			char lastCharacter = GermanTextProcessor.GetLastCharacter(outputString, cursorPos);
			if (GermanTextProcessor.SSounds.Contains(char.ToLower(lastCharacter, GermanTextProcessor.CultureInfo)))
			{
				outputString.Append('\'');
			}
			else
			{
				outputString.Append('s');
			}
			if (flag)
			{
				outputString.Append("</b></a>");
			}
		}

		private void AddSuffixWordGroup(string token, int wordGroupIndex, StringBuilder outputString)
		{
			bool flag = char.IsUpper(outputString[outputString.Length - GermanTextProcessor.WordGroupsNoTags[wordGroupIndex].Length]);
			string text = GermanTextProcessor.WordGroups[wordGroupIndex].Item1;
			outputString.Remove(outputString.Length - GermanTextProcessor.WordGroupsNoTags[wordGroupIndex].Length, GermanTextProcessor.WordGroupsNoTags[wordGroupIndex].Length);
			text = text.Replace("{.nn}", token);
			if (token.Equals("{.n}"))
			{
				text = text.Replace("{.nnp}", "{.np}");
				text = text.Replace("{.pnpgroup}", "{.pn}");
				text = text.Replace("{.ajw}", "{.wn}");
				text = text.Replace("{.ajm}", "{.mn}");
				text = text.Replace("{.ajs}", "{.sn}");
				text = text.Replace("{.ajwp}", "{.wnp}");
				text = text.Replace("{.ajmp}", "{.mnp}");
				text = text.Replace("{.ajsp}", "{.snp}");
			}
			else
			{
				text = text.Replace("{.ajw}", token.Insert(2, "w"));
				text = text.Replace("{.ajm}", token.Insert(2, "m"));
				text = text.Replace("{.ajs}", token.Insert(2, "s"));
				text = text.Replace("{.pngroup}", token.Insert(2, "p"));
				if (token.Contains("p"))
				{
					text = text.Replace("{.nnp}", token);
					text = text.Replace("{.ajwp}", token.Insert(2, "w"));
					text = text.Replace("{.ajmp}", token.Insert(2, "m"));
					text = text.Replace("{.ajsp}", token.Insert(2, "s"));
					text = text.Replace("{.pnpgroup}", token.Insert(2, "p"));
				}
				else
				{
					text = text.Replace("{.nnp}", token.Insert(3, "p"));
					text = text.Replace("{.ajwp}", token.Insert(2, "w").Insert(4, "p"));
					text = text.Replace("{.ajmp}", token.Insert(2, "m").Insert(4, "p"));
					text = text.Replace("{.ajsp}", token.Insert(2, "s").Insert(4, "p"));
					text = text.Replace("{.pnpgroup}", token.Insert(2, "p").Insert(4, "p"));
				}
			}
			GermanTextProcessor._doesComeFromWordGroup = true;
			string text2 = base.Process(text);
			GermanTextProcessor._doesComeFromWordGroup = false;
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
			wordGroupIndex = -1;
			int num2 = GermanTextProcessor.WordGroupsNoTags.Count - 1;
			while (0 <= num2)
			{
				if (curPos - tokenLength - 2 - GermanTextProcessor.WordGroupsNoTags[num2].Length >= 0 && GermanTextProcessor.WordGroupsNoTags[num2].Length > num && sourceText.Substring(curPos - tokenLength - 2 - GermanTextProcessor.WordGroupsNoTags[num2].Length, GermanTextProcessor.WordGroupsNoTags[num2].Length).Equals(GermanTextProcessor.WordGroupsNoTags[num2]))
				{
					wordGroupIndex = num2;
					num = GermanTextProcessor.WordGroupsNoTags[num2].Length;
				}
				num2--;
			}
			return num > 0;
		}

		private void AddSuffixNounNominativePlural(StringBuilder outputString)
		{
			if (!this.Feminine)
			{
				if ((this.Masculine || this.Neuter) && !this.AddSuffixForNDeclension(outputString))
				{
					outputString.Append('e');
				}
				return;
			}
			if (GermanTextProcessor.GetLastCharacter(outputString) == 'e')
			{
				outputString.Append('n');
				return;
			}
			if (GermanTextProcessor.GetEnding(outputString, 2) == "in")
			{
				outputString.Append("nen");
				return;
			}
			outputString.Append("e");
		}

		private void AddSuffixNounGenitive(StringBuilder outputString)
		{
			if (!this.Feminine && (this.Masculine || this.Neuter) && !this.AddSuffixForNDeclension(outputString))
			{
				char lastCharacter = GermanTextProcessor.GetLastCharacter(outputString);
				if (!this.IsVowel(lastCharacter))
				{
					outputString.Append('e');
				}
				outputString.Append('s');
			}
		}

		private void AddSuffixNounGenitivePlural(StringBuilder outputString)
		{
			if (!this.Feminine)
			{
				if ((this.Masculine || this.Neuter) && !this.AddSuffixForNDeclension(outputString))
				{
					char lastCharacter = GermanTextProcessor.GetLastCharacter(outputString);
					if (!this.IsVowel(lastCharacter))
					{
						outputString.Append("e");
					}
				}
				return;
			}
			if (GermanTextProcessor.GetLastCharacter(outputString) == 'e')
			{
				outputString.Append('n');
				return;
			}
			if (GermanTextProcessor.GetEnding(outputString, 2) == "in")
			{
				outputString.Append("nen");
				return;
			}
			outputString.Append("e");
		}

		private void AddSuffixNounDative(StringBuilder outputString)
		{
			if (this.Masculine || this.Neuter)
			{
				this.AddSuffixForNDeclension(outputString);
			}
		}

		private void AddSuffixNounDativePlural(StringBuilder outputString)
		{
			char lastCharacter = GermanTextProcessor.GetLastCharacter(outputString);
			if (!this.IsVowel(lastCharacter) && lastCharacter != 'r')
			{
				outputString.Append('e');
			}
			outputString.Append('n');
		}

		private void AddSuffixNounAccusative(StringBuilder outputString)
		{
			if (this.Masculine || this.Neuter)
			{
				this.AddSuffixForNDeclension(outputString);
			}
		}

		private void AddSuffixNounAccusativePlural(StringBuilder outputString)
		{
			this.AddSuffixNounNominativePlural(outputString);
		}

		private void AddSuffixWeakNominative(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			if (this.Plural)
			{
				outputString.Append("en");
				return;
			}
			outputString.Append('e');
		}

		private void AddSuffixMixedNominative(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			if (this.Masculine)
			{
				outputString.Append("er");
				return;
			}
			if (this.Feminine)
			{
				outputString.Append("e");
				return;
			}
			if (this.Neuter)
			{
				outputString.Append("es");
				return;
			}
			if (this.Plural)
			{
				outputString.Append("en");
			}
		}

		private void AddSuffixStrongNominative(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			if (this.Masculine)
			{
				outputString.Append("er");
				return;
			}
			if (this.Feminine || this.Plural)
			{
				outputString.Append("e");
				return;
			}
			if (this.Neuter)
			{
				outputString.Append("es");
			}
		}

		private void AddSuffixWeakAccusative(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			if (this.Masculine || this.Plural)
			{
				outputString.Append("en");
				return;
			}
			outputString.Append("e");
		}

		private void AddSuffixMixedAccusative(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			if (this.Masculine || this.Plural)
			{
				outputString.Append("en");
				return;
			}
			if (this.Feminine)
			{
				outputString.Append("e");
				return;
			}
			outputString.Append("es");
		}

		private void AddSuffixStrongAccusative(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			if (this.Masculine)
			{
				outputString.Append("en");
				return;
			}
			if (this.Feminine || this.Plural)
			{
				outputString.Append("e");
				return;
			}
			outputString.Append("es");
		}

		private void AddSuffixWeakDative(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			outputString.Append("en");
		}

		private void AddSuffixMixedDative(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			outputString.Append("en");
		}

		private void AddSuffixStrongDative(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			if (this.Feminine)
			{
				outputString.Append("er");
				return;
			}
			if (this.Masculine || this.Neuter)
			{
				outputString.Append("em");
				return;
			}
			outputString.Append("en");
		}

		private void AddSuffixWeakGenitive(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			outputString.Append("en");
		}

		private void AddSuffixMixedGenitive(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			outputString.Append("en");
		}

		private void AddSuffixStrongGenitive(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			if (this.Feminine || this.Plural)
			{
				outputString.Append("er");
				return;
			}
			if (this.Masculine || this.Neuter)
			{
				outputString.Append("en");
			}
		}

		private void AddSuffixWeakNominativePlural(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			outputString.Append("en");
		}

		private void AddSuffixMixedNominativePlural(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			outputString.Append("en");
		}

		private void AddSuffixStrongNominativePlural(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			outputString.Append("e");
		}

		private void AddSuffixStrongAccusativePlural(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			outputString.Append("e");
		}

		private void AddSuffixMixedDativePlural(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			outputString.Append("en");
		}

		private void AddSuffixStrongDativePlural(StringBuilder outputString)
		{
			this.ModifyAdjective(outputString);
			outputString.Append("en");
		}

		private bool AddSuffixForNDeclension(StringBuilder outputString)
		{
			bool flag = false;
			string ending = GermanTextProcessor.GetEnding(outputString, 3);
			if (ending == "ent" || ending == "ant" || ending == "ist" || ending.EndsWith("at"))
			{
				flag = true;
				outputString.Append("en");
			}
			else if (ending.EndsWith("e"))
			{
				flag = true;
				outputString.Append("n");
			}
			return flag;
		}

		private void AddPronounArticle(string sourceText, int cursorPos, string token, ref StringBuilder outputString)
		{
			int num = sourceText.Remove(cursorPos - token.Length - 2).LastIndexOf('}') + 1;
			int num2 = cursorPos - token.Length - 2 - num;
			string text = "";
			if (num2 > 0 && GermanTextProcessor._curGender != GermanTextProcessor.WordGenderEnum.NoDeclination)
			{
				string text2 = sourceText.Substring(num, num2);
				char c = text2[0];
				text2 = text2.ToLowerInvariant();
				Dictionary<GermanTextProcessor.WordGenderEnum, GermanTextProcessor.DictionaryWord> dictionary;
				if (GermanTextProcessor.PronounArticleDictionary.TryGetValue(text2, out dictionary))
				{
					text = GermanTextProcessor.DictionaryWordWithCase(token, dictionary[GermanTextProcessor._curGender]);
				}
				if (text.Length > 0)
				{
					if (char.IsLower(c))
					{
						text = text.ToLowerInvariant();
					}
					outputString.Remove(outputString.Length - num2, num2);
					outputString.Append(text);
					Dictionary<string, string> dictionary2;
					if (GermanTextProcessor.ArticlePronounReplacementDictionary.TryGetValue(text.ToLowerInvariant(), out dictionary2))
					{
						string previousWord = this.GetPreviousWord(sourceText, cursorPos, token, ref outputString);
						string text3;
						if (dictionary2.TryGetValue(previousWord, out text3))
						{
							outputString = outputString.Replace(previousWord + " " + text, text3);
						}
					}
				}
			}
		}

		private string GetPreviousWord(string sourceText, int cursorPos, string token, ref StringBuilder outputString)
		{
			string[] array = sourceText.Substring(0, cursorPos).Split(new char[] { ' ' });
			int num = array.Length;
			if (num < 2)
			{
				return "";
			}
			return array[num - 2];
		}

		private void SetFeminine()
		{
			GermanTextProcessor._curGender = GermanTextProcessor.WordGenderEnum.Feminine;
		}

		private void SetNeuter()
		{
			GermanTextProcessor._curGender = GermanTextProcessor.WordGenderEnum.Neuter;
		}

		private void SetMasculine()
		{
			GermanTextProcessor._curGender = GermanTextProcessor.WordGenderEnum.Masculine;
		}

		private void SetPlural()
		{
			GermanTextProcessor._curGender = GermanTextProcessor.WordGenderEnum.Plural;
		}

		private bool IsVowel(char c)
		{
			return Array.IndexOf<char>(GermanTextProcessor.Vowels, c) >= 0;
		}

		private int FindLastVowel(StringBuilder outputText)
		{
			for (int i = outputText.Length - 1; i >= 0; i--)
			{
				if (this.IsVowel(outputText[i]))
				{
					return i;
				}
			}
			return -1;
		}

		private void RemoveLastCharacter(StringBuilder outputString)
		{
			outputString.Remove(outputString.Length - 1, 1);
		}

		private bool IsLastCharVowel(StringBuilder outputString)
		{
			char c = outputString[outputString.Length - 1];
			return this.IsVowel(c);
		}

		private void ModifyAdjective(StringBuilder outputString)
		{
			string ending = GermanTextProcessor.GetEnding(outputString, 2);
			if (this.Neuter && ending == "es")
			{
				outputString.Remove(outputString.Length - 2, 2);
				return;
			}
			if (GermanTextProcessor._curGender != GermanTextProcessor.WordGenderEnum.NoDeclination && ending[1] == 'e')
			{
				outputString.Remove(outputString.Length - 1, 1);
			}
		}

		private string GetVowelEnding(StringBuilder outputString)
		{
			if (outputString.Length == 0)
			{
				return "";
			}
			char c = outputString[outputString.Length - 1];
			if (!this.IsVowel(c))
			{
				return "";
			}
			if (outputString.Length > 1 && this.IsVowel(outputString[outputString.Length - 2]))
			{
				return outputString[outputString.Length - 2].ToString() + c.ToString();
			}
			return c.ToString();
		}

		private bool IsLink(string sourceText, int tokenLength, int cursorPos)
		{
			string text = sourceText.Remove(cursorPos - tokenLength);
			for (int i = 0; i < GermanTextProcessor.LinkList.Count; i++)
			{
				if (sourceText.Length >= GermanTextProcessor.LinkList[i].Length && text.EndsWith(GermanTextProcessor.LinkList[i]))
				{
					GermanTextProcessor.LinkList.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		private bool ShouldDeclineWord(string sourceText, int cursorPos, string token)
		{
			int num = sourceText.Remove(cursorPos - token.Length - 2).LastIndexOf('}') + 1;
			if (cursorPos - token.Length - 2 - num > 0)
			{
				string text = sourceText.Substring(num, cursorPos - token.Length - 2 - num);
				List<string> list;
				if (GermanTextProcessor.DoNotDeclineList.TryGetValue(char.ToUpper(text[0]), out list))
				{
					return !list.Contains(text);
				}
			}
			return true;
		}

		private bool IsIrregularWord(string sourceText, int cursorPos, string token, out string irregularWord, out int lengthOfWordToReplace)
		{
			int num = sourceText.Remove(cursorPos - token.Length - 2).LastIndexOf('}') + 1;
			lengthOfWordToReplace = cursorPos - token.Length - 2 - num;
			irregularWord = "";
			string text = "";
			if (lengthOfWordToReplace > 0)
			{
				text = sourceText.Substring(num, lengthOfWordToReplace);
				char c = char.ToUpperInvariant(text[0]);
				List<GermanTextProcessor.DictionaryWord> list;
				if (this.Masculine && GermanTextProcessor.IrregularMasculineDictionary.TryGetValue(c, out list))
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].Nominative.Equals(text, StringComparison.InvariantCultureIgnoreCase))
						{
							irregularWord = GermanTextProcessor.DictionaryWordWithCase(token, list[i]);
							break;
						}
					}
				}
				else if (this.Feminine && GermanTextProcessor.IrregularFeminineDictionary.TryGetValue(c, out list))
				{
					for (int j = 0; j < list.Count; j++)
					{
						if (list[j].Nominative.Equals(text, StringComparison.InvariantCultureIgnoreCase))
						{
							irregularWord = GermanTextProcessor.DictionaryWordWithCase(token, list[j]);
							break;
						}
					}
				}
				else if (this.Neuter && GermanTextProcessor.IrregularNeuterDictionary.TryGetValue(c, out list))
				{
					for (int k = 0; k < list.Count; k++)
					{
						if (list[k].Nominative.Equals(text, StringComparison.InvariantCultureIgnoreCase))
						{
							irregularWord = GermanTextProcessor.DictionaryWordWithCase(token, list[k]);
							break;
						}
					}
				}
				else if (this.Plural && GermanTextProcessor.IrregularPluralDictionary.TryGetValue(c, out list))
				{
					for (int l = 0; l < list.Count; l++)
					{
						if (list[l].Nominative.Equals(text, StringComparison.InvariantCultureIgnoreCase))
						{
							irregularWord = GermanTextProcessor.DictionaryWordWithCase(token, list[l]);
							break;
						}
					}
				}
			}
			if (irregularWord.Length > 0)
			{
				if (char.IsLower(text[0]))
				{
					irregularWord = irregularWord.ToLower();
				}
				return true;
			}
			return false;
		}

		private bool IsRecordedWithPreviousTag(string sourceText, int cursorPos)
		{
			for (int i = 0; i < GermanTextProcessor.WordGroups.Count; i++)
			{
				if (GermanTextProcessor.WordGroups[i].Item1 == sourceText && GermanTextProcessor.WordGroups[i].Item2 != cursorPos)
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
				GermanTextProcessor.WordGroups.Add(new ValueTuple<string, int>(sourceText, cursorPos));
				string text = sourceText.Replace("{.nn}", "{.n}");
				text = text.Replace("{.pngroup}", "{.pn}");
				text = text.Replace("{.ajw}", "{.wn}");
				text = text.Replace("{.ajm}", "{.mn}");
				text = text.Replace("{.ajs}", "{.sn}");
				text = text.Replace("{.nnp}", "{.np}");
				text = text.Replace("{.pnpgroup}", "{.pnp}");
				text = text.Replace("{.ajwp}", "{.wnp}");
				text = text.Replace("{.ajmp}", "{.mnp}");
				text = text.Replace("{.ajsp}", "{.snp}");
				GermanTextProcessor._doesComeFromWordGroup = true;
				GermanTextProcessor.WordGroupsNoTags.Add(base.Process(text));
				GermanTextProcessor._doesComeFromWordGroup = false;
			}
		}

		private static string DictionaryWordWithCase(string token, GermanTextProcessor.DictionaryWord dictionaryWord)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(token);
			if (num <= 1662879919U)
			{
				if (num <= 1423190704U)
				{
					if (num <= 1104981350U)
					{
						if (num <= 774788651U)
						{
							if (num != 534400075U)
							{
								if (num != 711915317U)
								{
									if (num != 774788651U)
									{
										goto IL_749;
									}
									if (!(token == ".ajmp"))
									{
										goto IL_749;
									}
									goto IL_718;
								}
								else
								{
									if (!(token == ".ajsp"))
									{
										goto IL_749;
									}
									goto IL_718;
								}
							}
							else
							{
								if (!(token == ".nnp"))
								{
									goto IL_749;
								}
								goto IL_718;
							}
						}
						else if (num != 845547889U)
						{
							if (num != 1088203731U)
							{
								if (num != 1104981350U)
								{
									goto IL_749;
								}
								if (!(token == ".sd"))
								{
									goto IL_749;
								}
								goto IL_73B;
							}
							else
							{
								if (!(token == ".sg"))
								{
									goto IL_749;
								}
								goto IL_72D;
							}
						}
						else
						{
							if (!(token == ".ajwp"))
							{
								goto IL_749;
							}
							goto IL_718;
						}
					}
					else if (num <= 1241467968U)
					{
						if (num != 1188869445U)
						{
							if (num != 1205647064U)
							{
								if (num != 1241467968U)
								{
									goto IL_749;
								}
								if (!(token == ".ap"))
								{
									goto IL_749;
								}
								goto IL_726;
							}
							else
							{
								if (!(token == ".sn"))
								{
									goto IL_749;
								}
								goto IL_711;
							}
						}
						else if (!(token == ".sa"))
						{
							goto IL_749;
						}
					}
					else if (num != 1243880729U)
					{
						if (num != 1309564182U)
						{
							if (num != 1423190704U)
							{
								goto IL_749;
							}
							if (!(token == ".a"))
							{
								goto IL_749;
							}
						}
						else
						{
							if (!(token == ".gp"))
							{
								goto IL_749;
							}
							goto IL_734;
						}
					}
					else
					{
						if (!(token == ".nn"))
						{
							goto IL_749;
						}
						goto IL_711;
					}
				}
				else if (num <= 1578991824U)
				{
					if (num <= 1507078799U)
					{
						if (num != 1446012479U)
						{
							if (num != 1478729074U)
							{
								if (num != 1507078799U)
								{
									goto IL_749;
								}
								if (!(token == ".d"))
								{
									goto IL_749;
								}
								goto IL_73B;
							}
							else
							{
								if (!(token == ".pap"))
								{
									goto IL_749;
								}
								goto IL_726;
							}
						}
						else
						{
							if (!(token == ".pdp"))
							{
								goto IL_749;
							}
							goto IL_742;
						}
					}
					else if (num != 1514108693U)
					{
						if (num != 1523856418U)
						{
							if (num != 1578991824U)
							{
								goto IL_749;
							}
							if (!(token == ".md"))
							{
								goto IL_749;
							}
							goto IL_73B;
						}
						else
						{
							if (!(token == ".g"))
							{
								goto IL_749;
							}
							goto IL_72D;
						}
					}
					else
					{
						if (!(token == ".pnp"))
						{
							goto IL_749;
						}
						goto IL_718;
					}
				}
				else if (num <= 1624940444U)
				{
					if (num != 1591385206U)
					{
						if (num != 1592120681U)
						{
							if (num != 1624940444U)
							{
								goto IL_749;
							}
							if (!(token == ".pg"))
							{
								goto IL_749;
							}
							goto IL_72D;
						}
						else if (!(token == ".wa"))
						{
							goto IL_749;
						}
					}
					else if (!(token == ".pa"))
					{
						goto IL_749;
					}
				}
				else if (num <= 1629324681U)
				{
					if (num != 1625675919U)
					{
						if (num != 1629324681U)
						{
							goto IL_749;
						}
						if (!(token == ".mg"))
						{
							goto IL_749;
						}
						goto IL_72D;
					}
					else
					{
						if (!(token == ".wg"))
						{
							goto IL_749;
						}
						goto IL_72D;
					}
				}
				else if (num != 1642453538U)
				{
					if (num != 1662879919U)
					{
						goto IL_749;
					}
					if (!(token == ".ma"))
					{
						goto IL_749;
					}
				}
				else
				{
					if (!(token == ".wd"))
					{
						goto IL_749;
					}
					goto IL_73B;
				}
				return dictionaryWord.Accusative;
				IL_72D:
				return dictionaryWord.Genitive;
			}
			if (num <= 2093905499U)
			{
				if (num <= 1745635181U)
				{
					if (num <= 1680098823U)
					{
						if (num != 1674854989U)
						{
							if (num != 1675273301U)
							{
								if (num != 1680098823U)
								{
									goto IL_749;
								}
								if (!(token == ".np"))
								{
									goto IL_749;
								}
								goto IL_718;
							}
							else
							{
								if (!(token == ".pd"))
								{
									goto IL_749;
								}
								goto IL_73B;
							}
						}
						else if (!(token == ".n"))
						{
							goto IL_749;
						}
					}
					else if (num != 1680457860U)
					{
						if (num != 1743119252U)
						{
							if (num != 1745635181U)
							{
								goto IL_749;
							}
							if (!(token == ".dp"))
							{
								goto IL_749;
							}
							goto IL_742;
						}
						else if (!(token == ".wn"))
						{
							goto IL_749;
						}
					}
					else
					{
						if (!(token == ".pgp"))
						{
							goto IL_749;
						}
						goto IL_734;
					}
				}
				else if (num <= 1811819932U)
				{
					if (num != 1746768014U)
					{
						if (num != 1775939015U)
						{
							if (num != 1811819932U)
							{
								goto IL_749;
							}
							if (!(token == ".pnpgroup"))
							{
								goto IL_749;
							}
							goto IL_718;
						}
						else if (!(token == ".pn"))
						{
							goto IL_749;
						}
					}
					else if (!(token == ".mn"))
					{
						goto IL_749;
					}
				}
				else if (num != 1859563286U)
				{
					if (num != 1927659500U)
					{
						if (num != 2093905499U)
						{
							goto IL_749;
						}
						if (!(token == ".wap"))
						{
							goto IL_749;
						}
						goto IL_726;
					}
					else
					{
						if (!(token == ".wnp"))
						{
							goto IL_749;
						}
						goto IL_718;
					}
				}
				else
				{
					if (!(token == ".wdp"))
					{
						goto IL_749;
					}
					goto IL_742;
				}
			}
			else if (num <= 3290807067U)
			{
				if (num <= 2921699449U)
				{
					if (num != 2141918926U)
					{
						if (num != 2295634285U)
						{
							if (num != 2921699449U)
							{
								goto IL_749;
							}
							if (!(token == ".ajm"))
							{
								goto IL_749;
							}
						}
						else
						{
							if (!(token == ".wgp"))
							{
								goto IL_749;
							}
							goto IL_734;
						}
					}
					else if (!(token == ".pngroup"))
					{
						goto IL_749;
					}
				}
				else if (num != 3194106265U)
				{
					if (num != 3262202479U)
					{
						if (num != 3290807067U)
						{
							goto IL_749;
						}
						if (!(token == ".ajw"))
						{
							goto IL_749;
						}
					}
					else
					{
						if (!(token == ".sap"))
						{
							goto IL_749;
						}
						goto IL_726;
					}
				}
				else
				{
					if (!(token == ".sgp"))
					{
						goto IL_749;
					}
					goto IL_734;
				}
			}
			else if (num <= 3362880480U)
			{
				if (num != 3294919074U)
				{
					if (num != 3357917543U)
					{
						if (num != 3362880480U)
						{
							goto IL_749;
						}
						if (!(token == ".mdp"))
						{
							goto IL_749;
						}
						goto IL_742;
					}
					else if (!(token == ".ajs"))
					{
						goto IL_749;
					}
				}
				else
				{
					if (!(token == ".sdp"))
					{
						goto IL_749;
					}
					goto IL_742;
				}
			}
			else if (num <= 3665422075U)
			{
				if (num != 3363015288U)
				{
					if (num != 3665422075U)
					{
						goto IL_749;
					}
					if (!(token == ".mgp"))
					{
						goto IL_749;
					}
					goto IL_734;
				}
				else
				{
					if (!(token == ".snp"))
					{
						goto IL_749;
					}
					goto IL_718;
				}
			}
			else if (num != 3834331098U)
			{
				if (num != 3867047693U)
				{
					goto IL_749;
				}
				if (!(token == ".map"))
				{
					goto IL_749;
				}
				goto IL_726;
			}
			else
			{
				if (!(token == ".mnp"))
				{
					goto IL_749;
				}
				goto IL_718;
			}
			IL_711:
			return dictionaryWord.Nominative;
			IL_718:
			return dictionaryWord.NominativePlural;
			IL_726:
			return dictionaryWord.AccusativePlural;
			IL_734:
			return dictionaryWord.GenitivePlural;
			IL_73B:
			return dictionaryWord.Dative;
			IL_742:
			return dictionaryWord.DativePlural;
			IL_749:
			return "MISSING IRREGULAR WORD IN LIST";
		}

		private static char GetLastCharacter(StringBuilder outputText, int cursorPos)
		{
			if (cursorPos < outputText.Length)
			{
				cursorPos = outputText.Length;
			}
			for (int i = cursorPos - 1; i >= 0; i--)
			{
				if (char.IsLetter(outputText[i]))
				{
					return outputText[i];
				}
			}
			return '*';
		}

		private static char GetLastCharacter(StringBuilder outputString)
		{
			if (outputString.Length <= 0)
			{
				return '*';
			}
			return outputString[outputString.Length - 1];
		}

		private static char GetSecondLastCharacter(StringBuilder outputString)
		{
			if (outputString.Length <= 1)
			{
				return '*';
			}
			return outputString[outputString.Length - 2];
		}

		private static string GetEnding(StringBuilder outputString, int numChars)
		{
			numChars = MathF.Min(numChars, outputString.Length);
			return outputString.ToString(outputString.Length - numChars, numChars);
		}

		private static readonly CultureInfo CultureInfo = new CultureInfo("de-DE");

		[ThreadStatic]
		private static GermanTextProcessor.WordGenderEnum _curGender;

		[TupleElementNames(new string[] { "wordGroup", "firstMarkerPost" })]
		[ThreadStatic]
		private static List<ValueTuple<string, int>> _wordGroups = new List<ValueTuple<string, int>>();

		[ThreadStatic]
		private static List<string> _wordGroupsNoTags = new List<string>();

		[ThreadStatic]
		private static List<string> _linkList = new List<string>();

		[ThreadStatic]
		private static bool _doesComeFromWordGroup = false;

		private static char[] Vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'ä', 'ü', 'ö' };

		private const string Consonants = "BbCcDdFfGgHhJjKkLlMmNnPpRrSsTtWwYyZz";

		private static char[] SSounds = new char[] { 's', 'ß', 'z', 'x' };

		private static readonly Dictionary<char, List<GermanTextProcessor.DictionaryWord>> IrregularMasculineDictionary = new Dictionary<char, List<GermanTextProcessor.DictionaryWord>> { 
		{
			'N',
			new List<GermanTextProcessor.DictionaryWord>
			{
				new GermanTextProcessor.DictionaryWord("Name", "Namen", "Namens", "Namen", "Namen", "Namen", "Namen", "Namen")
			}
		} };

		private static readonly Dictionary<char, List<GermanTextProcessor.DictionaryWord>> IrregularFeminineDictionary = new Dictionary<char, List<GermanTextProcessor.DictionaryWord>>();

		private static readonly Dictionary<char, List<GermanTextProcessor.DictionaryWord>> IrregularNeuterDictionary = new Dictionary<char, List<GermanTextProcessor.DictionaryWord>> { 
		{
			'I',
			new List<GermanTextProcessor.DictionaryWord>
			{
				new GermanTextProcessor.DictionaryWord("Imperium", "Imperien", "Imperiums", "Imperien", "Imperium", "Imperien", "Imperium", "Imperien")
			}
		} };

		private static readonly Dictionary<char, List<GermanTextProcessor.DictionaryWord>> IrregularPluralDictionary = new Dictionary<char, List<GermanTextProcessor.DictionaryWord>>();

		private static readonly Dictionary<string, Dictionary<GermanTextProcessor.WordGenderEnum, GermanTextProcessor.DictionaryWord>> PronounArticleDictionary = new Dictionary<string, Dictionary<GermanTextProcessor.WordGenderEnum, GermanTextProcessor.DictionaryWord>>
		{
			{
				"der",
				new Dictionary<GermanTextProcessor.WordGenderEnum, GermanTextProcessor.DictionaryWord>
				{
					{
						GermanTextProcessor.WordGenderEnum.Masculine,
						new GermanTextProcessor.DictionaryWord("Der", "Die", "Des", "Der", "Dem", "Den", "Den", "Die")
					},
					{
						GermanTextProcessor.WordGenderEnum.Feminine,
						new GermanTextProcessor.DictionaryWord("Die", "Die", "Der", "Der", "Der", "Den", "Die", "Die")
					},
					{
						GermanTextProcessor.WordGenderEnum.Neuter,
						new GermanTextProcessor.DictionaryWord("Das", "Die", "Des", "Der", "Dem", "Den", "Das", "Die")
					},
					{
						GermanTextProcessor.WordGenderEnum.Plural,
						new GermanTextProcessor.DictionaryWord("Die", "Die", "Der", "Der", "Den", "Den", "Die", "Die")
					}
				}
			},
			{
				"ein",
				new Dictionary<GermanTextProcessor.WordGenderEnum, GermanTextProcessor.DictionaryWord>
				{
					{
						GermanTextProcessor.WordGenderEnum.Masculine,
						new GermanTextProcessor.DictionaryWord("Ein", "", "Eines", "", "Einem", "", "Einen", "")
					},
					{
						GermanTextProcessor.WordGenderEnum.Feminine,
						new GermanTextProcessor.DictionaryWord("Eine", "", "Einer", "", "Einer", "", "Eine", "")
					},
					{
						GermanTextProcessor.WordGenderEnum.Neuter,
						new GermanTextProcessor.DictionaryWord("Ein", "", "Eines", "", "Einem", "", "Ein", "")
					},
					{
						GermanTextProcessor.WordGenderEnum.Plural,
						new GermanTextProcessor.DictionaryWord("", "", "", "", "", "", "", "")
					}
				}
			},
			{
				"dieser",
				new Dictionary<GermanTextProcessor.WordGenderEnum, GermanTextProcessor.DictionaryWord>
				{
					{
						GermanTextProcessor.WordGenderEnum.Masculine,
						new GermanTextProcessor.DictionaryWord("Dieser", "Diese", "Dieses", "Dieser", "Diesem", "Diesen", "Diesen", "Diese")
					},
					{
						GermanTextProcessor.WordGenderEnum.Feminine,
						new GermanTextProcessor.DictionaryWord("Diese", "Diese", "Dieser", "Dieser", "Dieser", "Diesen", "Diese", "Diese")
					},
					{
						GermanTextProcessor.WordGenderEnum.Neuter,
						new GermanTextProcessor.DictionaryWord("Dieses", "Diese", "Dieses", "Dieser", "Diesem", "Diesen", "Dieses", "Diese")
					},
					{
						GermanTextProcessor.WordGenderEnum.Plural,
						new GermanTextProcessor.DictionaryWord("Diese", "Diese", "Dieser", "Dieser", "Diesen", "Diesen", "Diese", "Diese")
					}
				}
			},
			{
				"jeder",
				new Dictionary<GermanTextProcessor.WordGenderEnum, GermanTextProcessor.DictionaryWord>
				{
					{
						GermanTextProcessor.WordGenderEnum.Masculine,
						new GermanTextProcessor.DictionaryWord("Jeder", "Alle", "Jedes", "Aller", "Jedem", "Allen", "Jeden", "Alle")
					},
					{
						GermanTextProcessor.WordGenderEnum.Feminine,
						new GermanTextProcessor.DictionaryWord("Jede", "Alle", "Jeder", "Aller", "Jeder", "Allen", "Jede", "Alle")
					},
					{
						GermanTextProcessor.WordGenderEnum.Neuter,
						new GermanTextProcessor.DictionaryWord("Jedes", "Alle", "Jedes", "Aller", "Jedem", "Allen", "Jedes", "Alle")
					},
					{
						GermanTextProcessor.WordGenderEnum.Plural,
						new GermanTextProcessor.DictionaryWord("Alle", "Alle", "Aller", "Aller", "Allen", "Allen", "Alle", "Alle")
					}
				}
			},
			{
				"kein",
				new Dictionary<GermanTextProcessor.WordGenderEnum, GermanTextProcessor.DictionaryWord>
				{
					{
						GermanTextProcessor.WordGenderEnum.Masculine,
						new GermanTextProcessor.DictionaryWord("Kein", "Keine", "Keines", "Keiner", "Keinem", "Keinen", "Keinen", "Keine")
					},
					{
						GermanTextProcessor.WordGenderEnum.Feminine,
						new GermanTextProcessor.DictionaryWord("Keine", "Keine", "Keiner", "Keiner", "Keiner", "Keinen", "Keine", "Keine")
					},
					{
						GermanTextProcessor.WordGenderEnum.Neuter,
						new GermanTextProcessor.DictionaryWord("Kein", "Keine", "Keines", "Keiner", "Keinem", "Keinen", "Kein", "Keine")
					},
					{
						GermanTextProcessor.WordGenderEnum.Plural,
						new GermanTextProcessor.DictionaryWord("Keine", "Keine", "Keiner", "Keiner", "Keinen", "Keinen", "Keine", "Keine")
					}
				}
			},
			{
				"dein",
				new Dictionary<GermanTextProcessor.WordGenderEnum, GermanTextProcessor.DictionaryWord>
				{
					{
						GermanTextProcessor.WordGenderEnum.Masculine,
						new GermanTextProcessor.DictionaryWord("Dein", "Deine", "Deines", "Deiner", "Deinem", "Deinen", "Deinen", "Deine")
					},
					{
						GermanTextProcessor.WordGenderEnum.Feminine,
						new GermanTextProcessor.DictionaryWord("Deine", "Deine", "Deiner", "Deiner", "Deiner", "Deinen", "Deine", "Deine")
					},
					{
						GermanTextProcessor.WordGenderEnum.Neuter,
						new GermanTextProcessor.DictionaryWord("Dein", "Deine", "Deines", "Deiner", "Deinem", "Deinen", "Dein", "Deine")
					},
					{
						GermanTextProcessor.WordGenderEnum.Plural,
						new GermanTextProcessor.DictionaryWord("Deine", "Deine", "Deiner", "Deiner", "Deinen", "Deinen", "Deine", "Deine")
					}
				}
			},
			{
				"ihr",
				new Dictionary<GermanTextProcessor.WordGenderEnum, GermanTextProcessor.DictionaryWord>
				{
					{
						GermanTextProcessor.WordGenderEnum.Masculine,
						new GermanTextProcessor.DictionaryWord("Ihr", "Ihre", "Ihres", "Ihrer", "Ihrem", "Ihren", "Ihren", "Ihre")
					},
					{
						GermanTextProcessor.WordGenderEnum.Feminine,
						new GermanTextProcessor.DictionaryWord("Ihre", "Ihre", "Ihrer", "Ihrer", "Ihrer", "Ihren", "Ihre", "Ihre")
					},
					{
						GermanTextProcessor.WordGenderEnum.Neuter,
						new GermanTextProcessor.DictionaryWord("Ihr", "Ihre", "Ihres", "Ihrer", "Ihrem", "Ihren", "Ihr", "Ihre")
					},
					{
						GermanTextProcessor.WordGenderEnum.Plural,
						new GermanTextProcessor.DictionaryWord("Ihre", "Ihre", "Ihrer", "Ihrer", "Ihren", "Ihren", "Ihre", "Ihre")
					}
				}
			},
			{
				"euer",
				new Dictionary<GermanTextProcessor.WordGenderEnum, GermanTextProcessor.DictionaryWord>
				{
					{
						GermanTextProcessor.WordGenderEnum.Masculine,
						new GermanTextProcessor.DictionaryWord("Euer", "Eure", "Eures", "Eurer ", "Eurem", "Euren", "Euren", "Eure")
					},
					{
						GermanTextProcessor.WordGenderEnum.Feminine,
						new GermanTextProcessor.DictionaryWord("Eure", "Eure", "Eurer", "Eurer ", "Eurer", "Euren", "Eure", "Eure")
					},
					{
						GermanTextProcessor.WordGenderEnum.Neuter,
						new GermanTextProcessor.DictionaryWord("Euer", "Eure", "Eures", "Eurer ", "Eurem", "Euren", "Euer", "Eure")
					},
					{
						GermanTextProcessor.WordGenderEnum.Plural,
						new GermanTextProcessor.DictionaryWord("Eure", "Eure", "Eurer ", "Eurer ", "Euren", "Euren", "Eure", "Eure")
					}
				}
			},
			{
				"welcher",
				new Dictionary<GermanTextProcessor.WordGenderEnum, GermanTextProcessor.DictionaryWord>
				{
					{
						GermanTextProcessor.WordGenderEnum.Masculine,
						new GermanTextProcessor.DictionaryWord("Welcher", "Welche", "Welches", "Welcher", "Welchem", "Welchen", "Welchen", "Welche")
					},
					{
						GermanTextProcessor.WordGenderEnum.Feminine,
						new GermanTextProcessor.DictionaryWord("Welche", "Welche", "Welcher", "Welcher", "Welcher", "Welchen", "Welche", "Welche")
					},
					{
						GermanTextProcessor.WordGenderEnum.Neuter,
						new GermanTextProcessor.DictionaryWord("Welches", "Welche", "Welches", "Welcher", "Welchem", "Welchen", "Welches", "Welche")
					},
					{
						GermanTextProcessor.WordGenderEnum.Plural,
						new GermanTextProcessor.DictionaryWord("Welche", "Welche", "Welcher", "Welcher", "Welchen", "Welchen", "Welche", "Welche")
					}
				}
			}
		};

		private static readonly Dictionary<string, Dictionary<string, string>> ArticlePronounReplacementDictionary = new Dictionary<string, Dictionary<string, string>> { 
		{
			"dem",
			new Dictionary<string, string>
			{
				{ "von", "vom" },
				{ "vom", "vom" },
				{ "zu", "zum" },
				{ "an", "am" }
			}
		} };

		private static readonly Dictionary<char, List<string>> DoNotDeclineList = new Dictionary<char, List<string>>
		{
			{
				'A',
				new List<string> { "Avlonos", "Argoron", "Arkit", "Airit", "Aldusunit", "Asraloving", "Acapanos", "Angarys" }
			},
			{
				'B',
				new List<string>
				{
					"Banu Hulyan", "Banu Sarmal", "Banu Sarran", "Baltait", "Banu Atij", "Banu Qaraz", "Banu Ruwaid", "Beni Zilal", "Banu Habbab", "Banu Qild",
					"Banu Arbas", "Bochit", "Boranoving", "Bani Aska", "Bani Dhamin", "Bani Fasus", "Bani Julul", "Bani Kinyan", "Bani Laikh", "Bani Mushala",
					"Bani Nir", "Bani Tharuq", "Bani Yatash", "Bani Zus", "Balastisos"
				}
			},
			{
				'C',
				new List<string> { "Chonis", "Corenios", "Comnos", "Charait", "Corstases" }
			},
			{
				'D',
				new List<string>
				{
					"dey Meroc", "Dolentos", "dey Molarn", "dey Gunric", "dey Cortain", "dey Rothad", "dey Jelind", "dey Fortes", "dey Arromanc", "dey Tihr",
					"Dionicos", "dey Valant", "dey Folcun", "Delicos"
				}
			},
			{
				'E',
				new List<string> { "Eleftheroi", "Elaches", "Elysos" }
			},
			{
				'F',
				new List<string>
				{
					"fen Uvain", "fen Caernacht", "fen Gruffendoc", "fen Morcar", "fen Penraic", "fen Giall", "fen Eingal", "fen Derngil", "fen Aertus", "fen Brachar",
					"fen Crusac", "fen Domus", "fen Earach", "fen Fiachan", "fen Loen", "fen Morain", "fen Seanel", "fen Tuil", "Folyoroving"
				}
			},
			{
				'G',
				new List<string> { "Gundaroving", "Ghilman", "Gendiroving", "Gessios" }
			},
			{
				'H',
				new List<string> { "Hongeros", "Harfit" }
			},
			{
				'I',
				new List<string> { "Isyaroving", "Impestores", "Ingchit", "Iskanoving" }
			},
			{
				'J',
				new List<string> { "Julios", "Jawwal", "Jalos" }
			},
			{
				'K',
				new List<string> { "Karakhergit", "Koltit", "Kuloving", "Khergit", "Kostoroving" }
			},
			{
				'L',
				new List<string> { "Lonalion", "Leoniparden", "Lestharos" }
			},
			{
				'M',
				new List<string> { "Mestricaros", "Maneolis", "Maranjit", "Maregoving", "Meones" }
			},
			{
				'N',
				new List<string> { "Neutral", "Neretzes", "Nathanys" }
			},
			{
				'O',
				new List<string> { "Ormidoving", "Oburit", "Osticos", "Oranarit", "Opynates" }
			},
			{
				'P',
				new List<string> { "Prienicos", "Phalentes", "Pethros", "Palladios", "Paltos", "Phenigos" }
			},
			{
				'S',
				new List<string> { "Skolderbroda", "Seeratten", "Sorados", "Serapides", "Seeräuber", "Sunit", "Suratoving", "Stracanasthes", "Sumessos" }
			},
			{
				'T',
				new List<string> { "Tigrit", "Togaroving", "Tokhit", "Therycos" }
			},
			{
				'U',
				new List<string> { "Urkhunait", "Ubroving", "Ubchit" }
			},
			{
				'V',
				new List<string> { "Varros", "Vizartos", "Vagiroving", "Verborgene Hand", "Vatatzes", "Vetranis", "Vezhoving", "Vyshoving" }
			},
			{
				'Y',
				new List<string> { "Yanserit", "Yujit", "Yerchoving" }
			},
			{
				'Z',
				new List<string> { "Zhanoving", "Zebales" }
			}
		};

		private enum WordGenderEnum
		{
			Masculine,
			Feminine,
			Neuter,
			Plural,
			NoDeclination
		}

		private static class NounTokens
		{
			public const string Nominative = ".n";

			public const string Accusative = ".a";

			public const string Genitive = ".g";

			public const string Dative = ".d";

			public const string NominativePlural = ".np";

			public const string AccusativePlural = ".ap";

			public const string GenitivePlural = ".gp";

			public const string DativePlural = ".dp";

			public static readonly string[] TokenList = new string[] { ".n", ".a", ".g", ".d", ".np", ".ap", ".gp", ".dp" };
		}

		private static class AdjectiveTokens
		{
			public const string WeakNominative = ".wn";

			public const string MixedNominative = ".mn";

			public const string StrongNominative = ".sn";

			public const string WeakAccusative = ".wa";

			public const string MixedAccusative = ".ma";

			public const string StrongAccusative = ".sa";

			public const string WeakDative = ".wd";

			public const string MixedDative = ".md";

			public const string StrongDative = ".sd";

			public const string WeakGenitive = ".wg";

			public const string MixedGenitive = ".mg";

			public const string StrongGenitive = ".sg";

			public const string WeakNominativePlural = ".wnp";

			public const string MixedNominativePlural = ".mnp";

			public const string StrongNominativePlural = ".snp";

			public const string WeakAccusativePlural = ".wap";

			public const string MixedAccusativePlural = ".map";

			public const string StrongAccusativePlural = ".sap";

			public const string WeakDativePlural = ".wdp";

			public const string MixedDativePlural = ".mdp";

			public const string StrongDativePlural = ".sdp";

			public const string WeakGenitivePlural = ".wgp";

			public const string MixedGenitivePlural = ".mgp";

			public const string StrongGenitivePlural = ".sgp";

			public static readonly string[] TokenList = new string[]
			{
				".wn", ".mn", ".sn", ".wa", ".ma", ".sa", ".wd", ".md", ".sd", ".wg",
				".mg", ".sg", ".wnp", ".mnp", ".snp", ".wap", ".map", ".sap", ".wdp", ".mdp",
				".sdp", ".wgp", ".mgp", ".sgp"
			};
		}

		private static class PronounAndArticleTokens
		{
			public const string Nominative = ".pn";

			public const string Accusative = ".pa";

			public const string Genitive = ".pg";

			public const string Dative = ".pd";

			public const string NominativePlural = ".pnp";

			public const string AccusativePlural = ".pap";

			public const string GenitivePlural = ".pgp";

			public const string DativePlural = ".pdp";

			public static readonly string[] TokenList = new string[] { ".pn", ".pa", ".pg", ".pd", ".pnp", ".pap", ".pgp", ".pdp" };
		}

		private static class GenderTokens
		{
			public const string Masculine = ".M";

			public const string Feminine = ".F";

			public const string Neuter = ".N";

			public const string Plural = ".P";

			public static readonly string[] TokenList = new string[] { ".M", ".F", ".N", ".P" };
		}

		private static class WordGroupTokens
		{
			public const string NounNominative = ".nn";

			public const string PronounAndArticleNominative = ".pngroup";

			public const string AdjectiveNominativeWeak = ".ajw";

			public const string AdjectiveNominativeMixed = ".ajm";

			public const string AdjectiveNominativeStrong = ".ajs";

			public const string NounNominativeWithBrackets = "{.nn}";

			public const string PronounAndArticleNominativeWithBrackets = "{.pngroup}";

			public const string AdjectiveNominativeWeakWithBrackets = "{.ajw}";

			public const string AdjectiveNominativeMixedWithBrackets = "{.ajm}";

			public const string AdjectiveNominativeStrongWithBrackets = "{.ajs}";

			public const string NounNominativePlural = ".nnp";

			public const string PronounAndArticleNominativePlural = ".pnpgroup";

			public const string AdjectiveNominativeWeakPlural = ".ajwp";

			public const string AdjectiveNominativeMixedPlural = ".ajmp";

			public const string AdjectiveNominativeStrongPlural = ".ajsp";

			public const string NounNominativePluralWithBrackets = "{.nnp}";

			public const string PronounAndArticleNominativePluralWithBrackets = "{.pnpgroup}";

			public const string AdjectiveNominativeWeakPluralWithBrackets = "{.ajwp}";

			public const string AdjectiveNominativeMixedPluralWithBrackets = "{.ajmp}";

			public const string AdjectiveNominativeStrongPluralWithBrackets = "{.ajsp}";

			public static readonly string[] TokenList = new string[] { ".nn", ".pngroup", ".ajm", ".ajs", ".ajw", ".nnp", ".pnpgroup", ".ajmp", ".ajsp", ".ajwp" };
		}

		private static class OtherTokens
		{
			public const string PossessionToken = ".o";
		}

		private struct DictionaryWord
		{
			public DictionaryWord(string nominative, string nominativePlural, string genitive, string genitivePlural, string dative, string dativePlural, string accusative, string accusativePlural)
			{
				this.Nominative = nominative;
				this.NominativePlural = nominativePlural;
				this.Accusative = accusative;
				this.Genitive = genitive;
				this.Dative = dative;
				this.AccusativePlural = accusativePlural;
				this.GenitivePlural = genitivePlural;
				this.DativePlural = dativePlural;
			}

			public readonly string Nominative;

			public readonly string NominativePlural;

			public readonly string Accusative;

			public readonly string Genitive;

			public readonly string Dative;

			public readonly string AccusativePlural;

			public readonly string GenitivePlural;

			public readonly string DativePlural;
		}
	}
}
