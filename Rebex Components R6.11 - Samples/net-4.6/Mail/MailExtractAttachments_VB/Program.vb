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
Imports Rebex.Mail

Class Program

    <STAThread()> Shared Function Main(ByVal args() As String) As Integer
            ' set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key


        ' If no argument specified show syntax
        If args.Length < 1 Then
            ShowHelp()
            Return 1
        End If

        ' Load the mail message from disk
        Dim mail As New MailMessage
        mail.Load(args(0))

        ' Decrypt the message if it is encrypted
        If mail.IsEncrypted Then
            If Not mail.CanDecrypt Then
                Console.WriteLine("Message cannot be decrypted. You do not have the private key.")
                Return 2
            End If
            mail.Decrypt()
        End If

        ' Validate the signature if the message is signed
        If mail.IsSigned Then
            Dim result As MailSignatureValidity = mail.ValidateSignature()
            If result.Valid Then
                Console.WriteLine("The message is signed and the signature is valid.")
            Else
                Console.WriteLine("The message is signed, but the signature is not valid.")
                ValidationHelper.ShowProblems(result)
            End If
        End If

        Console.WriteLine("Message contains {0} attachments.", mail.Attachments.Count)

        ' If message has no attachments, just exit
        If mail.Attachments.Count = 0 Then Return 0

        Dim attachment As attachment
        For Each attachment In mail.Attachments
            ' Save the file
            Console.WriteLine("Saving '{0}' ({1}).", attachment.FileName, attachment.MediaType)
            attachment.Save(attachment.FileName)
        Next attachment

        Return 0
    End Function    'Main

    Private Shared Sub ShowHelp()
        Dim applicationName As String = AppDomain.CurrentDomain.FriendlyName
        Console.WriteLine("=====================================================================")
        Console.WriteLine(" {0} ", applicationName)
        Console.WriteLine("=====================================================================")
        Console.WriteLine("")
        Console.WriteLine("Extracts all attachments from an e-mail message.")
        Console.WriteLine("Supported e-mail formats: .EML (MIME) and .MSG (Microsoft Outlook).")
        Console.WriteLine("")
        Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.")
        Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/")
        Console.WriteLine("")
        Console.WriteLine("Syntax is: {0} <mailfile.eml|mailfile.msg>", applicationName)
    End Sub    'ShowHelp


End Class 'Program