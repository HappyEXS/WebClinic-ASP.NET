using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using przychodnia.Models;

namespace przychodnia.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly StorageContext storageContext;
        private readonly List<SpecialityModel> specialities;


        public ScheduleController(StorageContext storageContext)
        {
            this.storageContext = storageContext;
            this.specialities = storageContext.Specialities.ToList();
        }

        public async Task<IActionResult> Index(DateTime startDate, DateTime endDate, int specID = -1, bool searched=false)
        {
            List<ScheduleModel> schedules;
            QueryTemplate queryTemplate = new QueryTemplate();
            if (searched == false)
            {
                queryTemplate.StartDate = DateTime.Now.Date;
                queryTemplate.EndDate = DateTime.Now.Date.AddDays(7);
            }
            else
            {
                queryTemplate.StartDate = startDate;
                queryTemplate.EndDate = endDate;
                queryTemplate.SpecialityID = specID;
            }
            if (specID != -1)
            {
                schedules = await storageContext.Schedules
                    .Include(sch => sch.Doctor).ThenInclude(doc => doc.Speciality)
                    .Where(sch => sch.Doctor.Speciality.SpecialityID == specID)
                    .Where(sch => sch.StartTime > queryTemplate.StartDate)
                    .Where(sch => sch.EndTime < queryTemplate.EndDate)
                    .OrderBy(sch => sch.StartTime).ToListAsync();
            }
            else
                schedules = await storageContext.Schedules
                    .Include(sch => sch.Doctor).ThenInclude(doc => doc.Speciality)
                    .Where(sch => sch.StartTime > queryTemplate.StartDate)
                    .Where(sch => sch.EndTime < queryTemplate.EndDate)
                    .OrderBy(sch => sch.StartTime).ToListAsync();
            
            ViewBag.specialities = specialities;
            ViewBag.schedules = schedules;
            return View(queryTemplate);
        }

        public async Task<IActionResult> Create()
        {
            var scheduleM = new ScheduleModel();
            ViewBag.doctors = await storageContext.Doctors.ToListAsync();
            return View(scheduleM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScheduleModel schedule)
        {
            ViewBag.doctors = await storageContext.Doctors.ToListAsync();
            if (schedule.StartTime >= schedule.EndTime)
            {
                ModelState.AddModelError(nameof(schedule.EndTime), "End time must be after the start time.");
            }
            TimeSpan diffTime = schedule.EndTime - schedule.StartTime;
            TimeSpan maxTimeSpan = new TimeSpan(8, 0, 0);
            if (TimeSpan.Compare(diffTime, maxTimeSpan) > 0)
            {
                ModelState.AddModelError(nameof(schedule.EndTime), "Schedule can not be longer than 8 hours.");
            }
            if (schedule.StartTime < DateTime.Now.AddDays(-1))
            {
                ModelState.AddModelError(nameof(schedule.StartTime), "Can not add a schedule more than a day ago.");
            }

            if (ModelState.IsValid)
            {
                storageContext.Schedules.Add(schedule);
                await storageContext.SaveChangesAsync();
                AddVisitsForSchedule(schedule);
                return RedirectToAction(nameof(Index));
            }
            return View(schedule);
        }

        private void AddVisitsForSchedule(ScheduleModel schedule)
        {
            TimeSpan oneVisitSpan = new TimeSpan(0, 15, 0);
            DateTime tmpDateTime = schedule.StartTime;

            while (tmpDateTime < schedule.EndTime)
            {
                var visit = new VisitModel();
                visit.ScheduleID = schedule.ScheduleID;
                visit.StartTime = tmpDateTime;
                storageContext.Visits.Add(visit);
                tmpDateTime += oneVisitSpan;
            }
            storageContext.SaveChangesAsync();
        }

        public IActionResult Search(QueryTemplate queryScheduleView)
        {
            return RedirectToAction("Index", "Schedule", 
                                    new
                                    {
                                        startDate = queryScheduleView.StartDate,
                                        endDate = queryScheduleView.EndDate,
                                        specID = queryScheduleView.SpecialityID,
                                        searched = true
                                    });
        }
    }
}
