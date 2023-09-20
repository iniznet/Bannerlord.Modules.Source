using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	// Token: 0x02000080 RID: 128
	public class MultiplayerPollProgressionWidget : Widget
	{
		// Token: 0x060006F7 RID: 1783 RVA: 0x00014A92 File Offset: 0x00012C92
		public MultiplayerPollProgressionWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x00014A9B File Offset: 0x00012C9B
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x060006F9 RID: 1785 RVA: 0x00014AA4 File Offset: 0x00012CA4
		// (set) Token: 0x060006FA RID: 1786 RVA: 0x00014AAC File Offset: 0x00012CAC
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

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x060006FB RID: 1787 RVA: 0x00014AE9 File Offset: 0x00012CE9
		// (set) Token: 0x060006FC RID: 1788 RVA: 0x00014AF1 File Offset: 0x00012CF1
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

		// Token: 0x04000316 RID: 790
		private const string _activeState = "Active";

		// Token: 0x04000317 RID: 791
		private const string _inactiveState = "Inactive";

		// Token: 0x04000318 RID: 792
		private bool _hasOngoingPoll;

		// Token: 0x04000319 RID: 793
		private ListPanel _pollExtension;
	}
}
