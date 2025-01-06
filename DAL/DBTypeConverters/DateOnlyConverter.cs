using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.DBTypeConverters
{
    internal class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter() : base(
            x => x.ToDateTime(TimeOnly.MinValue), 
            x => DateOnly.FromDateTime(x))
        { } 
    }
}
