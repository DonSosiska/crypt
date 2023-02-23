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

Imports System
Imports System.Xml
Imports System.IO
Imports System.Globalization
Imports System.Collections.Generic

''' <summary>
''' Class for loading and saving simple configuration files.
''' </summary>
Public Class Configuration

    ''' <summary>
    ''' Filename for I/O operations.
    ''' </summary>
    Private ReadOnly _fileName As String

    ''' <summary>
    ''' Hashtable for in-memory config values storage.
    ''' </summary>
    Private ReadOnly _entries As Dictionary(Of String, String)

    ''' <summary>
    ''' Gets an Int32 value from the configuration.
    ''' </summary>
    ''' <param name="key">The key.</param>
    ''' <param name="defaultValue">Default value.</param>
    ''' <returns>An Int32 value from the configuration.</returns>
    Public Function GetInt32(ByVal key As String, ByVal defaultValue As Integer) As Integer
        Try
            Dim val As String = GetString(key)
            If val Is Nothing Then Return defaultValue
            Return Convert.ToInt32(val)
        Catch
            Return defaultValue
        End Try
    End Function

    ''' <summary>
    ''' Gets the Int32 value from the configuration.
    ''' </summary>
    ''' <param name="key">The key.</param>
    ''' <returns>An Int32 value from the configuration.</returns>
    Public Function GetInt32(ByVal key As String) As Integer
        Return CInt(GetValue(key, GetType(Integer)))
    End Function

    ''' <summary>
    ''' Gets an Boolean value from the configuration.
    ''' </summary>
    ''' <param name="key">The key.</param>
    ''' <param name="defaultValue">Default value.</param>
    ''' <returns>An Boolean value from the configuration.</returns>
    Public Function GetBoolean(ByVal key As String, ByVal defaultValue As Boolean) As Boolean
        Try
            Dim val As String = GetString(key)
            If val Is Nothing Then Return defaultValue
            Return Convert.ToBoolean(val)
        Catch
            Return defaultValue
        End Try
    End Function

    ''' <summary>
    ''' Gets the string value from the configuration.
    ''' </summary>
    ''' <param name="key">The key.</param>
    ''' <returns>An string value from the configuration.</returns>
    Public Function GetString(ByVal key As String, ByVal defaultValue As String) As String
        Dim o As Object = GetValue(key, GetType(String))

        If o Is Nothing Then
            Return defaultValue
        Else
            Return o.ToString()
        End If
    End Function

    ''' <summary>
    ''' Gets the string value from the configuration.
    ''' </summary>
    ''' <param name="key">The key.</param>
    ''' <returns>An string value from the configuration.</returns>
    Public Function GetString(ByVal key As String) As String
        Return GetString(key, Nothing)
    End Function


    ''' <summary>
    ''' Gets the object value from the configuration.
    ''' </summary>
    ''' <param name="key">The key.</param>
    ''' <param name="type">The type.</param>
    ''' <returns>Value read from the configuration.</returns>
    Public Function GetValue(ByVal key As String, ByVal type As Type) As Object
        If key Is Nothing Then Throw New ArgumentNullException()
        Dim value As String = Nothing
        _entries.TryGetValue(key, value)

        If type.IsEnum Then
            If value Is Nothing Then Return [Enum].ToObject(type, 0)
            Return [Enum].Parse(type, value.ToString(), True)
        End If

        If value Is Nothing Then Return Nothing

        Try
            Return Convert.ChangeType(value, type, CultureInfo.InvariantCulture)
        Catch
            ' ignore unsupported types or values
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Sets the specified configuration value.
    ''' </summary>
    ''' <param name="key">Key.</param>
    ''' <param name="value">Value.</param>
    Public Sub SetValue(ByVal key As String, ByVal value As Object)
        If key Is Nothing Then Throw New ArgumentNullException()

        If value IsNot Nothing Then
            Dim type As Type = value.[GetType]()

            If type.IsEnum Then
                _entries(key) = Convert.ChangeType(value, [Enum].GetUnderlyingType(type), CultureInfo.InvariantCulture).ToString()
            ElseIf TypeOf value Is String Then
                _entries(key) = CStr(value)
            Else
                _entries(key) = String.Format(CultureInfo.InvariantCulture, "{0}", value)
            End If
        Else
            _entries.Remove(key)
        End If
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the <see cref="Configuration"/> class.
    ''' </summary>
    ''' <param name="fileName">The filename where configuration is stored.</param>
    Public Sub New(ByVal fileName As String)
        _fileName = fileName
        _entries = New Dictionary(Of String, String)()
        Load()
    End Sub

    ''' <summary>
    ''' Saves the configuration to a file.
    ''' </summary>
    Public Sub Save()
        Dim xml As New XmlDocument()
        Dim config As XmlElement = xml.CreateElement("configuration")
        xml.AppendChild(config)

        For Each entry As KeyValuePair(Of String, String) In _entries
            Dim key As XmlElement = xml.CreateElement("key")
            config.AppendChild(key)
            Dim atrName As XmlAttribute = xml.CreateAttribute("name")
            atrName.Value = entry.Key
            key.Attributes.Append(atrName)
            Dim atrVal As XmlAttribute = xml.CreateAttribute("value")
            atrVal.Value = entry.Value
            key.Attributes.Append(atrVal)
        Next

        Dim configPath As String = Path.GetDirectoryName(_fileName)

        If Not Directory.Exists(configPath) Then
            Directory.CreateDirectory(configPath)
        End If

        xml.Save(_fileName)
    End Sub

    ''' <summary>
    ''' Loads the configuration from a file.
    ''' </summary>
    Private Sub Load()
        If Not File.Exists(_fileName) Then
            Return
        End If

        Dim xml As New XmlDocument()
        xml.Load(_fileName)
        Dim config As XmlElement = xml("configuration")
        _entries.Clear()

        For Each key As XmlNode In config.ChildNodes
            Dim item As String = Nothing
            Dim name As String = Nothing

            If key("value") IsNot Nothing Then
                item = key("value").InnerText
            ElseIf key.Attributes("value") IsNot Nothing Then
                item = key.Attributes("value").Value
            End If

            If key("name") IsNot Nothing Then
                name = key("name").InnerText
            ElseIf key.Attributes("name") IsNot Nothing Then
                name = key.Attributes("name").Value
            End If

            If name IsNot Nothing AndAlso item IsNot Nothing Then
                _entries.Add(name, item)
            End If
        Next
    End Sub
End Class
