using System;

namespace psai.Editor
{
	[Serializable]
	public class ProjectProperties : ICloneable
	{
		public int WarningThresholdPreBeatMillis { get; set; }

		public bool DefaultCalculatePostAndPrebeatLengthBasedOnBeats { get; set; }

		public int DefaultSegmentSuitabilites { get; set; }

		public ProjectProperties()
		{
			this.DefaultBpm = 100f;
			this.DefaultPostbeats = 4f;
			this.DefaultPrebeats = 1f;
			this.WarningThresholdPreBeatMillis = 1500;
			this.DefaultSegmentSuitabilites = 3;
			this.ForceFullRebuild = true;
		}

		public bool ForceFullRebuild { get; set; }

		public float VolumeBoost
		{
			get
			{
				return this._volumeBoost;
			}
			set
			{
				if (value >= 0f && value <= 600f)
				{
					this._volumeBoost = value;
					return;
				}
				Console.Out.WriteLine("invalid value for VolumeBoost");
			}
		}

		public int ExportSoundQualityInPercent
		{
			get
			{
				return this._exportSoundQualityInPercent;
			}
			set
			{
				if (value >= 1 && value <= 100)
				{
					this._exportSoundQualityInPercent = value;
				}
			}
		}

		public float DefaultPrebeats { get; set; }

		public float DefaultPostbeats { get; set; }

		public float DefaultBpm { get; set; }

		public int DefaultPrebeatLengthInSamples { get; set; }

		public int DefaultPostbeatLengthInSamples { get; set; }

		public ProjectProperties ShallowCopy()
		{
			return (ProjectProperties)base.MemberwiseClone();
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}

		private float _volumeBoost;

		private int _exportSoundQualityInPercent = 100;
	}
}
