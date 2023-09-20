using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade
{
	public static class MusicParameters
	{
		public static int SmallBattleTreshold
		{
			get
			{
				return (int)MusicParameters._parameters[0];
			}
		}

		public static int MediumBattleTreshold
		{
			get
			{
				return (int)MusicParameters._parameters[1];
			}
		}

		public static int LargeBattleTreshold
		{
			get
			{
				return (int)MusicParameters._parameters[2];
			}
		}

		public static float SmallBattleDistanceTreshold
		{
			get
			{
				return MusicParameters._parameters[3];
			}
		}

		public static float MediumBattleDistanceTreshold
		{
			get
			{
				return MusicParameters._parameters[4];
			}
		}

		public static float LargeBattleDistanceTreshold
		{
			get
			{
				return MusicParameters._parameters[5];
			}
		}

		public static float MaxBattleDistanceTreshold
		{
			get
			{
				return MusicParameters._parameters[6];
			}
		}

		public static float MinIntensity
		{
			get
			{
				return MusicParameters._parameters[7];
			}
		}

		public static float DefaultStartIntensity
		{
			get
			{
				return MusicParameters._parameters[8];
			}
		}

		public static float PlayerChargeEffectMultiplierOnIntensity
		{
			get
			{
				return MusicParameters._parameters[9];
			}
		}

		public static float BattleSizeEffectOnStartIntensity
		{
			get
			{
				return MusicParameters._parameters[10];
			}
		}

		public static float RandomEffectMultiplierOnStartIntensity
		{
			get
			{
				return MusicParameters._parameters[11];
			}
		}

		public static float FriendlyTroopDeadEffectOnIntensity
		{
			get
			{
				return MusicParameters._parameters[12];
			}
		}

		public static float EnemyTroopDeadEffectOnIntensity
		{
			get
			{
				return MusicParameters._parameters[13];
			}
		}

		public static float PlayerTroopDeadEffectMultiplierOnIntensity
		{
			get
			{
				return MusicParameters._parameters[14];
			}
		}

		public static float BattleRatioTresholdOnIntensity
		{
			get
			{
				return MusicParameters._parameters[15];
			}
		}

		public static float BattleTurnsOneSideCooldown
		{
			get
			{
				return MusicParameters._parameters[16];
			}
		}

		public static float CampaignDarkModeThreshold
		{
			get
			{
				return MusicParameters._parameters[17];
			}
		}

		public static void LoadFromXml()
		{
			MusicParameters._parameters = new float[18];
			string text = ModuleHelper.GetModuleFullPath("Native") + "ModuleData/music_parameters.xml";
			XmlDocument xmlDocument = new XmlDocument();
			StreamReader streamReader = new StreamReader(text);
			string text2 = streamReader.ReadToEnd();
			xmlDocument.LoadXml(text2);
			streamReader.Close();
			foreach (object obj in xmlDocument.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.Name == "music_parameters")
				{
					using (IEnumerator enumerator2 = xmlNode.ChildNodes.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							object obj2 = enumerator2.Current;
							XmlNode xmlNode2 = (XmlNode)obj2;
							if (xmlNode2.NodeType == XmlNodeType.Element)
							{
								MusicParameters.MusicParametersEnum musicParametersEnum = (MusicParameters.MusicParametersEnum)Enum.Parse(typeof(MusicParameters.MusicParametersEnum), xmlNode2.Attributes["id"].Value);
								float num = float.Parse(xmlNode2.Attributes["value"].Value, CultureInfo.InvariantCulture);
								MusicParameters._parameters[(int)musicParametersEnum] = num;
							}
						}
						break;
					}
				}
			}
			Debug.Print("MusicParameters have been resetted.", 0, Debug.DebugColor.Green, 281474976710656UL);
		}

		private static float[] _parameters;

		public const float ZeroIntensity = 0f;

		private enum MusicParametersEnum
		{
			SmallBattleTreshold,
			MediumBattleTreshold,
			LargeBattleTreshold,
			SmallBattleDistanceTreshold,
			MediumBattleDistanceTreshold,
			LargeBattleDistanceTreshold,
			MaxBattleDistanceTreshold,
			MinIntensity,
			DefaultStartIntensity,
			PlayerChargeEffectMultiplierOnIntensity,
			BattleSizeEffectOnStartIntensity,
			RandomEffectMultiplierOnStartIntensity,
			FriendlyTroopDeadEffectOnIntensity,
			EnemyTroopDeadEffectOnIntensity,
			PlayerTroopDeadEffectMultiplierOnIntensity,
			BattleRatioTresholdOnIntensity,
			BattleTurnsOneSideCooldown,
			CampaignDarkModeThreshold,
			Count
		}
	}
}
