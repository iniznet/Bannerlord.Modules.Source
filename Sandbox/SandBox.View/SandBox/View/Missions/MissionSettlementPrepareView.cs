using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions
{
	[DefaultView]
	public class MissionSettlementPrepareView : MissionView
	{
		public override void AfterStart()
		{
			base.AfterStart();
			this.SetOwnerBanner();
		}

		private void SetOwnerBanner()
		{
			Campaign campaign = Campaign.Current;
			if (campaign != null && campaign.GameMode == 1)
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				bool flag;
				if (currentSettlement == null)
				{
					flag = null != null;
				}
				else
				{
					Clan ownerClan = currentSettlement.OwnerClan;
					flag = ((ownerClan != null) ? ownerClan.Banner : null) != null;
				}
				if (flag && base.Mission.Scene != null)
				{
					bool flag2 = false;
					foreach (GameEntity gameEntity in base.Mission.Scene.FindEntitiesWithTag("bd_banner_b"))
					{
						Action<Texture> action = delegate(Texture tex)
						{
							Material material = Mesh.GetFromResource("bd_banner_b").GetMaterial();
							uint num = (uint)material.GetShader().GetMaterialShaderFlagMask("use_tableau_blending", true);
							ulong shaderFlags = material.GetShaderFlags();
							material.SetShaderFlags(shaderFlags | (ulong)num);
							material.SetTexture(1, tex);
						};
						BannerVisualExtensions.GetTableauTextureLarge(Settlement.CurrentSettlement.OwnerClan.Banner, action);
						flag2 = true;
					}
					if (flag2)
					{
						base.Mission.Scene.SetClothSimulationState(false);
					}
				}
			}
		}
	}
}
