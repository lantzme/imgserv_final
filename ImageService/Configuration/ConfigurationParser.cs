﻿using System.Configuration;

namespace ImageService.Configuration
{
    public class ConfigurationParser
    {
        #region Properties
        public IImageConfiguration Configuration { get; }
        private static ConfigurationParser _parser;
        #endregion

        #region C'tor
        /// <summary>
        /// The c'tor for a ConfigParser
        /// </summary>
        public ConfigurationParser()
        {
            string handlerName = ConfigurationManager.AppSettings["Handler"];
            Configuration = new ImageConfiguration
            {
                // Configure according to the following settings in AppConfig:
                Handlers = handlerName.Split(';'),
                LogName = ConfigurationManager.AppSettings["LogName"],
                SourceName = ConfigurationManager.AppSettings["SourceName"],
                OutputDir = ConfigurationManager.AppSettings["OutputDir"],
                ThumbnailSize = int.Parse(ConfigurationManager.AppSettings["ThumbnailSize"])
            };
        }

        /// <summary>
        /// Singleton implementation of the class - crates a parse.
        /// </summary>
        /// <returns>ConfigurationParser object</returns>
        public static ConfigurationParser GetParse()
        {
            return _parser ?? (_parser = new ConfigurationParser());
        }
        #endregion
    }
}