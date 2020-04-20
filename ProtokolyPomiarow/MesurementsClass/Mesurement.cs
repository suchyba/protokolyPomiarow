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
        [DataMember] public int? Number { get; set; }
        [DataMember] public string Source { get; set; }
        [DataMember] public string Destination { get; set; }
        [DataMember] public CableType Type { get; set; }
        [DataMember] public int? NumberOfWire { get; set; }
        [DataMember] public double? Distance { get; set; }
        [DataMember] public int? CountOfPig { get; set; }
        [DataMember] public int? CountOfWeld { get; set; }
        public double? MaxAttenuation { get; private set; }
        [DataMember] public double? RealAttenuation { get; set; }
        [DataMember] public Nullable<bool> PropperValue { get; set; }
        [DataMember] public bool IsPropperValueManuallySet { get; set; } = false;

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
        public Mesurement(int position, string source, string dest, CableType type, int now, double dist, int cop, int cow, double realA, bool result) : this(position, source, dest, type, now, dist, cop, cow, realA)
        {
            IsPropperValueManuallySet = true;
            PropperValue = result;
        }
        public Mesurement(Mesurement m, int pos)
        {
            Number = pos;
            Source = m.Source;
            Destination = m.Destination;
            Type = m.Type;
            NumberOfWire = m.NumberOfWire;
            Distance = m.Distance;
            CountOfPig = m.CountOfPig;
            CountOfWeld = m.CountOfWeld;
            RealAttenuation = m.RealAttenuation;
            MaxAttenuation = m.MaxAttenuation;
            PropperValue = m.PropperValue;
        }

        public Mesurement(string label)
        {
            Number = null;
            Source = label;
            Destination = null;
            Type = null;
            NumberOfWire = null;
            Distance = null;
            CountOfPig = null;
            CountOfWeld = null;
            RealAttenuation = null;
            MaxAttenuation = null;
            PropperValue = null;
        }

        public void RefreshAttenuation()
        {
            if (Number != null)
                MaxAttenuation = CountOfWeld * MainWindow.activeProject.WeldAttenuation + CountOfPig * MainWindow.activeProject.PigAttenuation + Distance * Type.Attenuation;
            else
                MaxAttenuation = null;
            if (!IsPropperValueManuallySet && Number != null)
                PropperValue = RealAttenuation < MaxAttenuation;
        }
    }
}
