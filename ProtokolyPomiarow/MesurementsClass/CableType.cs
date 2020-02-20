using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProtokolyPomiarow.MesurementsClass
{
    [DataContract]
    public class CableType
    {
        [DataMember] public double Attenuation { get; set; }
        [DataMember] public string Name { get; private set; }
        public override string ToString()
        {
            return Name;
        }
        public CableType(string name, double attenuation)
        {
            Name = name;
            Attenuation = attenuation;
        }
        public CableType()
        {

        }
    }
}
