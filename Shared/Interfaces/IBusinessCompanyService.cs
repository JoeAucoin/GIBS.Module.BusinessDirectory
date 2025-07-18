﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace GIBS.Module.BusinessDirectory.Services
{
    public interface IBusinessCompanyService
    {
        Task<List<Models.BusinessCompany>> GetBusinessCompaniesAsync(int moduleId);
        Task<Models.BusinessCompany> GetBusinessCompanyAsync(int companyId, int moduleId);
        Task<Models.BusinessCompany> AddBusinessCompanyAsync(Models.BusinessCompany businessCompany);
        Task<Models.BusinessCompany> UpdateBusinessCompanyAsync(Models.BusinessCompany businessCompany);
        Task DeleteBusinessCompanyAsync(int companyId, int module);

        Task<List<Models.BusinessCompany>> GetBusinessCompaniesByTypeAsync(int typeId, int moduleId);

        Task<List<Models.BusinessCompany>> GetBusinessCompaniesByIsNewItemAsync(int moduleId);

        /// <summary>
        /// Uploads an image and returns the image URL.
        /// </summary>
        /// <param name="fileBytes">The image file bytes.</param>
        /// <param name="fileName">The image file name.</param>
        /// <returns>URL of the uploaded image.</returns>
        Task<string> UploadImageAsync(byte[] fileBytes, string fileName);

        Task UpdateCompanyAttributesAsync(int companyId, int moduleId, List<int> attributeIds);

        Task<List<Models.BusinessCompany>> GetCompanyAttributesAsync(int companyId, int moduleId);
        Task<Oqtane.Models.File> ResizeImageAsync(int fileId, int width, int height, int moduleId);
    }
}
