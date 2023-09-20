using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200001D RID: 29
	[ApplicationInterfaceBase]
	internal interface IClothSimulatorComponent
	{
		// Token: 0x06000185 RID: 389
		[EngineMethod("set_maxdistance_multiplier", false)]
		void SetMaxDistanceMultiplier(UIntPtr cloth_pointer, float multiplier);
	}
}
