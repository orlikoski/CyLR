using System;
using CyLR;
using Xunit;
using System.IO;
using CyLR.archive;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;

namespace CyLRTests.archive
{
    public class SharpZipArchiveTests
    {
        [Fact]
        public void TestRoundtripArchive()
        {
            string entryName = "TestFile";
            string entryData = "TestData";
            //Zip up some data
            var testData = Encoding.Unicode.GetBytes(entryData);
            var testFileData = new MemoryStream(testData);
            CyLR.archive.File file = new CyLR.archive.File(entryName, testFileData, new DateTime());
            var zipFile = CreateZipArchive(new[] { file }, "","3");

            //Unzip that data
            var unzippedData = new MemoryStream();
            var unzippedFile = new ZipFile(zipFile);
            var unzipEntry = unzippedFile.GetEntry(entryName);
            Assert.True(unzipEntry.IsFile);

            var unzipStream = unzippedFile.GetInputStream(unzipEntry);
            unzipStream.CopyTo(unzippedData);

            Assert.Equal(testData, unzippedData.ToArray());
        }

        [Fact]
        public void TestRoundtripPasswordProtectedArchive()
        {
            string entryName = "TestFile";
            string entryData = "TestData";
            string archivePassword = "TestPassword";
            string archiveCompressionLevel = "3";
            //Zip up some data
            var testData = Encoding.Unicode.GetBytes(entryData);
            var testFileData = new MemoryStream(testData);
            CyLR.archive.File file = new CyLR.archive.File(entryName, testFileData, new DateTime());
            var zipFile = CreateZipArchive(new[] { file }, archivePassword, archiveCompressionLevel);

            //Unzip that data
            var unzippedData = new MemoryStream();
            var unzippedFile = new ZipFile(zipFile);
            var unzipEntry = unzippedFile.GetEntry(entryName);
            Assert.True(unzipEntry.IsFile);

            Assert.Throws<ZipException>(() =>
            {
                unzippedFile.GetInputStream(unzipEntry);
            });

            unzippedFile.Password = archivePassword;

            var unzipStream = unzippedFile.GetInputStream(unzipEntry);
            unzipStream.CopyTo(unzippedData);

            Assert.Equal(testData, unzippedData.ToArray());
        }

        private static MemoryStream CreateZipArchive(IEnumerable<CyLR.archive.File> testData, string password, string level)
        {
            var zipFile = new MemoryStream();
            var archive = new SharpZipArchive(zipFile, password, level);
            using (archive)
            {
                archive.CollectFilesToArchive(testData);
            }
            return zipFile;
        }
    }
}
