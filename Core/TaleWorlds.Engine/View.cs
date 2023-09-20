using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000094 RID: 148
	[EngineClass("rglView")]
	public abstract class View : NativeObject
	{
		// Token: 0x06000B5B RID: 2907 RVA: 0x0000C845 File Offset: 0x0000AA45
		internal View(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		// Token: 0x06000B5C RID: 2908 RVA: 0x0000C854 File Offset: 0x0000AA54
		public void SetScale(Vec2 scale)
		{
			EngineApplicationInterface.IView.SetScale(base.Pointer, scale.x, scale.y);
		}

		// Token: 0x06000B5D RID: 2909 RVA: 0x0000C872 File Offset: 0x0000AA72
		public void SetOffset(Vec2 offset)
		{
			EngineApplicationInterface.IView.SetOffset(base.Pointer, offset.x, offset.y);
		}

		// Token: 0x06000B5E RID: 2910 RVA: 0x0000C890 File Offset: 0x0000AA90
		public void SetRenderOrder(int value)
		{
			EngineApplicationInterface.IView.SetRenderOrder(base.Pointer, value);
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x0000C8A3 File Offset: 0x0000AAA3
		public void SetRenderOption(View.ViewRenderOptions optionEnum, bool value)
		{
			EngineApplicationInterface.IView.SetRenderOption(base.Pointer, (int)optionEnum, value);
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x0000C8B7 File Offset: 0x0000AAB7
		public void SetRenderTarget(Texture texture)
		{
			EngineApplicationInterface.IView.SetRenderTarget(base.Pointer, texture.Pointer);
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x0000C8CF File Offset: 0x0000AACF
		public void SetDepthTarget(Texture texture)
		{
			EngineApplicationInterface.IView.SetDepthTarget(base.Pointer, texture.Pointer);
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x0000C8E7 File Offset: 0x0000AAE7
		public void DontClearBackground()
		{
			this.SetRenderOption(View.ViewRenderOptions.ClearColor, false);
			this.SetRenderOption(View.ViewRenderOptions.ClearDepth, false);
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x0000C8F9 File Offset: 0x0000AAF9
		public void SetClearColor(uint rgba)
		{
			EngineApplicationInterface.IView.SetClearColor(base.Pointer, rgba);
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x0000C90C File Offset: 0x0000AB0C
		public void SetEnable(bool value)
		{
			EngineApplicationInterface.IView.SetEnable(base.Pointer, value);
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x0000C91F File Offset: 0x0000AB1F
		public void SetRenderOnDemand(bool value)
		{
			EngineApplicationInterface.IView.SetRenderOnDemand(base.Pointer, value);
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x0000C932 File Offset: 0x0000AB32
		public void SetAutoDepthTargetCreation(bool value)
		{
			EngineApplicationInterface.IView.SetAutoDepthTargetCreation(base.Pointer, value);
		}

		// Token: 0x06000B67 RID: 2919 RVA: 0x0000C945 File Offset: 0x0000AB45
		public void SetSaveFinalResultToDisk(bool value)
		{
			EngineApplicationInterface.IView.SetSaveFinalResultToDisk(base.Pointer, value);
		}

		// Token: 0x06000B68 RID: 2920 RVA: 0x0000C958 File Offset: 0x0000AB58
		public void SetFileNameToSaveResult(string name)
		{
			EngineApplicationInterface.IView.SetFileNameToSaveResult(base.Pointer, name);
		}

		// Token: 0x06000B69 RID: 2921 RVA: 0x0000C96B File Offset: 0x0000AB6B
		public void SetFileTypeToSave(View.TextureSaveFormat format)
		{
			EngineApplicationInterface.IView.SetFileTypeToSave(base.Pointer, (int)format);
		}

		// Token: 0x06000B6A RID: 2922 RVA: 0x0000C97E File Offset: 0x0000AB7E
		public void SetFilePathToSaveResult(string name)
		{
			EngineApplicationInterface.IView.SetFilePathToSaveResult(base.Pointer, name);
		}

		// Token: 0x020000C9 RID: 201
		public enum TextureSaveFormat
		{
			// Token: 0x04000432 RID: 1074
			TextureTypeUnknown,
			// Token: 0x04000433 RID: 1075
			TextureTypeBmp,
			// Token: 0x04000434 RID: 1076
			TextureTypeJpg,
			// Token: 0x04000435 RID: 1077
			TextureTypePng,
			// Token: 0x04000436 RID: 1078
			TextureTypeDds,
			// Token: 0x04000437 RID: 1079
			TextureTypeTif,
			// Token: 0x04000438 RID: 1080
			TextureTypePsd,
			// Token: 0x04000439 RID: 1081
			TextureTypeRaw
		}

		// Token: 0x020000CA RID: 202
		public enum PostfxConfig : uint
		{
			// Token: 0x0400043B RID: 1083
			pfx_config_bloom = 1U,
			// Token: 0x0400043C RID: 1084
			pfx_config_sunshafts,
			// Token: 0x0400043D RID: 1085
			pfx_config_motionblur = 4U,
			// Token: 0x0400043E RID: 1086
			pfx_config_dof = 8U,
			// Token: 0x0400043F RID: 1087
			pfx_config_tsao = 16U,
			// Token: 0x04000440 RID: 1088
			pfx_config_fxaa = 64U,
			// Token: 0x04000441 RID: 1089
			pfx_config_smaa = 128U,
			// Token: 0x04000442 RID: 1090
			pfx_config_temporal_smaa = 256U,
			// Token: 0x04000443 RID: 1091
			pfx_config_temporal_resolve = 512U,
			// Token: 0x04000444 RID: 1092
			pfx_config_temporal_filter = 1024U,
			// Token: 0x04000445 RID: 1093
			pfx_config_contour = 2048U,
			// Token: 0x04000446 RID: 1094
			pfx_config_ssr = 4096U,
			// Token: 0x04000447 RID: 1095
			pfx_config_sssss = 8192U,
			// Token: 0x04000448 RID: 1096
			pfx_config_streaks = 16384U,
			// Token: 0x04000449 RID: 1097
			pfx_config_lens_flares = 32768U,
			// Token: 0x0400044A RID: 1098
			pfx_config_chromatic_aberration = 65536U,
			// Token: 0x0400044B RID: 1099
			pfx_config_vignette = 131072U,
			// Token: 0x0400044C RID: 1100
			pfx_config_sharpen = 262144U,
			// Token: 0x0400044D RID: 1101
			pfx_config_grain = 524288U,
			// Token: 0x0400044E RID: 1102
			pfx_config_temporal_shadow = 1048576U,
			// Token: 0x0400044F RID: 1103
			pfx_config_editor_scene = 2097152U,
			// Token: 0x04000450 RID: 1104
			pfx_config_custom1 = 16777216U,
			// Token: 0x04000451 RID: 1105
			pfx_config_custom2 = 33554432U,
			// Token: 0x04000452 RID: 1106
			pfx_config_custom3 = 67108864U,
			// Token: 0x04000453 RID: 1107
			pfx_config_custom4 = 134217728U,
			// Token: 0x04000454 RID: 1108
			pfx_config_hexagon_vignette = 268435456U,
			// Token: 0x04000455 RID: 1109
			pfx_config_screen_rt_injection = 536870912U,
			// Token: 0x04000456 RID: 1110
			pfx_config_high_dof = 1073741824U,
			// Token: 0x04000457 RID: 1111
			pfx_lower_bound = 1U,
			// Token: 0x04000458 RID: 1112
			pfx_upper_bound = 536870912U
		}

		// Token: 0x020000CB RID: 203
		public enum ViewRenderOptions
		{
			// Token: 0x0400045A RID: 1114
			ClearColor,
			// Token: 0x0400045B RID: 1115
			ClearDepth
		}
	}
}
