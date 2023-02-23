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
Imports Rebex.Net



''' <summary>
''' Sample application for sendig email via SMTP.
''' </summary>
Class Program
    Private Shared _server As String = Nothing
    Private Shared _port As Integer = Smtp.DefaultPort
    Private Shared _from As String = Nothing
    Private Shared _to As String = Nothing
    Private Shared _subject As String = Nothing
    Private Shared _body As String = Nothing


    ''' <summary>
    ''' Program entrypoint.
    ''' </summary>
    ''' <param name="args">Program arguments.</param>
    ''' <returns>Status: 0=OK; 1=Not sent; 2=Bad arguments;</returns>
    <STAThread()> Overloads Shared Function Main(ByVal args() As String) As Integer
            ' set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key


        ' read command line
        Dim result As Integer = ParseCommandline(args)
        If result <> 0 Then
            Return result
        End If

        Try
            ' send message
            Console.WriteLine("Sending message using {0}:{1}...", _server, _port)
            Smtp.Send(_from, _to, _subject, _body, _server, _port)
            Return 0
        Catch x As Exception
            Console.WriteLine("Error occured: {0}" & ControlChars.Lf, x.Message)
            Console.WriteLine(x)
            Return 1
        End Try
    End Function    'Main


    ''' <summary>
    ''' Reads server hostname and server port from argument in the form of "servername:port".
    ''' </summary>
    ''' <param name="serverAddress">Host[:port].</param>
    Private Shared Sub ParseServerHostnameAndPort(ByVal serverAddress As String)
        Dim p As Integer = serverAddress.IndexOf(":"c)

        If p >= 0 Then
            _server = serverAddress.Substring(0, p)
            _port = Integer.Parse(serverAddress.Substring((p + 1)))

            If _port <= 0 OrElse _port > 65535 Then
                Throw New ApplicationException(String.Format("Invalid port {0}.", _port))
            End If
        Else
            _server = serverAddress
        End If
    End Sub    'ParseServerHostnameAndPort


    ''' <summary>
    ''' Reads arguments from command line and stores them into class properties.
    ''' </summary>
    ''' <param name="args">Arguments passed to the Main function</param>
    ''' <returns>0 = success, anything other = fail</returns>
    Private Shared Function ParseCommandline(ByVal args() As String) As Integer
        Try
            Dim i As Integer
            For i = 0 To args.Length - 1
                Dim arg As String = args(i)

                ' most parameters starts with "-" or "/" 
                If Not arg.StartsWith("-") AndAlso Not arg.StartsWith("/") Then
                    If Not (_server Is Nothing) Then
                        Throw New ApplicationException(String.Format("Unexpected argument '{0}'.", arg))
                    End If
                    ParseServerHostnameAndPort(arg)
                    GoTo ContinueFor1
                End If

                If i >= args.Length - 1 Then
                    Throw New ApplicationException(String.Format("Missing value for argument '{0}'.", arg))
                End If
                Select Case arg.Substring(1).ToLower()
                    Case "from"
                        i += 1
                        _from = args(i)

                    Case "to"
                        i += 1
                        _to = args(i)

                    Case "subject"
                        i += 1
                        _subject = args(i)

                    Case "body"
                        i += 1
                        _body = args(i)

                    Case Else
                        Throw New ApplicationException(String.Format("Unknown argument '{0}'.", arg))
                End Select
ContinueFor1:
            Next i

            '
            ' Check mandatory arguments
            '
            If _server Is Nothing Then
                Throw New ApplicationException("Server name not specified.")
            End If
            If _from Is Nothing Then
                Throw New ApplicationException("Sender (from:) not specified.")
            End If
            If _to Is Nothing Then
                Throw New ApplicationException("Recipient (to:) not specified.")
            End If
            If _subject Is Nothing Then
                Throw New ApplicationException("Mail subject not specified.")
            End If
            If _body Is Nothing Then
                Throw New ApplicationException("Mail body not specified.")
            End If
            Return 0          ' success
        Catch ex As ApplicationException
            Console.WriteLine("Error: {0}", ex.Message)

            Dim applicationName As String = AppDomain.CurrentDomain.FriendlyName
            Console.WriteLine("=====================================================================")
            Console.WriteLine(" {0} ", applicationName)
            Console.WriteLine("=====================================================================")
            Console.WriteLine("")
            Console.WriteLine("Sends simple e-mail from the command line.")
            Console.WriteLine("")
            Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.")
            Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/")
            Console.WriteLine("")

            Console.WriteLine()
            Console.WriteLine("Syntax: {0} server[:port]" & ControlChars.Lf & _
                ControlChars.Tab & "-from mail@domain" & ControlChars.Lf & _
                ControlChars.Tab & "-to mail@domain;mail2@domain2" & ControlChars.Lf & _
                ControlChars.Tab & "-subject ""subject""" & ControlChars.Lf & _
                ControlChars.Tab & "-body ""text""" & ControlChars.Lf, _
                applicationName)

            Return 2
        End Try
    End Function    'ParseCommandline
End Class 'Program
