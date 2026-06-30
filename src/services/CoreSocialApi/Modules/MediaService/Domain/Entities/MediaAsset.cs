using Domain.Common;
using MediaService.Domain.Enums;

namespace MediaService.Domain.Entities
{
    public sealed class MediaAsset : AuditableEntity<Guid>
    {
        private MediaAsset()
        {
        }

        public Guid UploadedByUserId { get; private set; }

        public string? OriginalFileName { get; private set; }

        public string? StorageFileName { get; private set; }

        public string? StoragePath { get; private set; }

        public string? PublicUrl { get; private set; }

        public string? ContentType { get; private set; }

        public long? FileSizeInBytes { get; private set; }

        public MediaType MediaType { get; private set; }

        public MediaStatus Status { get; private set; }

        public string? FailureReason { get; private set; }

        public static MediaAsset CreateUploadSession(
            Guid id,
            Guid uploadedByUserId,
            MediaType mediaType,
            DateTimeOffset createdAtUtc)
        {
            return new MediaAsset
            {
                Id = id,
                UploadedByUserId = uploadedByUserId,
                MediaType = mediaType,
                Status = MediaStatus.Pending,
                CreatedAtUtc = createdAtUtc
            };
        }

        public void StartUpload(DateTimeOffset updatedAtUtc)
        {
            if (Status != MediaStatus.Pending)
                throw new InvalidOperationException("Only pending media can start upload.");

            Status = MediaStatus.Uploading;
            UpdatedAtUtc = updatedAtUtc;
        }

        public void MarkUploaded(
            string originalFileName,
            string storageFileName,
            string storagePath,
            string publicUrl,
            string contentType,
            long fileSizeInBytes,
            DateTimeOffset updatedAtUtc)
        {
            if (Status != MediaStatus.Uploading && Status != MediaStatus.Pending)
                throw new InvalidOperationException("Media cannot be marked uploaded from its current status.");

            OriginalFileName = originalFileName;
            StorageFileName = storageFileName;
            StoragePath = storagePath;
            PublicUrl = publicUrl;
            ContentType = contentType;
            FileSizeInBytes = fileSizeInBytes;
            Status = MediaStatus.Ready;
            UpdatedAtUtc = updatedAtUtc;
            FailureReason = null;
        }

        public void MarkFailed(string failureReason, DateTimeOffset updatedAtUtc)
        {
            Status = MediaStatus.Failed;
            FailureReason = failureReason;
            UpdatedAtUtc = updatedAtUtc;
        }

        public void Delete(DateTimeOffset updatedAtUtc)
        {
            if (Status == MediaStatus.Deleted)
                return;

            Status = MediaStatus.Deleted;
            UpdatedAtUtc = updatedAtUtc;
        }
    }
}
