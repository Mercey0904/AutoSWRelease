Public Class GitCommit
    Public Property Hash As String
    Public Property Message As String
    Public Property Author As String
    Public Property [Date] As DateTime

    Public ReadOnly Property ShortHash As String
        Get
            Return If(Hash?.Length >= 7, Hash.Substring(0, 7), Hash)
        End Get
    End Property

    Public Sub New()
    End Sub

    Public Sub New(hash As String, message As String, author As String, commitDate As DateTime)
        Me.Hash = hash
        Me.Message = message
        Me.Author = author
        Me.Date = commitDate
    End Sub
End Class