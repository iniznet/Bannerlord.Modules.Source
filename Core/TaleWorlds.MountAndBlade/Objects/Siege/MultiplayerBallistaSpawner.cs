using System;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	public class MultiplayerBallistaSpawner : BallistaSpawner
	{
		protected internal override void OnPreInit()
		{
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
		}
	}
}
