VIPO Do It Yourself  es una plantilla de Unity con todo lo necesario para que hagas tu propia interfaz de VIPO!

Qué es un VIPO?
===========

VIPO  representa en inglés : "Virtual Interactive Puppet Overlay" y es un tipo de VTuber. Normalmente, controlas un personaje como un muñeco y tus espectadores pueden interactuar con tu interafz mediante diferentes eventos de Twitch como : Mensajes del chat y comandos, bits, suscripciones...   

Un VIPO puede desarrollarse con cualquier motor de videojuegos. Ya hay algunos VTubers que usan Unreal Engine, Unity e incluso Godot! Algunos increíbles ejemplos son : 

- [DoigSwift](https://www.twitch.tv/doigswift)

- [ReneRightHere](https://www.twitch.tv/renerighthere)

- [Drako_Fox](https://www.twitch.tv/drako_fox)


¿Qué puedo hacer con VIPO DIY?
----------------

Aunque Twitch ofrece diferentes formas de acceder a eventos de Twitch, estas pueden ser demasiado complicado para un principiante. VIPO DIY, con la ayuda de Streamer.bot, conseguimos registrar la mayoría de los eventos de Twitch y lo transformamos en datos para que el usuario los pueda usar de la forma que quiera.

Actualmente, VIPO DIY puede registrar los siguientes eventos: Mensajes del chat, Comandos, Recompensas de Twitch, Bits, Suscripciones, Suscripciones de regalo, Raids y Seguidores.

Instalación e importar aplicaciones de terceros
===========

Instalar Streamer.bot y Spout
----------------
Para instalar el proyecto es necesario tener Streamer.bot que podras descargar en su [página](https://streamer.bot). También es necesario descargar el plugin de Spout para OBS Studio en esta [página](https://knowledge.offworld.live/en/articles/5059810-spout-plugin-for-obs-studio) siguiendo las instrucciones.

¿COMO IMPORTAR ACCIONES Y REALIZAR PRUEBAS?
----------------
Para importar las acciones, hay que copiar el string contenido en el archivo **"VIPO DIY Streamerbot configuration"** en la carpeta **"StreamerBot Stuff"**. Una vez se ha insertado el string en el cuadro que provee Streamer.bot, debería cambiar a la siguiente imagen

![Ventana tras introducir el enlace](ImagesReadMe/ImportGuide.png)

Selecciona **"Import"** y las acciones estarán importadas.

Para comprobar el funcionamiento de una acción, en el apartado de **"Triggers"** habra ciertos triggers a activar si se hace click derecho y se activa **"Test Trigger"**.

Como activar el servidor UDP en Streamer.bot
----------------
En la aplicación de Streamer.bot hay una sección llamada **"Servers/Clients"**.

![Servidor UDP](ImagesReadMe/UDPServerSetUp.png)

En la sección **"UDP Server"** deja el puerto en 4242 o asegurate que el puerto sea el mismo que en el proyecto de Unity en el objeto **"StreamerBotManager"** en la variable UDP Server Port en el script **UDP Send**. Este mismo objeto tiene un script **Streamer Bot Event Manager** con una variable Port, la cual tiene que tener el mismo valor en Unity y en todos los **"UDP Broadcast"** que realizan todas las acciones.

![Servidor UDP en Unity](ImagesReadMe/UDPUnity.png)

COMO IMPORTAR EL GRUPO DE STREAMERS PARA RAIDS
----------------
En Streamer.bot, en el apartado de **"Settings"** en **"Groups"**, se debe crear un grupo llamado **"Streamers"** y hacer click derecho y seleccionar **"Import from File"** para importar un archivo y selecciona el archivo **"Streamers Group"** en la carpeta **"StreamerBot Stuff"**. Para añadir más usuarios, escribe el nombre exacto del usuario en Twitch en el cuadro **"Add Multiple Twitch Users to Group"** y seleccionar Add Users

![Grupo de streamers en Streamer.bot](ImagesReadMe/GrupoStreamers.png)

REQUISITOS DEL SISTEMA
----------------

Estos son los requisitos del sistema para ejecutar la aplicación exportada de esta plantilla de Unity, la aplicación de Streamer.bot Y la aplicación de OBS. VIPO DIY  ha sido probado con estos requisitos pero no son necesariamente los requisitos mínimos

- SO : Windows 10 x64, Windows 11 x64
- CPU Processor : Intel® Core™ i7-1165G7 Processor 2.8 GHz (12M Cache, up to 4.7 GHz, 4 cores)
- Memory : 16GB DDR4 on board
- Graphics : NVIDIA® GeForce® GTX 1650 Max Q 4GB GDDR6

Estos requisitos NO incluyen nada que el usuario quiera añadir. Si quieres añadir más caracteristicas, puede que tu aplicación requiera mas recursos de tu sistema.

DISCLAIMER
===========
- VIPO DIY usa 6000.0.34
- VIPO DIY se ha probado con Streamer.bot 1.0.4
- Cuando la aplicación final haya sido exportada, se esta ejecutando en segundo plano como cualquier otra aplicación. Puedes grabar esa aplicación como ventana desde OBS. 
- La documentacion dentro del proyecto esta en inglés y español
- Recomiendo encarecidamente un conocimiento básico de Unity y programación en C# para aprovechar al máximo el potencial  de esta plantilla.