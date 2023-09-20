using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class RandomParticleSpawner : ScriptComponentBehavior
	{
		private void InitScript()
		{
			this._timeUntilNextParticleSpawn = this.spawnInterval;
		}

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

		protected internal override void OnInit()
		{
			base.OnInit();
			this.InitScript();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this.OnInit();
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

		protected internal override void OnTick(float dt)
		{
			this.CheckSpawnParticle(dt);
		}

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.CheckSpawnParticle(dt);
		}

		protected internal override bool MovesEntity()
		{
			return true;
		}

		private float _timeUntilNextParticleSpawn;

		public float spawnInterval = 3f;
	}
}
