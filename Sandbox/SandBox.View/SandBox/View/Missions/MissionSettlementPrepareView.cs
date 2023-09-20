using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions
{
	// Token: 0x0200001C RID: 28
	[DefaultView]
	public class MissionSettlementPrepareView : MissionView
	{
		// Token: 0x060000A8 RID: 168 RVA: 0x0000926F File Offset: 0x0000746F
		public override void AfterStart()
		{
			base.AfterStart();
			this.SetOwnerBanner();
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00009280 File Offset: 0x00007480
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
						Settlement.CurrentSettlement.OwnerClan.Banner.GetTableauTextureLarge(action);
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
