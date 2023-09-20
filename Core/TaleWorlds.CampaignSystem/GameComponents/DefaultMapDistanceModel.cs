using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000117 RID: 279
	public class DefaultMapDistanceModel : MapDistanceModel
	{
		// Token: 0x17000600 RID: 1536
		// (get) Token: 0x060015E6 RID: 5606 RVA: 0x00067851 File Offset: 0x00065A51
		// (set) Token: 0x060015E7 RID: 5607 RVA: 0x00067859 File Offset: 0x00065A59
		public override float MaximumDistanceBetweenTwoSettlements { get; set; }

		// Token: 0x060015E8 RID: 5608 RVA: 0x00067864 File Offset: 0x00065A64
		public void LoadCacheFromFile(BinaryReader reader)
		{
			this._settlementDistanceCache.Clear();
			if (reader == null)
			{
				for (int i = 0; i < Settlement.All.Count; i++)
				{
					Settlement settlement = Settlement.All[i];
					this._settlementsToConsider.Add(settlement);
					for (int j = i + 1; j < Settlement.All.Count; j++)
					{
						Settlement settlement2 = Settlement.All[j];
						float distance = this.GetDistance(settlement.GatePosition, settlement2.GatePosition, settlement.CurrentNavigationFace, settlement2.CurrentNavigationFace);
						if (settlement.Id.InternalValue <= settlement2.Id.InternalValue)
						{
							this.AddNewPairToDistanceCache(new ValueTuple<Settlement, Settlement>(settlement, settlement2), distance);
						}
						else
						{
							this.AddNewPairToDistanceCache(new ValueTuple<Settlement, Settlement>(settlement2, settlement), distance);
						}
					}
				}
				int numberOfNavigationMeshFaces = Campaign.Current.MapSceneWrapper.GetNumberOfNavigationMeshFaces();
				for (int k = 0; k < numberOfNavigationMeshFaces; k++)
				{
					PathFaceRecord faceIndex = new PathFaceRecord(k, -1, -1);
					Vec2 navigationMeshCenterPosition = Campaign.Current.MapSceneWrapper.GetNavigationMeshCenterPosition(faceIndex);
					faceIndex = Campaign.Current.MapSceneWrapper.GetFaceIndex(navigationMeshCenterPosition);
					TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(faceIndex);
					if (faceTerrainType != TerrainType.Mountain && faceTerrainType != TerrainType.Lake && faceTerrainType != TerrainType.Water && faceTerrainType != TerrainType.River && faceTerrainType != TerrainType.Canyon && faceTerrainType != TerrainType.RuralArea)
					{
						float num = float.MaxValue;
						Settlement settlement3 = null;
						foreach (Settlement settlement4 in Settlement.All)
						{
							float num2;
							if ((settlement3 == null || navigationMeshCenterPosition.DistanceSquared(settlement4.GatePosition) < num * num) && Campaign.Current.MapSceneWrapper.GetPathDistanceBetweenAIFaces(faceIndex, settlement4.CurrentNavigationFace, navigationMeshCenterPosition, settlement4.GatePosition, 0.1f, num, out num2) && num2 < num)
							{
								num = num2;
								settlement3 = settlement4;
							}
						}
						if (settlement3 != null)
						{
							this._navigationMeshClosestSettlementCache[k] = settlement3;
						}
					}
				}
				return;
			}
			int num3 = reader.ReadInt32();
			for (int l = 0; l < num3; l++)
			{
				Settlement settlement5 = Settlement.Find(reader.ReadString());
				this._settlementsToConsider.Add(settlement5);
				for (int m = l + 1; m < num3; m++)
				{
					Settlement settlement6 = Settlement.Find(reader.ReadString());
					float num4 = reader.ReadSingle();
					if (settlement5.Id.InternalValue <= settlement6.Id.InternalValue)
					{
						this.AddNewPairToDistanceCache(new ValueTuple<Settlement, Settlement>(settlement5, settlement6), num4);
					}
					else
					{
						this.AddNewPairToDistanceCache(new ValueTuple<Settlement, Settlement>(settlement6, settlement5), num4);
					}
				}
			}
			for (int n = reader.ReadInt32(); n >= 0; n = reader.ReadInt32())
			{
				Settlement settlement7 = Settlement.Find(reader.ReadString());
				this._navigationMeshClosestSettlementCache[n] = settlement7;
			}
		}

		// Token: 0x060015E9 RID: 5609 RVA: 0x00067B6C File Offset: 0x00065D6C
		public override float GetDistance(Settlement fromSettlement, Settlement toSettlement)
		{
			float num;
			if (fromSettlement == toSettlement)
			{
				num = 0f;
			}
			else if (fromSettlement.Id.InternalValue <= toSettlement.Id.InternalValue)
			{
				ValueTuple<Settlement, Settlement> valueTuple = new ValueTuple<Settlement, Settlement>(fromSettlement, toSettlement);
				if (!this._settlementDistanceCache.TryGetValue(valueTuple, out num))
				{
					num = this.GetDistance(fromSettlement.GatePosition, toSettlement.GatePosition, fromSettlement.CurrentNavigationFace, toSettlement.CurrentNavigationFace);
					this.AddNewPairToDistanceCache(valueTuple, num);
				}
			}
			else
			{
				ValueTuple<Settlement, Settlement> valueTuple2 = new ValueTuple<Settlement, Settlement>(toSettlement, fromSettlement);
				if (!this._settlementDistanceCache.TryGetValue(valueTuple2, out num))
				{
					num = this.GetDistance(toSettlement.GatePosition, fromSettlement.GatePosition, toSettlement.CurrentNavigationFace, fromSettlement.CurrentNavigationFace);
					this.AddNewPairToDistanceCache(valueTuple2, num);
				}
			}
			return num;
		}

		// Token: 0x060015EA RID: 5610 RVA: 0x00067C2C File Offset: 0x00065E2C
		public override float GetDistance(MobileParty fromParty, Settlement toSettlement)
		{
			float num;
			if (fromParty.CurrentSettlement != null)
			{
				num = this.GetDistance(fromParty.CurrentSettlement, toSettlement);
			}
			else if (fromParty.CurrentNavigationFace.FaceIndex == toSettlement.CurrentNavigationFace.FaceIndex)
			{
				num = fromParty.Position2D.Distance(toSettlement.GatePosition);
			}
			else
			{
				Settlement closestSettlementForNavigationMesh = this.GetClosestSettlementForNavigationMesh(fromParty.CurrentNavigationFace);
				num = fromParty.Position2D.Distance(toSettlement.GatePosition) - closestSettlementForNavigationMesh.GatePosition.Distance(toSettlement.GatePosition) + this.GetDistance(closestSettlementForNavigationMesh, toSettlement);
			}
			return num;
		}

		// Token: 0x060015EB RID: 5611 RVA: 0x00067CC8 File Offset: 0x00065EC8
		public override float GetDistance(MobileParty fromParty, MobileParty toParty)
		{
			float num;
			if (fromParty.CurrentNavigationFace.FaceIndex == toParty.CurrentNavigationFace.FaceIndex)
			{
				num = fromParty.Position2D.Distance(toParty.Position2D);
			}
			else
			{
				Settlement settlement = fromParty.CurrentSettlement ?? this.GetClosestSettlementForNavigationMesh(fromParty.CurrentNavigationFace);
				Settlement settlement2 = toParty.CurrentSettlement ?? this.GetClosestSettlementForNavigationMesh(toParty.CurrentNavigationFace);
				num = fromParty.Position2D.Distance(toParty.Position2D) - settlement.GatePosition.Distance(settlement2.GatePosition) + this.GetDistance(settlement, settlement2);
			}
			return num;
		}

		// Token: 0x060015EC RID: 5612 RVA: 0x00067D70 File Offset: 0x00065F70
		public override bool GetDistance(Settlement fromSettlement, Settlement toSettlement, float maximumDistance, out float distance)
		{
			bool flag;
			if (fromSettlement == toSettlement)
			{
				distance = 0f;
				flag = true;
			}
			else if (fromSettlement.CurrentNavigationFace.FaceIndex == toSettlement.CurrentNavigationFace.FaceIndex)
			{
				distance = fromSettlement.GatePosition.Distance(toSettlement.GatePosition);
				flag = distance <= maximumDistance;
			}
			else if (fromSettlement.Id.InternalValue <= toSettlement.Id.InternalValue)
			{
				ValueTuple<Settlement, Settlement> valueTuple = new ValueTuple<Settlement, Settlement>(fromSettlement, toSettlement);
				if (this._settlementDistanceCache.TryGetValue(valueTuple, out distance))
				{
					flag = distance <= maximumDistance;
				}
				else
				{
					flag = this.GetDistanceWithDistanceLimit(fromSettlement.GatePosition, toSettlement.GatePosition, Campaign.Current.MapSceneWrapper.GetFaceIndex(fromSettlement.GatePosition), Campaign.Current.MapSceneWrapper.GetFaceIndex(toSettlement.GatePosition), maximumDistance, out distance);
					if (flag)
					{
						this.AddNewPairToDistanceCache(valueTuple, distance);
					}
				}
			}
			else
			{
				ValueTuple<Settlement, Settlement> valueTuple2 = new ValueTuple<Settlement, Settlement>(toSettlement, fromSettlement);
				if (this._settlementDistanceCache.TryGetValue(valueTuple2, out distance))
				{
					flag = distance <= maximumDistance;
				}
				else
				{
					flag = this.GetDistanceWithDistanceLimit(toSettlement.GatePosition, fromSettlement.GatePosition, Campaign.Current.MapSceneWrapper.GetFaceIndex(toSettlement.GatePosition), Campaign.Current.MapSceneWrapper.GetFaceIndex(fromSettlement.GatePosition), maximumDistance, out distance);
					if (flag)
					{
						this.AddNewPairToDistanceCache(valueTuple2, distance);
					}
				}
			}
			return flag;
		}

		// Token: 0x060015ED RID: 5613 RVA: 0x00067EE4 File Offset: 0x000660E4
		public override bool GetDistance(MobileParty fromParty, Settlement toSettlement, float maximumDistance, out float distance)
		{
			bool flag = false;
			if (fromParty.CurrentSettlement != null)
			{
				flag = this.GetDistance(fromParty.CurrentSettlement, toSettlement, maximumDistance, out distance);
			}
			else if (fromParty.CurrentNavigationFace.FaceIndex == toSettlement.CurrentNavigationFace.FaceIndex)
			{
				distance = fromParty.Position2D.Distance(toSettlement.GatePosition);
				flag = distance <= maximumDistance;
			}
			else
			{
				Settlement closestSettlementForNavigationMesh = this.GetClosestSettlementForNavigationMesh(fromParty.CurrentNavigationFace);
				if (this.GetDistance(closestSettlementForNavigationMesh, toSettlement, maximumDistance, out distance))
				{
					distance += fromParty.Position2D.Distance(toSettlement.GatePosition) - closestSettlementForNavigationMesh.GatePosition.Distance(toSettlement.GatePosition);
					flag = distance <= maximumDistance;
				}
			}
			return flag;
		}

		// Token: 0x060015EE RID: 5614 RVA: 0x00067FA8 File Offset: 0x000661A8
		public override bool GetDistance(IMapPoint fromMapPoint, MobileParty toParty, float maximumDistance, out float distance)
		{
			bool flag = false;
			if (fromMapPoint.CurrentNavigationFace.FaceIndex == toParty.CurrentNavigationFace.FaceIndex)
			{
				distance = fromMapPoint.Position2D.Distance(toParty.Position2D);
				flag = distance <= maximumDistance;
			}
			else
			{
				Settlement closestSettlementForNavigationMesh = this.GetClosestSettlementForNavigationMesh(fromMapPoint.CurrentNavigationFace);
				Settlement settlement = toParty.CurrentSettlement ?? this.GetClosestSettlementForNavigationMesh(toParty.CurrentNavigationFace);
				if (this.GetDistance(closestSettlementForNavigationMesh, settlement, maximumDistance, out distance))
				{
					distance += fromMapPoint.Position2D.Distance(toParty.Position2D) - closestSettlementForNavigationMesh.GatePosition.Distance(settlement.GatePosition);
					flag = distance <= maximumDistance;
				}
			}
			return flag;
		}

		// Token: 0x060015EF RID: 5615 RVA: 0x00068068 File Offset: 0x00066268
		public override bool GetDistance(IMapPoint fromMapPoint, Settlement toSettlement, float maximumDistance, out float distance)
		{
			bool flag = false;
			if (fromMapPoint.CurrentNavigationFace.FaceIndex == toSettlement.CurrentNavigationFace.FaceIndex)
			{
				distance = fromMapPoint.Position2D.Distance(toSettlement.GatePosition);
				flag = distance <= maximumDistance;
			}
			else
			{
				distance = 100f;
				Settlement closestSettlementForNavigationMesh = this.GetClosestSettlementForNavigationMesh(fromMapPoint.CurrentNavigationFace);
				if (this.GetDistance(closestSettlementForNavigationMesh, toSettlement, maximumDistance, out distance))
				{
					distance += fromMapPoint.Position2D.Distance(toSettlement.GatePosition) - closestSettlementForNavigationMesh.GatePosition.Distance(toSettlement.GatePosition);
					flag = distance <= maximumDistance;
				}
			}
			return flag;
		}

		// Token: 0x060015F0 RID: 5616 RVA: 0x00068118 File Offset: 0x00066318
		public override bool GetDistance(IMapPoint fromMapPoint, in Vec2 toPoint, float maximumDistance, out float distance)
		{
			bool flag = false;
			PathFaceRecord faceIndex = Campaign.Current.MapSceneWrapper.GetFaceIndex(toPoint);
			if (fromMapPoint.CurrentNavigationFace.FaceIndex == faceIndex.FaceIndex)
			{
				distance = fromMapPoint.Position2D.Distance(toPoint);
				flag = distance <= maximumDistance;
			}
			else
			{
				Settlement closestSettlementForNavigationMesh = this.GetClosestSettlementForNavigationMesh(fromMapPoint.CurrentNavigationFace);
				Settlement closestSettlementForNavigationMesh2 = this.GetClosestSettlementForNavigationMesh(faceIndex);
				if (this.GetDistance(closestSettlementForNavigationMesh, closestSettlementForNavigationMesh2, maximumDistance, out distance))
				{
					distance += fromMapPoint.Position2D.Distance(toPoint) - closestSettlementForNavigationMesh.GatePosition.Distance(closestSettlementForNavigationMesh2.GatePosition);
					flag = distance <= maximumDistance;
				}
			}
			return flag;
		}

		// Token: 0x060015F1 RID: 5617 RVA: 0x000681DC File Offset: 0x000663DC
		private float GetDistance(Vec2 pos1, Vec2 pos2, PathFaceRecord faceIndex1, PathFaceRecord faceIndex2)
		{
			float num;
			Campaign.Current.MapSceneWrapper.GetPathDistanceBetweenAIFaces(faceIndex1, faceIndex2, pos1, pos2, 0.1f, float.MaxValue, out num);
			return num;
		}

		// Token: 0x060015F2 RID: 5618 RVA: 0x0006820B File Offset: 0x0006640B
		private bool GetDistanceWithDistanceLimit(Vec2 pos1, Vec2 pos2, PathFaceRecord faceIndex1, PathFaceRecord faceIndex2, float distanceLimit, out float distance)
		{
			if (pos1.DistanceSquared(pos2) > distanceLimit * distanceLimit)
			{
				distance = float.MaxValue;
				return false;
			}
			return Campaign.Current.MapSceneWrapper.GetPathDistanceBetweenAIFaces(faceIndex1, faceIndex2, pos1, pos2, 0.1f, distanceLimit, out distance);
		}

		// Token: 0x060015F3 RID: 5619 RVA: 0x00068244 File Offset: 0x00066444
		public override Settlement GetClosestSettlementForNavigationMesh(PathFaceRecord face)
		{
			Settlement settlement;
			if (!this._navigationMeshClosestSettlementCache.TryGetValue(face.FaceIndex, out settlement))
			{
				Vec2 navigationMeshCenterPosition = Campaign.Current.MapSceneWrapper.GetNavigationMeshCenterPosition(face);
				float num = float.MaxValue;
				foreach (Settlement settlement2 in this._settlementsToConsider)
				{
					float num2 = settlement2.GatePosition.DistanceSquared(navigationMeshCenterPosition);
					if (num > num2)
					{
						num = num2;
						settlement = settlement2;
					}
				}
				this._navigationMeshClosestSettlementCache[face.FaceIndex] = settlement;
			}
			return settlement;
		}

		// Token: 0x060015F4 RID: 5620 RVA: 0x000682F0 File Offset: 0x000664F0
		private void AddNewPairToDistanceCache(ValueTuple<Settlement, Settlement> pair, float distance)
		{
			this._settlementDistanceCache.Add(pair, distance);
			if (distance > this.MaximumDistanceBetweenTwoSettlements)
			{
				this.MaximumDistanceBetweenTwoSettlements = distance;
				Campaign.Current.UpdateMaximumDistanceBetweenTwoSettlements();
			}
		}

		// Token: 0x040007A8 RID: 1960
		private readonly Dictionary<ValueTuple<Settlement, Settlement>, float> _settlementDistanceCache = new Dictionary<ValueTuple<Settlement, Settlement>, float>();

		// Token: 0x040007A9 RID: 1961
		private readonly Dictionary<int, Settlement> _navigationMeshClosestSettlementCache = new Dictionary<int, Settlement>();

		// Token: 0x040007AA RID: 1962
		private readonly List<Settlement> _settlementsToConsider = new List<Settlement>();
	}
}
