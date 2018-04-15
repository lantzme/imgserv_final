using System.Collections.Generic;
using System.Threading.Tasks;
using ImageService.Commands;
using ImageService.Configuration;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        #region Members
        private Dictionary<int, ICommand> _commandsDict;
        private IImageServiceModal _modal;
        #endregion

        #region C'tor
        /// <summary>
        /// A c'tor for an ImageController.
        /// </summary>
        /// <param name="modalParameters">An IModalParameters object to work with</param>
        public ImageController(IModalParameters modalParameters)
        {
            CreateImageModal(modalParameters);

            // Create the command dictionary which will be used:
            CreateDictionary();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates an IImageServiceModal object instance based on the params.
        /// </summary>
        /// <param name="modalParameters">IModalParameters object</param>
        private void CreateImageModal(IModalParameters modalParameters)
        {
            _modal = new ImageServiceModal(modalParameters.OutputDir, modalParameters.ThumbnailSize);
        }

        /// <summary>
        /// Create a commands dictionary.
        /// </summary>
        private void CreateDictionary()
        {
            ICommand commandValue = new NewFileCommand(_modal);
            int commandKey = (int) CommandEnum.NewFileCommand;

            // Match values and keys:
            _commandsDict = new Dictionary<int, ICommand>()
            {
                {commandKey, commandValue}
            };
        }

        /// <summary>
        /// Execute the command by sending the ID to ExecuteCommandById.
        /// </summary>
        /// <param name="commandId">The command ID</param>
        /// <param name="args">The recieved args</param>
        /// <param name="resultSuccesful">A result boolean</param>
        public string ExecuteCommand(int commandId, string[] args, out bool resultSuccesful)
        {
            Task<CommandResult> task = new Task<CommandResult>(() => ExecuteCommandById(commandId, args));
            task.Start();
            task.Wait();
            CommandResult result = task.Result;
            resultSuccesful = result.IsExecutedSuccessfully;
            return result.Messege;
        }

        /// <summary>
        /// Execute the input command by the given ID.
        /// </summary>
        /// <param name="commandId">Command id to execute</param>
        /// <param name="args">Command arguments</param>
        /// <returns>CommandResult object</returns>
        private CommandResult ExecuteCommandById(int commandId, string[] args)
        {
            bool executionResult;
            string message = _commandsDict[commandId].Execute(args, out executionResult);
            return new CommandResult()
            {
                Messege = message,
                IsExecutedSuccessfully = executionResult
            };
        }
        #endregion
    }
}
