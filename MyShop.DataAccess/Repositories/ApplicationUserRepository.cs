﻿using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Repositories;

public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
{

    private readonly ApplicationDbContext _context;

    public ApplicationUserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
