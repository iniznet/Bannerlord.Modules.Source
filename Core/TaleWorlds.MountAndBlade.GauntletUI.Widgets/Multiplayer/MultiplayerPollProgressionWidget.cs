using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	public class MultiplayerPollProgressionWidget : Widget
	{
		public MultiplayerPollProgressionWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
		}

		public bool HasOngoingPoll
		{
			get
			{
				return this._hasOngoingPoll;
			}
			set
			{
				if (value != this._hasOngoingPoll)
				{
					this._hasOngoingPoll = value;
					base.OnPropertyChanged(value, "HasOngoingPoll");
					ListPanel pollExtension = this.PollExtension;
					if (pollExtension == null)
					{
						return;
					}
					pollExtension.SetState(value ? "Active" : "Inactive");
				}
			}
		}

		[Editor(false)]
		public ListPanel PollExtension
		{
			get
			{
				return this._pollExtension;
			}
			set
			{
				if (value != this._pollExtension)
				{
					this._pollExtension = value;
					base.OnPropertyChanged<ListPanel>(value, "PollExtension");
					this._pollExtension.SetState("Inactive");
				}
			}
		}

		private const string _activeState = "Active";

		private const string _inactiveState = "Inactive";

		private bool _hasOngoingPoll;

		private ListPanel _pollExtension;
	}
}
