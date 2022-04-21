using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using Microsoft.Azure.StreamAnalytics;
using Microsoft.Azure.StreamAnalytics.Serialization;

namespace adfLogsCustomDeserializer;

public class adfActivityRunDeserializer : StreamDeserializer<AdfActivityRun>
{
        // streamingDiagnostics is used to write error to diagnostic logs
        private StreamingDiagnostics? streamingDiagnostics;

        // Initializes the operator and provides context that is required for publishing diagnostics
        public override void Initialize(StreamingContext streamingContext)
        {
            this.streamingDiagnostics = streamingContext.Diagnostics;
        }

        public override IEnumerable<AdfActivityRun> Deserialize(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var adfActivityRunInputPayload = new AdfActivityRunInputPayload();

                try {
                    // each payload is sent individually via a stream - this is not a continuous stream of events - so we can read it entirely
                    string payload = streamReader.ReadToEnd();
                    // this specific schema encapsulates activities in a top level array called records
                    adfActivityRunInputPayload = JsonConvert.DeserializeObject<AdfActivityRunInputPayload>(payload);
                }
                catch(Exception e){
                    this.streamingDiagnostics!.WriteError(
                        briefMessage: "Custom deserializer - ADF Any Runs - Unable to deserialize the payload as Json with this schema",
                        detailedMessage: e.Message);
                    throw;
                }
                if (adfActivityRunInputPayload is not null) { // Payload was deserialized successfully
                    if (adfActivityRunInputPayload.Records is not null){ // There is a top level array named records, else we discard
                        foreach (AdfActivityRun activityRun in adfActivityRunInputPayload.Records){
                            if (activityRun.ActivityRunId is not null) { // In the array, this item is an activity, else we discard
                                yield return activityRun;
                            }
                        }
                    }
                }
            }
        }
    }

public class AdfActivityRunInputPayload {
	[JsonProperty("records")]
    public List<AdfActivityRun>? Records {get;set;}
}

// https://docs.microsoft.com/en-us/azure/data-factory/monitor-schema-logs-events#activity-run-log-attributes
public class AdfActivityRun {

	[JsonProperty("Level")]
    public string Level {get;set;} = string.Empty;

    [JsonProperty("correlationId")]
    public string CorrelationId {get;set;} = string.Empty;

    [JsonProperty("time")]
    public string Time {get;set;} = string.Empty;

    [JsonProperty("activityRunId")]
    public string ActivityRunId {get;set;} = string.Empty;

    [JsonProperty("pipelineRunId")]
    public string PipelineRunId {get;set;} = string.Empty;

    [JsonProperty("resourceId")]
    public string ResourceId {get;set;} = string.Empty;

    [JsonProperty("category")]
    public string Category {get;set;} = string.Empty;

    [JsonProperty("level")]
    public string LevelDescription {get;set;} = string.Empty;

    [JsonProperty("operationName")]
    public string OperationName {get;set;} = string.Empty;

    [JsonProperty("activityIterationCount")]
    public int ActivityIterationCount {get;set;}

    [JsonProperty("pipelineName")]
    public string PipelineName {get;set;} = string.Empty;

    [JsonProperty("activityName")]
    public string ActivityName {get;set;} = string.Empty;

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
    [JsonProperty("activityType")]
    public string ActivityType {get;set;} = string.Empty;

    [JsonProperty("tags")]
    public string Tags {get;set;} = string.Empty;

    [JsonProperty("recoveryStatus")]
    public string RecoveryStatus {get;set;} = string.Empty;

    [JsonProperty("activityRetryCount")]
    public int ActivityRetryCount {get;set;}

    [JsonProperty("billingResourceId")]
    public string BillingResourceId {get;set;} = string.Empty;

}
