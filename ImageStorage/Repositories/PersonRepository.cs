using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using ImageStorage.Models;

namespace ImageStorage.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private IConfiguration _config;

        public PersonRepository(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Person> GetPersons()
        {
            var persons = new List<Person>();

            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from Person";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            persons.Add(new Person
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                ImageId = reader.IsDBNull(reader.GetOrdinal("ImageId")) ? null : reader.GetInt32(reader.GetOrdinal("ImageId"))
                            });
                        }
                    }
                }
            }

            return persons;
        }

        public Person GetPersonById(int id)
        {
            Person person = null;

            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from Person where Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            person = new Person
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                ImageId = reader.IsDBNull(reader.GetOrdinal("ImageId")) ? null : reader.GetInt32(reader.GetOrdinal("ImageId"))
                            };
                        }
                    }
                }
            }

            return person;
        }

        public void CreatePerson(Person person)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"insert into Person (Name, ImageId)
                                        output INSERTED.Id
                                        VALUES (@name, @imageId)";
                    cmd.Parameters.AddWithValue("@name", person.Name);
                    cmd.Parameters.AddWithValue("@imageId", person.ImageId.HasValue ? person.ImageId : DBNull.Value);

                    person.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void UpdatePerson(Person person)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"update Person 
                                        set Name = @name, ImageId = @imageId
                                        where Id = @id";
                    cmd.Parameters.AddWithValue("@id", person.Id);
                    cmd.Parameters.AddWithValue("@name", person.Name);
                    cmd.Parameters.AddWithValue("@imageId", person.ImageId.HasValue ? person.ImageId : DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeletePerson(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"delete from Person
                                        where Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
