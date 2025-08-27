using GreenTrace.Api.Infrastructure.Entities;
using GreenTrace.Api.ViewModels.Documents;

namespace GreenTrace.Api.Mappers;

public static class DocumentMapper
{
    public static Document ToEntity(this UploadDocumentViewModel vm) => new()
    {
        CompanyId = vm.CompanyId,
        ReportId = vm.ReportId,
        FileName = vm.FileName,
        StorageUrl = vm.StorageUrl,
        MimeType = vm.MimeType,
        Sha256 = vm.Sha256
    };

    public static DocumentViewModel ToViewModel(this Document doc) =>
        new(doc.Id, doc.CompanyId, doc.ReportId, doc.FileName, doc.StorageUrl, doc.MimeType, doc.Sha256);
}
