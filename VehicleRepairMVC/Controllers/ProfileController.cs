using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleRepairMVC.Context;

namespace VehicleRepairMVC.Controllers
{
    public class ProfileController : Controller
    {
        private readonly VehicleRepairDbContext _context;

        public ProfileController(VehicleRepairDbContext context)
        {
            _context = context;
        }

        public IActionResult MyAppointments()
        {
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";
            if (HttpContext.Session.GetInt32("LoggedInUserId").HasValue)
            {
                int? loggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId");
                if (loggedInUserId.HasValue)
                {
                    var approvedAppointments = _context.Appointments
                        .Include(a => a.Car)
                        .Where(a => a.Car.UserID == loggedInUserId.Value && (a.Status == "Onaylandı" || a.Status=="İptal Edildi"))
                        .ToList();

                    return View(approvedAppointments);
                }
            }

            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound();
            }

            // Randevu tarihinden şu anki zamanı çıkararak fark oluştur
            var timeDifference = appointment.AppointmentDate - DateTime.Now;

            // Eğer randevu tarihi geçmişse veya fark 24 saatten az ise, randevuyu iptal etmeye izin verme
            if (timeDifference.TotalHours < 24)
            {
                TempData["ErrorMessage"] = "Randevuyu son 24 saat içinde iptal edemezsiniz!";
                return RedirectToAction("MyAppointments"); // Hata mesajıyla birlikte MyAppointments sayfasına yönlendir
            }

            // Randevuyu iptal etme işlemi
            appointment.Status = "İptal Edildi"; // Statusu güncelle
            await _context.SaveChangesAsync();

            return RedirectToAction("MyAppointments"); // Silme işlemi tamamlandıktan sonra MyAppointments sayfasına yönlendir.
        }

        public IActionResult MyVehicles()
        {
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";
            if (HttpContext.Session.GetInt32("LoggedInUserId").HasValue)
            {
                int? loggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId");
                if (loggedInUserId.HasValue)
                {
                    var userCars = _context.Cars.Where(c => c.UserID== loggedInUserId.Value).ToList();
                    return View(userCars);
                }
            }
            return RedirectToAction("Index", "Login");
        }
    }
}
