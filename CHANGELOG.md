# CHANGE LOG

## 2.2.0 - 2020-07-09

This version includes numerous modifications and introduction of new features,
highlighted below:

### Added

* Logging is available, to destinations including the console, a log file, and
  embedded within the resulting archive. The log name is specified with `-l`
  and verbosity is adjusted with `-v` to increase or `-q` to silence.
* Added `CUSTOM_PATH_TEMPLATE.txt` with documentation on how to specify custom
  paths for collection.
* Implemented enumeration of files system contents in the same manner cross
  platform
* Through new FS enumeration, eliminated extra scanning/duplicate collection
  of data within symbolic link directories. Eliminated dependency on the
  `find` binary.
* Enabled the use of globbing patterns within paths. This includes patterns
  such as:
  * `**/*.plist`
  * `/home/*/.*sh_history`
  * `\Windows\Temp\[a-z0-9][a-z0-9][a-z0-9][a-z0-9]\*`
  * `**/Library/*Support/Google/Chrome/Default/History*`
* Enabled the use of regular expressions within paths. This includes full line
  and substring patterns, such as:
  * `.*mawlare.*`
  * `^C:\Windows\Temp\[A-Za-z0-9]{8}\.*$`
  * `^C:\Windows\System32\Config\(SOFTWARE|SYSTEM|SAM|SECURITY).*$`
* Added functionality to allow the user to select whether the existence of a
  custom collection list (`-c`) should be in addition to versus in place of
  the default artifact list. Continues to default to the replacement option
  where it will only collect specified files.
* Modified config file to support specification of path pattern type. Can be
  one of `static`, `glob`, or `regex`. Format should be a tab delimited text
  file with one pattern type and path per line. A line starting with a pound
  character will be ignored.
* Provided status messages to summarize the number of files scanned and paths
  staged for collection.
* Increased documentation of source code.

### Removed

* Removed collection of Windows Search path due to large size on some systems
  (`%PROGRAMDATA%\Microsoft\Search\Data\Applications\Windows`).
  Please use `-c` to re-include as needed.

### Changed

* Edited build scripts to point to C:\Program Files\7z instead of x86 folder
* Improved the default collection paths for Linux platforms.
* Modified the USNJrnl collection argument to default to disabled collection.
* Improved SFTP handling to collect to a local zip file and attempt the
  upload three times, with a 30 second delay between attempts.
* Semantic changes to packaging and build scripts to avoid alias use.
* Added packaging script check to see if packaging tool was local before
  re-downloading.
* Updated argument usage information.
* Added tests to increase coverage of Arguments.cs
