using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using przychodnia.Models;
using System.Linq;
using System.Numerics;

namespace przychodnia.Controllers
{
    public class DoctorController : Controller
    {
        private readonly StorageContext storageContext;
        private readonly List<SpecialityModel> specialities;
        public DoctorController(StorageContext storageContext)
        {
            this.storageContext = storageContext;
            this.specialities = storageContext.Specialities.ToList();
        }

        public async Task<IActionResult> Index()
        {
            var doctors = await storageContext.Doctors.Include(doc => doc.Speciality).ToListAsync();
            return View(doctors);
        }

        public IActionResult Login(string errorMesssage)
        {
            ViewBag.msg = errorMesssage;
            return View();
        }

        private bool CheckLogin(string email, string password)
        {
            var doctor = storageContext.Doctors.Where(doc => doc.Email == email).FirstOrDefault();
            if (doctor == null)
                return false;
            if (doctor.Password == password)
                return true;
            else
                return false;
        }

        public async Task<IActionResult> LoginDoctor(string email, string password)
        {
            if (CheckLogin(email, password))
            {
                var doctor = await storageContext.Doctors.Where(doc => doc.Email == email).FirstOrDefaultAsync();
                if (doctor.IsDirector)
                    HttpContext.Session.SetString("IsDirector", "true");
                
                HttpContext.Session.SetString("UserType", "doctor");
                HttpContext.Session.SetInt32("UserID", doctor.DoctorID);
                HttpContext.Session.SetString("UserName", doctor.Name + " " + doctor.Surname);
                return RedirectToAction(nameof(Dashboard));
            }
            var message = "Incorrect email or password!";
            return RedirectToAction("Login", "Doctor", new { errorMesssage=message });
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsDirector");
            HttpContext.Session.Remove("UserType");
            HttpContext.Session.Remove("UserID");
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Create() 
        {
            var doctor = new DoctorModel();
            doctor.DateOfBirth = DateTime.Parse("01-01-1990");
            ViewBag.specialities = specialities;
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorModel doctor)
        {
            if (ModelState.IsValid)
            {
                storageContext.Doctors.Add(doctor);
                await storageContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.specialities = specialities;
            return View(doctor);
        }

        public async Task<IActionResult> Delete(int doctorID)
        {
            var doctor = await storageContext.Doctors.FindAsync(doctorID);
            storageContext.Doctors.Remove(doctor);
            await storageContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int doctorID)
        {
            var doctor = await storageContext.Doctors.FindAsync(doctorID);
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DoctorModel doctor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    storageContext.Update(doctor);
                    await storageContext.SaveChangesAsync();
                    if (HttpContext.Session.GetInt32("UserID") == doctor.DoctorID)
                        return RedirectToAction("Dashboard", "Doctor");
                    else
                        return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException e)
                {
                    ViewBag.msg = "Can not save changes becouse someone else made changes at the same time!";
                }
                
            }
            return View(doctor);
        }

        private async Task<List<ScheduleModel>> GetSchedulesForAWeek(int doctorID, DateTime weekStartDT)
        {
            var weekEndDT = weekStartDT.AddDays(7);
            var schedules = await storageContext.Schedules
                    .Where(sch => sch.DoctorID == doctorID)
                    .Where(sch => sch.StartTime > weekStartDT)
                    .Where(sch => sch.EndTime < weekEndDT)
                    .OrderBy(sch => sch.StartTime).ToListAsync();

            return schedules;
        }

            public async Task<IActionResult> ScheduleAsync(int doctorID, string weekStart)
        {
            var weekStartDT = DateTime.Parse(weekStart);
            var schedules = await GetSchedulesForAWeek(doctorID, weekStartDT);

            var doctor = await storageContext.Doctors.FindAsync(doctorID);
            ViewBag.Schedules = schedules;
            ViewBag.Doctor = doctor;
            ViewBag.WeekStart = weekStartDT;
            ViewBag.PreviousWeekStart = weekStartDT.AddDays(-7);
            ViewBag.NextWeekStart = weekStartDT.AddDays(7);
            return View();
        }

        public async Task<IActionResult> DeleteSchedule(int doctorID, int scheduleID, string weekStart)
        {
            var schedule = await storageContext.Schedules.FindAsync(scheduleID);
            storageContext.Schedules.Remove(schedule);
            await storageContext.SaveChangesAsync();
            return RedirectToAction("Schedule", "Doctor", new { doctorID = doctorID, weekStart = weekStart});

        }

        public async Task<IActionResult> Dashboard() 
        {
            int userID = (int)HttpContext.Session.GetInt32("UserID");
            var doctor = await storageContext.Doctors.FindAsync(userID);
            ViewBag.visits = await storageContext.Visits.Include(v => v.Patient).Include(v => v.Schedule)
                                .ThenInclude(sch => sch.Doctor).ThenInclude(doc => doc.Speciality)
                                .Where(v => v.Schedule.Doctor.DoctorID == userID)
                                .Where(v => v.Available == false)
                                .OrderBy(v => v.StartTime).ToListAsync();
            return View(doctor);
        }

        public async Task<IActionResult> EditDescription(int visitID)
        {
            var visit = await storageContext.Visits.Include(v => v.Patient).Include(v => v.Schedule)
                                .ThenInclude(sch => sch.Doctor).ThenInclude(doc => doc.Speciality)
                                .FirstOrDefaultAsync(v => v.VisitID ==visitID);
            return View(visit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDescription(VisitModel visit)
        {
            if (ModelState.IsValid)
            {
                storageContext.Update(visit);
                await storageContext.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            return View(visit);
        }

    }
}
