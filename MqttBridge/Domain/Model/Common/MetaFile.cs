namespace Domain.Model.Common;

using System.Security.Cryptography;
using System.Text;

public class MetaFile
{
    public string? FileName { get; set; } = null;
    public byte[]? Payload { get; set; } = null;
    public string? HashMd5 { get; set; } = string.Empty;
    public int NumOfTotalFiles { get; set; } = 0;
    public int IndexOfFile { get; set; } = 0;

    public MetaFile(string? filePath, int numOfTotalFiles = 1, int indexOfFile = 1)
    {
        if (filePath != null)
        {
            this.FileName = filePath;
            this.Payload = File.ReadAllBytes(filePath);
            this.HashMd5 = CalculateFileMD5(filePath);
            this.NumOfTotalFiles = numOfTotalFiles;
            this.IndexOfFile = indexOfFile;
        }
    }

    public static string CalculateFileMD5(string filePath)
    {
        using (MD5 md5 = MD5.Create())
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }
    }

    public static string CalculateStringMD5(string content)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(content));
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}
