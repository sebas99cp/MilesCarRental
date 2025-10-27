# ?? Miles Car Rental API

API REST para búsqueda y gestión de alquiler de vehículos en Colombia, implementada con .NET 8 y arquitectura limpia.

## ??? Arquitectura

El proyecto sigue los principios de **Clean Architecture** con separación en capas:

- **Domain**: Entidades, especificaciones, repositorios y lógica de negocio
- **Application**: Casos de uso, DTOs, validadores y servicios de aplicación
- **Infrastructure**: Implementaciones de repositorios (in-memory)
- **MilesCarRental (API)**: Controllers, middleware, filtros y configuración
- **Tests**: Pruebas unitarias con xUnit

## ? Características

### Core Features
- ? Búsqueda de vehículos por ubicación (municipio/departamento)
- ? Filtrado por clase de vehículo (ECON, SUV, VAN, LUX, ELEC)
- ? Soporte para devolución en ubicación diferente (one-way rental)
- ? Resolución automática de mercado (Nacional/Internacional)
- ? Paginación de resultados
- ? Disponibilidad en tiempo real

### Technical Features
- ??? **Rate Limiting**: 100 peticiones por minuto por usuario/IP
- ?? **Logging estructurado**: Serilog con archivo rotativo diario
- ?? **OpenTelemetry**: Trazabilidad y observabilidad
- ?? **Validación**: FluentValidation en todas las entradas
- ?? **Manejo global de excepciones**: Middleware centralizado
- ?? **Health Checks**: `/health`, `/health/ready`, `/health/live`
- ?? **Swagger/OpenAPI**: Documentación interactiva con ejemplos

## ?? Inicio Rápido

### Requisitos Previos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Instalación

```bash
# Clonar el repositorio
git clone https://github.com/sebas99cp/MilesCarRental.git
cd MilesCarRental

# Restaurar dependencias
dotnet restore

# Ejecutar la aplicación
dotnet run --project MilesCarRental
```

La API estará disponible en: `https://localhost:7000` (o el puerto configurado)

## ?? Uso de la API

### Endpoint Principal

**`GET /api/v1/vehicles/search`**

Busca vehículos disponibles según los criterios especificados. El sistema detecta automáticamente el mercado basándose en los municipios de recogida y devolución.

#### Parámetros de Query

| Parámetro | Tipo | Requerido | Descripción | Ejemplo |
|-----------|------|-----------|-------------|---------|
| `pickupLocation` | string | ? | Código del municipio de recogida | `BOGOTA` |
| `returnLocation` | string | ? | Código del municipio de devolución | `MEDELLIN` |
| `classCode` | string | ? | Código de clase de vehículo | `SUV`, `ECON`, `VAN` |
| `page` | int | ? | Número de página (default: 1) | `1` |
| `pageSize` | int | ? | Tamaño de página (default: 10, max: 100) | `20` |

#### Ejemplo de Petición

```bash
curl -X GET "https://localhost:7000/api/v1/vehicles/search?pickupLocation=BOGOTA&returnLocation=MEDELLIN&classCode=SUV&page=1&pageSize=10"
```

#### Ejemplo de Respuesta

```json
{
  "market": "CUNDINAMARCA",
  "pickup": {
    "municipioId": "BOGOTA",
    "municipioName": "Bogotá",
    "departamentoId": "CUNDINAMARCA",
    "departamentoName": "Cundinamarca"
  },
  "return": {
    "municipioId": "MEDELLIN",
    "municipioName": "Medellín",
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
- **`GET /health/live`**: Liveness probe (solo check básico)

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
- **Archivo**: `logs/milescarrental-YYYY-MM-DD.log` (rotación diaria)

### Tracing
OpenTelemetry instrumenta automáticamente:
- Peticiones HTTP (AspNetCore)
- Excepciones capturadas
- Operaciones personalizadas con source `MilesCarRental`

## ?? Mercados Soportados

El sistema **detecta automáticamente el mercado** basándose en el municipio de recogida especificado:

- **Detección Automática**: El mercado se resuelve a partir del departamento al que pertenece el municipio de recogida
- **Filtrado Inteligente**: Los vehículos mostrados son aquellos que operan en el mercado detectado
- **One-Way Rental**: El sistema valida si cada vehículo permite devolución en el municipio especificado

**Ejemplo de flujo:**
1. Cliente especifica: `pickupLocation=BOGOTA` y `returnLocation=MEDELLIN`
2. Sistema detecta: `market=CUNDINAMARCA` (basado en Bogotá)
3. Sistema filtra: Vehículos que operan en CUNDINAMARCA
4. Sistema valida: Cuáles permiten devolución en Medellín

## ?? Rate Limiting

- **Límite**: 100 peticiones por minuto
- **Identificación**: Por nombre de usuario autenticado o IP
- **Cola**: Hasta 10 peticiones en espera
- **Respuesta**: `429 Too Many Requests` cuando se excede

## ??? Tecnologías Utilizadas

- **Framework**: ASP.NET Core 8.0
- **Logging**: Serilog
- **Validación**: FluentValidation
- **Documentación**: Swashbuckle (Swagger/OpenAPI)
- **Observabilidad**: OpenTelemetry
- **Testing**: xUnit, FluentAssertions
- **Patrón**: Clean Architecture, Repository Pattern, Specification Pattern

## ?? Estructura de Carpetas

```
MilesCarRental/
??? Domain/      # Capa de dominio
?   ??? Entities/           # Entidades de negocio
?   ??? Repositories/  # Interfaces de repositorios
?   ??? Services/           # Interfaces de servicios de dominio
?   ??? Specifications/     # Especificaciones de filtrado
?   ??? Exceptions/         # Excepciones de dominio
??? Application/            # Capa de aplicación
?   ??? DTOs/      # Data Transfer Objects
?   ??? Services/      # Servicios de aplicación
?   ??? Validators/         # Validadores FluentValidation
?   ??? Extensions/         # Extensiones y mapeos
??? Infrastructure/         # Capa de infraestructura
?   ??? InMemory/           # Repositorios en memoria
?   ??? Extensions/  # Configuración de servicios
??? MilesCarRental/    # Capa de presentación (API)
?   ??? Controllers/        # Controladores API
?   ??? Middleware/         # Middleware personalizado
?   ??? Filters/            # Filtros de acción
?   ??? HealthChecks/   # Health checks
?   ??? Swagger/    # Configuración Swagger
??? Tests/   # Pruebas unitarias
```

## ?? Contribución

Las contribuciones son bienvenidas. Por favor:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## ?? Licencia

Este proyecto está bajo la licencia MIT. Ver el archivo `LICENSE` para más detalles.

## ?? Autor

**Sebastian CP**
- GitHub: [@sebas99cp](https://github.com/sebas99cp)
- Email: api@milescarrental.com

## ?? Enlaces Útiles

- [Documentación API (Swagger)](https://localhost:7000/swagger)
- [Repositorio GitHub](https://github.com/sebas99cp/MilesCarRental)

---

? Si este proyecto te fue útil, considera darle una estrella en GitHub!
