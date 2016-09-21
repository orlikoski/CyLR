## CyLR

CyLR — Live Response Collection tool by Alan Orlikoski and Jason Yegge

## What is CyLR?
The CyLR tool collects forensic artifacts from hosts with NTFS file systems quickly, securely and minimizes impact to the host.

The main features are:
*  Collection process does not use Windows API
*  Optimized to store the collected artifacts in memory (minimizing or removing entirely the need to write additional artifacts on the host disk)
*  Built in SFTP capability

## What's New

## Fixes

## Known Bugs

## Important Notes

## SYNOPSIS

```
CyLR.exe [--help] [-o] [-u] [-p] [-s] [-m]
```

## DESCRIPTION

CyLR tool collects forensic artifacts from hosts with NTFS file systems quickly, securely and minimizes impact to the host.

The standard list of collected artifacts are:
* "\Windows\System32\config"
* "\Windows\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup"
* "\Windows\Prefetch"
* "\Windows\Tasks"
* "\Windows\SchedLgU.Txt"
* "\Windows\System32\winevt\logs"
* "\Windows\System32\drivers\etc\hosts"
* "$MFT"

## ARGUMENTS

## OPTIONS
* `--help` — Show help message and exit.
* '-o' — Defines the directory that the zip archive will be created in. Defaults to current working directory. (applies to SFTP and local storage options)
* SFTP Options
    * '-u' — SFTP username
    * '-p' — SFTP password
    * '-s' — SFTP Server resolvable hostname or IP address
    * '-m' — Attempt to collect artifacts 100% in memory. WARNING: This may use a lot of memory depending on the size of artifacts

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