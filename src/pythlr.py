#!python3
import io, os, sys, argparse, subprocess, csv, time, datetime, re, multiprocessing, shutil, tempfile, zipfile
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


# Sample usage

# Example for copying the pagefile off a running system
# RawCopy.exe /FileNamePath:C:\pagefile.sys /OutputPath:E:\output

# Example for copying the SYSTEM hive off a running system
# RawCopy.exe /FileNamePath:C:\WINDOWS\system32\config\SYSTEM /OutputPath:E:\output

# Example for extracting the $MFT by specifying its index number, into to the program directory.
# RawCopy.exe /FileNamePath:C:0

# Example for extracting MFT reference number 30224 and all attributes including $DATA, and dumping it into C:\tmp:
# RawCopy.exe /FileNamePath:C:30224 /OutputPath:C:\tmp /AllAttr:1

# Example for accessing a disk image and extracting MftRef ($LogFile) from volume number 2.
# RawCopy.exe /ImageFile:e:\temp\diskimage.dd /ImageVolume:2 /FileNamePath:c:2 /OutputPath:e:\out

# Example for accessing partition/volume image and extracting file.ext and dumping it into E:\out.
# RawCopy.exe /ImageFile:e:\temp\partimage.dd /ImageVolume:1 /FileNamePath:c:\file.ext /OutputPath:e:\out

# Example for making a raw dirlisting in detailed mode in c:\$Extend:
# RawCopy.exe /FileNamePath:c:\$Extend /RawDirMode:1

# Example for making a raw dirlisting in basic mode in c:\System Volume Information inside a disk image file:
# RawCopy.exe /ImageFile:e:\temp\diskimage.dd /ImageVolume:1 /FileNamePath:"c:\System Volume Information" /RawDirMode:2

# Example for making a raw dirlisting in detailed mode on the root level inside a shadow copy:
# RawCopy.exe /FileNamePath:\\.\HarddiskVolumeShadowCopy1:x:\ /RawDirMode:1

# Example for extracting $MFT from partition 2 on harddisk 1 and dumping it into e:\out:
# RawCopy.exe /FileNamePath:\\.\Harddisk0Partition2:0 /OutputPath:e:\out

# Example for extracting $MFT from second volume on PhysicalDrive0:
# RawCopy.exe /FileNamePath:\\.\PhysicalDrive0:0 /ImageVolume:2 /OutputPath:e:\out
rawcopy=r"F:\GitHub\Build\Pythlr\Beta\rawcopy.exe"
rawcopy64=r"F:\GitHub\Build\Pythlr\Beta\rawcopy64.exe"

#filenamepath=r"C:\mtmp\test.txt"
outputpath=tempfile.TemporaryDirectory(prefix="LRDATADIR_")
print("Created Directory", outputpath)
outputzip=r"F:\GitHub\Build\Pythlr\Beta\data\LRdata.zip"



# Complete list of artifacts (MFT must be last for some reason or it doesn't work)
LR_artifacts = [os.path.expandvars(r"%SystemRoot%")+r"\System32\config",r"C:0"]


# Function to copy file to a directory
def get_file(filenamepath):
    if not os.path.isfile(filenamepath):
        if not filenamepath == r"C:0":
            print("File does not exist:", filenamepath)
            return
    print("Collecting file:", filenamepath)
    try:
        subprocess.call([rawcopy64,r"/FileNamePath:"+filenamepath,r"/OutputPath:"+outputpath])
    except:
        subprocess.call([rawcopy,r"/FileNamePath:"+filenamepath,r"/OutputPath:"+outputpath])
    filename = outputpath+"\\"+os.path.basename(filenamepath)
    mftname = outputpath+"\\"+"$MFT"
    while not os.path.exists(filename) and not os.path.exists(mftname):
        time.sleep(1)
    if filenamepath == r"C:0":
        filenamepath = "$MFT"
    print("File collected:")#" ", filenamepath)
    return

# Function to copy all files in directory and sub-directories to a directory
def get_dir(dirnamepath):
    for dirname, subdirs, files in os.walk(dirnamepath):
            for filename in files:
                filenamepath =os.path.join(dirname, filename)
                get_file(filenamepath)
    return


with tempfile.TemporaryDirectory(prefix="LRDATADIR_") as outputpath:
    for artifact in LR_artifacts:
        if os.path.isdir(artifact):
            get_dir(artifact)
        else:
            get_file(artifact)
    print("All artifacts collected")
    if os.path.isfile(outputzip):
        print("deleting existing file:", outputzip)
        os.remove(outputzip)
    print("Compressing artifacts into:", outputzip)
    zf = zipfile.ZipFile(outputzip, "w")
    for dirname, subdirs, files in os.walk(outputpath):
        zf.write(dirname)
        for filename in files:
            zf.write(os.path.join(dirname, filename))
    zf.close()
    print("Compression complete")






print("Process Complete: LR DATA file is:", outputzip)
