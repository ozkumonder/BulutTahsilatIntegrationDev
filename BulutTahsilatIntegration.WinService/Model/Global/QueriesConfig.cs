using System.Collections.Generic;
using System.Xml.Serialization;

namespace BulutTahsilatIntegration.WinService.Model.Global
{
    [XmlRoot(ElementName = "Queries")]
    public class QueriesConfig
    {
        [XmlElement(ElementName = "Query")]
        public List<Query> Query { get; set; }
    }

    [XmlRoot(ElementName = "Query")]
    public class Query
    {
        [XmlElement(ElementName = "sql")]
        public string Sql { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }
}
