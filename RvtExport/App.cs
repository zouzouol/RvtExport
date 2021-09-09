#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Reflection;
#endregion

namespace RvtExport
{
    class App : IExternalApplication
    {
        /// <summary>
        /// Add buttons for our command
        /// to the ribbon panel.
        /// </summary>
        void PopulatePanel(RibbonPanel p)
        {
            string path = Assembly.GetExecutingAssembly().Location;

            RibbonItemData i1 = new PushButtonData(
                "RvtCommand", "BorderExport", path, "RvtExport.CmdExportBorders");

            RibbonItemData i2 = new PushButtonData(
                "RvtCommand2", "LevelExport", path, "RvtExport.CmdExportViewplanToDxf");

            p.AddItem(i1);
            p.AddItem(i2);
        }

        public Result OnStartup(UIControlledApplication a)
        {
            PopulatePanel( a.CreateRibbonPanel("Rvt Export"));

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
