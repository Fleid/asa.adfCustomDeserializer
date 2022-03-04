using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using Microsoft.Azure.StreamAnalytics;
using Microsoft.Azure.StreamAnalytics.Serialization;

namespace adfPipelineRunDeserializer;

public class adfPipelineRunDeserializer : StreamDeserializer<AdfPipelineRun>
{
        // streamingDiagnostics is used to write error to diagnostic logs
        private StreamingDiagnostics streamingDiagnostics;

        // Initializes the operator and provides context that is required for publishing diagnostics
        public override void Initialize(StreamingContext streamingContext)
        {
            this.streamingDiagnostics = streamingContext.Diagnostics;
        }

        public override IEnumerable<AdfPipelineRun> Deserialize(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                // each event is sent via a stream - this is not a continuous stream of events - so we can read it entirely
                string payload = streamReader.ReadToEnd();

                // this specific schema encapsulate activities in a top level array called records
                var customEvent = JsonConvert.DeserializeObject<CustomEvent>(payload);
                foreach (AdfPipelineRun pipelineRun in customEvent.Records){
                    if (pipelineRun.RunId is not null) {
                        //This is really an activity, we can output, else we discard
                        yield return pipelineRun;
                    }
                }
            }
        }
    }


public class CustomEvent {
	[JsonProperty("records")]
    public List<AdfPipelineRun> Records {get;set;}
}

// https://docs.microsoft.com/en-us/azure/data-factory/monitor-schema-logs-events#pipeline-run-log-attributes
public class AdfPipelineRun {

	[JsonProperty("Level")]
    public string Level {get;set;}

    [JsonProperty("correlationId")]
    public string CorrelationId {get;set;}

    [JsonProperty("time")]
    public string Time {get;set;}

    [JsonProperty("runId")]
    public string RunId {get;set;}

    [JsonProperty("resourceId")]
    public string ResourceId {get;set;}

    [JsonProperty("category")]
    public string Category {get;set;}

    [JsonProperty("level")]
    public string LevelDescription {get;set;}

    [JsonProperty("operationName")]
    public string OperationName {get;set;}

    [JsonProperty("pipelineName")]
    public string PipelineName {get;set;}

    [JsonProperty("start")]
    public string Start {get;set;}

    [JsonProperty("end")]
    public string End {get;set;}

    [JsonProperty("status")]
    public string Status {get;set;}

    [JsonProperty("location")]
    public string Location {get;set;}

/*-- dynamic types are not yet supported
    [JsonProperty("properties")]
	public dynamic Properties {get;set;}
*/

    [JsonProperty("tags")]
    public string Tags {get;set;}

    [JsonProperty("groupId")]
    public string GroupId {get;set;}

}
