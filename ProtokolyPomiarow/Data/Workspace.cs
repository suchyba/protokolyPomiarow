using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProtokolyPomiarow.Data
{
    [DataContract]
    public class Workspace
    {
        [DataMember] public List<string> Customers { get; private set; } = new List<string>();
        [DataMember] public List<string> Objects { get; private set; } = new List<string>();
        [DataMember] public List<string> LightSources { get; private set; } = new List<string>();
        [DataMember] public List<string> Gauges { get; private set; } = new List<string>();
    }
}
