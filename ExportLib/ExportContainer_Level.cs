using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ExportLib
{
    [DataContract]
    public class ExportContainer_Level : ExportContainerBase
    {
        [DataContract]
        public class LevelData
        {
            [DataMember]
            public string levelName { get; set; }//层名
            [DataMember]
            public double levelMinZ { get; set; }//层底标高
            [DataMember]
            public double levelMaxZ { get; set; }//层顶标高
            [DataMember]
            public string planeDrawing { get; set; }//平面图dxf文件名
        }

        [DataMember]
        public List<LevelData> levels;
    }
}
