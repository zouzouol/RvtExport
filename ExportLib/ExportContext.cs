#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Autodesk.Revit.DB;
//using Autodesk.Revit.Utility;
using Newtonsoft.Json;
#endregion // Namespaces

namespace ExportLib
{
    public class ExportContext : IExportContext
    {
        /// <summary>
        /// Scale entire top level BIM object node in JSON
        /// output. A scale of 1.0 will output the model in 
        /// millimetres. Currently we scale it to decimetres
        /// so that a typical model has a chance of fitting 
        /// into a cube with side length 100, i.e. 10 metres.
        /// </summary>
        double _scale_bim = 1.0;

        /// <summary>
        /// Scale applied to each vertex in each individual 
        /// BIM element. This can be used to scale the model 
        /// down from millimetres to metres, e.g.
        /// Currently we stick with millimetres after all
        /// at this level.
        /// </summary>
        double _scale_vertex = 1.0;

        /// <summary>
        /// If true, switch Y and Z coordinate 
        /// and flip X to negative to convert from
        /// Revit coordinate system to standard 3d
        /// computer graphics coordinate system with
        /// Z pointing out of screen, X towards right,
        /// Y up.
        /// </summary>
        bool _switch_coordinates = false;

        string _filename;

        Stack<ElementId> _elementStack = new Stack<ElementId>();
        Stack<Transform> _transformationStack = new Stack<Transform>();


        private List<int> _proceedLinkDocIds = new List<int>();
        // the linkdocument reference count, the key value pair is the total ref count for the link doc and the processed count
        Dictionary<string, KeyValuePair<int, int>> _linkDocumentRefCountDic;
        Stack<Document> _docStack;

        Document _doc
        {
            get
            {
                return _docStack.Peek();
            }
        }

        public string myjs = null;

        Transform CurrentTransform
        {
            get
            {
                return _transformationStack.Peek();
            }
        }

        public ExportContainer_Border _exportContainer;
        public Dictionary<string, VertexLookupInt> borderVertices;//key:element.uuid
        public List<Curve> allCurves = new List<Curve>();

        string _currentElementUid;
        ExportContainer_Border.BorderData _currentBorderData;
        List<int> _currentIndices = new List<int>();
        List<Tuple<int, int>> _currentCurveArray = new List<Tuple<int, int>>();//存储curve的顶点索引，用于判断重复的curve


        VertexLookupInt CurrentVerticesPerElement
        {
            get
            {
                return borderVertices[_currentElementUid];
            }
        }

        public override string ToString()
        {
            return myjs;
        }

        public ExportContext(Document document, string filename)
        {
            //_doc = document;
            _filename = filename;


            _docStack = new Stack<Document>();
            _docStack.Push(document);

            _linkDocumentRefCountDic = new Dictionary<string, KeyValuePair<int, int>>();
        }

        public bool Start()
        {
            _transformationStack.Push(Transform.Identity);
            _exportContainer = new ExportContainer_Border();
            _exportContainer.metadata = new Metadata();
            _exportContainer.metadata.type = "border";
            _exportContainer.metadata.version = 1.0;
            _exportContainer.metadata.generator = "Revit exporter";
            _exportContainer.borders = new List<ExportContainer_Border.BorderData>();
            borderVertices = new Dictionary<string, VertexLookupInt>();

            return true;
        }

        public void Finish()
        {
            JsonSerializerSettings settings
              = new JsonSerializerSettings();

            settings.NullValueHandling
              = NullValueHandling.Ignore;

            Formatting formatting = Formatting.Indented;

            myjs = JsonConvert.SerializeObject(
              _exportContainer, formatting, settings);

            File.WriteAllText(_filename, myjs);
        }

        public void OnPolymesh(PolymeshTopology polymesh)
        {
        }

        public void OnMaterial(MaterialNode node)
        {
        }

        public bool IsCanceled()
        {
            // This method is invoked many 
            // times during the export process.

            return false;
        }

        public void OnRPC(RPCNode node)
        {
            Debug.WriteLine("OnRPC: " + node.NodeName);
            //Asset asset = node.GetAsset();
            //Debug.WriteLine( "OnRPC: Asset:"
            //  + ( ( asset != null ) ? asset.Name : "Null" ) );
        }

        public RenderNodeAction OnViewBegin(ViewNode node)
        {
            Debug.WriteLine("OnViewBegin: "
              + node.NodeName + "(" + node.ViewId.IntegerValue
              + "): LOD: " + node.LevelOfDetail);

            return RenderNodeAction.Proceed;
        }

        public void OnViewEnd(ElementId elementId)
        {
            Debug.WriteLine("OnViewEnd: Id: " + elementId.IntegerValue);
            // Note: This method is invoked even for a view that was skipped.
        }

        public RenderNodeAction OnElementBegin(
          ElementId elementId)
        {
            Element e = _doc.GetElement(elementId);
            //if (e.Category.CategoryType != CategoryType.Model)
            //    return RenderNodeAction.Skip;

            string uid = e.UniqueId;
            if (null == e.Category)
            {
                Debug.WriteLine("\r\n*** Non-category element!\r\n");
                return RenderNodeAction.Skip;
            }
            else
            {
                Debug.WriteLine(string.Format(
                  "OnElementBegin: id {0} category {1} name {2}",
                  elementId.IntegerValue, e.Category.Name, e.Name));
            }

            _elementStack.Push(elementId);

            _currentElementUid = uid;
            if (!borderVertices.ContainsKey(_currentElementUid))
            {
                borderVertices.Add(_currentElementUid, new VertexLookupInt());
            }
            _currentCurveArray = new List<Tuple<int, int>>();
            _currentIndices = new List<int>();

            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd(
          ElementId id)
        {
            // Note: this method is invoked even for 
            // elements that were skipped.

            Element e = _doc.GetElement(id);
            string uid = e.UniqueId;

            if (null == e.Category)
            {
                Debug.WriteLine("\r\n*** Non-category element!\r\n");
                return;
            }
            else
            {
                Debug.WriteLine(string.Format(
                  "OnElementEnd: id {0} category {1} name {2}",
                  id.IntegerValue, e.Category.Name, e.Name));
            }

            var data = new ExportContainer_Border.BorderData();
            data.uuid = _currentElementUid;
            data.vertices = new List<Tuple<long, long, long>>();
            data.index = new List<int>();

            if (CurrentVerticesPerElement.Count > 0)
            {
                foreach (var item in CurrentVerticesPerElement)
                {
                    var pt = item.Key;
                    var tuple = new Tuple<long, long, long>(pt.X, pt.Y, pt.Z);
                    data.vertices.Add(tuple);
                }

                data.index.AddRange(_currentIndices);
                _exportContainer.borders.Add(data);
            }

            _elementStack.Pop();
        }

        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
            // This method is invoked only if the 
            // custom exporter was set to include faces.

            Transform t = CurrentTransform;
            var clist = new List<Curve>();
            var face = node.GetFace();
            if (face != null)
            {
                var edgeloops = face.EdgeLoops;
                foreach (EdgeArray item in edgeloops)
                {
                    foreach (Edge edge in item)
                    {
                        var curve = edge.AsCurve();
                        if (curve != null)
                        {
                            clist.Add(curve);
                        }
                    }
                }
            }

            if (clist.Count > 0)
            {
                foreach (var item in clist)
                {
                    if (item is Line)
                    {
                        var line = item as Line;
                        var s = line.StartPoint();
                        var e = line.EndPoint();
                        s = t.OfPoint(s);
                        e = t.OfPoint(e);
                        var v1 = CurrentVerticesPerElement.AddVertex(new PointInt(s, _switch_coordinates));
                        var v2 = CurrentVerticesPerElement.AddVertex(new PointInt(e, _switch_coordinates));

                        var tuple = new Tuple<int, int>(v1, v2);
                        var tuple2 = new Tuple<int, int>(v2, v1);
                        if (!_currentCurveArray.Contains(tuple) && !_currentCurveArray.Contains(tuple2))
                        {
                            _currentCurveArray.Add(tuple);
                            _currentIndices.Add(v1);
                            _currentIndices.Add(v2);

                            var ct = t.OfCurve(item);
                            if (ct != null)
                                allCurves.Add(ct);
                        }
                    }
                    else if (item is Arc)
                    {
                        var arc = item as Arc;
                        var pts = arc.Tessellate();
                        var plist = new List<int>();
                        if (pts.Count > 0)
                        {
                            foreach (var ap in pts)
                            {
                                var v1 = t.OfPoint(ap);
                                var vp = CurrentVerticesPerElement.AddVertex(new PointInt(v1, _switch_coordinates));
                                plist.Add(vp);
                            }

                            for (int i = 0; i < plist.Count - 1; i++)
                            {
                                _currentIndices.Add(plist[i]);
                                _currentIndices.Add(plist[i + 1]);
                            }
                        }

                        var ct = t.OfCurve(item);
                        if (ct != null)
                            allCurves.Add(ct);
                    }
                }

            }

            Debug.WriteLine("  OnFaceBegin: " + node.NodeName);
            return RenderNodeAction.Proceed;
        }
        static int faceIndex = 0;

        public void OnFaceEnd(FaceNode node)
        {
            // This method is invoked only if the 
            // custom exporter was set to include faces.

            Debug.WriteLine(++faceIndex);
            Debug.WriteLine("  OnFaceEnd: " + node.NodeName);

            // Note: This method is invoked even for faces that were skipped.
        }

        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            Debug.WriteLine("  OnInstanceBegin: " + node.NodeName
              + " symbol: " + node.GetSymbolId().IntegerValue);

            // This method marks the start of processing a family instance

            _transformationStack.Push(CurrentTransform.Multiply(
              node.GetTransform()));

            // We can either skip this instance or proceed with rendering it.
            return RenderNodeAction.Proceed;
        }

        public void OnInstanceEnd(InstanceNode node)
        {
            Debug.WriteLine("  OnInstanceEnd: " + node.NodeName);
            // Note: This method is invoked even for instances that were skipped.
            _transformationStack.Pop();
        }

        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            Debug.WriteLine("  OnLinkBegin: " + node.NodeName + " Document: " + node.GetDocument().Title + ": Id: " + node.GetSymbolId().IntegerValue);


            if (_proceedLinkDocIds.Contains(node.GetSymbolId().IntegerValue))
            {
                Document doc = node.GetDocument();
                if (_linkDocumentRefCountDic.ContainsKey(doc.PathName))
                {
                    if (_linkDocumentRefCountDic[doc.PathName].Key == _linkDocumentRefCountDic[doc.PathName].Value)
                        return RenderNodeAction.Skip;
                }
            }

            _docStack.Push(node.GetDocument());
            _transformationStack.Push(CurrentTransform.Multiply(node.GetTransform()));
            return RenderNodeAction.Proceed;
        }

        public void OnLinkEnd(LinkNode node)
        {
            Debug.WriteLine("  OnLinkEnd: " + node.NodeName);
            // Note: This method is invoked even for instances that were skipped.

            Document doc = node.GetDocument();
            if (_proceedLinkDocIds.Contains(node.GetSymbolId().IntegerValue))
            {
                if (_linkDocumentRefCountDic.ContainsKey(doc.PathName))
                {
                    if (_linkDocumentRefCountDic[doc.PathName].Key == _linkDocumentRefCountDic[doc.PathName].Value)
                        return;
                    else
                        _linkDocumentRefCountDic[doc.PathName] = new KeyValuePair<int, int>(_linkDocumentRefCountDic[doc.PathName].Key, _linkDocumentRefCountDic[doc.PathName].Value + 1);
                }
            }
            else
            {
                if (_linkDocumentRefCountDic.ContainsKey(doc.PathName))
                    _linkDocumentRefCountDic[doc.PathName] = new KeyValuePair<int, int>(_linkDocumentRefCountDic[doc.PathName].Key, _linkDocumentRefCountDic[doc.PathName].Value + 1);
            }


            _docStack.Pop();
            _transformationStack.Pop();

            _proceedLinkDocIds.Add(node.GetSymbolId().IntegerValue);
        }

        public void OnLight(LightNode node)
        {
            Debug.WriteLine("OnLight: " + node.NodeName);
            //Asset asset = node.GetAsset();
            //Debug.WriteLine( "OnLight: Asset:" + ( ( asset != null ) ? asset.Name : "Null" ) );
        }
    }
}
