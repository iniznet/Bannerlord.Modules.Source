using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200035B RID: 859
	public interface IMoveableSiegeWeapon
	{
		// Token: 0x17000870 RID: 2160
		// (get) Token: 0x06002EDB RID: 11995
		SiegeWeaponMovementComponent MovementComponent { get; }

		// Token: 0x06002EDC RID: 11996
		void HighlightPath();

		// Token: 0x06002EDD RID: 11997
		void SwitchGhostEntityMovementMode(bool isGhostEnabled);

		// Token: 0x06002EDE RID: 11998
		MatrixFrame GetInitialFrame();
	}
}
