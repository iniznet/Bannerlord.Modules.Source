using System;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	public class MultiplayerTrebuchetSpawner : TrebuchetSpawner
	{
		protected internal override void OnPreInit()
		{
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
		}
	}
}
