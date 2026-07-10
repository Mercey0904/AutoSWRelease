Imports System.Net
Imports System.Net.Http
Imports System.Net.Mail
Imports System.Text
Imports Newtonsoft.Json

Public Class NotificationService
    Private ReadOnly config As AppConfig
    Private ReadOnly logger As IActivityLogger

    Public Sub New(appConfig As AppConfig, activityLogger As IActivityLogger)
        config = appConfig
        logger = activityLogger
    End Sub

    Public Async Function SendReleaseSuccessAsync(version As String, changelog As String) As Task
        Await Task.Run(Sub()
                           Dim subject = $"New Release: v{version}"
                           Dim body = $"New release {version} is available!{vbCrLf}{vbCrLf}Changelog:{vbCrLf}{changelog}"

                           SendEmail(subject, body, config.GetEmailRecipientsList())
                           SendWebhookNotification(version, changelog)
                       End Sub)
    End Function

    Public Sub SendValidationFailedEmail(version As String, errors As List(Of String))
        Dim subject = "Release Failed - Version Validation Errors"
        Dim body = BuildValidationErrorBody(version, errors)
        SendEmail(subject, body, config.GetEmailRecipientsList())
    End Sub

    Public Sub SendReleaseFailedEmail(errorMessage As String)
        SendEmail("Release Failed - Error",
                 $"An error occurred during the release process: {errorMessage}",
                 config.GetEmailRecipientsList())
    End Sub

    Public Sub SendNoNewCommitsEmail()
        SendEmail("No commits to release", "Everything is updated", config.GetEmailRecipientsList())
    End Sub

    Public Sub SendTestEmail()
        Try
            Using client As New SmtpClient(config.SmtpServer, config.SmtpPort)
                client.EnableSsl = True
                client.Credentials = New NetworkCredential(config.SmtpUsername, config.SmtpPassword)

                Using message As New MailMessage()
                    message.From = New MailAddress(config.SmtpUsername)
                    message.To.Add(config.SmtpUsername)
                    message.Subject = "Test Email from Auto Deployer"
                    message.Body = "This is a test email from your auto deployment system."

                    client.Send(message)
                    logger.Log("Test email sent successfully")
                End Using
            End Using
        Catch ex As Exception
            logger.Log($"Test email failed: {ex.Message}")
            Throw
        End Try
    End Sub

    Private Sub SendEmail(subject As String, body As String, recipients As List(Of String))
        Try
            Using smtp As New SmtpClient(config.SmtpServer, config.SmtpPort)
                smtp.Credentials = New NetworkCredential(config.SmtpUsername, config.SmtpPassword)
                smtp.EnableSsl = True

                Using mail As New MailMessage()
                    mail.From = New MailAddress(config.SmtpUsername)
                    mail.Subject = subject
                    mail.Body = body

                    For Each recipient In recipients
                        If Not String.IsNullOrWhiteSpace(recipient) Then
                            mail.To.Add(recipient.Trim())
                        End If
                    Next

                    If mail.To.Count > 0 Then
                        smtp.Send(mail)
                        logger.Log($"Email sent: {subject}")
                    Else
                        logger.Log("No email recipients configured")
                    End If
                End Using
            End Using
        Catch ex As Exception
            logger.Log($"Error sending email: {ex.Message}")
        End Try
    End Sub

    Private Sub SendWebhookNotification(version As String, changelog As String)
        Dim payload As New Dictionary(Of String, Object) From {
            {"event", "new_release"},
            {"version", version},
            {"changelog", changelog},
            {"timestamp", DateTime.Now.ToString("o")}
        }

        SendWebhooks(payload)
    End Sub

    Private Sub SendWebhooks(payload As Dictionary(Of String, Object))
        For Each webhookUrl In config.GetWebhookUrlsList()
            If String.IsNullOrWhiteSpace(webhookUrl) Then Continue For

            Task.Run(Sub()
                         Try
                             Using client As New HttpClient()
                                 Dim json = JsonConvert.SerializeObject(payload)
                                 Dim content As New StringContent(json, Encoding.UTF8, "application/json")

                                 Dim response = client.PostAsync(webhookUrl.Trim(), content).Result

                                 If response.IsSuccessStatusCode Then
                                     logger.Log($"Webhook sent successfully to {webhookUrl}")
                                 Else
                                     logger.Log($"Webhook failed to {webhookUrl}: {response.StatusCode}")
                                 End If
                             End Using
                         Catch ex As Exception
                             logger.Log($"Webhook error to {webhookUrl}: {ex.Message}")
                         End Try
                     End Sub)
        Next
    End Sub

    Private Function BuildValidationErrorBody(version As String, errors As List(Of String)) As String
        Dim body As New StringBuilder()
        body.AppendLine($"Version validation failed for: {version}")
        body.AppendLine()
        body.AppendLine("Errors found:")
        For Each [error] In errors
            body.AppendLine($"  • {[error]}")
        Next
        Return body.ToString()
    End Function

    Public Sub SendTestWebhook()
        Try
            Dim testPayload As New Dictionary(Of String, Object) From {
                {"event", "test"},
                {"message", "Test webhook from Release Automation System"},
                {"timestamp", DateTime.Now.ToString("o")}
            }

            For Each webhookUrl In config.GetWebhookUrlsList()
                If String.IsNullOrWhiteSpace(webhookUrl) Then Continue For

                Using client As New HttpClient()
                    Dim json = JsonConvert.SerializeObject(testPayload)
                    Dim content As New StringContent(json, Encoding.UTF8, "application/json")

                    Dim response = client.PostAsync(webhookUrl.Trim(), content).Result

                    If response.IsSuccessStatusCode Then
                        logger.Log($"Test webhook sent successfully to {webhookUrl}")
                    Else
                        logger.Log($"Test webhook failed to {webhookUrl}: {response.StatusCode}")
                    End If
                End Using
            Next

        Catch ex As Exception
            logger.Log($"Test webhook failed: {ex.Message}")
            Throw
        End Try
    End Sub
End Class
