using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TaleWorlds.Library.Http
{
	// Token: 0x020000A5 RID: 165
	public class HttpGetRequest : IHttpRequestTask
	{
		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060005FF RID: 1535 RVA: 0x00013094 File Offset: 0x00011294
		// (set) Token: 0x06000600 RID: 1536 RVA: 0x0001309C File Offset: 0x0001129C
		public HttpRequestTaskState State { get; private set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000601 RID: 1537 RVA: 0x000130A5 File Offset: 0x000112A5
		// (set) Token: 0x06000602 RID: 1538 RVA: 0x000130AD File Offset: 0x000112AD
		public bool Successful { get; private set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000603 RID: 1539 RVA: 0x000130B6 File Offset: 0x000112B6
		// (set) Token: 0x06000604 RID: 1540 RVA: 0x000130BE File Offset: 0x000112BE
		public string ResponseData { get; private set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000605 RID: 1541 RVA: 0x000130C7 File Offset: 0x000112C7
		// (set) Token: 0x06000606 RID: 1542 RVA: 0x000130CF File Offset: 0x000112CF
		public HttpStatusCode ResponseStatusCode { get; private set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000607 RID: 1543 RVA: 0x000130D8 File Offset: 0x000112D8
		// (set) Token: 0x06000608 RID: 1544 RVA: 0x000130E0 File Offset: 0x000112E0
		public Exception Exception { get; private set; }

		// Token: 0x06000609 RID: 1545 RVA: 0x000130E9 File Offset: 0x000112E9
		static HttpGetRequest()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x000130F5 File Offset: 0x000112F5
		public HttpGetRequest(string address)
		{
			this._address = address;
			this.State = HttpRequestTaskState.NotStarted;
			this.ResponseData = "";
			this.ResponseStatusCode = HttpStatusCode.OK;
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x00013121 File Offset: 0x00011321
		private void SetFinishedAsSuccessful(string responseData, HttpStatusCode statusCode)
		{
			this.Successful = true;
			this.ResponseData = responseData;
			this.ResponseStatusCode = statusCode;
			if (this._httpWebResponse != null)
			{
				this._httpWebResponse.Close();
			}
			this.State = HttpRequestTaskState.Finished;
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x00013152 File Offset: 0x00011352
		private void SetFinishedAsUnsuccessful(Exception e)
		{
			this.Successful = false;
			this.Exception = e;
			if (this._httpWebResponse != null)
			{
				this._httpWebResponse.Close();
			}
			this.State = HttpRequestTaskState.Finished;
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x0001317C File Offset: 0x0001137C
		public void Start()
		{
			this.DoTask();
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x00013188 File Offset: 0x00011388
		public async Task DoTask()
		{
			this.State = HttpRequestTaskState.Working;
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this._address);
				httpWebRequest.Accept = "application/json";
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Method = "GET";
				httpWebRequest.UserAgent = "WarRide Server";
				WebResponse webResponse = await httpWebRequest.GetResponseAsync();
				this._httpWebResponse = (HttpWebResponse)webResponse;
				Stream responseStream = this._httpWebResponse.GetResponseStream();
				byte[] readBuffer = new byte[1024];
				StringBuilder requestData = new StringBuilder("");
				int num;
				do
				{
					num = await responseStream.ReadAsync(readBuffer, 0, 1024);
					if (num > 0)
					{
						requestData.Append(Encoding.ASCII.GetString(readBuffer, 0, num));
					}
				}
				while (num > 0);
				string text = "";
				if (requestData.Length > 1)
				{
					text = requestData.ToString();
				}
				responseStream.Close();
				this.SetFinishedAsSuccessful(text, this._httpWebResponse.StatusCode);
				responseStream = null;
				readBuffer = null;
				requestData = null;
			}
			catch (Exception ex)
			{
				this.SetFinishedAsUnsuccessful(ex);
			}
		}

		// Token: 0x040001C1 RID: 449
		private const int BufferSize = 1024;

		// Token: 0x040001C2 RID: 450
		private HttpWebResponse _httpWebResponse;

		// Token: 0x040001C3 RID: 451
		private readonly string _address;
	}
}
