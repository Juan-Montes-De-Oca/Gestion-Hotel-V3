using System;

public class Room
{
    public int Number { get; set; }
    public string Type { get; set; }
    public decimal PricePerNight { get; set; }
    public int Days { get; set; }
    public string Status { get; set; }
    public string Details { get; set; }

    // Propiedad calculada para el total a pagar
    public decimal TotalPayment => PricePerNight * Days;

    public Room(int number, string type, decimal pricePerNight, int days, string status, string details)
    {
        Number = number;
        Type = type;
        PricePerNight = pricePerNight;
        Days = days;
        Status = status;
        Details = details;
    }

    public void ShowDetails()
    {
        Console.WriteLine($"N.º: {Number} | Tipo: {Type} | Precio/Noche: ${PricePerNight:F2} | Días: {Days} | Total: ${TotalPayment:F2} | Estado: {Status} | Detalle: {Details}");
    }
}