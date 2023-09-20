using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000019 RID: 25
	[ApplicationInterfaceBase]
	internal interface IMaterial
	{
		// Token: 0x06000114 RID: 276
		[EngineMethod("create_copy", false)]
		Material CreateCopy(UIntPtr materialPointer);

		// Token: 0x06000115 RID: 277
		[EngineMethod("get_from_resource", false)]
		Material GetFromResource(string materialName);

		// Token: 0x06000116 RID: 278
		[EngineMethod("get_default_material", false)]
		Material GetDefaultMaterial();

		// Token: 0x06000117 RID: 279
		[EngineMethod("get_outline_material", false)]
		Material GetOutlineMaterial(UIntPtr materialPointer);

		// Token: 0x06000118 RID: 280
		[EngineMethod("get_name", false)]
		string GetName(UIntPtr materialPointer);

		// Token: 0x06000119 RID: 281
		[EngineMethod("set_name", false)]
		void SetName(UIntPtr materialPointer, string name);

		// Token: 0x0600011A RID: 282
		[EngineMethod("get_alpha_blend_mode", false)]
		int GetAlphaBlendMode(UIntPtr materialPointer);

		// Token: 0x0600011B RID: 283
		[EngineMethod("set_alpha_blend_mode", false)]
		void SetAlphaBlendMode(UIntPtr materialPointer, int alphaBlendMode);

		// Token: 0x0600011C RID: 284
		[EngineMethod("release", false)]
		void Release(UIntPtr materialPointer);

		// Token: 0x0600011D RID: 285
		[EngineMethod("set_shader", false)]
		void SetShader(UIntPtr materialPointer, UIntPtr shaderPointer);

		// Token: 0x0600011E RID: 286
		[EngineMethod("get_shader", false)]
		Shader GetShader(UIntPtr materialPointer);

		// Token: 0x0600011F RID: 287
		[EngineMethod("get_shader_flags", false)]
		ulong GetShaderFlags(UIntPtr materialPointer);

		// Token: 0x06000120 RID: 288
		[EngineMethod("set_shader_flags", false)]
		void SetShaderFlags(UIntPtr materialPointer, ulong shaderFlags);

		// Token: 0x06000121 RID: 289
		[EngineMethod("set_mesh_vector_argument", false)]
		void SetMeshVectorArgument(UIntPtr materialPointer, float x, float y, float z, float w);

		// Token: 0x06000122 RID: 290
		[EngineMethod("set_texture", false)]
		void SetTexture(UIntPtr materialPointer, int textureType, UIntPtr texturePointer);

		// Token: 0x06000123 RID: 291
		[EngineMethod("set_texture_at_slot", false)]
		void SetTextureAtSlot(UIntPtr materialPointer, int textureSlotIndex, UIntPtr texturePointer);

		// Token: 0x06000124 RID: 292
		[EngineMethod("get_texture", false)]
		Texture GetTexture(UIntPtr materialPointer, int textureType);

		// Token: 0x06000125 RID: 293
		[EngineMethod("set_alpha_test_value", false)]
		void SetAlphaTestValue(UIntPtr materialPointer, float alphaTestValue);

		// Token: 0x06000126 RID: 294
		[EngineMethod("get_alpha_test_value", false)]
		float GetAlphaTestValue(UIntPtr materialPointer);

		// Token: 0x06000127 RID: 295
		[EngineMethod("get_flags", false)]
		uint GetFlags(UIntPtr materialPointer);

		// Token: 0x06000128 RID: 296
		[EngineMethod("set_flags", false)]
		void SetFlags(UIntPtr materialPointer, uint flags);

		// Token: 0x06000129 RID: 297
		[EngineMethod("add_material_shader_flag", false)]
		void AddMaterialShaderFlag(UIntPtr materialPointer, string flagName, bool showErrors);

		// Token: 0x0600012A RID: 298
		[EngineMethod("set_area_map_scale", false)]
		void SetAreaMapScale(UIntPtr materialPointer, float scale);

		// Token: 0x0600012B RID: 299
		[EngineMethod("set_enable_skinning", false)]
		void SetEnableSkinning(UIntPtr materialPointer, bool enable);

		// Token: 0x0600012C RID: 300
		[EngineMethod("using_skinning", false)]
		bool UsingSkinning(UIntPtr materialPointer);
	}
}
