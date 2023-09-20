using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	// Token: 0x02000034 RID: 52
	public class VideoPlaybackScreen : ScreenBase, IGameStateListener
	{
		// Token: 0x06000263 RID: 611 RVA: 0x000163C6 File Offset: 0x000145C6
		public VideoPlaybackScreen(VideoPlaybackState videoPlaybackState)
		{
			this._videoPlaybackState = videoPlaybackState;
			this._videoPlayerView = VideoPlayerView.CreateVideoPlayerView();
			this._videoPlayerView.SetRenderOrder(-10000);
		}

		// Token: 0x06000264 RID: 612 RVA: 0x000163F0 File Offset: 0x000145F0
		protected override void OnFrameTick(float dt)
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
		}

		// Token: 0x06000265 RID: 613 RVA: 0x00016469 File Offset: 0x00014669
		void IGameStateListener.OnInitialize()
		{
			this._videoPlayerView.PlayVideo(this._videoPlaybackState.VideoPath, this._videoPlaybackState.AudioPath, this._videoPlaybackState.FrameRate);
			LoadingWindow.DisableGlobalLoadingWindow();
			Utilities.DisableGlobalLoadingWindow();
		}

		// Token: 0x06000266 RID: 614 RVA: 0x000164A1 File Offset: 0x000146A1
		void IGameStateListener.OnFinalize()
		{
			this._videoPlayerView.FinalizePlayer();
		}

		// Token: 0x06000267 RID: 615 RVA: 0x000164AE File Offset: 0x000146AE
		void IGameStateListener.OnActivate()
		{
			base.OnActivate();
		}

		// Token: 0x06000268 RID: 616 RVA: 0x000164B6 File Offset: 0x000146B6
		void IGameStateListener.OnDeactivate()
		{
			base.OnDeactivate();
		}

		// Token: 0x0400017D RID: 381
		protected VideoPlaybackState _videoPlaybackState;

		// Token: 0x0400017E RID: 382
		protected VideoPlayerView _videoPlayerView;

		// Token: 0x0400017F RID: 383
		protected float _totalElapsedTimeSinceVideoStart;
	}
}
