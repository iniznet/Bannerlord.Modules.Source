using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000148 RID: 328
	public class DefaultTroopSupplierProbabilityModel : TroopSupplierProbabilityModel
	{
		// Token: 0x060017FE RID: 6142 RVA: 0x00079364 File Offset: 0x00077564
		public override void EnqueueTroopSpawnProbabilitiesAccordingToUnitSpawnPrioritization(MapEventParty battleParty, FlattenedTroopRoster priorityTroops, bool includePlayer, int sizeOfSide, bool forcePriorityTroops, List<ValueTuple<FlattenedTroopRosterElement, MapEventParty, float>> priorityList)
		{
			UnitSpawnPrioritizations unitSpawnPrioritizations = UnitSpawnPrioritizations.HighLevel;
			bool flag = PlayerEncounter.Current != null && PlayerEncounter.Current.IsSallyOutAmbush;
			if (battleParty.Party == PartyBase.MainParty && !flag)
			{
				unitSpawnPrioritizations = Game.Current.UnitSpawnPrioritization;
			}
			if (unitSpawnPrioritizations != UnitSpawnPrioritizations.Default && !forcePriorityTroops)
			{
				StackArray.StackArray8Int stackArray8Int = default(StackArray.StackArray8Int);
				int num = 0;
				foreach (FlattenedTroopRosterElement flattenedTroopRosterElement in battleParty.Troops)
				{
					if (this.CanTroopJoinBattle(flattenedTroopRosterElement, includePlayer))
					{
						int num2 = (int)flattenedTroopRosterElement.Troop.DefaultFormationClass;
						int num3 = stackArray8Int[num2];
						stackArray8Int[num2] = num3 + 1;
						num++;
					}
				}
				StackArray.StackArray8Int stackArray8Int2 = default(StackArray.StackArray8Int);
				float num4 = 1000f;
				using (IEnumerator<FlattenedTroopRosterElement> enumerator = battleParty.Troops.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FlattenedTroopRosterElement flattenedTroopRosterElement2 = enumerator.Current;
						if (this.CanTroopJoinBattle(flattenedTroopRosterElement2, includePlayer))
						{
							CharacterObject troop = flattenedTroopRosterElement2.Troop;
							FormationClass formationClass = troop.GetFormationClass();
							float num6;
							if (priorityTroops != null && this.IsPriorityTroop(flattenedTroopRosterElement2, priorityTroops))
							{
								float num5 = num4;
								num4 = num5 - 1f;
								num6 = num5;
							}
							else
							{
								float num7 = (float)stackArray8Int[(int)formationClass] / (float)((unitSpawnPrioritizations == UnitSpawnPrioritizations.Homogeneous) ? (stackArray8Int2[(int)formationClass] + 1) : num);
								float num8;
								if (!troop.IsHero)
								{
									num8 = num7;
								}
								else
								{
									num4 = (num8 = num4) - 1f;
								}
								num6 = num8;
								if (!troop.IsHero && (unitSpawnPrioritizations == UnitSpawnPrioritizations.HighLevel || unitSpawnPrioritizations == UnitSpawnPrioritizations.LowLevel))
								{
									num6 += (float)troop.Level;
									if (unitSpawnPrioritizations == UnitSpawnPrioritizations.LowLevel)
									{
										num6 *= -1f;
									}
								}
							}
							int num3 = (int)formationClass;
							int num2 = stackArray8Int[num3];
							stackArray8Int[num3] = num2 - 1;
							num2 = (int)formationClass;
							num3 = stackArray8Int2[num2];
							stackArray8Int2[num2] = num3 + 1;
							priorityList.Add(new ValueTuple<FlattenedTroopRosterElement, MapEventParty, float>(flattenedTroopRosterElement2, battleParty, num6));
						}
					}
					return;
				}
			}
			int numberOfHealthyMembers = battleParty.Party.NumberOfHealthyMembers;
			foreach (FlattenedTroopRosterElement flattenedTroopRosterElement3 in battleParty.Troops)
			{
				if (this.CanTroopJoinBattle(flattenedTroopRosterElement3, includePlayer))
				{
					float num9 = 1f;
					if (flattenedTroopRosterElement3.Troop.IsHero)
					{
						num9 *= 150f;
						if (priorityTroops != null)
						{
							UniqueTroopDescriptor uniqueTroopDescriptor = priorityTroops.FindIndexOfCharacter(flattenedTroopRosterElement3.Troop);
							if (uniqueTroopDescriptor.IsValid)
							{
								num9 *= 100f;
								priorityTroops.Remove(uniqueTroopDescriptor);
							}
						}
						if (flattenedTroopRosterElement3.Troop.HeroObject.IsHumanPlayerCharacter)
						{
							num9 *= 10f;
						}
						priorityList.Add(new ValueTuple<FlattenedTroopRosterElement, MapEventParty, float>(flattenedTroopRosterElement3, battleParty, num9));
					}
					else
					{
						int num10 = 0;
						int num11 = 0;
						for (int i = 0; i < battleParty.Party.MemberRoster.Count; i++)
						{
							TroopRosterElement elementCopyAtIndex = battleParty.Party.MemberRoster.GetElementCopyAtIndex(i);
							if (!elementCopyAtIndex.Character.IsHero)
							{
								if (elementCopyAtIndex.Character == flattenedTroopRosterElement3.Troop)
								{
									num10 = i - num11;
									break;
								}
							}
							else
							{
								num11++;
							}
						}
						int num12 = (int)(100f / MathF.Pow(1.2f, (float)num10));
						if (num12 < 10)
						{
							num12 = 10;
						}
						int num13 = numberOfHealthyMembers / sizeOfSide * 100;
						if (num13 < 10)
						{
							num13 = 10;
						}
						int num14 = 0;
						if (priorityTroops != null)
						{
							UniqueTroopDescriptor uniqueTroopDescriptor2 = priorityTroops.FindIndexOfCharacter(flattenedTroopRosterElement3.Troop);
							if (uniqueTroopDescriptor2.IsValid)
							{
								num14 = 20000;
								priorityTroops.Remove(uniqueTroopDescriptor2);
							}
						}
						num9 = (float)(num14 + MBRandom.RandomInt((int)((float)num12 * 0.5f + (float)num13 * 0.5f)));
						priorityList.Add(new ValueTuple<FlattenedTroopRosterElement, MapEventParty, float>(flattenedTroopRosterElement3, battleParty, num9));
					}
				}
			}
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x00079760 File Offset: 0x00077960
		private bool IsPriorityTroop(FlattenedTroopRosterElement troop, FlattenedTroopRoster priorityTroops)
		{
			foreach (FlattenedTroopRosterElement flattenedTroopRosterElement in priorityTroops)
			{
				if (flattenedTroopRosterElement.Troop == troop.Troop)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x000797B8 File Offset: 0x000779B8
		private bool CanTroopJoinBattle(FlattenedTroopRosterElement troopRoster, bool includePlayer)
		{
			return !troopRoster.IsWounded && !troopRoster.IsRouted && !troopRoster.IsKilled && (includePlayer || !troopRoster.Troop.IsPlayerCharacter);
		}
	}
}
