using System;

namespace ImageService.Modal.Event
{
    public class CommandRecievedEventArgs : EventArgs
    {
        #region Properties
        public int CommandId { get; set; }            // The Command ID
        public string[] Args { get; set; }
        public string RequestDirPath { get; set; }  // The Request Directory
        #endregion

        #region C'tor
        /// <summary>
        /// The constructor of the class.
        /// </summary>
        /// <param name="id">Command ID</param>
        /// <param name="args">Command parameters</param>
        /// <param name="path">The directory path</param>
        public CommandRecievedEventArgs(int id, string[] args, string path)
        {
            CommandId = id;
            Args = args;
            RequestDirPath = path;
        }
        #endregion
    }
}
