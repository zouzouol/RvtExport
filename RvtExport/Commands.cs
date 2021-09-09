using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using System.IO;
using Newtonsoft.Json;
using ExportLib;

namespace RvtExport
{
    /*导出平面*/
    [Transaction(TransactionMode.Manual)]
    public class CmdExportViewplanToDxf : IExternalCommand
    {
        string _folder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;

            if (null == doc)
            {
                return Result.Failed;
            }

            if (!Util.BrowseDirectory(ref _folder, true))
            {
                return Result.Cancelled;
            }

            IExportPlan.ExportPlans(doc, _folder);

            return Result.Succeeded;
        }
    }

    /*导出边线*/
    [Transaction(TransactionMode.Manual)]
    public class CmdExportBorders : IExternalCommand
    {
        #region SelectFile
        /// <summary>
        /// Store the last user selected output folder
        /// in the current editing session.
        /// </summary>
        static string _output_folder_path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        /// <summary>
        /// Return true is user selects and confirms
        /// output file name and folder.
        /// </summary>
        static bool SelectFile( ref string folder_path, ref string filename)
        {
            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.Title = "Select JSON Output File";
            dlg.Filter = "JSON files|*.json";
            if (null != folder_path && 0 < folder_path.Length)
            {
                dlg.InitialDirectory = folder_path;
            }

            dlg.FileName = filename;
            bool rc = System.Windows.Forms.DialogResult.OK == dlg.ShowDialog();
            if (rc)
            {
                filename = Path.Combine(dlg.InitialDirectory, dlg.FileName);
                folder_path = Path.GetDirectoryName( filename);
            }
            return rc;
        }
        #endregion // SelectFile

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Check that we are in a 3D view.
            View3D view = doc.ActiveView as View3D;
            if (null == view)
            {
                Util.ErrorMsg( "You must be in a 3D view to export.");
                return Result.Failed;
            }

            // Prompt for output filename selection.
            string filename = doc.PathName;
            if (0 == filename.Length)
            {
                filename = doc.Title;
            }

            filename = Path.GetFileName(filename) + "_borders.json";
            if (!SelectFile(ref _output_folder_path, ref filename))
            {
                return Result.Cancelled;
            }

            filename = Path.Combine(_output_folder_path, filename);
            IExportBorder.ExportView3D(doc.ActiveView as View3D, filename);

            return Result.Succeeded;
        }
    }
}