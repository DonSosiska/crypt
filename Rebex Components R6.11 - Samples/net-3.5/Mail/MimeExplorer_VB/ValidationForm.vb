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

Imports Rebex.Security.Certificates
Imports Rebex.Security.Cryptography.Pkcs

Public Class ValidationForm
    Inherits System.Windows.Forms.Form

    Public Sub New(ByVal result As SignatureValidationResult)
        MyBase.New()
        InitializeComponent()

        If result Is Nothing Then
            Return
        End If

        Dim certStatus As ValidationStatus = result.CertificateValidationStatus
        Dim vs As ValidationStatus
        For Each vs In [Enum].GetValues(GetType(ValidationStatus))
            If (vs And certStatus) <> 0 Then
                textCertProblem.Text += ValidationHelper.GetCertificateValidationStatusDescription(vs) + vbCrLf
            End If
        Next

        Dim signStatus As SignatureValidationStatus = result.Status
        Dim ss As SignatureValidationStatus
        For Each ss In [Enum].GetValues(GetType(SignatureValidationStatus))
            If (ss And signStatus) <> 0 Then
                textSignProblem.Text += ValidationHelper.GetSignatureValidationStatusDescription(ss) + vbCrLf
            End If
        Next
    End Sub

#Region " Windows Form Designer generated code "

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
    Friend WithEvents textSignProblem As System.Windows.Forms.TextBox
    Friend WithEvents label1 As System.Windows.Forms.Label
    Friend WithEvents panel2 As System.Windows.Forms.Panel
    Friend WithEvents textCertProblem As System.Windows.Forms.TextBox
    Friend WithEvents label6 As System.Windows.Forms.Label
    Friend WithEvents btnClose As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.textSignProblem = New System.Windows.Forms.TextBox
        Me.label1 = New System.Windows.Forms.Label
        Me.panel2 = New System.Windows.Forms.Panel
        Me.textCertProblem = New System.Windows.Forms.TextBox
        Me.label6 = New System.Windows.Forms.Label
        Me.btnClose = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'textSignProblem
        '
        Me.textSignProblem.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.textSignProblem.Location = New System.Drawing.Point(8, 160)
        Me.textSignProblem.Multiline = True
        Me.textSignProblem.Name = "textSignProblem"
        Me.textSignProblem.ReadOnly = True
        Me.textSignProblem.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.textSignProblem.Size = New System.Drawing.Size(384, 88)
        Me.textSignProblem.TabIndex = 14
        Me.textSignProblem.Text = ""
        '
        'label1
        '
        Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.label1.Location = New System.Drawing.Point(8, 8)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(384, 24)
        Me.label1.TabIndex = 13
        Me.label1.Text = "Signature validation problems:"
        '
        'panel2
        '
        Me.panel2.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.panel2.Location = New System.Drawing.Point(8, 128)
        Me.panel2.Name = "panel2"
        Me.panel2.Size = New System.Drawing.Size(380, 1)
        Me.panel2.TabIndex = 5
        '
        'textCertProblem
        '
        Me.textCertProblem.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.textCertProblem.Location = New System.Drawing.Point(8, 32)
        Me.textCertProblem.Multiline = True
        Me.textCertProblem.Name = "textCertProblem"
        Me.textCertProblem.ReadOnly = True
        Me.textCertProblem.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.textCertProblem.Size = New System.Drawing.Size(384, 88)
        Me.textCertProblem.TabIndex = 12
        Me.textCertProblem.Text = ""
        '
        'label6
        '
        Me.label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.label6.Location = New System.Drawing.Point(8, 136)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(392, 24)
        Me.label6.TabIndex = 11
        Me.label6.Text = "Certificate validation problems:"
        '
        'btnClose
        '
        Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnClose.Location = New System.Drawing.Point(312, 256)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.TabIndex = 6
        Me.btnClose.Text = "Close"
        '
        'ValidationForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(394, 288)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.label6)
        Me.Controls.Add(Me.panel2)
        Me.Controls.Add(Me.textSignProblem)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.textCertProblem)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        Me.ShowInTaskbar = False
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(424, 332)
        Me.MinimumSize = New System.Drawing.Size(424, 332)
        Me.Name = "ValidationForm"
        Me.Text = "Signature Is Not Valid"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

End Class
