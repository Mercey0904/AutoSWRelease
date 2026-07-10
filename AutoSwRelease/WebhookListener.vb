' Webhook Listener class
Imports System.IO
Imports System.Net
Imports System.Text

Public Class WebhookListener
    Private listener As HttpListener
    Private port As Integer
    Private isRunning As Boolean

    Public Event WebhookReceived(payload As String)

    Public Sub New(listenerPort As Integer)
        port = listenerPort
    End Sub

    Public Sub Start()
        Try
            listener = New HttpListener()
            listener.Prefixes.Add($"http://localhost:{port}/webhook/")
            listener.Start()
            isRunning = True
            listener.BeginGetContext(AddressOf OnContext, Nothing)
        Catch ex As Exception
            Throw New Exception($"Failed to start webhook listener: {ex.Message}")
        End Try
    End Sub

    Private Sub OnContext(result As IAsyncResult)
        If Not isRunning Then Return

        Try
            Dim context = listener.EndGetContext(result)

            ' Continue listening for next request
            If isRunning Then
                listener.BeginGetContext(AddressOf OnContext, Nothing)
            End If

            ' Read request body
            Using reader As New StreamReader(context.Request.InputStream)
                Dim payload = reader.ReadToEnd()
                RaiseEvent WebhookReceived(payload)
            End Using

            ' Send response
            Dim response = Encoding.UTF8.GetBytes("OK")
            context.Response.ContentLength64 = response.Length
            context.Response.StatusCode = 200
            context.Response.OutputStream.Write(response, 0, response.Length)
            context.Response.Close()
        Catch ex As Exception
            ' Handle error silently to keep listener running
        End Try
    End Sub

    Public Sub [Stop]()
        isRunning = False
        If listener IsNot Nothing Then
            Try
                listener.Stop()
                listener.Close()
            Catch ex As Exception
                ' Ignore errors on shutdown
            End Try
        End If
    End Sub
End Class
