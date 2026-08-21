using CMS_Admission.Areas.Attendance.DAL;
using CMS_Admission.Areas.Faculty.DAL;
using CMS_Admission.Areas.Feedback.DAL;
using CMS_Admission.Areas.FYUGP.DAL;
using CMS_Admission.Areas.LearningSchedule.DAL;
using CMS_Admission.Areas.UGAdmission.DAL;
using CMS_Admission.DAL;
using CMS_Admission.Email;
using CMS_Admission.Models;
using CMS_Admission.Models.CCA;
using CMS_Admission.Models.Exam;
using CMS_Admission.Models.Feedback;
using CMS_Admission.Models.FYUGP;
using CMS_Admission.Models.NIRF;
using CMS_Admission.Models.QuestionBank;
using CMS_Admission.Models.StudentPortal;
//using Microsoft.Office.Interop.Word;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Configuration;
//using System.Web.UI.WebControls;
//using DocumentFormat.OpenXml.Packaging;
using System.Data;
//using iTextSharp.text;
using Docnet.Core;
using Docnet.Core.Models;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.IO;
using System.Drawing.Imaging;
//using PdfiumViewer;

using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CMS_Admission.Areas.Faculty.Controllers
{
    public class FacultyController : Controller
    {
        //
        // GET: /Faculty/Faculty/
        CMSDBContext db = new CMSDBContext();
        DALLogin objLogin = new DALLogin();
        DALFaculty objfaculty = new DALFaculty();
        CMSDBContext dbExam = new CMSDBContext();
        DALFeedback objFeed = new DALFeedback();
        DALAttendance objAtt = new DALAttendance();
        DALLearningSchedule objLearn = new DALLearningSchedule();
        DALFYUGP objFYUGP = new DALFYUGP();
        public AccountService objservice = new AccountService();
        public CMS_Admission.font_awesome.Email.SmsService objsmsservice = new CMS_Admission.font_awesome.Email.SmsService();
        public CMS_Admission.font_awesome.encryptdecrypt.EncryptDecryptPassword objpwd = new CMS_Admission.font_awesome.encryptdecrypt.EncryptDecryptPassword();
        #region ATTENDANCE
        public ActionResult attendance_Home()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    List<AttendanceReport> intrnl = objfaculty.getProgrammesFacultyWise(Faculty_Id).ToList();
                    return View(intrnl);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult StudentAttendanceRequestView(string UPRN, int Grp_No)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    string Mobile = Session["Mobile"].ToString();
                    int Role_Id = Convert.ToInt32(Session["Role_Id"].ToString());
                    int dep_id = Convert.ToInt32(Session["DepId"]);
                    ViewBag.UPRN = UPRN;
                    ViewBag.Leave_Reason = db.CMS_Student_Attendance_Requests.Where(x => x.UPRN == UPRN && x.Grp_No == Grp_No).Select(x => x.Category).FirstOrDefault();
                    ViewBag.Grp_No = Grp_No;
                    return View();

                    //if (Role_Id == 13)
                    //{
                    //    int Acc_Yr_Sem_Id = 0;
                    //    int Acc_Yr_Sem_Pgm_Id = 0;
                    //    int Pgm_Id = 0;
                    //    List<Student> students = new List<Student>();
                    //    List<Schedule> sh = objfaculty.get_Class_Faculty_Sem_Id(dep_id, Faculty_Id).ToList();
                    //    foreach (var i in sh)
                    //    {
                    //        Acc_Yr_Sem_Id = i.Acc_Yr_sem_Id;
                    //        Acc_Yr_Sem_Pgm_Id = i.Acc_Yr_Sem_Pgm_Id;
                    //        Pgm_Id = db.CMS_AcademicYr_Sem_Programmes.Where(x => x.Active_Status == true && x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Sem_Pgm_Id).Select(x => x.Pgm_Id).FirstOrDefault();

                    //        //students = objfaculty.get_Student_Class_By_Classwarden(Acc_Yr_Sem_Id,Pgm_Id).ToList();
                    //    }
                    //    ViewBag.Students = new SelectList(objfaculty.get_Student_By_Classwarden(Acc_Yr_Sem_Id, Pgm_Id), "UPRN", "Name");
                    //    return View();
                    //}
                }
                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");
            }
        }
        public ActionResult View_QuestionCountDept()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Dep_Id = Convert.ToInt32(Session["DepId"].ToString());
                    ViewBag.Programmes_Type = new SelectList(objfaculty.getProgrammeTypesByDep(Dep_Id), "Pgm_Type_Id", "Pgm_Type", 1);
                    int Pgm_TypeID = objfaculty.getProgrammeTypesByDep(Dep_Id).Select(x => x.Pgm_Type_Id).FirstOrDefault();
                    ViewBag.Semester = new SelectList(objfaculty.getSemesterByDep(Dep_Id, Pgm_TypeID), "Acc_Yr_Sem_Id", "Semester");
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");
            }
        }
        //public ActionResult SplAttendanceRequests()
        //{
        //    if (Session["Log_Id"] != null)
        //    {
        //        try
        //        {
        //            ViewBag.Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
        //            return View();
        //        }
        //        catch
        //        {
        //            return Redirect("~/Login/Error_Page");
        //        }
        //    }
        //    else
        //    {
        //        return Redirect("~/Login/Login");

        //    }
        //}
        public ActionResult AttendanceRequests()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    ViewBag.HOD = db.CMS_HODs.Where(x => x.Faculty_Id == Faculty_Id && x.Active_Status == true).Count();
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");
            }
        }

        public ActionResult Search_Semester_Students_Req(int Acc_Yr_Sem_Id, int actStatus)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Student> students = new List<Student>();
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    students = objfaculty.get_Student_CRequts(Acc_Yr_Sem_Id, Faculty_Id, actStatus).ToList();
                    var serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    var resultData = students;
                    var result = new ContentResult
                    {
                        Content = serializer.Serialize(resultData),
                        ContentType = "application/json"
                    };
                    return result;
                }
                catch (Exception ex)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult AttendanceRequestView_Student(string UPRN, int Grp_No)
        {
            if (Session["Log_Id"] != null)
            {
                int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                int dep_id = Convert.ToInt32(Session["DepId"]);
                List<CMS_Stud_Att_Req> st = new List<CMS_Stud_Att_Req>();
                st = objfaculty.get_Student_From_AttReq(UPRN, Grp_No).ToList();
                ViewBag.Name = st[0].Name;
                ViewBag.Name = st[0].UPRN;

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Approve_Att_Req(List<CMS_Stud_Att_Req> jsonData, string UPRN, int Grp_No)
        {
            if (Session["Log_Id"] != null)
            {
                if (jsonData != null)
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Approve_Att_Req(jsonData, UPRN, Created_By, Grp_No);

                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
                // return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Approve_Att_Reqall(List<CMS_Stud_Att_Req> jsonData)
        {
            if (Session["Log_Id"] != null)
            {
                if (jsonData != null)
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Approve_Att_ReqAll(jsonData, Created_By);

                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
                // return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Reject_Att_Reqall(List<CMS_Stud_Att_Req> jsonData)
        {
            if (Session["Log_Id"] != null)
            {
                if (jsonData != null)
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Reject_Att_ReqAll(jsonData, Created_By);

                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
                // return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Reject_Att_Req(List<CMS_Stud_Att_Req> jsonData, string UPRN, int Grp_No)
        {
            if (Session["Log_Id"] != null)
            {
                if (jsonData != null)
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Reject_Att_Req(jsonData, UPRN, Created_By, Grp_No);

                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
                // return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Checked_Att_Req(List<CMS_Stud_Att_Req> jsonData, string UPRN, int Grp_No)
        {
            if (Session["Log_Id"] != null)
            {
                if (jsonData != null)
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Checked_Att_Req(jsonData, UPRN, Created_By, Grp_No);

                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
                // return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region FYUGP CCA

        public ActionResult CCA_EvaluationMethod(int Course_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    List<CCA> courses = objfaculty.getCourseDetails(Course_Sem_Id).ToList();
                    ViewBag.Course_Sem_Id = Course_Sem_Id;
                    return View(courses);
                }

                catch (Exception Ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult CCA_Distribution(int totalCO, int Course_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.EvalMethods = new SelectList(objfaculty.getEvaluationMethods(), "Eval_Id", "Method");
                    ViewBag.emDetails = objfaculty.getEMDetails(Course_Sem_Id, 0).ToList();
                    ViewBag.totalCO = totalCO;
                    ViewBag.Course_Sem_Id = Course_Sem_Id;
                    return PartialView();
                }

                catch (Exception Ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult CCA_MarkEntry(int Course_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid LogId = new Guid(Session["Log_Id"].ToString());
                    List<CCA> courses = objfaculty.getCourseDetails(Course_Sem_Id).ToList();
                    ViewBag.Course_Sem_Id = Course_Sem_Id;
                    ViewBag.emDistribution = objfaculty.getEMDetails(Course_Sem_Id, 0).ToList();
                    int Acc_Yr_sem_Id = courses.Select(x => x.Acc_Yr_sem_Id).FirstOrDefault();
                    CMS_Internal_MarkEntry_Schedule sch = db.CMS_Internal_MarkEntry_Schedules.Where(x => x.Acc_Yr_Sem_Id == Acc_Yr_sem_Id && x.Active_Status == true).FirstOrDefault();
                    Boolean Status = false;

                    if (sch != null)
                    {
                        if (sch.Start_Date <= DateTime.Now && sch.End_Date >= DateTime.Now)
                        {
                            Status = true;
                        }
                        //if (Acc_Yr_sem_Id == 116)
                        //{
                        //    int bca= (from a in db.CMS_AcademicYr_Sem_Programmes
                        //                join b in db.CMS_Course_Semesters on a.Acc_Yr_Sem_Pgm_Id equals b.Acc_Yr_Sem_Pgm_Id
                        //                where b.Course_Sem_Id == Course_Sem_Id && a.Acc_Yr_sem_Id == 116 && a.Pgm_Id == 13 select b.Course_Sem_Id).Count();
                        //    if (bca >0)
                        //    {
                        //        Status = true;
                        //    }
                        //    else
                        //    {
                        //        Status = false;
                        //    }
                        //}

                    }
                    ViewBag.Status = Status;
                    ViewBag.StudentMarks = objfaculty.getCCAMarks(Course_Sem_Id, 0, LogId).ToList();
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());

                    return View(courses);
                }

                catch (Exception Ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult CCA_StudentMark(int Course_Sem_Id, int Eval_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid LogId = new Guid(Session["Log_Id"].ToString());
                    List<CCA> courses = objfaculty.getCourseDetails(Course_Sem_Id).ToList();
                    ViewBag.Course_Sem_Id = Course_Sem_Id;
                    var list = objfaculty.getEMDetails(Course_Sem_Id, Eval_Id).ToList();
                    ViewBag.emDistribution = list;
                    ViewBag.MaxMark = list.Select(x => x.totalMark).FirstOrDefault();
                    ViewBag.EnteredMaxMarks = objfaculty.getenteredMaxMarks(Course_Sem_Id, Eval_Id).ToList();
                    ViewBag.Method = db.CCA_EvaluationMethods.Where(x => x.Eval_Id == Eval_Id).Select(x => x.Method).FirstOrDefault();
                    int Acc_Yr_sem_Id = courses.Select(x => x.Acc_Yr_sem_Id).FirstOrDefault();
                    CMS_Internal_MarkEntry_Schedule sch = db.CMS_Internal_MarkEntry_Schedules.Where(x => x.Acc_Yr_Sem_Id == Acc_Yr_sem_Id && x.Active_Status == true).FirstOrDefault();
                    Boolean Status = false;

                    if (sch != null)
                    {
                        if (sch.Start_Date <= DateTime.Now && sch.End_Date >= DateTime.Now)
                        {
                            Status = true;
                        }
                    }
                    ViewBag.Status = Status;
                    ViewBag.StudentMarks = objfaculty.getCCAMarks(Course_Sem_Id, Eval_Id, LogId).ToList();
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());

                    return View(courses);
                }

                catch (Exception Ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        [HttpPost]
        public ActionResult Insert_CCAMarks(List<CCA> ccaMarks)

        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid LogId = new Guid(Session["Log_Id"].ToString());
                    int val = objfaculty.Insert_CCAMarks(ccaMarks, LogId);
                    return Json(val, JsonRequestBehavior.AllowGet);
                }

                catch (Exception ex)
                {
                    return Json(ex.InnerException.ToString(), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CCA_FormA(int Course_Sem_Id)
        {

            if (Session["Log_Id"] != null)
            {

                try
                {

                    LocalReport lr = new LocalReport();
                    string path = Path.Combine(Server.MapPath("~/Report"), "CCA_FormA.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }

                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    List<CCA> dtls = objfaculty.getCCA_FormA(Course_Sem_Id).ToList();

                    List<CCA> courses = objfaculty.getCourseDetails(Course_Sem_Id).ToList();
                    var CO = dtls.Select(x => x.UPRN).Distinct().Count();
                    // COs for this CCA Component Type
                    var COs = dtls.Select(x => x.CONo).Distinct().ToList();

                    //Distribution
                    List<CCA> distribution = new List<CCA>();
                    var methods = dtls.GroupBy(x => x.Method).ToList();
                    foreach (var method in methods)
                    {
                        CCA m = new CCA();
                        m.Method = method.Key;
                        m.MaxMark = method.Sum(x => x.MaxMark);
                        COs = method.Select(x => x.CONo).Distinct().ToList();
                        m.methodCOs = string.Join(",", COs);
                        m.coMark = "";
                        foreach (var mt in method)
                        {
                            m.coMark += " CO" + mt.CONo + "=" + mt.MaxMark + ",";
                        }
                        m.coMark = m.coMark.Remove(m.coMark.Length - 1);
                        distribution.Add(m);
                    }

                    //Mapping
                    List<CCA> mapping = new List<CCA>();
                    var maps = dtls.GroupBy(x => x.CONo).ToList();
                    foreach (var map in maps)
                    {
                        CCA m = new CCA();
                        m.CONo = map.Key;
                        m.MaxMark = map.Select(x => new { x.MaxMark, x.Dis_Id }).Distinct().Sum(x => x.MaxMark);
                        var mds = map.Select(x => x.Method).Distinct().ToList();
                        m.Method = string.Join(",", mds);
                        mapping.Add(m);
                    }

                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "DataSet1";
                    reportDataSource.Value = dtls;
                    lr.DataSources.Add(reportDataSource);

                    ReportDataSource reportDataSource1 = new ReportDataSource();
                    reportDataSource1.Name = "DataSet2";
                    reportDataSource1.Value = mapping;
                    lr.DataSources.Add(reportDataSource1);

                    ReportDataSource reportDataSource2 = new ReportDataSource();
                    reportDataSource2.Name = "DataSet3";
                    reportDataSource2.Value = courses;
                    lr.DataSources.Add(reportDataSource2);
                    lr.Refresh();

                    List<ReportParameter> paraList = new List<ReportParameter>();
                    paraList.Add(new ReportParameter("CO", CO.ToString()));
                    lr.SetParameters(paraList.ToArray());

                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;
                    string deviceInfo =

                        "<DeviceInfo>" +

                        "<OutputFormat>" + reportType + "</OutputFormat>" +

                        "<PageWidth>11.5in</PageWidth>" +

                        "</DeviceInfo>";


                    Warning[] warning;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = lr.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warning);
                    //  return File(renderedBytes, "pdf");
                    return File(renderedBytes, mimeType);

                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }

        }
        #endregion

        #region INTERNSHIP
        public ActionResult CCA_Internship()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (isUserAuthenticated())
                    {


                        int facultyId = Convert.ToInt32(Session["Faculty_Id"].ToString());
                        bool isSupervisor = db.CCA_InternshipSupervisors
                             .Any(x => x.Fac_Id == facultyId && x.Active_Status);

                        if (isSupervisor || facultyId == 68)
                        {
                            return RedirectToAction(
                                "CCA_InternshipSupervisor",
                                "Faculty"   // change controller name if different
                            );
                        }
                        // 1️⃣ Load internship CCA components
                        var components = db.CCA_InternshipComponents
                                           .Where(c => c.IsForInternship == true && c.ComponentType == "CCA")
                                           .OrderBy(c => c.DisplayOrder)
                                           .ToList();

                        // 2️⃣ Get current academic year
                        int AccYr = objfaculty.getSpecificAcademicYear().Acc_yr_Id;

                        // 3️⃣ Get Academic Year + Semester ID for Semester IV
                        int accYrSemId = db.CMS_AccademicYearSemesters
                                            .Where(x => x.Sem_Id == 4 && x.Acc_yr_Id == AccYr)
                                            .Select(x => x.Acc_Yr_Sem_Id)
                                            .FirstOrDefault();

                        // 4️⃣ Load students assigned to the faculty (mentor mapping)
                        var studentData = (
                            from mentor in db.FYUGP_Internship_Mentors
                            join uprn in db.CMS_UPRNs on mentor.UPRN equals uprn.UPRN
                            join stu in db.CMS_Students on uprn.Admission_No equals stu.Admission_No
                            join pgm in db.CMS_Programmes on stu.Pgm_Id equals pgm.Pgm_Id
                            join sclass in db.CMS_StudentClasss on uprn.UPRN equals sclass.UPRN
                            join accpgm in db.CMS_AcademicYr_Sem_Programmes on sclass.Acc_Yr_Sem_Pgm_Id equals accpgm.Acc_Yr_Sem_Pgm_Id
                            where mentor.Faculty_Id == facultyId
                                  && mentor.Acc_Yr_Id == AccYr
                                  && mentor.Active_Status
                                  && accpgm.Acc_Yr_sem_Id == accYrSemId
                            select new
                            {
                                mentor.UPRN,
                                StudentName = stu.Name,
                                Programme = pgm.Pgm_Name,
                                Pgm_Id = stu.Pgm_Id,
                                IPO = mentor.IPO,
                                mentor.Proposal_Form

                            }
                        ).Distinct().ToList();

                        // 5️⃣ Build ViewModel for each student
                        var students = studentData
     .Select(s => new CCA.StudentInternshipMarkRow
     {
         UPRN = s.UPRN,
         StudentName = s.StudentName,
         Programme = s.Programme,
         IPO = s.IPO,
         Proposal_Form = s.Proposal_Form,

         ComponentMarks = components.Select(c =>
         {
             var savedMark = db.CCA_InternshipMarks
                 .FirstOrDefault(m =>
                     m.UPRN == s.UPRN &&
                     m.ComponentId == c.ComponentId &&
                     m.Acc_Yr_Sem_Id == accYrSemId);

             return new CCA.ComponentMarkEntry
             {
                 ComponentId = c.ComponentId,
                 MaxMark = (s.Pgm_Id == 13) ? c.MaxMark * 2 : c.MaxMark,

                 // 🔑 Marks
                 MarkAwarded = savedMark != null ? savedMark.MarkAwarded : 0m,

                 // 🔑 Status flags for mentor edit flow
                 IsSubmitted = savedMark != null && savedMark.Is_Submitted,
                 ReviewStatus = savedMark != null ? savedMark.Review_Status : "Draft"
             };
         }).ToList()
     })
     .ToList();

                        var studentPgmIds = studentData.Select(x => x.Pgm_Id).Distinct().ToList();

                        if (studentPgmIds.Count == 1 && studentPgmIds.Contains(13))
                        {
                            components.ForEach(c => c.MaxMark = c.MaxMark * 2);
                        }
                        // 6️⃣ Send data to View
                        CCA.InternshipCCAMarkViewModel vm = new CCA.InternshipCCAMarkViewModel
                        {
                            Acc_Yr_Sem_Id = accYrSemId,
                            FacultyId = facultyId,
                            Components = components,
                            Students = students
                        };

                        return View(vm);

                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }
                }

                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult SaveInternshipSingleRow(CCA.InternshipCCAMarkViewModel model, int SingleRowIndex)
        {
            try
            {
                if (model == null)
                {
                    return Json(new { success = false, message = "Model is null." });
                }

                if (Session["Log_Id"] == null)
                {
                    return Json(new { success = false, message = "Session expired. Please login again." });
                }

                if (model.Students == null || !model.Students.Any())
                {
                    return Json(new { success = false, message = "Student data not received." });
                }
                int facultyId = model.FacultyId;
                int accYrSemId = model.Acc_Yr_Sem_Id;
                Guid logid = new Guid(Session["Log_Id"].ToString());
                // ----------- VALIDATE INDEX ----------
                var row = model.Students.FirstOrDefault();

                if (row == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Unable to read student data."
                    });
                }
                bool isFinalized = db.CCA_InternshipMarks.Any(m =>
                                   m.Acc_Yr_Sem_Id == accYrSemId &&
                                   m.UPRN == row.UPRN &&
                                   m.Is_Submitted);

                if (isFinalized)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Marks already finalized by supervisor."
                    });
                }
                // ----------- VALIDATE STUDENT ----------
                if (string.IsNullOrWhiteSpace(row.UPRN))
                {
                    return Json(new { success = false, message = "Student UPRN missing." });
                }

                // ----------- VALIDATE COMPONENT LIST ----------
                if (row.ComponentMarks == null || row.ComponentMarks.Count == 0)
                {
                    return Json(new { success = false, message = "No component marks found to save." });
                }
                int id = db.CCA_InternshipMarks.Count();
                foreach (var comp in row.ComponentMarks)
                {
                    // Load component from DB
                    var dbComp = db.CCA_InternshipComponents
                                   .FirstOrDefault(c => c.ComponentId == comp.ComponentId && c.ComponentType == "CCA");

                    if (dbComp == null)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Component ID " + comp.ComponentId + " does not exist."
                        });
                    }

                    // ----------- VALIDATION: MARK >= 0 ----------
                    if (comp.MarkAwarded < 0)
                    {
                        return Json(new
                        {
                            success = false,
                            message = dbComp.ComponentName + ": marks cannot be negative."
                        });
                    }

                    // ----------- VALIDATION: MARK <= MAX ----------
                    //if (comp.MarkAwarded > dbComp.MaxMark)
                    //{
                    //    return Json(new
                    //    {
                    //        success = false,
                    //        message = dbComp.ComponentName +": cannot exceed max "+dbComp.MaxMark+"."
                    //    });
                    //}

                    // ----------- SAVE / UPDATE ----------
                    var existing = db.CCA_InternshipMarks
                                     .FirstOrDefault(m =>
                                         m.UPRN == row.UPRN &&
                                         m.ComponentId == comp.ComponentId &&
                                         m.Acc_Yr_Sem_Id == accYrSemId);

                    if (existing != null)
                    {
                        existing.MarkAwarded = comp.MarkAwarded;
                        existing.Updated_By = logid;
                        existing.Updated_Date = DateTime.Now;
                        existing.Review_Status = "Draft";
                    }
                    else
                    {
                        id++;
                        db.CCA_InternshipMarks.Add(new CCA_InternshipMark
                        {
                            Id = id,
                            UPRN = row.UPRN,
                            ComponentId = comp.ComponentId,
                            MarkAwarded = comp.MarkAwarded,
                            FacultyId = facultyId,
                            Acc_Yr_Sem_Id = accYrSemId,
                            Review_Status = "Draft",

                            Created_By = logid,
                            Created_Date = DateTime.Now,
                            Updated_By = logid,
                            Updated_Date = DateTime.Now
                        });
                    }
                }
                var mentor = db.FYUGP_Internship_Mentors
       .FirstOrDefault(x => x.UPRN == row.UPRN && x.Faculty_Id == model.FacultyId);

                if (mentor != null)
                {
                    mentor.IPO = row.IPO;

                    // 🔹 FILE UPLOAD
                    if (row.CompletionFile != null && row.CompletionFile.ContentLength > 0)
                    {
                        string extension = Path.GetExtension(row.CompletionFile.FileName);

                        string fileName = row.UPRN + extension;

                        string folderPath = Server.MapPath("~/Images/Internship/");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        string fullPath = Path.Combine(folderPath, fileName);

                        // 🔁 Replace existing file
                        row.CompletionFile.SaveAs(fullPath);

                        // Save filename in DB
                        mentor.Proposal_Form = fileName;
                    }
                }
                db.SaveChanges();

                return Json(new { success = true, message = "Marks saved successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveInternshipMarks(CCA.InternshipCCAMarkViewModel model)
        {
            try
            {
                if (Session["Log_Id"] == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Session expired. Please login again."
                    });
                }

                if (model == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No data received."
                    });
                }

                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid data submitted."
                    });
                }

                if (model.Students == null || !model.Students.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "No students found."
                    });
                }

                Guid logid = Guid.Parse(Session["Log_Id"].ToString());

                var allComponentIds = model.Students
                    .Where(s => s.ComponentMarks != null)
                    .SelectMany(s => s.ComponentMarks)
                    .Select(c => c.ComponentId)
                    .Distinct()
                    .ToList();

                var dbComponents = db.CCA_InternshipComponents
                    .Where(c => allComponentIds.Contains(c.ComponentId))
                    .ToDictionary(c => c.ComponentId);

                int id = db.CCA_InternshipMarks.Any()
                    ? db.CCA_InternshipMarks.Max(x => x.Id)
                    : 0;

                foreach (var student in model.Students)
                {
                    if (string.IsNullOrWhiteSpace(student.UPRN))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Student UPRN missing."
                        });
                    }

                    bool isFinalized = db.CCA_InternshipMarks.Any(m =>
                        m.UPRN == student.UPRN &&
                        m.Acc_Yr_Sem_Id == model.Acc_Yr_Sem_Id &&
                        m.Is_Submitted);

                    if (isFinalized)
                    {
                        return Json(new
                        {
                            success = false,
                            message = student.UPRN + " has already been finalized."
                        });
                    }

                    if (student.ComponentMarks == null || !student.ComponentMarks.Any())
                    {
                        continue;
                    }

                    foreach (var comp in student.ComponentMarks)
                    {
                        CCA_InternshipComponent dbComp;

                        if (!dbComponents.TryGetValue(comp.ComponentId, out dbComp))
                        {
                            return Json(new
                            {
                                success = false,
                                message = "Invalid component found."
                            });
                        }

                        if (comp.MarkAwarded < 0)
                        {
                            return Json(new
                            {
                                success = false,
                                message = dbComp.ComponentName +
                                          ": marks cannot be negative."
                            });
                        }

                        // Uncomment if needed
                        /*
                        if (comp.MarkAwarded > dbComp.MaxMark)
                        {
                            return Json(new
                            {
                                success = false,
                                message = dbComp.ComponentName +
                                          ": max " + dbComp.MaxMark
                            });
                        }
                        */

                        var existing = db.CCA_InternshipMarks.FirstOrDefault(m =>
                            m.UPRN == student.UPRN &&
                            m.ComponentId == comp.ComponentId &&
                            m.Acc_Yr_Sem_Id == model.Acc_Yr_Sem_Id);

                        if (existing != null)
                        {
                            existing.MarkAwarded = comp.MarkAwarded;
                            existing.Updated_Date = DateTime.Now;
                            existing.Updated_By = logid;
                            existing.Review_Status = "Draft";
                        }
                        else
                        {
                            id++;

                            db.CCA_InternshipMarks.Add(new CCA_InternshipMark
                            {
                                Id = id,
                                UPRN = student.UPRN,
                                ComponentId = comp.ComponentId,
                                MarkAwarded = comp.MarkAwarded,
                                FacultyId = model.FacultyId,
                                Acc_Yr_Sem_Id = model.Acc_Yr_Sem_Id,
                                Created_Date = DateTime.Now,
                                Updated_Date = DateTime.Now,
                                Created_By = logid,
                                Updated_By = logid,
                                Review_Status = "Draft"
                            });
                        }
                    }

                    var mentor = db.FYUGP_Internship_Mentors
                        .FirstOrDefault(x =>
                            x.UPRN == student.UPRN &&
                            x.Faculty_Id == model.FacultyId);

                    if (mentor != null)
                    {
                        mentor.IPO = student.IPO;

                        if (student.CompletionFile != null &&
                            student.CompletionFile.ContentLength > 0)
                        {
                            if (student.CompletionFile.ContentLength >
                                (3 * 1024 * 1024))
                            {
                                return Json(new
                                {
                                    success = false,
                                    message = "File size should be less than 3 MB for UPRN: "
                                              + student.UPRN
                                });
                            }

                            string extension = Path.GetExtension(
                                student.CompletionFile.FileName);

                            string fileName = student.UPRN + extension;

                            string folderPath = Server.MapPath("~/Images/Internship/");

                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }

                            string fullPath = Path.Combine(folderPath, fileName);

                            student.CompletionFile.SaveAs(fullPath);

                            mentor.Proposal_Form = fileName;
                        }
                    }
                }

                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "All marks saved successfully!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
        [HttpPost]
        public ActionResult SubmitFinalInternshipMarks()
        {
            int supervisorId = Convert.ToInt32(Session["Faculty_Id"]);
            int AccYr = 9;
            //objfaculty.getCurrentAcademicYear().Acc_yr_Id;
            int accYrSemId = db.CMS_AccademicYearSemesters
                                   .Where(x => x.Sem_Id == 4 && x.Acc_yr_Id == AccYr)
                                   .Select(x => x.Acc_Yr_Sem_Id)
                                   .FirstOrDefault();
            var marks = (
                   from mentor in db.FYUGP_Internship_Mentors
                   join sclass in db.CMS_StudentClasss on mentor.UPRN equals sclass.UPRN
                   join accpgm in db.CMS_AcademicYr_Sem_Programmes
                   on sclass.Acc_Yr_Sem_Pgm_Id equals accpgm.Acc_Yr_Sem_Pgm_Id
                   join sup in db.CCA_InternshipSupervisors on accpgm.Pgm_Id equals sup.Pgm_Id
                   join intmrk in db.CCA_InternshipMarks
                   on mentor.UPRN equals intmrk.UPRN
                   where sup.Fac_Id == supervisorId
                       && mentor.Acc_Yr_Id == AccYr
                       && mentor.Active_Status
                       && accpgm.Acc_Yr_sem_Id == accYrSemId
                       && intmrk.Review_Status == "Approved"
                   select

               intmrk).ToList();

            if (!marks.Any())
                return Json(new { success = false, message = "No marks found." });

            foreach (var m in marks)
            {
                m.Is_Submitted = true;
                m.Submitted_By = supervisorId;
                m.Submitted_Date = DateTime.Now;
            }

            db.SaveChanges();

            return Json(new { success = true, message = "Final marks submitted successfully." });
        }

        public ActionResult CCA_InternshipSupervisor()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (isUserAuthenticated())
                    {
                        int facultyId = Convert.ToInt32(Session["Faculty_Id"].ToString());

                        // 1️⃣ Load internship CCA components
                        var components = db.CCA_InternshipComponents
                                           .Where(c => c.IsForInternship == true && c.ComponentType == "CCA")
                                           .OrderBy(c => c.DisplayOrder)
                                           .ToList();

                        // 2️⃣ Get current academic year
                        int AccYr = 9;
                        //objfaculty.getCurrentAcademicYear().Acc_yr_Id;

                        // 3️⃣ Get Academic Year + Semester ID for Semester IV
                        int accYrSemId = db.CMS_AccademicYearSemesters
                                            .Where(x => x.Sem_Id == 4 && x.Acc_yr_Id == AccYr)
                                            .Select(x => x.Acc_Yr_Sem_Id)
                                            .FirstOrDefault();
                        var allMarks = db.CCA_InternshipMarks
                                        .Where(m => m.Acc_Yr_Sem_Id == accYrSemId)
                                        .ToList();
                        // 4️⃣ Load students assigned to the faculty (mentor mapping)
                        var studentData = (
                    from mentor in db.FYUGP_Internship_Mentors
                    join fac in db.CMS_Facultys on mentor.Faculty_Id equals fac.Faculty_Id
                    join uprn in db.CMS_UPRNs on mentor.UPRN equals uprn.UPRN
                    join stu in db.CMS_Students on uprn.Admission_No equals stu.Admission_No
                    join pgm in db.CMS_Programmes on stu.Pgm_Id equals pgm.Pgm_Id
                    join sclass in db.CMS_StudentClasss on uprn.UPRN equals sclass.UPRN
                    join accpgm in db.CMS_AcademicYr_Sem_Programmes
                    on sclass.Acc_Yr_Sem_Pgm_Id equals accpgm.Acc_Yr_Sem_Pgm_Id
                    join sup in db.CCA_InternshipSupervisors
                    on accpgm.Pgm_Id equals sup.Pgm_Id
                    where (facultyId == 68 || sup.Fac_Id == facultyId)
                        && mentor.Acc_Yr_Id == AccYr
                        && mentor.Active_Status
                        && accpgm.Acc_Yr_sem_Id == accYrSemId
                    select new
                    {
                        uprn.UPRN,
                        StudentName = stu.Name,
                        Programme = pgm.Pgm_Name,
                        Pgm_Id = pgm.Pgm_Id,
                        MentorName = fac.Name,  // 👈 Mentor
                        mentor.IPO,
                        mentor.Proposal_Form
                    }
                )
                .GroupBy(x => x.UPRN)
                .Select(g => g.FirstOrDefault())
                .ToList();

                        // 5️⃣ Build ViewModel for each student
                        var students = studentData.Select(s => new CCA.StudentInternshipMarkRow
                        {
                            UPRN = s.UPRN,
                            StudentName = s.StudentName,
                            Programme = s.Programme,
                            MentorName = s.MentorName,

                            IPO = s.IPO,
                            Proposal_Form = s.Proposal_Form,
                            ComponentMarks = components.Select(c => new CCA.ComponentMarkEntry
                            {
                                ComponentId = c.ComponentId,
                                MaxMark = (s.Pgm_Id == 13) ? c.MaxMark * 2 : c.MaxMark,

                                MarkAwarded = allMarks
                                    .Where(m => m.UPRN == s.UPRN && m.ComponentId == c.ComponentId)
                                    .Select(m => m.MarkAwarded)
                                    .FirstOrDefault()
                            }).ToList(),

                            IsSubmitted = allMarks.Any(m =>
                                m.UPRN == s.UPRN && m.Is_Submitted),
                            ReviewStatus = db.CCA_InternshipMarks
                                            .Where(m => m.UPRN == s.UPRN && m.Acc_Yr_Sem_Id == accYrSemId)
                                            .Select(m => m.Review_Status)
                                             .FirstOrDefault() ?? "Draft",
                        }).ToList();
                        var studentPgmIds = studentData.Select(x => x.Pgm_Id).Distinct().ToList();

                        if (studentPgmIds.Count == 1 && studentPgmIds.Contains(13))
                        {
                            components.ForEach(c => c.MaxMark = c.MaxMark * 2);
                        }
                        // 6️⃣ Send data to View
                        CCA.InternshipCCAMarkViewModel vm = new CCA.InternshipCCAMarkViewModel
                        {
                            Acc_Yr_Sem_Id = accYrSemId,
                            FacultyId = facultyId,
                            Components = components,
                            Students = students
                        };

                        return View(vm);

                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }
                }

                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult RejectStudent(string uprn, int accYrSemId)
        {
            var records = db.CCA_InternshipMarks
                .Where(x => x.UPRN == uprn && x.Acc_Yr_Sem_Id == accYrSemId)
                .ToList();

            if (!records.Any())
            {
                return Json(new { success = false, message = "No marks found." });
            }

            records.ForEach(r =>
            {
                r.Review_Status = "Rejected";
                r.Updated_Date = DateTime.Now;
            });

            db.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Student marks rejected and sent back to mentor."
            });
        }
        [HttpPost]
        public JsonResult ApproveSingleStudent(string uprn)
        {
            try
            {
                var records = db.CCA_InternshipMarks
                                .Where(x => x.UPRN == uprn)
                                .ToList();

                foreach (var r in records)
                {

                    r.Review_Status = "Approved";
                    r.Submitted_Date = DateTime.Now;
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ApproveAllStudents()
        {
            try
            {
                int facultyId = Convert.ToInt32(Session["Faculty_Id"].ToString());


                // 2️⃣ Get current academic year
                int AccYr = 9;
                //objfaculty.getCurrentAcademicYear().Acc_yr_Id;

                // 3️⃣ Get Academic Year + Semester ID for Semester IV
                int accYrSemId = db.CMS_AccademicYearSemesters
                                    .Where(x => x.Sem_Id == 4 && x.Acc_yr_Id == AccYr)
                                    .Select(x => x.Acc_Yr_Sem_Id)
                                    .FirstOrDefault();
                var records = (
                   from mentor in db.FYUGP_Internship_Mentors
                   join sclass in db.CMS_StudentClasss on mentor.UPRN equals sclass.UPRN
                   join accpgm in db.CMS_AcademicYr_Sem_Programmes on sclass.Acc_Yr_Sem_Pgm_Id equals accpgm.Acc_Yr_Sem_Pgm_Id
                   join sup in db.CCA_InternshipSupervisors on accpgm.Pgm_Id equals sup.Pgm_Id
                   join intmrk in db.CCA_InternshipMarks on mentor.UPRN equals intmrk.UPRN
                   where sup.Fac_Id == facultyId
                       && mentor.Acc_Yr_Id == AccYr
                       && mentor.Active_Status
                       //&& accpgm.Acc_Yr_sem_Id == accYrSemId
                       && intmrk.Review_Status == "Draft"
                   select

               intmrk).ToList();

                foreach (var r in records)
                {

                    r.Review_Status = "Approved";
                    r.Submitted_Date = DateTime.Now;
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        public ActionResult ESE_Internship()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (isUserAuthenticated())
                    {


                        int facultyId = Convert.ToInt32(Session["Faculty_Id"].ToString());
                        bool isSupervisor = db.CCA_InternshipSupervisors
                             .Any(x => x.Fac_Id == facultyId && x.Active_Status);

                        if (isSupervisor || facultyId == 68)
                        {
                            return RedirectToAction(
                                "ESE_InternshipSupervisor",
                                "Faculty"   // change controller name if different
                            );
                        }
                        // 1️⃣ Load internship CCA components
                        var components = db.CCA_InternshipComponents
                                           .Where(c => c.IsForInternship == true && c.ComponentType == "ESE")
                                           .OrderBy(c => c.DisplayOrder)
                                           .ToList();

                        // 2️⃣ Get current academic year
                        int AccYr = objfaculty.getSpecificAcademicYear().Acc_yr_Id;

                        // 3️⃣ Get Academic Year + Semester ID for Semester IV
                        int accYrSemId = db.CMS_AccademicYearSemesters
                                            .Where(x => x.Sem_Id == 4 && x.Acc_yr_Id == AccYr)
                                            .Select(x => x.Acc_Yr_Sem_Id)
                                            .FirstOrDefault();

                        // 4️⃣ Load students assigned to the faculty (mentor mapping)
                        var studentData = (
                            from mentor in db.FYUGP_Internship_Mentors
                            join uprn in db.CMS_UPRNs on mentor.UPRN equals uprn.UPRN
                            join stu in db.CMS_Students on uprn.Admission_No equals stu.Admission_No
                            join pgm in db.CMS_Programmes on stu.Pgm_Id equals pgm.Pgm_Id
                            join sclass in db.CMS_StudentClasss on uprn.UPRN equals sclass.UPRN
                            join accpgm in db.CMS_AcademicYr_Sem_Programmes on sclass.Acc_Yr_Sem_Pgm_Id equals accpgm.Acc_Yr_Sem_Pgm_Id
                            where mentor.Faculty_Id == facultyId
                                  && mentor.Acc_Yr_Id == AccYr
                                  && mentor.Active_Status
                                  && accpgm.Acc_Yr_sem_Id == accYrSemId
                            select new
                            {
                                mentor.UPRN,
                                StudentName = stu.Name,
                                Programme = pgm.Pgm_Name,
                                Pgm_Id = pgm.Pgm_Id,
                                IPO = mentor.IPO,
                                mentor.Completion_Letter

                            }
                        ).Distinct().ToList();

                        // 5️⃣ Build ViewModel for each student
                        var students = studentData
     .Select(s => new CCA.StudentInternshipMarkRow
     {
         UPRN = s.UPRN,
         StudentName = s.StudentName,
         Programme = s.Programme,
         IPO = s.IPO,
         Completion_Letter = s.Completion_Letter,

         ComponentMarks = components.Select(c =>
         {
             var savedMark = db.ESE_InternshipMarks
                 .FirstOrDefault(m =>
                     m.UPRN == s.UPRN &&
                     m.ComponentId == c.ComponentId &&
                     m.Acc_Yr_Sem_Id == accYrSemId);

             return new CCA.ComponentMarkEntry
             {
                 ComponentId = c.ComponentId,
                 MaxMark = (s.Pgm_Id == 13) ? c.MaxMark * 2 : c.MaxMark,

                 // 🔑 Marks
                 MarkAwarded = savedMark != null ? savedMark.MarkAwarded : 0m,

                 // 🔑 Status flags for mentor edit flow
                 IsSubmitted = savedMark != null && savedMark.Is_Submitted,
                 ReviewStatus = savedMark != null ? savedMark.Review_Status : "Draft"
             };
         }).ToList()
     })
     .ToList();
                        var studentPgmIds = studentData.Select(x => x.Pgm_Id).Distinct().ToList();

                        if (studentPgmIds.Count == 1 && studentPgmIds.Contains(13))
                        {
                            components.ForEach(c => c.MaxMark = c.MaxMark * 2);
                        }

                        // 6️⃣ Send data to View
                        CCA.InternshipCCAMarkViewModel vm = new CCA.InternshipCCAMarkViewModel
                        {
                            Acc_Yr_Sem_Id = accYrSemId,
                            FacultyId = facultyId,
                            Components = components,
                            Students = students
                        };

                        return View(vm);

                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }
                }

                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]

        public ActionResult ESE_SaveInternshipSingleRow(CCA.InternshipCCAMarkViewModel model, int SingleRowIndex)
        {
            try
            {
                if (Session["Log_Id"] == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Session expired. Please login again."
                    });
                }

                if (model == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No data received."
                    });
                }

                if (model.Students == null || !model.Students.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Student data not received."
                    });
                }

                Guid logid = Guid.Parse(Session["Log_Id"].ToString());

                int facultyId = model.FacultyId;
                int accYrSemId = model.Acc_Yr_Sem_Id;

                // Since only one row is posted
                var row = model.Students.FirstOrDefault();

                if (row == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Unable to read student data."
                    });
                }

                bool isFinalized = db.ESE_InternshipMarks.Any(m =>
                    m.Acc_Yr_Sem_Id == accYrSemId &&
                    m.UPRN == row.UPRN &&
                    m.Is_Submitted);

                if (isFinalized)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Marks already finalized by supervisor."
                    });
                }

                if (string.IsNullOrWhiteSpace(row.UPRN))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Student UPRN missing."
                    });
                }

                if (row.ComponentMarks == null || !row.ComponentMarks.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "No component marks found to save."
                    });
                }

                int id = db.ESE_InternshipMarks.Any()
                    ? db.ESE_InternshipMarks.Max(x => x.Id)
                    : 0;

                foreach (var comp in row.ComponentMarks)
                {
                    var dbComp = db.CCA_InternshipComponents.FirstOrDefault(c =>
                        c.ComponentId == comp.ComponentId &&
                        c.ComponentType == "ESE");

                    if (dbComp == null)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Component ID " + comp.ComponentId + " does not exist."
                        });
                    }

                    if (comp.MarkAwarded < 0)
                    {
                        return Json(new
                        {
                            success = false,
                            message = dbComp.ComponentName +
                                      ": marks cannot be negative."
                        });
                    }

                    /*
                    if (comp.MarkAwarded > dbComp.MaxMark)
                    {
                        return Json(new
                        {
                            success = false,
                            message = dbComp.ComponentName +
                                      ": cannot exceed max " + dbComp.MaxMark
                        });
                    }
                    */

                    var existing = db.ESE_InternshipMarks.FirstOrDefault(m =>
                        m.UPRN == row.UPRN &&
                        m.ComponentId == comp.ComponentId &&
                        m.Acc_Yr_Sem_Id == accYrSemId);

                    if (existing != null)
                    {
                        existing.MarkAwarded = comp.MarkAwarded;
                        existing.Updated_By = logid;
                        existing.Updated_Date = DateTime.Now;
                        existing.Review_Status = "Draft";
                    }
                    else
                    {
                        id++;

                        db.ESE_InternshipMarks.Add(new ESE_InternshipMark
                        {
                            Id = id,
                            UPRN = row.UPRN,
                            ComponentId = comp.ComponentId,
                            MarkAwarded = comp.MarkAwarded,
                            FacultyId = facultyId,
                            Acc_Yr_Sem_Id = accYrSemId,
                            Review_Status = "Draft",

                            Created_By = logid,
                            Created_Date = DateTime.Now,
                            Updated_By = logid,
                            Updated_Date = DateTime.Now
                        });
                    }
                }

                var mentor = db.FYUGP_Internship_Mentors
                    .FirstOrDefault(x =>
                        x.UPRN == row.UPRN &&
                        x.Faculty_Id == facultyId);

                if (mentor != null)
                {
                    mentor.IPO = row.IPO;

                    if (row.CompletionFile != null &&
                        row.CompletionFile.ContentLength > 0)
                    {
                        if (row.CompletionFile.ContentLength >
                            (3 * 1024 * 1024))
                        {
                            return Json(new
                            {
                                success = false,
                                message = "File size should be less than 3 MB."
                            });
                        }

                        string extension = Path.GetExtension(
                            row.CompletionFile.FileName);

                        string fileName = "completion_" +
                                          row.UPRN +
                                          extension;

                        string folderPath = Server.MapPath("~/Images/Internship/");

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        string fullPath = Path.Combine(folderPath, fileName);

                        row.CompletionFile.SaveAs(fullPath);

                        mentor.Completion_Letter = fileName;
                    }
                }

                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Marks saved successfully!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ESE_SaveInternshipMarks(CCA.InternshipCCAMarkViewModel model)
        {
            try
            {
                if (Session["Log_Id"] == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Session expired. Please login again."
                    });
                }

                if (model == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No data received."
                    });
                }

                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid data submitted."
                    });
                }

                if (model.Students == null || !model.Students.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "No students found."
                    });
                }

                Guid logid = Guid.Parse(Session["Log_Id"].ToString());

                var allComponentIds = model.Students
                    .Where(s => s.ComponentMarks != null)
                    .SelectMany(s => s.ComponentMarks)
                    .Select(c => c.ComponentId)
                    .Distinct()
                    .ToList();

                var dbComponents = db.CCA_InternshipComponents
                    .Where(c =>
                        allComponentIds.Contains(c.ComponentId) &&
                        c.ComponentType == "ESE")
                    .ToDictionary(c => c.ComponentId);

                int id = db.ESE_InternshipMarks.Any()
                    ? db.ESE_InternshipMarks.Max(x => x.Id)
                    : 0;

                foreach (var student in model.Students)
                {
                    if (string.IsNullOrWhiteSpace(student.UPRN))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Student UPRN missing."
                        });
                    }

                    bool isFinalized = db.ESE_InternshipMarks.Any(m =>
                        m.UPRN == student.UPRN &&
                        m.Acc_Yr_Sem_Id == model.Acc_Yr_Sem_Id &&
                        m.Is_Submitted);

                    if (isFinalized)
                    {
                        return Json(new
                        {
                            success = false,
                            message = student.UPRN + " has already been finalized."
                        });
                    }

                    if (student.ComponentMarks == null || !student.ComponentMarks.Any())
                    {
                        continue;
                    }

                    foreach (var comp in student.ComponentMarks)
                    {
                        CCA_InternshipComponent dbComp;

                        if (!dbComponents.TryGetValue(comp.ComponentId, out dbComp))
                        {
                            return Json(new
                            {
                                success = false,
                                message = "Invalid component found."
                            });
                        }

                        if (comp.MarkAwarded < 0)
                        {
                            return Json(new
                            {
                                success = false,
                                message = dbComp.ComponentName +
                                          ": marks cannot be negative."
                            });
                        }

                        /*
                        if (comp.MarkAwarded > dbComp.MaxMark)
                        {
                            return Json(new
                            {
                                success = false,
                                message = dbComp.ComponentName +
                                          ": max " + dbComp.MaxMark
                            });
                        }
                        */

                        var existing = db.ESE_InternshipMarks.FirstOrDefault(m =>
                            m.UPRN == student.UPRN &&
                            m.ComponentId == comp.ComponentId &&
                            m.Acc_Yr_Sem_Id == model.Acc_Yr_Sem_Id);

                        if (existing != null)
                        {
                            existing.MarkAwarded = comp.MarkAwarded;
                            existing.Updated_Date = DateTime.Now;
                            existing.Updated_By = logid;
                            existing.Review_Status = "Draft";
                        }
                        else
                        {
                            id++;

                            db.ESE_InternshipMarks.Add(new ESE_InternshipMark
                            {
                                Id = id,
                                UPRN = student.UPRN,
                                ComponentId = comp.ComponentId,
                                MarkAwarded = comp.MarkAwarded,
                                FacultyId = model.FacultyId,
                                Acc_Yr_Sem_Id = model.Acc_Yr_Sem_Id,
                                Created_Date = DateTime.Now,
                                Updated_Date = DateTime.Now,
                                Created_By = logid,
                                Updated_By = logid,
                                Review_Status = "Draft"
                            });
                        }
                    }

                    var mentor = db.FYUGP_Internship_Mentors
                        .FirstOrDefault(x =>
                            x.UPRN == student.UPRN &&
                            x.Faculty_Id == model.FacultyId);

                    if (mentor != null)
                    {
                        mentor.IPO = student.IPO;

                        if (student.CompletionFile != null &&
                            student.CompletionFile.ContentLength > 0)
                        {
                            if (student.CompletionFile.ContentLength >
                                (3 * 1024 * 1024))
                            {
                                return Json(new
                                {
                                    success = false,
                                    message = "File size should be less than 3 MB for UPRN: "
                                              + student.UPRN
                                });
                            }

                            string extension = Path.GetExtension(
                                student.CompletionFile.FileName);

                            string fileName = "completion_" +
                                              student.UPRN +
                                              extension;

                            string folderPath = Server.MapPath("~/Images/Internship/");

                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }

                            string fullPath = Path.Combine(folderPath, fileName);

                            student.CompletionFile.SaveAs(fullPath);

                            mentor.Completion_Letter = fileName;
                        }
                    }
                }

                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "All marks saved successfully!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        [HttpPost]
        public ActionResult ESE_SubmitFinalInternshipMarks()
        {
            int supervisorId = Convert.ToInt32(Session["Faculty_Id"]);
            int AccYr = 9;
            //objfaculty.getCurrentAcademicYear().Acc_yr_Id;
            int accYrSemId = db.CMS_AccademicYearSemesters
                                   .Where(x => x.Sem_Id == 4 && x.Acc_yr_Id == AccYr)
                                   .Select(x => x.Acc_Yr_Sem_Id)
                                   .FirstOrDefault();
            var marks = (
                   from mentor in db.FYUGP_Internship_Mentors
                   join sclass in db.CMS_StudentClasss on mentor.UPRN equals sclass.UPRN
                   join accpgm in db.CMS_AcademicYr_Sem_Programmes
                   on sclass.Acc_Yr_Sem_Pgm_Id equals accpgm.Acc_Yr_Sem_Pgm_Id
                   join sup in db.CCA_InternshipSupervisors on accpgm.Pgm_Id equals sup.Pgm_Id
                   join intmrk in db.ESE_InternshipMarks
                   on mentor.UPRN equals intmrk.UPRN
                   where sup.Fac_Id == supervisorId
                       && mentor.Acc_Yr_Id == AccYr
                       && mentor.Active_Status
                       && accpgm.Acc_Yr_sem_Id == accYrSemId
                       && intmrk.Review_Status == "Approved"
                   select

               intmrk).ToList();

            if (!marks.Any())
                return Json(new { success = false, message = "No marks found." });

            foreach (var m in marks)
            {
                m.Is_Submitted = true;
                m.Submitted_By = supervisorId;
                m.Submitted_Date = DateTime.Now;
            }

            db.SaveChanges();

            return Json(new { success = true, message = "Final marks submitted successfully." });
        }

        public ActionResult ESE_InternshipSupervisor()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (isUserAuthenticated())
                    {
                        int facultyId = Convert.ToInt32(Session["Faculty_Id"].ToString());

                        // 1️⃣ Load internship CCA components
                        var components = db.CCA_InternshipComponents
                                           .Where(c => c.IsForInternship == true && c.ComponentType == "ESE")
                                           .OrderBy(c => c.DisplayOrder)
                                           .ToList();

                        // 2️⃣ Get current academic year
                        int AccYr = 9;
                        //objfaculty.getCurrentAcademicYear().Acc_yr_Id;

                        // 3️⃣ Get Academic Year + Semester ID for Semester IV
                        int accYrSemId = db.CMS_AccademicYearSemesters
                                            .Where(x => x.Sem_Id == 4 && x.Acc_yr_Id == AccYr)
                                            .Select(x => x.Acc_Yr_Sem_Id)
                                            .FirstOrDefault();
                        var allMarks = db.ESE_InternshipMarks
                                        .Where(m => m.Acc_Yr_Sem_Id == accYrSemId)
                                        .ToList();
                        // 4️⃣ Load students assigned to the faculty (mentor mapping)
                        var studentData = (
                    from mentor in db.FYUGP_Internship_Mentors
                    join fac in db.CMS_Facultys on mentor.Faculty_Id equals fac.Faculty_Id
                    join uprn in db.CMS_UPRNs on mentor.UPRN equals uprn.UPRN
                    join stu in db.CMS_Students on uprn.Admission_No equals stu.Admission_No
                    join pgm in db.CMS_Programmes on stu.Pgm_Id equals pgm.Pgm_Id
                    join sclass in db.CMS_StudentClasss on uprn.UPRN equals sclass.UPRN
                    join accpgm in db.CMS_AcademicYr_Sem_Programmes
                    on sclass.Acc_Yr_Sem_Pgm_Id equals accpgm.Acc_Yr_Sem_Pgm_Id
                    join sup in db.CCA_InternshipSupervisors
                    on accpgm.Pgm_Id equals sup.Pgm_Id
                    where (facultyId == 68 || sup.Fac_Id == facultyId)
                        && mentor.Acc_Yr_Id == AccYr
                        && mentor.Active_Status
                        && accpgm.Acc_Yr_sem_Id == accYrSemId
                    select new
                    {
                        uprn.UPRN,
                        StudentName = stu.Name,
                        Programme = pgm.Pgm_Name,
                        MentorName = fac.Name,  // 👈 Mentor
                        mentor.IPO,
                        mentor.Completion_Letter
                    }
                )
                .GroupBy(x => x.UPRN)
                .Select(g => g.FirstOrDefault())
                .ToList();

                        // 5️⃣ Build ViewModel for each student
                        var students = studentData.Select(s => new CCA.StudentInternshipMarkRow
                        {
                            UPRN = s.UPRN,
                            StudentName = s.StudentName,
                            Programme = s.Programme,
                            MentorName = s.MentorName,

                            IPO = s.IPO,
                            Completion_Letter = s.Completion_Letter,
                            ComponentMarks = components.Select(c => new CCA.ComponentMarkEntry
                            {
                                ComponentId = c.ComponentId,
                                MaxMark = c.MaxMark,

                                MarkAwarded = allMarks
                                    .Where(m => m.UPRN == s.UPRN && m.ComponentId == c.ComponentId)
                                    .Select(m => m.MarkAwarded)
                                    .FirstOrDefault()
                            }).ToList(),

                            IsSubmitted = allMarks.Any(m =>
                                m.UPRN == s.UPRN && m.Is_Submitted),
                            ReviewStatus = db.ESE_InternshipMarks
                                            .Where(m => m.UPRN == s.UPRN && m.Acc_Yr_Sem_Id == accYrSemId)
                                            .Select(m => m.Review_Status)
                                             .FirstOrDefault() ?? "Draft",
                        }).ToList();
                        // 6️⃣ Send data to View
                        CCA.InternshipCCAMarkViewModel vm = new CCA.InternshipCCAMarkViewModel
                        {
                            Acc_Yr_Sem_Id = accYrSemId,
                            FacultyId = facultyId,
                            Components = components,
                            Students = students
                        };

                        return View(vm);

                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }
                }

                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult ESE_RejectStudent(string uprn, int accYrSemId)
        {
            var records = db.ESE_InternshipMarks
                .Where(x => x.UPRN == uprn && x.Acc_Yr_Sem_Id == accYrSemId)
                .ToList();

            if (!records.Any())
            {
                return Json(new { success = false, message = "No marks found." });
            }

            records.ForEach(r =>
            {
                r.Review_Status = "Rejected";
                r.Updated_Date = DateTime.Now;
            });

            db.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Student marks rejected and sent back to mentor."
            });
        }
        [HttpPost]
        public JsonResult ESE_ApproveSingleStudent(string uprn)
        {
            try
            {
                var records = db.ESE_InternshipMarks
                                .Where(x => x.UPRN == uprn)
                                .ToList();

                foreach (var r in records)
                {

                    r.Review_Status = "Approved";
                    r.Submitted_Date = DateTime.Now;
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ESE_ApproveAllStudents()
        {
            try
            {
                int facultyId = Convert.ToInt32(Session["Faculty_Id"].ToString());


                // 2️⃣ Get current academic year
                int AccYr = 9;
                //objfaculty.getCurrentAcademicYear().Acc_yr_Id;

                // 3️⃣ Get Academic Year + Semester ID for Semester IV
                int accYrSemId = db.CMS_AccademicYearSemesters
                                    .Where(x => x.Sem_Id == 4 && x.Acc_yr_Id == AccYr)
                                    .Select(x => x.Acc_Yr_Sem_Id)
                                    .FirstOrDefault();
                var records = (
                   from mentor in db.FYUGP_Internship_Mentors
                   join sclass in db.CMS_StudentClasss on mentor.UPRN equals sclass.UPRN
                   join accpgm in db.CMS_AcademicYr_Sem_Programmes on sclass.Acc_Yr_Sem_Pgm_Id equals accpgm.Acc_Yr_Sem_Pgm_Id
                   join sup in db.CCA_InternshipSupervisors on accpgm.Pgm_Id equals sup.Pgm_Id
                   join intmrk in db.ESE_InternshipMarks on mentor.UPRN equals intmrk.UPRN
                   where sup.Fac_Id == facultyId
                       && mentor.Acc_Yr_Id == AccYr
                       && mentor.Active_Status
                       //&& accpgm.Acc_Yr_sem_Id == accYrSemId
                       && intmrk.Review_Status == "Draft"
                   select

               intmrk).ToList();

                foreach (var r in records)
                {

                    r.Review_Status = "Approved";
                    r.Submitted_Date = DateTime.Now;
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region ISE
        //        public ActionResult ISE_QuestionMapping(string CourseCode, string ISE_Type, int CourseSemId)
        //        {
        //            if (Session["Log_Id"] != null)
        //            {
        //                try
        //                {
        //                    if (isUserAuthenticated())
        //                    {
        //                        int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
        //                        ViewBag.CourseCode = CourseCode;
        //                        ViewBag.ISE_Type = ISE_Type;
        //                        ViewBag.CourseSemId = CourseSemId;
        //                        ViewBag.CourseName = db.CMS_Courses.Where(c=>c.Course_Code==CourseCode && c.Active_Status).Select(c=>c.Course_Name).FirstOrDefault();
        //                        ViewBag.CourseType = db.CMS_Courses.Where(c => c.Course_Code == CourseCode && c.Active_Status).Select(c => c.Course_Nature).FirstOrDefault(); ;
        //                        var sectionMarks = db.Exam_Section_Marks
        //                            .Where(x => x.Course_Sem_Id == CourseSemId
        //                            && x.Exam_Type == ISE_Type
        //                            && x.Active_Status)
        //                            .ToList();
        //                        ViewBag.SectionIdMap = sectionMarks != null
        //     ? sectionMarks.ToDictionary(
        //           x => x.Sec_Id.ToString(),   // convert key to string
        //           x => x.Id
        //       )
        //     : new Dictionary<string, int>();
        //                        var sectionIds = sectionMarks.Select(x => x.Id).ToList();
        //                        bool isMCQExam = sectionMarks.Any()
        //                 && sectionMarks.All(x => x.Qn_Type == "MCQ");
        //                        ViewBag.ExamPatternType = isMCQExam ? "MCQ" : "Theory";
        //                        var mappings = db.CMS_FYUGP_Question_Mappings
        //                            .Where(x =>
        //                                sectionIds.Contains(x.Section_Id) &&
        //                                x.Course_Code == CourseCode &&
        //                                x.Active_Status)
        //                            .ToList();
        //                        ViewBag.IsMarksSaved = sectionMarks.Any();
        //                        // If mapping already done
        //                        if (mappings.Any())
        //                        {
        //                            var vm = (
        //    from s in sectionMarks
        //    from q in mappings
        //        .Where(x => x.Section_Id == s.Id)
        //        .DefaultIfEmpty()   // 🔑 LEFT JOIN
        //    orderby s.Sec_Id, q != null ? q.Question_No : 0
        //    select new Question_Mapping
        //    {
        //        // Mapping Id (nullable-safe)
        //        SectionMarkId = q != null ? q.Qns_Map_Id : 0,

        //        // Section info
        //        Sec_Id = s.Id,
        //        SectionName = ((char)(64 + s.Sec_Id)).ToString(),
        //        Qn_Type = s.Qn_Type,
        //        Mark = s.Mark,
        //        Total_Questions = s.Total_Questions,
        //        Max_Questions=s.Max_Questions,
        //        // Question number (default = 1)
        //        Question_No = q != null ? q.Question_No : 1,

        //        // Mapping fields (nullable-safe)
        //        Module = q?.Module,
        //        Unit = q?.Unit,
        //        CO = q?.CO,
        //        Learning_Domain = q?.Learning_Domain,
        //        Difficulty_Level = q?.Difficulty_Level
        //    }
        //).ToList();

        //                            ViewBag.IsMapped = mappings.Any();

        //                            ViewBag.MaxMarks = sectionMarks.Sum(x => x.Sec_Max_Mark);
        //                            return View(vm);
        //                        }
        //                        // Not saved → hide table
        //                        ViewBag.MaxMarks = sectionMarks.Sum(x => x.Sec_Max_Mark);
        //                        return View(new List<Question_Mapping>());

        //                    }
        //                    else
        //                    {
        //                        return Redirect("Faculty_Login");
        //                    }
        //                }

        //                catch
        //                {
        //                    return Redirect("~/Login/Error_Page");
        //                }
        //            }
        //            else
        //            {
        //                return Redirect("Faculty_Login");

        //            }
        //        }

        public ActionResult ISE_QuestionMapping(string CourseCode, string ISE_Type, int CourseSemId)
        {
            if (Session["Log_Id"] == null)
                return Redirect("Faculty_Login");

            try
            {
                if (!isUserAuthenticated())
                    return Redirect("Faculty_Login");

                int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                Guid logid = new Guid(Session["Log_Id"].ToString());
                // 🔹 Basic ViewBag Data
                ViewBag.CourseCode = CourseCode;
                ViewBag.ISE_Type = ISE_Type;
                ViewBag.CourseSemId = CourseSemId;

                var course = db.CMS_Courses
                               .FirstOrDefault(c => c.Course_Code == CourseCode && c.Active_Status);

                ViewBag.CourseName = course?.Course_Name;
                ViewBag.CourseType = course?.Course_Nature;

                // 🔹 Get Section Marks for current semester
                var sectionMarks = db.Exam_Section_Marks
                    .Where(x => x.Course_Sem_Id == CourseSemId
                             && x.Exam_Type == ISE_Type
                             && x.Active_Status)
                    .ToList();

                ViewBag.IsMarksSaved = sectionMarks.Any();
                ViewBag.MaxMarks = sectionMarks.Sum(x => x.Sec_Max_Mark);

                ViewBag.SectionIdMap = sectionMarks.Any()
                    ? sectionMarks.ToDictionary(x => x.Sec_Id.ToString(), x => x.Id)
                    : new Dictionary<string, int>();

                bool isMCQExam = sectionMarks.Any() &&
                                 sectionMarks.All(x => x.Qn_Type == "MCQ");
                bool isMDCExam = sectionMarks.Any() &&
                                sectionMarks.All(x => x.Category == "MDC");

                ViewBag.ExamPatternType = isMCQExam ? "MCQ" : "Theory";
                if (isMCQExam)
                {
                    ViewBag.ExamPatternType = "MCQ";
                }
                else if (isMDCExam)
                {
                    ViewBag.ExamPatternType = "MDC";
                }
                else
                {
                    ViewBag.ExamPatternType = "Theory";
                }

                if (!sectionMarks.Any())
                    return View(new List<Question_Mapping>());

                var currentSectionIds = sectionMarks.Select(x => x.Id).ToList();

                // ============================================================
                // 1️⃣ Try to get mappings for CURRENT CourseSemId
                // ============================================================

                var mappings = db.CMS_FYUGP_Question_Mappings
                    .Where(x =>
                        currentSectionIds.Contains(x.Section_Id) &&
                        x.Course_Code == CourseCode &&
                        x.Created_By == logid &&
                        x.Active_Status)
                    .ToList();

                // ============================================================
                // 2️⃣ If no mapping found → Load previous semester mapping
                // ============================================================

                if (!mappings.Any())
                {
                    var previousMappings = db.CMS_FYUGP_Question_Mappings
                        .Where(x =>
                            x.Course_Code == CourseCode &&
                            x.Created_By == logid &&
                            x.Active_Status)
                        .OrderByDescending(x => x.Qns_Map_Id)
                        .ToList();

                    if (previousMappings.Any() && !isMDCExam)
                    {
                        var prevaccyrId = objfaculty.getSpecificAcademicYear();
                        var previousSectionIds = previousMappings
                                                 .Select(x => x.Section_Id)
                                                 .Distinct()
                                                 .ToList();

                        var previousSections = db.Exam_Section_Marks
                            .Where(x =>
                                previousSectionIds.Contains(x.Id) &&
                                x.Course_Sem_Id != CourseSemId &&
                                x.Exam_Type == ISE_Type && x.Acc_Yr_Id == prevaccyrId.Acc_yr_Id &&
                                x.Active_Status)
                            .ToList();

                        foreach (var currentSection in sectionMarks)
                        {
                            var matchedOldSection = previousSections
                                .FirstOrDefault(x => x.Sec_Id == currentSection.Sec_Id);

                            if (matchedOldSection != null)
                            {
                                var oldSectionMappings = previousMappings
                                    .Where(x => x.Section_Id == matchedOldSection.Id)
                                    .ToList();

                                foreach (var oldMap in oldSectionMappings)
                                {
                                    mappings.Add(new CMS_FYUGP_Question_Mapping
                                    {
                                        Section_Id = currentSection.Id,
                                        Qns_Map_Id = currentSection.Id,
                                        Course_Code = CourseCode,
                                        //Faculty_Id = Faculty_Id,
                                        Question_No = oldMap.Question_No,
                                        Module = oldMap.Module,
                                        Unit = oldMap.Unit,
                                        CO = oldMap.CO,
                                        Learning_Domain = oldMap.Learning_Domain,
                                        Difficulty_Level = oldMap.Difficulty_Level,
                                        Active_Status = true
                                    });
                                }
                            }
                        }
                    }
                }

                // ============================================================
                // 3️⃣ Prepare ViewModel (LEFT JOIN Style)
                // ============================================================

                var vm = (
                    from s in sectionMarks
                    from q in mappings
                        .Where(x => x.Section_Id == s.Id)
                        .DefaultIfEmpty()
                    orderby s.Sec_Id, q != null ? q.Question_No : 0
                    select new Question_Mapping
                    {
                        SectionMarkId = q != null ? q.Qns_Map_Id : 0,

                        Sec_Id = s.Id,
                        SectionName = ((char)(64 + s.Sec_Id)).ToString(),

                        Qn_Type = s.Qn_Type,
                        Mark = s.Mark,
                        Total_Questions = s.Total_Questions,
                        Max_Questions = s.Max_Questions,

                        Question_No = q != null ? q.Question_No : 1,

                        Module = q?.Module,
                        Unit = q?.Unit,
                        CO = q?.CO,
                        Learning_Domain = q?.Learning_Domain,
                        Difficulty_Level = q?.Difficulty_Level
                    }
                ).ToList();

                ViewBag.IsMapped = mappings.Any();

                return View(vm);
            }
            catch
            {
                return Redirect("~/Login/Error_Page");
            }
        }

        [HttpPost]
        public ActionResult SaveSectionMarks(List<Exam_Section_Mark> model)
        {
            if (model == null || !model.Any())
                return Json(new { success = false, message = "No data received" });
            try
            {
                string corseCode = model.Select(x => x.Course_Code).FirstOrDefault();
                int corseSemId = model.Select(x => (int)x.Course_Sem_Id).FirstOrDefault();
                var crs = (from a in db.CMS_Course_Semesters
                           join b in db.CMS_Courses on a.Course_Id equals b.Course_Id
                           where a.Course_Sem_Id == corseSemId
                           select new { a.Category, b.Course_Nature }).FirstOrDefault();
                string Exam_Type = model.Select(x => x.Exam_Type).FirstOrDefault();
                int count = db.Exam_Section_Marks.Where(x => x.Course_Sem_Id == corseSemId && x.Active_Status && x.Exam_Type == Exam_Type).Count();
                if (count == 0)
                {
                    int id = db.Exam_Section_Marks.Max(x => (int?)x.Id) ?? 1;

                    foreach (var item in model)
                    {
                        id++;
                        Exam_Section_Mark m = new Exam_Section_Mark();
                        m.Id = id;
                        m.Course_Code = item.Course_Code;
                        m.Category = crs.Category;
                        m.Course_Type = crs.Course_Nature;
                        m.Sec_Id = item.Sec_Id;
                        m.Mark = item.Mark;
                        m.Qn_Type = item.Qn_Type;
                        m.Max_Questions = item.Max_Questions;
                        m.Total_Questions = item.Total_Questions;
                        m.Created_By = new Guid(Session["Log_Id"].ToString());
                        m.Created_On = DateTime.Now;
                        m.Acc_Yr_Id = objfaculty.getCurrentAcademicYear().Acc_yr_Id;
                        m.Active_Status = true;
                        m.Course_Sem_Id = item.Course_Sem_Id;
                        m.Exam_Type = item.Exam_Type;
                        m.Sec_Max_Mark = item.Sec_Max_Mark;
                        db.Exam_Section_Marks.Add(m);
                    }
                    //db.Exam_Section_Marks.AddRange(model);

                    db.SaveChanges();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }
        [HttpPost]
        public ActionResult SaveQuestionMapping(List<CMS_FYUGP_Question_Mapping> model)
        {
            if (model == null || !model.Any())
                return Json(new { success = false, message = "No data received" });

            try
            {
                int nextId = db.CMS_FYUGP_Question_Mappings
                    .Max(x => (int?)x.Qns_Map_Id) ?? 1;

                Guid userId = new Guid(Session["Log_Id"].ToString());

                foreach (var item in model)
                {
                    CMS_FYUGP_Question_Mapping dbRow = db.CMS_FYUGP_Question_Mappings.Where(x => x.Course_Code == item.Course_Code && x.Qns_Map_Id == item.Section_Id && x.Active_Status).FirstOrDefault();

                    if (dbRow != null)
                    {

                        dbRow.Module = item.Module;
                        dbRow.Unit = item.Unit;
                        dbRow.CO = item.CO;
                        dbRow.Learning_Domain = item.Learning_Domain;
                        dbRow.Difficulty_Level = item.Difficulty_Level;

                        dbRow.Modified_By = userId;
                        dbRow.Modified_On = DateTime.Now;
                        db.SaveChanges();

                    }
                    else
                    {
                        nextId++;
                        item.Qns_Map_Id = nextId;
                        item.Created_By = userId;
                        item.Created_On = DateTime.Now;
                        item.Active_Status = true;

                        db.CMS_FYUGP_Question_Mappings.Add(item);
                    }
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult DownloadScoreSheet(string CourseCode, string ISE_Type, int CourseSemId)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    var course = (from a in db.CMS_Courses
                                  join b in db.CMS_Course_Semesters on a.Course_Id equals b.Course_Id
                                  join c in db.CMS_Semesters on a.Semester equals c.Sem_Id
                                  where b.Course_Sem_Id == CourseSemId
                                  select new { a.Course_Name, b.Category, c.Semester }).FirstOrDefault();
                    string CourseName = course.Course_Name + "( " + course.Category + " )";
                    string DepName = (from a in db.CMS_Course_Semesters
                                      join b in db.CMS_AcademicYr_Sem_Programmes on a.Acc_Yr_Sem_Pgm_Id equals b.Acc_Yr_Sem_Pgm_Id
                                      join c in db.CMS_Programmes on b.Pgm_Id equals c.Pgm_Id
                                      join d in db.CMS_Departments on c.Dep_Id equals d.Dep_Id
                                      where a.Course_Sem_Id == CourseSemId
                                      select d.Department
                                     ).FirstOrDefault();
                    var result =
                (
                from sc in db.CMS_StudentClasss
                join uprn in db.CMS_UPRNs on sc.UPRN equals uprn.UPRN
                join stu in db.CMS_Students on uprn.Admission_No equals stu.Admission_No
                join cs in db.CMS_FYUGP_Course_Selections on uprn.UPRN equals cs.UPRN
                join pgm in db.CMS_Programmes on stu.Pgm_Id equals pgm.Pgm_Id
                join sem in db.CMS_Course_Semesters on cs.Course_Sem_Id equals sem.Course_Sem_Id

                // 🔑 JOIN SECTION MARKS (course-level)
                join sec in db.Exam_Section_Marks
                on cs.Course_Sem_Id equals sec.Course_Sem_Id

                // 🔑 LEFT JOIN QUESTION MAPPING
                join qm in db.CMS_FYUGP_Question_Mappings
                on sec.Id equals qm.Section_Id into qmJoin
                from qm in qmJoin.DefaultIfEmpty()

                where sc.Active_Status
                && cs.Active_Status
                && cs.Course_Sem_Id == CourseSemId
                && (
              (cs.Allot_Status == "Allotted" && sem.Category == "MDC") ||
              (cs.Allot_Status == "Allotted" && sem.Category == "SEC") ||
              (cs.Allot_Status == "Allotted" && sem.Category == "VAC") ||
              (sem.Category != "MDC" && sem.Category != "VAC" && sem.Category != "SEC")
                )

                select new Online_MarkEntry
                {
                    UPRN = uprn.UPRN,
                    Name = stu.Name,
                    Programme = DepName,
                    CourseCode = CourseCode,
                    CourseName = CourseName,
                    Sec_Id = sec.Id,
                    Quest_No = qm != null ? qm.Question_No : 0,
                    Sem = course.Semester

                }
                )
                .Distinct()
                .OrderBy(x => x.UPRN)
                .ThenBy(x => x.Sec_Id)
                .ThenBy(x => x.Quest_No)
                .ToList();

                    var QnNo = result.Max(x => x.Quest_No);
                    LocalReport lr = new LocalReport();
                    string path = Path.Combine(Server.MapPath("~/Report"), "ScoreSheet.rdlc");
                    if (QnNo > 12)
                    {
                        path = Path.Combine(Server.MapPath("~/Report"), "ScoreSheet25.rdlc");
                    }
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Admission_Register");
                    }


                    var stud = result;


                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "DataSet1";
                    reportDataSource.Value = stud;
                    lr.DataSources.Add(reportDataSource);
                    //  lr.EnableExternalImages = true;


                    var width = "11.69in";

                    string reportType = "PDF";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;
                    string deviceInfo =
                        "<DeviceInfo>" +
                        "<OutputFormat>" + reportType + "</OutputFormat>" +
                       "<PageWidth>" + width + "</PageWidth>" +   // A4 Landscape width
        "<PageHeight>8.27in</PageHeight>" +  // A4 Landscape height
        "<MarginTop>0.5in</MarginTop>" +
        "<MarginLeft>0.5in</MarginLeft>" +
        "<MarginRight>0.5in</MarginRight>" +
        "<MarginBottom>0.5in</MarginBottom>" +
                        "</DeviceInfo>";
                    Warning[] warning;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = lr.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warning);
                    //  return File(renderedBytes, "pdf");
                    return File(renderedBytes, mimeType);
                }

                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }
        }
        #endregion

        #region FYUGP MARK ENTRY & QUESTION MAPPING
        public Boolean isUserAuthenticated()
        {
            string UserRole = Session["Role"].ToString();
            if (UserRole == "HOD" || UserRole == "Faculty" || UserRole == "Class Warden")
            {
                return true;
            }

            else
            {
                return false;
            }


        }
        public ActionResult CourseQuestionSection(string CourseCode, string Course_Type, string ISE_Type, string Category, int Stream, int CourseSemId)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (isUserAuthenticated())
                    {
                        int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                        ViewBag.CourseCode = CourseCode;
                        ViewBag.CourseName = db.CMS_Courses.Where(x => x.Course_Code == CourseCode && x.Active_Status == true).Select(x => x.Course_Name).FirstOrDefault();
                        ViewBag.Course_Type = Course_Type;
                        ViewBag.ISE_Type = ISE_Type;
                        ViewBag.Category = Category;
                        ViewBag.Stream = Stream;
                        ViewBag.CourseSemId = CourseSemId;
                        return View();
                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult QuestionPapers(string Course_Code)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (isUserAuthenticated())
                    {
                        int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                        List<Exam_Online> P = new List<Exam_Online>();

                        P = objfaculty.getAllQuestions(Course_Code).ToList();
                        return View(P);
                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }


        [HttpPost]
        public ActionResult InsertCourseSection(List<Question_Bunk> courseSections, string origin)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid LogId = new Guid(Session["Log_Id"].ToString());
                    int val = objfaculty.AddCourseSection(courseSections, LogId, origin);
                    if (val > 1)
                    {
                        CMS_Course crs = db.CMS_Courses.Where(x => x.Course_Id == val).FirstOrDefault();
                        return Json(crs, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(val, JsonRequestBehavior.AllowGet);
                    }

                }

                catch (Exception ex)
                {

                    return Json(ex.InnerException.ToString(), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public ActionResult GetCourseSectionDetails(string ExamType, int CourseSemId)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {
                    List<Exam_Section_Mark> pso = new List<Exam_Section_Mark>();
                    var q = (from a in db.Exam_Section_Marks
                             where a.Active_Status == true && a.Course_Sem_Id == CourseSemId && a.Exam_Type == ExamType
                             select a).OrderBy(x => x.Id).ToList();
                    foreach (var i in q)
                    {
                        Exam_Section_Mark p = new Exam_Section_Mark();
                        p.Mark = i.Mark;
                        p.Sec_Id = i.Sec_Id;
                        p.Max_Questions = i.Max_Questions;
                        p.Total_Questions = i.Total_Questions;
                        pso.Add(p);
                    }

                    var serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    var resultData = pso;
                    var result = new ContentResult
                    {
                        Content = serializer.Serialize(resultData),
                        ContentType = "application/json"
                    };

                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult QuestionMapping(string CourseCode, string ISE_Type, int CourseSemId, string CourseType)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (isUserAuthenticated())
                    {
                        int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                        ViewBag.CourseCode = CourseCode;
                        ViewBag.ISE_Type = ISE_Type;
                        ViewBag.CourseSemId = CourseSemId;
                        ViewBag.Course_Type = CourseType;
                        List<Exam_QuestionSection> lst = objfaculty.getCoursesSection(ISE_Type, CourseSemId).ToList();

                        return View(lst);
                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        [HttpPost]
        public ActionResult GetCourseSection(string ExamType, int CourseSemId)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {
                    List<Exam_QuestionSection> lst = objfaculty.getCoursesSection(ExamType, CourseSemId).ToList();
                    return Json(lst, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult GetCourseSectionDetailswithSecId(string Course_Code, int SectionId, string ExamType, int CourseSemId)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {
                    List<Question_Mapping> lst = objfaculty.getCoursesSectionDetails(Course_Code, SectionId, ExamType, CourseSemId).ToList();
                    return Json(lst, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public ActionResult InsertESEQuestionMapping(List<CMS_FYUGP_Question_Mapping> qnsMapping, string Exam_Type, string NotNo)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid LogId = new Guid(Session["Log_Id"].ToString());
                    int val = 0;
                    var NotDetails = db.Exam_Notifications.Where(x => x.Notification_No == NotNo && x.Active_Status == true).FirstOrDefault();

                    if (NotDetails.Category == "Special")  // special exam
                    {
                        val = objfaculty.InsertSpecialEXam_QuestionMapping(qnsMapping, NotNo, LogId);
                    }
                    else
                    {

                        val = objfaculty.InsertESEQuestionMapping(qnsMapping, LogId);
                    }
                    return Json(val, JsonRequestBehavior.AllowGet);

                }

                catch (Exception ex)
                {

                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public ActionResult InsertQuestionMapping(List<CMS_FYUGP_Question_Mapping> qnsMapping, string Exam_Type, int CourseSemId)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid LogId = new Guid(Session["Log_Id"].ToString());

                    int val = objfaculty.InsertQuestionMapping(qnsMapping, LogId, Exam_Type, CourseSemId);
                    return Json(val, JsonRequestBehavior.AllowGet);

                }

                catch (Exception ex)
                {

                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }

        }

        #endregion 

        [HttpPost]
        public ActionResult ForgotPwd(SignUp log)
        {
            try
            {

                int Role_Id = 0;
                CMS_AcademicYear ac = objfaculty.getCurrentAcademicYear();
                int facultyid = db.CMS_Facultys.Where(x => x.Active_Status == true && (x.Mobile == log.Username || x.Email == log.Username)).Select(x => x.Faculty_Id).FirstOrDefault();
                //int cnts = db.CMS_HODs.Where(x => x.Active_Status == true && x.Faculty_Id == facultyid && x.Acc_Yr== ac.Acc_yr_Id).Select(x => x.Faculty_Id).Count();
                //int warden_Count = (from a in db.CMS_ClassWardens
                //                    join b in db.CMS_AccademicYearSemesters on a.Acc_Yr_sem_Id equals b.Acc_Yr_Sem_Id
                //                    where a.Active_Status == true && b.Acc_yr_Id == ac.Acc_yr_Id && a.Faculty_Id == facultyid
                //                    select new
                //                    {
                //                        a
                //                    }).Count();
                //if (warden_Count != 0 && cnts != 0)
                //{
                //    Role_Id = 13;
                //}
                //else if (cnts != 0)
                //{

                //    Role_Id = 13;

                //}
                //else if (warden_Count != 0)
                //{
                //    Role_Id = 15;
                //}
                //else
                //{
                //    Role_Id = 12;
                //}


                //if (log.Username == "9495727688")
                //{
                //    Role_Id = 13;
                //}
                //if (log.Username == "8289918958")
                //{
                //    Role_Id = 35;
                //}
                CMS_Login login = db.CMS_Logins.Where(x => (x.Mobile == log.Username || x.Email == log.Username) && x.Role_Id == 12 && x.Active_Status == true).FirstOrDefault();
                if (login == null && log.Username.Contains("@"))
                {
                    String FacMObile = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Email == log.Username).Select(x => x.Mobile).FirstOrDefault();
                    login = db.CMS_Logins.Where(x => x.Mobile == FacMObile && x.Role_Id == 12 && x.Active_Status == true).FirstOrDefault();
                }

                if (login != null)
                {
                    bool status;
                    String Password = CreateRandomPassword();
                    log.Password = Password;
                    login.Password = Password;
                    login.Password_Status = 2;
                    db.SaveChanges();
                    if (login.Mobile == log.Username)
                    {

                        string mobileNumber = login.Mobile;
                        string message = HttpUtility.UrlEncode("Dear " + login.Name + ",Your New Password is : " + Password + ".Please login using this password.");

                        //Your authentication key
                        string authKey = "2037ANB8jQf25f112b1eP43";
                        string senderId = "CMSKTM";
                        //Prepare you post parameters
                        StringBuilder sbPostData = new StringBuilder();
                        sbPostData.AppendFormat("authkey={0}", authKey);
                        sbPostData.AppendFormat("&mobiles={0}", mobileNumber);
                        sbPostData.AppendFormat("&message={0}", message);
                        sbPostData.AppendFormat("&sender={0}", senderId);
                        sbPostData.AppendFormat("&route={0}", 4);
                        sbPostData.AppendFormat("&country={0}", 91);
                        sbPostData.AppendFormat("&DLT_TE_ID={0}", "1207160947280543090");

                        //Call Send SMS API

                        string sendSMSUri = "http://adlinks.websmsc.com/api/sendhttp.php?";

                        //Create HTTPWebrequest
                        HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);

                        //Prepare and Add URL Encoded data
                        UTF8Encoding encoding = new UTF8Encoding();
                        byte[] data = encoding.GetBytes(sbPostData.ToString());
                        //Specify post method
                        httpWReq.Method = "POST";
                        httpWReq.ContentType = "application/x-www-form-urlencoded";
                        httpWReq.ContentLength = data.Length;
                        using (Stream stream = httpWReq.GetRequestStream())
                        {
                            stream.Write(data, 0, data.Length);
                        }
                        //Get the response
                        HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        string responseString = reader.ReadToEnd();

                        // Close the response
                        reader.Close();
                        response.Close();

                    }
                    else
                    {
                        string To = string.Empty;
                        string body = string.Empty;
                        //bool status;

                        To = login.Email.ToString().Trim();
                        string Subject = "CMS College";
                        using (StreamReader reader1 = new StreamReader(Server.MapPath("~/Email Template/OTP.html")))
                        {
                            body = reader1.ReadToEnd();


                        }
                        body = body.Replace("{Name}", login.Name.ToString());
                        body = body.Replace("{Password}", Password);

                        status = objservice.SendEMail2(body, To, Subject);
                    }
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult Faculty_Feedback_Home()
        {
            try
            {
                Session.Abandon();
                int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                int Dep_Feed_Id = db.CMS_Feedback_FacultyToCurriculums.Where(x => x.Faculty_Id == Faculty_Id && x.Active_Status == true).Select(x => x.Dep_Feed_Id).FirstOrDefault();
                if (Dep_Feed_Id > 0)
                {
                    return Content("<script language='javascript' type='text/javascript'>alert('Already Submitted Your Valuable Feedback. Thank You !!!')");

                }
                else
                {
                    return View();
                }
            }
            catch
            {

                return Redirect("~/Login/Error_Page");
            }

        }
        public ActionResult Departmental_Performance_Evaluations()
        {
            int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
            int Dep_Feed_Id = db.CMS_Feedback_FacultyToCurriculums.Where(x => x.Faculty_Id == Faculty_Id && x.Active_Status == true).Select(x => x.Dep_Feed_Id).FirstOrDefault();
            if (Dep_Feed_Id > 0)
            {
                ViewBag.Msg = "Already Submitted Your Valuable Feedback. Thank You !!!";
                return View();
            }
            else
            {
                if (Faculty_Id > 0)
                {

                    int deptID = db.CMS_Facultys.Where(x => x.Faculty_Id == Faculty_Id && x.Active_Status == true).Select(x => x.Dep_Id).FirstOrDefault();
                    ViewBag.Dept_Id = deptID;
                    ViewBag.Department = db.CMS_Departments.Where(x => x.Dep_Id == deptID && x.Active_Status == true).Select(x => x.Department).FirstOrDefault();
                    List<CMS_Feedback_Question> quest = objFeed.getFeedbackQuestion_Faculty().ToList(); // feedback questions for Category 'Faculty' by LL
                    return View(quest);
                }
                else
                {
                    TempData["Msg"] = "Online Department evaluation portal is closed";
                    return RedirectToAction("Home", "Feedback", new { area = "Feedback" });
                }
            }

        }
        [HttpPost]
        public ActionResult Submit_Feedback_Faculty(List<TeacherFeedback> Feed, int Dep_Id, string suggestions)
        {
            int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
            if (Faculty_Id != null)
            {
                try
                {

                    objFeed.addFeedback_Faculty(Feed, Dep_Id, Faculty_Id, suggestions);
                    return Json(1, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult GradPaymnt_List()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Student> sch = new List<Student>();
                    sch = objfaculty.getPaymentList_gard().ToList();   // list
                    ViewBag.UG = sch.Where(x => x.Pgm_Type_Id == 1).Count();
                    ViewBag.PG = sch.Where(x => x.Pgm_Type_Id == 2).Count();
                    ViewBag.Bvoc = sch.Where(x => x.Pgm_Type_Id == 8).Count();
                    ViewBag.Phd = sch.Where(x => x.Pgm_Type_Id == 4).Count();
                    ViewBag.total = sch.Count();
                    ViewBag.Amount = sch.Select(x => x.Amount).Sum();
                    return View(sch);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");
            }
        }
        #region Student_Acheivemennt
        public ActionResult Stud_AchievementAdd()
        {
            if (Session["Log_Id"] == null)
                return Redirect("Faculty_Login");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Stud_AchievementAdd(CMS_StudentAchievement model, HttpPostedFileBase CertificateFile)
        {
            if (Session["Log_Id"] == null)
                return Redirect("Faculty_Login");

            try
            {
                if (string.IsNullOrWhiteSpace(model.UPRN))
                    ModelState.AddModelError("UPRN", "UPRN is required");

                if (string.IsNullOrWhiteSpace(model.Achievement))
                    ModelState.AddModelError("Achievement", "Achievement is required");

                if (ModelState.IsValid)
                {
                    model.Created_Date = DateTime.Now;
                    model.Created_By = Guid.Parse(Session["Log_Id"].ToString());
                    model.Active_Status = true;

                    // first save to generate Id
                    db.CMS_StudentAchievements.Add(model);
                    db.SaveChanges();

                    // upload certificate after Id is generated
                    if (CertificateFile != null && CertificateFile.ContentLength > 0)
                    {
                        string folderPath = Server.MapPath("~/Images/AchievementCertificates/");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        string extension = Path.GetExtension(CertificateFile.FileName);

                        // File name format: UPRN_Id.extension
                        string fileName = model.UPRN + "_" + model.Id + extension;

                        string savePath = Path.Combine(folderPath, fileName);
                        CertificateFile.SaveAs(savePath);

                        // save file name in DB
                        model.Certificate = fileName;

                        // update record
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    TempData["Success"] = "Student achievement added successfully.";
                    return RedirectToAction("Stud_AchievementAdd");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }
        [HttpPost]
        public JsonResult GetStudentDetailsByUPRN(string uprn)
        {
            try
            {
                if (Session["Log_Id"] == null)
                    return Json(new { status = false, message = "Session expired" }, JsonRequestBehavior.AllowGet);

                if (string.IsNullOrWhiteSpace(uprn))
                    return Json(new { status = false, message = "UPRN is required" }, JsonRequestBehavior.AllowGet);

                var student = (from u in db.CMS_UPRNs
                               join s in db.CMS_Students on u.Admission_No equals s.Admission_No
                               join pg in db.CMS_Programmes on s.Pgm_Id equals pg.Pgm_Id
                               join d in db.CMS_Departments on pg.Dep_Id equals d.Dep_Id into deptJoin
                               from d in deptJoin.DefaultIfEmpty()
                               where u.UPRN == uprn
                               select new
                               {
                                   StudentName = s.Name,
                                   Department = d != null ? d.Department : ""
                               }).FirstOrDefault();

                if (student == null)
                {
                    return Json(new
                    {
                        status = false,
                        message = "No student found for this UPRN"
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    status = true,
                    name = student.StudentName,
                    department = student.Department
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Stud_AchievementList()
        {
            if (Session["Log_Id"] == null)
                return RedirectToAction("Faculty_Login", "Faculty");

            Guid logid = Guid.Parse(Session["Log_Id"].ToString());
            var data = (from a in db.CMS_StudentAchievements
                        join u in db.CMS_UPRNs on a.UPRN equals u.UPRN
                        join s in db.CMS_Students on u.Admission_No equals s.Admission_No
                        join pg in db.CMS_Programmes on s.Pgm_Id equals pg.Pgm_Id
                        join d in db.CMS_Departments on pg.Dep_Id equals d.Dep_Id into deptJoin
                        from d in deptJoin.DefaultIfEmpty()
                        where a.Created_By == logid
                        orderby a.Created_Date descending
                        select new StudentAchievement
                        {
                            Id = a.Id,
                            UPRN = a.UPRN,
                            Achievement = a.Achievement,
                            Certificate = a.Certificate,
                            Created_Date = a.Created_Date,
                            StudentName = s.Name,
                            DepartmentName = d != null ? d.Dep_Name : ""
                        }).ToList();

            return View(data);
        }
        public ActionResult Stud_AchievementListAll()
        {
            if (Session["Log_Id"] == null)
                return RedirectToAction("Faculty_Login", "Faculty");

            Guid logid = Guid.Parse(Session["Log_Id"].ToString());
            var data = (from a in db.CMS_StudentAchievements
                        join u in db.CMS_UPRNs on a.UPRN equals u.UPRN
                        join s in db.CMS_Students on u.Admission_No equals s.Admission_No
                        join pg in db.CMS_Programmes on s.Pgm_Id equals pg.Pgm_Id

                        // LEFT JOIN with login table
                        join lg in db.CMS_Logins on a.Created_By equals lg.Log_Id into loginJoin
                        from lg in loginJoin.DefaultIfEmpty()

                            // LEFT JOIN with department
                        join d in db.CMS_Departments on pg.Dep_Id equals d.Dep_Id into deptJoin
                        from d in deptJoin.DefaultIfEmpty()

                        where a.Active_Status == true
                        orderby a.Created_Date descending
                        select new StudentAchievement
                        {
                            Id = a.Id,
                            UPRN = a.UPRN,
                            Achievement = a.Achievement,
                            Certificate = a.Certificate,
                            Created_Date = a.Created_Date,
                            StudentName = s.Name,
                            DepartmentName = d != null ? d.Dep_Name : "",

                            // If faculty added -> faculty name from login table
                            // If student added -> student name
                            Name = lg != null ? lg.Name : s.Name + "(Student)"
                        }).ToList();

            return View(data);
        }
        public ActionResult Stud_AchievementDelete(int id)
        {
            if (Session["Log_Id"] == null)
                return Redirect("Faculty_Login");

            var achievement = db.CMS_StudentAchievements.FirstOrDefault(x => x.Id == id);
            if (achievement != null)
            {
                achievement.Active_Status = false;
                db.SaveChanges();
                TempData["Success"] = "Achievement deleted successfully.";
            }

            return RedirectToAction("Stud_AchievementList");
        }
        // =========================
        // STUDENT ACHIEVEMENT EDIT
        // =========================
        public ActionResult Stud_AchievementEdit(int id)
        {
            if (Session["Log_Id"] == null)
                return Redirect("Faculty_Login");

            try
            {
                var achievement = db.CMS_StudentAchievements
                                    .FirstOrDefault(x => x.Id == id && x.Active_Status == true);

                if (achievement == null)
                {
                    TempData["Error"] = "Achievement not found.";
                    return RedirectToAction("Stud_AchievementList");
                }

                LoadAchievementStudentDetails(achievement.UPRN);

                return View(achievement);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Stud_AchievementList");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Stud_AchievementEdit(CMS_StudentAchievement model, HttpPostedFileBase CertificateFile)
        {
            if (Session["Log_Id"] == null)
                return Redirect("Faculty_Login");

            try
            {
                if (string.IsNullOrWhiteSpace(model.UPRN))
                    ModelState.AddModelError("UPRN", "UPRN is required");

                if (string.IsNullOrWhiteSpace(model.Achievement))
                    ModelState.AddModelError("Achievement", "Achievement is required");

                var achievement = db.CMS_StudentAchievements.FirstOrDefault(x => x.Id == model.Id && x.Active_Status == true);
                if (achievement == null)
                {
                    TempData["Error"] = "Achievement not found.";
                    return RedirectToAction("Stud_AchievementList");
                }

                if (!ModelState.IsValid)
                {
                    LoadAchievementStudentDetails(model.UPRN);
                    model.Certificate = achievement.Certificate; // keep old certificate while returning view
                    return View(model);
                }

                achievement.UPRN = model.UPRN.Trim();
                achievement.Achievement = model.Achievement.Trim();

                // Upload new certificate if provided
                if (CertificateFile != null && CertificateFile.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/Images/AchievementCertificates/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // delete old certificate
                    if (!string.IsNullOrEmpty(achievement.Certificate))
                    {
                        string oldFilePath = Path.Combine(folderPath, achievement.Certificate);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(CertificateFile.FileName);
                    string savePath = Path.Combine(folderPath, fileName);
                    CertificateFile.SaveAs(savePath);

                    achievement.Certificate = fileName;
                }

                achievement.Active_Status = true;

                db.SaveChanges();

                TempData["Success"] = "Student achievement updated successfully.";
                return RedirectToAction("Stud_AchievementList");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                LoadAchievementStudentDetails(model.UPRN);
                return View(model);
            }
        }

        private void LoadAchievementStudentDetails(string uprn)
        {
            ViewBag.StudentName = "";
            ViewBag.DepartmentName = "";

            if (string.IsNullOrWhiteSpace(uprn))
                return;

            uprn = uprn.Trim();

            var student = (from u in db.CMS_UPRNs
                           join s in db.CMS_Students on u.Admission_No equals s.Admission_No
                           join pg in db.CMS_Programmes on s.Pgm_Id equals pg.Pgm_Id
                           join d in db.CMS_Departments on pg.Dep_Id equals d.Dep_Id into deptJoin
                           from d in deptJoin.DefaultIfEmpty()
                           where u.UPRN == uprn
                           select new
                           {
                               StudentName = s.Name,
                               DepartmentName = d != null ? d.Dep_Name : ""
                           }).FirstOrDefault();

            if (student != null)
            {
                ViewBag.StudentName = student.StudentName;
                ViewBag.DepartmentName = student.DepartmentName;
            }
        }

        #endregion
        #region DeptActivity
        public ActionResult DeptActivityCreate()
        {
            if (Session["Log_Id"] == null)
                return Redirect("Faculty_Login");
            //TempData["Success"] = "Activity saved successfully.";
            ViewBag.Department = new SelectList(
                db.CMS_Departments.Where(x => x.Active_Status),
                "Dep_Id",
                "Dep_Name");

            return View();
        }
        public ActionResult DeptActivityIndex()
        {
            if (Session["Log_Id"] == null)
                return Redirect("Faculty_Login");

            ViewBag.Department = new SelectList(
                db.CMS_Departments.Where(x => x.Active_Status),
                "Dep_Id",
                "Dep_Name");

            return View();
        }
        public ActionResult DeptActivityDetails(int id)
        {
            var model = (from a in db.CMS_DeptActivitys
                         join d in db.CMS_Departments
                         on a.Department_Id equals d.Dep_Id
                         where a.Activity_Id == id
                         select new Deptpartment_Activity
                         {
                             Activity_Id = a.Activity_Id,
                             Department_Name = d.Dep_Name,
                             Event_Name = a.Event_Name,
                             Nature_Of_Event = a.Nature_Of_Event,
                             Start_Date = a.Start_Date,
                             End_Date = a.End_Date,
                             Venue = a.Venue,
                             Collaboration = a.Collaboration,
                             Speaker_Name = a.Speaker_Name,
                             Speaker_Designation = a.Speaker_Designation,
                             Speaker_Organization = a.Speaker_Organization,
                             Speaker_Topic = a.Speaker_Topic,
                             Objectives = a.Objectives,
                             Highlights = a.Highlights,
                             Outcomes = a.Outcomes,
                             Funding_Agency = a.Funding_Agency,
                             Funding_Amount = a.Funding_Amount,
                             Internal_Participants = a.Internal_Participants,
                             External_Participants = a.External_Participants,
                             Video_Link = a.Video_Link,
                             Website_Link = a.Website_Link,
                             Brochure_File = a.Brochure_File,
                             Invitation_File = a.Invitation_File,
                             Report_File = a.Report_File,
                             Attendance_File1 = a.Attendance_File1,
                             Attendance_File2 = a.Attendance_File2,
                             Photo1 = a.Photo1,
                             Photo2 = a.Photo2,
                             Photo3 = a.Photo3,
                             Photo4 = a.Photo4,
                             Photo5 = a.Photo5
                         }).FirstOrDefault();

            if (model != null)
            {
                model.BrochurePages = new List<string>();
                model.AttendancePages1 = new List<string>();

                // Brochure
                if (!string.IsNullOrWhiteSpace(model.Brochure_File))
                {
                    try
                    {
                        string pdfPath = model.Brochure_File;

                        if (pdfPath.StartsWith("~/"))
                            pdfPath = Server.MapPath(pdfPath);
                        else if (pdfPath.StartsWith("/"))
                            pdfPath = Server.MapPath("~" + pdfPath);
                        else
                            pdfPath = Server.MapPath("~/Images/DeptActivity/" + Path.GetFileName(pdfPath));

                        string ext = Path.GetExtension(pdfPath).ToLower();

                        if (System.IO.File.Exists(pdfPath) && ext == ".pdf")
                        {
                            using (var docReader = Docnet.Core.DocLib.Instance.GetDocReader(pdfPath, new Docnet.Core.Models.PageDimensions(1200, 1600)))
                            {
                                int pageCount = docReader.GetPageCount();

                                for (int i = 0; i < pageCount; i++)
                                {
                                    using (var pageReader = docReader.GetPageReader(i))
                                    {
                                        var rawBytes = pageReader.GetImage();
                                        int width = pageReader.GetPageWidth();
                                        int height = pageReader.GetPageHeight();

                                        using (var bitmap = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                                        {
                                            var bmpData = bitmap.LockBits(
                                                new System.Drawing.Rectangle(0, 0, width, height),
                                                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                                            System.Runtime.InteropServices.Marshal.Copy(rawBytes, 0, bmpData.Scan0, rawBytes.Length);
                                            bitmap.UnlockBits(bmpData);

                                            using (var ms = new MemoryStream())
                                            {
                                                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                                string base64 = Convert.ToBase64String(ms.ToArray());
                                                model.BrochurePages.Add("data:image/png;base64," + base64);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.BrochureError = ex.Message;
                    }
                }

                // Attendance_File1
                if (!string.IsNullOrWhiteSpace(model.Attendance_File1))
                {
                    try
                    {
                        string pdfPath = model.Attendance_File1;

                        if (pdfPath.StartsWith("~/"))
                            pdfPath = Server.MapPath(pdfPath);
                        else if (pdfPath.StartsWith("/"))
                            pdfPath = Server.MapPath("~" + pdfPath);
                        else
                            pdfPath = Server.MapPath("~/Images/DeptActivity/" + Path.GetFileName(pdfPath));

                        string ext = Path.GetExtension(pdfPath).ToLower();

                        if (System.IO.File.Exists(pdfPath) && ext == ".pdf")
                        {
                            using (var docReader = Docnet.Core.DocLib.Instance.GetDocReader(pdfPath, new Docnet.Core.Models.PageDimensions(1200, 1600)))
                            {
                                int pageCount = docReader.GetPageCount();

                                for (int i = 0; i < pageCount; i++)
                                {
                                    using (var pageReader = docReader.GetPageReader(i))
                                    {
                                        var rawBytes = pageReader.GetImage();
                                        int width = pageReader.GetPageWidth();
                                        int height = pageReader.GetPageHeight();

                                        using (var bitmap = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                                        {
                                            var bmpData = bitmap.LockBits(
                                                new System.Drawing.Rectangle(0, 0, width, height),
                                                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                                            System.Runtime.InteropServices.Marshal.Copy(rawBytes, 0, bmpData.Scan0, rawBytes.Length);
                                            bitmap.UnlockBits(bmpData);

                                            using (var ms = new MemoryStream())
                                            {
                                                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                                string base64 = Convert.ToBase64String(ms.ToArray());
                                                model.AttendancePages1.Add("data:image/png;base64," + base64);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.AttendanceError = ex.Message;
                    }
                }
            }

            return View(model);
        }

        public ActionResult DeptActivityList()
        {
            if (Session["Log_Id"] == null)
                return Redirect("Faculty_Login");
            Guid logid = Guid.Parse(Session["Log_Id"].ToString());

            var data = (from a in db.CMS_DeptActivitys
                        join d in db.CMS_Departments
                            on a.Department_Id equals d.Dep_Id
                        where a.Created_By == logid && a.Active_Status == true
                        select new Deptpartment_Activity
                        {
                            Activity_Id = a.Activity_Id,
                            Dep_Id = a.Department_Id,
                            Department_Name = d.Dep_Name,
                            Event_Name = a.Event_Name,
                            Nature_Of_Event = a.Nature_Of_Event,
                            Start_Date = a.Start_Date,
                            Venue = a.Venue,
                            Internal_Participants = a.Internal_Participants,
                            External_Participants = a.External_Participants
                        }).ToList();

            return View(data);
        }
        public ActionResult DeptActivityListAll()
        {
            if (Session["Log_Id"] == null)
                return Redirect("Faculty_Login");
            Guid logid = Guid.Parse(Session["Log_Id"].ToString());

            var data = (from a in db.CMS_DeptActivitys
                        join d in db.CMS_Departments
                            on a.Department_Id equals d.Dep_Id
                        where a.Active_Status == true
                        select new Deptpartment_Activity
                        {
                            Activity_Id = a.Activity_Id,
                            Dep_Id = a.Department_Id,
                            Department_Name = d.Dep_Name,
                            Event_Name = a.Event_Name,
                            Nature_Of_Event = a.Nature_Of_Event,
                            Start_Date = a.Start_Date,
                            Venue = a.Venue,
                            Internal_Participants = a.Internal_Participants,
                            External_Participants = a.External_Participants,
                        }).ToList();

            return View(data);
        }
        public ActionResult DeptActivityDelete(int id)
        {
            var activity = db.CMS_DeptActivitys.Find(id);

            if (activity != null)
            {
                activity.Active_Status = false;

                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success"] = "Activity deleted successfully.";
            }

            return RedirectToAction("DeptActivityList");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeptActivityCreate(
     CMS_DeptActivity model,
     HttpPostedFileBase Brochure_File,
     HttpPostedFileBase Attendance_File1,
     HttpPostedFileBase Photo1,
     HttpPostedFileBase Photo2,
     HttpPostedFileBase Photo3,
     HttpPostedFileBase Photo4,
     HttpPostedFileBase Photo5)
        {
            if (Session["Log_Id"] == null)
                return RedirectToAction("Login", "Home");

            ViewBag.Department = new SelectList(
                db.CMS_Departments.Where(x => x.Active_Status),
                "Dep_Id",
                "Dep_Name");

            // ===== REPORT VALIDATION =====
            if (string.IsNullOrWhiteSpace(model.Objectives))
            {
                ModelState.AddModelError("Objectives", "Report is required.");
            }
            if (model.End_Date == DateTime.MinValue || model.End_Date == new DateTime(1, 1, 1))
            {
                model.End_Date = model.Start_Date;
            }
            //if (!ModelState.IsValid)
            //{
            //    foreach (var item in ModelState)
            //    {
            //        foreach (var error in item.Value.Errors)
            //        {
            //            System.Diagnostics.Debug.WriteLine(
            //                $"Field: {item.Key}, Error: {error.ErrorMessage}, Exception: {error.Exception?.Message}");
            //        }
            //    }
            //}
            //if (ModelState.IsValid)
            //{
            CMS_DeptActivity activity = new CMS_DeptActivity
            {
                Department_Id = model.Department_Id,
                Event_Name = model.Event_Name,
                Nature_Of_Event = model.Nature_Of_Event,
                Start_Date = model.Start_Date,
                End_Date = model.End_Date,
                Venue = model.Venue,
                Collaboration = model.Collaboration,

                Speaker_Name = model.Speaker_Name,
                Speaker_Designation = model.Speaker_Designation,
                Speaker_Organization = model.Speaker_Organization,
                Speaker_Topic = model.Speaker_Topic,

                // save report text here
                //Objectives = model.Objectives,
                // save report with paragraph breaks
                Objectives = string.IsNullOrWhiteSpace(model.Objectives) ? null : model.Objectives.Trim(),
                Highlights = model.Highlights,
                Outcomes = model.Outcomes,

                Funding_Agency = model.Funding_Agency,
                Funding_Amount = model.Funding_Amount,

                Internal_Participants = model.Internal_Participants,
                External_Participants = model.External_Participants,

                Video_Link = model.Video_Link,
                Website_Link = model.Website_Link,
                Active_Status = true,
                Created_By = (Guid)Session["Log_Id"],
                Created_Date = DateTime.Now
            };

            db.CMS_DeptActivitys.Add(activity);
            db.SaveChanges();

            int actid = activity.Activity_Id;

            activity.Brochure_File = SaveFile(Brochure_File, "Brochure_File", actid);
            activity.Attendance_File1 = SaveFile(Attendance_File1, "Attendance_File1", actid);
            activity.Photo1 = SaveFile(Photo1, "Photo1", actid);
            activity.Photo2 = SaveFile(Photo2, "Photo2", actid);
            activity.Photo3 = SaveFile(Photo3, "Photo3", actid);
            activity.Photo4 = SaveFile(Photo4, "Photo4", actid);
            activity.Photo5 = SaveFile(Photo5, "Photo5", actid);

            db.SaveChanges();
            TempData["Success"] = "Department activity saved successfully.";
            //return RedirectToAction("DeptActivityIndex");
            return RedirectToAction("DeptActivityList");
            //}

            return View(model);
        }
        private string SaveFile(HttpPostedFileBase file, string prefix, int activityId)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            string extension = Path.GetExtension(file.FileName).ToLower();

            string[] allowed =
            {
        ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png"
    };

            if (!allowed.Contains(extension))
                return null;

            string fileName = $"{activityId}_{prefix}{extension}";

            string folder = Server.MapPath("~/Images/DeptActivity/");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            file.SaveAs(Path.Combine(folder, fileName));

            return fileName;
        }

        #endregion
        public ActionResult PasswordAssistance()
        {
            return View();
        }
        [HttpPost]
        public ActionResult getFacultyStatus(string Mobile)
        {

            try
            {
                int Role_Id = 0;
                string mobile = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Mobile).FirstOrDefault();
                if (mobile != null)
                {
                    CMS_AcademicYear ac = objfaculty.getCurrentAcademicYear();
                    int facultyid = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Faculty_Id).FirstOrDefault();
                    int cnts = db.CMS_HODs.Where(x => x.Active_Status == true && x.Faculty_Id == facultyid && x.Acc_Yr == ac.Acc_yr_Id).Select(x => x.Faculty_Id).Count();
                    //int warden_Count = db.CMS_ClassWardens.Where(x => x.Active_Status == true && x.Faculty_Id == facultyid).Select(x => x.Faculty_Id).Count();
                    int warden_Count = (from a in db.CMS_ClassWardens
                                        join b in db.CMS_AccademicYearSemesters on a.Acc_Yr_sem_Id equals b.Acc_Yr_Sem_Id
                                        where a.Active_Status == true && b.Acc_yr_Id == ac.Acc_yr_Id && a.Faculty_Id == facultyid
                                        select new
                                        {
                                            a
                                        }).Count();
                    if (warden_Count != 0 && cnts != 0)
                    {
                        Role_Id = 13;
                    }
                    else if (cnts != 0)
                    {

                        Role_Id = 13;

                    }
                    else if (warden_Count != 0)
                    {
                        Role_Id = 15;
                    }
                    else
                    {
                        Role_Id = 12;
                    }

                    if (Mobile == "9495727688")
                    {
                        Role_Id = 13;
                    }
                    //if (Mobile == "8289918958")
                    //{
                    //    Role_Id = 35;
                    //}
                    int? cnt = 0;
                    cnt = db.CMS_Logins.Where(x => x.Active_Status == true && x.Mobile == Mobile && x.Role_Id == 12).Select(x => x.Password_Status).FirstOrDefault();
                    if (cnt == null)
                        cnt = 0;
                    return Json(cnt, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["Invalid"] = "Invalid Mobile";
                    return Json(-1, JsonRequestBehavior.AllowGet);
                }


            }
            catch
            {
                return Redirect("~/Login/Error_Page");
            }

        }
        [HttpPost]
        public ActionResult ChangePassword(string password)
        {
            if (Session["Faculty_Id"] != null)
            {
                try
                {
                    int facid = Convert.ToInt32(Session["Faculty_Id"]);
                    int RoleId = Convert.ToInt32(Session["Role_Id"]);
                    string Mob = Session["Mobile"].ToString();
                    string retVal = objfaculty.UpdatePassword(Mob, password, RoleId);

                    return Json(retVal, JsonRequestBehavior.AllowGet);
                }

                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");
            }


        }


        public ActionResult Faculty_Reset_Passwords()
        {
            if (Session["Faculty_Id"] != null)
            {

                try
                {
                    //   int facid = Convert.ToInt32(Session["Fac_Id"]);

                    return View();
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");
            }

        }


        public ActionResult Faculty_Login()
        {
            ViewBag.Roles = new SelectList(objfaculty.getAllroles(), "Role_Id", "Role");



            if (Request.Cookies["mobile"] != null)
                ViewBag.Mobile = Request.Cookies["mobile"].Value;
            if (Request.Cookies["pswd"] != null)
                ViewBag.Pswd = Request.Cookies["pswd"].Value;
            if (Request.Cookies["mobile"] != null && Request.Cookies["pswd"] != null)
                ViewBag.Rememberme = true;

            return View();
        }
        public ActionResult Faculty_SignUp()
        {
            return View();
        }

        public ActionResult Faculty_Home()
        {

            if (Session["Fac_Id"] != null)
            {
                try
                {
                    int facid = Convert.ToInt32(Session["Fac_Id"]);
                    List<Schedule> sh = objfaculty.getAllCourses(facid).ToList();
                    return View(sh);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Faculty_Login", "Faculty", new { area = "Faculty" });
        }
        private static string CreateRandomPassword()
        {
            string allowedChars = "0123456789";
            char[] chars = new char[4];
            Random rd = new Random();

            for (int i = 0; i < 4; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }


        [HttpPost]
        public ActionResult Faculty_SignUps(string Name, string Mobile, string Email, string fpwd, string SubmitAction, string Rememberme)
        {
            Guid Log_Id = db.CMS_Logins.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Log_Id).FirstOrDefault();
            objfaculty.GetIp(Log_Id.ToString());

            string dpASSWORD = objpwd.DecryptCipherTextToPlainText("q3wKQ4BSNdDPFlodXkWYi0BZQkzeN5FN");
            bool status;
            int count = 0;
            int Role_Id = 0;
            string otp = "";
            try
            {

                if (Mobile != null)
                {

                    if (Rememberme == "on")
                    {
                        Response.Cookies["mobile"].Value = Mobile;
                        Response.Cookies["pswd"].Value = fpwd;
                        Response.Cookies["mobile"].Expires = DateTime.Now.AddDays(1500);
                        Response.Cookies["pswd"].Expires = DateTime.Now.AddDays(1500);
                    }
                    else
                    {
                        Response.Cookies["mobile"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["pswd"].Expires = DateTime.Now.AddDays(-1);
                    }
                    string mobile = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Mobile).FirstOrDefault();
                    if (mobile != null)
                    {
                        CMS_AcademicYear ac = objfaculty.getCurrentAcademicYear();
                        int facultyid = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Faculty_Id).FirstOrDefault();
                        int cnts = db.CMS_HODs.Where(x => x.Active_Status == true && x.Faculty_Id == facultyid && x.Acc_Yr == ac.Acc_yr_Id).Select(x => x.Faculty_Id).Count();
                        //int warden_Count = db.CMS_ClassWardens.Where(x => x.Active_Status == true && x.Faculty_Id == facultyid).Select(x => x.Faculty_Id).Count();
                        int warden_Count = (from a in db.CMS_ClassWardens
                                            join b in db.CMS_AccademicYearSemesters on a.Acc_Yr_sem_Id equals b.Acc_Yr_Sem_Id
                                            where a.Active_Status == true && b.Acc_yr_Id == ac.Acc_yr_Id && a.Faculty_Id == facultyid
                                            select new
                                            {
                                                a
                                            }).Count();
                        if (warden_Count != 0 && cnts != 0)
                        {
                            Role_Id = 13;
                        }
                        else if (cnts != 0)
                        {

                            Role_Id = 13;

                        }
                        else if (warden_Count != 0)
                        {

                            Role_Id = 15;

                        }
                        else
                        {
                            Role_Id = 12;
                        }

                        Session["Mobile"] = Mobile;
                        if (Session["Mobile"].ToString() == "9495727688")
                        {
                            Role_Id = 13;
                        }
                        //if (Session["Mobile"].ToString() == "8289918958")
                        //{
                        //    Role_Id = 35;
                        //}
                        int cnt = db.CMS_Logins.Where(x => x.Active_Status == true && x.Mobile == Mobile && x.Role_Id == 12).Select(x => x.Mobile).Count();
                        Session["Faculty_Id"] = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Faculty_Id).FirstOrDefault();
                        Session["DepId"] = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Dep_Id).FirstOrDefault();
                        Session["Faculty"] = "Faculty_Login";
                        Session["Photo"] = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Photo).FirstOrDefault();
                        Session["Faculty"] = "Faculty_Login";

                        Name = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Name).FirstOrDefault();
                        Email = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Email).FirstOrDefault();

                        // Your authentication key
                        ////string authKey = "2037ANB8jQf25f112b1eP43";
                        //Multiple mobiles numbers separated by comma
                        string mobileNumber = Mobile;
                        //Sender ID,While using route4 sender id should be 6 characters long.
                        //string senderId = "TXTAPI";
                        //Your message to send, Add URL encoding here.

                        Session["Role_Id"] = Role_Id;
                        int? FacStatus = 0;
                        FacStatus = db.CMS_Logins.Where(x => x.Active_Status == true && x.Mobile == Mobile && x.Role_Id == 12).Select(x => x.Password_Status).FirstOrDefault();

                        if (FacStatus == 0 || FacStatus == null)
                        {

                            string WorkingServer = ConfigurationManager.AppSettings["WorkingServer"].ToString();
                            if (WorkingServer.Equals("Local"))
                            {
                                otp = "2582";
                            }
                            else
                            {
                                otp = CreateRandomPassword();
                                string message = HttpUtility.UrlEncode(otp + " is your One TimePassword.");
                                //status = objsmsservice.SendSMS(mobileNumber, message);
                                string authKey = "2037ANB8jQf25f112b1eP43";
                                string senderId = "CMSKTM";
                                StringBuilder sbPostData = new StringBuilder();
                                sbPostData.AppendFormat("authkey={0}", authKey);
                                sbPostData.AppendFormat("&mobiles={0}", mobileNumber);
                                sbPostData.AppendFormat("&message={0}", message);
                                sbPostData.AppendFormat("&sender={0}", senderId);
                                sbPostData.AppendFormat("&route={0}", 4);
                                sbPostData.AppendFormat("&country={0}", 91);
                                sbPostData.AppendFormat("&DLT_TE_ID={0}", "1207161777677622625");
                                string sendSMSUri = "http://adlinks.websmsc.com/api/sendhttp.php?";
                                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                                UTF8Encoding encoding = new UTF8Encoding();
                                byte[] data = encoding.GetBytes(sbPostData.ToString());
                                httpWReq.Method = "POST";
                                httpWReq.ContentType = "application/x-www-form-urlencoded";
                                httpWReq.ContentLength = data.Length;
                                using (Stream stream = httpWReq.GetRequestStream())
                                {
                                    stream.Write(data, 0, data.Length);
                                }
                                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                                StreamReader reader = new StreamReader(response.GetResponseStream());
                                string responseString = reader.ReadToEnd();
                                reader.Close();
                                response.Close();

                                try
                                {
                                    string To = string.Empty;
                                    string body = string.Empty;
                                    //bool status;

                                    To = Email.ToString().Trim();
                                    string Subject = "CMS College";
                                    using (StreamReader reader1 = new StreamReader(Server.MapPath("~/Email Template/OTP.html")))
                                    {
                                        body = reader1.ReadToEnd();


                                    }
                                    body = body.Replace("{Name}", Name);
                                    body = body.Replace("{Password}", otp);
                                    status = objservice.SendEMail2(body, To, Subject);
                                }
                                catch
                                {

                                }

                            }



                            if (cnt == 0)
                            {

                                objfaculty.Faculty_SignUp(Mobile, Name, Email, otp, Role_Id);
                            }
                            else
                            {
                                objfaculty.UpdateLogin(Mobile, Name, Email, otp, Role_Id);

                            }

                            // return RedirectToAction("Faculty_SignUp");
                            if (SubmitAction == "1")
                            {
                                return Json(Url.Action("Faculty_SignUp", "Faculty"));

                            }

                            return RedirectToAction("Faculty_SignUp");

                        }
                        else if (FacStatus == 2)
                        {
                            return Json(Url.Action("Faculty_SignUp", "Faculty"));
                        }
                        else
                        {

                            string efpwd = objpwd.EncryptPlainTextToCipherText(fpwd);
                            //string dp = db.CMS_Logins.Where(x => x.Active_Status == true && x.Mobile == Mobile && x.Role_Id == Role_Id).Select(x => x.Password).FirstOrDefault();
                            //string dpASSWORD = objpwd.DecryptCipherTextToPlainText(dp);
                            int checkpwd = db.CMS_Logins.Where(x => x.Active_Status == true && x.Mobile == Mobile && x.Role_Id == 12 && x.Password == efpwd).Select(x => x.Mobile).Count();
                            string WorkingServer = ConfigurationManager.AppSettings["WorkingServer"].ToString();
                            if (WorkingServer.Equals("Local"))
                            {
                                return RedirectToAction("Login_Check", new { Otp1 = efpwd });
                            }
                            if (checkpwd > 0)
                            {
                                // return RedirectToAction("Internal_Assesment");
                                return RedirectToAction("Login_Check", new { Otp1 = efpwd });
                            }
                            else
                            {
                                TempData["Invalid"] = "Invalid Password";
                                return RedirectToAction("Faculty_Login");
                            }
                        }


                    }
                    else
                    {
                        TempData["Invalid"] = "Invalid Mobile";
                        return RedirectToAction("Faculty_Login");
                    }
                }
                else
                {
                    TempData["Invalid"] = "Invalid Mobile";
                    return Json(Url.Action("Faculty_Login", "Faculty"));

                }

                // return RedirectToAction("Faculty_SignUp");


                // TempData["Invalid"] = "Invalid Mobile";
                // return Json(Url.Action("Faculty_Login", "Faculty"));   

            }

            catch
            {
                TempData["Invalid"] = "Invalid Mobile";
                return RedirectToAction("Faculty_Login");
            }

            // return RedirectToAction("Portal_Close");
        }


        public ActionResult ResendOTP()
        {
            bool status;
            int count = 0;
            int Role_Id = 0;
            string otp = "";

            if (Session["Mobile"] != null)
            {
                try
                {
                    var mobileNumber = Session["Mobile"].ToString();
                    int facultyid = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == mobileNumber).Select(x => x.Faculty_Id).FirstOrDefault();
                    int cnts = db.CMS_HODs.Where(x => x.Active_Status == true && x.Faculty_Id == facultyid).Select(x => x.Faculty_Id).Count();
                    int warden_Count = db.CMS_ClassWardens.Where(x => x.Active_Status == true && x.Faculty_Id == facultyid).Select(x => x.Faculty_Id).Count();
                    if (warden_Count != 0 && cnts != 0)
                    {
                        Role_Id = 13;
                    }
                    else if (cnts != 0)
                    {

                        Role_Id = 13;

                    }
                    else if (warden_Count != 0)
                    {

                        Role_Id = 15;

                    }
                    else
                    {
                        Role_Id = 12;
                    }
                    if (Session["Mobile"].ToString() == "9495727688")
                    {
                        Role_Id = 13;
                    }
                    //if (Session["Mobile"].ToString() == "8289918958")
                    //{
                    //    Role_Id = 35;
                    //}
                    int cnt = db.CMS_Logins.Where(x => x.Active_Status == true && x.Mobile == mobileNumber).Select(x => x.Mobile).Count();
                    string WorkingServer = ConfigurationManager.AppSettings["WorkingServer"].ToString();
                    if (WorkingServer.Equals("Local"))
                    {
                        otp = "2582";
                    }
                    else
                    {
                        otp = CreateRandomPassword();
                        string message = HttpUtility.UrlEncode(otp + " is your One TimePassword.");
                        status = objsmsservice.SendSMS(mobileNumber, message);
                    }

                    var Name = "";
                    var Email = "";
                    if (cnt == 0)
                    {

                        objfaculty.Faculty_SignUp(mobileNumber, Name, Email, otp, Role_Id);
                    }
                    else
                    {
                        objfaculty.UpdateLogin(mobileNumber, Name, Email, otp, Role_Id);
                    }

                    return RedirectToAction("Faculty_SignUp");

                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }


        public ActionResult Faculty_Details()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"]);
                    CMS_AcademicYear ac = objfaculty.getCurrentAcademicYear();
                    int Dep_Id = db.CMS_HODs.Where(x => x.Active_Status == true && x.Faculty_Id == Faculty_Id).Select(x => x.Dep_Id).FirstOrDefault();
                    ViewBag.Dep = Dep_Id;

                    List<Schedule> fac = objfaculty.getDepartmentFaculties(Dep_Id).ToList();
                    ViewBag.Designation = new SelectList(objfaculty.getAllDesignation(), "DesignationId", "Designation_name");
                    ViewBag.Department = new SelectList(objfaculty.getAllDepartment(), "Dep_Id", "Department");
                    return View(fac);

                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Project_Details()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    //int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"]);
                    //CMS_AcademicYear ac = objfaculty.getCurrentAcademicYear();
                    //int Dep_Id = db.CMS_HODs.Where(x => x.Active_Status == true && x. == Faculty_Id).Select(x => x.Dep_Id).FirstOrDefault();
                    //  ViewBag.Dep = Dep_Id;

                    //List<Schedule> fac = objfaculty.getDepartmentFaculties(Dep_Id).ToList();
                    //ViewBag.Designation = new SelectList(objfaculty.getAllDesignation(), "DesignationId", "Designation_name");
                    ViewBag.Department = new SelectList(objfaculty.getAllDepartment(), "Dep_Id", "Department");
                    return View();

                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult sw_Payment()
        {
            try
            {
                if (Session["Faculty_Id"] == null)
                {
                    ValueFromData(Session["Mobile"].ToString());
                }

                int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());

                //if (Faculty_Id == 151 || Faculty_Id == 41) { ViewBag.status = 1; }
                //else { ViewBag.status = 0; }

                var list = (from a in db.CMS_Facultys
                            join b in db.CMS_Designations on a.Des_Id equals b.DesignationId
                            where a.Faculty_Id == Faculty_Id// && a.Active_Status == true
                            select new { a, b }).ToList().FirstOrDefault();
                int paycount = db.CMS_FacultyPayments.Where(x => x.Faculty_Id == Faculty_Id && x.Pay_Status == true && x.Role == "Faculty" && x.Type == "2024").Count();
                //int paycount = 1;
                ViewBag.PaidCount = paycount;
                Session["Fee"] = 2000;
                //Session["Fee"] = 500;
                Session["FeeName"] = "SWFee";
                return View();
            }
            catch
            {
                return RedirectToAction("Faculty_Login");
            }
        }
        public ActionResult SWFee_Payment()
        {
            try
            {
                if (Session["Log_Id"] != null)
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    int retVal = objfaculty.SWSubmit(Faculty_Id);
                    return View();
                }
                else
                { return RedirectToAction("Faculty_Login"); }
            }
            catch
            {
                return Redirect("~/Login/Error_Page");
            }
        }
        public ActionResult SWFee_Receipt()
        {
            try
            {

                int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                string Fee = Session["Fee"].ToString();
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Report"), "ReceiptSW.rdlc");

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }

                List<CMS_FacultyPayment> list = db.CMS_FacultyPayments.Where(x => x.Faculty_Id == Faculty_Id && x.Role == "Faculty" && x.Type == "2024").ToList();
                var datee = db.CMS_FacultyPayments.Where(x => x.Faculty_Id == Faculty_Id && x.Role == "Faculty" && x.Type == "2024").Select(x => x.Pay_date).FirstOrDefault().ToString("dd/MM/yyyy");
                var no = "SW/24/" + Faculty_Id;
                var fac = (from a in db.CMS_Facultys
                           join b in db.CMS_Departments on a.Dep_Id equals b.Dep_Id
                           join c in db.CMS_Designations on a.Des_Id equals c.DesignationId
                           where a.Faculty_Id == Faculty_Id// && a.Active_Status == true
                           select new { a, b, c }).ToList().FirstOrDefault();
                string name = fac.a.Name;//db.CMS_Facultys.Where(x => x.Faculty_Id == Faculty_Id).Select(x => x.Name).FirstOrDefault();
                string Department = fac.b.Department;
                string Designation = fac.c.Designation_name;
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet1";
                reportDataSource.Value = list;
                lr.DataSources.Add(reportDataSource);

                List<ReportParameter> paraList = new List<ReportParameter>();
                paraList.Add(new ReportParameter("Department", Department));
                paraList.Add(new ReportParameter("no", no));
                paraList.Add(new ReportParameter("Name", name));
                paraList.Add(new ReportParameter("Designation", Designation));
                paraList.Add(new ReportParameter("Amount", Fee));
                paraList.Add(new ReportParameter("datee", datee));
                lr.SetParameters(paraList.ToArray());
                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;
                string deviceInfo =

                    "<DeviceInfo>" +

                    "<OutputFormat>" + reportType + "</OutputFormat>" +

                    "<PageWidth>8.5in</PageWidth>" +

                    "</DeviceInfo>";


                Warning[] warning;
                string[] streams;
                byte[] renderedBytes;

                renderedBytes = lr.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warning);
                //  return File(renderedBytes, "pdf");
                return File(renderedBytes, mimeType);
            }
            catch (Exception)
            {
                return RedirectToAction("Faculty_Login", "Faculty");
            }
        }
        #region Modifications
        public ActionResult Dashboard()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    CMS_AcademicYear Acc_Yr = objfaculty.getCurrentAcademicYear();
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    int DepId = Convert.ToInt32(Session["DepId"].ToString());
                    ViewBag.Faculty_Id = Faculty_Id;
                    ViewBag.Faculties = db.CMS_Facultys.Where(x => x.Dep_Id == DepId && x.Active_Status == true).Count();
                    int stream = db.CMS_Departments.Where(x => x.Dep_Id == DepId).Select(x => x.Stream_Type_Id).FirstOrDefault();
                    ViewBag.Department = db.CMS_Departments.Where(x => x.Dep_Id == DepId).Select(x => x.Department).FirstOrDefault();
                    ViewBag.UGCourses = (from a in db.CMS_Course_Semesters
                                         join b in db.CMS_AcademicYr_Sem_Programmes on a.Acc_Yr_Sem_Pgm_Id equals b.Acc_Yr_Sem_Pgm_Id
                                         join c in db.CMS_Programmes on b.Pgm_Id equals c.Pgm_Id
                                         join d in db.CMS_AccademicYearSemesters on b.Acc_Yr_sem_Id equals d.Acc_Yr_Sem_Id
                                         join e in db.CMS_Courses on a.Course_Id equals e.Course_Id
                                         where a.Active_Status == true && d.Acc_yr_Id == Acc_Yr.Acc_yr_Id && d.Active_Status == true && c.Active_Status == true
                                         && b.Active_Status == true && (c.Pgm_Type_Id == 1 || c.Pgm_Type_Id == 8) && e.Dep_Id == DepId && c.Stream_Type_Id == stream
                                         select e.Course_Code).Distinct().Count();
                    ViewBag.PGCourses = (from a in db.CMS_Course_Semesters
                                         join b in db.CMS_AcademicYr_Sem_Programmes on a.Acc_Yr_Sem_Pgm_Id equals b.Acc_Yr_Sem_Pgm_Id
                                         join c in db.CMS_Programmes on b.Pgm_Id equals c.Pgm_Id
                                         join d in db.CMS_AccademicYearSemesters on b.Acc_Yr_sem_Id equals d.Acc_Yr_Sem_Id
                                         join e in db.CMS_Courses on a.Course_Id equals e.Course_Id
                                         where a.Active_Status == true && d.Acc_yr_Id == Acc_Yr.Acc_yr_Id && d.Active_Status == true && c.Active_Status == true
                                         && b.Active_Status == true && c.Pgm_Type_Id == 2 && e.Dep_Id == DepId && c.Stream_Type_Id == stream
                                         select e.Course_Code).Distinct().Count();
                    var q = (from a in db.CMS_AcademicYr_Sem_Programmes
                             join b in db.CMS_AccademicYearSemesters on a.Acc_Yr_sem_Id equals b.Acc_Yr_Sem_Id
                             join c in db.CMS_Programmes on a.Pgm_Id equals c.Pgm_Id
                             join d in db.CMS_StudentClasss on a.Acc_Yr_Sem_Pgm_Id equals d.Acc_Yr_Sem_Pgm_Id
                             join e in db.CMS_Semesters on b.Sem_Id equals e.Sem_Id
                             where b.Active_Status == true && c.Active_Status == true && c.Active_Status == true
                             && c.Dep_Id == DepId && b.Acc_yr_Id == Acc_Yr.Acc_yr_Id && d.Active_Status == true
                             && b.Start_Date <= DateTime.Today && b.End_Date >= DateTime.Today
                             select new
                             {
                                 d.UPRN,
                                 c.Pgm_Type_Id,
                                 e.Semester,
                                 e.Class_Id

                             }).GroupBy(x => x.Pgm_Type_Id).ToList();
                    List<Programme> pgm = new List<Programme>();
                    foreach (var item in q)
                    {
                        Programme p = new Programme();
                        p.Pgm_Type = db.CMS_ProgrammeTypes.Where(x => x.Pgm_Type_Id == item.Key).Select(x => x.Programme_Type).FirstOrDefault();
                        if (item.Key == 1)
                        {
                            p.First = item.Where(x => x.Class_Id == 1).Count();
                            p.Second = item.Where(x => x.Class_Id == 2).Count();
                            p.Third = item.Where(x => x.Class_Id == 3).Count();
                        }
                        if (item.Key == 2)
                        {
                            p.First = item.Where(x => x.Class_Id == 4).Count();
                            p.Second = item.Where(x => x.Class_Id == 5).Count();
                            p.Third = 0;
                        }
                        if (item.Key == 8)
                        {
                            p.First = item.Where(x => x.Class_Id == 7).Count();
                            p.Second = item.Where(x => x.Class_Id == 8).Count();
                            p.Third = item.Where(x => x.Class_Id == 9).Count();
                        }
                        pgm.Add(p);
                    }

                    return View(pgm);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Internal_Mark_Home()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    List<Internals> intrnl = objfaculty.getCoursesFacultyWise(Faculty_Id).ToList();
                    return View(intrnl);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult ViewFormA(int Course_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    var item = (from a in db.CMS_Course_Semesters
                                join b in db.CMS_Courses on a.Course_Id equals b.Course_Id
                                join c in db.CMS_AcademicYr_Sem_Programmes on a.Acc_Yr_Sem_Pgm_Id equals c.Acc_Yr_Sem_Pgm_Id
                                join d in db.CMS_AccademicYearSemesters on c.Acc_Yr_sem_Id equals d.Acc_Yr_Sem_Id
                                join e in db.CMS_Course_Teachers on a.Course_Sem_Id equals e.Course_Sem_Id
                                where a.Course_Sem_Id == Course_Sem_Id && e.Active_Status == true && e.Paper_In_Charge == true
                                select new { a.Course_Sem_Id, a.Course_Nature_Type, b.Pgm_Type_Id, a.Acc_Yr_Sem_Pgm_Id, c.Acc_Yr_sem_Id, d.Acc_yr_Id, e.Faculty_Id }).FirstOrDefault();
                    ViewBag.Course_Sem_Id = item.Course_Sem_Id;
                    ViewBag.Acc_Yr_Sem_Pgm_Id = item.Acc_Yr_Sem_Pgm_Id;
                    ViewBag.Acc_yr_Id = item.Acc_yr_Id;
                    ViewBag.Faculty_Id = item.Faculty_Id;
                    int Ass_Types = (from a in db.CMS_InternalTypes
                                     join b in db.CMS_InternalAssesments on a.Int_TYpe_Id equals b.Int_Type_Id
                                     join c in db.CMS_AssesmentTypes on b.Ass_Type_Id equals c.Ass_Type_Id
                                     where a.Type == item.Course_Nature_Type.Trim() && a.Active_Status == true && a.Pgm_Type_Id == item.Pgm_Type_Id
                                     && b.Active_Status == true
                                     select new
                                     {
                                         b.Int_Ass_Id
                                     }).Distinct().Count();
                    int total = dbExam.CMS_Internal_Marks.Where(x => x.Acc_Yr_Sem_Pgm_Id == item.Acc_Yr_Sem_Pgm_Id && x.Course_Sem_Id == Course_Sem_Id && x.Active_Status == true).Select(x => x.Int_Ass_Id).Distinct().Count();
                    Boolean AForm_Status = false;
                    if (Ass_Types == total)
                        AForm_Status = true;
                    ViewBag.AForm_Status = AForm_Status;

                    CMS_Internal_MarkEntry_Schedule sch = db.CMS_Internal_MarkEntry_Schedules.Where(x => x.Acc_Yr_Sem_Id == item.Acc_Yr_sem_Id && x.Active_Status == true).FirstOrDefault();
                    Boolean Status = false;
                    if (sch != null)
                    {
                        if (sch.Start_Date <= DateTime.Now && sch.End_Date >= DateTime.Now)
                        {
                            Status = true;
                        }
                    }
                    ViewBag.Status = Status;

                    List<Internals> intrnl = objfaculty.ViewFormA(Course_Sem_Id).ToList();

                    return View(intrnl);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Add_Internal_Mark(int Acc_Yr_Sem_Pgm_Id, int Course_Sem_Id, int Int_Ass_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Internals> intrnl = objfaculty.getInternalMark(Acc_Yr_Sem_Pgm_Id, Course_Sem_Id, Int_Ass_Id).ToList();
                    var q = (from a in db.CMS_Course_Semesters
                             join b in db.CMS_Courses on a.Course_Id equals b.Course_Id
                             join c in db.CMS_AcademicYr_Sem_Programmes on a.Acc_Yr_Sem_Pgm_Id equals c.Acc_Yr_Sem_Pgm_Id
                             join d in db.CMS_AccademicYearSemesters on c.Acc_Yr_sem_Id equals d.Acc_Yr_Sem_Id
                             join e in db.CMS_Semesters on d.Sem_Id equals e.Sem_Id
                             join f in db.CMS_Programmes on c.Pgm_Id equals f.Pgm_Id
                             where a.Course_Sem_Id == Course_Sem_Id
                             select new
                             {
                                 b.Course_Code,
                                 b.Course_Name,
                                 e.Semester,
                                 f.Programme
                             }).FirstOrDefault();
                    if (q != null)
                    {
                        ViewBag.Course_Code = q.Course_Code;
                        ViewBag.Course_Name = q.Course_Name;
                        ViewBag.Semester = q.Semester;
                        ViewBag.Programme = q.Programme;
                    }
                    ViewBag.Assesment = (from a in db.CMS_InternalAssesments
                                         join b in db.CMS_AssesmentTypes on a.Ass_Type_Id equals b.Ass_Type_Id
                                         where a.Int_Ass_Id == Int_Ass_Id && a.Active_Status == true && b.Active_Status == true
                                         select b.AssesmentType).FirstOrDefault();
                    ViewBag.Ass_Mark = (from a in db.CMS_InternalAssesments
                                        where a.Int_Ass_Id == Int_Ass_Id && a.Active_Status == true
                                        select a.Max_Mark).FirstOrDefault();
                    ViewBag.Acc_Yr_Sem_Pgm_Id = Acc_Yr_Sem_Pgm_Id;
                    ViewBag.Course_Sem_Id = Course_Sem_Id;
                    ViewBag.Int_Ass_Id = Int_Ass_Id;
                    ViewBag.Int_Type_Id = db.CMS_InternalAssesments.Where(x => x.Active_Status == true && x.Int_Ass_Id == Int_Ass_Id).Select(x => x.Int_Type_Id).FirstOrDefault(); ;
                    // ViewBag.Total = intrnl.Select(x => x.MaxMark).FirstOrDefault();
                    var MarkDetails = intrnl.Select(x => x.MarkDefinition).FirstOrDefault();
                    ViewBag.Total = MarkDetails;



                    if (ViewBag.Total.Count == 0 && ViewBag.Assesment == "Attendance")
                    {
                        ViewBag.AttTotal = intrnl.Select(x => x.Entered_Max_Mark).FirstOrDefault();
                    }


                    return View(intrnl);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult deleteInternalMarks(int Acc_Yr_Sem_Pgm_Id, int Course_Sem_Id, int Int_Ass_Id, int maxMark, int markCount)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    var SemDetails = (from c in db.CMS_AcademicYr_Sem_Programmes
                                      join d in db.CMS_Programmes on c.Pgm_Id equals d.Pgm_Id
                                      join e in db.CMS_AccademicYearSemesters on c.Acc_Yr_sem_Id equals e.Acc_Yr_Sem_Id
                                      where c.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Sem_Pgm_Id
                                      select new
                                      {

                                          e.Sem_Id,
                                          e.Acc_yr_Id
                                      }).FirstOrDefault();

                    Exam_Notification NotSch = db.Exam_Notifications.Where(x => x.Sem_Id == SemDetails.Sem_Id && x.Acc_Yr_Id == SemDetails.Acc_yr_Id && x.Exam_Type == "Regular" && x.Active_Status == true).FirstOrDefault();
                    if (NotSch != null)
                    {
                        Exam_Result_Schedule ResSch = db.Exam_Result_Schedules.Where(x => x.Notification_No == NotSch.Notification_No && x.Active_Status == true).FirstOrDefault();
                        if (ResSch == null)
                        {

                            List<CMS_Internal_Mark> mark = db.CMS_Internal_Marks.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Sem_Pgm_Id && x.Course_Sem_Id == Course_Sem_Id && x.Int_Ass_Id == Int_Ass_Id && x.Int_Mark_Count == markCount && x.Entered_Max_Mark == maxMark).ToList();
                            foreach (var item in mark)
                            {
                                item.Active_Status = false;
                            }
                            db.SaveChanges();
                            return Json(1, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            return Json(0, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(0, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Add_Internal_Mark_Student(int Acc_Yr_Sem_Pgm_Id, int Course_Sem_Id, int Int_Ass_Id, string Uprn)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Internals> intrnl = objfaculty.getInternalMarkStudentWise(Acc_Yr_Sem_Pgm_Id, Course_Sem_Id, Int_Ass_Id, Uprn).ToList();

                    ViewBag.Assesment = (from a in db.CMS_InternalAssesments
                                         join b in db.CMS_AssesmentTypes on a.Ass_Type_Id equals b.Ass_Type_Id
                                         where a.Int_Ass_Id == Int_Ass_Id && a.Active_Status == true && b.Active_Status == true
                                         select b.AssesmentType).FirstOrDefault();
                    ViewBag.Ass_Mark = (from a in db.CMS_InternalAssesments
                                        where a.Int_Ass_Id == Int_Ass_Id && a.Active_Status == true
                                        select a.Max_Mark).FirstOrDefault();
                    ViewBag.Acc_Yr_Sem_Pgm_Id = Acc_Yr_Sem_Pgm_Id;
                    ViewBag.Course_Sem_Id = Course_Sem_Id;
                    ViewBag.Int_Ass_Id = Int_Ass_Id;
                    ViewBag.Int_Type_Id = db.CMS_InternalAssesments.Where(x => x.Active_Status == true && x.Int_Ass_Id == Int_Ass_Id).Select(x => x.Int_Type_Id).FirstOrDefault(); ;
                    var MarkDetails = intrnl.Select(x => x.MarkDefinition).FirstOrDefault();
                    ViewBag.Total = MarkDetails;
                    if (ViewBag.Total.Count == 0 && ViewBag.Assesment == "Attendance")
                    {
                        ViewBag.AttTotal = intrnl.Select(x => x.Entered_Max_Mark).FirstOrDefault();
                    }


                    return View(intrnl);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Error_Page");

            }

        }
        public ActionResult Add_ISE_Mark(int Acc_Yr_Sem_Pgm_Id, int Course_Sem_Id, int Int_Ass_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Internals> intrnl = objfaculty.getInternalMark(Acc_Yr_Sem_Pgm_Id, Course_Sem_Id, Int_Ass_Id).ToList();
                    var q = (from a in db.CMS_Course_Semesters
                             join b in db.CMS_Courses on a.Course_Id equals b.Course_Id
                             join c in db.CMS_AcademicYr_Sem_Programmes on a.Acc_Yr_Sem_Pgm_Id equals c.Acc_Yr_Sem_Pgm_Id
                             join d in db.CMS_AccademicYearSemesters on c.Acc_Yr_sem_Id equals d.Acc_Yr_Sem_Id
                             join e in db.CMS_Semesters on d.Sem_Id equals e.Sem_Id
                             join f in db.CMS_Programmes on c.Pgm_Id equals f.Pgm_Id
                             where a.Course_Sem_Id == Course_Sem_Id
                             select new
                             {
                                 b.Course_Code,
                                 b.Course_Name,
                                 e.Semester,
                                 f.Programme
                             }).FirstOrDefault();
                    if (q != null)
                    {
                        ViewBag.Course_Code = q.Course_Code;
                        ViewBag.Course_Name = q.Course_Name;
                        ViewBag.Semester = q.Semester;
                        ViewBag.Programme = q.Programme;
                    }
                    ViewBag.Assesment = (from a in db.CMS_InternalAssesments
                                         join b in db.CMS_AssesmentTypes on a.Ass_Type_Id equals b.Ass_Type_Id
                                         where a.Int_Ass_Id == Int_Ass_Id && a.Active_Status == true && b.Active_Status == true
                                         select b.AssesmentType).FirstOrDefault();
                    ViewBag.Ass_Mark = (from a in db.CMS_InternalAssesments
                                        where a.Int_Ass_Id == Int_Ass_Id && a.Active_Status == true
                                        select a.Max_Mark).FirstOrDefault();
                    ViewBag.Acc_Yr_Sem_Pgm_Id = Acc_Yr_Sem_Pgm_Id;
                    ViewBag.Course_Sem_Id = Course_Sem_Id;
                    ViewBag.Int_Ass_Id = Int_Ass_Id;
                    return View(intrnl);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult addInternalMarks(Semesters ac)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ac.Created_By = new Guid(Session["Log_Id"].ToString());
                    ac.Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    int retVal = objfaculty.addInternalMarks(ac);
                    return Json(retVal, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        public ActionResult getAttendanceMark(int Acc_Yr_Sem_Pgm_Id, int maxMark, decimal per)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int pgmTypeId = (from a in db.CMS_AcademicYr_Sem_Programmes
                                     join b in db.CMS_Programmes on a.Pgm_Id equals b.Pgm_Id
                                     where a.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Sem_Pgm_Id
                                     select b.Pgm_Type_Id).FirstOrDefault();
                    decimal intMark = db.CMS_Internal_Attendance_Marks.Where(x => x.Max_Mark == maxMark && x.Per_From <= per && x.Per_To >= per && x.Pgm_Type_Id == pgmTypeId).Select(x => x.Mark).FirstOrDefault();

                    return Json(intMark, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        public ActionResult getMaxMark(int Int_Type_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    int maxmark = db.CMS_InternalAssesments.Where(x => x.Int_Type_Id == Int_Type_Id && x.Active_Status == true).Select(x => x.Max_Mark).FirstOrDefault();

                    return Json(maxmark, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        #endregion
        #region Faculty_profile
        public ActionResult profileView()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    Guid created_by = new Guid(Session["Log_Id"].ToString());
                    int dep_id = Convert.ToInt32(Session["DepId"]);
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"]);
                    ViewBag.Designation = new SelectList(objfaculty.getAllDesignation(), "Designation_name", "Designation_name");
                    List<CMS_Admission.Models.Faculty> fac = objfaculty.FacultyDeatails(Faculty_Id).ToList();
                    return View(fac);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult profile()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    Guid created_by = new Guid(Session["Log_Id"].ToString());
                    int dep_id = Convert.ToInt32(Session["DepId"]);
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"]);
                    ViewBag.Designation = new SelectList(objfaculty.getAllDesignation(), "Designation_name", "Designation_name");
                    List<CMS_Admission.Models.Faculty> fac = objfaculty.FacultyDeatails(Faculty_Id).ToList();
                    if (Faculty_Id == 62)
                    {
                        ViewBag.Faculty_Id = "62";
                        // LoadGscholarProfile();
                    }
                    return View(fac);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Edit_Profile(CMS_Faculty fac, HttpPostedFileBase Photo)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (Photo != null)
                    {
                        Conversion convert = new Conversion();
                        var filePath = string.Empty;
                        string extension = System.IO.Path.GetExtension(Photo.FileName).Trim();

                        string filename1 = fac.Name + extension;
                        filePath = Path.Combine(Server.MapPath("~/Images/Faculty_Photo"), filename1);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        Image bm = System.Drawing.Image.FromStream(Photo.InputStream);
                        convert.ResizeImage((Bitmap)bm, 150, 200, 80, filePath);
                        fac.Photo = filename1;

                    }
                    fac.Created_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Edit_Faclty(fac);
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion
        public ActionResult Portal_Close()
        {

            return View();

        }
        public ActionResult AdonPaymnt_List()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Student> sch = new List<Student>();
                    sch = objfaculty.getPaymentList().ToList();
                    return View(sch);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");
            }
        }
        [HttpGet]
        public ActionResult GetPassword(string Mob)
        {
            string pasw = db.CMS_Logins.Where(x => x.Mobile == Mob && x.Active_Status == true && (x.Role_Id == 12 || x.Role_Id == 13 || x.Role_Id == 15)).Select(x => x.Password).FirstOrDefault();
            string dpASSWORD = objpwd.DecryptCipherTextToPlainText(pasw);
            ViewBag.password = dpASSWORD;
            return View();
        }

        public ActionResult Time_Table()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    CMS_AcademicYear ac = objfaculty.getCurrentAcademicYear();
                    //ViewBag.Updated_Year = new SelectList(objLearn.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");
                    // ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Faculties = new SelectList(objfaculty.getAllFaculties().OrderBy(x => x.Name), "Faculty_Id", "Name");
                    ViewBag.Sem = new SelectList(objfaculty.Search_Semstr(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    int DepId = Convert.ToInt32(Session["DepId"].ToString());
                    List<Schedule> sc = objfaculty.getTimetable_Programme(DepId).ToList();
                    ViewBag.CurrentYear = db.CMS_AcademicYears.Where(x => x.Active_Status == true && x.Acc_yr_Id == ac.Acc_yr_Id).Select(x => x.Year).FirstOrDefault();
                    ViewBag.DepartmentName = db.CMS_Departments.Where(x => x.Dep_Id == DepId && x.Active_Status == true).Select(x => x.Department).FirstOrDefault();

                    return View(sc);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Create_Time_Table()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.Updated_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");

                    // ViewBag.Sem = new SelectList(objfaculty.Search_Semstr(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Sem = new SelectList(objfaculty.Search_Semstr(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Day = new SelectList(objfaculty.getDays(), "Day_Id", "Day");

                    return View();
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult Get_Faculty(int Faculty_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    List<Schedule> sch = new List<Schedule>();
                    sch = objfaculty.Get_Faculty(Faculty_Id).ToList();
                    return Json(sch, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Get_Dy()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    List<CMS_Day> sch = new List<CMS_Day>();
                    sch = objfaculty.getDays().ToList();
                    return Json(sch, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Edit_Faculty(CMS_Faculty cf, HttpPostedFileBase Photo)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (Photo != null)
                    {
                        Conversion convert = new Conversion();
                        var filePath = string.Empty;
                        string extension = System.IO.Path.GetExtension(Photo.FileName).Trim();

                        // extension1 = System.IO.Path.GetExtension(Request.Files["Image1"].FileName);
                        string filename1 = Photo.FileName;
                        filePath = Path.Combine(Server.MapPath("~/Images/Faculty_Photo"), filename1);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        Image bm = System.Drawing.Image.FromStream(Photo.InputStream);
                        convert.ResizeImage((Bitmap)bm, 150, 200, 80, filePath);
                        cf.Photo = filename1;

                    }


                    objfaculty.Edit_Faculty(cf);

                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete_Faculty(CMS_Faculty cf)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    cf.Deleted_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Delete_Faculty(cf);

                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult getAllFaculties()
        {

            try
            {

                if (Session["Log_Id"] != null)
                {

                    List<Schedule> hsc = new List<Schedule>();
                    hsc = objfaculty.getAllFaculties().ToList();
                    return Json(hsc, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Add_Faculty(CMS_Faculty fac, HttpPostedFileBase Photo)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    fac.Created_By = new Guid(Session["Log_Id"].ToString());
                    var c = db.CMS_Facultys.Where(x => x.Mobile == fac.Mobile).Select(x => x.Faculty_Id).Count();
                    if (c > 0)
                    { return Json(2, JsonRequestBehavior.AllowGet); }
                    else
                    {
                        if (Photo != null)
                        {
                            Conversion convert = new Conversion();
                            var filePath = string.Empty;
                            string extension = System.IO.Path.GetExtension(Photo.FileName).Trim();

                            // extension1 = System.IO.Path.GetExtension(Request.Files["Image1"].FileName);
                            string filename1 = fac.Name + extension;
                            filePath = Path.Combine(Server.MapPath("~/Images/Faculty_Photo"), filename1);
                            Photo.SaveAs(filePath);
                            //Image bm = System.Drawing.Image.FromStream(Marklist.InputStream);
                            //convert.ResizeImage((Bitmap)bm, 150, 200, 80, filePath);
                            fac.Photo = filename1;

                        }
                        objfaculty.Add_Faculty(fac);
                        return Json(1, JsonRequestBehavior.AllowGet);
                    }
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Send_SMS(Schedule DS)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    string mob = DS.Mobile;
                    if (mob != "0" && mob != null)
                    {
                        objfaculty.Send_SMS(DS);

                        return Json(1, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(0, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Send_Email(Schedule DS)
        {
            if (Session["Log_Id"] != null)
            {
                int count = 0;
                try
                {


                    string email = DS.Email;
                    if (email != "0" && email != null)
                    {

                        string To = string.Empty;
                        string body = string.Empty;
                        bool status;

                        To = DS.Email;
                        string Subject = "CMS College";
                        using (StreamReader reader1 = new StreamReader(Server.MapPath("~/Email Template/FacultyRegistration.html")))
                        {
                            body = reader1.ReadToEnd();


                        }
                        body = body.Replace("{Name}", DS.Name);
                        body = body.Replace("{Content}", DS.Mail);


                        status = objservice.SendEMail2(body, To, Subject);
                        count++;
                        return Json(2, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(0, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (SystemException ex)
                {
                    if (count != 0)
                    {
                        return Json(0, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(0, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Search_Faculty(int Dep_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    List<Schedule> sch = new List<Schedule>();
                    sch = objfaculty.getFaculty(Dep_Id).ToList();
                    return Json(sch, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        #region AssignFaculty
        public ActionResult Assign_Faculty()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    int Dep_Id = Convert.ToInt32(Session["DepId"].ToString());
                    ViewBag.Programmes_Type = new SelectList(objfaculty.getProgrammeTypesByDep(Dep_Id), "Pgm_Type_Id", "Pgm_Type", 1);
                    int Pgm_TypeID = objfaculty.getProgrammeTypesByDep(Dep_Id).Select(x => x.Pgm_Type_Id).FirstOrDefault();
                    ViewBag.Semester = new SelectList(objfaculty.getSemesterByDep(Dep_Id, Pgm_TypeID), "Acc_Yr_Sem_Id", "Semester");
                    return View();

                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult getSemesterByDep(int Pgm_Type_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Dep_Id = Convert.ToInt32(Session["DepId"].ToString());
                    List<Programme> sem = objfaculty.getSemesterByDep(Dep_Id, Pgm_Type_Id).ToList();
                    return Json(sem, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult getCoursesByDep(int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Dep_Id = Convert.ToInt32(Session["DepId"].ToString());
                    List<Schedule> courses = objfaculty.getCoursesByDep(Dep_Id, Acc_Yr_Sem_Id).ToList();
                    return Json(courses, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult getCourseAndFacultyDetails(int Course_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Dep_Id = Convert.ToInt32(Session["DepId"].ToString());
                    List<Schedule> courses = objfaculty.getCourseAndFacultyDetails(Dep_Id, Course_Sem_Id).ToList();
                    return Json(courses, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        public ActionResult Login_Check(string OTP1, string OTP2, string OTP3, string OTP4)
        {

            try
            {

                string mob = Session["Mobile"].ToString();
                string OTP = OTP1 + OTP2 + OTP3 + OTP4;


                int roleid = 0;
                string WorkingServer = ConfigurationManager.AppSettings["WorkingServer"].ToString();
                if (WorkingServer.Equals("Local"))
                {
                    roleid = Convert.ToInt32(Session["Role_Id"]);
                    //if (Session["Mobile"].ToString() == "9495727688")
                    //{
                    //    roleid = 13;
                    //}
                    OTP = db.CMS_Logins.Where(x => x.Active_Status == true && x.Role_Id == 12 && x.Mobile == mob).Select(x => x.Password).FirstOrDefault();

                }
                else
                {
                    roleid = Convert.ToInt32(Session["Role_Id"]);
                    //db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Mobile == mob).Select(x => x.Role_Id).FirstOrDefault();
                    if (Session["Mobile"].ToString() == "9495727688")
                    {
                        roleid = 13;
                    }
                }
                if (roleid != 0)
                {
                    if (roleid == 35)
                    {
                        Session["Name"] = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == 12 && x.Mobile == mob).Select(x => x.Name).FirstOrDefault();
                        Session["Log_Id"] = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == 12 && x.Mobile == mob).Select(x => x.Log_Id).FirstOrDefault();

                        Session["Role"] = db.CMS_Roles.Where(x => x.Role_Id == roleid).Select(x => x.Role).FirstOrDefault();

                        List<Menu> pgms = objLogin.getMenu_Faculty(roleid, mob).ToList();
                        Session["Menu"] = pgms.ToList();
                        if (pgms.Count > 0)
                        {
                            string Mobile = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == 12).Select(x => x.Mobile).FirstOrDefault();
                            string Email = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == 12).Select(x => x.Email).FirstOrDefault();
                            string Name = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == 12).Select(x => x.Name).FirstOrDefault();
                            if (Name == null || Name == "")
                            {
                                Name = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Name).FirstOrDefault();
                                Session["Name"] = Name;
                            }
                            if (Email == null || Email == "")
                            {
                                Email = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Email).FirstOrDefault();
                            }
                            objfaculty.UpdateLogin(Mobile, Name, Email, OTP, roleid);
                            string s = pgms[0].SubMenus[0].Action;
                            string s1 = pgms[0].Name_Controller;
                            string s2 = pgms[0].Area;
                            int? pwdChangeStatus = 0;
                            pwdChangeStatus = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == 12 && x.Mobile == mob).Select(x => x.Password_Status).FirstOrDefault();
                            if (WorkingServer.Equals("Local"))
                            {
                                return RedirectToAction("Internal_Mark_Home");
                            }
                            if (pwdChangeStatus == 1)
                            {
                                return RedirectToAction("Internal_Mark_Home");
                            }
                            else
                            {
                                return RedirectToAction("Faculty_Reset_Passwords");
                            }
                        }
                        else
                        {
                            TempData["Invalid"] = "Invalid OTP";
                            return RedirectToAction("Faculty_SignUp");

                        }
                    }
                    else
                    {
                        Session["Name"] = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == 12 && x.Mobile == mob).Select(x => x.Name).FirstOrDefault();
                        Session["Log_Id"] = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == 12 && x.Mobile == mob).Select(x => x.Log_Id).FirstOrDefault();

                        Session["Role"] = db.CMS_Roles.Where(x => x.Role_Id == roleid).Select(x => x.Role).FirstOrDefault();

                        List<Menu> pgms = objLogin.getMenu_Faculty(roleid, mob).ToList();
                        Session["Menu"] = pgms.ToList();
                        Session["SubMenu"] = objLogin.getMenuByRole(roleid).ToList();
                        if (pgms.Count > 0)
                        {
                            string Mobile = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == 12).Select(x => x.Mobile).FirstOrDefault();
                            string Email = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == 12).Select(x => x.Email).FirstOrDefault();
                            string Name = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == 12).Select(x => x.Name).FirstOrDefault();
                            if (Name == null || Name == "")
                            {
                                Name = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Name).FirstOrDefault();
                                Session["Name"] = Name;
                            }
                            if (Email == null || Email == "")
                            {
                                Email = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Email).FirstOrDefault();
                            }
                            objfaculty.UpdateLogin(Mobile, Name, Email, OTP, roleid);
                            //string s = pgms[0].SubMenus[0].Action;
                            //string s1 = pgms[0].Name_Controller;
                            //string s2 = pgms[0].Area;
                            int? pwdChangeStatus = 0;
                            pwdChangeStatus = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == 12 && x.Mobile == mob).Select(x => x.Password_Status).FirstOrDefault();
                            if (WorkingServer.Equals("Local"))
                            {
                                return RedirectToAction("Faculty_Dashboard");
                            }
                            if (pwdChangeStatus == 1)
                            {
                                return RedirectToAction("Faculty_Dashboard");
                            }
                            else
                            {
                                return RedirectToAction("Faculty_Reset_Passwords");
                            }
                        }
                        else
                        {
                            TempData["Invalid"] = "Invalid OTP";
                            return RedirectToAction("Faculty_SignUp");

                        }
                    }
                    //else if (roleid == 13)
                    //{
                    //    Session["Name"] = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == roleid && x.Mobile == mob).Select(x => x.Name).FirstOrDefault();
                    //    Session["Log_Id"] = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == roleid && x.Mobile == mob).Select(x => x.Log_Id).FirstOrDefault();

                    //    Session["Role"] = db.CMS_Roles.Where(x => x.Role_Id == roleid).Select(x => x.Role).FirstOrDefault();

                    //    List<Menu> pgms = objLogin.getMenu_Faculty(roleid, mob).ToList();
                    //    Session["Menu"] = pgms.ToList();
                    //    if (pgms.Count > 0)
                    //    {
                    //        string Mobile = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == roleid).Select(x => x.Mobile).FirstOrDefault();
                    //        string Email = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == roleid).Select(x => x.Email).FirstOrDefault();
                    //        string Name = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == roleid).Select(x => x.Name).FirstOrDefault();
                    //        if (Name == null || Name == "")
                    //        {
                    //            Name = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Name).FirstOrDefault();
                    //            Session["Name"] = Name;
                    //        }
                    //        if (Email == null || Email == "")
                    //        {
                    //            Email = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Email).FirstOrDefault();
                    //        }
                    //        objfaculty.UpdateLogin(Mobile, Name, Email, OTP, roleid);
                    //        string s = pgms[0].SubMenus[0].Action;
                    //        string s1 = pgms[0].Name_Controller;
                    //        string s2 = pgms[0].Area;
                    //        int? pwdChangeStatus = 0;
                    //        pwdChangeStatus = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == roleid && x.Mobile == mob).Select(x => x.Password_Status).FirstOrDefault();
                    //        if (WorkingServer.Equals("Local"))
                    //        {
                    //            return RedirectToAction("Faculty_Main_Home");
                    //        }
                    //        if (pwdChangeStatus == 1)
                    //        {
                    //            return RedirectToAction("Faculty_Main_Home");
                    //        }
                    //        else
                    //        {
                    //            return RedirectToAction("Faculty_Reset_Passwords");
                    //        }
                    //    }

                    //    else
                    //    {
                    //        TempData["Invalid"] = "Invalid OTP";
                    //        return RedirectToAction("Faculty_SignUp");

                    //    }
                    //}
                    //else if (roleid == 15)
                    //{
                    //    Session["Name"] = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == roleid && x.Mobile == mob).Select(x => x.Name).FirstOrDefault();
                    //    Session["Log_Id"] = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == roleid && x.Mobile == mob).Select(x => x.Log_Id).FirstOrDefault();

                    //    Session["Role"] = db.CMS_Roles.Where(x => x.Role_Id == roleid).Select(x => x.Role).FirstOrDefault();

                    //    List<Menu> pgms = objLogin.getMenu_Faculty(roleid, mob).ToList();
                    //    Session["Menu"] = pgms.ToList();
                    //    if (pgms.Count > 0)
                    //    {
                    //        string Mobile = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == roleid).Select(x => x.Mobile).FirstOrDefault();
                    //        string Email = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == roleid).Select(x => x.Email).FirstOrDefault();
                    //        string Name = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == roleid).Select(x => x.Name).FirstOrDefault();
                    //        if (Name == null || Name == "")
                    //        {
                    //            Name = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Name).FirstOrDefault();
                    //            Session["Name"] = Name;
                    //        }
                    //        if (Email == null || Email == "")
                    //        {
                    //            Email = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Email).FirstOrDefault();
                    //        }
                    //        objfaculty.UpdateLogin(Mobile, Name, Email, OTP, roleid);
                    //        string s = pgms[0].SubMenus[0].Action;
                    //        string s1 = pgms[0].Name_Controller;
                    //        string s2 = pgms[0].Area;
                    //        //return RedirectToAction("Internal_Assesment");
                    //        int? pwdChangeStatus = 0;
                    //        pwdChangeStatus = db.CMS_Logins.Where(x => x.Active_Status == true && x.Password == OTP && x.Role_Id == roleid && x.Mobile == mob).Select(x => x.Password_Status).FirstOrDefault();
                    //        if (WorkingServer.Equals("Local"))
                    //        {
                    //            return RedirectToAction("Faculty_Main_Home_Classwarden");
                    //        }
                    //        if (pwdChangeStatus == 1)
                    //        {
                    //            return RedirectToAction("Faculty_Main_Home_Classwarden");
                    //        }
                    //        else
                    //        {
                    //            return RedirectToAction("Faculty_Reset_Passwords");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        TempData["Invalid"] = "Invalid OTP";
                    //        return RedirectToAction("Faculty_SignUp");

                    //    }
                    //}





                    return RedirectToAction("Faculty_SignUp");
                }

                else
                {
                    TempData["Invalid"] = "Invalid OTP";
                    return RedirectToAction("Faculty_SignUp");
                }




            }
            catch
            {
                TempData["Invalid"] = "Invalid OTP";
                return RedirectToAction("Faculty_Login");
            }


        }

        public ActionResult ValueFromData(string Mobile)
        {

            try
            {

                string mob = Mobile;
                int roleid = 0;
                roleid = db.CMS_Logins.Where(x => x.Active_Status == true && x.Mobile == mob && (x.Role_Id == 12 || x.Role_Id == 13 || x.Role_Id == 15)).Select(x => x.Role_Id).FirstOrDefault();

                Session["Name"] = db.CMS_Logins.Where(x => x.Active_Status == true && x.Role_Id == roleid && x.Mobile == mob).Select(x => x.Name).FirstOrDefault();
                Session["Log_Id"] = db.CMS_Logins.Where(x => x.Active_Status == true && x.Role_Id == roleid && x.Mobile == mob).Select(x => x.Log_Id).FirstOrDefault();
                Session["Role"] = db.CMS_Roles.Where(x => x.Role_Id == roleid).Select(x => x.Role).FirstOrDefault();
                Session["Faculty_Id"] = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == mob).Select(x => x.Faculty_Id).FirstOrDefault();
                Session["Mobile"] = Mobile;
                List<Menu> pgms = objLogin.getMenu_Faculty(roleid, mob).ToList();
                Session["Menu"] = pgms.ToList();
                if (pgms.Count > 0)
                {
                    string Email = db.CMS_Logins.Where(x => x.Active_Status == true && x.Role_Id == roleid && x.Mobile == mob).Select(x => x.Email).FirstOrDefault();
                    string Name = db.CMS_Logins.Where(x => x.Active_Status == true && x.Role_Id == roleid && x.Mobile == mob).Select(x => x.Name).FirstOrDefault();
                    string s = pgms[0].SubMenus[0].Action;
                    string s1 = pgms[0].Name_Controller;
                    string s2 = pgms[0].Area;
                    return RedirectToAction("sw_Payment");

                }
                else
                {
                    TempData["Invalid"] = "Login Again";
                    return RedirectToAction("Faculty_SignUp");
                }


                //  return RedirectToAction("Faculty_SignUp");

            }
            catch
            {
                TempData["Invalid"] = "Login Again";
                return RedirectToAction("Faculty_Login");
            }
        }
        //[HttpPost]
        //public ActionResult ResendOTP(string Mobile)
        //{
        //    int count = 0;
        //    try
        //    {
        //        int faid = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == Mobile).Select(x => x.Faculty_Id).FirstOrDefault();
        //        string OTP = dbExam.CMS_Faculty_Logins.Where(x => x.Active_Status == true && x.Fac_Id == faid).Select(x => x.OTP).FirstOrDefault();
        //        if (OTP != null)
        //        {
        //            if (Mobile != null)
        //            {
        //                // string otp = CreateRandomPassword();
        //                //objfaculty.Faculty_SignUp(Mobile, OTP);
        //                //Your authentication key
        //                string authKey = "A291bfb88c02e836b3ad7abb5d1c98839";
        //                //Multiple mobiles numbers separated by comma
        //                string mobileNumber = Mobile;
        //                //Sender ID,While using route4 sender id should be 6 characters long.
        //                string senderId = "CMSKTM";
        //                //Your message to send, Add URL encoding here.
        //                string message = HttpUtility.UrlEncode(OTP + " is your One Time Password.");

        //                //Prepare you post parameters
        //                StringBuilder sbPostData = new StringBuilder();
        //                sbPostData.AppendFormat("workingkey={0}", authKey);
        //                sbPostData.AppendFormat("&to={0}", mobileNumber);
        //                sbPostData.AppendFormat("&message={0}", message);
        //                sbPostData.AppendFormat("&sender={0}", senderId);
        //                //sbPostData.AppendFormat("&route={0}", "default");


        //                //Call Send SMS API
        //                string sendSMSUri = "http://alerts.smsclogin.com/api/web2sms.php?";
        //                //Create HTTPWebrequest
        //                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
        //                //Prepare and Add URL Encoded data
        //                UTF8Encoding encoding = new UTF8Encoding();
        //                byte[] data = encoding.GetBytes(sbPostData.ToString());
        //                //Specify post method
        //                httpWReq.Method = "POST";
        //                httpWReq.ContentType = "application/x-www-form-urlencoded";
        //                httpWReq.ContentLength = data.Length;
        //                using (Stream stream = httpWReq.GetRequestStream())
        //                {
        //                    stream.Write(data, 0, data.Length);
        //                }
        //                //Get the response
        //                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
        //                StreamReader reader = new StreamReader(response.GetResponseStream());
        //                string responseString = reader.ReadToEnd();

        //                // Close the response
        //                reader.Close();
        //                response.Close();
        //                count++;
        //                return Json(1, JsonRequestBehavior.AllowGet);
        //            }
        //            return Json(2, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            if (Mobile != null)
        //            {
        //                 string otp = CreateRandomPassword();
        //                 objfaculty.Faculty_SignUp(Mobile, otp);
        //                //Your authentication key
        //                string authKey = "A291bfb88c02e836b3ad7abb5d1c98839";
        //                //Multiple mobiles numbers separated by comma
        //                string mobileNumber = Mobile;
        //                //Sender ID,While using route4 sender id should be 6 characters long.
        //                string senderId = "CMSKTM";
        //                //Your message to send, Add URL encoding here.
        //                string message = HttpUtility.UrlEncode(otp + " is your One Time Password.");

        //                //Prepare you post parameters
        //                StringBuilder sbPostData = new StringBuilder();
        //                sbPostData.AppendFormat("workingkey={0}", authKey);
        //                sbPostData.AppendFormat("&to={0}", mobileNumber);
        //                sbPostData.AppendFormat("&message={0}", message);
        //                sbPostData.AppendFormat("&sender={0}", senderId);
        //                //sbPostData.AppendFormat("&route={0}", "default");


        //                //Call Send SMS API
        //                string sendSMSUri = "http://alerts.smsclogin.com/api/web2sms.php?";
        //                //Create HTTPWebrequest
        //                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
        //                //Prepare and Add URL Encoded data
        //                UTF8Encoding encoding = new UTF8Encoding();
        //                byte[] data = encoding.GetBytes(sbPostData.ToString());
        //                //Specify post method
        //                httpWReq.Method = "POST";
        //                httpWReq.ContentType = "application/x-www-form-urlencoded";
        //                httpWReq.ContentLength = data.Length;
        //                using (Stream stream = httpWReq.GetRequestStream())
        //                {
        //                    stream.Write(data, 0, data.Length);
        //                }
        //                //Get the response
        //                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
        //                StreamReader reader = new StreamReader(response.GetResponseStream());
        //                string responseString = reader.ReadToEnd();

        //                // Close the response
        //                reader.Close();
        //                response.Close();
        //                count++;
        //                return Json(1, JsonRequestBehavior.AllowGet);
        //            }
        //            return Json(2, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    catch
        //    {
        //        return Json(0, JsonRequestBehavior.AllowGet);
        //    }


        //}
        public ActionResult Internal_Assesment()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Internals> assmnt = objfaculty.getAllAssesments().ToList();
                    ViewBag.Assesment = dbExam.CMS_AssesmentTypes.Where(X => X.Active_Status == true).ToList();
                    return View(assmnt);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Assign_Internal_Mark(int Sem_Id, int Acc_Yr_Pgm_Id, int Course_Id, int Int_Ass_Id, int Acc_Yr_Id, int Pgm_Type_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.Sem_Id = Sem_Id;
                    ViewBag.Acc_Yr_Pgm_Id = Acc_Yr_Pgm_Id;
                    ViewBag.Course_Id = Course_Id;
                    ViewBag.Int_Ass_Id = Int_Ass_Id;
                    ViewBag.year = Acc_Yr_Id;
                    ViewBag.Pgm_Type_Id = Pgm_Type_Id;
                    int assid = db.CMS_InternalAssesments.Where(x => x.Int_Ass_Id == Int_Ass_Id && x.Active_Status == true).Select(x => x.Ass_Type_Id).FirstOrDefault();
                    ViewBag.ass = db.CMS_AssesmentTypes.Where(x => x.Active_Status == true && x.Ass_Type_Id == assid).Select(x => x.AssesmentType).FirstOrDefault();
                    ViewBag.Updated_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");

                    ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Internal_Type = new SelectList(objfaculty.getInternalType(), "Int_TYpe_Id", "Type");
                    //ViewBag.Sem = new SelectList(objfaculty.Search_Sem(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Sem = new SelectList(objfaculty.Search_Semst(), "Acc_Yr_Sem_Id", "Semester");
                    return View();
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult getAssesmenttype(int Int_TYpe_Id)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {

                    List<Internals> internalass = objfaculty.getAssesmenttype(Int_TYpe_Id).ToList();
                    return Json(internalass, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Internal_Mark()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (Session["Mobile"].ToString() == "9495727688")
                    {
                        return RedirectToAction("Internal_Mark_Home");
                    }
                    else
                    {
                        ViewBag.Updated_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");

                        ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                        ViewBag.Internal_Type = new SelectList(objfaculty.getInternalType(), "Int_TYpe_Id", "Type");
                        //ViewBag.Sem = new SelectList(objfaculty.Search_Sem(), "Acc_Yr_Sem_Id", "Semester");
                        ViewBag.Sem = new SelectList(objfaculty.Search_Semst(), "Acc_Yr_Sem_Id", "Semester");
                        int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                        List<Internals> intrnl = objfaculty.getCoursesFacultyWise(Faculty_Id).ToList();
                        return View(intrnl);
                    }
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult getAssesment(int Course_Sem_Id, int Acc_Yr_Pgm_Id, int Pgm_Type_Id, int Acc_Yr_Sem_Id)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {

                    List<Internals> internalass = objfaculty.getAssesment(Course_Sem_Id, Acc_Yr_Pgm_Id, Pgm_Type_Id, Acc_Yr_Sem_Id).ToList();
                    return Json(internalass, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Add_InternalType(CMS_InternalType cs)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {

                    cs.Created_By = new Guid(Session["Log_Id"].ToString());
                    int IntId = objfaculty.Add_InternalType(cs);
                    return Json(IntId, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Add_InternalAssesmentType(CMS_AssesmentType cs)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {

                    cs.Created_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Add_InternalAssesmentType(cs);
                    return Json(1, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Edit_InternalAssesment(Internals cs)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {

                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Edit_InternalAssesment(cs, Created_By);
                    return Json(1, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Add_InternalAssesment(Internals cs)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {

                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Add_InternalAssesment(cs, Created_By);
                    return Json(1, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Search_Semester(int pgmtypeid)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Programme> P = new List<Programme>();

                    P = objfaculty.Search_Semester(pgmtypeid).ToList();


                    return Json(P, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getProgramme(int Acc_Yr_Sem_Id, int Faculty_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    CMS_Login cl = db.CMS_Logins.Where(x => x.Active_Status == true && x.Log_Id == Created_By).FirstOrDefault();
                    CMS_Faculty cf = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == cl.Mobile && x.Faculty_Id == Faculty_Id).FirstOrDefault();
                    List<Programme> P = new List<Programme>();
                    if (cf != null)
                    {
                        P = objfaculty.Search_Semester_Pgm(Acc_Yr_Sem_Id, cf.Faculty_Id).ToList();
                    }

                    return Json(P, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getProgrammeCourse(int Acc_Yr_Sem_Id, int Acc_Yr_Pgm_Id, int Faculty_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    CMS_Login cl = db.CMS_Logins.Where(x => x.Active_Status == true && x.Log_Id == Created_By).FirstOrDefault();
                    CMS_Faculty cf = db.CMS_Facultys.Where(x => x.Active_Status == true && (x.Mobile == cl.Mobile || x.Email == cl.Email) && x.Faculty_Id == Faculty_Id).FirstOrDefault();
                    List<Semesters> cs = new List<Semesters>();
                    if (cf != null)
                    {
                        cs = objfaculty.getProgrammeCourse(Acc_Yr_Sem_Id, Faculty_Id, Acc_Yr_Pgm_Id).ToList();
                    }

                    return Json(cs, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Search_uprn_studentss(int Sem_Id, int Pgm_Id, DateTime date, int Lang_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Programme> students = new List<Programme>();

                    students = objfaculty.Search_uprn_studentss(Sem_Id, Pgm_Id, date, Lang_Id).ToList();


                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Search_uprn_student_Second(int Pgm_Id, int Lang_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Programme> students = new List<Programme>();

                    students = objfaculty.Search_uprn_student_Second(Pgm_Id, Lang_Id).ToList();


                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        public ActionResult getCurrentDay(string currDate)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    DateTime edate = Convert.ToDateTime(currDate);

                    int sch = objfaculty.getdate(edate);
                    return Json(sch, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Search_Single_Attendance(int Sem_Id, int Pgm_Id, DateTime date)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Programme> students = new List<Programme>();

                    students = objfaculty.Search_Single_Attendance(Sem_Id, Pgm_Id, date).ToList();


                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Delete_Attendance(string Hour, string date, int Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    objfaculty.Delete_Attendance(Hour, date, Pgm_Id);
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult Search_uprn_student(int Pgm_Id, int Int_Ass_Id, int Course_Id, int Pgm_Type_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Programme> students = new List<Programme>();

                    students = objfaculty.Search_uprn_student(Pgm_Id, Int_Ass_Id, Course_Id, Pgm_Type_Id).ToList();


                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Search_student_internal(int Pgm_Id, int Int_Ass_Id, int Course_Id, int Faculty_Id, int Pgm_Type_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Programme> intmark = new List<Programme>();

                    intmark = objfaculty.Search_student_internal(Pgm_Id, Int_Ass_Id, Course_Id, Faculty_Id, Pgm_Type_Id).ToList();


                    return Json(intmark, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Search_student_internal_count(int Pgm_Id, int Int_Ass_Id, int Course_Id, int Faculty_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    int intmark = dbExam.CMS_Internal_Marks.Where(x => x.Active_Status == true && x.Acc_Yr_Sem_Pgm_Id == Pgm_Id && x.Int_Ass_Id == Int_Ass_Id && x.Course_Sem_Id == Course_Id && x.Faculty_Id == Faculty_Id).GroupBy(x => x.UPRN).Count();


                    return Json(intmark, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        public ActionResult Update_Internal_Marks(Semesters ac)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    CMS_InternalAssesment intassmnt = new CMS_InternalAssesment();
                    int maxmark = 0, count = 0;
                    if (ac.Pgm_Type_Id == 1 || ac.Pgm_Type_Id == 8)
                    {
                        intassmnt = dbExam.CMS_InternalAssesments.Where(x => x.Active_Status == true && x.Int_Ass_Id == ac.Int_Ass_Id && x.Pgm_Type_Id == 1).FirstOrDefault();
                        maxmark = dbExam.CMS_InternalAssesments.Where(x => x.Int_Ass_Id == ac.Int_Ass_Id && x.Active_Status == true && x.Pgm_Type_Id == 1).Select(x => x.Max_Mark).FirstOrDefault();

                    }
                    else
                    {
                        intassmnt = dbExam.CMS_InternalAssesments.Where(x => x.Active_Status == true && x.Int_Ass_Id == ac.Int_Ass_Id && x.Pgm_Type_Id == ac.Pgm_Type_Id).FirstOrDefault();
                        maxmark = dbExam.CMS_InternalAssesments.Where(x => x.Int_Ass_Id == ac.Int_Ass_Id && x.Active_Status == true && x.Pgm_Type_Id == ac.Pgm_Type_Id).Select(x => x.Max_Mark).FirstOrDefault();

                    }
                    int asstypeid = dbExam.CMS_InternalAssesments.Where(x => x.Int_Ass_Id == ac.Int_Ass_Id && x.Active_Status == true).Select(x => x.Ass_Type_Id).FirstOrDefault();
                    string Int_Ass = dbExam.CMS_AssesmentTypes.Where(x => x.Ass_Type_Id == asstypeid && x.Active_Status == true).Select(x => x.AssesmentType).FirstOrDefault();


                    foreach (var item in ac.IntMark)
                    {
                        string[] s = item.Split(',');

                        foreach (var item1 in s)
                        {
                            string[] s1 = item1.Split('#');

                            double mark = Convert.ToDouble(s1[0]);
                            int uprn = Convert.ToInt32(s1[1]);


                            //if (ac.Int_Ass_Id == 5 || ac.Int_Ass_Id == 2)
                            //{
                            //    int mrk = maxmark / 2;
                            //    if (mrk < mark)
                            //    {
                            //        return Json(3, JsonRequestBehavior.AllowGet);
                            //    }
                            //}
                            //else
                            //{
                            if (ac.Pgm_Type_Id == 1 || ac.Pgm_Type_Id == 8)
                            {
                                if (ac.Sem_Id <= 13)
                                {
                                    if (intassmnt.Ass_Type_Id == 1 && ((intassmnt.Int_Type_Id == 1) || (intassmnt.Int_Type_Id == 2) || (intassmnt.Int_Type_Id == 8)))
                                    {
                                        double working = Convert.ToDouble(s1[2]);
                                        if (working < mark)
                                        {
                                            return Json(5, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            //ac.Created_By = new Guid(Session["Log_Id"].ToString());
                                            //objfaculty.Update_Internal_Marks(ac);
                                            //  return Json(1, JsonRequestBehavior.AllowGet);
                                            count = 1;
                                        }

                                    }
                                    else if (intassmnt.Ass_Type_Id == 1)
                                    {
                                        double working = Convert.ToDouble(s1[2]);
                                        if (working < mark)
                                        {
                                            return Json(4, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            //ac.Created_By = new Guid(Session["Log_Id"].ToString());
                                            //objfaculty.Update_Internal_Marks(ac);
                                            //  return Json(1, JsonRequestBehavior.AllowGet);
                                            count = 1;
                                        }

                                    }
                                    else
                                    {
                                        if (maxmark < mark)
                                        {
                                            return Json(3, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            //ac.Created_By = new Guid(Session["Log_Id"].ToString());
                                            //objfaculty.Update_Internal_Marks(ac);
                                            //return Json(1, JsonRequestBehavior.AllowGet);
                                            count = 1;
                                        }
                                    }
                                }
                                else
                                {
                                    if (intassmnt.Ass_Type_Id == 1 && ((intassmnt.Int_Type_Id == 1) || (intassmnt.Int_Type_Id == 2) || (intassmnt.Int_Type_Id == 8)))
                                    {


                                        double working = Convert.ToDouble(s1[2]);
                                        if (working < mark)
                                        {
                                            return Json(5, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            //ac.Created_By = new Guid(Session["Log_Id"].ToString());
                                            //objfaculty.Update_Internal_Marks(ac);
                                            //return Json(1, JsonRequestBehavior.AllowGet);
                                            count = 1;
                                        }

                                    }
                                    else if (intassmnt.Ass_Type_Id == 1)
                                    {
                                        double working = Convert.ToDouble(s1[2]);
                                        if (working < mark)
                                        {
                                            return Json(4, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            //ac.Created_By = new Guid(Session["Log_Id"].ToString());
                                            //objfaculty.Update_Internal_Marks(ac);
                                            //return Json(1, JsonRequestBehavior.AllowGet);
                                            count = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (maxmark < mark)
                                        {
                                            return Json(3, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            //ac.Created_By = new Guid(Session["Log_Id"].ToString());
                                            //objfaculty.Update_Internal_Marks(ac);
                                            //return Json(1, JsonRequestBehavior.AllowGet);
                                            count = 1;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (intassmnt.Ass_Type_Id == 1 && ((intassmnt.Int_Type_Id == 1) || (intassmnt.Int_Type_Id == 2) || (intassmnt.Int_Type_Id == 8)))

                                {
                                    double working = Convert.ToDouble(s1[2]);
                                    if (working < mark)
                                    {
                                        return Json(5, JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        //ac.Created_By = new Guid(Session["Log_Id"].ToString());
                                        //objfaculty.Update_Internal_Marks(ac);
                                        //return Json(1, JsonRequestBehavior.AllowGet);
                                        count = 1;
                                    }

                                }
                                else if (intassmnt.Ass_Type_Id == 1)
                                {
                                    double working = Convert.ToDouble(s1[2]);
                                    if (working < mark)
                                    {
                                        return Json(4, JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        //ac.Created_By = new Guid(Session["Log_Id"].ToString());
                                        //objfaculty.Update_Internal_Marks(ac);
                                        //return Json(1, JsonRequestBehavior.AllowGet);
                                        count = 1;
                                    }
                                }
                                else
                                {
                                    if (maxmark < mark)
                                    {
                                        return Json(3, JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        //ac.Created_By = new Guid(Session["Log_Id"].ToString());
                                        //objfaculty.Update_Internal_Marks(ac);
                                        //return Json(1, JsonRequestBehavior.AllowGet);
                                        count = 1;
                                    }
                                }
                            }
                            //}
                        }



                    }
                    if (count == 1)
                    {
                        ac.Created_By = new Guid(Session["Log_Id"].ToString());
                        objfaculty.Update_Internal_Marks(ac);
                        return Json(1, JsonRequestBehavior.AllowGet);
                    }
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }




            // return Json(0, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Add_Internal_Marks(Semesters ac)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    CMS_InternalAssesment asstype = dbExam.CMS_InternalAssesments.Where(x => x.Int_Ass_Id == ac.Int_Ass_Id && x.Active_Status == true).FirstOrDefault();
                    //int internaltypeid = dbExam.CMS_InternalTypes.Where(x => x.Int_TYpe_Id == asstype.Int_Type_Id && x.Active_Status == true).Select(x => x.).FirstOrDefault();
                    string assesment = dbExam.CMS_AssesmentTypes.Where(x => x.Ass_Type_Id == asstype.Ass_Type_Id && x.Active_Status == true).Select(x => x.AssesmentType).FirstOrDefault();
                    //int maxmark = dbExam.CMS_InternalAssesments.Where(x => x.Int_Ass_Id == ac.Int_Ass_Id && x.Active_Status == true).Select(x => x.Max_Mark).FirstOrDefault();
                    //foreach (var item in ac.IntMark)
                    //{
                    //    string[] s = item.Split(',');

                    //    foreach (var item1 in s)
                    //    {
                    //        string[] s1 = item1.Split('#');

                    //        double mark = Convert.ToDouble(s1[0]);
                    //        int uprn = Convert.ToInt32(s1[1]);

                    //        if (ac.Int_Ass_Id == 5 || ac.Int_Ass_Id==2)
                    //        {
                    //            int mrk = maxmark / 2;
                    //            if (mrk < mark)
                    //            {
                    //                return Json(3, JsonRequestBehavior.AllowGet);
                    //            }
                    //}
                    //else
                    //{

                    //    if (maxmark < mark)
                    //    {
                    //        return Json(3, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    //    }



                    //}
                    int count = 0;
                    if (assesment == "Attendance" && (asstype.Int_Type_Id == 1 || asstype.Int_Type_Id == 2 || asstype.Int_Type_Id == 8))
                    {
                        //DateTime Att_EndDate = db.CMS_AcademicYr_Sem_Programmes.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Active_Status == true).Select(x => x.Att_End_Date).FirstOrDefault();
                        DateTime dt = DateTime.Now;

                        DateTime Att_EndDate = db.CMS_AcademicYr_Sem_Programmes.Where(x => x.Acc_Yr_Sem_Pgm_Id == ac.Pgm_Id && x.Active_Status == true).Select(x => x.End_Date).FirstOrDefault();
                        if (Att_EndDate > dt)
                        {
                            return Json(2, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            ac.Created_By = new Guid(Session["Log_Id"].ToString());
                            objfaculty.Add_Internal_Marks(ac);
                            return Json(1, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        ac.Created_By = new Guid(Session["Log_Id"].ToString());
                        objfaculty.Add_Internal_Marks(ac);
                        return Json(1, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }




            // return Json(0, JsonRequestBehavior.AllowGet);
        }
        public ActionResult A_Form()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    //Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    //CMS_Login cl = db.CMS_Logins.Where(x => x.Active_Status == true && x.Log_Id == Created_By).FirstOrDefault();
                    //CMS_Faculty cf = db.CMS_Facultys.Where(x => x.Active_Status == true && (x.Mobile == cl.Mobile || x.Email == cl.Email)).FirstOrDefault();
                    //ViewBag.Hod = db.CMS_HODs.Where(x => x.Active_Status == true && x.Faculty_Id == cf.Faculty_Id).Select(x => x.HOD_Id).FirstOrDefault();
                    ViewBag.Updated_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");


                    ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Internal_Type = new SelectList(objfaculty.getInternalType(), "Int_TYpe_Id", "Type");
                    // ViewBag.Sem = new SelectList(objfaculty.Search_Sem(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Sem = new SelectList(objfaculty.Search_Semst(), "Acc_Yr_Sem_Id", "Semester");
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Form_A(int Course_Sem_Id, int Acc_Yr_Pgm_Id, int Faculty_Id, int Acc_Yr_Id, int Pgm_Type_Id, int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    var s = db.CMS_AcademicYears.Where(x => x.Acc_yr_Id == Acc_Yr_Id && x.Active_Status == true).FirstOrDefault();
                    var Course_Id = db.CMS_Course_Semesters.Where(x => x.Active_Status == true && x.Course_Sem_Id == Course_Sem_Id).Select(x => x.Course_Id).FirstOrDefault();
                    int PgmId = db.CMS_AcademicYr_Sem_Programmes.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Active_Status == true).Select(x => x.Pgm_Id).FirstOrDefault();
                    int GroupId = db.CMS_Programmes.Where(x => x.Pgm_Id == PgmId && x.Active_Status == true).Select(x => x.Group_Id).FirstOrDefault();

                    Session["Accyr"] = db.CMS_AcademicYears.Where(x => x.Acc_yr_Id == Acc_Yr_Id && x.Active_Status == true).Select(x => x.Year).FirstOrDefault();
                    int facid = dbExam.CMS_Internal_Marks.Where(x => x.Active_Status == true && x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Course_Sem_Id == Course_Sem_Id && x.Faculty_Id == Faculty_Id).Select(x => x.Faculty_Id).FirstOrDefault();
                    if (facid != 0)
                    {
                        Session["faculty"] = db.CMS_Facultys.Where(x => x.Faculty_Id == facid).Select(x => x.Name).FirstOrDefault();
                        Session["form"] = objfaculty.Assesment_AForm(Course_Sem_Id, Acc_Yr_Pgm_Id);
                        Session["GroupId"] = GroupId;
                        Session["Course_Nature_Type"] = db.CMS_Course_Semesters.Where(x => x.Active_Status == true && x.Course_Sem_Id == Course_Sem_Id).Select(x => x.Course_Nature_Type).FirstOrDefault();
                        Session["Pgm_Type_Id"] = Pgm_Type_Id;
                        Internals intrnl = objfaculty.getFormA(Course_Sem_Id, Acc_Yr_Pgm_Id, Pgm_Type_Id, Acc_Yr_Sem_Id);
                        int count = dbExam.CMS_Internal_Temps.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Course_Sem_Id == Course_Sem_Id && x.Faculty_Id == Faculty_Id && x.Active_Status == true).Count();
                        if (count == 0)
                        {
                            Guid Created_By = new Guid(Session["Log_Id"].ToString());
                            foreach (var a in intrnl.students)
                            {
                                decimal total = a.grand_total;
                                int Form_Id = dbExam.CMS_Internal_Temps.Count();
                                CMS_Internal_Temp ab = new CMS_Internal_Temp { Form_Id = Form_Id + 1, Acc_Yr_Sem_Pgm_Id = Acc_Yr_Pgm_Id, UPRN = a.UPRN, Grand_Total = total, Course_Sem_Id = Course_Sem_Id, Faculty_Id = Faculty_Id, Created_By = Created_By, Created_Date = DateTime.Now, Active_Status = true };
                                dbExam.CMS_Internal_Temps.Add(ab);
                                dbExam.SaveChanges();
                            }
                        }

                        return View(intrnl);
                    }
                    else
                    {
                        return Redirect("~/Login/Error_Page");
                    }
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Form_A_New(int Course_Sem_Id, int Acc_Yr_Pgm_Id, int Acc_Yr_Id, int Faculty_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    List<Internals> intrnl = objfaculty.printFormA(Course_Sem_Id, Acc_Yr_Pgm_Id, Faculty_Id, Created_By).ToList();
                    return View(intrnl);

                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult getProgramme_Types(int Acc_Yr_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    int dep_id = Convert.ToInt32(Session["DepId"]);//Session["DepId"];
                    List<Schedule> sh = objfaculty.Search_PgmType(dep_id, Acc_Yr_Id).ToList();
                    return Json(sh, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult Form_B(int Acc_Yr_Sem_Id, int Acc_Yr_Pgm_Id, int Acc_Yr_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    // var s = objfaculty.getCurrentAcademicYear();
                    int accyrid = db.CMS_AccademicYearSemesters.Where(x => x.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id && x.Active_Status == true).Select(x => x.Acc_yr_Id).FirstOrDefault();
                    var s = db.CMS_AcademicYears.Where(x => x.Acc_yr_Id == accyrid && x.Active_Status == true).FirstOrDefault();
                    int semid = db.CMS_AccademicYearSemesters.Where(x => x.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id && x.Active_Status == true && x.Acc_yr_Id == s.Acc_yr_Id).Select(x => x.Sem_Id).FirstOrDefault();
                    Session["SemId"] = semid.ToString();
                    Session["Pgmid"] = db.CMS_AcademicYr_Sem_Programmes.Where(x => x.Active_Status == true && x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id).Select(x => x.Pgm_Id).FirstOrDefault();
                    Session["semester"] = db.CMS_Semesters.Where(x => x.Active_Status == true && x.Sem_Id == semid).Select(x => x.Semester).FirstOrDefault();
                    int pgmid = db.CMS_AcademicYr_Sem_Programmes.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Active_Status == true).Select(x => x.Pgm_Id).FirstOrDefault();
                    Session["pgm"] = db.CMS_Programmes.Where(x => x.Active_Status == true && x.Pgm_Id == pgmid).Select(x => x.Programme).FirstOrDefault();
                    Session["Accyr"] = db.CMS_AcademicYears.Where(x => x.Acc_yr_Id == Acc_Yr_Id && x.Active_Status == true).Select(x => x.Year).FirstOrDefault();
                    ViewBag.Course = objfaculty.getFormB_Course(Acc_Yr_Sem_Id, Acc_Yr_Pgm_Id).ToList();
                    int PgmTypeId = db.CMS_Programmes.Where(x => x.Pgm_Id == pgmid && x.Active_Status == true).Select(x => x.Pgm_Type_Id).FirstOrDefault();
                    ViewBag.Pgmtype = db.CMS_ProgrammeTypes.Where(x => x.Pgm_Type_Id == PgmTypeId && x.Active_Status == true).Select(x => x.Programme_Type).FirstOrDefault();

                    List<Internals> intrnl = objfaculty.getFormB(Acc_Yr_Sem_Id, Acc_Yr_Pgm_Id).ToList();

                    return View(intrnl);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult B_Form()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.Updated_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");
                    ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Internal_Type = new SelectList(objfaculty.getInternalType(), "Int_TYpe_Id", "Type");
                    ViewBag.Sem = new SelectList(objfaculty.Search_Semst(), "Acc_Yr_Sem_Id", "Semester");
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Search_SemesterDetails(int Acc_Yr_Id, int Pgm_Type_Id, int? Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    Guid created_by = new Guid(Session["Log_Id"].ToString());
                    int dep_id = Convert.ToInt32(Session["DepId"]);//Session["DepId"];

                    List<BForm> sh = objfaculty.Search_Details(dep_id, Acc_Yr_Id, Pgm_Type_Id, Pgm_Id).ToList();
                    return Json(sh, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult get_AformCount(int Course_Sem_Id, int Faculty_Id, int Acc_Yr_Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    int count = dbExam.CMS_Internal_Temps.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Course_Sem_Id == Course_Sem_Id && x.Faculty_Id == Faculty_Id && x.Active_Status == true).Count();
                    return Json(count, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        public ActionResult getFormAAttendance(int Course_Sem_Id, int Acc_Yr_Pgm_Id, int Pgm_Type_Id, int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    // Internals intrnl = objfaculty.getFormA(Course_Sem_Id, Acc_Yr_Pgm_Id, Pgm_Type_Id, Acc_Yr_Sem_Id);
                    Internals intrnl = objfaculty.getFormAAttendance(Course_Sem_Id, Acc_Yr_Pgm_Id, Pgm_Type_Id, Acc_Yr_Sem_Id);
                    return Json(intrnl, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Delete_AForm(int Course_Sem_Id, int Acc_Yr_Pgm_Id, int Faculty_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    objfaculty.Delete_AForm(Course_Sem_Id, Acc_Yr_Pgm_Id, Faculty_Id);
                    return RedirectToAction("A_Form", "Faculty");
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult getInternaltype(int Int_TYpe_Id)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {

                    List<Internals> internalass = objfaculty.getInternaltype(Int_TYpe_Id).ToList();
                    return Json(internalass, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult getInternalMarkCount(int Course_Sem_Id, int Acc_Yr_Pgm_Id, int Faculty_Id, int Pgm_Type_Id, int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int count = 0;
                    int total = 0;
                    List<Internals> internalass = objfaculty.getAssesment(Course_Sem_Id, Acc_Yr_Pgm_Id, Pgm_Type_Id, Acc_Yr_Sem_Id).ToList();

                    var s = internalass.Select(x => x.Int_Ass_Id).Distinct().ToList();
                    List<Programme> students = objfaculty.Search_student(Acc_Yr_Pgm_Id, Course_Sem_Id, Pgm_Type_Id).ToList();
                    total = dbExam.CMS_Internal_Marks.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Course_Sem_Id == Course_Sem_Id && x.Faculty_Id == Faculty_Id && x.Active_Status == true).Count();
                    if (total == 0)
                    {
                        return Json(count, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        foreach (var st in s)
                        {
                            //if ((st == 5) || (st == 2))
                            //{
                            //    total = dbExam.CMS_Internal_Marks.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Int_Ass_Id == st && x.Course_Sem_Id == Course_Sem_Id && x.Faculty_Id == Faculty_Id && x.Active_Status == true).Count();
                            //    if (total == 0)
                            //    {
                            //        count = st;
                            //        break;
                            //    }
                            //    else
                            //    {
                            //        if (total == students.Count())
                            //        {
                            //            count = st;
                            //            break;
                            //        }
                            //        else
                            //        {
                            //            count = 1;
                            //        }
                            //    }

                            //}
                            //else
                            //{

                            total = dbExam.CMS_Internal_Marks.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Int_Ass_Id == st && x.Course_Sem_Id == Course_Sem_Id && x.Faculty_Id == Faculty_Id && x.Active_Status == true).Count();
                            if (total == 0)
                            {
                                count = st;
                                break;
                            }
                            else
                            {
                                count = 100;
                            }

                            //}
                        }
                        return Json(count, JsonRequestBehavior.AllowGet);
                    }

                    // return Json(count, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getCount(int Pgm_Id, int Int_Ass_Id, int Course_Id, int Faculty_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int count = 0;
                    if ((Int_Ass_Id == 22) || (Int_Ass_Id == 23) || (Int_Ass_Id == 31) || (Int_Ass_Id == 27) || (Int_Ass_Id == 30))
                    {
                        count = 0;
                    }
                    else
                    {
                        count = dbExam.CMS_Internal_Marks.Where(x => x.Acc_Yr_Sem_Pgm_Id == Pgm_Id && x.Int_Ass_Id == Int_Ass_Id && x.Course_Sem_Id == Course_Id && x.Faculty_Id == Faculty_Id && x.Active_Status == true).Count();

                    }

                    return Json(count, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getAssesment_Type(int Int_Ass_Id, int Pgm_Type_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int count = 0;
                    if (Pgm_Type_Id == 1 || Pgm_Type_Id == 8)
                    {
                        count = dbExam.CMS_InternalAssesments.Where(x => x.Int_Ass_Id == Int_Ass_Id && x.Active_Status == true && x.Pgm_Type_Id == 1).Select(x => x.Int_Type_Id).FirstOrDefault();
                    }
                    else
                    {
                        count = dbExam.CMS_InternalAssesments.Where(x => x.Int_Ass_Id == Int_Ass_Id && x.Active_Status == true && x.Pgm_Type_Id == Pgm_Type_Id).Select(x => x.Int_Type_Id).FirstOrDefault();
                    }

                    return Json(count, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getInternalCount(int Course_Sem_Id, int Acc_Yr_Pgm_Id, int Faculty_Id, int Int_Ass_Id, int Pgm_Type_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int count = 0;
                    int total = 0;
                    //List<Internals> internalass = objfaculty.getAssesment(Course_Sem_Id, Acc_Yr_Pgm_Id).ToList();

                    //var s = internalass.Select(x => x.Int_Ass_Id).Distinct().ToList();
                    List<Programme> students = objfaculty.Search_student(Acc_Yr_Pgm_Id, Course_Sem_Id, Pgm_Type_Id).ToList();
                    total = dbExam.CMS_Internal_Marks.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Course_Sem_Id == Course_Sem_Id && x.Faculty_Id == Faculty_Id && x.Active_Status == true && x.Int_Ass_Id == Int_Ass_Id).Count();


                    if ((Int_Ass_Id == 22) || (Int_Ass_Id == 23) || (Int_Ass_Id == 31) || (Int_Ass_Id == 27) || (Int_Ass_Id == 30))
                    {
                        total = dbExam.CMS_Internal_Marks.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Int_Ass_Id == Int_Ass_Id && x.Course_Sem_Id == Course_Sem_Id && x.Faculty_Id == Faculty_Id && x.Active_Status == true).Count();


                        count = total / students.Count();
                    }



                    return Json(count, JsonRequestBehavior.AllowGet);




                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getcourseCount(int Acc_Yr_Sem_Id, int Acc_Yr_Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int count = 0;
                    int SemId = db.CMS_AccademicYearSemesters.Where(x => x.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id && x.Active_Status == true).Select(x => x.Sem_Id).FirstOrDefault();
                    if (SemId != 5)
                    {
                        var course = (from a in db.CMS_Course_Semesters
                                      join b in db.CMS_Courses on a.Course_Id equals b.Course_Id
                                      join e in db.CMS_AcademicYr_Sem_Programmes on a.Acc_Yr_Sem_Pgm_Id equals e.Acc_Yr_Sem_Pgm_Id
                                      join f in db.CMS_AccademicYearSemesters on e.Acc_Yr_sem_Id equals f.Acc_Yr_Sem_Id
                                      join g in db.CMS_Semesters on f.Sem_Id equals g.Sem_Id
                                      join d in db.CMS_Programmes on e.Pgm_Id equals d.Pgm_Id

                                      where e.Acc_Yr_sem_Id == Acc_Yr_Sem_Id && a.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id
                                      && b.Active_Status == true && a.Active_Status == true && a.Exam_Status == "Yes"
                                      select new { a.Course_Sem_Id }).Distinct().ToList();

                        int total = dbExam.CMS_Internal_Temps.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Active_Status == true).Select(x => x.Course_Sem_Id).Distinct().Count();


                        if (course.Count() == total)
                        {
                            count = 1;
                        }
                        else
                        {
                            count = 0;
                        }

                    }
                    else
                    {
                        var course = (from a in db.CMS_Course_Semesters
                                      join b in db.CMS_Courses on a.Course_Id equals b.Course_Id
                                      join e in db.CMS_AcademicYr_Sem_Programmes on a.Acc_Yr_Sem_Pgm_Id equals e.Acc_Yr_Sem_Pgm_Id
                                      join f in db.CMS_AccademicYearSemesters on e.Acc_Yr_sem_Id equals f.Acc_Yr_Sem_Id
                                      join g in db.CMS_Semesters on f.Sem_Id equals g.Sem_Id
                                      join d in db.CMS_Programmes on e.Pgm_Id equals d.Pgm_Id

                                      where e.Acc_Yr_sem_Id == Acc_Yr_Sem_Id && a.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id
                                      && b.Active_Status == true && a.Active_Status == true && a.Exam_Status == "Yes" && b.Course_Type != "Open Course"
                                      select a.Course_Sem_Id).Distinct().ToList();

                        int total = (from a in db.CMS_Internal_Temps
                                     where a.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && a.Active_Status == true && course.Contains(a.Course_Sem_Id)
                                     select a.Course_Sem_Id).Distinct().Count();

                        //   dbExam.CMS_Internal_Temps.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Active_Status == true && course.Contains(x.Course_Sem_Id)).Select(x => x.Course_Sem_Id).Distinct().Count();


                        if (course.Count() == total)
                        {
                            count = 1;
                            var s = dbExam.CMS_Open_Courses.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Active_Status == true).Select(x => x.Course_Sem_Id).Distinct().ToList();
                            int total_open = (from a in db.CMS_Internal_Temps
                                              where a.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && a.Active_Status == true && s.Contains(a.Course_Sem_Id)
                                              select a.Course_Sem_Id).Distinct().Count();
                            if (s.Count() == total_open)
                            {
                                count = 1;
                            }
                            else
                            {
                                count = 2;
                            }

                        }
                        else
                        {
                            count = 0;
                            var s = dbExam.CMS_Open_Courses.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Active_Status == true).Select(x => x.Course_Sem_Id).Distinct().ToList();
                            int total_open = (from a in db.CMS_Internal_Temps
                                              where a.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && a.Active_Status == true && s.Contains(a.Course_Sem_Id)
                                              select a.Course_Sem_Id).Distinct().Count();
                            if (s.Count() == total_open)
                            {
                                count = 4;
                            }
                            else
                            {
                                count = 3;
                            }
                        }
                    }

                    return Json(count, JsonRequestBehavior.AllowGet);




                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getFaculties_By_Course(int Course_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Schedule> sch = new List<Schedule>();

                    sch = objfaculty.getFaculties_By_Course(Course_Sem_Id).ToList();
                    // ViewBag.Hour = objLearn.getHours().ToList();


                    return Json(sch, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getCoursess_Department(int Course_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int courseid = db.CMS_Course_Semesters.Where(x => x.Course_Sem_Id == Course_Sem_Id && x.Active_Status == true).Select(x => x.Course_Id).FirstOrDefault();
                    int DepId = db.CMS_Courses.Where(x => x.Course_Id == courseid).Select(x => x.Dep_Id).FirstOrDefault();

                    return Json(DepId, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getFaculties_By_Coursess(int Course_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Schedule> sch = new List<Schedule>();

                    sch = objfaculty.getFaculties_By_Coursess(Course_Sem_Id).ToList();
                    // ViewBag.Hour = objLearn.getHours().ToList();


                    return Json(sch, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        public ActionResult Check_Timetable(int Day_Id, int Pgm_Id, int Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int q = objfaculty.Check_Timetable(Day_Id, Pgm_Id, Sem_Id);
                    if (q == 0)
                    {


                        return Json(0, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(2, JsonRequestBehavior.AllowGet);
                    }


                    // return Json(0, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Add_Timetable(List<Timetable> item)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    Guid logid = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Add_Timetable(item, logid);
                    return Json(1, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult getTimetable_Programme(int Acc_Yr_Sem_Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Schedule> students = new List<Schedule>();

                    students = objfaculty.getTimetable_Programme(Acc_Yr_Sem_Pgm_Id).ToList();
                    // ViewBag.Hour = objLearn.getHours().ToList();


                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getCourses_By_Pgm_Sem(int Acc_Yr_Sem_Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Schedule> sch = new List<Schedule>();

                    sch = objfaculty.getCourses_By_Pgm_Sem(Acc_Yr_Sem_Pgm_Id).ToList();
                    // ViewBag.Hour = objLearn.getHours().ToList();


                    return Json(sch, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getProgramme_HOD(int Acc_Yr_Sem_Id, int Faculty_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    CMS_Login cl = db.CMS_Logins.Where(x => x.Active_Status == true && x.Log_Id == Created_By).FirstOrDefault();
                    CMS_Faculty cf = db.CMS_Facultys.Where(x => x.Active_Status == true && (x.Mobile == cl.Mobile || x.Email == cl.Email) && x.Faculty_Id == Faculty_Id).FirstOrDefault();
                    List<Programme> P = new List<Programme>();
                    if (cf != null)
                    {
                        P = objfaculty.getProgramme_HOD(Acc_Yr_Sem_Id, cf.Faculty_Id).ToList();
                    }

                    return Json(P, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getProgramme_Wisehod(int Acc_Yr_Sem_Id, int Faculty_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    CMS_Login cl = db.CMS_Logins.Where(x => x.Active_Status == true && x.Log_Id == Created_By).FirstOrDefault();
                    CMS_Faculty cf = db.CMS_Facultys.Where(x => x.Active_Status == true && (x.Mobile == cl.Mobile || x.Email == cl.Email) && x.Faculty_Id == Faculty_Id).FirstOrDefault();
                    List<Programme> P = new List<Programme>();
                    if (cf != null)
                    {
                        P = objfaculty.getProgramme_Wisehod(Acc_Yr_Sem_Id, cf.Faculty_Id).ToList();
                    }

                    return Json(P, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        public ActionResult getAllSemCourse_By_Pgm(int Pgm_Id, int Acc_Yr_Sem_Id, int Faculty_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Schedule> sh = new List<Schedule>();
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    CMS_Login cl = db.CMS_Logins.Where(x => x.Active_Status == true && x.Log_Id == Created_By).FirstOrDefault();
                    CMS_Faculty cf = db.CMS_Facultys.Where(x => x.Active_Status == true && (x.Mobile == cl.Mobile || x.Email == cl.Email) && x.Faculty_Id == Faculty_Id).FirstOrDefault();
                    if (cf != null)
                    {
                        sh = objfaculty.getAllSemCourse_By_Pgm(Pgm_Id, Acc_Yr_Sem_Id, cf.Faculty_Id).ToList();
                    }
                    return Json(sh, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult getAllSem(int Pgm_Id, int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    List<Schedule> sh = objfaculty.getAllSem(Pgm_Id, Acc_Yr_Sem_Id).ToList();
                    return Json(sh, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult getAllSemCourse(int Course_Sem_Id, int Course_Id, int Faculty_Id, int Faculty_Id1, int Faculty_Id2)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    List<Schedule> sch = new List<Schedule>();
                    sch = objfaculty.getAllSemCourse(Course_Sem_Id, Course_Id, Faculty_Id, Faculty_Id1, Faculty_Id2).ToList();
                    return Json(sch, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Search_Faculties_By_Courses(int Course_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    List<Schedule> sch = new List<Schedule>();
                    sch = objfaculty.Search_Faculties_By_Courses(Course_Id).ToList();
                    return Json(sch, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Delete_All_Course(int Course_Id, int Course_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    objfaculty.Delete_All_Course(Course_Id, Course_Sem_Id);

                    return RedirectToAction("Faculty_Details", "Faculty", new { Course_Id = Course_Id, Course_Sem_Id = Course_Sem_Id });
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Edit_Full_Course(Schedule cf)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    cf.Created_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Edit_Full_Course(cf);

                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult getAllSemCourse_By_Pgms()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    List<Schedule> sh = objfaculty.getAllSemCourse().ToList();
                    return Json(sh, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult getAllSems()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    List<Schedule> sh = objfaculty.getAllSemesters().ToList();
                    return Json(sh, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult getFaculties_By_CoursePgm(int Acc_Yr_Sem_Pgm_Id, int Day_Id, int Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Schedule> sch = new List<Schedule>();

                    sch = objfaculty.getFaculties_By_CoursePgm(Acc_Yr_Sem_Pgm_Id, Day_Id, Sem_Id).ToList();
                    // ViewBag.Hour = objLearn.getHours().ToList();


                    return Json(sch, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Class_Warden()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.Updated_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");
                    ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Sem = new SelectList(objfaculty.Search_Semst(), "Acc_Yr_Sem_Id", "Semester");
                    Guid created_by = new Guid(Session["Log_Id"].ToString());
                    int dep_id = Convert.ToInt32(Session["DepId"]);//Session["DepId"];
                    List<Schedule> sh = objfaculty.Search_ClassWarden(dep_id).ToList();
                    return View(sh);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Student_List()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }
        }
        public ActionResult Print_Student_Profile(int Admsn_No)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    LocalReport lr = new LocalReport();
                    string path = Path.Combine(Server.MapPath("~/Report"), "Student_Form.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Student_List");
                    }

                    List<CMS_Student> stud = new List<CMS_Student>();
                    stud = db.CMS_Students.Where(x => x.Admission_No == Admsn_No).ToList();
                    int yrr = stud.Select(x => x.Acc_Yr_Id).FirstOrDefault();
                    var yer = db.CMS_AcademicYears.Where(x => x.Acc_yr_Id == yrr).Select(x => x.Year).FirstOrDefault();
                    var tm = DateTime.Now.Month.ToString();
                    var td = DateTime.Now.Day.ToString();
                    var ccyr = DateTime.Now.Year;
                    var currntyr = DateTime.Now.Year.ToString();
                    var nxt = ccyr + 1;
                    var nxtyr = nxt.ToString();
                    var pgmid = db.CMS_Students.Where(x => x.Admission_No == Admsn_No).Select(x => x.Pgm_Id).FirstOrDefault();
                    var pgmm = db.CMS_Programmes.Where(x => x.Pgm_Id == pgmid).Select(x => x.Programme).FirstOrDefault();

                    var dis_Id = db.CMS_Students.Where(x => x.Admission_No == Admsn_No).Select(x => x.Per_Dis_Id).FirstOrDefault();
                    var district = db.CMS_Districts.Where(x => x.Dis_Id == dis_Id).Select(x => x.District).FirstOrDefault();

                    var streamid = db.CMS_Programmes.Where(x => x.Pgm_Id == pgmid).Select(x => x.Stream_Type_Id).FirstOrDefault();
                    var Streamtype = db.CMS_StreamTypes.Where(x => x.Stream_Type_Id == streamid).Select(x => x.Stream).FirstOrDefault();

                    var pgmTypeId = db.CMS_Programmes.Where(x => x.Pgm_Id == pgmid).Select(x => x.Pgm_Type_Id).FirstOrDefault();
                    string Applid = stud.Select(b => b.Applcn_Id).FirstOrDefault();
                    string QualReg = string.Empty;
                    if (pgmTypeId == 1 || pgmTypeId == 8)
                    {
                        QualReg = db.CMS_UGApplications.Where(x => x.Applcn_Id == Applid).Select(x => x.Qual_Reg_No).FirstOrDefault();
                    }
                    else
                    {
                        QualReg = db.CMS_PGApplications.Where(x => x.Applcn_Id == Applid).Select(x => x.Qual_Reg_No).FirstOrDefault();
                    }

                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "DataSet1";
                    reportDataSource.Value = stud;
                    lr.DataSources.Add(reportDataSource);
                    lr.EnableExternalImages = true;
                    string FilePath = @"file:\" + AppDomain.CurrentDomain.BaseDirectory + "Images\\Student_Photo\\";
                    List<ReportParameter> paraList = new List<ReportParameter>();

                    string photo = db.CMS_Students.Where(x => x.Admission_No == Admsn_No).Select(x => x.Photo).FirstOrDefault();
                    paraList.Add(new ReportParameter("ImgPath", FilePath + photo));
                    paraList.Add(new ReportParameter("photo", photo));
                    paraList.Add(new ReportParameter("year1", yer));
                    paraList.Add(new ReportParameter("year2", nxtyr));
                    paraList.Add(new ReportParameter("pgm", pgmm));
                    paraList.Add(new ReportParameter("district", district));
                    paraList.Add(new ReportParameter("Streamtype", Streamtype));
                    paraList.Add(new ReportParameter("td", td));
                    paraList.Add(new ReportParameter("tm", tm));
                    paraList.Add(new ReportParameter("QualReg", QualReg));

                    lr.SetParameters(paraList.ToArray());

                    string reportType = "PDF";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;
                    string deviceInfo =

                        "<DeviceInfo>" +

                        "<OutputFormat>" + reportType + "</OutputFormat>" +

                        "<PageWidth>8.5in</PageWidth>" +

                        "</DeviceInfo>";


                    Warning[] warning;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = lr.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warning);
                    //  return File(renderedBytes, "pdf");
                    return File(renderedBytes, mimeType);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult getClass()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int dep_id = Convert.ToInt32(Session["DepId"]);//Session["DepId"];
                    List<Schedule> sh = objfaculty.get_Class(dep_id).ToList();
                    return Json(sh, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Search_Semester_Students_All(int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Student> students = new List<Student>();
                    int dep_id = Convert.ToInt32(Session["DepId"]);//Session["DepId"];
                    students = objfaculty.getSemesterProgrammes_StudentLISt(Acc_Yr_Sem_Id, dep_id).ToList();
                    var serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    var resultData = students;
                    var result = new ContentResult
                    {
                        Content = serializer.Serialize(resultData),
                        ContentType = "application/json"
                    };
                    return result;
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        public ActionResult View_ClassWarden(int Faculty_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int dep_id = Convert.ToInt32(Session["DepId"]);//Session["DepId"];
                    List<Schedule> sh = objfaculty.Search_ClassWarden(dep_id).ToList();
                    return Json(sh, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Search_Faculties_By_Programme(int Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    List<Schedule> sch = new List<Schedule>();
                    sch = objfaculty.Search_Faculties_By_Programme(Pgm_Id).ToList();
                    return Json(sch, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Search_Faculties_By_Programmes(int Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    List<Schedule> sch = new List<Schedule>();
                    sch = objfaculty.Search_Faculties_By_Programmes(Pgm_Id).ToList();
                    return Json(sch, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult Search_Faculties_By_ProgrammesYr(int Acc_Yr_sem_Id, int Acc_Yr_Sem_Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    List<Schedule> sch = new List<Schedule>();
                    sch = objfaculty.Search_Faculties_By_ProgrammesYr(Acc_Yr_sem_Id, Acc_Yr_Sem_Pgm_Id).ToList();
                    return Json(sch, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Add_ClassWarden(CMS_ClassWarden ds)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {

                    ds.Created_By = new Guid(Session["Log_Id"].ToString());
                    int applcn = db.CMS_ClassWardens.Where(x => x.Acc_Yr_Sem_Pgm_Id == ds.Acc_Yr_Sem_Pgm_Id && x.Acc_Yr_sem_Id == ds.Acc_Yr_sem_Id && x.Active_Status == true).Count();
                    if (applcn == 0)
                    {
                        objfaculty.Add_ClassWarden(ds);
                        return Json(1, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(2, JsonRequestBehavior.AllowGet);
                    }


                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult Edit_Warden(CMS_ClassWarden ds)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {

                    ds.Created_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Edit_Warden(ds);
                    return Json(1, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Delete_Warden(int Warden_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    // cf.Deleted_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Delete_wardens(Warden_Id);
                    return RedirectToAction("Class_Warden", "Faculty");
                    // return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        //Feedback

        public ActionResult Feedback()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Fac_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    ViewBag.Ans = db.CMS_Feedback_Answers.Where(x => x.Active_Status == true && x.Feed_Category == "Faculty").ToList();

                    List<FeedbackReport> rpt = objfaculty.generateReport(Fac_Id).ToList();
                    return View(rpt);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult Generated_A_Forms()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.Updated_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");

                    ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Internal_Type = new SelectList(objfaculty.getInternalType(), "Int_TYpe_Id", "Type");
                    //ViewBag.Sem = new SelectList(objfaculty.Search_Sem(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Sem = new SelectList(objfaculty.Search_Semst(), "Acc_Yr_Sem_Id", "Semester");
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Print_Form_A(int Course_Sem_Id, int Acc_Yr_Pgm_Id, int Acc_Yr_Id, int Pgm_Type_Id, int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    var s = db.CMS_AcademicYears.Where(x => x.Active_Status == true && x.Acc_yr_Id == Acc_Yr_Id).FirstOrDefault();
                    var Course_Id = db.CMS_Course_Semesters.Where(x => x.Active_Status == true && x.Course_Sem_Id == Course_Sem_Id).Select(x => x.Course_Id).FirstOrDefault();
                    int PgmId = db.CMS_AcademicYr_Sem_Programmes.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Active_Status == true).Select(x => x.Pgm_Id).FirstOrDefault();
                    int GroupId = db.CMS_Programmes.Where(x => x.Pgm_Id == PgmId && x.Active_Status == true).Select(x => x.Group_Id).FirstOrDefault();
                    Session["Accyr"] = db.CMS_AcademicYears.Where(x => x.Acc_yr_Id == s.Acc_yr_Id && x.Active_Status == true).Select(x => x.Year).FirstOrDefault();
                    int facid = dbExam.CMS_Internal_Marks.Where(x => x.Active_Status == true && x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Course_Sem_Id == Course_Sem_Id).Select(x => x.Faculty_Id).FirstOrDefault();
                    if (facid != 0)
                    {
                        Session["faculty"] = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Faculty_Id == facid).Select(x => x.Name).FirstOrDefault();
                        Session["Pgm_Type_Id"] = Pgm_Type_Id;
                        Session["form"] = objfaculty.Assesment_AForm(Course_Sem_Id, Acc_Yr_Pgm_Id);
                        Session["GroupId"] = GroupId;
                        Session["Course_Nature_Type"] = db.CMS_Course_Semesters.Where(x => x.Active_Status == true && x.Course_Sem_Id == Course_Sem_Id).Select(x => x.Course_Nature_Type).FirstOrDefault();

                        //    Internals intrnl = objfaculty.getFormA(Course_Sem_Id, Acc_Yr_Pgm_Id, Pgm_Type_Id, Acc_Yr_Sem_Id);
                        Internals intrnl = objfaculty.getFormA(Course_Sem_Id, Acc_Yr_Pgm_Id, Pgm_Type_Id, Acc_Yr_Sem_Id);

                        return View(intrnl);
                    }
                    else
                    {
                        return Redirect("~/Login/Error_Page");
                    }
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult getProgrammenewCourse(int Acc_Yr_Sem_Id, int Acc_Yr_Pgm_Id, int Faculty_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    List<Semesters> cs = new List<Semesters>();

                    cs = objfaculty.getProgrammenewCourse(Acc_Yr_Sem_Id, Acc_Yr_Pgm_Id, Faculty_Id).ToList();


                    return Json(cs, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getInternalMarkCountcourse(int Course_Sem_Id, int Acc_Yr_Pgm_Id, int Pgm_Type_Id, int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int count = 0;
                    int total = 0;
                    List<Internals> internalass = objfaculty.getAssesment(Course_Sem_Id, Acc_Yr_Pgm_Id, Pgm_Type_Id, Acc_Yr_Sem_Id).ToList();

                    var s = internalass.Select(x => x.Int_Ass_Id).Distinct().ToList();
                    List<Programme> students = objfaculty.Search_student(Acc_Yr_Pgm_Id, Course_Sem_Id, Pgm_Type_Id).ToList();
                    total = dbExam.CMS_Internal_Marks.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Course_Sem_Id == Course_Sem_Id && x.Active_Status == true).Count();
                    if (total == 0)
                    {
                        return Json(count, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        foreach (var st in s)
                        {
                            //if ((st == 5) || (st == 2))
                            //{
                            //    total = db.CMS_Internal_Marks.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Int_Ass_Id == st && x.Course_Sem_Id == Course_Sem_Id && x.Active_Status == true).Count();
                            //    if (total == 0)
                            //    {
                            //        count = st;
                            //        break;
                            //    }
                            //    else
                            //    {
                            //        if (total == students.Count())
                            //        {
                            //            count = st;
                            //            break;
                            //        }
                            //        else
                            //        {
                            //            count = 1;
                            //        }
                            //    }

                            //}
                            //else
                            //{

                            total = dbExam.CMS_Internal_Marks.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Int_Ass_Id == st && x.Course_Sem_Id == Course_Sem_Id && x.Active_Status == true).Count();
                            if (total == 0)
                            {
                                count = st;
                                break;
                            }
                            else
                            {
                                count = 100;
                            }

                            //}
                        }
                        return Json(count, JsonRequestBehavior.AllowGet);
                    }

                    // return Json(count, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult get_AformnewCount(int Course_Sem_Id, int Acc_Yr_Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    int count = dbExam.CMS_Internal_Temps.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Course_Sem_Id == Course_Sem_Id && x.Active_Status == true).Count();
                    return Json(count, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getAllSemCourse_By_newPgms(int Acc_Yr_Sem_Id, int Acc_Yr_Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    List<Schedule> sh = objfaculty.getAllSemCourse_By_newPgms(Acc_Yr_Sem_Id, Acc_Yr_Pgm_Id).ToList();
                    return Json(sh, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult get_Aform_LastDate_Count(int Acc_Yr_Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    DateTime dt = DateTime.Now;
                    int count = 0;
                    DateTime Att_EndDate = db.CMS_AcademicYr_Sem_Programmes.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Active_Status == true).Select(x => x.End_Date).FirstOrDefault();
                    if (Att_EndDate > dt)
                    {
                        count = 1;
                    }
                    else
                    {
                        count = 0;
                    }
                    return Json(count, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult get_Aform_LastDate(int Acc_Yr_Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    DateTime Att_EndDate = db.CMS_AcademicYr_Sem_Programmes.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Pgm_Id && x.Active_Status == true).Select(x => x.End_Date).FirstOrDefault();

                    return Json(Att_EndDate, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Add_Attendance_Last_Date(Schedule p)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    CMS_MontlyAttendance_Date ch = db.CMS_MontlyAttendance_Dates.Where(x => x.Active_Status == true && x.Acc_Yr_Sem_Id == p.Acc_Yr_sem_Id).FirstOrDefault();
                    if (ch != null)
                    {
                        return Json(2, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        objfaculty.Add_Attendance_Last_Date(p, Created_By);
                        return Json(1, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Add_Attendance(Programme p)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int pgmid = Convert.ToInt32(p.Pgm_Id);
                    //var count = (from a in db.CMS_Course_Semesters
                    //             where a.Acc_Yr_Sem_Pgm_Id == pgmid && a.Active_Status == true
                    //             join c in db.CMS_Course_Teachers on a.Course_Sem_Id equals c.Course_Sem_Id
                    //             join b in db.CMS_Attendances on c.Course_Teacher_Id equals b.Course_Teacher_Id
                    //             where b.Hour_Id == p.Hour_Id && b.Date == p.Date && b.Active_Status == true && c.Active_Status == true
                    //             select new
                    //             {
                    //                 b,
                    //             }).ToList();
                    //if (count.Count() == 0)
                    //{
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());

                    objfaculty.Add_Attendance(p, Created_By);
                    return Json(1, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(2, JsonRequestBehavior.AllowGet);
                    //}


                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult markAllAbsent(string Hour, string date, int Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.markAllAbsent(Hour, date, Pgm_Id, Created_By);
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Search_SemesterByAccademicYear(int Acc_Yr_Id, int Pgm_Type_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Programme> students = new List<Programme>();

                    students = objfaculty.Search_SemesterByAccademicYear(Acc_Yr_Id, Pgm_Type_Id).ToList();


                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Search_SemesterByAccademicYear_Acc_Yr_Id(int Acc_Yr_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Programme> students = new List<Programme>();

                    students = objfaculty.Search_SemesterByAccademicYear_Acc_Yr_Id(Acc_Yr_Id).ToList();


                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Search_SemesterByAccademicYr(int Acc_Yr_Id, int Pgm_Type_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Programme> students = new List<Programme>();

                    students = objfaculty.Search_SemesterByAccademicYr(Acc_Yr_Id, Pgm_Type_Id).ToList();


                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult Attendance()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.Faculty_Id = Session["Faculty_Id"].ToString();
                    int Fac_Id = Convert.ToInt32(Session["Faculty_Id"]);
                    ViewBag.Dep = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Faculty_Id == Fac_Id).Select(x => x.Dep_Id).FirstOrDefault();
                    ViewBag.Course = new SelectList(objfaculty.Search_Courses(), "Cc_Id", "Course_Name");
                    ViewBag.Semester = new SelectList(objfaculty.Search_Semester(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Language = new SelectList(objfaculty.getAllcommonlanguage(), "Cl_Id", "Common_Language");
                    ViewBag.Date = DateTime.Now;
                    ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Hour = new SelectList(objfaculty.getHour(), "Hour_Id", "Hour");
                    ViewBag.Day = new SelectList(objfaculty.getDay(), "Day_Id", "Day");
                    ViewBag.Academic_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");
                    ViewBag.SecondLanguage = new SelectList(objfaculty.Search_SecondLanguage(), "Lang_Id", "Language");
                    ViewBag.Course = new SelectList(objfaculty.Search_Courses(), "Cc_Id", "Course_Name");
                    ViewBag.Semester = new SelectList(objfaculty.Search_Semester(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Language = new SelectList(objfaculty.getAllcommonlanguage(), "Cl_Id", "Common_Language");
                    DateTime today = DateTime.Today;

                    if (today.DayOfWeek != DayOfWeek.Sunday)
                    {
                        ViewBag.Date = today.ToString("dd-MM-yyyy");
                        ViewBag.Today = objfaculty.getdate(DateTime.Today);
                    }
                    ViewBag.Sem = new SelectList(objfaculty.Search_Sem(), "Acc_Yr_Sem_Id", "Semester");
                    //ViewBag.Hour = new SelectList(objfaculty.getHour(), "Hour_Id", "Hour");
                    //ViewBag.Day = new SelectList(objfaculty.getDay(), "Day_Id", "Day");
                    ViewBag.DisabledDates = objfaculty.getHolidays(DateTime.Today);
                    ViewBag.SecondLang = new SelectList(objfaculty.Search_SecondLanguage(), "Lang_Id", "Language");
                    //ViewBag.Academic_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        //[HttpPost]
        //public ActionResult Search_Faculties_By_Programme(int Pgm_Id)
        //{
        //    if (Session["Log_Id"] != null)
        //    {
        //        try
        //        {


        //            List<Schedule> sch = new List<Schedule>();
        //            sch = objfaculty.Search_Faculties_By_Programmes(Pgm_Id).ToList();
        //            return Json(sch, JsonRequestBehavior.AllowGet);
        //            //}
        //        }
        //        catch
        //        {
        //            return Json(0, JsonRequestBehavior.AllowGet);

        //        }
        //    }
        //    else
        //    {
        //        return Json(0, JsonRequestBehavior.AllowGet);
        //    }

        //}
        public ActionResult Search_SemesterByAccademicYearss(int Acc_Yr_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    List<Programme> students = new List<Programme>();

                    students = objfaculty.Search_SemesterByAccademicYearss(Acc_Yr_Id).ToList();


                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Single_Attendance()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.SecondLanguage = new SelectList(objfaculty.Search_SecondLanguage(), "Lang_Id", "Language");
                    ViewBag.Course = new SelectList(objfaculty.Search_Courses(), "Cc_Id", "Course_Name");
                    ViewBag.Semester = new SelectList(objfaculty.Search_Semester(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Language = new SelectList(objfaculty.getAllcommonlanguage(), "Cl_Id", "Common_Language");
                    DateTime today = DateTime.Today;

                    if (today.DayOfWeek != DayOfWeek.Sunday)
                    {
                        ViewBag.Date = today.ToString("dd-MM-yyyy");
                        ViewBag.Today = objfaculty.getdate(DateTime.Today);
                    }
                    ViewBag.Sem = new SelectList(objfaculty.Search_Sem(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Hour = new SelectList(objfaculty.getHour(), "Hour_Id", "Hour");
                    ViewBag.Day = new SelectList(objfaculty.getDay(), "Day_Id", "Day");
                    ViewBag.DisabledDates = objfaculty.getHolidays(DateTime.Today);
                    ViewBag.SecondLang = new SelectList(objfaculty.Search_SecondLanguage(), "Lang_Id", "Language");
                    ViewBag.Academic_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        // Modifications
        //#region Modifications
        //public ActionResult Dashboard()
        //{
        //    if (Session["Log_Id"] != null)
        //    {
        //        try
        //        {
        //           CMS_AcademicYear Acc_Yr =objfaculty.getCurrentAcademicYear();
        //            int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
        //            int DepId = Convert.ToInt32(Session["DepId"].ToString());
        //            ViewBag.Faculties = db.CMS_Facultys.Where(x => x.Dep_Id == DepId && x.Active_Status == true).Count();
        //            int stream = db.CMS_Departments.Where(x => x.Dep_Id == DepId).Select(x => x.Stream_Type_Id).FirstOrDefault();
        //            ViewBag.UGCourses = (from a in db.CMS_Course_Semesters
        //                                 join b in db.CMS_AcademicYr_Sem_Programmes on a.Acc_Yr_Sem_Pgm_Id equals b.Acc_Yr_Sem_Pgm_Id
        //                                 join c in db.CMS_Programmes on b.Pgm_Id equals c.Pgm_Id
        //                                 join d in db.CMS_AccademicYearSemesters on b.Acc_Yr_sem_Id equals d.Acc_Yr_Sem_Id
        //                                 join e in db.CMS_Courses on a.Course_Id equals e.Course_Id
        //                                 where a.Active_Status == true && d.Acc_yr_Id == Acc_Yr.Acc_yr_Id && d.Active_Status == true && c.Active_Status == true
        //                                 && b.Active_Status == true && (c.Pgm_Type_Id == 1 || c.Pgm_Type_Id == 8) && e.Dep_Id==DepId && c.Stream_Type_Id==stream
        //                                 select e.Course_Code).Distinct().Count();
        //            ViewBag.PGCourses = (from a in db.CMS_Course_Semesters
        //                                 join b in db.CMS_AcademicYr_Sem_Programmes on a.Acc_Yr_Sem_Pgm_Id equals b.Acc_Yr_Sem_Pgm_Id
        //                                 join c in db.CMS_Programmes on b.Pgm_Id equals c.Pgm_Id
        //                                 join d in db.CMS_AccademicYearSemesters on b.Acc_Yr_sem_Id equals d.Acc_Yr_Sem_Id
        //                                 join e in db.CMS_Courses on a.Course_Id equals e.Course_Id
        //                                 where a.Active_Status == true && d.Acc_yr_Id == Acc_Yr.Acc_yr_Id && d.Active_Status == true && c.Active_Status == true
        //                                 && b.Active_Status == true && c.Pgm_Type_Id == 2 && e.Dep_Id == DepId && c.Stream_Type_Id == stream
        //                               select e.Course_Code).Distinct().Count();
        //            //var q=(from  b in db.CMS_AcademicYr_Sem_Programmes 
        //            //       join c in db.CMS_AccademicYearSemesters on b.Acc_Yr_sem_Id equals c.Acc_Yr_Sem_Id
        //            //       join d in db.CMS_Programmes on b.Pgm_Id equals d.Pgm_Id
        //            //       where  b.Active_Status==true && c.Active_Status==true && d.Active_Status==true
        //            //       && d.Dep_Id==DepId 
        //            return View();
        //        }

        //        catch
        //        {
        //            return Redirect("~/Login/Error_Page");
        //        }
        //    }
        //    else
        //    {
        //        return Redirect("Faculty_Login");

        //    }
        //}
        //public ActionResult Internal_Mark_Home()
        //{
        //    if (Session["Log_Id"] != null)
        //    {
        //        try
        //        {

        //            int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
        //            List<Internals> intrnl = objfaculty.getCoursesFacultyWise(Faculty_Id).ToList();
        //            return View(intrnl);
        //        }

        //        catch
        //        {
        //            return Redirect("~/Login/Error_Page");
        //        }
        //    }
        //    else
        //    {
        //        return Redirect("Faculty_Login");

        //    }
        //}
        //public ActionResult ViewFormA(int Course_Sem_Id)
        //{
        //    if (Session["Log_Id"] != null)
        //    {
        //        try
        //        {

        //            //int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
        //            List<Internals> intrnl = objfaculty.ViewFormA(Course_Sem_Id).ToList();
        //            return View(intrnl);
        //        }

        //        catch
        //        {
        //            return Redirect("~/Login/Error_Page");
        //        }
        //    }
        //    else
        //    {
        //        return Redirect("Faculty_Login");

        //    }
        //}
        //#endregion
        public ActionResult Reports()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Updated_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");
                    DateTime dt = DateTime.Now;
                    ViewBag.Year = db.CMS_AcademicYears.Where(x => x.Start_Date <= dt && x.End_Date >= dt).Select(x => x.Acc_yr_Id).FirstOrDefault();
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult studentAttendanceRegister()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.UPRN_No = new SelectList(objfaculty.getuprn(), "UPRN", "UPRN");
                    ViewBag.Sem = new SelectList(objfaculty.Search_Semss(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Academic_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Search_ProgrammeAllpgm(int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    List<Programme> pgms = objfaculty.getSemesterProgrammes(Acc_Yr_Sem_Id).ToList();
                    return Json(pgms, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Search_Semester_Students(int Acc_Yr_Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Student> students = new List<Student>();

                    students = objfaculty.Search_Semester_Students(Acc_Yr_Pgm_Id).ToList();


                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getStudent_Attendance(string uprn, string firstDay, string lastDay, string Key)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    DateTime dt = DateTime.Today;
                    DateTime sdate = Convert.ToDateTime(firstDay);
                    DateTime edate = Convert.ToDateTime(lastDay);
                    CMS_AcademicYear acy = objfaculty.getCurrentAcademicYear();
                    CMS_AccademicYearSemester pgm = db.CMS_AccademicYearSemesters.Where(x => x.Start_Date <= dt && x.End_Date >= dt && x.Acc_yr_Id == acy.Acc_yr_Id).FirstOrDefault();

                    ViewBag.hour = db.CMS_Bvoc_Hours.ToList();
                    //if (Key == "Today")
                    //{
                    //    sdate = dt;

                    //}

                    //if (Key == "sem")
                    //{
                    //    sdate = pgm.Start_Date;
                    //    edate = pgm.End_Date;
                    if (edate > dt)
                    {
                        edate = dt;
                    }

                    //}
                    List<AttendanceReport> ar = objfaculty.getStudentDetails(uprn, sdate).ToList();
                    if (ar.Count > 0)
                    {
                        ViewBag.Name = ar[0].Name;
                        ViewBag.UPRN = ar[0].UPRN;
                        ViewBag.Programme = "Sem " + ar[0].Sem + "  " + ar[0].Programme;
                    }
                    List<AttendanceReport> att = objfaculty.getStudent_Attendance(uprn, sdate, edate).ToList();
                    ViewBag.key = Key;
                    ViewBag.sdate = sdate;
                    ViewBag.edate = edate;
                    ViewBag.WorkingDays = att.Count();
                    int f = att.Where(x => x.Day_Status == "F").Count();
                    double h = (double)att.Where(x => x.Day_Status == "H").Count() / 2.0;
                    ViewBag.PresentDays = f + h;
                    if (att.Count() > 0)
                    {
                        double s = (f + h) * 100 / att.Count();
                        double s1 = Math.Round(s, 2);
                        ViewBag.Percentage = s1;
                    }
                    else
                    {
                        ViewBag.Percentage = 0;
                    }
                    return PartialView(att);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult Get_SemDates(string uprn)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    List<CMS_AccademicYearSemester> accsem = objfaculty.getSemDates().ToList();
                    return Json(accsem, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult get_attndnc(string UPRN, string Date)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    List<AttendanceReport> sch = new List<AttendanceReport>();
                    sch = objfaculty.get_attndnc(UPRN, Date).ToList();
                    return Json(sch, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult programmeAttendanceRegister()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.Sem = new SelectList(objfaculty.Search_Semss(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Updated_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");
                    DateTime dt = DateTime.Now;
                    ViewBag.Year = db.CMS_AcademicYears.Where(x => x.Start_Date <= dt && x.End_Date >= dt).Select(x => x.Acc_yr_Id).FirstOrDefault();
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Search_SemesterByAccademicSem(int Acc_Yr_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    List<Programme> students = new List<Programme>();

                    students = objfaculty.Search_SemesterByAccademicSem(Acc_Yr_Id).ToList();


                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Search_SemesterBy_Classwarden(int Acc_Yr_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    List<Programme> students = new List<Programme>();


                    CMS_AcademicYear Acc_Yr = objfaculty.getCurrentAcademicYear();
                    var hodRecord = db.CMS_HODs.Where(h => h.Faculty_Id == Faculty_Id && h.Acc_Yr == Acc_Yr.Acc_yr_Id && h.Active_Status == true).FirstOrDefault();
                    if (hodRecord != null)
                    {
                        Session["hodRecord"] = hodRecord.HOD_Id;

                        students = objfaculty.Search_SemesterBy_Classwarden(Acc_Yr_Id, Faculty_Id).ToList();

                        if (students.Count == 0)
                        {
                            students = objfaculty.Search_SemesterByAccademicSem(Acc_Yr_Id).ToList();
                        }
                    }
                    else
                    {

                        students = objfaculty.Search_SemesterBy_Classwarden(Acc_Yr_Id, Faculty_Id).ToList();
                    }
                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getProgramme_Attendance(int Acc_Yr_Sem_Pgm_Id, string firstDay, string lastDay)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    // List<AttendanceReport> att = new List<AttendanceReport>();

                    List<AttendanceReport> rep = objfaculty.getProgramme_Attendance(Acc_Yr_Sem_Pgm_Id, firstDay, lastDay).ToList();
                    //  var result = rep.Select(x => x.report1).Distinct().ToList();
                    //foreach (var item in result)
                    //{
                    //    AttendanceReport att_rep = new AttendanceReport();
                    //    att_rep.UPRN = item;
                    //    att_rep.Name = rep.Where(x => x.uprn == item).Select(x => x.name).FirstOrDefault();
                    //    att_rep.rpt = rep.Where(x => x.uprn == item).OrderBy(x => x.date).ToList();
                    //    att_rep.pdays = rep.Where(x => x.uprn == item && x.day_status == "X").Count() + ((double)rep.Where(x => x.uprn == item && x.day_status == "H").Count() / 2.0);
                    //    att_rep.Working_Days = rep.Select(x => x.date).Distinct().Count();
                    //    double s = att_rep.pdays * 100 / att_rep.Working_Days;
                    //    double s1 = Math.Round(s, 2);
                    //    att_rep.Percentage = s1;
                    //    att.Add(att_rep);
                    //}
                    ViewBag.PgmType = (from a in db.CMS_AcademicYr_Sem_Programmes
                                       join b in db.CMS_Programmes on a.Pgm_Id equals b.Pgm_Id
                                       where a.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Sem_Pgm_Id
                                       select b.Pgm_Type_Id).FirstOrDefault();
                    if (rep.Count > 0)
                    {
                        ViewBag.Dates = rep[0].WorkingDates.ToList();
                    }
                    return View(rep);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult monthlyAttendanceReport()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Updated_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");
                    DateTime dt = DateTime.Now;
                    ViewBag.Year = db.CMS_AcademicYears.Where(x => x.Start_Date <= dt && x.End_Date >= dt).Select(x => x.Acc_yr_Id).FirstOrDefault();
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult getProgrammeDetails(int Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    CMS_AcademicYr_Sem_Programme pgm = db.CMS_AcademicYr_Sem_Programmes.Where(x => x.Acc_Yr_Sem_Pgm_Id == Pgm_Id).FirstOrDefault();
                    return Json(pgm, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getAttendanceReport_Monthly(int Acc_Yr_Sem_Pgm_Id, string firstDay, string lastDay, int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    var s = objfaculty.getCurrentAcademicYear();
                    //  int semid = db.CMS_AccademicYearSemesters.Where(x => x.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id && x.Active_Status == true && x.Acc_yr_Id == s.Acc_yr_Id).Select(x => x.Sem_Id).FirstOrDefault();
                    ViewBag.semester = db.CMS_AccademicYearSemesters.Where(x => x.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id && x.Active_Status == true && x.Acc_yr_Id == s.Acc_yr_Id).Select(x => x.Sem_Id).FirstOrDefault();
                    List<AttendanceReport> rep = objfaculty.getAttendanceReport_Monthly(Acc_Yr_Sem_Pgm_Id, firstDay, lastDay).ToList();

                    return View(rep);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult semsterAttendanceReport()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Updated_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");
                    DateTime dt = DateTime.Now;
                    ViewBag.Year = db.CMS_AcademicYears.Where(x => x.Start_Date <= dt && x.End_Date >= dt).Select(x => x.Acc_yr_Id).FirstOrDefault();
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult getAttendanceReport_Semester(int Acc_Yr_Sem_Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    List<AttendanceReport> rep = objfaculty.getAttendanceReport_Semester(Acc_Yr_Sem_Pgm_Id).ToList();

                    return View(rep);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Search_ProgrammeDetails(int Acc_YrId, int Pgm_Type_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int dep_id = Convert.ToInt32(Session["DepId"]);//Session["DepId"];
                    var PgmIds = db.CMS_Programmes.Where(x => x.Pgm_Type_Id == Pgm_Type_Id && x.Dep_Id == dep_id && x.Active_Status == true).ToList();
                    return Json(PgmIds, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");
            }
        }

        public ActionResult Allotted_Applicants()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    int dep_id = db.CMS_HODs.Where(x => x.Faculty_Id == id).Select(x => x.Dep_Id).FirstOrDefault();

                    if (dep_id == 0)
                    {
                        dep_id = (from a in db.CMS_ClassWardens
                                  join b in db.CMS_Facultys on a.Faculty_Id equals b.Faculty_Id
                                  where a.Faculty_Id == id
                                  select b.Dep_Id).FirstOrDefault();
                    }

                    if (dep_id != 0)
                    {
                        int count = db.CMS_Programmes.Where(x => x.Dep_Id == dep_id).Select(x => x.Pgm_Id).Count();
                        if (dep_id == 24)
                        {
                            count = count + 1;
                        }
                        ViewBag.count = count;
                        if (count >= 2)
                        {
                            ViewBag.ProgrammeTypes = new SelectList(objfaculty.getAllProgrammeTypes(), "Pgm_Type_Id", "Programme_Type");
                            return View();
                        }
                        else
                        {
                            int Pgm_Type_Id = db.CMS_Programmes.Where(x => x.Dep_Id == dep_id).Select(x => x.Pgm_Type_Id).FirstOrDefault();
                            int Pgm_Id = db.CMS_Programmes.Where(x => x.Dep_Id == dep_id).Select(x => x.Pgm_Id).FirstOrDefault();
                            //string pgm_id = Pgm_Id.ToString();
                            List<RankList> aplcnts = new List<RankList>();
                            aplcnts = objfaculty.getAllottedList(Pgm_Id, Pgm_Type_Id).ToList();
                            return View(aplcnts);
                        }
                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        [HttpPost]
        public ActionResult getProgrammes(int Pgm_Type_Id)
        {
            int id = Convert.ToInt32(Session["Faculty_Id"].ToString());
            int dep_id = db.CMS_HODs.Where(x => x.Faculty_Id == id).Select(x => x.Dep_Id).FirstOrDefault();
            if (dep_id == 0)
            {
                dep_id = (from a in db.CMS_ClassWardens
                          join b in db.CMS_Facultys on a.Faculty_Id equals b.Faculty_Id
                          where a.Faculty_Id == id
                          select b.Dep_Id).FirstOrDefault();
            }
            if (dep_id == 5 || dep_id == 6 || dep_id == 22 || dep_id == 24 || dep_id == 16)
            {
                List<Programme> aplcnts = new List<Programme>();
                aplcnts = objfaculty.getAllProgrammes(dep_id, Pgm_Type_Id).ToList();
                return Json(aplcnts, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult getAllottedList(int Pgm_Type_Id)
        {
            int id = Convert.ToInt32(Session["Faculty_Id"].ToString());
            int dep_id = db.CMS_HODs.Where(x => x.Faculty_Id == id).Select(x => x.Dep_Id).FirstOrDefault();
            if (dep_id == 0)
            {
                dep_id = (from a in db.CMS_ClassWardens
                          join b in db.CMS_Facultys on a.Faculty_Id equals b.Faculty_Id
                          where a.Faculty_Id == id
                          select b.Dep_Id).FirstOrDefault();
            }
            int Pgm_id = db.CMS_Programmes.Where(x => x.Dep_Id == dep_id && x.Pgm_Type_Id == Pgm_Type_Id).Select(x => x.Pgm_Id).FirstOrDefault();
            //string pgm_id = Pgm_id.ToString();
            //checking haiii 2 test
            List<RankList> aplcnts = new List<RankList>();
            aplcnts = objfaculty.getAllottedList(Pgm_id, Pgm_Type_Id).ToList();
            return Json(aplcnts, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult getAllottedListprogramme(int Pgm_Type_Id, int Pgm_Id)
        {
            //string pgm_id = Pgm_Id.ToString();
            List<RankList> aplcnts = new List<RankList>();
            aplcnts = objfaculty.getAllottedList(Pgm_Id, Pgm_Type_Id).ToList();
            return Json(aplcnts, JsonRequestBehavior.AllowGet);
        }

        #region Mooc Courses
        public ActionResult MoocCourses()
        {
            if (Session["Faculty_Id"] != null)
            {
                try
                {
                    Guid id = (Guid)Session["Log_Id"];
                    CMS_Admission.Areas.Allotment.Controllers.DAL.DALAllotment objAdmsn = new CMS_Admission.Areas.Allotment.Controllers.DAL.DALAllotment();
                    // ViewBag.Programme = new SelectList(objAdmsn.getAllProgrammes(), "Pgm_Id", "Programme");
                    // ViewBag.PartTwo = new SelectList(objAdmsn.getAllPartTwoSubjects(), "Lang_Id", "Language");
                    ViewBag.Programmes_Type = new SelectList(objAdmsn.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Updated_Year = new SelectList(objAdmsn.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");

                    List<MOOC_Registration> aplcnts = new List<MOOC_Registration>();
                    aplcnts = objfaculty.getAllMoocApplicants().ToList();
                    return View(aplcnts);
                    // return Json(aplcnts, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        [HttpPost]
        public ActionResult Search_Student_MoocPgm_Typeid(int Pgm_Type_Id, int Pgm_Id, int Acc_Yr_Id)
        {
            List<MOOC_Registration> aplcnts = new List<MOOC_Registration>();
            aplcnts = objfaculty.getAllMoocApplicantsbyId(Pgm_Type_Id, Pgm_Id, Acc_Yr_Id).ToList();
            //  return View(aplcnts);
            return Json(aplcnts, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region WorkDiary
        public ActionResult Work_Diary_View()
        {
            if (Session["Faculty_Id"] != null)
            {
                try
                {
                    int facid = Convert.ToInt32(Session["Faculty_Id"]);
                    List<Faculty_Work_Diary> list = objfaculty.getWorkListByFacultyId(facid).ToList();
                    return View(list);
                }
                catch (Exception ex)
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        public ActionResult Work_Diary()
        {
            if (Session["Faculty_Id"] != null)
            {
                try
                {


                    int facid = Convert.ToInt32(Session["Faculty_Id"]);
                    // IEnumerable<Programme> lstSem = objfaculty.Get_Semesterbycourse();
                    List<Internals> lstSem = objfaculty.getFacultyWiseCourse(facid);
                    Internals objInt = new Internals();
                    objInt.Semester = "None";
                    objInt.Acc_Yr_Sem_Pgm_Id = 0;
                    //lstSem = lstSem.Append(objInt);
                    lstSem.Add(objInt);

                    ViewBag.SemesterTask1 = new SelectList(lstSem, "Acc_Yr_Sem_Pgm_Id", "Semester");

                    IEnumerable<WorkMode> lstWM = objfaculty.GetWorkModes();
                    ViewBag.WorkModeTask1 = new SelectList(lstWM, "Id", "Work_Mode");

                    IEnumerable<Faculty_Duty> lstDuty = objfaculty.GetDuties();
                    ViewBag.DutyTask1 = new SelectList(lstDuty, "Id", "Duty");

                    IEnumerable<ICT_Platform> lstICT = objfaculty.GetICTPlatform();
                    ViewBag.ICTTask1 = new SelectList(lstICT, "Id", "ICT_Platforms");


                    List<Faculty_Teaching_Hours> obj = new List<Faculty_Teaching_Hours>();

                    if (Request.QueryString["Id"] != null)
                    {
                        int WorkId = Convert.ToInt32(Request.QueryString["Id"]);
                        obj = objfaculty.GetWorkDetailsById(WorkId);
                        string strWorkDate = db.CMS_Faculty_Diarys.Where(x => x.Id == WorkId).Select(x => x.Work_Date).FirstOrDefault().ToString("dd/MM/yyyy");
                        ViewBag.WorkDate = strWorkDate;
                    }
                    else
                    {
                        obj = objfaculty.GetWorkDetailsByDate(facid, DateTime.Today.Date);
                        ViewBag.WorkDate = DateTime.Now.ToString("dd/MM/yyyy");
                    }



                    return View(obj);
                }
                catch
                {
                    return Redirect("Faculty_Login");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult getWorkDetails(string dtWorkDate)
        {
            if (dtWorkDate != "")
            {
                DateTime dt = Convert.ToDateTime(dtWorkDate);
                int facid = Convert.ToInt32(Session["Faculty_Id"]);
                List<Faculty_Teaching_Hours> obj = new List<Faculty_Teaching_Hours>();
                obj = objfaculty.GetWorkDetailsByDate(facid, dt);
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult Add_WorkDetails(List<Faculty_Teaching_Hours> item, string dtWorkDate)
        {

            if (Session["Log_Id"] != null)
            {
                try
                {
                    int facId = Convert.ToInt32(Session["Faculty_Id"]);
                    int deptId = Convert.ToInt32(Session["DepId"]);
                    int val = objfaculty.AddWorkDetails(item, dtWorkDate, facId, deptId);
                    return Json(val, JsonRequestBehavior.AllowGet);

                }

                catch (Exception ex)
                {

                    return Json(ex.InnerException.ToString(), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult Work_Diary_Details_Edit()
        {
            if (Session["Faculty_Id"] != null)
            {
                try
                {


                    int facid = Convert.ToInt32(Session["Faculty_Id"]);
                    int SelectedSemTask1 = 0; int SelectedSemTask2 = 0; int SelectedSemTask3 = 0; int SelectedSemTask4 = 0; int SelectedSemTask5 = 0;
                    int SelectedCourseTask1 = 0; int SelectedCourseTask2 = 0; int SelectedCourseTask3 = 0; int SelectedCourseTask4 = 0; int SelectedCourseTask5 = 0;
                    int SelectedWorkTask1 = 0; int SelectedWorkTask2 = 0; int SelectedWorkTask3 = 0; int SelectedWorkTask4 = 0; int SelectedWorkTask5 = 0;
                    int SelectedICTTask1 = 0; int SelectedICTTask2 = 0; int SelectedICTTask3 = 0; int SelectedICTTask4 = 0; int SelectedICTTask5 = 0;
                    int SelectedDutyTask1 = 0; int SelectedDutyTask2 = 0; int SelectedDutyTask3 = 0; int SelectedDutyTask4 = 0; int SelectedDutyTask5 = 0;

                    string strWorkModeDetails1 = string.Empty;
                    string strWorkModeDetails2 = string.Empty;
                    string strWorkModeDetails3 = string.Empty;
                    string strWorkModeDetails4 = string.Empty;
                    string strWorkModeDetails5 = string.Empty;

                    string strDutyDetails1 = string.Empty;
                    string strDutyDetails2 = string.Empty;
                    string strDutyDetails3 = string.Empty;
                    string strDutyDetails4 = string.Empty;
                    string strDutyDetails5 = string.Empty;

                    if (Request.QueryString["Id"] != null)
                    {
                        int WorkId = Convert.ToInt32(Request.QueryString["Id"]);

                        Faculty_Work_Diary lst = objfaculty.getWorkById(WorkId);
                        ViewBag.WorkDate = lst.Work_Date;
                        List<Faculty_Teaching_Hours> obj = objfaculty.GetWorkDetailsById(WorkId);
                        foreach (var item in obj)
                        {
                            int intHours = item.Hours;
                            if (intHours == 1)
                            {
                                ViewBag.chkHour1 = true;
                            }
                            else if (intHours == 2)
                            {
                                ViewBag.chkHour2 = true;
                            }
                            else if (intHours == 3)
                            {
                                ViewBag.chkHour3 = true;
                            }
                            else if (intHours == 4)
                            {
                                ViewBag.chkHour4 = true;
                            }
                            else if (intHours == 5)
                            {
                                ViewBag.chkHour5 = true;
                            }
                            else
                            {
                                ViewBag.chkHour1 = false;
                                ViewBag.chkHour2 = false;
                                ViewBag.chkHour3 = false;
                                ViewBag.chkHour4 = false;
                                ViewBag.chkHour5 = false;
                            }

                        }
                        SelectedSemTask1 = obj.Where(x => x.Hours == 1).Select(a => a.Semester).FirstOrDefault();
                        SelectedSemTask2 = obj.Where(x => x.Hours == 2).Select(a => a.Semester).FirstOrDefault();
                        SelectedSemTask3 = obj.Where(x => x.Hours == 3).Select(a => a.Semester).FirstOrDefault();
                        SelectedSemTask4 = obj.Where(x => x.Hours == 4).Select(a => a.Semester).FirstOrDefault();
                        SelectedSemTask5 = obj.Where(x => x.Hours == 5).Select(a => a.Semester).FirstOrDefault();
                        SelectedCourseTask1 = obj.Where(x => x.Hours == 1).Select(a => a.Course).FirstOrDefault();
                        SelectedCourseTask2 = obj.Where(x => x.Hours == 2).Select(a => a.Course).FirstOrDefault();
                        SelectedCourseTask3 = obj.Where(x => x.Hours == 3).Select(a => a.Course).FirstOrDefault();
                        SelectedCourseTask4 = obj.Where(x => x.Hours == 4).Select(a => a.Course).FirstOrDefault();
                        SelectedCourseTask5 = obj.Where(x => x.Hours == 5).Select(a => a.Course).FirstOrDefault();
                        SelectedWorkTask1 = obj.Where(x => x.Hours == 1).Select(a => a.Work_Mode).FirstOrDefault();
                        SelectedWorkTask2 = obj.Where(x => x.Hours == 2).Select(a => a.Work_Mode).FirstOrDefault();
                        SelectedWorkTask3 = obj.Where(x => x.Hours == 3).Select(a => a.Work_Mode).FirstOrDefault();
                        SelectedWorkTask4 = obj.Where(x => x.Hours == 4).Select(a => a.Work_Mode).FirstOrDefault();
                        SelectedWorkTask5 = obj.Where(x => x.Hours == 5).Select(a => a.Work_Mode).FirstOrDefault();
                        SelectedICTTask1 = obj.Where(x => x.Hours == 1).Select(a => a.ICT_Platform).FirstOrDefault();
                        SelectedICTTask2 = obj.Where(x => x.Hours == 2).Select(a => a.ICT_Platform).FirstOrDefault();
                        SelectedICTTask3 = obj.Where(x => x.Hours == 3).Select(a => a.ICT_Platform).FirstOrDefault();
                        SelectedICTTask4 = obj.Where(x => x.Hours == 4).Select(a => a.ICT_Platform).FirstOrDefault();
                        SelectedICTTask5 = obj.Where(x => x.Hours == 5).Select(a => a.ICT_Platform).FirstOrDefault();
                        SelectedDutyTask1 = obj.Where(x => x.Hours == 1).Select(a => a.Duty).FirstOrDefault();
                        SelectedDutyTask2 = obj.Where(x => x.Hours == 2).Select(a => a.Duty).FirstOrDefault();
                        SelectedDutyTask3 = obj.Where(x => x.Hours == 3).Select(a => a.Duty).FirstOrDefault();
                        SelectedDutyTask4 = obj.Where(x => x.Hours == 4).Select(a => a.Duty).FirstOrDefault();
                        SelectedDutyTask5 = obj.Where(x => x.Hours == 5).Select(a => a.Duty).FirstOrDefault();
                        ViewBag.Details1 = obj.Where(x => x.Hours == 1).Select(a => a.Details).FirstOrDefault();
                        ViewBag.Details2 = obj.Where(x => x.Hours == 2).Select(a => a.Details).FirstOrDefault();
                        ViewBag.Details3 = obj.Where(x => x.Hours == 3).Select(a => a.Details).FirstOrDefault();
                        ViewBag.Details4 = obj.Where(x => x.Hours == 4).Select(a => a.Details).FirstOrDefault();
                        ViewBag.Details5 = obj.Where(x => x.Hours == 5).Select(a => a.Details).FirstOrDefault();

                        ViewBag.WorkModeDetails1 = obj.Where(x => x.Hours == 1).Select(a => a.strWorkModeRemarks.Trim()).FirstOrDefault();
                        ViewBag.WorkModeDetails2 = obj.Where(x => x.Hours == 2).Select(a => a.strWorkModeRemarks.Trim()).FirstOrDefault();
                        ViewBag.WorkModeDetails3 = obj.Where(x => x.Hours == 3).Select(a => a.strWorkModeRemarks.Trim()).FirstOrDefault();
                        ViewBag.WorkModeDetails4 = obj.Where(x => x.Hours == 4).Select(a => a.strWorkModeRemarks.Trim()).FirstOrDefault();
                        ViewBag.WorkModeDetails5 = obj.Where(x => x.Hours == 5).Select(a => a.strWorkModeRemarks.Trim()).FirstOrDefault();

                        ViewBag.DutyDetails1 = obj.Where(x => x.Hours == 1).Select(a => a.strDutyRemarks.Trim()).FirstOrDefault();
                        ViewBag.DutyDetails2 = obj.Where(x => x.Hours == 2).Select(a => a.strDutyRemarks.Trim()).FirstOrDefault();
                        ViewBag.DutyDetails3 = obj.Where(x => x.Hours == 3).Select(a => a.strDutyRemarks.Trim()).FirstOrDefault();
                        ViewBag.DutyDetails4 = obj.Where(x => x.Hours == 4).Select(a => a.strDutyRemarks.Trim()).FirstOrDefault();
                        ViewBag.DutyDetails5 = obj.Where(x => x.Hours == 5).Select(a => a.strDutyRemarks.Trim()).FirstOrDefault();

                        ViewBag.ICTDetails1 = obj.Where(x => x.Hours == 1).Select(a => a.strICTRemarks).FirstOrDefault();
                        ViewBag.ICTDetails2 = obj.Where(x => x.Hours == 2).Select(a => a.strICTRemarks).FirstOrDefault();
                        ViewBag.ICTDetails3 = obj.Where(x => x.Hours == 3).Select(a => a.strICTRemarks).FirstOrDefault();
                        ViewBag.ICTDetails4 = obj.Where(x => x.Hours == 4).Select(a => a.strICTRemarks).FirstOrDefault();
                        ViewBag.ICTDetails5 = obj.Where(x => x.Hours == 5).Select(a => a.strICTRemarks).FirstOrDefault();



                    }
                    IEnumerable<Programme> lstSem = objfaculty.Get_Semesterbycourse();
                    // IEnumerable<Programme> lstSem = objfaculty.GetSemester();
                    List<Programme> pgms = new List<Programme>();
                    var p = new Programme()
                    {
                        Acc_Yr_Sem_Id = 0,
                        Semester = "---Select---"
                    };
                    pgms.Add(p);
                    lstSem = lstSem.Union(pgms);

                    ViewBag.SemesterTask1 = new SelectList(lstSem, "Acc_Yr_Sem_Id", "Semester", SelectedSemTask1);
                    ViewBag.SemesterTask2 = new SelectList(lstSem, "Acc_Yr_Sem_Id", "Semester", SelectedSemTask2);
                    ViewBag.SemesterTask3 = new SelectList(lstSem, "Acc_Yr_Sem_Id", "Semester", SelectedSemTask3);
                    ViewBag.SemesterTask4 = new SelectList(lstSem, "Acc_Yr_Sem_Id", "Semester", SelectedSemTask4);
                    ViewBag.SemesterTask5 = new SelectList(lstSem, "Acc_Yr_Sem_Id", "Semester", SelectedSemTask5);

                    IEnumerable<Semesters> lstCourse1 = objfaculty.getProgrammeCoursebySem(SelectedSemTask1, facid).ToList();
                    IEnumerable<Semesters> lstCourse2 = objfaculty.getProgrammeCoursebySem(SelectedSemTask2, facid).ToList();
                    IEnumerable<Semesters> lstCourse3 = objfaculty.getProgrammeCoursebySem(SelectedSemTask3, facid).ToList();
                    IEnumerable<Semesters> lstCourse4 = objfaculty.getProgrammeCoursebySem(SelectedSemTask4, facid).ToList();
                    IEnumerable<Semesters> lstCourse5 = objfaculty.getProgrammeCoursebySem(SelectedSemTask5, facid).ToList();
                    // IEnumerable<Schedule> lstCourse = objfaculty.getAllCoursesByFaculty(facid);
                    // IEnumerable<Schedule> lstCourse = objfaculty.getAllCourses(facid);
                    // IEnumerable<Schedule> lstc = objfaculty.getAllCourses(facid);
                    List<Semesters> sch = new List<Semesters>();
                    var s = new Semesters()
                    {
                        Course_Id = 0,
                        Course_Name = "---Select---"
                    };
                    sch.Add(s);
                    lstCourse1 = lstCourse1.Union(sch);
                    lstCourse2 = lstCourse2.Union(sch);
                    lstCourse3 = lstCourse3.Union(sch);
                    lstCourse4 = lstCourse4.Union(sch);
                    lstCourse5 = lstCourse5.Union(sch);

                    ViewBag.CoursesTask1 = new SelectList(lstCourse1, "Course_Id", "Course_Name", SelectedCourseTask1);
                    ViewBag.CoursesTask2 = new SelectList(lstCourse2, "Course_Id", "Course_Name", SelectedCourseTask2);
                    ViewBag.CoursesTask3 = new SelectList(lstCourse3, "Course_Id", "Course_Name", SelectedCourseTask3);
                    ViewBag.CoursesTask4 = new SelectList(lstCourse4, "Course_Id", "Course_Name", SelectedCourseTask4);
                    ViewBag.CoursesTask5 = new SelectList(lstCourse5, "Course_Id", "Course_Name", SelectedCourseTask5);

                    IEnumerable<WorkMode> lstWM = objfaculty.GetWorkModes();
                    List<WorkMode> wrk = new List<WorkMode>();
                    var w = new WorkMode()
                    {
                        Id = 0,
                        Work_Mode = "---Select---"
                    };
                    wrk.Add(w);
                    lstWM = lstWM.Union(wrk);

                    ViewBag.WorkModeTask1 = new SelectList(lstWM, "Id", "Work_Mode", SelectedWorkTask1);
                    ViewBag.WorkModeTask2 = new SelectList(lstWM, "Id", "Work_Mode", SelectedWorkTask2);
                    ViewBag.WorkModeTask3 = new SelectList(lstWM, "Id", "Work_Mode", SelectedWorkTask3);
                    ViewBag.WorkModeTask4 = new SelectList(lstWM, "Id", "Work_Mode", SelectedWorkTask4);
                    ViewBag.WorkModeTask5 = new SelectList(lstWM, "Id", "Work_Mode", SelectedWorkTask5);


                    IEnumerable<Faculty_Duty> lstDuty = objfaculty.GetDuties();
                    List<Faculty_Duty> dut = new List<Faculty_Duty>();
                    var y = new Faculty_Duty()
                    {
                        Id = 0,
                        Duty = "---Select---"
                    };
                    dut.Add(y);
                    lstDuty = lstDuty.Union(dut);
                    ViewBag.DutyTask1 = new SelectList(lstDuty, "Id", "Duty", SelectedDutyTask1);
                    ViewBag.DutyTask2 = new SelectList(lstDuty, "Id", "Duty", SelectedDutyTask2);
                    ViewBag.DutyTask3 = new SelectList(lstDuty, "Id", "Duty", SelectedDutyTask3);
                    ViewBag.DutyTask4 = new SelectList(lstDuty, "Id", "Duty", SelectedDutyTask4);
                    ViewBag.DutyTask5 = new SelectList(lstDuty, "Id", "Duty", SelectedDutyTask5);


                    IEnumerable<ICT_Platform> lstICT = objfaculty.GetICTPlatform();
                    List<ICT_Platform> ict = new List<ICT_Platform>();
                    var i = new ICT_Platform()
                    {
                        Id = 0,
                        ICT_Platforms = "---Select---"
                    };
                    ict.Add(i);
                    lstICT = lstICT.Union(ict);


                    ViewBag.ICTTask1 = new SelectList(lstICT, "Id", "ICT_Platforms", SelectedICTTask1);
                    ViewBag.ICTTask2 = new SelectList(lstICT, "Id", "ICT_Platforms", SelectedICTTask2);
                    ViewBag.ICTTask3 = new SelectList(lstICT, "Id", "ICT_Platforms", SelectedICTTask3);
                    ViewBag.ICTTask4 = new SelectList(lstICT, "Id", "ICT_Platforms", SelectedICTTask4);
                    ViewBag.ICTTask5 = new SelectList(lstICT, "Id", "ICT_Platforms", SelectedICTTask5);


                    return View();
                }
                catch (Exception ex)
                {
                    return Redirect("Faculty_Login");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult Work_Diary_Details()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (Request.QueryString["Id"] != null)
                    {

                        int WorkId = Convert.ToInt32(Request.QueryString["Id"]);
                        Faculty_Work_Diary lst = objfaculty.getWorkById(WorkId);
                        ViewBag.Name = lst.Name;
                        ViewBag.TaskDate = lst.Work_Date;
                        List<Faculty_Teaching_Hours> lstWork = objfaculty.GetWorkDetailsById(WorkId).ToList();
                        return View(lstWork);

                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            return Redirect("Faculty_Login");
        }
        public ActionResult getProgrammeCourses(int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int facid = Convert.ToInt32(Session["Faculty_Id"]);
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    CMS_Login cl = db.CMS_Logins.Where(x => x.Active_Status == true && x.Log_Id == Created_By).FirstOrDefault();
                    CMS_Faculty cf = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Mobile == cl.Mobile && x.Faculty_Id == facid).FirstOrDefault();
                    List<Semesters> P = new List<Semesters>();
                    if (cf != null)
                    {
                        P = objfaculty.getProgrammeCoursebySem(Acc_Yr_Sem_Id, cf.Faculty_Id).ToList();
                    }

                    return Json(P, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }



        //Work Diary Approve
        public ActionResult Work_Diary_Approve()
        {

            if (Session["Log_Id"] != null)
            {
                try
                {
                    return View();

                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        [HttpPost]
        public ActionResult Work_Diary_Approves()
        {

            if (Session["Log_Id"] != null)
            {
                try
                {
                    DateTime dtNow = DateTime.Now;
                    int DepId = Convert.ToInt32(Session["DepId"].ToString());
                    int FactId = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    string role = Session["Role"].ToString();
                    IEnumerable<Schedule> objFact = objfaculty.Get_Faculty(FactId);
                    int desId = objFact.Select(x => x.Des_Id).FirstOrDefault();
                    if (desId == 5 || role == "HOD")
                    {
                        List<Faculty_Work_Diary> fd = objfaculty.getFacultyDiaryByDeptId(DepId, dtNow).ToList();
                        return Json(fd, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }

                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }




        [HttpPost]
        public ActionResult Work_Diary_Pending(DateTime dtDate)
        {

            if (Session["Log_Id"] != null)
            {
                try
                {
                    int DepId = Convert.ToInt32(Session["DepId"].ToString());
                    int FactId = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    string role = Session["Role"].ToString();
                    IEnumerable<Schedule> objFact = objfaculty.Get_Faculty(FactId);
                    int desId = objFact.Select(x => x.Des_Id).FirstOrDefault();
                    if (desId == 5 || role == "HOD")
                    {
                        List<Faculty_Work_Diary> fd = objfaculty.getPendingFacultyTaskList(DepId, dtDate).ToList();
                        return Json(fd, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }

                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }


        [HttpPost]
        public ActionResult WorkDiaryApprove(DateTime dtTaskDate)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    int DepId = Convert.ToInt32(Session["DepId"].ToString());
                    int FactId = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    string role = Session["Role"].ToString();
                    IEnumerable<Schedule> objFact = objfaculty.Get_Faculty(FactId);
                    int desId = objFact.Select(x => x.Des_Id).FirstOrDefault();
                    if (desId == 5 || role == "HOD")
                    {
                        List<Faculty_Work_Diary> fd = objfaculty.getFacultyDiaryByDeptId(DepId, dtTaskDate).ToList();
                        return Json(fd, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }
                }
                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");
            }
        }
        public ActionResult Work_Approval_Details()
        {

            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (Request.QueryString["Id"] != null)
                    {
                        int FactId = Convert.ToInt32(Session["Faculty_Id"].ToString());
                        IEnumerable<Schedule> objFact = objfaculty.Get_Faculty(FactId);
                        int desId = objFact.Select(x => x.Des_Id).FirstOrDefault();
                        string role = Session["Role"].ToString();
                        if (desId == 5 || role == "HOD")
                        {
                            int WorkId = Convert.ToInt32(Request.QueryString["Id"]);
                            @ViewBag.Id = WorkId;
                            Faculty_Work_Diary lst = objfaculty.getWorkById(WorkId);
                            ViewBag.Name = lst.Name;
                            ViewBag.TaskDate = lst.Work_Date;
                            List<Faculty_Teaching_Hours> lstWork = objfaculty.GetWorkDetailsByIdApprove(WorkId).ToList();
                            return View(lstWork);
                        }
                        else
                        {
                            return Redirect("Faculty_Login");
                        }


                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }

                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");
            }
        }

        [HttpPost]
        public ActionResult SendNotification(List<Faculty_Work_Diary> selectedItems, string Taskdate)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    foreach (var data in selectedItems)
                    {
                        int Faculty_Id = data.Faculty_Id;
                        CMS_Faculty objFac = objfaculty.GetMobile(Faculty_Id);
                        string FMobile = objFac.Mobile;
                        string FName = objFac.Name;
                        FMobile = "9496940261";
                        string message = "Dear " + FName + ", Add Task for " + Taskdate;
                        bool status = objsmsservice.SendSMS(FMobile, message);
                        objfaculty.AddTaskNotification(Faculty_Id, Taskdate);
                    }
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(-1, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Redirect("Faculty_Login");

        }


        [HttpPost]
        public ActionResult ApproveTask(int TaskId, string Comments)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    Faculty_Work_Diary lst = objfaculty.getWorkById(TaskId);
                    //int ApproveStatus = lst.Status;
                    //if (ApproveStatus == 2)
                    //{
                    //    return Json(3, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{

                    int FactId = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    string role = Session["Role"].ToString();
                    IEnumerable<Schedule> objFact = objfaculty.Get_Faculty(FactId);
                    int desId = objFact.Select(x => x.Des_Id).FirstOrDefault();
                    if (desId == 5 || role == "HOD")
                    {
                        int val = objfaculty.ApproveTask(FactId, TaskId, Comments);
                        return Json(val, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }

                    // }

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");
            }

        }



        //faculty diary view
        public ActionResult facultydiary()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.Department = new SelectList(objfaculty.getDepartmentDeatails(), "Dep_Id", "Department");

                    List<Faculty_Work_Diary> diary = new List<Faculty_Work_Diary>();
                    diary = objfaculty.getFacultyDiary().ToList();
                    return View(diary);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }


            //return View();
        }
        [HttpPost]
        public ActionResult Search_Faculty_Details(int depId, string fromDate, string toDate)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (depId == 25)
                        depId = 0;
                    List<Faculty_Work_Diary> sch = new List<Faculty_Work_Diary>();
                    if (fromDate != null && toDate != null && depId != 0)
                    {
                        sch = objfaculty.Get_FacultyDiarybyDate(depId, fromDate, toDate).ToList();
                    }
                    else if (fromDate != null && toDate != null && depId == 0)
                    {
                        sch = objfaculty.Get_FacultyDiarybyDate(fromDate, toDate).ToList();
                    }
                    else if (depId != 0)
                    {
                        sch = objfaculty.Get_FacultyDiary(depId).ToList();
                    }
                    else
                    {
                        sch = objfaculty.getFacultyDiary().ToList();
                    }
                    return Json(sch, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult Search_Faculty_Details_ByDate(int depId, string dtFromDate, string dtToDate)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (depId == 25)
                        depId = 0;
                    List<Faculty_Work_Diary> sch = new List<Faculty_Work_Diary>();
                    if (depId != 0)
                    {
                        sch = objfaculty.Get_FacultyDiarybyDate(depId, dtFromDate, dtToDate).ToList();
                    }
                    else
                    {
                        sch = objfaculty.Get_FacultyDiarybyDate(dtFromDate, dtToDate).ToList();
                    }
                    return Json(sch, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult getModuleDetails(int Course_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<CMS_Module_Details> lstModule = new List<CMS_Module_Details>();
                    lstModule = objfaculty.GetModuleDetails(Course_Id).ToList();


                    return Json(lstModule, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }


        public ActionResult getSubModuleDetails(int Course_Id, int ModuleId)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<CMS_Module_Details> lstModule = new List<CMS_Module_Details>();
                    lstModule = objfaculty.GetSubModuleDetails(Course_Id, ModuleId).ToList();


                    return Json(lstModule, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult Work_Diary_Principal()
        {
            ViewBag.Department = new SelectList(objfaculty.getDepartmentDeatails(), "Dep_Id", "Department");
            CMS_Admission.Areas.COE.DAL.DALCOE coe = new CMS_Admission.Areas.COE.DAL.DALCOE();
            ViewBag.Programmes_Type = new SelectList(coe.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");

            //List<Faculty_Work_Diary> diary = new List<Faculty_Work_Diary>();
            //diary = objfaculty.getFacultyDiary().ToList();
            //return View(diary);
            return View();
        }

        [HttpPost]
        public ActionResult Get_Faculty_WorkDetails_PView(int depId)
        {
            List<Faculty_Work_Diary> sch = new List<Faculty_Work_Diary>();
            if (Session["Log_Id"] != null)
            {
                try
                {

                    if (depId != -1)
                    {
                        sch = objfaculty.Get_Faculty_WorkDetails_PView(depId).ToList();
                        return Json(sch, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(0, JsonRequestBehavior.AllowGet);
                    }

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Redirect("Faculty_Login");
            }

        }

        public ActionResult Search_SemesterByPgmType(int Pgm_Type_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Notification> students = new List<Notification>();

                    students = objfaculty.get_Semester(Pgm_Type_Id).ToList();


                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult getProgrammess(int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Programme> P = new List<Programme>();

                    P = objfaculty.Search_Semester_Pgm(Acc_Yr_Sem_Id).ToList();
                    return Json(P, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");


            }
        }

        [HttpPost]
        public ActionResult getFacultyWorkDetailsByPgm(int Acc_Yr_Sem_Id, int Pgm_Id, int Pgm_Type_Id)
        {
            List<Faculty_Work_Diary> sch = new List<Faculty_Work_Diary>();
            if (Session["Log_Id"] != null)
            {
                try
                {

                    if (Acc_Yr_Sem_Id != -1 && Pgm_Id != -1)
                    {
                        sch = objfaculty.getFacultyWorkDetailsByPgm(Acc_Yr_Sem_Id, Pgm_Id).ToList();
                    }
                    else
                    {
                        sch = objfaculty.getFacultyWorkDetailsByPgms(Acc_Yr_Sem_Id, Pgm_Type_Id, Pgm_Id).ToList();
                    }
                    return Json(sch, JsonRequestBehavior.AllowGet);


                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Redirect("Faculty_Login");
            }

        }


        //Work Diary HOD view

        public ActionResult Work_Diary_Report_HOD()
        {

            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<SelectListItem> years = objfaculty.getMonths();
                    int Dep_Id = Convert.ToInt32(Session["DepId"].ToString());
                    int month = DateTime.Today.Month;
                    ViewBag.Faculty = objfaculty.getFacultyWorkReport(Dep_Id, month).ToList();
                    return View(years);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult getFacultyWiseReport(int Month_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Dep_Id = Convert.ToInt32(Session["DepId"].ToString());
                    List<Faculty_Work_Diary> workDiary = objfaculty.getFacultyWorkReport(Dep_Id, Month_Id).ToList();
                    return Json(workDiary, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult getProgrammeWiseReport(int Month_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Dep_Id = Convert.ToInt32(Session["DepId"].ToString());
                    List<Faculty_Work_Diary> workDiary = objfaculty.getProgrammeWiseReport(Dep_Id, Month_Id).ToList();
                    return Json(workDiary, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Graph_Report()
        {

            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<SelectListItem> months = objfaculty.getAcademicYears();
                    ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type", 1);
                    ViewBag.Semester = new SelectList(objfaculty.GetSemester(), "Acc_Yr_Sem_Id", "Semester");
                    return View(months);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult getGraphData(int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    List<Faculty_Work_Diary> workDiary = objfaculty.getGraphData(Acc_Yr_Sem_Id).ToList();
                    return Json(workDiary, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult approveWork(int Month_Id, int Faculty_Id, int Course_Sem_Id, string Remark)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid approved_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.approveWork(Month_Id, Faculty_Id, Course_Sem_Id, Remark, approved_By);
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult getRemarks(int Month_Id, int Faculty_Id, int Course_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid approved_By = new Guid(Session["Log_Id"].ToString());
                    List<Faculty_Work_Diary> workDiary = objfaculty.getRemarks(Month_Id, Faculty_Id, Course_Sem_Id).ToList();
                    return Json(workDiary, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult approveWorkAll(int Month_Id, string[] Faculty)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid approved_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.approveWorkAll(Month_Id, Faculty, approved_By);
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult print_Report(int depId)
        {

            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "FacultyDiaryReport.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Index");
            }
            if (depId != -1)
            {



                List<Faculty_Work_Diary> stud = objfaculty.Get_Faculty_Report_PView(depId).ToList();
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DSFacultyDiary";
                reportDataSource.Value = stud;
                lr.DataSources.Add(reportDataSource);



                ReportParameterCollection reportparameter = new ReportParameterCollection();
                // string ExamName = stud.Select(x => x.Exam_Name).FirstOrDefault().ToUpper();
                // string ExamDate = stud.Select(x => x.Exam_Start_Date).FirstOrDefault().ToString("MMMM") + " " + stud.Select(x => x.Exam_Start_Date.Year).FirstOrDefault();
                //string ExamType = stud.Select(x => x.Exam_Type).FirstOrDefault().ToUpper() + " EXAMINATION";
                //reportparameter.Add(new ReportParameter("ExamName", ExamName));
                //reportparameter.Add(new ReportParameter("ExamDate", ExamDate));
                //reportparameter.Add(new ReportParameter("ExamType", ExamType));

                //lr.EnableExternalImages = true;
                //string FilePath = @"file:\" + AppDomain.CurrentDomain.BaseDirectory + "\\" + "Images\\Student_Photo\\"; //Application.StartupPath is for WinForms, you should try AppDomain.CurrentDomain.BaseDirectory  for .net
                //reportparameter.Add(new ReportParameter("ImgPath", FilePath));
                //lr.SetParameters(reportparameter);

                //  lr.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessing);

                lr.Refresh();

                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;
                string deviceInfo =

                    "<DeviceInfo>" +

                    "<OutputFormat>" + reportType + "</OutputFormat>" +

                    "<PageWidth>8.5in</PageWidth>" +

                    "</DeviceInfo>";


                Warning[] warning;
                string[] streams;
                byte[] renderedBytes;

                renderedBytes = lr.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warning);
                //  return File(renderedBytes, "pdf");
                return File(renderedBytes, mimeType);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }



        }

        [HttpPost]
        public ActionResult Delete_WorkDetails(int Work_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid deleted_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.deleteWork(Work_Id, deleted_By);
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region StudentAttendance

        public ActionResult AttendanceStudent(int AccYrSemPgmId)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.AccYrSemPgmId = AccYrSemPgmId;
                    ViewBag.Faculty_Id = Session["Faculty_Id"].ToString();
                    int Fac_Id = Convert.ToInt32(Session["Faculty_Id"]);
                    ViewBag.Dep = db.CMS_Facultys.Where(x => x.Active_Status == true && x.Faculty_Id == Fac_Id).Select(x => x.Dep_Id).FirstOrDefault();
                    ViewBag.Course = new SelectList(objfaculty.Search_Courses(), "Cc_Id", "Course_Name");
                    ViewBag.Semester = new SelectList(objfaculty.Search_Semester(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Language = new SelectList(objfaculty.getAllcommonlanguage(), "Cl_Id", "Common_Language");
                    ViewBag.Date = DateTime.Now;
                    ViewBag.Programmes_Type = new SelectList(objfaculty.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Hour = new SelectList(objfaculty.getHour(), "Hour_Id", "Hour");
                    ViewBag.Day = new SelectList(objfaculty.getDay(), "Day_Id", "Day");
                    ViewBag.Academic_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");
                    ViewBag.SecondLanguage = new SelectList(objfaculty.Search_SecondLanguage(), "Lang_Id", "Language");
                    ViewBag.Course = new SelectList(objfaculty.Search_Courses(), "Cc_Id", "Course_Name");
                    ViewBag.Semester = new SelectList(objfaculty.Search_Semester(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Language = new SelectList(objfaculty.getAllcommonlanguage(), "Cl_Id", "Common_Language");
                    DateTime today = DateTime.Today;

                    if (today.DayOfWeek != DayOfWeek.Sunday)
                    {
                        ViewBag.Date = today.ToString("dd-MM-yyyy");
                        ViewBag.Today = objfaculty.getdate(DateTime.Today);
                    }
                    ViewBag.Sem = new SelectList(objfaculty.Search_Sem(), "Acc_Yr_Sem_Id", "Semester");
                    //ViewBag.Hour = new SelectList(objfaculty.getHour(), "Hour_Id", "Hour");
                    //ViewBag.Day = new SelectList(objfaculty.getDay(), "Day_Id", "Day");
                    ViewBag.DisabledDates = objfaculty.getHolidays(DateTime.Today);
                    ViewBag.SecondLang = new SelectList(objfaculty.Search_SecondLanguage(), "Lang_Id", "Language");
                    //ViewBag.Academic_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");


                    // int SelectedItem = 0;
                    IEnumerable<Semesters> lstCourses = objfaculty.getProgrammeCoursebySem(AccYrSemPgmId, Fac_Id);
                    //List<Semesters> dut = new List<Semesters>();
                    //var y = new Semesters()
                    //{
                    //    Course_Sem_Id = 0,
                    //    Course_Name = "---Select---"
                    //};
                    //dut.Add(y);
                    //lstCourses = lstCourses.Union(dut);
                    ViewBag.AssignedCourses = new SelectList(lstCourses, "Course_Sem_Id", "Course_Name");

                    ViewBag.PgmType = (from a in db.CMS_AcademicYr_Sem_Programmes
                                       join b in db.CMS_Programmes on a.Pgm_Id equals b.Pgm_Id
                                       where a.Acc_Yr_Sem_Pgm_Id == AccYrSemPgmId
                                       select b.Pgm_Type_Id).FirstOrDefault();

                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult Search_course_studentss(int CourseSemId, DateTime date, int Hour)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Programme> students = new List<Programme>();
                    int FacId = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    students = objfaculty.Search_course_studentss(CourseSemId, date, Hour, FacId).ToList();
                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult Add_Attendance_Student(Programme p)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int FacId = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    int retVal = objfaculty.Add_Attendance_Student(p, Created_By, FacId);
                    return Json(retVal, JsonRequestBehavior.AllowGet);


                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }


        //  public ActionResult Delete_Attendance_Hour(string Hours, string date, int Course_Sem_Id, int Acc_Yr_Sem_Pgm_Id)
        public ActionResult Delete_Attendance_Hour(Programme p)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    int FacId = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    int retVal = objfaculty.Delete_Attendance_Hour(p.Hours, p.Date.ToString(), p.Course_Sem_Id, p.Acc_Yr_Sem_Pgm_Id, Created_By, FacId);
                    return Json(retVal, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }


        public ActionResult Search_Single_Attendance_stud(int Course_Sem_Id, int Acc_Yr_Sem_Pgm_Id, int Hour, DateTime date)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Programme> students = new List<Programme>();
                    int FacId = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    students = objfaculty.Search_Course_Attendance(Course_Sem_Id, Acc_Yr_Sem_Pgm_Id, Hour, date, FacId).ToList();


                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult Faculty_Main_Home()
        {
            return View();
        }
        public ActionResult Faculty_Main_Home_Faculty()
        {
            return View();
        }
        public ActionResult Faculty_Main_Home_Classwarden()
        {
            return View();
        }
        public ActionResult Faculty_Dashboard()
        {
            int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
            ViewBag.Faculty_Id = Faculty_Id;
            var sh = 0; ViewBag.HOD = 0;
            CMS_AcademicYear ac = objfaculty.getCurrentAcademicYear();
            ViewBag.pCount = objfaculty.get_Student_CRequts(1, Faculty_Id, 1).Count();
            var splfac = db.CMS_SplAtt_Facultys.Where(x => x.Faculty_Id == Faculty_Id && x.Active_Status == true).Count();
            var hod = db.CMS_HODs.Where(x => x.Faculty_Id == Faculty_Id && x.Acc_Yr == ac.Acc_yr_Id && x.Active_Status == true).Count();
            if (hod > 0 || splfac > 0)
            {
                ViewBag.Spl = 1;
            }
            return View();
        }
        public ActionResult programmewise_Students_Attendance_Report()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    List<AttendanceReport> intrnl = objfaculty.getProgrammesFacultyWise(Faculty_Id).ToList();
                    ViewBag.Programme_Type = new SelectList(objfaculty.getProgrammesFacultyWise(Faculty_Id), "Pgm_Type_Id", "Programme_Type");
                    return View();

                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult studentAttendanceRegister_Report()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    ViewBag.Programme_Type = new SelectList(objfaculty.getProgrammeTypeFacultyWise(Faculty_Id), "Pgm_Type_Id", "Pgm_Type");
                    DateTime dt = DateTime.Now;
                    ViewBag.Year = db.CMS_AcademicYears.Where(x => x.Start_Date <= dt && x.End_Date >= dt).Select(x => x.Acc_yr_Id).FirstOrDefault();
                    // ViewBag.UPRN_No = new SelectList(objfaculty.getuprn(), "UPRN", "UPRN");
                    ViewBag.Sem = new SelectList(objfaculty.Search_Semss(), "Acc_Yr_Sem_Id", "Semester");
                    ViewBag.Academic_Year = new SelectList(objfaculty.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");
                    return View();
                }
                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }
        }

        public ActionResult getClassFacultyWise(int Pgm_Type_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    List<AttendanceReport> classes = new List<AttendanceReport>();
                    classes = objfaculty.getClassFacultyWise(Faculty_Id, Pgm_Type_Id).ToList();
                    return Json(classes, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult getPgmFacultyWise(int Pgm_Type_Id, int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    List<AttendanceReport> pgms = new List<AttendanceReport>();
                    pgms = objfaculty.getPgmFacultyWise(Faculty_Id, Pgm_Type_Id, Acc_Yr_Sem_Id).ToList();
                    int Acc_Yr_Sem_Pgm_Id = pgms[0].Acc_Yr_Sem_Pgm_Id;
                    Session["Acc_Yr_Sem_Pgm_Id"] = Acc_Yr_Sem_Pgm_Id.ToString();
                    return Json(pgms, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult getStudentFacultyWise(int Acc_Yr_Sem_Pgm_Id)
        {

            if (Session["Log_Id"] != null)
            {
                try
                {

                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    int Acc_Yr_Sem_Pgm_ID = Acc_Yr_Sem_Pgm_Id;

                    List<Student> students = new List<Student>();

                    students = objAtt.Search_Semester_Students(Acc_Yr_Sem_Pgm_ID).ToList();


                    return Json(students, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult getStudentProgramme_Attendance(int Acc_Yr_Sem_Pgm_Id, string firstDay, string lastDay, string UPRN)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    // List<AttendanceReport> att = new List<AttendanceReport>();
                    // int Acc_Yr_Sem_Pgm_Id = Convert.ToInt32(Session["Acc_Yr_Sem_Pgm_Id"].ToString());
                    //List<AttendanceReport> rep = objfaculty.getProgramme_Attendance(Acc_Yr_Sem_Pgm_Id, firstDay, lastDay).ToList();
                    List<AttendanceReport> rep = objfaculty.getStudentProgramme_Attendance(Acc_Yr_Sem_Pgm_Id, firstDay, lastDay, UPRN).ToList();
                    ViewBag.PgmType = (from a in db.CMS_AcademicYr_Sem_Programmes
                                       join b in db.CMS_Programmes on a.Pgm_Id equals b.Pgm_Id
                                       where a.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Sem_Pgm_Id
                                       select b.Pgm_Type_Id).FirstOrDefault();
                    if (rep.Count > 0)
                    {
                        ViewBag.Dates = rep[0].WorkingDates.ToList();
                    }
                    return View(rep);
                }
                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult programmeAttendanceRegister_Report()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    ViewBag.Programme_Type = new SelectList(objfaculty.getProgrammeTypeFacultyWise(Faculty_Id), "Pgm_Type_Id", "Pgm_Type");
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        #endregion



        public ActionResult ViewFormA_Updated(int Course_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    var item = (from a in db.CMS_Course_Semesters
                                join b in db.CMS_Courses on a.Course_Id equals b.Course_Id
                                join c in db.CMS_AcademicYr_Sem_Programmes on a.Acc_Yr_Sem_Pgm_Id equals c.Acc_Yr_Sem_Pgm_Id
                                join d in db.CMS_AccademicYearSemesters on c.Acc_Yr_sem_Id equals d.Acc_Yr_Sem_Id
                                join e in db.CMS_Course_Teachers on a.Course_Sem_Id equals e.Course_Sem_Id
                                where a.Course_Sem_Id == Course_Sem_Id && e.Active_Status == true && e.Paper_In_Charge == true
                                select new { a.Course_Sem_Id, a.Course_Nature_Type, b.Pgm_Type_Id, a.Acc_Yr_Sem_Pgm_Id, c.Acc_Yr_sem_Id, d.Acc_yr_Id, e.Faculty_Id, c.Pgm_Id }).FirstOrDefault();
                    ViewBag.Course_Sem_Id = item.Course_Sem_Id;
                    ViewBag.Acc_Yr_Sem_Pgm_Id = item.Acc_Yr_Sem_Pgm_Id;
                    ViewBag.Acc_yr_Id = item.Acc_yr_Id;
                    ViewBag.Faculty_Id = item.Faculty_Id;
                    int Ass_Types = (from a in db.CMS_InternalTypes
                                     join b in db.CMS_InternalAssesments on a.Int_TYpe_Id equals b.Int_Type_Id
                                     join c in db.CMS_AssesmentTypes on b.Ass_Type_Id equals c.Ass_Type_Id
                                     where a.Type == item.Course_Nature_Type.Trim() && a.Active_Status == true && a.Pgm_Type_Id == item.Pgm_Type_Id
                                     && b.Active_Status == true
                                     select new
                                     {
                                         b.Int_Ass_Id
                                     }).Distinct().Count();
                    int total = dbExam.CMS_Internal_Marks.Where(x => x.Acc_Yr_Sem_Pgm_Id == item.Acc_Yr_Sem_Pgm_Id && x.Course_Sem_Id == Course_Sem_Id && x.Active_Status == true).Select(x => x.Int_Ass_Id).Distinct().Count();
                    Boolean AForm_Status = false;
                    if (Ass_Types == total)
                        AForm_Status = true;
                    ViewBag.AForm_Status = AForm_Status;

                    CMS_Internal_MarkEntry_Schedule sch = db.CMS_Internal_MarkEntry_Schedules.Where(x => x.Acc_Yr_Sem_Id == item.Acc_Yr_sem_Id && x.Active_Status == true).FirstOrDefault();
                    Boolean Status = false;

                    if (sch != null)
                    {
                        if (sch.Start_Date <= DateTime.Now && sch.End_Date >= DateTime.Now)
                        {
                            Status = true;
                        }
                        if (item.Pgm_Id == 25 || item.Pgm_Id == 26 || item.Pgm_Id == 27)
                        {
                            Status = true;
                        }
                    }
                    ViewBag.Status = Status;
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());

                    List<Internals> intrnl = new List<Internals>();
                    //  intrnl = objfaculty.ViewFormA(Course_Sem_Id).ToList();

                    intrnl = objfaculty.ViewFormAUpdated(Course_Sem_Id, item.Faculty_Id, Created_By).ToList();
                    //objfaculty.UpdateAttendance(Course_Sem_Id, item.Faculty_Id, Created_By);
                    return View(intrnl);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult Add_Internal_Mark_Updated(int Acc_Yr_Sem_Pgm_Id, int Course_Sem_Id, int Int_Ass_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Internals> intrnl = objfaculty.getInternalMark_Updated(Acc_Yr_Sem_Pgm_Id, Course_Sem_Id, Int_Ass_Id).ToList();
                    var q = (from a in db.CMS_Course_Semesters
                             join b in db.CMS_Courses on a.Course_Id equals b.Course_Id
                             join c in db.CMS_AcademicYr_Sem_Programmes on a.Acc_Yr_Sem_Pgm_Id equals c.Acc_Yr_Sem_Pgm_Id
                             join d in db.CMS_AccademicYearSemesters on c.Acc_Yr_sem_Id equals d.Acc_Yr_Sem_Id
                             join e in db.CMS_Semesters on d.Sem_Id equals e.Sem_Id
                             join f in db.CMS_Programmes on c.Pgm_Id equals f.Pgm_Id
                             where a.Course_Sem_Id == Course_Sem_Id
                             select new
                             {
                                 b.Course_Code,
                                 b.Course_Name,
                                 e.Semester,
                                 f.Programme
                             }).FirstOrDefault();
                    if (q != null)
                    {
                        ViewBag.Course_Code = q.Course_Code;
                        ViewBag.Course_Name = q.Course_Name;
                        ViewBag.Semester = q.Semester;
                        ViewBag.Programme = q.Programme;
                    }
                    ViewBag.Assesment = (from a in db.CMS_InternalAssesments
                                         join b in db.CMS_AssesmentTypes on a.Ass_Type_Id equals b.Ass_Type_Id
                                         where a.Int_Ass_Id == Int_Ass_Id && a.Active_Status == true && b.Active_Status == true
                                         select b.AssesmentType).FirstOrDefault();
                    ViewBag.Ass_Mark = (from a in db.CMS_InternalAssesments
                                        where a.Int_Ass_Id == Int_Ass_Id && a.Active_Status == true
                                        select a.Max_Mark).FirstOrDefault();
                    ViewBag.Acc_Yr_Sem_Pgm_Id = Acc_Yr_Sem_Pgm_Id;
                    ViewBag.Course_Sem_Id = Course_Sem_Id;
                    ViewBag.Int_Ass_Id = Int_Ass_Id;
                    ViewBag.Int_Type_Id = db.CMS_InternalAssesments.Where(x => x.Active_Status == true && x.Int_Ass_Id == Int_Ass_Id).Select(x => x.Int_Type_Id).FirstOrDefault(); ;
                    // ViewBag.Total = intrnl.Select(x => x.MaxMark).FirstOrDefault();
                    var MarkDetails = intrnl.Select(x => x.MarkDefinition).FirstOrDefault();
                    ViewBag.Total = MarkDetails;



                    if (ViewBag.Total.Count == 0 && ViewBag.Assesment == "Attendance")
                    {
                        ViewBag.AttTotal = intrnl.Select(x => x.Entered_Max_Mark).FirstOrDefault();
                    }


                    return View(intrnl);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult getMaxMarkAttendance(int Int_Type_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    int maxmark = db.CMS_InternalAssesments.Where(x => x.Int_Type_Id == Int_Type_Id && x.Active_Status == true && x.Ass_Type_Id == 1).Select(x => x.Max_Mark).FirstOrDefault();

                    return Json(maxmark, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult getProgrammeFacultyWise(int Acc_Yr_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    List<AttendanceReport> pgms = objfaculty.getProgrammeFacultyWise(Faculty_Id, Acc_Yr_Sem_Id).ToList();
                    return Json(pgms, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        #region Mentor
        public ActionResult MentorList()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    CMS_AcademicYear accyr = objfaculty.getCurrentAcademicYear();
                    ViewBag.CurrentYr = accyr.Year;
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    string Mobile = Session["Mobile"].ToString();

                    int coordinatorStatus = db.CMS_Logins.Where(x => x.Mobile == Mobile && x.Role_Id == 60 && x.Active_Status == true).Count();
                    ViewBag.coordinatorStatus = coordinatorStatus;

                    List<Student> request = objfaculty.getM_Stunds(Faculty_Id).ToList();
                    return View(request);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult ViewAllMentorList()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    List<Student> request = objfaculty.getM_Stunds(0).OrderBy(x => x.Programme).ThenBy(x => x.Class).ToList();
                    return View(request);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Add_New_Meeting(int Id, string Name, int Meeting_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    CMS_Mentor_Meeting meet = db.CMS_Mentor_Meetings.Where(x => x.Meeting_Id == Meeting_Id).FirstOrDefault();
                    if (meet == null)
                    {
                        meet = new CMS_Mentor_Meeting
                        {
                            Meeting_Id = 0,
                            Mentor_Id = 0,
                            Time = null,
                            Issue_Raised = null,
                            Decisions_Taken = null,
                            Next_Meeting_Date = null,
                            Created_Date = DateTime.Now,
                            Active_Status = true
                        };
                        ViewBag.ID = Id;
                        ViewBag.Name = Name;
                    }
                    else
                    {
                        ViewBag.ID = meet.Mentor_Id;
                        ViewBag.Name = (from a in db.CMS_Mentors
                                        join b in db.CMS_UPRNs on a.UPRN equals b.UPRN
                                        join c in db.CMS_Students on b.Admission_No equals c.Admission_No
                                        where a.Id == meet.Mentor_Id
                                        select c.Name).FirstOrDefault();
                    }

                    return View(meet);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");
            }
        }
        [HttpPost]
        public ActionResult Add_Meeting(CMS_Mentor_Meeting meet)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid Log_Id = new Guid(Session["Log_Id"].ToString());
                    meet.Created_By = Log_Id;
                    objfaculty.add_Meeting_Details(meet);
                    return Json(1, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        public ActionResult View_Meeting(string UPRN)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Student> list = objfaculty.getMeetingDetails(UPRN).ToList();
                    return Json(list, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        public ActionResult delete_Meeting(int Meeting_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    CMS_Mentor_Meeting meet = db.CMS_Mentor_Meetings.Where(x => x.Meeting_Id == Meeting_Id).FirstOrDefault();
                    meet.Active_Status = false;
                    db.SaveChanges();
                    return Json(1, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult MentoringRecord(string UPRN)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    int Admsn_No = db.CMS_UPRNs.Where(x => x.UPRN == UPRN && x.Active_Status == true).Select(x => x.Admission_No).FirstOrDefault();
                    LocalReport lr = new LocalReport();
                    string path = Path.Combine(Server.MapPath("~/Report"), "MentoringRecord.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Admission_Register");
                    }
                    List<CMS_Student> stud = new List<CMS_Student>();
                    stud = db.CMS_Students.Where(x => x.Admission_No == Admsn_No).ToList();
                    var ccyr = DateTime.Now.Year;
                    CMS_AcademicYear Acc_Yrr = objfaculty.getCurrentAcademicYear();
                    var currntyr = DateTime.Now.Year.ToString();
                    var nxt = ccyr + 1;
                    var nxtyr = nxt.ToString();
                    var pgmid = db.CMS_Students.Where(x => x.Admission_No == Admsn_No).Select(x => x.Pgm_Id).FirstOrDefault();
                    var pgmm = db.CMS_Programmes.Where(x => x.Pgm_Id == pgmid).Select(x => x.Programme).FirstOrDefault();
                    var dis_Id = db.CMS_Students.Where(x => x.Admission_No == Admsn_No).Select(x => x.Per_Dis_Id).FirstOrDefault();
                    var district = db.CMS_Districts.Where(x => x.Dis_Id == dis_Id).Select(x => x.District).FirstOrDefault();
                    var pgmTypeId = db.CMS_Programmes.Where(x => x.Pgm_Id == pgmid).Select(x => x.Pgm_Type_Id).FirstOrDefault();
                    var Acc_Yr_Id = db.CMS_Students.Where(x => x.Admission_No == Admsn_No).Select(x => x.Acc_Yr_Id).FirstOrDefault();
                    var batId = db.CMS_Batchs.Where(x => x.Acc_Yr_Id == Acc_Yr_Id && x.Pgm_Type_Id == pgmTypeId).Select(x => x.Batch).FirstOrDefault();
                    var facId = db.CMS_Mentors.Where(x => x.UPRN == UPRN && x.Acc_Yr_Id == Acc_Yrr.Acc_yr_Id).Select(x => x.Fac_Id).FirstOrDefault();
                    var mentor = db.CMS_Facultys.Where(x => x.Faculty_Id == facId).Select(x => x.Name).FirstOrDefault();
                    var desId = db.CMS_Facultys.Where(x => x.Faculty_Id == facId).Select(x => x.Des_Id).FirstOrDefault();
                    var Des = db.CMS_Designations.Where(x => x.DesignationId == desId).Select(x => x.Designation_name).FirstOrDefault();
                    var depid = db.CMS_Facultys.Where(x => x.Faculty_Id == facId).Select(x => x.Dep_Id).FirstOrDefault();
                    var Dep = db.CMS_Departments.Where(x => x.Dep_Id == depid).Select(x => x.Department).FirstOrDefault();
                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "DataSet1";
                    reportDataSource.Value = stud;
                    lr.DataSources.Add(reportDataSource);
                    lr.EnableExternalImages = true;

                    List<Student> list = objfaculty.getMeetingDetails(UPRN).ToList();
                    ReportDataSource reportDataSource1 = new ReportDataSource();
                    reportDataSource1.Name = "DataSet2";
                    reportDataSource1.Value = list;
                    lr.DataSources.Add(reportDataSource1);

                    string FilePath = @"file:\" + AppDomain.CurrentDomain.BaseDirectory + "Images\\Student_Photo\\";
                    List<ReportParameter> paraList = new List<ReportParameter>();

                    string photo = db.CMS_Students.Where(x => x.Admission_No == Admsn_No).Select(x => x.Photo).FirstOrDefault();
                    paraList.Add(new ReportParameter("ImgPath", FilePath + photo));
                    paraList.Add(new ReportParameter("photo", photo));
                    paraList.Add(new ReportParameter("year1", currntyr));
                    paraList.Add(new ReportParameter("year2", nxtyr));
                    paraList.Add(new ReportParameter("pgm", pgmm));
                    paraList.Add(new ReportParameter("district", district));
                    paraList.Add(new ReportParameter("uprn", UPRN));
                    paraList.Add(new ReportParameter("batch", batId));
                    paraList.Add(new ReportParameter("mentor", mentor));
                    paraList.Add(new ReportParameter("Dep", Des + " - " + Dep));
                    lr.SetParameters(paraList.ToArray());
                    string reportType = "PDF";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;
                    string deviceInfo =
                        "<DeviceInfo>" +
                        "<OutputFormat>" + reportType + "</OutputFormat>" +
                        "<PageWidth>8.5in</PageWidth>" +
                        "</DeviceInfo>";
                    Warning[] warning;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = lr.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warning);
                    //  return File(renderedBytes, "pdf");
                    return File(renderedBytes, mimeType);
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        #endregion



        #region GoogleScholar
        public void LoadGscholarProfile()
        {
            if (Session["Log_Id"] != null)
            {
                Guid Log_Id = new Guid(Session["Log_Id"].ToString());
                int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                objfaculty.Add_Scholar_Json(Log_Id, Faculty_Id);
            }

        }

        public void LoadGscholarProfile1()
        {
            if (Session["Log_Id"] != null)
            {
                Guid Log_Id = new Guid(Session["Log_Id"].ToString());
                using (StreamReader r = new StreamReader(@"C:\Users\user\Desktop\\vibinipe2\\vibinipe2.json"))
                {
                    string json = r.ReadToEnd();
                    dynamic array = JsonConvert.DeserializeObject(json);


                    //author Id
                    var parm = array["search_parameters"];
                    string author_id = parm["author_id"];

                    //author Link
                    var metadata = array["search_metadata"];
                    string authlink = metadata["google_scholar_author_url"];

                    //author
                    var author = array["author"];
                    var name = author["name"];
                    var affiliations = author["affiliations"];
                    var email = author["email"];
                    var interests = author["interests"];


                    CMS_GoogleScholar Exist = db.CMS_GoogleScholars.Where(x => x.Author_Id == author_id).FirstOrDefault();
                    if (Exist == null)
                    {
                        CMS_GoogleScholar Sch = new CMS_GoogleScholar();
                        int Scholar_Id = db.CMS_GoogleScholars.Select(x => x.Scholar_Id).Max();
                        Sch.Scholar_Id = Scholar_Id + 1;
                        // Sch.Scholar_Id = 1;
                        Sch.Author_Name = name;
                        Sch.Author_Id = author_id;
                        Sch.Affiliations = affiliations;
                        Sch.Email = email;
                        Sch.Link = authlink;
                        Sch.Active_Status = true;
                        Sch.Created_by = Log_Id;
                        Sch.Created_On = DateTime.Now;
                        db.CMS_GoogleScholars.Add(Sch);
                        db.SaveChanges();
                    }

                    //articles
                    var articles = array["articles"];

                    foreach (var item in articles)
                    {
                        var title = item["title"];
                        var authors = item["authors"];
                        var publication = item["publication"];
                        var link = item["link"];
                        var cited_by = item["cited_by"];
                        var value = cited_by["value"];
                        //CMS_GoogleScholarArticle Articles = db.CMS_GoogleScholarArticles.Where(x => x.Scholar_Id == author_id).FirstOrDefault();
                        //if (Articles == null)
                        //{
                        //}
                    }
                }
            }
        }

        public ActionResult GScholar()
        {
            try
            {
                List<GoogleScholar> Scholar = objfaculty.getGoogleScholarDetails("Q9QgjcEAAAAJ");
                return View(Scholar);
            }
            catch
            {

                return Redirect("~/Login/Error_Page");
            }

        }

        [HttpPost]
        public ActionResult Add_Articles(CMS_GoogleScholarArticle Articles)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    Guid Log_Id = new Guid(Session["Log_Id"].ToString());
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    objfaculty.Add_Scholar_Articles(Log_Id, Faculty_Id, Articles);
                    return Json(1, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult Delete_Article(int Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    Guid Log_Id = new Guid(Session["Log_Id"].ToString());
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    objfaculty.Delete_Articles(Log_Id, Faculty_Id, Id);
                    return Json(1, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region CareerProgression

        public ActionResult Progression()
        {
            if (Session["Log_Id"] == null)
                return Redirect("~/Login/Login");

            try
            {
                int facultyId = Convert.ToInt32(Session["Faculty_Id"]);
                string role = Session["Role"].ToString(); // "Admin" or "Warden"
                if (facultyId == 65 || facultyId == 201 || facultyId == 62 || facultyId == 38) { role = "Admin"; }
                var students = objfaculty
                                .getStudentsCareerProgressionList(facultyId, role)
                                .ToList();

                ViewBag.TotalStudents = students.Count();
                ViewBag.Responded = students.Count(x => x.Career_Progression != null);
                ViewBag.Verified = students.Count(x => x.Verified_Status == true);

                ViewBag.Programme = new SelectList(
                                        objfaculty.get_Programme_All(),
                                        "Pgm_Id",
                                        "Programme");

                ViewBag.Status = role == "Admin" ? "Active" : "Inactive";

                return View(students);
            }
            catch
            {
                return Redirect("~/Login/Error_Page");
            }
        }

        [HttpPost]
        public ActionResult Update_Status(int Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    CMS_StudentProgression invg = db.CMS_StudentProgressions.Where(x => x.ProID == Id).FirstOrDefault();
                    int status = 1;
                    if (invg != null)
                    {
                        if (invg.Verified_Status == false)
                        {
                            invg.Verified_Status = true;
                            invg.Verified_By = new Guid(Session["Log_Id"].ToString());
                            invg.Verified_Date = DateTime.Now;
                        }
                        else
                        {
                            invg.Verified_Status = false;
                            status = 2;
                        }
                        db.SaveChanges();
                    }
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult getRemark(int id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    CMS_StudentProgression pro = db.CMS_StudentProgressions.Where(x => x.ProID == id).FirstOrDefault();
                    return Json(pro, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }

            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Remark_Update(CMS_StudentProgression p, HttpPostedFileBase Photo)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    CMS_StudentProgression pro = db.CMS_StudentProgressions.Where(x => x.ProID == p.ProID).FirstOrDefault();
                    if (pro != null)
                    {
                        if (Photo != null)
                        {
                            Conversion convert = new Conversion();
                            var filePath = string.Empty;
                            string extension = System.IO.Path.GetExtension(Photo.FileName).Trim();

                            // extension1 = System.IO.Path.GetExtension(Request.Files["Image1"].FileName);
                            string filename1 = pro.UPRN + extension;
                            filePath = Path.Combine(Server.MapPath("~/Images/StudentProgression"), filename1);
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }
                            Photo.SaveAs(filePath);
                            // Image bm = System.Drawing.Image.FromStream(Photo.InputStream);
                            //convert.ResizeImage((Bitmap)bm, 150, 200, 80, filePath);
                            pro.Upload = filename1;
                        }
                        pro.Name_Comp_Insti = p.Name_Comp_Insti;
                        pro.Designation_Course = p.Designation_Course;
                        pro.Salary = p.Salary;
                        pro.Remark = p.Remark;
                        pro.Career_Progression = p.Career_Progression;
                        db.SaveChanges();
                    }

                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }
        }
        [HttpPost]
        public ActionResult getProgrammeWiseStudentsProgression(int Pgm_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    List<Student> pro = objfaculty.getProgrammeWiseStudentsProgression(Pgm_Id).ToList();
                    //ViewBag.TotalStudents = pro.Count();
                    //ViewBag.Responded = pro.Where(x => x.Career_Progression != null).Count();
                    //ViewBag.Verified = pro.Where(x => x.Verified_Status == true).Count();
                    return Json(pro, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);

                }

            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion




        public ActionResult getMarked_Attendance_Facultywise()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.UPRN_No = new SelectList(objAtt.getuprn(), "UPRN", "UPRN");
                    ViewBag.Sem = new SelectList(objAtt.Search_Sem(), "Acc_Yr_Sem_Id", "Semester");

                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Faculty_Login");

            }
        }

        public ActionResult getFacultyWise_MarkedAttendance(int Pgm_Id, DateTime sdate, DateTime edate, int Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    List<AttendanceReport> sch = new List<AttendanceReport>();
                    ViewBag.PgmType = (from a in db.CMS_AcademicYr_Sem_Programmes
                                       join b in db.CMS_Programmes on a.Pgm_Id equals b.Pgm_Id
                                       where a.Acc_Yr_Sem_Pgm_Id == Pgm_Id
                                       select b.Pgm_Type_Id).FirstOrDefault();
                    ViewBag.faculty = objAtt.Search_ProgrammesWise_Faculties(Pgm_Id).ToList();
                    sch = objAtt.getmarked_FacultyAttendance(Pgm_Id, sdate, edate, Sem_Id).ToList();
                    return PartialView(sch);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Faculty_Login");

            }
        }
        public ActionResult download()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Faculty_Login");

            }
        }
        public ActionResult PG_Applicants()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    int Dep_Id = db.CMS_Facultys.Where(x => x.Faculty_Id == Faculty_Id).Select(x => x.Dep_Id).FirstOrDefault();
                    var Pgms = (from a in db.CMS_PG_Programmes
                                join b in db.CMS_Programmes on a.Pgm_Id equals b.Code
                                where b.Dep_Id == Dep_Id
                                select a.Pgm_Id).ToList();

                    var applicants = (from a in db.CMS_PGApplications
                                      where (Pgms.Contains(a.Choice1) || Pgms.Contains(a.Choice2) || Pgms.Contains(a.Choice3)) && a.Acc_Yr_Id == 8
                                      select new { a }).ToList().Distinct();
                    List<RankList> list = new List<RankList>();
                    foreach (var item in applicants)
                    {
                        RankList r = new RankList();
                        r.Applcn_Id = item.a.Applcn_Id;
                        r.Name = item.a.Name;
                        r.Mobile = item.a.Mobile;
                        r.Email = item.a.Email;
                        r.Remark = item.a.Remark;
                        r.Programme = db.CMS_Online_Document_Uploads.Where(x => x.Applcn_Id == item.a.Applcn_Id && x.Doc_Id == 21).Select(x => x.Document_Name).FirstOrDefault();
                        list.Add(r);
                    }

                    return View(list);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Faculty_Login");

            }
        }

        #region Question bank

        public ActionResult Course_List()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid logid = new Guid(Session["Log_Id"].ToString());
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());

                    List<Schedule> courses =
                        objfaculty.getQBCourses(Faculty_Id, logid).ToList();


                    // Check Question Bank opening/closing status
                    foreach (var item in courses)
                    {
                        bool isOpen = false;

                        var qbStatus = db.CMS_QB_OPCL_Statuss
                            .FirstOrDefault(x =>
                                x.Acc_Yr_Sem_Id == item.Acc_Yr_sem_Id &&
                                x.Pgm_Id == item.Pgm_Id &&
                                x.Active_Status == true);

                        if (qbStatus != null &&
                            qbStatus.Open_Date.HasValue &&
                            qbStatus.Close_Date.HasValue)
                        {
                            DateTime today = DateTime.Today;

                            isOpen = today >= qbStatus.Open_Date.Value.Date &&
                                     today < qbStatus.Close_Date.Value.Date;
                        }

                        item.QB_Open_Status = isOpen;
                    }

                    // Only show courses whose Question Bank is currently open
                    courses = courses
                        .Where(x => x.QB_Open_Status)
                        .ToList();

                    return View(courses);
                }
                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");
            }
        }
        public ActionResult check_QBStatus(int Course_Id, int EditStatus)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    var acc = objfaculty.getCurrentAcademicYear();
                    string CourseCode = db.CMS_Courses.Where(x => x.Course_Id == Course_Id).Select(x => x.Course_Code).FirstOrDefault();
                    int cochk = db.CMS_CourseOutcomes.Where(x => x.Active_Status == true && x.Course_Code == CourseCode).Count();
                    int cdchk = db.CMS_Course_Descriptions.Where(x => x.Active_Status == true && x.Course_Code == CourseCode).Count();

                    int sec_mark_chk = db.Exam_Section_Marks.Where(x => x.Active_Status == true && x.Course_Code == CourseCode && x.Exam_Type == "SemExam" && x.Acc_Yr_Id == acc.Acc_yr_Id).Count();
                    Session["Course_Id"] = Course_Id;

                    if (cochk == 0)
                    {
                        return Redirect("~/OBE/OBE/CourseOutcome");
                    }
                    else if (cdchk == 0)
                    {
                        return Redirect("~/OBE/OBE/Course_Description");
                    }
                    else if (EditStatus == 1 && cochk != 0)
                    {
                        return Redirect("~/OBE/OBE/Course_Description");
                    }
                    else if (sec_mark_chk == 0)
                    {
                        return RedirectToAction("QPproforma", new { Course_Id = Course_Id });
                    }
                    else
                    {
                        Guid logid = new Guid(Session["Log_Id"].ToString());
                        int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                        List<Schedule> courses = objfaculty.getQBCourses(Faculty_Id, logid).ToList();
                        return RedirectToAction("Add_Questions", new { Course_Id = Course_Id });

                    }
                }

                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult QPproforma(int Course_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (isUserAuthenticated())
                    {

                        int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                        string CourseCode = db.CMS_Courses.Where(x => x.Course_Id == Course_Id).Select(x => x.Course_Code).FirstOrDefault();
                        ViewBag.Course = db.CMS_Courses.Where(x => x.Course_Code == CourseCode).FirstOrDefault();
                        ViewBag.Exam_Type = db.CMS_Courses.Where(x => x.Course_Code == CourseCode).Select(x => x.Exam_Type).FirstOrDefault();
                        // ViewBag.Max_Mark = db.CMS_Courses.Where(x => x.Course_Code == CourseCode).Select(x => x.MaxMark).FirstOrDefault();
                        var course = db.CMS_Courses.FirstOrDefault(x => x.Course_Code == CourseCode);

                        ViewBag.Max_Mark = course.Course_Type == "MDC"
                            ? $"{course.MaxMark.GetValueOrDefault()}MDC"
                            : course.MaxMark.GetValueOrDefault().ToString();
                        ViewBag.Time = db.CMS_Courses.Where(x => x.Course_Code == CourseCode).Select(x => x.Time).FirstOrDefault();
                        return View();
                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }
                }

                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Add_Questions(int Course_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    string Course_Code = db.CMS_Courses.Where(x => x.Course_Id == Course_Id).Select(x => x.Course_Code).FirstOrDefault();
                    ViewBag.Course_Code = Course_Code;
                    ViewBag.Course_Id = Course_Id;
                    ViewBag.Course_Name = db.CMS_Courses.Where(x => x.Course_Code == Course_Code).Select(x => x.Course_Name).FirstOrDefault();
                    ViewBag.Exam_Type = db.CMS_Courses.Where(x => x.Course_Code == Course_Code).Select(x => x.Exam_Type).FirstOrDefault();

                    ViewBag.Section = new SelectList(objfaculty.getExam_QuestionSection(Course_Code), "Sec_Id", "Section");
                    ViewBag.Module = new SelectList(objfaculty.getModule(Course_Code), "Module", "Module");
                    return View();
                }
                catch (Exception ex)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult get_SubModule(double Module, string Course_Code)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Question_Bunk> qbk = new List<Question_Bunk>();

                    qbk = objfaculty.get_SubModule(Module, Course_Code).ToList();
                    return Json(qbk, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }

        //[HttpPost]
        [HttpPost, ValidateInput(false)]
        public ActionResult Add_Question_Answer(Question_Bunk qb)
        {
            try
            {
                if (Session["Log_Id"] != null)
                {
                    qb.Created_By = new Guid(Session["Log_Id"].ToString());
                    int retVal = objfaculty.Add_Question_Answer(qb);
                    return Json(retVal, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(2, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(2, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult View_All_Questions(int Course_Id)
        {
            var acc = objfaculty.getCurrentAcademicYear();




            ViewBag.Course_Id = Course_Id;
            string code = db.CMS_Courses.Where(x => x.Course_Id == Course_Id && x.Active_Status == true).Select(x => x.Course_Code).FirstOrDefault();
            string name = db.CMS_Courses.Where(x => x.Course_Id == Course_Id && x.Active_Status == true).Select(x => x.Course_Name).FirstOrDefault();

            int cochk = db.CMS_CourseOutcomes.Where(x => x.Active_Status == true && x.Course_Code == code).Count();
            int cdchk = db.CMS_Course_Descriptions.Where(x => x.Active_Status == true && x.Course_Code == code).Count();

            int sec_mark_chk = db.Exam_Section_Marks.Where(x => x.Active_Status == true && x.Course_Code == code && x.Exam_Type == "SemExam" && x.Acc_Yr_Id == acc.Acc_yr_Id).Count();
            Session["Course_Id"] = Course_Id;

            if (cochk == 0)
            {
                return Redirect("~/OBE/OBE/CourseOutcome");
            }
            else if (cdchk == 0)
            {
                return Redirect("~/OBE/OBE/Course_Description");
            }
            else if (sec_mark_chk == 0)
            {
                return RedirectToAction("QPproforma", new { Course_Id = Course_Id });
            }
            else
            {



                ViewBag.Course_Code = code;
                ViewBag.Course_Name = name;
                ViewBag.Exam_Type = db.CMS_Courses.Where(x => x.Course_Code == code && x.Active_Status == true).Select(x => x.Exam_Type).FirstOrDefault();
                ViewBag.Section = new SelectList(objfaculty.getExam_QuestionSection(code), "Sec_Id", "Section");
                ViewBag.Module = new SelectList(objfaculty.getModule(code), "Module", "Module");
                return View();
            }






        }
        [HttpPost]
        public ActionResult View_All_Questions_Qry(Question_Bunk qb)
        {
            CMS_AcademicYear Acc_Yr = objfaculty.getCurrentAcademicYear();
            List<Question_Bunk> qbnk = new List<Question_Bunk>();
            qb.Created_By = new Guid(Session["Log_Id"].ToString());
            qb.Qn_Type = db.Exam_Section_Marks.Where(x => x.Course_Code == qb.Course_Code && x.Sec_Id == qb.Sec_Id && x.Active_Status == true && x.Exam_Type == "SemExam" && x.Acc_Yr_Id == Acc_Yr.Acc_yr_Id).Select(x => x.Qn_Type).FirstOrDefault();
            qbnk = objfaculty.get_All_Questions_By_Faculty(qb).ToList();
            return Json(qbnk, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult View_All_Questions_Qry_Written(Question_Bunk qb)
        {
            CMS_AcademicYear Acc_Yr = objfaculty.getCurrentAcademicYear();
            List<Question_Bunk> qbnk = new List<Question_Bunk>();
            qb.Created_By = new Guid(Session["Log_Id"].ToString());
            qb.Qn_Type = db.Exam_Section_Marks.Where(x => x.Course_Code == qb.Course_Code && x.Sec_Id == qb.Sec_Id && x.Active_Status == true && x.Exam_Type == "SemExam" && x.Acc_Yr_Id == Acc_Yr.Acc_yr_Id).Select(x => x.Qn_Type).FirstOrDefault();
            ViewBag.QnType = qb.Qn_Type;
            qbnk = objfaculty.get_All_Questions_By_Faculty(qb).ToList();

            var jsonResult = Json(qbnk, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult Delete_All_Questions(int Quest_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    objfaculty.Delete_All_Questions(Quest_Id);

                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }
        }

        public ActionResult Edit_Questions(int Quest_Id, string Exam_Type, int Course_Id, string Qn_Type)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Question_Bunk> ox = objfaculty.getAllQuestionsWithQuest_Id(Quest_Id, Exam_Type, Qn_Type).ToList();
                    ViewBag.Section = new SelectList(objfaculty.getExam_QuestionSection(ox[0].Course_Code), "Sec_Id", "Section");
                    ViewBag.Module = new SelectList(objfaculty.getModule(ox[0].Course_Code), "Module", "Module");
                    ViewBag.Quest_Id = Quest_Id;
                    ViewBag.Course_Id = Course_Id;
                    ViewBag.Exam_Type = ox[0].Exam_Type;
                    ViewBag.Qn_Type = Qn_Type;
                    return View(ox);
                }
                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {

                return Redirect("~/Login/Login");
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult update_Questions(Question_Bunk on)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.update_Questions(on);
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public ActionResult getCourseDetails(string Course_Code)
        {
            try
            {
                if (Session["Log_Id"] != null)
                {

                    CMS_Course cs = db.CMS_Courses.Where(x => x.Active_Status == true && x.Course_Code == Course_Code).FirstOrDefault();
                    return Json(cs, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }



        public ActionResult QB_Upload()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    return View();
                }

                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        [HttpPost]
        public ActionResult QB_Upload_Word(string filename)
        {
            //if (Session["Log_Id"] != null)
            //{
            //    try
            //    {
            //        string textBuilder = objfaculty.TextFromWord(filename);
            //        return View(textBuilder);
            //    }

            //    catch (Exception ex)
            //    {
            //        return Redirect("~/Login/Error_Page");
            //    }
            //}
            //else
            //{
            //    return Redirect("Faculty_Login");

            //}

            if (Request.Files.Count > 0)
            {
                try
                {


                    var filePath = string.Empty;
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;

                    // int Doc_Id = Convert.ToInt32(Doc_Qt_Id);

                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        //string ids = Request.Form["Doc_Qt_Id"].ToString();
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        //string textBuilder = objfaculty.TextFromWord(fname);
                        //return View(textBuilder);

                        string textBuilder = objfaculty.Read_Word(fname);
                    }
                    return Json(1, JsonRequestBehavior.AllowGet);
                    // }
                    //return Json(1, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(2, JsonRequestBehavior.AllowGet);
                    //}
                }

                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult get_SectionA_MCQ(int Sec_Id, string Course_Code)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    string sec = "";
                    if (Sec_Id != 0)
                    {
                        sec = objfaculty.get_SectionA_MCQ(Sec_Id, Course_Code);
                        return Json(sec, JsonRequestBehavior.AllowGet);
                    }

                    return Json(0, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult get_Each_Section(int Sec_Id, string Course_Code)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<Exam_Section_Mark> sec = new List<Exam_Section_Mark>();
                    if (Sec_Id != 0)
                    {
                        sec = objfaculty.get_Each_Section(Sec_Id, Course_Code).ToList();
                        return Json(sec, JsonRequestBehavior.AllowGet);
                    }

                    return Json(0, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult Print(string Course_Code, string Exam_Type, int Sec_Id, string Qn_Type)
        {

            List<Question_Bunk> qbnk = new List<Question_Bunk>();
            Guid Created_By = new Guid(Session["Log_Id"].ToString());
            ViewBag.Qn_Type = db.Exam_Section_Marks.Where(x => x.Course_Code == Course_Code && x.Sec_Id == Sec_Id && x.Active_Status == true && x.Exam_Type == "SemExam").Select(x => x.Qn_Type).FirstOrDefault();
            qbnk = objfaculty.Print(Course_Code, Exam_Type, Sec_Id, Qn_Type).ToList();
            //return View(qbnk);

            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Report"), "QA.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Index");
            }


            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = qbnk;
            lr.DataSources.Add(reportDataSource);
            lr.EnableExternalImages = true;

            string reportType = "Word";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

                "<DeviceInfo>" +

                "<OutputFormat>" + reportType + "</OutputFormat>" +

                "<PageWidth>8.7in</PageWidth>" +
                "</DeviceInfo>";

            Warning[] warning;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warning);
            //  return File(renderedBytes, "pdf");
            return File(renderedBytes, mimeType);
        }

        #endregion Question bank

        #region Internal Exam - Mark Entry
        public ActionResult InternalExam_MarkEntry()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    List<Valuation> intrnl = objfaculty.getISE_CourseList(Faculty_Id).ToList();



                    ViewBag.ISEConsolidated = objfaculty.getISE_ConsolidatedList(Faculty_Id).ToList();
                    return View(intrnl);
                }

                catch (Exception)
                {
                    return RedirectToAction("Faculty_Login", "Faculty", new { area = "Faculty" });
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult InsertISECourseSection(List<Exam_Section_Mark> courseSections)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid LogId = new Guid(Session["Log_Id"].ToString());
                    int val = objfaculty.AddISECourseSection(courseSections, LogId);
                    if (val > 1)
                    {
                        CMS_Course crs = db.CMS_Courses.Where(x => x.Course_Id == val).FirstOrDefault();
                        return Json(crs, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(val, JsonRequestBehavior.AllowGet);
                    }

                }

                catch (Exception ex)
                {

                    return Json(ex.InnerException.ToString(), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }

        }


        public ActionResult Start_ValuationISE(int Acc_Yr_Sem_Pgm_Id, string CourseCode, int Course_Sem_Id, string Course_Type, string ISE_Type, string Category, int Stream)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (isUserAuthenticated())
                    {
                        int Examiner_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                        //List<Valuation> objVAl = new List<Valuation>();

                        List<Valuation> objVAl = objfaculty.getStudentsForISEValuation(Acc_Yr_Sem_Pgm_Id, Course_Sem_Id, Examiner_Id, ISE_Type, Category, Stream).ToList();
                        ViewBag.Course_Sem_Id = Course_Sem_Id;
                        ViewBag.CourseCode = CourseCode;
                        ViewBag.Course_Nature = Course_Type;
                        Session["CourseCode"] = CourseCode;
                        string Course_Name = db.CMS_Courses.Where(x => x.Course_Code == CourseCode).Select(x => x.Course_Name).FirstOrDefault();
                        Session["Course"] = Course_Name;
                        ViewBag.Category = Category;
                        ViewBag.Course_Type = Category;
                        ViewBag.Acc_Yr_Sem_Pgm_Id = Acc_Yr_Sem_Pgm_Id;
                        ViewBag.ISE_Type = ISE_Type;
                        ViewBag.Stream = Stream;



                        return View(objVAl);
                    }
                    else
                    {
                        return RedirectToAction("Faculty_Login", "Faculty", new { area = "Faculty" });
                    }
                }
                catch
                {
                    return RedirectToAction("Faculty_Login", "Faculty", new { area = "Faculty" });
                }
            }
            else
            {
                return RedirectToAction("Faculty_Login", "Faculty", new { area = "Faculty" });
            }
        }

        public ActionResult AnswerScriptISE(int Acc_Yr_Sem_Pgm_Id, string UPRN, string CourseCode, int Course_Sem_Id, string Course_Type, string ISE_Type, string Category, int Stream)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.Acc_Yr_Sem_Pgm_Id = Acc_Yr_Sem_Pgm_Id;
                    ViewBag.Course_Sem_Id = Course_Sem_Id;
                    ViewBag.CourseCode = CourseCode;
                    ViewBag.UPRN = UPRN;
                    ViewBag.FormDate = (from b in db.CMS_AcademicYr_Sem_Programmes
                                        join c in db.CMS_AccademicYearSemesters
                                            on b.Acc_Yr_sem_Id equals c.Acc_Yr_Sem_Id
                                        join d in db.CMS_ISE_MarkEntry_Schedules
                                            on c.Acc_Yr_Sem_Id equals d.Acc_Yr_Sem_Id
                                        where b.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Sem_Pgm_Id && d.ISE_Type == ISE_Type && d.Active_Status
                                        select (DateTime?)d.Form_Date)  // nullable to avoid exception
                     .FirstOrDefault();

                    ViewBag.Name = (from a in db.CMS_UPRNs
                                    join b in db.CMS_Students on a.Admission_No equals b.Admission_No
                                    where a.UPRN == UPRN
                                    select b.Name).FirstOrDefault();
                    ViewBag.Course_Type = Course_Type;
                    ViewBag.Category = Category;
                    ViewBag.ISE_Type = ISE_Type;
                    ViewBag.Stream = Stream;
                    ViewBag.CourseName = db.CMS_Courses.Where(x => x.Course_Code == CourseCode && x.Active_Status == true).Select(x => x.Course_Name).FirstOrDefault();
                    string Code = Session["CourseCode"].ToString();
                    List<Online_MarkEntry> dtls = objfaculty.getAnswerSheetISE(Course_Sem_Id, UPRN, Code, Course_Type, ISE_Type).ToList();
                    return View(dtls);

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Faculty_Login", "Faculty", new { area = "Faculty" });
                }
            }
            else
            {
                return RedirectToAction("Faculty_Login", "Faculty", new { area = "Faculty" });
            }
        }

        [HttpPost]
        public ActionResult Add_Mark(string UPRN, decimal? Mark, int Pat_Id, string CourseCode, string Course_Type, string ISE_Type, string Category, int Stream, int CourseSemId)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {
                    string Course_Code = Session["CourseCode"].ToString();
                    Guid Log_Id = new Guid(Session["Log_Id"].ToString());

                    //Decimal? MArkValue = Mark;

                    //if (MArkValue.HasValue)
                    //{
                    //  decimal Mmark = Convert.ToDecimal(MArkValue);
                    objfaculty.Add_Mark(UPRN, Course_Code, Mark, Log_Id, Pat_Id, Course_Type, "First Valuation");
                    // }

                    //  decimal total = objvaluation.CalculateMark(Faux_Code,  Pat_Id, Course_Code);
                    Online_MarkEntry objMArk = objfaculty.CalculateMark(UPRN, Pat_Id, Course_Code, Course_Type, ISE_Type, Category, Stream, CourseSemId);
                    return Json(objMArk, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult Add_Mark_Final(List<Exam_ISEValuationMark> FinalMark)
        {
            try
            {

                if (Session["Log_Id"] != null)
                {
                    Guid Log_Id = new Guid(Session["Log_Id"].ToString());
                    string Course_Code = Session["CourseCode"].ToString();

                    foreach (var item in FinalMark)
                    {

                        objfaculty.Add_Mark(item.UPRN, Course_Code, item.Mark, Log_Id, item.Qns_Map_Id, item.Course_Type, "First Valuation");

                    }

                    return Json(1, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult MarkSheet(string CourseCode, string Course_Type, string ExamType, int Acc_Yr_Sem_Pgm_Id, int Course_Sem_Id)
        {

            if (Session["Log_Id"] != null)
            {

                try
                {

                    LocalReport lr = new LocalReport();
                    string path = Path.Combine(Server.MapPath("~/Report"), "ISEMarkSheet.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }
                    List<Online_MarkEntry> dtls = objfaculty.getMarkSheet(CourseCode, Course_Type, ExamType, Acc_Yr_Sem_Pgm_Id, Course_Sem_Id).ToList();

                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "DataSet1";
                    reportDataSource.Value = dtls;
                    lr.DataSources.Add(reportDataSource);


                    var course = db.CMS_Courses.Where(x => x.Course_Code == CourseCode && x.Active_Status == true).FirstOrDefault();
                    List<ReportParameter> paraList = new List<ReportParameter>();
                    paraList.Add(new ReportParameter("Course_Code", course.Course_Code));
                    paraList.Add(new ReportParameter("Course_Name", course.Course_Name));
                    paraList.Add(new ReportParameter("Course_Type", dtls[0].Category));
                    paraList.Add(new ReportParameter("Sem", dtls[0].Sem));
                    paraList.Add(new ReportParameter("Group", dtls[0].Group));
                    lr.SetParameters(paraList.ToArray());

                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;
                    string deviceInfo =

                        "<DeviceInfo>" +

                        "<OutputFormat>" + reportType + "</OutputFormat>" +

                        "<PageWidth>8.5in</PageWidth>" +

                        "</DeviceInfo>";


                    Warning[] warning;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = lr.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warning);
                    //  return File(renderedBytes, "pdf");
                    return File(renderedBytes, mimeType);

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Faculty_Login", "Faculty", new { area = "Faculty" });
                }
            }
            else
            {
                return RedirectToAction("Faculty_Login", "Faculty", new { area = "Faculty" });

            }

        }


        public ActionResult ISEConsolidatedMarkSheet(string ExamType, int Acc_Yr_Sem_Pgm_Id)
        {

            if (Session["Log_Id"] != null)
            {

                try
                {

                    LocalReport lr = new LocalReport();
                    // string path = Path.Combine(Server.MapPath("~/Report"), "ISEConsolidatedMarkSheet.rdlc");
                    string path = Path.Combine(Server.MapPath("~/Report"), "ISEConsolidatedMarkReport.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }
                    List<Online_MarkEntry> dtls = objfaculty.getISEConsolidatedMarkSheet(ExamType, Acc_Yr_Sem_Pgm_Id).ToList();

                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "DataSet1";
                    reportDataSource.Value = dtls;
                    lr.DataSources.Add(reportDataSource);

                    //List<ReportParameter> paraList = new List<ReportParameter>();
                    //paraList.Add(new ReportParameter("ExamType", ExamType));
                    //paraList.Add(new ReportParameter("Semester", dtls[0].Sem));
                    //paraList.Add(new ReportParameter("Year", dtls[0].AccademicYear));
                    //lr.SetParameters(paraList.ToArray());


                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;
                    string deviceInfo =

                        "<DeviceInfo>" +

                        "<OutputFormat>" + reportType + "</OutputFormat>" +

                        "<PageWidth>11.69in</PageWidth>" +

                        "</DeviceInfo>";


                    Warning[] warning;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = lr.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warning);
                    //  return File(renderedBytes, "pdf");
                    return File(renderedBytes, mimeType);

                }
                catch (Exception ex)
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Faculty_Login", "Faculty", new { area = "Faculty" });

            }

        }



        [HttpPost]
        public ActionResult MarkAsAbsent(List<Exam_ISEValuationMark> FinalMark)
        {
            try
            {

                if (Session["Log_Id"] != null)
                {
                    Guid Log_Id = new Guid(Session["Log_Id"].ToString());
                    string Course_Code = Session["CourseCode"].ToString();

                    foreach (var item in FinalMark)
                    {

                        objfaculty.MarkAsAbsent(item.UPRN, Course_Code, item.Mark, Log_Id, item.Qns_Map_Id, item.Course_Type, "First Valuation");
                    }

                    return Json(1, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult MarkAsPresent(List<Exam_ISEValuationMark> FinalMark)
        {
            try
            {

                if (Session["Log_Id"] != null)
                {
                    Guid Log_Id = new Guid(Session["Log_Id"].ToString());
                    string Course_Code = Session["CourseCode"].ToString();

                    foreach (var item in FinalMark)
                    {

                        objfaculty.MarkAsPresent(item.UPRN, Course_Code, item.Mark, Log_Id, item.Qns_Map_Id, item.Course_Type, "First Valuation");
                    }

                    return Json(1, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult getISECODetails(int Course_Sem_Id, int Eval_Id)
        {

            try
            {
                if (Session["Log_Id"] != null)
                {
                    IEnumerable<CCA> obj = objfaculty.getISECODetails(Course_Sem_Id, Eval_Id);
                    return Json(obj, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Syllabus Entry

        public ActionResult Syllabus_Course_List(Nullable<int> Course_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {



                    //ViewBag.Department = new SelectList(objLearn.getAllDep(), "Dep_Id", "Department");
                    //ViewBag.Programmes_Type = new SelectList(objLearn.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");

                    Guid logid = new Guid(Session["Log_Id"].ToString());
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    Schedule crs = new Schedule();

                    if (Course_Id != null)
                    {
                        int CrsId = Convert.ToInt32(Course_Id);
                        ViewBag.CourseId = Course_Id;
                        crs = objfaculty.getSyllabusCourseById(CrsId);

                    }
                    crs.sch = new List<Schedule>();
                    List<Schedule> courses = objfaculty.getSyllabusCourses(Faculty_Id, logid).ToList();
                    crs.sch = courses;

                    return View(crs);
                }

                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult Course_Description(int Course_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    TempData["Course_Id"] = Course_Id;
                    return Redirect("~/Faculty/Faculty/Syllabus_Course_Description");

                }

                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult Syllabus_Course_Description(int Course_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid logid = new Guid(Session["Log_Id"].ToString());
                    int Fac_Id = Convert.ToInt32(Session["Faculty_Id"]);
                    int Dep_Id = db.CMS_Facultys.Where(x => x.Faculty_Id == Fac_Id).Select(x => x.Dep_Id).FirstOrDefault();
                    // ViewBag.Courses = new SelectList(objfaculty.getSyllabusCourses(Fac_Id), "Course_Id", "Course_Name", Course_Id);
                    ViewBag.Courses = new SelectList(objfaculty.getSyllabusCourses(Fac_Id, logid), "Course_Id", "Course_Name");
                    ViewBag.Course = db.CMS_Courses.Where(x => x.Course_Id == Course_Id && x.Syllabus == "HONOURS25").Select(x => x.Course_Name).FirstOrDefault();
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Faculty/Faculty/Faculty_Login");

            }
        }

        public ActionResult Course_SyllabusPrint(int Course_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.CourseId = Course_Id; // For hidden field
                    Guid logid = new Guid(Session["Log_Id"].ToString());
                    int Fac_Id = Convert.ToInt32(Session["Faculty_Id"]);
                    int Dep_Id = db.CMS_Facultys.Where(x => x.Faculty_Id == Fac_Id).Select(x => x.Dep_Id).FirstOrDefault();
                    // ViewBag.Courses = new SelectList(objfaculty.getSyllabusCourses(Fac_Id), "Course_Id", "Course_Name", Course_Id);
                    ViewBag.Courses = new SelectList(objfaculty.getSyllabusCourses(Fac_Id, logid), "Course_Id", "Course_Name");
                    ViewBag.Course = db.CMS_Courses.Where(x => x.Course_Id == Course_Id && x.Syllabus == "HONOURS25").Select(x => x.Course_Name).FirstOrDefault();
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Faculty/Faculty/Faculty_Login");

            }
        }

        public string GenerateCourseCode(int depid, int? semId, string CourseType)
        {
            string CourseCode = string.Empty;
            int code = 1;
            var crs = db.CMS_Courses.Where(x => x.Dep_Id == depid && x.Syllabus == "HONOURS25" && x.Semester == semId).ToList().OrderByDescending(x => x.Course_Id).FirstOrDefault();
            if (crs != null)
            {
                // FYZY24111101
                string substring = crs.Course_Code.Substring(crs.Course_Code.Length - 2);
                code = Convert.ToInt32(substring) + 1;

            }
            string strCode = code.ToString();
            if (code < 10)
            {
                strCode = "0" + code;
            }
            int Yr = 25;

            string dep;
            switch (depid)
            {
                case 1:
                    dep = "EN";
                    break;
                case 2:
                    dep = "ML";
                    break;
                case 3:
                    dep = "HN";
                    break;
                case 4:
                    dep = "MT";
                    break;
                case 5:
                    dep = "PH";
                    break;
                case 6:
                    dep = "CH";
                    break;
                case 7:
                    dep = "ZY";
                    break;
                case 8:
                    dep = "BY";
                    break;
                case 9:
                    dep = "HS";
                    break;
                case 10:
                    dep = "BY";
                    break;
                case 11:
                    dep = "HY";
                    break;
                case 12:
                    dep = "EC";
                    break;
                case 13:
                    dep = "CM";
                    break;
                case 14:
                    dep = "CE";
                    break;
                case 15:
                    dep = "BT";
                    break;
                case 16:
                    dep = "CA";
                    break;
                case 17:
                    dep = "ST";
                    break;
                case 18:
                    dep = "PE";
                    break;
                case 19:
                    dep = "SC";
                    break;
                case 20:
                    dep = "PS";
                    break;
                case 24:
                    dep = "TT";
                    break;
                default:
                    dep = "XX";
                    break;

            }
            int TypeId = 1;
            switch (CourseType)
            {
                case "DSC":
                    TypeId = 1;
                    break;
                case "MDC":
                    TypeId = 4;
                    break;
                case "AEC":
                    TypeId = 8;
                    break;
                case "VAC":
                    TypeId = 9;
                    break;
                case "SEC":
                    TypeId = 5;
                    break;

                default:
                    TypeId = 1;
                    break;

            }
            CourseCode = "FY" + dep + Yr + "1" + semId + TypeId + strCode;
            return CourseCode;
        }

        [HttpPost]
        public ActionResult Add_Course(CMS_Course cs)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {
                    int Fac_Id = Convert.ToInt32(Session["Faculty_Id"]);
                    int Dep_Id = db.CMS_Facultys.Where(x => x.Faculty_Id == Fac_Id).Select(x => x.Dep_Id).FirstOrDefault();
                    cs.Dep_Id = Dep_Id;
                    cs.Pgm_Type_Id = 1;
                    //  var  count = db.CMS_Courses.Where(x => x.Course_Name.ToUpper().Trim() == cs.Course_Name.ToUpper().Trim()&& x.Syllabus== "HONOURS25" && x.Course_Type==cs.Course_Type && x.Active_Status == true).FirstOrDefault();
                    var count = db.CMS_Courses.Where(x => x.Course_Id == cs.Course_Id && x.Active_Status == true).FirstOrDefault();
                    if (count == null)
                    {

                        cs.Syllabus = "HONOURS25";
                        cs.Course_Code = GenerateCourseCode(Dep_Id, cs.Semester, cs.Course_Type);
                        cs.Created_By = new Guid(Session["Log_Id"].ToString());
                        objLearn.Add_Course(cs);
                        return Json(1, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        cs.Course_Code = count.Course_Code;
                        objLearn.Edit_Course(cs);
                        return Json(2, JsonRequestBehavior.AllowGet);
                    }


                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult check_CourseStatus(int Course_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    string CourseCode = db.CMS_Courses.Where(x => x.Course_Id == Course_Id).Select(x => x.Course_Code).FirstOrDefault();
                    int cochk = db.CMS_CourseOutcomes.Where(x => x.Active_Status == true && x.Course_Code == CourseCode).Count();
                    int cdchk = db.CMS_Course_Descriptions.Where(x => x.Active_Status == true && x.Course_Code == CourseCode).Count();


                    TempData["Course_Id"] = Course_Id;

                    if (cochk == 0)
                    {
                        return Redirect("~/Faculty/Faculty/Syllabus_CourseOutcome?Course_Id=" + Course_Id);
                    }
                    else
                    {
                        return Redirect("~/Faculty/Faculty/Syllabus_Course_Description?Course_Id=" + Course_Id);
                    }


                }

                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult Syllabus_CourseOutcome(int Course_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid logid = new Guid(Session["Log_Id"].ToString());
                    int Fac_Id = Convert.ToInt32(Session["Faculty_Id"]);
                    int Dep_Id = db.CMS_Facultys.Where(x => x.Faculty_Id == Fac_Id).Select(x => x.Dep_Id).FirstOrDefault();

                    ViewBag.Courses = new SelectList(objfaculty.getSyllabusCourses(Fac_Id, logid), "Course_Id", "Course_Name", Course_Id);
                    ViewBag.Course = db.CMS_Courses.Where(x => x.Course_Id == Course_Id).Select(x => x.Course_Name).FirstOrDefault();
                    return View();
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult printSyllabus_CourseOutcome(int Course_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid logid = new Guid(Session["Log_Id"].ToString());
                    int Fac_Id = Convert.ToInt32(Session["Faculty_Id"]);

                    ViewBag.Course = db.CMS_Courses
                                       .Where(x => x.Course_Id == Course_Id)
                                       .Select(x => x.Course_Name)
                                       .FirstOrDefault();

                    // 🔥 THIS LINE WAS MISSING
                    ViewBag.EditCourse_Id = Course_Id;

                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");
            }
        }


        public ActionResult Edit_CourseOutcome(int Course_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    TempData["Course_Id"] = Course_Id;
                    ViewBag.EditCourse_Id = Course_Id;
                    return Redirect("~/Faculty/Faculty/Syllabus_CourseOutcome");

                }

                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        public ActionResult AddNewCourseDescription(CMS_OBE obe)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    CMS_Admission.Areas.OBE.DAL.DALOBE objOBE = new OBE.DAL.DALOBE();
                    objOBE.AddNewCourseDescription(obe, Created_By);
                    return Json(1, JsonRequestBehavior.AllowGet);
                }

                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }


        public ActionResult AddCourseModule(CMS_OBE obe)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    CMS_Admission.Areas.OBE.DAL.DALOBE objOBE = new OBE.DAL.DALOBE();
                    objOBE.Add_Course_Module(obe, Created_By);
                    return Json(1, JsonRequestBehavior.AllowGet);
                }

                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }


        public ActionResult AddNewLearningActivity(CMS_OBE courseActivities)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    CMS_Admission.Areas.OBE.DAL.DALOBE objOBE = new OBE.DAL.DALOBE();
                    objOBE.AddNewLearningActivity(courseActivities, Created_By);
                    return Json(1, JsonRequestBehavior.AllowGet);
                }

                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete_Course(int Course_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    objLearn.Delete_Course(Course_Id);

                    return RedirectToAction("Syllabus_Course_List");
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }
        }


        public ActionResult Delete_LActivity(int LActId)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {

                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    objfaculty.Delete_LActivity(LActId, Created_By);
                    return Json(1, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        public ActionResult ESEQuestionMapping()
        {

            if (Session["Log_Id"] != null)
            {

                try
                {
                    if (isUserAuthenticated())
                    {
                        int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                        Valuation lstVal = new Valuation();
                        List<Valuation> lst = objfaculty.getcourseTeacherCourse(Faculty_Id).ToList();
                        lstVal.lstValuation = lst;
                        return View(lstVal);

                    }
                    else
                    {

                        return RedirectToAction("Faculty_Login", "Faculty", new { area = "Faculty" });

                    }
                }
                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return RedirectToAction("Faculty_Login", "Faculty", new { area = "Faculty" });
            }
        }

        #region Club Attendance

        public ActionResult Club_Attendance_Home()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    List<Clubs> attn = objfaculty.get_Facultywise_Club(Faculty_Id).ToList();
                    return View(attn);
                }

                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult Club_Attendance(int ClubId)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    DateTime Today = DateTime.Now.Date;
                    ViewBag.Date = Today;
                    ViewBag.ClubId = ClubId;
                    ViewBag.ClubName = db.CMS_Clubss.Where(x => x.Club_Id == ClubId && x.Active_Status == true).Select(x => x.Club).FirstOrDefault();
                    Clubs clb = new Clubs();
                    List<Clubs> attn = objfaculty.get_Club_Students(Today, ClubId, Faculty_Id).ToList();
                    clb.lstClub = attn;
                    return View(clb);
                }
                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }


        public ActionResult Add_Club_Attendance(Clubs att)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int FacId = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    int retVal = objfaculty.Add_Club_Attendance(att, Created_By, FacId);
                    return Json(retVal, JsonRequestBehavior.AllowGet);


                }
                catch (Exception ec)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult get_Club_Attendance(int ClubId, DateTime Date)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    DateTime Today = DateTime.Now.Date;
                    Clubs clb = new Clubs();
                    List<Clubs> attn = objfaculty.get_Club_Students(Date, ClubId, Faculty_Id).ToList();
                    return Json(attn, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult Club_Students(int ClubId)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    ViewBag.ClubId = ClubId;
                    ViewBag.ClubName = db.CMS_Clubss.Where(x => x.Club_Id == ClubId && x.Active_Status == true).Select(x => x.Club).FirstOrDefault();
                    return View();
                }
                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }
        #endregion

        #region Promotion Documents

        public ActionResult Promotion_Documents()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {


                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Faculty_Login");

            }
        }

        #endregion

        public ActionResult Teacher_Specific_Content(int Course_Sem_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    List<FYUGP_TSC> moduleDetails = db.FYUGP_TSCs.Where(x => x.Course_Sem_Id == Course_Sem_Id && x.Active_Status == true).ToList();
                    ViewBag.Course_Sem_Id = Course_Sem_Id;
                    ViewBag.Course_Name = (from a in db.CMS_Course_Semesters
                                           join b in db.CMS_Courses on a.Course_Id equals b.Course_Id
                                           where a.Course_Sem_Id == Course_Sem_Id && b.Active_Status == true
                                           select b.Course_Name).FirstOrDefault() + "( " + (from a in db.CMS_Course_Semesters
                                                                                            join b in db.CMS_Courses on a.Course_Id equals b.Course_Id
                                                                                            where a.Course_Sem_Id == Course_Sem_Id && b.Active_Status == true
                                                                                            select a.Category).FirstOrDefault() + " )";
                    ViewBag.EvalMethods = new SelectList(objfaculty.getEvaluationMethods(), "Eval_Id", "Method");
                    if (moduleDetails.Count() == 0)
                    {
                        int courseID = db.CMS_Course_Semesters.Where(x => x.Course_Sem_Id == Course_Sem_Id).Select(x => x.Course_Id).FirstOrDefault();
                        var courseSemId = db.FYUGP_TSCs
                     .Where(x => x.Course_Id == courseID && x.Active_Status)
                     .OrderByDescending(x => x.Course_Sem_Id) // pick latest
                     .Select(x => x.Course_Sem_Id)
                     .FirstOrDefault();
                        if (courseSemId > 0)
                        {
                            moduleDetails = db.FYUGP_TSCs.Where(x => x.Course_Sem_Id == courseSemId && x.Active_Status == true).ToList();
                            ViewBag.Status = "DataFound";
                            var evalMethods = db.FYUG_TSC_Eval_Methods.Where(x => x.Course_Sem_Id == Course_Sem_Id && x.Active_Status).ToList();

                            if (!evalMethods.Any())
                            {
                                // Copy from previous Course_Sem_Id
                                CopyEvaluationMethods(courseSemId, Course_Sem_Id);
                            }


                        }
                    }

                    ViewBag.SelectedEvalMethods = db.FYUG_TSC_Eval_Methods
                                               .Where(x => x.Course_Sem_Id == Course_Sem_Id && x.Active_Status)
                                               .Select(x => new
                                               {
                                                   Eval_Id = x.Eval_Id,
                                                   Eval_Method = x.Eval_Method
                                               }).ToList();





                    return View(moduleDetails);

                }

                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }


        public JsonResult CopyEvaluationMethods(int sourceCourseSemId, int targetCourseSemId)
        {
            try
            {
                var methods = db.FYUG_TSC_Eval_Methods
                    .Where(x => x.Course_Sem_Id == sourceCourseSemId && x.Active_Status)
                    .ToList();

                if (!methods.Any())
                {
                    return Json(new { status = false, message = "No evaluation methods found to copy." });
                }

                foreach (var item in methods)
                {
                    // Prevent duplicate entries
                    bool exists = db.FYUG_TSC_Eval_Methods.Any(x =>
                        x.Course_Sem_Id == targetCourseSemId &&
                        x.Eval_Id == item.Eval_Id &&
                        x.Active_Status);

                    if (!exists)
                    {
                        int count = db.FYUG_TSC_Eval_Methods.Count();
                        db.FYUG_TSC_Eval_Methods.Add(new FYUG_TSC_Eval_Method
                        {
                            TSC_Eval_Id = count++,
                            Course_Sem_Id = targetCourseSemId,
                            Eval_Id = item.Eval_Id,
                            Eval_Method = item.Eval_Method,
                            Created_Date = DateTime.Now,
                            Created_By = new Guid(Session["Log_Id"].ToString()),
                            Active_Status = true
                        });
                    }
                }

                db.SaveChanges();

                return Json(new
                {
                    status = true,
                    message = "Evaluation methods copied successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult AddNewCourseDescription(List<FYUGP_TSC> Modules)
        {
            if (Modules == null || Modules.Count == 0)
            {
                return Json(new { success = false, message = "No modules received." });
            }

            try
            {
                int count = db.FYUGP_TSCs.Count();
                int courseSemId = Modules.Select(x => x.Course_Sem_Id).FirstOrDefault();
                int courseID = db.CMS_Course_Semesters.Where(x => x.Course_Sem_Id == courseSemId).Select(x => x.Course_Id).FirstOrDefault();

                foreach (var module in Modules)
                {

                    // Check if record already exists
                    var existing = db.FYUGP_TSCs
                        .FirstOrDefault(x =>
                            x.TSC_Id == module.TSC_Id
                             && x.Course_Sem_Id == courseSemId && x.Active_Status);

                    if (existing != null)
                    {
                        // --------- UPDATE ---------
                        existing.Module = module.Module;
                        existing.SubModule = module.SubModule;
                        existing.Description = module.Description;
                        existing.Hours = module.Hours;
                        existing.CO = module.CO;
                        existing.Updated_By = new Guid(Session["Log_Id"].ToString());
                        existing.Updated_Date = DateTime.Now;
                    }
                    else
                    {
                        count++;
                        // map to entity
                        var entity = new FYUGP_TSC
                        {
                            TSC_Id = count,
                            Course_Id = courseID,
                            Course_Sem_Id = module.Course_Sem_Id,
                            Module = module.Module,
                            SubModule = module.SubModule,
                            Description = module.Description,
                            Hours = module.Hours,
                            CO = module.CO,
                            Created_By = new Guid(Session["Log_Id"].ToString()),
                            Created_Date = DateTime.Now,
                            Active_Status = true
                        };

                        db.FYUGP_TSCs.Add(entity);
                    }
                }

                db.SaveChanges(); // save to database

                return Json(new { success = true, message = "Modules saved successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult AddEvaluationMethod(FYUG_TSC_Eval_Method model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Eval_Method))
                {
                    return Json(new { status = false, message = "Invalid evaluation method." });
                }

                int count = db.FYUG_TSC_Eval_Methods.Count();
                // Create new record
                var entity = new FYUG_TSC_Eval_Method
                {
                    TSC_Eval_Id = count++,
                    Course_Sem_Id = model.Course_Sem_Id,
                    Eval_Method = model.Eval_Method,
                    Eval_Id = model.Eval_Id,
                    Created_Date = DateTime.Now,
                    Created_By = new Guid(Session["Log_Id"].ToString()),
                    Active_Status = true
                };

                db.FYUG_TSC_Eval_Methods.Add(entity);
                db.SaveChanges();

                return Json(new
                {
                    status = true,
                    message = "Evaluation method added successfully.",
                    id = entity.TSC_Eval_Id
                });

            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Error: " + ex.Message });
            }
        }
        [HttpPost]
        public JsonResult DeleteEvaluationMethod(int Course_Sem_Id, int Eval_Id)
        {
            try
            {
                var record = db.FYUG_TSC_Eval_Methods
                                .FirstOrDefault(x => x.Course_Sem_Id == Course_Sem_Id &&
                                                     x.Eval_Id == Eval_Id &&
                                                     x.Active_Status);

                if (record == null)
                {
                    return Json(new { status = false, message = "Record not found." });
                }

                // Soft Delete (recommended)
                record.Active_Status = false;
                record.Updated_Date = DateTime.Now;
                record.Updated_By = new Guid(Session["Log_Id"].ToString()); // Optional

                db.SaveChanges();

                return Json(new { status = true, message = "Evaluation method removed successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteModule(int Module_Id)
        {
            try
            {

                var record = db.FYUGP_TSCs.Where(x => x.TSC_Id == Module_Id).FirstOrDefault();
                if (record != null)
                {
                    record.Active_Status = false;
                    record.Updated_Date = DateTime.Now;
                    record.Updated_By = new Guid(Session["Log_Id"].ToString());
                    db.SaveChanges();
                    return Json(new { status = true, message = "Module deleted successfully." });
                }
                else
                {
                    return Json(new { status = false, message = "Module not found." });
                }

            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Error: " + ex.Message });
            }
        }


        public ActionResult Makeup_Activity_List()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    // 🚫 Block for specific date
                    DateTime dt = DateTime.Now;
                    //if (dt.Date >= new DateTime(2026, 01, 28))
                    //{
                    //    return Content(@"<script type='text/javascript'>alert('Access Denied: This view is not available for faculty after 28-01-2026.');window.location.href = '/Faculty/Faculty/Faculty_Dashboard';</script>", "text/html");
                    //}
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    ViewBag.Dep_Id = db.CMS_Facultys.Where(x => x.Faculty_Id == Faculty_Id).Select(x => x.Dep_Id).FirstOrDefault();
                    ViewBag.Programmes_Type = new SelectList(objFYUGP.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Updated_Year = new SelectList(objFYUGP.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");

                    int Acc_Yr = db.CMS_AcademicYears.Where(x => x.Start_Date <= dt && x.End_Date >= dt).Select(x => x.Acc_yr_Id).FirstOrDefault();
                    ViewBag.Year = Acc_Yr;
                    List<AttendanceReport> data = objfaculty.getProgrammesFacultyWise(Faculty_Id).ToList();

                    bool isFaculty = data != null && data.Count > 0;
                    ViewBag.isFaculty = isFaculty;

                    var hodRecord = db.CMS_HODs.Where(h => h.Faculty_Id == Faculty_Id && h.Acc_Yr == Acc_Yr && h.Active_Status).FirstOrDefault();

                    int Hod_Id = hodRecord != null ? hodRecord.HOD_Id : 0;

                    bool isHod = Hod_Id > 0;


                    bool isClassWarden = (
                        from cw in db.CMS_ClassWardens
                        join sem in db.CMS_AccademicYearSemesters
                            on cw.Acc_Yr_sem_Id equals sem.Acc_Yr_Sem_Id
                        where cw.Faculty_Id == Faculty_Id
                              && cw.Active_Status == true
                              && sem.Acc_yr_Id == Acc_Yr
                        select cw
                    ).Any();

                    bool isHodOrClassWarden = isHod || isClassWarden;

                    ViewBag.IsHodOrClassWarden = isHodOrClassWarden;

                    var programmeTypes = data
                        .SelectMany(x => x.report1)
                        .GroupBy(x => new { x.Pgm_Type_Id, x.Programme_Type })
                        .Select(g => new SelectListItem
                        {
                            Value = g.Key.Pgm_Type_Id.ToString(),
                            Text = g.Key.Programme_Type
                        })
                        .OrderBy(x => x.Text)
                        .ToList();


                    var semesters = data
                        .SelectMany(x => x.report1)
                        .GroupBy(x => new { x.Acc_Yr_Sem_Id, x.Class, x.Sem })
                        .Select(g => new SelectListItem
                        {
                            Value = g.Key.Acc_Yr_Sem_Id.ToString(),
                            Text = g.Key.Class + " - " + g.Key.Sem
                        })
                        .OrderBy(x => x.Text)
                        .ToList();

                    ViewBag.ProgrammeTypes = programmeTypes;
                    ViewBag.Semesters = semesters;

                    return View(data);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }
        }

        public JsonResult GetCoursesBySemester(int AccYrSemId)
        {
            int facultyId = Convert.ToInt32(Session["Faculty_Id"]);

            var lstCourses = (
                from a in db.CMS_Course_Teachers
                join b in db.CMS_Course_Semesters on a.Course_Sem_Id equals b.Course_Sem_Id
                join c in db.CMS_AcademicYr_Sem_Programmes on b.Acc_Yr_Sem_Pgm_Id equals c.Acc_Yr_Sem_Pgm_Id
                join d in db.CMS_Courses on b.Course_Id equals d.Course_Id
                join e in db.CMS_Programmes on c.Pgm_Id equals e.Pgm_Id
                where a.Faculty_Id == facultyId && c.Acc_Yr_sem_Id == AccYrSemId && a.Active_Status == true && b.Active_Status == true && d.Active_Status == true && c.Active_Status == true &&
                (b.Course_Nature_Type == "Theory" || b.Course_Nature_Type == "Theory With Practical" || b.Course_Nature_Type == "Theory With Practicum")
                select new
                {
                    a.Course_Sem_Id,
                    CourseText = (c.Pgm_Id == 15 || c.Pgm_Id == 17)
                                ? b.Category + " - " + d.Course_Name + " - " + e.Programme_hons
                                : b.Category + " - " + d.Course_Name
                }
            ).Distinct().ToList();

            return Json(lstCourses, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Makeup_Activity_CourseWiseAttendance(int Acc_Yr_Sem_Id, string Report_Type, string sdate, string edate)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    DateTime SDate = Convert.ToDateTime(sdate);
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    DateTime date;
                    if (Acc_Yr_Sem_Id == 120)
                    {
                        if (DateTime.TryParseExact(sdate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                        {
                            if (date.Month == 11)
                            {
                                SDate = db.CMS_AccademicYearSemesters.Where(x => x.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id && x.Active_Status == true).Select(x => x.Start_Date).FirstOrDefault();
                            }
                            else
                            {
                                SDate = Convert.ToDateTime(sdate);
                            }
                        }
                    }
                    else if (Acc_Yr_Sem_Id == 122 || Acc_Yr_Sem_Id == 124)
                    {
                        if (DateTime.TryParseExact(sdate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                        {
                            if (date.Month == 12)
                            {
                                SDate = db.CMS_AccademicYearSemesters.Where(x => x.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id && x.Active_Status == true).Select(x => x.Start_Date).FirstOrDefault();
                            }
                            else
                            {
                                SDate = Convert.ToDateTime(sdate);
                            }
                        }
                    }


                    DateTime EDate = Convert.ToDateTime(edate);

                    var Run = db.CMS_Makeup_Activity_Report_Runss.Where(x => x.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id && x.StartDate == SDate && x.EndDate == EDate && x.Active_Status == true).OrderByDescending(r => r.RunId).FirstOrDefault();
                    bool isClosed = DateTime.Now > Run.Close_Date;

                    List<AttendanceReport> report = new List<AttendanceReport>();
                    int Hod_Id = Session["hodRecord"] != null ? Convert.ToInt32(Session["hodRecord"].ToString()) : 0;
                    string facultyRole = "ClassWarden";
                    if (Hod_Id > 0)
                    {
                        report = objfaculty.Makeup_Activity_CourseWiseAttendance(Acc_Yr_Sem_Id, Report_Type, SDate, EDate, Faculty_Id).ToList();
                        facultyRole = "ClassWarden";
                        if (report.Count() == 0)
                        {
                            report = objfaculty.Makeup_Activity_CourseWiseAttendance_HOD(Acc_Yr_Sem_Id, Report_Type, SDate, EDate, Faculty_Id).ToList();
                            facultyRole = "HOD";
                        }

                    }
                    else
                    {
                        report = objfaculty.Makeup_Activity_CourseWiseAttendance(Acc_Yr_Sem_Id, Report_Type, SDate, EDate, Faculty_Id).ToList();
                        facultyRole = "ClassWarden";
                    }

                    return Json(new { Role = facultyRole, Students = report, IsClosed = isClosed, }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }
        }

        public JsonResult GetMakeupActivities(int Course_Sem_Id, string sdate, string edate, int Acc_Yr_Sem_Id)
        {
            DateTime SDate = Convert.ToDateTime(sdate);
            int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
            DateTime date;
            if (Acc_Yr_Sem_Id == 120)
            {
                if (DateTime.TryParseExact(sdate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    if (date.Month == 11)
                    {
                        SDate = db.CMS_AccademicYearSemesters.Where(x => x.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id && x.Active_Status == true).Select(x => x.Start_Date).FirstOrDefault();
                    }
                    else
                    {
                        SDate = Convert.ToDateTime(sdate);
                    }
                }
            }
            else if (Acc_Yr_Sem_Id == 122 || Acc_Yr_Sem_Id == 124)
            {
                if (DateTime.TryParseExact(sdate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    if (date.Month == 12)
                    {
                        SDate = db.CMS_AccademicYearSemesters.Where(x => x.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id && x.Active_Status == true).Select(x => x.Start_Date).FirstOrDefault();
                    }
                    else
                    {
                        SDate = Convert.ToDateTime(sdate);
                    }
                }
            }


            DateTime EDate = Convert.ToDateTime(edate);
            var records = (
                from m in db.CMS_Makeup_Activity_Records
                join u in db.CMS_UPRNs on m.UPRN equals u.UPRN
                join s in db.CMS_Students on u.Admission_No equals s.Admission_No
                join r in db.CMS_Makeup_Activity_Report_Runss on m.RunId equals r.RunId
                where m.Course_Sem_Id == Course_Sem_Id && u.Active_Status == true && s.Active_Status == true && m.Percentage < 75 && r.StartDate == SDate && r.EndDate == EDate && m.Status != "0" && m.Status != "-1" && m.Status != "Previously Generated"
                select new
                {
                    StudentName = s.Name,
                    UPRN = u.UPRN,
                    Hours = m.TotalHours,
                    Present = m.Present,
                    Percentage = m.Percentage,
                    Status = m.Status,
                    RunId = m.RunId
                }
            ).Distinct().ToList();

            return Json(records, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Makeup_Activity_List_Dean()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    ViewBag.Programmes_Type = new SelectList(objFYUGP.getAllProgramme(), "Pgm_Type_Id", "Pgm_Type");
                    ViewBag.Updated_Year = new SelectList(objFYUGP.getAllUpdatedAcademicYears(), "Acc_Yr_Id", "Year");
                    DateTime dt = DateTime.Now;
                    ViewBag.Year = db.CMS_AcademicYears.Where(x => x.Start_Date <= dt && x.End_Date >= dt).Select(x => x.Acc_yr_Id).FirstOrDefault();
                    return View();
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }
        }

        public ActionResult Makeup_Activity_CourseWiseAttendance_Dean(int Acc_Yr_Sem_Id, string Report_Type, string sdate, string edate)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    DateTime SDate = Convert.ToDateTime(sdate);
                    DateTime date;
                    if (Acc_Yr_Sem_Id == 120)
                    {
                        if (DateTime.TryParseExact(sdate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                        {
                            if (date.Month == 11)
                            {
                                SDate = db.CMS_AccademicYearSemesters.Where(x => x.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id && x.Active_Status == true).Select(x => x.Start_Date).FirstOrDefault();
                            }
                            else
                            {
                                SDate = Convert.ToDateTime(sdate);
                            }
                        }
                    }
                    else if (Acc_Yr_Sem_Id == 122 || Acc_Yr_Sem_Id == 124)
                    {
                        if (DateTime.TryParseExact(sdate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                        {
                            if (date.Month == 12)
                            {
                                SDate = db.CMS_AccademicYearSemesters.Where(x => x.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id && x.Active_Status == true).Select(x => x.Start_Date).FirstOrDefault();
                            }
                            else
                            {
                                SDate = Convert.ToDateTime(sdate);
                            }
                        }
                    }


                    DateTime EDate = Convert.ToDateTime(edate);

                    //int Pgm_Id = db.CMS_AcademicYr_Sem_Programmes.Where(x => x.Acc_Yr_Sem_Pgm_Id == Acc_Yr_Sem_Pgm_Id).Select(x => x.Pgm_Id).FirstOrDefault();
                    var report = objfaculty.Makeup_Activity_CourseWiseAttendance_Dean(Acc_Yr_Sem_Id, Report_Type, SDate, EDate).ToList();

                    return Json(report, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }
        }

        [HttpPost]
        public JsonResult Mark_Activity_Completed(StudentActivityUpdate student)
        {
            if (Session["Log_Id"] == null)
                return Json(new { success = false, message = "Session expired" });

            try
            {
                Guid Log_Id = new Guid(Session["Log_Id"].ToString());
                string UPRN = student.UPRN.ToString();

                int RunId = db.CMS_Makeup_Activity_Report_Runss
                    .Where(r => r.RunId == student.RunId)
                    .OrderByDescending(r => r.RunId)
                    .Select(r => r.RunId)
                    .FirstOrDefault();

                // Find existing record for the single course
                var record = db.CMS_Makeup_Activity_Records.FirstOrDefault(r =>
                    r.UPRN == UPRN &&
                    r.Course_Id == student.Course_Id &&
                    r.Course_Sem_Id == student.Course_Sem_Id &&
                    r.RunId == RunId
                );

                if (record != null)
                {
                    record.Status = "Completed";
                    record.UpdatedOn = DateTime.Now;
                    record.UpdatedBy = Log_Id;
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult Mark_Activity_NotCompleted(StudentActivityUpdate student)
        {
            if (Session["Log_Id"] == null)
                return Json(new { success = false, message = "Session expired" });

            try
            {
                Guid Log_Id = new Guid(Session["Log_Id"].ToString());
                string UPRN = student.UPRN.ToString();

                int RunId = db.CMS_Makeup_Activity_Report_Runss
                    .Where(r => r.RunId == student.RunId)
                    .OrderByDescending(r => r.RunId)
                    .Select(r => r.RunId)
                    .FirstOrDefault();

                // Find existing record for the single course
                var record = db.CMS_Makeup_Activity_Records.FirstOrDefault(r =>
                    r.UPRN == UPRN &&
                    r.Course_Id == student.Course_Id &&
                    r.Course_Sem_Id == student.Course_Sem_Id &&
                    r.RunId == RunId
                );

                if (record != null)
                {
                    record.Status = "Not Completed";
                    record.UpdatedOn = DateTime.Now;
                    record.UpdatedBy = Log_Id;
                }

                db.SaveChanges();
                return Json(new { success = true });
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public ActionResult Makeup_Activity_Course_Teacher()
        {
            DateTime dt = DateTime.Now;
            int Year = db.CMS_AcademicYears.Where(x => x.Start_Date <= dt && x.End_Date >= dt).Select(x => x.Acc_yr_Id).FirstOrDefault();
            int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"]);

            var data = objfaculty.GetSemesterClassList(Faculty_Id, Year);

            return Json(data, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetSemesterClassList()
        {
            DateTime dt = DateTime.Now;
            int Year = db.CMS_AcademicYears.Where(x => x.Start_Date <= dt && x.End_Date >= dt).Select(x => x.Acc_yr_Id).FirstOrDefault();
            int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"]);

            var data = objfaculty.GetSemesterClassList(Faculty_Id, Year);

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        #region Exam Attendance

        public ActionResult Exam_Attendance()
        {

            if (Session["Log_Id"] != null)
            {
                if (isUserAuthenticated())
                {
                    try
                    {
                        DateTime today = DateTime.Today;
                        var ReSchexamDate = db.Exam_Reschedules
                            .Where(x => x.Rescheduled_Date == today)
                            .Select(x => x.Exam_Date)
                            .FirstOrDefault();

                        if (ReSchexamDate != default(DateTime))
                        {
                            today = ReSchexamDate.Date;
                        }

                        ViewBag.ExamDate = today.ToString("yyyy-MM-dd");
                        string session = DateTime.Now.Hour < 12 ? "FN" : "AN";
                        ViewBag.TSession = DateTime.Now.Hour < 12 ? "FN" : "AN";
                        TimeSpan now = DateTime.Now.TimeOfDay;
                        int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"]);
                        int roomId = (from a in db.ING_Schedules
                                      join b in db.ING_ExamDetails
                                          on a.Exam_Id equals b.Id
                                      join c in db.ING_Invigilators on a.Invigilator_Id equals c.Id
                                      where b.ExamDate == today
                                            && b.Session == session
                                            && c.Fac_Id == Faculty_Id
                                            && a.Active_Status && b.Active_Status
                                            && now >= a.Attendance_Start_Time && now <= a.Attendance_End_Time
                                      select a.Room_Id).FirstOrDefault();


                        //   COE.DAL.DALCOE objcoe = new COE.DAL.DALCOE();
                        //  List<Allocation> strength = objcoe.getExaminationAttendanceRecord(today, session, roomId).ToList();
                        // return View(strength);
                        return View();
                    }
                    catch
                    {
                        return Redirect("~/Login/Error_Page");
                    }
                }
                else
                {
                    return Redirect("Faculty_Login");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }
        }


        [HttpPost]
        public ActionResult getExaminationAttendanceRecord(DateTime Date, string exmSession)
        {
            if (Session["Log_Id"] != null)
            {

                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"]);
                    TimeSpan now = DateTime.Now.TimeOfDay;
                    int roomId = (from a in db.ING_Schedules
                                  join b in db.ING_ExamDetails
                                      on a.Exam_Id equals b.Id
                                  join c in db.ING_Invigilators on a.Invigilator_Id equals c.Id
                                  where b.ExamDate == Date && a.Active_Status && b.Active_Status
                                        && b.Session == exmSession
                                        && c.Fac_Id == Faculty_Id
                                          && now >= a.Attendance_Start_Time && now <= a.Attendance_End_Time
                                  select a.Room_Id).FirstOrDefault();
                    COE.DAL.DALCOE objcoe = new COE.DAL.DALCOE();
                    List<Allocation> strength = objcoe.getExaminationAttendanceRecord(Date, exmSession, roomId).ToList();

                    return Json(strength, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult Mark_Attendance(string Date, int Room_No, string ExmSession)
        {

            if (Session["Log_Id"] != null)
            {

                try
                {
                    DateTime date = Convert.ToDateTime(Date);
                    COE.DAL.DALCOE objcoe = new COE.DAL.DALCOE();
                    List<Allocation> strength = objcoe.getExaminationAttendanceRecord_Roomwise(date, Room_No, ExmSession).ToList();
                    return View(strength);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }
        }


        public ActionResult Print_RoomWiseAttendance(string Date, int Room_No, string ExmSession)
        {

            if (Session["Log_Id"] != null)
            {

                try
                {
                    DateTime date = Convert.ToDateTime(Date);
                    COE.DAL.DALCOE objcoe = new COE.DAL.DALCOE();
                    List<Allocation> strength = objcoe.getExaminationAttendanceRecord_Roomwise(date, Room_No, ExmSession).ToList();
                    return View(strength);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }
        }

        #endregion


        public ActionResult Previous_QuestionPaper()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    List<Internals> intrnl = objfaculty.get_Faculty_CourseList(Faculty_Id).ToList();
                    return View(intrnl);

                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }

        }
        public ActionResult Previous_QuestionPaperView(string CourseCode)
        {
            if (Session["Log_Id"] == null)
                return Redirect("~/Login/Login");

            try
            {
                int facultyId = Convert.ToInt32(Session["Faculty_Id"]);

                var model = objfaculty
                                .get_Faculty_QPList(CourseCode)
                                .ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                return Redirect("~/Login/Error_Page");
            }
        }



        public ActionResult DownloadWordFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return HttpNotFound();

            // Security: remove path traversal
            fileName = Path.GetFileName(fileName);

            string filePath = Server.MapPath("~/Images/QP_Upload/" + fileName);

            if (!System.IO.File.Exists(filePath))
                return HttpNotFound();

            return File(filePath,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileName);
        }


        public ActionResult ESE_SectionMark(string CourseCode, string Course_Type, string Exam_Type, string Category, string NotNo)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    if (isUserAuthenticated())
                    {
                        int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                        ViewBag.CourseCode = CourseCode;
                        ViewBag.CourseName = db.CMS_Courses.Where(x => x.Course_Code == CourseCode && x.Active_Status == true).Select(x => x.Course_Name).FirstOrDefault();
                        ViewBag.Course_Type = Course_Type;
                        ViewBag.Exam_Type = Exam_Type;
                        ViewBag.Category = Category;
                        ViewBag.NotNo = NotNo;
                        int AccYr = objfaculty.getSpecificAcademicYear().Acc_yr_Id;
                        if (Exam_Type == "SemExam" && Course_Type == "Theory")
                        {
                            var data = (from esm in db.Exam_Section_Marks
                                        join cqs in db.Exam_QuestionSections
                                            on esm.Sec_Id equals cqs.Sec_Id
                                        where esm.Course_Code == CourseCode
                                              && esm.Exam_Type == Exam_Type
                                              && esm.Course_Type == Course_Type
                                              && esm.Active_Status
                                              && esm.Acc_Yr_Id == AccYr
                                        select new
                                        {
                                            esm.Sec_Id,
                                            cqs.Section,
                                            esm.Mark,
                                            esm.Total_Questions,
                                            esm.Max_Questions,
                                            esm.Qn_Type
                                        }).ToList();
                            ViewBag.SectionData = JsonConvert.SerializeObject(data);
                        }
                        return View();
                    }
                    else
                    {
                        return Redirect("Faculty_Login");
                    }
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        public ActionResult SemExamQuestionMapping(string CourseCode, string Course_Type, string NotNo)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    // if (isUserAuthenticated())
                    //  {
                    ViewBag.CourseCode = CourseCode;
                    ViewBag.NotNo = NotNo;
                    var examDet = db.Exam_Notifications.Where(x => x.Notification_No == NotNo && x.Active_Status == true).FirstOrDefault();
                    ViewBag.ExamName = examDet.Exam_Name;
                    ViewBag.CourseName = db.CMS_Courses.Where(x => x.Course_Code == CourseCode && x.Active_Status == true).Select(x => x.Course_Name).FirstOrDefault();
                    ViewBag.CourseType = Course_Type;
                    COE.DAL.DALCOE objcoe = new COE.DAL.DALCOE();
                    ViewBag.Section = new SelectList(objcoe.getCoursesSection(CourseCode, Course_Type), "Sec_Id", "Section");
                    var Role = Session["Role"].ToString();
                    ViewBag.Role = Role;



                    return View();
                    // }
                    //else
                    //{
                    //    return Redirect("Login");
                    //}
                }

                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("Faculty_Login");

            }
        }

        [HttpPost]
        public JsonResult AutoSaveRow(CMS_FYUGP_Question_Mapping model, string NotNo)
        {
            try
            {
                Guid userId = new Guid(Session["Log_Id"].ToString());

                var notification = db.Exam_Notifications
                                     .FirstOrDefault(x => x.Notification_No == NotNo
                                                       && x.Active_Status);

                if (notification == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Notification not found"
                    });
                }

                bool isSpecial = notification.Category == "Special";

                if (isSpecial)
                {
                    SaveSpecialExamMapping(model, userId, NotNo);
                }
                else
                {
                    SaveRegularMapping(model, userId);
                }

                return Json(new
                {
                    success = true,
                    message = "Saved successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        private void SaveSpecialExamMapping(CMS_FYUGP_Question_Mapping model, Guid userId, string NotNo)
        {
            var existing = db.CMS_FYUGP_SpecialExam_Mappings
                             .FirstOrDefault(x =>
                                 x.Course_Code == model.Course_Code &&
                                 x.Section_Id == model.Section_Id &&
                                 x.Question_No == model.Question_No &&
                                 x.Not_No == NotNo &&
                                 x.Active_Status);

            if (existing != null)
            {
                UpdateMapping(existing, model, userId);
            }
            else
            {
                int nextId = (db.CMS_FYUGP_SpecialExam_Mappings
                                .Max(x => (int?)x.Qns_Map_Id) ?? 0) + 1;

                var entity = new CMS_FYUGP_SpecialExam_Mapping
                {
                    Qns_Map_Id = nextId,
                    Course_Code = model.Course_Code,
                    Section_Id = model.Section_Id,
                    Question_No = model.Question_No,
                    Module = model.Module,
                    Unit = model.Unit,
                    CO = model.CO,
                    Learning_Domain = model.Learning_Domain,
                    Difficulty_Level = model.Difficulty_Level,
                    Created_By = userId,
                    Created_On = DateTime.Now,
                    Not_No = NotNo,
                    Course_Type = model.Course_Type,
                    Active_Status = true
                };

                db.CMS_FYUGP_SpecialExam_Mappings.Add(entity);
            }

            db.SaveChanges();
        }

        private void SaveRegularMapping(CMS_FYUGP_Question_Mapping model, Guid userId)
        {
            var existing = db.CMS_FYUGP_Question_Mappings
                             .FirstOrDefault(x =>
                                 x.Course_Code == model.Course_Code &&
                                 x.Section_Id == model.Section_Id &&
                                 x.Question_No == model.Question_No &&
                                 x.Active_Status);

            if (existing != null)
            {
                UpdateMapping(existing, model, userId);
            }
            else
            {
                int nextId = (db.CMS_FYUGP_Question_Mappings
                                .Max(x => (int?)x.Qns_Map_Id) ?? 0) + 1;

                model.Qns_Map_Id = nextId;
                model.Created_By = userId;
                model.Created_On = DateTime.Now;
                model.Active_Status = true;

                db.CMS_FYUGP_Question_Mappings.Add(model);
            }

            db.SaveChanges();
        }

        private void UpdateMapping(dynamic existing, CMS_FYUGP_Question_Mapping model, Guid userId)
        {
            existing.Module = model.Module;
            existing.Unit = model.Unit;
            existing.CO = model.CO;
            existing.Learning_Domain = model.Learning_Domain;
            existing.Difficulty_Level = model.Difficulty_Level;
            existing.Modified_By = userId;
            existing.Modified_On = DateTime.Now;
        }


        #region MCQ 
        public ActionResult MCQView()
        {
            if (Session["Log_Id"] != null)
            {
                try
                {

                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
                    List<Exam_Online> online = objfaculty.getMcqQuestions(Faculty_Id).ToList();
                    return View(online);


                }
                catch (Exception ex)
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {

                return Redirect("Faculty_Login");
            }
        }
        public ActionResult ViewMcqQuestions(string Course_Code)
        {
            ViewBag.Course_Code = Course_Code;
            int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());
            var courses = objfaculty.getMcqQuestions(Faculty_Id)
    .SelectMany(x => x.online)
    .GroupBy(x => new { x.Course_Code, x.Course_Title })
    .Select(g => new
    {
        Course_Code = g.Key.Course_Code,
        Course_Name = g.Key.Course_Title
    })
    .ToList();
            ViewBag.CourseList = new SelectList(
       courses,
       "Course_Code",
       "Course_Name",
       Course_Code
   );


            return View();
        }

        public ActionResult getAllMCQQuestions(string Code)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    Guid Created_By = new Guid(Session["Log_Id"].ToString());
                    List<Exam_Online> P = new List<Exam_Online>();

                    P = objfaculty.getAllMCQQuestions(Code).ToList();

                    return Json(P, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult MCQ_Upload_Question(string Course_Code)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    ViewBag.Course_Code = Course_Code;
                    ViewBag.Course_Name = db.CMS_Courses.Where(x => x.Course_Code == Course_Code && x.Active_Status == true).Select(x => x.Course_Name).FirstOrDefault();
                    int Faculty_Id = Convert.ToInt32(Session["Faculty_Id"].ToString());

                    return View();


                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {

                return Redirect("Faculty_Login");
            }
        }

        [HttpPost]
        public ActionResult Add_MCQ_Questions(Exam_Online cs)
        {

            try
            {

                if (Session["Log_Id"] != null)
                {
                    Conversion convert = new Conversion();
                    cs.Created_By = new Guid(Session["Log_Id"].ToString());
                    int retVal = objfaculty.Add_MCQ_Questions(cs);
                    return Json(retVal, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpGet]
        public JsonResult CheckDuplicate(string courseCode, string question, int? questId = null)
        {
            if (string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(question))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            question = question.Trim().ToLower();

            var query = db.Exam_MCQ_QBs.Where(x =>
                x.Active_Status &&
                x.Course_Code == courseCode &&
                x.Question.Trim().ToLower() == question);

            // Ignore the current record while editing
            if (questId.HasValue)
            {
                query = query.Where(x => x.Quest_Id != questId.Value);
            }

            bool exists = query.Any();

            return Json(exists, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Add_Questions_Images(HttpPostedFileBase fname, string Course_Code)
        {
            try
            {
                if (Session["Log_Id"] == null || fname == null || fname.ContentLength == 0)
                    return Json(new { success = false });
                if (fname.ContentLength > 2 * 1024 * 1024)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Maximum upload size is 2 MB."
                    });
                }
                string ext = Path.GetExtension(fname.FileName).ToLower();

                string[] allowed = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

                if (!allowed.Contains(ext))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Only image files are allowed."
                    });
                }

                var accYr = objfaculty.getCurrentAcademicYear();

                string extension = Path.GetExtension(fname.FileName);

                string fileName = string.Format("{0}_{1}_{2}{3}",
                    accYr.Acc_yr_Id,
                    Course_Code,
                    Guid.NewGuid().ToString("N"),
                    extension);

                string physicalPath = Server.MapPath("~/Images/Qp_Photo/" + fileName);

                fname.SaveAs(physicalPath);

                return Json(new
                {
                    success = true,
                    fileName = fileName,
                    url = "/Images/Qp_Photo/" + fileName
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        public ActionResult Edit_MCQ_Question(int Quest_Id, string Course_Code)
        {
            var vm = new MCQEditVM();
            ViewBag.QuuestId = Quest_Id;
            vm.Question = db.Exam_MCQ_QBs
                            .FirstOrDefault(x => x.Quest_Id == Quest_Id);

            vm.Options = objfaculty.get_MCQQuestionsWithQuest_Id(Quest_Id)
                                   .OrderBy(x => x.Opt_Id)
                                   .ToList();
            int currentQuestId = Quest_Id;
            var allQuestions = objfaculty.getAllMCQQuestions(Course_Code).ToList();

            var currentIndex = allQuestions.FindIndex(x => x.Quest_Id == currentQuestId);

            int? nextQuestId = null;

            if (currentIndex >= 0 && currentIndex + 1 < allQuestions.Count)
            {
                nextQuestId = allQuestions[currentIndex + 1].Quest_Id;
            }

            ViewBag.NextQuestId = nextQuestId;

            return View(vm);
        }


        [HttpPost]
        public ActionResult Update_MCQ_Questions(Exam_Online model)
        {
            if (Session["Log_Id"] == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Session expired. Please login again."
                });
            }

            try
            {
                Guid createdBy = new Guid(Session["Log_Id"].ToString());
                model.Created_By = createdBy;
                objfaculty.Edit_MCQ_Questions(model);

                return Json(new
                {
                    success = true,
                    message = "Question updated successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        public ActionResult Delete_MCQ_Questions(int Quest_Id)
        {
            if (Session["Log_Id"] != null)
            {
                try
                {
                    objfaculty.Delete_MCQ_Questions(Quest_Id);

                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Redirect("~/Login/Error_Page");
                }
            }
            else
            {
                return Redirect("~/Login/Login");

            }
        }
        #endregion







        public ActionResult QB_Open_Close()
        {
            if (Session["Log_Id"] == null)
                return RedirectToAction("Login");

            ViewBag.AcademicYears = db.CMS_AcademicYears
                .Where(x => x.Active_Status)
                .OrderByDescending(x => x.Acc_yr_Id)
                .ToList();

            ViewBag.Semesters = db.CMS_Semesters
                .Where(x => x.Active_Status)
                .OrderBy(x => x.Sem_Id)
                .ToList();

            return View();
        }

        [HttpPost]

        public JsonResult LoadProgrammes(int Acc_Yr_Sem_Id)
        {
            var programmes = (from a in db.CMS_AcademicYr_Sem_Programmes
                              join b in db.CMS_Programmes
                                  on a.Pgm_Id equals b.Pgm_Id
                              where a.Acc_Yr_sem_Id == Acc_Yr_Sem_Id
                                    && a.Active_Status
                                    && b.Active_Status
                              orderby b.Programme
                              select new
                              {
                                  b.Pgm_Id,
                                  b.Programme
                              }).Distinct().ToList();

            return Json(programmes, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SaveQBStatus(List<int> ProgrammeIds,
                                   int Acc_Yr_Sem_Id,
                                   DateTime OpenDate,
                                   DateTime CloseDate,
                                   bool IsOpen)
        {
            try
            {
                Guid logId = new Guid(Session["Log_Id"].ToString());

                foreach (int pgmId in ProgrammeIds)
                {
                    var qb = db.CMS_QB_OPCL_Statuss
                        .FirstOrDefault(x => x.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id
                                          && x.Pgm_Id == pgmId
                                          && x.Active_Status);

                    if (qb == null)
                    {
                        qb = new CMS_QB_OPCL_Status();

                        qb.Acc_Yr_Sem_Id = Acc_Yr_Sem_Id;
                        qb.Pgm_Id = pgmId;
                        qb.Created_By = logId;
                        qb.Created_Date = DateTime.Now;
                        qb.Active_Status = true;

                        db.CMS_QB_OPCL_Statuss.Add(qb);
                    }

                    qb.Open_Date = OpenDate;
                    qb.Close_Date = CloseDate;
                    qb.Is_Open = IsOpen;
                    qb.Updated_By = logId;
                    qb.Updated_Date = DateTime.Now;
                }

                db.SaveChanges();

                return Json(new
                {
                    Status = true,
                    Message = "Saved Successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }
        [HttpPost]
        public JsonResult GetQBStatus(int Acc_Yr_Sem_Id)
        {
            try
            {
                DateTime today = DateTime.Today;

                var data = (from a in db.CMS_QB_OPCL_Statuss
                            join b in db.CMS_Programmes
                                on a.Pgm_Id equals b.Pgm_Id
                            where a.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id
                                  && a.Active_Status == true
                            select new
                            {
                                a.Id,
                                a.Pgm_Id,
                                b.Programme,
                                a.Open_Date,
                                a.Close_Date,
                                a.Is_Open
                            }).ToList();

                foreach (var item in data)
                {
                    var record = db.CMS_QB_OPCL_Statuss
                        .FirstOrDefault(x => x.Id == item.Id);

                    if (record != null)
                    {
                        // Before opening date = Closed
                        if (today < record.Open_Date.Value.Date)
                        {
                            record.Is_Open = false;
                        }
                        // Close date reached = Closed
                        else if (today >= record.Close_Date.Value.Date)
                        {
                            record.Is_Open = false;
                        }
                        // Between opening and closing dates = Open
                        else
                        {
                            record.Is_Open = true;
                        }
                    }
                }

                db.SaveChanges();

                // Reload after updating Is_Open
                var result = (from a in db.CMS_QB_OPCL_Statuss
                              join b in db.CMS_Programmes
                                  on a.Pgm_Id equals b.Pgm_Id
                              where a.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id
                                    && a.Active_Status == true
                              select new
                              {
                                  a.Id,
                                  a.Pgm_Id,
                                  b.Programme,
                                  a.Open_Date,
                                  a.Close_Date,
                                  a.Is_Open
                              }).ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Error = true,
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult DeleteQBStatus(int Id)
        {
            var qb = db.CMS_QB_OPCL_Statuss.FirstOrDefault(x => x.Id == Id);

            if (qb != null)
            {
                qb.Active_Status = false;
                qb.Updated_Date = DateTime.Now;
                qb.Updated_By = new Guid(Session["Log_Id"].ToString());

                db.SaveChanges();
            }

            return Json(true);
        }
        [HttpPost]
        public JsonResult LoadSemesters(int Acc_Yr_Id)
        {
            var semesters = (from a in db.CMS_AccademicYearSemesters
                             join b in db.CMS_Semesters
                                on a.Sem_Id equals b.Sem_Id
                             where a.Acc_yr_Id == Acc_Yr_Id
                                   && a.Active_Status == true
                             orderby a.Sem_Id
                             select new
                             {
                                 a.Acc_Yr_Sem_Id,
                                 b.Semester
                             }).ToList();

            return Json(semesters, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetQBProgrammeStatus(int Acc_Yr_Sem_Id, int Pgm_Id)
        {
            try
            {
                var qb = db.CMS_QB_OPCL_Statuss
                    .FirstOrDefault(x =>
                        x.Acc_Yr_Sem_Id == Acc_Yr_Sem_Id &&
                        x.Pgm_Id == Pgm_Id &&
                        x.Active_Status == true);

                if (qb == null)
                {
                    return Json(new
                    {
                        Exists = false
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    Exists = true,
                    OpenDate = qb.Open_Date.HasValue
                        ? qb.Open_Date.Value.ToString("yyyy-MM-dd")
                        : "",

                    CloseDate = qb.Close_Date.HasValue
                        ? qb.Close_Date.Value.ToString("yyyy-MM-dd")
                        : "",

                    Is_Open = qb.Is_Open
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Exists = false,
                    Error = true,
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
