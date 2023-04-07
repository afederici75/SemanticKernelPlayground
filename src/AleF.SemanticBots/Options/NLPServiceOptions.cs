namespace AleF.SemanticKernel.Options;

public class NLPServiceOptions
{
    public const string SettingName = "NLPService";

    public string Model { get; set; } = "text-davinci-003";
    public string EmbeddingModel { get; set; } = "text-embedding-ada-002";
    public required string ApiKey { get; set; }
    public string? Organization { get; set; } = string.Empty;
}