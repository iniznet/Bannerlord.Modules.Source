using System;
using System.Collections.Generic;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	public sealed class ManagedScriptHolder : DotNetObject
	{
		[EngineCallback]
		internal static ManagedScriptHolder CreateManagedScriptHolder()
		{
			return new ManagedScriptHolder();
		}

		public ManagedScriptHolder()
		{
			this.TickComponentsParallelAuxMTPredicate = new TWParallel.ParallelForWithDtAuxPredicate(this.TickComponentsParallelAuxMT);
			this.TickComponentsParallel2AuxMTPredicate = new TWParallel.ParallelForWithDtAuxPredicate(this.TickComponentsParallel2AuxMT);
			this.TickComponentsOccasionallyParallelAuxMTPredicate = new TWParallel.ParallelForWithDtAuxPredicate(this.TickComponentsOccasionallyParallelAuxMT);
		}

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

		[EngineCallback]
		internal int GetNumberOfScripts()
		{
			return this._scriptComponentsToTick.Count;
		}

		private void TickComponentsParallelAuxMT(int startInclusive, int endExclusive, float dt)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this._scriptComponentsToParallelTick[i].OnTickParallel(dt);
			}
		}

		private void TickComponentsParallel2AuxMT(int startInclusive, int endExclusive, float dt)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this._scriptComponentsToParallelTick2[i].OnTickParallel2(dt);
			}
		}

		private void TickComponentsOccasionallyParallelAuxMT(int startInclusive, int endExclusive, float dt)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this._scriptComponentsToTickOccasionally[i].OnTickOccasionally(dt);
			}
		}

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

		public static object AddRemoveLockObject = new object();

		private readonly List<ScriptComponentBehavior> _scriptComponentsToTick = new List<ScriptComponentBehavior>(512);

		private readonly List<ScriptComponentBehavior> _scriptComponentsToParallelTick = new List<ScriptComponentBehavior>(64);

		private readonly List<ScriptComponentBehavior> _scriptComponentsToParallelTick2 = new List<ScriptComponentBehavior>(512);

		private readonly List<ScriptComponentBehavior> _scriptComponentsToTickOccasionally = new List<ScriptComponentBehavior>(512);

		private readonly List<ScriptComponentBehavior> _scriptComponentsToTickForEditor = new List<ScriptComponentBehavior>(512);

		private int _nextIndexToTickOccasionally;

		private readonly List<ScriptComponentBehavior> _scriptComponentsToAddToTick = new List<ScriptComponentBehavior>();

		private readonly List<ScriptComponentBehavior> _scriptComponentsToRemoveFromTick = new List<ScriptComponentBehavior>();

		private readonly List<ScriptComponentBehavior> _scriptComponentsToAddToParallelTick = new List<ScriptComponentBehavior>();

		private readonly List<ScriptComponentBehavior> _scriptComponentsToRemoveFromParallelTick = new List<ScriptComponentBehavior>();

		private readonly List<ScriptComponentBehavior> _scriptComponentsToAddToParallelTick2 = new List<ScriptComponentBehavior>();

		private readonly List<ScriptComponentBehavior> _scriptComponentsToRemoveFromParallelTick2 = new List<ScriptComponentBehavior>();

		private readonly List<ScriptComponentBehavior> _scriptComponentsToAddToTickOccasionally = new List<ScriptComponentBehavior>();

		private readonly List<ScriptComponentBehavior> _scriptComponentsToRemoveFromTickOccasionally = new List<ScriptComponentBehavior>();

		private readonly List<ScriptComponentBehavior> _scriptComponentsToAddToTickForEditor = new List<ScriptComponentBehavior>();

		private readonly List<ScriptComponentBehavior> _scriptComponentsToRemoveFromTickForEditor = new List<ScriptComponentBehavior>();

		private readonly TWParallel.ParallelForWithDtAuxPredicate TickComponentsParallelAuxMTPredicate;

		private readonly TWParallel.ParallelForWithDtAuxPredicate TickComponentsParallel2AuxMTPredicate;

		private readonly TWParallel.ParallelForWithDtAuxPredicate TickComponentsOccasionallyParallelAuxMTPredicate;
	}
}
