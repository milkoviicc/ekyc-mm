namespace eKYC.Domain.ReferenceData;

/// <summary>
/// An allowed from/to transition (CL_Clnt_PrcsSt_To) in the client processing-status workflow.
/// Drives the state machine in eKYC.Application rather than hard-coding valid transitions in code.
/// </summary>
public sealed class ClientProcessingStatusTransition
{
    public int ClClntPrcsStToId { get; set; }
    public string ClntPrcsStCdFr { get; set; } = string.Empty;
    public string ClntPrcsStCdTo { get; set; } = string.Empty;
}
