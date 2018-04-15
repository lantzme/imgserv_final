namespace ImageService.Configuration
{
    public interface IImageConfiguration : IModalParameters, IImageServerParameters
    {
        string SourceName { get; set; }
        string LogName { get; set; }
    }
}