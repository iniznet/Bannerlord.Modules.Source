using System;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	public static class InformationManager
	{
		public static event Action<InformationMessage> DisplayMessageInternal;

		public static event Action ClearAllMessagesInternal;

		public static event Action<string> OnAddSystemNotification;

		public static event Action<Type, object[]> OnShowTooltip;

		public static event Action OnHideTooltip;

		public static event Action<InquiryData, bool, bool> OnShowInquiry;

		public static event Action<TextInquiryData, bool, bool> OnShowTextInquiry;

		public static event Action OnHideInquiry;

		public static void DisplayMessage(InformationMessage message)
		{
			Action<InformationMessage> displayMessageInternal = InformationManager.DisplayMessageInternal;
			if (displayMessageInternal == null)
			{
				return;
			}
			displayMessageInternal(message);
		}

		public static void ClearAllMessages()
		{
			Action clearAllMessagesInternal = InformationManager.ClearAllMessagesInternal;
			if (clearAllMessagesInternal == null)
			{
				return;
			}
			clearAllMessagesInternal();
		}

		public static void AddSystemNotification(string message)
		{
			Action<string> onAddSystemNotification = InformationManager.OnAddSystemNotification;
			if (onAddSystemNotification == null)
			{
				return;
			}
			onAddSystemNotification(message);
		}

		public static void ShowTooltip(Type type, params object[] args)
		{
			Action<Type, object[]> onShowTooltip = InformationManager.OnShowTooltip;
			if (onShowTooltip == null)
			{
				return;
			}
			onShowTooltip(type, args);
		}

		public static void HideTooltip()
		{
			Action onHideTooltip = InformationManager.OnHideTooltip;
			if (onHideTooltip == null)
			{
				return;
			}
			onHideTooltip();
		}

		public static void ShowInquiry(InquiryData data, bool pauseGameActiveState = false, bool prioritize = false)
		{
			Action<InquiryData, bool, bool> onShowInquiry = InformationManager.OnShowInquiry;
			if (onShowInquiry == null)
			{
				return;
			}
			onShowInquiry(data, pauseGameActiveState, prioritize);
		}

		public static void ShowTextInquiry(TextInquiryData textData, bool pauseGameActiveState = false, bool prioritize = false)
		{
			Action<TextInquiryData, bool, bool> onShowTextInquiry = InformationManager.OnShowTextInquiry;
			if (onShowTextInquiry == null)
			{
				return;
			}
			onShowTextInquiry(textData, pauseGameActiveState, prioritize);
		}

		public static void HideInquiry()
		{
			Action onHideInquiry = InformationManager.OnHideInquiry;
			if (onHideInquiry == null)
			{
				return;
			}
			onHideInquiry();
		}

		public static void RegisterIsAnyTooltipActiveCallback(Func<bool> callback)
		{
			InformationManager._isAnyTooltipActiveCallbacks.Add(callback);
		}

		public static void UnregisterIsAnyTooltipActiveCallback(Func<bool> callback)
		{
			InformationManager._isAnyTooltipActiveCallbacks.Remove(callback);
		}

		public static void RegisterIsAnyTooltipExtendedCallback(Func<bool> callback)
		{
			InformationManager._isAnyTooltipExtendedCallbacks.Add(callback);
		}

		public static void UnregisterIsAnyTooltipExtendedCallback(Func<bool> callback)
		{
			InformationManager._isAnyTooltipExtendedCallbacks.Remove(callback);
		}

		public static bool GetIsAnyTooltipActive()
		{
			for (int i = 0; i < InformationManager._isAnyTooltipActiveCallbacks.Count; i++)
			{
				if (InformationManager._isAnyTooltipActiveCallbacks[i]())
				{
					return true;
				}
			}
			return false;
		}

		public static bool GetIsAnyTooltipExtended()
		{
			for (int i = 0; i < InformationManager._isAnyTooltipExtendedCallbacks.Count; i++)
			{
				if (InformationManager._isAnyTooltipExtendedCallbacks[i]())
				{
					return true;
				}
			}
			return false;
		}

		public static bool GetIsAnyTooltipActiveAndExtended()
		{
			return InformationManager.GetIsAnyTooltipActive() && InformationManager.GetIsAnyTooltipExtended();
		}

		public static void Clear()
		{
			InformationManager.DisplayMessageInternal = null;
			InformationManager.OnShowInquiry = null;
			InformationManager.OnShowTextInquiry = null;
			InformationManager.OnHideInquiry = null;
			InformationManager.IsAnyInquiryActive = null;
			InformationManager.OnShowTooltip = null;
			InformationManager.OnHideTooltip = null;
		}

		public static Func<bool> IsAnyInquiryActive;

		private static List<Func<bool>> _isAnyTooltipActiveCallbacks = new List<Func<bool>>();

		private static List<Func<bool>> _isAnyTooltipExtendedCallbacks = new List<Func<bool>>();
	}
}
