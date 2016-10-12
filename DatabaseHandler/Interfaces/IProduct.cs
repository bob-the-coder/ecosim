using System;

namespace DatabaseHandler.Interfaces
{
    public interface IProduct
    {
        Guid Id { get; set; }
        Guid Name { get; set; }
    }
}
