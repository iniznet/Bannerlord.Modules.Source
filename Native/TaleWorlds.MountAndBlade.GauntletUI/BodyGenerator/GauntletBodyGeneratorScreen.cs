using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator
{
	// Token: 0x0200004A RID: 74
	[OverrideView(typeof(FaceGeneratorScreen))]
	public class GauntletBodyGeneratorScreen : ScreenBase, IFaceGeneratorScreen
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600038A RID: 906 RVA: 0x00014ECF File Offset: 0x000130CF
		public IFaceGeneratorHandler Handler
		{
			get
			{
				return this._facegenLayer;
			}
		}

		// Token: 0x0600038B RID: 907 RVA: 0x00014ED8 File Offset: 0x000130D8
		public GauntletBodyGeneratorScreen(BasicCharacterObject character, bool openedFromMultiplayer, IFaceGeneratorCustomFilter filter)
		{
			this._facegenLayer = new BodyGeneratorView(new ControlCharacterCreationStage(this.OnExit), GameTexts.FindText("str_done", null), new ControlCharacterCreationStage(this.OnExit), GameTexts.FindText("str_cancel", null), character, openedFromMultiplayer, filter, null, null, null, null, null);
		}

		// Token: 0x0600038C RID: 908 RVA: 0x00014F2C File Offset: 0x0001312C
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this._facegenLayer.OnTick(dt);
		}

		// Token: 0x0600038D RID: 909 RVA: 0x00014F41 File Offset: 0x00013141
		public void OnExit()
		{
			ScreenManager.PopScreen();
		}

		// Token: 0x0600038E RID: 910 RVA: 0x00014F48 File Offset: 0x00013148
		protected override void OnInitialize()
		{
			base.OnInitialize();
			Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
			base.AddLayer(this._facegenLayer.GauntletLayer);
		}

		// Token: 0x0600038F RID: 911 RVA: 0x00014F71 File Offset: 0x00013171
		protected override void OnFinalize()
		{
			base.OnFinalize();
			if (LoadingWindow.GetGlobalLoadingWindowState())
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}
			Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
		}

		// Token: 0x06000390 RID: 912 RVA: 0x00014F95 File Offset: 0x00013195
		protected override void OnActivate()
		{
			base.OnActivate();
			base.AddLayer(this._facegenLayer.SceneLayer);
		}

		// Token: 0x06000391 RID: 913 RVA: 0x00014FB0 File Offset: 0x000131B0
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
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

		// Token: 0x040001FC RID: 508
		private const int ViewOrderPriority = 15;

		// Token: 0x040001FD RID: 509
		private readonly BodyGeneratorView _facegenLayer;
	}
}
