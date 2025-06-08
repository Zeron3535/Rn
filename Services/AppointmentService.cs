using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinaKuafor.Data;
using TinaKuafor.Models;

namespace TinaKuafor.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Service)
                .OrderBy(a => a.DateTime)
                .ToListAsync();
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task CreateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsAppointmentAvailableAsync(DateTime dateTime, int duration)
        {
            var endTime = dateTime.AddMinutes(duration);

            var conflictingAppointment = await _context.Appointments
                .Include(a => a.Service) // İYİLEŞTİRME: Sorguya Service dahil edildi.
                .FirstOrDefaultAsync(a =>
                    a.Service != null && // İYİLEŞTİRME: Null kontrolü eklendi.
                    ((dateTime >= a.DateTime && dateTime < a.DateTime.AddMinutes(a.Service.Duration)) ||
                    (endTime > a.DateTime && endTime <= a.DateTime.AddMinutes(a.Service.Duration)))
                );

            return conflictingAppointment == null;
        }
    }
}