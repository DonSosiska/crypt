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

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Reflection
Imports System.Windows.Forms
Imports Rebex.Net


''' <summary>
''' Helper class for manipulating data.
''' </summary>
Friend NotInheritable Class ConnectionEditorUtils
    Private Sub New()
    End Sub
    Public Const HttpDefaultPort As Integer = 80
    Public Const HttpsDefaultPort As Integer = 443
    Public Const MinPort As Integer = 1
    Public Const MaxPort As Integer = &HFFFF

    ''' <summary>
    ''' Binds list of given ComboBox to some values of given enum.<br/>
    ''' (DataMember uses enum VALUES, DisplayMember uses enum NAMES or DESCRIPTIONS (if available).
    ''' </summary>
    ''' <typeparam name="EnumType">Type of enum the values are taken from.</typeparam>
    ''' <param name="combo">Instance of ComboBox to bind into.</param>
    ''' <param name="excludeValues">Collection of enum values that will be excluded from the bound values.<br/>May be null or empty.</param>
    Public Shared Sub BindComboWithEnum(Of EnumType)(combo As ComboBox, excludeValues As ICollection(Of EnumType))
        If excludeValues Is Nothing Then
            excludeValues = New List(Of EnumType)()
        End If

        Dim lst As New List(Of Tuple(Of EnumType, String))()
        For Each value As EnumType In [Enum].GetValues(GetType(EnumType))
            If Not excludeValues.Contains(value) Then
                lst.Add(New Tuple(Of EnumType, String)(value, GetEnumValueDescription(value)))
            End If
        Next

        combo.ValueMember = "First"
        combo.DisplayMember = "Second"
        combo.DataSource = lst
    End Sub
    ''' <summary>
    ''' Returns value of <see cref="DescriptionAttribute"/> placed on given enum field.<br/>
    ''' If there is no <see cref="DescriptionAttribute"/> on that field, a field name is returned.
    ''' </summary>
    ''' <typeparam name="T">Enum type</typeparam>
    ''' <returns>Field name or field description, never null.</returns>
    Friend Shared Function GetEnumValueDescription(Of T)(value As T) As String
        Dim field As FieldInfo = value.[GetType]().GetField(value.ToString())
        Dim dattribute As DescriptionAttribute = TryCast(Attribute.GetCustomAttribute(field, GetType(DescriptionAttribute)), DescriptionAttribute)
        Return If(dattribute Is Nothing, value.ToString(), dattribute.Description)
    End Function

    ''' <summary>
    ''' Encapsulates side effects when setting the protocol.
    ''' </summary>
    ''' <param name="oldMode">Current protocol mode.</param>
    ''' <param name="newMode">New protocol mode to be set.</param>
    ''' <param name="setProtocolDelegate">Delegate that sets the protocol to the target property.</param>
    ''' <param name="serverPort">Current value of server port.</param>
    ''' <param name="setServerPortDelegate">Delegate that sets the port to the target property.</param>
    ''' <param name="proxyType">Current value of proxy type.</param>
    ''' <param name="setProxyTypeDelegate">Delegate that sets the proxy type to the target property.</param>
    Public Shared Sub SetProtocol(oldMode As ProtocolMode, newMode As ProtocolMode, setProtocolDelegate As Action(Of ProtocolMode), serverPort As Integer, setServerPortDelegate As Action(Of Integer))
        Dim newServerPort As Integer = serverPort

        ' if the previous protocol uses it's default port...
        If serverPort = 0 OrElse (oldMode = ProtocolMode.HTTP AndAlso serverPort = HttpDefaultPort) OrElse (oldMode = ProtocolMode.HTTPS AndAlso serverPort = HttpsDefaultPort) Then
            ' ...set the default port also for the new protocol
            Select Case newMode
                Case ProtocolMode.HTTP
                    newServerPort = HttpDefaultPort
                    Exit Select
                Case ProtocolMode.HTTPS
                    newServerPort = HttpsDefaultPort
                    Exit Select
            End Select
        End If

        If newMode <> oldMode Then
            setProtocolDelegate(newMode)
        End If
        If newServerPort <> serverPort Then
            setServerPortDelegate(newServerPort)
        End If
    End Sub

    Public Shared Sub ShowException(message As String, caption As String, ex As Exception)
        MessageBox.Show(String.Format("{0}" & vbCr & vbLf & "{1}" & vbCr & vbLf & "{2}", message, "Detailed description:", ex.Message), caption, MessageBoxButtons.OK, MessageBoxIcon.[Error])
    End Sub

    Public Shared Sub ShowWarning(message As String, caption As String)
        MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End Sub

End Class

Friend Class Tuple(Of T1, T2)
    Public ReadOnly Property First() As T1
        Get
            Return _first
        End Get
    End Property
    Private ReadOnly _first As T1
    Public ReadOnly Property Second() As T2
        Get
            Return _second
        End Get
    End Property
    Private ReadOnly _second As T2
    Public Sub New(first As T1, second As T2)
        _first = first
        _second = second
    End Sub
End Class


