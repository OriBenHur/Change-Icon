/*-----------------------------------------------------------------------------
File:           FolderIcon.cs
Copyright:      (c) 2005, Evan Stone, All Rights Reserved
Author:         Evan Stone
Description:    Simple class that demonstrates how to assign an icon to a folder.
Version:        1.0
Date:           January 17, 2005
Comments: 
EULA:           THIS SOURCE CODE MAY NOT BE DISTRIBUTED IN ANY FASHION WITHOUT
                THE PRIOR CONSENT OF THE AUTHOR. THIS SOURCE CODE IS LICENSED 
                “AS IS” WITHOUT WARRANTY AS TO ITS PERFORMANCE AND THE 
                COPYRIGHT HOLDER MAKES NO WARRANTIES OF ANY KIND, EXPRESSED 
                OR IMPLIED, INCLUDING BUT NOT LIMITED TO IMPLIED WARRANTIES 
                OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE. 
                IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, 
                INDIRECT, INCIDENTAL, SPECIAL, PUNITIVE OR CONSEQUENTIAL 
                DAMAGES OR LOST PROFITS, EVEN IF THE END USER HAS OR HAS NOT 
                BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
-----------------------------------------------------------------------------*/

using System.IO;

namespace Change_Icon
{
    public class FolderIcon
    {
        private string _folderPath = "";
        private string _iniPath = "";

        /// <summary>
        /// Creates new FolderIcon object with the path to the target folder.
        /// </summary>
        /// <param name="folderPath">Path to the target folder</param>
        public FolderIcon(string folderPath)
        {
            FolderPath = folderPath;
        }


        /// <summary>
        /// Configures a folder in Windows Explorer to display an icon.
        /// </summary>
        /// <param name="iconFilePath">Path to icon file</param>
        public void CreateFolderIcon(string iconFilePath)
        {
            if (!CreateFolder()) return;
            CreateDesktopIniFile(iconFilePath);
            SetIniFileAttributes();
            SetFolderAttributes();
        }


        /// <summary>
        /// Sets the Target Folder path and configures it to display an icon in Windows Explorer.
        /// </summary>
        /// <param name="targetFolderPath">Folder to display with icon</param>
        /// <param name="iconFilePath">Path to icon [-containing] file</param>
        public void CreateFolderIcon(string targetFolderPath, string iconFilePath)
        {
            FolderPath = targetFolderPath;
            CreateFolderIcon(iconFilePath);
        }


        /// <summary>FolderPath</summary>
        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                _folderPath = value;
                if (!_folderPath.EndsWith("\\"))
                {
                    _folderPath += "\\";
                }
            }
        }


        /// <summary>
        /// INI Path and Filename
        /// </summary>
        public string IniPath
        {
            get { return _iniPath; }
            set { _iniPath = value; }
        }


        /// <summary>CreateFolder creates a directory if it does not currently exist.</summary>
        /// <returns>Returns true if successful, false if an error occurred.</returns>
        private bool CreateFolder()
        {
            // Check for a path in the folderPath variable, which we use to 
            // create the folder if it does not exist.
            if (FolderPath.Length == 0)
            {
                return false;
            }

            // If the directory exists, then just return true.
            if (Directory.Exists(FolderPath))
            {
                return true;
            }

            try
            {
                // Try to create the directory.
                Directory.CreateDirectory(FolderPath);
            }
            catch
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Creates the desktop.ini file which points to a .ico file
        /// </summary>
        /// <param name="iconFilePath">Path to icon [-containing] file</param>
        /// <param name="getIconFromDll">Indicates that the icon is embedded in a DLL (or EXE)</param>
        /// <param name="iconIndex">Index of icon embedded in DLL or EXE; set to zero if getIconFromDLL is false</param>
        private void CreateDesktopIniFile(string iconFilePath, bool getIconFromDll = false, int iconIndex = 0)
        {
            // check some things that must (or should) be true before we continue...
            // determine if the Folder exists
            if (!Directory.Exists(FolderPath))
            {
                return;
            }

            // determine whether the icon file exists
            if (!File.Exists(iconFilePath))
            {
                return;
            }

            if (!getIconFromDll)
            {
                iconIndex = 0;
            }

            // Set path to the desktop.ini file
            IniPath = FolderPath + "desktop.ini";

            // Write .ini settings to the desktop.ini file

            IniWriter.WriteValue(".ShellClassInfo", "IconResource","" + "," + iconIndex, IniPath);
            IniWriter.WriteValue("ViewState", "Mode", "", IniPath);
            IniWriter.WriteValue("ViewState", "Vid", "", IniPath);
            IniWriter.WriteValue("ViewState", "FolderType", "", IniPath);
        }


        /// <summary>
        /// Sets the ini file folder's attributes to Hidden and System
        /// </summary>
        private void SetIniFileAttributes()
        {
            // determine if the Folder exists
            if (!File.Exists(IniPath))
            {
                return;
            }

            // Set ini file attribute to "Hidden"
            if ((File.GetAttributes(IniPath) & FileAttributes.Hidden) != FileAttributes.Hidden)
            {
                File.SetAttributes(IniPath, File.GetAttributes(IniPath) | FileAttributes.Hidden);
            }

            // Set ini file attribute to "System"
            if ((File.GetAttributes(IniPath) & FileAttributes.System) != FileAttributes.System)
            {
                File.SetAttributes(IniPath, File.GetAttributes(IniPath) | FileAttributes.System);
            }
        }


        /// <summary>
        /// Sets the folder's attributes to System
        /// </summary>
        private void SetFolderAttributes()
        {
            // determine if the Folder exists
            if (!Directory.Exists(FolderPath))
            {
                return;
            }

            // Set folder attribute to "System"
            if ((File.GetAttributes(FolderPath) & FileAttributes.System) != FileAttributes.System)
            {
                File.SetAttributes(FolderPath, File.GetAttributes(FolderPath) | FileAttributes.System);
            }
        }
    }
}
