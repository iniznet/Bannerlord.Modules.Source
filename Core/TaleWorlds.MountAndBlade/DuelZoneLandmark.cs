using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000376 RID: 886
	public class DuelZoneLandmark : ScriptComponentBehavior, IFocusable
	{
		// Token: 0x170008AA RID: 2218
		// (get) Token: 0x06003053 RID: 12371 RVA: 0x000C6C56 File Offset: 0x000C4E56
		public FocusableObjectType FocusableObjectType
		{
			get
			{
				return FocusableObjectType.None;
			}
		}

		// Token: 0x06003054 RID: 12372 RVA: 0x000C6C59 File Offset: 0x000C4E59
		public void OnFocusGain(Agent userAgent)
		{
		}

		// Token: 0x06003055 RID: 12373 RVA: 0x000C6C5B File Offset: 0x000C4E5B
		public void OnFocusLose(Agent userAgent)
		{
		}

		// Token: 0x06003056 RID: 12374 RVA: 0x000C6C5D File Offset: 0x000C4E5D
		public TextObject GetInfoTextForBeingNotInteractable(Agent userAgent)
		{
			return TextObject.Empty;
		}

		// Token: 0x06003057 RID: 12375 RVA: 0x000C6C64 File Offset: 0x000C4E64
		public string GetDescriptionText(GameEntity gameEntity = null)
		{
			return string.Empty;
		}

		// Token: 0x04001416 RID: 5142
		public TroopType ZoneTroopType;
	}
}
