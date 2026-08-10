using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application
{
    public static class DepInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITeacherAssignmentService, TeacherAssignmentService>();
            services.AddScoped<IAppSettingsService, AppSettingsService>();
            services.AddScoped<IAssignmentService, AssignmentService>();
            services.AddScoped<ISubmissionService, SubmissionService>();


            return services;
        }
    }
}
