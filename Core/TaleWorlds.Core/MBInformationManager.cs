using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	// Token: 0x020000A6 RID: 166
	public static class MBInformationManager
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000812 RID: 2066 RVA: 0x0001BB14 File Offset: 0x00019D14
		// (remove) Token: 0x06000813 RID: 2067 RVA: 0x0001BB48 File Offset: 0x00019D48
		public static event Action<string, int, BasicCharacterObject, string> FiringQuickInformation;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000814 RID: 2068 RVA: 0x0001BB7C File Offset: 0x00019D7C
		// (remove) Token: 0x06000815 RID: 2069 RVA: 0x0001BBB0 File Offset: 0x00019DB0
		public static event Action<MultiSelectionInquiryData, bool, bool> OnShowMultiSelectionInquiry;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000816 RID: 2070 RVA: 0x0001BBE4 File Offset: 0x00019DE4
		// (remove) Token: 0x06000817 RID: 2071 RVA: 0x0001BC18 File Offset: 0x00019E18
		public static event Action<InformationData> OnAddMapNotice;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000818 RID: 2072 RVA: 0x0001BC4C File Offset: 0x00019E4C
		// (remove) Token: 0x06000819 RID: 2073 RVA: 0x0001BC80 File Offset: 0x00019E80
		public static event Action<InformationData> OnRemoveMapNotice;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x0600081A RID: 2074 RVA: 0x0001BCB4 File Offset: 0x00019EB4
		// (remove) Token: 0x0600081B RID: 2075 RVA: 0x0001BCE8 File Offset: 0x00019EE8
		public static event Action<SceneNotificationData> OnShowSceneNotification;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x0600081C RID: 2076 RVA: 0x0001BD1C File Offset: 0x00019F1C
		// (remove) Token: 0x0600081D RID: 2077 RVA: 0x0001BD50 File Offset: 0x00019F50
		public static event Action OnHideSceneNotification;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x0600081E RID: 2078 RVA: 0x0001BD84 File Offset: 0x00019F84
		// (remove) Token: 0x0600081F RID: 2079 RVA: 0x0001BDB8 File Offset: 0x00019FB8
		public static event Func<bool> IsAnySceneNotificationActive;

		// Token: 0x06000820 RID: 2080 RVA: 0x0001BDEB File Offset: 0x00019FEB
		public static void AddQuickInformation(TextObject message, int priority = 0, BasicCharacterObject announcerCharacter = null, string soundEventPath = "")
		{
			Action<string, int, BasicCharacterObject, string> firingQuickInformation = MBInformationManager.FiringQuickInformation;
			if (firingQuickInformation != null)
			{
				firingQuickInformation(message.ToString(), priority, announcerCharacter, soundEventPath);
			}
			Debug.Print(message.ToString(), 0, Debug.DebugColor.White, 1125899906842624UL);
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x0001BE1D File Offset: 0x0001A01D
		public static void ShowMultiSelectionInquiry(MultiSelectionInquiryData data, bool pauseGameActiveState = false, bool prioritize = false)
		{
			Action<MultiSelectionInquiryData, bool, bool> onShowMultiSelectionInquiry = MBInformationManager.OnShowMultiSelectionInquiry;
			if (onShowMultiSelectionInquiry == null)
			{
				return;
			}
			onShowMultiSelectionInquiry(data, pauseGameActiveState, prioritize);
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x0001BE31 File Offset: 0x0001A031
		public static void AddNotice(InformationData data)
		{
			Action<InformationData> onAddMapNotice = MBInformationManager.OnAddMapNotice;
			if (onAddMapNotice == null)
			{
				return;
			}
			onAddMapNotice(data);
		}

		// Token: 0x06000823 RID: 2083 RVA: 0x0001BE43 File Offset: 0x0001A043
		public static void MapNoticeRemoved(InformationData data)
		{
			Action<InformationData> onRemoveMapNotice = MBInformationManager.OnRemoveMapNotice;
			if (onRemoveMapNotice == null)
			{
				return;
			}
			onRemoveMapNotice(data);
		}

		// Token: 0x06000824 RID: 2084 RVA: 0x0001BE55 File Offset: 0x0001A055
		public static void ShowHint(string hint)
		{
			InformationManager.ShowTooltip(typeof(string), new object[] { hint });
		}

		// Token: 0x06000825 RID: 2085 RVA: 0x0001BE70 File Offset: 0x0001A070
		public static void HideInformations()
		{
			InformationManager.HideTooltip();
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x0001BE77 File Offset: 0x0001A077
		public static void ShowSceneNotification(SceneNotificationData data)
		{
			Action<SceneNotificationData> onShowSceneNotification = MBInformationManager.OnShowSceneNotification;
			if (onShowSceneNotification == null)
			{
				return;
			}
			onShowSceneNotification(data);
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x0001BE89 File Offset: 0x0001A089
		public static void HideSceneNotification()
		{
			Action onHideSceneNotification = MBInformationManager.OnHideSceneNotification;
			if (onHideSceneNotification == null)
			{
				return;
			}
			onHideSceneNotification();
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x0001BE9C File Offset: 0x0001A09C
		public static bool? GetIsAnySceneNotificationActive()
		{
			Func<bool> isAnySceneNotificationActive = MBInformationManager.IsAnySceneNotificationActive;
			if (isAnySceneNotificationActive == null)
			{
				return null;
			}
			return new bool?(isAnySceneNotificationActive());
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x0001BEC6 File Offset: 0x0001A0C6
		public static void Clear()
		{
			MBInformationManager.FiringQuickInformation = null;
			MBInformationManager.OnShowMultiSelectionInquiry = null;
			MBInformationManager.OnAddMapNotice = null;
			MBInformationManager.OnRemoveMapNotice = null;
			MBInformationManager.OnShowSceneNotification = null;
			MBInformationManager.OnHideSceneNotification = null;
		}
	}
}
