using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000181 RID: 385
	public static class BannerlordConfig
	{
		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x060013AC RID: 5036 RVA: 0x0004DCB0 File Offset: 0x0004BEB0
		public static int MinBattleSize
		{
			get
			{
				return BannerlordConfig._battleSizes[0];
			}
		}

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x060013AD RID: 5037 RVA: 0x0004DCB9 File Offset: 0x0004BEB9
		public static int MaxBattleSize
		{
			get
			{
				return BannerlordConfig._battleSizes[BannerlordConfig._battleSizes.Length - 1];
			}
		}

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x060013AE RID: 5038 RVA: 0x0004DCCA File Offset: 0x0004BECA
		public static int MinReinforcementWaveCount
		{
			get
			{
				return BannerlordConfig._reinforcementWaveCounts[0];
			}
		}

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x060013AF RID: 5039 RVA: 0x0004DCD3 File Offset: 0x0004BED3
		public static int MaxReinforcementWaveCount
		{
			get
			{
				return BannerlordConfig._reinforcementWaveCounts[BannerlordConfig._reinforcementWaveCounts.Length - 1];
			}
		}

		// Token: 0x060013B0 RID: 5040 RVA: 0x0004DCE4 File Offset: 0x0004BEE4
		public static void Initialize()
		{
			string text = Utilities.LoadBannerlordConfigFile();
			if (string.IsNullOrEmpty(text))
			{
				BannerlordConfig.Save();
			}
			else
			{
				bool flag = false;
				string[] array = text.Split(new char[] { '\n' });
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[] { '=' });
					PropertyInfo property = typeof(BannerlordConfig).GetProperty(array2[0]);
					if (property == null)
					{
						flag = true;
					}
					else
					{
						string text2 = array2[1];
						try
						{
							if (property.PropertyType == typeof(string))
							{
								string text3 = Regex.Replace(text2, "\\r", "");
								property.SetValue(null, text3);
							}
							else if (property.PropertyType == typeof(float))
							{
								float num;
								if (float.TryParse(text2, out num))
								{
									property.SetValue(null, num);
								}
								else
								{
									flag = true;
								}
							}
							else if (property.PropertyType == typeof(int))
							{
								int num2;
								if (int.TryParse(text2, out num2))
								{
									BannerlordConfig.ConfigPropertyInt customAttribute = property.GetCustomAttribute<BannerlordConfig.ConfigPropertyInt>();
									if (customAttribute == null || customAttribute.IsValidValue(num2))
									{
										property.SetValue(null, num2);
									}
									else
									{
										flag = true;
									}
								}
								else
								{
									flag = true;
								}
							}
							else if (property.PropertyType == typeof(bool))
							{
								bool flag2;
								if (bool.TryParse(text2, out flag2))
								{
									property.SetValue(null, flag2);
								}
								else
								{
									flag = true;
								}
							}
							else
							{
								flag = true;
								Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\BannerlordConfig.cs", "Initialize", 113);
							}
						}
						catch
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					BannerlordConfig.Save();
				}
				MBAPI.IMBBannerlordConfig.ValidateOptions();
			}
			MBTextManager.TryChangeVoiceLanguage(BannerlordConfig.VoiceLanguage);
			MBTextManager.ChangeLanguage(BannerlordConfig.Language);
			MBTextManager.LocalizationDebugMode = NativeConfig.LocalizationDebugMode;
		}

		// Token: 0x060013B1 RID: 5041 RVA: 0x0004DEE8 File Offset: 0x0004C0E8
		public static SaveResult Save()
		{
			Dictionary<PropertyInfo, object> dictionary = new Dictionary<PropertyInfo, object>();
			foreach (PropertyInfo propertyInfo in typeof(BannerlordConfig).GetProperties())
			{
				if (propertyInfo.GetCustomAttribute<BannerlordConfig.ConfigProperty>() != null)
				{
					dictionary.Add(propertyInfo, propertyInfo.GetValue(null, null));
				}
			}
			string text = "";
			foreach (KeyValuePair<PropertyInfo, object> keyValuePair in dictionary)
			{
				text = string.Concat(new string[]
				{
					text,
					keyValuePair.Key.Name,
					"=",
					keyValuePair.Value.ToString(),
					"\n"
				});
			}
			SaveResult saveResult = Utilities.SaveConfigFile(text);
			MBAPI.IMBBannerlordConfig.ValidateOptions();
			return saveResult;
		}

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x060013B2 RID: 5042 RVA: 0x0004DFC8 File Offset: 0x0004C1C8
		public static string DefaultLanguage
		{
			get
			{
				return BannerlordConfig.GetDefaultLanguage();
			}
		}

		// Token: 0x060013B3 RID: 5043 RVA: 0x0004DFCF File Offset: 0x0004C1CF
		public static int GetRealBattleSize()
		{
			return BannerlordConfig._battleSizes[BannerlordConfig.BattleSize];
		}

		// Token: 0x060013B4 RID: 5044 RVA: 0x0004DFDC File Offset: 0x0004C1DC
		public static int GetRealBattleSizeForSiege()
		{
			return BannerlordConfig._siegeBattleSizes[BannerlordConfig.BattleSize];
		}

		// Token: 0x060013B5 RID: 5045 RVA: 0x0004DFE9 File Offset: 0x0004C1E9
		public static int GetReinforcementWaveCount()
		{
			return BannerlordConfig._reinforcementWaveCounts[BannerlordConfig.ReinforcementWaveCount];
		}

		// Token: 0x060013B6 RID: 5046 RVA: 0x0004DFF6 File Offset: 0x0004C1F6
		public static int GetRealBattleSizeForSallyOut()
		{
			return BannerlordConfig._sallyOutBattleSizes[BannerlordConfig.BattleSize];
		}

		// Token: 0x060013B7 RID: 5047 RVA: 0x0004E003 File Offset: 0x0004C203
		private static string GetDefaultLanguage()
		{
			return LocalizedTextManager.GetLocalizationCodeOfISOLanguageCode(Utilities.GetSystemLanguage());
		}

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x060013B8 RID: 5048 RVA: 0x0004E00F File Offset: 0x0004C20F
		// (set) Token: 0x060013B9 RID: 5049 RVA: 0x0004E018 File Offset: 0x0004C218
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static string Language
		{
			get
			{
				return BannerlordConfig._language;
			}
			set
			{
				if (BannerlordConfig._language != value)
				{
					if (MBTextManager.LanguageExistsInCurrentConfiguration(value, NativeConfig.IsDevelopmentMode) && MBTextManager.ChangeLanguage(value))
					{
						BannerlordConfig._language = value;
					}
					else if (MBTextManager.ChangeLanguage("English"))
					{
						BannerlordConfig._language = "English";
					}
					else
					{
						Debug.FailedAssert("Language cannot be set!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\BannerlordConfig.cs", "Language", 351);
					}
					MBTextManager.LocalizationDebugMode = NativeConfig.LocalizationDebugMode;
				}
			}
		}

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x060013BA RID: 5050 RVA: 0x0004E08A File Offset: 0x0004C28A
		// (set) Token: 0x060013BB RID: 5051 RVA: 0x0004E094 File Offset: 0x0004C294
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static string VoiceLanguage
		{
			get
			{
				return BannerlordConfig._voiceLanguage;
			}
			set
			{
				if (BannerlordConfig._voiceLanguage != value)
				{
					if (MBTextManager.LanguageExistsInCurrentConfiguration(value, NativeConfig.IsDevelopmentMode) && MBTextManager.TryChangeVoiceLanguage(value))
					{
						BannerlordConfig._voiceLanguage = value;
						return;
					}
					if (MBTextManager.TryChangeVoiceLanguage("English"))
					{
						BannerlordConfig._voiceLanguage = "English";
						return;
					}
					Debug.FailedAssert("Voice Language cannot be set!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\BannerlordConfig.cs", "VoiceLanguage", 378);
				}
			}
		}

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x060013BC RID: 5052 RVA: 0x0004E0FA File Offset: 0x0004C2FA
		// (set) Token: 0x060013BD RID: 5053 RVA: 0x0004E101 File Offset: 0x0004C301
		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2 }, false)]
		public static int AttackDirectionControl { get; set; } = 1;

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x060013BE RID: 5054 RVA: 0x0004E109 File Offset: 0x0004C309
		// (set) Token: 0x060013BF RID: 5055 RVA: 0x0004E110 File Offset: 0x0004C310
		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2 }, false)]
		public static int DefendDirectionControl { get; set; } = 0;

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x060013C0 RID: 5056 RVA: 0x0004E118 File Offset: 0x0004C318
		// (set) Token: 0x060013C1 RID: 5057 RVA: 0x0004E11F File Offset: 0x0004C31F
		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2, 3, 4, 5 }, false)]
		public static int NumberOfCorpses
		{
			get
			{
				return BannerlordConfig._numberOfCorpses;
			}
			set
			{
				BannerlordConfig._numberOfCorpses = value;
			}
		}

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x060013C2 RID: 5058 RVA: 0x0004E127 File Offset: 0x0004C327
		// (set) Token: 0x060013C3 RID: 5059 RVA: 0x0004E12E File Offset: 0x0004C32E
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool ShowBlood { get; set; } = true;

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x060013C4 RID: 5060 RVA: 0x0004E136 File Offset: 0x0004C336
		// (set) Token: 0x060013C5 RID: 5061 RVA: 0x0004E13D File Offset: 0x0004C33D
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool DisplayAttackDirection { get; set; } = true;

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x060013C6 RID: 5062 RVA: 0x0004E145 File Offset: 0x0004C345
		// (set) Token: 0x060013C7 RID: 5063 RVA: 0x0004E14C File Offset: 0x0004C34C
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool DisplayTargetingReticule { get; set; } = true;

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x060013C8 RID: 5064 RVA: 0x0004E154 File Offset: 0x0004C354
		// (set) Token: 0x060013C9 RID: 5065 RVA: 0x0004E15B File Offset: 0x0004C35B
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool ForceVSyncInMenus { get; set; } = true;

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x060013CA RID: 5066 RVA: 0x0004E163 File Offset: 0x0004C363
		// (set) Token: 0x060013CB RID: 5067 RVA: 0x0004E16A File Offset: 0x0004C36A
		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2, 3, 4, 5, 6 }, false)]
		public static int BattleSize
		{
			get
			{
				return BannerlordConfig._battleSize;
			}
			set
			{
				BannerlordConfig._battleSize = value;
			}
		}

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x060013CC RID: 5068 RVA: 0x0004E172 File Offset: 0x0004C372
		// (set) Token: 0x060013CD RID: 5069 RVA: 0x0004E179 File Offset: 0x0004C379
		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2, 3 }, false)]
		public static int ReinforcementWaveCount { get; set; } = 3;

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x060013CE RID: 5070 RVA: 0x0004E181 File Offset: 0x0004C381
		public static float CivilianAgentCount
		{
			get
			{
				return (float)BannerlordConfig.GetRealBattleSize() * 0.5f;
			}
		}

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x060013CF RID: 5071 RVA: 0x0004E18F File Offset: 0x0004C38F
		// (set) Token: 0x060013D0 RID: 5072 RVA: 0x0004E196 File Offset: 0x0004C396
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static float FirstPersonFov { get; set; } = 65f;

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x060013D1 RID: 5073 RVA: 0x0004E19E File Offset: 0x0004C39E
		// (set) Token: 0x060013D2 RID: 5074 RVA: 0x0004E1A5 File Offset: 0x0004C3A5
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static float UIScale { get; set; } = 1f;

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x060013D3 RID: 5075 RVA: 0x0004E1AD File Offset: 0x0004C3AD
		// (set) Token: 0x060013D4 RID: 5076 RVA: 0x0004E1B4 File Offset: 0x0004C3B4
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static float CombatCameraDistance { get; set; } = 1f;

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x060013D5 RID: 5077 RVA: 0x0004E1BC File Offset: 0x0004C3BC
		// (set) Token: 0x060013D6 RID: 5078 RVA: 0x0004E1C3 File Offset: 0x0004C3C3
		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2, 3 }, false)]
		public static int TurnCameraWithHorseInFirstPerson { get; set; } = 2;

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x060013D7 RID: 5079 RVA: 0x0004E1CB File Offset: 0x0004C3CB
		// (set) Token: 0x060013D8 RID: 5080 RVA: 0x0004E1D2 File Offset: 0x0004C3D2
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool ReportDamage { get; set; } = true;

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x060013D9 RID: 5081 RVA: 0x0004E1DA File Offset: 0x0004C3DA
		// (set) Token: 0x060013DA RID: 5082 RVA: 0x0004E1E1 File Offset: 0x0004C3E1
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool ReportBark { get; set; } = true;

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x060013DB RID: 5083 RVA: 0x0004E1E9 File Offset: 0x0004C3E9
		// (set) Token: 0x060013DC RID: 5084 RVA: 0x0004E1F0 File Offset: 0x0004C3F0
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool LockTarget { get; set; } = false;

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x060013DD RID: 5085 RVA: 0x0004E1F8 File Offset: 0x0004C3F8
		// (set) Token: 0x060013DE RID: 5086 RVA: 0x0004E1FF File Offset: 0x0004C3FF
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableTutorialHints { get; set; } = true;

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x060013DF RID: 5087 RVA: 0x0004E207 File Offset: 0x0004C407
		// (set) Token: 0x060013E0 RID: 5088 RVA: 0x0004E20E File Offset: 0x0004C40E
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static int AutoSaveInterval
		{
			get
			{
				return BannerlordConfig._autoSaveInterval;
			}
			set
			{
				if (value == 4)
				{
					BannerlordConfig._autoSaveInterval = -1;
					return;
				}
				BannerlordConfig._autoSaveInterval = value;
			}
		}

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x060013E1 RID: 5089 RVA: 0x0004E221 File Offset: 0x0004C421
		// (set) Token: 0x060013E2 RID: 5090 RVA: 0x0004E228 File Offset: 0x0004C428
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static float FriendlyTroopsBannerOpacity { get; set; } = 1f;

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x060013E3 RID: 5091 RVA: 0x0004E230 File Offset: 0x0004C430
		// (set) Token: 0x060013E4 RID: 5092 RVA: 0x0004E237 File Offset: 0x0004C437
		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2 }, false)]
		public static int ReportCasualtiesType { get; set; } = 0;

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x060013E5 RID: 5093 RVA: 0x0004E23F File Offset: 0x0004C43F
		// (set) Token: 0x060013E6 RID: 5094 RVA: 0x0004E246 File Offset: 0x0004C446
		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2 }, false)]
		public static int AutoTrackAttackedSettlements { get; set; } = 0;

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x060013E7 RID: 5095 RVA: 0x0004E24E File Offset: 0x0004C44E
		// (set) Token: 0x060013E8 RID: 5096 RVA: 0x0004E255 File Offset: 0x0004C455
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool ReportPersonalDamage { get; set; } = true;

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x060013E9 RID: 5097 RVA: 0x0004E25D File Offset: 0x0004C45D
		// (set) Token: 0x060013EA RID: 5098 RVA: 0x0004E264 File Offset: 0x0004C464
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool SlowDownOnOrder { get; set; } = true;

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x060013EB RID: 5099 RVA: 0x0004E26C File Offset: 0x0004C46C
		// (set) Token: 0x060013EC RID: 5100 RVA: 0x0004E273 File Offset: 0x0004C473
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool StopGameOnFocusLost
		{
			get
			{
				return BannerlordConfig._stopGameOnFocusLost;
			}
			set
			{
				BannerlordConfig._stopGameOnFocusLost = value;
			}
		}

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x060013ED RID: 5101 RVA: 0x0004E27B File Offset: 0x0004C47B
		// (set) Token: 0x060013EE RID: 5102 RVA: 0x0004E282 File Offset: 0x0004C482
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool ReportExperience { get; set; } = true;

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x060013EF RID: 5103 RVA: 0x0004E28A File Offset: 0x0004C48A
		// (set) Token: 0x060013F0 RID: 5104 RVA: 0x0004E291 File Offset: 0x0004C491
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableDamageTakenVisuals { get; set; } = true;

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x060013F1 RID: 5105 RVA: 0x0004E299 File Offset: 0x0004C499
		// (set) Token: 0x060013F2 RID: 5106 RVA: 0x0004E2A0 File Offset: 0x0004C4A0
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableVerticalAimCorrection { get; set; } = true;

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x060013F3 RID: 5107 RVA: 0x0004E2A8 File Offset: 0x0004C4A8
		// (set) Token: 0x060013F4 RID: 5108 RVA: 0x0004E2AF File Offset: 0x0004C4AF
		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1 }, false)]
		public static int CrosshairType { get; set; } = 0;

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x060013F5 RID: 5109 RVA: 0x0004E2B7 File Offset: 0x0004C4B7
		// (set) Token: 0x060013F6 RID: 5110 RVA: 0x0004E2BE File Offset: 0x0004C4BE
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableGenericAvatars { get; set; } = false;

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x060013F7 RID: 5111 RVA: 0x0004E2C6 File Offset: 0x0004C4C6
		// (set) Token: 0x060013F8 RID: 5112 RVA: 0x0004E2CD File Offset: 0x0004C4CD
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableGenericNames { get; set; } = false;

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x060013F9 RID: 5113 RVA: 0x0004E2D5 File Offset: 0x0004C4D5
		// (set) Token: 0x060013FA RID: 5114 RVA: 0x0004E2DC File Offset: 0x0004C4DC
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool HideFullServers { get; set; } = false;

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x060013FB RID: 5115 RVA: 0x0004E2E4 File Offset: 0x0004C4E4
		// (set) Token: 0x060013FC RID: 5116 RVA: 0x0004E2EB File Offset: 0x0004C4EB
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool HideEmptyServers { get; set; } = false;

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x060013FD RID: 5117 RVA: 0x0004E2F3 File Offset: 0x0004C4F3
		// (set) Token: 0x060013FE RID: 5118 RVA: 0x0004E2FA File Offset: 0x0004C4FA
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool HidePasswordProtectedServers { get; set; } = false;

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x060013FF RID: 5119 RVA: 0x0004E302 File Offset: 0x0004C502
		// (set) Token: 0x06001400 RID: 5120 RVA: 0x0004E309 File Offset: 0x0004C509
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool HideUnofficialServers { get; set; } = false;

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x06001401 RID: 5121 RVA: 0x0004E311 File Offset: 0x0004C511
		// (set) Token: 0x06001402 RID: 5122 RVA: 0x0004E318 File Offset: 0x0004C518
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool HideModuleIncompatibleServers { get; set; } = false;

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x06001403 RID: 5123 RVA: 0x0004E320 File Offset: 0x0004C520
		// (set) Token: 0x06001404 RID: 5124 RVA: 0x0004E327 File Offset: 0x0004C527
		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1 }, false)]
		public static int OrderType
		{
			get
			{
				return BannerlordConfig._orderType;
			}
			set
			{
				BannerlordConfig._orderType = value;
			}
		}

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x06001405 RID: 5125 RVA: 0x0004E32F File Offset: 0x0004C52F
		// (set) Token: 0x06001406 RID: 5126 RVA: 0x0004E336 File Offset: 0x0004C536
		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1 }, false)]
		public static int OrderLayoutType
		{
			get
			{
				return BannerlordConfig._orderLayoutType;
			}
			set
			{
				BannerlordConfig._orderLayoutType = value;
			}
		}

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x06001407 RID: 5127 RVA: 0x0004E33E File Offset: 0x0004C53E
		// (set) Token: 0x06001408 RID: 5128 RVA: 0x0004E345 File Offset: 0x0004C545
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableVoiceChat { get; set; } = true;

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06001409 RID: 5129 RVA: 0x0004E34D File Offset: 0x0004C54D
		// (set) Token: 0x0600140A RID: 5130 RVA: 0x0004E354 File Offset: 0x0004C554
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableDeathIcon { get; set; } = true;

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x0600140B RID: 5131 RVA: 0x0004E35C File Offset: 0x0004C55C
		// (set) Token: 0x0600140C RID: 5132 RVA: 0x0004E363 File Offset: 0x0004C563
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableNetworkAlertIcons { get; set; } = true;

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x0600140D RID: 5133 RVA: 0x0004E36B File Offset: 0x0004C56B
		// (set) Token: 0x0600140E RID: 5134 RVA: 0x0004E372 File Offset: 0x0004C572
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableSingleplayerChatBox { get; set; } = true;

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x0600140F RID: 5135 RVA: 0x0004E37A File Offset: 0x0004C57A
		// (set) Token: 0x06001410 RID: 5136 RVA: 0x0004E381 File Offset: 0x0004C581
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableMultiplayerChatBox { get; set; } = true;

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x06001411 RID: 5137 RVA: 0x0004E389 File Offset: 0x0004C589
		// (set) Token: 0x06001412 RID: 5138 RVA: 0x0004E390 File Offset: 0x0004C590
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static float ChatBoxSizeX { get; set; } = 495f;

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x06001413 RID: 5139 RVA: 0x0004E398 File Offset: 0x0004C598
		// (set) Token: 0x06001414 RID: 5140 RVA: 0x0004E39F File Offset: 0x0004C59F
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static float ChatBoxSizeY { get; set; } = 340f;

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x06001415 RID: 5141 RVA: 0x0004E3A7 File Offset: 0x0004C5A7
		// (set) Token: 0x06001416 RID: 5142 RVA: 0x0004E3AE File Offset: 0x0004C5AE
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static string LatestSaveGameName { get; set; } = string.Empty;

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x06001417 RID: 5143 RVA: 0x0004E3B6 File Offset: 0x0004C5B6
		// (set) Token: 0x06001418 RID: 5144 RVA: 0x0004E3BD File Offset: 0x0004C5BD
		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool HideBattleUI { get; set; } = false;

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06001419 RID: 5145 RVA: 0x0004E3C5 File Offset: 0x0004C5C5
		// (set) Token: 0x0600141A RID: 5146 RVA: 0x0004E3CC File Offset: 0x0004C5CC
		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2, 3 }, false)]
		public static int UnitSpawnPrioritization { get; set; } = 0;

		// Token: 0x0400065C RID: 1628
		private static int[] _battleSizes = new int[] { 200, 300, 400, 500, 600, 800, 1000 };

		// Token: 0x0400065D RID: 1629
		private static int[] _siegeBattleSizes = new int[] { 150, 230, 320, 425, 540, 625, 1000 };

		// Token: 0x0400065E RID: 1630
		private static int[] _sallyOutBattleSizes = new int[] { 150, 200, 240, 280, 320, 360, 400 };

		// Token: 0x0400065F RID: 1631
		private static int[] _reinforcementWaveCounts = new int[] { 3, 4, 5, 0 };

		// Token: 0x04000660 RID: 1632
		public static double SiegeBattleSizeMultiplier = 0.8;

		// Token: 0x04000661 RID: 1633
		public const int DefaultAttackDirectionControl = 1;

		// Token: 0x04000662 RID: 1634
		public const int DefaultDefendDirectionControl = 0;

		// Token: 0x04000663 RID: 1635
		public const int DefaultNumberOfCorpses = 3;

		// Token: 0x04000664 RID: 1636
		public const bool DefaultShowBlood = true;

		// Token: 0x04000665 RID: 1637
		public const bool DefaultDisplayAttackDirection = true;

		// Token: 0x04000666 RID: 1638
		public const bool DefaultDisplayTargetingReticule = true;

		// Token: 0x04000667 RID: 1639
		public const bool DefaultForceVSyncInMenus = true;

		// Token: 0x04000668 RID: 1640
		public const int DefaultBattleSize = 2;

		// Token: 0x04000669 RID: 1641
		public const int DefaultReinforcementWaveCount = 3;

		// Token: 0x0400066A RID: 1642
		public const float DefaultBattleSizeMultiplier = 0.5f;

		// Token: 0x0400066B RID: 1643
		public const float DefaultFirstPersonFov = 65f;

		// Token: 0x0400066C RID: 1644
		public const float DefaultUIScale = 1f;

		// Token: 0x0400066D RID: 1645
		public const float DefaultCombatCameraDistance = 1f;

		// Token: 0x0400066E RID: 1646
		public const int DefaultCombatAI = 0;

		// Token: 0x0400066F RID: 1647
		public const int DefaultTurnCameraWithHorseInFirstPerson = 2;

		// Token: 0x04000670 RID: 1648
		public const int DefaultAutoSaveInterval = 30;

		// Token: 0x04000671 RID: 1649
		public const float DefaultFriendlyTroopsBannerOpacity = 1f;

		// Token: 0x04000672 RID: 1650
		public const bool DefaultReportDamage = true;

		// Token: 0x04000673 RID: 1651
		public const bool DefaultReportBark = true;

		// Token: 0x04000674 RID: 1652
		public const bool DefaultEnableTutorialHints = true;

		// Token: 0x04000675 RID: 1653
		public const int DefaultReportCasualtiesType = 0;

		// Token: 0x04000676 RID: 1654
		public const int DefaultAutoTrackAttackedSettlements = 0;

		// Token: 0x04000677 RID: 1655
		public const bool DefaultReportPersonalDamage = true;

		// Token: 0x04000678 RID: 1656
		public const bool DefaultStopGameOnFocusLost = true;

		// Token: 0x04000679 RID: 1657
		public const bool DefaultSlowDownOnOrder = true;

		// Token: 0x0400067A RID: 1658
		public const bool DefaultReportExperience = true;

		// Token: 0x0400067B RID: 1659
		public const bool DefaultEnableDamageTakenVisuals = true;

		// Token: 0x0400067C RID: 1660
		public const bool DefaultEnableVoiceChat = true;

		// Token: 0x0400067D RID: 1661
		public const bool DefaultEnableDeathIcon = true;

		// Token: 0x0400067E RID: 1662
		public const bool DefaultEnableNetworkAlertIcons = true;

		// Token: 0x0400067F RID: 1663
		public const bool DefaultEnableVerticalAimCorrection = true;

		// Token: 0x04000680 RID: 1664
		public const bool DefaultSingleplayerEnableChatBox = true;

		// Token: 0x04000681 RID: 1665
		public const bool DefaultMultiplayerEnableChatBox = true;

		// Token: 0x04000682 RID: 1666
		public const float DefaultChatBoxSizeX = 495f;

		// Token: 0x04000683 RID: 1667
		public const float DefaultChatBoxSizeY = 340f;

		// Token: 0x04000684 RID: 1668
		public const int DefaultCrosshairType = 0;

		// Token: 0x04000685 RID: 1669
		public const bool DefaultEnableGenericAvatars = false;

		// Token: 0x04000686 RID: 1670
		public const bool DefaultEnableGenericNames = false;

		// Token: 0x04000687 RID: 1671
		public const bool DefaultHideFullServers = false;

		// Token: 0x04000688 RID: 1672
		public const bool DefaultHideEmptyServers = false;

		// Token: 0x04000689 RID: 1673
		public const bool DefaultHidePasswordProtectedServers = false;

		// Token: 0x0400068A RID: 1674
		public const bool DefaultHideUnofficialServers = false;

		// Token: 0x0400068B RID: 1675
		public const bool DefaultHideModuleIncompatibleServers = false;

		// Token: 0x0400068C RID: 1676
		public const int DefaultOrderLayoutType = 0;

		// Token: 0x0400068D RID: 1677
		public const bool DefaultHideBattleUI = false;

		// Token: 0x0400068E RID: 1678
		public const int DefaultUnitSpawnPrioritization = 0;

		// Token: 0x0400068F RID: 1679
		public const int DefaultOrderType = 0;

		// Token: 0x04000690 RID: 1680
		public const bool DefaultLockTarget = false;

		// Token: 0x04000691 RID: 1681
		private static string _language = BannerlordConfig.DefaultLanguage;

		// Token: 0x04000692 RID: 1682
		private static string _voiceLanguage = BannerlordConfig.DefaultLanguage;

		// Token: 0x04000695 RID: 1685
		private static int _numberOfCorpses = 3;

		// Token: 0x0400069A RID: 1690
		private static int _battleSize = 2;

		// Token: 0x040006A4 RID: 1700
		private static int _autoSaveInterval = 30;

		// Token: 0x040006AA RID: 1706
		private static bool _stopGameOnFocusLost = true;

		// Token: 0x040006B6 RID: 1718
		private static int _orderType = 0;

		// Token: 0x040006B7 RID: 1719
		private static int _orderLayoutType = 0;

		// Token: 0x020004F8 RID: 1272
		private interface IConfigPropertyBoundChecker<T>
		{
		}

		// Token: 0x020004F9 RID: 1273
		private abstract class ConfigProperty : Attribute
		{
		}

		// Token: 0x020004FA RID: 1274
		private sealed class ConfigPropertyInt : BannerlordConfig.ConfigProperty
		{
			// Token: 0x060038FC RID: 14588 RVA: 0x000E6F34 File Offset: 0x000E5134
			public ConfigPropertyInt(int[] possibleValues, bool isRange = false)
			{
				this._possibleValues = possibleValues;
				this._isRange = isRange;
				bool isRange2 = this._isRange;
			}

			// Token: 0x060038FD RID: 14589 RVA: 0x000E6F54 File Offset: 0x000E5154
			public bool IsValidValue(int value)
			{
				if (this._isRange)
				{
					return value >= this._possibleValues[0] && value <= this._possibleValues[1];
				}
				int[] possibleValues = this._possibleValues;
				for (int i = 0; i < possibleValues.Length; i++)
				{
					if (possibleValues[i] == value)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x04001B57 RID: 6999
			private int[] _possibleValues;

			// Token: 0x04001B58 RID: 7000
			private bool _isRange;
		}

		// Token: 0x020004FB RID: 1275
		private sealed class ConfigPropertyUnbounded : BannerlordConfig.ConfigProperty
		{
		}
	}
}
