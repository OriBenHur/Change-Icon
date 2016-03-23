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
using System;
using System.IO;
using System.Windows.Forms;

namespace Change_Icon
{
    public class FolderIcon
    {
        private string folderPath = "";
        private string iniPath = "";

        /// <summary>
        /// Creates new FolderIcon object with the path to the target folder.
        /// </summary>
        /// <param name="folderPath">Path to the target folder</param>
        public FolderIcon(string folderPath)
        {
            this.FolderPath = folderPath;
        }


        /// <summary>
        /// Configures a folder in Windows Explorer to display an icon.
        /// </summary>
        /// <param name="iconFilePath">Path to icon file</param>
        /// <param name="infoTip">Text to be displayed in the InfoTip shown by Windows Explorer</param>
        public void CreateFolderIcon(string iconFilePath)
        {
            if (CreateFolder())
            {
                CreateDesktopIniFile(iconFilePath);
                SetIniFileAttributes();
                SetFolderAttributes();
            }
        }


        /// <summary>
        /// Sets the Target Folder path and configures it to display an icon in Windows Explorer.
        /// </summary>
        /// <param name="targetFolderPath">Folder to display with icon</param>
        /// <param name="iconFilePath">Path to icon [-containing] file</param>
        /// <param name="infoTip">Text to be displayed in the InfoTip shown by Windows Explorer</param>
        public void CreateFolderIcon(string targetFolderPath, string iconFilePath)
        {
            this.FolderPath = targetFolderPath;
            this.CreateFolderIcon(iconFilePath);
        }


        /// <summary>FolderPath</summary>
        public string FolderPath
        {
            get { return this.folderPath; }
            set
            {
                folderPath = value;
                if (!this.folderPath.EndsWith("\\"))
                {
                    this.folderPath += "\\";
                }
            }
        }


        /// <summary>
        /// INI Path and Filename
        /// </summary>
        public string IniPath
        {
            get { return iniPath; }
            set { iniPath = value; }
        }


        /// <summary>CreateFolder creates a directory if it does not currently exist.</summary>
        /// <returns>Returns true if successful, false if an error occurred.</returns>
        private bool CreateFolder()
        {
            // Check for a path in the folderPath variable, which we use to 
            // create the folder if it does not exist.
            if (this.FolderPath.Length == 0)
            {
                return false;
            }

            // If the directory exists, then just return true.
            if (Directory.Exists(this.FolderPath))
            {
                return true;
            }

            try
            {
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(this.FolderPath);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Creates the desktop.ini file which points to a .ico file
        /// </summary>
        /// <param name="iconFilePath">Path to icon [-containing] file</param>
        /// <param name="getIconFromDLL">Indicates that the icon is embedded in a DLL (or EXE)</param>
        /// <param name="iconIndex">Index of icon embedded in DLL or EXE; set to zero if getIconFromDLL is false</param>
        /// <param name="infoTip">Text to be displayed in the InfoTip shown by Windows Explorer</param>
        private bool CreateDesktopIniFile(string iconFilePath, bool getIconFromDLL, int iconIndex)
        {
            // check some things that must (or should) be true before we continue...
            // determine if the Folder exists
            if (!Directory.Exists(this.FolderPath))
            {
                return false;
            }

            // determine whether the icon file exists
            if (!File.Exists(iconFilePath))
            {
                return false;
            }

            if (!getIconFromDLL)
            {
                iconIndex = 0;
            }

            // Set path to the desktop.ini file
            this.IniPath = this.FolderPath + "desktop.ini";

            // Write .ini settings to the desktop.ini file

            IniWriter.WriteValue(".ShellClassInfo", "IconResource","" + "," + iconIndex.ToString(), this.IniPath);
            IniWriter.WriteValue("ViewState", "Mode", "", this.IniPath);
            IniWriter.WriteValue("ViewState", "Vid", "", this.IniPath);
            IniWriter.WriteValue("ViewState", "FolderType", "", this.IniPath);

            return true;
        }


        /// <summary>
        /// Creates a desktop.ini file to reference an icon file.
        /// </summary>
        /// <param name="iconFilePath">Path to icon file (.ico)</param>
        /// <param name="infoTip">Text to be displayed in the InfoTip shown by Windows Explorer</param>
        private void CreateDesktopIniFile(string iconFilePath)
        {
            this.CreateDesktopIniFile(iconFilePath, false, 0);
        }


        /// <summary>
        /// Sets the ini file folder's attributes to Hidden and System
        /// </summary>
        private bool SetIniFileAttributes()
        {
            // determine if the Folder exists
            if (!File.Exists(this.IniPath))
            {
                return false;
            }

            // Set ini file attribute to "Hidden"
            if ((File.GetAttributes(this.IniPath) & FileAttributes.Hidden) != FileAttributes.Hidden)
            {
                File.SetAttributes(this.IniPath, File.GetAttributes(this.IniPath) | FileAttributes.Hidden);
            }

            // Set ini file attribute to "System"
            if ((File.GetAttributes(this.IniPath) & FileAttributes.System) != FileAttributes.System)
            {
                File.SetAttributes(this.IniPath, File.GetAttributes(this.IniPath) | FileAttributes.System);
            }

            return true;

        }


        /// <summary>
        /// Sets the folder's attributes to System
        /// </summary>
        private bool SetFolderAttributes()
        {
            // determine if the Folder exists
            if (!Directory.Exists(this.FolderPath))
            {
                return false;
            }

            // Set folder attribute to "System"
            if ((File.GetAttributes(this.FolderPath) & FileAttributes.System) != FileAttributes.System)
            {
                File.SetAttributes(this.FolderPath, File.GetAttributes(this.FolderPath) | FileAttributes.System);
            }

            return true;

        }

    }
}
