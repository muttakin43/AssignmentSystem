using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.DTOs.AppSettings
{
    public record AppSettingDto(Guid Id, string Key, string Value, string? Description);

    public record CreateAppSettingRequest(string Key, string Value, string? Description);

    public record UpdateAppSettingRequest(string Value, string? Description);
}
