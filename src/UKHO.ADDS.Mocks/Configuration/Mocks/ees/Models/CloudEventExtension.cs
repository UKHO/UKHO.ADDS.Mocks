using System.Text.Json.Serialization;
using CloudNative.CloudEvents;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.Ees.Models
{
    public class CloudEventExtension
    {
        public CloudEvent cloudEvent { get; set; }
        public string specVersion { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public Uri? source { get; set; }
        public string id { get; set; } = string.Empty;
        public DateTime time { get; set; }
        public string subject { get; set; } = string.Empty;
        public string dataContentType { get; set; } = string.Empty;
        public Data? data { get; set; }

        [JsonConstructor]
        public CloudEventExtension(          
            string specversion,
            string type,
            Uri source,
            string id,
            DateTime time,
            string subject,
            string datacontenttype,
            Data? data)
        {
            this.cloudEvent = new CloudEvent(CloudEventsSpecVersion.V1_0)
            {
                Data = data,
                DataContentType = datacontenttype,
                Id = id,
                Subject = subject,
                Time = time,
                Type = type,
                Source = source
            };
            this.specVersion = specVersion;
            this.type = type;
            this.source = source;
            this.id = id;
            this.time = time;
            this.subject = subject;
            this.dataContentType = dataContentType;
            this.data = data;
        }
    }

    public class Data
    {
        public string traceId { get; set; } = string.Empty;
        public string dataSetName { get; set; } = string.Empty;
        public string productName { get; set; } = string.Empty;
        public string productType { get; set; } = string.Empty;
        public int editionNumber { get; set; }
        public int updateNumber { get; set; }
        public bool releasable { get; set; }
        public Bundle[] bundle { get; set; } = null!;
        public Status status { get; set; } = null!;
        public Boundingbox boundingBox { get; set; } = null!;
        public S63 s63 { get; set; } = null!;
        public Signature signature { get; set; } = null!;
        public Ancillaryfile[] ancillaryFiles { get; set; } = null!;
    }

    public class Status
    {
        public string statusName { get; set; } = string.Empty;
        public DateTime statusDate { get; set; }
        public bool isNewCell { get; set; }
    }

    public class Boundingbox
    {
        public float northLimit { get; set; }
        public float southLimit { get; set; }
        public float eastLimit { get; set; }
        public float westLimit { get; set; }
    }

    public class S63
    {
        public bool compression { get; set; }
        public string s57Crc { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string hash { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
        public string fileSize { get; set; } = string.Empty;
    }

    public class Signature
    {
        public string name { get; set; } = string.Empty;
        public string hash { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
        public string fileSize { get; set; } = string.Empty;
    }

    public class Bundle
    {
        public string bundleType { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
    }

    public class Ancillaryfile
    {
        public string name { get; set; } = string.Empty;
        public string hash { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
        public string fileSize { get; set; } = string.Empty;
    }
}
