using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public static class CombatLogManager
	{
		public static event CombatLogManager.OnPrintCombatLogHandler OnGenerateCombatLog;

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

		private static void Print(TextObject message, CombatLogColor logColor = CombatLogColor.White)
		{
			Debug.Print(message.ToString(), 0, (Debug.DebugColor)logColor, 562949953421312UL);
		}

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

		public delegate void OnPrintCombatLogHandler(CombatLogData logData);
	}
}
