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
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.IO
Imports System.Text
Imports Rebex.Mime

Public Class Viewer
    Inherits System.Windows.Forms.Form

    Private _image As Image = Nothing
    Private _control As Control = Nothing

    Public Sub New(ByVal entity As MimeEntity)
        Me.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        '
        ' Image entity
        '
        If entity.ContentType.MediaType.StartsWith("image/") Then
            Dim source As Stream = entity.GetContentStream()
            Try
                _image = New Bitmap([source])
            Catch
                _image = Nothing
            Finally
                source.Close()
            End Try

            If Not (_image Is Nothing) Then
                AutoScroll = True
                Dim mw As Integer = 0 'AutoScrollMargin.Width;
                Dim mh As Integer = 0 'AutoScrollMargin.Height;
                ClientSize = New Size(Math.Min(_image.Width + mw, 640), Math.Min(_image.Height + mh, 480))

                Dim box As New PictureBox
                box.SetBounds(0, 0, _image.Width, _image.Height)
                box.Parent = Me
                box.Image = _image
                Return
            End If
        End If

        AutoScroll = False
        Dim txt As New TextBox
        txt.BackColor = Color.White
        txt.Multiline = True
        txt.ReadOnly = True
        txt.ScrollBars = ScrollBars.Both
        txt.SetBounds(0, 0, ClientSize.Width, ClientSize.Height)
        txt.Font = New Font(FontFamily.GenericMonospace, 10)

        '
        ' Text entity
        '
        txt.Text = FormatMimeEntity(entity)
        txt.Select(0, 0)
        txt.Parent = Me
        _control = txt
    End Sub

#Region "Windows Form Designer generated code"

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        ' 
        ' Viewer
        ' 
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(632, 453)
        Me.Name = "Viewer"
        Me.Text = "Viewer"

    End Sub

#End Region


    Private Sub Viewer_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.SizeChanged
        If Not (_control Is Nothing) Then
            _control.Size = ClientSize
        End If
    End Sub 'Viewer_SizeChanged

    Private Function FormatMimeEntity(ByVal entity As MimeEntity) As String

        If (entity Is Nothing) Then
            Return ""
        End If

        Dim source As Stream = entity.GetContentStream()
        Try
            If entity.ContentType.MediaType.StartsWith("text/") Then
                'patch text data for textbox
                If (entity.ContentString <> Nothing) Then
                    Return entity.ContentString.Replace(vbLf, vbCrLf)
                Else
                    Return ""
                End If
            End If

            Dim sb As New StringBuilder(CInt([source].Length) * 3)
            Dim b As Integer
            Dim n As Integer = 0
            b = source.ReadByte()

            While (b >= 0)
                sb.AppendFormat("{0:x2}", b)
                n += 1
                If n Mod 8 = 0 Then sb.Append(" ")

                If n = 40000 Then
                    sb.Append(ControlChars.Cr & ControlChars.Lf & ControlChars.Cr & ControlChars.Lf & "And more data follows...")
                    Exit While
                End If
                b = source.ReadByte()

            End While
            Return sb.ToString()
        Finally
            source.Close()
        End Try
    End Function
End Class 'Viewer
