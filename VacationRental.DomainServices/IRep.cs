using System.Collections.Generic;

namespace VacationRental.DomainServices
{
    public interface IRep<T> where T:class
    {
        T GetById(int id);

        IEnumerable<T> GetAll();

        bool Exists(int id);

        int Count();

        void Add(T newObject);
    }
}