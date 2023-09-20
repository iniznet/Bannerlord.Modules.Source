using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Overlay;

namespace SandBox.View.Menu
{
	// Token: 0x02000035 RID: 53
	public abstract class MenuView : SandboxView
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000181 RID: 385 RVA: 0x000116B3 File Offset: 0x0000F8B3
		// (set) Token: 0x06000182 RID: 386 RVA: 0x000116BB File Offset: 0x0000F8BB
		internal bool Removed { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000183 RID: 387 RVA: 0x000116C4 File Offset: 0x0000F8C4
		public virtual bool ShouldUpdateMenuAfterRemoved
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000184 RID: 388 RVA: 0x000116C7 File Offset: 0x0000F8C7
		// (set) Token: 0x06000185 RID: 389 RVA: 0x000116CF File Offset: 0x0000F8CF
		public MenuViewContext MenuViewContext { get; internal set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000186 RID: 390 RVA: 0x000116D8 File Offset: 0x0000F8D8
		// (set) Token: 0x06000187 RID: 391 RVA: 0x000116E0 File Offset: 0x0000F8E0
		public MenuContext MenuContext { get; internal set; }

		// Token: 0x06000188 RID: 392 RVA: 0x000116E9 File Offset: 0x0000F8E9
		protected internal virtual void OnMenuContextUpdated(MenuContext newMenuContext)
		{
		}

		// Token: 0x06000189 RID: 393 RVA: 0x000116EB File Offset: 0x0000F8EB
		protected internal virtual void OnOverlayTypeChange(GameOverlays.MenuOverlayType newType)
		{
		}

		// Token: 0x0600018A RID: 394 RVA: 0x000116ED File Offset: 0x0000F8ED
		protected internal virtual void OnCharacterDeveloperOpened()
		{
		}

		// Token: 0x0600018B RID: 395 RVA: 0x000116EF File Offset: 0x0000F8EF
		protected internal virtual void OnCharacterDeveloperClosed()
		{
		}

		// Token: 0x0600018C RID: 396 RVA: 0x000116F1 File Offset: 0x0000F8F1
		protected internal virtual void OnBackgroundMeshNameSet(string name)
		{
		}

		// Token: 0x0600018D RID: 397 RVA: 0x000116F3 File Offset: 0x0000F8F3
		protected internal virtual void OnHourlyTick()
		{
		}

		// Token: 0x0600018E RID: 398 RVA: 0x000116F5 File Offset: 0x0000F8F5
		protected internal virtual void OnResume()
		{
		}

		// Token: 0x0600018F RID: 399 RVA: 0x000116F7 File Offset: 0x0000F8F7
		protected internal virtual void OnMapConversationActivated()
		{
		}

		// Token: 0x06000190 RID: 400 RVA: 0x000116F9 File Offset: 0x0000F8F9
		protected internal virtual void OnMapConversationDeactivated()
		{
		}

		// Token: 0x040000E8 RID: 232
		protected const float ContextAlphaModifier = 8.5f;
	}
}
