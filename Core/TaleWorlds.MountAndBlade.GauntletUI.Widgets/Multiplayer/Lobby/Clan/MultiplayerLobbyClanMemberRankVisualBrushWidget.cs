using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Clan
{
	// Token: 0x020000A5 RID: 165
	public class MultiplayerLobbyClanMemberRankVisualBrushWidget : BrushWidget
	{
		// Token: 0x060008A5 RID: 2213 RVA: 0x00018E13 File Offset: 0x00017013
		public MultiplayerLobbyClanMemberRankVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x00018E24 File Offset: 0x00017024
		private void UpdateTypeVisual()
		{
			if (this.Type == 0)
			{
				this.SetState("Member");
				return;
			}
			if (this.Type == 1)
			{
				this.SetState("Officer");
				return;
			}
			if (this.Type == 2)
			{
				this.SetState("Leader");
				return;
			}
			Debug.FailedAssert("This member type is not defined in widget", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Multiplayer\\Lobby\\Clan\\MultiplayerLobbyClanMemberRankVisualBrushWidget.cs", "UpdateTypeVisual", 28);
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x060008A7 RID: 2215 RVA: 0x00018E85 File Offset: 0x00017085
		// (set) Token: 0x060008A8 RID: 2216 RVA: 0x00018E8D File Offset: 0x0001708D
		[Editor(false)]
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (this._type != value)
				{
					this._type = value;
					base.OnPropertyChanged(value, "Type");
					this.UpdateTypeVisual();
				}
			}
		}

		// Token: 0x040003F2 RID: 1010
		private int _type = -1;
	}
}
