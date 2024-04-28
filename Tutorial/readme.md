# RABBITMQ GH
## INTRODUCTION
RabbitMQ GH is a Grasshopper plugin for implementing RabbitMQ messaging capabilities to exchange data between external applications and Grasshopper scripts in real time, with a high degree of flexibility and scalability.

## TABLE OF CONTENTS
- [Installation](#installation)
- [Use patterns](#use-patterns)

## INSTALLATION
### Erlang installation
Download the current [Erlang](https://erlang.org/download/otp_versions_tree.html) version installer and follow the default setup in the installation wizard.

Add Erlang to the Environment variables. To do it, go to
*Control Panel>System and Security>System*. A new window will open. Click on *Advanced system settings* under the *Device specifications* collapsable panel. When the *System Properties* window appears, click on the *Environment Variables* button at the bottom of the *Advanced* tab.

On the *User variables* section use the *New...* button to add a new variable with **ERLANG_HOME** as the *Variable name* and the installation path as the *variable value* (By default the installation path should be *C:\Program Files\Erlang OTP*).

To verify that the variable was correctly added, open a command prompt and enter the ```ECHO %ERLANG_HOME%``` command. If the installation path is printed on the screen, the variable is correctly added.

### RabbitMQ installation
On RabbitMQ´s [GitHub page](https://github.com/rabbitmq/rabbitmq-server/) go to the *Releases* section on the right-hand side of the window. Click on the latest release and navigate to the *assets* section and download the *rabbitmq-server-windows-\<version>.zip* file, where\<version> corresponds to the latest release version number. Extract the compressed files to the directory where you want RabbitMQ to be stored.

To run RabbitMQ, open a terminal window and go to the installation directory. Once there, run the ```rabbitmq-server.bat``` command. The current Erlang version and the plugins being used should be printed on the screen.

In Windows, RabbitMQ does not run on startup by default. To change that, the Startup type of the RabbitMQ Server must be set to *Automatic*

If the plugins are not working properly, follow the same procedure described in the following section to activate them.

### Management plugin activation
To activate the management plugin, open a terminal window and go to the installation directory. After that, run the ```rabbitmq-plugins enable rabbitmq_management``` command. This should enable the plugin and it should be working the next time RabbitMQ is launched. The same command works for activating other plugins by replacing *rabbitmq_management* with the plugin's name after it has been downloaded into the plugin's directory located on RabbitMQ's installation directory.

For further information, follow this [link](https://youtube.com/playlist?list=PLalrWAGybpB-UHbRDhFsBgXJM1g6T4IvO&si=324s3u0WqtVukrIl).

### Grasshopper plugin installation
Download the [plugin files]() and copy them into your Grasshopper component folder. If you don´t know the location of this folder on your computer, you can find it using Grasshopper's interface. Go to *File>Special Folders>Components Folder* and a file explorer window will open in the correct directory.

In general, downloaded *.gha* files can be blocked by Windows OS, so verify that all the downloaded files are unblocked. To do so, find each file in the file explorer and right-click on it. In the context menu select *Properties*. This will open a new window. Go to the lower part of the *general* tab and check the checkbox labeled *Unblock*.

For the use of the plugin, even after closing the *.gh* files, the connections created in the session will remain open if the *Run* toggle is set to ```True```. To prevent this, make sure to set the toggle to ```False``` before closing the file.

### Additional resources
* [Linux machine deployment](https://gcore.com/learning/how-to-install-rabbitmq-ubuntu/)
* [AWS deployment](https://docs.aws.amazon.com/amazon-mq/latest/developer-guide/working-with-rabbitmq.html)

## MANAGEMENT PLUGIN
To open the management plugin, in your web browser navigate to ```localhost:15672``` If no credentials have been established, use ```guest``` as the default *username* and *password* to log in.

## USE PATTERNS
Example files covering several use patterns for Grasshopper, Python, and C# are located in the [Examples folder](https://github.com/LiorSkoury-git/RabbitMQ_Grasshopper/tree/Documentation/Tutorial/Examples). 

Most of them build on top of the consumer-producer pattern. It has two main components: A producer that sends messages to a RabbitMQ queue and a consumer that receives the messages from the queue and processes them.

These are all the patterns covered in the example files with a detailed explanation in [this](Examples/readme.md) file:

- [Basic consumer](https://github.com/LiorSkoury-git/RabbitMQ_Grasshopper/tree/Documentation/Tutorial/Examples/Basic_consumer)
- [Competing consumers](https://github.com/LiorSkoury-git/RabbitMQ_Grasshopper/tree/Documentation/Tutorial/Examples/Competing_consumers)
- [Publisher-subscriber](https://github.com/LiorSkoury-git/RabbitMQ_Grasshopper/tree/Documentation/Tutorial/Examples/Publisher_subscriber)
- [Request-reply](https://github.com/LiorSkoury-git/RabbitMQ_Grasshopper/tree/Documentation/Tutorial/Examples/Request_reply)
- [Basic routing](https://github.com/LiorSkoury-git/RabbitMQ_Grasshopper/tree/Documentation/Tutorial/Examples/Basic_routing)
- [Topic routing](https://github.com/LiorSkoury-git/RabbitMQ_Grasshopper/tree/Documentation/Tutorial/Examples/Topic_routing)
