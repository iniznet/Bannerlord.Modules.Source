using System;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000015 RID: 21
	[LibraryInterfaceBase]
	internal interface IManaged
	{
		// Token: 0x0600004A RID: 74
		[EngineMethod("increase_reference_count", false)]
		void IncreaseReferenceCount(UIntPtr ptr);

		// Token: 0x0600004B RID: 75
		[EngineMethod("decrease_reference_count", false)]
		void DecreaseReferenceCount(UIntPtr ptr);

		// Token: 0x0600004C RID: 76
		[EngineMethod("get_class_type_definition_count", false)]
		int GetClassTypeDefinitionCount();

		// Token: 0x0600004D RID: 77
		[EngineMethod("get_class_type_definition", false)]
		void GetClassTypeDefinition(int index, ref EngineClassTypeDefinition engineClassTypeDefinition);

		// Token: 0x0600004E RID: 78
		[EngineMethod("release_managed_object", false)]
		void ReleaseManagedObject(UIntPtr ptr);
	}
}
