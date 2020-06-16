[![Build Status](https://travis-ci.org/orlikoski/CyLR.svg?branch=master)](https://travis-ci.org/orlikoski/CyLR)  
## CyLR

CyLR — Live Response Collection tool by Alan Orlikoski and Jason Yegge

## Please Read
[Open Letter to the users of Skadi, CyLR, and CDQR](https://docs.google.com/document/d/1L6CBvFd7d1Qf4IxSJSdkKMTdbBuWzSzUM3u_h5ZCegY/edit?usp=sharing)

## Videos and Media
*  [OSDFCON 2017](http://www.osdfcon.org/presentations/2017/Asif-Matadar_Rapid-Incident-Response.pdf) Slides: Walk-through different techniques that are required to provide forensics results for Windows and *nix environments (Including CyLR and CDQR)

## What is CyLR?
The CyLR tool collects forensic artifacts from hosts with NTFS file systems quickly, securely and minimizes impact to the host.

The main features are:
*  Quick collection (it's really fast)
*  Raw file collection process does not use Windows API
*  Optimized to store the collected artifacts in memory (minimizing or removing entirely the need to write additional artifacts on the host disk)
*  Built in SFTP capability

CyLR uses .NET Core and runs natively on Windows, Linux, and MacOS. Self contained applications for the following are included in releases for version 2.0 and higher.
 - Windows x86
 - Windows x64
 - Linux x64
 - MacOS x64

## SYNOPSIS

```
CyLR.exe [--help] [-od] [-of] [-u] [-p] [-s] [-c] [-zp] [-zl]
```

## DESCRIPTION

CyLR tool collects forensic artifacts from hosts with NTFS file systems quickly, securely and minimizes impact to the host.

The standard list of collected artifacts are:

Windows Default
* System Level Artifacts
    * %SYSTEMROOT%\SchedLgU.Txt
    * %SYSTEMROOT%\Tasks
    * %SYSTEMROOT%\Prefetch
    * %SYSTEMROOT%\inf\setupapi.dev.log
    * %SYSTEMROOT%\Appcompat\Programs
    * %SYSTEMROOT%\System32\drivers\etc\hosts
    * %SYSTEMROOT%\System32\sru
    * %SYSTEMROOT%\System32\winevt\logs
    * %SYSTEMROOT%\System32\Tasks
    * %SYSTEMROOT%\System32\LogFiles\W3SVC1
    * %SYSTEMROOT%\System32\config\SAM
    * %SYSTEMROOT%\System32\config\SYSTEM
    * %SYSTEMROOT%\System32\config\SOFTWARE
    * %SYSTEMROOT%\System32\config\SECURITY
    * %SYSTEMROOT%\System32\config\SAM.LOG1
    * %SYSTEMROOT%\System32\config\SYSTEM.LOG1
    * %SYSTEMROOT%\System32\config\SOFTWARE.LOG1
    * %SYSTEMROOT%\System32\config\SECURITY.LOG1
    * %SYSTEMROOT%\System32\config\SAM.LOG2
    * %SYSTEMROOT%\System32\config\SYSTEM.LOG2
    * %SYSTEMROOT%\System32\config\SOFTWARE.LOG2
    * %SYSTEMROOT%\System32\config\SECURITY.LOG2
    * %PROGRAMDATA%\Microsoft\Search\Data\Applications\Windows
    * %PROGRAMDATA%\Microsoft\Windows\Start Menu\Programs\Startup
    * %SystemDrive%\$Recycle.Bin
    * %SystemDrive%\$LogFile
    * %SystemDrive%\$MFT
* Artifacts For All Users
    * {user.ProfilePath}\NTUSER.DAT
    * {user.ProfilePath}\AppData\Local\Microsoft\Windows\UsrClass.dat
    * {user.ProfilePath}\AppData\Roaming\Microsoft\Windows\Recent
    * {user.ProfilePath}\NTUSER.DAT
    * {user.ProfilePath}\NTUSER.DAT.LOG1
    * {user.ProfilePath}\NTUSER.DAT.LOG2
    * {user.ProfilePath}\AppData\Local\Microsoft\Windows\UsrClass.dat
    * {user.ProfilePath}\AppData\Local\Microsoft\Windows\UsrClass.dat.LOG1
    * {user.ProfilePath}\AppData\Local\Microsoft\Windows\UsrClass.dat.LOG2
    * {user.ProfilePath}\AppData\Local\Microsoft\Windows\Explorer
    * {user.ProfilePath}\AppData\Local\Google\Chrome\User Data\Default\History\
    * {user.ProfilePath}\AppData\Local\Microsoft\Windows\WebCache\
    * {user.ProfilePath}\AppData\Local\ConnectedDevicesPlatform
    * {user.ProfilePath}\AppData\Roaming\Microsoft\Windows\Recent\AutomaticDestinations\
    * {user.ProfilePath}\AppData\Roaming\Microsoft\Windows\PowerShell\PSReadline
    * {user.ProfilePath}\AppData\Roaming\Mozilla\Firefox\Profiles\

Mac and Linux Default
*  "/var/log",
*  "/private/var/log/",
*  "/.fseventsd",
*  "/etc/hosts.allow",
*  "/etc/hosts.deny",
*  "/etc/hosts",
*  "/System/Library/StartupItems",
*  "/System/Library/LaunchAgents",
*  "/System/Library/LaunchDaemons",
*  "/Library/LaunchAgents",
*  "/Library/LaunchDaemons",
*  "/Library/StartupItems",
*  "/etc/passwd",
*  "/etc/group"
*  All plist files
*  All .bash_history files
*  All .sh_history files
*  Chrome Preference files 
*  FireFox Preference Files

## ARGUMENTS

## OPTIONS
* '-\-help' — Show help message and exit.
* '-od' — Defines the directory that the zip archive will be created in. Defaults to current working directory. (applies to SFTP and local storage options)
* '-of' — Defines the name of the zip archive will be created. Defaults to host machine's name.
* '-zp' — If specified, the resulting zip file will be password protected with this password.
* '-zl [1-9]' - If specified with an integer it will change the compression level. If the flag is not passed it defaults to 3.  
* SFTP Options
    * '-u' — SFTP username
    * '-p' — SFTP password
    * '-s' — SFTP Server resolvable hostname or IP address and port. If no port is given then 22 is used by default.  The format is <server name>:<port>.  Usage: -s 8.8.8.8:22"
* '-c' — Optional argument to provide custom list of artifact files and directories (one entry per line). NOTE: Must use full path including drive letter on each line.  MFT can be collected by "C:\\$MFT" or "D:\\$MFT" and so on.  Usage: -c <path to config file>


## DEPENDENCIES
in general: some kind of administrative rights on the target (root, sudo, administrator,...).

CyLR now uses .NET Core and now runs natively on Windows, Linux, and MacOS as a .NET Core app or a self contained executable.

### Windows
 - None

### Linux/macOS
 - None


## EXAMPLES
Standard collection
    ```
    CyLR.exe
    ```

Linux/macOS collection
    ```
    ./CyLR
    ```

Collect artifacts and store data in "C:\Temp\LRData"
    ```
    CyLR.exe -o "C:\Temp\LRData"
    ```

Collect artifacts and store data in ".\LRData"
    ```
    CyLR.exe -o LRData
    ```

Collect artifacts and send data to SFTP server 8.8.8.8
    ```
    CyLR.exe -u username -p password -s 8.8.8.8
    ```

Collect artifacts, send data to SFTP server 8.8.8.8, and keep all artifacts in memory
    ```
    CyLR.exe -u username -p password -s 8.8.8.8 -m
    ```

## AUTHORS
* [Jason Yegge](https://github.com/Lansatac)
* [Alan Orlikoski](https://github.com/rough007)
