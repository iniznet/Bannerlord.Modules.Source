using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x02000056 RID: 86
	[EngineClass("rglManaged_script_component")]
	public sealed class ManagedScriptComponent : ScriptComponent
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000734 RID: 1844 RVA: 0x00006085 File Offset: 0x00004285
		public ScriptComponentBehavior ScriptComponentBehavior
		{
			get
			{
				return EngineApplicationInterface.IScriptComponent.GetScriptComponentBehavior(base.Pointer);
			}
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x00006097 File Offset: 0x00004297
		public void SetVariableEditorWidgetStatus(string field, bool enabled)
		{
			EngineApplicationInterface.IScriptComponent.SetVariableEditorWidgetStatus(base.Pointer, field, enabled);
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x000060AB File Offset: 0x000042AB
		private ManagedScriptComponent()
		{
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x000060B3 File Offset: 0x000042B3
		internal ManagedScriptComponent(UIntPtr pointer)
			: base(pointer)
		{
		}
	}
}
