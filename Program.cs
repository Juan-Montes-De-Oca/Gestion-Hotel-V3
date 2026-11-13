using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int option;
        bool isValidOption;

        do
        {
            Console.Clear();
            Console.WriteLine("==================================================");
            Console.WriteLine("     SISTEMA DE GESTIÓN DE HOTEL (CON BD)         ");
            Console.WriteLine("==================================================");
            Console.WriteLine("1. Registrar nueva habitación");
            Console.WriteLine("2. Listar todas las habitaciones");
            Console.WriteLine("3. Buscar habitación");
            Console.WriteLine("4. Modificar habitación");
            Console.WriteLine("5. Eliminar habitación");
            Console.WriteLine("0. Salir del programa");
            Console.WriteLine("==================================================");
            Console.WriteLine("Elija una opción correspondiente (0 - 5): ");

            isValidOption = int.TryParse(Console.ReadLine(), out option);

            if (!isValidOption)
            {
                Console.WriteLine("\nERROR! Opción inválida. Introduzca un número del 0 al 5.");
                Console.WriteLine("\nPresione Enter para continuar...");
                Console.ReadLine();
                continue;
            }

            switch (option)
            {
                case 1:
                    Console.WriteLine("\n=== REGISTRAR NUEVA HABITACIÓN ===");
                    
                    int roomNum;
                    while (true)
                    {
                        Console.WriteLine("Ingrese el número de la habitación (Ej. 101):");
                        if (!int.TryParse(Console.ReadLine(), out roomNum) || roomNum <= 0)
                        {
                            Console.WriteLine("ERROR! Debe ingresar un número entero positivo.");
                            continue;
                        }

                        if (RoomService.GetRoomByNumber(roomNum) != null)
                        {
                            Console.WriteLine($"ERROR! La habitación #{roomNum} ya está registrada en la base de datos.");
                            continue;
                        }
                        break;
                    }

                    // Selecciona el tipo de habitación y obtiene su precio estándar automáticamente
                    var selectedRoomInfo = RoomService.ChooseRoomTypeWithStandardPrice();
                    string roomType = selectedRoomInfo.TypeName;
                    decimal pricePerNight = selectedRoomInfo.PricePerNight;

                    Console.WriteLine($"-> Precio estándar asignado: ${pricePerNight:F2} por noche.");

                    // Pide la cantidad de días de estancia
                    int days;
                    while (true)
                    {
                        Console.WriteLine("Ingrese la cantidad de días que se estará usando:");
                        if (int.TryParse(Console.ReadLine(), out days) && days > 0)
                        {
                            break;
                        }
                        Console.WriteLine("ERROR! Ingrese un número de días válido mayor a 0.");
                    }

                    string roomStatusVal = RoomService.ChooseStatus();

                    Console.WriteLine("Ingrese detalles o ubicación (Ej. Vista al mar):");
                    string details = (Console.ReadLine() ?? "").Trim();
                    if (string.IsNullOrWhiteSpace(details)) details = "Estándar";

                    // Crea el objeto Room calculando el total automáticamente
                    Room newRoom = new Room(roomNum, roomType, pricePerNight, days, roomStatusVal, details);

                    Console.WriteLine($"\n[Cálculo] Total a pagar por {days} día(s): ${newRoom.TotalPayment:F2}");

                    bool isInserted = RoomService.AddRoom(newRoom);
                    if (isInserted)
                    {
                        Console.WriteLine($"\n¡Éxito! Habitación #{roomNum} guardada en la base de datos.");
                    }
                    else
                    {
                        Console.WriteLine("\nERROR! No se pudo registrar en la base de datos.");
                    }

                    Console.WriteLine("\nPresione Enter para continuar...");
                    Console.ReadLine();
                    break;

                case 2:
                    Console.WriteLine("\n=== LISTADO GENERAL DE HABITACIONES ===");
                    List<Room> rooms = RoomService.GetAllRooms();

                    if (rooms.Count == 0)
                    {
                        Console.WriteLine("No hay habitaciones registradas en la base de datos.");
                    }
                    else
                    {
                        for (int i = 0; i < rooms.Count; i++)
                        {
                            Console.Write($"[{i + 1}] ");
                            rooms[i].ShowDetails();
                        }
                        Console.WriteLine($"\nTotal de habitaciones: {rooms.Count}");
                    }
                    Console.WriteLine("\nPresione Enter para continuar...");
                    Console.ReadLine();
                    break;

                case 3:
                    Console.WriteLine("\n=== BUSCAR HABITACIÓN ===");
                    Console.WriteLine("Ingrese el número de la habitación a buscar:");
                    if (int.TryParse(Console.ReadLine(), out int searchNum))
                    {
                        Room? foundRoom = RoomService.GetRoomByNumber(searchNum);
                        if (foundRoom != null)
                        {
                            Console.WriteLine("\n[¡Encontrada en la Base de Datos!]");
                            foundRoom.ShowDetails();
                        }
                        else
                        {
                            Console.WriteLine("No se encontró ninguna habitación con ese número.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ERROR! Entrada inválida.");
                    }
                    Console.WriteLine("\nPresione Enter para continuar...");
                    Console.ReadLine();
                    break;

                case 4:
                    Console.WriteLine("\n=== MODIFICAR HABITACIÓN ===");
                    Console.WriteLine("Ingrese el número de la habitación a modificar:");
                    if (int.TryParse(Console.ReadLine(), out int modNum))
                    {
                        Room? roomToModify = RoomService.GetRoomByNumber(modNum);
                        if (roomToModify != null)
                        {
                            Console.WriteLine($"Datos actuales -> Precio Total: ${roomToModify.TotalPayment:F2} | Estado: {roomToModify.Status}");

                            decimal newPrice = roomToModify.TotalPayment;
                            Console.WriteLine("Ingrese el nuevo precio total (o presione Enter para mantener el actual):");
                            string priceInput = (Console.ReadLine() ?? "").Trim();
                            if (priceInput != "" && decimal.TryParse(priceInput, out decimal tempPrice) && tempPrice >= 0)
                            {
                                newPrice = tempPrice;
                            }

                            string newStatus = roomToModify.Status;
                            Console.WriteLine("¿Desea cambiar el estado? (1. Sí / 2. No):");
                            if (int.TryParse(Console.ReadLine(), out int changeStatus) && changeStatus == 1)
                            {
                                newStatus = RoomService.ChooseStatus();
                            }

                            bool isUpdated = RoomService.UpdateRoom(modNum, newPrice, newStatus);
                            if (isUpdated)
                            {
                                Console.WriteLine("\n¡Habitación actualizada correctamente!");
                            }
                            else
                            {
                                Console.WriteLine("\nERROR! No se pudo actualizar.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No existe una habitación con ese número.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ERROR! Entrada inválida.");
                    }
                    Console.WriteLine("\nPresione Enter para continuar...");
                    Console.ReadLine();
                    break;

                case 5:
                    Console.WriteLine("\n=== ELIMINAR HABITACIÓN ===");
                    Console.WriteLine("Ingrese el número de la habitación a eliminar:");
                    if (int.TryParse(Console.ReadLine(), out int delNum))
                    {
                        Room? roomToDelete = RoomService.GetRoomByNumber(delNum);
                        if (roomToDelete != null)
                        {
                            Console.WriteLine($"¿Está seguro de eliminar la habitación #{delNum}? (S/N):");
                            string confirm = (Console.ReadLine() ?? "").Trim().ToUpper();

                            if (confirm == "S")
                            {
                                bool isDeleted = RoomService.DeleteRoom(delNum);
                                if (isDeleted)
                                {
                                    Console.WriteLine("\n¡Habitación eliminada de la base de datos!");
                                }
                                else
                                {
                                    Console.WriteLine("\nERROR! No se pudo eliminar.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("\nOperación cancelada.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No se encontró ninguna habitación con ese número.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ERROR! Entrada inválida.");
                    }
                    Console.WriteLine("\nPresione Enter para continuar...");
                    Console.ReadLine();
                    break;

                case 0:
                    Console.WriteLine("\n=== GRACIAS POR UTILIZAR NUESTRO SISTEMA. HASTA PRONTO! ===");
                    break;

                default:
                    Console.WriteLine("\nERROR! Opción fuera de rango.");
                    Console.WriteLine("\nPresione Enter para continuar...");
                    Console.ReadLine();
                    break;
            }
        }
        while (option != 0);
    }
}