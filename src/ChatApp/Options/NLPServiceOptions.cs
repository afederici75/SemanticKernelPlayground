namespace ChatApp.Options;

public class NLPServiceOptions
{
    public const string SettingName = "OpenAI";

    public string Model { get; set; } = "text-davinci-003";
    public string EmbeddingModel { get; set; } = "text-embedding-ada-002";
    public string ApiKey { get; set; } = "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";
    public string Organization { get; set; } = string.Empty;
}