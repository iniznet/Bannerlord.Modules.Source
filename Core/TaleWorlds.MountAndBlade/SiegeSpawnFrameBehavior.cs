using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002C6 RID: 710
	public class SiegeSpawnFrameBehavior : SpawnFrameBehaviorBase
	{
		// Token: 0x060026FF RID: 9983 RVA: 0x0009398C File Offset: 0x00091B8C
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

		// Token: 0x06002700 RID: 9984 RVA: 0x00093AF0 File Offset: 0x00091CF0
		public override MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn)
		{
			List<GameEntity> list = new List<GameEntity>();
			GameEntity gameEntity = this._spawnZonesByTeam[(int)team.Side].First((GameEntity sz) => sz.HasTag(string.Format("{0}{1}", "sp_zone_", this._activeSpawnZoneIndex)));
			list.AddRange(from sp in gameEntity.GetChildren()
				where sp.HasTag("spawnpoint")
				select sp);
			return base.GetSpawnFrameFromSpawnPoints(list, team, hasMount);
		}

		// Token: 0x06002701 RID: 9985 RVA: 0x00093B5B File Offset: 0x00091D5B
		public void OnFlagDeactivated(FlagCapturePoint flag)
		{
			this._activeSpawnZoneIndex++;
		}

		// Token: 0x04000E74 RID: 3700
		public const string SpawnZoneTagAffix = "sp_zone_";

		// Token: 0x04000E75 RID: 3701
		public const string SpawnZoneEnableTagAffix = "enable_";

		// Token: 0x04000E76 RID: 3702
		public const string SpawnZoneDisableTagAffix = "disable_";

		// Token: 0x04000E77 RID: 3703
		public const int StartingActiveSpawnZoneIndex = 0;

		// Token: 0x04000E78 RID: 3704
		private List<GameEntity>[] _spawnPointsByTeam;

		// Token: 0x04000E79 RID: 3705
		private List<GameEntity>[] _spawnZonesByTeam;

		// Token: 0x04000E7A RID: 3706
		private int _activeSpawnZoneIndex;
	}
}
