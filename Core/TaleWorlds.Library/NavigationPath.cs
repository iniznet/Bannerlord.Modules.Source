using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace TaleWorlds.Library
{
	public class NavigationPath : ISerializable
	{
		public Vec2[] PathPoints { get; private set; }

		[CachedData]
		public int Size { get; set; }

		public NavigationPath()
		{
			this.PathPoints = new Vec2[128];
		}

		protected NavigationPath(SerializationInfo info, StreamingContext context)
		{
			this.PathPoints = new Vec2[128];
			this.Size = info.GetInt32("s");
			for (int i = 0; i < this.Size; i++)
			{
				float single = info.GetSingle("x" + i);
				float single2 = info.GetSingle("y" + i);
				this.PathPoints[i] = new Vec2(single, single2);
			}
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("s", this.Size);
			for (int i = 0; i < this.Size; i++)
			{
				info.AddValue("x" + i, this.PathPoints[i].x);
				info.AddValue("y" + i, this.PathPoints[i].y);
			}
		}

		public Vec2 this[int i]
		{
			get
			{
				return this.PathPoints[i];
			}
		}

		private const int PathSize = 128;
	}
}
