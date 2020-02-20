using ProtokolyPomiarow.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProtokolyPomiarow.MesurementsClass
{
    [DataContract]
    public class Mesurement
    {
        [DataMember] public int Number { get; set; }
        [DataMember] public string Source { get; set; }
        [DataMember] public string Destination { get; set; }
        [DataMember] public CableType Type { get; set; }
        [DataMember] public int NumberOfWire { get; set; }
        [DataMember] public double Distance { get; set; }
        [DataMember] public int CountOfPig { get; set; }
        [DataMember] public int CountOfWeld { get; set; }
        public double MaxAttenuation { get;  private set; }
        [DataMember] public double RealAttenuation { get; set; }
        [DataMember] public bool PropperValue { get; set; }
        /*public Mesurement()
        {
            Number = -1;
            Source = "";
            Destination = "";
            Type = null;
            NumberOfWire = -1;
            Distance = -1;
            CountOfPig = -1;
            CountOfWeld = -1;
            MaxAttenuation = -1;
            RealAttenuation = -1;
            PropperValue = false;
        }*/

        public Mesurement(int position, string source, string dest, CableType type, int now, double dist, int cop, int cow, double realA)
        {
            Number = position;
            Source = source;
            Destination = dest;
            Type = type;
            NumberOfWire = now;
            Distance = dist;
            CountOfPig = cop;
            CountOfWeld = cow;

            MaxAttenuation = CountOfWeld * MainWindow.activeProject.WeldAttenuation + CountOfPig * MainWindow.activeProject.PigAttenuation + Distance * type.Attenuation;
            RealAttenuation = realA;

            PropperValue = RealAttenuation < MaxAttenuation;
        }

        public void RefreshAttenuation()
        {
            MaxAttenuation = CountOfWeld * MainWindow.activeProject.WeldAttenuation + CountOfPig * MainWindow.activeProject.PigAttenuation + Distance * Type.Attenuation;
            PropperValue = RealAttenuation < MaxAttenuation;
        }
    }
}
