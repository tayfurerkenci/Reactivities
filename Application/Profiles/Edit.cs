using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            //public Profile Profile { get; set; }
            public string DisplayName { get; set; }
            public string Bio { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {   
                RuleFor(x=>x.DisplayName).NotEmpty();    
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            //private readonly IMapper _mapper;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                //_mapper = mapper;
                _context = context;
                _userAccessor=userAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x=>x.UserName==_userAccessor.GetUsername());

                user.Bio=request.Bio ?? user.Bio;
                user.DisplayName= request.DisplayName ?? user.DisplayName;
                
                var success = await _context.SaveChangesAsync() > 0;
                
                if(!success){
                    return Result<Unit>.Failure("Failed to update profile");
                }

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}