Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json.Linq

' Helper class to hold repository information
Public Class RepoInfo
    Public Property Workspace As String
    Public Property RepoSlug As String
End Class

'all bitbucket related pull commit
Public Class BitbucketService
    Private ReadOnly config As AppConfig
    Private ReadOnly logger As IActivityLogger

    Public Sub New(appConfig As AppConfig, activityLogger As IActivityLogger)
        config = appConfig
        logger = activityLogger
    End Sub

    'get most recent commit
    Public Async Function GetLatestCommitAsync(branch As String) As Task(Of GitCommit)
        Try
            Using client As New HttpClient()
                SetupAuthentication(client)

                Dim repoInfo = ParseRepoUrl()
                Dim workspace = repoInfo.Workspace
                Dim repoSlug = repoInfo.RepoSlug
                Dim apiUrl = $"https://api.bitbucket.org/2.0/repositories/{workspace}/{repoSlug}/commits/{branch}"

                Dim response = Await client.GetAsync(apiUrl)
                response.EnsureSuccessStatusCode()

                Dim content = Await response.Content.ReadAsStringAsync()
                Dim jsonResponse = JObject.Parse(content)

                Dim commits = jsonResponse("values")
                If commits.HasValues Then
                    Return CreateGitCommitFromJson(commits(0))
                End If
            End Using
        Catch ex As Exception
            logger.Log($"Error fetching commits: {ex.Message}")
        End Try

        Return Nothing
    End Function

    'get commit history between two points
    Public Async Function GetCommitsBetweenAsync(fromCommitHash As String, toCommitHash As String) As Task(Of List(Of GitCommit))
        Dim commitList As New List(Of GitCommit)()

        Try
            Using client As New HttpClient()
                SetupAuthentication(client)

                Dim repoInfo = ParseRepoUrl()
                Dim workspace = repoInfo.Workspace
                Dim repoSlug = repoInfo.RepoSlug
                Dim apiUrl = $"https://api.bitbucket.org/2.0/repositories/{workspace}/{repoSlug}/commits/{toCommitHash}?exclude={fromCommitHash}"

                Dim response = Await client.GetAsync(apiUrl)
                response.EnsureSuccessStatusCode()

                Dim content = Await response.Content.ReadAsStringAsync()
                Dim jsonResponse = JObject.Parse(content)

                Dim commits = jsonResponse("values")
                If commits IsNot Nothing Then
                    For Each commit In commits
                        commitList.Add(CreateGitCommitFromJson(commit))
                    Next
                End If
            End Using
        Catch ex As Exception
            logger.Log($"Error fetching commits between {fromCommitHash} and {toCommitHash}: {ex.Message}")
        End Try

        Return commitList
    End Function

    Public Async Function GetRecentCommitsChangelogAsync(count As Integer) As Task(Of String)
        Try
            Using client As New HttpClient()
                SetupAuthentication(client)

                Dim repoInfo = ParseRepoUrl()
                Dim workspace = repoInfo.Workspace
                Dim repoSlug = repoInfo.RepoSlug
                Dim apiUrl = $"https://api.bitbucket.org/2.0/repositories/{workspace}/{repoSlug}/commits?pagelen={count}"

                Dim response = Await client.GetAsync(apiUrl)
                If response.IsSuccessStatusCode Then
                    Dim content = Await response.Content.ReadAsStringAsync()
                    Dim jsonResponse = JObject.Parse(content)

                    Dim changelog As New StringBuilder()
                    Dim commits = jsonResponse("values")

                    If commits IsNot Nothing AndAlso commits.Count > 0 Then
                        changelog.AppendLine($"Recent changes ({commits.Count} commits):")
                        changelog.AppendLine()

                        For Each commit In commits
                            Dim message = commit("message").ToString().Split(vbLf)(0)
                            Dim author = "Unknown"
                            Dim dateStr = ""

                            If commit("author") IsNot Nothing Then
                                author = commit("author")("raw").ToString()
                            End If

                            If commit("date") IsNot Nothing Then
                                dateStr = $" - {DateTime.Parse(commit("date").ToString()):MMM dd}"
                            End If

                            changelog.AppendLine($"- {message}{dateStr} by {author}")
                        Next

                        Return changelog.ToString()
                    Else
                        Return "Initial release - no commit history available"
                    End If
                Else
                    Return "Initial release - could not fetch commit history"
                End If
            End Using
        Catch ex As Exception
            logger.Log($"Error generating changelog: {ex.Message}")
            Return "Initial release"
        End Try
    End Function

    Public Async Function GetFileContentAsync(filePath As String) As Task(Of String)
        Try
            Using client As New HttpClient()
                SetupAuthentication(client)

                Dim repoInfo = ParseRepoUrl()
                Dim workspace = repoInfo.Workspace
                Dim repoSlug = repoInfo.RepoSlug
                Dim apiUrl = $"https://api.bitbucket.org/2.0/repositories/{workspace}/{repoSlug}/src/master/{filePath}"

                Dim response = Await client.GetAsync(apiUrl)
                If response.IsSuccessStatusCode Then
                    Return Await response.Content.ReadAsStringAsync()
                End If
            End Using
        Catch ex As Exception
            logger.Log($"Error fetching file from Bitbucket: {ex.Message}")
        End Try

        Return Nothing
    End Function

    Private Sub SetupAuthentication(client As HttpClient)
        Dim credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{config.Username}:{config.ApiToken}"))
        client.DefaultRequestHeaders.Authorization = New Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials)
    End Sub

    Private Function ParseRepoUrl() As RepoInfo
        Try
            Dim uri As New Uri(config.RepoUrl)
            Dim pathParts = uri.AbsolutePath.Trim("/"c).Split("/"c)

            If pathParts.Length >= 2 Then
                Return New RepoInfo With {
                    .Workspace = pathParts(0),
                    .RepoSlug = pathParts(1).Replace(".git", "")
                }
            Else
                Throw New Exception($"Invalid repository URL format: {config.RepoUrl}")
            End If
        Catch ex As Exception
            logger.Log($"Error parsing repository URL: {ex.Message}")
            Throw
        End Try
    End Function

    Private Function CreateGitCommitFromJson(commit As JToken) As GitCommit
        Try
            Dim commitHash = commit("hash").ToString()
            Dim commitMessage = commit("message").ToString().Split(vbLf)(0)
            Dim authorName = "Unknown"
            Dim commitDate As DateTime = DateTime.Now

            If commit("author") IsNot Nothing Then
                authorName = commit("author")("raw").ToString()
            End If

            If commit("date") IsNot Nothing Then
                DateTime.TryParse(commit("date").ToString(), commitDate)
            End If

            logger.Log($"Latest commit found: {commitHash.Substring(0, 7)}")
            logger.Log($"  Author: {authorName}")
            logger.Log($"  Date: {commitDate}")
            logger.Log($"  Message: {commitMessage}")

            Return New GitCommit(commitHash, commitMessage, authorName, commitDate)
        Catch ex As Exception
            logger.Log($"Error parsing commit JSON: {ex.Message}")
            Return Nothing
        End Try
    End Function
End Class

