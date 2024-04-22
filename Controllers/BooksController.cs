using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace API__Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public BooksController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _connection = new MySqlConnection(connectionString);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var books = new List<Book>();
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = "SELECT * FROM Books";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        books.Add(new Book
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Title = reader["Title"].ToString(),
                            Author = reader["Author"].ToString(),
                            Year = Convert.ToInt32(reader["Year"])
                        });
                    }
                }
                _connection.Close();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
            return Ok(books);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _connection.Open();
                var command = (MySqlCommand)_connection.CreateCommand();  // Явное приведение к типу MySqlCommand
                command.CommandText = "DELETE FROM Books WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);  // Теперь можно использовать AddWithValue
                var deleted = command.ExecuteNonQuery();
                _connection.Close();

                if (deleted > 0)
                {
                    return Ok($"Book with Id {id} was deleted.");
                }
                else
                {
                    return NotFound($"Book with Id {id} not found.");
                }
            }
            catch (Exception ex)
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public class Book
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public int Year { get; set; }
        }
    }
}
