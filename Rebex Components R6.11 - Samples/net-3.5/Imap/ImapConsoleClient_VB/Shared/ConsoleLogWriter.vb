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

Imports System.Collections.Generic
Imports System.Text

Public Class ConsoleLogWriter
    Inherits LogWriterBase
    Public Sub New(logAs As LogLevel)
        Level = logAs
    End Sub

    Protected Overrides Sub WriteMessage(message As String)
        Console.WriteLine(message)
    End Sub

    Public Overrides Sub Write(logAs As LogLevel, objectType As Type, objectId As Integer, area As String, message As String)
        If logAs < Level Then
            Return
        End If
        WriteMessage(String.Format("{0}: {1}", area, message))
    End Sub
End Class
