using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000018 RID: 24
	[ApplicationInterfaceBase]
	internal interface ITexture
	{
		// Token: 0x060000FA RID: 250
		[EngineMethod("get_cur_object", false)]
		void GetCurObject(UIntPtr texturePointer, bool blocking);

		// Token: 0x060000FB RID: 251
		[EngineMethod("get_from_resource", false)]
		Texture GetFromResource(string textureName);

		// Token: 0x060000FC RID: 252
		[EngineMethod("load_texture_from_path", false)]
		Texture LoadTextureFromPath(string fileName, string folder);

		// Token: 0x060000FD RID: 253
		[EngineMethod("check_and_get_from_resource", false)]
		Texture CheckAndGetFromResource(string textureName);

		// Token: 0x060000FE RID: 254
		[EngineMethod("get_name", false)]
		string GetName(UIntPtr texturePointer);

		// Token: 0x060000FF RID: 255
		[EngineMethod("set_name", false)]
		void SetName(UIntPtr texturePointer, string name);

		// Token: 0x06000100 RID: 256
		[EngineMethod("get_width", false)]
		int GetWidth(UIntPtr texturePointer);

		// Token: 0x06000101 RID: 257
		[EngineMethod("get_height", false)]
		int GetHeight(UIntPtr texturePointer);

		// Token: 0x06000102 RID: 258
		[EngineMethod("get_memory_size", false)]
		int GetMemorySize(UIntPtr texturePointer);

		// Token: 0x06000103 RID: 259
		[EngineMethod("is_render_target", false)]
		bool IsRenderTarget(UIntPtr texturePointer);

		// Token: 0x06000104 RID: 260
		[EngineMethod("release_next_frame", false)]
		void ReleaseNextFrame(UIntPtr texturePointer);

		// Token: 0x06000105 RID: 261
		[EngineMethod("release", false)]
		void Release(UIntPtr texturePointer);

		// Token: 0x06000106 RID: 262
		[EngineMethod("create_render_target", false)]
		Texture CreateRenderTarget(string name, int width, int height, bool autoMipmaps, bool isTableau, bool createUninitialized, bool always_valid);

		// Token: 0x06000107 RID: 263
		[EngineMethod("create_depth_target", false)]
		Texture CreateDepthTarget(string name, int width, int height);

		// Token: 0x06000108 RID: 264
		[EngineMethod("create_from_byte_array", false)]
		Texture CreateFromByteArray(byte[] data, int width, int height);

		// Token: 0x06000109 RID: 265
		[EngineMethod("create_from_memory", false)]
		Texture CreateFromMemory(byte[] data);

		// Token: 0x0600010A RID: 266
		[EngineMethod("save_to_file", false)]
		void SaveToFile(UIntPtr texturePointer, string fileName);

		// Token: 0x0600010B RID: 267
		[EngineMethod("set_texture_as_always_valid", false)]
		void SaveTextureAsAlwaysValid(UIntPtr texturePointer);

		// Token: 0x0600010C RID: 268
		[EngineMethod("release_gpu_memories", false)]
		void ReleaseGpuMemories();

		// Token: 0x0600010D RID: 269
		[EngineMethod("transform_render_target_to_resource_texture", false)]
		void TransformRenderTargetToResourceTexture(UIntPtr texturePointer, string name);

		// Token: 0x0600010E RID: 270
		[EngineMethod("remove_continous_tableau_texture", false)]
		void RemoveContinousTableauTexture(UIntPtr texturePointer);

		// Token: 0x0600010F RID: 271
		[EngineMethod("set_tableau_view", false)]
		void SetTableauView(UIntPtr texturePointer, UIntPtr tableauView);

		// Token: 0x06000110 RID: 272
		[EngineMethod("create_texture_from_path", false)]
		Texture CreateTextureFromPath(PlatformFilePath filePath);

		// Token: 0x06000111 RID: 273
		[EngineMethod("get_render_target_component", false)]
		RenderTargetComponent GetRenderTargetComponent(UIntPtr texturePointer);

		// Token: 0x06000112 RID: 274
		[EngineMethod("get_tableau_view", false)]
		TableauView GetTableauView(UIntPtr texturePointer);

		// Token: 0x06000113 RID: 275
		[EngineMethod("is_loaded", false)]
		bool IsLoaded(UIntPtr texturePointer);
	}
}
