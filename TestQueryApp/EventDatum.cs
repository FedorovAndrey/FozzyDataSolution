using System;
using System.Collections.Generic;

namespace TestQueryApp;

public partial class EventDatum
{
    public long Externalseqno { get; set; }

    public Guid Mappedserverguid { get; set; }

    public DateTime? Acknowledgetime { get; set; }

    public short? AcknowledgetimeTzinfo { get; set; }

    public Guid? Aggregatedserverguid { get; set; }

    public short? Alarmstate { get; set; }

    public string? Alarmtext { get; set; }

    public short? Assignedstate { get; set; }

    public string? Assignedtodomain { get; set; }

    public string? Assignedtoname { get; set; }

    public string? Assignedtouniqueuserid { get; set; }

    public int? Bacneteventtype { get; set; }

    public bool? Basicevaluationstate { get; set; }

    public string? Cwsid { get; set; }

    public string? Cwssourceid { get; set; }

    public string? Category { get; set; }

    public string? Category2 { get; set; }

    public string? Changedpropertytype { get; set; }

    public string? Command { get; set; }

    public string? Comment { get; set; }

    public string? Controldescr { get; set; }

    public string? Controlsource { get; set; }

    public int? Count { get; set; }

    public string? Description { get; set; }

    public string? Devicename { get; set; }

    public short? Disabledcause { get; set; }

    public string? Domainname { get; set; }

    public short? Evaluationstate { get; set; }

    public Guid Eventguid { get; set; }

    public string? Firstname { get; set; }

    public string? Forcedvalue { get; set; }

    public string? Groupname { get; set; }

    public bool? Hidden { get; set; }

    public int? Indivndx { get; set; }

    public string? Inetaddr { get; set; }

    public short? Ineteventtypeid { get; set; }

    public string? Itemname { get; set; }

    public string? Lastname { get; set; }

    public string? Linkname { get; set; }

    public string? Messagetext { get; set; }

    public string? Monitoredvalue { get; set; }

    public string? Monitoredvaluetype { get; set; }

    public string? Monitoredvariable { get; set; }

    public string? Note { get; set; }

    public Guid? Originatedguid { get; set; }

    public short? Previousalarmstate { get; set; }

    public int? Priority { get; set; }

    public Guid? Serverguid { get; set; }

    public string? Sigmaeventparams { get; set; }

    public short? Sigmaeventtype { get; set; }

    public string? Source { get; set; }

    public string? Sourcename { get; set; }

    public string? Sourceserver { get; set; }

    public string? Stationname { get; set; }

    public long? Systemalarmid { get; set; }

    public long? Systemeventid { get; set; }

    public int? Type { get; set; }

    public short? Tenantndx { get; set; }

    public string? Textfield { get; set; }

    public DateTime Timestamp { get; set; }

    public short TimestampTzinfo { get; set; }

    public DateTime? Triggeredtimestamp { get; set; }

    public short? TriggeredtimestampTzinfo { get; set; }

    public Guid? Uniquealarmid { get; set; }

    public string? Uniqueuserid { get; set; }

    public long? Unit { get; set; }

    public string? Username { get; set; }

    public string? Valueafter { get; set; }

    public string? Valuebefore { get; set; }

    public short? Zone { get; set; }

    public string? Signature1 { get; set; }

    public string? Signaturecomment { get; set; }

    public bool? Containssignatureinfo { get; set; }

    public long? Valuebeforeunit { get; set; }

    public long? Valueafterunit { get; set; }

    public string? Operationid { get; set; }
}
