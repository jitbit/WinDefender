# WinDefender
Invokes Windows Defender scan from C# .NET Core 6 (on Windows). Works even in ASP.NET

## Usage

Add [this file](/WinDefender.cs) to your project then call:

```csharp
//check by filename
bool isVirus = await WinDefender.IsVirus(@"c:\path\to\file");

//check by byte array
byte[] fileContents = ReadFileFromSomewhere();
bool isVirus = await WinDefender.IsVirus(fileContents);

//cancellation token support if you want ot abort
bool isVirus = await WinDefender.IsVirus(fileContents, cancellationToken);

```

## Background

Windows defender comes with a CLI tool. It does not launch the actual antivirus process, it's just a CLI-API to the antivirus that is always running. Even if you have "real time protection" disabled (recommended for server environment) the CLI still works.
