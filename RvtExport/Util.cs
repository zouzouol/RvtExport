using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RvtExport
{
    class Util
    {
        /// <summary>
        /// Prompt user to interactively select a directory.
        /// </summary>
        /// <param name="path">Input initial path and return selected value</param>
        /// <param name="allowCreate">Enable creation of new folder</param>
        /// <returns>True on successful selection</returns>
        public static bool BrowseDirectory(
          ref string path,
          bool allowCreate)
        {
            FolderBrowserDialog browseDlg
              = new FolderBrowserDialog();

            browseDlg.SelectedPath = path;
            browseDlg.ShowNewFolderButton = allowCreate;

            bool rc = (DialogResult.OK
              == browseDlg.ShowDialog());

            if (rc)
            {
                path = browseDlg.SelectedPath;
            }

            return rc;
        }

        const string _caption = "revit_export";

        /// <summary>
        /// Display an error message to the user.
        /// </summary>
        public static void ErrorMsg(string msg)
        {
            Debug.WriteLine(msg);
            MessageBox.Show(msg,
              _caption,
              MessageBoxButtons.OK,
              MessageBoxIcon.Error);
        }

        /// <summary>
        /// Return a string for a real number
        /// formatted to two decimal places.
        /// </summary>
        public static string RealString(double a)
        {
            return a.ToString("0.##");
        }

        /// <summary>
        /// Return a string for an XYZ point
        /// or vector with its coordinates
        /// formatted to two decimal places.
        /// </summary>
        public static string PointString(XYZ p)
        {
            return string.Format("({0},{1},{2})",
              RealString(p.X),
              RealString(p.Y),
              RealString(p.Z));
        }

        /// <summary>
        /// Return an integer value for a Revit Color.
        /// </summary>
        public static int ColorToInt(Color color)
        {
            return ((int)color.Red) << 16
              | ((int)color.Green) << 8
              | (int)color.Blue;
        }


        /// <summary>
        /// Extract a true or false value from the given
        /// string, accepting yes/no, Y/N, true/false, T/F
        /// and 1/0. We are extremely tolerant, i.e., any
        /// value starting with one of the characters y, n,
        /// t or f is also accepted. Return false if no 
        /// valid Boolean value can be extracted.
        /// </summary>
        public static bool GetTrueOrFalse(
          string s,
          out bool val)
        {
            val = false;

            if (s.Equals(Boolean.TrueString,
              StringComparison.OrdinalIgnoreCase))
            {
                val = true;
                return true;
            }
            if (s.Equals(Boolean.FalseString,
              StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (s.Equals("1"))
            {
                val = true;
                return true;
            }
            if (s.Equals("0"))
            {
                return true;
            }
            s = s.ToLower();

            if ('t' == s[0] || 'y' == s[0])
            {
                val = true;
                return true;
            }
            if ('f' == s[0] || 'n' == s[0])
            {
                return true;
            }
            return false;
        }
    }
}
