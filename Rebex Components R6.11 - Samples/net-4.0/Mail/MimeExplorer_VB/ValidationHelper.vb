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
Imports Rebex.Mail
Imports Rebex.Security.Certificates
Imports Rebex.Security.Cryptography.Pkcs


''' <summary>
''' Provides various helper methods useful for signature validation.
''' </summary>
Public Class ValidationHelper

    ''' <summary>
    ''' Displays the list of problems of the message that was not validated successfully.
    ''' </summary>
    ''' <param name="result">Validation result.</param>
    Public Shared Sub ShowProblems(ByVal result As MailSignatureValidity)
        Console.WriteLine("There were following problems:")

        Dim missing() As String = result.GetMissingSignatures()
        If missing.Length > 0 Then
            Console.WriteLine("- The signatures for the following senders are missing:")
            Dim i As Integer
            For i = 0 To missing.Length
                Console.WriteLine("    {0}", missing(i))
            Next
        End If

        Dim certStatus As ValidationStatus = result.CertificateValidationStatus
        Dim vs As ValidationStatus
        For Each vs In [Enum].GetValues(GetType(ValidationStatus))
            If (vs And certStatus) <> 0 Then
                Console.WriteLine("- {0}", GetCertificateValidationStatusDescription(vs))
            End If
        Next

        Dim signStatus As SignatureValidationStatus = result.Status
        Dim ss As SignatureValidationStatus
        For Each ss In [Enum].GetValues(GetType(SignatureValidationStatus))
            If (ss And signStatus) <> 0 Then
                Console.WriteLine("- {0}", GetSignatureValidationStatusDescription(ss))
            End If
        Next
    End Sub 'Validate

    ''' <summary>
    ''' Returns a descriptions of a single ValidationStatus flag.
    ''' </summary>
    ''' <param name="status">Certificate validation status</param>
    ''' <returns>A description.</returns>
    Public Shared Function GetCertificateValidationStatusDescription(ByVal status As ValidationStatus) As String
        Select Case status
            Case ValidationStatus.TimeNotValid
                Return "Certificate has expired or is not valid yet."
            Case ValidationStatus.Revoked
                Return "Certificate has been revoked."
            Case ValidationStatus.RootNotTrusted
                Return "Certificate was issued by an untrusted authority."
            Case ValidationStatus.IncompleteChain
                Return "Certificate does not chain up to a trusted root authority."
            Case ValidationStatus.Malformed
                Return "Certificate is malformed."
            Case Else
                Return status.ToString()
        End Select
    End Function 'GetCertificateValidationStatusDescription

    ''' <summary>
    ''' Returns a descriptions of a single SignatureValidationStatus flag.
    ''' </summary>
    ''' <param name="status">Signature validation status</param>
    ''' <returns>A description.</returns>
    Public Shared Function GetSignatureValidationStatusDescription(ByVal status As SignatureValidationStatus) As String
        Select Case status
            Case SignatureValidationStatus.CertificateNotValid
                Return "Certificate is not valid."
            Case SignatureValidationStatus.CertificateNotAvailable
                Return "Certificate is not available."
            Case SignatureValidationStatus.UnsupportedDigestAlgorithm
                Return "A digest algorithm is not supported."
            Case SignatureValidationStatus.UnsupportedSignatureAlgorithm
                Return "A signature algorithm is not supported."
            Case SignatureValidationStatus.InvalidSignature
                Return "A signature is invalid."
            Case SignatureValidationStatus.InvalidKeyUsage
                Return "Invalid key usage."
            Case SignatureValidationStatus.ContentTypeMismatch
                Return "Content type mismatch."
            Case Else
                Return "Unknown error."
        End Select
    End Function 'GetSignatureValidationStatusDescription
End Class 'ConsoleValidation
