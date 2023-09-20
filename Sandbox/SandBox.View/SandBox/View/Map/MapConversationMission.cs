using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace SandBox.View.Map
{
	public class MapConversationMission : ICampaignMission
	{
		GameState ICampaignMission.State
		{
			get
			{
				return GameStateManager.Current.ActiveState;
			}
		}

		IMissionTroopSupplier ICampaignMission.AgentSupplier
		{
			get
			{
				return null;
			}
		}

		Location ICampaignMission.Location { get; set; }

		Alley ICampaignMission.LastVisitedAlley { get; set; }

		MissionMode ICampaignMission.Mode
		{
			get
			{
				return 1;
			}
		}

		public MapConversationTableau ConversationTableau { get; private set; }

		public MapConversationMission()
		{
			CampaignMission.Current = this;
			this._conversationPlayQueue = new Queue<MapConversationMission.ConversationPlayArgs>();
		}

		public void SetConversationTableau(MapConversationTableau tableau)
		{
			this.ConversationTableau = tableau;
			this.PlayCachedConversations();
		}

		public void Tick(float dt)
		{
			this.PlayCachedConversations();
		}

		public void OnFinalize()
		{
			this.ConversationTableau = null;
			this._conversationPlayQueue = null;
			CampaignMission.Current = null;
		}

		private void PlayCachedConversations()
		{
			if (this.ConversationTableau != null)
			{
				while (this._conversationPlayQueue.Count > 0)
				{
					MapConversationMission.ConversationPlayArgs conversationPlayArgs = this._conversationPlayQueue.Dequeue();
					this.ConversationTableau.OnConversationPlay(conversationPlayArgs.IdleActionId, conversationPlayArgs.IdleFaceAnimId, conversationPlayArgs.ReactionId, conversationPlayArgs.ReactionFaceAnimId, conversationPlayArgs.SoundPath);
				}
			}
		}

		void ICampaignMission.OnConversationPlay(string idleActionId, string idleFaceAnimId, string reactionId, string reactionFaceAnimId, string soundPath)
		{
			if (this.ConversationTableau != null)
			{
				this.ConversationTableau.OnConversationPlay(idleActionId, idleFaceAnimId, reactionId, reactionFaceAnimId, soundPath);
				return;
			}
			this._conversationPlayQueue.Enqueue(new MapConversationMission.ConversationPlayArgs(idleActionId, idleFaceAnimId, reactionId, reactionFaceAnimId, soundPath));
		}

		void ICampaignMission.AddAgentFollowing(IAgent agent)
		{
		}

		bool ICampaignMission.AgentLookingAtAgent(IAgent agent1, IAgent agent2)
		{
			return false;
		}

		bool ICampaignMission.CheckIfAgentCanFollow(IAgent agent)
		{
			return false;
		}

		bool ICampaignMission.CheckIfAgentCanUnFollow(IAgent agent)
		{
			return false;
		}

		void ICampaignMission.EndMission()
		{
		}

		void ICampaignMission.OnCharacterLocationChanged(LocationCharacter locationCharacter, Location fromLocation, Location toLocation)
		{
		}

		void ICampaignMission.OnCloseEncounterMenu()
		{
		}

		void ICampaignMission.OnConversationContinue()
		{
		}

		void ICampaignMission.OnConversationEnd(IAgent agent)
		{
		}

		void ICampaignMission.OnConversationStart(IAgent agent, bool setActionsInstantly)
		{
		}

		void ICampaignMission.OnProcessSentence()
		{
		}

		void ICampaignMission.RemoveAgentFollowing(IAgent agent)
		{
		}

		void ICampaignMission.SetMissionMode(MissionMode newMode, bool atStart)
		{
		}

		private Queue<MapConversationMission.ConversationPlayArgs> _conversationPlayQueue;

		public struct ConversationPlayArgs
		{
			public ConversationPlayArgs(string idleActionId, string idleFaceAnimId, string reactionId, string reactionFaceAnimId, string soundPath)
			{
				this.IdleActionId = idleActionId;
				this.IdleFaceAnimId = idleFaceAnimId;
				this.ReactionId = reactionId;
				this.ReactionFaceAnimId = reactionFaceAnimId;
				this.SoundPath = soundPath;
			}

			public readonly string IdleActionId;

			public readonly string IdleFaceAnimId;

			public readonly string ReactionId;

			public readonly string ReactionFaceAnimId;

			public readonly string SoundPath;
		}
	}
}
