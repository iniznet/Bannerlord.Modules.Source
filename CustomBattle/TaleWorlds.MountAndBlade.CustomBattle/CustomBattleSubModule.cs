using System;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI.SceneNotification;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	public class CustomBattleSubModule : MBSubModuleBase
	{
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("CustomBattle", new TextObject("{=4gOGGbeQ}Custom Battle", null), 5000, delegate
			{
				MBGameManager.StartNewGame(new CustomGameManager());
			}, () => new ValueTuple<bool, TextObject>(false, null)));
		}

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();
			if (!this._initialized)
			{
				if (!Utilities.CommandLineArgumentExists("VisualTests"))
				{
					GauntletSceneNotification.Current.RegisterContextProvider(new CustomBattleSceneNotificationContextProvider());
				}
				this._initialized = true;
			}
		}

		private bool _initialized;
	}
}
