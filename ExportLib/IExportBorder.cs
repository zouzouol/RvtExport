using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportLib
{
    public class IExportBorder
    {

        /// <summary>
        /// Export a given 3D view to JSON using
        /// our custom exporter context.
        /// </summary>
        public static void ExportView3D(View3D view3d, string filename)
        {
            Document doc = view3d.Document;
            ExportContext context = new ExportContext(doc, filename);
            CustomExporter exporter = new CustomExporter(doc, context);
            exporter.ShouldStopOnError = false;
            exporter.Export(view3d);

            /*test*/
            var clist = context.allCurves;
            if (clist.Count > 0)
            {
                //Transaction trans = new Transaction(doc, "test");
                //trans.Start();
                //foreach (var item in clist)
                //{
                //    item.NewModelCurveExt(doc);
                //}
                //trans.Commit();
            }


            /*test  绘制导出的border数据，用模型线显示*/
            var exportContainer = context._exportContainer;
            var borders = exportContainer.borders;
            List<Line> lines = new List<Line>();
            foreach (var b in borders)
            {
                List<XYZ> pts = new List<XYZ>();
                foreach (var p in b.vertices)
                {
                    XYZ pt = new XYZ(p.Item1 / 304.8, p.Item2 / 304.8, p.Item3 / 304.8);
                    pts.Add(pt);
                }

                for (int i = 0; i < b.index.Count - 1; i += 2)
                {
                    Line line = Line.CreateBound(pts[b.index[i]], pts[b.index[i + 1]]);
                    lines.Add(line);
                }
            }

            if (lines.Count > 0)
            {
                Transaction trans = new Transaction(doc, "test");
                trans.Start();
                foreach (var item in lines)
                {
                    item.NewModelCurveExt(doc);
                }
                trans.Commit();
            }
        }
    }
}
