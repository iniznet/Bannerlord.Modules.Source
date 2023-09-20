using System;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200030D RID: 781
	public static class MultiplayerOptionsExtensions
	{
		// Token: 0x06002A50 RID: 10832 RVA: 0x000A4940 File Offset: 0x000A2B40
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

		// Token: 0x06002A51 RID: 10833 RVA: 0x000A49C0 File Offset: 0x000A2BC0
		public static bool GetBoolValue(this MultiplayerOptions.OptionType optionType, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			int num;
			MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).GetValue(out num);
			return num == 1;
		}

		// Token: 0x06002A52 RID: 10834 RVA: 0x000A49E4 File Offset: 0x000A2BE4
		public static int GetIntValue(this MultiplayerOptions.OptionType optionType, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			int num;
			MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).GetValue(out num);
			return num;
		}

		// Token: 0x06002A53 RID: 10835 RVA: 0x000A4A08 File Offset: 0x000A2C08
		public static string GetStrValue(this MultiplayerOptions.OptionType optionType, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			string text;
			MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).GetValue(out text);
			return text;
		}

		// Token: 0x06002A54 RID: 10836 RVA: 0x000A4A29 File Offset: 0x000A2C29
		public static void SetValue(this MultiplayerOptions.OptionType optionType, bool value, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).UpdateValue(value ? 1 : 0);
		}

		// Token: 0x06002A55 RID: 10837 RVA: 0x000A4A44 File Offset: 0x000A2C44
		public static void SetValue(this MultiplayerOptions.OptionType optionType, int value, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).UpdateValue(value);
		}

		// Token: 0x06002A56 RID: 10838 RVA: 0x000A4A59 File Offset: 0x000A2C59
		public static void SetValue(this MultiplayerOptions.OptionType optionType, string value, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).UpdateValue(value);
		}

		// Token: 0x06002A57 RID: 10839 RVA: 0x000A4A6E File Offset: 0x000A2C6E
		public static int GetMinimumValue(this MultiplayerOptions.OptionType optionType)
		{
			return optionType.GetOptionProperty().BoundsMin;
		}

		// Token: 0x06002A58 RID: 10840 RVA: 0x000A4A7B File Offset: 0x000A2C7B
		public static int GetMaximumValue(this MultiplayerOptions.OptionType optionType)
		{
			return optionType.GetOptionProperty().BoundsMax;
		}

		// Token: 0x06002A59 RID: 10841 RVA: 0x000A4A88 File Offset: 0x000A2C88
		public static MultiplayerOptionsProperty GetOptionProperty(this MultiplayerOptions.OptionType optionType)
		{
			return (MultiplayerOptionsProperty)optionType.GetType().GetField(optionType.ToString()).GetCustomAttributes(typeof(MultiplayerOptionsProperty), false)
				.Single<object>();
		}
	}
}
