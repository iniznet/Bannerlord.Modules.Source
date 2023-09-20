using System;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	// Token: 0x020003AC RID: 940
	public class MultiplayerFireTrebuchetSpawner : TrebuchetSpawner
	{
		// Token: 0x060032E0 RID: 13024 RVA: 0x000D27E5 File Offset: 0x000D09E5
		protected internal override void OnPreInit()
		{
			this._spawnerMissionHelperFire = new SpawnerEntityMissionHelper(this, true);
		}
	}
}
