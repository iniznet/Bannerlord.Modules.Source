using System;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	public static class HyperlinkTexts
	{
		public static TextObject GetSettlementHyperlinkText(string link, TextObject settlementName)
		{
			TextObject textObject = new TextObject("{=!}{.link}<a style=\"Link.Settlement\" href=\"event:{LINK}\"><b>{SETTLEMENT_NAME}</b></a>", null);
			textObject.SetTextVariable("LINK", link);
			textObject.SetTextVariable("SETTLEMENT_NAME", settlementName);
			return textObject;
		}

		public static TextObject GetKingdomHyperlinkText(string link, TextObject kingdomName)
		{
			TextObject textObject = new TextObject("{=!}{.link}<a style=\"Link.Kingdom\" href=\"event:{LINK}\"><b>{KINGDOM_NAME}</b></a>", null);
			textObject.SetTextVariable("LINK", link);
			textObject.SetTextVariable("KINGDOM_NAME", kingdomName);
			return textObject;
		}

		public static TextObject GetHeroHyperlinkText(string link, TextObject heroName)
		{
			TextObject textObject = new TextObject("{=!}{.link}<a style=\"Link.Hero\" href=\"event:{LINK}\"><b>{HERO_NAME}</b></a>", null);
			textObject.SetTextVariable("LINK", link);
			textObject.SetTextVariable("HERO_NAME", heroName);
			return textObject;
		}

		public static TextObject GetConceptHyperlinkText(string link, TextObject conceptName)
		{
			TextObject textObject = new TextObject("{=!}{.link}<a style=\"Link.Concept\" href=\"event:{LINK}\"><b>{CONCEPT_NAME}</b></a>", null);
			textObject.SetTextVariable("LINK", link);
			textObject.SetTextVariable("CONCEPT_NAME", conceptName);
			return textObject;
		}

		public static TextObject GetClanHyperlinkText(string link, TextObject clanName)
		{
			TextObject textObject = new TextObject("{=!}{.link}<a style=\"Link.Clan\" href=\"event:{LINK}\"><b>{CLAN_NAME}</b></a>", null);
			textObject.SetTextVariable("LINK", link);
			textObject.SetTextVariable("CLAN_NAME", clanName);
			return textObject;
		}

		public static TextObject GetUnitHyperlinkText(string link, TextObject unitName)
		{
			TextObject textObject = new TextObject("{=!}{.link}<a style=\"Link.Unit\" href=\"event:{LINK}\"><b>{UNIT_NAME}</b></a>", null);
			textObject.SetTextVariable("LINK", link);
			textObject.SetTextVariable("UNIT_NAME", unitName);
			return textObject;
		}

		public static string GetGenericHyperlinkText(string link, string name)
		{
			return string.Concat(new string[] { "<a style=\"Link\" href=\"event:", link, "\"><b>", name, "</b></a>" });
		}

		public static string GetKeyHyperlinkText(string keyID)
		{
			string text = "None";
			int num = 16;
			HyperlinkTexts.ConsoleType consoleType = HyperlinkTexts.ConsoleType.Xbox;
			uint num2 = <PrivateImplementationDetails>.ComputeStringHash(keyID);
			if (num2 <= 2083773698U)
			{
				if (num2 <= 1039550435U)
				{
					if (num2 <= 215134355U)
					{
						if (num2 <= 130952070U)
						{
							if (num2 <= 106449248U)
							{
								if (num2 <= 97396832U)
								{
									if (num2 != 75071339U)
									{
										if (num2 != 97396832U)
										{
											goto IL_155A;
										}
										if (!(keyID == "D3"))
										{
											goto IL_155A;
										}
									}
									else
									{
										if (!(keyID == "LeftAlt"))
										{
											goto IL_155A;
										}
										goto IL_1509;
									}
								}
								else if (num2 != 100894848U)
								{
									if (num2 != 106449248U)
									{
										goto IL_155A;
									}
									if (!(keyID == "RightMouseButton"))
									{
										goto IL_155A;
									}
									goto IL_1404;
								}
								else
								{
									if (!(keyID == "Extended"))
									{
										goto IL_155A;
									}
									goto IL_155A;
								}
							}
							else if (num2 <= 115203180U)
							{
								if (num2 != 114174451U)
								{
									if (num2 != 115203180U)
									{
										goto IL_155A;
									}
									if (!(keyID == "BackSpace"))
									{
										goto IL_155A;
									}
									goto IL_13E6;
								}
								else if (!(keyID == "D2"))
								{
									goto IL_155A;
								}
							}
							else if (num2 != 117964108U)
							{
								if (num2 != 130952070U)
								{
									goto IL_155A;
								}
								if (!(keyID == "D1"))
								{
									goto IL_155A;
								}
							}
							else
							{
								if (!(keyID == "NumpadMinus"))
								{
									goto IL_155A;
								}
								num = 24;
								text = "-";
								goto IL_155A;
							}
						}
						else if (num2 <= 181284927U)
						{
							if (num2 <= 147729689U)
							{
								if (num2 != 139650141U)
								{
									if (num2 != 147729689U)
									{
										goto IL_155A;
									}
									if (!(keyID == "D0"))
									{
										goto IL_155A;
									}
								}
								else
								{
									if (!(keyID == "OpenBraces"))
									{
										goto IL_155A;
									}
									goto IL_155A;
								}
							}
							else if (num2 != 164507308U)
							{
								if (num2 != 181284927U)
								{
									goto IL_155A;
								}
								if (!(keyID == "D6"))
								{
									goto IL_155A;
								}
							}
							else if (!(keyID == "D7"))
							{
								goto IL_155A;
							}
						}
						else if (num2 <= 198356736U)
						{
							if (num2 != 198062546U)
							{
								if (num2 != 198356736U)
								{
									goto IL_155A;
								}
								if (!(keyID == "F9"))
								{
									goto IL_155A;
								}
								goto IL_13D7;
							}
							else if (!(keyID == "D5"))
							{
								goto IL_155A;
							}
						}
						else if (num2 != 214840165U)
						{
							if (num2 != 215134355U)
							{
								goto IL_155A;
							}
							if (!(keyID == "F8"))
							{
								goto IL_155A;
							}
							goto IL_13D7;
						}
						else if (!(keyID == "D4"))
						{
							goto IL_155A;
						}
					}
					else if (num2 <= 382910545U)
					{
						if (num2 <= 302657454U)
						{
							if (num2 <= 265173022U)
							{
								if (num2 != 254900552U)
								{
									if (num2 != 265173022U)
									{
										goto IL_155A;
									}
									if (!(keyID == "D9"))
									{
										goto IL_155A;
									}
								}
								else
								{
									if (!(keyID == "Insert"))
									{
										goto IL_155A;
									}
									goto IL_155A;
								}
							}
							else if (num2 != 281950641U)
							{
								if (num2 != 302657454U)
								{
									goto IL_155A;
								}
								if (!(keyID == "ControllerRStickUp"))
								{
									goto IL_155A;
								}
								goto IL_154A;
							}
							else if (!(keyID == "D8"))
							{
								goto IL_155A;
							}
						}
						else if (num2 <= 354454743U)
						{
							if (num2 != 332577688U)
							{
								if (num2 != 354454743U)
								{
									goto IL_155A;
								}
								if (!(keyID == "ControllerRThumb"))
								{
									goto IL_155A;
								}
								num = ((consoleType == HyperlinkTexts.ConsoleType.Xbox) ? 12 : 10);
								text = "controllerrthumb";
								goto IL_155A;
							}
							else
							{
								if (!(keyID == "F1"))
								{
									goto IL_155A;
								}
								goto IL_13D7;
							}
						}
						else if (num2 != 366132926U)
						{
							if (num2 != 382910545U)
							{
								goto IL_155A;
							}
							if (!(keyID == "F2"))
							{
								goto IL_155A;
							}
							goto IL_13D7;
						}
						else
						{
							if (!(keyID == "F3"))
							{
								goto IL_155A;
							}
							goto IL_13D7;
						}
					}
					else if (num2 <= 433243402U)
					{
						if (num2 <= 399688164U)
						{
							if (num2 != 389828744U)
							{
								if (num2 != 399688164U)
								{
									goto IL_155A;
								}
								if (!(keyID == "F5"))
								{
									goto IL_155A;
								}
								goto IL_13D7;
							}
							else
							{
								if (!(keyID == "MouseScrollUp"))
								{
									goto IL_155A;
								}
								goto IL_1404;
							}
						}
						else if (num2 != 416465783U)
						{
							if (num2 != 433243402U)
							{
								goto IL_155A;
							}
							if (!(keyID == "F7"))
							{
								goto IL_155A;
							}
							goto IL_13D7;
						}
						else
						{
							if (!(keyID == "F4"))
							{
								goto IL_155A;
							}
							goto IL_13D7;
						}
					}
					else if (num2 <= 513712005U)
					{
						if (num2 != 450021021U)
						{
							if (num2 != 513712005U)
							{
								goto IL_155A;
							}
							if (!(keyID == "Right"))
							{
								goto IL_155A;
							}
							goto IL_13D7;
						}
						else
						{
							if (!(keyID == "F6"))
							{
								goto IL_155A;
							}
							goto IL_13D7;
						}
					}
					else if (num2 != 575450500U)
					{
						if (num2 != 1039550435U)
						{
							goto IL_155A;
						}
						if (!(keyID == "ControllerLDown"))
						{
							goto IL_155A;
						}
						goto IL_1413;
					}
					else
					{
						if (!(keyID == "Apostrophe"))
						{
							goto IL_155A;
						}
						goto IL_155A;
					}
				}
				else
				{
					if (num2 <= 1702612722U)
					{
						if (num2 <= 1231278590U)
						{
							if (num2 <= 1107541039U)
							{
								if (num2 <= 1050238388U)
								{
									if (num2 != 1044186795U)
									{
										if (num2 != 1050238388U)
										{
											goto IL_155A;
										}
										if (!(keyID == "Equals"))
										{
											goto IL_155A;
										}
										goto IL_155A;
									}
									else
									{
										if (!(keyID == "PageUp"))
										{
											goto IL_155A;
										}
										goto IL_155A;
									}
								}
								else if (num2 != 1081442551U)
								{
									if (num2 != 1107541039U)
									{
										goto IL_155A;
									}
									if (!(keyID == "ControllerRTrigger"))
									{
										goto IL_155A;
									}
								}
								else
								{
									if (!(keyID == "CloseBraces"))
									{
										goto IL_155A;
									}
									goto IL_155A;
								}
							}
							else if (num2 <= 1138704245U)
							{
								if (num2 != 1123244352U)
								{
									if (num2 != 1138704245U)
									{
										goto IL_155A;
									}
									if (!(keyID == "X1MouseButton"))
									{
										goto IL_155A;
									}
									goto IL_155A;
								}
								else
								{
									if (!(keyID == "Up"))
									{
										goto IL_155A;
									}
									goto IL_13D7;
								}
							}
							else if (num2 != 1174120482U)
							{
								if (num2 != 1231278590U)
								{
									goto IL_155A;
								}
								if (!(keyID == "RightControl"))
								{
									goto IL_155A;
								}
								goto IL_14FE;
							}
							else
							{
								if (!(keyID == "ControllerLUp"))
								{
									goto IL_155A;
								}
								goto IL_1413;
							}
						}
						else if (num2 <= 1469573738U)
						{
							if (num2 <= 1391791790U)
							{
								if (num2 != 1296647161U)
								{
									if (num2 != 1391791790U)
									{
										goto IL_155A;
									}
									if (!(keyID == "Home"))
									{
										goto IL_155A;
									}
									goto IL_155A;
								}
								else if (!(keyID == "ControllerLTrigger"))
								{
									goto IL_155A;
								}
							}
							else if (num2 != 1428210068U)
							{
								if (num2 != 1469573738U)
								{
									goto IL_155A;
								}
								if (!(keyID == "Delete"))
								{
									goto IL_155A;
								}
								goto IL_155A;
							}
							else
							{
								if (!(keyID == "LeftShift"))
								{
									goto IL_155A;
								}
								goto IL_14F3;
							}
						}
						else if (num2 <= 1537849368U)
						{
							if (num2 != 1529719870U)
							{
								if (num2 != 1537849368U)
								{
									goto IL_155A;
								}
								if (!(keyID == "ControllerROption"))
								{
									goto IL_155A;
								}
								num = 16;
								text = ((consoleType == HyperlinkTexts.ConsoleType.Ps4) ? (keyID.ToLower() + "_4") : keyID.ToLower());
								goto IL_155A;
							}
							else
							{
								if (!(keyID == "ControllerLLeft"))
								{
									goto IL_155A;
								}
								goto IL_1413;
							}
						}
						else if (num2 != 1650792303U)
						{
							if (num2 != 1702612722U)
							{
								goto IL_155A;
							}
							if (!(keyID == "ControllerRBumper"))
							{
								goto IL_155A;
							}
							goto IL_1438;
						}
						else
						{
							if (!(keyID == "ControllerRStickRight"))
							{
								goto IL_155A;
							}
							goto IL_154A;
						}
						num = 16;
						text = keyID.ToLower();
						goto IL_155A;
					}
					if (num2 <= 1893487785U)
					{
						if (num2 <= 1852896292U)
						{
							if (num2 <= 1806183147U)
							{
								if (num2 != 1706424088U)
								{
									if (num2 != 1806183147U)
									{
										goto IL_155A;
									}
									if (!(keyID == "MiddleMouseButton"))
									{
										goto IL_155A;
									}
									goto IL_1404;
								}
								else
								{
									if (!(keyID == "Comma"))
									{
										goto IL_155A;
									}
									goto IL_155A;
								}
							}
							else if (num2 != 1843154928U)
							{
								if (num2 != 1852896292U)
								{
									goto IL_155A;
								}
								if (!(keyID == "ControllerRLeft"))
								{
									goto IL_155A;
								}
								goto IL_1429;
							}
							else if (!(keyID == "Numpad4"))
							{
								goto IL_155A;
							}
						}
						else if (num2 <= 1868010299U)
						{
							if (num2 != 1859932547U)
							{
								if (num2 != 1868010299U)
								{
									goto IL_155A;
								}
								if (!(keyID == "ControllerRStick"))
								{
									goto IL_155A;
								}
								goto IL_154A;
							}
							else if (!(keyID == "Numpad5"))
							{
								goto IL_155A;
							}
						}
						else if (num2 != 1876710166U)
						{
							if (num2 != 1893487785U)
							{
								goto IL_155A;
							}
							if (!(keyID == "Numpad7"))
							{
								goto IL_155A;
							}
						}
						else if (!(keyID == "Numpad6"))
						{
							goto IL_155A;
						}
					}
					else if (num2 <= 1943820642U)
					{
						if (num2 <= 1910265404U)
						{
							if (num2 != 1898928778U)
							{
								if (num2 != 1910265404U)
								{
									goto IL_155A;
								}
								if (!(keyID == "Numpad0"))
								{
									goto IL_155A;
								}
							}
							else
							{
								if (!(keyID == "Slash"))
								{
									goto IL_155A;
								}
								goto IL_155A;
							}
						}
						else if (num2 != 1927043023U)
						{
							if (num2 != 1943820642U)
							{
								goto IL_155A;
							}
							if (!(keyID == "Numpad2"))
							{
								goto IL_155A;
							}
						}
						else if (!(keyID == "Numpad1"))
						{
							goto IL_155A;
						}
					}
					else if (num2 <= 2008406340U)
					{
						if (num2 != 1960598261U)
						{
							if (num2 != 2008406340U)
							{
								goto IL_155A;
							}
							if (!(keyID == "ControllerLStickUp"))
							{
								goto IL_155A;
							}
							goto IL_1526;
						}
						else if (!(keyID == "Numpad3"))
						{
							goto IL_155A;
						}
					}
					else if (num2 != 2044486356U)
					{
						if (num2 != 2061263975U)
						{
							if (num2 != 2083773698U)
							{
								goto IL_155A;
							}
							if (!(keyID == "ControllerRStickLeft"))
							{
								goto IL_155A;
							}
							goto IL_154A;
						}
						else if (!(keyID == "Numpad9"))
						{
							goto IL_155A;
						}
					}
					else if (!(keyID == "Numpad8"))
					{
						goto IL_155A;
					}
				}
				num = 24;
				text = keyID.Substring(keyID.Length - 1);
				goto IL_155A;
			}
			if (num2 <= 3373006507U)
			{
				if (num2 <= 2913305049U)
				{
					if (num2 <= 2457286800U)
					{
						if (num2 <= 2267317284U)
						{
							if (num2 <= 2144691513U)
							{
								if (num2 != 2112836247U)
								{
									if (num2 != 2144691513U)
									{
										goto IL_155A;
									}
									if (!(keyID == "ControllerRDown"))
									{
										goto IL_155A;
									}
									goto IL_1429;
								}
								else
								{
									if (!(keyID == "ControllerRStickDown"))
									{
										goto IL_155A;
									}
									goto IL_154A;
								}
							}
							else if (num2 != 2157724748U)
							{
								if (num2 != 2267317284U)
								{
									goto IL_155A;
								}
								if (!(keyID == "Period"))
								{
									goto IL_155A;
								}
								goto IL_155A;
							}
							else
							{
								if (!(keyID == "ControllerLStickLeft"))
								{
									goto IL_155A;
								}
								goto IL_1526;
							}
						}
						else if (num2 <= 2365054562U)
						{
							if (num2 != 2340347977U)
							{
								if (num2 != 2365054562U)
								{
									goto IL_155A;
								}
								if (!(keyID == "NumpadEnter"))
								{
									goto IL_155A;
								}
							}
							else
							{
								if (!(keyID == "Tilde"))
								{
									goto IL_155A;
								}
								num = 24;
								text = "tilde";
								goto IL_155A;
							}
						}
						else if (num2 != 2434225852U)
						{
							if (num2 != 2457286800U)
							{
								goto IL_155A;
							}
							if (!(keyID == "Left"))
							{
								goto IL_155A;
							}
							goto IL_13D7;
						}
						else
						{
							if (!(keyID == "RightAlt"))
							{
								goto IL_155A;
							}
							goto IL_1509;
						}
					}
					else if (num2 <= 2761510965U)
					{
						if (num2 <= 2728445041U)
						{
							if (num2 != 2595691489U)
							{
								if (num2 != 2728445041U)
								{
									goto IL_155A;
								}
								if (!(keyID == "ControllerLStickDown"))
								{
									goto IL_155A;
								}
								goto IL_1526;
							}
							else
							{
								if (!(keyID == "ControllerLStick"))
								{
									goto IL_155A;
								}
								goto IL_1526;
							}
						}
						else if (num2 != 2746130317U)
						{
							if (num2 != 2761510965U)
							{
								goto IL_155A;
							}
							if (!(keyID == "Down"))
							{
								goto IL_155A;
							}
							goto IL_13D7;
						}
						else
						{
							if (!(keyID == "ControllerLThumb"))
							{
								goto IL_155A;
							}
							num = ((consoleType == HyperlinkTexts.ConsoleType.Xbox) ? 12 : 10);
							text = "controllerlthumb";
							goto IL_155A;
						}
					}
					else if (num2 <= 2769091631U)
					{
						if (num2 != 2762355378U)
						{
							if (num2 != 2769091631U)
							{
								goto IL_155A;
							}
							if (!(keyID == "CapsLock"))
							{
								goto IL_155A;
							}
							goto IL_13E6;
						}
						else
						{
							if (!(keyID == "X2MouseButton"))
							{
								goto IL_155A;
							}
							goto IL_155A;
						}
					}
					else if (num2 != 2906557000U)
					{
						if (num2 != 2913305049U)
						{
							goto IL_155A;
						}
						if (!(keyID == "ControllerLStickRight"))
						{
							goto IL_155A;
						}
						goto IL_1526;
					}
					else
					{
						if (!(keyID == "ControllerLBumper"))
						{
							goto IL_155A;
						}
						goto IL_1438;
					}
				}
				else if (num2 <= 3241480638U)
				{
					if (num2 <= 3082514982U)
					{
						if (num2 <= 3001337907U)
						{
							if (num2 != 2952291245U)
							{
								if (num2 != 3001337907U)
								{
									goto IL_155A;
								}
								if (!(keyID == "LeftMouseButton"))
								{
									goto IL_155A;
								}
								goto IL_1404;
							}
							else if (!(keyID == "Enter"))
							{
								goto IL_155A;
							}
						}
						else if (num2 != 3036628469U)
						{
							if (num2 != 3082514982U)
							{
								goto IL_155A;
							}
							if (!(keyID == "Escape"))
							{
								goto IL_155A;
							}
							goto IL_13D7;
						}
						else
						{
							if (!(keyID == "LeftControl"))
							{
								goto IL_155A;
							}
							goto IL_14FE;
						}
					}
					else if (num2 <= 3222007936U)
					{
						if (num2 != 3093862813U)
						{
							if (num2 != 3222007936U)
							{
								goto IL_155A;
							}
							if (!(keyID == "E"))
							{
								goto IL_155A;
							}
							goto IL_13D7;
						}
						else
						{
							if (!(keyID == "NumpadPeriod"))
							{
								goto IL_155A;
							}
							goto IL_155A;
						}
					}
					else if (num2 != 3238785555U)
					{
						if (num2 != 3241480638U)
						{
							goto IL_155A;
						}
						if (!(keyID == "PageDown"))
						{
							goto IL_155A;
						}
						goto IL_155A;
					}
					else
					{
						if (!(keyID == "D"))
						{
							goto IL_155A;
						}
						goto IL_13D7;
					}
				}
				else if (num2 <= 3289118412U)
				{
					if (num2 <= 3255563174U)
					{
						if (num2 != 3250860581U)
						{
							if (num2 != 3255563174U)
							{
								goto IL_155A;
							}
							if (!(keyID == "G"))
							{
								goto IL_155A;
							}
							goto IL_13D7;
						}
						else
						{
							if (!(keyID == "Space"))
							{
								goto IL_155A;
							}
							goto IL_13E6;
						}
					}
					else if (num2 != 3272340793U)
					{
						if (num2 != 3289118412U)
						{
							goto IL_155A;
						}
						if (!(keyID == "A"))
						{
							goto IL_155A;
						}
						goto IL_13D7;
					}
					else
					{
						if (!(keyID == "F"))
						{
							goto IL_155A;
						}
						goto IL_13D7;
					}
				}
				else if (num2 <= 3322673650U)
				{
					if (num2 != 3294917732U)
					{
						if (num2 != 3322673650U)
						{
							goto IL_155A;
						}
						if (!(keyID == "C"))
						{
							goto IL_155A;
						}
						goto IL_13D7;
					}
					else
					{
						if (!(keyID == "NumpadPlus"))
						{
							goto IL_155A;
						}
						num = 24;
						text = "+";
						goto IL_155A;
					}
				}
				else if (num2 != 3339451269U)
				{
					if (num2 != 3356228888U)
					{
						if (num2 != 3373006507U)
						{
							goto IL_155A;
						}
						if (!(keyID == "L"))
						{
							goto IL_155A;
						}
						goto IL_13D7;
					}
					else
					{
						if (!(keyID == "M"))
						{
							goto IL_155A;
						}
						goto IL_13D7;
					}
				}
				else
				{
					if (!(keyID == "B"))
					{
						goto IL_155A;
					}
					goto IL_13D7;
				}
				num = 12;
				text = "enter";
				goto IL_155A;
			}
			if (num2 <= 3574337935U)
			{
				if (num2 <= 3473672221U)
				{
					if (num2 <= 3406561745U)
					{
						if (num2 <= 3388411298U)
						{
							if (num2 != 3388260431U)
							{
								if (num2 != 3388411298U)
								{
									goto IL_155A;
								}
								if (!(keyID == "ControllerLOption"))
								{
									goto IL_155A;
								}
								num = ((consoleType == HyperlinkTexts.ConsoleType.Xbox) ? 14 : 8);
								text = ((consoleType == HyperlinkTexts.ConsoleType.Ps4) ? (keyID.ToLower() + "_4") : keyID.ToLower());
								goto IL_155A;
							}
							else
							{
								if (!(keyID == "Minus"))
								{
									goto IL_155A;
								}
								goto IL_155A;
							}
						}
						else if (num2 != 3389784126U)
						{
							if (num2 != 3406561745U)
							{
								goto IL_155A;
							}
							if (!(keyID == "N"))
							{
								goto IL_155A;
							}
						}
						else if (!(keyID == "O"))
						{
							goto IL_155A;
						}
					}
					else if (num2 <= 3440116983U)
					{
						if (num2 != 3423339364U)
						{
							if (num2 != 3440116983U)
							{
								goto IL_155A;
							}
							if (!(keyID == "H"))
							{
								goto IL_155A;
							}
						}
						else if (!(keyID == "I"))
						{
							goto IL_155A;
						}
					}
					else if (num2 != 3456894602U)
					{
						if (num2 != 3473672221U)
						{
							goto IL_155A;
						}
						if (!(keyID == "J"))
						{
							goto IL_155A;
						}
					}
					else if (!(keyID == "K"))
					{
						goto IL_155A;
					}
				}
				else if (num2 <= 3507227459U)
				{
					if (num2 <= 3485937324U)
					{
						if (num2 != 3482547786U)
						{
							if (num2 != 3485937324U)
							{
								goto IL_155A;
							}
							if (!(keyID == "ControllerRUp"))
							{
								goto IL_155A;
							}
							goto IL_1429;
						}
						else
						{
							if (!(keyID == "End"))
							{
								goto IL_155A;
							}
							goto IL_155A;
						}
					}
					else if (num2 != 3490449840U)
					{
						if (num2 != 3507227459U)
						{
							goto IL_155A;
						}
						if (!(keyID == "T"))
						{
							goto IL_155A;
						}
					}
					else if (!(keyID == "U"))
					{
						goto IL_155A;
					}
				}
				else if (num2 <= 3540782697U)
				{
					if (num2 != 3524005078U)
					{
						if (num2 != 3540782697U)
						{
							goto IL_155A;
						}
						if (!(keyID == "V"))
						{
							goto IL_155A;
						}
					}
					else if (!(keyID == "W"))
					{
						goto IL_155A;
					}
				}
				else if (num2 != 3557560316U)
				{
					if (num2 != 3574337935U)
					{
						goto IL_155A;
					}
					if (!(keyID == "P"))
					{
						goto IL_155A;
					}
				}
				else if (!(keyID == "Q"))
				{
					goto IL_155A;
				}
			}
			else if (num2 <= 3736956062U)
			{
				if (num2 <= 3691781268U)
				{
					if (num2 <= 3592460967U)
					{
						if (num2 != 3591115554U)
						{
							if (num2 != 3592460967U)
							{
								goto IL_155A;
							}
							if (!(keyID == "RightShift"))
							{
								goto IL_155A;
							}
							goto IL_14F3;
						}
						else if (!(keyID == "S"))
						{
							goto IL_155A;
						}
					}
					else if (num2 != 3607893173U)
					{
						if (num2 != 3691781268U)
						{
							goto IL_155A;
						}
						if (!(keyID == "Y"))
						{
							goto IL_155A;
						}
					}
					else if (!(keyID == "R"))
					{
						goto IL_155A;
					}
				}
				else if (num2 <= 3708558887U)
				{
					if (num2 != 3703400824U)
					{
						if (num2 != 3708558887U)
						{
							goto IL_155A;
						}
						if (!(keyID == "X"))
						{
							goto IL_155A;
						}
					}
					else if (!(keyID == "F10"))
					{
						goto IL_155A;
					}
				}
				else if (num2 != 3720178443U)
				{
					if (num2 != 3736956062U)
					{
						goto IL_155A;
					}
					if (!(keyID == "F12"))
					{
						goto IL_155A;
					}
				}
				else if (!(keyID == "F11"))
				{
					goto IL_155A;
				}
			}
			else if (num2 <= 3821858654U)
			{
				if (num2 <= 3737220883U)
				{
					if (num2 != 3737177789U)
					{
						if (num2 != 3737220883U)
						{
							goto IL_155A;
						}
						if (!(keyID == "ControllerLRight"))
						{
							goto IL_155A;
						}
						goto IL_1413;
					}
					else
					{
						if (!(keyID == "MouseScrollDown"))
						{
							goto IL_155A;
						}
						goto IL_1404;
					}
				}
				else if (num2 != 3742114125U)
				{
					if (num2 != 3821858654U)
					{
						goto IL_155A;
					}
					if (!(keyID == "SemiColon"))
					{
						goto IL_155A;
					}
					goto IL_155A;
				}
				else if (!(keyID == "Z"))
				{
					goto IL_155A;
				}
			}
			else if (num2 <= 3890594748U)
			{
				if (num2 != 3862950033U)
				{
					if (num2 != 3890594748U)
					{
						goto IL_155A;
					}
					if (!(keyID == "NumpadMultiply"))
					{
						goto IL_155A;
					}
					goto IL_155A;
				}
				else
				{
					if (!(keyID == "ControllerRRight"))
					{
						goto IL_155A;
					}
					goto IL_1429;
				}
			}
			else if (num2 != 4080261303U)
			{
				if (num2 != 4149056477U)
				{
					if (num2 != 4219689196U)
					{
						goto IL_155A;
					}
					if (!(keyID == "Tab"))
					{
						goto IL_155A;
					}
					num = 12;
					text = keyID.ToLower();
					goto IL_155A;
				}
				else
				{
					if (!(keyID == "NumpadSlash"))
					{
						goto IL_155A;
					}
					goto IL_155A;
				}
			}
			else
			{
				if (!(keyID == "BackSlash"))
				{
					goto IL_155A;
				}
				goto IL_155A;
			}
			IL_13D7:
			num = 24;
			text = keyID.ToLower();
			goto IL_155A;
			IL_13E6:
			num = 10;
			text = keyID.ToLower();
			goto IL_155A;
			IL_1404:
			num = 16;
			text = keyID.ToLower();
			goto IL_155A;
			IL_1413:
			num = ((consoleType == HyperlinkTexts.ConsoleType.Xbox) ? 16 : 10);
			text = keyID.ToLower();
			goto IL_155A;
			IL_1429:
			num = 14;
			text = keyID.ToLower();
			goto IL_155A;
			IL_1438:
			num = ((consoleType == HyperlinkTexts.ConsoleType.Xbox) ? 14 : 20);
			text = keyID.ToLower();
			goto IL_155A;
			IL_14F3:
			num = 14;
			text = "shift";
			goto IL_155A;
			IL_14FE:
			num = 12;
			text = "control";
			goto IL_155A;
			IL_1509:
			num = 24;
			text = "alt";
			goto IL_155A;
			IL_1526:
			num = ((consoleType == HyperlinkTexts.ConsoleType.Xbox) ? 12 : 10);
			text = "controllerlstick";
			goto IL_155A;
			IL_154A:
			num = ((consoleType == HyperlinkTexts.ConsoleType.Xbox) ? 12 : 10);
			text = "controllerrstick";
			IL_155A:
			if (consoleType == HyperlinkTexts.ConsoleType.Ps4 || consoleType == HyperlinkTexts.ConsoleType.Ps5)
			{
				text += "_ps";
			}
			return string.Format("<img src=\"General\\InputKeys\\{0}\" extend=\"{1}\">", text, num);
		}

		public const string GoldIcon = "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">";

		public const string MoraleIcon = "{=!}<img src=\"General\\Icons\\Morale@2x\" extend=\"8\">";

		public const string InfluenceIcon = "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"7\">";

		public const string IssueAvailableIcon = "{=!}<img src=\"General\\Icons\\icon_issue_available_square\" extend=\"4\">";

		public const string IssueActiveIcon = "{=!}<img src=\"General\\Icons\\icon_issue_active_square\" extend=\"4\">";

		public const string TrackedIssueIcon = "{=!}<img src=\"General\\Icons\\issue_target_icon\" extend=\"4\">";

		public const string QuestAvailableIcon = "{=!}<img src=\"General\\Icons\\icon_quest_available_square\" extend=\"4\">";

		public const string QuestActiveIcon = "{=!}<img src=\"General\\Icons\\icon_issue_active_square\" extend=\"4\">";

		public const string StoryQuestActiveIcon = "{=!}<img src=\"General\\Icons\\icon_story_quest_active_square\" extend=\"4\">";

		public const string TrackedStoryQuestIcon = "{=!}<img src=\"General\\Icons\\quest_target_icon\" extend=\"4\">";

		public const string InPrisonIcon = "{=!}<img src=\"Clan\\Status\\icon_inprison\">";

		public const string ChildIcon = "{=!}<img src=\"Clan\\Status\\icon_ischild\">";

		public const string PregnantIcon = "{=!}<img src=\"Clan\\Status\\icon_pregnant\">";

		public const string IllIcon = "{=!}<img src=\"Clan\\Status\\icon_terminallyill\">";

		public const string HeirIcon = "{=!}<img src=\"Clan\\Status\\icon_heir\">";

		public const string UnreadIcon = "{=!}<img src=\"MapMenuUnread2x\">";

		public const string UnselectedPerkIcon = "{=!}<img src=\"CharacterDeveloper\\UnselectedPerksIcon\" extend=\"2\">";

		public const string HorseIcon = "{=!}<img src=\"StdAssets\\ItemIcons\\Mount\" extend=\"16\">";

		public const string CrimeIcon = "{=!}<img src=\"SPGeneral\\MapOverlay\\Settlement\\icon_crime\" extend=\"16\">";

		public const string UpgradeAvailableIcon = "{=!}<img src=\"PartyScreen\\upgrade_icon\" extend=\"5\">";

		private enum ConsoleType
		{
			Xbox,
			Ps4,
			Ps5
		}
	}
}
