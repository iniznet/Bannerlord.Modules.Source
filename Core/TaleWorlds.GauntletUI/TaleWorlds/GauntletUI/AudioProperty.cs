using System;

namespace TaleWorlds.GauntletUI
{
	public class AudioProperty
	{
		[Editor(false)]
		public string AudioName { get; set; }

		[Editor(false)]
		public bool Delay { get; set; }

		[Editor(false)]
		public float DelaySeconds { get; set; }

		public void FillFrom(AudioProperty audioProperty)
		{
			this.AudioName = audioProperty.AudioName;
			this.Delay = audioProperty.Delay;
			this.DelaySeconds = audioProperty.DelaySeconds;
		}
	}
}
