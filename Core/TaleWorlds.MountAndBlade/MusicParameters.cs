using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001BD RID: 445
	public static class MusicParameters
	{
		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x0600199B RID: 6555 RVA: 0x0005BAA5 File Offset: 0x00059CA5
		public static int SmallBattleTreshold
		{
			get
			{
				return (int)MusicParameters._parameters[0];
			}
		}

		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x0600199C RID: 6556 RVA: 0x0005BAAF File Offset: 0x00059CAF
		public static int MediumBattleTreshold
		{
			get
			{
				return (int)MusicParameters._parameters[1];
			}
		}

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x0600199D RID: 6557 RVA: 0x0005BAB9 File Offset: 0x00059CB9
		public static int LargeBattleTreshold
		{
			get
			{
				return (int)MusicParameters._parameters[2];
			}
		}

		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x0600199E RID: 6558 RVA: 0x0005BAC3 File Offset: 0x00059CC3
		public static float SmallBattleDistanceTreshold
		{
			get
			{
				return MusicParameters._parameters[3];
			}
		}

		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x0600199F RID: 6559 RVA: 0x0005BACC File Offset: 0x00059CCC
		public static float MediumBattleDistanceTreshold
		{
			get
			{
				return MusicParameters._parameters[4];
			}
		}

		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x060019A0 RID: 6560 RVA: 0x0005BAD5 File Offset: 0x00059CD5
		public static float LargeBattleDistanceTreshold
		{
			get
			{
				return MusicParameters._parameters[5];
			}
		}

		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x060019A1 RID: 6561 RVA: 0x0005BADE File Offset: 0x00059CDE
		public static float MaxBattleDistanceTreshold
		{
			get
			{
				return MusicParameters._parameters[6];
			}
		}

		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x060019A2 RID: 6562 RVA: 0x0005BAE7 File Offset: 0x00059CE7
		public static float MinIntensity
		{
			get
			{
				return MusicParameters._parameters[7];
			}
		}

		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x060019A3 RID: 6563 RVA: 0x0005BAF0 File Offset: 0x00059CF0
		public static float DefaultStartIntensity
		{
			get
			{
				return MusicParameters._parameters[8];
			}
		}

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x060019A4 RID: 6564 RVA: 0x0005BAF9 File Offset: 0x00059CF9
		public static float PlayerChargeEffectMultiplierOnIntensity
		{
			get
			{
				return MusicParameters._parameters[9];
			}
		}

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x060019A5 RID: 6565 RVA: 0x0005BB03 File Offset: 0x00059D03
		public static float BattleSizeEffectOnStartIntensity
		{
			get
			{
				return MusicParameters._parameters[10];
			}
		}

		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x060019A6 RID: 6566 RVA: 0x0005BB0D File Offset: 0x00059D0D
		public static float RandomEffectMultiplierOnStartIntensity
		{
			get
			{
				return MusicParameters._parameters[11];
			}
		}

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x060019A7 RID: 6567 RVA: 0x0005BB17 File Offset: 0x00059D17
		public static float FriendlyTroopDeadEffectOnIntensity
		{
			get
			{
				return MusicParameters._parameters[12];
			}
		}

		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x060019A8 RID: 6568 RVA: 0x0005BB21 File Offset: 0x00059D21
		public static float EnemyTroopDeadEffectOnIntensity
		{
			get
			{
				return MusicParameters._parameters[13];
			}
		}

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x060019A9 RID: 6569 RVA: 0x0005BB2B File Offset: 0x00059D2B
		public static float PlayerTroopDeadEffectMultiplierOnIntensity
		{
			get
			{
				return MusicParameters._parameters[14];
			}
		}

		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x060019AA RID: 6570 RVA: 0x0005BB35 File Offset: 0x00059D35
		public static float BattleRatioTresholdOnIntensity
		{
			get
			{
				return MusicParameters._parameters[15];
			}
		}

		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x060019AB RID: 6571 RVA: 0x0005BB3F File Offset: 0x00059D3F
		public static float BattleTurnsOneSideCooldown
		{
			get
			{
				return MusicParameters._parameters[16];
			}
		}

		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x060019AC RID: 6572 RVA: 0x0005BB49 File Offset: 0x00059D49
		public static float CampaignDarkModeThreshold
		{
			get
			{
				return MusicParameters._parameters[17];
			}
		}

		// Token: 0x060019AD RID: 6573 RVA: 0x0005BB54 File Offset: 0x00059D54
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

		// Token: 0x04000804 RID: 2052
		private static float[] _parameters;

		// Token: 0x04000805 RID: 2053
		public const float ZeroIntensity = 0f;

		// Token: 0x02000518 RID: 1304
		private enum MusicParametersEnum
		{
			// Token: 0x04001BC4 RID: 7108
			SmallBattleTreshold,
			// Token: 0x04001BC5 RID: 7109
			MediumBattleTreshold,
			// Token: 0x04001BC6 RID: 7110
			LargeBattleTreshold,
			// Token: 0x04001BC7 RID: 7111
			SmallBattleDistanceTreshold,
			// Token: 0x04001BC8 RID: 7112
			MediumBattleDistanceTreshold,
			// Token: 0x04001BC9 RID: 7113
			LargeBattleDistanceTreshold,
			// Token: 0x04001BCA RID: 7114
			MaxBattleDistanceTreshold,
			// Token: 0x04001BCB RID: 7115
			MinIntensity,
			// Token: 0x04001BCC RID: 7116
			DefaultStartIntensity,
			// Token: 0x04001BCD RID: 7117
			PlayerChargeEffectMultiplierOnIntensity,
			// Token: 0x04001BCE RID: 7118
			BattleSizeEffectOnStartIntensity,
			// Token: 0x04001BCF RID: 7119
			RandomEffectMultiplierOnStartIntensity,
			// Token: 0x04001BD0 RID: 7120
			FriendlyTroopDeadEffectOnIntensity,
			// Token: 0x04001BD1 RID: 7121
			EnemyTroopDeadEffectOnIntensity,
			// Token: 0x04001BD2 RID: 7122
			PlayerTroopDeadEffectMultiplierOnIntensity,
			// Token: 0x04001BD3 RID: 7123
			BattleRatioTresholdOnIntensity,
			// Token: 0x04001BD4 RID: 7124
			BattleTurnsOneSideCooldown,
			// Token: 0x04001BD5 RID: 7125
			CampaignDarkModeThreshold,
			// Token: 0x04001BD6 RID: 7126
			Count
		}
	}
}
