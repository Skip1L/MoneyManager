using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Helpers
{
    public static class DateOfBirthHelper
    {
        public static bool IsDateOfBirthValid(DateTime dateOfBirth)
        {
            return dateOfBirth <= DateTime.UtcNow;
        }
    }
}
