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
Imports System.Collections
Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports Rebex.Net
Imports Rebex.Mime
Imports Rebex.Mail
Imports Rebex.Mime.Headers
Imports System.Drawing

''' <summary>
''' Simple message viewer.
''' </summary>

Public Class MessageView
    Inherits System.Windows.Forms.Form
    Private _message As MailMessage
    Private _rawMessage() As Byte

#Region "Controls"

    Private views As System.Windows.Forms.TabControl
    Private tabMessage As System.Windows.Forms.TabPage
    Private tabAttachments As System.Windows.Forms.TabPage
    Private label1 As System.Windows.Forms.Label
    Private label2 As System.Windows.Forms.Label
    Private label3 As System.Windows.Forms.Label
    Private label4 As System.Windows.Forms.Label
    Private tabHeaders As System.Windows.Forms.TabPage
    Private tabRawMessage As System.Windows.Forms.TabPage
    Private headerValue As System.Windows.Forms.ColumnHeader
    Private headerName As System.Windows.Forms.ColumnHeader
    Private attachmentName As System.Windows.Forms.ColumnHeader
    Private attachmentMimeType As System.Windows.Forms.ColumnHeader
    Private attachmentSize As System.Windows.Forms.ColumnHeader
    Private attachmentId As System.Windows.Forms.ColumnHeader
    Private txtSubject As System.Windows.Forms.TextBox
    Private txtFrom As System.Windows.Forms.TextBox
    Private attachmentsListView As System.Windows.Forms.ListView
    Private headersListView As System.Windows.Forms.ListView
    Private txtCc As System.Windows.Forms.TextBox
    Private txtTo As System.Windows.Forms.TextBox
    Friend WithEvents tabBody As System.Windows.Forms.TabControl
    Friend WithEvents tabBodyText As System.Windows.Forms.TabPage
    Private WithEvents txtTextBody As System.Windows.Forms.TextBox
    Friend WithEvents tabBodyHtml As System.Windows.Forms.TabPage
    Private WithEvents txtHtmlBody As System.Windows.Forms.TextBox
    Private txtRawText As System.Windows.Forms.RichTextBox

#End Region

    ''' <summary>
    ''' Initializes a new instance of the <see cref="MessageView"/> class.
    ''' </summary>
    ''' <param name="message">The parsed MimeMessage.</param>
    ''' <param name="raw">The raw unparsed message data obtained from the server.</param>
    Public Sub New(ByVal message As MailMessage, ByVal raw() As Byte)

        InitializeComponent()

        _message = message
        _rawMessage = raw

        '
        ' Initialize tabs
        '
        SuspendLayout()

        ' Basic mail message info - from, to, txtSubject, body, ...
        FillMailMessageTab()

        ' Attachments
        FillAttachmentsTab()

        ' Message headers
        FillMessageHeaders()

        ' Raw (unparsed) message data
        FillRawTab()

        ResumeLayout(False)
    End Sub 'New


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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MessageView))
        Me.views = New System.Windows.Forms.TabControl()
        Me.tabMessage = New System.Windows.Forms.TabPage()
        Me.tabBody = New System.Windows.Forms.TabControl()
        Me.tabBodyText = New System.Windows.Forms.TabPage()
        Me.txtTextBody = New System.Windows.Forms.TextBox()
        Me.tabBodyHtml = New System.Windows.Forms.TabPage()
        Me.txtHtmlBody = New System.Windows.Forms.TextBox()
        Me.label1 = New System.Windows.Forms.Label()
        Me.txtSubject = New System.Windows.Forms.TextBox()
        Me.txtCc = New System.Windows.Forms.TextBox()
        Me.txtTo = New System.Windows.Forms.TextBox()
        Me.txtFrom = New System.Windows.Forms.TextBox()
        Me.label2 = New System.Windows.Forms.Label()
        Me.label3 = New System.Windows.Forms.Label()
        Me.label4 = New System.Windows.Forms.Label()
        Me.tabAttachments = New System.Windows.Forms.TabPage()
        Me.attachmentsListView = New System.Windows.Forms.ListView()
        Me.attachmentId = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.attachmentMimeType = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.attachmentName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.attachmentSize = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.tabHeaders = New System.Windows.Forms.TabPage()
        Me.headersListView = New System.Windows.Forms.ListView()
        Me.headerName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.headerValue = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.tabRawMessage = New System.Windows.Forms.TabPage()
        Me.txtRawText = New System.Windows.Forms.RichTextBox()
        Me.views.SuspendLayout()
        Me.tabMessage.SuspendLayout()
        Me.tabBody.SuspendLayout()
        Me.tabBodyText.SuspendLayout()
        Me.tabBodyHtml.SuspendLayout()
        Me.tabAttachments.SuspendLayout()
        Me.tabHeaders.SuspendLayout()
        Me.tabRawMessage.SuspendLayout()
        Me.SuspendLayout()
        '
        'views
        '
        Me.views.Controls.Add(Me.tabMessage)
        Me.views.Controls.Add(Me.tabAttachments)
        Me.views.Controls.Add(Me.tabHeaders)
        Me.views.Controls.Add(Me.tabRawMessage)
        Me.views.Dock = System.Windows.Forms.DockStyle.Fill
        Me.views.Location = New System.Drawing.Point(0, 0)
        Me.views.Name = "views"
        Me.views.SelectedIndex = 0
        Me.views.Size = New System.Drawing.Size(692, 470)
        Me.views.TabIndex = 0
        '
        'tabMessage
        '
        Me.tabMessage.Controls.Add(Me.tabBody)
        Me.tabMessage.Controls.Add(Me.label1)
        Me.tabMessage.Controls.Add(Me.txtSubject)
        Me.tabMessage.Controls.Add(Me.txtCc)
        Me.tabMessage.Controls.Add(Me.txtTo)
        Me.tabMessage.Controls.Add(Me.txtFrom)
        Me.tabMessage.Controls.Add(Me.label2)
        Me.tabMessage.Controls.Add(Me.label3)
        Me.tabMessage.Controls.Add(Me.label4)
        Me.tabMessage.Location = New System.Drawing.Point(4, 22)
        Me.tabMessage.Name = "tabMessage"
        Me.tabMessage.Size = New System.Drawing.Size(684, 444)
        Me.tabMessage.TabIndex = 3
        Me.tabMessage.Text = "Mail Message"
        '
        'tabBody
        '
        Me.tabBody.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tabBody.Controls.Add(Me.tabBodyText)
        Me.tabBody.Controls.Add(Me.tabBodyHtml)
        Me.tabBody.Location = New System.Drawing.Point(3, 121)
        Me.tabBody.Name = "tabBody"
        Me.tabBody.SelectedIndex = 0
        Me.tabBody.Size = New System.Drawing.Size(677, 320)
        Me.tabBody.TabIndex = 10
        '
        'tabBodyText
        '
        Me.tabBodyText.Controls.Add(Me.txtTextBody)
        Me.tabBodyText.Location = New System.Drawing.Point(4, 22)
        Me.tabBodyText.Name = "tabBodyText"
        Me.tabBodyText.Padding = New System.Windows.Forms.Padding(3)
        Me.tabBodyText.Size = New System.Drawing.Size(669, 294)
        Me.tabBodyText.TabIndex = 0
        Me.tabBodyText.Text = "Text"
        Me.tabBodyText.UseVisualStyleBackColor = True
        '
        'txtTextBody
        '
        Me.txtTextBody.BackColor = System.Drawing.SystemColors.Window
        Me.txtTextBody.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtTextBody.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtTextBody.Location = New System.Drawing.Point(3, 3)
        Me.txtTextBody.Multiline = True
        Me.txtTextBody.Name = "txtTextBody"
        Me.txtTextBody.ReadOnly = True
        Me.txtTextBody.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtTextBody.Size = New System.Drawing.Size(663, 288)
        Me.txtTextBody.TabIndex = 7
        '
        'tabBodyHtml
        '
        Me.tabBodyHtml.Controls.Add(Me.txtHtmlBody)
        Me.tabBodyHtml.Location = New System.Drawing.Point(4, 22)
        Me.tabBodyHtml.Name = "tabBodyHtml"
        Me.tabBodyHtml.Padding = New System.Windows.Forms.Padding(3)
        Me.tabBodyHtml.Size = New System.Drawing.Size(669, 294)
        Me.tabBodyHtml.TabIndex = 1
        Me.tabBodyHtml.Text = "HTML"
        Me.tabBodyHtml.UseVisualStyleBackColor = True
        '
        'txtHtmlBody
        '
        Me.txtHtmlBody.BackColor = System.Drawing.SystemColors.Window
        Me.txtHtmlBody.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtHtmlBody.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtHtmlBody.Location = New System.Drawing.Point(3, 3)
        Me.txtHtmlBody.Multiline = True
        Me.txtHtmlBody.Name = "txtHtmlBody"
        Me.txtHtmlBody.ReadOnly = True
        Me.txtHtmlBody.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtHtmlBody.Size = New System.Drawing.Size(663, 288)
        Me.txtHtmlBody.TabIndex = 10
        '
        'label1
        '
        Me.label1.Location = New System.Drawing.Point(8, 8)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(100, 23)
        Me.label1.TabIndex = 5
        Me.label1.Text = "From:"
        Me.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtSubject
        '
        Me.txtSubject.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSubject.BackColor = System.Drawing.SystemColors.Window
        Me.txtSubject.Location = New System.Drawing.Point(112, 95)
        Me.txtSubject.Name = "txtSubject"
        Me.txtSubject.ReadOnly = True
        Me.txtSubject.Size = New System.Drawing.Size(568, 20)
        Me.txtSubject.TabIndex = 3
        '
        'txtCc
        '
        Me.txtCc.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtCc.BackColor = System.Drawing.SystemColors.Window
        Me.txtCc.Location = New System.Drawing.Point(112, 66)
        Me.txtCc.Name = "txtCc"
        Me.txtCc.Size = New System.Drawing.Size(568, 20)
        Me.txtCc.TabIndex = 7
        '
        'txtTo
        '
        Me.txtTo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTo.BackColor = System.Drawing.SystemColors.Window
        Me.txtTo.Location = New System.Drawing.Point(112, 37)
        Me.txtTo.Name = "txtTo"
        Me.txtTo.Size = New System.Drawing.Size(568, 20)
        Me.txtTo.TabIndex = 8
        '
        'txtFrom
        '
        Me.txtFrom.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFrom.BackColor = System.Drawing.SystemColors.Window
        Me.txtFrom.Location = New System.Drawing.Point(112, 8)
        Me.txtFrom.Name = "txtFrom"
        Me.txtFrom.ReadOnly = True
        Me.txtFrom.Size = New System.Drawing.Size(568, 20)
        Me.txtFrom.TabIndex = 0
        '
        'label2
        '
        Me.label2.Location = New System.Drawing.Point(8, 37)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(100, 23)
        Me.label2.TabIndex = 5
        Me.label2.Text = "To:"
        Me.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'label3
        '
        Me.label3.Location = New System.Drawing.Point(8, 66)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(100, 23)
        Me.label3.TabIndex = 5
        Me.label3.Text = "Cc:"
        Me.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'label4
        '
        Me.label4.Location = New System.Drawing.Point(8, 95)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(100, 23)
        Me.label4.TabIndex = 5
        Me.label4.Text = "Subject:"
        Me.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'tabAttachments
        '
        Me.tabAttachments.Controls.Add(Me.attachmentsListView)
        Me.tabAttachments.Location = New System.Drawing.Point(4, 22)
        Me.tabAttachments.Name = "tabAttachments"
        Me.tabAttachments.Size = New System.Drawing.Size(684, 444)
        Me.tabAttachments.TabIndex = 4
        Me.tabAttachments.Text = "Attachments"
        '
        'attachmentsListView
        '
        Me.attachmentsListView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.attachmentId, Me.attachmentMimeType, Me.attachmentName, Me.attachmentSize})
        Me.attachmentsListView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.attachmentsListView.FullRowSelect = True
        Me.attachmentsListView.GridLines = True
        Me.attachmentsListView.Location = New System.Drawing.Point(0, 0)
        Me.attachmentsListView.Name = "attachmentsListView"
        Me.attachmentsListView.Size = New System.Drawing.Size(684, 444)
        Me.attachmentsListView.TabIndex = 0
        Me.attachmentsListView.UseCompatibleStateImageBehavior = False
        Me.attachmentsListView.View = System.Windows.Forms.View.Details
        '
        'attachmentId
        '
        Me.attachmentId.Text = "Id"
        Me.attachmentId.Width = 41
        '
        'attachmentMimeType
        '
        Me.attachmentMimeType.Text = "MIME type"
        Me.attachmentMimeType.Width = 100
        '
        'attachmentName
        '
        Me.attachmentName.Text = "Name"
        Me.attachmentName.Width = 450
        '
        'attachmentSize
        '
        Me.attachmentSize.Text = "Size (bytes)"
        Me.attachmentSize.Width = 80
        '
        'tabHeaders
        '
        Me.tabHeaders.Controls.Add(Me.headersListView)
        Me.tabHeaders.Location = New System.Drawing.Point(4, 22)
        Me.tabHeaders.Name = "tabHeaders"
        Me.tabHeaders.Size = New System.Drawing.Size(684, 444)
        Me.tabHeaders.TabIndex = 0
        Me.tabHeaders.Text = "Headers"
        '
        'headersListView
        '
        Me.headersListView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.headerName, Me.headerValue})
        Me.headersListView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.headersListView.FullRowSelect = True
        Me.headersListView.GridLines = True
        Me.headersListView.Location = New System.Drawing.Point(0, 0)
        Me.headersListView.Name = "headersListView"
        Me.headersListView.Size = New System.Drawing.Size(684, 444)
        Me.headersListView.TabIndex = 0
        Me.headersListView.UseCompatibleStateImageBehavior = False
        Me.headersListView.View = System.Windows.Forms.View.Details
        '
        'headerName
        '
        Me.headerName.Text = "Name"
        Me.headerName.Width = 200
        '
        'headerValue
        '
        Me.headerValue.Text = "Value"
        Me.headerValue.Width = 400
        '
        'tabRawMessage
        '
        Me.tabRawMessage.Controls.Add(Me.txtRawText)
        Me.tabRawMessage.Location = New System.Drawing.Point(4, 22)
        Me.tabRawMessage.Name = "tabRawMessage"
        Me.tabRawMessage.Size = New System.Drawing.Size(684, 444)
        Me.tabRawMessage.TabIndex = 2
        Me.tabRawMessage.Text = "Raw Message"
        '
        'txtRawText
        '
        Me.txtRawText.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtRawText.BackColor = System.Drawing.SystemColors.Window
        Me.txtRawText.Location = New System.Drawing.Point(0, 0)
        Me.txtRawText.Name = "txtRawText"
        Me.txtRawText.ReadOnly = True
        Me.txtRawText.Size = New System.Drawing.Size(680, 448)
        Me.txtRawText.TabIndex = 0
        Me.txtRawText.Text = ""
        '
        'MessageView
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(692, 470)
        Me.Controls.Add(Me.views)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "MessageView"
        Me.Text = "MessageView"
        Me.views.ResumeLayout(False)
        Me.tabMessage.ResumeLayout(False)
        Me.tabMessage.PerformLayout()
        Me.tabBody.ResumeLayout(False)
        Me.tabBodyText.ResumeLayout(False)
        Me.tabBodyText.PerformLayout()
        Me.tabBodyHtml.ResumeLayout(False)
        Me.tabBodyHtml.PerformLayout()
        Me.tabAttachments.ResumeLayout(False)
        Me.tabHeaders.ResumeLayout(False)
        Me.tabRawMessage.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub 'InitializeComponent 
#End Region


    ''' <summary>
    ''' Loads content of the main mail message tab
    ''' </summary>
    Public Sub FillMailMessageTab()
        ' from, to, cc
        Me.txtFrom.Text = _message.From.ToString()
        Me.txtTo.Text = _message.To.ToString()
        Me.txtCc.Text = _message.CC.ToString()

        ' subject
        Me.txtSubject.Text = _message.Subject

        ' body
        Me.txtTextBody.Text = _message.BodyText.Replace(ControlChars.Lf, ControlChars.Cr & ControlChars.Lf)
        Me.txtHtmlBody.Text = _message.BodyHtml.Replace(ControlChars.Lf, ControlChars.Cr & ControlChars.Lf)
        If String.IsNullOrEmpty(_message.BodyText) Then
            Me.tabBody.SelectTab(Me.tabBodyHtml)
        Else
            Me.tabBody.SelectTab(Me.tabBodyText)
        End If

        ' window title
        Me.Text = _message.Subject
    End Sub 'FillMailMessageTab


    ''' <summary>
    ''' Loads content of the message headers tab.
    ''' </summary>
    Public Sub FillMessageHeaders()
        Dim headers As MimeHeaderCollection = _message.Headers

        ' show all message header
        Dim i As Integer
        For i = 0 To headers.Count - 1
            Dim header As MimeHeader = headers(i)

            ' add name column
            Dim item As New ListViewItem(header.Name)

            ' add header raw content column
            item.SubItems.Add(header.Raw)

            ' show unparsed (corrupted) headers in red                
            If header.Unparsable Then
                item.ForeColor = Color.Red
            End If
            ' add row to the ListView                
            headersListView.Items.Add(item)
        Next i
    End Sub 'FillMessageHeaders


    ''' <summary>
    ''' Loads the content of the attachments tab.
    ''' </summary>
    Public Sub FillAttachmentsTab()
        ' cycle through all message attachments
        Dim i As Integer
        For i = 0 To _message.Attachments.Count - 1
            Dim attachment As attachment = _message.Attachments(i)

            Dim item As New ListViewItem(i.ToString()) ' attachment index
            item.SubItems.Add(attachment.MediaType) ' type
            item.SubItems.Add(attachment.FileName) ' filename
            item.SubItems.Add(attachment.GetContentLength.ToString()) ' length in bytes
            attachmentsListView.Items.Add(item)
        Next i
    End Sub 'FillAttachmentsTab


    ''' <summary>
    ''' Loads the content of the raw message tab.
    ''' </summary>
    Public Sub FillRawTab()
        If Not (_rawMessage Is Nothing) Then
            Dim [text] As String = Encoding.Default.GetString(_rawMessage, 0, Math.Min(_rawMessage.Length, 500000))
            txtRawText.Text = [text]
        Else
            txtRawText.Text = ""
        End If
    End Sub 'FillRawTab


    Private Sub cc_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs)
    End Sub 'cc_TextChanged
End Class 'MessageView 

