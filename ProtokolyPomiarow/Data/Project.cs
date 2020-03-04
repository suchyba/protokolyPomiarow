using ProtokolyPomiarow.MesurementsClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProtokolyPomiarow.Data
{
    [DataContract]
    public class Project
    {
        [DataMember] public double WeldAttenuation { get; set; } = 0.1;
        [DataMember] public double PigAttenuation { get; set; } = 0.25;
        [DataMember] public List<Mesurement> Mesurements { get; private set; } = new List<Mesurement>();
        [DataMember] public List<CableType> CableTypes { get; private set; } = new List<CableType>();
        [DataMember] public string Localization { get; set; } = null;
        [DataMember] public int ProtocolNumber { get; set; }
        [DataMember] public DateTime DocumentDate { get; set; }
        [DataMember] public DateTime MesurementDate { get; set; }
        [DataMember] public string CustomerInfo { get; set; }
        [DataMember] public string ObjectInfo { get; set; }
        [DataMember] public string LightSourceInfo { get; set; }
        [DataMember] public string GaugeInfo { get; set; }
        public Project()
        {
            AddCableType("Jednomod", 0.25);
            AddCableType("Wielomod", 0.3);
        }
        public void AddMesurement(string source, string dest, CableType type, int now, double dist, int cop, int cow, double realA)
        {
            Mesurement m = new Mesurement(Mesurements.Count + 1, source, dest, type, now, dist, cop, cow, realA);
            Mesurements.Add(m);
        }
        public void AddMesurement(Mesurement m)
        {
            Mesurements.Add(new Mesurement(m, Mesurements.Count + 1));
        }
        public void AddCableType(string name, double a)
        {
            CableType c = new CableType(name, a);
            CableTypes.Add(c);
        }
        public void RefreshId()
        {
            int i = 1;
            foreach (var item in Mesurements)
            {
                item.Number = i++;
            }
        }
        public void RefreshAllAttenuation()
        {
            foreach (var mesure in Mesurements)
            {
                mesure.RefreshAttenuation();
            }
        }
        public void LoadNewMesurementsList(List<Mesurement> newList)
        {
            Mesurements.Clear();
            Mesurements = newList;
        }
        public void LoadNewCableTypesList(List<CableType> newList)
        {
            CableTypes.Clear();
            CableTypes = newList;
        }
    }
}
