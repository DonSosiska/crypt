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
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using Rebex.Net;

namespace Rebex.Samples
{
    /// <summary>
    /// Helper class for manipulating data.
    /// </summary>
    internal static class ConnectionEditorUtils
    {
        public const int HttpDefaultPort = 80;
        public const int HttpsDefaultPort = 443;
        public const int MinPort = 1;
        public const int MaxPort = 0xFFFF;

        /// <summary>
        /// Binds list of given ComboBox to some values of given enum.<br/>
        /// (DataMember uses enum VALUES, DisplayMember uses enum NAMES or DESCRIPTIONS (if available).
        /// </summary>
        /// <typeparam name="EnumType">Type of enum the values are taken from.</typeparam>
        /// <param name="combo">Instance of ComboBox to bind into.</param>
        /// <param name="excludeValues">Collection of enum values that will be excluded from the bound values.<br/>May be null or empty.</param>
        public static void BindComboWithEnum<EnumType>(ComboBox combo, ICollection<EnumType> excludeValues)
        {
            if (excludeValues == null)
                excludeValues = new List<EnumType>();

            List<Tuple<EnumType, string>> lst = new List<Tuple<EnumType, string>>();
            foreach (EnumType value in Enum.GetValues(typeof(EnumType)))
                if (!excludeValues.Contains(value))
                    lst.Add(new Tuple<EnumType, string>(value, GetEnumValueDescription(value)));

            combo.ValueMember = "First";
            combo.DisplayMember = "Second";
            combo.DataSource = lst;
        }
        /// <summary>
        /// Returns value of <see cref="DescriptionAttribute"/> placed on given enum field.<br/>
        /// If there is no <see cref="DescriptionAttribute"/> on that field, a field name is returned.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>Field name or field description, never null.</returns>
        internal static string GetEnumValueDescription<T>(T value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        /// <summary>
        /// Encapsulates side effects when setting the protocol.
        /// </summary>
        /// <param name="oldMode">Current protocol mode.</param>
        /// <param name="newMode">New protocol mode to be set.</param>
        /// <param name="setProtocolDelegate">Delegate that sets the protocol to the target property.</param>
        /// <param name="serverPort">Current value of server port.</param>
        /// <param name="setServerPortDelegate">Delegate that sets the port to the target property.</param>
        /// <param name="proxyType">Current value of proxy type.</param>
        /// <param name="setProxyTypeDelegate">Delegate that sets the proxy type to the target property.</param>
        public static void SetProtocol(
            ProtocolMode oldMode,
            ProtocolMode newMode,
            Action<ProtocolMode> setProtocolDelegate,
            int serverPort,
            Action<int> setServerPortDelegate)
        {
            int newServerPort = serverPort;

            // if the previous protocol uses it's default port...
            if (serverPort == 0 ||
                (oldMode == ProtocolMode.HTTP && serverPort == HttpDefaultPort) ||
                (oldMode == ProtocolMode.HTTPS && serverPort == HttpsDefaultPort))
            {
                // ...set the default port also for the new protocol
                switch (newMode)
                {
                    case ProtocolMode.HTTP:
                        newServerPort = HttpDefaultPort;
                        break;
                    case ProtocolMode.HTTPS:
                        newServerPort = HttpsDefaultPort;
                        break;
                }
            }

            if (newMode != oldMode)
                setProtocolDelegate(newMode);
            if (newServerPort != serverPort)
                setServerPortDelegate(newServerPort);
        }

        public static void ShowException(string message, string caption, Exception ex)
        {
            MessageBox.Show(string.Format("{0}\r\n{1}\r\n{2}", message, "Detailed description:", ex.Message), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowWarning(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

    }

    internal class Tuple<T1, T2>
    {
        public T1 First { get { return _first; } }
        private readonly T1 _first;
        public T2 Second { get { return _second; } }
        private readonly T2 _second;
        public Tuple(T1 first, T2 second)
        {
            _first = first;
            _second = second;
        }
    }

}
