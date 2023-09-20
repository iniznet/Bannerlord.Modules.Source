using System;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	public class MultiplayerFireTrebuchetSpawner : TrebuchetSpawner
	{
		protected internal override void OnPreInit()
		{
			this._spawnerMissionHelperFire = new SpawnerEntityMissionHelper(this, true);
		}
	}
}
