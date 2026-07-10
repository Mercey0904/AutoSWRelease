# WindowsApp1 - Auto Deployment System

##  Project Overview
A VB.NET Windows application with automated deployment via webhook triggers. The system automatically builds, versions, and releases the application when new commits are pushed to the repository.

##  Quick Setup

### 1. Repository Structure
**CRITICAL**: Your Bitbucket repository must have this exact structure:
```
windowsapp1/                 # Repository root
├── WindowsApp1/            # Project folder (must match exactly)
│   ├── WindowsApp1.vbproj  # Project file
│   ├── My Project/
│   │   └── AssemblyInfo.vb # Version file
│   ├── bin/
│   │   └── Release/
│   │       └── WindowsApp1.exe
│   └── ... (other source files)
└── README.md
```

### 2. Email Configuration (SMTP)

#### For Gmail:
```vb
SMTP Server: smtp.gmail.com
Port: 587
Username: your.email@gmail.com
Password: YOUR_APP_PASSWORD  ← NOT your regular password!
```

####  App Password Setup (Required):
1. Go to [Google Account Settings](https://myaccount.google.com/security)
2. Enable **2-Factor Authentication**
3. Generate **App Password**:
   - Security → 2-Step Verification → App passwords
   - Select "Mail" and your device
   - Use the 16-character password generated

#### For Outlook/Office 365:
```vb
SMTP Server: smtp.office365.com
Port: 587
Username: your.email@company.com
Password: YOUR_PASSWORD
```

### 3. Webhook Configuration

#### Bitbucket Webhook URL Format:
```
https://api.bitbucket.org/2.0/repositories/{workspace}/{repo_slug}/commit/{commit_hash}
```

#### Webhook Settings in Bitbucket:
1. Go to: `Repository Settings → Webhooks`
2. Add Webhook:
   - **URL**: `http://your-server:8080/webhook`
   - **Triggers**: Push, Commit comments
   - **SSL**: Enabled

### 4. Directory Structure (Local Development)

#### Correct Paths for Webhook:
```bash
Working Directory: C:\deploy\windowsapp1\
Installer Output: C:\deploy\windowsapp1\WindowsApp1\bin\Release\
Executable Name: WindowsApp1.exe
```

#### Development Paths:
```bash
Source: C:\dev\WindowsApp1\WindowsApp1\  # Your working copy
Deploy: C:\deploy\windowsapp1\           # Webhook target (clean)
```

## ⚙️ Configuration Files

### AssemblyInfo.vb (Version Control)
```vb
<Assembly: AssemblyVersion("1.0.1.0")>
<Assembly: AssemblyFileVersion("1.0.1.0")>
```

### App.config (Email Settings)
```xml
<appSettings>
    <add key="SmtpServer" value="smtp.gmail.com"/>
    <add key="SmtpPort" value="587"/>
    <add key="SmtpUsername" value="your.email@gmail.com"/>
    <add key="SmtpPassword" value="YOUR_APP_PASSWORD_HERE"/>
</appSettings>
```

## 🛠️ Build & Deployment

### Manual Build Commands
```cmd
# Build Release version
msbuild WindowsApp1.vbproj /p:Configuration=Release

# Or using Visual Studio
devenv WindowsApp1.sln /Build Release
```

### Webhook Automation
The system automatically:
- Listens for commits on port 8080
- Clones the latest version
- Builds the project in Release mode
- Extracts version from AssemblyInfo.vb
- Creates deployment packages

##  Troubleshooting

### Common Issues & Solutions

#### 1. SMTP Authentication Failed
**Problem**: "5.7.0 Authentication Required"
**Solution**: 
- Use App Password, not regular password
- Enable 2-Factor Authentication
- Check SMTP server/port settings

#### 2. Webhook Not Triggering
**Problem**: No deployment on commit
**Solution**:
- Verify webhook URL ends with `/webhook`
- Check if port 8080 is accessible
- Confirm repository structure matches exactly

#### 3. Version Detection Failed
**Problem**: "Duplicate version 0.0.0.0"
**Solution**:
- Ensure AssemblyInfo.vb exists in `My Project/` folder
- Check version numbers are incremented
- Verify file is committed to repository

#### 4. Nested Directory Issues
**Problem**: Multiple `WindowsApp1` folders
**Solution**:
- Use clean deployment directory
- Ensure repository has correct structure
- Remove existing `.git` folders from deployment target

##  Notification System

The application sends email notifications for:
- Successful deployments
- Build failures
- Version updates
- System errors

### Email Template
```
Subject: [WindowsApp1] Deployment Notification
Body: 
Application: WindowsApp1
Version: 1.0.1.0
Status: SUCCESS/FAILED
Timestamp: 2025-10-21 11:44:53
Commit: 88ccb96
```

##  Security Notes

- Never commit real passwords to repository
- Use environment variables for sensitive data
- App passwords are preferred over regular passwords
- Webhook should use HTTPS in production
- Restrict webhook access to trusted IPs

##  Support

For deployment issues:
1. Check webhook logs in `C:\deploy\logs\`
2. Verify repository structure matches exactly
3. Confirm SMTP settings use App Password
4. Ensure all paths use consistent casing

---

**Remember**: The directory structure and URL formats are critical for the automated deployment system to work correctly!
