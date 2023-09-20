using System;
using TaleWorlds.Engine;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000377 RID: 887
	public interface IFocusable
	{
		// Token: 0x06003059 RID: 12377
		void OnFocusGain(Agent userAgent);

		// Token: 0x0600305A RID: 12378
		void OnFocusLose(Agent userAgent);

		// Token: 0x170008AB RID: 2219
		// (get) Token: 0x0600305B RID: 12379
		FocusableObjectType FocusableObjectType { get; }

		// Token: 0x0600305C RID: 12380
		TextObject GetInfoTextForBeingNotInteractable(Agent userAgent);

		// Token: 0x0600305D RID: 12381
		string GetDescriptionText(GameEntity gameEntity = null);
	}
}
