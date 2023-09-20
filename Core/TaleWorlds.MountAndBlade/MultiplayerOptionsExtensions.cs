﻿using System;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public static class MultiplayerOptionsExtensions
	{
		public static string GetValueText(this MultiplayerOptions.OptionType optionType, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			switch (optionType.GetOptionProperty().OptionValueType)
			{
			case MultiplayerOptions.OptionValueType.Bool:
				return optionType.GetBoolValue(mode).ToString();
			case MultiplayerOptions.OptionValueType.Integer:
			case MultiplayerOptions.OptionValueType.Enum:
				return optionType.GetIntValue(mode).ToString();
			case MultiplayerOptions.OptionValueType.String:
				return optionType.GetStrValue(mode);
			default:
				Debug.FailedAssert("Missing OptionValueType for optionType: " + optionType, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\MultiplayerOptions.cs", "GetValueText", 1062);
				return null;
			}
		}

		public static bool GetBoolValue(this MultiplayerOptions.OptionType optionType, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			int num;
			MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).GetValue(out num);
			return num == 1;
		}

		public static int GetIntValue(this MultiplayerOptions.OptionType optionType, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			int num;
			MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).GetValue(out num);
			return num;
		}

		public static string GetStrValue(this MultiplayerOptions.OptionType optionType, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			string text;
			MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).GetValue(out text);
			return text;
		}

		public static void SetValue(this MultiplayerOptions.OptionType optionType, bool value, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).UpdateValue(value ? 1 : 0);
		}

		public static void SetValue(this MultiplayerOptions.OptionType optionType, int value, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).UpdateValue(value);
		}

		public static void SetValue(this MultiplayerOptions.OptionType optionType, string value, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).UpdateValue(value);
		}

		public static int GetMinimumValue(this MultiplayerOptions.OptionType optionType)
		{
			return optionType.GetOptionProperty().BoundsMin;
		}

		public static int GetMaximumValue(this MultiplayerOptions.OptionType optionType)
		{
			return optionType.GetOptionProperty().BoundsMax;
		}

		public static MultiplayerOptionsProperty GetOptionProperty(this MultiplayerOptions.OptionType optionType)
		{
			return (MultiplayerOptionsProperty)optionType.GetType().GetField(optionType.ToString()).GetCustomAttributes(typeof(MultiplayerOptionsProperty), false)
				.Single<object>();
		}
	}
}
