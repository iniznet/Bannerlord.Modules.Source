using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tournament
{
	// Token: 0x02000046 RID: 70
	public class TournamentMatchWidget : Widget
	{
		// Token: 0x060003B5 RID: 949 RVA: 0x0000C5F6 File Offset: 0x0000A7F6
		public TournamentMatchWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x0000C5FF File Offset: 0x0000A7FF
		// (set) Token: 0x060003B7 RID: 951 RVA: 0x0000C608 File Offset: 0x0000A808
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

		// Token: 0x0400019B RID: 411
		private int _state;
	}
}
