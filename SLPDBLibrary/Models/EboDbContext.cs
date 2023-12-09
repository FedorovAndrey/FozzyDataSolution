using Microsoft.EntityFrameworkCore;

namespace SLPDBLibrary.Models;

public partial class EboDbContext : DbContext
{
    public EboDbContext()
    {
    }

    public EboDbContext(DbContextOptions<EboDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Enumeration> Enumerations { get; set; }

    public virtual DbSet<EventDatum> EventData { get; set; }

    public virtual DbSet<EventView> EventViews { get; set; }

    public virtual DbSet<Hyper210Chunk> Hyper210Chunks { get; set; }

    public virtual DbSet<Hyper21Chunk> Hyper21Chunks { get; set; }

    public virtual DbSet<Hyper22Chunk> Hyper22Chunks { get; set; }

    public virtual DbSet<Hyper23Chunk> Hyper23Chunks { get; set; }

    public virtual DbSet<Hyper24Chunk> Hyper24Chunks { get; set; }

    public virtual DbSet<Hyper25Chunk> Hyper25Chunks { get; set; }

    public virtual DbSet<Hyper26Chunk> Hyper26Chunks { get; set; }

    public virtual DbSet<Hyper27Chunk> Hyper27Chunks { get; set; }

    public virtual DbSet<Hyper28Chunk> Hyper28Chunks { get; set; }

    public virtual DbSet<Hyper29Chunk> Hyper29Chunks { get; set; }

    public virtual DbSet<Nspschemachangelog> Nspschemachangelogs { get; set; }

    public virtual DbSet<TbBranch> TbBranches { get; set; }

    public virtual DbSet<TbBranchSquare> TbBranchSquares { get; set; }

    public virtual DbSet<TbCategory> TbCategories { get; set; }

    public virtual DbSet<TbCity> TbCities { get; set; }

    public virtual DbSet<TbClimate> TbClimates { get; set; }

    public virtual DbSet<TbEmployee> TbEmployees { get; set; }

    public virtual DbSet<TbMeter> TbMeters { get; set; }

    public virtual DbSet<TbMeterRole> TbMeterRoles { get; set; }

    public virtual DbSet<TbMeterType> TbMeterTypes { get; set; }

    public virtual DbSet<TbRegion> TbRegions { get; set; }

    public virtual DbSet<TbTraidingSquare> TbTraidingSquares { get; set; }

    public virtual DbSet<TbTypeOfHeating> TbTypeOfHeatings { get; set; }

    public virtual DbSet<TrendDatum> TrendData { get; set; }

    public virtual DbSet<TrendMetum> TrendMeta { get; set; }

    public virtual DbSet<TrendView> TrendViews { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<Versioninfo> Versioninfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=sfpv-pgdb015;Port=5432;Database=ebo_db;Username=ebo_user;Password=too8er4vohn8Zooc",
            builder => {
                builder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(10), null);
            });
        


    }

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

        modelBuilder.Entity<Hyper210Chunk>(entity =>
        {
            entity.HasKey(e => new { e.Serverguid, e.Externallogid, e.Timestamp, e.Seqno }).HasName("10_20_trend_data_pkey");

            entity.ToTable("_hyper_2_10_chunk", "_timescaledb_internal");

            entity.HasIndex(e => e.Externalseqno, "_hyper_2_10_chunk_idx_trend_data_externalseqno");

            entity.HasIndex(e => new { e.Serverguid, e.Timestamp }, "_hyper_2_10_chunk_trend_data_serverguid_timestamp_idx").IsDescending(false, true);

            entity.HasIndex(e => e.Timestamp, "_hyper_2_10_chunk_trend_data_timestamp_idx").IsDescending();

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

            entity.HasOne(d => d.Externallog).WithMany(p => p.Hyper210Chunks)
                .HasForeignKey(d => d.Externallogid)
                .HasConstraintName("10_19_fk_trend_data_externallogid");
        });

        modelBuilder.Entity<Hyper21Chunk>(entity =>
        {
            entity.HasKey(e => new { e.Serverguid, e.Externallogid, e.Timestamp, e.Seqno }).HasName("1_2_trend_data_pkey");

            entity.ToTable("_hyper_2_1_chunk", "_timescaledb_internal");

            entity.HasIndex(e => e.Externalseqno, "_hyper_2_1_chunk_idx_trend_data_externalseqno");

            entity.HasIndex(e => new { e.Serverguid, e.Timestamp }, "_hyper_2_1_chunk_trend_data_serverguid_timestamp_idx").IsDescending(false, true);

            entity.HasIndex(e => e.Timestamp, "_hyper_2_1_chunk_trend_data_timestamp_idx").IsDescending();

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

            entity.HasOne(d => d.Externallog).WithMany(p => p.Hyper21Chunks)
                .HasForeignKey(d => d.Externallogid)
                .HasConstraintName("1_1_fk_trend_data_externallogid");
        });

        modelBuilder.Entity<Hyper22Chunk>(entity =>
        {
            entity.HasKey(e => new { e.Serverguid, e.Externallogid, e.Timestamp, e.Seqno }).HasName("2_4_trend_data_pkey");

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
                .HasConstraintName("2_3_fk_trend_data_externallogid");
        });

        modelBuilder.Entity<Hyper23Chunk>(entity =>
        {
            entity.HasKey(e => new { e.Serverguid, e.Externallogid, e.Timestamp, e.Seqno }).HasName("3_6_trend_data_pkey");

            entity.ToTable("_hyper_2_3_chunk", "_timescaledb_internal");

            entity.HasIndex(e => e.Externalseqno, "_hyper_2_3_chunk_idx_trend_data_externalseqno");

            entity.HasIndex(e => new { e.Serverguid, e.Timestamp }, "_hyper_2_3_chunk_trend_data_serverguid_timestamp_idx").IsDescending(false, true);

            entity.HasIndex(e => e.Timestamp, "_hyper_2_3_chunk_trend_data_timestamp_idx").IsDescending();

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

            entity.HasOne(d => d.Externallog).WithMany(p => p.Hyper23Chunks)
                .HasForeignKey(d => d.Externallogid)
                .HasConstraintName("3_5_fk_trend_data_externallogid");
        });

        modelBuilder.Entity<Hyper24Chunk>(entity =>
        {
            entity.HasKey(e => new { e.Serverguid, e.Externallogid, e.Timestamp, e.Seqno }).HasName("4_8_trend_data_pkey");

            entity.ToTable("_hyper_2_4_chunk", "_timescaledb_internal");

            entity.HasIndex(e => e.Externalseqno, "_hyper_2_4_chunk_idx_trend_data_externalseqno");

            entity.HasIndex(e => new { e.Serverguid, e.Timestamp }, "_hyper_2_4_chunk_trend_data_serverguid_timestamp_idx").IsDescending(false, true);

            entity.HasIndex(e => e.Timestamp, "_hyper_2_4_chunk_trend_data_timestamp_idx").IsDescending();

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

            entity.HasOne(d => d.Externallog).WithMany(p => p.Hyper24Chunks)
                .HasForeignKey(d => d.Externallogid)
                .HasConstraintName("4_7_fk_trend_data_externallogid");
        });

        modelBuilder.Entity<Hyper25Chunk>(entity =>
        {
            entity.HasKey(e => new { e.Serverguid, e.Externallogid, e.Timestamp, e.Seqno }).HasName("5_10_trend_data_pkey");

            entity.ToTable("_hyper_2_5_chunk", "_timescaledb_internal");

            entity.HasIndex(e => e.Externalseqno, "_hyper_2_5_chunk_idx_trend_data_externalseqno");

            entity.HasIndex(e => new { e.Serverguid, e.Timestamp }, "_hyper_2_5_chunk_trend_data_serverguid_timestamp_idx").IsDescending(false, true);

            entity.HasIndex(e => e.Timestamp, "_hyper_2_5_chunk_trend_data_timestamp_idx").IsDescending();

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

            entity.HasOne(d => d.Externallog).WithMany(p => p.Hyper25Chunks)
                .HasForeignKey(d => d.Externallogid)
                .HasConstraintName("5_9_fk_trend_data_externallogid");
        });

        modelBuilder.Entity<Hyper26Chunk>(entity =>
        {
            entity.HasKey(e => new { e.Serverguid, e.Externallogid, e.Timestamp, e.Seqno }).HasName("6_12_trend_data_pkey");

            entity.ToTable("_hyper_2_6_chunk", "_timescaledb_internal");

            entity.HasIndex(e => e.Externalseqno, "_hyper_2_6_chunk_idx_trend_data_externalseqno");

            entity.HasIndex(e => new { e.Serverguid, e.Timestamp }, "_hyper_2_6_chunk_trend_data_serverguid_timestamp_idx").IsDescending(false, true);

            entity.HasIndex(e => e.Timestamp, "_hyper_2_6_chunk_trend_data_timestamp_idx").IsDescending();

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

            entity.HasOne(d => d.Externallog).WithMany(p => p.Hyper26Chunks)
                .HasForeignKey(d => d.Externallogid)
                .HasConstraintName("6_11_fk_trend_data_externallogid");
        });

        modelBuilder.Entity<Hyper27Chunk>(entity =>
        {
            entity.HasKey(e => new { e.Serverguid, e.Externallogid, e.Timestamp, e.Seqno }).HasName("7_14_trend_data_pkey");

            entity.ToTable("_hyper_2_7_chunk", "_timescaledb_internal");

            entity.HasIndex(e => e.Externalseqno, "_hyper_2_7_chunk_idx_trend_data_externalseqno");

            entity.HasIndex(e => new { e.Serverguid, e.Timestamp }, "_hyper_2_7_chunk_trend_data_serverguid_timestamp_idx").IsDescending(false, true);

            entity.HasIndex(e => e.Timestamp, "_hyper_2_7_chunk_trend_data_timestamp_idx").IsDescending();

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

            entity.HasOne(d => d.Externallog).WithMany(p => p.Hyper27Chunks)
                .HasForeignKey(d => d.Externallogid)
                .HasConstraintName("7_13_fk_trend_data_externallogid");
        });

        modelBuilder.Entity<Hyper28Chunk>(entity =>
        {
            entity.HasKey(e => new { e.Serverguid, e.Externallogid, e.Timestamp, e.Seqno }).HasName("8_16_trend_data_pkey");

            entity.ToTable("_hyper_2_8_chunk", "_timescaledb_internal");

            entity.HasIndex(e => e.Externalseqno, "_hyper_2_8_chunk_idx_trend_data_externalseqno");

            entity.HasIndex(e => new { e.Serverguid, e.Timestamp }, "_hyper_2_8_chunk_trend_data_serverguid_timestamp_idx").IsDescending(false, true);

            entity.HasIndex(e => e.Timestamp, "_hyper_2_8_chunk_trend_data_timestamp_idx").IsDescending();

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

            entity.HasOne(d => d.Externallog).WithMany(p => p.Hyper28Chunks)
                .HasForeignKey(d => d.Externallogid)
                .HasConstraintName("8_15_fk_trend_data_externallogid");
        });

        modelBuilder.Entity<Hyper29Chunk>(entity =>
        {
            entity.HasKey(e => new { e.Serverguid, e.Externallogid, e.Timestamp, e.Seqno }).HasName("9_18_trend_data_pkey");

            entity.ToTable("_hyper_2_9_chunk", "_timescaledb_internal");

            entity.HasIndex(e => e.Externalseqno, "_hyper_2_9_chunk_idx_trend_data_externalseqno");

            entity.HasIndex(e => new { e.Serverguid, e.Timestamp }, "_hyper_2_9_chunk_trend_data_serverguid_timestamp_idx").IsDescending(false, true);

            entity.HasIndex(e => e.Timestamp, "_hyper_2_9_chunk_trend_data_timestamp_idx").IsDescending();

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

            entity.HasOne(d => d.Externallog).WithMany(p => p.Hyper29Chunks)
                .HasForeignKey(d => d.Externallogid)
                .HasConstraintName("9_17_fk_trend_data_externallogid");
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

        modelBuilder.Entity<TbBranch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tbBranch_pkey");

            entity.ToTable("tbBranch");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(null, null, null, null, true, null)
                .HasColumnName("ID");
            entity.Property(e => e.BranchSquare).HasDefaultValueSql("0.0");
            entity.Property(e => e.CategoryId)
                .HasDefaultValueSql("0")
                .HasColumnName("CategoryID");
            entity.Property(e => e.CityId)
                .HasDefaultValueSql("0")
                .HasColumnName("CityID");
            entity.Property(e => e.Mvz).HasColumnName("MVZ");
            entity.Property(e => e.RegionId)
                .HasDefaultValueSql("0")
                .HasColumnName("RegionID");
            entity.Property(e => e.TradingSquare).HasDefaultValueSql("0.0");
        });

        modelBuilder.Entity<TbBranchSquare>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tbBranchSquare");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("ID");
        });

        modelBuilder.Entity<TbCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tbCategories_pkey");

            entity.ToTable("tbCategories");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("ID");
            entity.Property(e => e.BranchSquareId).HasColumnName("BranchSquareID");
            entity.Property(e => e.ClimateId).HasColumnName("ClimateID");
            entity.Property(e => e.TreadingSquareId).HasColumnName("TreadingSquareID");
            entity.Property(e => e.TypeOfHeatingId).HasColumnName("TypeOfHeatingID");
        });

        modelBuilder.Entity<TbCity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tbCities_pkey");

            entity.ToTable("tbCity");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<TbClimate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tbClimate_pkey");

            entity.ToTable("tbClimate");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<TbEmployee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tbEmployees_pkey");

            entity.ToTable("tbEmployees");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<TbMeter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tbMeters_pkey");

            entity.ToTable("tbMeter");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("ID");
            entity.Property(e => e.RoleId).HasDefaultValueSql("0");
            entity.Property(e => e.TypeId).HasColumnName("TypeID");
        });

        modelBuilder.Entity<TbMeterRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tbMeterRoles_pkey");

            entity.ToTable("tbMeterRoles");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("ID");
        });

        modelBuilder.Entity<TbMeterType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tbMeterType_pkey");

            entity.ToTable("tbMeterType");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("ID");
        });

        modelBuilder.Entity<TbRegion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tbRegions_pkey");

            entity.ToTable("tbRegion");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("ID");
        });

        modelBuilder.Entity<TbTraidingSquare>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tbTraidingSquare_pkey");

            entity.ToTable("tbTraidingSquare");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("ID");
        });

        modelBuilder.Entity<TbTypeOfHeating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tbTypeOfHeating_pkey");

            entity.ToTable("tbTypeOfHeating");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
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
