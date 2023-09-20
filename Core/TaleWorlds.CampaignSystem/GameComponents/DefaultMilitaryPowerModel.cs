using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultMilitaryPowerModel : MilitaryPowerModel
	{
		public static void ChangeExistingBattleModifiers(List<ValueTuple<DefaultMilitaryPowerModel.PowerFlags, float>> newBattleModifiers)
		{
			foreach (ValueTuple<DefaultMilitaryPowerModel.PowerFlags, float> valueTuple in newBattleModifiers)
			{
				if (DefaultMilitaryPowerModel._battleModifiers.ContainsKey(valueTuple.Item1))
				{
					DefaultMilitaryPowerModel._battleModifiers[valueTuple.Item1] = valueTuple.Item2;
				}
			}
		}

		public override float GetTroopPower(float defaultTroopPower, float leaderModifier = 0f, float contextModifier = 0f)
		{
			return defaultTroopPower * (1f + leaderModifier + contextModifier);
		}

		public override float GetTroopPower(CharacterObject troop, BattleSideEnum side, MapEvent.PowerCalculationContext context, float leaderModifier)
		{
			float defaultTroopPower = Campaign.Current.Models.MilitaryPowerModel.GetDefaultTroopPower(troop);
			float contextModifier = Campaign.Current.Models.MilitaryPowerModel.GetContextModifier(troop, side, context);
			return Campaign.Current.Models.MilitaryPowerModel.GetTroopPower(defaultTroopPower, leaderModifier, contextModifier);
		}

		public override float GetLeaderModifierInMapEvent(MapEvent mapEvent, BattleSideEnum battleSideEnum)
		{
			Hero hero = ((battleSideEnum == BattleSideEnum.Attacker) ? mapEvent.AttackerSide.LeaderParty.LeaderHero : mapEvent.DefenderSide.LeaderParty.LeaderHero);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			if (hero != null)
			{
				foreach (PerkObject perkObject in PerkObject.All)
				{
					if (perkObject.PrimaryRole == SkillEffect.PerkRole.Captain && hero.GetPerkValue(perkObject))
					{
						float num5 = perkObject.RequiredSkillValue / (float)Campaign.Current.Models.CharacterDevelopmentModel.MaxSkillRequiredForEpicPerkBonus;
						if (num5 <= 0.3f)
						{
							num++;
						}
						else if (num5 <= 0.6f)
						{
							num2++;
						}
						else if (num5 <= 0.9f)
						{
							num3++;
						}
						else
						{
							num4++;
						}
					}
				}
			}
			return (float)num * 0.01f + (float)num2 * 0.02f + (float)num3 * 0.03f + (float)num4 * 0.06f;
		}

		public override float GetContextModifier(CharacterObject troop, BattleSideEnum battleSideEnum, MapEvent.PowerCalculationContext context)
		{
			DefaultMilitaryPowerModel.PowerFlags powerFlags = DefaultMilitaryPowerModel.PowerFlags.Invalid;
			if (troop.HasMount())
			{
				powerFlags |= (troop.IsRanged ? DefaultMilitaryPowerModel.PowerFlags.HorseArcher : DefaultMilitaryPowerModel.PowerFlags.Cavalry);
			}
			else if (troop.IsRanged)
			{
				powerFlags = DefaultMilitaryPowerModel.PowerFlags.Archer;
			}
			else
			{
				powerFlags = DefaultMilitaryPowerModel.PowerFlags.Infantry;
			}
			switch (context)
			{
			case MapEvent.PowerCalculationContext.Default:
			case MapEvent.PowerCalculationContext.PlainBattle:
			case MapEvent.PowerCalculationContext.SteppeBattle:
			case MapEvent.PowerCalculationContext.DesertBattle:
			case MapEvent.PowerCalculationContext.DuneBattle:
			case MapEvent.PowerCalculationContext.SnowBattle:
				powerFlags |= DefaultMilitaryPowerModel.PowerFlags.Flat;
				break;
			case MapEvent.PowerCalculationContext.ForestBattle:
				powerFlags |= DefaultMilitaryPowerModel.PowerFlags.Forest;
				break;
			case MapEvent.PowerCalculationContext.RiverCrossingBattle:
				powerFlags |= DefaultMilitaryPowerModel.PowerFlags.RiverCrossing;
				break;
			case MapEvent.PowerCalculationContext.Village:
				powerFlags |= DefaultMilitaryPowerModel.PowerFlags.Village;
				break;
			case MapEvent.PowerCalculationContext.Siege:
				powerFlags |= DefaultMilitaryPowerModel.PowerFlags.Siege;
				break;
			}
			powerFlags |= ((battleSideEnum == BattleSideEnum.Attacker) ? DefaultMilitaryPowerModel.PowerFlags.Attacker : DefaultMilitaryPowerModel.PowerFlags.Defender);
			return DefaultMilitaryPowerModel._battleModifiers[powerFlags];
		}

		public override float GetDefaultTroopPower(CharacterObject troop)
		{
			int num = (troop.IsHero ? (troop.HeroObject.Level / 4 + 1) : troop.Tier);
			return (float)((2 + num) * (10 + num)) * 0.02f * (troop.IsHero ? 1.5f : (troop.IsMounted ? 1.2f : 1f));
		}

		private const float LowTierCaptainPerkPowerBoost = 0.01f;

		private const float MidTierCaptainPerkPowerBoost = 0.02f;

		private const float HighTierCaptainPerkPowerBoost = 0.03f;

		private const float UltraTierCaptainPerkPowerBoost = 0.06f;

		private static Dictionary<DefaultMilitaryPowerModel.PowerFlags, float> _battleModifiers = new Dictionary<DefaultMilitaryPowerModel.PowerFlags, float>
		{
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Siege | DefaultMilitaryPowerModel.PowerFlags.Infantry,
				0f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Village | DefaultMilitaryPowerModel.PowerFlags.Infantry,
				0.05f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.RiverCrossing | DefaultMilitaryPowerModel.PowerFlags.Infantry,
				0f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Forest | DefaultMilitaryPowerModel.PowerFlags.Infantry,
				0.05f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Flat | DefaultMilitaryPowerModel.PowerFlags.Infantry,
				0f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Siege | DefaultMilitaryPowerModel.PowerFlags.Infantry,
				0f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Village | DefaultMilitaryPowerModel.PowerFlags.Infantry,
				0.05f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.RiverCrossing | DefaultMilitaryPowerModel.PowerFlags.Infantry,
				0.05f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Forest | DefaultMilitaryPowerModel.PowerFlags.Infantry,
				0.05f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Flat | DefaultMilitaryPowerModel.PowerFlags.Infantry,
				0f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Siege | DefaultMilitaryPowerModel.PowerFlags.Archer,
				-0.2f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Village | DefaultMilitaryPowerModel.PowerFlags.Archer,
				-0.1f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.RiverCrossing | DefaultMilitaryPowerModel.PowerFlags.Archer,
				0f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Forest | DefaultMilitaryPowerModel.PowerFlags.Archer,
				-0.1f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Flat | DefaultMilitaryPowerModel.PowerFlags.Archer,
				0f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Siege | DefaultMilitaryPowerModel.PowerFlags.Archer,
				0.3f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Village | DefaultMilitaryPowerModel.PowerFlags.Archer,
				0.05f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.RiverCrossing | DefaultMilitaryPowerModel.PowerFlags.Archer,
				0.1f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Forest | DefaultMilitaryPowerModel.PowerFlags.Archer,
				-0.5f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Flat | DefaultMilitaryPowerModel.PowerFlags.Archer,
				0f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Siege | DefaultMilitaryPowerModel.PowerFlags.Cavalry,
				-0.1f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Village | DefaultMilitaryPowerModel.PowerFlags.Cavalry,
				0f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.RiverCrossing | DefaultMilitaryPowerModel.PowerFlags.Cavalry,
				-0.15f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Forest | DefaultMilitaryPowerModel.PowerFlags.Cavalry,
				-0.2f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Flat | DefaultMilitaryPowerModel.PowerFlags.Cavalry,
				0.25f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Siege | DefaultMilitaryPowerModel.PowerFlags.Cavalry,
				-0.1f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Village | DefaultMilitaryPowerModel.PowerFlags.Cavalry,
				-0.1f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.RiverCrossing | DefaultMilitaryPowerModel.PowerFlags.Cavalry,
				-0.05f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Forest | DefaultMilitaryPowerModel.PowerFlags.Cavalry,
				-0.15f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Flat | DefaultMilitaryPowerModel.PowerFlags.Cavalry,
				0.1f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Siege | DefaultMilitaryPowerModel.PowerFlags.HorseArcher,
				-0.2f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Village | DefaultMilitaryPowerModel.PowerFlags.HorseArcher,
				0.1f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.RiverCrossing | DefaultMilitaryPowerModel.PowerFlags.HorseArcher,
				-0.1f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Forest | DefaultMilitaryPowerModel.PowerFlags.HorseArcher,
				-0.3f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Attacker | DefaultMilitaryPowerModel.PowerFlags.Flat | DefaultMilitaryPowerModel.PowerFlags.HorseArcher,
				0.3f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Siege | DefaultMilitaryPowerModel.PowerFlags.HorseArcher,
				0.3f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Village | DefaultMilitaryPowerModel.PowerFlags.HorseArcher,
				0f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.RiverCrossing | DefaultMilitaryPowerModel.PowerFlags.HorseArcher,
				0f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Forest | DefaultMilitaryPowerModel.PowerFlags.HorseArcher,
				-0.25f
			},
			{
				DefaultMilitaryPowerModel.PowerFlags.Defender | DefaultMilitaryPowerModel.PowerFlags.Flat | DefaultMilitaryPowerModel.PowerFlags.HorseArcher,
				0.15f
			}
		};

		[Flags]
		public enum PowerFlags
		{
			Invalid = 0,
			Attacker = 1,
			Defender = 2,
			Siege = 4,
			Village = 8,
			RiverCrossing = 16,
			Forest = 32,
			Flat = 64,
			Infantry = 128,
			Archer = 256,
			Cavalry = 512,
			HorseArcher = 1024
		}
	}
}
