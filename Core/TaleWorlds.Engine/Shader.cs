using System;

namespace TaleWorlds.Engine
{
	public sealed class Shader : Resource
	{
		internal Shader(UIntPtr ptr)
			: base(ptr)
		{
		}

		public static Shader GetFromResource(string shaderName)
		{
			return EngineApplicationInterface.IShader.GetFromResource(shaderName);
		}

		public string Name
		{
			get
			{
				return EngineApplicationInterface.IShader.GetName(base.Pointer);
			}
		}

		public ulong GetMaterialShaderFlagMask(string flagName, bool showErrors = true)
		{
			return EngineApplicationInterface.IShader.GetMaterialShaderFlagMask(base.Pointer, flagName, showErrors);
		}
	}
}
