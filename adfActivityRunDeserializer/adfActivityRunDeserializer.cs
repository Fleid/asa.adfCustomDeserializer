using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using Microsoft.Azure.StreamAnalytics;
using Microsoft.Azure.StreamAnalytics.Serialization;

namespace adfActivityRunDeserializer;

public class adfActivityRunDeserializer : StreamDeserializer<AdfActivityRun>
{
        // streamingDiagnostics is used to write error to diagnostic logs
        private StreamingDiagnostics streamingDiagnostics;

        // Initializes the operator and provides context that is required for publishing diagnostics
        public override void Initialize(StreamingContext streamingContext)
        {
            this.streamingDiagnostics = streamingContext.Diagnostics;
        }

        public override IEnumerable<AdfActivityRun> Deserialize(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                // each event is sent via a stream - this is not a continuous stream of events - so we can read it entirely
                string payload = streamReader.ReadToEnd();

                // this specific schema encapsulate activities in a top level array called records
                var customEvent = JsonConvert.DeserializeObject<CustomEvent>(payload);
                foreach (AdfActivityRun activityRun in customEvent.Records){
                    if (activityRun.ActivityRunId is not null) {
                        //This is really an activity, we can output, else we discard
                        yield return activityRun;
                    }
                }
            }
        }
    }


public class CustomEvent {
	[JsonProperty("records")]
    public List<AdfActivityRun> Records {get;set;}
}

// https://docs.microsoft.com/en-us/azure/data-factory/monitor-schema-logs-events#activity-run-log-attributes
public class AdfActivityRun {

	[JsonProperty("Level")]
    public string Level {get;set;}

    [JsonProperty("correlationId")]
    public string CorrelationId {get;set;}

    [JsonProperty("time")]
    public string Time {get;set;}

    [JsonProperty("activityRunId")]
    public string ActivityRunId {get;set;}

    [JsonProperty("pipelineRunId")]
    public string PipelineRunId {get;set;}

    [JsonProperty("resourceId")]
    public string ResourceId {get;set;}

    [JsonProperty("category")]
    public string Category {get;set;}

    [JsonProperty("level")]
    public string LevelDescription {get;set;}

    [JsonProperty("operationName")]
    public string OperationName {get;set;}

    [JsonProperty("activityIterationCount")]
    public int ActivityIterationCount {get;set;}

    [JsonProperty("pipelineName")]
    public string PipelineName {get;set;}

    [JsonProperty("activityName")]
    public string ActivityName {get;set;}

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
    [JsonProperty("activityType")]
    public string ActivityType {get;set;}


    [JsonProperty("tags")]
    public string Tags {get;set;}

    [JsonProperty("recoveryStatus")]
    public string RecoveryStatus {get;set;}

    [JsonProperty("activityRetryCount")]
    public int ActivityRetryCount {get;set;}

    [JsonProperty("billingResourceId")]
    public string BillingResourceId {get;set;}

}
