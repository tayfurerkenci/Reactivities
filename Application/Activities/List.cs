using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {

        public class Query : IRequest<Result<List<ActivityDto>>>
        {

        }

        public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;

            }

            public async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {

                var activities = await _context.Activities
                    // eager loading causes bad performance for that we should use projectto
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, 
                        new {currentUsername = _userAccessor.GetUsername()})
                    // .Include(a => a.Attendees)
                    // .ThenInclude(u => u.AppUser)
                    .ToListAsync(cancellationToken);

                // var activitiesToReturn=_mapper.Map<List<ActivityDto>>(activities);

                return Result<List<ActivityDto>>.Success(activities);
            }
        }
    }
}