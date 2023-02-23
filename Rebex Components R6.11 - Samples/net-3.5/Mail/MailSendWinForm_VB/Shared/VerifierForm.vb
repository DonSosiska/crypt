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

Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms

Imports Rebex.Security.Certificates

''' <summary>
''' Summary description for Verifier.
''' </summary>
Public Class VerifierForm
    Inherits System.Windows.Forms.Form
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.Container = Nothing

    Private panel1 As System.Windows.Forms.Panel
    Private panel2 As System.Windows.Forms.Panel
    Private label5 As System.Windows.Forms.Label
    Private label4 As System.Windows.Forms.Label
    Private label3 As System.Windows.Forms.Label
    Private label2 As System.Windows.Forms.Label
    Private label1 As System.Windows.Forms.Label
    Private lblProblem As System.Windows.Forms.Label
    Private lblSubject As System.Windows.Forms.Label
    Private lblIssuer As System.Windows.Forms.Label
    Private lblValidFrom As System.Windows.Forms.Label
    Private lblValidTo As System.Windows.Forms.Label
    Private label6 As System.Windows.Forms.Label
    Private lblHostname As System.Windows.Forms.Label
    Private label8 As System.Windows.Forms.Label
    Private label9 As System.Windows.Forms.Label
    Private txtThumbprint As System.Windows.Forms.TextBox
    Private WithEvents btnReject As System.Windows.Forms.Button
    Private WithEvents btnAccept As System.Windows.Forms.Button
    Private WithEvents btnOkAndTrust As System.Windows.Forms.Button

    Private _accepted As Boolean = False
    Private _addIssuerCertificateAuthothorityToTrustedCaStore As Boolean = False

    ''' <summary>
    ''' Gets or sets a problem to display.
    ''' </summary>
    Public Property Problem() As String
        Get
            Return lblProblem.Text
        End Get
        Set(value As String)
            lblProblem.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether to display the 'Accept and Add' button.
    ''' </summary>
    Public Property ShowAddIssuerToTrustedCaStoreButton() As Boolean
        Get
            Return btnOkAndTrust.Visible
        End Get
        Set(value As Boolean)
            btnOkAndTrust.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the certificate was accepted.
    ''' </summary>
    Public ReadOnly Property Accepted() As Boolean
        Get
            Return _accepted
        End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the 'Accept and Add' button was clicked.
    ''' </summary>
    Public ReadOnly Property AddIssuerCertificateAuthothorityToTrustedCaStore() As Boolean
        Get
            Return _addIssuerCertificateAuthothorityToTrustedCaStore
        End Get
    End Property

    ''' <summary>
    ''' Initializes new instance of the <see cref="VerifierForm"/>.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Initializes new instance of the <see cref="VerifierForm"/>.
    ''' </summary>
    Public Sub New(cert As Certificate)
        Me.New()
        lblHostname.Text = cert.GetCommonName()
        lblSubject.Text = cert.GetSubjectName()
        lblIssuer.Text = cert.GetIssuerName()
        lblValidFrom.Text = cert.GetEffectiveDate().ToString()
        lblValidTo.Text = cert.GetExpirationDate().ToString()
        txtThumbprint.Text = cert.Thumbprint
    End Sub

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            If components IsNot Nothing Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"
    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.btnReject = New System.Windows.Forms.Button()
        Me.btnAccept = New System.Windows.Forms.Button()
        Me.panel1 = New System.Windows.Forms.Panel()
        Me.txtThumbprint = New System.Windows.Forms.TextBox()
        Me.label9 = New System.Windows.Forms.Label()
        Me.lblHostname = New System.Windows.Forms.Label()
        Me.label8 = New System.Windows.Forms.Label()
        Me.label6 = New System.Windows.Forms.Label()
        Me.lblValidTo = New System.Windows.Forms.Label()
        Me.lblValidFrom = New System.Windows.Forms.Label()
        Me.lblIssuer = New System.Windows.Forms.Label()
        Me.lblSubject = New System.Windows.Forms.Label()
        Me.lblProblem = New System.Windows.Forms.Label()
        Me.panel2 = New System.Windows.Forms.Panel()
        Me.label5 = New System.Windows.Forms.Label()
        Me.label4 = New System.Windows.Forms.Label()
        Me.label3 = New System.Windows.Forms.Label()
        Me.label2 = New System.Windows.Forms.Label()
        Me.label1 = New System.Windows.Forms.Label()
        Me.btnOkAndTrust = New System.Windows.Forms.Button()
        Me.panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnReject
        '
        Me.btnReject.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnReject.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnReject.Location = New System.Drawing.Point(254, 334)
        Me.btnReject.Name = "btnReject"
        Me.btnReject.Size = New System.Drawing.Size(72, 23)
        Me.btnReject.TabIndex = 2
        Me.btnReject.Text = "Reject"
        '
        'btnAccept
        '
        Me.btnAccept.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAccept.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnAccept.Location = New System.Drawing.Point(336, 334)
        Me.btnAccept.Name = "btnAccept"
        Me.btnAccept.Size = New System.Drawing.Size(72, 23)
        Me.btnAccept.TabIndex = 1
        Me.btnAccept.Text = "Accept"
        '
        'panel1
        '
        Me.panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.panel1.Controls.Add(Me.txtThumbprint)
        Me.panel1.Controls.Add(Me.label9)
        Me.panel1.Controls.Add(Me.lblHostname)
        Me.panel1.Controls.Add(Me.label8)
        Me.panel1.Controls.Add(Me.label6)
        Me.panel1.Controls.Add(Me.lblValidTo)
        Me.panel1.Controls.Add(Me.lblValidFrom)
        Me.panel1.Controls.Add(Me.lblIssuer)
        Me.panel1.Controls.Add(Me.lblSubject)
        Me.panel1.Controls.Add(Me.lblProblem)
        Me.panel1.Controls.Add(Me.panel2)
        Me.panel1.Controls.Add(Me.label5)
        Me.panel1.Controls.Add(Me.label4)
        Me.panel1.Controls.Add(Me.label3)
        Me.panel1.Controls.Add(Me.label2)
        Me.panel1.Location = New System.Drawing.Point(8, 8)
        Me.panel1.Name = "panel1"
        Me.panel1.Size = New System.Drawing.Size(400, 320)
        Me.panel1.TabIndex = 3
        '
        'txtThumbprint
        '
        Me.txtThumbprint.BackColor = System.Drawing.SystemColors.Window
        Me.txtThumbprint.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtThumbprint.Location = New System.Drawing.Point(80, 201)
        Me.txtThumbprint.Name = "txtThumbprint"
        Me.txtThumbprint.ReadOnly = True
        Me.txtThumbprint.Size = New System.Drawing.Size(312, 13)
        Me.txtThumbprint.TabIndex = 16
        '
        'label9
        '
        Me.label9.Location = New System.Drawing.Point(8, 201)
        Me.label9.Name = "label9"
        Me.label9.Size = New System.Drawing.Size(72, 23)
        Me.label9.TabIndex = 14
        Me.label9.Text = "Thumbprint:"
        '
        'lblHostname
        '
        Me.lblHostname.Location = New System.Drawing.Point(80, 32)
        Me.lblHostname.Name = "lblHostname"
        Me.lblHostname.Size = New System.Drawing.Size(312, 16)
        Me.lblHostname.TabIndex = 13
        '
        'label8
        '
        Me.label8.Location = New System.Drawing.Point(8, 32)
        Me.label8.Name = "label8"
        Me.label8.Size = New System.Drawing.Size(72, 23)
        Me.label8.TabIndex = 12
        Me.label8.Text = "Hostname:"
        '
        'label6
        '
        Me.label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.label6.Location = New System.Drawing.Point(8, 8)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(216, 23)
        Me.label6.TabIndex = 11
        Me.label6.Text = "Certificate details:"
        '
        'lblValidTo
        '
        Me.lblValidTo.Location = New System.Drawing.Point(80, 176)
        Me.lblValidTo.Name = "lblValidTo"
        Me.lblValidTo.Size = New System.Drawing.Size(312, 23)
        Me.lblValidTo.TabIndex = 10
        '
        'lblValidFrom
        '
        Me.lblValidFrom.Location = New System.Drawing.Point(80, 152)
        Me.lblValidFrom.Name = "lblValidFrom"
        Me.lblValidFrom.Size = New System.Drawing.Size(312, 23)
        Me.lblValidFrom.TabIndex = 9
        '
        'lblIssuer
        '
        Me.lblIssuer.Location = New System.Drawing.Point(80, 104)
        Me.lblIssuer.Name = "lblIssuer"
        Me.lblIssuer.Size = New System.Drawing.Size(312, 40)
        Me.lblIssuer.TabIndex = 8
        '
        'lblSubject
        '
        Me.lblSubject.Location = New System.Drawing.Point(80, 56)
        Me.lblSubject.Name = "lblSubject"
        Me.lblSubject.Size = New System.Drawing.Size(312, 40)
        Me.lblSubject.TabIndex = 7
        '
        'lblProblem
        '
        Me.lblProblem.ForeColor = System.Drawing.Color.Red
        Me.lblProblem.Location = New System.Drawing.Point(8, 238)
        Me.lblProblem.Name = "lblProblem"
        Me.lblProblem.Size = New System.Drawing.Size(384, 72)
        Me.lblProblem.TabIndex = 6
        '
        'panel2
        '
        Me.panel2.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.panel2.Location = New System.Drawing.Point(8, 227)
        Me.panel2.Name = "panel2"
        Me.panel2.Size = New System.Drawing.Size(384, 3)
        Me.panel2.TabIndex = 5
        '
        'label5
        '
        Me.label5.Location = New System.Drawing.Point(8, 176)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(72, 23)
        Me.label5.TabIndex = 4
        Me.label5.Text = "Valid to:"
        '
        'label4
        '
        Me.label4.Location = New System.Drawing.Point(8, 152)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(72, 23)
        Me.label4.TabIndex = 3
        Me.label4.Text = "Valid from:"
        '
        'label3
        '
        Me.label3.Location = New System.Drawing.Point(8, 104)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(72, 23)
        Me.label3.TabIndex = 2
        Me.label3.Text = "Issuer:"
        '
        'label2
        '
        Me.label2.Location = New System.Drawing.Point(8, 56)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(72, 23)
        Me.label2.TabIndex = 1
        Me.label2.Text = "Subject:"
        '
        'label1
        '
        Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.label1.Location = New System.Drawing.Point(8, 8)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(256, 23)
        Me.label1.TabIndex = 0
        Me.label1.Text = "CERTIFICATE INFORMATION:"
        '
        'btnOkAndTrust
        '
        Me.btnOkAndTrust.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOkAndTrust.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnOkAndTrust.Location = New System.Drawing.Point(16, 334)
        Me.btnOkAndTrust.Name = "btnOkAndTrust"
        Me.btnOkAndTrust.Size = New System.Drawing.Size(232, 23)
        Me.btnOkAndTrust.TabIndex = 5
        Me.btnOkAndTrust.Text = "OK && Always &Trust This Authority"
        Me.btnOkAndTrust.Visible = False
        '
        'VerifierForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(418, 364)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnOkAndTrust)
        Me.Controls.Add(Me.panel1)
        Me.Controls.Add(Me.btnAccept)
        Me.Controls.Add(Me.btnReject)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.Name = "VerifierForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Certificate"
        Me.panel1.ResumeLayout(False)
        Me.panel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
#End Region

    ''' <summary>
    ''' Handles 'Accept' button click event.
    ''' </summary>
    Private Sub btnAccept_Click(sender As Object, e As System.EventArgs) Handles btnAccept.Click
        _accepted = True
        Me.Close()
    End Sub

    ''' <summary>
    ''' Handles 'Reject' button click event.
    ''' </summary>
    Private Sub btnReject_Click(sender As Object, e As System.EventArgs) Handles btnReject.Click
        Me.Close()
    End Sub

    ''' <summary>
    ''' Handles 'Accept and Add' button click event.
    ''' </summary>
    Private Sub btnOkAndTrust_Click(sender As Object, e As System.EventArgs) Handles btnOkAndTrust.Click
        _accepted = True
        _addIssuerCertificateAuthothorityToTrustedCaStore = True
        Me.Close()
    End Sub
End Class
