using AutoMapper;
using Clinical.Application.DTOs.Appoinment;
using Clinical.Application.DTOs.Department;
using Clinical.Application.DTOs.Doctor;
using Clinical.Application.DTOs.DoctorSchedule;
using Clinical.Application.DTOs.MedicalRecord;
using Clinical.Application.DTOs.Patients;
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
           
            CreateMap<Department, DepartmentDto>()
                .ForMember(
                    dest => dest.DoctorCount,
                    opt => opt.MapFrom(src => src.Doctors.Count));

            CreateMap<CreateDepartmentDto, Department>();

            CreateMap<UpdateDepartmentDto, Department>();

            //Patient mappings
           
            CreateMap<UpdatePatientDto, Patient>();

            CreateMap<Patient, PatientDto>();
            //appoinment mappings
            CreateMap<Appointment, AppointmentDto>()
              .ForMember(dest => dest.PatientName,
                  opt => opt.MapFrom(src => src.Patient.User.FirstName + " " + src.Patient.User.LastName))
              .ForMember(dest => dest.DoctorName,
                  opt => opt.MapFrom(src => src.Doctor.User.FirstName + " " + src.Doctor.User.LastName));

            CreateMap<CreateAppointmentDto, Appointment>();

            // Only overwrite fields the client is allowed to change
            CreateMap<UpdateAppointmentDto, Appointment>();

            //DoctorSchedule mappings
            CreateMap<DoctorSchedule, DoctorScheduleDto>()
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Doctor.User.FirstName + " " + src.Doctor.User.LastName));

            CreateMap<CreateDoctorScheduleDto, DoctorSchedule>();

            CreateMap<UpdateDoctorScheduleDto, DoctorSchedule>();
            //medical record mappings
            CreateMap<MedicalRecord, MedicalRecordDto>();

            CreateMap<CreateMedicalRecordRequest, MedicalRecord>();

            CreateMap<UpdateMedicalRecordDto, MedicalRecord>();

        }
    }
}
