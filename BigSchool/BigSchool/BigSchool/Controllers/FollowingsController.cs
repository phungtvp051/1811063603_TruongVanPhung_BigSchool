using BigSchool.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BigSchool.Controllers
{
    public class FollowingsController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Follow(Following follow)
        {
            BigSchoolContext context = new BigSchoolContext();
            var userId = User.Identity.GetUserId();
            if (userId == null)
            {
                return BadRequest("please login first !");
            }
            if (userId == follow.FolloweeId)
            {
                return BadRequest("Cannot follow myself !");
            }
            Following find = context.Followings.FirstOrDefault(p => p.FollowerId == userId && p.FolloweeId == follow.FolloweeId);
            if (find != null)
            {
                /*return BadRequest("The already following exists !");*/
                context.Followings.Remove(context.Followings.SingleOrDefault(p => p.FollowerId == userId && p.FolloweeId == follow.FolloweeId));
                context.SaveChanges();
                return Ok("cancel");
            }
            follow.FollowerId = userId;
            context.Followings.Add(follow);
            context.SaveChanges();
            return Ok();
        }
    }
}
