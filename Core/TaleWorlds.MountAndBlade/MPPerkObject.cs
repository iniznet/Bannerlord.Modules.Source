using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200031A RID: 794
	public class MPPerkObject : IReadOnlyPerkObject
	{
		// Token: 0x1700079F RID: 1951
		// (get) Token: 0x06002AC0 RID: 10944 RVA: 0x000A5EB1 File Offset: 0x000A40B1
		public TextObject Name
		{
			get
			{
				return new TextObject(this._name, null);
			}
		}

		// Token: 0x170007A0 RID: 1952
		// (get) Token: 0x06002AC1 RID: 10945 RVA: 0x000A5EBF File Offset: 0x000A40BF
		public TextObject Description
		{
			get
			{
				return new TextObject(this._description, null);
			}
		}

		// Token: 0x170007A1 RID: 1953
		// (get) Token: 0x06002AC2 RID: 10946 RVA: 0x000A5ECD File Offset: 0x000A40CD
		public bool HasBannerBearer { get; }

		// Token: 0x170007A2 RID: 1954
		// (get) Token: 0x06002AC3 RID: 10947 RVA: 0x000A5ED5 File Offset: 0x000A40D5
		public List<string> GameModes { get; }

		// Token: 0x170007A3 RID: 1955
		// (get) Token: 0x06002AC4 RID: 10948 RVA: 0x000A5EDD File Offset: 0x000A40DD
		public int PerkListIndex { get; }

		// Token: 0x170007A4 RID: 1956
		// (get) Token: 0x06002AC5 RID: 10949 RVA: 0x000A5EE5 File Offset: 0x000A40E5
		public string IconId { get; }

		// Token: 0x170007A5 RID: 1957
		// (get) Token: 0x06002AC6 RID: 10950 RVA: 0x000A5EED File Offset: 0x000A40ED
		public string HeroIdleAnimOverride { get; }

		// Token: 0x170007A6 RID: 1958
		// (get) Token: 0x06002AC7 RID: 10951 RVA: 0x000A5EF5 File Offset: 0x000A40F5
		public string HeroMountIdleAnimOverride { get; }

		// Token: 0x170007A7 RID: 1959
		// (get) Token: 0x06002AC8 RID: 10952 RVA: 0x000A5EFD File Offset: 0x000A40FD
		public string TroopIdleAnimOverride { get; }

		// Token: 0x170007A8 RID: 1960
		// (get) Token: 0x06002AC9 RID: 10953 RVA: 0x000A5F05 File Offset: 0x000A4105
		public string TroopMountIdleAnimOverride { get; }

		// Token: 0x06002ACA RID: 10954 RVA: 0x000A5F10 File Offset: 0x000A4110
		public MPPerkObject(MissionPeer peer, string name, string description, List<string> gameModes, int perkListIndex, string iconId, IEnumerable<MPConditionalEffect> conditionalEffects, IEnumerable<MPPerkEffectBase> effects, string heroIdleAnimOverride, string heroMountIdleAnimOverride, string troopIdleAnimOverride, string troopMountIdleAnimOverride)
		{
			this._peer = peer;
			this._name = name;
			this._description = description;
			this.GameModes = gameModes;
			this.PerkListIndex = perkListIndex;
			this.IconId = iconId;
			this._conditionalEffects = new MPConditionalEffect.ConditionalEffectContainer(conditionalEffects);
			this._effects = new List<MPPerkEffectBase>(effects);
			this.HeroIdleAnimOverride = heroIdleAnimOverride;
			this.HeroMountIdleAnimOverride = heroMountIdleAnimOverride;
			this.TroopIdleAnimOverride = troopIdleAnimOverride;
			this.TroopMountIdleAnimOverride = troopMountIdleAnimOverride;
			this._perkEventFlags = MPPerkCondition.PerkEventFlags.None;
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				foreach (MPPerkCondition mpperkCondition in mpconditionalEffect.Conditions)
				{
					this._perkEventFlags |= mpperkCondition.EventFlags;
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect2 in this._conditionalEffects)
			{
				using (List<MPPerkCondition>.Enumerator enumerator2 = mpconditionalEffect2.Conditions.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current is BannerBearerCondition)
						{
							this.HasBannerBearer = true;
						}
					}
				}
			}
		}

		// Token: 0x06002ACB RID: 10955 RVA: 0x000A60A0 File Offset: 0x000A42A0
		private MPPerkObject(XmlNode node)
		{
			this._peer = null;
			this._conditionalEffects = new MPConditionalEffect.ConditionalEffectContainer();
			this._effects = new List<MPPerkEffectBase>();
			this._name = node.Attributes["name"].Value;
			this._description = node.Attributes["description"].Value;
			this.GameModes = new List<string>(node.Attributes["game_mode"].Value.Split(new char[] { ',' }));
			for (int i = 0; i < this.GameModes.Count; i++)
			{
				this.GameModes[i] = this.GameModes[i].Trim();
			}
			this.IconId = node.Attributes["icon"].Value;
			this.PerkListIndex = 0;
			XmlNode xmlNode = node.Attributes["perk_list"];
			if (xmlNode != null)
			{
				this.PerkListIndex = Convert.ToInt32(xmlNode.Value);
				int perkListIndex = this.PerkListIndex;
				this.PerkListIndex = perkListIndex - 1;
			}
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode2 = (XmlNode)obj;
				if (xmlNode2.NodeType != XmlNodeType.Comment && xmlNode2.NodeType != XmlNodeType.SignificantWhitespace)
				{
					if (xmlNode2.Name == "ConditionalEffect")
					{
						this._conditionalEffects.Add(new MPConditionalEffect(this.GameModes, xmlNode2));
					}
					else if (xmlNode2.Name == "Effect")
					{
						this._effects.Add(MPPerkEffect.CreateFrom(xmlNode2));
					}
					else if (xmlNode2.Name == "OnSpawnEffect")
					{
						this._effects.Add(MPOnSpawnPerkEffect.CreateFrom(xmlNode2));
					}
					else if (xmlNode2.Name == "RandomOnSpawnEffect")
					{
						this._effects.Add(MPRandomOnSpawnPerkEffect.CreateFrom(xmlNode2));
					}
					else
					{
						Debug.FailedAssert("Unknown child element", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\MPPerkObject.cs", ".ctor", 761);
					}
				}
			}
			XmlAttribute xmlAttribute = node.Attributes["hero_idle_anim"];
			this.HeroIdleAnimOverride = ((xmlAttribute != null) ? xmlAttribute.Value : null);
			XmlAttribute xmlAttribute2 = node.Attributes["hero_mount_idle_anim"];
			this.HeroMountIdleAnimOverride = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
			XmlAttribute xmlAttribute3 = node.Attributes["troop_idle_anim"];
			this.TroopIdleAnimOverride = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
			XmlAttribute xmlAttribute4 = node.Attributes["troop_mount_idle_anim"];
			this.TroopMountIdleAnimOverride = ((xmlAttribute4 != null) ? xmlAttribute4.Value : null);
			this._perkEventFlags = MPPerkCondition.PerkEventFlags.None;
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				foreach (MPPerkCondition mpperkCondition in mpconditionalEffect.Conditions)
				{
					this._perkEventFlags |= mpperkCondition.EventFlags;
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect2 in this._conditionalEffects)
			{
				using (List<MPPerkCondition>.Enumerator enumerator3 = mpconditionalEffect2.Conditions.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						if (enumerator3.Current is BannerBearerCondition)
						{
							this.HasBannerBearer = true;
						}
					}
				}
			}
		}

		// Token: 0x06002ACC RID: 10956 RVA: 0x000A6488 File Offset: 0x000A4688
		public MPPerkObject Clone(MissionPeer peer)
		{
			return new MPPerkObject(peer, this._name, this._description, this.GameModes, this.PerkListIndex, this.IconId, this._conditionalEffects, this._effects, this.HeroIdleAnimOverride, this.HeroMountIdleAnimOverride, this.TroopIdleAnimOverride, this.TroopMountIdleAnimOverride);
		}

		// Token: 0x06002ACD RID: 10957 RVA: 0x000A64DD File Offset: 0x000A46DD
		public void Reset()
		{
			this._conditionalEffects.ResetStates();
		}

		// Token: 0x06002ACE RID: 10958 RVA: 0x000A64EC File Offset: 0x000A46EC
		private void OnEvent(bool isWarmup, MPPerkCondition.PerkEventFlags flags)
		{
			if ((flags & this._perkEventFlags) != MPPerkCondition.PerkEventFlags.None)
			{
				foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
				{
					if ((flags & mpconditionalEffect.EventFlags) != MPPerkCondition.PerkEventFlags.None)
					{
						mpconditionalEffect.OnEvent(isWarmup, this._peer, this._conditionalEffects);
					}
				}
			}
		}

		// Token: 0x06002ACF RID: 10959 RVA: 0x000A6560 File Offset: 0x000A4760
		private void OnEvent(bool isWarmup, Agent agent, MPPerkCondition.PerkEventFlags flags)
		{
			if (((agent != null) ? agent.MissionPeer : null) == null && agent != null)
			{
				MissionPeer owningAgentMissionPeer = agent.OwningAgentMissionPeer;
			}
			if ((flags & this._perkEventFlags) != MPPerkCondition.PerkEventFlags.None)
			{
				foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
				{
					if ((flags & mpconditionalEffect.EventFlags) != MPPerkCondition.PerkEventFlags.None)
					{
						mpconditionalEffect.OnEvent(isWarmup, agent, this._conditionalEffects);
					}
				}
			}
		}

		// Token: 0x06002AD0 RID: 10960 RVA: 0x000A65E8 File Offset: 0x000A47E8
		private void OnTick(bool isWarmup, int tickCount)
		{
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.IsTickRequired)
				{
					mpconditionalEffect.OnTick(isWarmup, this._peer, tickCount);
				}
			}
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if ((!isWarmup || !mpperkEffectBase.IsDisabledInWarmup) && mpperkEffectBase.IsTickRequired)
				{
					mpperkEffectBase.OnTick(this._peer, tickCount);
				}
			}
		}

		// Token: 0x06002AD1 RID: 10961 RVA: 0x000A66A8 File Offset: 0x000A48A8
		private float GetDamage(bool isWarmup, Agent agent, WeaponComponentData attackerWeapon, DamageTypes damageType, bool isAlternativeAttack)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetDamage(attackerWeapon, damageType, isAlternativeAttack);
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetDamage(attackerWeapon, damageType, isAlternativeAttack);
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002AD2 RID: 10962 RVA: 0x000A67CC File Offset: 0x000A49CC
		private float GetMountDamage(bool isWarmup, Agent agent, WeaponComponentData attackerWeapon, DamageTypes damageType, bool isAlternativeAttack)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetMountDamage(attackerWeapon, damageType, isAlternativeAttack);
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetMountDamage(attackerWeapon, damageType, isAlternativeAttack);
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002AD3 RID: 10963 RVA: 0x000A68F0 File Offset: 0x000A4AF0
		private float GetDamageTaken(bool isWarmup, Agent agent, WeaponComponentData attackerWeapon, DamageTypes damageType)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetDamageTaken(attackerWeapon, damageType);
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetDamageTaken(attackerWeapon, damageType);
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002AD4 RID: 10964 RVA: 0x000A6A10 File Offset: 0x000A4C10
		private float GetMountDamageTaken(bool isWarmup, Agent agent, WeaponComponentData attackerWeapon, DamageTypes damageType)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetMountDamageTaken(attackerWeapon, damageType);
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetMountDamageTaken(attackerWeapon, damageType);
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002AD5 RID: 10965 RVA: 0x000A6B30 File Offset: 0x000A4D30
		private float GetSpeedBonusEffectiveness(bool isWarmup, Agent agent)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetSpeedBonusEffectiveness(agent);
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetSpeedBonusEffectiveness(agent);
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002AD6 RID: 10966 RVA: 0x000A6C4C File Offset: 0x000A4E4C
		private float GetShieldDamage(bool isWarmup, Agent attacker, Agent defender, bool isCorrectSideBlock)
		{
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetShieldDamage(isCorrectSideBlock);
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(attacker))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetShieldDamage(isCorrectSideBlock);
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002AD7 RID: 10967 RVA: 0x000A6D50 File Offset: 0x000A4F50
		private float GetShieldDamageTaken(bool isWarmup, Agent attacker, Agent defender, bool isCorrectSideBlock)
		{
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetShieldDamageTaken(isCorrectSideBlock);
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(defender))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetShieldDamageTaken(isCorrectSideBlock);
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002AD8 RID: 10968 RVA: 0x000A6E54 File Offset: 0x000A5054
		private float GetRangedAccuracy(bool isWarmup, Agent agent)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetRangedAccuracy();
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetRangedAccuracy();
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002AD9 RID: 10969 RVA: 0x000A6F70 File Offset: 0x000A5170
		private float GetThrowingWeaponSpeed(bool isWarmup, Agent agent, WeaponComponentData attackerWeapon)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetThrowingWeaponSpeed(attackerWeapon);
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetThrowingWeaponSpeed(attackerWeapon);
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002ADA RID: 10970 RVA: 0x000A708C File Offset: 0x000A528C
		private float GetDamageInterruptionThreshold(bool isWarmup, Agent agent)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetDamageInterruptionThreshold();
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetDamageInterruptionThreshold();
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002ADB RID: 10971 RVA: 0x000A71A8 File Offset: 0x000A53A8
		private float GetMountManeuver(bool isWarmup, Agent agent)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetMountManeuver();
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetMountManeuver();
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002ADC RID: 10972 RVA: 0x000A72C4 File Offset: 0x000A54C4
		private float GetMountSpeed(bool isWarmup, Agent agent)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetMountSpeed();
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetMountSpeed();
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002ADD RID: 10973 RVA: 0x000A73E0 File Offset: 0x000A55E0
		private float GetRangedHeadShotDamage(bool isWarmup, Agent agent)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetRangedHeadShotDamage();
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetRangedHeadShotDamage();
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002ADE RID: 10974 RVA: 0x000A74FC File Offset: 0x000A56FC
		public float GetTroopCountMultiplier(bool isWarmup)
		{
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				IOnSpawnPerkEffect onSpawnPerkEffect = mpperkEffectBase as IOnSpawnPerkEffect;
				if (onSpawnPerkEffect != null && (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup))
				{
					num += onSpawnPerkEffect.GetTroopCountMultiplier();
				}
			}
			return num;
		}

		// Token: 0x06002ADF RID: 10975 RVA: 0x000A7570 File Offset: 0x000A5770
		public float GetExtraTroopCount(bool isWarmup)
		{
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				IOnSpawnPerkEffect onSpawnPerkEffect = mpperkEffectBase as IOnSpawnPerkEffect;
				if (onSpawnPerkEffect != null && (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup))
				{
					num += onSpawnPerkEffect.GetExtraTroopCount();
				}
			}
			return num;
		}

		// Token: 0x06002AE0 RID: 10976 RVA: 0x000A75E4 File Offset: 0x000A57E4
		public List<ValueTuple<EquipmentIndex, EquipmentElement>> GetAlternativeEquipments(bool isWarmup, bool isPlayer, List<ValueTuple<EquipmentIndex, EquipmentElement>> alternativeEquipments, bool getAllEquipments = false)
		{
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				IOnSpawnPerkEffect onSpawnPerkEffect = mpperkEffectBase as IOnSpawnPerkEffect;
				if (onSpawnPerkEffect != null && (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup))
				{
					alternativeEquipments = onSpawnPerkEffect.GetAlternativeEquipments(isPlayer, alternativeEquipments, getAllEquipments);
				}
			}
			return alternativeEquipments;
		}

		// Token: 0x06002AE1 RID: 10977 RVA: 0x000A7654 File Offset: 0x000A5854
		private float GetDrivenPropertyBonus(bool isWarmup, Agent agent, DrivenProperty drivenProperty, float baseValue)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetDrivenPropertyBonus(drivenProperty, baseValue);
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				float num2 = 0f;
				foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
				{
					if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
					{
						num += mpperkEffectBase2.GetDrivenPropertyBonus(drivenProperty, baseValue);
					}
				}
				if (num2 != 0f && mpconditionalEffect.Check(agent))
				{
					num += num2;
				}
			}
			return num;
		}

		// Token: 0x06002AE2 RID: 10978 RVA: 0x000A7788 File Offset: 0x000A5988
		public float GetDrivenPropertyBonusOnSpawn(bool isWarmup, bool isPlayer, DrivenProperty drivenProperty, float baseValue)
		{
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				IOnSpawnPerkEffect onSpawnPerkEffect = mpperkEffectBase as IOnSpawnPerkEffect;
				if (onSpawnPerkEffect != null && (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup))
				{
					num += onSpawnPerkEffect.GetDrivenPropertyBonusOnSpawn(isPlayer, drivenProperty, baseValue);
				}
			}
			return num;
		}

		// Token: 0x06002AE3 RID: 10979 RVA: 0x000A7800 File Offset: 0x000A5A00
		public float GetHitpoints(bool isWarmup, bool isPlayer)
		{
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				IOnSpawnPerkEffect onSpawnPerkEffect = mpperkEffectBase as IOnSpawnPerkEffect;
				if (onSpawnPerkEffect != null && (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup))
				{
					num += onSpawnPerkEffect.GetHitpoints(isPlayer);
				}
			}
			return num;
		}

		// Token: 0x06002AE4 RID: 10980 RVA: 0x000A7874 File Offset: 0x000A5A74
		private float GetEncumbrance(bool isWarmup, Agent agent, bool isOnBody)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			float num = 0f;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetEncumbrance(isOnBody);
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetEncumbrance(isOnBody);
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002AE5 RID: 10981 RVA: 0x000A7990 File Offset: 0x000A5B90
		private int GetGoldOnKill(bool isWarmup, Agent agent, float attackerValue, float victimValue)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			int num = 0;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetGoldOnKill(attackerValue, victimValue);
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetGoldOnKill(attackerValue, victimValue);
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002AE6 RID: 10982 RVA: 0x000A7AAC File Offset: 0x000A5CAC
		private int GetGoldOnAssist(bool isWarmup, Agent agent)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			int num = 0;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetGoldOnAssist();
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetGoldOnAssist();
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002AE7 RID: 10983 RVA: 0x000A7BC4 File Offset: 0x000A5DC4
		private int GetRewardedGoldOnAssist(bool isWarmup, Agent agent)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			int num = 0;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					num += mpperkEffectBase.GetRewardedGoldOnAssist();
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							num += mpperkEffectBase2.GetRewardedGoldOnAssist();
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002AE8 RID: 10984 RVA: 0x000A7CDC File Offset: 0x000A5EDC
		private bool GetIsTeamRewardedOnDeath(bool isWarmup, Agent agent)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if ((!isWarmup || !mpperkEffectBase.IsDisabledInWarmup) && mpperkEffectBase.GetIsTeamRewardedOnDeath())
				{
					return true;
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if ((!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup) && mpperkEffectBase2.GetIsTeamRewardedOnDeath())
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06002AE9 RID: 10985 RVA: 0x000A7DFC File Offset: 0x000A5FFC
		private void CalculateRewardedGoldOnDeath(bool isWarmup, Agent agent, List<ValueTuple<MissionPeer, int>> teamMembers)
		{
			Agent agent2;
			if ((agent2 = agent) == null)
			{
				MissionPeer peer = this._peer;
				agent2 = ((peer != null) ? peer.ControlledAgent : null);
			}
			agent = agent2;
			teamMembers.Shuffle<ValueTuple<MissionPeer, int>>();
			foreach (MPPerkEffectBase mpperkEffectBase in this._effects)
			{
				if (!isWarmup || !mpperkEffectBase.IsDisabledInWarmup)
				{
					mpperkEffectBase.CalculateRewardedGoldOnDeath(agent, teamMembers);
				}
			}
			foreach (MPConditionalEffect mpconditionalEffect in this._conditionalEffects)
			{
				if (mpconditionalEffect.Check(agent))
				{
					foreach (MPPerkEffectBase mpperkEffectBase2 in mpconditionalEffect.Effects)
					{
						if (!isWarmup || !mpperkEffectBase2.IsDisabledInWarmup)
						{
							mpperkEffectBase2.CalculateRewardedGoldOnDeath(agent, teamMembers);
						}
					}
				}
			}
		}

		// Token: 0x06002AEA RID: 10986 RVA: 0x000A7F10 File Offset: 0x000A6110
		public static int GetTroopCount(MultiplayerClassDivisions.MPHeroClass heroClass, MPPerkObject.MPOnSpawnPerkHandler onSpawnPerkHandler)
		{
			float num = (float)MathF.Ceiling((float)MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) * heroClass.TroopMultiplier);
			if (onSpawnPerkHandler != null)
			{
				num *= 1f + onSpawnPerkHandler.GetTroopCountMultiplier();
				num += onSpawnPerkHandler.GetExtraTroopCount();
			}
			return MathF.Max(MathF.Ceiling(num), 1);
		}

		// Token: 0x06002AEB RID: 10987 RVA: 0x000A7F5B File Offset: 0x000A615B
		public static IReadOnlyPerkObject Deserialize(XmlNode node)
		{
			return new MPPerkObject(node);
		}

		// Token: 0x06002AEC RID: 10988 RVA: 0x000A7F64 File Offset: 0x000A6164
		public static MPPerkObject.MPPerkHandler GetPerkHandler(Agent agent)
		{
			object obj;
			if (agent == null)
			{
				obj = null;
			}
			else
			{
				MissionPeer missionPeer = agent.MissionPeer;
				obj = ((missionPeer != null) ? missionPeer.SelectedPerks : null);
			}
			object obj2;
			if ((obj2 = obj) == null)
			{
				if (agent == null)
				{
					obj2 = null;
				}
				else
				{
					MissionPeer owningAgentMissionPeer = agent.OwningAgentMissionPeer;
					obj2 = ((owningAgentMissionPeer != null) ? owningAgentMissionPeer.SelectedPerks : null);
				}
			}
			MBReadOnlyList<MPPerkObject> mbreadOnlyList = obj2;
			if (mbreadOnlyList != null && mbreadOnlyList.Count > 0 && !agent.IsMount)
			{
				return new MPPerkObject.MPPerkHandlerInstance(agent);
			}
			return null;
		}

		// Token: 0x06002AED RID: 10989 RVA: 0x000A7FC4 File Offset: 0x000A61C4
		public static MPPerkObject.MPPerkHandler GetPerkHandler(MissionPeer peer)
		{
			MBReadOnlyList<MPPerkObject> mbreadOnlyList = ((peer != null) ? peer.SelectedPerks : null) ?? ((peer != null) ? peer.SelectedPerks : null);
			if (mbreadOnlyList != null && mbreadOnlyList.Count > 0)
			{
				return new MPPerkObject.MPPerkHandlerInstance(peer);
			}
			return null;
		}

		// Token: 0x06002AEE RID: 10990 RVA: 0x000A8004 File Offset: 0x000A6204
		public static MPPerkObject.MPCombatPerkHandler GetCombatPerkHandler(Agent attacker, Agent defender)
		{
			Agent agent = ((attacker != null && attacker.IsMount) ? attacker.RiderAgent : attacker);
			object obj;
			if (agent == null)
			{
				obj = null;
			}
			else
			{
				MissionPeer missionPeer = agent.MissionPeer;
				obj = ((missionPeer != null) ? missionPeer.SelectedPerks : null);
			}
			object obj2;
			if ((obj2 = obj) == null)
			{
				if (agent == null)
				{
					obj2 = null;
				}
				else
				{
					MissionPeer owningAgentMissionPeer = agent.OwningAgentMissionPeer;
					obj2 = ((owningAgentMissionPeer != null) ? owningAgentMissionPeer.SelectedPerks : null);
				}
			}
			MBReadOnlyList<MPPerkObject> mbreadOnlyList = obj2;
			Agent agent2 = ((defender != null && defender.IsMount) ? defender.RiderAgent : defender);
			object obj3;
			if (agent2 == null)
			{
				obj3 = null;
			}
			else
			{
				MissionPeer missionPeer2 = agent2.MissionPeer;
				obj3 = ((missionPeer2 != null) ? missionPeer2.SelectedPerks : null);
			}
			object obj4;
			if ((obj4 = obj3) == null)
			{
				if (agent2 == null)
				{
					obj4 = null;
				}
				else
				{
					MissionPeer owningAgentMissionPeer2 = agent2.OwningAgentMissionPeer;
					obj4 = ((owningAgentMissionPeer2 != null) ? owningAgentMissionPeer2.SelectedPerks : null);
				}
			}
			MBReadOnlyList<MPPerkObject> mbreadOnlyList2 = obj4;
			if (attacker != defender && ((mbreadOnlyList != null && mbreadOnlyList.Count > 0) || (mbreadOnlyList2 != null && mbreadOnlyList2.Count > 0)))
			{
				return new MPPerkObject.MPCombatPerkHandlerInstance(attacker, defender);
			}
			return null;
		}

		// Token: 0x06002AEF RID: 10991 RVA: 0x000A80CA File Offset: 0x000A62CA
		public static MPPerkObject.MPOnSpawnPerkHandler GetOnSpawnPerkHandler(MissionPeer peer)
		{
			if ((((peer != null) ? peer.SelectedPerks : null) ?? ((peer != null) ? peer.SelectedPerks : null)) != null)
			{
				return new MPPerkObject.MPOnSpawnPerkHandlerInstance(peer);
			}
			return null;
		}

		// Token: 0x06002AF0 RID: 10992 RVA: 0x000A80F2 File Offset: 0x000A62F2
		public static MPPerkObject.MPOnSpawnPerkHandler GetOnSpawnPerkHandler(IEnumerable<IReadOnlyPerkObject> perks)
		{
			if (perks != null)
			{
				return new MPPerkObject.MPOnSpawnPerkHandlerInstance(perks);
			}
			return null;
		}

		// Token: 0x06002AF1 RID: 10993 RVA: 0x000A8100 File Offset: 0x000A6300
		public static void RaiseEventForAllPeers(MPPerkCondition.PerkEventFlags flags)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(networkCommunicator.GetComponent<MissionPeer>());
					if (perkHandler != null)
					{
						perkHandler.OnEvent(flags);
					}
				}
			}
		}

		// Token: 0x06002AF2 RID: 10994 RVA: 0x000A8164 File Offset: 0x000A6364
		public static void RaiseEventForAllPeersOnTeam(Team side, MPPerkCondition.PerkEventFlags flags)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (component != null && component.Team == side)
					{
						MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(component);
						if (perkHandler != null)
						{
							perkHandler.OnEvent(flags);
						}
					}
				}
			}
		}

		// Token: 0x06002AF3 RID: 10995 RVA: 0x000A81D4 File Offset: 0x000A63D4
		public static void TickAllPeerPerks(int tickCount)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (component != null && component.Team != null && component.Culture != null && component.Team.Side != BattleSideEnum.None)
					{
						MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(component);
						if (perkHandler != null)
						{
							perkHandler.OnTick(tickCount);
						}
					}
				}
			}
		}

		// Token: 0x06002AF4 RID: 10996 RVA: 0x000A8258 File Offset: 0x000A6458
		[CommandLineFunctionality.CommandLineArgumentFunction("raise_event", "mp_perks")]
		public static string RaiseEventForAllPeersCommand(List<string> strings)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				MPPerkCondition.PerkEventFlags perkEventFlags = MPPerkCondition.PerkEventFlags.None;
				using (List<string>.Enumerator enumerator = strings.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MPPerkCondition.PerkEventFlags perkEventFlags2;
						if (Enum.TryParse<MPPerkCondition.PerkEventFlags>(enumerator.Current, true, out perkEventFlags2))
						{
							perkEventFlags |= perkEventFlags2;
						}
					}
				}
				MPPerkObject.RaiseEventForAllPeers(perkEventFlags);
				return "Raised event with flags " + perkEventFlags;
			}
			return "Can't run this command on clients";
		}

		// Token: 0x06002AF5 RID: 10997 RVA: 0x000A82D4 File Offset: 0x000A64D4
		[CommandLineFunctionality.CommandLineArgumentFunction("tick_perks", "mp_perks")]
		public static string TickAllPeerPerksCommand(List<string> strings)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				int num;
				if (strings.Count == 0 || !int.TryParse(strings[0], out num))
				{
					num = 1;
				}
				MPPerkObject.TickAllPeerPerks(num);
				return "Peer perks on tick with tick count " + num;
			}
			return "Can't run this command on clients";
		}

		// Token: 0x04001049 RID: 4169
		private readonly MissionPeer _peer;

		// Token: 0x0400104A RID: 4170
		private readonly MPConditionalEffect.ConditionalEffectContainer _conditionalEffects;

		// Token: 0x0400104B RID: 4171
		private readonly MPPerkCondition.PerkEventFlags _perkEventFlags;

		// Token: 0x0400104C RID: 4172
		private readonly string _name;

		// Token: 0x0400104D RID: 4173
		private readonly string _description;

		// Token: 0x0400104E RID: 4174
		private readonly List<MPPerkEffectBase> _effects;

		// Token: 0x02000634 RID: 1588
		private class MPOnSpawnPerkHandlerInstance : MPPerkObject.MPOnSpawnPerkHandler
		{
			// Token: 0x06003DEB RID: 15851 RVA: 0x000F3A59 File Offset: 0x000F1C59
			public MPOnSpawnPerkHandlerInstance(IEnumerable<IReadOnlyPerkObject> perks)
				: base(perks)
			{
			}

			// Token: 0x06003DEC RID: 15852 RVA: 0x000F3A62 File Offset: 0x000F1C62
			public MPOnSpawnPerkHandlerInstance(MissionPeer peer)
				: base(peer)
			{
			}
		}

		// Token: 0x02000635 RID: 1589
		private class MPPerkHandlerInstance : MPPerkObject.MPPerkHandler
		{
			// Token: 0x06003DED RID: 15853 RVA: 0x000F3A6B File Offset: 0x000F1C6B
			public MPPerkHandlerInstance(Agent agent)
				: base(agent)
			{
			}

			// Token: 0x06003DEE RID: 15854 RVA: 0x000F3A74 File Offset: 0x000F1C74
			public MPPerkHandlerInstance(MissionPeer peer)
				: base(peer)
			{
			}
		}

		// Token: 0x02000636 RID: 1590
		private class MPCombatPerkHandlerInstance : MPPerkObject.MPCombatPerkHandler
		{
			// Token: 0x06003DEF RID: 15855 RVA: 0x000F3A7D File Offset: 0x000F1C7D
			public MPCombatPerkHandlerInstance(Agent attacker, Agent defender)
				: base(attacker, defender)
			{
			}
		}

		// Token: 0x02000637 RID: 1591
		public class MPOnSpawnPerkHandler
		{
			// Token: 0x170009DF RID: 2527
			// (get) Token: 0x06003DF0 RID: 15856 RVA: 0x000F3A88 File Offset: 0x000F1C88
			public bool IsWarmup
			{
				get
				{
					Mission mission = Mission.Current;
					bool? flag;
					if (mission == null)
					{
						flag = null;
					}
					else
					{
						MissionMultiplayerGameModeBase missionBehavior = mission.GetMissionBehavior<MissionMultiplayerGameModeBase>();
						if (missionBehavior == null)
						{
							flag = null;
						}
						else
						{
							MultiplayerWarmupComponent warmupComponent = missionBehavior.WarmupComponent;
							flag = ((warmupComponent != null) ? new bool?(warmupComponent.IsInWarmup) : null);
						}
					}
					return flag ?? false;
				}
			}

			// Token: 0x06003DF1 RID: 15857 RVA: 0x000F3AEE File Offset: 0x000F1CEE
			protected MPOnSpawnPerkHandler(IEnumerable<IReadOnlyPerkObject> perks)
			{
				this._perks = perks;
			}

			// Token: 0x06003DF2 RID: 15858 RVA: 0x000F3AFD File Offset: 0x000F1CFD
			protected MPOnSpawnPerkHandler(MissionPeer peer)
			{
				this._perks = peer.SelectedPerks;
			}

			// Token: 0x06003DF3 RID: 15859 RVA: 0x000F3B14 File Offset: 0x000F1D14
			public float GetTroopCountMultiplier()
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (IReadOnlyPerkObject readOnlyPerkObject in this._perks)
				{
					num += readOnlyPerkObject.GetTroopCountMultiplier(isWarmup);
				}
				return num;
			}

			// Token: 0x06003DF4 RID: 15860 RVA: 0x000F3B74 File Offset: 0x000F1D74
			public float GetExtraTroopCount()
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (IReadOnlyPerkObject readOnlyPerkObject in this._perks)
				{
					num += readOnlyPerkObject.GetExtraTroopCount(isWarmup);
				}
				return num;
			}

			// Token: 0x06003DF5 RID: 15861 RVA: 0x000F3BD4 File Offset: 0x000F1DD4
			public IEnumerable<ValueTuple<EquipmentIndex, EquipmentElement>> GetAlternativeEquipments(bool isPlayer)
			{
				List<ValueTuple<EquipmentIndex, EquipmentElement>> list = null;
				bool isWarmup = this.IsWarmup;
				foreach (IReadOnlyPerkObject readOnlyPerkObject in this._perks)
				{
					list = readOnlyPerkObject.GetAlternativeEquipments(isWarmup, isPlayer, list, false);
				}
				return list;
			}

			// Token: 0x06003DF6 RID: 15862 RVA: 0x000F3C30 File Offset: 0x000F1E30
			public float GetDrivenPropertyBonusOnSpawn(bool isPlayer, DrivenProperty drivenProperty, float baseValue)
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (IReadOnlyPerkObject readOnlyPerkObject in this._perks)
				{
					num += readOnlyPerkObject.GetDrivenPropertyBonusOnSpawn(isWarmup, isPlayer, drivenProperty, baseValue);
				}
				return num;
			}

			// Token: 0x06003DF7 RID: 15863 RVA: 0x000F3C94 File Offset: 0x000F1E94
			public float GetHitpoints(bool isPlayer)
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (IReadOnlyPerkObject readOnlyPerkObject in this._perks)
				{
					num += readOnlyPerkObject.GetHitpoints(isWarmup, isPlayer);
				}
				return num;
			}

			// Token: 0x04002027 RID: 8231
			private IEnumerable<IReadOnlyPerkObject> _perks;
		}

		// Token: 0x02000638 RID: 1592
		public class MPPerkHandler
		{
			// Token: 0x170009E0 RID: 2528
			// (get) Token: 0x06003DF8 RID: 15864 RVA: 0x000F3CF4 File Offset: 0x000F1EF4
			public bool IsWarmup
			{
				get
				{
					Mission mission = Mission.Current;
					bool? flag;
					if (mission == null)
					{
						flag = null;
					}
					else
					{
						MissionMultiplayerGameModeBase missionBehavior = mission.GetMissionBehavior<MissionMultiplayerGameModeBase>();
						if (missionBehavior == null)
						{
							flag = null;
						}
						else
						{
							MultiplayerWarmupComponent warmupComponent = missionBehavior.WarmupComponent;
							flag = ((warmupComponent != null) ? new bool?(warmupComponent.IsInWarmup) : null);
						}
					}
					return flag ?? false;
				}
			}

			// Token: 0x06003DF9 RID: 15865 RVA: 0x000F3D5C File Offset: 0x000F1F5C
			protected MPPerkHandler(Agent agent)
			{
				this._agent = agent;
				Agent agent2 = this._agent;
				object obj;
				if (agent2 == null)
				{
					obj = null;
				}
				else
				{
					MissionPeer missionPeer = agent2.MissionPeer;
					obj = ((missionPeer != null) ? missionPeer.SelectedPerks : null);
				}
				object obj2;
				if ((obj2 = obj) == null)
				{
					Agent agent3 = this._agent;
					if (agent3 == null)
					{
						obj2 = null;
					}
					else
					{
						MissionPeer owningAgentMissionPeer = agent3.OwningAgentMissionPeer;
						obj2 = ((owningAgentMissionPeer != null) ? owningAgentMissionPeer.SelectedPerks : null);
					}
				}
				this._perks = obj2 ?? new MBList<MPPerkObject>();
			}

			// Token: 0x06003DFA RID: 15866 RVA: 0x000F3DC5 File Offset: 0x000F1FC5
			protected MPPerkHandler(MissionPeer peer)
			{
				this._agent = ((peer != null) ? peer.ControlledAgent : null);
				this._perks = ((peer != null) ? peer.SelectedPerks : null) ?? new MBList<MPPerkObject>();
			}

			// Token: 0x06003DFB RID: 15867 RVA: 0x000F3DFC File Offset: 0x000F1FFC
			public void OnEvent(MPPerkCondition.PerkEventFlags flags)
			{
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					mpperkObject.OnEvent(isWarmup, flags);
				}
			}

			// Token: 0x06003DFC RID: 15868 RVA: 0x000F3E58 File Offset: 0x000F2058
			public void OnEvent(Agent agent, MPPerkCondition.PerkEventFlags flags)
			{
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					mpperkObject.OnEvent(isWarmup, agent, flags);
				}
			}

			// Token: 0x06003DFD RID: 15869 RVA: 0x000F3EB4 File Offset: 0x000F20B4
			public void OnTick(int tickCount)
			{
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					mpperkObject.OnTick(isWarmup, tickCount);
				}
			}

			// Token: 0x06003DFE RID: 15870 RVA: 0x000F3F10 File Offset: 0x000F2110
			public float GetDrivenPropertyBonus(DrivenProperty drivenProperty, float baseValue)
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					num += mpperkObject.GetDrivenPropertyBonus(isWarmup, this._agent, drivenProperty, baseValue);
				}
				return num;
			}

			// Token: 0x06003DFF RID: 15871 RVA: 0x000F3F7C File Offset: 0x000F217C
			public float GetRangedAccuracy()
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					num += mpperkObject.GetRangedAccuracy(isWarmup, this._agent);
				}
				return num;
			}

			// Token: 0x06003E00 RID: 15872 RVA: 0x000F3FE8 File Offset: 0x000F21E8
			public float GetThrowingWeaponSpeed(WeaponComponentData attackerWeapon)
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					num += mpperkObject.GetThrowingWeaponSpeed(isWarmup, this._agent, attackerWeapon);
				}
				return num;
			}

			// Token: 0x06003E01 RID: 15873 RVA: 0x000F4054 File Offset: 0x000F2254
			public float GetDamageInterruptionThreshold()
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					num += mpperkObject.GetDamageInterruptionThreshold(isWarmup, this._agent);
				}
				return num;
			}

			// Token: 0x06003E02 RID: 15874 RVA: 0x000F40C0 File Offset: 0x000F22C0
			public float GetMountManeuver()
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					num += mpperkObject.GetMountManeuver(isWarmup, this._agent);
				}
				return num;
			}

			// Token: 0x06003E03 RID: 15875 RVA: 0x000F412C File Offset: 0x000F232C
			public float GetMountSpeed()
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					num += mpperkObject.GetMountSpeed(isWarmup, this._agent);
				}
				return num;
			}

			// Token: 0x06003E04 RID: 15876 RVA: 0x000F4198 File Offset: 0x000F2398
			public int GetGoldOnKill(float attackerValue, float victimValue)
			{
				int num = 0;
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					num += mpperkObject.GetGoldOnKill(isWarmup, this._agent, attackerValue, victimValue);
				}
				return num;
			}

			// Token: 0x06003E05 RID: 15877 RVA: 0x000F4200 File Offset: 0x000F2400
			public int GetGoldOnAssist()
			{
				int num = 0;
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					num += mpperkObject.GetGoldOnAssist(isWarmup, this._agent);
				}
				return num;
			}

			// Token: 0x06003E06 RID: 15878 RVA: 0x000F4268 File Offset: 0x000F2468
			public int GetRewardedGoldOnAssist()
			{
				int num = 0;
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					num += mpperkObject.GetRewardedGoldOnAssist(isWarmup, this._agent);
				}
				return num;
			}

			// Token: 0x06003E07 RID: 15879 RVA: 0x000F42D0 File Offset: 0x000F24D0
			public bool GetIsTeamRewardedOnDeath()
			{
				bool isWarmup = this.IsWarmup;
				using (List<MPPerkObject>.Enumerator enumerator = this._perks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.GetIsTeamRewardedOnDeath(isWarmup, this._agent))
						{
							return true;
						}
					}
				}
				return false;
			}

			// Token: 0x06003E08 RID: 15880 RVA: 0x000F4338 File Offset: 0x000F2538
			public IEnumerable<ValueTuple<MissionPeer, int>> GetTeamGoldRewardsOnDeath()
			{
				if (this.GetIsTeamRewardedOnDeath())
				{
					Agent agent = this._agent;
					MissionPeer missionPeer;
					if ((missionPeer = ((agent != null) ? agent.MissionPeer : null)) == null)
					{
						Agent agent2 = this._agent;
						missionPeer = ((agent2 != null) ? agent2.OwningAgentMissionPeer : null);
					}
					MissionPeer missionPeer2 = missionPeer;
					List<ValueTuple<MissionPeer, int>> list = new List<ValueTuple<MissionPeer, int>>();
					foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
					{
						MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
						if (component != missionPeer2 && component.Team == missionPeer2.Team)
						{
							list.Add(new ValueTuple<MissionPeer, int>(component, 0));
						}
					}
					bool isWarmup = this.IsWarmup;
					foreach (MPPerkObject mpperkObject in this._perks)
					{
						mpperkObject.CalculateRewardedGoldOnDeath(isWarmup, this._agent, list);
					}
					return list;
				}
				return null;
			}

			// Token: 0x06003E09 RID: 15881 RVA: 0x000F4434 File Offset: 0x000F2634
			public float GetEncumbrance(bool isOnBody)
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					num += mpperkObject.GetEncumbrance(isWarmup, this._agent, isOnBody);
				}
				return num;
			}

			// Token: 0x04002028 RID: 8232
			private readonly Agent _agent;

			// Token: 0x04002029 RID: 8233
			private readonly MBReadOnlyList<MPPerkObject> _perks;
		}

		// Token: 0x02000639 RID: 1593
		public class MPCombatPerkHandler
		{
			// Token: 0x170009E1 RID: 2529
			// (get) Token: 0x06003E0A RID: 15882 RVA: 0x000F44A0 File Offset: 0x000F26A0
			public bool IsWarmup
			{
				get
				{
					Mission mission = Mission.Current;
					bool? flag;
					if (mission == null)
					{
						flag = null;
					}
					else
					{
						MissionMultiplayerGameModeBase missionBehavior = mission.GetMissionBehavior<MissionMultiplayerGameModeBase>();
						if (missionBehavior == null)
						{
							flag = null;
						}
						else
						{
							MultiplayerWarmupComponent warmupComponent = missionBehavior.WarmupComponent;
							flag = ((warmupComponent != null) ? new bool?(warmupComponent.IsInWarmup) : null);
						}
					}
					return flag ?? false;
				}
			}

			// Token: 0x06003E0B RID: 15883 RVA: 0x000F4508 File Offset: 0x000F2708
			protected MPCombatPerkHandler(Agent attacker, Agent defender)
			{
				this._attacker = attacker;
				this._defender = defender;
				attacker = ((attacker != null && attacker.IsMount) ? attacker.RiderAgent : attacker);
				defender = ((defender != null && defender.IsMount) ? defender.RiderAgent : defender);
				MBList<MPPerkObject> mblist;
				if (attacker == null)
				{
					mblist = null;
				}
				else
				{
					MissionPeer missionPeer = attacker.MissionPeer;
					mblist = ((missionPeer != null) ? missionPeer.SelectedPerks : null);
				}
				MBList<MPPerkObject> mblist2;
				if ((mblist2 = mblist) == null)
				{
					MBList<MPPerkObject> mblist3;
					if (attacker == null)
					{
						mblist3 = null;
					}
					else
					{
						MissionPeer owningAgentMissionPeer = attacker.OwningAgentMissionPeer;
						mblist3 = ((owningAgentMissionPeer != null) ? owningAgentMissionPeer.SelectedPerks : null);
					}
					mblist2 = mblist3 ?? new MBList<MPPerkObject>();
				}
				this._attackerPerks = mblist2;
				MBList<MPPerkObject> mblist4;
				if (defender == null)
				{
					mblist4 = null;
				}
				else
				{
					MissionPeer missionPeer2 = defender.MissionPeer;
					mblist4 = ((missionPeer2 != null) ? missionPeer2.SelectedPerks : null);
				}
				MBList<MPPerkObject> mblist5;
				if ((mblist5 = mblist4) == null)
				{
					MBList<MPPerkObject> mblist6;
					if (defender == null)
					{
						mblist6 = null;
					}
					else
					{
						MissionPeer owningAgentMissionPeer2 = defender.OwningAgentMissionPeer;
						mblist6 = ((owningAgentMissionPeer2 != null) ? owningAgentMissionPeer2.SelectedPerks : null);
					}
					mblist5 = mblist6 ?? new MBList<MPPerkObject>();
				}
				this._defenderPerks = mblist5;
			}

			// Token: 0x06003E0C RID: 15884 RVA: 0x000F45DC File Offset: 0x000F27DC
			public float GetDamage(WeaponComponentData attackerWeapon, DamageTypes damageType, bool isAlternativeAttack)
			{
				float num = 0f;
				if (this._attackerPerks.Count > 0 && this._defender != null)
				{
					bool isWarmup = this.IsWarmup;
					if (this._defender.IsMount)
					{
						foreach (MPPerkObject mpperkObject in this._attackerPerks)
						{
							num += mpperkObject.GetMountDamage(isWarmup, this._attacker, attackerWeapon, damageType, isAlternativeAttack);
						}
					}
					foreach (MPPerkObject mpperkObject2 in this._attackerPerks)
					{
						num += mpperkObject2.GetDamage(isWarmup, this._attacker, attackerWeapon, damageType, isAlternativeAttack);
					}
				}
				return num;
			}

			// Token: 0x06003E0D RID: 15885 RVA: 0x000F46C4 File Offset: 0x000F28C4
			public float GetDamageTaken(WeaponComponentData attackerWeapon, DamageTypes damageType)
			{
				float num = 0f;
				if (this._defenderPerks.Count > 0)
				{
					bool isWarmup = this.IsWarmup;
					if (this._defender.IsMount)
					{
						using (List<MPPerkObject>.Enumerator enumerator = this._defenderPerks.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								MPPerkObject mpperkObject = enumerator.Current;
								num += mpperkObject.GetMountDamageTaken(isWarmup, this._defender, attackerWeapon, damageType);
							}
							return num;
						}
					}
					foreach (MPPerkObject mpperkObject2 in this._defenderPerks)
					{
						num += mpperkObject2.GetDamageTaken(isWarmup, this._defender, attackerWeapon, damageType);
					}
				}
				return num;
			}

			// Token: 0x06003E0E RID: 15886 RVA: 0x000F47A0 File Offset: 0x000F29A0
			public float GetSpeedBonusEffectiveness()
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._attackerPerks)
				{
					num += mpperkObject.GetSpeedBonusEffectiveness(isWarmup, this._attacker);
				}
				return num;
			}

			// Token: 0x06003E0F RID: 15887 RVA: 0x000F480C File Offset: 0x000F2A0C
			public float GetShieldDamage(bool isCorrectSideBlock)
			{
				float num = 0f;
				if (this._defender != null)
				{
					bool isWarmup = this.IsWarmup;
					foreach (MPPerkObject mpperkObject in this._attackerPerks)
					{
						num += mpperkObject.GetShieldDamage(isWarmup, this._attacker, this._defender, isCorrectSideBlock);
					}
				}
				return num;
			}

			// Token: 0x06003E10 RID: 15888 RVA: 0x000F4888 File Offset: 0x000F2A88
			public float GetShieldDamageTaken(bool isCorrectSideBlock)
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._defenderPerks)
				{
					num += mpperkObject.GetShieldDamageTaken(isWarmup, this._attacker, this._defender, isCorrectSideBlock);
				}
				return num;
			}

			// Token: 0x06003E11 RID: 15889 RVA: 0x000F48FC File Offset: 0x000F2AFC
			public float GetRangedHeadShotDamage()
			{
				float num = 0f;
				if (this._attacker != null)
				{
					bool isWarmup = this.IsWarmup;
					foreach (MPPerkObject mpperkObject in this._attackerPerks)
					{
						num += mpperkObject.GetRangedHeadShotDamage(isWarmup, this._attacker);
					}
				}
				return num;
			}

			// Token: 0x0400202A RID: 8234
			private readonly Agent _attacker;

			// Token: 0x0400202B RID: 8235
			private readonly Agent _defender;

			// Token: 0x0400202C RID: 8236
			private readonly MBReadOnlyList<MPPerkObject> _attackerPerks;

			// Token: 0x0400202D RID: 8237
			private readonly MBReadOnlyList<MPPerkObject> _defenderPerks;
		}
	}
}
