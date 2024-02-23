using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using przychodnia.Models;

namespace przychodnia.Controllers
{
    public class VisitController : Controller
    {
        private readonly StorageContext storageContext;
        private readonly List<SpecialityModel> specialities;

        public VisitController(StorageContext storageContext)
        {
            this.storageContext = storageContext;
            this.specialities = storageContext.Specialities.ToList();
        }
        public async Task<IActionResult> Index(DateTime startDate, DateTime endDate, int specID = -1, bool searched = false, string message = "")
        {
            List<VisitModel> visits;
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
                visits = await storageContext.Visits.Include(v => v.Schedule)
                        .ThenInclude(sch => sch.Doctor).ThenInclude(doc => doc.Speciality)
                        .Where(v => v.Available == true)
                        .Where(v => v.Schedule.Doctor.Speciality.SpecialityID == specID)
                        .Where(v => v.StartTime > queryTemplate.StartDate)
                        .Where(v => v.StartTime < queryTemplate.EndDate)
                        .OrderBy(v => v.StartTime).ToListAsync();
            else
                visits = await storageContext.Visits.Include(v => v.Schedule)
                        .ThenInclude(sch => sch.Doctor).ThenInclude(doc => doc.Speciality)
                        .Where(v => v.Available == true)
                        .Where(v => v.StartTime > queryTemplate.StartDate)
                        .Where(v => v.StartTime < queryTemplate.EndDate)
                        .OrderBy(v => v.StartTime).ToListAsync();
            
            ViewBag.msg = message;
            ViewBag.visits = visits;
            ViewBag.specialities = specialities;
            return View(queryTemplate);
        }

        public IActionResult Search(QueryTemplate queryVisitView) 
        {
            return RedirectToAction("Index", "Visit",
                                    new
                                    {
                                        startDate = queryVisitView.StartDate,
                                        endDate = queryVisitView.EndDate,
                                        specID = queryVisitView.SpecialityID,
                                        searched = true
                                    });
        }
        public async Task<IActionResult> SignUpForVisit(QueryTemplate queryVisitView, int visitID, int patientID)
        {
            var visit = await storageContext.Visits.FindAsync(visitID);
            if (visit.PatientID != null || visit.Available == false)
            {
                return RedirectToAction("Index", "Visit", new {
                    startDate = queryVisitView.StartDate,
                    endDate = queryVisitView.EndDate,
                    specID = queryVisitView.SpecialityID,
                    searched = true,
                    message = "Can not sign up for a visit because visit is not available anymore!" });
            }
            visit.PatientID = patientID;
            visit.Available = false;
            storageContext.Update(visit);
            await storageContext.SaveChangesAsync();

            return RedirectToAction("Index", "Visit", 
                                    new
                                    {
                                        startDate = queryVisitView.StartDate,
                                        endDate = queryVisitView.EndDate,
                                        specID = queryVisitView.SpecialityID,
                                        searched = true
                                    });
        }
    }
}
