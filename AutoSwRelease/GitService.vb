Imports System.IO

Public Class GitService
    Private ReadOnly config As AppConfig
    Private ReadOnly logger As IActivityLogger

    Public Sub New(appConfig As AppConfig, activityLogger As IActivityLogger)
        config = appConfig
        logger = activityLogger
    End Sub

    Public Function GetRepositoryPath() As String
        Dim repoName = Path.GetFileName(config.RepoUrl).Replace(".git", "")
        Dim repoPath = Path.Combine(config.WorkingDirectory, repoName)

        If Not Path.IsPathRooted(repoPath) Then
            repoPath = Path.GetFullPath(repoPath)
        End If

        Return repoPath
    End Function

    Public Async Function EnsureLatestCodeAsync(commitHash As String) As Task
        Await Task.Run(Sub() EnsureLatestCode(commitHash))
    End Function

    Private Sub EnsureLatestCode(commitHash As String)
        Dim repoPath = GetRepositoryPath()

        If Not IsGitInstalled() Then
            Throw New Exception("Git is not installed or not in PATH.")
        End If

        If Directory.Exists(repoPath) Then
            If Not IsGitRepository(repoPath) Then
                logger.Log($"Deleting non-Git folder: {repoPath}")
                Directory.Delete(repoPath, True)
                CloneRepository(repoPath)
            Else
                PullRepository(repoPath, commitHash)
            End If
        Else
            CloneRepository(repoPath)
        End If

        logger.Log($"✓ Code updated to commit: {commitHash.Substring(0, 7)}")
    End Sub

    Private Sub PullRepository(repoPath As String, commitHash As String)
        If Not IsGitRepository(repoPath) Then
            Throw New Exception($"Folder '{repoPath}' is not a Git repository")
        End If

        RunGitCommand(repoPath, "fetch origin")

        If Not String.IsNullOrEmpty(commitHash) Then
            RunGitCommand(repoPath, $"reset --hard {commitHash}")
        Else
            RunGitCommand(repoPath, "reset --hard origin/master")
        End If

        RunGitCommand(repoPath, "clean -fd")
        logger.Log("Repository updated successfully")
    End Sub

    Private Sub CloneRepository(repoPath As String)
        Dim parentDir = Path.GetDirectoryName(repoPath)
        If Not Directory.Exists(parentDir) Then
            Directory.CreateDirectory(parentDir)
        End If

        RunGitCommand(parentDir, $"clone {config.RepoUrl} {Path.GetFileName(repoPath)}")
        logger.Log("✓ Repository cloned successfully")
    End Sub

    Private Function RunGitCommand(workingDirectory As String, arguments As String) As String
        Dim psi As New ProcessStartInfo() With {
            .FileName = "git",
            .Arguments = arguments,
            .UseShellExecute = False,
            .RedirectStandardOutput = True,
            .RedirectStandardError = True,
            .CreateNoWindow = True,
            .WorkingDirectory = workingDirectory
        }

        Using proc As Process = Process.Start(psi)
            Dim output = proc.StandardOutput.ReadToEnd()
            Dim errorOutput = proc.StandardError.ReadToEnd()
            proc.WaitForExit()

            If proc.ExitCode <> 0 Then
                Throw New Exception($"Git command '{arguments}' failed: {errorOutput}")
            End If

            Return output
        End Using
    End Function

    Private Function IsGitRepository(repoPath As String) As Boolean
        Return Directory.Exists(repoPath) AndAlso Directory.Exists(Path.Combine(repoPath, ".git"))
    End Function

    Private Function IsGitInstalled() As Boolean
        Try
            Dim psi As New ProcessStartInfo() With {
                .FileName = "git",
                .Arguments = "--version",
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True
            }

            Using proc As Process = Process.Start(psi)
                proc.WaitForExit()
                Return proc.ExitCode = 0
            End Using
        Catch
            Return False
        End Try
    End Function
End Class