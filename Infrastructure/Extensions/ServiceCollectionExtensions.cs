namespace MilesCarRental.Infrastructure.Extensions;

using Microsoft.Extensions.DependencyInjection;
using MilesCarRental.Domain.Repositories;
using MilesCarRental.Infrastructure.InMemory;
using MilesCarRental.Domain.Entities;
using MilesCarRental.Domain.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryData(this IServiceCollection services)
    {
        // Ubicaciones - Departamentos de Colombia
        var locations = new List<Location>
        {
            // CASANARE
            new("YOPAL", "Yopal", "CASANARE"),
            new("AGUAZUL", "Aguazul", "CASANARE"),
            new("TAURAMENA", "Tauramena", "CASANARE"),
            new("VILLANUEVA", "Villanueva", "CASANARE"),
            new("MONTERREY", "Monterrey", "CASANARE"),
    
            // CUNDINAMARCA
            new("BOGOTA", "Bogotá", "CUNDINAMARCA"),
            new("SOACHA", "Soacha", "CUNDINAMARCA"),
            new("FUSAGASUGA", "Fusagasugá", "CUNDINAMARCA"),
            new("FACATATIVA", "Facatativá", "CUNDINAMARCA"),
            new("ZIPAQUIRA", "Zipaquirá", "CUNDINAMARCA"),
            new("CHIA", "Chía", "CUNDINAMARCA"),
 
            // ANTIOQUIA
            new("MEDELLIN", "Medellín", "ANTIOQUIA"),
            new("BELLO", "Bello", "ANTIOQUIA"),
            new("ENVIGADO", "Envigado", "ANTIOQUIA"),
            new("ITAGUI", "Itagüí", "ANTIOQUIA"),
            new("RIONEGRO", "Rionegro", "ANTIOQUIA"),
         
            // VALLE DEL CAUCA
            new("CALI", "Cali", "VALLE DEL CAUCA"),
            new("PALMIRA", "Palmira", "VALLE DEL CAUCA"),
            new("BUENAVENTURA", "Buenaventura", "VALLE DEL CAUCA"),
            new("TULUA", "Tuluá", "VALLE DEL CAUCA"),
       
            // ATLANTICO
            new("BARRANQUILLA", "Barranquilla", "ATLANTICO"),
            new("SOLEDAD", "Soledad", "ATLANTICO"),
            new("MALAMBO", "Malambo", "ATLANTICO"),
     
            // SANTANDER
            new("BUCARAMANGA", "Bucaramanga", "SANTANDER"),
            new("FLORIDABLANCA", "Floridablanca", "SANTANDER"),
            new("GIRON", "Girón", "SANTANDER"),
            new("PIEDECUESTA", "Piedecuesta", "SANTANDER"),
     
            // BOLIVAR
            new("CARTAGENA", "Cartagena", "BOLIVAR"),
            new("MAGANGUE", "Magangué", "BOLIVAR"),
         
            // META
            new("VILLAVICENCIO", "Villavicencio", "META"),
            new("ACACIAS", "Acacías", "META"),
            new("GRANADA", "Granada", "META"),
        };

        var vehicles = new List<Vehicle>
        {
            // Económicos CASANARE
            new("VH-001", "ECON", "Económico", 5, "AT", "Car", new() { "CASANARE" }, new() { "YOPAL", "AGUAZUL", "TAURAMENA" }, true),
            new("VH-002", "ECON", "Económico", 5, "MT", "Car", new() { "CASANARE" }, new() { "YOPAL", "VILLANUEVA", "MONTERREY" }, true),
            new("VH-003", "ECON", "Económico", 5, "AT", "Car", new() { "CASANARE", "CUNDINAMARCA" }, new() { "YOPAL", "BOGOTA" }, true),
    
            // Económicos CUNDINAMARCA
            new("VH-004", "ECON", "Económico", 5, "AT", "Car", new() { "CUNDINAMARCA" }, new() { "BOGOTA", "SOACHA", "CHIA", "ZIPAQUIRA" }, true),
            new("VH-005", "ECON", "Económico", 5, "MT", "Car", new() { "CUNDINAMARCA" }, new() { "BOGOTA", "FUSAGASUGA", "FACATATIVA" }, true),
      
            // Económicos otras ciudades
            new("VH-006", "ECON", "Económico", 5, "AT", "Car", new() { "ANTIOQUIA" }, new() { "MEDELLIN", "BELLO", "ENVIGADO", "ITAGUI" }, true),
            new("VH-007", "ECON", "Económico", 5, "AT", "Car", new() { "VALLE DEL CAUCA" }, new() { "CALI", "PALMIRA", "TULUA" }, true),
            new("VH-008", "ECON", "Económico", 5, "MT", "Car", new() { "ATLANTICO" }, new() { "BARRANQUILLA", "SOLEDAD", "MALAMBO" }, true),
            
            // SUVs
            new("VH-009", "SUV", "SUV Compacto", 5, "AT", "SUV", new() { "CASANARE" }, new() { "YOPAL", "TAURAMENA", "AGUAZUL" }, true),
            new("VH-010", "SUV", "SUV Compacto", 5, "AT", "SUV", new() { "CUNDINAMARCA" }, new() { "BOGOTA", "CHIA", "ZIPAQUIRA" }, true),
            new("VH-011", "SUV", "SUV Mediano", 7, "AT", "SUV", new() { "CUNDINAMARCA", "ANTIOQUIA" }, new() { "BOGOTA", "MEDELLIN" }, true),
            new("VH-012", "SUV", "SUV Grande", 7, "AT", "SUV", new() { "ANTIOQUIA" }, new() { "MEDELLIN", "RIONEGRO", "ENVIGADO" }, true),
            new("VH-013", "SUV", "SUV Compacto", 5, "AT", "SUV", new() { "VALLE DEL CAUCA" }, new() { "CALI", "PALMIRA" }, true),
            new("VH-014", "SUV", "SUV Mediano", 7, "AT", "SUV", new() { "SANTANDER" }, new() { "BUCARAMANGA", "FLORIDABLANCA", "GIRON", "PIEDECUESTA" }, true),
            new("VH-015", "SUV", "SUV Grande", 7, "AT", "SUV", new() { "BOLIVAR" }, new() { "CARTAGENA" }, true),
            
            // Camionetas
            new("VH-016", "PICKUP", "Camioneta 4x4", 5, "MT", "Truck", new() { "CASANARE" }, new() { "YOPAL", "AGUAZUL", "TAURAMENA", "VILLANUEVA" }, true),
            new("VH-017", "PICKUP", "Camioneta 4x4", 5, "AT", "Truck", new() { "CASANARE", "META" }, new() { "YOPAL", "VILLAVICENCIO" }, true),
            new("VH-018", "PICKUP", "Camioneta 4x4", 5, "AT", "Truck", new() { "CUNDINAMARCA" }, new() { "BOGOTA", "FACATATIVA" }, true),
            new("VH-019", "PICKUP", "Camioneta Doble Cabina", 5, "AT", "Truck", new() { "ANTIOQUIA" }, new() { "MEDELLIN", "RIONEGRO" }, true),
   
            // Vans
            new("VH-020", "VAN", "Van 7 Pasajeros", 7, "AT", "Van", new() { "CUNDINAMARCA" }, new() { "BOGOTA", "SOACHA", "CHIA" }, true),
            new("VH-021", "VAN", "Van 12 Pasajeros", 12, "MT", "Van", new() { "CUNDINAMARCA" }, new() { "BOGOTA", "FUSAGASUGA" }, true),
            new("VH-022", "VAN", "Van 7 Pasajeros", 7, "AT", "Van", new() { "ANTIOQUIA" }, new() { "MEDELLIN", "ENVIGADO", "BELLO" }, true),
            new("VH-023", "VAN", "Van 12 Pasajeros", 12, "AT", "Van", new() { "VALLE DEL CAUCA" }, new() { "CALI", "PALMIRA" }, true),
            new("VH-024", "VAN", "Van 7 Pasajeros", 7, "AT", "Van", new() { "ATLANTICO" }, new() { "BARRANQUILLA", "SOLEDAD" }, true),
            
            // Lujo
            new("VH-025", "LUX", "Sedán de Lujo", 5, "AT", "Car", new() { "CUNDINAMARCA" }, new() { "BOGOTA", "CHIA" }, true),
            new("VH-026", "LUX", "SUV de Lujo", 5, "AT", "SUV", new() { "CUNDINAMARCA" }, new() { "BOGOTA" }, true),
            new("VH-027", "LUX", "Sedán Ejecutivo", 5, "AT", "Car", new() { "ANTIOQUIA" }, new() { "MEDELLIN", "ENVIGADO" }, true),
            new("VH-028", "LUX", "SUV Premium", 7, "AT", "SUV", new() { "VALLE DEL CAUCA" }, new() { "CALI" }, true),
            new("VH-029", "LUX", "Sedán de Lujo", 5, "AT", "Car", new() { "BOLIVAR" }, new() { "CARTAGENA" }, true),
            
            // Deportivos
            new("VH-030", "SPORT", "Deportivo Compacto", 4, "MT", "Car", new() { "CUNDINAMARCA" }, new() { "BOGOTA" }, true),
            new("VH-031", "SPORT", "Deportivo", 2, "AT", "Car", new() { "ANTIOQUIA" }, new() { "MEDELLIN" }, true),
  
            // Compactos
            new("VH-032", "COMP", "Compacto", 5, "MT", "Car", new() { "CUNDINAMARCA" }, new() { "BOGOTA", "SOACHA", "FUSAGASUGA", "FACATATIVA", "ZIPAQUIRA" }, true),
            new("VH-033", "COMP", "Compacto", 5, "AT", "Car", new() { "CUNDINAMARCA" }, new() { "BOGOTA", "CHIA" }, true),
            new("VH-034", "COMP", "Compacto", 5, "AT", "Car", new() { "ANTIOQUIA" }, new() { "MEDELLIN", "BELLO", "ITAGUI" }, true),
            new("VH-035", "COMP", "Compacto", 5, "MT", "Car", new() { "VALLE DEL CAUCA" }, new() { "CALI", "PALMIRA", "TULUA" }, true),
            new("VH-036", "COMP", "Compacto", 5, "AT", "Car", new() { "SANTANDER" }, new() { "BUCARAMANGA", "FLORIDABLANCA", "GIRON" }, true),
          
            // No disponibles
            new("VH-037", "ECON", "Económico", 5, "AT", "Car", new() { "CASANARE" }, new() { "YOPAL" }, false),
            new("VH-038", "SUV", "SUV", 5, "AT", "SUV", new() { "CUNDINAMARCA" }, new() { "BOGOTA" }, false),
            
            // Eléctricos
            new("VH-039", "ELEC", "Eléctrico", 5, "AT", "Car", new() { "CUNDINAMARCA" }, new() { "BOGOTA", "CHIA" }, true),
            new("VH-040", "ELEC", "SUV Eléctrico", 5, "AT", "SUV", new() { "ANTIOQUIA" }, new() { "MEDELLIN", "ENVIGADO" }, true),
        };

        services.AddSingleton<ILocationRepository>(new InMemoryLocationRepository(locations));
        services.AddSingleton<IVehicleRepository>(new InMemoryVehicleRepository(vehicles));
        services.AddSingleton<IMarketResolver, MarketResolver>();

        return services;
    }
}
