using System;

namespace SandBox.View.Map
{
	public class MapConversationView : MapView
	{
		protected internal override void OnFinalize()
		{
			base.OnFinalize();
			this.ConversationMission.OnFinalize();
			this.ConversationMission = null;
		}

		protected void CreateConversationMission()
		{
			this.ConversationMission = new MapConversationMission();
		}

		public MapConversationMission ConversationMission;
	}
}
