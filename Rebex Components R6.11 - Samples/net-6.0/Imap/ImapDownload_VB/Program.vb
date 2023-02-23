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


''' <summary>
''' Sample application for downloading mail messages from IMAP servers.
''' </summary>
Class Program

    ''' <summary>
    ''' Program entrypoint.
    ''' </summary>
    ''' <param name="args">Program arguments.</param>
    ''' <returns>Status. (0=success, anything else = failure)</returns>
    <STAThread()> Overloads Shared Function Main(ByVal args() As String) As Integer
            ' set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key


        '
        ' Parse command line arguments. Show command line syntax in case of error.
        '
        Dim config As Arguments
        Try
            config = New Arguments(args)
        Catch e As Exception
            Dim applicationName As String = AppDomain.CurrentDomain.FriendlyName
            Console.WriteLine("=====================================================================")
            Console.WriteLine(" {0} ", applicationName)
            Console.WriteLine("=====================================================================")
            Console.WriteLine("")
            Console.WriteLine("Downloads e-mail messages from IMAP server.")
            Console.WriteLine("")
            Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.")
            Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/")
            Console.WriteLine("")

            Console.WriteLine()
            Console.WriteLine("Syntax: {0} none|explicit|implicit server[:port] -username username -password password [-folder path] [-format MIME|MSG]", applicationName)
            Console.WriteLine(("Specific error: " & e.Message))
            Return 2
        End Try

        Dim client As New Imap
        Try
            ' set SSL to accept all server certificates...
            ' do not do this in production code, server certificates should
            ' be verified - use ValidatingCertificate event instead
            client.Settings.SslAcceptAllCertificates = True

            ' connect
            Console.WriteLine("Connecting {0} to {1}:{2}...", config.Security, config.Server, config.Port)
            client.Connect(config.Server, config.Port, config.Security)

            ' Login
            Console.WriteLine("Authorizing as {0}...", config.User)
            client.Login(config.User, config.Password)

            ' Select folder
            Console.WriteLine("Selecting folder '{0}'...", config.Folder)
            client.SelectFolder(config.Folder)
            Dim folder As ImapFolder = client.CurrentFolder

            ' Show number of messages in the folder
            Console.WriteLine("{0} messages found.", folder.TotalMessageCount)

            ' Get message list
            Console.WriteLine("Retrieving message list...")
            Dim list As ImapMessageCollection = client.GetMessageList()

            ' Download each message
            Dim i As Integer
            For i = 0 To list.Count - 1
                Dim message As ImapMessageInfo = list(i)

                ' Create filename from email unique ID
                Dim filename As String = FixFilename(message.UniqueId)

                If config.Format = Mail.MailFormat.OutlookMsg Then
                    filename &= ".msg"
                Else
                    filename &= ".eml"
                End If

                ' Download messages (new only)
                If File.Exists(filename) Then
                    Console.WriteLine("Skipping message {0}...", message.UniqueId)
                Else
                    Console.WriteLine("Retrieving message {0} and saving as {1}...", message.UniqueId, filename)

                    If config.Format = Mail.MailFormat.OutlookMsg Then
                        ' get message from IMAP (and parse it)
                        Dim messageToSave As MailMessage = client.GetMailMessage(message.UniqueId)

                        ' save message in MSG format
                        messageToSave.Save(filename, MailFormat.OutlookMsg)
                    Else
                        ' get message from IMAP directly into file in MIME format (without parsing)
                        client.GetMessage(message.UniqueId, filename)
                    End If

                End If
            Next i

            ' Disconnect
            Console.WriteLine("Disconnecting...")
            client.Disconnect()

            Return 0
        Catch x As Exception
            Console.WriteLine("Error occured: {0}" & ControlChars.Lf, x.Message)
            Console.WriteLine(x)

            Return 1
        Finally
            client.Dispose()
        End Try
    End Function    'Main

    ''' <summary>
    ''' Creates a usable filename from string parameter.
    ''' </summary>
    ''' <param name="originalFilename">String to be converted - e.g. mail subject or unique ID.</param>
    ''' <returns>A usable filename based on the supplied string.</returns>
    Private Shared Function FixFilename(ByVal originalFilename As String) As String
        Return BitConverter.ToString(Encoding.ASCII.GetBytes(originalFilename)).Replace("-", "")
    End Function    'FixFilename

End Class 'Program 
