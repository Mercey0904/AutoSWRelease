<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.LbRepoSet = New System.Windows.Forms.Label()
        Me.LbRepoURL = New System.Windows.Forms.Label()
        Me.LbUserN = New System.Windows.Forms.Label()
        Me.LbWorkingDir = New System.Windows.Forms.Label()
        Me.TbRepoURL = New System.Windows.Forms.TextBox()
        Me.TbUsername = New System.Windows.Forms.TextBox()
        Me.TbWorkingDirectory = New System.Windows.Forms.TextBox()
        Me.LbInstOutputD = New System.Windows.Forms.Label()
        Me.TbInstallerOutputDirectory = New System.Windows.Forms.TextBox()
        Me.TbSmtpPort = New System.Windows.Forms.TextBox()
        Me.TbSmtpServer = New System.Windows.Forms.TextBox()
        Me.LbPort = New System.Windows.Forms.Label()
        Me.LbEmailServer = New System.Windows.Forms.Label()
        Me.BtSaveSettings = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.BtClearAll = New System.Windows.Forms.Button()
        Me.BtClearRelease = New System.Windows.Forms.Button()
        Me.TbApiToken = New System.Windows.Forms.TextBox()
        Me.LbApiToken = New System.Windows.Forms.Label()
        Me.TbWebhookUrls = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TbEmailRecipients = New System.Windows.Forms.TextBox()
        Me.LbEmailRecipients = New System.Windows.Forms.Label()
        Me.TbSmtpPassword = New System.Windows.Forms.TextBox()
        Me.LbSmtpPassword = New System.Windows.Forms.Label()
        Me.TbSmtpUsername = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.TbActivityLog = New System.Windows.Forms.TextBox()
        Me.LbActivityL = New System.Windows.Forms.Label()
        Me.LbStaDash = New System.Windows.Forms.Label()
        Me.TbLatestReleased = New System.Windows.Forms.TextBox()
        Me.LbLatestRelease = New System.Windows.Forms.Label()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.BtTestWh = New System.Windows.Forms.Button()
        Me.BtSendEmail = New System.Windows.Forms.Button()
        Me.BtCheckNewR = New System.Windows.Forms.Button()
        Me.LbManualControl = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'LbRepoSet
        '
        Me.LbRepoSet.AutoSize = True
        Me.LbRepoSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.LbRepoSet.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LbRepoSet.Location = New System.Drawing.Point(16, 25)
        Me.LbRepoSet.Name = "LbRepoSet"
        Me.LbRepoSet.Size = New System.Drawing.Size(194, 20)
        Me.LbRepoSet.TabIndex = 0
        Me.LbRepoSet.Text = "Configuration Settings:"
        '
        'LbRepoURL
        '
        Me.LbRepoURL.AutoSize = True
        Me.LbRepoURL.Location = New System.Drawing.Point(16, 80)
        Me.LbRepoURL.Name = "LbRepoURL"
        Me.LbRepoURL.Size = New System.Drawing.Size(126, 20)
        Me.LbRepoURL.TabIndex = 1
        Me.LbRepoURL.Text = "Repository URL:"
        '
        'LbUserN
        '
        Me.LbUserN.AutoSize = True
        Me.LbUserN.Location = New System.Drawing.Point(16, 125)
        Me.LbUserN.Name = "LbUserN"
        Me.LbUserN.Size = New System.Drawing.Size(87, 20)
        Me.LbUserN.TabIndex = 2
        Me.LbUserN.Text = "Username:"
        '
        'LbWorkingDir
        '
        Me.LbWorkingDir.AutoSize = True
        Me.LbWorkingDir.Location = New System.Drawing.Point(16, 230)
        Me.LbWorkingDir.Name = "LbWorkingDir"
        Me.LbWorkingDir.Size = New System.Drawing.Size(138, 20)
        Me.LbWorkingDir.TabIndex = 4
        Me.LbWorkingDir.Text = "Working Directory:"
        '
        'TbRepoURL
        '
        Me.TbRepoURL.Location = New System.Drawing.Point(265, 77)
        Me.TbRepoURL.Name = "TbRepoURL"
        Me.TbRepoURL.Size = New System.Drawing.Size(774, 26)
        Me.TbRepoURL.TabIndex = 5
        '
        'TbUsername
        '
        Me.TbUsername.Location = New System.Drawing.Point(265, 125)
        Me.TbUsername.Name = "TbUsername"
        Me.TbUsername.Size = New System.Drawing.Size(407, 26)
        Me.TbUsername.TabIndex = 6
        '
        'TbWorkingDirectory
        '
        Me.TbWorkingDirectory.Location = New System.Drawing.Point(265, 230)
        Me.TbWorkingDirectory.Name = "TbWorkingDirectory"
        Me.TbWorkingDirectory.Size = New System.Drawing.Size(774, 26)
        Me.TbWorkingDirectory.TabIndex = 8
        '
        'LbInstOutputD
        '
        Me.LbInstOutputD.AutoSize = True
        Me.LbInstOutputD.Location = New System.Drawing.Point(16, 279)
        Me.LbInstOutputD.Name = "LbInstOutputD"
        Me.LbInstOutputD.Size = New System.Drawing.Size(189, 20)
        Me.LbInstOutputD.TabIndex = 10
        Me.LbInstOutputD.Text = "Installer Output Directory:"
        '
        'TbInstallerOutputDirectory
        '
        Me.TbInstallerOutputDirectory.Location = New System.Drawing.Point(265, 279)
        Me.TbInstallerOutputDirectory.Name = "TbInstallerOutputDirectory"
        Me.TbInstallerOutputDirectory.Size = New System.Drawing.Size(774, 26)
        Me.TbInstallerOutputDirectory.TabIndex = 11
        '
        'TbSmtpPort
        '
        Me.TbSmtpPort.Location = New System.Drawing.Point(837, 325)
        Me.TbSmtpPort.Name = "TbSmtpPort"
        Me.TbSmtpPort.Size = New System.Drawing.Size(161, 26)
        Me.TbSmtpPort.TabIndex = 18
        '
        'TbSmtpServer
        '
        Me.TbSmtpServer.Location = New System.Drawing.Point(265, 325)
        Me.TbSmtpServer.Name = "TbSmtpServer"
        Me.TbSmtpServer.Size = New System.Drawing.Size(407, 26)
        Me.TbSmtpServer.TabIndex = 17
        '
        'LbPort
        '
        Me.LbPort.AutoSize = True
        Me.LbPort.Location = New System.Drawing.Point(746, 326)
        Me.LbPort.Name = "LbPort"
        Me.LbPort.Size = New System.Drawing.Size(42, 20)
        Me.LbPort.TabIndex = 14
        Me.LbPort.Text = "Port:"
        '
        'LbEmailServer
        '
        Me.LbEmailServer.AutoSize = True
        Me.LbEmailServer.Location = New System.Drawing.Point(16, 328)
        Me.LbEmailServer.Name = "LbEmailServer"
        Me.LbEmailServer.Size = New System.Drawing.Size(106, 20)
        Me.LbEmailServer.TabIndex = 13
        Me.LbEmailServer.Text = "SMTP Server:"
        '
        'BtSaveSettings
        '
        Me.BtSaveSettings.Location = New System.Drawing.Point(1093, 547)
        Me.BtSaveSettings.Name = "BtSaveSettings"
        Me.BtSaveSettings.Size = New System.Drawing.Size(169, 31)
        Me.BtSaveSettings.TabIndex = 22
        Me.BtSaveSettings.Text = "Save Settings"
        Me.BtSaveSettings.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.BtClearAll)
        Me.Panel1.Controls.Add(Me.BtClearRelease)
        Me.Panel1.Controls.Add(Me.TbApiToken)
        Me.Panel1.Controls.Add(Me.LbApiToken)
        Me.Panel1.Controls.Add(Me.TbWebhookUrls)
        Me.Panel1.Controls.Add(Me.BtSaveSettings)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.TbEmailRecipients)
        Me.Panel1.Controls.Add(Me.LbEmailRecipients)
        Me.Panel1.Controls.Add(Me.TbSmtpPassword)
        Me.Panel1.Controls.Add(Me.LbSmtpPassword)
        Me.Panel1.Controls.Add(Me.TbSmtpUsername)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.TbWorkingDirectory)
        Me.Panel1.Controls.Add(Me.TbUsername)
        Me.Panel1.Controls.Add(Me.TbRepoURL)
        Me.Panel1.Controls.Add(Me.LbWorkingDir)
        Me.Panel1.Controls.Add(Me.TbSmtpPort)
        Me.Panel1.Controls.Add(Me.TbSmtpServer)
        Me.Panel1.Controls.Add(Me.LbUserN)
        Me.Panel1.Controls.Add(Me.LbPort)
        Me.Panel1.Controls.Add(Me.LbRepoURL)
        Me.Panel1.Controls.Add(Me.LbRepoSet)
        Me.Panel1.Controls.Add(Me.LbEmailServer)
        Me.Panel1.Controls.Add(Me.LbInstOutputD)
        Me.Panel1.Controls.Add(Me.TbInstallerOutputDirectory)
        Me.Panel1.Location = New System.Drawing.Point(36, 26)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1295, 625)
        Me.Panel1.TabIndex = 23
        '
        'BtClearAll
        '
        Me.BtClearAll.Location = New System.Drawing.Point(724, 547)
        Me.BtClearAll.Name = "BtClearAll"
        Me.BtClearAll.Size = New System.Drawing.Size(156, 31)
        Me.BtClearAll.TabIndex = 30
        Me.BtClearAll.Text = "Clear All"
        Me.BtClearAll.UseVisualStyleBackColor = True
        '
        'BtClearRelease
        '
        Me.BtClearRelease.Location = New System.Drawing.Point(912, 547)
        Me.BtClearRelease.Name = "BtClearRelease"
        Me.BtClearRelease.Size = New System.Drawing.Size(156, 31)
        Me.BtClearRelease.TabIndex = 29
        Me.BtClearRelease.Text = "Clear Release"
        Me.BtClearRelease.UseMnemonic = False
        Me.BtClearRelease.UseVisualStyleBackColor = True
        '
        'TbApiToken
        '
        Me.TbApiToken.Location = New System.Drawing.Point(265, 179)
        Me.TbApiToken.Name = "TbApiToken"
        Me.TbApiToken.Size = New System.Drawing.Size(774, 26)
        Me.TbApiToken.TabIndex = 28
        '
        'LbApiToken
        '
        Me.LbApiToken.AutoSize = True
        Me.LbApiToken.Location = New System.Drawing.Point(16, 177)
        Me.LbApiToken.Name = "LbApiToken"
        Me.LbApiToken.Size = New System.Drawing.Size(91, 20)
        Me.LbApiToken.TabIndex = 27
        Me.LbApiToken.Text = "API Token: "
        '
        'TbWebhookUrls
        '
        Me.TbWebhookUrls.Location = New System.Drawing.Point(353, 486)
        Me.TbWebhookUrls.Name = "TbWebhookUrls"
        Me.TbWebhookUrls.Size = New System.Drawing.Size(909, 26)
        Me.TbWebhookUrls.TabIndex = 26
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(16, 486)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(310, 20)
        Me.Label2.TabIndex = 25
        Me.Label2.Text = "Webhooks URL (seperate with semicolon):"
        '
        'TbEmailRecipients
        '
        Me.TbEmailRecipients.Location = New System.Drawing.Point(353, 433)
        Me.TbEmailRecipients.Name = "TbEmailRecipients"
        Me.TbEmailRecipients.Size = New System.Drawing.Size(909, 26)
        Me.TbEmailRecipients.TabIndex = 24
        '
        'LbEmailRecipients
        '
        Me.LbEmailRecipients.AutoSize = True
        Me.LbEmailRecipients.Location = New System.Drawing.Point(16, 433)
        Me.LbEmailRecipients.Name = "LbEmailRecipients"
        Me.LbEmailRecipients.Size = New System.Drawing.Size(315, 20)
        Me.LbEmailRecipients.TabIndex = 23
        Me.LbEmailRecipients.Text = "Email Recipients (seperate with semicolon):"
        '
        'TbSmtpPassword
        '
        Me.TbSmtpPassword.Location = New System.Drawing.Point(825, 372)
        Me.TbSmtpPassword.Name = "TbSmtpPassword"
        Me.TbSmtpPassword.Size = New System.Drawing.Size(407, 26)
        Me.TbSmtpPassword.TabIndex = 22
        '
        'LbSmtpPassword
        '
        Me.LbSmtpPassword.AutoSize = True
        Me.LbSmtpPassword.Location = New System.Drawing.Point(659, 378)
        Me.LbSmtpPassword.Name = "LbSmtpPassword"
        Me.LbSmtpPassword.Size = New System.Drawing.Size(129, 20)
        Me.LbSmtpPassword.TabIndex = 21
        Me.LbSmtpPassword.Text = "SMTP Password:"
        '
        'TbSmtpUsername
        '
        Me.TbSmtpUsername.Location = New System.Drawing.Point(265, 375)
        Me.TbSmtpUsername.Name = "TbSmtpUsername"
        Me.TbSmtpUsername.Size = New System.Drawing.Size(377, 26)
        Me.TbSmtpUsername.TabIndex = 20
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(14, 381)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(134, 20)
        Me.Label1.TabIndex = 19
        Me.Label1.Text = "SMTP Username:"
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.SystemColors.ButtonHighlight
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.TbActivityLog)
        Me.Panel2.Controls.Add(Me.LbActivityL)
        Me.Panel2.Controls.Add(Me.LbStaDash)
        Me.Panel2.Controls.Add(Me.TbLatestReleased)
        Me.Panel2.Controls.Add(Me.LbLatestRelease)
        Me.Panel2.Location = New System.Drawing.Point(36, 686)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1294, 252)
        Me.Panel2.TabIndex = 24
        '
        'TbActivityLog
        '
        Me.TbActivityLog.Location = New System.Drawing.Point(26, 152)
        Me.TbActivityLog.Multiline = True
        Me.TbActivityLog.Name = "TbActivityLog"
        Me.TbActivityLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TbActivityLog.Size = New System.Drawing.Size(1221, 67)
        Me.TbActivityLog.TabIndex = 28
        Me.TbActivityLog.WordWrap = False
        '
        'LbActivityL
        '
        Me.LbActivityL.AutoSize = True
        Me.LbActivityL.Location = New System.Drawing.Point(22, 114)
        Me.LbActivityL.Name = "LbActivityL"
        Me.LbActivityL.Size = New System.Drawing.Size(93, 20)
        Me.LbActivityL.TabIndex = 27
        Me.LbActivityL.Text = "Activity Log:"
        '
        'LbStaDash
        '
        Me.LbStaDash.AutoSize = True
        Me.LbStaDash.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LbStaDash.Location = New System.Drawing.Point(22, 24)
        Me.LbStaDash.Name = "LbStaDash"
        Me.LbStaDash.Size = New System.Drawing.Size(155, 20)
        Me.LbStaDash.TabIndex = 1
        Me.LbStaDash.Text = "Status Dashboard"
        '
        'TbLatestReleased
        '
        Me.TbLatestReleased.Location = New System.Drawing.Point(239, 62)
        Me.TbLatestReleased.Name = "TbLatestReleased"
        Me.TbLatestReleased.Size = New System.Drawing.Size(361, 26)
        Me.TbLatestReleased.TabIndex = 26
        '
        'LbLatestRelease
        '
        Me.LbLatestRelease.AutoSize = True
        Me.LbLatestRelease.Location = New System.Drawing.Point(21, 65)
        Me.LbLatestRelease.Name = "LbLatestRelease"
        Me.LbLatestRelease.Size = New System.Drawing.Size(184, 20)
        Me.LbLatestRelease.TabIndex = 25
        Me.LbLatestRelease.Text = "Latest Released Version"
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.SystemColors.ButtonHighlight
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel3.Controls.Add(Me.BtTestWh)
        Me.Panel3.Controls.Add(Me.BtSendEmail)
        Me.Panel3.Controls.Add(Me.BtCheckNewR)
        Me.Panel3.Controls.Add(Me.LbManualControl)
        Me.Panel3.Location = New System.Drawing.Point(37, 966)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1292, 117)
        Me.Panel3.TabIndex = 25
        '
        'BtTestWh
        '
        Me.BtTestWh.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.BtTestWh.Location = New System.Drawing.Point(532, 57)
        Me.BtTestWh.Name = "BtTestWh"
        Me.BtTestWh.Size = New System.Drawing.Size(203, 38)
        Me.BtTestWh.TabIndex = 14
        Me.BtTestWh.Text = "Test Webhook"
        Me.BtTestWh.UseVisualStyleBackColor = False
        '
        'BtSendEmail
        '
        Me.BtSendEmail.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.BtSendEmail.Location = New System.Drawing.Point(287, 59)
        Me.BtSendEmail.Name = "BtSendEmail"
        Me.BtSendEmail.Size = New System.Drawing.Size(181, 35)
        Me.BtSendEmail.TabIndex = 13
        Me.BtSendEmail.Text = "Send Test Email"
        Me.BtSendEmail.UseVisualStyleBackColor = False
        '
        'BtCheckNewR
        '
        Me.BtCheckNewR.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.BtCheckNewR.Location = New System.Drawing.Point(18, 59)
        Me.BtCheckNewR.Name = "BtCheckNewR"
        Me.BtCheckNewR.Size = New System.Drawing.Size(217, 35)
        Me.BtCheckNewR.TabIndex = 12
        Me.BtCheckNewR.Text = "Check for New Release"
        Me.BtCheckNewR.UseVisualStyleBackColor = False
        '
        'LbManualControl
        '
        Me.LbManualControl.AutoSize = True
        Me.LbManualControl.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LbManualControl.Location = New System.Drawing.Point(21, 26)
        Me.LbManualControl.Name = "LbManualControl"
        Me.LbManualControl.Size = New System.Drawing.Size(135, 20)
        Me.LbManualControl.TabIndex = 11
        Me.LbManualControl.Text = "Manual Control:"
        '
        'FormMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(1365, 1135)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "FormMain"
        Me.Text = "Release Automation System"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LbRepoSet As Label
    Friend WithEvents LbRepoURL As Label
    Friend WithEvents LbUserN As Label
    Friend WithEvents LbWorkingDir As Label
    Friend WithEvents TbRepoURL As TextBox
    Friend WithEvents TbUsername As TextBox
    Friend WithEvents TbWorkingDirectory As TextBox
    Friend WithEvents LbInstOutputD As Label
    Friend WithEvents TbInstallerOutputDirectory As TextBox
    Friend WithEvents TbSmtpPort As TextBox
    Friend WithEvents TbSmtpServer As TextBox
    Friend WithEvents LbPort As Label
    Friend WithEvents LbEmailServer As Label
    Friend WithEvents BtSaveSettings As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents TbEmailRecipients As TextBox
    Friend WithEvents LbEmailRecipients As Label
    Friend WithEvents TbSmtpPassword As TextBox
    Friend WithEvents LbSmtpPassword As Label
    Friend WithEvents TbSmtpUsername As TextBox
    Friend WithEvents TbWebhookUrls As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents LbStaDash As Label
    Friend WithEvents TbActivityLog As TextBox
    Friend WithEvents LbActivityL As Label
    Friend WithEvents TbLatestReleased As TextBox
    Friend WithEvents LbLatestRelease As Label
    Friend WithEvents Panel3 As Panel
    Friend WithEvents BtSendEmail As Button
    Friend WithEvents BtCheckNewR As Button
    Friend WithEvents LbManualControl As Label
    Friend WithEvents BtTestWh As Button
    Friend WithEvents TbApiToken As TextBox
    Friend WithEvents LbApiToken As Label
    Friend WithEvents BtClearRelease As Button
    Friend WithEvents BtClearAll As Button
End Class
