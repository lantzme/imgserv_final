
using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {
        #region Events
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        #endregion

        #region Methods
        /// <summary>
        /// This method logs a message to the logger.
        /// </summary>
        /// <param name="message">The message to log in string form.</param>
        /// <param name="type">The type of message we inform the logger about.</param>
        public void Log(string message, MessageTypeEnum type)
        {
            var msgEventArgs = new MessageRecievedEventArgs
            {
                Message = message,
                Status = type
            };
            MessageRecieved?.Invoke(this, msgEventArgs);
        }
        #endregion
    }
}
