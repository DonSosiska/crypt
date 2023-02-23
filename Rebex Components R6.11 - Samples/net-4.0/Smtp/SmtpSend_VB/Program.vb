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
Imports System.IO
Imports System.Text
Imports Rebex.Net
Imports Rebex.Mail
Imports Rebex.Mime.Headers
Imports Rebex.Security.Certificates

''' <summary>
''' Sample application for sending e-mail via SMTP.
''' </summary>
Class Program

    ''' <summary>
    ''' Program entrypoint.
    ''' </summary>
    ''' <param name="args">Program arguments.</param>
    ''' <returns>Status. (0=success, 2=bad arguments)</returns>
    <STAThread()> Overloads Shared Function Main(ByVal args() As String) As Integer
            ' set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key


        Dim config As Arguments

        Try
            config = New Arguments(args)
        Catch e As Exception
            Dim applicationName As String = AppDomain.CurrentDomain.FriendlyName
            Console.WriteLine("=====================================================================")
            Console.WriteLine(" {0} ", applicationName)
            Console.WriteLine("=====================================================================")
            Console.WriteLine("")
            Console.WriteLine("Sends e-mail from command line.")
            Console.WriteLine("")
            Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.")
            Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/")
            Console.WriteLine("")

            Console.WriteLine("Syntax: {0} server[:port]" & ControlChars.Lf & _
            "        [-security none|explicit|implicit]" & ControlChars.Lf & _
            "        [-sign yes]" & ControlChars.Lf & _
            "        [-encrypt yes]" & ControlChars.Lf & _
            "        [-certificate path1 [-certificate path2 [...]]]" & ControlChars.Lf & _
            "        -from mail@domain" & ControlChars.Lf & _
            "        -to mail@domain;mail2@domain2" & ControlChars.Lf & _
            "        [-subject ""subject""]" & ControlChars.Lf & _
            "        [-body ""text""]" & ControlChars.Lf & _
            "        [-file file.eml|file.msg]" & ControlChars.Lf & _
            "        [-username login]" & ControlChars.Lf & _
            "        [-password pass]" & ControlChars.Lf, _
            applicationName)

            Console.WriteLine("Specific error: " + e.Message)

            Return 2
        End Try

        ' Build the message
        Dim message As MailMessage = config.Message

        '
        ' Sign and/or encrypt email if needed
        '
        If (config.Sign Or config.Encrypt) Then
            ' load the certificates specified on the command line
            Dim specified As CertificateStore = config.LoadCertificates()

            ' if the email should be both signed and encrypted it's recommended
            ' to sign it first and than encrypt it. This ensures proper
            ' handling in Outlook and OutlookExpress.

            '
            ' Sign email
            '
            If config.Sign Then
                Dim signers As ArrayList = New ArrayList
                Dim my As CertificateStore = New CertificateStore(CertificateStoreName.My)

                ' Get all needed certificates located in user's certificate store or specified on the command line
                Dim address As MailAddress
                For Each address In message.From
                    Dim cert As Certificate = FindCertificate(address.Address, specified, my, CertificateFindOptions.HasPrivateKey Or CertificateFindOptions.IsTimeValid)

                    If (cert Is Nothing) Then
                        Console.WriteLine("Certificate for signer '{0}' was not found.", address.Address)
                        Return 3
                    End If
                    ' add the certificate to signers collection
                    signers.Add(cert)

                    ' Sign the message
                    message.Sign(CType(signers.ToArray(GetType(Certificate)), Certificate()))
                Next
            End If

            '
            ' Encrypt email
            '
            If config.Encrypt Then
                Dim recipients As ArrayList = New ArrayList
                Dim addressBook As CertificateStore
                If CertificateStore.Exists(CertificateStoreName.AddressBook) Then
                    addressBook = New CertificateStore(CertificateStoreName.AddressBook)
                Else
                    addressBook = Nothing
                End If

                ' Prepare certificates located in user's cetificate store or specified on the command line
                Dim address As MailAddress
                For Each address In message.To
                    Dim cert As Certificate = FindCertificate(address.Address, specified, addressBook, CertificateFindOptions.IsTimeValid)
                    If cert Is Nothing Then
                        Console.WriteLine("Certificate for recipient '{0}' was not found.", address.Address)
                        Return 3
                    End If
                    recipients.Add(cert)
                Next


                ' Add sender certificates to recipients array now.
                ' Otherwise they will not be able to read the message they have sent.
                Dim my As CertificateStore = New CertificateStore(CertificateStoreName.My)

                ' Get all needed certificates located in user's certificate store or specified on the command line
                For Each address In message.From
                    Dim cert As Certificate = FindCertificate(address.Address, specified, my, CertificateFindOptions.IsTimeValid)

                    If Not (cert Is Nothing) Then
                        ' add the certificate to recipients collection
                        recipients.Add(cert)
                    End If
                Next

                ' Encrypt the message
                message.Encrypt(CType(recipients.ToArray(GetType(Certificate)), Certificate()))
            End If
        End If

        Dim client As New Smtp
        Try
            ' set SSL parameters to accept all server certificates...
            ' do not do this in production code, server certificates should
            ' be verified - use ValidatingCertificate event instead
            client.Settings.SslAcceptAllCertificates = True

            ' connect
            Console.WriteLine("Connecting {0} to {1}:{2}...", config.Security, config.Server, config.Port)
            client.Connect(config.Server, config.Port, config.Security)

            ' login if username and password was submitted
            If Not (config.User Is Nothing) AndAlso Not (config.Password Is Nothing) Then
                Console.WriteLine("Logging in as {0}...", config.User)
                client.Login(config.User, config.Password)
            End If

            ' send message
            Console.WriteLine("Sending message...")
            client.Send(message)

            Return 0
        Catch x As Exception
            ' Show exception message
            Console.WriteLine("Error occured: {0}", x.Message)

            ' Show list of rejected recipients + rejection reason
            If TypeOf x Is SmtpException Then
                Dim sx As SmtpException = CType(x, SmtpException)
                ' Display recipients reject by the server
                Dim rr As SmtpRejectedRecipient
                For Each rr In sx.GetRejectedRecipients()
                    Console.WriteLine("Rejected: {0} - {1}", rr.Address, rr.Response.Description)
                Next rr
            End If

            ' Show exception details
            Console.WriteLine()
            Console.WriteLine(x)
            Return 1
        Finally
            ' disconnect
            Console.WriteLine("Disconnecting...")
            client.Disconnect()
            client.Dispose()
        End Try
    End Function    'Main

    ''' <summary>
    ''' Finds the certificate in the certificate store.
    ''' </summary>
    ''' <param name="address">The email address.</param>
    ''' <param name="specified">Primary certificate store.</param>
    ''' <param name="additional">Additional certificate store.</param>
    ''' <param name="options">CertificateFindOptions.</param>
    ''' <returns></returns>
    Public Shared Function FindCertificate(ByVal address As String, ByVal specified As CertificateStore, ByVal additional As CertificateStore, ByVal options As CertificateFindOptions) As Certificate
        ' try to find certificate in the primary CertificateStore
        Dim cert As Certificate() = specified.FindCertificatesForMailAddress(address, options)

        ' if not found try to locate it in the secondary CertificateStore
        If (cert.Length = 0 And Not additional Is Nothing) Then
            cert = additional.FindCertificatesForMailAddress(address, options)
        End If

        If (cert.Length = 0) Then
            Return Nothing
        End If

        Return cert(0)
    End Function

End Class 'Program
