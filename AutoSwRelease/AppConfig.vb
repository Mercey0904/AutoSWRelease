Imports System.Reflection

Public Class AppConfig
    Public Property RepoUrl As String = ""
    Public Property Username As String = ""
    Public Property ApiToken As String = ""
    Public Property WorkingDirectory As String = ""
    Public Property InstallerOutputDirectory As String = ""
    Public Property SmtpServer As String = ""
    Public Property SmtpPort As Integer = 587
    Public Property SmtpUsername As String = ""
    Public Property SmtpPassword As String = ""
    Public Property EmailRecipients As String = "" ' Semicolon-separated
    Public Property WebhookUrls As String = "" ' Semicolon-separated
    Public Property LastReleasedVersion As String = ""
    Public Property LastReleaseCommitHash As String = ""
    Public Property WebhookListenerPort As Integer = 8080

    ' Helper methods to convert string to/from List
    Public Function GetEmailRecipientsList() As List(Of String)
        If String.IsNullOrWhiteSpace(EmailRecipients) Then
            Return New List(Of String)()
        End If
        Return EmailRecipients.Split(";"c).Where(Function(s) Not String.IsNullOrWhiteSpace(s)).ToList()
    End Function

    Public Function GetWebhookUrlsList() As List(Of String)
        If String.IsNullOrWhiteSpace(WebhookUrls) Then
            Return New List(Of String)()
        End If
        Return WebhookUrls.Split(";"c).Where(Function(s) Not String.IsNullOrWhiteSpace(s)).ToList()
    End Function

    ' Static load method
    Public Shared Function Load() As AppConfig
        Return ConfigManager.LoadConfig()
    End Function

    ' Instance save method
    Public Sub Save()
        ConfigManager.SaveConfig(Me)
    End Sub
End Class

' Configuration Manager for XML persistence
Public Class ConfigManager
    Private Shared ReadOnly ConfigPath As String =
        IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml")

    Public Shared Function LoadConfig() As AppConfig
        Dim config As New AppConfig()

        If Not IO.File.Exists(ConfigPath) Then
            SaveConfig(config)  ' Create a default file
            Return config
        End If

        Try
            Dim doc As XDocument = XDocument.Load(ConfigPath)
            Dim root = doc.Root

            For Each prop As PropertyInfo In GetType(AppConfig).GetProperties()
                If Not prop.CanWrite Then Continue For

                Dim node = root.Element(prop.Name)
                If node IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(node.Value) Then
                    Try
                        If prop.PropertyType Is GetType(Integer) Then
                            prop.SetValue(config, Integer.Parse(node.Value))
                        ElseIf prop.PropertyType Is GetType(String) Then
                            prop.SetValue(config, node.Value)
                        End If
                    Catch ex As Exception
                        ' Skip invalid values, use default
                    End Try
                End If
            Next
        Catch ex As Exception
            ' If load fails, return default config
        End Try

        Return config
    End Function

    'get value using reflections
    Public Shared Sub SaveConfig(config As AppConfig)
        Try
            Dim root As New XElement("AppConfig")

            For Each prop As PropertyInfo In GetType(AppConfig).GetProperties()
                If Not prop.CanRead Then Continue For

                Dim value = prop.GetValue(config)
                root.Add(New XElement(prop.Name, If(value, "").ToString()))
            Next

            Dim doc As New XDocument(root)
            doc.Save(ConfigPath)
        Catch ex As Exception
            ' Handle save error
            Throw New Exception($"Failed to save configuration: {ex.Message}")
        End Try
    End Sub
End Class
