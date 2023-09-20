using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	// Token: 0x0200000B RID: 11
	public class InputKeyVisualWidget : Widget
	{
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x00003B2C File Offset: 0x00001D2C
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x00003B34 File Offset: 0x00001D34
		public bool HideIfNone { get; set; }

		// Token: 0x060000AA RID: 170 RVA: 0x00003B3D File Offset: 0x00001D3D
		public InputKeyVisualWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00003B5C File Offset: 0x00001D5C
		private string GetKeyVisualName(string keyID)
		{
			string text = "None";
			uint num = <PrivateImplementationDetails>.ComputeStringHash(keyID);
			if (num <= 2144691513U)
			{
				if (num <= 1107541039U)
				{
					if (num <= 265173022U)
					{
						if (num <= 139650141U)
						{
							if (num <= 106449248U)
							{
								if (num <= 97396832U)
								{
									if (num != 75071339U)
									{
										if (num != 97396832U)
										{
											return text;
										}
										if (!(keyID == "D3"))
										{
											return text;
										}
										goto IL_15AB;
									}
									else
									{
										if (!(keyID == "LeftAlt"))
										{
											return text;
										}
										goto IL_160B;
									}
								}
								else if (num != 100894848U)
								{
									if (num != 106449248U)
									{
										return text;
									}
									if (!(keyID == "RightMouseButton"))
									{
										return text;
									}
									goto IL_1572;
								}
								else
								{
									if (!(keyID == "Extended"))
									{
										return text;
									}
									return text;
								}
							}
							else if (num <= 115203180U)
							{
								if (num != 114174451U)
								{
									if (num != 115203180U)
									{
										return text;
									}
									if (!(keyID == "BackSpace"))
									{
										return text;
									}
									goto IL_1572;
								}
								else
								{
									if (!(keyID == "D2"))
									{
										return text;
									}
									goto IL_15A0;
								}
							}
							else if (num != 117964108U)
							{
								if (num != 130952070U)
								{
									if (num != 139650141U)
									{
										return text;
									}
									if (!(keyID == "OpenBraces"))
									{
										return text;
									}
									return text;
								}
								else
								{
									if (!(keyID == "D1"))
									{
										return text;
									}
									goto IL_1595;
								}
							}
							else
							{
								if (!(keyID == "NumpadMinus"))
								{
									return text;
								}
								return "-";
							}
						}
						else if (num <= 198062546U)
						{
							if (num <= 164507308U)
							{
								if (num != 147729689U)
								{
									if (num != 164507308U)
									{
										return text;
									}
									if (!(keyID == "D7"))
									{
										return text;
									}
									goto IL_15CB;
								}
								else if (!(keyID == "D0"))
								{
									return text;
								}
							}
							else if (num != 181284927U)
							{
								if (num != 198062546U)
								{
									return text;
								}
								if (!(keyID == "D5"))
								{
									return text;
								}
								goto IL_15BB;
							}
							else
							{
								if (!(keyID == "D6"))
								{
									return text;
								}
								goto IL_15C3;
							}
						}
						else if (num <= 214840165U)
						{
							if (num != 198356736U)
							{
								if (num != 214840165U)
								{
									return text;
								}
								if (!(keyID == "D4"))
								{
									return text;
								}
								goto IL_15B3;
							}
							else
							{
								if (!(keyID == "F9"))
								{
									return text;
								}
								goto IL_1572;
							}
						}
						else if (num != 215134355U)
						{
							if (num != 254900552U)
							{
								if (num != 265173022U)
								{
									return text;
								}
								if (!(keyID == "D9"))
								{
									return text;
								}
								goto IL_15DB;
							}
							else
							{
								if (!(keyID == "Insert"))
								{
									return text;
								}
								return text;
							}
						}
						else
						{
							if (!(keyID == "F8"))
							{
								return text;
							}
							goto IL_1572;
						}
					}
					else if (num <= 416465783U)
					{
						if (num <= 354454743U)
						{
							if (num <= 302657454U)
							{
								if (num != 281950641U)
								{
									if (num != 302657454U)
									{
										return text;
									}
									if (!(keyID == "ControllerRStickUp"))
									{
										return text;
									}
									goto IL_162B;
								}
								else
								{
									if (!(keyID == "D8"))
									{
										return text;
									}
									goto IL_15D3;
								}
							}
							else if (num != 332577688U)
							{
								if (num != 354454743U)
								{
									return text;
								}
								if (!(keyID == "ControllerRThumb"))
								{
									return text;
								}
								return "controllerrthumb";
							}
							else
							{
								if (!(keyID == "F1"))
								{
									return text;
								}
								goto IL_1572;
							}
						}
						else if (num <= 382910545U)
						{
							if (num != 366132926U)
							{
								if (num != 382910545U)
								{
									return text;
								}
								if (!(keyID == "F2"))
								{
									return text;
								}
								goto IL_1572;
							}
							else
							{
								if (!(keyID == "F3"))
								{
									return text;
								}
								goto IL_1572;
							}
						}
						else if (num != 389828744U)
						{
							if (num != 399688164U)
							{
								if (num != 416465783U)
								{
									return text;
								}
								if (!(keyID == "F4"))
								{
									return text;
								}
								goto IL_1572;
							}
							else
							{
								if (!(keyID == "F5"))
								{
									return text;
								}
								goto IL_1572;
							}
						}
						else
						{
							if (!(keyID == "MouseScrollUp"))
							{
								return text;
							}
							goto IL_1572;
						}
					}
					else if (num <= 575450500U)
					{
						if (num <= 450021021U)
						{
							if (num != 433243402U)
							{
								if (num != 450021021U)
								{
									return text;
								}
								if (!(keyID == "F6"))
								{
									return text;
								}
								goto IL_1572;
							}
							else
							{
								if (!(keyID == "F7"))
								{
									return text;
								}
								goto IL_1572;
							}
						}
						else if (num != 513712005U)
						{
							if (num != 575450500U)
							{
								return text;
							}
							if (!(keyID == "Apostrophe"))
							{
								return text;
							}
							return text;
						}
						else
						{
							if (!(keyID == "Right"))
							{
								return text;
							}
							goto IL_1572;
						}
					}
					else if (num <= 1044186795U)
					{
						if (num != 1039550435U)
						{
							if (num != 1044186795U)
							{
								return text;
							}
							if (!(keyID == "PageUp"))
							{
								return text;
							}
							return text;
						}
						else
						{
							if (!(keyID == "ControllerLDown"))
							{
								return text;
							}
							goto IL_1572;
						}
					}
					else if (num != 1050238388U)
					{
						if (num != 1081442551U)
						{
							if (num != 1107541039U)
							{
								return text;
							}
							if (!(keyID == "ControllerRTrigger"))
							{
								return text;
							}
							goto IL_1572;
						}
						else
						{
							if (!(keyID == "CloseBraces"))
							{
								return text;
							}
							return text;
						}
					}
					else
					{
						if (!(keyID == "Equals"))
						{
							return text;
						}
						return text;
					}
				}
				else if (num <= 1706424088U)
				{
					if (num <= 1355078617U)
					{
						if (num <= 1231278590U)
						{
							if (num <= 1138704245U)
							{
								if (num != 1123244352U)
								{
									if (num != 1138704245U)
									{
										return text;
									}
									if (!(keyID == "X1MouseButton"))
									{
										return text;
									}
									return text;
								}
								else
								{
									if (!(keyID == "Up"))
									{
										return text;
									}
									goto IL_1572;
								}
							}
							else if (num != 1174120482U)
							{
								if (num != 1231278590U)
								{
									return text;
								}
								if (!(keyID == "RightControl"))
								{
									return text;
								}
								goto IL_1603;
							}
							else
							{
								if (!(keyID == "ControllerLUp"))
								{
									return text;
								}
								goto IL_1572;
							}
						}
						else if (num <= 1304745760U)
						{
							if (num != 1296647161U)
							{
								if (num != 1304745760U)
								{
									return text;
								}
								if (!(keyID == "F21"))
								{
									return text;
								}
								goto IL_1572;
							}
							else
							{
								if (!(keyID == "ControllerLTrigger"))
								{
									return text;
								}
								goto IL_1572;
							}
						}
						else if (num != 1321523379U)
						{
							if (num != 1338300998U)
							{
								if (num != 1355078617U)
								{
									return text;
								}
								if (!(keyID == "F22"))
								{
									return text;
								}
								goto IL_1572;
							}
							else
							{
								if (!(keyID == "F23"))
								{
									return text;
								}
								goto IL_1572;
							}
						}
						else
						{
							if (!(keyID == "F20"))
							{
								return text;
							}
							goto IL_1572;
						}
					}
					else if (num <= 1469573738U)
					{
						if (num <= 1391791790U)
						{
							if (num != 1388633855U)
							{
								if (num != 1391791790U)
								{
									return text;
								}
								if (!(keyID == "Home"))
								{
									return text;
								}
								return text;
							}
							else
							{
								if (!(keyID == "F24"))
								{
									return text;
								}
								goto IL_1572;
							}
						}
						else if (num != 1428210068U)
						{
							if (num != 1469573738U)
							{
								return text;
							}
							if (!(keyID == "Delete"))
							{
								return text;
							}
							return text;
						}
						else
						{
							if (!(keyID == "LeftShift"))
							{
								return text;
							}
							goto IL_15FB;
						}
					}
					else if (num <= 1537849368U)
					{
						if (num != 1529719870U)
						{
							if (num != 1537849368U)
							{
								return text;
							}
							if (!(keyID == "ControllerROption"))
							{
								return text;
							}
							goto IL_157E;
						}
						else
						{
							if (!(keyID == "ControllerLLeft"))
							{
								return text;
							}
							goto IL_1572;
						}
					}
					else if (num != 1650792303U)
					{
						if (num != 1702612722U)
						{
							if (num != 1706424088U)
							{
								return text;
							}
							if (!(keyID == "Comma"))
							{
								return text;
							}
							return text;
						}
						else
						{
							if (!(keyID == "ControllerRBumper"))
							{
								return text;
							}
							goto IL_1572;
						}
					}
					else
					{
						if (!(keyID == "ControllerRStickRight"))
						{
							return text;
						}
						goto IL_162B;
					}
				}
				else if (num <= 1910265404U)
				{
					if (num <= 1859932547U)
					{
						if (num <= 1843154928U)
						{
							if (num != 1806183147U)
							{
								if (num != 1843154928U)
								{
									return text;
								}
								if (!(keyID == "Numpad4"))
								{
									return text;
								}
								goto IL_15B3;
							}
							else
							{
								if (!(keyID == "MiddleMouseButton"))
								{
									return text;
								}
								goto IL_1572;
							}
						}
						else if (num != 1852896292U)
						{
							if (num != 1859932547U)
							{
								return text;
							}
							if (!(keyID == "Numpad5"))
							{
								return text;
							}
							goto IL_15BB;
						}
						else
						{
							if (!(keyID == "ControllerRLeft"))
							{
								return text;
							}
							goto IL_1572;
						}
					}
					else if (num <= 1876710166U)
					{
						if (num != 1868010299U)
						{
							if (num != 1876710166U)
							{
								return text;
							}
							if (!(keyID == "Numpad6"))
							{
								return text;
							}
							goto IL_15C3;
						}
						else
						{
							if (!(keyID == "ControllerRStick"))
							{
								return text;
							}
							goto IL_162B;
						}
					}
					else if (num != 1893487785U)
					{
						if (num != 1898928778U)
						{
							if (num != 1910265404U)
							{
								return text;
							}
							if (!(keyID == "Numpad0"))
							{
								return text;
							}
						}
						else
						{
							if (!(keyID == "Slash"))
							{
								return text;
							}
							return text;
						}
					}
					else
					{
						if (!(keyID == "Numpad7"))
						{
							return text;
						}
						goto IL_15CB;
					}
				}
				else if (num <= 2008406340U)
				{
					if (num <= 1943820642U)
					{
						if (num != 1927043023U)
						{
							if (num != 1943820642U)
							{
								return text;
							}
							if (!(keyID == "Numpad2"))
							{
								return text;
							}
							goto IL_15A0;
						}
						else
						{
							if (!(keyID == "Numpad1"))
							{
								return text;
							}
							goto IL_1595;
						}
					}
					else if (num != 1960598261U)
					{
						if (num != 2008406340U)
						{
							return text;
						}
						if (!(keyID == "ControllerLStickUp"))
						{
							return text;
						}
						goto IL_161B;
					}
					else
					{
						if (!(keyID == "Numpad3"))
						{
							return text;
						}
						goto IL_15AB;
					}
				}
				else if (num <= 2061263975U)
				{
					if (num != 2044486356U)
					{
						if (num != 2061263975U)
						{
							return text;
						}
						if (!(keyID == "Numpad9"))
						{
							return text;
						}
						goto IL_15DB;
					}
					else
					{
						if (!(keyID == "Numpad8"))
						{
							return text;
						}
						goto IL_15D3;
					}
				}
				else if (num != 2083773698U)
				{
					if (num != 2112836247U)
					{
						if (num != 2144691513U)
						{
							return text;
						}
						if (!(keyID == "ControllerRDown"))
						{
							return text;
						}
						goto IL_1572;
					}
					else
					{
						if (!(keyID == "ControllerRStickDown"))
						{
							return text;
						}
						goto IL_162B;
					}
				}
				else
				{
					if (!(keyID == "ControllerRStickLeft"))
					{
						return text;
					}
					goto IL_162B;
				}
				return "0";
				IL_1595:
				return "1";
				IL_15A0:
				return "2";
				IL_15AB:
				return "3";
				IL_15B3:
				return "4";
				IL_15BB:
				return "5";
				IL_15C3:
				return "6";
				IL_15CB:
				return "7";
				IL_15D3:
				return "8";
				IL_15DB:
				return "9";
				IL_162B:
				return "controllerrstick";
			}
			if (num <= 3423339364U)
			{
				if (num <= 3082514982U)
				{
					if (num <= 2746130317U)
					{
						if (num <= 2365054562U)
						{
							if (num <= 2267317284U)
							{
								if (num != 2157724748U)
								{
									if (num != 2267317284U)
									{
										return text;
									}
									if (!(keyID == "Period"))
									{
										return text;
									}
									return text;
								}
								else
								{
									if (!(keyID == "ControllerLStickLeft"))
									{
										return text;
									}
									goto IL_161B;
								}
							}
							else if (num != 2340347977U)
							{
								if (num != 2365054562U)
								{
									return text;
								}
								if (!(keyID == "NumpadEnter"))
								{
									return text;
								}
							}
							else
							{
								if (!(keyID == "Tilde"))
								{
									return text;
								}
								goto IL_1572;
							}
						}
						else if (num <= 2457286800U)
						{
							if (num != 2434225852U)
							{
								if (num != 2457286800U)
								{
									return text;
								}
								if (!(keyID == "Left"))
								{
									return text;
								}
								goto IL_1572;
							}
							else
							{
								if (!(keyID == "RightAlt"))
								{
									return text;
								}
								goto IL_160B;
							}
						}
						else if (num != 2595691489U)
						{
							if (num != 2728445041U)
							{
								if (num != 2746130317U)
								{
									return text;
								}
								if (!(keyID == "ControllerLThumb"))
								{
									return text;
								}
								return "controllerlthumb";
							}
							else
							{
								if (!(keyID == "ControllerLStickDown"))
								{
									return text;
								}
								goto IL_161B;
							}
						}
						else
						{
							if (!(keyID == "ControllerLStick"))
							{
								return text;
							}
							goto IL_161B;
						}
					}
					else if (num <= 2906557000U)
					{
						if (num <= 2762355378U)
						{
							if (num != 2761510965U)
							{
								if (num != 2762355378U)
								{
									return text;
								}
								if (!(keyID == "X2MouseButton"))
								{
									return text;
								}
								return text;
							}
							else
							{
								if (!(keyID == "Down"))
								{
									return text;
								}
								goto IL_1572;
							}
						}
						else if (num != 2769091631U)
						{
							if (num != 2906557000U)
							{
								return text;
							}
							if (!(keyID == "ControllerLBumper"))
							{
								return text;
							}
							goto IL_1572;
						}
						else
						{
							if (!(keyID == "CapsLock"))
							{
								return text;
							}
							goto IL_1572;
						}
					}
					else if (num <= 2952291245U)
					{
						if (num != 2913305049U)
						{
							if (num != 2952291245U)
							{
								return text;
							}
							if (!(keyID == "Enter"))
							{
								return text;
							}
						}
						else
						{
							if (!(keyID == "ControllerLStickRight"))
							{
								return text;
							}
							goto IL_161B;
						}
					}
					else if (num != 3001337907U)
					{
						if (num != 3036628469U)
						{
							if (num != 3082514982U)
							{
								return text;
							}
							if (!(keyID == "Escape"))
							{
								return text;
							}
							goto IL_1572;
						}
						else
						{
							if (!(keyID == "LeftControl"))
							{
								return text;
							}
							goto IL_1603;
						}
					}
					else
					{
						if (!(keyID == "LeftMouseButton"))
						{
							return text;
						}
						goto IL_1572;
					}
					return "enter";
				}
				if (num <= 3294917732U)
				{
					if (num <= 3241480638U)
					{
						if (num <= 3222007936U)
						{
							if (num != 3093862813U)
							{
								if (num != 3222007936U)
								{
									return text;
								}
								if (!(keyID == "E"))
								{
									return text;
								}
							}
							else
							{
								if (!(keyID == "NumpadPeriod"))
								{
									return text;
								}
								return text;
							}
						}
						else if (num != 3238785555U)
						{
							if (num != 3241480638U)
							{
								return text;
							}
							if (!(keyID == "PageDown"))
							{
								return text;
							}
							return text;
						}
						else if (!(keyID == "D"))
						{
							return text;
						}
					}
					else if (num <= 3255563174U)
					{
						if (num != 3250860581U)
						{
							if (num != 3255563174U)
							{
								return text;
							}
							if (!(keyID == "G"))
							{
								return text;
							}
						}
						else if (!(keyID == "Space"))
						{
							return text;
						}
					}
					else if (num != 3272340793U)
					{
						if (num != 3289118412U)
						{
							if (num != 3294917732U)
							{
								return text;
							}
							if (!(keyID == "NumpadPlus"))
							{
								return text;
							}
							return "+";
						}
						else if (!(keyID == "A"))
						{
							return text;
						}
					}
					else if (!(keyID == "F"))
					{
						return text;
					}
				}
				else if (num <= 3373006507U)
				{
					if (num <= 3339451269U)
					{
						if (num != 3322673650U)
						{
							if (num != 3339451269U)
							{
								return text;
							}
							if (!(keyID == "B"))
							{
								return text;
							}
						}
						else if (!(keyID == "C"))
						{
							return text;
						}
					}
					else if (num != 3356228888U)
					{
						if (num != 3373006507U)
						{
							return text;
						}
						if (!(keyID == "L"))
						{
							return text;
						}
					}
					else if (!(keyID == "M"))
					{
						return text;
					}
				}
				else if (num <= 3388411298U)
				{
					if (num != 3388260431U)
					{
						if (num != 3388411298U)
						{
							return text;
						}
						if (!(keyID == "ControllerLOption"))
						{
							return text;
						}
						goto IL_157E;
					}
					else
					{
						if (!(keyID == "Minus"))
						{
							return text;
						}
						return text;
					}
				}
				else if (num != 3389784126U)
				{
					if (num != 3406561745U)
					{
						if (num != 3423339364U)
						{
							return text;
						}
						if (!(keyID == "I"))
						{
							return text;
						}
					}
					else if (!(keyID == "N"))
					{
						return text;
					}
				}
				else if (!(keyID == "O"))
				{
					return text;
				}
			}
			else if (num <= 3703400824U)
			{
				if (num <= 3540782697U)
				{
					if (num <= 3482547786U)
					{
						if (num <= 3456894602U)
						{
							if (num != 3440116983U)
							{
								if (num != 3456894602U)
								{
									return text;
								}
								if (!(keyID == "K"))
								{
									return text;
								}
							}
							else if (!(keyID == "H"))
							{
								return text;
							}
						}
						else if (num != 3473672221U)
						{
							if (num != 3482547786U)
							{
								return text;
							}
							if (!(keyID == "End"))
							{
								return text;
							}
							return text;
						}
						else if (!(keyID == "J"))
						{
							return text;
						}
					}
					else if (num <= 3490449840U)
					{
						if (num != 3485937324U)
						{
							if (num != 3490449840U)
							{
								return text;
							}
							if (!(keyID == "U"))
							{
								return text;
							}
						}
						else if (!(keyID == "ControllerRUp"))
						{
							return text;
						}
					}
					else if (num != 3507227459U)
					{
						if (num != 3524005078U)
						{
							if (num != 3540782697U)
							{
								return text;
							}
							if (!(keyID == "V"))
							{
								return text;
							}
						}
						else if (!(keyID == "W"))
						{
							return text;
						}
					}
					else if (!(keyID == "T"))
					{
						return text;
					}
				}
				else if (num <= 3585957491U)
				{
					if (num <= 3569179872U)
					{
						if (num != 3557560316U)
						{
							if (num != 3569179872U)
							{
								return text;
							}
							if (!(keyID == "F18"))
							{
								return text;
							}
						}
						else if (!(keyID == "Q"))
						{
							return text;
						}
					}
					else if (num != 3574337935U)
					{
						if (num != 3585957491U)
						{
							return text;
						}
						if (!(keyID == "F19"))
						{
							return text;
						}
					}
					else if (!(keyID == "P"))
					{
						return text;
					}
				}
				else if (num <= 3592460967U)
				{
					if (num != 3591115554U)
					{
						if (num != 3592460967U)
						{
							return text;
						}
						if (!(keyID == "RightShift"))
						{
							return text;
						}
						goto IL_15FB;
					}
					else if (!(keyID == "S"))
					{
						return text;
					}
				}
				else if (num != 3607893173U)
				{
					if (num != 3691781268U)
					{
						if (num != 3703400824U)
						{
							return text;
						}
						if (!(keyID == "F10"))
						{
							return text;
						}
					}
					else if (!(keyID == "Y"))
					{
						return text;
					}
				}
				else if (!(keyID == "R"))
				{
					return text;
				}
			}
			else if (num <= 3787288919U)
			{
				if (num <= 3737177789U)
				{
					if (num <= 3720178443U)
					{
						if (num != 3708558887U)
						{
							if (num != 3720178443U)
							{
								return text;
							}
							if (!(keyID == "F11"))
							{
								return text;
							}
						}
						else if (!(keyID == "X"))
						{
							return text;
						}
					}
					else if (num != 3736956062U)
					{
						if (num != 3737177789U)
						{
							return text;
						}
						if (!(keyID == "MouseScrollDown"))
						{
							return text;
						}
					}
					else if (!(keyID == "F12"))
					{
						return text;
					}
				}
				else if (num <= 3742114125U)
				{
					if (num != 3737220883U)
					{
						if (num != 3742114125U)
						{
							return text;
						}
						if (!(keyID == "Z"))
						{
							return text;
						}
					}
					else if (!(keyID == "ControllerLRight"))
					{
						return text;
					}
				}
				else if (num != 3753733681U)
				{
					if (num != 3770511300U)
					{
						if (num != 3787288919U)
						{
							return text;
						}
						if (!(keyID == "F15"))
						{
							return text;
						}
					}
					else if (!(keyID == "F14"))
					{
						return text;
					}
				}
				else if (!(keyID == "F13"))
				{
					return text;
				}
			}
			else if (num <= 3862950033U)
			{
				if (num <= 3820844157U)
				{
					if (num != 3804066538U)
					{
						if (num != 3820844157U)
						{
							return text;
						}
						if (!(keyID == "F17"))
						{
							return text;
						}
					}
					else if (!(keyID == "F16"))
					{
						return text;
					}
				}
				else if (num != 3821858654U)
				{
					if (num != 3862950033U)
					{
						return text;
					}
					if (!(keyID == "ControllerRRight"))
					{
						return text;
					}
				}
				else
				{
					if (!(keyID == "SemiColon"))
					{
						return text;
					}
					return text;
				}
			}
			else if (num <= 3958280132U)
			{
				if (num != 3890594748U)
				{
					if (num != 3958280132U)
					{
						return text;
					}
					if (!(keyID == "ControllerShare"))
					{
						return text;
					}
					return text;
				}
				else
				{
					if (!(keyID == "NumpadMultiply"))
					{
						return text;
					}
					return text;
				}
			}
			else if (num != 4080261303U)
			{
				if (num != 4149056477U)
				{
					if (num != 4219689196U)
					{
						return text;
					}
					if (!(keyID == "Tab"))
					{
						return text;
					}
				}
				else
				{
					if (!(keyID == "NumpadSlash"))
					{
						return text;
					}
					return text;
				}
			}
			else
			{
				if (!(keyID == "BackSlash"))
				{
					return text;
				}
				return text;
			}
			IL_1572:
			return keyID.ToLower();
			IL_157E:
			return keyID.ToLower();
			IL_15FB:
			return "shift";
			IL_1603:
			return "control";
			IL_160B:
			return "alt";
			IL_161B:
			text = "controllerlstick";
			return text;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x0000519C File Offset: 0x0000339C
		private void SetKeyVisual(string visualName)
		{
			if (visualName == "None" && this.HideIfNone)
			{
				base.IsVisible = false;
				return;
			}
			string text = this.IconsPath + "\\" + visualName;
			base.Sprite = base.Context.SpriteData.GetSprite(text);
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000AD RID: 173 RVA: 0x000051EF File Offset: 0x000033EF
		// (set) Token: 0x060000AE RID: 174 RVA: 0x000051F7 File Offset: 0x000033F7
		public string KeyID
		{
			get
			{
				return this._keyID;
			}
			set
			{
				if (value != this._keyID)
				{
					this._keyID = value;
					this._visualName = this.GetKeyVisualName(value);
					this.SetKeyVisual(this._visualName);
				}
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000AF RID: 175 RVA: 0x00005227 File Offset: 0x00003427
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x0000522F File Offset: 0x0000342F
		public string IconsPath
		{
			get
			{
				return this._iconsPath;
			}
			set
			{
				if (value != this._iconsPath)
				{
					this._iconsPath = value;
					this.SetKeyVisual(this._visualName);
				}
			}
		}

		// Token: 0x04000048 RID: 72
		private string _visualName = "None";

		// Token: 0x04000049 RID: 73
		private string _keyID;

		// Token: 0x0400004A RID: 74
		private string _iconsPath = "General\\InputKeys";
	}
}
