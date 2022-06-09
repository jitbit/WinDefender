# WinDefender
Invokes Windows Defender scan from C# .NET Core (on Windows). Works even in ASP.NET

## Background

Windows defender comes with a CLI tool. It does not launch the actual antivirus process, it's just a CLI-API to the antivirus that is always running. Even if you have "real time protection" disabled (recommended for server environment).
