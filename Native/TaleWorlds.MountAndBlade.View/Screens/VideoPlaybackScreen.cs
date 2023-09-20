using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	public class VideoPlaybackScreen : ScreenBase, IGameStateListener
	{
		public VideoPlaybackScreen(VideoPlaybackState videoPlaybackState)
		{
			this._videoPlaybackState = videoPlaybackState;
			this._videoPlayerView = VideoPlayerView.CreateVideoPlayerView();
			this._videoPlayerView.SetRenderOrder(-10000);
		}

		protected sealed override void OnFrameTick(float dt)
		{
			this._totalElapsedTimeSinceVideoStart += dt;
			base.OnFrameTick(dt);
			if (this._videoPlaybackState.CanUserSkip && (Input.IsKeyReleased(1) || Input.IsKeyReleased(251)))
			{
				this._videoPlayerView.StopVideo();
			}
			if (this._videoPlayerView.IsVideoFinished())
			{
				this._videoPlaybackState.OnVideoFinished();
				this._videoPlayerView.SetEnable(false);
				this._videoPlayerView = null;
			}
			if (ScreenManager.TopScreen == this)
			{
				this.OnVideoPlaybackTick(dt);
			}
		}

		protected virtual void OnVideoPlaybackTick(float dt)
		{
		}

		void IGameStateListener.OnInitialize()
		{
			this._videoPlayerView.PlayVideo(this._videoPlaybackState.VideoPath, this._videoPlaybackState.AudioPath, this._videoPlaybackState.FrameRate);
			LoadingWindow.DisableGlobalLoadingWindow();
			Utilities.DisableGlobalLoadingWindow();
		}

		void IGameStateListener.OnFinalize()
		{
			this._videoPlayerView.FinalizePlayer();
		}

		void IGameStateListener.OnActivate()
		{
			base.OnActivate();
		}

		void IGameStateListener.OnDeactivate()
		{
			base.OnDeactivate();
		}

		protected VideoPlaybackState _videoPlaybackState;

		protected VideoPlayerView _videoPlayerView;

		protected float _totalElapsedTimeSinceVideoStart;
	}
}
