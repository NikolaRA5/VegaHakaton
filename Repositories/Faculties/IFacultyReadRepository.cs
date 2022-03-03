﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Repositories.Base;

namespace Repositories.Faculties
{
    public interface IFacultyReadRepository : IReadBaseRepository<Guid, Faculty>
    {
        IEnumerable<Room> GetRoomsByFacultyId(Guid facultyId);
    }
}
