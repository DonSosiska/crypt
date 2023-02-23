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
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports Rebex.Security.Certificates
Imports System.Security.Cryptography.X509Certificates

Public Partial Class ImportCertificateDialog
    Inherits Form
    ''' <summary>
    ''' Gets or sets client certificate filename used for SSL authentization.<br/>
    ''' </summary>
    Public Property ClientCertificateFilename() As String
        Get
            Return _clientCertificateFilename
        End Get
        Set
            _clientCertificateFilename = value
        End Set
    End Property
    Private _clientCertificateFilename As String = Nothing

    ''' <summary>
    ''' Gets or sets client certificate used for SSL authentization.
    ''' </summary>
    <Browsable(False)> _
    Public Property ClientCertificate() As CertificateChain
        Get
            Return _clientCertificate
        End Get
        Set
            _clientCertificate = value
        End Set
    End Property
    Private _clientCertificate As CertificateChain = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub btnFile_Click(sender As Object, e As EventArgs) Handles btnFile.Click
        ' ask for certificate file
        Dim fd As New OpenFileDialog()
        fd.Title = "Choose certificate file"
        fd.Filter = String.Format("{0} (*.pfx)|*.pfx|{1}|*.*", "Certificate files", "All files")
        If fd.ShowDialog() <> DialogResult.OK Then
            Return
        End If
        ClientCertificateFilename = fd.FileName
        LoadClientCertificate()

        DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

    Private Sub btnStore_Click(sender As Object, e As EventArgs) Handles btnStore.Click
        Dim store As New CertificateStore(CertificateStoreName.My, CertificateStoreLocation.CurrentUser)
        Dim certs As Certificate() = store.FindCertificates(CertificateFindOptions.None)

        Dim x509Certs As New X509Certificate2Collection()

        For Each cert As Certificate In certs
            x509Certs.Add(cert)
        Next

        ' ask for certificate from store
        x509Certs = X509Certificate2UI.SelectFromCollection(x509Certs, "Certificate store: MY", "Select a certificate from the store", X509SelectionFlag.SingleSelection)

        If x509Certs.Count = 1 Then
            Dim certificate As New Certificate(x509Certs(0))
            ClientCertificate = CertificateChain.BuildFrom(certificate)

            DialogResult = DialogResult.OK
        Else
            DialogResult = DialogResult.Cancel
        End If
    End Sub

    ''' <summary>
    ''' Loads a certificate from the <see cref="ClientCertificateFilename"/> into the <see cref="ClientCertificate"/> property.<br/>
    ''' If the certificate requires a password, the method prompts for the password.
    ''' </summary>
    ''' <returns>True - the key certificate been loaded or an user does not want any certificate. False - an error occurred.</returns>
    Private Function LoadClientCertificate() As Boolean
        Try
            ' try to load the certificate 
            Dim certificate As CertificateChain = Nothing
            ' (first attempt will be with empty password - only if it won't succeed, we will prompt for the password
            Dim firstAttempt As Boolean = True
            Dim password As String = ""
            While certificate Is Nothing
                Try
                    ClientCertificate = CertificateChain.LoadPfx(ClientCertificateFilename, password, KeySetOptions.Exportable)
                    ' success
                    Return True
                ' handle invalid password
                Catch ex As CertificateException
                    If ex.Message <> "PFX password is not valid." Then
                        ConnectionEditorUtils.ShowException("Error occurred when importing certificate.", "Import failed", ex)
                        Return False
                    End If
                    Dim dialogPrompt As String
                    If firstAttempt Then
                        dialogPrompt = "The certificate is password protected." & vbCr & vbLf & "Enter the certificate password:"
                    Else
                        dialogPrompt = "The password is wrong." & vbCr & vbLf & "Enter again?"
                    End If
                    Dim pd As New PassphraseDialog(dialogPrompt)
                    pd.Size = New Size(pd.Width, pd.Height + 13)
                    ' add extra line for the 2-lines password prompt
                    If pd.ShowDialog() <> DialogResult.OK Then
                        Return True
                    End If
                    password = pd.Passphrase
                    firstAttempt = False
                Catch ex As Exception
                    ConnectionEditorUtils.ShowException("Error occurred when importing certificate.", "Import failed", ex)
                    Return False
                End Try
            End While
                ' never reached
            Return False
        Finally
            ' once the certificate has not been successfully loaded, clear the stored filename
            If ClientCertificate Is Nothing Then
                ClientCertificateFilename = Nothing
            End If
        End Try
    End Function
End Class
