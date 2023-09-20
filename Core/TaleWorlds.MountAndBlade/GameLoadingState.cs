using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class GameLoadingState : GameState
	{
		public override bool IsMusicMenuState
		{
			get
			{
				return true;
			}
		}

		public void SetLoadingParameters(MBGameManager gameLoader)
		{
			Game.OnGameCreated += this.OnGameCreated;
			this._gameLoader = gameLoader;
		}

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

		private bool _loadingFinished;

		private MBGameManager _gameLoader;
	}
}
