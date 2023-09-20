using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using TaleWorlds.Diamond.InnerProcess;
using TaleWorlds.Library;
using TaleWorlds.Library.Http;
using TaleWorlds.ServiceDiscovery.Client;

namespace TaleWorlds.Diamond.ClientApplication
{
	// Token: 0x02000055 RID: 85
	public class DiamondClientApplication
	{
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060001F8 RID: 504 RVA: 0x00005C50 File Offset: 0x00003E50
		// (set) Token: 0x060001F9 RID: 505 RVA: 0x00005C58 File Offset: 0x00003E58
		public ApplicationVersion ApplicationVersion { get; private set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060001FA RID: 506 RVA: 0x00005C61 File Offset: 0x00003E61
		public ParameterContainer Parameters
		{
			get
			{
				return this._parameters;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060001FB RID: 507 RVA: 0x00005C69 File Offset: 0x00003E69
		// (set) Token: 0x060001FC RID: 508 RVA: 0x00005C71 File Offset: 0x00003E71
		public IReadOnlyDictionary<string, string> ProxyAddressMap { get; private set; }

		// Token: 0x060001FD RID: 509 RVA: 0x00005C7C File Offset: 0x00003E7C
		public DiamondClientApplication(ApplicationVersion applicationVersion, ParameterContainer parameters)
		{
			this.ApplicationVersion = applicationVersion;
			this._parameters = parameters;
			this._clientApplicationObjects = new Dictionary<string, DiamondClientApplicationObject>();
			this._clientObjects = new Dictionary<string, IClient>();
			this._sessionlessClientObjects = new Dictionary<string, ISessionlessClient>();
			this.ProxyAddressMap = new Dictionary<string, string>();
			ServicePointManager.DefaultConnectionLimit = 1000;
			ServicePointManager.Expect100Continue = false;
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00005CD9 File Offset: 0x00003ED9
		public DiamondClientApplication(ApplicationVersion applicationVersion)
			: this(applicationVersion, new ParameterContainer())
		{
		}

		// Token: 0x060001FF RID: 511 RVA: 0x00005CE8 File Offset: 0x00003EE8
		public object GetObject(string name)
		{
			DiamondClientApplicationObject diamondClientApplicationObject;
			this._clientApplicationObjects.TryGetValue(name, out diamondClientApplicationObject);
			return diamondClientApplicationObject;
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00005D05 File Offset: 0x00003F05
		public void AddObject(string name, DiamondClientApplicationObject applicationObject)
		{
			this._clientApplicationObjects.Add(name, applicationObject);
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00005D14 File Offset: 0x00003F14
		public void Initialize(ClientApplicationConfiguration applicationConfiguration)
		{
			this._parameters = applicationConfiguration.Parameters;
			foreach (string text in applicationConfiguration.Clients)
			{
				this.CreateClient(text, applicationConfiguration.SessionProviderType);
			}
			foreach (string text2 in applicationConfiguration.SessionlessClients)
			{
				this.CreateSessionlessClient(text2, applicationConfiguration.SessionProviderType);
			}
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00005D7C File Offset: 0x00003F7C
		private void CreateClient(string clientConfiguration, SessionProviderType sessionProviderType)
		{
			Type type = DiamondClientApplication.FindType(clientConfiguration);
			object obj = this.CreateClientSessionProvider(clientConfiguration, type, sessionProviderType, this._parameters);
			IClient client = (IClient)Activator.CreateInstance(type, new object[] { this, obj });
			this._clientObjects.Add(clientConfiguration, client);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00005DC8 File Offset: 0x00003FC8
		private void CreateSessionlessClient(string clientConfiguration, SessionProviderType sessionProviderType)
		{
			Type type = DiamondClientApplication.FindType(clientConfiguration);
			object obj = this.CreateSessionlessClientDriverProvider(clientConfiguration, type, sessionProviderType, this._parameters);
			ISessionlessClient sessionlessClient = (ISessionlessClient)Activator.CreateInstance(type, new object[] { this, obj });
			this._sessionlessClientObjects.Add(clientConfiguration, sessionlessClient);
		}

		// Token: 0x06000204 RID: 516 RVA: 0x00005E14 File Offset: 0x00004014
		public object CreateSessionlessClientDriverProvider(string clientName, Type clientType, SessionProviderType sessionProviderType, ParameterContainer parameters)
		{
			if (sessionProviderType == SessionProviderType.Rest || sessionProviderType == SessionProviderType.ThreadedRest)
			{
				Type type = typeof(GenericRestSessionlessClientDriverProvider<>).MakeGenericType(new Type[] { clientType });
				string text;
				parameters.TryGetParameter(clientName + ".Address", out text);
				ushort num;
				parameters.TryGetParameterAsUInt16(clientName + ".Port", out num);
				bool flag;
				parameters.TryGetParameterAsBool(clientName + ".IsSecure", out flag);
				if (ServiceAddress.IsServiceAddress(text))
				{
					string text2;
					parameters.TryGetParameter(clientName + ".ServiceDiscovery.Address", out text2);
					ServiceAddressManager.ResolveAddress(text2, text, ref text, ref num, ref flag);
				}
				string text3;
				IHttpDriver httpDriver;
				if (parameters.TryGetParameter(clientName + ".HttpDriver", out text3))
				{
					httpDriver = HttpDriverManager.GetHttpDriver(text3);
				}
				else
				{
					httpDriver = HttpDriverManager.GetDefaultHttpDriver();
				}
				return Activator.CreateInstance(type, new object[] { text, num, flag, httpDriver });
			}
			throw new NotImplementedException("Other session provider types are not supported yet.");
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00005F18 File Offset: 0x00004118
		public object CreateClientSessionProvider(string clientName, Type clientType, SessionProviderType sessionProviderType, ParameterContainer parameters)
		{
			object obj;
			if (sessionProviderType == SessionProviderType.Rest || sessionProviderType == SessionProviderType.ThreadedRest)
			{
				Type type = ((sessionProviderType == SessionProviderType.Rest) ? typeof(GenericRestSessionProvider<>) : typeof(GenericThreadedRestSessionProvider<>)).MakeGenericType(new Type[] { clientType });
				string text;
				parameters.TryGetParameter(clientName + ".Address", out text);
				ushort num;
				parameters.TryGetParameterAsUInt16(clientName + ".Port", out num);
				bool flag;
				parameters.TryGetParameterAsBool(clientName + ".IsSecure", out flag);
				if (ServiceAddress.IsServiceAddress(text))
				{
					string text2;
					parameters.TryGetParameter(clientName + ".ServiceDiscovery.Address", out text2);
					ServiceAddressManager.ResolveAddress(text2, text, ref text, ref num, ref flag);
				}
				string text3 = clientName + ".Proxy.";
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (KeyValuePair<string, string> keyValuePair in parameters.Iterator)
				{
					if (keyValuePair.Key.StartsWith(text3) && keyValuePair.Key.Length > text3.Length)
					{
						dictionary[keyValuePair.Key.Substring(text3.Length)] = keyValuePair.Value;
					}
				}
				this.ProxyAddressMap = dictionary;
				string text4;
				if (dictionary.TryGetValue(text, out text4))
				{
					text = text4;
				}
				string text5;
				IHttpDriver httpDriver;
				if (parameters.TryGetParameter(clientName + ".HttpDriver", out text5))
				{
					httpDriver = HttpDriverManager.GetHttpDriver(text5);
				}
				else
				{
					httpDriver = HttpDriverManager.GetDefaultHttpDriver();
				}
				obj = Activator.CreateInstance(type, new object[] { text, num, flag, httpDriver });
			}
			else
			{
				if (sessionProviderType != SessionProviderType.InnerProcess)
				{
					throw new NotImplementedException("Other session provider types are not supported yet.");
				}
				InnerProcessManager innerProcessManager = ((InnerProcessManagerClientObject)this.GetObject("InnerProcessManager")).InnerProcessManager;
				Type type2 = typeof(GenericInnerProcessSessionProvider<>).MakeGenericType(new Type[] { clientType });
				ushort num2;
				parameters.TryGetParameterAsUInt16(clientName + ".Port", out num2);
				obj = Activator.CreateInstance(type2, new object[] { innerProcessManager, num2 });
			}
			return obj;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x00006148 File Offset: 0x00004348
		private static Assembly[] GetDiamondAssemblies()
		{
			List<Assembly> list = new List<Assembly>();
			Assembly assembly = typeof(PeerId).Assembly;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			list.Add(assembly);
			foreach (Assembly assembly2 in assemblies)
			{
				AssemblyName[] referencedAssemblies = assembly2.GetReferencedAssemblies();
				for (int j = 0; j < referencedAssemblies.Length; j++)
				{
					if (referencedAssemblies[j].ToString() == assembly.GetName().ToString())
					{
						list.Add(assembly2);
						break;
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000207 RID: 519 RVA: 0x000061D8 File Offset: 0x000043D8
		private static Type FindType(string name)
		{
			Assembly[] diamondAssemblies = DiamondClientApplication.GetDiamondAssemblies();
			Type type = null;
			Assembly[] array = diamondAssemblies;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (Type type2 in array[i].GetTypes())
				{
					if (type2.Name == name)
					{
						type = type2;
					}
				}
			}
			return type;
		}

		// Token: 0x06000208 RID: 520 RVA: 0x00006230 File Offset: 0x00004430
		public T GetClient<T>(string name) where T : class, IClient
		{
			IClient client;
			if (this._clientObjects.TryGetValue(name, out client))
			{
				return client as T;
			}
			return default(T);
		}

		// Token: 0x06000209 RID: 521 RVA: 0x00006264 File Offset: 0x00004464
		public T GetSessionlessClient<T>(string name) where T : class, ISessionlessClient
		{
			ISessionlessClient sessionlessClient;
			if (this._sessionlessClientObjects.TryGetValue(name, out sessionlessClient))
			{
				return sessionlessClient as T;
			}
			return default(T);
		}

		// Token: 0x0600020A RID: 522 RVA: 0x00006298 File Offset: 0x00004498
		public void Update()
		{
			foreach (IClient client in this._clientObjects.Values)
			{
			}
		}

		// Token: 0x040000B8 RID: 184
		private ParameterContainer _parameters;

		// Token: 0x040000B9 RID: 185
		private Dictionary<string, DiamondClientApplicationObject> _clientApplicationObjects;

		// Token: 0x040000BA RID: 186
		private Dictionary<string, IClient> _clientObjects;

		// Token: 0x040000BB RID: 187
		private Dictionary<string, ISessionlessClient> _sessionlessClientObjects;
	}
}
