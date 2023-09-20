using System;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI.SceneNotification;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	// Token: 0x0200000D RID: 13
	public class CustomBattleSubModule : MBSubModuleBase
	{
		// Token: 0x060000B5 RID: 181 RVA: 0x00007458 File Offset: 0x00005658
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("CustomBattle", new TextObject("{=4gOGGbeQ}Custom Battle", null), 5000, delegate
			{
				MBGameManager.StartNewGame(new CustomGameManager());
			}, () => new ValueTuple<bool, TextObject>(false, null)));
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000074CD File Offset: 0x000056CD
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

		// Token: 0x0400006D RID: 109
		private bool _initialized;
	}
}
