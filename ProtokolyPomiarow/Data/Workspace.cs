using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ProtokolyPomiarow.Data
{
    public enum NumeringOption { XX, XX_YYYY};
    [DataContract]
    public class Workspace
    {
        [DataMember] public List<string> Customers { get; private set; } = new List<string>();
        [DataMember] public List<string> Objects { get; private set; } = new List<string>();
        [DataMember] public List<string> LightSources { get; private set; } = new List<string>();
        [DataMember] public List<string> Gauges { get; private set; } = new List<string>();
        [DataMember] public List<string> People { get; private set; } = new List<string>();
        [DataMember] public List<string> Opinions { get; private set; } = new List<string>();
        [DataMember] public int LastProtocolNumber { get; set; } = 0;
        [DataMember] public NumeringOption ProtocolsNumeringOption { get; set; } = NumeringOption.XX_YYYY;
        [DataMember] public List<Project> Projects { get; private set; } = new List<Project>();
        [DataMember] public string LogoImg { get; set; }
    }
}
