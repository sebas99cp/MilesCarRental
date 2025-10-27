# ?? Miles Car Rental API

API REST para b�squeda y gesti�n de alquiler de veh�culos en Colombia, implementada con .NET 8 y arquitectura limpia.

## ??? Arquitectura

El proyecto sigue los principios de **Clean Architecture** con separaci�n en capas:

- **Domain**: Entidades, especificaciones, repositorios y l�gica de negocio
- **Application**: Casos de uso, DTOs, validadores y servicios de aplicaci�n
- **Infrastructure**: Implementaciones de repositorios (in-memory)
- **MilesCarRental (API)**: Controllers, middleware, filtros y configuraci�n
- **Tests**: Pruebas unitarias con xUnit

## ? Caracter�sticas

### Core Features
- ? B�squeda de veh�culos por ubicaci�n (municipio/departamento)
- ? Filtrado por clase de veh�culo (ECON, SUV, VAN, LUX, ELEC)
- ? Soporte para devoluci�n en ubicaci�n diferente (one-way rental)
- ? Resoluci�n autom�tica de mercado (Nacional/Internacional)
- ? Paginaci�n de resultados
- ? Disponibilidad en tiempo real

### Technical Features
- ??? **Rate Limiting**: 100 peticiones por minuto por usuario/IP
- ?? **Logging estructurado**: Serilog con archivo rotativo diario
- ?? **OpenTelemetry**: Trazabilidad y observabilidad
- ?? **Validaci�n**: FluentValidation en todas las entradas
- ?? **Manejo global de excepciones**: Middleware centralizado
- ?? **Health Checks**: `/health`, `/health/ready`, `/health/live`
- ?? **Swagger/OpenAPI**: Documentaci�n interactiva con ejemplos

## ?? Inicio R�pido

### Requisitos Previos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Instalaci�n

```bash
# Clonar el repositorio
git clone https://github.com/sebas99cp/MilesCarRental.git
cd MilesCarRental

# Restaurar dependencias
dotnet restore

# Ejecutar la aplicaci�n
dotnet run --project MilesCarRental
```

La API estar� disponible en: `https://localhost:7000` (o el puerto configurado)

## ?? Uso de la API

### Endpoint Principal

**`GET /api/v1/vehicles/search`**

Busca veh�culos disponibles seg�n los criterios especificados. El sistema detecta autom�ticamente el mercado bas�ndose en los municipios de recogida y devoluci�n.

#### Par�metros de Query

| Par�metro | Tipo | Requerido | Descripci�n | Ejemplo |
|-----------|------|-----------|-------------|---------|
| `pickupLocation` | string | ? | C�digo del municipio de recogida | `BOGOTA` |
| `returnLocation` | string | ? | C�digo del municipio de devoluci�n | `MEDELLIN` |
| `classCode` | string | ? | C�digo de clase de veh�culo | `SUV`, `ECON`, `VAN` |
| `page` | int | ? | N�mero de p�gina (default: 1) | `1` |
| `pageSize` | int | ? | Tama�o de p�gina (default: 10, max: 100) | `20` |

#### Ejemplo de Petici�n

```bash
curl -X GET "https://localhost:7000/api/v1/vehicles/search?pickupLocation=BOGOTA&returnLocation=MEDELLIN&classCode=SUV&page=1&pageSize=10"
```

#### Ejemplo de Respuesta

```json
{
  "market": "CUNDINAMARCA",
  "pickup": {
    "municipioId": "BOGOTA",
    "municipioName": "Bogot�",
    "departamentoId": "CUNDINAMARCA",
    "departamentoName": "Cundinamarca"
  },
  "return": {
    "municipioId": "MEDELLIN",
    "municipioName": "Medell�n",
    "departamentoId": "ANTIOQUIA",
    "departamentoName": "Antioquia"
  },
  "criteria": {
    "classCode": "SUV",
    "page": 1,
    "pageSize": 10
  },
  "paging": {
    "total": 15,
    "page": 1,
    "pageSize": 10,
    "totalPages": 2
  },
  "vehicles": [
  {
      "vehicleId": "VH-001",
      "classCode": "SUV",
      "className": "SUV Grande",
      "capacity": 7,
      "transmission": "AT",
      "vehicleType": "SUV",
      "isElectric": false,
 "allowsDifferentDropoff": true
 }
  ]
}
```

### Health Check Endpoints

- **`GET /health`**: Estado completo del sistema con detalles
- **`GET /health/ready`**: Readiness probe (API + Database)
- **`GET /health/live`**: Liveness probe (solo check b�sico)

## ?? Testing

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar con cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## ?? Monitoreo y Logs

### Logs
Los logs se generan en formato estructurado y se guardan en:
- **Consola**: Output en tiempo real
- **Archivo**: `logs/milescarrental-YYYY-MM-DD.log` (rotaci�n diaria)

### Tracing
OpenTelemetry instrumenta autom�ticamente:
- Peticiones HTTP (AspNetCore)
- Excepciones capturadas
- Operaciones personalizadas con source `MilesCarRental`

## ?? Mercados Soportados

El sistema **detecta autom�ticamente el mercado** bas�ndose en el municipio de recogida especificado:

- **Detecci�n Autom�tica**: El mercado se resuelve a partir del departamento al que pertenece el municipio de recogida
- **Filtrado Inteligente**: Los veh�culos mostrados son aquellos que operan en el mercado detectado
- **One-Way Rental**: El sistema valida si cada veh�culo permite devoluci�n en el municipio especificado

**Ejemplo de flujo:**
1. Cliente especifica: `pickupLocation=BOGOTA` y `returnLocation=MEDELLIN`
2. Sistema detecta: `market=CUNDINAMARCA` (basado en Bogot�)
3. Sistema filtra: Veh�culos que operan en CUNDINAMARCA
4. Sistema valida: Cu�les permiten devoluci�n en Medell�n

## ?? Rate Limiting

- **L�mite**: 100 peticiones por minuto
- **Identificaci�n**: Por nombre de usuario autenticado o IP
- **Cola**: Hasta 10 peticiones en espera
- **Respuesta**: `429 Too Many Requests` cuando se excede

## ??? Tecnolog�as Utilizadas

- **Framework**: ASP.NET Core 8.0
- **Logging**: Serilog
- **Validaci�n**: FluentValidation
- **Documentaci�n**: Swashbuckle (Swagger/OpenAPI)
- **Observabilidad**: OpenTelemetry
- **Testing**: xUnit, FluentAssertions
- **Patr�n**: Clean Architecture, Repository Pattern, Specification Pattern

## ?? Estructura de Carpetas

```
MilesCarRental/
??? Domain/      # Capa de dominio
?   ??? Entities/           # Entidades de negocio
?   ??? Repositories/  # Interfaces de repositorios
?   ??? Services/           # Interfaces de servicios de dominio
?   ??? Specifications/     # Especificaciones de filtrado
?   ??? Exceptions/         # Excepciones de dominio
??? Application/            # Capa de aplicaci�n
?   ??? DTOs/      # Data Transfer Objects
?   ??? Services/      # Servicios de aplicaci�n
?   ??? Validators/         # Validadores FluentValidation
?   ??? Extensions/         # Extensiones y mapeos
??? Infrastructure/         # Capa de infraestructura
?   ??? InMemory/           # Repositorios en memoria
?   ??? Extensions/  # Configuraci�n de servicios
??? MilesCarRental/    # Capa de presentaci�n (API)
?   ??? Controllers/        # Controladores API
?   ??? Middleware/         # Middleware personalizado
?   ??? Filters/            # Filtros de acci�n
?   ??? HealthChecks/   # Health checks
?   ??? Swagger/    # Configuraci�n Swagger
??? Tests/   # Pruebas unitarias
```

## ?? Contribuci�n

Las contribuciones son bienvenidas. Por favor:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## ?? Licencia

Este proyecto est� bajo la licencia MIT. Ver el archivo `LICENSE` para m�s detalles.

## ?? Autor

**Sebastian CP**
- GitHub: [@sebas99cp](https://github.com/sebas99cp)
- Email: api@milescarrental.com

## ?? Enlaces �tiles

- [Documentaci�n API (Swagger)](https://localhost:7000/swagger)
- [Repositorio GitHub](https://github.com/sebas99cp/MilesCarRental)

---

? Si este proyecto te fue �til, considera darle una estrella en GitHub!
