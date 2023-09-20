using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	[EngineStruct("rglCapsule_data", false)]
	public struct CapsuleData
	{
		public Vec3 P1
		{
			get
			{
				return this._globalData.P1;
			}
			set
			{
				this._globalData.P1 = value;
			}
		}

		public Vec3 P2
		{
			get
			{
				return this._globalData.P2;
			}
			set
			{
				this._globalData.P2 = value;
			}
		}

		public float Radius
		{
			get
			{
				return this._globalData.Radius;
			}
			set
			{
				this._globalData.Radius = value;
			}
		}

		internal float LocalRadius
		{
			get
			{
				return this._localData.Radius;
			}
			set
			{
				this._localData.Radius = value;
			}
		}

		internal Vec3 LocalP1
		{
			get
			{
				return this._localData.P1;
			}
			set
			{
				this._localData.P1 = value;
			}
		}

		internal Vec3 LocalP2
		{
			get
			{
				return this._localData.P2;
			}
			set
			{
				this._localData.P2 = value;
			}
		}

		public CapsuleData(float radius, Vec3 p1, Vec3 p2)
		{
			this._globalData = new FtlCapsuleData(radius, p1, p2);
			this._localData = new FtlCapsuleData(radius, p1, p2);
		}

		public Vec3 GetBoxMin()
		{
			return new Vec3(MathF.Min(this.P1.x, this.P2.x) - this.Radius, MathF.Min(this.P1.y, this.P2.y) - this.Radius, MathF.Min(this.P1.z, this.P2.z) - this.Radius, -1f);
		}

		public Vec3 GetBoxMax()
		{
			return new Vec3(MathF.Max(this.P1.x, this.P2.x) + this.Radius, MathF.Max(this.P1.y, this.P2.y) + this.Radius, MathF.Max(this.P1.z, this.P2.z) + this.Radius, -1f);
		}

		private FtlCapsuleData _globalData;

		private FtlCapsuleData _localData;
	}
}
