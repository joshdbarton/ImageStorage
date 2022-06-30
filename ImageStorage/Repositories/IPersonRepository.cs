using ImageStorage.Models;
using System.Collections.Generic;

namespace ImageStorage.Repositories
{
    public interface IPersonRepository
    {
        void CreatePerson(Person person);
        void DeletePerson(int id);
        Person GetPersonById(int id);
        List<Person> GetPersons();
        void UpdatePerson(Person person);
    }
}