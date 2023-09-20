using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class QueryData<T> : IQueryData
	{
		public QueryData(Func<T> valueFunc, float lifetime)
		{
			this._cachedValue = default(T);
			this._expireTime = 0f;
			this._lifetime = lifetime;
			this._valueFunc = valueFunc;
			this._syncGroup = null;
		}

		public QueryData(Func<T> valueFunc, float lifetime, T defaultCachedValue)
		{
			this._cachedValue = defaultCachedValue;
			this._expireTime = 0f;
			this._lifetime = lifetime;
			this._valueFunc = valueFunc;
			this._syncGroup = null;
		}

		public void Evaluate(float currentTime)
		{
			this.SetValue(this._valueFunc(), currentTime);
		}

		public void SetValue(T value, float currentTime)
		{
			this._cachedValue = value;
			this._expireTime = currentTime + this._lifetime;
		}

		public T GetCachedValue()
		{
			return this._cachedValue;
		}

		public T GetCachedValueWithMaxAge(float age)
		{
			if (Mission.Current.CurrentTime > this._expireTime - this._lifetime + MathF.Min(this._lifetime, age))
			{
				this.Expire();
				return this.Value;
			}
			return this._cachedValue;
		}

		public T Value
		{
			get
			{
				float currentTime = Mission.Current.CurrentTime;
				if (currentTime >= this._expireTime)
				{
					if (this._syncGroup != null)
					{
						IQueryData[] syncGroup = this._syncGroup;
						for (int i = 0; i < syncGroup.Length; i++)
						{
							syncGroup[i].Evaluate(currentTime);
						}
					}
					this.Evaluate(currentTime);
				}
				return this._cachedValue;
			}
		}

		public void Expire()
		{
			this._expireTime = 0f;
		}

		public static void SetupSyncGroup(params IQueryData[] groupItems)
		{
			for (int i = 0; i < groupItems.Length; i++)
			{
				groupItems[i].SetSyncGroup(groupItems);
			}
		}

		public void SetSyncGroup(IQueryData[] syncGroup)
		{
			this._syncGroup = syncGroup;
		}

		private T _cachedValue;

		private float _expireTime;

		private readonly float _lifetime;

		private readonly Func<T> _valueFunc;

		private IQueryData[] _syncGroup;
	}
}
