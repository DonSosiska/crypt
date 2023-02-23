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

Imports System.IO
Imports System.Windows.Forms
Imports System.Xml
Imports System.Collections.Generic
Imports System.ComponentModel


''' <summary>
''' Represents a connection dialog for connecting to an EWS server.
''' </summary>
Partial Public Class ConnectionForm
    Inherits Form
    Private ReadOnly _data As ConnectionFormData

    ''' <summary>
    ''' Gets the data holder object.
    ''' </summary>
    Public ReadOnly Property Data() As ConnectionFormData
        Get
            Return _data
        End Get
    End Property

    Private Sub New()
        Me.New(New ConnectionFormData())
        If LicenseManager.UsageMode <> LicenseUsageMode.Designtime Then
            Throw New InvalidOperationException("Don't use the parameter-less constructor. It is intended for Visual Studio design mode only.")
        End If
    End Sub

    ''' <summary>
    ''' Initializes new instance of the <see cref="ConnectionForm"/>.
    ''' </summary>
    Public Sub New(data As ConnectionFormData)
        If data Is Nothing Then
            Throw New ArgumentNullException()
        End If

        _data = data

        InitializeComponent()

        ucConnectionEditor.AdaptToCurrentVisibility()

        SetData()
    End Sub

    ''' <summary>
    ''' Propagate current form fields values to the <see cref="Data"/> object.
    ''' </summary>
    Private Sub GetData()
        ucConnectionEditor.GetData(_data)
    End Sub

    ''' <summary>
    ''' Propagate current <see cref="Data"/> values to form fields.
    ''' </summary>
    Private Sub SetData()
        ucConnectionEditor.SetData(_data)
    End Sub

    ''' <summary>
    ''' Handles 'Connect' button click event..
    ''' </summary>
    Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
        Dim errors As List(Of String) = ucConnectionEditor.GetValidationErrors()
        If errors.Count > 0 Then
            Dim msg As String = String.Join(vbCr & vbLf, errors.ToArray())
            ConnectionEditorUtils.ShowWarning(msg, "Invalid data")
            Return
        End If

        DialogResult = DialogResult.OK
        Close()
    End Sub

    ''' <summary>
    ''' Handles 'Esc' key press event.
    ''' </summary>
    Private Sub ConnectionForm_KeyPress(sender As Object, e As KeyPressEventArgs) Handles MyBase.KeyPress
        ' when 'esc' is pressed the form is closed
        If e.KeyChar = Convert.ToChar(Keys.Escape) Then
            DialogResult = DialogResult.Cancel
            Close()
        End If
    End Sub

    ''' <summary>
    ''' Handles form closing event.
    ''' </summary>
    Private Sub ConnectionForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' we gather values from form-fields to the Data object regardless the closing reason
        GetData()
    End Sub
End Class

