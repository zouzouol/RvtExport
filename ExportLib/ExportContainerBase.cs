using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ExportLib
{
    [DataContract]
    public class Metadata
    {
        [DataMember]
        public string type { get; set; } //  "level"
        [DataMember]
        public double version { get; set; } // 1.0
        [DataMember]
        public string generator { get; set; } //  "Revit exporter"
    }

    [DataContract]
    public class ExportContainerBase
    {

        [DataMember]
        public Metadata metadata { get; set; }
    }
}
