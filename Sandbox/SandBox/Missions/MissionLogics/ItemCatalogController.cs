using System;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class ItemCatalogController : MissionLogic
	{
		public MBReadOnlyList<ItemObject> AllItems { get; private set; }

		public ItemCatalogController()
		{
			this._campaign = Campaign.Current;
			this._game = Game.Current;
			this.timer = new Timer(base.Mission.CurrentTime, 1f, true);
		}

		public override void AfterStart()
		{
			base.AfterStart();
			base.Mission.SetMissionMode(2, true);
			this.AllItems = Game.Current.ObjectManager.GetObjectTypeList<ItemObject>();
			if (!this._campaign.IsInitializedSinglePlayerReferences)
			{
				this._campaign.InitializeSinglePlayerReferences();
			}
			CharacterObject playerCharacter = CharacterObject.PlayerCharacter;
			MobileParty.MainParty.MemberRoster.AddToCounts(playerCharacter, 1, false, 0, 0, true, -1);
			if (!Extensions.IsEmpty<Team>(base.Mission.Teams))
			{
				throw new MBIllegalValueException("Number of teams is not 0.");
			}
			base.Mission.Teams.Add(0, 4284776512U, uint.MaxValue, null, true, false, true);
			base.Mission.Teams.Add(1, 4281877080U, uint.MaxValue, null, true, false, true);
			base.Mission.PlayerTeam = base.Mission.AttackerTeam;
			EquipmentElement equipmentElement = playerCharacter.Equipment[0];
			EquipmentElement equipmentElement2 = playerCharacter.Equipment[1];
			EquipmentElement equipmentElement3 = playerCharacter.Equipment[2];
			EquipmentElement equipmentElement4 = playerCharacter.Equipment[3];
			EquipmentElement equipmentElement5 = playerCharacter.Equipment[4];
			playerCharacter.Equipment[0] = equipmentElement;
			playerCharacter.Equipment[1] = equipmentElement2;
			playerCharacter.Equipment[2] = equipmentElement3;
			playerCharacter.Equipment[3] = equipmentElement4;
			playerCharacter.Equipment[4] = equipmentElement5;
			ItemObject itemObject = this.AllItems[0];
			Equipment equipment = new Equipment();
			equipment.AddEquipmentToSlotWithoutAgent(this.GetEquipmentIndexOfItem(itemObject), new EquipmentElement(this.AllItems[0], null, null, false));
			AgentBuildData agentBuildData = new AgentBuildData(playerCharacter);
			agentBuildData.Equipment(equipment);
			Mission mission = base.Mission;
			AgentBuildData agentBuildData2 = agentBuildData.Team(base.Mission.AttackerTeam);
			Vec3 vec = new Vec3(15f, 12f, 1f, -1f);
			this._playerAgent = mission.SpawnAgent(agentBuildData2.InitialPosition(ref vec).InitialDirection(ref Vec2.Forward).Controller(2), false);
			this._playerAgent.WieldInitialWeapons(2);
			this._playerAgent.Health = 10000f;
		}

		private EquipmentIndex GetEquipmentIndexOfItem(ItemObject item)
		{
			if (Extensions.HasAnyFlag<ItemFlags>(item.ItemFlags, 12288))
			{
				return 4;
			}
			switch (item.ItemType)
			{
			case 1:
				return 10;
			case 2:
				return 0;
			case 3:
				return 0;
			case 4:
				return 0;
			case 5:
				return 0;
			case 6:
				return 0;
			case 7:
				return 0;
			case 8:
				return 0;
			case 9:
				return 0;
			case 10:
				return 0;
			case 12:
				return 5;
			case 13:
				return 6;
			case 14:
				return 7;
			case 15:
				return 8;
			case 16:
				return 0;
			case 17:
				return 0;
			case 18:
				return 0;
			case 19:
				return 10;
			case 22:
				return 9;
			case 23:
				return 11;
			}
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\MissionLogics\\ItemCatalogController.cs", "GetEquipmentIndexOfItem", 147);
			return -1;
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this.timer.Check(base.Mission.CurrentTime))
			{
				if (!Directory.Exists("ItemCatalog"))
				{
					Directory.CreateDirectory("ItemCatalog");
				}
				ItemCatalogController.BeforeCatalogTickDelegate beforeCatalogTick = this.BeforeCatalogTick;
				if (beforeCatalogTick != null)
				{
					beforeCatalogTick(this.curItemIndex);
				}
				this.timer.Reset(base.Mission.CurrentTime);
				MatrixFrame matrixFrame = default(MatrixFrame);
				matrixFrame.origin = new Vec3(10000f, 10000f, 10000f, -1f);
				this._playerAgent.AgentVisuals.SetFrame(ref matrixFrame);
				this._playerAgent.TeleportToPosition(matrixFrame.origin);
				Blow blow;
				blow..ctor(this._playerAgent.Index);
				blow.DamageType = 2;
				blow.BaseMagnitude = 1E+09f;
				blow.Position = this._playerAgent.Position;
				this._playerAgent.Die(blow, 20);
				this._playerAgent = null;
				for (int i = base.Mission.Agents.Count - 1; i >= 0; i--)
				{
					Agent agent = base.Mission.Agents[i];
					Blow blow2;
					blow2..ctor(agent.Index);
					blow2.DamageType = 2;
					blow2.BaseMagnitude = 1E+09f;
					blow2.Position = agent.Position;
					Blow blow3 = blow2;
					agent.TeleportToPosition(matrixFrame.origin);
					agent.Die(blow3, 20);
				}
				ItemObject itemObject = this.AllItems[this.curItemIndex];
				Equipment equipment = new Equipment();
				equipment.AddEquipmentToSlotWithoutAgent(this.GetEquipmentIndexOfItem(itemObject), new EquipmentElement(itemObject, null, null, false));
				AgentBuildData agentBuildData = new AgentBuildData(this._game.PlayerTroop);
				agentBuildData.Equipment(equipment);
				Mission mission = base.Mission;
				AgentBuildData agentBuildData2 = agentBuildData.Team(base.Mission.AttackerTeam);
				Vec3 vec = new Vec3(15f, 12f, 1f, -1f);
				this._playerAgent = mission.SpawnAgent(agentBuildData2.InitialPosition(ref vec).InitialDirection(ref Vec2.Forward).Controller(2), false);
				this._playerAgent.WieldInitialWeapons(2);
				this._playerAgent.Health = 10000f;
				Action afterCatalogTick = this.AfterCatalogTick;
				if (afterCatalogTick != null)
				{
					afterCatalogTick();
				}
				this.curItemIndex++;
			}
		}

		public event ItemCatalogController.BeforeCatalogTickDelegate BeforeCatalogTick;

		public event Action AfterCatalogTick;

		private Campaign _campaign;

		private Game _game;

		private Agent _playerAgent;

		private int curItemIndex = 1;

		private Timer timer;

		public delegate void BeforeCatalogTickDelegate(int currentItemIndex);
	}
}
