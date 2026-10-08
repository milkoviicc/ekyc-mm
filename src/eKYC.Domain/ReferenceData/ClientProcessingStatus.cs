namespace eKYC.Domain.ReferenceData;

/// <summary>
/// A single processing-status value in the client onboarding/review workflow (CL_Clnt_PrcsSt).
/// </summary>
public sealed class ClientProcessingStatus
{
    public int ClntPrcsStId { get; set; }
    public string ClntPrcsStCd { get; set; } = string.Empty;
    public string? Status { get; set; }
    public string? ClntPrcsStDspn { get; set; }
}
