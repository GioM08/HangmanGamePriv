# HangmanGame

**HangmanGame** es una aplicación de escritorio desarrollada en **C# con .NET Framework 4.7.2**, basada en el juego clásico del ahorcado. El sistema permite a los usuarios registrarse, iniciar sesión, crear partidas, unirse a partidas disponibles, jugar en modalidad multijugador, consultar puntajes, administrar amistades y comunicarse durante una partida mediante chat.

El proyecto fue desarrollado como parte de la Experiencia Educativa de **Tecnologías para la Construcción de Software**, con el objetivo de aplicar una arquitectura cliente-servidor, comunicación distribuida, separación por capas, persistencia de datos y buenas prácticas de diseño de software.

## Repositorio principal

Este repositorio concentra la versión integrada del sistema, incluyendo el backend, el cliente de escritorio, la lógica de negocio, las entidades compartidas y el acceso a datos.

## Repositorios que precedieron a esta versión

Antes de integrar el proyecto en este repositorio, el desarrollo se trabajó de forma separada en los siguientes repositorios:

* Backend y servicios iniciales:
  https://github.com/AlejandroMartinez5/Hangman

* Cliente de escritorio inicial:
  https://github.com/AnaGeorgina15/HangmanClient

Estos repositorios sirvieron como base para la construcción de la versión integrada de HangmanGame, donde se unificaron las funcionalidades del cliente, los servicios, la lógica de negocio y la persistencia de datos.

## Descripción general del sistema

HangmanGame permite que un usuario cree una partida seleccionando una categoría de palabra. Otro usuario puede unirse a una partida disponible y ambos jugadores participan en el proceso de adivinar la palabra. Durante la partida, el sistema mantiene actualizado el estado del juego, registra los movimientos realizados y determina si el jugador gana, pierde o abandona la partida.

Además del flujo principal del juego, el sistema incluye funcionalidades complementarias como recuperación de contraseña, verificación de correo electrónico, historial de partidas, tabla de posiciones, perfil de usuario, sistema de amistades y chat en tiempo real.

## Funcionalidades principales

* Registro de usuarios.
* Inicio de sesión.
* Recuperación de contraseña.
* Verificación de correo electrónico.
* Creación de partidas.
* Consulta y unión a partidas disponibles.
* Adivinar letras durante una partida.
* Control de victoria, derrota y abandono de partida.
* Historial de partidas y puntajes.
* Tabla de posiciones.
* Gestión de amigos.
* Envío, respuesta, cancelación y eliminación de solicitudes de amistad.
* Visualización y edición de perfil.
* Chat entre jugadores durante una partida.
* Soporte de idioma mediante archivos de recursos `.resx`.

## Arquitectura del proyecto

El sistema está organizado mediante una arquitectura por capas, con el propósito de separar responsabilidades y facilitar el mantenimiento del código.

### Capas principales

* **HangmanGameWPF**
  Aplicación de escritorio desarrollada en WPF. Representa la capa de presentación con la que interactúa el usuario.

* **HangmanGameServices**
  Capa de servicios WCF encargada de exponer las operaciones del sistema al cliente.

* **HangmanGameBusiness**
  Capa de lógica de negocio. Contiene validaciones, reglas del juego, seguridad, recuperación de cuenta y gestión de usuarios.

* **HangmanGameData**
  Capa de acceso a datos. Utiliza LINQ to SQL y el patrón repositorio para consultar y modificar la base de datos.

* **HangmanGameEntities**
  Capa de entidades y DTOs utilizados para transportar información entre cliente, servicios y lógica de negocio.

* **HangmanGameDB**
  Base de datos relacional en SQL Server, donde se almacena la información de usuarios, partidas, palabras, categorías, movimientos, amistades y puntajes.

## Tecnologías utilizadas

| Tecnología             | Uso dentro del proyecto                                 |
| ---------------------- | ------------------------------------------------------- |
| C#                     | Lenguaje principal de desarrollo                        |
| .NET Framework 4.7.2   | Plataforma base del sistema                             |
| WPF                    | Desarrollo de la interfaz gráfica de escritorio         |
| WCF                    | Comunicación entre cliente y servidor                   |
| WCF dúplex / callbacks | Sincronización del estado de la partida en tiempo real  |
| TCP Sockets            | Chat entre jugadores                                    |
| SQL Server             | Persistencia de datos                                   |
| LINQ to SQL            | Mapeo y consultas hacia la base de datos                |
| PBKDF2                 | Protección de contraseñas mediante hashing              |
| `.resx`                | Localización de textos y mensajes                       |
| DTOs                   | Transporte seguro y ordenado de información entre capas |

## Comunicación del sistema

La aplicación cliente no accede directamente a la base de datos. En su lugar, se comunica con el backend mediante servicios WCF. Esta decisión permite separar la interfaz gráfica de la lógica del sistema y centralizar las reglas de negocio en el servidor.

Para el flujo de la partida se utiliza WCF dúplex, lo que permite que el servidor notifique cambios al cliente sin que este tenga que consultar constantemente el estado de la partida. Por otro lado, el chat se maneja mediante sockets TCP como un canal independiente de mensajería.

## Seguridad

El sistema no almacena contraseñas en texto plano. Para proteger las credenciales de los usuarios, se utiliza PBKDF2, generando hashes con sal e iteraciones. Además, la información que viaja entre capas se maneja mediante DTOs, evitando exponer directamente las entidades internas de la base de datos.

## Base de datos

La base de datos del sistema fue diseñada en SQL Server. Entre sus principales entidades se encuentran:

* Users
* Categories
* Words
* Games
* Moves
* FriendRequests
* Friends
* VerificationCodes

Estas tablas permiten almacenar la información necesaria para el funcionamiento del juego, la administración de usuarios, la gestión de partidas, el historial, las amistades y los procesos de verificación de cuenta.

## Integrantes

* Alejandro Martínez Ramírez
* Ana Georgina Rejón Osorio
* Giovanni Morales Nesticapan
* Joana Xcaret García Canseco

## Estado del proyecto

El proyecto integra las funcionalidades principales requeridas para el funcionamiento de HangmanGame como aplicación cliente-servidor. La versión actual concentra el trabajo previamente desarrollado en repositorios separados y lo organiza en una solución unificada con capas definidas, servicios WCF, cliente WPF y persistencia en SQL Server.
