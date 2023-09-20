using System;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Library;

namespace SandBox.View.Map
{
	// Token: 0x02000056 RID: 86
	public abstract class MapView : SandboxView
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000378 RID: 888 RVA: 0x0001D61C File Offset: 0x0001B81C
		// (set) Token: 0x06000379 RID: 889 RVA: 0x0001D624 File Offset: 0x0001B824
		public MapScreen MapScreen { get; internal set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600037A RID: 890 RVA: 0x0001D62D File Offset: 0x0001B82D
		// (set) Token: 0x0600037B RID: 891 RVA: 0x0001D635 File Offset: 0x0001B835
		public MapState MapState { get; internal set; }

		// Token: 0x0600037C RID: 892 RVA: 0x0001D63E File Offset: 0x0001B83E
		protected internal virtual void CreateLayout()
		{
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0001D640 File Offset: 0x0001B840
		protected internal virtual void OnResume()
		{
		}

		// Token: 0x0600037E RID: 894 RVA: 0x0001D642 File Offset: 0x0001B842
		protected internal virtual void OnHourlyTick()
		{
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0001D644 File Offset: 0x0001B844
		protected internal virtual void OnStartWait(string waitMenuId)
		{
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0001D646 File Offset: 0x0001B846
		protected internal virtual void OnMainPartyEncounter()
		{
		}

		// Token: 0x06000381 RID: 897 RVA: 0x0001D648 File Offset: 0x0001B848
		protected internal virtual void OnDispersePlayerLeadedArmy()
		{
		}

		// Token: 0x06000382 RID: 898 RVA: 0x0001D64A File Offset: 0x0001B84A
		protected internal virtual void OnArmyLeft()
		{
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0001D64C File Offset: 0x0001B84C
		protected internal virtual bool IsEscaped()
		{
			return false;
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0001D64F File Offset: 0x0001B84F
		protected internal virtual void OnOverlayCreated()
		{
		}

		// Token: 0x06000385 RID: 901 RVA: 0x0001D651 File Offset: 0x0001B851
		protected internal virtual void OnOverlayClosed()
		{
		}

		// Token: 0x06000386 RID: 902 RVA: 0x0001D653 File Offset: 0x0001B853
		protected internal virtual void OnMenuModeTick(float dt)
		{
		}

		// Token: 0x06000387 RID: 903 RVA: 0x0001D655 File Offset: 0x0001B855
		protected internal virtual void OnMapScreenUpdate(float dt)
		{
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0001D657 File Offset: 0x0001B857
		protected internal virtual void OnIdleTick(float dt)
		{
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0001D659 File Offset: 0x0001B859
		protected internal virtual void OnMapTerrainClick()
		{
		}

		// Token: 0x0600038A RID: 906 RVA: 0x0001D65B File Offset: 0x0001B85B
		protected internal virtual void OnSiegeEngineClick(MatrixFrame siegeEngineFrame)
		{
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0001D65D File Offset: 0x0001B85D
		protected internal virtual void OnMapConversationStart()
		{
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0001D65F File Offset: 0x0001B85F
		protected internal virtual void OnMapConversationUpdate(ConversationCharacterData playerConversationData, ConversationCharacterData partnerConversationData)
		{
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0001D661 File Offset: 0x0001B861
		protected internal virtual void OnMapConversationOver()
		{
		}

		// Token: 0x040001CD RID: 461
		protected const float ContextAlphaModifier = 8.5f;
	}
}
