namespace ImageService.Configuration
{
    public class ImageConfiguration : IImageConfiguration
    {
        public string[] Handlers { get; set; }
        public string OutputDir { get; set; }
        public string SourceName { get; set; }
        public string LogName { get; set; }
        public int ThumbnailSize { get; set; }
    }
}