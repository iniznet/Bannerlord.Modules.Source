using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001E2 RID: 482
	public abstract class DamageParticleModel : GameModel
	{
		// Token: 0x06001B49 RID: 6985
		public abstract void GetMeleeAttackBloodParticles(Agent attacker, Agent victim, in Blow blow, in AttackCollisionData collisionData, out HitParticleResultData particleResultData);

		// Token: 0x06001B4A RID: 6986
		public abstract void GetMeleeAttackSweatParticles(Agent attacker, Agent victim, in Blow blow, in AttackCollisionData collisionData, out HitParticleResultData particleResultData);

		// Token: 0x06001B4B RID: 6987
		public abstract int GetMissileAttackParticle(Agent attacker, Agent victim, in Blow blow, in AttackCollisionData collisionData);
	}
}
