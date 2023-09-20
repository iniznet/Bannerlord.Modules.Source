using System;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	// Token: 0x02000037 RID: 55
	public static class InformationManager
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x060001A0 RID: 416 RVA: 0x000062F8 File Offset: 0x000044F8
		// (remove) Token: 0x060001A1 RID: 417 RVA: 0x0000632C File Offset: 0x0000452C
		public static event Action<InformationMessage> DisplayMessageInternal;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x060001A2 RID: 418 RVA: 0x00006360 File Offset: 0x00004560
		// (remove) Token: 0x060001A3 RID: 419 RVA: 0x00006394 File Offset: 0x00004594
		public static event Action ClearAllMessagesInternal;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x060001A4 RID: 420 RVA: 0x000063C8 File Offset: 0x000045C8
		// (remove) Token: 0x060001A5 RID: 421 RVA: 0x000063FC File Offset: 0x000045FC
		public static event Action<string> OnAddSystemNotification;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x060001A6 RID: 422 RVA: 0x00006430 File Offset: 0x00004630
		// (remove) Token: 0x060001A7 RID: 423 RVA: 0x00006464 File Offset: 0x00004664
		public static event Action<Type, object[]> OnShowTooltip;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x060001A8 RID: 424 RVA: 0x00006498 File Offset: 0x00004698
		// (remove) Token: 0x060001A9 RID: 425 RVA: 0x000064CC File Offset: 0x000046CC
		public static event Action OnHideTooltip;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x060001AA RID: 426 RVA: 0x00006500 File Offset: 0x00004700
		// (remove) Token: 0x060001AB RID: 427 RVA: 0x00006534 File Offset: 0x00004734
		public static event Action<InquiryData, bool, bool> OnShowInquiry;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x060001AC RID: 428 RVA: 0x00006568 File Offset: 0x00004768
		// (remove) Token: 0x060001AD RID: 429 RVA: 0x0000659C File Offset: 0x0000479C
		public static event Action<TextInquiryData, bool, bool> OnShowTextInquiry;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x060001AE RID: 430 RVA: 0x000065D0 File Offset: 0x000047D0
		// (remove) Token: 0x060001AF RID: 431 RVA: 0x00006604 File Offset: 0x00004804
		public static event Action OnHideInquiry;

		// Token: 0x060001B0 RID: 432 RVA: 0x00006637 File Offset: 0x00004837
		public static void DisplayMessage(InformationMessage message)
		{
			Action<InformationMessage> displayMessageInternal = InformationManager.DisplayMessageInternal;
			if (displayMessageInternal == null)
			{
				return;
			}
			displayMessageInternal(message);
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00006649 File Offset: 0x00004849
		public static void ClearAllMessages()
		{
			Action clearAllMessagesInternal = InformationManager.ClearAllMessagesInternal;
			if (clearAllMessagesInternal == null)
			{
				return;
			}
			clearAllMessagesInternal();
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000665A File Offset: 0x0000485A
		public static void AddSystemNotification(string message)
		{
			Action<string> onAddSystemNotification = InformationManager.OnAddSystemNotification;
			if (onAddSystemNotification == null)
			{
				return;
			}
			onAddSystemNotification(message);
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000666C File Offset: 0x0000486C
		public static void ShowTooltip(Type type, params object[] args)
		{
			Action<Type, object[]> onShowTooltip = InformationManager.OnShowTooltip;
			if (onShowTooltip == null)
			{
				return;
			}
			onShowTooltip(type, args);
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000667F File Offset: 0x0000487F
		public static void HideTooltip()
		{
			Action onHideTooltip = InformationManager.OnHideTooltip;
			if (onHideTooltip == null)
			{
				return;
			}
			onHideTooltip();
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00006690 File Offset: 0x00004890
		public static void ShowInquiry(InquiryData data, bool pauseGameActiveState = false, bool prioritize = false)
		{
			Action<InquiryData, bool, bool> onShowInquiry = InformationManager.OnShowInquiry;
			if (onShowInquiry == null)
			{
				return;
			}
			onShowInquiry(data, pauseGameActiveState, prioritize);
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x000066A4 File Offset: 0x000048A4
		public static void ShowTextInquiry(TextInquiryData textData, bool pauseGameActiveState = false, bool prioritize = false)
		{
			Action<TextInquiryData, bool, bool> onShowTextInquiry = InformationManager.OnShowTextInquiry;
			if (onShowTextInquiry == null)
			{
				return;
			}
			onShowTextInquiry(textData, pauseGameActiveState, prioritize);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x000066B8 File Offset: 0x000048B8
		public static void HideInquiry()
		{
			Action onHideInquiry = InformationManager.OnHideInquiry;
			if (onHideInquiry == null)
			{
				return;
			}
			onHideInquiry();
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x000066C9 File Offset: 0x000048C9
		public static void RegisterIsAnyTooltipActiveCallback(Func<bool> callback)
		{
			InformationManager._isAnyTooltipActiveCallbacks.Add(callback);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x000066D6 File Offset: 0x000048D6
		public static void UnregisterIsAnyTooltipActiveCallback(Func<bool> callback)
		{
			InformationManager._isAnyTooltipActiveCallbacks.Remove(callback);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x000066E4 File Offset: 0x000048E4
		public static void RegisterIsAnyTooltipExtendedCallback(Func<bool> callback)
		{
			InformationManager._isAnyTooltipExtendedCallbacks.Add(callback);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x000066F1 File Offset: 0x000048F1
		public static void UnregisterIsAnyTooltipExtendedCallback(Func<bool> callback)
		{
			InformationManager._isAnyTooltipExtendedCallbacks.Remove(callback);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00006700 File Offset: 0x00004900
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

		// Token: 0x060001BD RID: 445 RVA: 0x00006738 File Offset: 0x00004938
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

		// Token: 0x060001BE RID: 446 RVA: 0x0000676F File Offset: 0x0000496F
		public static bool GetIsAnyTooltipActiveAndExtended()
		{
			return InformationManager.GetIsAnyTooltipActive() && InformationManager.GetIsAnyTooltipExtended();
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000677F File Offset: 0x0000497F
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

		// Token: 0x04000092 RID: 146
		public static Func<bool> IsAnyInquiryActive;

		// Token: 0x0400009B RID: 155
		private static List<Func<bool>> _isAnyTooltipActiveCallbacks = new List<Func<bool>>();

		// Token: 0x0400009C RID: 156
		private static List<Func<bool>> _isAnyTooltipExtendedCallbacks = new List<Func<bool>>();
	}
}
