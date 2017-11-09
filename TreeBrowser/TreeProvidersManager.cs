using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TreeBrowserPluginInterface;
using System.Linq;
using ConsoleUI;
using System.Diagnostics;

namespace TreeBrowser {
	static public class TreeProvidersManager {
		const string PLUGIN_DIR = "./plugins";
		private class Plugin {
			public AppDomain appDomain;
			public Assembly assembly;
			public List<ITreeProvider> treeProviders = new List<ITreeProvider>();
		}
		static private FileSystemWatcher fsWatcher;
		static private SortedList<string, Plugin> plugins = new SortedList<string, Plugin>();
		static private SortedList<string, ITreeProvider> providers = new SortedList<string, ITreeProvider>();
		static public bool RegisterProvider(ITreeProvider provider){
			string name = provider.GetProviderName();
			if (providers.ContainsKey(name)){
				int num = 1;
				name += " ({1})";
				while(providers.ContainsKey(string.Format(name, num))){
					num++;
				}
				name = string.Format(name, num);
			}
			providers.Add(name, provider);
			Debug.WriteLine("Registered TreeProvider \"{0}\"", new object[] { name });
			Trace.WriteLine("Registered TreeProvider \"" + name + "\"");
			return true;
		}

		static public bool RegisterProvider(string dll) {
			Plugin plugin = new Plugin();
			plugin.appDomain = AppDomain.CreateDomain(
				dll,
				AppDomain.CurrentDomain.Evidence,
				new AppDomainSetup() {
					ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
					ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
					LoaderOptimization = LoaderOptimization.MultiDomainHost,
					PrivateBinPath = PLUGIN_DIR
				}
			);
			Debug.WriteLine("New AppDomain");
			Debug.Indent();
			Debug.WriteLine("FriendlyName = {0}", new object[]{plugin.appDomain.FriendlyName });
			Debug.WriteLine("ApplicationBase = {0}", new object[] { plugin.appDomain.SetupInformation.ApplicationBase });
			Debug.WriteLine("ConfigurationFile = {0}", new object[] { plugin.appDomain.SetupInformation.ConfigurationFile });
			Debug.WriteLine("PrivateBinPath = {0}", new object[] { plugin.appDomain.SetupInformation.PrivateBinPath });
			Debug.Unindent();
			dll = Path.GetFullPath(Path.Combine(PLUGIN_DIR, dll));
			//plugin.appDomain.Load(File.ReadAllBytes("TreeBrowserPluginInterface.dll"));
			//plugin.appDomain.Load(File.ReadAllBytes(dll));
			plugin.assembly = Assembly.LoadFrom(dll);
			if(plugin.assembly != null){
				Trace.WriteLine("Assembly \"" + dll + "\" loaded.");
				Debug.WriteLine("Assembly \"{0}\" loaded.", new object[] { dll });
				Debug.Indent();
				Debug.WriteLine(plugin.assembly.FullName);
				Debug.Unindent();
			} else {
				Trace.WriteLine("Assembly \"" + dll + "\" could not be loaded.");
				Debug.WriteLine("Assembly \"{0}\" could not be loaded.", new object[] { dll });
				return false;
			}
			bool somethingLoaded = false;
			foreach(Type t in plugin.assembly.GetTypes()){
				if(t.IsAbstract){
					Debug.WriteLine("Type \"{0}\" is abstract.", new object[] { t.Name });
					continue;
				}
				if(!t.IsMarshalByRef){
					Debug.WriteLine("Type \"{0}\" is not MarshalByRef.", new object[] { t.Name });
					continue;
				}
				if(t.GetInterfaces().Contains(typeof(ITreeProvider))){
					object prov = plugin.appDomain.CreateInstanceAndUnwrap(
						plugin.assembly.FullName,
						t.FullName
					);
					RegisterProvider((ITreeProvider)prov);
					plugin.treeProviders.Add((ITreeProvider)prov);
					somethingLoaded = true;
					Trace.WriteLine("Type \"" + t.Name + "\" implements interface.", t.Name);
					Debug.WriteLine("Type \"{0}\" implements interface.", new object[] { t.Name });
				} else {
					Debug.WriteLine("Type \"{0}\" does not implements interface.", new object[] { t.Name });
				}
			}
			if(somethingLoaded){
				plugins.Add(plugin.appDomain.FriendlyName, plugin);
			}
			return somethingLoaded;
		}

		static public bool UnregisterProvider(string dll){
			Plugin p = plugins[dll];
			if (p == null)
				return false;
			foreach (ITreeProvider tp in p.treeProviders) {
				Trace.WriteLine("Unregistered provider \"" + tp.GetProviderName() + "\".");
				Debug.WriteLine("Unregistered provider \"{0}\".", new object[] { tp.GetProviderName() });
				providers.Remove(providers.Where((arg) => arg.Value == tp).First().Key);
			}
			Debug.WriteLine("Unloaded AppDomain \"{0}\".", new object[] { p.appDomain.FriendlyName });
			AppDomain.Unload(p.appDomain);
			return true;
		}

		static public SortedList<string, ITreeProvider> GetProviders(){
			return providers;
		}

		static public void LoadPlugins(){
			Trace.WriteLine("Loading plugins...");
			Trace.Indent();
			Debug.WriteLine("Loading plugins...");
			Debug.Indent();
			DirectoryInfo di = new DirectoryInfo(PLUGIN_DIR);
			foreach (FileInfo fi in di.GetFiles()) {
				if (fi.Extension == ".dll"){
					RegisterProvider(fi.Name);
				}
			}
			Trace.Unindent();
			Trace.Flush();
			Debug.Unindent();
			Debug.Flush();
		}

		static public void StartFSWatcher(){
			fsWatcher = new FileSystemWatcher(PLUGIN_DIR, "*.dll");

			fsWatcher.Created += (sender, e) => {
				Debug.WriteLine("FSWatcher - Created");
				Debug.Indent();
				if(RegisterProvider(e.Name)){
					ConsoleManager.ProvidersChanged();
				}
				Debug.Unindent();
			};

			fsWatcher.Deleted += (sender, e) => {
				Debug.WriteLine("FSWatcher - Deleted");
				Debug.Indent();
				if(UnregisterProvider(e.Name)){
					ConsoleManager.ProvidersChanged();
				}
				Debug.Unindent();
			};
			fsWatcher.EnableRaisingEvents = true;
		}
	}
}
