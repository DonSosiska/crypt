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
using Rebex.Net;
using Rebex.Security.Certificates;

namespace Rebex.Samples
{
    /// <summary>
    /// Holds data of connection form.
    /// </summary>
    public class ConnectionData
    {
        /// <summary>
        /// Gets or sets server host address.
        /// </summary>
        public string ServerHost { get; set; }

        /// <summary>
        /// Gets or sets server port.
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        /// Gets or sets communication protocol.
        /// </summary>
        public ProtocolMode Protocol { get; set; }

        /// <summary>
        /// Gets or sets server login name.
        /// </summary>
        public string ServerUser { get; set; }

        /// <summary>
        /// Gets or sets server login password.
        /// </summary>
        public string ServerPassword { get; set; }

        /// <summary>
        /// Gets or sets info, whether the connection password can be persisted to file.
        /// </summary>
        public bool StorePassword { get; set; }

        /// <summary>
        /// Gets or sets a value, indicating whether to use Single Sign On.
        /// </summary>
        public bool UseSingleSignOn { get; set; }

        /// <summary>
        /// Gets or sets shared mailbox.
        /// </summary>
        public string Mailbox { get; set; }

        /// <summary>
        /// Gets or sets used proxy type (or <see cref="ProxyType.None"/> when no proxy is used).
        /// </summary>
        public ProxyType ProxyType { get; set; }

        /// <summary>
        /// Gets or sets proxy host address.
        /// </summary>
        public string ProxyHost { get; set; }

        /// <summary>
        /// Gets or sets proxy port.
        /// </summary>
        public int ProxyPort { get; set; }

        /// <summary>
        /// Gets or sets proxy login name.
        /// </summary>
        public string ProxyUser { get; set; }

        /// <summary>
        /// Gets or sets proxy login password.
        /// </summary>
        public string ProxyPassword { get; set; }

        /// <summary>
        /// Gets or sets hash value used for validating SSL server certificate 
        /// </summary>
        public CertificateVerifyingMode ServerCertificateVerifyingMode { get; set; }

        /// <summary>
        /// Gets or sets hash value used for validating SSL server certificate 
        /// (when <see cref="ServerCertificateVerifyingMode"/> is <c>LocalyStoredThumbprint</c>).
        /// </summary>
        public string ServerCertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets client certificate used for SSL authentication.
        /// </summary>
        public CertificateChain ClientCertificate { get; set; }

        /// <summary>
        /// Gets or sets client certificate filename.
        /// </summary>
        public string ClientCertificateFilename { get; set; }

        /// <summary>
        /// Gets or sets TLS/SSL protocol version.
        /// </summary>
        public TlsVersion TlsProtocol { get; set; }		

        /// <summary>
        /// Gets or sets allowed secure suites.
        /// </summary>
        public TlsCipherSuite AllowedSuite { get; set; }
    }
}
