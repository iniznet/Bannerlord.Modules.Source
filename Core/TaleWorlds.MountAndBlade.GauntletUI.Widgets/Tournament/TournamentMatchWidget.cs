using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tournament
{
	public class TournamentMatchWidget : Widget
	{
		public TournamentMatchWidget(UIContext context)
			: base(context)
		{
		}

		[Editor(false)]
		public int State
		{
			get
			{
				return this._state;
			}
			set
			{
				if (this._state != value)
				{
					this._state = value;
					foreach (Widget widget in base.AllChildren)
					{
						TournamentParticipantBrushWidget tournamentParticipantBrushWidget = widget as TournamentParticipantBrushWidget;
						if (tournamentParticipantBrushWidget != null)
						{
							tournamentParticipantBrushWidget.MatchState = this.State;
						}
					}
				}
			}
		}

		private int _state;
	}
}
