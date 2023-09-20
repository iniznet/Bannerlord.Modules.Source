using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000342 RID: 834
	public class RandomParticleSpawner : ScriptComponentBehavior
	{
		// Token: 0x06002C88 RID: 11400 RVA: 0x000ACD32 File Offset: 0x000AAF32
		private void InitScript()
		{
			this._timeUntilNextParticleSpawn = this.spawnInterval;
		}

		// Token: 0x06002C89 RID: 11401 RVA: 0x000ACD40 File Offset: 0x000AAF40
		private void CheckSpawnParticle(float dt)
		{
			this._timeUntilNextParticleSpawn -= dt;
			if (this._timeUntilNextParticleSpawn <= 0f)
			{
				int childCount = base.GameEntity.ChildCount;
				if (childCount > 0)
				{
					int num = MBRandom.RandomInt(childCount);
					GameEntity child = base.GameEntity.GetChild(num);
					int componentCount = child.GetComponentCount(GameEntity.ComponentType.ParticleSystemInstanced);
					for (int i = 0; i < componentCount; i++)
					{
						((ParticleSystem)child.GetComponentAtIndex(i, GameEntity.ComponentType.ParticleSystemInstanced)).Restart();
					}
				}
				this._timeUntilNextParticleSpawn += this.spawnInterval;
			}
		}

		// Token: 0x06002C8A RID: 11402 RVA: 0x000ACDCA File Offset: 0x000AAFCA
		protected internal override void OnInit()
		{
			base.OnInit();
			this.InitScript();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06002C8B RID: 11403 RVA: 0x000ACDE4 File Offset: 0x000AAFE4
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this.OnInit();
		}

		// Token: 0x06002C8C RID: 11404 RVA: 0x000ACDF2 File Offset: 0x000AAFF2
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

		// Token: 0x06002C8D RID: 11405 RVA: 0x000ACDFC File Offset: 0x000AAFFC
		protected internal override void OnTick(float dt)
		{
			this.CheckSpawnParticle(dt);
		}

		// Token: 0x06002C8E RID: 11406 RVA: 0x000ACE05 File Offset: 0x000AB005
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.CheckSpawnParticle(dt);
		}

		// Token: 0x06002C8F RID: 11407 RVA: 0x000ACE15 File Offset: 0x000AB015
		protected internal override bool MovesEntity()
		{
			return true;
		}

		// Token: 0x040010FE RID: 4350
		private float _timeUntilNextParticleSpawn;

		// Token: 0x040010FF RID: 4351
		public float spawnInterval = 3f;
	}
}
