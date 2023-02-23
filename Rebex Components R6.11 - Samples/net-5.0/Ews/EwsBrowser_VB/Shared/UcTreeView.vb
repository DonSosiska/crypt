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
''' Represents hierarchical tree view.
''' </summary>
Partial Public Class UcTreeView
    Inherits UserControl
    ''' <summary>
    ''' Retrieves child nodes of the node identified by the specified id.
    ''' </summary>
    ''' <remarks>
    ''' Due to Designer issues method has to be declared as virtual instead of abstract.
    ''' </remarks>
    Protected Overridable Function GetItemsAsync(id As Object) As Task(Of IEnumerable(Of INodeInfo))
        Throw New NotImplementedException()
    End Function

    ''' <summary>
    ''' Returns selected node.
    ''' </summary>
    Protected Property SelectedNode() As Node
        Get
            Return m_SelectedNode
        End Get
        Private Set(value As Node)
            m_SelectedNode = value
        End Set
    End Property
    Private m_SelectedNode As Node

    ''' <summary>
    ''' Occurs when selection is changed.
    ''' </summary>
    Public Event SelectionChanged As EventHandler

    ''' <summary>
    ''' Occurs when an exception is raised.
    ''' </summary>
    Public Event ErrorOccured As EventHandler(Of ErrorOccuredEventArgs)

    ''' <summary>
    ''' Initializes new instance of the <see cref="UcTreeView"/>.
    ''' </summary>
    ''' <remarks>
    ''' This class is not intended to be used publicly.
    ''' Only descendants can call the constructor.
    ''' </remarks>
    Protected Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Clears the content of the <see cref="UcTreeView"/>. 
    ''' </summary>
    Public Overridable Sub Clear()
        Try
            treeView.SuspendLayout()
            treeView.Nodes.Clear()
            treeView.ResumeLayout()
        Catch ex As Exception
            HandleError(ex)
        End Try
    End Sub

    ''' <summary>
    ''' Binds the root object to the <see cref="UcTreeView"/> and starts retrieval of its child nodes.
    ''' Also selects the initial node if found.
    ''' </summary>
    Protected Async Function BindAsync(rootId As Object, initialName As String) As Task
        Try
            Await LoadNodesAsync(rootId, treeView.Nodes)

            If initialName IsNot Nothing Then
                For Each node As Node In treeView.Nodes
                    If node.Text.Equals(initialName, StringComparison.OrdinalIgnoreCase) Then
                        treeView.SelectedNode = node
                        Return
                    End If
                Next
            End If

            If treeView.Nodes.Count > 0 Then
                treeView.SelectedNode = treeView.Nodes(0)
            End If
        Catch ex As Exception
            HandleError(ex)
        End Try
    End Function

    ''' <summary>
    ''' Retrieves child nodes and adds them to the <see cref="UcTreeView"/>.
    ''' </summary>
    Private Async Function LoadNodesAsync(id As Object, nodes As TreeNodeCollection) As Task
        Try
            Dim list As IEnumerable(Of INodeInfo) = Await GetItemsAsync(id)

            treeView.SuspendLayout()
            nodes.Clear()
            For Each info As INodeInfo In list
                nodes.Add(New Node(info))
            Next
            treeView.ResumeLayout()
        Catch ex As Exception
            HandleError(ex)
        End Try
    End Function

    ''' <summary>
    ''' Handles node expansion event.
    ''' </summary>
    Private Async Sub treeView_AfterExpand(sender As Object, e As TreeViewEventArgs)
        Dim node As Node = DirectCast(e.Node, Node)
        If Not node.IsLoaded Then
            Try
                Await LoadNodesAsync(node.Info.Id, node.Nodes)
                node.IsLoaded = True
            Catch ex As Exception
                HandleError(ex)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Handles node selection event.
    ''' </summary>
    Private Sub treeView_AfterSelect(sender As Object, e As TreeViewEventArgs)
        Dim node As Node = TryCast(treeView.SelectedNode, Node)
        If node Is Nothing Then
            Return
        End If

        If SelectedNode Is node Then
            Return
        End If

        SelectedNode = node

        RaiseEvent SelectionChanged(Me, EventArgs.Empty)
    End Sub

    ''' <summary>
    ''' Handles errors by raising the <see cref="ErrorOccured"/> event.
    ''' Throws exception if not handled by event.
    ''' </summary>
    Private Sub HandleError(ex As Exception)
        Dim args As New ErrorOccuredEventArgs(ex)

        RaiseEvent ErrorOccured(Me, args)

        If args.Handled Then Return

        Throw New Exception("An unhandled exception occurred. --> " & ex.Message, ex)
    End Sub

    ''' <summary>
    ''' A node of the the <see cref="UcTreeView"/>.
    ''' </summary>
    Protected Class Node
        Inherits TreeNode

        Public Property Info() As INodeInfo
            Get
                Return m_Info
            End Get
            Private Set(value As INodeInfo)
                m_Info = value
            End Set
        End Property
        Private m_Info As INodeInfo

        Public Property IsLoaded() As Boolean
            Get
                Return m_IsLoaded
            End Get
            Set(value As Boolean)
                m_IsLoaded = value
            End Set
        End Property
        Private m_IsLoaded As Boolean

        Public Sub New(nodeInfo As INodeInfo)
            MyBase.New(nodeInfo.Name)

            Info = nodeInfo
            Info.Owner = Me

            If nodeInfo.HasChildNodes = False Then
                IsLoaded = True
            Else
                Nodes.Add(New TreeNode("Loading..."))
            End If
        End Sub
    End Class

    ''' <summary>
    ''' Defines interface for <see cref="UcTreeView"/>'s items.
    ''' </summary>
    Protected Interface INodeInfo
        ReadOnly Property Id() As Object
        ReadOnly Property Name() As String
        ReadOnly Property ItemsTotal() As System.Nullable(Of Integer)
        ReadOnly Property HasChildNodes() As System.Nullable(Of Boolean)
        Property Owner() As Node
    End Interface

    ''' <summary>
    ''' Represents an <see cref="UcTreeView"/>'s item.
    ''' </summary>
    Public Class NodeInfo(Of TId)
        Implements INodeInfo

        Private m_Owner As Node
        Private m_HasChildNodes As System.Nullable(Of Boolean)

        ''' <summary>
        ''' Gets the ID of the info.
        ''' </summary>
        Public Property Id() As TId
            Get
                Return m_Id
            End Get
            Private Set(value As TId)
                m_Id = value
            End Set
        End Property
        Private m_Id As TId

        ''' <summary>
        ''' Gets the Name of the info.
        ''' </summary>
        Public ReadOnly Property Name() As String Implements UcTreeView.INodeInfo.Name
            Get
                Return m_Name
            End Get
        End Property
        Private m_Name As String

        ''' <summary>
        ''' Gets the total items count within the info.
        ''' </summary>
        Public ReadOnly Property ItemsTotal() As System.Nullable(Of Integer) Implements UcTreeView.INodeInfo.ItemsTotal
            Get
                Return m_ItemsTotal
            End Get
        End Property
        Private m_ItemsTotal As System.Nullable(Of Integer)

        ''' <summary>
        ''' Gets the Path of the info.
        ''' </summary>
        Public ReadOnly Property Path() As String
            Get
                Return If(m_Owner Is Nothing, Name, m_Owner.FullPath)
            End Get
        End Property

        ''' <summary>
        ''' Initializes new instance of the <see cref="NodeInfo{TId}"/>.
        ''' </summary>
        Public Sub New(id As TId, name As String, itemsTotal As System.Nullable(Of Integer), hasChildNodes As System.Nullable(Of Boolean))
            m_Id = id
            m_Name = name
            m_ItemsTotal = ItemsTotal
            m_HasChildNodes = hasChildNodes
        End Sub

        Private ReadOnly Property INodeInfo_Id() As Object Implements INodeInfo.Id
            Get
                Return Me.Id
            End Get
        End Property

        Private ReadOnly Property INodeInfo_HasChildNodes() As System.Nullable(Of Boolean) Implements INodeInfo.HasChildNodes
            Get
                Return m_HasChildNodes
            End Get
        End Property

        Private Property INodeInfo_Owner() As Node Implements INodeInfo.Owner
            Get
                Return m_Owner
            End Get
            Set(value As Node)
                m_Owner = value
            End Set
        End Property
    End Class
End Class

