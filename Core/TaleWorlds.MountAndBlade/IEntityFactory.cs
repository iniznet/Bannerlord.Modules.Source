using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200033C RID: 828
	public interface IEntityFactory
	{
		// Token: 0x06002C63 RID: 11363
		GameEntity MakeEntity(params object[] paramObjects);
	}
}
