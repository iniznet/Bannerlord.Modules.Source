using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001A5 RID: 421
	[ScriptingInterfaceBase]
	internal interface IMBDebugExtensions
	{
		// Token: 0x0600172A RID: 5930
		[EngineMethod("render_debug_circle_on_terrain", false)]
		void RenderDebugCircleOnTerrain(UIntPtr scenePointer, ref MatrixFrame frame, float radius, uint color, bool depthCheck, bool isDotted);

		// Token: 0x0600172B RID: 5931
		[EngineMethod("render_debug_arc_on_terrain", false)]
		void RenderDebugArcOnTerrain(UIntPtr scenePointer, ref MatrixFrame frame, float radius, float beginAngle, float endAngle, uint color, bool depthCheck, bool isDotted);

		// Token: 0x0600172C RID: 5932
		[EngineMethod("render_debug_line_on_terrain", false)]
		void RenderDebugLineOnTerrain(UIntPtr scenePointer, Vec3 position, Vec3 direction, uint color, bool depthCheck, float time, bool isDotted, float pointDensity);

		// Token: 0x0600172D RID: 5933
		[EngineMethod("override_native_parameter", false)]
		void OverrideNativeParameter(string paramName, float value);

		// Token: 0x0600172E RID: 5934
		[EngineMethod("reload_native_parameters", false)]
		void ReloadNativeParameters();
	}
}
