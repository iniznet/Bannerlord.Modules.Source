using System;
using TaleWorlds.SaveSystem.Load;

namespace TaleWorlds.SaveSystem.Resolvers
{
	// Token: 0x02000031 RID: 49
	public interface IObjectResolver
	{
		// Token: 0x060001C1 RID: 449
		bool CheckIfRequiresAdvancedResolving(object originalObject);

		// Token: 0x060001C2 RID: 450
		object ResolveObject(object originalObject);

		// Token: 0x060001C3 RID: 451
		object AdvancedResolveObject(object originalObject, MetaData metaData, ObjectLoadData objectLoadData);
	}
}
