using System;
using System.Drawing;
using System.IO;
using ImageService.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Drawing.Imaging;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        private string m_OutputFolder;                   // The Output Folder
        private int m_thumbnailSize;                      // The Size Of The Thumbnail Size
        private static Regex r = new Regex(":");     // A regex to be used later for fetching image date
        #endregion

        #region C'tor
        /// <summary>
        /// The c'tor for an ImageServiceModal.
        /// </summary>
        /// <param name="m_OutputFolder">The modal's output folder</param>
        /// <param name="m_thumbnailSize">The required thumbnail size for images</param>
        public ImageServiceModal(string m_OutputFolder, int m_thumbnailSize)
        {
            this.m_OutputFolder = m_OutputFolder;
            this.m_thumbnailSize = m_thumbnailSize;
        }
        #endregion

        #region Methods
        /// <summary>
        /// This method adds a new image file to the output folder.
        /// </summary>
        /// <param name="path">The path to the image in the input folder</param>
        /// <param name="result">The AddFile method's result</param>
        /// <returns>A result string</returns>
        public string AddFile(string path, out bool result)
        {
            // Sleep for a bit to minimize attempting to read while files are still being copied
            Thread.Sleep(500);

            // Create the hidden main output folder if it doesn't yet exist
            DirectoryInfo di = Directory.CreateDirectory(this.m_OutputFolder);
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            try
            {
                if (File.Exists(path))
                {
                    // Fetch the date the pic was taken from the pic in the path
                    DateTime dt = new DateTime();

                    // Sleep before progressing to avoid conflicts when getting the DT:
                    Thread.Sleep(20);
                    try
                    {
                        dt = GetTakenDate(path);
                    }
                    catch { }
                    {
                        dt = File.GetCreationTime(path);
                    }

                    // Prepare the path
                    string m = dt.Month.ToString();
                    string y = dt.Year.ToString();
                    string thumbFolderPath = m_OutputFolder + "\\Thumbnails\\" + y + "\\" + m;
                    string actualImgFolderPath = m_OutputFolder + "\\" + y + "\\" + m;

                    // After acquiring month and year, create the subfolders in the output dir:
                    Directory.CreateDirectory(actualImgFolderPath);
                    Directory.CreateDirectory(thumbFolderPath);

                    // Get the filename from the path we got and add it to the 2 paths to work with:
                    string fileName = path.Substring(path.LastIndexOf("\\"));
                    actualImgFolderPath += fileName;
                    thumbFolderPath += fileName;

                    // Create a thumbnail then move the actual image:
                    HandleThumbnail(path, thumbFolderPath);
                    MoveImage(path, actualImgFolderPath);

                    result = false;
                    if (File.Exists(actualImgFolderPath) && File.Exists(thumbFolderPath))
                    {
                        // If both created files exist, set res. to true:
                        result = true;
                    }

                    // And return the result string:
                    string retMsg = "File has been successfully transfered to " + actualImgFolderPath;
                    return retMsg;
                }
                else
                {
                    throw new Exception("Image file does not exist in the specified path.");
                }
            }
            catch (Exception e)
            {
                // Adding has failed, set res. to false and return the exception:
                result = false;
                return e.ToString();
            }
        }

        /// <summary>
        /// This method fetches the picture's date taken from the EXIF.
        /// </summary>
        /// <param name="path">The path to the image.</param>
        /// <returns>a DateTime object</returns>
        public static DateTime GetTakenDate(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                myImage.Dispose();
                return DateTime.Parse(dateTaken);
            }
        }

        /// <summary>
        /// This method moves the image to the dedicated subfolder.
        /// </summary>
        /// <param name="path">The path to the image in the input folder.</param>
        /// <param name="moveTo">The path to the output folder.</param>
        public void MoveImage(string path, string moveTo)
        {
            // Sleep for a bit to avoid conflicts before moving the image:
            Thread.Sleep(10);

            // Check if the file already exists in the moveTo location:
            bool fileExists = false;
            int existingFiles = 0;
            string ext = Path.GetExtension(path);
            string toCheck = Path.GetDirectoryName(moveTo) + "\\" + Path.GetFileNameWithoutExtension(path);

            while (File.Exists(toCheck + ext))
            {
                fileExists = true;
                existingFiles++;
                toCheck = toCheck + "-" + existingFiles.ToString();
            }

            if (fileExists)
            {
                // If the file exists, move it with a different name:
                File.Move(path, toCheck + ext);
            }
            else
            {
                // Move the file normally:
                File.Move(path, moveTo);
            }
        }

        /// <summary>
        /// This method creates a thumbnail and adds it to the output folder.
        /// </summary>
        /// <param name="path">The path to the image in the input folder</param>
        /// <param name="thumbFolderPath">The path to the thumbnail folder</param>
        public void HandleThumbnail(string path, string thumbFolderPath)
        {
            // Create a thumbnail for the image:
            Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            Image img;

            // Safely get a new bitmap:
            using (var tempBitmap = new Bitmap(path))
            {
                img = new Bitmap(tempBitmap);
            }

            // Get the thumbnail and save it:
            Image thumb = img.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, myCallback, IntPtr.Zero);

            // Check if the file already exists in the thumbFolderPath:
            bool fileExists = false;
            int existingFiles = 0;
            string ext = Path.GetExtension(path);
            string toCheck = Path.GetDirectoryName(thumbFolderPath) + "\\" + Path.GetFileNameWithoutExtension(path);

            while (File.Exists(toCheck + ext))
            {
                fileExists = true;
                existingFiles++;
                toCheck = toCheck + "-" + existingFiles.ToString();
            }

            if (fileExists)
            {
                // If it exists, save it with a different name:
                thumb.Save(toCheck + ext);
            }
            else
            {
                // Save it normally:
                thumb.Save(thumbFolderPath);
            }

            // Free the resource:
            thumb.Dispose();
        }

        /// <summary>
        /// This unused method is required for GetThumbnailImage according to MS documentation.
        /// </summary>
        /// <returns>A boolean</returns>
        public bool ThumbnailCallback()
        {
            return false;
        }
    }

    #endregion
}
