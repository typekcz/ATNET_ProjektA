using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TreeBrowserPluginInterface;
using System.Linq;
using ConsoleUI;

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
			Program.log.WriteLine("Registered TreeProvider \"{0}\"", name);
			return true;
		}

		static public bool RegisterProvider(string dll){
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
			dll = Path.GetFullPath(Path.Combine(PLUGIN_DIR, dll));
			//plugin.appDomain.Load(File.ReadAllBytes("TreeBrowserPluginInterface.dll"));
			//plugin.appDomain.Load(File.ReadAllBytes(dll));
			plugin.assembly = Assembly.LoadFrom(dll);
			if(plugin.assembly != null){
				Program.log.WriteLine("Assembly \"{0}\" loaded.", dll);
			} else {
				Program.log.WriteLine("Assembly \"{0}\" could not be loaded.", dll);
				return false;
			}
			bool somethingLoaded = false;
			foreach(Type t in plugin.assembly.GetTypes()){
				if(t.IsAbstract){
					Program.log.WriteLine("Type \"{0}\" is abstract.", t);
					continue;
				}
				if(!t.IsMarshalByRef){
					Program.log.WriteLine("Type \"{0}\" is not MarshalByRef.", t);
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
					Program.log.WriteLine("Type \"{0}\" implements interface.", t);
				} else {
					Program.log.WriteLine("Type \"{0}\" does not implements interface.", t);
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
				providers.Remove(providers.Where((arg) => arg.Value == tp).First().Key);
			}
			AppDomain.Unload(p.appDomain);
			return true;
		}

		static public SortedList<string, ITreeProvider> GetProviders(){
			return providers;
		}

		static public void LoadPlugins(){
			Program.log.WriteLine(" --- LoadPlugins --- ");
			DirectoryInfo di = new DirectoryInfo(PLUGIN_DIR);
			foreach (FileInfo fi in di.GetFiles()) {
				if (fi.Extension == ".dll"){
					RegisterProvider(fi.Name);
				}
			}
		}

		static public void StartFSWatcher(){
			fsWatcher = new FileSystemWatcher(PLUGIN_DIR, "*.dll");

			fsWatcher.Created += (sender, e) => {
				Program.log.WriteLine(" --- FSWatcher --- ");
				if(RegisterProvider(e.Name)){
					ConsoleManager.ProvidersChanged();
				}
			};

			fsWatcher.Deleted += (sender, e) => {
				Program.log.WriteLine(" --- FSWatcher --- ");
				if(UnregisterProvider(e.Name)){
					ConsoleManager.ProvidersChanged();
				}
			};
			fsWatcher.EnableRaisingEvents = true;
		}
	}
}
