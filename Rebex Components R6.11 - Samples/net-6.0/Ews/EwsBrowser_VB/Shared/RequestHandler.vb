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

Imports System.Collections

Imports Rebex.Net
Imports Rebex.Security.Certificates


''' <summary>
''' Represents a handler for loading client certificates.
''' </summary>
Public Class RequestHandler
    Implements ICertificateRequestHandler
    ' certificate cache
    Private Shared _chosenCertificates As New Hashtable()

    Public Function Request(socket As TlsSocket, issuers As DistinguishedName()) As CertificateChain Implements ICertificateRequestHandler.Request
        ' try to locate certificate in cache
        Dim serverCertificateFingerprint As String = BitConverter.ToString(socket.ServerCertificate.LeafCertificate.GetCertHash())

        If _chosenCertificates.Contains(serverCertificateFingerprint) Then
            Return TryCast(_chosenCertificates(serverCertificateFingerprint), CertificateChain)
        End If

        ' try to locate certificate in Store
        Dim my As New CertificateStore("MY")
        Dim certs As Certificate()

        If issuers.Length > 0 Then
            certs = my.FindCertificates(issuers, CertificateFindOptions.IsTimeValid Or CertificateFindOptions.HasPrivateKey Or CertificateFindOptions.ClientAuthentication)
        Else
            certs = my.FindCertificates(CertificateFindOptions.IsTimeValid Or CertificateFindOptions.HasPrivateKey Or CertificateFindOptions.ClientAuthentication)
        End If

        If certs.Length = 0 Then
            Return Nothing
        End If

        ' confirm whether to use the certificate
        Dim rhForm As New RequesetHandlerForm()
        rhForm.LoadData(certs)

        If rhForm.ShowDialog() <> System.Windows.Forms.DialogResult.OK Then
            Return Nothing
        End If

        Dim chain As CertificateChain = Nothing

        If rhForm.Certificate IsNot Nothing Then
            chain = CertificateChain.BuildFrom(rhForm.Certificate)
        End If

        ' save chosen certificate to cache
        _chosenCertificates.Add(serverCertificateFingerprint, chain)

        Return chain
    End Function
End Class

