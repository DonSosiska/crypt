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
Imports System.Configuration
Imports System.Text
Imports Rebex.Net
Imports Rebex.Mime.Headers


''' <summary>
''' Command line utility to checks validity of e-mail addresses.
''' Parses specified e-mail addresses and resolves thair domain MX records.
''' </summary>

Class Program


    <STAThread()> Public Shared Function Main(ByVal args() As String) As Integer
            ' set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key


        If args.Length = 0 Then
            ' display syntax if no parameter is specified
            Dim applicationName As String = AppDomain.CurrentDomain.FriendlyName
            Console.WriteLine("=====================================================================")
            Console.WriteLine(" {0} ", applicationName)
            Console.WriteLine("=====================================================================")
            Console.WriteLine("")
            Console.WriteLine("Command line utility to check validity of e-mail addresses.")
            Console.WriteLine("Parses specified e-mail addresses and resolves their domain MX records.")
            Console.WriteLine("")
            Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.")
            Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/")
            Console.WriteLine("")

            Console.WriteLine("Syntax: {0} address1 [address2 [address3] ...]", applicationName)
            Return 1
        End If

        ' variables to hold total counts
        Dim total As Integer = 0
        Dim ok As Integer = 0

        ' iterate through the supplied list of arguments
        Dim i As Integer
        For i = 0 To args.Length - 1
            ' parse each argument into a address list
            Dim list As New MailAddressCollection(args(i))

            ' check each address validity and resolve MX
            Dim box As MailAddress
            For Each box In list
                total += 1
                If box.Host = "" Then
                    Console.WriteLine("Address '{0}': Invalid address.", box)
                Else
                    Try
                        ' resolve and display MX records, if available
                        Dim mailServers As String() = Smtp.ResolveDomainMX(box.Host)
                        If mailServers.Length = 0 Then
                            Console.WriteLine("Address '{0}' has no MX records.", box)
                        Else
                            Console.WriteLine("Address '{0}' resolves to: {1}.", box, String.Join(", ", mailServers))
                            ok += 1
                        End If
                    Catch x As NotSupportedException
                        Console.WriteLine("Sample feature is not supported. Reason: {0}", x.Message)
                    Catch x As Exception
                        Console.WriteLine("Address '{0}': {1}", box, x.Message)
                    End Try
                End If

            Next box
        Next i

        ' display statistics
        Console.WriteLine()
        Console.WriteLine("Resolved {1} out of {1} domains.", ok, total)
        Return 0
    End Function       'Main
End Class   'Program 'Rebex.Samples.VerifyAddress