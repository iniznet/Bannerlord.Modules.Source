using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Engine.Options
{
	public class NativeSelectionOptionData : NativeOptionData, ISelectionOptionData, IOptionData
	{
		public NativeSelectionOptionData(NativeOptions.NativeOptionsType type)
			: base(type)
		{
			this._selectableOptionsLimit = NativeSelectionOptionData.GetOptionsLimit(type);
			this._selectableOptionNames = NativeSelectionOptionData.GetOptionNames(type);
		}

		public int GetSelectableOptionsLimit()
		{
			return this._selectableOptionsLimit;
		}

		public IEnumerable<SelectionData> GetSelectableOptionNames()
		{
			return NativeSelectionOptionData.GetOptionNames(this.Type);
		}

		public static int GetOptionsLimit(NativeOptions.NativeOptionsType optionType)
		{
			switch (optionType)
			{
			case NativeOptions.NativeOptionsType.SoundDevice:
				return NativeOptions.GetSoundDeviceCount();
			case NativeOptions.NativeOptionsType.MaxSimultaneousSoundEventCount:
				return 4;
			case NativeOptions.NativeOptionsType.SoundOutput:
				return 3;
			case NativeOptions.NativeOptionsType.SoundPreset:
				return 4;
			case NativeOptions.NativeOptionsType.DisplayMode:
				return 3;
			case NativeOptions.NativeOptionsType.SelectedMonitor:
				return NativeOptions.GetMonitorDeviceCount();
			case NativeOptions.NativeOptionsType.SelectedAdapter:
				return NativeOptions.GetVideoDeviceCount();
			case NativeOptions.NativeOptionsType.ScreenResolution:
				return NativeOptions.GetResolutionCount() + 1;
			case NativeOptions.NativeOptionsType.RefreshRate:
				return NativeOptions.GetRefreshRateCount();
			case NativeOptions.NativeOptionsType.VSync:
				return 3;
			case NativeOptions.NativeOptionsType.OverAll:
				return 6;
			case NativeOptions.NativeOptionsType.ShaderQuality:
				return 3;
			case NativeOptions.NativeOptionsType.TextureBudget:
				return 4;
			case NativeOptions.NativeOptionsType.TextureQuality:
				return 3;
			case NativeOptions.NativeOptionsType.ShadowmapResolution:
				return 4;
			case NativeOptions.NativeOptionsType.ShadowmapType:
				return 3;
			case NativeOptions.NativeOptionsType.ShadowmapFiltering:
				return 2;
			case NativeOptions.NativeOptionsType.ParticleDetail:
				return 3;
			case NativeOptions.NativeOptionsType.ParticleQuality:
				return 3;
			case NativeOptions.NativeOptionsType.FoliageQuality:
				return 5;
			case NativeOptions.NativeOptionsType.CharacterDetail:
				return 5;
			case NativeOptions.NativeOptionsType.EnvironmentDetail:
				return 5;
			case NativeOptions.NativeOptionsType.TerrainQuality:
				return 3;
			case NativeOptions.NativeOptionsType.NumberOfRagDolls:
				return 6;
			case NativeOptions.NativeOptionsType.AnimationSamplingQuality:
				return 2;
			case NativeOptions.NativeOptionsType.Occlusion:
				return 2;
			case NativeOptions.NativeOptionsType.TextureFiltering:
				return 6;
			case NativeOptions.NativeOptionsType.WaterQuality:
				return 3;
			case NativeOptions.NativeOptionsType.Antialiasing:
				return 6;
			case NativeOptions.NativeOptionsType.DLSS:
				return NativeOptions.GetDLSSOptionCount();
			case NativeOptions.NativeOptionsType.LightingQuality:
				return 3;
			case NativeOptions.NativeOptionsType.DecalQuality:
				return 5;
			}
			return 0;
		}

		private static IEnumerable<SelectionData> GetOptionNames(NativeOptions.NativeOptionsType type)
		{
			if (type == NativeOptions.NativeOptionsType.SoundDevice)
			{
				int num;
				for (int i = 0; i < NativeOptions.GetSoundDeviceCount(); i = num + 1)
				{
					string soundDeviceName = NativeOptions.GetSoundDeviceName(i);
					if (soundDeviceName != "")
					{
						yield return new SelectionData(false, soundDeviceName);
					}
					num = i;
				}
			}
			else if (type == NativeOptions.NativeOptionsType.SelectedMonitor)
			{
				int num;
				for (int i = 0; i < NativeOptions.GetMonitorDeviceCount(); i = num + 1)
				{
					yield return new SelectionData(false, NativeOptions.GetMonitorDeviceName(i));
					num = i;
				}
			}
			else if (type == NativeOptions.NativeOptionsType.SelectedAdapter)
			{
				int num;
				for (int i = 0; i < NativeOptions.GetVideoDeviceCount(); i = num + 1)
				{
					yield return new SelectionData(false, NativeOptions.GetVideoDeviceName(i));
					num = i;
				}
			}
			else if (type == NativeOptions.NativeOptionsType.ScreenResolution)
			{
				int num;
				for (int j = 0; j < NativeOptions.GetResolutionCount(); j = num + 1)
				{
					Vec2 resolutionAtIndex = NativeOptions.GetResolutionAtIndex(j);
					yield return new SelectionData(false, string.Format("{0}x{1} ({2})", resolutionAtIndex.x, resolutionAtIndex.y, NativeSelectionOptionData.GetAspectRatioOfResolution((int)resolutionAtIndex.x, (int)resolutionAtIndex.y)));
					num = j;
				}
				int num2 = 0;
				int num3 = 0;
				int i = 0;
				int num4 = 0;
				NativeOptions.GetDesktopResolution(ref num2, ref num3);
				NativeOptions.GetResolution(ref i, ref num4);
				if (NativeOptions.GetDLSSTechnique() != 4 || num2 >= 3840)
				{
					yield return new SelectionData(true, "str_options_type_ScreenResolution_Desktop");
				}
				if (NativeOptions.GetDLSSTechnique() != 4 || i >= 3840)
				{
					yield return new SelectionData(true, "str_options_type_ScreenResolution_Custom");
				}
			}
			else if (type == NativeOptions.NativeOptionsType.RefreshRate)
			{
				int num;
				for (int i = 0; i < NativeOptions.GetRefreshRateCount(); i = num + 1)
				{
					int refreshRateAtIndex = NativeOptions.GetRefreshRateAtIndex(i);
					yield return new SelectionData(false, refreshRateAtIndex + " Hz");
					num = i;
				}
			}
			else
			{
				int i = NativeSelectionOptionData.GetOptionsLimit(type);
				string typeName = type.ToString();
				int num;
				for (int j = 0; j < i; j = num + 1)
				{
					yield return new SelectionData(true, "str_options_type_" + typeName + "_" + j.ToString());
					num = j;
				}
				typeName = null;
			}
			yield break;
		}

		private static string GetAspectRatioOfResolution(int width, int height)
		{
			return string.Format("{0}:{1}", width / MathF.GreatestCommonDivisor(width, height), height / MathF.GreatestCommonDivisor(width, height));
		}

		private readonly int _selectableOptionsLimit;

		private readonly IEnumerable<SelectionData> _selectableOptionNames;
	}
}
