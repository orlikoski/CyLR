using System.IO;
using Renci.SshNet;

namespace CyLR.write
{
    internal static class Sftp
    {
        public static void SendUsingSftp(Stream archiveStream, string sftpServer, int port, string userName, string userPassword, string destinationPath)
        {
            var client = new SftpClient(sftpServer, port, userName, userPassword);
            client.Connect();
            client.UploadFile(archiveStream, destinationPath);
            client.Disconnect();
        }
    }
}
