using System;
using System.Collections.Generic;
using System.ComponentModel;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	// Token: 0x02000042 RID: 66
	public class MissionAgentLabelView : MissionView
	{
		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060002F6 RID: 758 RVA: 0x0001A7A2 File Offset: 0x000189A2
		private OrderController PlayerOrderController
		{
			get
			{
				Team playerTeam = base.Mission.PlayerTeam;
				if (playerTeam == null)
				{
					return null;
				}
				return playerTeam.PlayerOrderController;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x0001A7BA File Offset: 0x000189BA
		private SiegeWeaponController PlayerSiegeWeaponController
		{
			get
			{
				Team playerTeam = base.Mission.PlayerTeam;
				if (playerTeam == null)
				{
					return null;
				}
				return playerTeam.PlayerOrderController.SiegeWeaponController;
			}
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0001A7D7 File Offset: 0x000189D7
		public MissionAgentLabelView()
		{
			this._agentMeshes = new Dictionary<Agent, MetaMesh>();
			this._labelMaterials = new Dictionary<Texture, Material>();
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0001A7F8 File Offset: 0x000189F8
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			base.Mission.Teams.OnPlayerTeamChanged += this.Mission_OnPlayerTeamChanged;
			base.Mission.OnMainAgentChanged += this.OnMainAgentChanged;
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			base.MissionScreen.OnSpectateAgentFocusIn += this.HandleSpectateAgentFocusIn;
			base.MissionScreen.OnSpectateAgentFocusOut += this.HandleSpectateAgentFocusOut;
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0001A88C File Offset: 0x00018A8C
		public override void AfterStart()
		{
			if (this.PlayerOrderController != null)
			{
				this.PlayerOrderController.OnSelectedFormationsChanged += this.OrderController_OnSelectedFormationsChanged;
				base.Mission.PlayerTeam.OnFormationsChanged += this.PlayerTeam_OnFormationsChanged;
			}
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0001A8CC File Offset: 0x00018ACC
		public override void OnMissionTick(float dt)
		{
			bool flag = this.IsOrderScreenVisible();
			bool flag2 = this.IsSiegeControllerScreenVisible();
			if (!flag && this._wasOrderScreenVisible)
			{
				this.SetHighlightForAgents(false, false, false);
			}
			if (!flag2 && this._wasSiegeControllerScreenVisible)
			{
				this.SetHighlightForAgents(false, true, false);
			}
			if (flag && !this._wasOrderScreenVisible)
			{
				this.SetHighlightForAgents(true, false, false);
			}
			if (flag2 && !this._wasSiegeControllerScreenVisible)
			{
				this.SetHighlightForAgents(true, true, false);
			}
			this._wasOrderScreenVisible = flag;
			this._wasSiegeControllerScreenVisible = flag2;
		}

		// Token: 0x060002FC RID: 764 RVA: 0x0001A948 File Offset: 0x00018B48
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.Mission.Teams.OnPlayerTeamChanged -= this.Mission_OnPlayerTeamChanged;
			base.Mission.OnMainAgentChanged -= this.OnMainAgentChanged;
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			base.MissionScreen.OnSpectateAgentFocusIn -= this.HandleSpectateAgentFocusIn;
			base.MissionScreen.OnSpectateAgentFocusOut -= this.HandleSpectateAgentFocusOut;
			if (this.PlayerOrderController != null)
			{
				this.PlayerOrderController.OnSelectedFormationsChanged -= this.OrderController_OnSelectedFormationsChanged;
				base.Mission.PlayerTeam.OnFormationsChanged -= this.PlayerTeam_OnFormationsChanged;
			}
		}

		// Token: 0x060002FD RID: 765 RVA: 0x0001AA18 File Offset: 0x00018C18
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (affectedAgent.IsHuman && this._agentMeshes.ContainsKey(affectedAgent))
			{
				if (affectedAgent.AgentVisuals != null)
				{
					affectedAgent.AgentVisuals.ReplaceMeshWithMesh(this._agentMeshes[affectedAgent], null, 13);
				}
				this._agentMeshes.Remove(affectedAgent);
			}
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0001AA70 File Offset: 0x00018C70
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			this.InitAgentLabel(agent, banner);
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0001AA7C File Offset: 0x00018C7C
		public override void OnAssignPlayerAsSergeantOfFormation(Agent agent)
		{
			float friendlyTroopsBannerOpacity = BannerlordConfig.FriendlyTroopsBannerOpacity;
			this._agentMeshes[agent].SetVectorArgument2(30f, 0.4f, 0.44f, 1f * friendlyTroopsBannerOpacity);
		}

		// Token: 0x06000300 RID: 768 RVA: 0x0001AAB6 File Offset: 0x00018CB6
		public override void OnClearScene()
		{
			this._agentMeshes.Clear();
			this._labelMaterials.Clear();
		}

		// Token: 0x06000301 RID: 769 RVA: 0x0001AACE File Offset: 0x00018CCE
		private void PlayerTeam_OnFormationsChanged(Team team, Formation formation)
		{
			if (this.IsOrderScreenVisible())
			{
				this.DehighlightAllAgents();
				this.SetHighlightForAgents(true, false, false);
			}
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0001AAE8 File Offset: 0x00018CE8
		private void Mission_OnPlayerTeamChanged(Team previousTeam, Team currentTeam)
		{
			this.DehighlightAllAgents();
			this._wasOrderScreenVisible = false;
			if (((previousTeam != null) ? previousTeam.PlayerOrderController : null) != null)
			{
				previousTeam.PlayerOrderController.OnSelectedFormationsChanged -= this.OrderController_OnSelectedFormationsChanged;
				previousTeam.PlayerOrderController.SiegeWeaponController.OnSelectedSiegeWeaponsChanged -= this.PlayerSiegeWeaponController_OnSelectedSiegeWeaponsChanged;
			}
			if (this.PlayerOrderController != null)
			{
				this.PlayerOrderController.OnSelectedFormationsChanged += this.OrderController_OnSelectedFormationsChanged;
				this.PlayerSiegeWeaponController.OnSelectedSiegeWeaponsChanged += this.PlayerSiegeWeaponController_OnSelectedSiegeWeaponsChanged;
			}
			this.SetHighlightForAgents(true, false, true);
			foreach (Agent agent in base.Mission.Agents)
			{
				this.UpdateVisibilityOfAgentMesh(agent);
			}
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0001ABD0 File Offset: 0x00018DD0
		private void OrderController_OnSelectedFormationsChanged()
		{
			this.DehighlightAllAgents();
			if (this.IsOrderScreenVisible())
			{
				this.SetHighlightForAgents(true, false, false);
			}
		}

		// Token: 0x06000304 RID: 772 RVA: 0x0001ABE9 File Offset: 0x00018DE9
		private void PlayerSiegeWeaponController_OnSelectedSiegeWeaponsChanged()
		{
			this.DehighlightAllAgents();
			this.SetHighlightForAgents(true, true, false);
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0001ABFC File Offset: 0x00018DFC
		public void OnAgentListSelectionChanged(bool selectionMode, List<Agent> affectedAgents)
		{
			foreach (Agent agent in affectedAgents)
			{
				float num = (selectionMode ? 1f : (-1f));
				if (this._agentMeshes.ContainsKey(agent))
				{
					MetaMesh metaMesh = this._agentMeshes[agent];
					float friendlyTroopsBannerOpacity = BannerlordConfig.FriendlyTroopsBannerOpacity;
					metaMesh.SetVectorArgument2(30f, 0.4f, 0.44f, num * friendlyTroopsBannerOpacity);
				}
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0001AC8C File Offset: 0x00018E8C
		private void InitAgentLabel(Agent agent, Banner peerBanner = null)
		{
			if (agent.IsHuman)
			{
				Banner banner = peerBanner ?? agent.Origin.Banner;
				if (banner != null)
				{
					MetaMesh copy = MetaMesh.GetCopy("troop_banner_selection", false, true);
					Material tableauMaterial = Material.GetFromResource("agent_label_with_tableau");
					Texture texture = banner.GetTableauTextureSmall(null);
					if (copy != null && tableauMaterial != null)
					{
						Texture fromResource = Texture.GetFromResource("banner_top_of_head");
						Material material;
						if (this._labelMaterials.TryGetValue(texture ?? fromResource, out material))
						{
							tableauMaterial = material;
						}
						else
						{
							tableauMaterial = tableauMaterial.CreateCopy();
							Action<Texture> action = delegate(Texture tex)
							{
								tableauMaterial.SetTexture(0, tex);
							};
							texture = banner.GetTableauTextureSmall(action);
							tableauMaterial.SetTexture(1, fromResource);
							this._labelMaterials.Add(texture, tableauMaterial);
						}
						copy.SetMaterial(tableauMaterial);
						copy.SetVectorArgument(0.5f, 0.5f, 0.25f, 0.25f);
						copy.SetVectorArgument2(30f, 0.4f, 0.44f, -1f);
						agent.AgentVisuals.AddMultiMesh(copy, 13);
						this._agentMeshes.Add(agent, copy);
						this.UpdateVisibilityOfAgentMesh(agent);
						this.UpdateSelectionVisibility(agent, this._agentMeshes[agent], new bool?(false));
					}
				}
			}
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0001ADF4 File Offset: 0x00018FF4
		private void UpdateVisibilityOfAgentMesh(Agent agent)
		{
			if (agent.IsActive() && this._agentMeshes.ContainsKey(agent))
			{
				bool flag = this.IsMeshVisibleForAgent(agent);
				this._agentMeshes[agent].SetVisibilityMask(flag ? 1 : 0);
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0001AE37 File Offset: 0x00019037
		private bool IsMeshVisibleForAgent(Agent agent)
		{
			return this.IsAllyInAllyTeam(agent) && base.MissionScreen.LastFollowedAgent != agent && BannerlordConfig.FriendlyTroopsBannerOpacity > 0f && !base.MissionScreen.IsPhotoModeEnabled;
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0001AE6C File Offset: 0x0001906C
		private void OnUpdateOpacityValueOfAgentMesh(Agent agent)
		{
			if (agent.IsActive() && this._agentMeshes.ContainsKey(agent))
			{
				this._agentMeshes[agent].SetVectorArgument2(30f, 0.4f, 0.44f, -BannerlordConfig.FriendlyTroopsBannerOpacity);
			}
		}

		// Token: 0x0600030A RID: 778 RVA: 0x0001AEAC File Offset: 0x000190AC
		private bool IsAllyInAllyTeam(Agent agent)
		{
			if (((agent != null) ? agent.Team : null) != null && agent != base.Mission.MainAgent)
			{
				Team team = null;
				Team team2;
				if (GameNetwork.IsSessionActive)
				{
					team2 = (GameNetwork.IsMyPeerReady ? PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer).Team : null);
				}
				else
				{
					team2 = base.Mission.PlayerTeam;
					team = base.Mission.PlayerAllyTeam;
				}
				return agent.Team == team2 || agent.Team == team;
			}
			return false;
		}

		// Token: 0x0600030B RID: 779 RVA: 0x0001AF28 File Offset: 0x00019128
		private void OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			foreach (Agent agent in this._agentMeshes.Keys)
			{
				this.UpdateVisibilityOfAgentMesh(agent);
			}
		}

		// Token: 0x0600030C RID: 780 RVA: 0x0001AF80 File Offset: 0x00019180
		private void HandleSpectateAgentFocusIn(Agent agent)
		{
			this.UpdateVisibilityOfAgentMesh(agent);
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0001AF89 File Offset: 0x00019189
		private void HandleSpectateAgentFocusOut(Agent agent)
		{
			this.UpdateVisibilityOfAgentMesh(agent);
		}

		// Token: 0x0600030E RID: 782 RVA: 0x0001AF94 File Offset: 0x00019194
		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType optionType)
		{
			if (optionType == 11)
			{
				foreach (Agent agent in base.Mission.Agents)
				{
					if (agent.IsHuman)
					{
						this.UpdateVisibilityOfAgentMesh(agent);
						if (this.IsMeshVisibleForAgent(agent))
						{
							this.OnUpdateOpacityValueOfAgentMesh(agent);
						}
					}
				}
			}
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0001B00C File Offset: 0x0001920C
		private bool IsAgentListeningToOrders(Agent agent)
		{
			if (this.IsOrderScreenVisible() && agent.Formation != null && this.IsAllyInAllyTeam(agent))
			{
				Func<Agent, bool> <>9__0;
				foreach (Formation formation in this.PlayerOrderController.SelectedFormations)
				{
					Func<Agent, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (Agent unit) => unit == agent);
					}
					if (formation.HasUnitsWithCondition(func))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0001B0BC File Offset: 0x000192BC
		private void UpdateSelectionVisibility(Agent agent, MetaMesh mesh, bool? visibility = null)
		{
			if (visibility == null)
			{
				visibility = new bool?(this.IsAgentListeningToOrders(agent));
			}
			float num = (visibility.Value ? 1f : (-1f));
			if (agent.MissionPeer == null)
			{
				float config = ManagedOptions.GetConfig(11);
				mesh.SetVectorArgument2(30f, 0.4f, 0.44f, num * config);
			}
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0001B11E File Offset: 0x0001931E
		private bool IsOrderScreenVisible()
		{
			return this.PlayerOrderController != null && base.MissionScreen.OrderFlag != null && base.MissionScreen.OrderFlag.IsVisible;
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0001B147 File Offset: 0x00019347
		private bool IsSiegeControllerScreenVisible()
		{
			return this.PlayerOrderController != null && base.MissionScreen.OrderFlag != null && base.MissionScreen.OrderFlag.IsVisible;
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0001B170 File Offset: 0x00019370
		private void SetHighlightForAgents(bool highlight, bool useSiegeMachineUsers, bool useAllTeamAgents)
		{
			if (this.PlayerOrderController == null)
			{
				bool flag = base.Mission.PlayerTeam == null;
				Debug.Print(string.Format("PlayerOrderController is null and playerTeamIsNull: {0}", flag), 0, 12, 17179869184UL);
			}
			if (useSiegeMachineUsers)
			{
				using (List<SiegeWeapon>.Enumerator enumerator = this.PlayerSiegeWeaponController.SelectedWeapons.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SiegeWeapon siegeWeapon = enumerator.Current;
						foreach (StandingPoint standingPoint in siegeWeapon.StandingPoints)
						{
							Agent userAgent = standingPoint.UserAgent;
							if (userAgent != null)
							{
								this.UpdateSelectionVisibility(userAgent, this._agentMeshes[userAgent], new bool?(highlight));
							}
						}
					}
					return;
				}
			}
			if (useAllTeamAgents)
			{
				if (this.PlayerOrderController.Owner == null)
				{
					return;
				}
				using (List<Agent>.Enumerator enumerator3 = this.PlayerOrderController.Owner.Team.ActiveAgents.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						Agent agent2 = enumerator3.Current;
						this.UpdateSelectionVisibility(agent2, this._agentMeshes[agent2], new bool?(highlight));
					}
					return;
				}
			}
			Action<Agent> <>9__0;
			foreach (Formation formation in this.PlayerOrderController.SelectedFormations)
			{
				Action<Agent> action;
				if ((action = <>9__0) == null)
				{
					action = (<>9__0 = delegate(Agent agent)
					{
						this.UpdateSelectionVisibility(agent, this._agentMeshes[agent], new bool?(highlight));
					});
				}
				formation.ApplyActionOnEachUnit(action, null);
			}
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0001B360 File Offset: 0x00019560
		private void DehighlightAllAgents()
		{
			foreach (KeyValuePair<Agent, MetaMesh> keyValuePair in this._agentMeshes)
			{
				this.UpdateSelectionVisibility(keyValuePair.Key, keyValuePair.Value, new bool?(false));
			}
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0001B3C8 File Offset: 0x000195C8
		public override void OnAgentTeamChanged(Team prevTeam, Team newTeam, Agent agent)
		{
			this.UpdateVisibilityOfAgentMesh(agent);
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0001B3D4 File Offset: 0x000195D4
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			foreach (Agent agent in base.Mission.Agents)
			{
				if (agent.IsHuman)
				{
					this.UpdateVisibilityOfAgentMesh(agent);
				}
			}
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0001B43C File Offset: 0x0001963C
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			foreach (Agent agent in base.Mission.Agents)
			{
				if (agent.IsHuman)
				{
					this.UpdateVisibilityOfAgentMesh(agent);
				}
			}
		}

		// Token: 0x04000217 RID: 535
		private const float _highlightedLabelScaleFactor = 30f;

		// Token: 0x04000218 RID: 536
		private const float _labelBannerWidth = 0.4f;

		// Token: 0x04000219 RID: 537
		private const float _labelBlackBorderWidth = 0.44f;

		// Token: 0x0400021A RID: 538
		private readonly Dictionary<Agent, MetaMesh> _agentMeshes;

		// Token: 0x0400021B RID: 539
		private readonly Dictionary<Texture, Material> _labelMaterials;

		// Token: 0x0400021C RID: 540
		private bool _wasOrderScreenVisible;

		// Token: 0x0400021D RID: 541
		private bool _wasSiegeControllerScreenVisible;
	}
}
