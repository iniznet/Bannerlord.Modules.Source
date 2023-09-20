using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000224 RID: 548
	public class GameLoadingState : GameState
	{
		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x06001E1A RID: 7706 RVA: 0x0006CAF8 File Offset: 0x0006ACF8
		public override bool IsMusicMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001E1C RID: 7708 RVA: 0x0006CB03 File Offset: 0x0006AD03
		public void SetLoadingParameters(MBGameManager gameLoader)
		{
			Game.OnGameCreated += this.OnGameCreated;
			this._gameLoader = gameLoader;
		}

		// Token: 0x06001E1D RID: 7709 RVA: 0x0006CB1D File Offset: 0x0006AD1D
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (!this._loadingFinished)
			{
				this._loadingFinished = this._gameLoader.DoLoadingForGameManager();
				return;
			}
			GameStateManager.Current = Game.Current.GameStateManager;
			this._gameLoader.OnLoadFinished();
		}

		// Token: 0x06001E1E RID: 7710 RVA: 0x0006CB5A File Offset: 0x0006AD5A
		private void OnGameCreated()
		{
			Game.OnGameCreated -= this.OnGameCreated;
			Game.Current.OnItemDeserializedEvent += delegate(ItemObject itemObject)
			{
				if (itemObject.Type == ItemObject.ItemTypeEnum.HandArmor)
				{
					Utilities.RegisterMeshForGPUMorph(itemObject.MultiMeshName);
				}
			};
		}

		// Token: 0x04000B20 RID: 2848
		private bool _loadingFinished;

		// Token: 0x04000B21 RID: 2849
		private MBGameManager _gameLoader;
	}
}
