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
	public class DiamondClientApplication
	{
		public ApplicationVersion ApplicationVersion { get; private set; }

		public ParameterContainer Parameters
		{
			get
			{
				return this._parameters;
			}
		}

		public IReadOnlyDictionary<string, string> ProxyAddressMap { get; private set; }

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

		public DiamondClientApplication(ApplicationVersion applicationVersion)
			: this(applicationVersion, new ParameterContainer())
		{
		}

		public object GetObject(string name)
		{
			DiamondClientApplicationObject diamondClientApplicationObject;
			this._clientApplicationObjects.TryGetValue(name, out diamondClientApplicationObject);
			return diamondClientApplicationObject;
		}

		public void AddObject(string name, DiamondClientApplicationObject applicationObject)
		{
			this._clientApplicationObjects.Add(name, applicationObject);
		}

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

		private void CreateClient(string clientConfiguration, SessionProviderType sessionProviderType)
		{
			Type type = DiamondClientApplication.FindType(clientConfiguration);
			object obj = this.CreateClientSessionProvider(clientConfiguration, type, sessionProviderType, this._parameters);
			IClient client = (IClient)Activator.CreateInstance(type, new object[] { this, obj });
			this._clientObjects.Add(clientConfiguration, client);
		}

		private void CreateSessionlessClient(string clientConfiguration, SessionProviderType sessionProviderType)
		{
			Type type = DiamondClientApplication.FindType(clientConfiguration);
			object obj = this.CreateSessionlessClientDriverProvider(clientConfiguration, type, sessionProviderType, this._parameters);
			ISessionlessClient sessionlessClient = (ISessionlessClient)Activator.CreateInstance(type, new object[] { this, obj });
			this._sessionlessClientObjects.Add(clientConfiguration, sessionlessClient);
		}

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

		public T GetClient<T>(string name) where T : class, IClient
		{
			IClient client;
			if (this._clientObjects.TryGetValue(name, out client))
			{
				return client as T;
			}
			return default(T);
		}

		public T GetSessionlessClient<T>(string name) where T : class, ISessionlessClient
		{
			ISessionlessClient sessionlessClient;
			if (this._sessionlessClientObjects.TryGetValue(name, out sessionlessClient))
			{
				return sessionlessClient as T;
			}
			return default(T);
		}

		public void Update()
		{
			foreach (IClient client in this._clientObjects.Values)
			{
			}
		}

		private ParameterContainer _parameters;

		private Dictionary<string, DiamondClientApplicationObject> _clientApplicationObjects;

		private Dictionary<string, IClient> _clientObjects;

		private Dictionary<string, ISessionlessClient> _sessionlessClientObjects;
	}
}
