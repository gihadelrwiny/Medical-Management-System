using AutoMapper;
using Clinical.Application.DTOs.Department;
using Clinical.Application.DTOs.Doctor;
using Clinical.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Clinical.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Doctor mappings
            CreateMap<Doctor, DoctorDto>();

            CreateMap<CreateDoctorRequest, Doctor>();

            CreateMap<UpdateDoctorDto, Doctor>();

            //Department mappings
            // Department
            CreateMap<Department, DepartmentDto>()
                .ForMember(
                    dest => dest.DoctorCount,
                    opt => opt.MapFrom(src => src.Doctors.Count));

            CreateMap<CreateDepartmentDto, Department>();

            CreateMap<UpdateDepartmentDto, Department>();


        }
    }
}
