using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions;

namespace TaleWorlds.MountAndBlade
{
	public class MPPerkObject : IReadOnlyPerkObject
	{
		public TextObject Name
		{
			get
			{
				return new TextObject(this._name, null);
			}
		}

		public TextObject Description
		{
			get
			{
				return new TextObject(this._description, null);
			}
		}

		public bool HasBannerBearer { get; }

		public List<string> GameModes { get; }

		public int PerkListIndex { get; }

		public string IconId { get; }

		public string HeroIdleAnimOverride { get; }

		public string HeroMountIdleAnimOverride { get; }

		public string TroopIdleAnimOverride { get; }

		public string TroopMountIdleAnimOverride { get; }

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
						Debug.FailedAssert("Unknown child element", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\MPPerkObject.cs", ".ctor", 750);
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

		public MPPerkObject Clone(MissionPeer peer)
		{
			return new MPPerkObject(peer, this._name, this._description, this.GameModes, this.PerkListIndex, this.IconId, this._conditionalEffects, this._effects, this.HeroIdleAnimOverride, this.HeroMountIdleAnimOverride, this.TroopIdleAnimOverride, this.TroopMountIdleAnimOverride);
		}

		public void Reset()
		{
			this._conditionalEffects.ResetStates();
		}

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

		public int GetExtraTroopCount(bool isWarmup)
		{
			int num = 0;
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

		public static int GetTroopCount(MultiplayerClassDivisions.MPHeroClass heroClass, int botsPerFormation, MPPerkObject.MPOnSpawnPerkHandler onSpawnPerkHandler)
		{
			int num = MathF.Ceiling((float)botsPerFormation * heroClass.TroopMultiplier - 1E-05f);
			if (onSpawnPerkHandler != null)
			{
				num += (int)onSpawnPerkHandler.GetExtraTroopCount();
			}
			return MathF.Max(num, 1);
		}

		public static IReadOnlyPerkObject Deserialize(XmlNode node)
		{
			return new MPPerkObject(node);
		}

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

		public static MPPerkObject.MPPerkHandler GetPerkHandler(MissionPeer peer)
		{
			MBReadOnlyList<MPPerkObject> mbreadOnlyList = ((peer != null) ? peer.SelectedPerks : null) ?? ((peer != null) ? peer.SelectedPerks : null);
			if (mbreadOnlyList != null && mbreadOnlyList.Count > 0)
			{
				return new MPPerkObject.MPPerkHandlerInstance(peer);
			}
			return null;
		}

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

		public static MPPerkObject.MPOnSpawnPerkHandler GetOnSpawnPerkHandler(MissionPeer peer)
		{
			if ((((peer != null) ? peer.SelectedPerks : null) ?? ((peer != null) ? peer.SelectedPerks : null)) != null)
			{
				return new MPPerkObject.MPOnSpawnPerkHandlerInstance(peer);
			}
			return null;
		}

		public static MPPerkObject.MPOnSpawnPerkHandler GetOnSpawnPerkHandler(IEnumerable<IReadOnlyPerkObject> perks)
		{
			if (perks != null)
			{
				return new MPPerkObject.MPOnSpawnPerkHandlerInstance(perks);
			}
			return null;
		}

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

		private readonly MissionPeer _peer;

		private readonly MPConditionalEffect.ConditionalEffectContainer _conditionalEffects;

		private readonly MPPerkCondition.PerkEventFlags _perkEventFlags;

		private readonly string _name;

		private readonly string _description;

		private readonly List<MPPerkEffectBase> _effects;

		private class MPOnSpawnPerkHandlerInstance : MPPerkObject.MPOnSpawnPerkHandler
		{
			public MPOnSpawnPerkHandlerInstance(IEnumerable<IReadOnlyPerkObject> perks)
				: base(perks)
			{
			}

			public MPOnSpawnPerkHandlerInstance(MissionPeer peer)
				: base(peer)
			{
			}
		}

		private class MPPerkHandlerInstance : MPPerkObject.MPPerkHandler
		{
			public MPPerkHandlerInstance(Agent agent)
				: base(agent)
			{
			}

			public MPPerkHandlerInstance(MissionPeer peer)
				: base(peer)
			{
			}
		}

		private class MPCombatPerkHandlerInstance : MPPerkObject.MPCombatPerkHandler
		{
			public MPCombatPerkHandlerInstance(Agent attacker, Agent defender)
				: base(attacker, defender)
			{
			}
		}

		public class MPOnSpawnPerkHandler
		{
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

			protected MPOnSpawnPerkHandler(IEnumerable<IReadOnlyPerkObject> perks)
			{
				this._perks = perks;
			}

			protected MPOnSpawnPerkHandler(MissionPeer peer)
			{
				this._perks = peer.SelectedPerks;
			}

			public float GetExtraTroopCount()
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (IReadOnlyPerkObject readOnlyPerkObject in this._perks)
				{
					if (readOnlyPerkObject != null)
					{
						num += (float)readOnlyPerkObject.GetExtraTroopCount(isWarmup);
					}
				}
				return num;
			}

			public IEnumerable<ValueTuple<EquipmentIndex, EquipmentElement>> GetAlternativeEquipments(bool isPlayer)
			{
				List<ValueTuple<EquipmentIndex, EquipmentElement>> list = null;
				bool isWarmup = this.IsWarmup;
				foreach (IReadOnlyPerkObject readOnlyPerkObject in this._perks)
				{
					if (readOnlyPerkObject != null)
					{
						list = readOnlyPerkObject.GetAlternativeEquipments(isWarmup, isPlayer, list, false);
					}
				}
				return list;
			}

			public float GetDrivenPropertyBonusOnSpawn(bool isPlayer, DrivenProperty drivenProperty, float baseValue)
			{
				float num = 0f;
				bool isWarmup = this.IsWarmup;
				foreach (IReadOnlyPerkObject readOnlyPerkObject in this._perks)
				{
					if (readOnlyPerkObject != null)
					{
						num += readOnlyPerkObject.GetDrivenPropertyBonusOnSpawn(isWarmup, isPlayer, drivenProperty, baseValue);
					}
				}
				return num;
			}

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

			private IEnumerable<IReadOnlyPerkObject> _perks;
		}

		public class MPPerkHandler
		{
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

			protected MPPerkHandler(MissionPeer peer)
			{
				this._agent = ((peer != null) ? peer.ControlledAgent : null);
				this._perks = ((peer != null) ? peer.SelectedPerks : null) ?? new MBList<MPPerkObject>();
			}

			public void OnEvent(MPPerkCondition.PerkEventFlags flags)
			{
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					mpperkObject.OnEvent(isWarmup, flags);
				}
			}

			public void OnEvent(Agent agent, MPPerkCondition.PerkEventFlags flags)
			{
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					mpperkObject.OnEvent(isWarmup, agent, flags);
				}
			}

			public void OnTick(int tickCount)
			{
				bool isWarmup = this.IsWarmup;
				foreach (MPPerkObject mpperkObject in this._perks)
				{
					mpperkObject.OnTick(isWarmup, tickCount);
				}
			}

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

			private readonly Agent _agent;

			private readonly MBReadOnlyList<MPPerkObject> _perks;
		}

		public class MPCombatPerkHandler
		{
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

			private readonly Agent _attacker;

			private readonly Agent _defender;

			private readonly MBReadOnlyList<MPPerkObject> _attackerPerks;

			private readonly MBReadOnlyList<MPPerkObject> _defenderPerks;
		}
	}
}
