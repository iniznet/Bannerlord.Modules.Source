using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public static class BannerlordConfig
	{
		public static int MinBattleSize
		{
			get
			{
				return BannerlordConfig._battleSizes[0];
			}
		}

		public static int MaxBattleSize
		{
			get
			{
				return BannerlordConfig._battleSizes[BannerlordConfig._battleSizes.Length - 1];
			}
		}

		public static int MinReinforcementWaveCount
		{
			get
			{
				return BannerlordConfig._reinforcementWaveCounts[0];
			}
		}

		public static int MaxReinforcementWaveCount
		{
			get
			{
				return BannerlordConfig._reinforcementWaveCounts[BannerlordConfig._reinforcementWaveCounts.Length - 1];
			}
		}

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

		public static string DefaultLanguage
		{
			get
			{
				return BannerlordConfig.GetDefaultLanguage();
			}
		}

		public static int GetRealBattleSize()
		{
			return BannerlordConfig._battleSizes[BannerlordConfig.BattleSize];
		}

		public static int GetRealBattleSizeForSiege()
		{
			return BannerlordConfig._siegeBattleSizes[BannerlordConfig.BattleSize];
		}

		public static int GetReinforcementWaveCount()
		{
			return BannerlordConfig._reinforcementWaveCounts[BannerlordConfig.ReinforcementWaveCount];
		}

		public static int GetRealBattleSizeForSallyOut()
		{
			return BannerlordConfig._sallyOutBattleSizes[BannerlordConfig.BattleSize];
		}

		private static string GetDefaultLanguage()
		{
			return LocalizedTextManager.GetLocalizationCodeOfISOLanguageCode(Utilities.GetSystemLanguage());
		}

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
						Debug.FailedAssert("Language cannot be set!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\BannerlordConfig.cs", "Language", 353);
					}
					MBTextManager.LocalizationDebugMode = NativeConfig.LocalizationDebugMode;
				}
			}
		}

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
					Debug.FailedAssert("Voice Language cannot be set!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\BannerlordConfig.cs", "VoiceLanguage", 380);
				}
			}
		}

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool GyroOverrideForAttackDefend { get; set; } = false;

		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2 }, false)]
		public static int AttackDirectionControl { get; set; } = 1;

		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2 }, false)]
		public static int DefendDirectionControl { get; set; } = 0;

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

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool ShowBlood { get; set; } = true;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool DisplayAttackDirection { get; set; } = true;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool DisplayTargetingReticule { get; set; } = true;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool ForceVSyncInMenus { get; set; } = true;

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

		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2, 3 }, false)]
		public static int ReinforcementWaveCount { get; set; } = 3;

		public static float CivilianAgentCount
		{
			get
			{
				return (float)BannerlordConfig.GetRealBattleSize() * 0.5f;
			}
		}

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static float FirstPersonFov { get; set; } = 65f;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static float UIScale { get; set; } = 1f;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static float CombatCameraDistance { get; set; } = 1f;

		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2, 3 }, false)]
		public static int TurnCameraWithHorseInFirstPerson { get; set; } = 2;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool ReportDamage { get; set; } = true;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool ReportBark { get; set; } = true;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool LockTarget { get; set; } = false;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableTutorialHints { get; set; } = true;

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

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static float FriendlyTroopsBannerOpacity { get; set; } = 1f;

		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2 }, false)]
		public static int ReportCasualtiesType { get; set; } = 0;

		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2 }, false)]
		public static int AutoTrackAttackedSettlements { get; set; } = 0;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool ReportPersonalDamage { get; set; } = true;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool SlowDownOnOrder { get; set; } = true;

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

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool ReportExperience { get; set; } = true;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableDamageTakenVisuals { get; set; } = true;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableVerticalAimCorrection { get; set; } = true;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static float ZoomSensitivityModifier { get; set; } = 0.66666f;

		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1 }, false)]
		public static int CrosshairType { get; set; } = 0;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableGenericAvatars { get; set; } = false;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableGenericNames { get; set; } = false;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool HideFullServers { get; set; } = false;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool HideEmptyServers { get; set; } = false;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool HidePasswordProtectedServers { get; set; } = false;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool HideUnofficialServers { get; set; } = false;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool HideModuleIncompatibleServers { get; set; } = false;

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

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableVoiceChat { get; set; } = true;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableDeathIcon { get; set; } = true;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableNetworkAlertIcons { get; set; } = true;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableSingleplayerChatBox { get; set; } = true;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool EnableMultiplayerChatBox { get; set; } = true;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static float ChatBoxSizeX { get; set; } = 495f;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static float ChatBoxSizeY { get; set; } = 340f;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static string LatestSaveGameName { get; set; } = string.Empty;

		[BannerlordConfig.ConfigPropertyUnbounded]
		public static bool HideBattleUI { get; set; } = false;

		[BannerlordConfig.ConfigPropertyInt(new int[] { 0, 1, 2, 3 }, false)]
		public static int UnitSpawnPrioritization { get; set; } = 0;

		private static int[] _battleSizes = new int[] { 200, 300, 400, 500, 600, 800, 1000 };

		private static int[] _siegeBattleSizes = new int[] { 150, 230, 320, 425, 540, 625, 1000 };

		private static int[] _sallyOutBattleSizes = new int[] { 150, 200, 240, 280, 320, 360, 400 };

		private static int[] _reinforcementWaveCounts = new int[] { 3, 4, 5, 0 };

		public static double SiegeBattleSizeMultiplier = 0.8;

		public const bool DefaultGyroOverrideForAttackDefend = false;

		public const int DefaultAttackDirectionControl = 1;

		public const int DefaultDefendDirectionControl = 0;

		public const int DefaultNumberOfCorpses = 3;

		public const bool DefaultShowBlood = true;

		public const bool DefaultDisplayAttackDirection = true;

		public const bool DefaultDisplayTargetingReticule = true;

		public const bool DefaultForceVSyncInMenus = true;

		public const int DefaultBattleSize = 2;

		public const int DefaultReinforcementWaveCount = 3;

		public const float DefaultBattleSizeMultiplier = 0.5f;

		public const float DefaultFirstPersonFov = 65f;

		public const float DefaultUIScale = 1f;

		public const float DefaultCombatCameraDistance = 1f;

		public const int DefaultCombatAI = 0;

		public const int DefaultTurnCameraWithHorseInFirstPerson = 2;

		public const int DefaultAutoSaveInterval = 30;

		public const float DefaultFriendlyTroopsBannerOpacity = 1f;

		public const bool DefaultReportDamage = true;

		public const bool DefaultReportBark = true;

		public const bool DefaultEnableTutorialHints = true;

		public const int DefaultReportCasualtiesType = 0;

		public const int DefaultAutoTrackAttackedSettlements = 0;

		public const bool DefaultReportPersonalDamage = true;

		public const bool DefaultStopGameOnFocusLost = true;

		public const bool DefaultSlowDownOnOrder = true;

		public const bool DefaultReportExperience = true;

		public const bool DefaultEnableDamageTakenVisuals = true;

		public const bool DefaultEnableVoiceChat = true;

		public const bool DefaultEnableDeathIcon = true;

		public const bool DefaultEnableNetworkAlertIcons = true;

		public const bool DefaultEnableVerticalAimCorrection = true;

		public const float DefaultZoomSensitivityModifier = 0.66666f;

		public const bool DefaultSingleplayerEnableChatBox = true;

		public const bool DefaultMultiplayerEnableChatBox = true;

		public const float DefaultChatBoxSizeX = 495f;

		public const float DefaultChatBoxSizeY = 340f;

		public const int DefaultCrosshairType = 0;

		public const bool DefaultEnableGenericAvatars = false;

		public const bool DefaultEnableGenericNames = false;

		public const bool DefaultHideFullServers = false;

		public const bool DefaultHideEmptyServers = false;

		public const bool DefaultHidePasswordProtectedServers = false;

		public const bool DefaultHideUnofficialServers = false;

		public const bool DefaultHideModuleIncompatibleServers = false;

		public const int DefaultOrderLayoutType = 0;

		public const bool DefaultHideBattleUI = false;

		public const int DefaultUnitSpawnPrioritization = 0;

		public const int DefaultOrderType = 0;

		public const bool DefaultLockTarget = false;

		private static string _language = BannerlordConfig.DefaultLanguage;

		private static string _voiceLanguage = BannerlordConfig.DefaultLanguage;

		private static int _numberOfCorpses = 3;

		private static int _battleSize = 2;

		private static int _autoSaveInterval = 30;

		private static bool _stopGameOnFocusLost = true;

		private static int _orderType = 0;

		private static int _orderLayoutType = 0;

		private interface IConfigPropertyBoundChecker<T>
		{
		}

		private abstract class ConfigProperty : Attribute
		{
		}

		private sealed class ConfigPropertyInt : BannerlordConfig.ConfigProperty
		{
			public ConfigPropertyInt(int[] possibleValues, bool isRange = false)
			{
				this._possibleValues = possibleValues;
				this._isRange = isRange;
				bool isRange2 = this._isRange;
			}

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

			private int[] _possibleValues;

			private bool _isRange;
		}

		private sealed class ConfigPropertyUnbounded : BannerlordConfig.ConfigProperty
		{
		}
	}
}
