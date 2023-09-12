using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TestQueryApp;

public partial class EcoStruxtureContext : DbContext
{
    public EcoStruxtureContext()
    {
    }

    public EcoStruxtureContext(DbContextOptions<EcoStruxtureContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Enumeration> Enumerations { get; set; }

    public virtual DbSet<EventDatum> EventData { get; set; }

    public virtual DbSet<EventView> EventViews { get; set; }

    public virtual DbSet<Hyper11Chunk> Hyper11Chunks { get; set; }

    public virtual DbSet<Hyper22Chunk> Hyper22Chunks { get; set; }

    public virtual DbSet<Nspschemachangelog> Nspschemachangelogs { get; set; }

    public virtual DbSet<TbBranche> TbBranches { get; set; }

    public virtual DbSet<TbCity> TbCities { get; set; }

    public virtual DbSet<TbRegion> TbRegions { get; set; }

    public virtual DbSet<TrendDatum> TrendData { get; set; }

    public virtual DbSet<TrendMetum> TrendMeta { get; set; }

    public virtual DbSet<TrendView> TrendViews { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<Versioninfo> Versioninfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=EcoStruxture;Username=admin;Password=srV0rl@nd");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("timescaledb");

        modelBuilder.Entity<Enumeration>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Enumvalue, e.Languageid }).HasName("enumerations_pkey");

            entity.ToTable("enumerations");

            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Enumvalue).HasColumnName("enumvalue");
            entity.Property(e => e.Languageid).HasColumnName("languageid");
            entity.Property(e => e.Enumtext).HasColumnName("enumtext");
        });

        modelBuilder.Entity<EventDatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("event_data");

            entity.HasIndex(e => new { e.Mappedserverguid, e.Timestamp }, "event_data_mappedserverguid_timestamp_idx").IsDescending(false, true);

            entity.HasIndex(e => e.Timestamp, "event_data_timestamp_idx").IsDescending();

            entity.HasIndex(e => e.Aggregatedserverguid, "idx_event_data_aggregatedserverguid");

            entity.HasIndex(e => e.Externalseqno, "idx_event_data_externalseqno");

            entity.HasIndex(e => e.Originatedguid, "idx_event_data_originatedguid");

            entity.HasIndex(e => e.Source, "idx_event_data_source");

            entity.HasIndex(e => e.Sourceserver, "idx_event_data_sourceserver");

            entity.HasIndex(e => e.Uniqueuserid, "idx_event_data_uniqueuserid");

            entity.HasIndex(e => new { e.Mappedserverguid, e.Timestamp, e.Eventguid }, "uk_mappedserverguid_timestamp_eventguid").IsUnique();

            entity.Property(e => e.Acknowledgetime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("acknowledgetime");
            entity.Property(e => e.AcknowledgetimeTzinfo).HasColumnName("acknowledgetime_tzinfo");
            entity.Property(e => e.Aggregatedserverguid).HasColumnName("aggregatedserverguid");
            entity.Property(e => e.Alarmstate).HasColumnName("alarmstate");
            entity.Property(e => e.Alarmtext).HasColumnName("alarmtext");
            entity.Property(e => e.Assignedstate).HasColumnName("assignedstate");
            entity.Property(e => e.Assignedtodomain).HasColumnName("assignedtodomain");
            entity.Property(e => e.Assignedtoname).HasColumnName("assignedtoname");
            entity.Property(e => e.Assignedtouniqueuserid).HasColumnName("assignedtouniqueuserid");
            entity.Property(e => e.Bacneteventtype).HasColumnName("bacneteventtype");
            entity.Property(e => e.Basicevaluationstate).HasColumnName("basicevaluationstate");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Category2).HasColumnName("category2");
            entity.Property(e => e.Changedpropertytype).HasColumnName("changedpropertytype");
            entity.Property(e => e.Command).HasColumnName("command");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Containssignatureinfo).HasColumnName("containssignatureinfo");
            entity.Property(e => e.Controldescr).HasColumnName("controldescr");
            entity.Property(e => e.Controlsource).HasColumnName("controlsource");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.Cwsid).HasColumnName("cwsid");
            entity.Property(e => e.Cwssourceid).HasColumnName("cwssourceid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Devicename).HasColumnName("devicename");
            entity.Property(e => e.Disabledcause).HasColumnName("disabledcause");
            entity.Property(e => e.Domainname).HasColumnName("domainname");
            entity.Property(e => e.Evaluationstate).HasColumnName("evaluationstate");
            entity.Property(e => e.Eventguid).HasColumnName("eventguid");
            entity.Property(e => e.Externalseqno)
                .ValueGeneratedOnAdd()
                .HasColumnName("externalseqno");
            entity.Property(e => e.Firstname).HasColumnName("firstname");
            entity.Property(e => e.Forcedvalue).HasColumnName("forcedvalue");
            entity.Property(e => e.Groupname).HasColumnName("groupname");
            entity.Property(e => e.Hidden).HasColumnName("hidden");
            entity.Property(e => e.Indivndx).HasColumnName("indivndx");
            entity.Property(e => e.Inetaddr).HasColumnName("inetaddr");
            entity.Property(e => e.Ineteventtypeid).HasColumnName("ineteventtypeid");
            entity.Property(e => e.Itemname).HasColumnName("itemname");
            entity.Property(e => e.Lastname).HasColumnName("lastname");
            entity.Property(e => e.Linkname).HasColumnName("linkname");
            entity.Property(e => e.Mappedserverguid).HasColumnName("mappedserverguid");
            entity.Property(e => e.Messagetext).HasColumnName("messagetext");
            entity.Property(e => e.Monitoredvalue).HasColumnName("monitoredvalue");
            entity.Property(e => e.Monitoredvaluetype).HasColumnName("monitoredvaluetype");
            entity.Property(e => e.Monitoredvariable).HasColumnName("monitoredvariable");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.Operationid).HasColumnName("operationid");
            entity.Property(e => e.Originatedguid).HasColumnName("originatedguid");
            entity.Property(e => e.Previousalarmstate).HasColumnName("previousalarmstate");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.Serverguid).HasColumnName("serverguid");
            entity.Property(e => e.Sigmaeventparams).HasColumnName("sigmaeventparams");
            entity.Property(e => e.Sigmaeventtype).HasColumnName("sigmaeventtype");
            entity.Property(e => e.Signature1).HasColumnName("signature1");
            entity.Property(e => e.Signaturecomment).HasColumnName("signaturecomment");
            entity.Property(e => e.Source).HasColumnName("source");
            entity.Property(e => e.Sourcename).HasColumnName("sourcename");
            entity.Property(e => e.Sourceserver).HasColumnName("sourceserver");
            entity.Property(e => e.Stationname).HasColumnName("stationname");
            entity.Property(e => e.Systemalarmid).HasColumnName("systemalarmid");
            entity.Property(e => e.Systemeventid).HasColumnName("systemeventid");
            entity.Property(e => e.Tenantndx).HasColumnName("tenantndx");
            entity.Property(e => e.Textfield).HasColumnName("textfield");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.TimestampTzinfo).HasColumnName("timestamp_tzinfo");
            entity.Property(e => e.Triggeredtimestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("triggeredtimestamp");
            entity.Property(e => e.TriggeredtimestampTzinfo).HasColumnName("triggeredtimestamp_tzinfo");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Uniquealarmid).HasColumnName("uniquealarmid");
            entity.Property(e => e.Uniqueuserid).HasColumnName("uniqueuserid");
            entity.Property(e => e.Unit).HasColumnName("unit");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Valueafter).HasColumnName("valueafter");
            entity.Property(e => e.Valueafterunit).HasColumnName("valueafterunit");
            entity.Property(e => e.Valuebefore).HasColumnName("valuebefore");
            entity.Property(e => e.Valuebeforeunit).HasColumnName("valuebeforeunit");
            entity.Property(e => e.Zone).HasColumnName("zone");
        });

        modelBuilder.Entity<EventView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("event_view");

            entity.Property(e => e.Acknowledgetime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("acknowledgetime");
            entity.Property(e => e.AcknowledgetimeTzinfo).HasColumnName("acknowledgetime_tzinfo");
            entity.Property(e => e.Aggregatedserverguid).HasColumnName("aggregatedserverguid");
            entity.Property(e => e.Alarmstate).HasColumnName("alarmstate");
            entity.Property(e => e.Alarmtext).HasColumnName("alarmtext");
            entity.Property(e => e.Assignedstate).HasColumnName("assignedstate");
            entity.Property(e => e.Assignedtodomain).HasColumnName("assignedtodomain");
            entity.Property(e => e.Assignedtoname).HasColumnName("assignedtoname");
            entity.Property(e => e.Assignedtouniqueuserid).HasColumnName("assignedtouniqueuserid");
            entity.Property(e => e.Bacneteventtype).HasColumnName("bacneteventtype");
            entity.Property(e => e.Basicevaluationstate).HasColumnName("basicevaluationstate");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Category2).HasColumnName("category2");
            entity.Property(e => e.Changedpropertytype).HasColumnName("changedpropertytype");
            entity.Property(e => e.Command).HasColumnName("command");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Containssignatureinfo).HasColumnName("containssignatureinfo");
            entity.Property(e => e.Controldescr).HasColumnName("controldescr");
            entity.Property(e => e.Controlsource).HasColumnName("controlsource");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.Cwsid).HasColumnName("cwsid");
            entity.Property(e => e.Cwssourceid).HasColumnName("cwssourceid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Devicename).HasColumnName("devicename");
            entity.Property(e => e.Disabledcause).HasColumnName("disabledcause");
            entity.Property(e => e.Domainname).HasColumnName("domainname");
            entity.Property(e => e.Evaluationstate).HasColumnName("evaluationstate");
            entity.Property(e => e.Eventguid).HasColumnName("eventguid");
            entity.Property(e => e.Externalseqno).HasColumnName("externalseqno");
            entity.Property(e => e.Firstname).HasColumnName("firstname");
            entity.Property(e => e.Forcedvalue).HasColumnName("forcedvalue");
            entity.Property(e => e.Groupname).HasColumnName("groupname");
            entity.Property(e => e.Hidden).HasColumnName("hidden");
            entity.Property(e => e.Indivndx).HasColumnName("indivndx");
            entity.Property(e => e.Inetaddr).HasColumnName("inetaddr");
            entity.Property(e => e.Ineteventtypeid).HasColumnName("ineteventtypeid");
            entity.Property(e => e.Itemname).HasColumnName("itemname");
            entity.Property(e => e.Lastname).HasColumnName("lastname");
            entity.Property(e => e.Linkname).HasColumnName("linkname");
            entity.Property(e => e.Mappedserverguid).HasColumnName("mappedserverguid");
            entity.Property(e => e.Messagetext).HasColumnName("messagetext");
            entity.Property(e => e.Monitoredvalue).HasColumnName("monitoredvalue");
            entity.Property(e => e.Monitoredvaluetype).HasColumnName("monitoredvaluetype");
            entity.Property(e => e.Monitoredvariable).HasColumnName("monitoredvariable");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.Operationid).HasColumnName("operationid");
            entity.Property(e => e.Originatedguid).HasColumnName("originatedguid");
            entity.Property(e => e.Previousalarmstate).HasColumnName("previousalarmstate");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.Serverguid).HasColumnName("serverguid");
            entity.Property(e => e.Sigmaeventparams).HasColumnName("sigmaeventparams");
            entity.Property(e => e.Sigmaeventtype).HasColumnName("sigmaeventtype");
            entity.Property(e => e.Signature1).HasColumnName("signature1");
            entity.Property(e => e.Signaturecomment).HasColumnName("signaturecomment");
            entity.Property(e => e.Source).HasColumnName("source");
            entity.Property(e => e.Sourcename).HasColumnName("sourcename");
            entity.Property(e => e.Sourceserver).HasColumnName("sourceserver");
            entity.Property(e => e.Stationname).HasColumnName("stationname");
            entity.Property(e => e.Systemalarmid).HasColumnName("systemalarmid");
            entity.Property(e => e.Systemeventid).HasColumnName("systemeventid");
            entity.Property(e => e.Tenantndx).HasColumnName("tenantndx");
            entity.Property(e => e.Textfield).HasColumnName("textfield");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.TimestampTzinfo).HasColumnName("timestamp_tzinfo");
            entity.Property(e => e.Triggeredtimestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("triggeredtimestamp");
            entity.Property(e => e.TriggeredtimestampTzinfo).HasColumnName("triggeredtimestamp_tzinfo");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Uniquealarmid).HasColumnName("uniquealarmid");
            entity.Property(e => e.Uniqueuserid).HasColumnName("uniqueuserid");
            entity.Property(e => e.Unit).HasColumnName("unit");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Valueafter).HasColumnName("valueafter");
            entity.Property(e => e.Valueafterunit).HasColumnName("valueafterunit");
            entity.Property(e => e.Valuebefore).HasColumnName("valuebefore");
            entity.Property(e => e.Valuebeforeunit).HasColumnName("valuebeforeunit");
            entity.Property(e => e.Zone).HasColumnName("zone");
        });

        modelBuilder.Entity<Hyper11Chunk>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("_hyper_1_1_chunk", "_timescaledb_internal");

            entity.HasIndex(e => new { e.Mappedserverguid, e.Timestamp, e.Eventguid }, "1_1_uk_mappedserverguid_timestamp_eventguid").IsUnique();

            entity.HasIndex(e => new { e.Mappedserverguid, e.Timestamp }, "_hyper_1_1_chunk_event_data_mappedserverguid_timestamp_idx").IsDescending(false, true);

            entity.HasIndex(e => e.Timestamp, "_hyper_1_1_chunk_event_data_timestamp_idx").IsDescending();

            entity.HasIndex(e => e.Aggregatedserverguid, "_hyper_1_1_chunk_idx_event_data_aggregatedserverguid");

            entity.HasIndex(e => e.Externalseqno, "_hyper_1_1_chunk_idx_event_data_externalseqno");

            entity.HasIndex(e => e.Originatedguid, "_hyper_1_1_chunk_idx_event_data_originatedguid");

            entity.HasIndex(e => e.Source, "_hyper_1_1_chunk_idx_event_data_source");

            entity.HasIndex(e => e.Sourceserver, "_hyper_1_1_chunk_idx_event_data_sourceserver");

            entity.HasIndex(e => e.Uniqueuserid, "_hyper_1_1_chunk_idx_event_data_uniqueuserid");

            entity.Property(e => e.Acknowledgetime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("acknowledgetime");
            entity.Property(e => e.AcknowledgetimeTzinfo).HasColumnName("acknowledgetime_tzinfo");
            entity.Property(e => e.Aggregatedserverguid).HasColumnName("aggregatedserverguid");
            entity.Property(e => e.Alarmstate).HasColumnName("alarmstate");
            entity.Property(e => e.Alarmtext).HasColumnName("alarmtext");
            entity.Property(e => e.Assignedstate).HasColumnName("assignedstate");
            entity.Property(e => e.Assignedtodomain).HasColumnName("assignedtodomain");
            entity.Property(e => e.Assignedtoname).HasColumnName("assignedtoname");
            entity.Property(e => e.Assignedtouniqueuserid).HasColumnName("assignedtouniqueuserid");
            entity.Property(e => e.Bacneteventtype).HasColumnName("bacneteventtype");
            entity.Property(e => e.Basicevaluationstate).HasColumnName("basicevaluationstate");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Category2).HasColumnName("category2");
            entity.Property(e => e.Changedpropertytype).HasColumnName("changedpropertytype");
            entity.Property(e => e.Command).HasColumnName("command");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Containssignatureinfo).HasColumnName("containssignatureinfo");
            entity.Property(e => e.Controldescr).HasColumnName("controldescr");
            entity.Property(e => e.Controlsource).HasColumnName("controlsource");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.Cwsid).HasColumnName("cwsid");
            entity.Property(e => e.Cwssourceid).HasColumnName("cwssourceid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Devicename).HasColumnName("devicename");
            entity.Property(e => e.Disabledcause).HasColumnName("disabledcause");
            entity.Property(e => e.Domainname).HasColumnName("domainname");
            entity.Property(e => e.Evaluationstate).HasColumnName("evaluationstate");
            entity.Property(e => e.Eventguid).HasColumnName("eventguid");
            entity.Property(e => e.Externalseqno).HasColumnName("externalseqno");
            entity.Property(e => e.Firstname).HasColumnName("firstname");
            entity.Property(e => e.Forcedvalue).HasColumnName("forcedvalue");
            entity.Property(e => e.Groupname).HasColumnName("groupname");
            entity.Property(e => e.Hidden).HasColumnName("hidden");
            entity.Property(e => e.Indivndx).HasColumnName("indivndx");
            entity.Property(e => e.Inetaddr).HasColumnName("inetaddr");
            entity.Property(e => e.Ineteventtypeid).HasColumnName("ineteventtypeid");
            entity.Property(e => e.Itemname).HasColumnName("itemname");
            entity.Property(e => e.Lastname).HasColumnName("lastname");
            entity.Property(e => e.Linkname).HasColumnName("linkname");
            entity.Property(e => e.Mappedserverguid).HasColumnName("mappedserverguid");
            entity.Property(e => e.Messagetext).HasColumnName("messagetext");
            entity.Property(e => e.Monitoredvalue).HasColumnName("monitoredvalue");
            entity.Property(e => e.Monitoredvaluetype).HasColumnName("monitoredvaluetype");
            entity.Property(e => e.Monitoredvariable).HasColumnName("monitoredvariable");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.Operationid).HasColumnName("operationid");
            entity.Property(e => e.Originatedguid).HasColumnName("originatedguid");
            entity.Property(e => e.Previousalarmstate).HasColumnName("previousalarmstate");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.Serverguid).HasColumnName("serverguid");
            entity.Property(e => e.Sigmaeventparams).HasColumnName("sigmaeventparams");
            entity.Property(e => e.Sigmaeventtype).HasColumnName("sigmaeventtype");
            entity.Property(e => e.Signature1).HasColumnName("signature1");
            entity.Property(e => e.Signaturecomment).HasColumnName("signaturecomment");
            entity.Property(e => e.Source).HasColumnName("source");
            entity.Property(e => e.Sourcename).HasColumnName("sourcename");
            entity.Property(e => e.Sourceserver).HasColumnName("sourceserver");
            entity.Property(e => e.Stationname).HasColumnName("stationname");
            entity.Property(e => e.Systemalarmid).HasColumnName("systemalarmid");
            entity.Property(e => e.Systemeventid).HasColumnName("systemeventid");
            entity.Property(e => e.Tenantndx).HasColumnName("tenantndx");
            entity.Property(e => e.Textfield).HasColumnName("textfield");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.TimestampTzinfo).HasColumnName("timestamp_tzinfo");
            entity.Property(e => e.Triggeredtimestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("triggeredtimestamp");
            entity.Property(e => e.TriggeredtimestampTzinfo).HasColumnName("triggeredtimestamp_tzinfo");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Uniquealarmid).HasColumnName("uniquealarmid");
            entity.Property(e => e.Uniqueuserid).HasColumnName("uniqueuserid");
            entity.Property(e => e.Unit).HasColumnName("unit");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Valueafter).HasColumnName("valueafter");
            entity.Property(e => e.Valueafterunit).HasColumnName("valueafterunit");
            entity.Property(e => e.Valuebefore).HasColumnName("valuebefore");
            entity.Property(e => e.Valuebeforeunit).HasColumnName("valuebeforeunit");
            entity.Property(e => e.Zone).HasColumnName("zone");
        });

        modelBuilder.Entity<Hyper22Chunk>(entity =>
        {
            entity.HasKey(e => new { e.Serverguid, e.Externallogid, e.Timestamp, e.Seqno }).HasName("2_3_trend_data_pkey");

            entity.ToTable("_hyper_2_2_chunk", "_timescaledb_internal");

            entity.HasIndex(e => e.Externalseqno, "_hyper_2_2_chunk_idx_trend_data_externalseqno");

            entity.HasIndex(e => new { e.Serverguid, e.Timestamp }, "_hyper_2_2_chunk_trend_data_serverguid_timestamp_idx").IsDescending(false, true);

            entity.HasIndex(e => e.Timestamp, "_hyper_2_2_chunk_trend_data_timestamp_idx").IsDescending();

            entity.Property(e => e.Serverguid).HasColumnName("serverguid");
            entity.Property(e => e.Externallogid).HasColumnName("externallogid");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.Seqno).HasColumnName("seqno");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Edittime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("edittime");
            entity.Property(e => e.EdittimeTzinfo).HasColumnName("edittime_tzinfo");
            entity.Property(e => e.Endtime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("endtime");
            entity.Property(e => e.EndtimeTzinfo).HasColumnName("endtime_tzinfo");
            entity.Property(e => e.Endvalue).HasColumnName("endvalue");
            entity.Property(e => e.Event).HasColumnName("event");
            entity.Property(e => e.Externalseqno).HasColumnName("externalseqno");
            entity.Property(e => e.Logtype).HasColumnName("logtype");
            entity.Property(e => e.Maxvalue).HasColumnName("maxvalue");
            entity.Property(e => e.Meterconstant).HasColumnName("meterconstant");
            entity.Property(e => e.Minvalue).HasColumnName("minvalue");
            entity.Property(e => e.Originalseqno).HasColumnName("originalseqno");
            entity.Property(e => e.Refseqno).HasColumnName("refseqno");
            entity.Property(e => e.Starttime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("starttime");
            entity.Property(e => e.StarttimeTzinfo).HasColumnName("starttime_tzinfo");
            entity.Property(e => e.Startvalue).HasColumnName("startvalue");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TimestampTzinfo).HasColumnName("timestamp_tzinfo");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.Externallog).WithMany(p => p.Hyper22Chunks)
                .HasForeignKey(d => d.Externallogid)
                .HasConstraintName("2_2_fk_trend_data_externallogid");
        });

        modelBuilder.Entity<Nspschemachangelog>(entity =>
        {
            entity.HasKey(e => e.CurrentInternalVersion).HasName("nspschemachangelog_pkey");

            entity.ToTable("nspschemachangelog", tb => tb.HasComment("Table to keep track of schema changes"));

            entity.Property(e => e.CurrentInternalVersion)
                .ValueGeneratedNever()
                .HasComment("Current internal version of the schema")
                .HasColumnName("current_internal_version");
            entity.Property(e => e.MinSupportedInternalVersion)
                .HasComment("Min supported internal version")
                .HasColumnName("min_supported_internal_version");
            entity.Property(e => e.TimeApplied)
                .HasComment("Timestamp when the change was applied")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time_applied");
        });

        modelBuilder.Entity<TbBranche>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tbBranche_pkey");

            entity.ToTable("tbBranche");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(null, null, null, null, true, null)
                .HasColumnName("ID");
            entity.Property(e => e.BranchSquare).HasDefaultValueSql("0.0");
            entity.Property(e => e.Category).HasDefaultValueSql("0");
            entity.Property(e => e.City).HasDefaultValueSql("0");
            entity.Property(e => e.Mvz)
                .HasDefaultValueSql("0")
                .HasColumnName("MVZ");
            entity.Property(e => e.Region).HasDefaultValueSql("1");
            entity.Property(e => e.TradingSquare).HasDefaultValueSql("0.0");
        });

        modelBuilder.Entity<TbCity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Cities_pkey");

            entity.ToTable("tbCities");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<TbRegion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Regions_pkey");

            entity.ToTable("tbRegions");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(null, null, null, null, true, null)
                .HasColumnName("ID");
        });

        modelBuilder.Entity<TrendDatum>(entity =>
        {
            entity.HasKey(e => new { e.Serverguid, e.Externallogid, e.Timestamp, e.Seqno }).HasName("trend_data_pkey");

            entity.ToTable("trend_data");

            entity.HasIndex(e => e.Externalseqno, "idx_trend_data_externalseqno");

            entity.HasIndex(e => new { e.Serverguid, e.Timestamp }, "trend_data_serverguid_timestamp_idx").IsDescending(false, true);

            entity.HasIndex(e => e.Timestamp, "trend_data_timestamp_idx").IsDescending();

            entity.Property(e => e.Serverguid).HasColumnName("serverguid");
            entity.Property(e => e.Externallogid).HasColumnName("externallogid");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.Seqno).HasColumnName("seqno");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Edittime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("edittime");
            entity.Property(e => e.EdittimeTzinfo).HasColumnName("edittime_tzinfo");
            entity.Property(e => e.Endtime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("endtime");
            entity.Property(e => e.EndtimeTzinfo).HasColumnName("endtime_tzinfo");
            entity.Property(e => e.Endvalue).HasColumnName("endvalue");
            entity.Property(e => e.Event).HasColumnName("event");
            entity.Property(e => e.Externalseqno)
                .ValueGeneratedOnAdd()
                .HasColumnName("externalseqno");
            entity.Property(e => e.Logtype).HasColumnName("logtype");
            entity.Property(e => e.Maxvalue).HasColumnName("maxvalue");
            entity.Property(e => e.Meterconstant).HasColumnName("meterconstant");
            entity.Property(e => e.Minvalue).HasColumnName("minvalue");
            entity.Property(e => e.Originalseqno).HasColumnName("originalseqno");
            entity.Property(e => e.Refseqno).HasColumnName("refseqno");
            entity.Property(e => e.Starttime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("starttime");
            entity.Property(e => e.StarttimeTzinfo).HasColumnName("starttime_tzinfo");
            entity.Property(e => e.Startvalue).HasColumnName("startvalue");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TimestampTzinfo).HasColumnName("timestamp_tzinfo");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.Externallog).WithMany(p => p.TrendData)
                .HasForeignKey(d => d.Externallogid)
                .HasConstraintName("fk_trend_data_externallogid");
        });

        modelBuilder.Entity<TrendMetum>(entity =>
        {
            entity.HasKey(e => e.Externallogid).HasName("trend_meta_pkey");

            entity.ToTable("trend_meta");

            entity.HasIndex(e => e.Originatedguid, "idx_trend_meta_originatedguid");

            entity.HasIndex(e => e.Serverguid, "idx_trend_meta_serverguid");

            entity.HasIndex(e => e.Source, "idx_trend_meta_source");

            entity.HasIndex(e => e.Timeadded, "idx_trend_meta_timeadded");

            entity.HasIndex(e => e.Originatedguid, "uk_trend_meta_originatedguid").IsUnique();

            entity.Property(e => e.Externallogid).HasColumnName("externallogid");
            entity.Property(e => e.Cleared).HasColumnName("cleared");
            entity.Property(e => e.Disabled).HasColumnName("disabled");
            entity.Property(e => e.Originatedguid).HasColumnName("originatedguid");
            entity.Property(e => e.Pendingexternalclear).HasColumnName("pendingexternalclear");
            entity.Property(e => e.Pendingexternaldelete).HasColumnName("pendingexternaldelete");
            entity.Property(e => e.Pendingexternaldisable).HasColumnName("pendingexternaldisable");
            entity.Property(e => e.Pendingexternalenable).HasColumnName("pendingexternalenable");
            entity.Property(e => e.Retentionperiod).HasColumnName("retentionperiod");
            entity.Property(e => e.Serverguid).HasColumnName("serverguid");
            entity.Property(e => e.Source).HasColumnName("source");
            entity.Property(e => e.Timeadded)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timeadded");
            entity.Property(e => e.Unit).HasColumnName("unit");
        });

        modelBuilder.Entity<TrendView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("trend_view");

            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Edittime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("edittime");
            entity.Property(e => e.EdittimeTzinfo).HasColumnName("edittime_tzinfo");
            entity.Property(e => e.Endtime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("endtime");
            entity.Property(e => e.EndtimeTzinfo).HasColumnName("endtime_tzinfo");
            entity.Property(e => e.Endvalue).HasColumnName("endvalue");
            entity.Property(e => e.Event).HasColumnName("event");
            entity.Property(e => e.Externallogid).HasColumnName("externallogid");
            entity.Property(e => e.Externalseqno).HasColumnName("externalseqno");
            entity.Property(e => e.Logtype).HasColumnName("logtype");
            entity.Property(e => e.Maxvalue).HasColumnName("maxvalue");
            entity.Property(e => e.Meterconstant).HasColumnName("meterconstant");
            entity.Property(e => e.Minvalue).HasColumnName("minvalue");
            entity.Property(e => e.Originalseqno).HasColumnName("originalseqno");
            entity.Property(e => e.Refseqno).HasColumnName("refseqno");
            entity.Property(e => e.Seqno).HasColumnName("seqno");
            entity.Property(e => e.Serverguid).HasColumnName("serverguid");
            entity.Property(e => e.Starttime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("starttime");
            entity.Property(e => e.StarttimeTzinfo).HasColumnName("starttime_tzinfo");
            entity.Property(e => e.Startvalue).HasColumnName("startvalue");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.TimestampTzinfo).HasColumnName("timestamp_tzinfo");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => new { e.Unitid, e.Categoryid, e.Languageid }).HasName("units_pkey");

            entity.ToTable("units");

            entity.Property(e => e.Unitid).HasColumnName("unitid");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Languageid).HasColumnName("languageid");
            entity.Property(e => e.Categorydisplayname).HasColumnName("categorydisplayname");
            entity.Property(e => e.Conversionfactor).HasColumnName("conversionfactor");
            entity.Property(e => e.Prefixfactor).HasColumnName("prefixfactor");
            entity.Property(e => e.Typesystem).HasColumnName("typesystem");
            entity.Property(e => e.Unitdescription).HasColumnName("unitdescription");
            entity.Property(e => e.Unitdisplayname).HasColumnName("unitdisplayname");
        });

        modelBuilder.Entity<Versioninfo>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Languageid }).HasName("versioninfo_pkey");

            entity.ToTable("versioninfo");

            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Languageid).HasColumnName("languageid");
            entity.Property(e => e.Version).HasColumnName("version");
        });
        modelBuilder.HasSequence("chunk_constraint_name", "_timescaledb_catalog");
        modelBuilder.HasSequence("chunk_copy_operation_id_seq", "_timescaledb_catalog");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
