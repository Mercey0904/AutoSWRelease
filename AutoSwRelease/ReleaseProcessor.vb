Imports System.Text

Public Class ReleaseProcessor
    Private ReadOnly config As AppConfig
    Private ReadOnly logger As IActivityLogger
    Private ReadOnly bitbucketService As BitbucketService
    Private ReadOnly gitService As GitService
    Private ReadOnly buildService As BuildService
    Private ReadOnly notificationService As NotificationService

    Public Event ReleaseCompleted As EventHandler(Of ReleaseResult)
    Public Event ReleaseFailed As EventHandler(Of ReleaseResult)

    Public Sub New(appConfig As AppConfig, activityLogger As IActivityLogger)
        config = appConfig
        logger = activityLogger
        bitbucketService = New BitbucketService(config, logger)
        gitService = New GitService(config, logger)
        buildService = New BuildService(config, logger, gitService)
        notificationService = New NotificationService(config, logger)
    End Sub

    Public Async Function ProcessReleaseAsync() As Task(Of ReleaseResult)
        Dim result As New ReleaseResult()

        Try
            logger.Log("Starting release process...")

            ' Step 1: Check for new commits
            If Not Await HasNewCommitsSinceLastReleaseAsync() Then
                result.Success = True
                result.Message = "No new commits to release"
                logger.Log(result.Message)
                notificationService.SendNoNewCommitsEmail()
                RaiseEvent ReleaseCompleted(Me, result)
                Return result
            End If

            ' Step 2: Get latest commit
            Dim latestCommit = Await bitbucketService.GetLatestCommitAsync("main")
            If latestCommit Is Nothing Then
                result.Success = False
                result.Message = "No commits found on main"
                logger.Log(result.Message)
                RaiseEvent ReleaseFailed(Me, result)
                Return result
            End If

            ' Step 3: Pull latest code
            Await gitService.EnsureLatestCodeAsync(latestCommit.Hash)

            ' Step 4: Extract and validate version
            Dim newVersion = buildService.ExtractVersionFromCode()
            Dim validationResult = buildService.ValidateVersion(newVersion)

            If Not validationResult.IsValid Then
                result.Success = False
                result.Message = $"Version validation failed: {String.Join(", ", validationResult.Errors)}"
                logger.Log(result.Message)
                notificationService.SendValidationFailedEmail(newVersion, validationResult.Errors)
                RaiseEvent ReleaseFailed(Me, result)
                Return result
            End If

            ' Step 5: Build installer
            Await buildService.BuildInstallerAsync(newVersion)

            ' Step 6: Generate changelog
            Dim changelog = Await GenerateChangelogAsync(latestCommit.Hash)

            ' Step 7: Save artifacts
            buildService.SaveChangelogFile(newVersion, changelog)
            buildService.SaveVersionHistory(newVersion)

            ' Step 8: Update configuration
            config.LastReleasedVersion = newVersion
            config.LastReleaseCommitHash = latestCommit.Hash
            config.Save()

            ' Step 9: Send notifications
            Await notificationService.SendReleaseSuccessAsync(newVersion, changelog)

            result.Success = True
            result.Message = $"Release {newVersion} completed successfully!"
            result.Version = newVersion
            logger.Log(result.Message)
            RaiseEvent ReleaseCompleted(Me, result)

        Catch ex As Exception
            result.Success = False
            result.Message = $"Release process failed: {ex.Message}"
            logger.Log(result.Message)
            notificationService.SendReleaseFailedEmail(ex.Message)
            RaiseEvent ReleaseFailed(Me, result)
        End Try

        Return result
    End Function

    Private Async Function HasNewCommitsSinceLastReleaseAsync() As Task(Of Boolean)
        If String.IsNullOrEmpty(config.LastReleasedVersion) Then
            Return True ' No previous release
        End If

        Dim latestCommit = Await bitbucketService.GetLatestCommitAsync("main")
        If latestCommit Is Nothing Then
            Return False
        End If

        If Not String.IsNullOrEmpty(config.LastReleaseCommitHash) Then
            Return latestCommit.Hash <> config.LastReleaseCommitHash
        End If

        Return True
    End Function

    Private Async Function GenerateChangelogAsync(currentCommitHash As String) As Task(Of String)
        Try
            If String.IsNullOrEmpty(config.LastReleaseCommitHash) Then
                ' First release - get recent commits with better error handling
                Return Await GetRecentCommitsWithFallback()
            Else
                Dim commits = Await bitbucketService.GetCommitsBetweenAsync(config.LastReleaseCommitHash, currentCommitHash)
                Return BuildChangelogFromCommits(commits)
            End If
        Catch ex As Exception
            logger.Log($"Error generating changelog: {ex.Message}")
            Return "Initial release - changelog generation failed"
        End Try
    End Function

    Private Async Function GetRecentCommitsWithFallback() As Task(Of String)
        Try
            ' Try to get recent commits
            Dim changelog = Await bitbucketService.GetRecentCommitsChangelogAsync(10)
            If changelog <> "Unable to generate changelog" Then
                Return changelog
            End If

            ' Fallback for first release
            Return "Initial release - see commit history for details"
        Catch ex As Exception
            Return "Initial release"
        End Try
    End Function

    Private Function BuildChangelogFromCommits(commits As List(Of GitCommit)) As String
        If commits.Count = 0 Then
            Return "No changes recorded"
        End If

        Dim changelog As New StringBuilder()
        changelog.AppendLine($"Total commits: {commits.Count}")
        changelog.AppendLine()

        For Each commit In commits
            Dim parsedDate As DateTime
            If DateTime.TryParse(commit.Date.ToString(), parsedDate) Then
                changelog.AppendLine($"- {commit.Message} ({parsedDate:MMM dd}) by {commit.Author}")
            Else
                changelog.AppendLine($"- {commit.Message} by {commit.Author}")
            End If
        Next

        Return changelog.ToString()
    End Function

    Public Sub ClearReleaseHistory()
        config.LastReleasedVersion = ""
        config.LastReleaseCommitHash = ""
        config.Save()
        buildService.ClearVersionHistory()
        logger.Log("Release history cleared")
    End Sub
End Class

Public Class ReleaseResult
    Public Property Success As Boolean
    Public Property Message As String
    Public Property Version As String
End Class