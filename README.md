## CyLR

CyLR — Live Response Collection tool by Alan Orlikoski and Jason Yegge

## What is CyLR?
The CyLR tool collects forensic artifacts from hosts with NTFS file systems quickly, securely and minimizes impact to the host.

The main features are:
*  Quick collection (it's really fast)
*  Raw file collection process does not use Windows API 
*  Optimized to store the collected artifacts in memory (minimizing or removing entirely the need to write additional artifacts on the host disk)
*  Built in SFTP capability

## What's New

## Fixes

## Known Bugs

## Important Notes

## SYNOPSIS

```
CyLR.exe [--help] [-o] [-u] [-p] [-s] [-m] [-c]
```

## DESCRIPTION

CyLR tool collects forensic artifacts from hosts with NTFS file systems quickly, securely and minimizes impact to the host.

The standard list of collected artifacts are:
* "C:\Windows\System32\config"
* "C:\Windows\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup"
* "C:\Windows\Prefetch"
* "C:\Windows\Tasks"
* "C:\Windows\SchedLgU.Txt"
* "C:\Windows\System32\winevt\logs"
* "C:\Windows\System32\drivers\etc\hosts"
* "C:$MFT"

## ARGUMENTS

## OPTIONS
* '-\-help' — Show help message and exit.
* '-o' — Defines the directory that the zip archive will be created in. Defaults to current working directory. (applies to SFTP and local storage options)
* SFTP Options
    * '-u' — SFTP username
    * '-p' — SFTP password
    * '-s' — SFTP Server resolvable hostname or IP address and port. If no port is given then 22 is used by default.  The format is <server name>:<port>.  Usage: -s 8.8.8.8:22"
    * '-m' — Attempt to collect artifacts 100% in memory. WARNING: This may use a lot of memory depending on the size of artifacts
    * '-c' — Optional argument to provide custom list of artifact files and directories (one entry per line). NOTE: Must use full path including drive letter on each line.  MFT can be collected by "C:\$MFT" or "D:\$MFT" and so on.  Usage: -c <path to config file>

## DEPENDENCIES

1. NTFS file system
2. Windows Operating System
3. .NET 4.x

## EXAMPLES
Standard collection
    ```
    CyLR.exe
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