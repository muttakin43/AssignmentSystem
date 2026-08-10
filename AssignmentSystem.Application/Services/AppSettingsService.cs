using AssignmentSystem.Application.DTOs.AppSettings;
using AssignmentSystem.Application.Exceptions;
using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Services
{
    public class AppSettingsService(IAppDbContext dbContext) : IAppSettingsService
    {
        public async Task<IReadOnlyList<AppSettingDto>> GetAllAsync(CancellationToken ct = default) =>
       await dbContext.AppSettings
           .OrderBy(s => s.Key)
           .Select(s => new AppSettingDto(s.Id, s.Key, s.Value, s.Description))
           .ToListAsync(ct);

        public async Task<AppSettingDto> GetByKeyAsync(string key, CancellationToken ct = default)
        {
            var setting = await dbContext.AppSettings.FirstOrDefaultAsync(s => s.Key == key, ct)
                ?? throw new NotFoundException($"Setting with key '{key}' was not found.");

            return new AppSettingDto(setting.Id, setting.Key, setting.Value, setting.Description);
        }

        public async Task<AppSettingDto> CreateAsync(CreateAppSettingRequest request, CancellationToken ct = default)
        {
            var keyTaken = await dbContext.AppSettings.AnyAsync(s => s.Key == request.Key, ct);
            if (keyTaken)
                throw new ConflictException($"A setting with key '{request.Key}' already exists.");

            var setting = new AppSettings
            {
                Key = request.Key,
                Value = request.Value,
                Description = request.Description
            };

            dbContext.AppSettings.Add(setting);
            await dbContext.SaveChangesAsync(ct);

            return new AppSettingDto(setting.Id, setting.Key, setting.Value, setting.Description);
        }

        public async Task<AppSettingDto> UpdateAsync(string key, UpdateAppSettingRequest request, CancellationToken ct = default)
        {
            var setting = await dbContext.AppSettings.FirstOrDefaultAsync(s => s.Key == key, ct)
                ?? throw new NotFoundException($"Setting with key '{key}' was not found.");

            setting.Value = request.Value;
            setting.Description = request.Description;

            await dbContext.SaveChangesAsync(ct);
            return new AppSettingDto(setting.Id, setting.Key, setting.Value, setting.Description);
        }

        public async Task DeleteAsync(string key, CancellationToken ct = default)
        {
            var setting = await dbContext.AppSettings.FirstOrDefaultAsync(s => s.Key == key, ct)
                ?? throw new NotFoundException($"Setting with key '{key}' was not found.");

            dbContext.AppSettings.Remove(setting);
            await dbContext.SaveChangesAsync(ct);
        }
    }
}
