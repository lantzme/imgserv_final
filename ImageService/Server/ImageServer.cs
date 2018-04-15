using System;
using ImageService.Configuration;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Modal.Event;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        public ILoggingService LoggingService { get; set; }
        #endregion

        #region C'tor
        /// <summary>
        /// The constructor of the program.
        /// </summary>
        /// <param name="modalParameters">IModalParameters object</param>
        /// <param name="serverParameters">IImageServerParameters object</param>
        public ImageServer(IModalParameters modalParameters, IImageServerParameters serverParameters)
        {
            CreateLogger();
            CreateHandlers(serverParameters, modalParameters);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create a handler object to every directory.
        /// </summary>
        /// <param name="serverParameters">IImageServerParameters object</param>
        /// <param name="modalParameters">IModalParameters modalParameters</param>
        private void CreateHandlers(IImageServerParameters serverParameters, IModalParameters modalParameters)
        {
            foreach (string dir in serverParameters.Handlers)
            {
                CreateHandlerByDirectory(dir, modalParameters);
            }
        }

        /// <summary>
        /// Create an ILoggingService object instance.
        /// </summary>
        private void CreateLogger()
        {
            LoggingService = new LoggingService();
        }

        /// <summary>
        /// Creates a handler for the input directory.
        /// </summary>
        /// <param name="pathToDir">A path for a dir which requires handling</param>
        /// <param name="modalParameters">An IModalParameters object</param>
        private void CreateHandlerByDirectory(string pathToDir, IModalParameters modalParameters)
        {
            IDirectoryHandler dirHandler = new DirectoyHandler(modalParameters, LoggingService);
            SubscribeHandlerEvents(dirHandler);
            dirHandler.StartHandleDirectory(pathToDir);
        }

        /// <summary>
        /// Subscribe events to the input directory handler.
        /// </summary>
        /// <param name="dirHandler">An IDirectoryHandler instance</param>
        private void SubscribeHandlerEvents(IDirectoryHandler dirHandler)
        {
            // Subscribing a command received function to the command event.
            CommandRecieved += dirHandler.OnCommandRecieved;

            // Subscribing a close handler function to the closing event.
            dirHandler.DirectoryClose += StopHandler;
        }

        /// <summary>
        /// This is a method for stopping a handler.
        /// </summary>
        /// <param name="sender">Raising object</param>
        /// <param name="eventArgs">Event arguments</param>
        private void StopHandler(object sender, DirectoryCloseEventArgs eventArgs)
        {
            var handler = (IDirectoryHandler) sender;

            // Stop the handler:
            CommandRecieved -=  handler.OnCommandRecieved;
        }

        /// <summary>
        /// This is a method for stopping the server.
        /// </summary>
        public void StopServer()
        {
            // Log the stoppage:
            LoggingService.Log("Stopping & closing the server", MessageTypeEnum.INFO);

            // Invoke close command:
            var eventArgs = new CommandRecievedEventArgs( (int) CommandEnum.CloseCommand, null, "");
            CommandRecieved?.Invoke(this, eventArgs);
        }
        #endregion
    }
}