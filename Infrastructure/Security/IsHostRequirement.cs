using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement
    {

    }

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IsHostRequirementHandler(DataContext dbContext, 
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            // getting user id from user claims via authorization handler context
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId==null) return Task.CompletedTask;

            // getting activity id from url parameters
            var activityId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues
                .SingleOrDefault(x=>x.Key == "id").Value?.ToString());

            // we cant await inside this method because it overrides the base method
            // so we shoudl use Result parameter
            // despite transient approach this entity will remain on the MEMORY!
            // prevent the bug we must use as no tracking method
            // findasync always tracks an entity we cant use with asnotracking
            var attendee = _dbContext.ActivityAttendees
                .AsNoTracking()
                .SingleOrDefaultAsync(x=> x.AppUserId==userId && x.ActivityId==activityId).Result;
                //.FindAsync(userId, activityId).Result;

            if(attendee ==null) return Task.CompletedTask;

            if(attendee.IsHost) context.Succeed(requirement);


            return Task.CompletedTask;

        }
    }
}