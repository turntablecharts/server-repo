using System;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Infrastructure.DAL
{
    public class BlobRepo : IBlobRepo
    {
         public string UploadFileToBlob(string fileName, byte[] fileData, string fileMimeType, string blobKey)
        {
            try
            {
                var task = Task.Run(() => this.UploadFileToBlobAsync(fileName, fileData, fileMimeType, blobKey));
                task.Wait();
                string fileUrl = task.Result;
                return fileUrl;
            }
            catch (Exception ex)
            {

                throw (ex);
            }
        }

        private async Task<string> UploadFileToBlobAsync(string fileName, byte[] fileData, string fileMimeType, string blobKey)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobKey);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                string containerName = "ttc-uploads";
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
                string generatedFileName = this.GenerateFileName(fileName);

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                if (generatedFileName != null && fileData != null)
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(generatedFileName);
                    cloudBlockBlob.Properties.ContentType = fileMimeType;
                    await cloudBlockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);

                    return cloudBlockBlob.Uri.ToString();
                }

                return "";
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string GenerateFileName(string fileName)
        {
            string strFileName = string.Empty;
            string[] strName = fileName.Split('.');
            strFileName = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/" + DateTime.Now.ToUniversalTime().ToString("yyyMMdd\\THHmmssfff") + "." + strName[strName.Length - 1];
            return strFileName;
        }
    
    }
}