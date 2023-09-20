using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	public class ReloadPhaseItemVM : ViewModel
	{
		public ReloadPhaseItemVM(float progress, float relativeDurationToMaxDuration)
		{
			this.Update(progress, relativeDurationToMaxDuration);
		}

		public void Update(float progress, float relativeDurationToMaxDuration)
		{
			this.Progress = progress;
			this.RelativeDurationToMaxDuration = relativeDurationToMaxDuration;
		}

		[DataSourceProperty]
		public float Progress
		{
			get
			{
				return this._progress;
			}
			set
			{
				if (value != this._progress)
				{
					this._progress = value;
					base.OnPropertyChangedWithValue(value, "Progress");
				}
			}
		}

		[DataSourceProperty]
		public float RelativeDurationToMaxDuration
		{
			get
			{
				return this._relativeDurationToMaxDuration;
			}
			set
			{
				if (value != this._relativeDurationToMaxDuration)
				{
					this._relativeDurationToMaxDuration = value;
					base.OnPropertyChangedWithValue(value, "RelativeDurationToMaxDuration");
				}
			}
		}

		private float _progress;

		private float _relativeDurationToMaxDuration;
	}
}
