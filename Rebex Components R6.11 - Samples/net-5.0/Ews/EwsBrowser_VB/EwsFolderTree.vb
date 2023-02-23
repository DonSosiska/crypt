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
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms

Imports Rebex.Net

''' <summary>
''' Strongly typed tree view, which represents hierarchy of EWS folders.
''' </summary>
Public Class EwsFolderTree
    Inherits UcTreeView
    ' EWS client object
    Private _ews As Ews

    ''' <summary>
    ''' Gets the selected folder.
    ''' </summary>
    Public ReadOnly Property SelectedFolder() As FolderInfo
        Get
            Return DirectCast(SelectedNode.Info, FolderInfo)
        End Get
    End Property

    ''' <summary>
    ''' Binds specified EWS client object to the <see cref="EwsFolderTree"/> and defines root folder.
    ''' </summary>
    Public Overloads Function BindAsync(ews As Ews, root As EwsFolderId) As Task
        _ews = ews

        Return MyBase.BindAsync(root, "Inbox")
    End Function

    ''' <summary>
    ''' Retrieves subfolders of the specified folder.
    ''' </summary>
    Protected Overrides Async Function GetItemsAsync(id As Object) As Task(Of IEnumerable(Of INodeInfo))
        ' wait for other operations to finish
        While _ews.IsBusy
            Await Task.Delay(100)
        End While

        Dim list As New List(Of FolderInfo)()
        ' retrieve subfolders
        Dim folders As IList(Of EwsFolderInfo) = Await _ews.GetFolderListAsync(DirectCast(id, EwsFolderId))
        For Each f As EwsFolderInfo In folders
            list.Add(New FolderInfo(f))
        Next
        Return list
    End Function

    ''' <summary>
    ''' Represents an EWS folder.
    ''' </summary>
    Public Class FolderInfo
        Inherits NodeInfo(Of EwsFolderId)
        ''' <summary>
        ''' Gets the total items count within the info.
        ''' </summary>
        Public Shadows ReadOnly Property ItemsTotal() As Integer
            Get
                Return MyBase.ItemsTotal.Value
            End Get
        End Property

        ''' <summary>
        ''' Initializes new instance of the <see cref="FolderInfo"/>.
        ''' </summary>
        Public Sub New(info As EwsFolderInfo)
            MyBase.New(info.Id, info.Name, info.ItemsTotal, info.ChildFolderCount > 0)
        End Sub
    End Class
End Class
