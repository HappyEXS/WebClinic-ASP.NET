using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using przychodnia.Models;
using System.Numerics;


namespace przychodnia.Controllers
{
    public class PatientController : Controller
    {
        private readonly StorageContext storageContext;
        public PatientController(StorageContext storageContext)
        {
            this.storageContext = storageContext;
        }

        public async Task<IActionResult> Index()
        {
            var patients = await storageContext.Patients.ToListAsync();
            return View(patients);
        }
        public IActionResult Login(string errorMesssage)
        {
            ViewBag.msg = errorMesssage;
            return View();
        }


        private bool CheckLogin(string email, string password)
        {
            var patient = storageContext.Patients.Where(pat => pat.Email == email).FirstOrDefault();
            if (patient == null)
                return false;
            if (patient.Password == password)
                return true;
            else
                return false;
        }

        public async Task<IActionResult> LoginPatient(string email, string password)
        {
            if (CheckLogin(email, password))
            {
                var patient = await storageContext.Patients.Where(pat => pat.Email == email).FirstOrDefaultAsync();
                HttpContext.Session.SetString("UserType", "patient");
                if (!patient.IsActive)
                    HttpContext.Session.SetString("PatientInactive", "true");
                HttpContext.Session.SetInt32("UserID", patient.PatientID);
                HttpContext.Session.SetString("UserName", patient.Name + " " + patient.Surname);
                return RedirectToAction(nameof(Dashboard));
            }
            var message = "Incorrect email or password!";
            return RedirectToAction("Login", "Patient", new { errorMesssage = message });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserType");
            HttpContext.Session.Remove("PatientInactive");
            HttpContext.Session.Remove("UserID");
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Create() 
        {
            var patient = new PatientModel();
            patient.DateOfBirth = DateTime.Parse("01-01-1990");
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientModel patient)
        {
            if (ModelState.IsValid)
            {
                storageContext.Patients.Add(patient);
                await storageContext.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            return View(patient);
        }

        public async Task<IActionResult> Edit()
        {
            var patient = await storageContext.Patients.FindAsync(HttpContext.Session.GetInt32("UserID"));
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PatientModel patient)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    storageContext.Update(patient);
                    await storageContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Dashboard));
                }
                catch (DbUpdateConcurrencyException e) 
                {
                    ViewBag.msg = "Can not save changes becouse someone else made changes at the same time!";
                }
                
            }
            return View(patient);
        }

        public async Task<IActionResult> Activate(int patientID)
        {
            var patient = await storageContext.Patients.FindAsync(patientID);
            patient.IsActive = true;
            await storageContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Disactivate(int patientID)
        {
            var patient = await storageContext.Patients.FindAsync(patientID);
            patient.IsActive = false;
            await storageContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int patientID)
        {
            var patient = await storageContext.Patients.FindAsync(patientID);
            storageContext.Patients.Remove(patient);
            await storageContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Dashboard()
        {
            int userID = (int)HttpContext.Session.GetInt32("UserID");
            var patient = await storageContext.Patients.FindAsync(userID);

            ViewBag.visits = await storageContext.Visits.Include(v => v.Patient).Include(v => v.Schedule)
                                .ThenInclude(sch => sch.Doctor).ThenInclude(doc => doc.Speciality)
                                .Where(v => v.Patient.PatientID == userID)
                                .OrderBy(v => v.StartTime).ToListAsync();
            return View(patient);
        }

        public async Task<IActionResult> CancelVisit(int visitID)
        {
            var visit = await storageContext.Visits.FindAsync(visitID);
            visit.PatientID = null;
            visit.Available = true;
            storageContext.Update(visit);
            await storageContext.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> VisitDetails(int visitID)
        {
            var visit = await storageContext.Visits.Include(v => v.Patient).Include(v => v.Schedule)
                                .ThenInclude(sch => sch.Doctor).ThenInclude(doc => doc.Speciality)
                                .FirstOrDefaultAsync(v => v.VisitID == visitID);
            return View(visit);
        }
    }
}
