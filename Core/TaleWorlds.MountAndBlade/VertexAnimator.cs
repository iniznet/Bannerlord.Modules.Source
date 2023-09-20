using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	public class VertexAnimator : SynchedMissionObject
	{
		public VertexAnimator()
		{
			this.Speed = 20f;
		}

		private void SetIsPlaying(bool value)
		{
			if (this._isPlaying != value)
			{
				this._isPlaying = value;
				base.SetScriptComponentToTick(this.GetTickRequirement());
			}
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			this.RefreshEditDataUsers();
			this.SetIsPlaying(true);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		protected internal override void OnEditorInit()
		{
			this.OnInit();
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (this._isPlaying)
			{
				return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
			}
			return base.GetTickRequirement();
		}

		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._isPlaying)
			{
				if (this._curAnimTime < (float)this.BeginKey)
				{
					this._curAnimTime = (float)this.BeginKey;
				}
				base.GameEntity.SetMorphFrameOfComponents(this._curAnimTime);
				this._curAnimTime += dt * this.Speed;
				if (this._curAnimTime > (float)this.EndKey)
				{
					if (this._curAnimTime > (float)this.EndKey && this._playOnce)
					{
						this.SetIsPlaying(false);
						this._curAnimTime = (float)this.EndKey;
						base.GameEntity.SetMorphFrameOfComponents(this._curAnimTime);
						return;
					}
					int num = 0;
					while (this._curAnimTime > (float)this.EndKey && ++num < 100)
					{
						this._curAnimTime = (float)this.BeginKey + (this._curAnimTime - (float)this.EndKey);
					}
				}
			}
		}

		public void PlayOnce()
		{
			this.Play();
			this._playOnce = true;
		}

		public void Pause()
		{
			this.SetIsPlaying(false);
		}

		public void Play()
		{
			this.Stop();
			this.Resume();
		}

		public void Resume()
		{
			this.SetIsPlaying(true);
		}

		public void Stop()
		{
			this.SetIsPlaying(false);
			this._curAnimTime = (float)this.BeginKey;
			Mesh firstMesh = base.GameEntity.GetFirstMesh();
			if (firstMesh != null)
			{
				firstMesh.MorphTime = this._curAnimTime;
			}
		}

		public void StopAndGoToEnd()
		{
			this.SetIsPlaying(false);
			this._curAnimTime = (float)this.EndKey;
			Mesh firstMesh = base.GameEntity.GetFirstMesh();
			if (firstMesh != null)
			{
				firstMesh.MorphTime = this._curAnimTime;
			}
		}

		public void SetAnimation(int beginKey, int endKey, float speed)
		{
			this.BeginKey = beginKey;
			this.EndKey = endKey;
			this.Speed = speed;
		}

		public void SetAnimationSynched(int beginKey, int endKey, float speed)
		{
			if (beginKey != this.BeginKey || endKey != this.EndKey || speed != this.Speed)
			{
				this.BeginKey = beginKey;
				this.EndKey = endKey;
				this.Speed = speed;
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetMissionObjectVertexAnimation(base.Id, beginKey, endKey, speed));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
		}

		public void SetProgressSynched(float value)
		{
			if (MathF.Abs(this.Progress - value) > 0.0001f)
			{
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetMissionObjectVertexAnimationProgress(base.Id, value));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				this.Progress = value;
			}
		}

		private float Progress
		{
			get
			{
				return (this._curAnimTime - (float)this.BeginKey) / (float)(this.EndKey - this.BeginKey);
			}
			set
			{
				this._curAnimTime = (float)this.BeginKey + value * (float)(this.EndKey - this.BeginKey);
				Mesh firstMesh = base.GameEntity.GetFirstMesh();
				if (firstMesh != null)
				{
					firstMesh.MorphTime = this._curAnimTime;
				}
			}
		}

		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			int count = this._animatedMeshes.Count;
			for (int i = 0; i < count; i++)
			{
				this._animatedMeshes[i].ReleaseEditDataUser();
			}
		}

		protected internal override void OnEditorTick(float dt)
		{
			int componentCount = base.GameEntity.GetComponentCount(GameEntity.ComponentType.MetaMesh);
			bool flag = false;
			for (int i = 0; i < componentCount; i++)
			{
				MetaMesh metaMesh = base.GameEntity.GetComponentAtIndex(i, GameEntity.ComponentType.MetaMesh) as MetaMesh;
				for (int j = 0; j < metaMesh.MeshCount; j++)
				{
					int count = this._animatedMeshes.Count;
					bool flag2 = false;
					for (int k = 0; k < count; k++)
					{
						if (metaMesh.GetMeshAtIndex(j) == this._animatedMeshes[k])
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.RefreshEditDataUsers();
			}
			this.OnTick(dt);
		}

		private void RefreshEditDataUsers()
		{
			foreach (Mesh mesh in this._animatedMeshes)
			{
				mesh.ReleaseEditDataUser();
			}
			this._animatedMeshes.Clear();
			int componentCount = base.GameEntity.GetComponentCount(GameEntity.ComponentType.MetaMesh);
			for (int i = 0; i < componentCount; i++)
			{
				MetaMesh metaMesh = base.GameEntity.GetComponentAtIndex(i, GameEntity.ComponentType.MetaMesh) as MetaMesh;
				for (int j = 0; j < metaMesh.MeshCount; j++)
				{
					Mesh meshAtIndex = metaMesh.GetMeshAtIndex(j);
					meshAtIndex.AddEditDataUser();
					meshAtIndex.HintVerticesDynamic();
					meshAtIndex.HintIndicesDynamic();
					this._animatedMeshes.Add(meshAtIndex);
					Mesh baseMesh = meshAtIndex.GetBaseMesh();
					if (baseMesh != null)
					{
						baseMesh.AddEditDataUser();
						this._animatedMeshes.Add(baseMesh);
					}
				}
			}
		}

		public override void WriteToNetwork()
		{
			base.WriteToNetwork();
			GameNetworkMessage.WriteIntToPacket(this.BeginKey, CompressionBasic.AnimationKeyCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.EndKey, CompressionBasic.AnimationKeyCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.Speed, CompressionBasic.VertexAnimationSpeedCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.Progress, CompressionBasic.AnimationProgressCompressionInfo);
		}

		public override void OnAfterReadFromNetwork(ValueTuple<BaseSynchedMissionObjectReadableRecord, ISynchedMissionObjectReadableRecord> synchedMissionObjectReadableRecord)
		{
			base.OnAfterReadFromNetwork(synchedMissionObjectReadableRecord);
			VertexAnimator.VertexAnimatorRecord vertexAnimatorRecord = (VertexAnimator.VertexAnimatorRecord)synchedMissionObjectReadableRecord.Item2;
			this.BeginKey = vertexAnimatorRecord.BeginKey;
			this.EndKey = vertexAnimatorRecord.EndKey;
			this.Speed = vertexAnimatorRecord.Speed;
			this.Progress = vertexAnimatorRecord.Progress;
		}

		public float Speed;

		public int BeginKey;

		public int EndKey;

		private bool _playOnce;

		private float _curAnimTime;

		private bool _isPlaying;

		private readonly List<Mesh> _animatedMeshes = new List<Mesh>();

		[DefineSynchedMissionObjectType(typeof(VertexAnimator))]
		public struct VertexAnimatorRecord : ISynchedMissionObjectReadableRecord
		{
			public int BeginKey { get; private set; }

			public int EndKey { get; private set; }

			public float Speed { get; private set; }

			public float Progress { get; private set; }

			public bool ReadFromNetwork(ref bool bufferReadValid)
			{
				this.BeginKey = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationKeyCompressionInfo, ref bufferReadValid);
				this.EndKey = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationKeyCompressionInfo, ref bufferReadValid);
				this.Speed = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.VertexAnimationSpeedCompressionInfo, ref bufferReadValid);
				this.Progress = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationProgressCompressionInfo, ref bufferReadValid);
				return bufferReadValid;
			}
		}
	}
}
