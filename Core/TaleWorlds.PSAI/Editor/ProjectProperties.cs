using System;

namespace psai.Editor
{
	// Token: 0x02000009 RID: 9
	[Serializable]
	public class ProjectProperties : ICloneable
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000084 RID: 132 RVA: 0x0000479E File Offset: 0x0000299E
		// (set) Token: 0x06000085 RID: 133 RVA: 0x000047A6 File Offset: 0x000029A6
		public int WarningThresholdPreBeatMillis { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000086 RID: 134 RVA: 0x000047AF File Offset: 0x000029AF
		// (set) Token: 0x06000087 RID: 135 RVA: 0x000047B7 File Offset: 0x000029B7
		public bool DefaultCalculatePostAndPrebeatLengthBasedOnBeats { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000088 RID: 136 RVA: 0x000047C0 File Offset: 0x000029C0
		// (set) Token: 0x06000089 RID: 137 RVA: 0x000047C8 File Offset: 0x000029C8
		public int DefaultSegmentSuitabilites { get; set; }

		// Token: 0x0600008A RID: 138 RVA: 0x000047D4 File Offset: 0x000029D4
		public ProjectProperties()
		{
			this.DefaultBpm = 100f;
			this.DefaultPostbeats = 4f;
			this.DefaultPrebeats = 1f;
			this.WarningThresholdPreBeatMillis = 1500;
			this.DefaultSegmentSuitabilites = 3;
			this.ForceFullRebuild = true;
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600008B RID: 139 RVA: 0x00004829 File Offset: 0x00002A29
		// (set) Token: 0x0600008C RID: 140 RVA: 0x00004831 File Offset: 0x00002A31
		public bool ForceFullRebuild { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600008D RID: 141 RVA: 0x0000483A File Offset: 0x00002A3A
		// (set) Token: 0x0600008E RID: 142 RVA: 0x00004842 File Offset: 0x00002A42
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

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600008F RID: 143 RVA: 0x0000486B File Offset: 0x00002A6B
		// (set) Token: 0x06000090 RID: 144 RVA: 0x00004873 File Offset: 0x00002A73
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

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000091 RID: 145 RVA: 0x00004885 File Offset: 0x00002A85
		// (set) Token: 0x06000092 RID: 146 RVA: 0x0000488D File Offset: 0x00002A8D
		public float DefaultPrebeats { get; set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000093 RID: 147 RVA: 0x00004896 File Offset: 0x00002A96
		// (set) Token: 0x06000094 RID: 148 RVA: 0x0000489E File Offset: 0x00002A9E
		public float DefaultPostbeats { get; set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000095 RID: 149 RVA: 0x000048A7 File Offset: 0x00002AA7
		// (set) Token: 0x06000096 RID: 150 RVA: 0x000048AF File Offset: 0x00002AAF
		public float DefaultBpm { get; set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000097 RID: 151 RVA: 0x000048B8 File Offset: 0x00002AB8
		// (set) Token: 0x06000098 RID: 152 RVA: 0x000048C0 File Offset: 0x00002AC0
		public int DefaultPrebeatLengthInSamples { get; set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000099 RID: 153 RVA: 0x000048C9 File Offset: 0x00002AC9
		// (set) Token: 0x0600009A RID: 154 RVA: 0x000048D1 File Offset: 0x00002AD1
		public int DefaultPostbeatLengthInSamples { get; set; }

		// Token: 0x0600009B RID: 155 RVA: 0x000048DA File Offset: 0x00002ADA
		public ProjectProperties ShallowCopy()
		{
			return (ProjectProperties)base.MemberwiseClone();
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000048E7 File Offset: 0x00002AE7
		public object Clone()
		{
			return base.MemberwiseClone();
		}

		// Token: 0x04000039 RID: 57
		private float _volumeBoost;

		// Token: 0x0400003A RID: 58
		private int _exportSoundQualityInPercent = 100;
	}
}
