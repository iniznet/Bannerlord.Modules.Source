using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200002D RID: 45
	[ApplicationInterfaceBase]
	internal interface IView
	{
		// Token: 0x06000423 RID: 1059
		[EngineMethod("set_render_option", false)]
		void SetRenderOption(UIntPtr ptr, int optionEnum, bool value);

		// Token: 0x06000424 RID: 1060
		[EngineMethod("set_render_order", false)]
		void SetRenderOrder(UIntPtr ptr, int value);

		// Token: 0x06000425 RID: 1061
		[EngineMethod("set_render_target", false)]
		void SetRenderTarget(UIntPtr ptr, UIntPtr texture_ptr);

		// Token: 0x06000426 RID: 1062
		[EngineMethod("set_depth_target", false)]
		void SetDepthTarget(UIntPtr ptr, UIntPtr texture_ptr);

		// Token: 0x06000427 RID: 1063
		[EngineMethod("set_scale", false)]
		void SetScale(UIntPtr ptr, float x, float y);

		// Token: 0x06000428 RID: 1064
		[EngineMethod("set_offset", false)]
		void SetOffset(UIntPtr ptr, float x, float y);

		// Token: 0x06000429 RID: 1065
		[EngineMethod("set_debug_render_functionality", false)]
		void SetDebugRenderFunctionality(UIntPtr ptr, bool value);

		// Token: 0x0600042A RID: 1066
		[EngineMethod("set_clear_color", false)]
		void SetClearColor(UIntPtr ptr, uint rgba);

		// Token: 0x0600042B RID: 1067
		[EngineMethod("set_enable", false)]
		void SetEnable(UIntPtr ptr, bool value);

		// Token: 0x0600042C RID: 1068
		[EngineMethod("set_render_on_demand", false)]
		void SetRenderOnDemand(UIntPtr ptr, bool value);

		// Token: 0x0600042D RID: 1069
		[EngineMethod("set_auto_depth_creation", false)]
		void SetAutoDepthTargetCreation(UIntPtr ptr, bool value);

		// Token: 0x0600042E RID: 1070
		[EngineMethod("set_save_final_result_to_disk", false)]
		void SetSaveFinalResultToDisk(UIntPtr ptr, bool value);

		// Token: 0x0600042F RID: 1071
		[EngineMethod("set_file_name_to_save_result", false)]
		void SetFileNameToSaveResult(UIntPtr ptr, string name);

		// Token: 0x06000430 RID: 1072
		[EngineMethod("set_file_type_to_save", false)]
		void SetFileTypeToSave(UIntPtr ptr, int type);

		// Token: 0x06000431 RID: 1073
		[EngineMethod("set_file_path_to_save_result", false)]
		void SetFilePathToSaveResult(UIntPtr ptr, string name);
	}
}
