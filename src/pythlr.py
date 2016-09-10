#!python3
import io, os, sys, argparse, subprocess, csv, time, datetime, re, multiprocessing, shutil, tempfile, zipfile
try:
    import zlib
    compression = zipfile.ZIP_DEFLATED
except:
    compression = zipfile.ZIP_STORED

modes = { zipfile.ZIP_DEFLATED: 'deflated',
          zipfile.ZIP_STORED:   'stored',
          }
###############################################################################
# Created by: Alan Orlikoski
# Version Info
#
# What's New
#
# Known Bugs
#
# DEPENDANCIES: 

###############################################################################


rawcopy=r"rawcopy.exe"
rawcopy64=r"rawcopy64.exe"

outputpath=tempfile.TemporaryDirectory(prefix="LRDATADIR_")
outputzip=r"LRdata.zip"



# Complete list of artifacts (MFT must be last for some reason or it doesn't work)
# Everything for LR
# LR_artifacts = [r"%SystemRoot%\System32\config",
#                 r"%SystemRoot%\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup",
#                 r"%SystemRoot%\Prefetch",
#                 r"%SystemRoot%\Tasks",
#                 r"%SystemRoot%\SchedLgU.Txt",
#                 r"%SystemRoot%\System32\winevt\logs",
#                 r"%SystemRoot%\System32\drivers\etc\hosts",
#                 r"C:0"]

# Laser Focused LR
LR_artifacts = [r"%SystemRoot%\System32\config\SAM",
                r"%SystemRoot%\System32\config\SECURITY",
                r"%SystemRoot%\System32\config\SOFTWARE",
                r"%SystemRoot%\System32\config\SYSTEM",
                r"%SystemRoot%\System32\winevt\logs\Application.evtx",
                r"%SystemRoot%\System32\winevt\logs\Security.evtx",
                r"%SystemRoot%\System32\winevt\logs\System.evtx",
                r"%SystemRoot%\System32\winevt\logs\Windows PowerShell.evtx",
                r"%SystemRoot%\Tasks",
                r"%SystemRoot%\SchedLgU.Txt",
                r"%SystemRoot%\System32\drivers\etc\hosts",
                r"C:0"]

# Testing LR
# LR_artifacts = [r"C:0"]


# Function to copy file to a directory
def get_file(filenamepath):
    if not os.path.isfile(filenamepath):
        if not filenamepath == r"C:0":
            print("File does not exist:", filenamepath)
            return
    if not filenamepath == r"C:0":
        print("Collecting file:", filenamepath)
    else:
        print("Collecting file: $MFT")
    sourcepath = filenamepath[2:]
    if sourcepath[0] != "\\":
        copypath = outputpath + "\\"+ os.path.dirname(sourcepath)
    else:
        copypath = outputpath + os.path.dirname(sourcepath)

    if not os.path.exists(copypath):
        os.makedirs(copypath)


    try:
        subprocess.call([rawcopy64,r"/FileNamePath:"+filenamepath,r"/OutputPath:"+copypath])
    except:
        subprocess.call([rawcopy,r"/FileNamePath:"+filenamepath,r"/OutputPath:"+copypath])
    filename = copypath+"\\"+os.path.basename(filenamepath)
    mftname = copypath+"$MFT"

    while not os.path.exists(filename) and not os.path.exists(mftname):
        time.sleep(1)
    if filenamepath == r"C:0":
        print("File written to:", mftname)
    else:
        print("File written to:", filename)
    print("")
    return

# Function to copy all files in directory and sub-directories to a directory
def get_dir(dirnamepath):
    for dirname, subdirs, files in os.walk(dirnamepath):
            for filename in files:
                filenamepath =os.path.join(dirname, filename)
                get_file(filenamepath)
    return

print("Collection process started")

with tempfile.TemporaryDirectory(prefix="LRDATADIR_") as outputpath:
    print("Created Temp Dir:", outputpath)
    for artifact in LR_artifacts:
        exp_artifact = os.path.expandvars(artifact)
        if os.path.isdir(exp_artifact):
            get_dir(exp_artifact)
        else:
            get_file(exp_artifact)
    print("All artifacts collected")
    if os.path.isfile(outputzip):
        print("deleting existing file:", outputzip)
        os.remove(outputzip)
    print("Compressing artifacts")
    zf = zipfile.ZipFile(outputzip, "w")
    for dirname, subdirs, files in os.walk(outputpath):
        for filename in files:
            fnameandpath = os.path.join(dirname, filename)
            zf.write(fnameandpath, os.path.relpath(fnameandpath, outputpath), compress_type=compression)
    zf.close()
    print("Compression complete")

print("Deleted Temp Dir:", outputpath)
print("LR DATA file is:", outputzip)
print("Collection process complete")