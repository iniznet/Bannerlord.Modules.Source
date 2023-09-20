using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	// Token: 0x02000404 RID: 1028
	public abstract class AgentApplyDamageModel : GameModel
	{
		// Token: 0x06003543 RID: 13635
		public abstract float CalculateDamage(in AttackInformation attackInformation, in AttackCollisionData collisionData, in MissionWeapon weapon, float baseDamage);

		// Token: 0x06003544 RID: 13636
		public abstract void DecideMissileWeaponFlags(Agent attackerAgent, MissionWeapon missileWeapon, ref WeaponFlags missileWeaponFlags);

		// Token: 0x06003545 RID: 13637
		public abstract void CalculateCollisionStunMultipliers(Agent attackerAgent, Agent defenderAgent, bool isAlternativeAttack, CombatCollisionResult collisionResult, WeaponComponentData attackerWeapon, WeaponComponentData defenderWeapon, out float attackerStunMultiplier, out float defenderStunMultiplier);

		// Token: 0x06003546 RID: 13638
		public abstract float CalculateStaggerThresholdMultiplier(Agent defenderAgent);

		// Token: 0x06003547 RID: 13639
		public abstract float CalculatePassiveAttackDamage(BasicCharacterObject attackerCharacter, in AttackCollisionData collisionData, float baseDamage);

		// Token: 0x06003548 RID: 13640
		public abstract MeleeCollisionReaction DecidePassiveAttackCollisionReaction(Agent attacker, Agent defender, bool isFatalHit);

		// Token: 0x06003549 RID: 13641
		public abstract float CalculateShieldDamage(in AttackInformation attackInformation, float baseDamage);

		// Token: 0x0600354A RID: 13642
		public abstract float GetDamageMultiplierForBodyPart(BoneBodyPartType bodyPart, DamageTypes type, bool isHuman);

		// Token: 0x0600354B RID: 13643
		public abstract bool CanWeaponIgnoreFriendlyFireChecks(WeaponComponentData weapon);

		// Token: 0x0600354C RID: 13644
		public abstract bool CanWeaponDismount(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData);

		// Token: 0x0600354D RID: 13645
		public abstract bool CanWeaponKnockback(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData);

		// Token: 0x0600354E RID: 13646
		public abstract bool CanWeaponKnockDown(Agent attackerAgent, Agent victimAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData);

		// Token: 0x0600354F RID: 13647
		public abstract bool DecideCrushedThrough(Agent attackerAgent, Agent defenderAgent, float totalAttackEnergy, Agent.UsageDirection attackDirection, StrikeType strikeType, WeaponComponentData defendItem, bool isPassiveUsageHit);

		// Token: 0x06003550 RID: 13648
		public abstract bool DecideAgentShrugOffBlow(Agent victimAgent, AttackCollisionData collisionData, in Blow blow);

		// Token: 0x06003551 RID: 13649
		public abstract bool DecideAgentDismountedByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow);

		// Token: 0x06003552 RID: 13650
		public abstract bool DecideAgentKnockedBackByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow);

		// Token: 0x06003553 RID: 13651
		public abstract bool DecideAgentKnockedDownByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow);

		// Token: 0x06003554 RID: 13652
		public abstract bool DecideMountRearedByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow);

		// Token: 0x06003555 RID: 13653
		public abstract float GetDismountPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData);

		// Token: 0x06003556 RID: 13654
		public abstract float GetKnockBackPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData);

		// Token: 0x06003557 RID: 13655
		public abstract float GetKnockDownPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData);

		// Token: 0x06003558 RID: 13656
		public abstract float GetHorseChargePenetration();
	}
}
