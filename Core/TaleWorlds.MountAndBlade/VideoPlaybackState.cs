﻿using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class VideoPlaybackState : GameState
	{
		public string VideoPath { get; private set; }

		public string AudioPath { get; private set; }

		public float FrameRate { get; private set; }

		public string SubtitleFileBasePath { get; private set; }

		public bool CanUserSkip { get; private set; }

		public void SetStartingParameters(string videoPath, string audioPath, string subtitleFileBasePath, float frameRate = 30f, bool canUserSkip = true)
		{
			this.VideoPath = videoPath;
			this.AudioPath = audioPath;
			this.FrameRate = frameRate;
			this.SubtitleFileBasePath = subtitleFileBasePath;
			this.CanUserSkip = canUserSkip;
		}

		public void SetOnVideoFinisedDelegate(Action onVideoFinised)
		{
			this._onVideoFinised = onVideoFinised;
		}

		public void OnVideoFinished()
		{
			Action onVideoFinised = this._onVideoFinised;
			if (onVideoFinised == null)
			{
				return;
			}
			onVideoFinised();
		}

		private Action _onVideoFinised;
	}
}
