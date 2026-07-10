Imports Newtonsoft.Json.Linq

Public Class FormMain
    Private config As AppConfig
    Private activityLog As New List(Of String)
    Private webhookListener As WebhookListener
    Private releaseProcessor As ReleaseProcessor

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadConfig()
        InitializeServices()
        StartWebhookListener()
        UpdateDashboard()
    End Sub

    Private Sub InitializeServices()
        releaseProcessor = New ReleaseProcessor(config, New FormLogger(Me))
        AddHandler releaseProcessor.ReleaseCompleted, AddressOf OnReleaseCompleted
        AddHandler releaseProcessor.ReleaseFailed, AddressOf OnReleaseFailed
    End Sub

    ' Clean button handlers
    Private Sub BtCheckNewR_Click(sender As Object, e As EventArgs) Handles BtCheckNewR.Click
        LogActivity("Manually checking for new release...")
        Task.Run(Async Function()
                     Await releaseProcessor.ProcessReleaseAsync()
                 End Function)
    End Sub

    Private Sub BtSendEmail_Click(sender As Object, e As EventArgs) Handles BtSendEmail.Click
        Task.Run(Sub()
                     Try
                         Dim notificationService = New NotificationService(config, New FormLogger(Me))
                         notificationService.SendTestEmail()
                         LogActivity("Test email sent successfully")
                         Invoke(Sub() MessageBox.Show("Test email sent!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information))
                     Catch ex As Exception
                         LogActivity($"Test email failed: {ex.Message}")
                         Invoke(Sub() MessageBox.Show($"Test email failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error))
                     End Try
                 End Sub)
    End Sub

    Private Sub BtTestWh_Click(sender As Object, e As EventArgs) Handles BtTestWh.Click
        Task.Run(Sub()
                     Try
                         Dim notificationService = New NotificationService(config, New FormLogger(Me))
                         notificationService.SendTestWebhook()
                         LogActivity("Test webhook sent successfully")
                         Invoke(Sub() MessageBox.Show("Test webhook sent!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information))
                     Catch ex As Exception
                         LogActivity($"Test webhook failed: {ex.Message}")
                         Invoke(Sub() MessageBox.Show($"Webhook failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error))
                     End Try
                 End Sub)
    End Sub

    Private Sub BtClearRelease_Click(sender As Object, e As EventArgs) Handles BtClearRelease.Click
        If MessageBox.Show("Clear all release history?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            releaseProcessor.ClearReleaseHistory()
            LogActivity("Release history cleared")
            UpdateDashboard()
            MessageBox.Show("Release history cleared successfully!", "Clear Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub BtSaveSettings_Click(sender As Object, e As EventArgs) Handles BtSaveSettings.Click
        SaveConfig()
    End Sub

    ' Event handlers
    Private Sub OnReleaseCompleted(sender As Object, e As ReleaseResult)
        LogActivity(e.Message)
        Invoke(Sub()
                   UpdateDashboard()
                   If e.Success AndAlso Not String.IsNullOrEmpty(e.Version) Then
                       MessageBox.Show($"Release {e.Version} completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                   Else
                       MessageBox.Show("Release check completed.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                   End If
               End Sub)
    End Sub

    Private Sub OnReleaseFailed(sender As Object, e As ReleaseResult)
        LogActivity(e.Message)
        Invoke(Sub()
                   UpdateDashboard()
                   MessageBox.Show($"Release failed: {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
               End Sub)
    End Sub

    ' Configuration Methods
    Private Sub LoadConfig()
        Try
            config = AppConfig.Load()

            ' Populate form controls from config
            For Each prop In GetType(AppConfig).GetProperties()
                If Not prop.CanRead Then Continue For

                Dim controlName As String = "Tb" & prop.Name
                Dim ctrl = Me.Controls.Find(controlName, True).FirstOrDefault()

                If ctrl IsNot Nothing AndAlso TypeOf ctrl Is TextBox Then
                    Dim value = prop.GetValue(config)
                    If value IsNot Nothing Then
                        DirectCast(ctrl, TextBox).Text = value.ToString()
                    End If
                End If
            Next

            LogActivity("Configuration loaded")

        Catch ex As Exception
            LogActivity($"Error loading configuration: {ex.Message}")
            config = New AppConfig() ' Use default config
        End Try
    End Sub

    Private Sub SaveConfig()
        Try
            ' Save form controls to config
            For Each prop In GetType(AppConfig).GetProperties()
                If Not prop.CanWrite Then Continue For

                Dim controlName As String = "Tb" & prop.Name
                Dim ctrl = Me.Controls.Find(controlName, True).FirstOrDefault()

                If ctrl IsNot Nothing AndAlso TypeOf ctrl Is TextBox Then
                    Dim value As String = DirectCast(ctrl, TextBox).Text

                    If prop.PropertyType = GetType(Integer) Then
                        Dim intValue As Integer
                        If Integer.TryParse(value, intValue) Then
                            prop.SetValue(config, intValue)
                        End If
                    ElseIf prop.PropertyType = GetType(String) Then
                        prop.SetValue(config, value)
                    End If
                End If
            Next

            config.Save()
            LogActivity("Configuration saved successfully")
            MessageBox.Show("Configuration saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            LogActivity($"Error saving configuration: {ex.Message}")
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Webhook Listener Methods
    Private Sub StartWebhookListener()
        Try
            webhookListener = New WebhookListener(config.WebhookListenerPort)
            AddHandler webhookListener.WebhookReceived, AddressOf OnWebhookReceived
            webhookListener.Start()
            LogActivity($"Webhook listener started on port {config.WebhookListenerPort}")
        Catch ex As Exception
            LogActivity($"Failed to start webhook listener: {ex.Message}")
        End Try
    End Sub

    Private Sub OnWebhookReceived(payload As String)
        Try
            Dim json = JObject.Parse(payload)

            Dim changes = json("push")("changes")
            If changes IsNot Nothing Then
                For Each change In changes
                    Dim branchName = change("new")("name").ToString()
                    If branchName.ToLower() = "master" OrElse branchName.ToLower() = "main" Then
                        LogActivity("Master branch updated - triggering release workflow")
                        Invoke(Sub() BtCheckNewR.PerformClick())
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            LogActivity($"Error processing webhook: {ex.Message}")
        End Try
    End Sub

    ' UI Methods
    Private Sub UpdateDashboard()
        If InvokeRequired Then
            Invoke(New Action(AddressOf UpdateDashboard))
            Return
        End If

        ' Update latest released version
        If TbLatestReleased IsNot Nothing Then
            TbLatestReleased.Text = If(String.IsNullOrEmpty(config.LastReleasedVersion), "None", config.LastReleasedVersion)
        End If

        ' Update activity log
        If TbActivityLog IsNot Nothing Then
            TbActivityLog.Lines = activityLog.ToArray()
            TbActivityLog.SelectionStart = TbActivityLog.Text.Length
            TbActivityLog.ScrollToCaret()
        End If
    End Sub

    Public Sub LogActivity(message As String)
        Dim logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}"
        activityLog.Add(logEntry)

        If InvokeRequired Then
            Invoke(Sub() UpdateDashboard())
        Else
            UpdateDashboard()
        End If
    End Sub

    ' Form Events
    Private Sub FormMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Stop webhook listener on close
        If webhookListener IsNot Nothing Then
            webhookListener.Stop()
        End If
    End Sub

    Private Sub BtClearAll_Click(sender As Object, e As EventArgs) Handles BtClearAll.Click
        Try
            ' Confirm with user since this is destructive
            Dim result = MessageBox.Show(
            "This will clear ALL configuration settings and release history." & vbCrLf & vbCrLf &
            "This includes:" & vbCrLf &
            "• All user input fields" & vbCrLf &
            "• Release history and version tracking" & vbCrLf &
            "• Email and webhook settings" & vbCrLf & vbCrLf &
            "Are you sure you want to continue?",
            "Clear All Settings and History",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2)

            If result = DialogResult.Yes Then
                ClearAllSettings()
                releaseProcessor.ClearReleaseHistory()
                LogActivity("✓ ALL settings and release history cleared")

                MessageBox.Show("All settings and release history have been cleared!" & vbCrLf &
                          "The application has been reset to default state.",
                          "Clear Complete",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Information)
            Else
                LogActivity("Clear all operation cancelled")
            End If

        Catch ex As Exception
            LogActivity($"Error clearing all settings: {ex.Message}")
            MessageBox.Show($"Error clearing settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ClearAllSettings()
        ' Create a new default configuration
        config = New AppConfig()

        ' Clear all text boxes
        For Each prop In GetType(AppConfig).GetProperties()
            If Not prop.CanRead Then Continue For

            Dim controlName As String = "Tb" & prop.Name
            Dim ctrl = Me.Controls.Find(controlName, True).FirstOrDefault()

            If ctrl IsNot Nothing AndAlso TypeOf ctrl Is TextBox Then
                DirectCast(ctrl, TextBox).Clear()
            End If
        Next

        ' Save the cleared configuration
        SaveConfig()

        ' Reinitialize services with cleared config
        InitializeServices()

        LogActivity("All user settings cleared")
    End Sub
End Class

' Logger that forwards to form
Public Class FormLogger
    Implements IActivityLogger
    Private ReadOnly form As FormMain

    Public Sub New(mainForm As FormMain)
        form = mainForm
    End Sub

    Public Sub Log(message As String) Implements IActivityLogger.Log
        form.LogActivity(message)
    End Sub
End Class

'provide consistant logging across interface
Public Interface IActivityLogger
    Sub Log(message As String)
End Interface