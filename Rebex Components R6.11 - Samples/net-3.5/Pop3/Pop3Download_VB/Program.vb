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
''' Sample application for downloading mail messages from POP3 servers.
''' </summary>
Public Class SmtpProgram

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
            Console.WriteLine("Downloads e-mail messages from POP3 server.")
            Console.WriteLine("")
            Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.")
            Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/")

            Console.WriteLine()
            Console.WriteLine("Syntax: {0} none|explicit|implicit server[:port] -username username -password password [-format mime|msg]", applicationName)
            Console.WriteLine(("Specific error: " & e.Message))
            Return 2
        End Try

        Dim client As New Pop3
        Try
            ' set SSL parameters to accept all server certificates...
            ' do not do this in production code, server certificates should
            ' be verified - use ValidatingCertificate event instead
            client.Settings.SslAcceptAllCertificates = True

            ' connect
            Console.WriteLine("Connecting {0} to {1}:{2}...", config.Security, config.Server, config.Port)
            client.Connect(config.Server, config.Port, config.Security)

            ' Login
            Console.WriteLine("Authorizing as {0}...", config.User)
            client.Login(config.User, config.Password)

            ' Get message list
            Console.WriteLine("Retrieving message list...")
            Dim list As Pop3MessageCollection = client.GetMessageList()
            Console.WriteLine("{0} messages found.", list.Count)

            ' Download each message
            Dim i As Integer
            For i = 0 To list.Count - 1
                Dim message As Pop3MessageInfo = list(i)

                ' Create filename from email unique ID
                Dim filename As String = FixFilename(message.UniqueId)

                ' Append the correct extension according to format
                If config.Format = Mail.MailFormat.Mime Then
                    filename &= ".eml"
                Else
                    filename &= ".msg"
                End If

                ' Download messages (new only)
                If File.Exists(filename) Then
                    Console.WriteLine("Skipping message {0}...", message.UniqueId)
                Else
                    Console.WriteLine("Retrieving message {0}...", message.UniqueId)

                    If config.Format = MailFormat.Mime Then
                        ' Save message from POP3 directly in MIME format (without parsing)
                        client.GetMessage(message.SequenceNumber, filename)
                    Else
                        ' Get message from POP3 (and parse it)
                        Dim mes As MailMessage = client.GetMailMessage(message.SequenceNumber)
                        ' Save message in MSG format
                        mes.Save(filename, MailFormat.OutlookMsg)
                    End If
                End If
            Next i

            Return 0
        Catch x As Exception
            Console.WriteLine("Error occured: {0}" & ControlChars.Lf, x.Message)
            Console.WriteLine(x)

            Return 1
        Finally
            Console.WriteLine("Disconnecting...")
            client.Disconnect()
            client.Dispose()
        End Try
    End Function    'Main


    ''' <summary>
    ''' Creates a valid filename from string parameter.
    ''' </summary>
    ''' <param name="originalFilename">String to be converted - e.g. mail subject or unique ID.</param>
    ''' <returns>A valid filename based on the supplied string.</returns>
    Private Shared Function FixFilename(ByVal originalFilename As String) As String
        ' Characters allowed in the filename
        Dim allowed As String = " .-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"

        ' Replace invalid charactes with its hex representation
        Dim sb As New StringBuilder
        Dim i As Integer
        Dim ch As Char() = originalFilename.ToCharArray()

        For i = 0 To originalFilename.Length - 1
            If allowed.IndexOf(ch(i)) < 0 Then
                sb.AppendFormat("_{0:X2}", CInt(Val(ch(i))))
            Else
                sb.Append(ch(i))
            End If
        Next i
        Return sb.ToString()
    End Function    'FixFilename

End Class 'Program
