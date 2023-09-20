﻿using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public abstract class DamageParticleModel : GameModel
	{
		public abstract void GetMeleeAttackBloodParticles(Agent attacker, Agent victim, in Blow blow, in AttackCollisionData collisionData, out HitParticleResultData particleResultData);

		public abstract void GetMeleeAttackSweatParticles(Agent attacker, Agent victim, in Blow blow, in AttackCollisionData collisionData, out HitParticleResultData particleResultData);

		public abstract int GetMissileAttackParticle(Agent attacker, Agent victim, in Blow blow, in AttackCollisionData collisionData);
	}
}
