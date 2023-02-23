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
''' Represents certificate's thumbprint verifier.
''' </summary>
Public Class ThumbprintVerifier
    Private _isAccepted As Boolean
    Private _thumbprint As String

    ''' <summary>
    ''' Gets the value indicating whether the validated certificate was accepted.
    ''' </summary>
    Public ReadOnly Property IsAccepted() As Boolean
        Get
            Return _isAccepted
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the thumbprint for certificate validation.
    ''' If a certificate with different thumbprint was accepted, 
    ''' this value is changed to the accepted certificate's thumbprint.
    ''' </summary>
    Public ReadOnly Property Thumbprint() As String
        Get
            Return _thumbprint
        End Get
    End Property

    ''' <summary>
    ''' Initializes new instance of the <see cref="ThumbprintVerifier"/>.
    ''' </summary>
    Public Sub New(thumbprint As String)
        _thumbprint = thumbprint
    End Sub

    ''' <summary>
    ''' Validates certificate based on the specified thumbprint.
    ''' </summary>
    Public Sub ValidatingCertificate(sender As Object, e As SslCertificateValidationEventArgs)
        Dim acceptResult As TlsCertificateAcceptance = TlsCertificateAcceptance.Bad

        ' compare thumbprints
        If String.Equals(Thumbprint, e.Certificate.Thumbprint, StringComparison.OrdinalIgnoreCase) Then
            acceptResult = TlsCertificateAcceptance.Accept
        Else
            Dim problem As String
            If String.IsNullOrEmpty(Thumbprint) Then
                problem = "No locally stored thumbprint to compare this server certificate with."
            Else
                problem = "Locally stored thumbprint differs from server certificate's thumbprint."
            End If

            ' check if the certificate is valid
            Dim res As ValidationResult = e.CertificateChain.Validate(e.ServerName, 0)
            If Not res.Valid Then
                problem = String.Format("{0}" & vbCr & vbLf & "{1}", problem, Verifier.GetProblemString(res))
            End If

            ' ask to trust this certificate
            Dim certForm As New VerifierForm(e.Certificate)
            certForm.Problem = problem
            certForm.ShowDialog()
            If certForm.Accepted Then
                acceptResult = TlsCertificateAcceptance.Accept
            End If
        End If

        If acceptResult = TlsCertificateAcceptance.Accept Then
            _thumbprint = e.Certificate.Thumbprint
            _isAccepted = True
            e.Accept()
        Else
            _isAccepted = False
            e.Reject(acceptResult)
        End If
    End Sub
End Class
