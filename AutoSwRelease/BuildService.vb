Imports System.IO
Imports System.Text

Public Class BuildService
    Private ReadOnly config As AppConfig
    Private ReadOnly logger As IActivityLogger
    Private ReadOnly gitService As GitService
    Private ReadOnly bitbucketService As BitbucketService

    Public Sub New(appConfig As AppConfig, activityLogger As IActivityLogger, gitSvc As GitService)
        config = appConfig
        logger = activityLogger
        gitService = gitSvc
        bitbucketService = New BitbucketService(config, logger)
    End Sub

    Public Function ExtractVersionFromCode() As String
        Dim repoPath = gitService.GetRepositoryPath()

        ' Recursive search for version info
        Dim version = FindVersionRecursively(repoPath)
        If Not String.IsNullOrEmpty(version) Then
            Return version
        End If

        ' Bitbucket fallback
        version = FindVersionFromBitbucket()
        If Not String.IsNullOrEmpty(version) Then
            Return version
        End If

        logger.Log("Version 0.0.0.0 used as fallback")
        Return "0.0.0.0"
    End Function

    Public Function ValidateVersion(version As String) As ValidationResult
        Dim errors As New List(Of String)

        ' Version format check
        Dim versionPattern As New Text.RegularExpressions.Regex("^\d+\.\d+\.\d+(\.\d+)?$")
        If Not versionPattern.IsMatch(version) Then
            errors.Add($"Invalid version format: {version}")
        End If

        ' Duplicate check
        If version = config.LastReleasedVersion Then
            errors.Add($"Duplicate version: {version}")
        End If

        ' Installer exists check - use dynamic project name
        Dim projectName = GetProjectName()
        Dim installerPath = Path.Combine(config.InstallerOutputDirectory, $"{projectName}_v{version}.exe")
        If File.Exists(installerPath) Then
            errors.Add($"Installer already exists: {installerPath}")
        End If

        ' Version increase check
        If Not String.IsNullOrEmpty(config.LastReleasedVersion) Then
            If Not IsNewerVersion(version, config.LastReleasedVersion) Then
                errors.Add($"Version {version} is not newer than {config.LastReleasedVersion}")
            End If
        End If

        Return New ValidationResult With {
            .IsValid = errors.Count = 0,
            .Errors = errors
        }
    End Function

    Public Async Function BuildInstallerAsync(version As String) As Task
        Await Task.Run(Sub() BuildInstaller(version))
    End Function

    Private Sub BuildInstaller(version As String)
        Dim repoPath = gitService.GetRepositoryPath()
        Dim solutionPath = FindSolutionFile(repoPath)
        Dim msbuildPath = FindMSBuild()

        BuildProject(msbuildPath, solutionPath)
        CopyBuiltExecutable(repoPath, version)
    End Sub

    Public Sub SaveChangelogFile(version As String, changelog As String)
        Try
            Dim projectName = GetProjectName()
            EnsureDirectoryExists(config.InstallerOutputDirectory)

            Dim changelogPath = Path.Combine(config.InstallerOutputDirectory, $"CHANGELOG_v{version}.txt")
            Dim content = BuildChangelogContent(projectName, version, changelog)

            File.WriteAllText(changelogPath, content)
            logger.Log($"✓ Changelog saved: CHANGELOG_v{version}.txt")
        Catch ex As Exception
            logger.Log($"Warning: Could not save changelog file - {ex.Message}")
        End Try
    End Sub

    Public Sub SaveVersionHistory(version As String)
        Try
            Dim historyFile = Path.Combine(config.InstallerOutputDirectory, "version_history.txt")
            File.AppendAllText(historyFile, $"{version}|{DateTime.Now:yyyy-MM-dd HH:mm:ss}{Environment.NewLine}")
            logger.Log($"Version {version} added to history")
        Catch ex As Exception
            logger.Log($"Warning: Could not save version history - {ex.Message}")
        End Try
    End Sub

    Public Sub ClearVersionHistory()
        Try
            Dim historyFile = Path.Combine(config.InstallerOutputDirectory, "version_history.txt")
            If File.Exists(historyFile) Then
                File.Delete(historyFile)
                logger.Log("Version history file deleted")
            End If
        Catch ex As Exception
            logger.Log($"Note: Could not delete version history file - {ex.Message}")
        End Try
    End Sub

    Private Function GetProjectName() As String
        Try
            Dim repoPath = gitService.GetRepositoryPath()

            ' Method 1: Get from solution file
            Dim solutionFile = FindSolutionFile(repoPath)
            If solutionFile IsNot Nothing Then
                Dim solutionName = Path.GetFileNameWithoutExtension(solutionFile)
                If Not String.IsNullOrEmpty(solutionName) Then
                    logger.Log($"Using project name from solution: {solutionName}")
                    Return solutionName
                End If
            End If

            ' Method 2: Get from repository name
            Dim repoName = Path.GetFileNameWithoutExtension(config.RepoUrl)
            If Not String.IsNullOrEmpty(repoName) Then
                logger.Log($"Using project name from repository: {repoName}")
                Return repoName
            End If

            ' Method 3: Fallback
            logger.Log("Using default project name: Application")
            Return "Application"

        Catch ex As Exception
            logger.Log($"Error getting project name: {ex.Message}")
            Return "Application"
        End Try
    End Function

    Private Function FindSolutionFile(repoPath As String) As String
        Try
            ' First, try to find any .sln file in the repository
            Dim solutionFiles = Directory.GetFiles(repoPath, "*.sln", SearchOption.AllDirectories)

            If solutionFiles.Length > 0 Then
                ' Prefer solution files in root directory
                Dim rootSolution = solutionFiles.FirstOrDefault(Function(f) Path.GetDirectoryName(f).Equals(repoPath, StringComparison.OrdinalIgnoreCase))
                If rootSolution IsNot Nothing Then
                    logger.Log($"Found solution file in root: {rootSolution}")
                    Return rootSolution
                End If

                ' Otherwise, take the first solution file found
                logger.Log($"Found solution file: {solutionFiles(0)}")
                Return solutionFiles(0)
            End If

            Throw New FileNotFoundException($"No solution files found in {repoPath}")

        Catch ex As Exception
            logger.Log($"Error finding solution file: {ex.Message}")
            Throw
        End Try
    End Function

    Private Function FindBuiltExecutable(repoPath As String) As String
        Try
            ' Search for any .exe files in Release folders
            Dim releaseFolders = Directory.GetDirectories(repoPath, "*", SearchOption.AllDirectories).Where(Function(d) d.ToLower().Contains("\bin\release\") OrElse d.ToLower().EndsWith("\bin\release")).ToList()

            logger.Log($"Searching {releaseFolders.Count} Release folders for executables")

            For Each releaseFolder In releaseFolders
                Dim exeFiles = Directory.GetFiles(releaseFolder, "*.exe")
                For Each exeFile In exeFiles
                    ' Skip test executables and other non-main executables
                    If Not IsTestOrUtilityExecutable(exeFile) Then
                        logger.Log($"Found main executable: {exeFile}")
                        Return exeFile
                    Else
                        logger.Log($"Skipping test/utility executable: {exeFile}")
                    End If
                Next
            Next

            ' Fallback: search all exe files and try to identify the main one
            Dim allExeFiles = Directory.GetFiles(repoPath, "*.exe", SearchOption.AllDirectories)
            For Each exeFile In allExeFiles
                If Not IsTestOrUtilityExecutable(exeFile) AndAlso File.Exists(exeFile) Then
                    Dim fileInfo = New FileInfo(exeFile)
                    If fileInfo.Length > 10240 Then ' Skip very small files (likely not main app)
                        logger.Log($"Found potential main executable: {exeFile}")
                        Return exeFile
                    End If
                End If
            Next

            Throw New FileNotFoundException($"No suitable main executable found in {repoPath}")

        Catch ex As Exception
            logger.Log($"Error finding built executable: {ex.Message}")
            Throw
        End Try
    End Function

    ' Helper to identify test/utility executables, prevent picking up wrong .exe file
    Private Function IsTestOrUtilityExecutable(filePath As String) As Boolean
        Dim fileName = Path.GetFileNameWithoutExtension(filePath).ToLower()
        Dim excludedKeywords As String() = {"test", "vstest", "nunit", "xunit", "mstest", "mock", "stub", "fake", "setup", "install", "uninstall"}

        Return excludedKeywords.Any(Function(keyword) fileName.Contains(keyword))
    End Function


    Private Sub CopyBuiltExecutable(repoPath As String, version As String)
        EnsureDirectoryExists(config.InstallerOutputDirectory)

        Dim builtExePath = FindBuiltExecutable(repoPath)
        Dim projectName = GetProjectName()
        Dim installerFileName = $"{projectName}_v{version}.exe"
        Dim outputPath = Path.Combine(config.InstallerOutputDirectory, installerFileName)

        File.Copy(builtExePath, outputPath, True)
        logger.Log($"✓ Installer created: {installerFileName}")
        logger.Log($"  Source: {builtExePath}")
        logger.Log($"  Destination: {outputPath}")
    End Sub

    ' IMPROVED: Changelog content with dynamic project name
    Private Function BuildChangelogContent(projectName As String, version As String, changelog As String) As String
        Dim content As New StringBuilder()
        content.AppendLine($"{projectName} Version {version}")
        content.AppendLine($"Release Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
        content.AppendLine("=" & New String("="c, 50))
        content.AppendLine()
        content.AppendLine("CHANGES:")
        content.AppendLine(changelog)
        content.AppendLine()
        content.AppendLine("=" & New String("="c, 50))
        content.AppendLine($"Installer: {projectName}_v{version}.exe")
        Return content.ToString()
    End Function

    'all file structure might be different goes to project directory until it finds AssemblyInfo
    Private Function FindVersionRecursively(rootPath As String) As String
        Try
            ' Search for AssemblyInfo files but EXCLUDE .git folders
            Dim assemblyInfoFiles = Directory.GetFiles(rootPath, "*AssemblyInfo*", SearchOption.AllDirectories).Where(Function(f) (f.EndsWith(".vb") OrElse f.EndsWith(".cs")) AndAlso
                              Not f.Contains("\.git\") AndAlso
                              Not f.Contains("\.git/")).ToList()

            logger.Log($"Found {assemblyInfoFiles.Count} AssemblyInfo files to check (excluding .git)")

            For Each filePath In assemblyInfoFiles
                logger.Log($"Checking: {filePath}")
                Dim version = ExtractVersionFromFile(filePath)
                If Not String.IsNullOrEmpty(version) Then
                    logger.Log($"✓ Version {version} found in: {Path.GetFileName(filePath)}")
                    Return version
                End If
            Next

            Return Nothing
        Catch ex As Exception
            logger.Log($"Error during recursive search: {ex.Message}")
            Return Nothing
        End Try
    End Function

    Private Function FindVersionFromBitbucket() As String
        logger.Log("Local search failed. Searching via Bitbucket API...")

        Try
            ' Try multiple possible paths for AssemblyInfo
            Dim filePaths As String() = {
                "My Project/AssemblyInfo.vb",
                "Properties/AssemblyInfo.cs",
                "**/AssemblyInfo.vb",  ' Try to find any AssemblyInfo.vb
                "**/AssemblyInfo.cs"   ' Try to find any AssemblyInfo.cs
            }

            For Each filePath In filePaths
                Try
                    Dim fileContent = bitbucketService.GetFileContentAsync(filePath).Result
                    If Not String.IsNullOrEmpty(fileContent) Then
                        Dim version = ExtractVersionFromContent(fileContent)
                        If Not String.IsNullOrEmpty(version) Then
                            Return version
                        End If
                    End If
                Catch
                    ' Continue to next file path if this one fails
                End Try
            Next
        Catch ex As Exception
            logger.Log($"Error searching Bitbucket: {ex.Message}")
        End Try

        Return Nothing
    End Function

    Private Function ExtractVersionFromFile(filePath As String) As String
        Try
            Dim lines = File.ReadAllLines(filePath)
            Return ExtractVersionFromLines(lines)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function ExtractVersionFromContent(fileContent As String) As String
        Dim lines = fileContent.Split({vbCrLf, vbLf}, StringSplitOptions.None)
        Return ExtractVersionFromLines(lines)
    End Function

    Private Function ExtractVersionFromLines(lines As String()) As String
        For Each line In lines
            Dim versionMatch = Text.RegularExpressions.Regex.Match(line,
                "AssemblyVersion\s*\(\s*""([0-9\.]+)""\s*\)",
                Text.RegularExpressions.RegexOptions.IgnoreCase)

            If versionMatch.Success Then
                Dim version = versionMatch.Groups(1).Value
                logger.Log($"✓ Version {version} extracted")
                Return version
            End If
        Next
        Return Nothing
    End Function

    Private Function FindMSBuild() As String
        Dim msbuildPaths As String() = {
            "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
            "C:\Program Files (x86)\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
            "C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
            "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
        }

        Dim msbuildPath = msbuildPaths.FirstOrDefault(Function(p) File.Exists(p))
        If String.IsNullOrEmpty(msbuildPath) Then
            msbuildPath = "MSBuild.exe"
        End If

        logger.Log($"Using MSBuild at: {msbuildPath}")
        Return msbuildPath
    End Function

    Private Sub BuildProject(msbuildPath As String, solutionPath As String)
        Dim psi As New ProcessStartInfo() With {
            .FileName = msbuildPath,
            .Arguments = $"""{solutionPath}"" /p:Configuration=Release /p:Platform=""Any CPU"" /t:Rebuild",
            .UseShellExecute = False,
            .RedirectStandardOutput = True,
            .RedirectStandardError = True,
            .CreateNoWindow = True
        }

        Using proc As Process = Process.Start(psi)
            Dim output = proc.StandardOutput.ReadToEnd()
            Dim errorOutput = proc.StandardError.ReadToEnd()
            proc.WaitForExit()

            If proc.ExitCode <> 0 Then
                Throw New Exception($"MSBuild failed with exit code {proc.ExitCode}. Errors: {errorOutput}")
            End If

            logger.Log("✓ Build completed successfully")
        End Using
    End Sub

    Private Sub EnsureDirectoryExists(directoryPath As String)
        If Not Directory.Exists(directoryPath) Then
            Directory.CreateDirectory(directoryPath)
        End If
    End Sub

    Private Function IsNewerVersion(newVer As String, oldVer As String) As Boolean
        Try
            Dim newParts = newVer.Split("."c).Select(Function(x) Integer.Parse(x)).ToArray()
            Dim oldParts = oldVer.Split("."c).Select(Function(x) Integer.Parse(x)).ToArray()

            For i As Integer = 0 To Math.Min(newParts.Length, oldParts.Length) - 1
                If newParts(i) > oldParts(i) Then Return True
                If newParts(i) < oldParts(i) Then Return False
            Next

            Return False
        Catch ex As Exception
            Return True
        End Try
    End Function
End Class

Public Class ValidationResult
    Public Property IsValid As Boolean
    Public Property Errors As List(Of String)
End Class