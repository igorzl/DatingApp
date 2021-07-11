using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(user => user.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .Include(p => p.Photos)
            .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            //_context.Update(user);
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            // was before pagination 
            // return await _context.Users
            //     .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            //     .ToListAsync();

            var queryAppUser = _context.Users
                .AsQueryable(); // for filtering

            queryAppUser = queryAppUser.Where(u => u.UserName != userParams.CurrentUsername);
            queryAppUser = queryAppUser.Where(u => u.Gender == userParams.Gender);

            var maxDob = DateTime.Today.AddYears(- userParams.MinAge);
            var minDob = DateTime.Today.AddYears(- userParams.MaxAge - 1);

            queryAppUser = queryAppUser.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            //new "switch" syntax
            queryAppUser = userParams.OrderBy switch
            {
                "created" => queryAppUser.OrderByDescending(u => u.Created),
                //default case
                _ => queryAppUser.OrderByDescending(u => u.LastActive)
            };

            var queryMemberDto = queryAppUser
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsNoTracking(); // readonly

            //here we will apply query on projected MemberDto structure

            // var query = _context.Users
            //                 .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            //                 .AsNoTracking()
            //                 .AsQueryable();
            // query = query.Where(u => u.Username != userParams.CurrentUsername);
            // query = query.Where(u => u.Gender == userParams.Gender);
            // query = query.Where(u => u.Age <= userParams.MaxAge && u.Age >= userParams.MinAge);

            return await PagedList<MemberDto>.CreateAsync(queryMemberDto, userParams.PageNumber, userParams.PageSize);
        }

    }
}