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

Imports System.ComponentModel


''' <summary>
''' Which protocol to use.
''' </summary>
Public Enum ProtocolMode
    ''' <summary>
    ''' HTTPS protocol.
    ''' </summary>
    <Description("EWS over HTTPS")>
    HTTPS = 0

    ''' <summary>
    ''' HTTP protocol.
    ''' </summary>
    <Description("EWS over HTTP")>
    HTTP = 1
End Enum

''' <summary>
''' How to verify server certificate in SSL communication.
''' </summary>
Public Enum CertificateVerifyingMode
    ''' <summary>
    ''' Don't verify the server certificate - accept any certificate.
    ''' </summary>
    AcceptAnyCertificate
    ''' <summary>
    ''' Verify the server certificate against trusted certificates stored in Windows.
    ''' </summary>
    UseWindowsInfrastructure
    ''' <summary>
    ''' Verify the server certificate against locally stored certificate thumbprint (SH1 hash). 
    ''' </summary>
    LocalyStoredThumbprint
End Enum

