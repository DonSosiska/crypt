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
Imports Rebex.Mail

''' <summary>
''' This program converts mail messages between different formats.
''' </summary>
Class Program
    ''' <summary>
    ''' The main entry point for the application.
    ''' </summary>
    <STAThread()> _
    Shared Sub Main(ByVal args As String())
            ' set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key


        If args.Length <> 3 Then
            ShowHelp()
            Return
        End If

        Dim sourcePath As String = args(1)
        Dim targetPath As String = args(2)

        ' check if the sourcePath exists
        If Not File.Exists(sourcePath) Then
            Console.WriteLine("Cannot find the source file.")
            Return
        End If

        Dim format As MailFormat
        Select Case args(0).ToLowerInvariant()
            Case "-tomime"
                format = MailFormat.Mime
                Exit Select
            Case "-tomsg"
                format = MailFormat.OutlookMsg
                Exit Select
            Case Else
                Console.WriteLine("Invalid format specified.")
                Return
        End Select

        ' convert the mail message
        Dim mail As New MailMessage
        mail.Load(sourcePath)
        mail.Save(targetPath, format)
    End Sub

    Private Shared Sub ShowHelp()
        Dim applicationName As String = AppDomain.CurrentDomain.FriendlyName
        Console.WriteLine("=====================================================================")
        Console.WriteLine(" {0} ", applicationName)
        Console.WriteLine("=====================================================================")
        Console.WriteLine()
        Console.WriteLine("Converts e-mail messages between different formats.")
        Console.WriteLine()
        Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.")
        Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/")

        Console.WriteLine()
        Console.WriteLine("Syntax: MailConverter.exe -toMime|-toMsg sourcepath targetpath")

        Console.WriteLine()
        Console.WriteLine("Example: MailConverter.exe -tomsg C:\mail.eml C:\mail.msg")
        Console.WriteLine("Example: MailConverter.exe -tomime C:\mail.msg C:\mail.eml")
    End Sub

End Class
