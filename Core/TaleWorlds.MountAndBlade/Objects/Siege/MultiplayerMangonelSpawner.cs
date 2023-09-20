using System;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	public class MultiplayerMangonelSpawner : MangonelSpawner
	{
		protected internal override void OnPreInit()
		{
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
		}
	}
}
