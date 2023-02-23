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

Imports System.Text

Imports Rebex.Net
Imports Rebex.Security.Certificates

''' <summary>
''' Represents certificate verifier.
''' </summary>
Public Class Verifier
    ''' <summary>
    ''' Validates given certificate.
    ''' </summary>
    Public Shared Sub ValidatingCertificate(sender As Object, e As SslCertificateValidationEventArgs)
        Dim verifier As New Verifier()
        Dim acceptResult As TlsCertificateAcceptance = verifier.Verify(e.ServerName, e.CertificateChain)
        If acceptResult = TlsCertificateAcceptance.Accept Then
            e.Accept()
        Else
            e.Reject(acceptResult)
        End If
    End Sub

    ''' <summary>
    ''' Validates given certificate chain.
    ''' </summary>
    Public Function Verify(commonName As String, certificateChain As CertificateChain) As TlsCertificateAcceptance
        ' validate certificate chain
        Dim res As ValidationResult = certificateChain.Validate(commonName, 0)
        If res.Valid Then
            Return TlsCertificateAcceptance.Accept
        End If

        ' determine whether to display button for adding root certificate
        Dim showAddToStore As Boolean = False
        If (res.Status And ValidationStatus.RootNotTrusted) <> 0 AndAlso certificateChain.RootCertificate IsNot Nothing Then
            showAddToStore = True
        End If

        Dim certForm As New VerifierForm(certificateChain.LeafCertificate)
        certForm.Problem = GetProblemString(res)
        certForm.ShowAddIssuerToTrustedCaStoreButton = showAddToStore
        certForm.ShowDialog()

        ' add certificate of the issuer CA to trusted authorities store
        If certForm.AddIssuerCertificateAuthothorityToTrustedCaStore Then
            Dim trustedCaStore As New CertificateStore(CertificateStoreName.Root)
            trustedCaStore.Add(certificateChain.RootCertificate)
        End If

        If certForm.Accepted Then
            Return TlsCertificateAcceptance.Accept
        End If

        If (res.Status And ValidationStatus.TimeNotValid) <> 0 Then
            Return TlsCertificateAcceptance.Expired
        End If
        If (res.Status And ValidationStatus.Revoked) <> 0 Then
            Return TlsCertificateAcceptance.Revoked
        End If
        If (res.Status And (ValidationStatus.RootNotTrusted Or ValidationStatus.IncompleteChain)) <> 0 Then
            Return TlsCertificateAcceptance.UnknownAuthority
        End If
        If (res.Status And (ValidationStatus.Malformed Or ValidationStatus.UnknownError)) <> 0 Then
            Return TlsCertificateAcceptance.Other
        End If

        Return TlsCertificateAcceptance.Bad
    End Function

    ''' <summary>
    ''' Gets the string representation of the <see cref="ValidationStatus"/>.
    ''' </summary>
    Public Shared Function GetProblemString(res As ValidationResult) As String
        Dim status As ValidationStatus = res.Status
        Dim sb As New StringBuilder()
        Dim values As ValidationStatus() = DirectCast([Enum].GetValues(GetType(ValidationStatus)), ValidationStatus())
        For i As Integer = 0 To values.Length - 1
            If (status And values(i)) = 0 Then
                Continue For
            End If

            status = status Xor values(i)
            Dim problem As String

            Select Case values(i)
                Case ValidationStatus.TimeNotValid
                    problem = "Server certificate has expired or is not valid yet."
                    Exit Select
                Case ValidationStatus.Revoked
                    problem = "Server certificate has been revoked."
                    Exit Select
                Case ValidationStatus.RootNotTrusted
                    problem = "Server certificate was issued by an untrusted authority."
                    Exit Select
                Case ValidationStatus.IncompleteChain
                    problem = "Server certificate does not chain up to a trusted root authority."
                    Exit Select
                Case ValidationStatus.Malformed
                    problem = "Server certificate is malformed."
                    Exit Select
                Case ValidationStatus.CnNotMatch
                    problem = "Server hostname does not match the certificate."
                    Exit Select
                Case ValidationStatus.UnknownError
                    problem = String.Format("Error {0:x} encountered while validating server's certificate.", res.NativeErrorCode)
                    Exit Select
                Case Else
                    problem = values(i).ToString()
                    Exit Select
            End Select

            sb.AppendFormat("{0}" & vbCr & vbLf, problem)
        Next
        Return sb.ToString()
    End Function
End Class
