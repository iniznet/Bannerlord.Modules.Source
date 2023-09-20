using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public abstract class MPPerkEffectBase
	{
		public virtual bool IsTickRequired
		{
			get
			{
				return false;
			}
		}

		public bool IsDisabledInWarmup { get; protected set; }

		public virtual void OnUpdate(Agent agent, bool newState)
		{
		}

		public virtual void OnTick(MissionPeer peer, int tickCount)
		{
			if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0)
			{
				MBList<IFormationUnit> mblist;
				if (peer == null)
				{
					mblist = null;
				}
				else
				{
					Formation controlledFormation = peer.ControlledFormation;
					mblist = ((controlledFormation != null) ? controlledFormation.Arrangement.GetAllUnits() : null);
				}
				MBList<IFormationUnit> mblist2 = mblist;
				if (mblist2 == null)
				{
					return;
				}
				using (List<IFormationUnit>.Enumerator enumerator = mblist2.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Agent agent;
						if ((agent = enumerator.Current as Agent) != null && agent.IsActive())
						{
							this.OnTick(agent, tickCount);
						}
					}
					return;
				}
			}
			if (peer != null)
			{
				Agent controlledAgent = peer.ControlledAgent;
				bool? flag = ((controlledAgent != null) ? new bool?(controlledAgent.IsActive()) : null);
				bool flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					this.OnTick(peer.ControlledAgent, tickCount);
				}
			}
		}

		public virtual void OnTick(Agent agent, int tickCount)
		{
		}

		public virtual float GetDamage(WeaponComponentData attackerWeapon, DamageTypes damageType, bool isAlternativeAttack)
		{
			return 0f;
		}

		public virtual float GetMountDamage(WeaponComponentData attackerWeapon, DamageTypes damageType, bool isAlternativeAttack)
		{
			return 0f;
		}

		public virtual float GetDamageTaken(WeaponComponentData attackerWeapon, DamageTypes damageType)
		{
			return 0f;
		}

		public virtual float GetMountDamageTaken(WeaponComponentData attackerWeapon, DamageTypes damageType)
		{
			return 0f;
		}

		public virtual float GetSpeedBonusEffectiveness(Agent attacker)
		{
			return 0f;
		}

		public virtual float GetShieldDamage(bool isCorrectSideBlock)
		{
			return 0f;
		}

		public virtual float GetShieldDamageTaken(bool isCorrectSideBlock)
		{
			return 0f;
		}

		public virtual float GetRangedAccuracy()
		{
			return 0f;
		}

		public virtual float GetThrowingWeaponSpeed(WeaponComponentData attackerWeapon)
		{
			return 0f;
		}

		public virtual float GetDamageInterruptionThreshold()
		{
			return 0f;
		}

		public virtual float GetMountManeuver()
		{
			return 0f;
		}

		public virtual float GetMountSpeed()
		{
			return 0f;
		}

		public virtual float GetRangedHeadShotDamage()
		{
			return 0f;
		}

		public virtual int GetGoldOnKill(float attackerValue, float victimValue)
		{
			return 0;
		}

		public virtual int GetGoldOnAssist()
		{
			return 0;
		}

		public virtual int GetRewardedGoldOnAssist()
		{
			return 0;
		}

		public virtual bool GetIsTeamRewardedOnDeath()
		{
			return false;
		}

		public virtual void CalculateRewardedGoldOnDeath(Agent agent, List<ValueTuple<MissionPeer, int>> teamMembers)
		{
		}

		public virtual float GetDrivenPropertyBonus(DrivenProperty drivenProperty, float baseValue)
		{
			return 0f;
		}

		public virtual float GetEncumbrance(bool isOnBody)
		{
			return 0f;
		}

		protected abstract void Deserialize(XmlNode node);
	}
}
