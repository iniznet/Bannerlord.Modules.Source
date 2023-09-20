﻿using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.PlatformService;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade
{
	public class MBProfileSelectionScreenBase : ScreenBase, IGameStateListener
	{
		public MBProfileSelectionScreenBase(ProfileSelectionState state)
		{
			this._state = state;
		}

		protected sealed override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (ScreenManager.TopScreen == this)
			{
				this.OnProfileSelectionTick(dt);
			}
		}

		protected virtual void OnProfileSelectionTick(float dt)
		{
		}

		protected void OnActivateProfileSelection()
		{
			PlatformServices.Instance.LoginUser();
		}

		void IGameStateListener.OnActivate()
		{
		}

		void IGameStateListener.OnDeactivate()
		{
		}

		void IGameStateListener.OnFinalize()
		{
		}

		void IGameStateListener.OnInitialize()
		{
			Utilities.DisableGlobalLoadingWindow();
			LoadingWindow.DisableGlobalLoadingWindow();
		}

		private ProfileSelectionState _state;
	}
}
