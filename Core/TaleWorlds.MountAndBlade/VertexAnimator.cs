using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000384 RID: 900
	public class VertexAnimator : SynchedMissionObject
	{
		// Token: 0x06003178 RID: 12664 RVA: 0x000CCDDA File Offset: 0x000CAFDA
		public VertexAnimator()
		{
			this.Speed = 20f;
		}

		// Token: 0x06003179 RID: 12665 RVA: 0x000CCDF8 File Offset: 0x000CAFF8
		private void SetIsPlaying(bool value)
		{
			if (this._isPlaying != value)
			{
				this._isPlaying = value;
				base.SetScriptComponentToTick(this.GetTickRequirement());
			}
		}

		// Token: 0x0600317A RID: 12666 RVA: 0x000CCE16 File Offset: 0x000CB016
		protected internal override void OnInit()
		{
			base.OnInit();
			this.RefreshEditDataUsers();
			this.SetIsPlaying(true);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x0600317B RID: 12667 RVA: 0x000CCE37 File Offset: 0x000CB037
		protected internal override void OnEditorInit()
		{
			this.OnInit();
		}

		// Token: 0x0600317C RID: 12668 RVA: 0x000CCE3F File Offset: 0x000CB03F
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (this._isPlaying)
			{
				return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
			}
			return base.GetTickRequirement();
		}

		// Token: 0x0600317D RID: 12669 RVA: 0x000CCE58 File Offset: 0x000CB058
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

		// Token: 0x0600317E RID: 12670 RVA: 0x000CCF3D File Offset: 0x000CB13D
		public void PlayOnce()
		{
			this.Play();
			this._playOnce = true;
		}

		// Token: 0x0600317F RID: 12671 RVA: 0x000CCF4C File Offset: 0x000CB14C
		public void Pause()
		{
			this.SetIsPlaying(false);
		}

		// Token: 0x06003180 RID: 12672 RVA: 0x000CCF55 File Offset: 0x000CB155
		public void Play()
		{
			this.Stop();
			this.Resume();
		}

		// Token: 0x06003181 RID: 12673 RVA: 0x000CCF63 File Offset: 0x000CB163
		public void Resume()
		{
			this.SetIsPlaying(true);
		}

		// Token: 0x06003182 RID: 12674 RVA: 0x000CCF6C File Offset: 0x000CB16C
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

		// Token: 0x06003183 RID: 12675 RVA: 0x000CCFB0 File Offset: 0x000CB1B0
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

		// Token: 0x06003184 RID: 12676 RVA: 0x000CCFF2 File Offset: 0x000CB1F2
		public void SetAnimation(int beginKey, int endKey, float speed)
		{
			this.BeginKey = beginKey;
			this.EndKey = endKey;
			this.Speed = speed;
		}

		// Token: 0x06003185 RID: 12677 RVA: 0x000CD00C File Offset: 0x000CB20C
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
					GameNetwork.WriteMessage(new SetMissionObjectVertexAnimation(this, beginKey, endKey, speed));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
		}

		// Token: 0x06003186 RID: 12678 RVA: 0x000CD06B File Offset: 0x000CB26B
		public void SetProgressSynched(float value)
		{
			if (MathF.Abs(this.Progress - value) > 0.0001f)
			{
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetMissionObjectVertexAnimationProgress(this, value));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				this.Progress = value;
			}
		}

		// Token: 0x170008DE RID: 2270
		// (get) Token: 0x06003187 RID: 12679 RVA: 0x000CD0A8 File Offset: 0x000CB2A8
		// (set) Token: 0x06003188 RID: 12680 RVA: 0x000CD0C8 File Offset: 0x000CB2C8
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

		// Token: 0x06003189 RID: 12681 RVA: 0x000CD114 File Offset: 0x000CB314
		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			int count = this._animatedMeshes.Count;
			for (int i = 0; i < count; i++)
			{
				this._animatedMeshes[i].ReleaseEditDataUser();
			}
		}

		// Token: 0x0600318A RID: 12682 RVA: 0x000CD154 File Offset: 0x000CB354
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

		// Token: 0x0600318B RID: 12683 RVA: 0x000CD1FC File Offset: 0x000CB3FC
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

		// Token: 0x0600318C RID: 12684 RVA: 0x000CD2F0 File Offset: 0x000CB4F0
		public override void WriteToNetwork()
		{
			base.WriteToNetwork();
			GameNetworkMessage.WriteIntToPacket(this.BeginKey, CompressionBasic.AnimationKeyCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.EndKey, CompressionBasic.AnimationKeyCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.Speed, CompressionBasic.VertexAnimationSpeedCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.Progress, CompressionBasic.AnimationProgressCompressionInfo);
		}

		// Token: 0x0600318D RID: 12685 RVA: 0x000CD344 File Offset: 0x000CB544
		public override bool ReadFromNetwork()
		{
			bool flag = true;
			flag = flag && base.ReadFromNetwork();
			if (flag)
			{
				int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationKeyCompressionInfo, ref flag);
				int num2 = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationKeyCompressionInfo, ref flag);
				float num3 = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.VertexAnimationSpeedCompressionInfo, ref flag);
				float num4 = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationProgressCompressionInfo, ref flag);
				if (flag)
				{
					this.BeginKey = num;
					this.EndKey = num2;
					this.Speed = num3;
					this.Progress = num4;
				}
			}
			return flag;
		}

		// Token: 0x040014AB RID: 5291
		public float Speed;

		// Token: 0x040014AC RID: 5292
		public int BeginKey;

		// Token: 0x040014AD RID: 5293
		public int EndKey;

		// Token: 0x040014AE RID: 5294
		private bool _playOnce;

		// Token: 0x040014AF RID: 5295
		private float _curAnimTime;

		// Token: 0x040014B0 RID: 5296
		private bool _isPlaying;

		// Token: 0x040014B1 RID: 5297
		private readonly List<Mesh> _animatedMeshes = new List<Mesh>();
	}
}
