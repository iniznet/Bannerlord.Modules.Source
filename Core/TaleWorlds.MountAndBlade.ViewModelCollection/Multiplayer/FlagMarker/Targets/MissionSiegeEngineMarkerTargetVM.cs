using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.FlagMarker.Targets
{
	public class MissionSiegeEngineMarkerTargetVM : MissionMarkerTargetVM
	{
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

		protected override float HeightOffset
		{
			get
			{
				return 2.5f;
			}
		}

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

		private readonly GameEntity _siegeEngine;

		public readonly BattleSideEnum Side;

		private string _siegeEngineID;
	}
}
