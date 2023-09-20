using System;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	public class MultiplayerFireBallistaSpawner : BallistaSpawner
	{
		protected internal override void OnPreInit()
		{
			this._spawnerMissionHelperFire = new SpawnerEntityMissionHelper(this, true);
		}
	}
}
