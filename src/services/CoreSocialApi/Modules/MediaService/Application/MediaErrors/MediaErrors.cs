using Application.Common;

namespace MediaService.Application.MediaErrors
{

    public static class MediaErrors
    {
        public static readonly Error NotFound =
            new("media.not_found", "Media was not found.");

        public static readonly Error InvalidFile =
            new("media.invalid_file", "The uploaded file is invalid.");

        public static readonly Error UnsupportedFileType =
            new("media.unsupported_file_type", "The file type is not supported.");

        public static readonly Error FileTooLarge =
            new("media.file_too_large", "The file size exceeds the allowed limit.");

        public static readonly Error UploadNotAllowed =
            new("media.upload_not_allowed", "Upload is not allowed for this media.");
    }
}
