using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard
{
	public class MultiplayerScoreboardStatsListPanel : ListPanel
	{
		public MultiplayerScoreboardStatsListPanel(UIContext context)
			: base(context)
		{
			this._nameColumnItemDescription = new ContainerItemDescription
			{
				WidgetId = "name"
			};
			this._scoreColumnItemDescription = new ContainerItemDescription
			{
				WidgetId = "score"
			};
			this._soldiersColumnItemDescription = new ContainerItemDescription
			{
				WidgetId = "soldiers"
			};
		}

		private void NameColumnWidthRatioUpdated()
		{
			this._nameColumnItemDescription.WidthStretchRatio = this.NameColumnWidthRatio;
			base.AddItemDescription(this._nameColumnItemDescription);
			base.SetMeasureAndLayoutDirty();
		}

		private void ScoreColumnWidthRatioUpdated()
		{
			this._scoreColumnItemDescription.WidthStretchRatio = this.ScoreColumnWidthRatio;
			base.AddItemDescription(this._scoreColumnItemDescription);
			base.SetMeasureAndLayoutDirty();
		}

		private void SoldiersColumnWidthRatioUpdated()
		{
			this._soldiersColumnItemDescription.WidthStretchRatio = this.SoldiersColumnWidthRatio;
			base.AddItemDescription(this._soldiersColumnItemDescription);
			base.SetMeasureAndLayoutDirty();
		}

		public float NameColumnWidthRatio
		{
			get
			{
				return this._nameColumnWidthRatio;
			}
			set
			{
				if (value != this._nameColumnWidthRatio)
				{
					this._nameColumnWidthRatio = value;
					base.OnPropertyChanged(value, "NameColumnWidthRatio");
					this.NameColumnWidthRatioUpdated();
				}
			}
		}

		public float ScoreColumnWidthRatio
		{
			get
			{
				return this._scoreColumnWidthRatio;
			}
			set
			{
				if (value != this._scoreColumnWidthRatio)
				{
					this._scoreColumnWidthRatio = value;
					base.OnPropertyChanged(value, "ScoreColumnWidthRatio");
					this.ScoreColumnWidthRatioUpdated();
				}
			}
		}

		public float SoldiersColumnWidthRatio
		{
			get
			{
				return this._soldiersColumnWidthRatio;
			}
			set
			{
				if (value != this._soldiersColumnWidthRatio)
				{
					this._soldiersColumnWidthRatio = value;
					base.OnPropertyChanged(value, "SoldiersColumnWidthRatio");
					this.SoldiersColumnWidthRatioUpdated();
				}
			}
		}

		private ContainerItemDescription _nameColumnItemDescription;

		private ContainerItemDescription _scoreColumnItemDescription;

		private ContainerItemDescription _soldiersColumnItemDescription;

		private const string _nameColumnWidgetID = "name";

		private const string _scoreColumnWidgetID = "score";

		private const string _soldiersColumnWidgetID = "soldiers";

		private float _nameColumnWidthRatio = 1f;

		private float _scoreColumnWidthRatio = 1f;

		private float _soldiersColumnWidthRatio = 1f;
	}
}
