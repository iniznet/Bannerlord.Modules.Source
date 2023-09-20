using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x02000046 RID: 70
	[DataContract]
	[Serializable]
	public sealed class RestResponse : RestData
	{
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000183 RID: 387 RVA: 0x00004FF5 File Offset: 0x000031F5
		// (set) Token: 0x06000184 RID: 388 RVA: 0x00004FFD File Offset: 0x000031FD
		[DataMember]
		public bool Successful { get; private set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000185 RID: 389 RVA: 0x00005006 File Offset: 0x00003206
		// (set) Token: 0x06000186 RID: 390 RVA: 0x0000500E File Offset: 0x0000320E
		[DataMember]
		public string SuccessfulReason { get; private set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000187 RID: 391 RVA: 0x00005017 File Offset: 0x00003217
		// (set) Token: 0x06000188 RID: 392 RVA: 0x0000501F File Offset: 0x0000321F
		[DataMember]
		public RestFunctionResult FunctionResult { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000189 RID: 393 RVA: 0x00005028 File Offset: 0x00003228
		// (set) Token: 0x0600018A RID: 394 RVA: 0x00005030 File Offset: 0x00003230
		[DataMember]
		public byte[] UserCertificate { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600018B RID: 395 RVA: 0x00005039 File Offset: 0x00003239
		public int RemainingMessageCount
		{
			get
			{
				if (this._responseMessages != null)
				{
					return this._responseMessages.Count;
				}
				return 0;
			}
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00005050 File Offset: 0x00003250
		public RestResponse()
		{
			this._responseMessages = new List<RestResponseMessage>();
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00005063 File Offset: 0x00003263
		public void SetSuccessful(bool successful, string succressfulReason)
		{
			this.Successful = successful;
			this.SuccessfulReason = succressfulReason;
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00005073 File Offset: 0x00003273
		public RestResponseMessage TryDequeueMessage()
		{
			if (this._responseMessages != null && this._responseMessages.Count > 0)
			{
				RestResponseMessage restResponseMessage = this._responseMessages[0];
				this._responseMessages.RemoveAt(0);
				return restResponseMessage;
			}
			return null;
		}

		// Token: 0x0600018F RID: 399 RVA: 0x000050A5 File Offset: 0x000032A5
		public void ClearMessageQueue()
		{
			this._responseMessages.Clear();
		}

		// Token: 0x06000190 RID: 400 RVA: 0x000050B2 File Offset: 0x000032B2
		public void EnqueueMessage(RestResponseMessage message)
		{
			this._responseMessages.Add(message);
		}

		// Token: 0x0400008F RID: 143
		[DataMember]
		private List<RestResponseMessage> _responseMessages;
	}
}
