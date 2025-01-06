using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Online_IT_Ticketing_System.Models;

namespace Online_IT_Ticketing_System.DAL
{
    public class DatabaseHelper
    {
        private readonly IConfiguration _configuration;

        public DatabaseHelper()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }

        // GetConnectionString
        private string GetConnectionString()
        {
            try
            {
                return _configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving the connection string: " + ex.Message);
                throw;
            }
        }

        // Register User
        public static bool SignUpUser(string firstName, string lastName, DateTime dob, string gender, string phoneNumber,
            string email, string address, string state, string city, string username, string passwordHash)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    conn.Open();

// Check if the email is already registered
                    using (SqlCommand checkEmailCmd = new SqlCommand("sp_CheckEmailExists", conn))
                    {
                        checkEmailCmd.CommandType = CommandType.StoredProcedure;
                        checkEmailCmd.Parameters.AddWithValue("@Email", email);

                        // Execute the email check stored procedure
                        var emailExists = (int)checkEmailCmd.ExecuteScalar();
                        if (emailExists > 0)
                        {
                            Console.WriteLine("The email is already registered.");
                            return false; // Stop registration process if email exists
                        }
                    }

                    // Proceed with user registration

                    using (SqlCommand cmd = new SqlCommand("sp_RegisterUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@DateOfBirth", dob);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@State", state);
                        cmd.Parameters.AddWithValue("@City", city);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash); // Storing the hashed password

                        return cmd.ExecuteNonQuery() > 0; // Returns true if registration was successful

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while executing the SignUp stored procedure: " + ex.Message);
                return false;
            }
        }

        // Validate User Login
        public static string ValidateUser(string username)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ValidateUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", username);

                        var result = cmd.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while validating user: " + ex.Message);
                return null;
            }
        }

        // Validate SubAdmin Login
        public static string ValidateSubAdmin(string username)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ValidateSubAdmin", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", username);

                        var result = cmd.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while validating SubAdmin: " + ex.Message);
                return null;
            }
        }

        // General method for executing a reader
        public static List<Dictionary<string, object>> ExecuteReader(string storedProcedure, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(storedProcedure, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var results = new List<Dictionary<string, object>>();
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                }

                                results.Add(row);
                            }

                            return results;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while executing the reader: " + ex.Message);
                throw;
            }
        }
        // Create SubAdmin
        public static bool CreateSubAdmin(string name, string jobField, string username, string email, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CreateSubAdmin", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@JobField", jobField);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating SubAdmin: {ex.Message}");
                return false;
            }
        }

        // Delete SubAdmin
        public static bool DeleteSubAdmin(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteSubAdmin", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting SubAdmin: {ex.Message}");
                return false;
            }
        }
        public static List<SubAdminModel> GetSubAdmins()
        {
            List<SubAdminModel> subAdmins = new List<SubAdminModel>();

            using (var connection = new SqlConnection(new DatabaseHelper().GetConnectionString()))
            {
                using (var command = new SqlCommand("sp_ViewAllSubAdmins", connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            subAdmins.Add(new SubAdminModel
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString()
                            });
                        }
                    }
                }
            }

            return subAdmins;
        }
        // Get All SubAdmins
        public static List<SubAdminModel> GetAllSubAdmins()
        {
            List<SubAdminModel> subAdmins = new List<SubAdminModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ViewAllSubAdmins", conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                subAdmins.Add(new SubAdminModel
                                {
                                    Id = (int)reader["Id"],
                                    Name = reader["Name"].ToString(),
                                    JobField = reader["JobField"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Password = reader["Password"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving SubAdmins: {ex.Message}");
            }

            return subAdmins;
        }

        // Get SubAdmin by ID
        public static SubAdminModel GetSubAdminById(int id)
        {
            SubAdminModel subAdmin = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    
                    using (SqlCommand cmd = new SqlCommand("GetSubAdminById", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id); 


                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                subAdmin = new SubAdminModel
                                {
                                    Id = (int)reader["Id"],
                                    Name = reader["Name"].ToString(),
                                    JobField = reader["JobField"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Password = reader["Password"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving SubAdmin by ID: {ex.Message}");
            }

            return subAdmin;
        }


        // Update SubAdmin
        public static bool UpdateSubAdmin(int id, string name, string jobField, string username, string email, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateSubAdmin", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@JobField", jobField);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating SubAdmin: {ex.Message}");
                return false;
            }
        }

        // Get All Tickets
        public static List<TicketModel> GetAllTickets()
        {
            List<TicketModel> tickets = new List<TicketModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetAllTickets", conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tickets.Add(new TicketModel
                                {
                                    TicketId = reader["TicketId"].ToString(),
                                    UserName = reader["UserName"].ToString(),
                                    Subject = reader["Subject"].ToString(),
                                    Topic = reader["Topic"].ToString(),
                                    Category = reader["Category"].ToString(),
                                    AttachmentPath = reader["AttachmentPath"].ToString(),
                                    AttachmentData = reader["AttachmentData"] != DBNull.Value ? (byte[])reader["AttachmentData"] : null,
                                    CreationDate = (DateTime)reader["CreationDate"],
                                    Status = reader["Status"].ToString(),
                                    AssignedTo = reader["AssignedTo"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
            }

            return tickets;
        }

        // Assign Ticket
        public static bool AssignTicket(string TicketId, int SubAdminId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AssignTicket", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TicketId", TicketId);
                        cmd.Parameters.AddWithValue("@SubAdminId", SubAdminId);

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning ticket: {ex.Message}");
                return false;
            }
        }

        // Send Message to User
        public static bool SendMessageToUser(string ticketId, string userName, string message)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SendMessageToUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TicketId", ticketId);
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        cmd.Parameters.AddWithValue("@Message", message);

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to user: {ex.Message}");
                return false;
            }
        }

        // Update Ticket Status
        public static bool UpdateTicketStatus(string ticketId, string status)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateTicketStatus", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TicketId", ticketId);
                        cmd.Parameters.AddWithValue("@Status", status);

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating ticket status: {ex.Message}");
                return false;
            }
        }





        public static int GetSubAdminIdByUsername(string username)
        {
            int subAdminId = 0;

            try
            {
                using (var connection = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    using (var command = new SqlCommand("GetSubAdminIdByUsername", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", username);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                subAdminId = (int)reader["Id"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching SubAdminId: {ex.Message}");
            }

            return subAdminId;
        }


        public static List<TicketModel> GetTicketsForSubAdmin(int subAdminId)
        {
            List<TicketModel> tickets = new List<TicketModel>();

            try
            {
                using (var connection = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    using (var command = new SqlCommand("GetTicketsForSubAdmin", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SubAdminId", subAdminId);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tickets.Add(new TicketModel
                                {
                                    TicketId = reader["TicketId"].ToString(),
                                    UserName = reader["Username"].ToString(),
                                    Topic = reader["Topic"].ToString(),
                                    Subject = reader["Subject"].ToString(),
                                    Category = reader["Category"].ToString(),
                                    CreationDate = Convert.ToDateTime(reader["CreationDate"]),
                                    AttachmentPath = reader["AttachmentPath"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    AttachmentData = reader["AttachmentData"] != DBNull.Value ? (byte[])reader["AttachmentData"] : null
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching tickets for SubAdmin: {ex.Message}");
            }

            return tickets;
        }



        public static void SaveTicketToDatabase(TicketModel ticket)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    using (SqlCommand command = new SqlCommand("sp_InsertTicket", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@UserName", ticket.UserName);
                        command.Parameters.AddWithValue("@TicketId", ticket.TicketId);
                        command.Parameters.AddWithValue("@Topic", ticket.Topic);
                        command.Parameters.AddWithValue("@Subject", ticket.Subject);
                        command.Parameters.AddWithValue("@Category", ticket.Category);
                        command.Parameters.AddWithValue("@CreationDate", ticket.CreationDate);
                        command.Parameters.AddWithValue("@Status", ticket.Status ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@AssignedTo", ticket.AssignedTo ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@AttachmentPath", ticket.AttachmentPath ?? (object)DBNull.Value);
                        command.Parameters.Add("@AttachmentData", SqlDbType.VarBinary).Value = ticket.AttachmentData ?? (object)DBNull.Value;

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving ticket: {ex.Message}");
            }
        }

        public static List<TicketModel> GetTicketsByUserName(string userName)
        {
            var tickets = new List<TicketModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetTicketsByUserName", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserName", userName);

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var ticket = new TicketModel
                                {
                                    UserName = reader["Username"]?.ToString(),
                                    TicketId = reader["TicketId"]?.ToString(),
                                    Topic = reader["Topic"]?.ToString(),
                                    Subject = reader["Subject"]?.ToString(),
                                    Category = reader["Category"]?.ToString(),
                                    Status = reader["Status"]?.ToString(),
                                    AttachmentPath = reader["AttachmentPath"]?.ToString(),
                                    CreationDate = reader["CreationDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreationDate"]) : DateTime.MinValue,
                                    AttachmentData = reader["AttachmentData"] != DBNull.Value ? (byte[])reader["AttachmentData"] : null,
                                };

                                ticket.Messages = GetMessagesByTicketId(ticket.TicketId);
                                tickets.Add(ticket);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching tickets for user: {ex.Message}");
            }

            return tickets;
        }

        public static List<MessageModel> GetMessagesByTicketId(string ticketId)
        {
            var messages = new List<MessageModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetMessagesByTicketId", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TicketId", ticketId);

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var message = new MessageModel
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    TicketId = reader["TicketId"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString(),
                                    Message = reader["Message"]?.ToString(),
                                    SentDate = reader["SentDate"] != DBNull.Value ? Convert.ToDateTime(reader["SentDate"]) : DateTime.MinValue,
                                };

                                messages.Add(message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching messages for ticket: {ex.Message}");
            }

            return messages;
        }

        public static (int totalTickets, int closedTickets, int processingTickets) GetTicketCountsForUser(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(new DatabaseHelper().GetConnectionString())) 
                {
                    using (var command = new SqlCommand("sp_GetTicketCounts", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", userId);

                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return (
                                    totalTickets: (int)reader["TotalTickets"],
                                    closedTickets: (int)reader["ClosedTickets"],
                                    processingTickets: (int)reader["ProcessingTickets"]
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching ticket counts: {ex.Message}");
            }
            return (0, 0, 0); // Return default values if there's an error
        }

        // Method to check if email is already registered
        public static bool IsEmailTaken(string email)
        {
            using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_CheckEmailExists", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    return (int)cmd.ExecuteScalar() > 0;  // If email exists, return true
                }
            }
        }

        // Method to check if username is already taken
        public static bool IsUsernameTaken(string username)
        {
            using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_CheckUsernameExists", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", username);
                    return (int)cmd.ExecuteScalar() > 0;  // If username exists, return true
                }
            }
        }



    }

}
    

    





