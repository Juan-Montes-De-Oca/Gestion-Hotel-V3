using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

public class RoomService
{
    private static string connectionString = "Server=JUAN\\SQLEXPRESS;Database=HotelDB;Trusted_Connection=True;TrustServerCertificate=True;";
    public static bool AddRoom(Room room)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO Rooms (Number, Type, Price, Status, Details) VALUES (@Number, @Type, @Price, @Status, @Details)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Number", room.Number);
                cmd.Parameters.AddWithValue("@Type", room.Type);
                cmd.Parameters.AddWithValue("@Price", room.TotalPayment);
                cmd.Parameters.AddWithValue("@Status", room.Status);
                cmd.Parameters.AddWithValue("@Details", $"Días: {room.Days} | {room.Details}");

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al guardar: {ex.Message}");
                    return false;
                }
            }
        }
    }

    public static List<Room> GetAllRooms()
    {
        List<Room> rooms = new List<Room>();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT Number, Type, Price, Status, Details FROM Rooms";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rooms.Add(new Room(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetDecimal(2),
                                1, // Días por defecto
                                reader.GetString(3),
                                reader.GetString(4)
                            ));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al listar: {ex.Message}");
                }
            }
        }
        return rooms;
    }

    public static Room? GetRoomByNumber(int number)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT Number, Type, Price, Status, Details FROM Rooms WHERE Number = @Number";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Number", number);
                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Room(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetDecimal(2),
                                1,
                                reader.GetString(3),
                                reader.GetString(4)
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar: {ex.Message}");
                }
            }
        }
        return null;
    }

    public static bool UpdateRoom(int number, decimal newPrice, string newStatus)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "UPDATE Rooms SET Price = @Price, Status = @Status WHERE Number = @Number";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Price", newPrice);
                cmd.Parameters.AddWithValue("@Status", newStatus);
                cmd.Parameters.AddWithValue("@Number", number);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al actualizar: {ex.Message}");
                    return false;
                }
            }
        }
    }

    public static bool DeleteRoom(int number)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM Rooms WHERE Number = @Number";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Number", number);
                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al eliminar: {ex.Message}");
                    return false;
                }
            }
        }
    }

    public static (string TypeName, decimal PricePerNight) ChooseRoomTypeWithStandardPrice()
    {
        string[] types = { "Individual (Single)", "Doble", "Matrimonial", "Suite Junior", "Suite Presidencial" };
        decimal[] prices = { 40.00m, 60.00m, 75.00m, 120.00m, 200.00m };

        while (true)
        {
            Console.WriteLine("\nSeleccione el tipo de habitación:");
            for (int i = 0; i < types.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {types[i]} - ${prices[i]:F2} la noche");
            }

            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= types.Length)
            {
                return (types[choice - 1], prices[choice - 1]);
            }
            Console.WriteLine("Opción inválida. Intente de nuevo.");
        }
    }

    public static string ChooseStatus()
    {
        string[] statuses = { "Disponible", "Ocupada", "Mantenimiento", "Limpieza" };
        while (true)
        {
            Console.WriteLine("\nSeleccione el estado:");
            for (int i = 0; i < statuses.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {statuses[i]}");
            }

            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= statuses.Length)
            {
                return statuses[choice - 1];
            }
            Console.WriteLine("Opción inválida. Intente de nuevo.");
        }
    }
}