namespace Dacoder.Webfileserver.Settings;

public class AppSettings
{
    public string FileServerPath { get; set; } = string.Empty;
    public string TimeStructure { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = string.Empty;
    public string DownloadHistoryFolderName { get; set; } = string.Empty;
    public string WebsiteAlias { get; set; } = string.Empty;
    public string PoorManClientCredential { get; set; } = string.Empty;
    public int ApplicationPort { get; set; }
    public string RelativePfxCertificateFileName { get; set; } = string.Empty;
    public string CertificatePassword { get; set; } = string.Empty;
    public string DownloadHistoryExclusionCsvFileList { get; set; } = string.Empty;

}
