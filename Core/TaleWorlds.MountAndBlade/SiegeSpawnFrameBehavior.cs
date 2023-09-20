using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
	public class SiegeSpawnFrameBehavior : SpawnFrameBehaviorBase
	{
		public override void Initialize()
		{
			base.Initialize();
			this._spawnPointsByTeam = new List<GameEntity>[2];
			this._spawnZonesByTeam = new List<GameEntity>[2];
			this._spawnPointsByTeam[1] = this.SpawnPoints.Where((GameEntity x) => x.HasTag("attacker")).ToList<GameEntity>();
			this._spawnPointsByTeam[0] = this.SpawnPoints.Where((GameEntity x) => x.HasTag("defender")).ToList<GameEntity>();
			this._spawnZonesByTeam[1] = (from sz in this._spawnPointsByTeam[1].Select((GameEntity sp) => sp.Parent).Distinct<GameEntity>()
				where sz != null
				select sz).ToList<GameEntity>();
			this._spawnZonesByTeam[0] = (from sz in this._spawnPointsByTeam[0].Select((GameEntity sp) => sp.Parent).Distinct<GameEntity>()
				where sz != null
				select sz).ToList<GameEntity>();
			this._activeSpawnZoneIndex = 0;
		}

		public override MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn)
		{
			List<GameEntity> list = new List<GameEntity>();
			GameEntity gameEntity = this._spawnZonesByTeam[(int)team.Side].First((GameEntity sz) => sz.HasTag(string.Format("{0}{1}", "sp_zone_", this._activeSpawnZoneIndex)));
			list.AddRange(from sp in gameEntity.GetChildren()
				where sp.HasTag("spawnpoint")
				select sp);
			return base.GetSpawnFrameFromSpawnPoints(list, team, hasMount);
		}

		public void OnFlagDeactivated(FlagCapturePoint flag)
		{
			this._activeSpawnZoneIndex++;
		}

		public const string SpawnZoneTagAffix = "sp_zone_";

		public const string SpawnZoneEnableTagAffix = "enable_";

		public const string SpawnZoneDisableTagAffix = "disable_";

		public const int StartingActiveSpawnZoneIndex = 0;

		private List<GameEntity>[] _spawnPointsByTeam;

		private List<GameEntity>[] _spawnZonesByTeam;

		private int _activeSpawnZoneIndex;
	}
}
