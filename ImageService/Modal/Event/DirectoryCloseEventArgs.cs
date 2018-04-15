using System;

namespace ImageService.Modal.Event
{
    public class DirectoryCloseEventArgs : EventArgs
    {
        #region Properties
        public string DirectoryPath { get; set; }
        public string Message { get; set; }              // The Message That goes to the logger
        #endregion

        #region C'tor
        /// <summary>
        /// The constructor of the class.
        /// </summary>
        /// <param name="dirPath">The directory path</param>
        /// <param name="message">Closing message</param>
        public DirectoryCloseEventArgs(string dirPath, string message)
        {
            DirectoryPath = dirPath;                         // Setting the Directory Name
            Message = message;                              // Storing the String
        }
        #endregion
    }
}
