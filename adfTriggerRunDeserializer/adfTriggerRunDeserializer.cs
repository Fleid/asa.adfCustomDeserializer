using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using Microsoft.Azure.StreamAnalytics;
using Microsoft.Azure.StreamAnalytics.Serialization;

namespace adfTriggerRunDeserializer;

public class adfTriggerRunDeserializer : StreamDeserializer<AdfTriggerRun>
{
        // streamingDiagnostics is used to write error to diagnostic logs
        private StreamingDiagnostics streamingDiagnostics;

        // Initializes the operator and provides context that is required for publishing diagnostics
        public override void Initialize(StreamingContext streamingContext)
        {
            this.streamingDiagnostics = streamingContext.Diagnostics;
        }

        public override IEnumerable<AdfTriggerRun> Deserialize(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var customEvent = new CustomEvent();

                try {
                    // each event is sent via a stream - this is not a continuous stream of events - so we can read it entirely
                    string payload = streamReader.ReadToEnd();
                    // this specific schema encapsulate activities in a top level array called records
                    customEvent = JsonConvert.DeserializeObject<CustomEvent>(payload);
                }
                catch(Exception e){
                    this.streamingDiagnostics.WriteError(
                        briefMessage: "Custom deserializer - ADF Any Runs - Unable to open stream as JSON: top level should be a single array named `records` of any kind of runs",
                        detailedMessage: e.Message);
                    throw;
                }

                foreach (AdfTriggerRun triggerRun in customEvent.Records){
                    if (triggerRun.TriggerId is not null) {
                        //This is really an activity, we can output, else we discard
                        yield return triggerRun;
                    }
                }
            }
        }
    }


public class CustomEvent {
	[JsonProperty("records")]
    public List<AdfTriggerRun> Records {get;set;}
}

// https://docs.microsoft.com/en-us/azure/data-factory/monitor-schema-logs-events#trigger-run-log-attributes
public class AdfTriggerRun {

	[JsonProperty("Level")]
    public string Level {get;set;}

    [JsonProperty("correlationId")]
    public string CorrelationId {get;set;}

    [JsonProperty("time")]
    public string Time {get;set;}

    [JsonProperty("triggerId")]
    public string TriggerId {get;set;}

    [JsonProperty("resourceId")]
    public string ResourceId {get;set;}

    [JsonProperty("category")]
    public string Category {get;set;}

    [JsonProperty("level")]
    public string LevelDescription {get;set;}

    [JsonProperty("operationName")]
    public string OperationName {get;set;}

    [JsonProperty("triggerName")]
    public string TriggerName {get;set;}

    [JsonProperty("triggerType")]
    public string TriggerType {get;set;}

    [JsonProperty("triggerEvent")]
    public string TriggerEvent {get;set;}

    [JsonProperty("start")]
    public string Start {get;set;}

    [JsonProperty("status")]
    public string Status {get;set;}

    [JsonProperty("location")]
    public string Location {get;set;}

    [JsonProperty("properties")]
	public Properties Properties {get;set;}

    [JsonProperty("tags")]
    public string Tags {get;set;}

    [JsonProperty("groupId")]
    public string GroupId {get;set;}

    [JsonProperty("meterId")]
    public string MeterId {get;set;}

    [JsonProperty("billingResourceId")]
    public string BillingResourceId {get;set;}
}


public class Properties {

	[JsonProperty("Parameters")]
    public Parameters Parameters {get;set;}

    // Missing properties to be defined if needed

}

public class Parameters {

	[JsonProperty("TriggerTime")]
    public string TriggerTime {get;set;}

	[JsonProperty("ScheduleTime")]
    public string ScheduleTime {get;set;}

	[JsonProperty("triggerObject")]
    public TriggerObject TriggerObject {get;set;}

}

public class TriggerObject {

	[JsonProperty("name")]
    public string Name {get;set;}

	[JsonProperty("startTime")]
    public string StartTime {get;set;}

	[JsonProperty("endTime")]
    public string EndTime {get;set;}

	[JsonProperty("scheduledTime")]
    public string ScheduledTime {get;set;}

	[JsonProperty("trackingId")]
    public string TrackingId {get;set;}

	[JsonProperty("clientTrackingId")]
    public string ClientTrackingId {get;set;}

	[JsonProperty("originHistoryName")]
    public string OriginHistoryName {get;set;}

	[JsonProperty("code")]
    public string Code {get;set;}

	[JsonProperty("status")]
    public string Status {get;set;}

}