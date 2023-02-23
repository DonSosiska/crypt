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
Imports System.Collections
Imports System.Windows.Forms

''' <summary>
''' Class for sorting collections of type System.Windows.Forms.ListViewItem.
''' </summary>
Public Class ListViewItemSorter
    Implements IComparer

    ''' <summary>
    ''' Types of value comparison.
    ''' </summary>
    Public Enum CompareTypes
        ''' <summary>
        ''' Compare values as strings.
        ''' </summary>
        Strings

        ''' <summary>
        ''' Compare values as integers.
        ''' </summary>
        Integers
    End Enum    'CompareTypes


    ''' <summary>
    ''' Specifies the column to be sorted.
    ''' </summary>
    Private _sortColumn As Integer = 0

    ''' <summary>
    ''' Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
    ''' </summary>
    Public Property SortColumn() As Integer
        Get
            Return _sortColumn
        End Get
        Set(ByVal Value As Integer)
            _sortColumn = Value
        End Set
    End Property

    ''' <summary>
    ''' Specifies the order in which to sort.
    ''' </summary>
    Private _sorting As SortOrder = SortOrder.None

    ''' <summary>
    ''' Gets or sets the order of sorting.
    ''' </summary>
    Public Property Sorting() As SortOrder
        Get
            Return _sorting
        End Get
        Set(ByVal Value As SortOrder)
            _sorting = Value
        End Set
    End Property

    ''' <summary>
    ''' Specifies the type of comparison.
    ''' </summary>
    Private _compareType As CompareTypes = CompareTypes.Strings

    ''' <summary>
    ''' Gets or sets the type of comparison.
    ''' </summary>
    Public Property CompareType() As CompareTypes
        Get
            Return _compareType
        End Get
        Set(ByVal Value As CompareTypes)
            _compareType = Value
        End Set
    End Property


    ''' <summary>
    ''' Case insensitive comparer object.
    ''' </summary>
    Private _objectCompare As New CaseInsensitiveComparer


    ''' <summary>
    ''' This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
    ''' </summary>
    ''' <param name="x">First object to be compared</param>
    ''' <param name="y">Second object to be compared</param>
    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare

        ' If None sorting is specified, don't compare
        If _sorting = SortOrder.None Then Return 0

        Dim result As Integer = 0
        Dim stringX As String = CType(x, ListViewItem).SubItems(_sortColumn).Text
        Dim stringY As String = CType(y, ListViewItem).SubItems(_sortColumn).Text

        ' Compare two ListViewItem values
        If CompareType = CompareTypes.Strings Then
            result = _objectCompare.Compare(stringX, stringY)
        Else
            result = Comparer.Default.Compare(Integer.Parse(stringX), Integer.Parse(stringY))
        End If

        ' If Descending sorting is specified, negate result
        If _sorting = SortOrder.Descending Then Return -result

        Return result

    End Function       'Compare

End Class 'ListViewItemSorter