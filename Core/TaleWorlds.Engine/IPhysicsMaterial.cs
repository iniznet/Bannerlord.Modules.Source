using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200002B RID: 43
	[ApplicationInterfaceBase]
	internal interface IPhysicsMaterial
	{
		// Token: 0x060003FE RID: 1022
		[EngineMethod("get_index_with_name", false)]
		PhysicsMaterial GetIndexWithName(string materialName);

		// Token: 0x060003FF RID: 1023
		[EngineMethod("get_material_count", false)]
		int GetMaterialCount();

		// Token: 0x06000400 RID: 1024
		[EngineMethod("get_material_name_at_index", false)]
		string GetMaterialNameAtIndex(int index);

		// Token: 0x06000401 RID: 1025
		[EngineMethod("get_material_flags_at_index", false)]
		PhysicsMaterialFlags GetFlagsAtIndex(int index);

		// Token: 0x06000402 RID: 1026
		[EngineMethod("get_restitution_at_index", false)]
		float GetRestitutionAtIndex(int index);

		// Token: 0x06000403 RID: 1027
		[EngineMethod("get_dynamic_friction_at_index", false)]
		float GetDynamicFrictionAtIndex(int index);

		// Token: 0x06000404 RID: 1028
		[EngineMethod("get_static_friction_at_index", false)]
		float GetStaticFrictionAtIndex(int index);

		// Token: 0x06000405 RID: 1029
		[EngineMethod("get_softness_at_index", false)]
		float GetSoftnessAtIndex(int index);
	}
}
