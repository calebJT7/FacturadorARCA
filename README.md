# Sistema Backend de Facturación (FacturadorARCA)

Este proyecto backend fue desarrollado en equipo como parte de una evaluación académica, simulando el facturador web de ARCA mediante una aplicación de consola en C#. El sistema está diseñado con un fuerte enfoque en la arquitectura limpia y buenas prácticas de desarrollo.

## Funcionalidades Principales (Operaciones CRUD)

- **Gestión de Entidades:** Alta, Baja, Modificación y Consulta de Clientes y Facturas[cite: 11].
- **Emisión de Comprobantes:** Generación de Facturas (A, B o C) validando el registro previo del cliente y calculando los importes totales de los ítems[cite: 16, 17, 29].
- **Validación de Reglas de Negocio:** Manejo de errores y validación de datos ingresados por el usuario con mensajes de confirmación/excepción[cite: 67, 69].

## Tecnologías y Arquitectura

- **Lenguaje:** C# / .NET
- **ORM & Base de Datos:** Entity Framework Core (Code First con Migraciones)[cite: 12].
- **Arquitectura:** Programación Orientada a Objetos (POO) aplicando Abstracción, Encapsulamiento y Separación de Responsabilidades[cite: 74, 76].

## Trabajo Colaborativo

- Desarrollado en un equipo de 3 integrantes, aplicando control de versiones y división de tareas lógicas[cite: 83].

## Instalación y Ejecución

1. Clonar el repositorio.
2. Abrir la solución en Visual Studio o VS Code.
3. Ejecutar las migraciones de Entity Framework para generar la base de datos local.
4. Ejecutar el proyecto mediante el comando `dotnet run`.
