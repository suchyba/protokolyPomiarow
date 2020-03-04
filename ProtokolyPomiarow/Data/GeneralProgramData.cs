using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProtokolyPomiarow.Data
{
    [DataContract]
    public class GeneralProgramData
    {
        [DataMember] public List<string> Customers { get; private set; }
    }
}
