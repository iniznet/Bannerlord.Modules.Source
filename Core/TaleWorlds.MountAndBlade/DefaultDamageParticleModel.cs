using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class DefaultDamageParticleModel : DamageParticleModel
	{
		public DefaultDamageParticleModel()
		{
			this._bloodStartHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_blood_sword_enter");
			this._bloodContinueHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_blood_sword_inside");
			this._bloodEndHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_blood_sword_exit");
			this._sweatStartHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_sweat_sword_enter");
			this._sweatContinueHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_sweat_sword_enter");
			this._sweatEndHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_sweat_sword_enter");
			this._missileHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_blood_sword_enter");
		}

		public override void GetMeleeAttackBloodParticles(Agent attacker, Agent victim, in Blow blow, in AttackCollisionData collisionData, out HitParticleResultData particleResultData)
		{
			particleResultData.StartHitParticleIndex = this._bloodStartHitParticleIndex;
			particleResultData.ContinueHitParticleIndex = this._bloodContinueHitParticleIndex;
			particleResultData.EndHitParticleIndex = this._bloodEndHitParticleIndex;
		}

		public override void GetMeleeAttackSweatParticles(Agent attacker, Agent victim, in Blow blow, in AttackCollisionData collisionData, out HitParticleResultData particleResultData)
		{
			particleResultData.StartHitParticleIndex = this._sweatStartHitParticleIndex;
			particleResultData.ContinueHitParticleIndex = this._sweatContinueHitParticleIndex;
			particleResultData.EndHitParticleIndex = this._sweatEndHitParticleIndex;
		}

		public override int GetMissileAttackParticle(Agent attacker, Agent victim, in Blow blow, in AttackCollisionData collisionData)
		{
			return this._missileHitParticleIndex;
		}

		private int _bloodStartHitParticleIndex = -1;

		private int _bloodContinueHitParticleIndex = -1;

		private int _bloodEndHitParticleIndex = -1;

		private int _sweatStartHitParticleIndex = -1;

		private int _sweatContinueHitParticleIndex = -1;

		private int _sweatEndHitParticleIndex = -1;

		private int _missileHitParticleIndex = -1;
	}
}
