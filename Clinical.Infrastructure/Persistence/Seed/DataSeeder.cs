using Clinical.Domain.Entities;
using Clinical.Domain.Enums;
using Clinical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Clinical.Infrastructure.Persistence.Seed
{
    /// <summary>
    /// Seeds initial development data into the database.
    /// The seeder is safe to run on every application startup because
    /// each Seed method checks whether the corresponding data already exists.
    /// </summary>
    public class DataSeeder
    {
        private readonly ClinicalDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public DataSeeder(
            ClinicalDbContext context,
            IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            // Apply pending migrations before seeding.
            await _context.Database.MigrateAsync();

            var departments = await SeedDepartmentsAsync();
            var users = await SeedUsersAsync();

            await SeedDoctorsAsync(users, departments);
            await SeedPatientsAsync(users);
        }

        private async Task<Department[]> SeedDepartmentsAsync()
        {
            if (await _context.Departments.AnyAsync())
            {
                return await _context.Departments.ToArrayAsync();
            }

            var departments = new[]
            {
                new Department
                {
                    Name = "Cardiology",
                    Description = "Heart and cardiovascular system."
                },

                new Department
                {
                    Name = "Neurology",
                    Description = "Brain, spine, and nervous system."
                },

                new Department
                {
                    Name = "Pediatrics",
                    Description = "Medical care for infants, children, and adolescents."
                },

                new Department
                {
                    Name = "Orthopedics",
                    Description = "Bones, joints, ligaments, and muscles."
                },

                new Department
                {
                    Name = "Dermatology",
                    Description = "Skin, hair, and nail conditions."
                }
            };

            await _context.Departments.AddRangeAsync(departments);
            await _context.SaveChangesAsync();

            return departments;
        }

        private async Task<(User Admin, User[] Doctors, User[] Patients)> SeedUsersAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                var existing = await _context.Users.ToArrayAsync();

                return
                (
                    existing.First(u => u.Role == UserRole.Admin),

                    existing
                        .Where(u => u.Role == UserRole.Doctor)
                        .ToArray(),

                    existing
                        .Where(u => u.Role == UserRole.Patient)
                        .ToArray()
                );
            }

            // =========================
            // Admin
            // =========================

            var admin = new User
            {
                FirstName = "System",
                LastName = "Admin",
                Email = "admin@clinical.local",
                Role = UserRole.Admin
            };

            admin.PasswordHash =
                _passwordHasher.HashPassword(admin, "Admin123!");


            // =========================
            // Doctors
            // =========================

            var doctorUsers = new[]
            {
                new User
                {
                    FirstName = "Sarah",
                    LastName = "Connor",
                    Email = "sarah.connor@clinical.local",
                    Role = UserRole.Doctor
                },

                new User
                {
                    FirstName = "James",
                    LastName = "Wilson",
                    Email = "james.wilson@clinical.local",
                    Role = UserRole.Doctor
                },

                new User
                {
                    FirstName = "Emily",
                    LastName = "Chen",
                    Email = "emily.chen@clinical.local",
                    Role = UserRole.Doctor
                }
            };

            foreach (var doctor in doctorUsers)
            {
                doctor.PasswordHash =
                    _passwordHasher.HashPassword(doctor, "Doctor123!");
            }


            // =========================
            // Patients
            // =========================

            var patientUsers = new[]
            {
                new User
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@clinical.local",
                    Role = UserRole.Patient
                },

                new User
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@clinical.local",
                    Role = UserRole.Patient
                },

                new User
                {
                    FirstName = "Michael",
                    LastName = "Brown",
                    Email = "michael.brown@clinical.local",
                    Role = UserRole.Patient
                }
            };

            foreach (var patient in patientUsers)
            {
                patient.PasswordHash =
                    _passwordHasher.HashPassword(patient, "Patient123!");
            }


            // =========================
            // Save Users
            // =========================

            await _context.Users.AddAsync(admin);
            await _context.Users.AddRangeAsync(doctorUsers);
            await _context.Users.AddRangeAsync(patientUsers);

            await _context.SaveChangesAsync();

            return (admin, doctorUsers, patientUsers);
        }

        private async Task SeedDoctorsAsync(
            (User Admin, User[] Doctors, User[] Patients) users,
            Department[] departments)
        {
            if (await _context.Doctors.AnyAsync())
            {
                return;
            }

            var doctors = new[]
            {
                new Doctor
                {
                    UserId = users.Doctors[0].Id,
                    DepartmentId = departments[0].Id,
                    PhoneNumber = "+1-555-0101"
                },

                new Doctor
                {
                    UserId = users.Doctors[1].Id,
                    DepartmentId = departments[1].Id,
                    PhoneNumber = "+1-555-0102"
                },

                new Doctor
                {
                    UserId = users.Doctors[2].Id,
                    DepartmentId = departments[2].Id,
                    PhoneNumber = "+1-555-0103"
                }
            };

            await _context.Doctors.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();


            // =========================
            // Doctor Schedules
            // Monday - Friday
            // 09:00 - 17:00
            // =========================

            foreach (var doctor in doctors)
            {
                var schedules = Enumerable
                    .Range(1, 5)
                    .Select(dayOffset => new DoctorSchedule
                    {
                        DoctorId = doctor.Id,

                        DayOfWeek = (DayOfWeek)dayOffset,

                        StartTime = new TimeSpan(9, 0, 0),

                        EndTime = new TimeSpan(17, 0, 0),

                        IsAvailable = true
                    });

                await _context.DoctorSchedules.AddRangeAsync(schedules);
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedPatientsAsync(
            (User Admin, User[] Doctors, User[] Patients) users)
        {
            if (await _context.Patients.AnyAsync())
            {
                return;
            }

            var patients = new[]
            {
                new Patient
                {
                    UserId = users.Patients[0].Id,
                    DateOfBirth = new DateTime(1990, 3, 14),
                    Gender = Gender.Male,
                    PhoneNumber = "+1-555-0201",
                    Address = "12 Elm Street, Springfield"
                },

                new Patient
                {
                    UserId = users.Patients[1].Id,
                    DateOfBirth = new DateTime(1985, 7, 22),
                    Gender = Gender.Female,
                    PhoneNumber = "+1-555-0202",
                    Address = "88 Oak Avenue, Springfield"
                },

                new Patient
                {
                    UserId = users.Patients[2].Id,
                    DateOfBirth = new DateTime(2000, 11, 5),
                    Gender = Gender.Male,
                    PhoneNumber = "+1-555-0203",
                    Address = "5 Maple Court, Springfield"
                }
            };

            await _context.Patients.AddRangeAsync(patients);

            await _context.SaveChangesAsync();
        }
    }
}