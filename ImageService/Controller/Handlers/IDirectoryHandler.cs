using System;
using ImageService.Modal.Event;

namespace ImageService.Controller.Handlers
{
    public interface IDirectoryHandler
    {
        #region Events
        /// <summary>
        /// The Event That Notifies that the Directory is being closed
        /// </summary>
        event EventHandler<DirectoryCloseEventArgs> DirectoryClose;
        #endregion

        #region Methods
        /// <summary>
        /// Initializes directory handling.
        /// </summary>
        /// <param name="dirPath">Valid directory path</param>
        void StartHandleDirectory(string dirPath);

        /// <summary>
        /// The Event that will be activated upon new Command
        /// </summary>
        /// <param name="sender">Raising object</param>
        /// <param name="e">Event parameters</param>
        void OnCommandRecieved(object sender, CommandRecievedEventArgs e);

        /// <summary>
        /// Stops handling a directory
        /// </summary>
        void StopHandleDirectory();
        #endregion
    }
}
