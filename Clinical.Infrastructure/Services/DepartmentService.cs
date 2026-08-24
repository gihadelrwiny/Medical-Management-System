using Clinical.Application.DTOs.Department;
using Clinical.Application.Interfaces;
using Clinical.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Infrastructure.Services
{
    public class DepartmentService:IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var includes = new Expression<Func<Department, object>>[] { d => d.Doctors };
            var departments = await _unitOfWork.Departments.GetAllAsync(includes, cancellationToken);

            return departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                DoctorCount = d.Doctors.Count
            });
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var includes = new Expression<Func<Department, object>>[] { d => d.Doctors };
            var department = await _unitOfWork.Departments.GetByIdAsync(id, includes, cancellationToken);

            if (department is null) return null;

            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                DoctorCount = department.Doctors.Count
            };
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken = default)
        {
            var department = new Department
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _unitOfWork.Departments.AddAsync(department, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                DoctorCount = 0
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateDepartmentDto dto, CancellationToken cancellationToken = default)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id, null, cancellationToken);
            if (department is null) return false;

            department.Name = dto.Name;
            department.Description = dto.Description;

            _unitOfWork.Departments.Update(department);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id, null, cancellationToken);
            if (department is null) return false;

            _unitOfWork.Departments.Delete(department);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

