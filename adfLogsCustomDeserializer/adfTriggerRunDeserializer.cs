using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using Microsoft.Azure.StreamAnalytics;
using Microsoft.Azure.StreamAnalytics.Serialization;

namespace adfLogsCustomDeserializer;

public class adfTriggerRunDeserializer : StreamDeserializer<AdfTriggerRun>
{
        // streamingDiagnostics is used to write error to diagnostic logs
        private StreamingDiagnostics? streamingDiagnostics;

        // Initializes the operator and provides context that is required for publishing diagnostics
        public override void Initialize(StreamingContext streamingContext)
        {
            this.streamingDiagnostics = streamingContext.Diagnostics;
        }

        public override IEnumerable<AdfTriggerRun> Deserialize(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var adfTriggerRunInputPayload = new AdfTriggerRunInputPayload();

                try {
                    // each event is sent via a stream - this is not a continuous stream of events - so we can read it entirely
                    string payload = streamReader.ReadToEnd();
                    // this specific schema encapsulate activities in a top level array called records
                    adfTriggerRunInputPayload = JsonConvert.DeserializeObject<AdfTriggerRunInputPayload>(payload);
                }
                catch(Exception e){
                    this.streamingDiagnostics!.WriteError(
                        briefMessage: "Custom deserializer - ADF Any Runs - Unable to deserialize the payload as Json with this schema",
                        detailedMessage: e.Message);
                    throw;
                }
                if (adfTriggerRunInputPayload is not null) { // Payload was deserialized successfully
                    if (adfTriggerRunInputPayload.Records is not null){ // There is a top level array named records, else we discard
                        foreach (AdfTriggerRun triggerRun in adfTriggerRunInputPayload.Records){
                            if (triggerRun.TriggerId is not null) { // In the array, this item is an activity, else we discard
                                yield return triggerRun;
                            }
                        }
                    }
                }
            }
        }
    }


public class AdfTriggerRunInputPayload {
	[JsonProperty("records")]
    public List<AdfTriggerRun>? Records {get;set;}
}

// https://docs.microsoft.com/en-us/azure/data-factory/monitor-schema-logs-events#trigger-run-log-attributes
public class AdfTriggerRun {

	[JsonProperty("Level")]
    public string Level {get;set;} = string.Empty;

    [JsonProperty("correlationId")]
    public string CorrelationId {get;set;} = string.Empty;

    [JsonProperty("time")]
    public string Time {get;set;} = string.Empty;

    [JsonProperty("triggerId")]
    public string TriggerId {get;set;} = string.Empty;

    [JsonProperty("resourceId")]
    public string ResourceId {get;set;} = string.Empty;

    [JsonProperty("category")]
    public string Category {get;set;} = string.Empty;

    [JsonProperty("level")]
    public string LevelDescription {get;set;} = string.Empty;

    [JsonProperty("operationName")]
    public string OperationName {get;set;} = string.Empty;

    [JsonProperty("triggerName")]
    public string TriggerName {get;set;} = string.Empty;

    [JsonProperty("triggerType")]
    public string TriggerType {get;set;} = string.Empty;

    [JsonProperty("triggerEvent")]
    public string TriggerEvent {get;set;} = string.Empty;

    [JsonProperty("start")]
    public string Start {get;set;} = string.Empty;

    [JsonProperty("status")]
    public string Status {get;set;} = string.Empty;

    [JsonProperty("location")]
    public string Location {get;set;} = string.Empty;

    [JsonProperty("properties")]
	public TriggerProperties? Properties {get;set;}

    [JsonProperty("tags")]
    public string Tags {get;set;} = string.Empty;

    [JsonProperty("groupId")]
    public string GroupId {get;set;} = string.Empty;

    [JsonProperty("meterId")]
    public string MeterId {get;set;} = string.Empty;

    [JsonProperty("billingResourceId")]
    public string BillingResourceId {get;set;} = string.Empty;
}


public class TriggerProperties {

	[JsonProperty("Parameters")]
    public TriggerPropertiesParameters? Parameters {get;set;}

    // Missing properties to be defined if needed

}

public class TriggerPropertiesParameters {

	[JsonProperty("TriggerTime")]
    public string TriggerTime {get;set;} = string.Empty;

	[JsonProperty("ScheduleTime")]
    public string ScheduleTime {get;set;} = string.Empty;

	[JsonProperty("triggerObject")]
    public TriggerObject? TriggerObject {get;set;}

}

public class TriggerObject {

	[JsonProperty("name")]
    public string Name {get;set;} = string.Empty;

	[JsonProperty("startTime")]
    public string StartTime {get;set;} = string.Empty;

	[JsonProperty("endTime")]
    public string EndTime {get;set;} = string.Empty;

	[JsonProperty("scheduledTime")]
    public string ScheduledTime {get;set;} = string.Empty;

	[JsonProperty("trackingId")]
    public string TrackingId {get;set;} = string.Empty;

	[JsonProperty("clientTrackingId")]
    public string ClientTrackingId {get;set;} = string.Empty;

	[JsonProperty("originHistoryName")]
    public string OriginHistoryName {get;set;} = string.Empty;

	[JsonProperty("code")]
    public string Code {get;set;} = string.Empty;

	[JsonProperty("status")]
    public string Status {get;set;} = string.Empty;

}