using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	public abstract class BattleBannerBearersModel : GameModel
	{
		protected BannerBearerLogic BannerBearerLogic
		{
			get
			{
				return this._bannerBearerLogic;
			}
		}

		public void InitializeModel(BannerBearerLogic bannerBearerLogic)
		{
			this._bannerBearerLogic = bannerBearerLogic;
		}

		public void FinalizeModel()
		{
			this._bannerBearerLogic = null;
		}

		public bool IsFormationBanner(Formation formation, SpawnedItemEntity item)
		{
			if (formation == null)
			{
				return false;
			}
			BannerBearerLogic bannerBearerLogic = this.BannerBearerLogic;
			return bannerBearerLogic != null && bannerBearerLogic.IsFormationBanner(formation, item);
		}

		public bool IsBannerSearchingAgent(Agent agent)
		{
			BannerBearerLogic bannerBearerLogic = this.BannerBearerLogic;
			return bannerBearerLogic != null && bannerBearerLogic.IsBannerSearchingAgent(agent);
		}

		public bool IsInteractableFormationBanner(SpawnedItemEntity item, Agent interactingAgent)
		{
			BannerBearerLogic bannerBearerLogic = this.BannerBearerLogic;
			Formation formation = ((bannerBearerLogic != null) ? bannerBearerLogic.GetFormationFromBanner(item) : null);
			return formation == null || formation.Captain == interactingAgent || interactingAgent.Formation == formation || (interactingAgent.IsPlayerControlled && interactingAgent.Team == formation.Team);
		}

		public bool HasFormationBanner(Formation formation)
		{
			if (formation == null)
			{
				return false;
			}
			BannerBearerLogic bannerBearerLogic = this.BannerBearerLogic;
			return ((bannerBearerLogic != null) ? bannerBearerLogic.GetFormationBanner(formation) : null) != null;
		}

		public bool HasBannerOnGround(Formation formation)
		{
			if (formation == null)
			{
				return false;
			}
			BannerBearerLogic bannerBearerLogic = this.BannerBearerLogic;
			return bannerBearerLogic != null && bannerBearerLogic.HasBannerOnGround(formation);
		}

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

		public abstract int GetMinimumFormationTroopCountToBearBanners();

		public abstract float GetBannerInteractionDistance(Agent interactingAgent);

		public abstract bool CanBannerBearerProvideEffectToFormation(Agent agent, Formation formation);

		public abstract bool CanAgentPickUpAnyBanner(Agent agent);

		public abstract bool CanAgentBecomeBannerBearer(Agent agent);

		public abstract int GetAgentBannerBearingPriority(Agent agent);

		public abstract bool CanFormationDeployBannerBearers(Formation formation);

		public abstract int GetDesiredNumberOfBannerBearersForFormation(Formation formation);

		public abstract ItemObject GetBannerBearerReplacementWeapon(BasicCharacterObject agentCharacter);

		public const float DefaultDetachmentCostMultiplier = 10f;

		private BannerBearerLogic _bannerBearerLogic;
	}
}
