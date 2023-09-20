using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	public static class MBInformationManager
	{
		public static event Action<string, int, BasicCharacterObject, string> FiringQuickInformation;

		public static event Action<MultiSelectionInquiryData, bool, bool> OnShowMultiSelectionInquiry;

		public static event Action<InformationData> OnAddMapNotice;

		public static event Action<InformationData> OnRemoveMapNotice;

		public static event Action<SceneNotificationData> OnShowSceneNotification;

		public static event Action OnHideSceneNotification;

		public static event Func<bool> IsAnySceneNotificationActive;

		public static void AddQuickInformation(TextObject message, int priority = 0, BasicCharacterObject announcerCharacter = null, string soundEventPath = "")
		{
			Action<string, int, BasicCharacterObject, string> firingQuickInformation = MBInformationManager.FiringQuickInformation;
			if (firingQuickInformation != null)
			{
				firingQuickInformation(message.ToString(), priority, announcerCharacter, soundEventPath);
			}
			Debug.Print(message.ToString(), 0, Debug.DebugColor.White, 1125899906842624UL);
		}

		public static void ShowMultiSelectionInquiry(MultiSelectionInquiryData data, bool pauseGameActiveState = false, bool prioritize = false)
		{
			Action<MultiSelectionInquiryData, bool, bool> onShowMultiSelectionInquiry = MBInformationManager.OnShowMultiSelectionInquiry;
			if (onShowMultiSelectionInquiry == null)
			{
				return;
			}
			onShowMultiSelectionInquiry(data, pauseGameActiveState, prioritize);
		}

		public static void AddNotice(InformationData data)
		{
			Action<InformationData> onAddMapNotice = MBInformationManager.OnAddMapNotice;
			if (onAddMapNotice == null)
			{
				return;
			}
			onAddMapNotice(data);
		}

		public static void MapNoticeRemoved(InformationData data)
		{
			Action<InformationData> onRemoveMapNotice = MBInformationManager.OnRemoveMapNotice;
			if (onRemoveMapNotice == null)
			{
				return;
			}
			onRemoveMapNotice(data);
		}

		public static void ShowHint(string hint)
		{
			InformationManager.ShowTooltip(typeof(string), new object[] { hint });
		}

		public static void HideInformations()
		{
			InformationManager.HideTooltip();
		}

		public static void ShowSceneNotification(SceneNotificationData data)
		{
			Action<SceneNotificationData> onShowSceneNotification = MBInformationManager.OnShowSceneNotification;
			if (onShowSceneNotification == null)
			{
				return;
			}
			onShowSceneNotification(data);
		}

		public static void HideSceneNotification()
		{
			Action onHideSceneNotification = MBInformationManager.OnHideSceneNotification;
			if (onHideSceneNotification == null)
			{
				return;
			}
			onHideSceneNotification();
		}

		public static bool? GetIsAnySceneNotificationActive()
		{
			Func<bool> isAnySceneNotificationActive = MBInformationManager.IsAnySceneNotificationActive;
			if (isAnySceneNotificationActive == null)
			{
				return null;
			}
			return new bool?(isAnySceneNotificationActive());
		}

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
