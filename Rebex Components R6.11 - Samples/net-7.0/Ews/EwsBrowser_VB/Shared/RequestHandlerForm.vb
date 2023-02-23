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
''' Represents confirmation dialog for selecting client certificates.
''' </summary>
Public Class RequesetHandlerForm
    Inherits System.Windows.Forms.Form
    Private WithEvents btnCancel As System.Windows.Forms.Button
    Private WithEvents btnNoCertificate As System.Windows.Forms.Button
    Private WithEvents btnOk As System.Windows.Forms.Button
    Private label1 As System.Windows.Forms.Label
    Private WithEvents cbCertList As System.Windows.Forms.ComboBox
    Private panel1 As System.Windows.Forms.Panel
    Private label6 As System.Windows.Forms.Label
    Private lblValidTo As System.Windows.Forms.Label
    Private lblValidFrom As System.Windows.Forms.Label
    Private lblIssuer As System.Windows.Forms.Label
    Private lblSubject As System.Windows.Forms.Label
    Private label5 As System.Windows.Forms.Label
    Private label4 As System.Windows.Forms.Label
    Private label3 As System.Windows.Forms.Label
    Private label2 As System.Windows.Forms.Label

    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.Container = Nothing

    Private _certs As Certificate()
    Private _selectedCertificate As Certificate

    ''' <summary>
    ''' Gets the selected certificate.
    ''' </summary>
    Public ReadOnly Property Certificate() As Certificate
        Get
            Return _selectedCertificate
        End Get
    End Property

    ''' <summary>
    ''' Initializes new instance of the <see cref="RequesetHandlerForm"/>.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
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

    ''' <summary>
    ''' Loads data from certificate to form controls.
    ''' </summary>
    Public Sub LoadData(certs As Certificate())
        _certs = certs

        For i As Integer = 0 To certs.Length - 1
            cbCertList.Items.Add(certs(i).GetSubjectName())
        Next

        If certs.Length > 0 Then
            cbCertList.SelectedIndex = 0

            lblSubject.Text = certs(0).GetSubjectName()
            lblIssuer.Text = certs(0).GetIssuerName()
            lblValidFrom.Text = certs(0).GetEffectiveDate().ToString()
            lblValidTo.Text = certs(0).GetExpirationDate().ToString()
        End If
    End Sub

#Region "Windows Form Designer generated code"
    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnOk = New System.Windows.Forms.Button()
        Me.cbCertList = New System.Windows.Forms.ComboBox()
        Me.label1 = New System.Windows.Forms.Label()
        Me.panel1 = New System.Windows.Forms.Panel()
        Me.label6 = New System.Windows.Forms.Label()
        Me.lblValidTo = New System.Windows.Forms.Label()
        Me.lblValidFrom = New System.Windows.Forms.Label()
        Me.lblIssuer = New System.Windows.Forms.Label()
        Me.lblSubject = New System.Windows.Forms.Label()
        Me.label5 = New System.Windows.Forms.Label()
        Me.label4 = New System.Windows.Forms.Label()
        Me.label3 = New System.Windows.Forms.Label()
        Me.label2 = New System.Windows.Forms.Label()
        Me.btnNoCertificate = New System.Windows.Forms.Button()
        Me.panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnCancel.Location = New System.Drawing.Point(336, 216)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "&Cancel"
        '
        'btnOk
        '
        Me.btnOk.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnOk.Location = New System.Drawing.Point(160, 216)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(75, 23)
        Me.btnOk.TabIndex = 0
        Me.btnOk.Text = "&OK"
        '
        'cbCertList
        '
        Me.cbCertList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbCertList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbCertList.Location = New System.Drawing.Point(104, 8)
        Me.cbCertList.Name = "cbCertList"
        Me.cbCertList.Size = New System.Drawing.Size(304, 21)
        Me.cbCertList.TabIndex = 2
        '
        'label1
        '
        Me.label1.Location = New System.Drawing.Point(8, 8)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(96, 23)
        Me.label1.TabIndex = 3
        Me.label1.Text = "Certificate:"
        Me.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'panel1
        '
        Me.panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.panel1.Controls.Add(Me.label6)
        Me.panel1.Controls.Add(Me.lblValidTo)
        Me.panel1.Controls.Add(Me.lblValidFrom)
        Me.panel1.Controls.Add(Me.lblIssuer)
        Me.panel1.Controls.Add(Me.lblSubject)
        Me.panel1.Controls.Add(Me.label5)
        Me.panel1.Controls.Add(Me.label4)
        Me.panel1.Controls.Add(Me.label3)
        Me.panel1.Controls.Add(Me.label2)
        Me.panel1.Location = New System.Drawing.Point(8, 40)
        Me.panel1.Name = "panel1"
        Me.panel1.Size = New System.Drawing.Size(400, 168)
        Me.panel1.TabIndex = 4
        '
        'label6
        '
        Me.label6.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.label6.Location = New System.Drawing.Point(8, 8)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(216, 23)
        Me.label6.TabIndex = 11
        Me.label6.Text = "Certificate details:"
        '
        'lblValidTo
        '
        Me.lblValidTo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblValidTo.Location = New System.Drawing.Point(80, 136)
        Me.lblValidTo.Name = "lblValidTo"
        Me.lblValidTo.Size = New System.Drawing.Size(224, 23)
        Me.lblValidTo.TabIndex = 10
        '
        'lblValidFrom
        '
        Me.lblValidFrom.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblValidFrom.Location = New System.Drawing.Point(80, 112)
        Me.lblValidFrom.Name = "lblValidFrom"
        Me.lblValidFrom.Size = New System.Drawing.Size(224, 23)
        Me.lblValidFrom.TabIndex = 9
        '
        'lblIssuer
        '
        Me.lblIssuer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblIssuer.Location = New System.Drawing.Point(80, 72)
        Me.lblIssuer.Name = "lblIssuer"
        Me.lblIssuer.Size = New System.Drawing.Size(312, 32)
        Me.lblIssuer.TabIndex = 8
        '
        'lblSubject
        '
        Me.lblSubject.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSubject.Location = New System.Drawing.Point(80, 32)
        Me.lblSubject.Name = "lblSubject"
        Me.lblSubject.Size = New System.Drawing.Size(312, 32)
        Me.lblSubject.TabIndex = 7
        '
        'label5
        '
        Me.label5.Location = New System.Drawing.Point(8, 136)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(72, 23)
        Me.label5.TabIndex = 4
        Me.label5.Text = "Valid to:"
        '
        'label4
        '
        Me.label4.Location = New System.Drawing.Point(8, 112)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(72, 23)
        Me.label4.TabIndex = 3
        Me.label4.Text = "Valid from:"
        '
        'label3
        '
        Me.label3.Location = New System.Drawing.Point(8, 72)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(72, 23)
        Me.label3.TabIndex = 2
        Me.label3.Text = "Issuer:"
        '
        'label2
        '
        Me.label2.Location = New System.Drawing.Point(8, 32)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(72, 23)
        Me.label2.TabIndex = 1
        Me.label2.Text = "Subject:"
        '
        'btnNoCertificate
        '
        Me.btnNoCertificate.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnNoCertificate.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnNoCertificate.Location = New System.Drawing.Point(240, 216)
        Me.btnNoCertificate.Name = "btnNoCertificate"
        Me.btnNoCertificate.Size = New System.Drawing.Size(88, 23)
        Me.btnNoCertificate.TabIndex = 5
        Me.btnNoCertificate.Text = "&No Certificate"
        '
        'RequesetHandlerForm
        '
        Me.AcceptButton = Me.btnOk
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(418, 248)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnNoCertificate)
        Me.Controls.Add(Me.panel1)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.cbCertList)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.btnCancel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "RequesetHandlerForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Select certificate"
        Me.panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
#End Region

    ''' <summary>
    ''' Handles certificate selection change event.
    ''' </summary>
    Private Sub cbCertList_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cbCertList.SelectedIndexChanged
        If cbCertList.SelectedIndex <> -1 Then
            Dim selIndex As Integer = cbCertList.SelectedIndex

            lblSubject.Text = _certs(selIndex).GetSubjectName()
            lblIssuer.Text = _certs(selIndex).GetIssuerName()
            lblValidFrom.Text = _certs(selIndex).GetEffectiveDate().ToString()
            lblValidTo.Text = _certs(selIndex).GetExpirationDate().ToString()
        End If
    End Sub

    ''' <summary>
    ''' Handles 'Ok' button click event.
    ''' </summary>
    Private Sub btnOk_Click(sender As Object, e As System.EventArgs) Handles btnOk.Click
        If cbCertList.SelectedIndex <> -1 Then
            _selectedCertificate = _certs(cbCertList.SelectedIndex)
        Else
            _selectedCertificate = Nothing
        End If

        Me.Close()
    End Sub

    ''' <summary>
    ''' Handles 'No' button click event.
    ''' </summary>
    Private Sub btnNoCertificate_Click(sender As Object, e As System.EventArgs) Handles btnNoCertificate.Click
        _selectedCertificate = Nothing
        Me.Close()
    End Sub
End Class

