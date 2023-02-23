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
Imports System.Windows.Forms

''' <summary>
''' Summary description for MailEditorMessageView.
''' </summary>
Public Class MailTextEditor
    Inherits System.Windows.Forms.Control

    Private Enum ViewType
        [Text]
        Html
    End Enum 'ViewType

    ' Control for message body.
    Private _view As TextBox = Nothing

    ' Indicates whether the message was modified.
    Private _modified As Boolean = False

    Private _currentView As ViewType = ViewType.Text


    Public Property Modified() As Boolean
        Get
            Return _modified
        End Get
        Set(ByVal Value As Boolean)
            _modified = Value
        End Set
    End Property


    Public WriteOnly Property [ReadOnly]() As Boolean
        Set(ByVal Value As Boolean)
            _view.Enabled = Not Value
        End Set
    End Property

    ''' <summary>
    ''' Constructor.
    ''' </summary>
    Public Sub New()
        _view = New TextBox
        _view.Multiline = True
        AddHandler _view.TextChanged, AddressOf _view_TextChanged
        _view.ScrollBars = ScrollBars.Both
        _view.Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Right Or AnchorStyles.Bottom
        Me.Controls.Add(_view)
    End Sub 'New

    ''' <summary>
    ''' Size of control.
    ''' </summary>
    Public Shadows Property Size() As System.Drawing.Size
        Get
            Return MyBase.Size
        End Get
        Set(ByVal Value As System.Drawing.Size)
            MyBase.Size = Value
            _view.Size = Value
        End Set
    End Property

    ''' <summary>
    ''' Cut to clipboard.
    ''' </summary>
    Public Sub Cut()
        Select Case _currentView
            Case ViewType.Text
                _view.Cut()
            Case ViewType.Html
        End Select
    End Sub 'Cut


    ''' <summary>
    ''' Copy to clipboard.
    ''' </summary>
    Public Sub Copy()
        Select Case _currentView
            Case ViewType.Text
                _view.Copy()
            Case ViewType.Html
        End Select
    End Sub 'Copy


    ''' <summary>
    ''' Paste from clipboard.
    ''' </summary>
    Public Sub Paste()
        Select Case _currentView
            Case ViewType.Text
                _view.Paste()
            Case ViewType.Html
        End Select
    End Sub 'Paste


    ''' <summary>
    ''' Select all data.
    ''' </summary>
    Public Sub SelectAll()
        Select Case _currentView
            Case ViewType.Text
                _view.SelectAll()
            Case ViewType.Html
        End Select
    End Sub 'SelectAll


    ''' <summary>
    ''' Add text to control.
    ''' </summary>
    ''' <param name="data"></param>
    Public Sub AppendText(ByVal data As String)
        Select Case _currentView
            Case ViewType.Text
                _view.AppendText(data)
            Case ViewType.Html
        End Select
    End Sub 'AppendText

    ''' <summary>
    ''' View text in control.
    ''' </summary>
    Public Overrides Property [Text]() As String
        Get
            Return _view.Text.Replace(ControlChars.Cr & ControlChars.Lf, ControlChars.Lf)
        End Get
        Set(ByVal Value As String)
            _view.Text = Value.Replace(ControlChars.Lf, ControlChars.Cr & ControlChars.Lf)
        End Set
    End Property


    Private Sub _view_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
        _modified = True
    End Sub '_view_TextChanged

End Class 'MailTextEditor
