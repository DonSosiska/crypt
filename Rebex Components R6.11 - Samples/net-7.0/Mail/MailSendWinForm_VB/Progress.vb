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
Imports Rebex.Net
Imports Rebex.Mail


''' <summary>
''' Transfer progress dialog.
''' </summary>
Public Class Progress
    Inherits System.Windows.Forms.Form
    Private lblStatus As System.Windows.Forms.Label
    Private pbMain As System.Windows.Forms.ProgressBar
    Private WithEvents btnOK As System.Windows.Forms.Button


    Public Sub New(ByVal smtp As Smtp)
        InitializeComponent()

        CheckForIllegalCrossThreadCalls = true

        ' bind handler
        AddHandler smtp.TransferProgress, AddressOf client_TransferProgressProxy
    End Sub 'New


    Public Sub Unbind(ByVal smtp As Smtp)
        ' unbind handler
        RemoveHandler smtp.TransferProgress, AddressOf client_TransferProgressProxy
    End Sub 'Unbind


    Public Sub SetFinished()
        pbMain.Value = pbMain.Maximum
        lblStatus.Text = "Mail message was sent successfully."
        btnOK.Visible = True
    End Sub 'SetFinished

#Region "Windows Form Designer generated code"

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.pbMain = New System.Windows.Forms.ProgressBar
        Me.lblStatus = New System.Windows.Forms.Label
        Me.btnOK = New System.Windows.Forms.Button
        Me.SuspendLayout()
        ' 
        ' pbMain
        ' 
        Me.pbMain.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pbMain.Location = New System.Drawing.Point(0, 63)
        Me.pbMain.Name = "pbMain"
        Me.pbMain.Size = New System.Drawing.Size(298, 25)
        Me.pbMain.TabIndex = 0
        ' 
        ' lblStatus
        ' 
        Me.lblStatus.Location = New System.Drawing.Point(8, 8)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(280, 24)
        Me.lblStatus.TabIndex = 1
        Me.lblStatus.Text = "Preparing email..."
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter
        ' 
        ' btnOK
        ' 
        Me.btnOK.Location = New System.Drawing.Point(104, 32)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.TabIndex = 2
        Me.btnOK.Text = "OK"
        Me.btnOK.Visible = False
        ' 
        ' Progress
        ' 
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(298, 88)
        Me.ControlBox = False
        Me.Controls.Add(btnOK)
        Me.Controls.Add(lblStatus)
        Me.Controls.Add(pbMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "Progress"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Progress"
        Me.ResumeLayout(False)
    End Sub 'InitializeComponent 
#End Region


    Private Sub btnOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Me.Close()
    End Sub 'btnOK_Click


    Public Property MessageLength() As Long
        Get
            Return pbMain.Maximum
        End Get
        Set(ByVal Value As Long)
            If Value > Integer.MaxValue Then
                pbMain.Maximum = Integer.MaxValue
            Else
                pbMain.Maximum = CInt(Value)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Because the TransferProgress event (which is handled by this method) will be triggered
    ''' while sending a message asynchronously using the Smtp.BeginSend method, it will run in
    ''' a background thread. Because we should only access methods (except Invoke) and properties
    ''' of any WinForm control from the thread that created and owns the control, we must pass
    ''' the event arguments to the <see cref="client_TransferProgress"/> method that runs in the
    ''' control's thread. This can be accomplished easily using the Invoke method.
    ''' </summary>
    ''' <param name="sender">Event sender.</param>
    ''' <param name="e">Event arguments.</param>
    Private Sub client_TransferProgressProxy(ByVal sender As Object, ByVal e As SmtpTransferProgressEventArgs)
        ' pass the arguments to a method that runs in the thread that owns this control
        Invoke(New EventHandler(Of SmtpTransferProgressEventArgs)(AddressOf client_TransferProgress), New Object() {sender, e})
    End Sub 'client_TransferProgressProxy


    ''' <summary>
    ''' This method is called through the Invoke method by <see cref="client_TransferProgressProxy"/>
    ''' when it receives a TransferProgress event. Therefore, it will always run in the main
    ''' application thread that owns this control and we can safely access properties of its child
    ''' controls.
    ''' </summary>
    ''' <param name="sender">Event sender.</param>
    ''' <param name="e">Event arguments.</param>
    Private Sub client_TransferProgress(ByVal sender As Object, ByVal e As SmtpTransferProgressEventArgs)
        If e.State = SmtpTransferState.None Then
            Return
        End If
        ' update the status and progress bar according to event data
        lblStatus.Text = String.Format("{0}... {1} bytes transferred.", e.State, e.BytesTransferred)

        If CInt(e.BytesTransferred) > pbMain.Maximum Then
            pbMain.Value = pbMain.Maximum
        Else
            pbMain.Value = CInt(e.BytesTransferred)
        End If
    End Sub 'client_TransferProgress
End Class 'Progress
