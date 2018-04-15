using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    static class Program
    {
        /// <summary>
        /// The main method of the entire service.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new ImageService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
