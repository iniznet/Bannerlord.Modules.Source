using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Objects.AreaMarkers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics.Towns
{
	// Token: 0x0200005E RID: 94
	public class WorkshopMissionHandler : MissionLogic
	{
		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000412 RID: 1042 RVA: 0x0001D5B6 File Offset: 0x0001B7B6
		public IEnumerable<Tuple<Workshop, GameEntity>> WorkshopSignEntities
		{
			get
			{
				return this._workshopSignEntities.AsEnumerable<Tuple<Workshop, GameEntity>>();
			}
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x0001D5C4 File Offset: 0x0001B7C4
		public WorkshopMissionHandler(Settlement settlement)
		{
			this._settlement = settlement;
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x0001D61A File Offset: 0x0001B81A
		public override void OnBehaviorInitialize()
		{
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
			this._workshopSignEntities = new List<Tuple<Workshop, GameEntity>>();
			this._listOfCurrentProps = new List<GameEntity>();
			this._propFrames = new Dictionary<int, Dictionary<string, List<MatrixFrame>>>();
			this._areaMarkers = new List<WorkshopAreaMarker>();
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x0001D65C File Offset: 0x0001B85C
		public override void EarlyStart()
		{
			for (int i = 0; i < this._settlement.Town.Workshops.Length; i++)
			{
				if (!this._settlement.Town.Workshops[i].WorkshopType.IsHidden)
				{
					this._propFrames.Add(i, new Dictionary<string, List<MatrixFrame>>());
					foreach (string text in this._propKinds)
					{
						this._propFrames[i].Add(text, new List<MatrixFrame>());
					}
				}
			}
			List<WorkshopAreaMarker> list = MBExtensions.FindAllWithType<WorkshopAreaMarker>(base.Mission.ActiveMissionObjects).ToList<WorkshopAreaMarker>();
			this._areaMarkers = list.FindAll((WorkshopAreaMarker x) => x.GameEntity.HasTag("workshop_area_marker"));
			foreach (WorkshopAreaMarker workshopAreaMarker in this._areaMarkers)
			{
			}
			foreach (GameEntity gameEntity in base.Mission.Scene.FindEntitiesWithTag("shop_prop").ToList<GameEntity>())
			{
				WorkshopAreaMarker workshopAreaMarker2 = this.FindWorkshop(gameEntity);
				if (workshopAreaMarker2 != null && this._propFrames.ContainsKey(workshopAreaMarker2.AreaIndex) && (this._settlement.Town.Workshops[workshopAreaMarker2.AreaIndex] == null || !this._settlement.Town.Workshops[workshopAreaMarker2.AreaIndex].WorkshopType.IsHidden))
				{
					foreach (string text2 in this._propKinds)
					{
						if (gameEntity.HasTag(text2))
						{
							this._propFrames[workshopAreaMarker2.AreaIndex][text2].Add(gameEntity.GetGlobalFrame());
							this._listOfCurrentProps.Add(gameEntity);
							break;
						}
					}
				}
			}
			this.SetBenches();
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x0001D884 File Offset: 0x0001BA84
		public override void AfterStart()
		{
			this.InitShopSigns();
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x0001D88C File Offset: 0x0001BA8C
		private WorkshopAreaMarker FindWorkshop(GameEntity prop)
		{
			foreach (WorkshopAreaMarker workshopAreaMarker in this._areaMarkers)
			{
				if (workshopAreaMarker.IsPositionInRange(prop.GlobalPosition))
				{
					return workshopAreaMarker;
				}
			}
			return null;
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x0001D8F0 File Offset: 0x0001BAF0
		private void SetBenches()
		{
			MissionAgentHandler missionAgentHandler = this._missionAgentHandler;
			if (missionAgentHandler != null)
			{
				missionAgentHandler.RemovePropReference(this._listOfCurrentProps);
			}
			foreach (GameEntity gameEntity in this._listOfCurrentProps)
			{
				gameEntity.Remove(89);
			}
			this._listOfCurrentProps.Clear();
			foreach (KeyValuePair<int, Dictionary<string, List<MatrixFrame>>> keyValuePair in this._propFrames)
			{
				int key = keyValuePair.Key;
				foreach (KeyValuePair<string, List<MatrixFrame>> keyValuePair2 in keyValuePair.Value)
				{
					List<string> prefabNames = this.GetPrefabNames(key, keyValuePair2.Key);
					if (prefabNames.Count != 0)
					{
						for (int i = 0; i < keyValuePair2.Value.Count; i++)
						{
							MatrixFrame matrixFrame = keyValuePair2.Value[i];
							this._listOfCurrentProps.Add(GameEntity.Instantiate(base.Mission.Scene, prefabNames[i % prefabNames.Count], matrixFrame));
						}
					}
				}
			}
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x0001DA60 File Offset: 0x0001BC60
		private void InitShopSigns()
		{
			if (Campaign.Current.GameMode == 1 && this._settlement != null && this._settlement.IsTown)
			{
				List<GameEntity> list = base.Mission.Scene.FindEntitiesWithTag("shop_sign").ToList<GameEntity>();
				foreach (WorkshopAreaMarker workshopAreaMarker in MBExtensions.FindAllWithType<WorkshopAreaMarker>(base.Mission.ActiveMissionObjects).ToList<WorkshopAreaMarker>())
				{
					Workshop workshop = this._settlement.Town.Workshops[workshopAreaMarker.AreaIndex];
					if (this._workshopSignEntities.All((Tuple<Workshop, GameEntity> x) => x.Item1 != workshop))
					{
						for (int i = 0; i < list.Count; i++)
						{
							GameEntity gameEntity = list[i];
							if (workshopAreaMarker.IsPositionInRange(gameEntity.GlobalPosition))
							{
								this._workshopSignEntities.Add(new Tuple<Workshop, GameEntity>(workshop, gameEntity));
								list.RemoveAt(i);
								break;
							}
						}
					}
				}
				foreach (Tuple<Workshop, GameEntity> tuple in this._workshopSignEntities)
				{
					GameEntity item = tuple.Item2;
					WorkshopType workshopType = tuple.Item1.WorkshopType;
					item.ClearComponents();
					MetaMesh copy = MetaMesh.GetCopy((workshopType != null) ? workshopType.SignMeshName : "shop_sign_merchantavailable", true, false);
					item.AddMultiMesh(copy, true);
				}
			}
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x0001DC10 File Offset: 0x0001BE10
		private List<string> GetPrefabNames(int areaIndex, string propKind)
		{
			List<string> list = new List<string>();
			Workshop workshop = this._settlement.Town.Workshops[areaIndex];
			if (workshop.WorkshopType != null)
			{
				if (propKind == this._propKinds[0])
				{
					list.Add(workshop.WorkshopType.PropMeshName1);
				}
				else if (propKind == this._propKinds[1])
				{
					list.Add(workshop.WorkshopType.PropMeshName2);
				}
				else if (propKind == this._propKinds[2])
				{
					list.AddRange(workshop.WorkshopType.PropMeshName3List);
				}
				else if (propKind == this._propKinds[3])
				{
					list.Add(workshop.WorkshopType.PropMeshName4);
				}
				else if (propKind == this._propKinds[4])
				{
					list.Add(workshop.WorkshopType.PropMeshName5);
				}
				else if (propKind == this._propKinds[5])
				{
					list.Add(workshop.WorkshopType.PropMeshName6);
				}
			}
			return list;
		}

		// Token: 0x040001E5 RID: 485
		private Settlement _settlement;

		// Token: 0x040001E6 RID: 486
		private MissionAgentHandler _missionAgentHandler;

		// Token: 0x040001E7 RID: 487
		private string[] _propKinds = new string[] { "a", "b", "c", "d", "e", "f" };

		// Token: 0x040001E8 RID: 488
		private Dictionary<int, Dictionary<string, List<MatrixFrame>>> _propFrames;

		// Token: 0x040001E9 RID: 489
		private List<GameEntity> _listOfCurrentProps;

		// Token: 0x040001EA RID: 490
		private List<WorkshopAreaMarker> _areaMarkers;

		// Token: 0x040001EB RID: 491
		private List<Tuple<Workshop, GameEntity>> _workshopSignEntities;
	}
}
