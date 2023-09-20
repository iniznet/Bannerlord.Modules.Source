using System;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	// Token: 0x020003AF RID: 943
	public class MultiplayerTrebuchetSpawner : TrebuchetSpawner
	{
		// Token: 0x060032E6 RID: 13030 RVA: 0x000D2859 File Offset: 0x000D0A59
		protected internal override void OnPreInit()
		{
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
		}
	}
}
