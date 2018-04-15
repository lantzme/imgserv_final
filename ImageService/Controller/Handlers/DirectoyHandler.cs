using System;
using System.Collections.Generic;
using System.IO;
using ImageService.Configuration;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Modal.Event;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private readonly IImageController _controller;
        private readonly ILoggingService _logger;
        private List<FileSystemWatcher> _watchers;
        private string _pathToDir;
        #endregion

        #region Events
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;
        #endregion

        #region C'tor
        /// <summary>
        /// A c'tor for a DirHandler
        /// </summary>
        /// <param name="modalParameters">IModalParameters object to create a controller</param>
        /// <param name="service">ILoggingService: a logger</param>
        public DirectoyHandler(IModalParameters modalParameters, ILoggingService service)
        {
            _logger = service;
            _controller = new ImageController(modalParameters);
            _watchers = new List<FileSystemWatcher>();
            _logger.Log("A directory handler was constructed", MessageTypeEnum.INFO);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Start handling the given dir.
        /// </summary>
        /// <param name="dirPath">Path to the dir to handle</param>
        public void StartHandleDirectory(string dirPath)
        {
            _pathToDir = dirPath;

            // Support the following file types, create watchers and write to the logger:
            var validFileTypes = new List<string> { "*.jpg", "*.png", "*.gif", "*.bmp" };
            CreateFileSystemWatchersToValidTypes(validFileTypes);
            _logger.Log("Started handling the directory " + _pathToDir,MessageTypeEnum.INFO);
        }

        /// <summary>
        /// Creates a FileSystemWatcher object for each input file type.
        /// </summary>
        /// <param name="types">Collection of valid file types</param>
        private void CreateFileSystemWatchersToValidTypes(IEnumerable<string> types)
        {
            foreach (var type in types)
            {
                CreateNewWatcher(type);
            }
        }

        /// <summary>
        /// Creates a new FileSystemWatcher for the input file type.
        /// </summary>
        /// <param name="type">The type of the file</param>
        private void CreateNewWatcher(string type)
        {
            var watcher = new FileSystemWatcher(_pathToDir, type)
            {
                EnableRaisingEvents = true
            };
            watcher.Created += WatcherCreatedEventHandler;
            _watchers.Add(watcher);
        }

        /// <summary>
        /// This method handles what happens upon recieving a command.
        /// </summary>
        /// <param name="sender">The command sender</param>
        /// <param name="e">The EventArgs recieved</param>
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (IsCloseCommand(e))
            {
                _logger.Log("Close command recieved", MessageTypeEnum.INFO);
                StopHandleDirectory();
            }

            //Not a close command:
            else
            {
                try
                {
                    ExecuteReceivedCommand(e);
                }
                catch (Exception exception)
                {

                    // Write to the logger:
                    _logger.Log($"Error occured while executing the received ommand (commandId:{e.CommandId}).",
                        MessageTypeEnum.WARNING);
                    _logger.Log($"Error content: {exception.Message}.", MessageTypeEnum.FAIL);
                }
            }
        }

        /// <summary>
        /// Returns True if the input argumet is a close command, otherwise False.
        /// </summary>
        /// <param name="e">Command argument object</param>
        /// <returns>Boolean result</returns>
        private static bool IsCloseCommand(CommandRecievedEventArgs e)
        {
            return e.CommandId == (int) CommandEnum.CloseCommand;
        }

        /// <summary>
        /// Execute the given command and inform the log of the result.
        /// </summary>
        /// <param name="e">Command arguments</param>
        private void ExecuteReceivedCommand(CommandRecievedEventArgs e)
        {
            bool commandExecutionResult;
            string message = _controller.ExecuteCommand(e.CommandId, e.Args, out commandExecutionResult);
            // Print the log according to the command result
            _logger.Log(message, commandExecutionResult ? MessageTypeEnum.INFO : MessageTypeEnum.FAIL);
        }

        /// <summary>
        /// Stop handling the given dir.
        /// </summary>
        public void StopHandleDirectory()
        {
            // Stop handling the directory and make sure FileSystemWatcher isn't watching anymore
            var stopDirectoryMessage = "Stopped handling folder:" + _pathToDir;
            var closeEventArgs = new DirectoryCloseEventArgs(_pathToDir, stopDirectoryMessage);
            DirectoryClose?.Invoke(this, closeEventArgs);
            _watchers.Clear();
            // Log it to the logger
            _logger.Log("Stopped handling the directory " + _pathToDir, MessageTypeEnum.INFO);
        }

        /// <summary>
        /// Handles the watcher event.
        /// </summary>
        /// <param name="sender">Raising object</param>
        /// <param name="e">Event arguments</param>
        private void WatcherCreatedEventHandler(object sender, FileSystemEventArgs e)
        {
            string[] commandArgs = { e.FullPath };

            // Prepare the arguments
            var commandRecievedEventArgs = new CommandRecievedEventArgs( (int) CommandEnum.NewFileCommand,
                commandArgs, _pathToDir);

            // Send them to OnCommandRecieved
            OnCommandRecieved(this, commandRecievedEventArgs);
        }
        #endregion
    }
}
