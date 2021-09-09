using Autodesk.Revit.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportLib
{
    public class IExportPlan
    {
        /// <summary>
        /// Export a given 3D view to JSON using
        /// our custom exporter context.
        /// </summary>
        public static void ExportPlans(Document doc, string folder)
        {
            ExportContainer_Level container = new ExportContainer_Level();
            container.levels = new List<ExportContainer_Level.LevelData>();
            container.metadata = new Metadata();
            container.metadata.type = "level";
            container.metadata.version = 1.0;
            container.metadata.generator = "Revit exporter";

            DXFExportOptions opt = new DXFExportOptions();
            var views = doc.GetViewPlans().Where(f=> f.ViewType == ViewType.FloorPlan);
            var vlist = views.OrderBy(f => f.GenLevel.Elevation).ToList();
            for (int i = 0; i < vlist.Count; i++)
            {
                View v = vlist[i];
                var data = new ExportContainer_Level.LevelData();
                data.levelName = v.Name;
                data.levelMinZ = v.GenLevel.Elevation * 304.8;
                if (i != vlist.Count - 1)
                {
                    View next = vlist[i + 1];
                    data.levelMaxZ = next.GenLevel.Elevation * 304.8;
                }
                else
                {
                    data.levelMaxZ = data.levelMinZ + 3000;
                }

                data.planeDrawing = v.Name + ".dxf";
                container.levels.Add(data);
                doc.Export(folder, v.Name, new List<ElementId>() { v.Id }, opt);
            }


            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            Formatting formatting = Formatting.Indented;
            string myjs = JsonConvert.SerializeObject(container, formatting, settings);
            // Prompt for output filename selection.
            string filename = doc.PathName;
            if (0 == filename.Length)
            {
                filename = doc.Title;
            }
            filename = Path.GetFileName(filename) + "_levels.json";
            filename = Path.Combine(folder, filename);
            File.WriteAllText(filename, myjs);
        }
    }
}
