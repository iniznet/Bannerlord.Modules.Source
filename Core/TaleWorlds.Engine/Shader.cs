using System;

namespace TaleWorlds.Engine
{
	// Token: 0x02000083 RID: 131
	public sealed class Shader : Resource
	{
		// Token: 0x06000A07 RID: 2567 RVA: 0x0000AE70 File Offset: 0x00009070
		internal Shader(UIntPtr ptr)
			: base(ptr)
		{
		}

		// Token: 0x06000A08 RID: 2568 RVA: 0x0000AE79 File Offset: 0x00009079
		public static Shader GetFromResource(string shaderName)
		{
			return EngineApplicationInterface.IShader.GetFromResource(shaderName);
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000A09 RID: 2569 RVA: 0x0000AE86 File Offset: 0x00009086
		public string Name
		{
			get
			{
				return EngineApplicationInterface.IShader.GetName(base.Pointer);
			}
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x0000AE98 File Offset: 0x00009098
		public ulong GetMaterialShaderFlagMask(string flagName, bool showErrors = true)
		{
			return EngineApplicationInterface.IShader.GetMaterialShaderFlagMask(base.Pointer, flagName, showErrors);
		}
	}
}
