'  
'   Rebex Sample Code License
' 
'   Copyright 2023, REBEX CR s.r.o.
'   All rights reserved.
'   https://www.rebex.net/
' 
'   Permission to use, copy, modify, and/or distribute this software for any
'   purpose with or without fee is hereby granted.
' 
'   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
'   EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
'   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
'   NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
'   HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
'   WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
'   FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
'   OTHER DEALINGS IN THE SOFTWARE.
' 

Imports Rebex.Net
Imports Rebex.Security.Certificates

    ''' <summary>
    ''' Holds data of connection form.
    ''' </summary>
    Public Class ConnectionData
        ''' <summary>
        ''' Gets or sets server host address.
        ''' </summary>
        Public Property ServerHost() As String
            Get
                Return m_ServerHost
            End Get
            Set
                m_ServerHost = Value
            End Set
        End Property
        Private m_ServerHost As String

        ''' <summary>
        ''' Gets or sets server port.
        ''' </summary>
        Public Property ServerPort() As Integer
            Get
                Return m_ServerPort
            End Get
            Set
                m_ServerPort = Value
            End Set
        End Property
        Private m_ServerPort As Integer

        ''' <summary>
        ''' Gets or sets communication protocol.
        ''' </summary>
        Public Property Protocol() As ProtocolMode
            Get
                Return m_Protocol
            End Get
            Set
                m_Protocol = Value
            End Set
        End Property
        Private m_Protocol As ProtocolMode

        ''' <summary>
        ''' Gets or sets server login name.
        ''' </summary>
        Public Property ServerUser() As String
            Get
                Return m_ServerUser
            End Get
            Set
                m_ServerUser = Value
            End Set
        End Property
        Private m_ServerUser As String

        ''' <summary>
        ''' Gets or sets server login password.
        ''' </summary>
        Public Property ServerPassword() As String
            Get
                Return m_ServerPassword
            End Get
            Set
                m_ServerPassword = Value
            End Set
        End Property
        Private m_ServerPassword As String

        ''' <summary>
        ''' Gets or sets info, whether the connection password can be persisted to file.
        ''' </summary>
        Public Property StorePassword() As Boolean
            Get
                Return m_StorePassword
            End Get
            Set
                m_StorePassword = Value
            End Set
        End Property
        Private m_StorePassword As Boolean

        ''' <summary>
        ''' Gets or sets a value, indicating whether to use Single Sign On.
        ''' </summary>
        Public Property UseSingleSignOn() As Boolean
            Get
                Return m_UseSingleSignOn
            End Get
            Set
                m_UseSingleSignOn = Value
            End Set
        End Property
        Private m_UseSingleSignOn As Boolean

        ''' <summary>
        ''' Gets or sets shared mailbox.
        ''' </summary>
        Public Property Mailbox() As String
            Get
                Return m_Mailbox
            End Get
            Set
                m_Mailbox = Value
            End Set
        End Property
        Private m_Mailbox As String

        ''' <summary>
        ''' Gets or sets used proxy type (or <see cref="ProxyType.None"/> when no proxy is used).
        ''' </summary>
        Public Property ProxyType() As ProxyType
            Get
                Return m_ProxyType
            End Get
            Set
                m_ProxyType = Value
            End Set
        End Property
        Private m_ProxyType As ProxyType

        ''' <summary>
        ''' Gets or sets proxy host address.
        ''' </summary>
        Public Property ProxyHost() As String
            Get
                Return m_ProxyHost
            End Get
            Set
                m_ProxyHost = Value
            End Set
        End Property
        Private m_ProxyHost As String

        ''' <summary>
        ''' Gets or sets proxy port.
        ''' </summary>
        Public Property ProxyPort() As Integer
            Get
                Return m_ProxyPort
            End Get
            Set
                m_ProxyPort = Value
            End Set
        End Property
        Private m_ProxyPort As Integer

        ''' <summary>
        ''' Gets or sets proxy login name.
        ''' </summary>
        Public Property ProxyUser() As String
            Get
                Return m_ProxyUser
            End Get
            Set
                m_ProxyUser = Value
            End Set
        End Property
        Private m_ProxyUser As String

        ''' <summary>
        ''' Gets or sets proxy login password.
        ''' </summary>
        Public Property ProxyPassword() As String
            Get
                Return m_ProxyPassword
            End Get
            Set
                m_ProxyPassword = Value
            End Set
        End Property
        Private m_ProxyPassword As String

        ''' <summary>
        ''' Gets or sets hash value used for validating SSL server certificate 
        ''' </summary>
        Public Property ServerCertificateVerifyingMode() As CertificateVerifyingMode
            Get
                Return m_ServerCertificateVerifyingMode
            End Get
            Set
                m_ServerCertificateVerifyingMode = Value
            End Set
        End Property
        Private m_ServerCertificateVerifyingMode As CertificateVerifyingMode

        ''' <summary>
        ''' Gets or sets hash value used for validating SSL server certificate 
        ''' (when <see cref="ServerCertificateVerifyingMode"/> is <c>LocalyStoredThumbprint</c>).
        ''' </summary>
        Public Property ServerCertificateThumbprint() As String
            Get
                Return m_ServerCertificateThumbprint
            End Get
            Set
                m_ServerCertificateThumbprint = Value
            End Set
        End Property
        Private m_ServerCertificateThumbprint As String

        ''' <summary>
        ''' Gets or sets client certificate used for SSL authentication.
        ''' </summary>
        Public Property ClientCertificate() As CertificateChain
            Get
                Return m_ClientCertificate
            End Get
            Set
                m_ClientCertificate = Value
            End Set
        End Property
        Private m_ClientCertificate As CertificateChain

        ''' <summary>
        ''' Gets or sets client certificate filename.
        ''' </summary>
        Public Property ClientCertificateFilename() As String
            Get
                Return m_ClientCertificateFilename
            End Get
            Set
                m_ClientCertificateFilename = Value
            End Set
        End Property
        Private m_ClientCertificateFilename As String

        ''' <summary>
        ''' Gets or sets TLS/SSL protocol version.
        ''' </summary>
        Public Property TlsProtocol() As TlsVersion
            Get
                Return m_TlsProtocol
            End Get
            Set
                m_TlsProtocol = Value
            End Set
        End Property
        Private m_TlsProtocol As TlsVersion

        ''' <summary>
        ''' Gets or sets allowed secure suites.
        ''' </summary>
        Public Property AllowedSuite() As TlsCipherSuite
            Get
                Return m_AllowedSuite
            End Get
            Set
                m_AllowedSuite = Value
            End Set
        End Property
        Private m_AllowedSuite As TlsCipherSuite
    End Class
