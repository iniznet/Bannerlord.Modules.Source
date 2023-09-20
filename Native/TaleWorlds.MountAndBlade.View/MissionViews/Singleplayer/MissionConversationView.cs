using System;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	public class MissionConversationView : MissionView
	{
		public static MissionConversationView Current
		{
			get
			{
				return Mission.Current.GetMissionBehavior<MissionConversationView>();
			}
		}
	}
}
