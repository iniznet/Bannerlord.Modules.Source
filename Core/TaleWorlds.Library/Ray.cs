using System;

namespace TaleWorlds.Library
{
	public struct Ray
	{
		public Vec3 Origin
		{
			get
			{
				return this._origin;
			}
			private set
			{
				this._origin = value;
			}
		}

		public Vec3 Direction
		{
			get
			{
				return this._direction;
			}
			private set
			{
				this._direction = value;
			}
		}

		public float MaxDistance
		{
			get
			{
				return this._maxDistance;
			}
			private set
			{
				this._maxDistance = value;
			}
		}

		public Vec3 EndPoint
		{
			get
			{
				return this.Origin + this.Direction * this.MaxDistance;
			}
		}

		public Ray(Vec3 origin, Vec3 direction, float maxDistance = 3.4028235E+38f)
		{
			this = default(Ray);
			this.Reset(origin, direction, maxDistance);
		}

		public Ray(Vec3 origin, Vec3 direction, bool useDirectionLenForMaxDistance)
		{
			this._origin = origin;
			this._direction = direction;
			float num = this._direction.Normalize();
			if (useDirectionLenForMaxDistance)
			{
				this._maxDistance = num;
				return;
			}
			this._maxDistance = float.MaxValue;
		}

		public void Reset(Vec3 origin, Vec3 direction, float maxDistance = 3.4028235E+38f)
		{
			this._origin = origin;
			this._direction = direction;
			this._maxDistance = maxDistance;
			this._direction.Normalize();
		}

		private Vec3 _origin;

		private Vec3 _direction;

		private float _maxDistance;
	}
}
