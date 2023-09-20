using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Clan
{
	public class MultiplayerLobbyClanMemberRankVisualBrushWidget : BrushWidget
	{
		public MultiplayerLobbyClanMemberRankVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

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

		private int _type = -1;
	}
}
