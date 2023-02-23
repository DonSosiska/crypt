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

Imports System.Xml
Imports System.IO
Imports Rebex.Net
Imports System.Text

''' <summary>
''' ConnectionEditor data extended for specific fields and operations used in Ews samples.
''' </summary>
Public Class ConnectionFormData
    Inherits ConnectionData
    ''' <summary>
    ''' Create a new instance and fill it with defaults.
    ''' </summary>
    Public Sub New()
        ' set defaults
        Protocol = ProtocolMode.HTTPS
        ServerHost = "outlook.office365.com"
        ServerPort = ConnectionEditorUtils.HttpsDefaultPort
        StorePassword = True
        ProxyType = ProxyType.None
        ServerCertificateVerifyingMode = CertificateVerifyingMode.UseWindowsInfrastructure
        TlsProtocol = TlsVersion.TLS13 Or TlsVersion.TLS12
        AllowedSuite = TlsCipherSuite.All
    End Sub

    ''' <summary>
    ''' Load data from a component-specific config file.
    ''' </summary>
    Public Sub LoadConfig()
        Dim xml As New XmlDocument()
        If Not File.Exists(ConfigFile) Then
            Return
        End If

        xml.Load(ConfigFile)

        Dim config As XmlElement = xml("configuration")
        For Each key As XmlNode In config.ChildNodes
            Dim item As String = Nothing
            Dim name As String = Nothing
            If key.Attributes("value") IsNot Nothing Then
                item = key.Attributes("value").Value
            End If
            If key.Attributes("name") IsNot Nothing Then
                name = key.Attributes("name").Value
            End If
            If key("value") IsNot Nothing Then
                item = key("value").InnerText
            End If
            If key("name") IsNot Nothing Then
                name = key("name").InnerText
            End If

            If name Is Nothing Then
                Continue For
            End If
            If item Is Nothing Then
                item = ""
            End If

            Select Case name
                Case "protocol"
                    Protocol = ToEnum(item, Protocol)
                    Exit Select
                Case "host"
                    ServerHost = item
                    Exit Select
                Case "port"
                    ServerPort = ToInt(item, ServerPort)
                    Exit Select
                Case "login"
                    ServerUser = item
                    Exit Select
                Case "password"
                    ServerPassword = item
                    StorePassword = Not String.IsNullOrEmpty(ServerPassword)
                    Exit Select
                Case "singleSignOn"
                    UseSingleSignOn = ToBoolean(item)
                    Exit Select
                Case "mailbox"
                    Mailbox = item
                    Exit Select
                Case "proxyType"
                    ProxyType = ToEnum(item, ProxyType)
                    Exit Select
                Case "proxyHost"
                    ProxyHost = item
                    Exit Select
                Case "proxyPort"
                    ProxyPort = ToInt(item, ProxyPort)
                    Exit Select
                Case "proxyLogin"
                    ProxyUser = item
                    Exit Select
                Case "proxyPassword"
                    ProxyPassword = item
                    Exit Select
                Case "serverCertificateThumbprint"
                    ServerCertificateThumbprint = item
                    Exit Select
                Case "serverCertificateVerifyingMode"
                    ServerCertificateVerifyingMode = ToEnum(item, ServerCertificateVerifyingMode)
                    Exit Select
                Case "clientCertificateFilename"
                    ClientCertificateFilename = item
                    Exit Select
                Case "tlsSslVersion"
                    TlsProtocol = ToEnum(item, TlsProtocol)
                    Exit Select
                Case "allowedSuite"
                    AllowedSuite = ToEnum(item, AllowedSuite)
                    Exit Select
            End Select
        Next
    End Sub

    ''' <summary>
    ''' Save values into config file.
    ''' </summary>
    Public Sub SaveConfig()
        Dim xml As New XmlDocument()
        Dim config As XmlElement = xml.CreateElement("configuration")
        xml.AppendChild(config)

        AddKey(xml, config, "protocol", Protocol.ToString())
        AddKey(xml, config, "host", ServerHost)
        AddKey(xml, config, "port", ServerPort.ToString())
        AddKey(xml, config, "login", ServerUser)
        Dim pwd As String = If(StorePassword, ServerPassword, "")
        AddKey(xml, config, "password", pwd)
        AddKey(xml, config, "singleSignOn", UseSingleSignOn.ToString())
        AddKey(xml, config, "mailbox", Mailbox)
        AddKey(xml, config, "proxyType", ProxyType.ToString())
        AddKey(xml, config, "proxyHost", ProxyHost)
        AddKey(xml, config, "proxyPort", ProxyPort.ToString())
        AddKey(xml, config, "proxyLogin", ProxyUser)
        AddKey(xml, config, "proxyPassword", ProxyPassword)
        AddKey(xml, config, "serverCertificateThumbprint", ServerCertificateThumbprint)
        AddKey(xml, config, "serverCertificateVerifyingMode", ServerCertificateVerifyingMode.ToString())
        AddKey(xml, config, "clientCertificateFilename", ClientCertificateFilename)
        AddKey(xml, config, "tlsSslVersion", VersionToString(TlsProtocol))
        AddKey(xml, config, "allowedSuite", AllowedSuite.ToString())

        Dim configPath As String = Path.GetDirectoryName(ConfigFile)
        If Not Directory.Exists(configPath) Then
            Directory.CreateDirectory(configPath)
        End If
        xml.Save(ConfigFile)
    End Sub

#Region "Utility methods"

    Private ReadOnly Property ConfigFile() As String
        Get
            Dim filename As String = String.Format("Rebex{0}Secure Mail{0}EwsBrowser.xml", Path.DirectorySeparatorChar)
            Return Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), filename)
        End Get
    End Property

    Private Shared Function ToInt(value As String, defaultValue As Integer) As Integer
        Try
            Return Int32.Parse(value)
        Catch
            Return defaultValue
        End Try
    End Function

    Private Shared Function ToBoolean(value As String) As [Boolean]
        Try
            Return [Boolean].Parse(value)
        Catch
            Return False
        End Try
    End Function

    Private Shared Function ToEnum(Of T)(value As String, defaultValue As T) As T
        Try
            Return DirectCast([Enum].Parse(GetType(T), value), T)
        Catch
            Return defaultValue
        End Try
    End Function

    Private Shared Function VersionToString(tlsVer As TlsVersion) As String

        Dim sb As StringBuilder = New StringBuilder()
        Dim addSeparator As Boolean = False

        Dim convert As Action(Of TlsVersion) =
            Sub(tv As TlsVersion)
                If ((tlsVer And tv) <> 0) Then

                    If (addSeparator) Then
                        sb.Append(", ")
                    End If
                    sb.Append(tv.ToString())
                    addSeparator = True
                End If
            End Sub

        convert(TlsVersion.TLS13)
        convert(TlsVersion.TLS12)
        convert(TlsVersion.TLS11)
        convert(TlsVersion.TLS10)

        Return sb.ToString()
    End Function

    Private Shared Sub AddKey(xml As XmlDocument, config As XmlElement, name As String, val As String)
        Dim key As XmlElement = xml.CreateElement("key")
        config.AppendChild(key)
        Dim atrName As XmlAttribute = xml.CreateAttribute("name")
        atrName.Value = name
        Dim atrVal As XmlAttribute = xml.CreateAttribute("value")
        atrVal.Value = val
        key.Attributes.Append(atrName)
        key.Attributes.Append(atrVal)
    End Sub

#End Region
End Class

