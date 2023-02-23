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


''' <summary>
''' Provides data for error handling events.
''' </summary>
Public Class ErrorOccuredEventArgs
    Inherits EventArgs
    Private _error As Exception
    Private _handled As Boolean

    ''' <summary>
    ''' An error provided by the producer.
    ''' </summary>
    Public ReadOnly Property [Error]() As Exception
        Get
            Return _error
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether the error was handled.
    ''' </summary>
    Public Property Handled() As Boolean
        Get
            Return _handled
        End Get
        Set
            _handled = value
        End Set
    End Property

    ''' <summary>
    ''' Initializes new instance of the <see cref="ErrorOccuredEventArgs"/>.
    ''' </summary>
    Public Sub New([error] As Exception)
        Me.New([error], False)
    End Sub

    ''' <summary>
    ''' Initializes new instance of the <see cref="ErrorOccuredEventArgs"/>.
    ''' </summary>
    Public Sub New([error] As Exception, handled As Boolean)
        _error = [error]
        _handled = handled
    End Sub
End Class
