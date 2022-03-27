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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            BigSchoolContext context = new BigSchoolContext();
            var upcomingCourse = context.Courses.Where(p => p.DateTime > DateTime.Now).OrderBy(p => p.DateTime).ToList();
            var userId = User.Identity.GetUserId();
            foreach (Course i in upcomingCourse)
            {
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(i.LecturerId);
                i.Name = user.Name;

                if (userId != null)
                {
                    i.isLogin = true;
                    Attendance find = context.Attendances.FirstOrDefault(p => p.CourseId == i.Id && p.Attendee == userId);
                    if (find == null)
                    {
                        i.isShowGoing = true;
                    }
                    Following findFollow = context.Followings.FirstOrDefault(p => p.FollowerId == userId && p.FolloweeId == i.LecturerId);
                    if (findFollow == null)
                        i.isShowFollow = true;
                }
            }
            return View(upcomingCourse);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}