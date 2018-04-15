namespace ImageService.Controller
{
    public interface IImageController
    {
        /// <summary>
        /// Executes the input command
        /// </summary>
        /// <param name="commandId">Command ID</param>
        /// <param name="args">Command arguments</param>
        /// <param name="result">Boolean result variable</param>
        /// <returns>Command res. message</returns>
        string ExecuteCommand(int commandId, string[] args, out bool result);
    }
}
