using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000318 RID: 792
	public abstract class MPPerkEffectBase
	{
		// Token: 0x17000794 RID: 1940
		// (get) Token: 0x06002A95 RID: 10901 RVA: 0x000A5D40 File Offset: 0x000A3F40
		public virtual bool IsTickRequired
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000795 RID: 1941
		// (get) Token: 0x06002A96 RID: 10902 RVA: 0x000A5D43 File Offset: 0x000A3F43
		// (set) Token: 0x06002A97 RID: 10903 RVA: 0x000A5D4B File Offset: 0x000A3F4B
		public bool IsDisabledInWarmup { get; protected set; }

		// Token: 0x06002A98 RID: 10904 RVA: 0x000A5D54 File Offset: 0x000A3F54
		public virtual void OnUpdate(Agent agent, bool newState)
		{
		}

		// Token: 0x06002A99 RID: 10905 RVA: 0x000A5D58 File Offset: 0x000A3F58
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

		// Token: 0x06002A9A RID: 10906 RVA: 0x000A5E30 File Offset: 0x000A4030
		public virtual void OnTick(Agent agent, int tickCount)
		{
		}

		// Token: 0x06002A9B RID: 10907 RVA: 0x000A5E32 File Offset: 0x000A4032
		public virtual float GetDamage(WeaponComponentData attackerWeapon, DamageTypes damageType, bool isAlternativeAttack)
		{
			return 0f;
		}

		// Token: 0x06002A9C RID: 10908 RVA: 0x000A5E39 File Offset: 0x000A4039
		public virtual float GetMountDamage(WeaponComponentData attackerWeapon, DamageTypes damageType, bool isAlternativeAttack)
		{
			return 0f;
		}

		// Token: 0x06002A9D RID: 10909 RVA: 0x000A5E40 File Offset: 0x000A4040
		public virtual float GetDamageTaken(WeaponComponentData attackerWeapon, DamageTypes damageType)
		{
			return 0f;
		}

		// Token: 0x06002A9E RID: 10910 RVA: 0x000A5E47 File Offset: 0x000A4047
		public virtual float GetMountDamageTaken(WeaponComponentData attackerWeapon, DamageTypes damageType)
		{
			return 0f;
		}

		// Token: 0x06002A9F RID: 10911 RVA: 0x000A5E4E File Offset: 0x000A404E
		public virtual float GetSpeedBonusEffectiveness(Agent attacker)
		{
			return 0f;
		}

		// Token: 0x06002AA0 RID: 10912 RVA: 0x000A5E55 File Offset: 0x000A4055
		public virtual float GetShieldDamage(bool isCorrectSideBlock)
		{
			return 0f;
		}

		// Token: 0x06002AA1 RID: 10913 RVA: 0x000A5E5C File Offset: 0x000A405C
		public virtual float GetShieldDamageTaken(bool isCorrectSideBlock)
		{
			return 0f;
		}

		// Token: 0x06002AA2 RID: 10914 RVA: 0x000A5E63 File Offset: 0x000A4063
		public virtual float GetRangedAccuracy()
		{
			return 0f;
		}

		// Token: 0x06002AA3 RID: 10915 RVA: 0x000A5E6A File Offset: 0x000A406A
		public virtual float GetThrowingWeaponSpeed(WeaponComponentData attackerWeapon)
		{
			return 0f;
		}

		// Token: 0x06002AA4 RID: 10916 RVA: 0x000A5E71 File Offset: 0x000A4071
		public virtual float GetDamageInterruptionThreshold()
		{
			return 0f;
		}

		// Token: 0x06002AA5 RID: 10917 RVA: 0x000A5E78 File Offset: 0x000A4078
		public virtual float GetMountManeuver()
		{
			return 0f;
		}

		// Token: 0x06002AA6 RID: 10918 RVA: 0x000A5E7F File Offset: 0x000A407F
		public virtual float GetMountSpeed()
		{
			return 0f;
		}

		// Token: 0x06002AA7 RID: 10919 RVA: 0x000A5E86 File Offset: 0x000A4086
		public virtual float GetRangedHeadShotDamage()
		{
			return 0f;
		}

		// Token: 0x06002AA8 RID: 10920 RVA: 0x000A5E8D File Offset: 0x000A408D
		public virtual int GetGoldOnKill(float attackerValue, float victimValue)
		{
			return 0;
		}

		// Token: 0x06002AA9 RID: 10921 RVA: 0x000A5E90 File Offset: 0x000A4090
		public virtual int GetGoldOnAssist()
		{
			return 0;
		}

		// Token: 0x06002AAA RID: 10922 RVA: 0x000A5E93 File Offset: 0x000A4093
		public virtual int GetRewardedGoldOnAssist()
		{
			return 0;
		}

		// Token: 0x06002AAB RID: 10923 RVA: 0x000A5E96 File Offset: 0x000A4096
		public virtual bool GetIsTeamRewardedOnDeath()
		{
			return false;
		}

		// Token: 0x06002AAC RID: 10924 RVA: 0x000A5E99 File Offset: 0x000A4099
		public virtual void CalculateRewardedGoldOnDeath(Agent agent, List<ValueTuple<MissionPeer, int>> teamMembers)
		{
		}

		// Token: 0x06002AAD RID: 10925 RVA: 0x000A5E9B File Offset: 0x000A409B
		public virtual float GetDrivenPropertyBonus(DrivenProperty drivenProperty, float baseValue)
		{
			return 0f;
		}

		// Token: 0x06002AAE RID: 10926 RVA: 0x000A5EA2 File Offset: 0x000A40A2
		public virtual float GetEncumbrance(bool isOnBody)
		{
			return 0f;
		}

		// Token: 0x06002AAF RID: 10927
		protected abstract void Deserialize(XmlNode node);
	}
}
