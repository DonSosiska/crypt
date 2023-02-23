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
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms

Class Program
    ''' <summary>
    ''' The main entry point for the application.
    ''' </summary>
    <STAThread()> Public Overloads Shared Sub Main(ByVal args() As String)
            ' set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key


        Application.EnableVisualStyles()
        Dim editor As New MailEditor

        ' If a parameter was specified, load the message.
        If args.Length <> 0 Then
            editor.LoadMessage(args(0))
        End If

        Application.Run(editor)
    End Sub    'Main
End Class 'Program
