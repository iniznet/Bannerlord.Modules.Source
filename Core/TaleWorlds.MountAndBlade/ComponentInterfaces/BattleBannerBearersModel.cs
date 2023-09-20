using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	// Token: 0x0200040B RID: 1035
	public abstract class BattleBannerBearersModel : GameModel
	{
		// Token: 0x1700093F RID: 2367
		// (get) Token: 0x06003576 RID: 13686 RVA: 0x000DE40A File Offset: 0x000DC60A
		protected BannerBearerLogic BannerBearerLogic
		{
			get
			{
				return this._bannerBearerLogic;
			}
		}

		// Token: 0x06003577 RID: 13687 RVA: 0x000DE412 File Offset: 0x000DC612
		public void InitializeModel(BannerBearerLogic bannerBearerLogic)
		{
			this._bannerBearerLogic = bannerBearerLogic;
		}

		// Token: 0x06003578 RID: 13688 RVA: 0x000DE41B File Offset: 0x000DC61B
		public void FinalizeModel()
		{
			this._bannerBearerLogic = null;
		}

		// Token: 0x06003579 RID: 13689 RVA: 0x000DE424 File Offset: 0x000DC624
		public bool IsFormationBanner(Formation formation, SpawnedItemEntity item)
		{
			if (formation == null)
			{
				return false;
			}
			BannerBearerLogic bannerBearerLogic = this.BannerBearerLogic;
			return bannerBearerLogic != null && bannerBearerLogic.IsFormationBanner(formation, item);
		}

		// Token: 0x0600357A RID: 13690 RVA: 0x000DE44C File Offset: 0x000DC64C
		public bool IsBannerSearchingAgent(Agent agent)
		{
			BannerBearerLogic bannerBearerLogic = this.BannerBearerLogic;
			return bannerBearerLogic != null && bannerBearerLogic.IsBannerSearchingAgent(agent);
		}

		// Token: 0x0600357B RID: 13691 RVA: 0x000DE46C File Offset: 0x000DC66C
		public bool IsInteractableFormationBanner(SpawnedItemEntity item, Agent interactingAgent)
		{
			BannerBearerLogic bannerBearerLogic = this.BannerBearerLogic;
			Formation formation = ((bannerBearerLogic != null) ? bannerBearerLogic.GetFormationFromBanner(item) : null);
			return formation == null || formation.Captain == interactingAgent || interactingAgent.Formation == formation || (interactingAgent.IsPlayerControlled && interactingAgent.Team == formation.Team);
		}

		// Token: 0x0600357C RID: 13692 RVA: 0x000DE4BE File Offset: 0x000DC6BE
		public bool HasFormationBanner(Formation formation)
		{
			if (formation == null)
			{
				return false;
			}
			BannerBearerLogic bannerBearerLogic = this.BannerBearerLogic;
			return ((bannerBearerLogic != null) ? bannerBearerLogic.GetFormationBanner(formation) : null) != null;
		}

		// Token: 0x0600357D RID: 13693 RVA: 0x000DE4DC File Offset: 0x000DC6DC
		public bool HasBannerOnGround(Formation formation)
		{
			if (formation == null)
			{
				return false;
			}
			BannerBearerLogic bannerBearerLogic = this.BannerBearerLogic;
			return bannerBearerLogic != null && bannerBearerLogic.HasBannerOnGround(formation);
		}

		// Token: 0x0600357E RID: 13694 RVA: 0x000DE501 File Offset: 0x000DC701
		public ItemObject GetFormationBanner(Formation formation)
		{
			if (formation == null)
			{
				return null;
			}
			BannerBearerLogic bannerBearerLogic = this.BannerBearerLogic;
			if (bannerBearerLogic == null)
			{
				return null;
			}
			return bannerBearerLogic.GetFormationBanner(formation);
		}

		// Token: 0x0600357F RID: 13695 RVA: 0x000DE51C File Offset: 0x000DC71C
		public List<Agent> GetFormationBannerBearers(Formation formation)
		{
			if (formation == null)
			{
				return new List<Agent>();
			}
			BannerBearerLogic bannerBearerLogic = this.BannerBearerLogic;
			if (bannerBearerLogic != null)
			{
				return bannerBearerLogic.GetFormationBannerBearers(formation);
			}
			return new List<Agent>();
		}

		// Token: 0x06003580 RID: 13696 RVA: 0x000DE549 File Offset: 0x000DC749
		public BannerComponent GetActiveBanner(Formation formation)
		{
			if (formation == null)
			{
				return null;
			}
			BannerBearerLogic bannerBearerLogic = this.BannerBearerLogic;
			if (bannerBearerLogic == null)
			{
				return null;
			}
			return bannerBearerLogic.GetActiveBanner(formation);
		}

		// Token: 0x06003581 RID: 13697
		public abstract int GetMinimumFormationTroopCountToBearBanners();

		// Token: 0x06003582 RID: 13698
		public abstract float GetBannerInteractionDistance(Agent interactingAgent);

		// Token: 0x06003583 RID: 13699
		public abstract bool CanBannerBearerProvideEffectToFormation(Agent agent, Formation formation);

		// Token: 0x06003584 RID: 13700
		public abstract bool CanAgentPickUpAnyBanner(Agent agent);

		// Token: 0x06003585 RID: 13701
		public abstract bool CanAgentBecomeBannerBearer(Agent agent);

		// Token: 0x06003586 RID: 13702
		public abstract int GetAgentBannerBearingPriority(Agent agent);

		// Token: 0x06003587 RID: 13703
		public abstract bool CanFormationDeployBannerBearers(Formation formation);

		// Token: 0x06003588 RID: 13704
		public abstract int GetDesiredNumberOfBannerBearersForFormation(Formation formation);

		// Token: 0x06003589 RID: 13705
		public abstract ItemObject GetBannerBearerReplacementWeapon(BasicCharacterObject agentCharacter);

		// Token: 0x040016EA RID: 5866
		public const float DefaultDetachmentCostMultiplier = 10f;

		// Token: 0x040016EB RID: 5867
		private BannerBearerLogic _bannerBearerLogic;
	}
}
