using System;
using CyLR;
using NUnit.Framework;
using System.IO;
using CyLR.archive;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;

namespace CyLRTests.archive
{
    [TestFixture]
    public class SharpZipArchiveTests
    {
        [Test]
        public void TestRoundtripArchive()
        {
            string entryName = "TestFile";
            string entryData = "TestData";
            //Zip up some data
            var testData = Encoding.Unicode.GetBytes(entryData);
            var testFileData = new MemoryStream(testData);
            CyLR.archive.File file = new CyLR.archive.File(entryName, testFileData, new DateTime());
            var zipFile = CreateZipArchive(new[] { file }, "");

            //Unzip that data
            var unzippedData = new MemoryStream();
            var unzippedFile = new ZipFile(zipFile);
            var unzipEntry = unzippedFile.GetEntry(entryName);
            Assert.IsTrue(unzipEntry.IsFile);

            var unzipStream = unzippedFile.GetInputStream(unzipEntry);
            unzipStream.CopyTo(unzippedData);

            Assert.AreEqual(testData, unzippedData.ToArray());
        }

        [Test]
        public void TestRoundtripPasswordProtectedArchive()
        {
            string entryName = "TestFile";
            string entryData = "TestData";
            string archivePassword = "TestPassword";
            //Zip up some data
            var testData = Encoding.Unicode.GetBytes(entryData);
            var testFileData = new MemoryStream(testData);
            CyLR.archive.File file = new CyLR.archive.File(entryName, testFileData, new DateTime());
            var zipFile = CreateZipArchive(new[] { file }, archivePassword);

            //Unzip that data
            var unzippedData = new MemoryStream();
            var unzippedFile = new ZipFile(zipFile);
            var unzipEntry = unzippedFile.GetEntry(entryName);
            Assert.IsTrue(unzipEntry.IsFile);

            Assert.Throws<ZipException>(() =>
            {
                unzippedFile.GetInputStream(unzipEntry);
            });

            unzippedFile.Password = archivePassword;

            var unzipStream = unzippedFile.GetInputStream(unzipEntry);
            unzipStream.CopyTo(unzippedData);

            Assert.AreEqual(testData, unzippedData.ToArray());
        }

        private static MemoryStream CreateZipArchive(IEnumerable<CyLR.archive.File> testData, string password)
        {
            var zipFile = new MemoryStream();
            var archive = new SharpZipArchive(zipFile, password);
            using (archive)
            {
                archive.CollectFilesToArchive(testData);
            }
            return zipFile;
        }
    }
}
