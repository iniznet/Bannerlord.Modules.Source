using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard
{
	public class MultiplayerScoreboardSideWidget : Widget
	{
		public MultiplayerScoreboardSideWidget(UIContext context)
			: base(context)
		{
			this._nameColumnItemDescription = new ContainerItemDescription();
			this._nameColumnItemDescription.WidgetIndex = 3;
		}

		private void AvatarColumnWidthRatioUpdated()
		{
			if (this.TitlesListPanel == null)
			{
				return;
			}
			this._nameColumnItemDescription.WidthStretchRatio = this.NameColumnWidthRatio;
			this.TitlesListPanel.AddItemDescription(this._nameColumnItemDescription);
		}

		private void UpdateBackgroundColors()
		{
			if (string.IsNullOrEmpty(this.CultureId))
			{
				return;
			}
			base.Color = this.CultureColor;
		}

		public Color CultureColor
		{
			get
			{
				return this._cultureColor;
			}
			set
			{
				if (value != this._cultureColor)
				{
					this._cultureColor = value;
					base.OnPropertyChanged(value, "CultureColor");
					this.UpdateBackgroundColors();
				}
			}
		}

		public string CultureId
		{
			get
			{
				return this._cultureId;
			}
			set
			{
				if (value != this._cultureId)
				{
					this._cultureId = value;
					base.OnPropertyChanged<string>(value, "CultureId");
					this.UpdateBackgroundColors();
				}
			}
		}

		public bool UseSecondary
		{
			get
			{
				return this._useSecondary;
			}
			set
			{
				if (value != this._useSecondary)
				{
					this._useSecondary = value;
					base.OnPropertyChanged(value, "UseSecondary");
					this.UpdateBackgroundColors();
				}
			}
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
					this.AvatarColumnWidthRatioUpdated();
				}
			}
		}

		public ListPanel TitlesListPanel
		{
			get
			{
				return this._titlesListPanel;
			}
			set
			{
				if (value != this._titlesListPanel)
				{
					this._titlesListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "TitlesListPanel");
					this.AvatarColumnWidthRatioUpdated();
				}
			}
		}

		private ContainerItemDescription _nameColumnItemDescription;

		private float _nameColumnWidthRatio = 1f;

		private ListPanel _titlesListPanel;

		private string _cultureId;

		private Color _cultureColor;

		private bool _useSecondary;
	}
}
