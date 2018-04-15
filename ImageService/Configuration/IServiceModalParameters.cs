namespace ImageService.Configuration
{
    public interface IModalParameters
    {
        string OutputDir { get; set; }
        int ThumbnailSize { get; set; }
    }
}