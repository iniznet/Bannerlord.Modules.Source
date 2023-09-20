using System;
using System.Collections.Generic;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000057 RID: 87
	public sealed class ManagedScriptHolder : DotNetObject
	{
		// Token: 0x06000738 RID: 1848 RVA: 0x000060BC File Offset: 0x000042BC
		[EngineCallback]
		internal static ManagedScriptHolder CreateManagedScriptHolder()
		{
			return new ManagedScriptHolder();
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x000060C4 File Offset: 0x000042C4
		public ManagedScriptHolder()
		{
			this.TickComponentsParallelAuxMTPredicate = new TWParallel.ParallelForWithDtAuxPredicate(this.TickComponentsParallelAuxMT);
			this.TickComponentsParallel2AuxMTPredicate = new TWParallel.ParallelForWithDtAuxPredicate(this.TickComponentsParallel2AuxMT);
			this.TickComponentsOccasionallyParallelAuxMTPredicate = new TWParallel.ParallelForWithDtAuxPredicate(this.TickComponentsOccasionallyParallelAuxMT);
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x000061C8 File Offset: 0x000043C8
		[EngineCallback]
		public void SetScriptComponentHolder(ScriptComponentBehavior sc)
		{
			sc.SetOwnerManagedScriptHolder(this);
			if (this._scriptComponentsToRemoveFromTickForEditor.IndexOf(sc) != -1)
			{
				this._scriptComponentsToRemoveFromTickForEditor.Remove(sc);
			}
			else
			{
				this._scriptComponentsToAddToTickForEditor.Add(sc);
			}
			sc.SetScriptComponentToTick(sc.GetTickRequirement());
		}

		// Token: 0x0600073B RID: 1851 RVA: 0x00006208 File Offset: 0x00004408
		public void AddScriptComponentToTickOccasionallyList(ScriptComponentBehavior sc)
		{
			int num = this._scriptComponentsToRemoveFromTickOccasionally.IndexOf(sc);
			if (num != -1)
			{
				this._scriptComponentsToRemoveFromTickOccasionally.RemoveAt(num);
				return;
			}
			this._scriptComponentsToAddToTickOccasionally.Add(sc);
		}

		// Token: 0x0600073C RID: 1852 RVA: 0x00006240 File Offset: 0x00004440
		public void AddScriptComponentToTickList(ScriptComponentBehavior sc)
		{
			int num = this._scriptComponentsToRemoveFromTick.IndexOf(sc);
			if (num != -1)
			{
				this._scriptComponentsToRemoveFromTick.RemoveAt(num);
				return;
			}
			this._scriptComponentsToAddToTick.Add(sc);
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x00006278 File Offset: 0x00004478
		public void AddScriptComponentToParallelTickList(ScriptComponentBehavior sc)
		{
			int num = this._scriptComponentsToRemoveFromParallelTick.IndexOf(sc);
			if (num != -1)
			{
				this._scriptComponentsToRemoveFromParallelTick.RemoveAt(num);
				return;
			}
			this._scriptComponentsToAddToParallelTick.Add(sc);
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x000062B0 File Offset: 0x000044B0
		public void AddScriptComponentToParallelTick2List(ScriptComponentBehavior sc)
		{
			int num = this._scriptComponentsToRemoveFromParallelTick2.IndexOf(sc);
			if (num != -1)
			{
				this._scriptComponentsToRemoveFromParallelTick2.RemoveAt(num);
				return;
			}
			this._scriptComponentsToAddToParallelTick2.Add(sc);
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x000062E8 File Offset: 0x000044E8
		[EngineCallback]
		public void RemoveScriptComponentFromAllTickLists(ScriptComponentBehavior sc)
		{
			object addRemoveLockObject = ManagedScriptHolder.AddRemoveLockObject;
			lock (addRemoveLockObject)
			{
				sc.SetScriptComponentToTickMT(ScriptComponentBehavior.TickRequirement.None);
				if (this._scriptComponentsToAddToTickForEditor.IndexOf(sc) != -1)
				{
					this._scriptComponentsToAddToTickForEditor.Remove(sc);
				}
				else if (this._scriptComponentsToRemoveFromTickForEditor.IndexOf(sc) == -1)
				{
					this._scriptComponentsToRemoveFromTickForEditor.Add(sc);
				}
			}
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x00006364 File Offset: 0x00004564
		public void RemoveScriptComponentFromTickList(ScriptComponentBehavior sc)
		{
			if (this._scriptComponentsToAddToTick.IndexOf(sc) >= 0)
			{
				this._scriptComponentsToAddToTick.Remove(sc);
				return;
			}
			if (this._scriptComponentsToRemoveFromTick.IndexOf(sc) == -1 && this._scriptComponentsToTick.IndexOf(sc) != -1)
			{
				this._scriptComponentsToRemoveFromTick.Add(sc);
			}
		}

		// Token: 0x06000741 RID: 1857 RVA: 0x000063B8 File Offset: 0x000045B8
		public void RemoveScriptComponentFromParallelTickList(ScriptComponentBehavior sc)
		{
			if (this._scriptComponentsToAddToParallelTick.IndexOf(sc) >= 0)
			{
				this._scriptComponentsToAddToParallelTick.Remove(sc);
				return;
			}
			if (this._scriptComponentsToRemoveFromParallelTick.IndexOf(sc) == -1 && this._scriptComponentsToParallelTick.IndexOf(sc) != -1)
			{
				this._scriptComponentsToRemoveFromParallelTick.Add(sc);
			}
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x0000640C File Offset: 0x0000460C
		public void RemoveScriptComponentFromParallelTick2List(ScriptComponentBehavior sc)
		{
			if (this._scriptComponentsToAddToParallelTick2.IndexOf(sc) >= 0)
			{
				this._scriptComponentsToAddToParallelTick2.Remove(sc);
				return;
			}
			if (this._scriptComponentsToRemoveFromParallelTick2.IndexOf(sc) == -1 && this._scriptComponentsToParallelTick2.IndexOf(sc) != -1)
			{
				this._scriptComponentsToRemoveFromParallelTick2.Add(sc);
			}
		}

		// Token: 0x06000743 RID: 1859 RVA: 0x00006460 File Offset: 0x00004660
		public void RemoveScriptComponentFromTickOccasionallyList(ScriptComponentBehavior sc)
		{
			if (this._scriptComponentsToAddToTickOccasionally.IndexOf(sc) >= 0)
			{
				this._scriptComponentsToAddToTickOccasionally.Remove(sc);
				return;
			}
			if (this._scriptComponentsToRemoveFromTickOccasionally.IndexOf(sc) == -1 && this._scriptComponentsToTickOccasionally.IndexOf(sc) != -1)
			{
				this._scriptComponentsToRemoveFromTickOccasionally.Add(sc);
			}
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x000064B4 File Offset: 0x000046B4
		[EngineCallback]
		internal int GetNumberOfScripts()
		{
			return this._scriptComponentsToTick.Count;
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x000064C4 File Offset: 0x000046C4
		private void TickComponentsParallelAuxMT(int startInclusive, int endExclusive, float dt)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this._scriptComponentsToParallelTick[i].OnTickParallel(dt);
			}
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x000064F0 File Offset: 0x000046F0
		private void TickComponentsParallel2AuxMT(int startInclusive, int endExclusive, float dt)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this._scriptComponentsToParallelTick2[i].OnTickParallel2(dt);
			}
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x0000651C File Offset: 0x0000471C
		private void TickComponentsOccasionallyParallelAuxMT(int startInclusive, int endExclusive, float dt)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this._scriptComponentsToTickOccasionally[i].OnTickOccasionally(dt);
			}
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x00006548 File Offset: 0x00004748
		[EngineCallback]
		internal void TickComponents(float dt)
		{
			foreach (ScriptComponentBehavior scriptComponentBehavior in this._scriptComponentsToRemoveFromParallelTick)
			{
				this._scriptComponentsToParallelTick.Remove(scriptComponentBehavior);
			}
			this._scriptComponentsToRemoveFromParallelTick.Clear();
			foreach (ScriptComponentBehavior scriptComponentBehavior2 in this._scriptComponentsToAddToParallelTick)
			{
				this._scriptComponentsToParallelTick.Add(scriptComponentBehavior2);
			}
			this._scriptComponentsToAddToParallelTick.Clear();
			TWParallel.For(0, this._scriptComponentsToParallelTick.Count, dt, this.TickComponentsParallelAuxMTPredicate, 1);
			foreach (ScriptComponentBehavior scriptComponentBehavior3 in this._scriptComponentsToRemoveFromParallelTick2)
			{
				this._scriptComponentsToParallelTick2.Remove(scriptComponentBehavior3);
			}
			this._scriptComponentsToRemoveFromParallelTick2.Clear();
			foreach (ScriptComponentBehavior scriptComponentBehavior4 in this._scriptComponentsToAddToParallelTick2)
			{
				this._scriptComponentsToParallelTick2.Add(scriptComponentBehavior4);
			}
			this._scriptComponentsToAddToParallelTick2.Clear();
			TWParallel.For(0, this._scriptComponentsToParallelTick2.Count, dt, this.TickComponentsParallel2AuxMTPredicate, 8);
			foreach (ScriptComponentBehavior scriptComponentBehavior5 in this._scriptComponentsToRemoveFromTick)
			{
				this._scriptComponentsToTick.Remove(scriptComponentBehavior5);
			}
			this._scriptComponentsToRemoveFromTick.Clear();
			foreach (ScriptComponentBehavior scriptComponentBehavior6 in this._scriptComponentsToAddToTick)
			{
				this._scriptComponentsToTick.Add(scriptComponentBehavior6);
			}
			this._scriptComponentsToAddToTick.Clear();
			foreach (ScriptComponentBehavior scriptComponentBehavior7 in this._scriptComponentsToTick)
			{
				scriptComponentBehavior7.OnTick(dt);
			}
			foreach (ScriptComponentBehavior scriptComponentBehavior8 in this._scriptComponentsToRemoveFromTickOccasionally)
			{
				this._scriptComponentsToTickOccasionally.Remove(scriptComponentBehavior8);
			}
			this._nextIndexToTickOccasionally = MathF.Max(0, this._nextIndexToTickOccasionally - this._scriptComponentsToRemoveFromTickOccasionally.Count);
			this._scriptComponentsToRemoveFromTickOccasionally.Clear();
			foreach (ScriptComponentBehavior scriptComponentBehavior9 in this._scriptComponentsToAddToTickOccasionally)
			{
				this._scriptComponentsToTickOccasionally.Add(scriptComponentBehavior9);
			}
			this._scriptComponentsToAddToTickOccasionally.Clear();
			int num = this._scriptComponentsToTickOccasionally.Count / 10 + 1;
			int num2 = Math.Min(this._nextIndexToTickOccasionally + num, this._scriptComponentsToTickOccasionally.Count);
			if (this._nextIndexToTickOccasionally < num2)
			{
				TWParallel.For(this._nextIndexToTickOccasionally, num2, dt, this.TickComponentsOccasionallyParallelAuxMTPredicate, 8);
				this._nextIndexToTickOccasionally = ((num2 >= this._scriptComponentsToTickOccasionally.Count) ? 0 : num2);
				return;
			}
			this._nextIndexToTickOccasionally = 0;
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x000068F4 File Offset: 0x00004AF4
		[EngineCallback]
		internal void TickComponentsEditor(float dt)
		{
			for (int i = 0; i < this._scriptComponentsToRemoveFromTickForEditor.Count; i++)
			{
				this._scriptComponentsToTickForEditor.Remove(this._scriptComponentsToRemoveFromTickForEditor[i]);
			}
			this._scriptComponentsToRemoveFromTickForEditor.Clear();
			for (int j = 0; j < this._scriptComponentsToAddToTickForEditor.Count; j++)
			{
				this._scriptComponentsToTickForEditor.Add(this._scriptComponentsToAddToTickForEditor[j]);
			}
			this._scriptComponentsToAddToTickForEditor.Clear();
			for (int k = 0; k < this._scriptComponentsToTickForEditor.Count; k++)
			{
				if (this._scriptComponentsToRemoveFromTickForEditor.IndexOf(this._scriptComponentsToTickForEditor[k]) == -1)
				{
					this._scriptComponentsToTickForEditor[k].OnEditorTick(dt);
				}
			}
		}

		// Token: 0x040000BE RID: 190
		public static object AddRemoveLockObject = new object();

		// Token: 0x040000BF RID: 191
		private readonly List<ScriptComponentBehavior> _scriptComponentsToTick = new List<ScriptComponentBehavior>(512);

		// Token: 0x040000C0 RID: 192
		private readonly List<ScriptComponentBehavior> _scriptComponentsToParallelTick = new List<ScriptComponentBehavior>(64);

		// Token: 0x040000C1 RID: 193
		private readonly List<ScriptComponentBehavior> _scriptComponentsToParallelTick2 = new List<ScriptComponentBehavior>(512);

		// Token: 0x040000C2 RID: 194
		private readonly List<ScriptComponentBehavior> _scriptComponentsToTickOccasionally = new List<ScriptComponentBehavior>(512);

		// Token: 0x040000C3 RID: 195
		private readonly List<ScriptComponentBehavior> _scriptComponentsToTickForEditor = new List<ScriptComponentBehavior>(512);

		// Token: 0x040000C4 RID: 196
		private int _nextIndexToTickOccasionally;

		// Token: 0x040000C5 RID: 197
		private readonly List<ScriptComponentBehavior> _scriptComponentsToAddToTick = new List<ScriptComponentBehavior>();

		// Token: 0x040000C6 RID: 198
		private readonly List<ScriptComponentBehavior> _scriptComponentsToRemoveFromTick = new List<ScriptComponentBehavior>();

		// Token: 0x040000C7 RID: 199
		private readonly List<ScriptComponentBehavior> _scriptComponentsToAddToParallelTick = new List<ScriptComponentBehavior>();

		// Token: 0x040000C8 RID: 200
		private readonly List<ScriptComponentBehavior> _scriptComponentsToRemoveFromParallelTick = new List<ScriptComponentBehavior>();

		// Token: 0x040000C9 RID: 201
		private readonly List<ScriptComponentBehavior> _scriptComponentsToAddToParallelTick2 = new List<ScriptComponentBehavior>();

		// Token: 0x040000CA RID: 202
		private readonly List<ScriptComponentBehavior> _scriptComponentsToRemoveFromParallelTick2 = new List<ScriptComponentBehavior>();

		// Token: 0x040000CB RID: 203
		private readonly List<ScriptComponentBehavior> _scriptComponentsToAddToTickOccasionally = new List<ScriptComponentBehavior>();

		// Token: 0x040000CC RID: 204
		private readonly List<ScriptComponentBehavior> _scriptComponentsToRemoveFromTickOccasionally = new List<ScriptComponentBehavior>();

		// Token: 0x040000CD RID: 205
		private readonly List<ScriptComponentBehavior> _scriptComponentsToAddToTickForEditor = new List<ScriptComponentBehavior>();

		// Token: 0x040000CE RID: 206
		private readonly List<ScriptComponentBehavior> _scriptComponentsToRemoveFromTickForEditor = new List<ScriptComponentBehavior>();

		// Token: 0x040000CF RID: 207
		private readonly TWParallel.ParallelForWithDtAuxPredicate TickComponentsParallelAuxMTPredicate;

		// Token: 0x040000D0 RID: 208
		private readonly TWParallel.ParallelForWithDtAuxPredicate TickComponentsParallel2AuxMTPredicate;

		// Token: 0x040000D1 RID: 209
		private readonly TWParallel.ParallelForWithDtAuxPredicate TickComponentsOccasionallyParallelAuxMTPredicate;
	}
}
