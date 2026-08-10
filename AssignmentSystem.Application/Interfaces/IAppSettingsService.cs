using AssignmentSystem.Application.DTOs.AppSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Interfaces
{
    public interface IAppSettingsService
    {
        Task<IReadOnlyList<AppSettingDto>> GetAllAsync(CancellationToken ct = default);
        Task<AppSettingDto> GetByKeyAsync(string key, CancellationToken ct = default);
        Task<AppSettingDto> CreateAsync(CreateAppSettingRequest request, CancellationToken ct = default);
        Task<AppSettingDto> UpdateAsync(string key, UpdateAppSettingRequest request, CancellationToken ct = default);
        Task DeleteAsync(string key, CancellationToken ct = default);
    }
}
