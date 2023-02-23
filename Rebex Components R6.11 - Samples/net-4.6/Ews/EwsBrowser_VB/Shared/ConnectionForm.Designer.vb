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



Partial Class ConnectionForm
    Private ucConnectionEditor As UcConnectionEditor

    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.btnConnect = New System.Windows.Forms.Button()
        Me.ucConnectionEditor = New Rebex.Samples.UcConnectionEditor()
        Me.SuspendLayout()
        '
        'btnConnect
        '
        Me.btnConnect.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnConnect.Location = New System.Drawing.Point(557, 350)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(75, 23)
        Me.btnConnect.TabIndex = 3
        Me.btnConnect.Text = "Connect"
        Me.btnConnect.UseVisualStyleBackColor = True
        '
        'ucConnectionEditor
        '
        Me.ucConnectionEditor.AllowedSuite = CType(((((((((((((((Rebex.Net.TlsCipherSuite.RSA_WITH_3DES_EDE_CBC_SHA Or Rebex.Net.TlsCipherSuite.RSA_WITH_AES_128_CBC_SHA) _
            Or Rebex.Net.TlsCipherSuite.RSA_WITH_AES_256_CBC_SHA) _
            Or Rebex.Net.TlsCipherSuite.DHE_DSS_WITH_3DES_EDE_CBC_SHA) _
            Or Rebex.Net.TlsCipherSuite.DHE_DSS_WITH_AES_128_CBC_SHA) _
            Or Rebex.Net.TlsCipherSuite.DHE_DSS_WITH_AES_256_CBC_SHA) _
            Or Rebex.Net.TlsCipherSuite.DHE_RSA_WITH_3DES_EDE_CBC_SHA) _
            Or Rebex.Net.TlsCipherSuite.DHE_RSA_WITH_AES_128_CBC_SHA) _
            Or Rebex.Net.TlsCipherSuite.DHE_RSA_WITH_AES_256_CBC_SHA) _
            Or Rebex.Net.TlsCipherSuite.RSA_WITH_AES_128_CBC_SHA256) _
            Or Rebex.Net.TlsCipherSuite.RSA_WITH_AES_256_CBC_SHA256) _
            Or Rebex.Net.TlsCipherSuite.DHE_DSS_WITH_AES_128_CBC_SHA256) _
            Or Rebex.Net.TlsCipherSuite.DHE_RSA_WITH_AES_128_CBC_SHA256) _
            Or Rebex.Net.TlsCipherSuite.DHE_DSS_WITH_AES_256_CBC_SHA256) _
            Or Rebex.Net.TlsCipherSuite.DHE_RSA_WITH_AES_256_CBC_SHA256), Rebex.Net.TlsCipherSuite)
        Me.ucConnectionEditor.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ucConnectionEditor.ClientCertificate = Nothing
        Me.ucConnectionEditor.ClientCertificateFilename = Nothing
        Me.ucConnectionEditor.Location = New System.Drawing.Point(12, 12)
        Me.ucConnectionEditor.Mailbox = ""
        Me.ucConnectionEditor.MinimumSize = New System.Drawing.Size(620, 0)
        Me.ucConnectionEditor.Name = "ucConnectionEditor"
        Me.ucConnectionEditor.Protocol = Rebex.Samples.ProtocolMode.HTTPS
        Me.ucConnectionEditor.ProxyHost = ""
        Me.ucConnectionEditor.ProxyPassword = ""
        Me.ucConnectionEditor.ProxyPort = 0
        Me.ucConnectionEditor.ProxyType = Rebex.Net.ProxyType.None
        Me.ucConnectionEditor.ProxyUser = ""
        Me.ucConnectionEditor.ServerCertificateThumbprint = ""
        Me.ucConnectionEditor.ServerCertificateVerifyingMode = Rebex.Samples.CertificateVerifyingMode.LocalyStoredThumbprint
        Me.ucConnectionEditor.ServerHost = ""
        Me.ucConnectionEditor.ServerPassword = ""
        Me.ucConnectionEditor.ServerPort = 0
        Me.ucConnectionEditor.ServerUser = ""
        Me.ucConnectionEditor.Size = New System.Drawing.Size(620, 334)
        Me.ucConnectionEditor.StorePassword = False
        Me.ucConnectionEditor.TabIndex = 0
        Me.ucConnectionEditor.TlsProtocol = CType(((Rebex.Net.TlsVersion.TLS10 Or Rebex.Net.TlsVersion.TLS11) _
            Or Rebex.Net.TlsVersion.TLS12), Rebex.Net.TlsVersion)
        Me.ucConnectionEditor.UseSingleSignOn = False
        '
        'ConnectionForm
        '
        Me.AcceptButton = Me.btnConnect
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(644, 382)
        Me.Controls.Add(Me.btnConnect)
        Me.Controls.Add(Me.ucConnectionEditor)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(660, 39)
        Me.Name = "ConnectionForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Connection"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private WithEvents btnConnect As System.Windows.Forms.Button
End Class

