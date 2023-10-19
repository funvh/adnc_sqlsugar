namespace Adnc.Infra.Serilog
{
    /// <summary>
    /// Serilog配置选项
    /// </summary>
    public class SerilogConfigOptions
    {
        public const string OptionName = "SerilogConfig";

        /// <summary>
        /// Seq地址
        /// </summary>
        public string? SeqUrl { get; set; }

        /// <summary>
        /// MongoDB地址
        /// </summary>
        public string? MongoDBUrl { get; set; }
    }
}