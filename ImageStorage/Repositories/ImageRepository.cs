using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStorage.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private IConfiguration _config;

        public ImageRepository(IConfiguration config)
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

        public Stream GetImageStreamById(int id)
        {

            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from Image where Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var data = reader.GetStream(reader.GetOrdinal("Image"));
                            if (data.Length > 0)
                            {
                                return data;
                            }
                        }
                        return null;
                    }
                }
            }
        }

        public int CreateImage(byte[] imageData)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"insert into Image (Image) output INSERTED.Id VALUES (@data)";
                    cmd.Parameters.AddWithValue("@data", imageData);

                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public void DeleteImage(int? id)
        {
            if (id.HasValue)
            {
                using (var conn = Connection)
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "delete from Image where Id = @id";
                        cmd.Parameters.AddWithValue("@id", id);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
