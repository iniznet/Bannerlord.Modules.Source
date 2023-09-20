using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001E4 RID: 484
	public class DefaultDamageParticleModel : DamageParticleModel
	{
		// Token: 0x06001B4F RID: 6991 RVA: 0x000601B8 File Offset: 0x0005E3B8
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

		// Token: 0x06001B50 RID: 6992 RVA: 0x0006026C File Offset: 0x0005E46C
		public override void GetMeleeAttackBloodParticles(Agent attacker, Agent victim, in Blow blow, in AttackCollisionData collisionData, out HitParticleResultData particleResultData)
		{
			particleResultData.StartHitParticleIndex = this._bloodStartHitParticleIndex;
			particleResultData.ContinueHitParticleIndex = this._bloodContinueHitParticleIndex;
			particleResultData.EndHitParticleIndex = this._bloodEndHitParticleIndex;
		}

		// Token: 0x06001B51 RID: 6993 RVA: 0x00060295 File Offset: 0x0005E495
		public override void GetMeleeAttackSweatParticles(Agent attacker, Agent victim, in Blow blow, in AttackCollisionData collisionData, out HitParticleResultData particleResultData)
		{
			particleResultData.StartHitParticleIndex = this._sweatStartHitParticleIndex;
			particleResultData.ContinueHitParticleIndex = this._sweatContinueHitParticleIndex;
			particleResultData.EndHitParticleIndex = this._sweatEndHitParticleIndex;
		}

		// Token: 0x06001B52 RID: 6994 RVA: 0x000602BE File Offset: 0x0005E4BE
		public override int GetMissileAttackParticle(Agent attacker, Agent victim, in Blow blow, in AttackCollisionData collisionData)
		{
			return this._missileHitParticleIndex;
		}

		// Token: 0x040008DA RID: 2266
		private int _bloodStartHitParticleIndex = -1;

		// Token: 0x040008DB RID: 2267
		private int _bloodContinueHitParticleIndex = -1;

		// Token: 0x040008DC RID: 2268
		private int _bloodEndHitParticleIndex = -1;

		// Token: 0x040008DD RID: 2269
		private int _sweatStartHitParticleIndex = -1;

		// Token: 0x040008DE RID: 2270
		private int _sweatContinueHitParticleIndex = -1;

		// Token: 0x040008DF RID: 2271
		private int _sweatEndHitParticleIndex = -1;

		// Token: 0x040008E0 RID: 2272
		private int _missileHitParticleIndex = -1;
	}
}
