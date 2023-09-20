using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator
{
	[OverrideView(typeof(FaceGeneratorScreen))]
	public class GauntletBodyGeneratorScreen : ScreenBase, IFaceGeneratorScreen
	{
		public IFaceGeneratorHandler Handler
		{
			get
			{
				return this._facegenLayer;
			}
		}

		public GauntletBodyGeneratorScreen(BasicCharacterObject character, bool openedFromMultiplayer, IFaceGeneratorCustomFilter filter)
		{
			this._facegenLayer = new BodyGeneratorView(new ControlCharacterCreationStage(this.OnExit), GameTexts.FindText("str_done", null), new ControlCharacterCreationStage(this.OnExit), GameTexts.FindText("str_cancel", null), character, openedFromMultiplayer, filter, null, null, null, null, null);
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this._facegenLayer.OnTick(dt);
		}

		public void OnExit()
		{
			ScreenManager.PopScreen();
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
			base.AddLayer(this._facegenLayer.GauntletLayer);
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			if (LoadingWindow.GetGlobalLoadingWindowState())
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}
			Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			base.AddLayer(this._facegenLayer.SceneLayer);
		}

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

		private const int ViewOrderPriority = 15;

		private readonly BodyGeneratorView _facegenLayer;
	}
}
