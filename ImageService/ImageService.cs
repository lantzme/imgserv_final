﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;
using ImageService.Configuration;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Server;

namespace ImageService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    public partial class ImageService : ServiceBase
    {
        #region Members
        private ImageServer _server;
        private ILoggingService _logger;

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
        private IImageConfiguration _configuration;
        private int _eventId = 1;
        #endregion

        #region C'tor
        /// <summary>
        /// The constructor of the class.
        /// </summary>
        public ImageService()
        {
            InitializeComponent();
            InitializeServiceObjects();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Parses the app.config into an IImageConfiguration object.
        /// </summary>
        private void ParseAppConfigFile()
        {
            try
            {
                ConfigurationParser parser = ConfigurationParser.GetParse();
                _configuration = parser.Configuration;
            }
            catch (Exception e)
            {
                _eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Initialize the eventlog object.
        /// </summary>
        private void CreateEventLog()
        {
            string eventSourceName = _configuration.SourceName;
            string logName = _configuration.LogName;
            _eventLog = new EventLog();

            // Create event logs source if it dosen't yet exist:
            if (!EventLog.SourceExists(eventSourceName))
            {
                EventLog.CreateEventSource(eventSourceName, logName);
            }
            _eventLog.Source = eventSourceName;
            _eventLog.Log = logName;
        }

        /// <summary>
        /// Initialize the server objects.
        /// </summary>
        private void InitializeServiceObjects()
        {

            // Parse the config file using the parser, create an event log and a server with a logger:
            ParseAppConfigFile();
            CreateEventLog();
            CreateServer();
            _logger = _server.LoggingService;
        }

        /// <summary>
        /// Create an ImageServer object instance.
        /// </summary>
        private void CreateServer()
        {
            IImageServerParameters serverParameters = _configuration;
            IModalParameters modalParameters = _configuration;
            _server = new ImageServer(modalParameters, serverParameters);
        }

        /// <summary>
        /// Write the input message to the eventlog.
        /// </summary>
        /// <param name="sender">Raising object</param>
        /// <param name="eventArgs">Event args</param>
        private void writeMessage(object sender, MessageRecievedEventArgs eventArgs)
        {
            // Write to the event log:
            _eventLog.WriteEntry(eventArgs.Message);
        }

        /// <summary>
        /// This method starts the service.
        /// </summary>
        /// <param name="args">Service params</param>
        protected override void OnStart(string[] args)
        {
            _eventLog.WriteEntry("In OnStart");
            _logger.MessageRecieved += writeMessage;

            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_START_PENDING,
                dwWaitHint = 100000
            };
            SetServiceStatus(ServiceHandle, ref serviceStatus);
            // Set up a timer to trigger every minute.  
            Timer timer = new Timer { Interval = 60000 };
            // 60 seconds  
            timer.Elapsed += OnTimer;
            timer.Start();

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(ServiceHandle, ref serviceStatus);
        }

        /// <summary>
        /// This method monitors the service.
        /// </summary>
        /// <param name="sender">Raising object</param>
        /// <param name="args">Event arguments</param>
        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.  
            _eventLog.WriteEntry("Monitoring the System", EventLogEntryType.Information, _eventId++);
        }

        /// <summary>
        /// This method stops the service.
        /// </summary>
        protected override void OnStop()
        {
            _eventLog.WriteEntry("In onStop.");
            _server.StopServer();
            _logger.MessageRecieved -= writeMessage;
        }
        #endregion
    }
}