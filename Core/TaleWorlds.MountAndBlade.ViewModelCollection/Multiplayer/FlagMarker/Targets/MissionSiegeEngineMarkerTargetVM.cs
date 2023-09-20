using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.FlagMarker.Targets
{
	// Token: 0x020000C2 RID: 194
	public class MissionSiegeEngineMarkerTargetVM : MissionMarkerTargetVM
	{
		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x06001264 RID: 4708 RVA: 0x0003C856 File Offset: 0x0003AA56
		public override Vec3 WorldPosition
		{
			get
			{
				if (!(this._siegeEngine != null))
				{
					return Vec3.One;
				}
				return this._siegeEngine.GlobalPosition;
			}
		}

		// Token: 0x17000600 RID: 1536
		// (get) Token: 0x06001265 RID: 4709 RVA: 0x0003C877 File Offset: 0x0003AA77
		protected override float HeightOffset
		{
			get
			{
				return 2.5f;
			}
		}

		// Token: 0x06001266 RID: 4710 RVA: 0x0003C880 File Offset: 0x0003AA80
		public MissionSiegeEngineMarkerTargetVM(SiegeWeapon siegeEngine)
			: base(MissionMarkerType.SiegeEngine)
		{
			this._siegeEngine = siegeEngine.GameEntity;
			this.Side = siegeEngine.Side;
			this.SiegeEngineID = siegeEngine.GetSiegeEngineType().StringId;
			uint num = ((this.Side == BattleSideEnum.Attacker) ? Mission.Current.AttackerTeam.Color : Mission.Current.DefenderTeam.Color);
			uint num2 = ((this.Side == BattleSideEnum.Attacker) ? Mission.Current.AttackerTeam.Color2 : Mission.Current.DefenderTeam.Color2);
			base.RefreshColor(num, num2);
		}

		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x06001267 RID: 4711 RVA: 0x0003C919 File Offset: 0x0003AB19
		// (set) Token: 0x06001268 RID: 4712 RVA: 0x0003C921 File Offset: 0x0003AB21
		[DataSourceProperty]
		public string SiegeEngineID
		{
			get
			{
				return this._siegeEngineID;
			}
			set
			{
				if (value != this._siegeEngineID)
				{
					this._siegeEngineID = value;
					base.OnPropertyChangedWithValue<string>(value, "SiegeEngineID");
				}
			}
		}

		// Token: 0x040008C9 RID: 2249
		private readonly GameEntity _siegeEngine;

		// Token: 0x040008CA RID: 2250
		public readonly BattleSideEnum Side;

		// Token: 0x040008CB RID: 2251
		private string _siegeEngineID;
	}
}
