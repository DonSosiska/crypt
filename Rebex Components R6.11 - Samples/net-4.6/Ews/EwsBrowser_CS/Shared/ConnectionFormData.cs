//  
//   Rebex Sample Code License
// 
//   Copyright 2023, REBEX CR s.r.o.
//   All rights reserved.
//   https://www.rebex.net/
// 
//   Permission to use, copy, modify, and/or distribute this software for any
//   purpose with or without fee is hereby granted.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//   EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//   NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//   HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//   WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//   FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//   OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using System.Xml;
using System.IO;
using Rebex.Net;
using System.Text;

namespace Rebex.Samples
{
    /// <summary>
    /// ConnectionEditor data extended for specific fields and operations used in Ews samples.
    /// </summary>
    public class ConnectionFormData : ConnectionData
    {
        /// <summary>
        /// Create a new instance and fill it with defaults.
        /// </summary>
        public ConnectionFormData()
        {
            // set defaults
            Protocol = ProtocolMode.HTTPS;
            ServerHost = "outlook.office365.com";
            ServerPort = ConnectionEditorUtils.HttpsDefaultPort;
            StorePassword = true;
            ProxyType = ProxyType.None;
            ServerCertificateVerifyingMode = CertificateVerifyingMode.UseWindowsInfrastructure;
            TlsProtocol = TlsVersion.TLS13 | TlsVersion.TLS12;
            AllowedSuite = TlsCipherSuite.All;
        }

        /// <summary>
        /// Load data from a component-specific config file.
        /// </summary>
        public void LoadConfig()
        {
            XmlDocument xml = new XmlDocument();
            if (!File.Exists(ConfigFile))
                return;

            xml.Load(ConfigFile);

            XmlElement config = xml["configuration"];
            foreach (XmlNode key in config.ChildNodes)
            {
                string item = null;
                string name = null;
                if (key.Attributes["value"] != null)
                    item = key.Attributes["value"].Value;
                if (key.Attributes["name"] != null)
                    name = key.Attributes["name"].Value;
                if (key["value"] != null)
                    item = key["value"].InnerText;
                if (key["name"] != null)
                    name = key["name"].InnerText;

                if (name == null)
                    continue;
                if (item == null)
                    item = "";

                switch (name)
                {
                    case "protocol": Protocol = ToEnum(item, Protocol); break;
                    case "host": ServerHost = item; break;
                    case "port": ServerPort = ToInt(item, ServerPort); break;
                    case "login": ServerUser = item; break;
                    case "password":
                        ServerPassword = item;
                        StorePassword = !string.IsNullOrEmpty(ServerPassword);
                        break;
                    case "singleSignOn": UseSingleSignOn = ToBoolean(item); break;
                    case "mailbox": Mailbox = item; break;
                    case "proxyType": ProxyType = ToEnum(item, ProxyType); break;
                    case "proxyHost": ProxyHost = item; break;
                    case "proxyPort": ProxyPort = ToInt(item, ProxyPort); break;
                    case "proxyLogin": ProxyUser = item; break;
                    case "proxyPassword": ProxyPassword = item; break;
                    case "serverCertificateThumbprint": ServerCertificateThumbprint = item; break;
                    case "serverCertificateVerifyingMode": ServerCertificateVerifyingMode = ToEnum(item, ServerCertificateVerifyingMode); break;
                    case "clientCertificateFilename": ClientCertificateFilename = item; break;
                    case "tlsSslVersion": TlsProtocol = ToEnum(item, TlsProtocol); break;
                    case "allowedSuite": AllowedSuite = ToEnum(item, AllowedSuite); break;
                }
            }
        }

        /// <summary>
        /// Save values into config file.
        /// </summary>
        public void SaveConfig()
        {
            XmlDocument xml = new XmlDocument();
            XmlElement config = xml.CreateElement("configuration");
            xml.AppendChild(config);

            AddKey(xml, config, "protocol", Protocol.ToString());
            AddKey(xml, config, "host", ServerHost);
            AddKey(xml, config, "port", ServerPort.ToString());
            AddKey(xml, config, "login", ServerUser);
            string pwd = StorePassword ? ServerPassword : "";
            AddKey(xml, config, "password", pwd);
            AddKey(xml, config, "singleSignOn", UseSingleSignOn.ToString());
            AddKey(xml, config, "mailbox", Mailbox);
            AddKey(xml, config, "proxyType", ProxyType.ToString());
            AddKey(xml, config, "proxyHost", ProxyHost);
            AddKey(xml, config, "proxyPort", ProxyPort.ToString());
            AddKey(xml, config, "proxyLogin", ProxyUser);
            AddKey(xml, config, "proxyPassword", ProxyPassword);
            AddKey(xml, config, "serverCertificateThumbprint", ServerCertificateThumbprint);
            AddKey(xml, config, "serverCertificateVerifyingMode", ServerCertificateVerifyingMode.ToString());
            AddKey(xml, config, "clientCertificateFilename", ClientCertificateFilename);
            AddKey(xml, config, "tlsSslVersion", ToString(TlsProtocol));
            AddKey(xml, config, "allowedSuite", AllowedSuite.ToString());

            string configPath = Path.GetDirectoryName(ConfigFile);
            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);
            xml.Save(ConfigFile);
        }

        #region Utility methods

        private string ConfigFile
        {
            get
            {
                string filename = string.Format(@"Rebex{0}Secure Mail{0}EwsBrowser.xml", Path.DirectorySeparatorChar);
                return Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), filename);
            }
        }

        private static int ToInt(string value, int defaultValue)
        {
            try
            {
                return Int32.Parse(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        private static Boolean ToBoolean(string value)
        {
            try
            {
                return Boolean.Parse(value);
            }
            catch
            {
                return false;
            }
        }

        private static T ToEnum<T>(string value, T defaultValue)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value);
            }
            catch
            {
                return defaultValue;
            }
        }

        private static string ToString(TlsVersion tlsVersion)
        {
            StringBuilder sb = new StringBuilder();
            bool addSeparator = false;

            Action<TlsVersion> convert = (tv =>
            {
                if ((tlsVersion & tv) != 0)
                {
                    if (addSeparator)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(tv.ToString());
                    addSeparator = true;
                }
            });

            convert(TlsVersion.TLS13);
            convert(TlsVersion.TLS12);
            convert(TlsVersion.TLS11);
            convert(TlsVersion.TLS10);

            return sb.ToString();
        }

        private static void AddKey(XmlDocument xml, XmlElement config, string name, string val)
        {
            XmlElement key = xml.CreateElement("key");
            config.AppendChild(key);
            XmlAttribute atrName = xml.CreateAttribute("name");
            atrName.Value = name;
            XmlAttribute atrVal = xml.CreateAttribute("value");
            atrVal.Value = val;
            key.Attributes.Append(atrName);
            key.Attributes.Append(atrVal);
        }

        #endregion
    }
}
