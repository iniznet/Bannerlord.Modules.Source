using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000028 RID: 40
	[ApplicationInterfaceBase]
	internal interface IScriptComponent
	{
		// Token: 0x060003E9 RID: 1001
		[EngineMethod("get_script_component_behavior", false)]
		ScriptComponentBehavior GetScriptComponentBehavior(UIntPtr pointer);

		// Token: 0x060003EA RID: 1002
		[EngineMethod("set_variable_editor_widget_status", false)]
		void SetVariableEditorWidgetStatus(UIntPtr pointer, string field, bool enabled);
	}
}
