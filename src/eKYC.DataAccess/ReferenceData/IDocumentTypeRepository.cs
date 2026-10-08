using eKYC.Domain.ReferenceData;

namespace eKYC.DataAccess.ReferenceData;

public interface IDocumentTypeRepository
{
    Task<IReadOnlyList<DocumentType>> GetAllAsync();
}
