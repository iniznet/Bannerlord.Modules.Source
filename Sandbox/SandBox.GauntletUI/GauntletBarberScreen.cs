using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI
{
	// Token: 0x02000004 RID: 4
	[GameStateScreen(typeof(BarberState))]
	public class GauntletBarberScreen : ScreenBase, IGameStateListener, IFaceGeneratorScreen
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002058 File Offset: 0x00000258
		public IFaceGeneratorHandler Handler
		{
			get
			{
				return this._facegenLayer;
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002060 File Offset: 0x00000260
		public GauntletBarberScreen(BarberState state)
		{
			LoadingWindow.EnableGlobalLoadingWindow();
			this._facegenLayer = new BodyGeneratorView(new ControlCharacterCreationStage(this.OnExit), GameTexts.FindText("str_done", null), new ControlCharacterCreationStage(this.OnExit), GameTexts.FindText("str_cancel", null), Hero.MainHero.CharacterObject, false, state.Filter, null, null, null, null, null);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020C7 File Offset: 0x000002C7
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this._facegenLayer.OnTick(dt);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020DC File Offset: 0x000002DC
		public void OnExit()
		{
			Game.Current.GameStateManager.PopState(0);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020EE File Offset: 0x000002EE
		protected override void OnInitialize()
		{
			base.OnInitialize();
			Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
			base.AddLayer(this._facegenLayer.GauntletLayer);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002117 File Offset: 0x00000317
		protected override void OnFinalize()
		{
			base.OnFinalize();
			if (LoadingWindow.GetGlobalLoadingWindowState())
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}
			Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000213B File Offset: 0x0000033B
		protected override void OnActivate()
		{
			base.OnActivate();
			base.AddLayer(this._facegenLayer.SceneLayer);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002154 File Offset: 0x00000354
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this._facegenLayer.SceneLayer.SceneView.SetEnable(false);
			this._facegenLayer.OnFinalize();
			LoadingWindow.EnableGlobalLoadingWindow();
			MBInformationManager.HideInformations();
			Mission mission = Mission.Current;
			if (mission != null)
			{
				foreach (Agent agent in mission.Agents)
				{
					agent.EquipItemsFromSpawnEquipment(false);
					agent.UpdateAgentProperties();
				}
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000021E8 File Offset: 0x000003E8
		void IGameStateListener.OnActivate()
		{
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000021EA File Offset: 0x000003EA
		void IGameStateListener.OnDeactivate()
		{
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000021EC File Offset: 0x000003EC
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000021EE File Offset: 0x000003EE
		void IGameStateListener.OnFinalize()
		{
		}

		// Token: 0x04000001 RID: 1
		private readonly BodyGeneratorView _facegenLayer;
	}
}
