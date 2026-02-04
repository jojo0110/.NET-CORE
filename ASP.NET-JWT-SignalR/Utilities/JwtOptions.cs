namespace LlamaEngineHost.Utilities
{
    public class JwtOptions
    {
        /// <summary>
        /// Gets or sets Key.
        /// </summary>
        public string Key { get; set; } = null!;

        /// <summary>
        /// Gets or sets Issuer.
        /// </summary>
        public string Issuer { get; set; } = null!;

        /// <summary>
        /// Gets or sets Audience.
        /// </summary>
        public string Audience { get; set; } = null!;

        /// <summary>
        /// Gets or sets ExpireDays.
        /// </summary>
        public int ExpireDays { get; set; } = 1;
    }

}