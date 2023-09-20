using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001D6 RID: 470
	public static class CombatLogManager
	{
		// Token: 0x1400001B RID: 27
		// (add) Token: 0x06001A62 RID: 6754 RVA: 0x0005D0C8 File Offset: 0x0005B2C8
		// (remove) Token: 0x06001A63 RID: 6755 RVA: 0x0005D0FC File Offset: 0x0005B2FC
		public static event CombatLogManager.OnPrintCombatLogHandler OnGenerateCombatLog;

		// Token: 0x06001A64 RID: 6756 RVA: 0x0005D130 File Offset: 0x0005B330
		public static void PrintDebugLogForInfo(Agent attackerAgent, Agent victimAgent, DamageTypes damageType, int speedBonus, int armorAmount, int inflictedDamage, int absorbedByArmor, sbyte collisionBone, float lostHpPercentage)
		{
			TextObject textObject = TextObject.Empty;
			CombatLogColor combatLogColor = CombatLogColor.White;
			bool isMine = attackerAgent.IsMine;
			bool isMine2 = victimAgent.IsMine;
			GameTexts.SetVariable("AMOUNT", inflictedDamage);
			GameTexts.SetVariable("DAMAGE_TYPE", damageType.ToString().ToLower());
			GameTexts.SetVariable("LOST_HP_PERCENTAGE", lostHpPercentage);
			if (isMine2)
			{
				GameTexts.SetVariable("ATTACKER_NAME", attackerAgent.Name);
				textObject = GameTexts.FindText("combat_log_player_attacked", null);
				combatLogColor = CombatLogColor.Red;
			}
			else if (isMine)
			{
				GameTexts.SetVariable("VICTIM_NAME", victimAgent.Name);
				textObject = GameTexts.FindText("combat_log_player_attacker", null);
				combatLogColor = CombatLogColor.Green;
			}
			CombatLogManager.Print(textObject, combatLogColor);
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "PrintDebugLogForInfo");
			if (armorAmount > 0)
			{
				GameTexts.SetVariable("ABSORBED_AMOUNT", absorbedByArmor);
				GameTexts.SetVariable("ARMOR_AMOUNT", armorAmount);
				mbstringBuilder.AppendLine<string>(GameTexts.FindText("combat_log_damage_absorbed", null).ToString());
			}
			if (victimAgent.IsHuman)
			{
				GameTexts.SetVariable("BONE", collisionBone.ToString());
				mbstringBuilder.AppendLine<string>(GameTexts.FindText("combat_log_hit_bone", null).ToString());
			}
			if (speedBonus != 0)
			{
				GameTexts.SetVariable("SPEED_BONUS", speedBonus);
				mbstringBuilder.AppendLine<string>(GameTexts.FindText("combat_log_speed_bonus", null).ToString());
			}
			CombatLogManager.Print(new TextObject(mbstringBuilder.ToStringAndRelease(), null), CombatLogColor.White);
		}

		// Token: 0x06001A65 RID: 6757 RVA: 0x0005D288 File Offset: 0x0005B488
		private static void Print(TextObject message, CombatLogColor logColor = CombatLogColor.White)
		{
			Debug.Print(message.ToString(), 0, (Debug.DebugColor)logColor, 562949953421312UL);
		}

		// Token: 0x06001A66 RID: 6758 RVA: 0x0005D2B0 File Offset: 0x0005B4B0
		public static void GenerateCombatLog(CombatLogData logData)
		{
			CombatLogManager.OnPrintCombatLogHandler onGenerateCombatLog = CombatLogManager.OnGenerateCombatLog;
			if (onGenerateCombatLog != null)
			{
				onGenerateCombatLog(logData);
			}
			foreach (ValueTuple<string, uint> valueTuple in logData.GetLogString())
			{
				InformationManager.DisplayMessage(new InformationMessage(valueTuple.Item1, Color.FromUint(valueTuple.Item2), "Combat"));
			}
		}

		// Token: 0x02000521 RID: 1313
		// (Invoke) Token: 0x06003981 RID: 14721
		public delegate void OnPrintCombatLogHandler(CombatLogData logData);
	}
}
