using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x0200000D RID: 13
	[EngineClass("rglCloth_simulator_component")]
	public sealed class ClothSimulatorComponent : GameEntityComponent
	{
		// Token: 0x0600004C RID: 76 RVA: 0x00002991 File Offset: 0x00000B91
		internal ClothSimulatorComponent(UIntPtr pointer)
			: base(pointer)
		{
		}

		// Token: 0x0600004D RID: 77 RVA: 0x0000299A File Offset: 0x00000B9A
		public void SetMaxDistanceMultiplier(float multiplier)
		{
			EngineApplicationInterface.IClothSimulatorComponent.SetMaxDistanceMultiplier(base.Pointer, multiplier);
		}
	}
}
