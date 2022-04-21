using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using Microsoft.Azure.StreamAnalytics;
using Microsoft.Azure.StreamAnalytics.Serialization;

namespace adfLogsCustomDeserializer;

public class adfPipelineRunDeserializer : StreamDeserializer<AdfPipelineRun>
{
        // streamingDiagnostics is used to write error to diagnostic logs
        private StreamingDiagnostics? streamingDiagnostics;

        // Initializes the operator and provides context that is required for publishing diagnostics
        public override void Initialize(StreamingContext streamingContext)
        {
            this.streamingDiagnostics = streamingContext.Diagnostics;
        }

        public override IEnumerable<AdfPipelineRun> Deserialize(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var adfPipelineRunInputPayload = new AdfPipelineRunInputPayload();

                try {
                    // each event is sent via a stream - this is not a continuous stream of events - so we can read it entirely
                    string payload = streamReader.ReadToEnd();
                    // this specific schema encapsulate activities in a top level array called records
                    adfPipelineRunInputPayload = JsonConvert.DeserializeObject<AdfPipelineRunInputPayload>(payload);
                }
                catch(Exception e){
                    this.streamingDiagnostics!.WriteError(
                        briefMessage: "Custom deserializer - ADF Any Runs - Unable to deserialize the payload as Json with this schema",
                        detailedMessage: e.Message);
                    throw;
                }
                if (adfPipelineRunInputPayload is not null) { // Payload was deserialized successfully
                    if (adfPipelineRunInputPayload.Records is not null){ // There is a top level array named records, else we discard
                        foreach (AdfPipelineRun pipelineRun in adfPipelineRunInputPayload.Records){
                            if (pipelineRun.RunId != string.Empty) { // In the array, this item is an activity, else we discard
                                yield return pipelineRun;
                            }
                        }
                    }
                }
            }
        }
    }


public class AdfPipelineRunInputPayload {
	[JsonProperty("records")]
    public List<AdfPipelineRun>? Records {get;set;}
}

// https://docs.microsoft.com/en-us/azure/data-factory/monitor-schema-logs-events#pipeline-run-log-attributes
public class AdfPipelineRun {

	[JsonProperty("Level")]
    public string Level {get;set;} = string.Empty;

    [JsonProperty("correlationId")]
    public string CorrelationId {get;set;} = string.Empty;

    [JsonProperty("time")]
    public string Time {get;set;} = string.Empty;

    [JsonProperty("runId")]
    public string RunId {get;set;} = string.Empty;

    [JsonProperty("resourceId")]
    public string ResourceId {get;set;} = string.Empty;

    [JsonProperty("category")]
    public string Category {get;set;} = string.Empty;

    [JsonProperty("level")]
    public string LevelDescription {get;set;} = string.Empty;

    [JsonProperty("operationName")]
    public string OperationName {get;set;} = string.Empty;

    [JsonProperty("pipelineName")]
    public string PipelineName {get;set;} = string.Empty;

    [JsonProperty("start")]
    public string Start {get;set;} = string.Empty;

    [JsonProperty("end")]
    public string End {get;set;} = string.Empty;

    [JsonProperty("status")]
    public string Status {get;set;} = string.Empty;

    [JsonProperty("location")]
    public string Location {get;set;} = string.Empty;

/*-- dynamic types are not yet supported
    [JsonProperty("properties")]
	public dynamic Properties {get;set;}
*/

    [JsonProperty("tags")]
    public string Tags {get;set;} = string.Empty;

    [JsonProperty("groupId")]
    public string GroupId {get;set;} = string.Empty;

}
