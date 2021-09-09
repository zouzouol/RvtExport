using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ExportLib
{
    [DataContract]
    public class ExportContainer_Border : ExportContainerBase
    {
        [DataContract]
        public class BorderData
        {
            [DataMember]
            public string uuid { get; set; }

            [DataMember]
            public List<Tuple<long, long, long>> vertices { get; set; } // millimetres

            [DataMember]
            public List<int> index { get; set; }
        }

        [DataMember]
        public List<BorderData> borders;
    }
}
