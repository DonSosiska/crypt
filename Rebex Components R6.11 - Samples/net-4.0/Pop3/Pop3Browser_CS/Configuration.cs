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
using System.Globalization;
using System.Collections.Generic;

namespace Rebex.Samples
{
    /// <summary>
    /// Class for loading and saving simple configuration files.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Filename for I/O operations.
        /// </summary>
        private readonly string _fileName;

        /// <summary>
        /// Hashtable for in-memory config values storage.
        /// </summary>
        private readonly Dictionary<string, string> _entries;

        /// <summary>
        /// Gets an Int32 value from the configuration.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>An Int32 value from the configuration.</returns>
        public int GetInt32(string key, int defaultValue)
        {
            try
            {
                string val = GetString(key);
                if (val == null)
                    return defaultValue;
                return Convert.ToInt32(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets the Int32 value from the configuration.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>An Int32 value from the configuration.</returns>
        public int GetInt32(string key)
        {
            return (int)GetValue(key, typeof(int));
        }

        /// <summary>
        /// Gets an Boolean value from the configuration.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>An Boolean value from the configuration.</returns>
        public bool GetBoolean(string key, bool defaultValue)
        {
            try
            {
                string val = GetString(key);
                if (val == null)
                    return defaultValue;
                return Convert.ToBoolean(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets the string value from the configuration.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>An string value from the configuration.</returns>
        public string GetString(string key, string defaultValue)
        {
            object o = GetValue(key, typeof(string));

            if (o == null)
                return defaultValue;
            else
                return o.ToString();
        }

        /// <summary>
        /// Gets the string value from the configuration.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>An string value from the configuration.</returns>
        public string GetString(string key)
        {
            return GetString(key, null);
        }

        /// <summary>
        /// Gets the object value from the configuration.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="type">The type.</param>
        /// <returns>Value read from the configuration.</returns>
        public object GetValue(string key, Type type)
        {
            if (key == null)
                throw new ArgumentNullException();

            string value;
            _entries.TryGetValue(key, out value);

            if (type.IsEnum)
            {
                if (value == null)
                    return Enum.ToObject(type, 0);

                return Enum.Parse(type, value.ToString(), true);
            }

            if (value == null)
                return null;

            try
            {
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            catch
            {
                // ignore unsupported types or values
                return null;
            }
        }

        /// <summary>
        /// Sets the specified configuration value.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void SetValue(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException();

            if (value != null)
            {
                Type type = value.GetType();
                if (type.IsEnum)
                    _entries[key] = Convert.ChangeType(value, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture).ToString();
                else if (value is string)
                    _entries[key] = (string)value;
                else
                    _entries[key] = string.Format(CultureInfo.InvariantCulture, "{0}", value);
            }
            else
            {
                _entries.Remove(key);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="fileName">The filename where configuration is stored.</param>
        public Configuration(string fileName)
        {
            _fileName = fileName;
            _entries = new Dictionary<string, string>();
            Load();
        }

        /// <summary>
        /// Saves the configuration to a file.
        /// </summary>
        public void Save()
        {
            var xml = new XmlDocument();
            XmlElement config = xml.CreateElement("configuration");
            xml.AppendChild(config);

            foreach (var entry in _entries)
            {
                XmlElement key = xml.CreateElement("key");
                config.AppendChild(key);

                XmlAttribute atrName = xml.CreateAttribute("name");
                atrName.Value = entry.Key;
                key.Attributes.Append(atrName);

                XmlAttribute atrVal = xml.CreateAttribute("value");
                atrVal.Value = entry.Value;
                key.Attributes.Append(atrVal);
            }

            string configPath = Path.GetDirectoryName(_fileName);
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }

            xml.Save(_fileName);
        }

        /// <summary>
        /// Loads the configuration from a file.
        /// </summary>
        private void Load()
        {
            if (!File.Exists(_fileName))
            {
                return;
            }

            var xml = new XmlDocument();
            xml.Load(_fileName);
            XmlElement config = xml["configuration"];

            _entries.Clear();

            foreach (XmlNode key in config.ChildNodes)
            {
                string item = null;
                string name = null;

                if (key["value"] != null)
                    item = key["value"].InnerText;
                else if (key.Attributes["value"] != null)
                    item = key.Attributes["value"].Value;

                if (key["name"] != null)
                    name = key["name"].InnerText;
                else if (key.Attributes["name"] != null)
                    name = key.Attributes["name"].Value;

                if (name != null && item != null)
                {
                    _entries.Add(name, item);
                }
            }
        }
    }
}
