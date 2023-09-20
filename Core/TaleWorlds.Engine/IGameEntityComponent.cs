using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000025 RID: 37
	[ApplicationInterfaceBase]
	internal interface IGameEntityComponent
	{
		// Token: 0x06000248 RID: 584
		[EngineMethod("get_entity", false)]
		GameEntity GetEntity(GameEntityComponent entityComponent);

		// Token: 0x06000249 RID: 585
		[EngineMethod("get_first_meta_mesh", false)]
		MetaMesh GetFirstMetaMesh(GameEntityComponent entityComponent);
	}
}
