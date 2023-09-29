using System;
using System.Collections.Generic;
using System.ComponentModel;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	public class MissionAgentLabelView : MissionView
	{
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

		public MissionAgentLabelView()
		{
			this._agentMeshes = new Dictionary<Agent, MetaMesh>();
			this._labelMaterials = new Dictionary<Texture, Material>();
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			base.Mission.Teams.OnPlayerTeamChanged += this.Mission_OnPlayerTeamChanged;
			base.Mission.OnMainAgentChanged += this.OnMainAgentChanged;
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			base.MissionScreen.OnSpectateAgentFocusIn += this.HandleSpectateAgentFocusIn;
			base.MissionScreen.OnSpectateAgentFocusOut += this.HandleSpectateAgentFocusOut;
		}

		public override void AfterStart()
		{
			if (this.PlayerOrderController != null)
			{
				this.PlayerOrderController.OnSelectedFormationsChanged += this.OrderController_OnSelectedFormationsChanged;
				base.Mission.PlayerTeam.OnFormationsChanged += this.PlayerTeam_OnFormationsChanged;
			}
			BannerBearerLogic missionBehavior = base.Mission.GetMissionBehavior<BannerBearerLogic>();
			if (missionBehavior != null)
			{
				missionBehavior.OnBannerBearerAgentUpdated += this.BannerBearerLogic_OnBannerBearerAgentUpdated;
			}
		}

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

		public override void OnRemoveBehavior()
		{
			this.UnregisterEvents();
			base.OnRemoveBehavior();
		}

		public override void OnMissionScreenFinalize()
		{
			this.UnregisterEvents();
			base.OnMissionScreenFinalize();
		}

		private void UnregisterEvents()
		{
			if (base.Mission != null)
			{
				base.Mission.Teams.OnPlayerTeamChanged -= this.Mission_OnPlayerTeamChanged;
				base.Mission.OnMainAgentChanged -= this.OnMainAgentChanged;
			}
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			if (base.MissionScreen != null)
			{
				base.MissionScreen.OnSpectateAgentFocusIn -= this.HandleSpectateAgentFocusIn;
				base.MissionScreen.OnSpectateAgentFocusOut -= this.HandleSpectateAgentFocusOut;
			}
			if (this.PlayerOrderController != null)
			{
				this.PlayerOrderController.OnSelectedFormationsChanged -= this.OrderController_OnSelectedFormationsChanged;
				if (base.Mission != null)
				{
					base.Mission.PlayerTeam.OnFormationsChanged -= this.PlayerTeam_OnFormationsChanged;
				}
			}
			BannerBearerLogic missionBehavior = base.Mission.GetMissionBehavior<BannerBearerLogic>();
			if (missionBehavior != null)
			{
				missionBehavior.OnBannerBearerAgentUpdated -= this.BannerBearerLogic_OnBannerBearerAgentUpdated;
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			this.RemoveAgentLabel(affectedAgent);
		}

		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			this.InitAgentLabel(agent, banner);
		}

		public override void OnAssignPlayerAsSergeantOfFormation(Agent agent)
		{
			float friendlyTroopsBannerOpacity = BannerlordConfig.FriendlyTroopsBannerOpacity;
			this._agentMeshes[agent].SetVectorArgument2(30f, 0.4f, 0.44f, 1f * friendlyTroopsBannerOpacity);
		}

		public override void OnClearScene()
		{
			this._agentMeshes.Clear();
			this._labelMaterials.Clear();
		}

		private void PlayerTeam_OnFormationsChanged(Team team, Formation formation)
		{
			if (this.IsOrderScreenVisible())
			{
				this.DehighlightAllAgents();
				this.SetHighlightForAgents(true, false, false);
			}
		}

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

		private void OrderController_OnSelectedFormationsChanged()
		{
			this.DehighlightAllAgents();
			if (this.IsOrderScreenVisible())
			{
				this.SetHighlightForAgents(true, false, false);
			}
		}

		private void PlayerSiegeWeaponController_OnSelectedSiegeWeaponsChanged()
		{
			this.DehighlightAllAgents();
			this.SetHighlightForAgents(true, true, false);
		}

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

		private void BannerBearerLogic_OnBannerBearerAgentUpdated(Agent agent, bool isBannerBearer)
		{
			this.RemoveAgentLabel(agent);
			this.InitAgentLabel(agent, null);
		}

		private void RemoveAgentLabel(Agent agent)
		{
			if (agent.IsHuman && this._agentMeshes.ContainsKey(agent))
			{
				if (agent.AgentVisuals != null)
				{
					agent.AgentVisuals.ReplaceMeshWithMesh(this._agentMeshes[agent], null, 13);
				}
				this._agentMeshes.Remove(agent);
			}
		}

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

		private void UpdateVisibilityOfAgentMesh(Agent agent)
		{
			if (agent.IsActive() && this._agentMeshes.ContainsKey(agent))
			{
				bool flag = this.IsMeshVisibleForAgent(agent);
				this._agentMeshes[agent].SetVisibilityMask(flag ? 1 : 0);
			}
		}

		private bool IsMeshVisibleForAgent(Agent agent)
		{
			return this.IsAllyInAllyTeam(agent) && base.MissionScreen.LastFollowedAgent != agent && BannerlordConfig.FriendlyTroopsBannerOpacity > 0f && !base.MissionScreen.IsPhotoModeEnabled;
		}

		private void OnUpdateOpacityValueOfAgentMesh(Agent agent)
		{
			if (agent.IsActive() && this._agentMeshes.ContainsKey(agent))
			{
				this._agentMeshes[agent].SetVectorArgument2(30f, 0.4f, 0.44f, -BannerlordConfig.FriendlyTroopsBannerOpacity);
			}
		}

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

		private void OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			foreach (Agent agent in this._agentMeshes.Keys)
			{
				this.UpdateVisibilityOfAgentMesh(agent);
			}
		}

		private void HandleSpectateAgentFocusIn(Agent agent)
		{
			this.UpdateVisibilityOfAgentMesh(agent);
		}

		private void HandleSpectateAgentFocusOut(Agent agent)
		{
			this.UpdateVisibilityOfAgentMesh(agent);
		}

		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType optionType)
		{
			if (optionType == 12)
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

		private void UpdateSelectionVisibility(Agent agent, MetaMesh mesh, bool? visibility = null)
		{
			if (visibility == null)
			{
				visibility = new bool?(this.IsAgentListeningToOrders(agent));
			}
			float num = (visibility.Value ? 1f : (-1f));
			if (agent.MissionPeer == null)
			{
				float config = ManagedOptions.GetConfig(12);
				mesh.SetVectorArgument2(30f, 0.4f, 0.44f, num * config);
			}
		}

		private bool IsOrderScreenVisible()
		{
			return this.PlayerOrderController != null && base.MissionScreen.OrderFlag != null && base.MissionScreen.OrderFlag.IsVisible;
		}

		private bool IsSiegeControllerScreenVisible()
		{
			return this.PlayerOrderController != null && base.MissionScreen.OrderFlag != null && base.MissionScreen.OrderFlag.IsVisible;
		}

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

		private void DehighlightAllAgents()
		{
			foreach (KeyValuePair<Agent, MetaMesh> keyValuePair in this._agentMeshes)
			{
				this.UpdateSelectionVisibility(keyValuePair.Key, keyValuePair.Value, new bool?(false));
			}
		}

		public override void OnAgentTeamChanged(Team prevTeam, Team newTeam, Agent agent)
		{
			this.UpdateVisibilityOfAgentMesh(agent);
		}

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

		private const float _highlightedLabelScaleFactor = 30f;

		private const float _labelBannerWidth = 0.4f;

		private const float _labelBlackBorderWidth = 0.44f;

		private readonly Dictionary<Agent, MetaMesh> _agentMeshes;

		private readonly Dictionary<Texture, Material> _labelMaterials;

		private bool _wasOrderScreenVisible;

		private bool _wasSiegeControllerScreenVisible;
	}
}
