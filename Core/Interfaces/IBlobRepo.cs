namespace Core.Interfaces
{
    public interface IBlobRepo
    {
        string UploadFileToBlob(string fileName, byte[] fileData, string fileMimeType, string blobKey);
    }
}