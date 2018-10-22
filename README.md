[![Build Status](https://travis-ci.org/orlikoski/CyLR.svg?branch=master)](https://travis-ci.org/orlikoski/CyLR)  
## CyLR

CyLR — Live Response Collection tool by Alan Orlikoski and Jason Yegge

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
CyLR.exe [--help] [-od] [-of] [-u] [-p] [-s] [-c] [-zp]
```

## DESCRIPTION

CyLR tool collects forensic artifacts from hosts with NTFS file systems quickly, securely and minimizes impact to the host.

The standard list of collected artifacts are:

Windows Default
* "C:\Windows\System32\config"
* "C:\Windows\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup"
* "C:\Windows\Prefetch"
* "C:\Windows\Tasks"
* "C:\Windows\SchedLgU.Txt"
* "C:\Windows\System32\winevt\logs"
* "C:\Windows\System32\drivers\etc\hosts"
* "C:$MFT"

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

## ARGUMENTS

## OPTIONS
* '-\-help' — Show help message and exit.
* '-od' — Defines the directory that the zip archive will be created in. Defaults to current working directory. (applies to SFTP and local storage options)
* '-of' — Defines the name of the zip archive will be created. Defaults to host machine's name.
* '-zp' — If specified, the resulting zip file will be password protected with this password.
* SFTP Options
    * '-u' — SFTP username
    * '-p' — SFTP password
    * '-s' — SFTP Server resolvable hostname or IP address and port. If no port is given then 22 is used by default.  The format is <server name>:<port>.  Usage: -s 8.8.8.8:22"
* '-c' — Optional argument to provide custom list of artifact files and directories (one entry per line). NOTE: Must use full path including drive letter on each line.  MFT can be collected by "C:\$MFT" or "D:\$MFT" and so on.  Usage: -c <path to config file>


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
