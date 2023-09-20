using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	public class MissionAgentLockItemVM : ViewModel
	{
		public Agent TrackedAgent { get; private set; }

		public MissionAgentLockItemVM(Agent agent, MissionAgentLockItemVM.LockStates initialLockState)
		{
			this.TrackedAgent = agent;
			this.LockState = (int)initialLockState;
		}

		public void SetLockState(MissionAgentLockItemVM.LockStates lockState)
		{
			this.LockState = (int)lockState;
		}

		public void UpdatePosition(Vec2 position)
		{
			this.Position = position;
		}

		[DataSourceProperty]
		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (value != this._position)
				{
					this._position = value;
					base.OnPropertyChangedWithValue(value, "Position");
				}
			}
		}

		[DataSourceProperty]
		public int LockState
		{
			get
			{
				return this._lockState;
			}
			set
			{
				if (value != this._lockState)
				{
					this._lockState = value;
					base.OnPropertyChangedWithValue(value, "LockState");
				}
			}
		}

		private Vec2 _position;

		private int _lockState = -1;

		public enum LockStates
		{
			Possible,
			Active
		}
	}
}
