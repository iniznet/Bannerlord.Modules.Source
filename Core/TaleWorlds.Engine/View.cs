using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	[EngineClass("rglView")]
	public abstract class View : NativeObject
	{
		internal View(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		public void SetScale(Vec2 scale)
		{
			EngineApplicationInterface.IView.SetScale(base.Pointer, scale.x, scale.y);
		}

		public void SetOffset(Vec2 offset)
		{
			EngineApplicationInterface.IView.SetOffset(base.Pointer, offset.x, offset.y);
		}

		public void SetRenderOrder(int value)
		{
			EngineApplicationInterface.IView.SetRenderOrder(base.Pointer, value);
		}

		public void SetRenderOption(View.ViewRenderOptions optionEnum, bool value)
		{
			EngineApplicationInterface.IView.SetRenderOption(base.Pointer, (int)optionEnum, value);
		}

		public void SetRenderTarget(Texture texture)
		{
			EngineApplicationInterface.IView.SetRenderTarget(base.Pointer, texture.Pointer);
		}

		public void SetDepthTarget(Texture texture)
		{
			EngineApplicationInterface.IView.SetDepthTarget(base.Pointer, texture.Pointer);
		}

		public void DontClearBackground()
		{
			this.SetRenderOption(View.ViewRenderOptions.ClearColor, false);
			this.SetRenderOption(View.ViewRenderOptions.ClearDepth, false);
		}

		public void SetClearColor(uint rgba)
		{
			EngineApplicationInterface.IView.SetClearColor(base.Pointer, rgba);
		}

		public void SetEnable(bool value)
		{
			EngineApplicationInterface.IView.SetEnable(base.Pointer, value);
		}

		public void SetRenderOnDemand(bool value)
		{
			EngineApplicationInterface.IView.SetRenderOnDemand(base.Pointer, value);
		}

		public void SetAutoDepthTargetCreation(bool value)
		{
			EngineApplicationInterface.IView.SetAutoDepthTargetCreation(base.Pointer, value);
		}

		public void SetSaveFinalResultToDisk(bool value)
		{
			EngineApplicationInterface.IView.SetSaveFinalResultToDisk(base.Pointer, value);
		}

		public void SetFileNameToSaveResult(string name)
		{
			EngineApplicationInterface.IView.SetFileNameToSaveResult(base.Pointer, name);
		}

		public void SetFileTypeToSave(View.TextureSaveFormat format)
		{
			EngineApplicationInterface.IView.SetFileTypeToSave(base.Pointer, (int)format);
		}

		public void SetFilePathToSaveResult(string name)
		{
			EngineApplicationInterface.IView.SetFilePathToSaveResult(base.Pointer, name);
		}

		public enum TextureSaveFormat
		{
			TextureTypeUnknown,
			TextureTypeBmp,
			TextureTypeJpg,
			TextureTypePng,
			TextureTypeDds,
			TextureTypeTif,
			TextureTypePsd,
			TextureTypeRaw
		}

		public enum PostfxConfig : uint
		{
			pfx_config_bloom = 1U,
			pfx_config_sunshafts,
			pfx_config_motionblur = 4U,
			pfx_config_dof = 8U,
			pfx_config_tsao = 16U,
			pfx_config_fxaa = 64U,
			pfx_config_smaa = 128U,
			pfx_config_temporal_smaa = 256U,
			pfx_config_temporal_resolve = 512U,
			pfx_config_temporal_filter = 1024U,
			pfx_config_contour = 2048U,
			pfx_config_ssr = 4096U,
			pfx_config_sssss = 8192U,
			pfx_config_streaks = 16384U,
			pfx_config_lens_flares = 32768U,
			pfx_config_chromatic_aberration = 65536U,
			pfx_config_vignette = 131072U,
			pfx_config_sharpen = 262144U,
			pfx_config_grain = 524288U,
			pfx_config_temporal_shadow = 1048576U,
			pfx_config_editor_scene = 2097152U,
			pfx_config_custom1 = 16777216U,
			pfx_config_custom2 = 33554432U,
			pfx_config_custom3 = 67108864U,
			pfx_config_custom4 = 134217728U,
			pfx_config_hexagon_vignette = 268435456U,
			pfx_config_screen_rt_injection = 536870912U,
			pfx_config_high_dof = 1073741824U,
			pfx_lower_bound = 1U,
			pfx_upper_bound = 536870912U
		}

		public enum ViewRenderOptions
		{
			ClearColor,
			ClearDepth
		}
	}
}
