using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class CourseController : Controller
    {
        BigSchoolContext context = new BigSchoolContext();

        [HttpGet]
        // GET: Courses
        public ActionResult Create()
        {
            //get list category
            BigSchoolContext context = new BigSchoolContext();
            Course objCourse = new Course();
            objCourse.ListCategory = context.Categories.ToList();


            return View(objCourse);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {
            
            //get list category
            BigSchoolContext context = new BigSchoolContext();
            ModelState.Remove("LecturerId");
            if (!ModelState.IsValid)
            {
                objCourse.ListCategory = context.Categories.ToList();
                return View("Create", objCourse);
            }
            

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LecturerId = user.Id;

            context.Courses.Add(objCourse);

            context.SaveChanges();



            return RedirectToAction("Index","Home");
        }

        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>(); foreach (Attendance temp in listAttendances)
            {
                Course objCourse = temp.Course; 
                objCourse.LectureName =System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LecturerId).Name;
                courses.Add(objCourse);
            }

            return View(courses);
        }
        public ActionResult Mine()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            var courses = context.Courses.Where(c => c.LecturerId == currentUser.Id && c.DateTime > DateTime.Now).ToList();
            foreach (Course i in courses)
            {
                i.LectureName = currentUser.Name;  //Name  la  cot  da  them  vao  Aspnetuser
            }
            return View(courses);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            //get list category
            BigSchoolContext context = new BigSchoolContext();
            var userId = User.Identity.GetUserId();
            Course course = context.Courses.FirstOrDefault(c => c.Id == id && c.LecturerId == userId);           
            course.ListCategory = context.Categories.ToList();
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }
        [HttpPost]
        public ActionResult Edit( Course f)
        {
            BigSchoolContext context = new BigSchoolContext();
            //List<Book> listbook = context.Books.ToList();
            Course find = context.Courses.FirstOrDefault(p => p.Id == f.Id);
            if (find == null)
            {
                return HttpNotFound();
            }


            find.Place = f.Place;
            find.DateTime = f.DateTime;
            find.CategoryId = f.CategoryId;
            context.SaveChanges();

            return RedirectToAction("Mine", "Course");
        }
        [HttpGet]
        [Authorize]
        public ActionResult Delete(int id)
        {

            BigSchoolContext context = new BigSchoolContext();
            var userId = User.Identity.GetUserId();
            Course course = context.Courses.FirstOrDefault(c => c.Id == id && c.LecturerId == userId);
            course.ListCategory = context.Categories.ToList();
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);

        }
        [HttpPost]
        public ActionResult Delete(Course f)
        {

            BigSchoolContext context = new BigSchoolContext();
            //List<Book> listbook = context.Books.ToList();
            

            

            Course show = context.Courses.Find(f.Id);
            context.Courses.Remove(show);
            context.SaveChanges();
            return RedirectToAction("Mine", "Course");

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();

            var listFollowee = context.Followings.Where(p => p.FollowerId == currentUser.Id).ToList();
            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (var course in listAttendances)
            {
                foreach (var item in listFollowee)
                {
                    if (item.FolloweeId == course.Course.LecturerId)
                    {
                        Course objCourse = course.Course;
                        objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                            .FindById(objCourse.LecturerId).Name;
                        courses.Add(objCourse);
                    }
                }
            }
            return View(courses);
        }
    }
}