#!python3
import io, os, sys, argparse, subprocess, time, datetime, re, tempfile, zipfile, threading, shutil
#from multiprocessing.pool import Pool


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

MFTREF_pattern = re.compile('^MFT Ref: (\d.{1,20})')
MFTFileName_pattern = re.compile('FileName: (\S.{1,200})')

rawcopy=r"rawcopy.exe"
rawcopy64=r"rawcopy64.exe"

outputpath="LRData"
outputzip=r"LRdata.zip"
max_cpu = True



# Complete list of artifacts (MFT must be last for some reason or it doesn't work)
# Everything for LR
LR_artifacts = [r"%SystemRoot%\System32\config",
                r"%SystemRoot%\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup",
                r"%SystemRoot%\Prefetch",
                r"%SystemRoot%\Tasks",
                r"%SystemRoot%\SchedLgU.Txt",
                r"%SystemRoot%\System32\winevt\logs",
                r"%SystemRoot%\System32\drivers\etc\hosts",
                r"C:0"]

# Laser Focused LR
# LR_artifacts = [r"%SystemRoot%\System32\config\SAM",
#                 r"%SystemRoot%\System32\config\SECURITY",
#                 r"%SystemRoot%\System32\config\SOFTWARE",
#                 r"%SystemRoot%\System32\config\SYSTEM",
#                 r"%SystemRoot%\System32\winevt\logs\Application.evtx",
#                 r"%SystemRoot%\System32\winevt\logs\Security.evtx",
#                 r"%SystemRoot%\System32\winevt\logs\System.evtx",
#                 r"%SystemRoot%\System32\winevt\logs\Windows PowerShell.evtx",
#                 r"%SystemRoot%\Tasks",
#                 r"%SystemRoot%\SchedLgU.Txt",
#                 r"%SystemRoot%\System32\drivers\etc\hosts",
#                 r"C:0"]

# Testing LR
# LR_artifacts = [r"%SystemRoot%\System32\config"]

# Call RawCopy command
def rawcopyfunc(filenamepath,copypath):
    command64bit = [rawcopy64,r"/FileNamePath:"+filenamepath,r"/OutputPath:"+copypath]
    command32bit = [rawcopy,r"/FileNamePath:"+filenamepath,r"/OutputPath:"+copypath]

    try:
        subprocess.call(command64bit,stdout=subprocess.DEVNULL)
        print("The command run was:"," ".join(command64bit))

    except:
        subprocess.call(command32bit,stdout=subprocess.DEVNULL)
        print("The command run was:"," ".join(command32bit))
    return


# Function to copy file to a directory
def get_file(filenamepath, copypath):
    if not os.path.isfile(filenamepath):
        if not filenamepath == r"C:0":
            print("File does not exist:", filenamepath)
            return
    if not filenamepath == r"C:0":
        print("Collecting file:", filenamepath)
    else:
        print("Collecting file: $MFT")

    if not os.path.exists(copypath):
        os.makedirs(copypath)

    rawcopyfunc(filenamepath,copypath)

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

# Function to return a list of the MFT numbers for each file in a directory
def get_dir(dirnamepath, copypath):
    print("Parsing the MFT for all files in:", dirnamepath)
    dp = subprocess.Popen([rawcopy64,r"/FileNamePath:"+dirnamepath,r"/RawDirMode:1"],stdout=subprocess.PIPE,stderr=subprocess.PIPE)
    fileinfo = []
    lof = []
    for line in io.TextIOWrapper(dp.stdout, encoding="utf-8"):
        MFTFileName = MFTFileName_pattern.search(line)
        if MFTFileName:
            fileinfo.append(MFTFileName.group(1))

        MFTREF = MFTREF_pattern.search(line)
        if MFTREF:
            fileinfo.append(MFTREF.group(1))
            lof.append(fileinfo)
            fileinfo = []

    threads = []
    for filerecords in lof:
        print("Collecting file:",dirnamepath+"\\"+filerecords[0])
        outputpath = copypath+"\\"+os.path.basename(dirnamepath)
        if not os.path.exists(outputpath):
            os.makedirs(outputpath)
        if max_cpu:
            t = threading.Thread(target=rawcopyfunc, args=("C:"+filerecords[1],outputpath))
            threads.append(t)
            t.start()
        else:
            rawcopyfunc("C:"+filerecords[1],outputpath)

    # Wait for all threads to finish
    for t in threads:
        t.join()


def main():
    ts = time.time()
    print("Collection process started")

    # Creating temp directory and gathering artifacts
    with tempfile.TemporaryDirectory(prefix="LRDATADIR_") as outputpath:
        print("Created Temp Dir:", outputpath)
        for artifact in LR_artifacts:
            exp_artifact = os.path.expandvars(artifact)
            sourcepath = exp_artifact[2:]
            if sourcepath[0] != "\\":
                copypath = outputpath + "\\"+ os.path.dirname(sourcepath)
            else:
                copypath = outputpath + os.path.dirname(sourcepath)

            if os.path.isdir(exp_artifact):
                get_dir(exp_artifact,copypath)
            else:
                get_file(exp_artifact,copypath)
        print("All artifacts collected")


        # Compressing Results
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

    # Wrapping it up
    print("Deleted Temp Dir:", outputpath)
    print("LR DATA file is:", outputzip)
    print("Collection process complete")
    print('Took {}s'.format(time.time() - ts))

if __name__ == '__main__':
   main()