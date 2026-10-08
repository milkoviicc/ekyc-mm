namespace eKYC.Domain.Revisions;

/// <summary>Mirrors TkLayoutRevisionFilter.java's year and revision-type dropdowns.</summary>
public sealed class DocRevisionFilter
{
    public int? Year { get; set; }
    public int? RevTypeId { get; set; }
}
