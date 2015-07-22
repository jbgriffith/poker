using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Configuration;
using System.Reflection;
using System.Xml;

namespace Poker.Utils {
	public static class ConfigUtils {
		/// <summary>
		/// Replaces or adds connection strings in System.Configuration.ConfigurationManager.
		/// </summary>
		/// <param name="overrideConfigFile">The path of the override config file. Defaults to c:\vsOverride.config</param>
		/// <returns>The number of added/modified entries.</returns>
		public static int OverrideConnectionStrings(string overrideConfigFile = @"c:\vsOverride.config") {
			int numChanged = 0;

			if (File.Exists(overrideConfigFile)) {
				ConfigXmlDocument over = new ConfigXmlDocument(); // read override file into xml doc and select connectionString "add" nodes
				over.Load(overrideConfigFile);
				XmlNode xn = over.DocumentElement.SelectSingleNode("connectionStrings");
				if (xn == null) return 0;
				XmlNodeList xnl = xn.SelectNodes("add");
				foreach (XmlNode n in xnl) { // for each named connection string, override config file version or create new entry
					string name, conn, prov;
					try {
						name = n.Attributes.GetNamedItem("name").Value;
						conn = n.Attributes.GetNamedItem("connectionString").Value;
						prov = n.Attributes.GetNamedItem("providerName").Value;
					}
					catch { continue; } // don't really care what the problem is, just don't process it

					numChanged++;
					ConnectionStringSettings csSettings = ConfigurationManager.ConnectionStrings[name]; // attempt to find override name in config
					if (csSettings != null) { // found: overwrite the connectionString
						var f = typeof(ConfigurationElement).GetField("_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
						f.SetValue(csSettings, false); // enable modifications
						csSettings.ConnectionString = conn;
						csSettings.ProviderName = prov;
						f.SetValue(csSettings, true); // make it readonly again
					}
					else { // not found: make a new connectionString
						var f = typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
						f.SetValue(ConfigurationManager.ConnectionStrings, false); // enable modifications
						ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings(name, conn, prov));
						f.SetValue(ConfigurationManager.ConnectionStrings, true); // make it readonly again
					}
				}
			}
			return numChanged;
		}
	}

}
