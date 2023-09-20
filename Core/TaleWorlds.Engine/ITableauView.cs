using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200002E RID: 46
	[ApplicationInterfaceBase]
	internal interface ITableauView
	{
		// Token: 0x06000432 RID: 1074
		[EngineMethod("create_tableau_view", false)]
		TableauView CreateTableauView();

		// Token: 0x06000433 RID: 1075
		[EngineMethod("set_sort_meshes", false)]
		void SetSortingEnabled(UIntPtr pointer, bool value);

		// Token: 0x06000434 RID: 1076
		[EngineMethod("set_continous_rendering", false)]
		void SetContinousRendering(UIntPtr pointer, bool value);

		// Token: 0x06000435 RID: 1077
		[EngineMethod("set_do_not_render_this_frame", false)]
		void SetDoNotRenderThisFrame(UIntPtr pointer, bool value);

		// Token: 0x06000436 RID: 1078
		[EngineMethod("set_delete_after_rendering", false)]
		void SetDeleteAfterRendering(UIntPtr pointer, bool value);
	}
}
