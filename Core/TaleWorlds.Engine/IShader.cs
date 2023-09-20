using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000017 RID: 23
	[ApplicationInterfaceBase]
	internal interface IShader
	{
		// Token: 0x060000F6 RID: 246
		[EngineMethod("get_from_resource", false)]
		Shader GetFromResource(string shaderName);

		// Token: 0x060000F7 RID: 247
		[EngineMethod("get_name", false)]
		string GetName(UIntPtr shaderPointer);

		// Token: 0x060000F8 RID: 248
		[EngineMethod("release", false)]
		void Release(UIntPtr shaderPointer);

		// Token: 0x060000F9 RID: 249
		[EngineMethod("get_material_shader_flag_mask", false)]
		ulong GetMaterialShaderFlagMask(UIntPtr shaderPointer, string flagName, bool showError);
	}
}
