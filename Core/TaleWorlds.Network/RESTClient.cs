using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaleWorlds.Network
{
	public class RESTClient
	{
		public RESTClient(string serviceAddress)
		{
			this._serviceAddress = serviceAddress;
		}

		private ServiceException GetServiceErrorCode(Stream stream)
		{
			string text = new StreamReader(stream).ReadToEnd();
			JObject jobject = JObject.Parse(text);
			if (jobject["ExceptionMessage"] != null)
			{
				return JsonConvert.DeserializeObject<ServiceExceptionModel>(text).ToServiceException();
			}
			if (jobject["error_description"] == null)
			{
				return new ServiceException("unknown", string.Empty);
			}
			if ((string)jobject["error"] == "invalid_grant")
			{
				return new ServiceException(string.Empty, "InvalidUsernameOrPassword");
			}
			return new ServiceException((string)jobject["error"], (string)jobject["error_description"]);
		}

		private HttpWebRequest CreateHttpRequest(string service, List<KeyValuePair<string, string>> headers, string contentType, string method)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(this._serviceAddress + service));
			httpWebRequest.Accept = "application/json";
			httpWebRequest.ContentType = contentType;
			httpWebRequest.Method = method;
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in headers)
				{
					httpWebRequest.Headers.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return httpWebRequest;
		}

		public async Task<TResult> Get<TResult>(string service, List<KeyValuePair<string, string>> headers)
		{
			HttpWebRequest httpWebRequest = this.CreateHttpRequest(service, headers, "application/json", "GET");
			TResult tresult;
			try
			{
				TaskAwaiter<WebResponse> taskAwaiter = httpWebRequest.GetResponseAsync().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<WebResponse> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<WebResponse>);
				}
				tresult = JsonConvert.DeserializeObject<TResult>(new StreamReader(taskAwaiter.GetResult().GetResponseStream()).ReadToEnd());
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
				{
					throw this.GetServiceErrorCode(ex.Response.GetResponseStream());
				}
				throw new Exception("HTTP Get Web Request Failed", ex);
			}
			catch (Exception ex2)
			{
				throw new Exception("HTTP Get Failed", ex2);
			}
			return tresult;
		}

		public async Task Get(string service, List<KeyValuePair<string, string>> headers)
		{
			HttpWebRequest httpWebRequest = this.CreateHttpRequest(service, headers, "application/json", "GET");
			try
			{
				await httpWebRequest.GetResponseAsync();
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
				{
					throw this.GetServiceErrorCode(ex.Response.GetResponseStream());
				}
				throw new Exception("HTTP Get Web Request Failed", ex);
			}
			catch (Exception ex2)
			{
				throw new Exception("HTTP Get Failed", ex2);
			}
		}

		public async Task<TResult> Post<TResult>(string service, List<KeyValuePair<string, string>> headers, string payLoad, string contentType = "application/json")
		{
			HttpWebRequest http = this.CreateHttpRequest(service, headers, contentType, "POST");
			ASCIIEncoding asciiencoding = new ASCIIEncoding();
			byte[] bytes = asciiencoding.GetBytes(payLoad);
			TResult tresult;
			try
			{
				Stream stream = await http.GetRequestStreamAsync();
				stream.Write(bytes, 0, bytes.Length);
				stream.Close();
				TaskAwaiter<WebResponse> taskAwaiter = http.GetResponseAsync().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<WebResponse> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<WebResponse>);
				}
				tresult = JsonConvert.DeserializeObject<TResult>(new StreamReader(taskAwaiter.GetResult().GetResponseStream()).ReadToEnd());
			}
			catch (WebException ex)
			{
				if (ex.Response == null)
				{
					throw new Exception("HTTP Post Web Request Failed", ex);
				}
				HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
				if (httpWebResponse.StatusCode == HttpStatusCode.Unauthorized || httpWebResponse.StatusCode == HttpStatusCode.NotFound)
				{
					throw ex;
				}
				throw this.GetServiceErrorCode(ex.Response.GetResponseStream());
			}
			catch (Exception ex2)
			{
				throw new Exception("HTTP Post Failed", ex2);
			}
			return tresult;
		}

		public async Task Post(string service, List<KeyValuePair<string, string>> headers, string payLoad, string contentType = "application/json")
		{
			HttpWebRequest http = this.CreateHttpRequest(service, headers, contentType, "POST");
			ASCIIEncoding asciiencoding = new ASCIIEncoding();
			byte[] bytes = asciiencoding.GetBytes(payLoad);
			try
			{
				Stream stream = await http.GetRequestStreamAsync();
				stream.Write(bytes, 0, bytes.Length);
				stream.Close();
				await http.GetResponseAsync();
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
				{
					throw this.GetServiceErrorCode(ex.Response.GetResponseStream());
				}
				throw new Exception("HTTP Post Web Request Failed", ex);
			}
			catch (Exception ex2)
			{
				throw new Exception("HTTP Post Failed", ex2);
			}
		}

		private string _serviceAddress;
	}
}
