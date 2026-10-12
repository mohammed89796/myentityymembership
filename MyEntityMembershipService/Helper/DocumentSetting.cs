using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Serilog;

namespace MyEntityMembershipService.Helper
{
    public static class DocumentSetting
    {
        private static string _bucketName = Environment.GetEnvironmentVariable("Storage_BucketName");
        private static string _accessKey = Environment.GetEnvironmentVariable("Storage_AccessKey");
        private static string _secretKey = Environment.GetEnvironmentVariable("Storage_SecretKey");
        private static string _endpoint = Environment.GetEnvironmentVariable("Storage_Endpoint");
        public static bool IsImageFile(IFormFile file)
        {
            if (file == null || file.FileName == null)
            {
                // Handle the case where the file is null or its file name is null
                return false;
            }
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".svg", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName);
            return allowedImageExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }
        public static string UploadImageWithSpecificFolder(IFormFile file, string FolderPath)
        {
            if (IsImageFile(file))
            {
                if (file == null)
                    throw new ArgumentException("No files provided for upload.");

                string uploadedFileName = "";
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", FolderPath);
                Directory.CreateDirectory(folderPath);


                var fileName = $"{Guid.NewGuid()}-{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(folderPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                return fileName;

            }

            return null;
        }
        //public static async Task<string> AddToContainerAndGetUrl(string connectionString, string containerName, IFormFile file)
        //{
        //    var serviceClient = new BlobServiceClient(connectionString);
        //    var containerClient = serviceClient.GetBlobContainerClient(containerName);

        //    BlobClient blobClient = containerClient.GetBlobClient($"player/{DateTime.Now}-{file.FileName}");
        //    string contentType = file.ContentType;

        //    using (var fileStream = file.OpenReadStream())
        //    {
        //        var blobUploadOptions = new BlobUploadOptions
        //        {
        //            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        //        };
        //        await blobClient.UploadAsync(fileStream, blobUploadOptions);
        //    }

        //    return blobClient.Uri.ToString();
        //}




        //public static bool IsFileValidFormatAndDoesnotExceedMaxSize(IFormFile file, List<string>? allowedExtensions = null)
        //{

        //    // Handle the case where the file is null or its file name is null
        //    if (file == null || file.FileName == null)
        //        return false;


        //    var fileExtension = Path.GetExtension(file.FileName);

        //    allowedExtensions ??= new List<string> { ".jpg", ".jpeg", ".png", ".pdf", ".webp" };

        //    var maxSize = 10 * 1024 * 1024;


        //    if (allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase) && file.Length <= maxSize)
        //        return true;

        //    else
        //        return false;

        //}

        public static async Task<string> AddToContainerAndGetUrl(string connectionString, string containerName, IFormFile file)
        {
            try
            {
                var s3Cred = new BasicAWSCredentials(_accessKey, _secretKey);
                var s3Config = new AmazonS3Config
                {
                    ServiceURL = _endpoint,
                    ForcePathStyle = true,
                    UseHttp = false
                };
                using var s3Client = new AmazonS3Client(s3Cred, s3Config);
                var ext = Path.GetExtension(file.FileName);
                var keyPrefix = "imagedocs/player";
                var objectKey = $"{keyPrefix}/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{ext}";
                await using var stream = file.OpenReadStream();
                var transfer = new TransferUtility(s3Client);
                var uploadReq = new TransferUtilityUploadRequest
                {
                    BucketName = _bucketName,
                    Key = objectKey,
                    InputStream = stream,
                    ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                        ? "application/octet-stream"
                        : file.ContentType,
                };

                await transfer.UploadAsync(uploadReq);

                return objectKey;

            }
            catch (Exception e)
            {
                Log.Error(e, "Error uploading file to S3");
                return string.Empty;
            }
        }

        public static async Task<string> GetPreSignedUrlAsync(string key, int expirationInMinutes = 60)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return null!;

                if (key.StartsWith("http"))
                {
                    //key = ExtractPathInsideBucket(key);
                    key = key.Replace("https://imagesdocs.blob.core.windows.net/buildup", "");
                }
                if (key.Contains(_endpoint))
                    key = ExtractPathInsideBucket(key);

                key = key.TrimStart('/');
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                    Verb = HttpVerb.GET,
                    ResponseHeaderOverrides = new ResponseHeaderOverrides
                    {
                        ContentDisposition = "inline"
                    },

                };
                var s3Cred = new BasicAWSCredentials(_accessKey, _secretKey);
                var s3Config = new AmazonS3Config
                {
                    ServiceURL = _endpoint,
                    ForcePathStyle = true,
                    UseHttp = false
                };
                using var s3Client = new AmazonS3Client(s3Cred, s3Config);
                var url = await s3Client.GetPreSignedURLAsync(request);
                return url;
            }
            catch (Exception ex)
            {

                Log.Error(ex, "Error generating pre-signed URL for S3 object: {Key}", key);
                return null!;
            }
        }
        public static async Task<List<string>> GetPreSignedUrlsAsync(List<string> keys, int expirationInMinutes = 60)
        {
            keys ??= new List<string>();
            var tasks = new List<Task<string>>();
            foreach (var key in keys)
            {
                var url = GetPreSignedUrlAsync(key, expirationInMinutes);
                tasks.Add(url);
            }
            var result = await Task.WhenAll(tasks);
            return result.ToList();
        }

        public static bool IsFileValidFormatAndDoesnotExceedMaxSize(IFormFile file, int maxSize = 15 * 1024 * 1024, List<string>? allowedExtensions = null)
        {

            // Handle the case where the file is null or its file name is null
            if (file == null || file.FileName == null)
                return false;


            var fileExtension = Path.GetExtension(file.FileName);

            allowedExtensions ??= new List<string> { ".jpg", ".jpeg", ".png", ".pdf", ".webp" };



            if (allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase) && file.Length <= maxSize)
                return true;

            else
                return false;

        }
        public static string? ExtractPathInsideBucket(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return null;

            // AbsolutePath example: "/pvc-123/imagedocs/player/file.png"
            var segments = uri.AbsolutePath
                .Split('/', StringSplitOptions.RemoveEmptyEntries);

            // We expect at least: [bucket, ...pathSegments]
            if (segments.Length < 2)
                return null;

            // Skip the bucket segment (segments[0]) and join the rest
            var key = string.Join('/', segments.Skip(1));

            // Decode URL-encoded characters (e.g. %20 -> space)
            key = Uri.UnescapeDataString(key);

            return key;
        }
    }
}
