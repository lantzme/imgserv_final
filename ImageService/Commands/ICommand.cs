namespace ImageService.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="args">Command arguments</param>
        /// <param name="result">Result variable</param>
        /// <returns>A result string</returns>
        string Execute(string[] args, out bool result);
    }
}
