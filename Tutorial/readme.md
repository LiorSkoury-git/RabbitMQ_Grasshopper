# RABBITMQ GH
## INTRODUCTION
RabbitMQ GH is a grasshopper plugin for the implementation RabbitMQ messaging capabilities to exchange data from and to external applications and grasshopper scripts in real time, with a high degree of flexibility and scalability.

## TABLE OF CONTENTS
- [Installation](#installation)
- [Use Patterns](#use-patterns)
## INSTALLATION
### Erlang installation
Downoload the current [Erlang](https://erlang.org/download/otp_versions_tree.html) version installer and follow the default setup in the installation wizard.

Add Erlang to the Environment variables. For that go to
*Control Panel>System and security>System*. A new window will open. Click on *Advanced system settings* under the *Device specifications* collapsable panel. When the *System properties* window appears, click on the *Environment Variables* button at the bottom of the *Advanced tab*.

On the User variables section use the *New...* button to add a ne variable with **ERLANG_HOME** as *Variable name* and the installation path as *Variable value* (By default the istallation path should be *C:\Program Files\Erlang OTP*).

To verify that the variable war correctly added, open a comand prompt and enter the ```ECHO %ERLANG_HOME%``` command. If the installation path is printed on the screen, the variable was currectly added.

### RabbitMQ installation
In RabbitMQ´s [GitHub page](https://github.com/rabbitmq/rabbitmq-server/) go to the *Realeases* section on the right hand side of the window. Click on the latest release and navigate to the *assets* section and download the *rabbitmq-server-windows-\<version>.zip* file, where\<version> corresponds to the latest release version number. Extract the compressed files to the directory where you want RabbitMQ to be stored.

To run RabbitMQ, open a termial window and go to the installation directory. Once there, run the ```rabbitmq-server.bat``` command. The current Erlang version and the plugins being used, should be printed on the screen.

In Windows, RabbitMQ does not run on startup by default. To change that, the Startup type of the RabbitMQ Server must be set to *Automatic*

If the plugins are not working properly, follow the same procedure described in the following section to activate them.



### Management plugin activation
To activate the management plugin,open a termial window and go to the installation directory. After that, run the ```rabbitmq-plugins enable rabbitmq_management``` command. This should enable the plugion and it should be working the next time RabbitMQ is launched. The same command works for activating other plugins by replacing *rabbitmq_management* by the plugin's name after it has been downloaded into the plugins directory located on RabbitMQ's installation directory.

For further information, visit this [link](https://youtube.com/playlist?list=PLalrWAGybpB-UHbRDhFsBgXJM1g6T4IvO&si=324s3u0WqtVukrIl).

### Grashopper plugin installation
Download the [plugin files]() and copy them into your grasshopper component folder. If you don´t know the location of this folder on your computer, you can fin it out in grasshoppers interface. Go to *File>Special Folders>Components Folder* and a file explorer window will open in the correct directory.

In general, downloaded *.gha* files can be blocked by Windows os, so verify that all the dowloaded files are unblocked. To do so, find each file in the file explorer and right click on it. In the context menu select *Properties*. This will open a new windodw. Go to the lower part to the *general* tab and check the chebox labeled *Unblock*.

For the use of the plugin, even after closing the *.gh* files, the connections created in the session wil remain open ir the *Run* toggle is set to ```True```. To prevent this, make sure to set the toggle to ```False``` before closing the file.

### Additional resources
* [Linux machine deployment](https://gcore.com/learning/how-to-install-rabbitmq-ubuntu/)
* [AWS deployment](https://docs.aws.amazon.com/amazon-mq/latest/developer-guide/working-with-rabbitmq.html)

## MANAGEMENT PLUGIN
To open the management plugin, in your web borwser navigate to ```localhost:15672``` If no credentials have been established, use ```guest``` as the default *username* and *password* to log in.

## USE PATTERNS
Example files covering several use patterns for Grasshopper, Python and C# are located in the [Examples](https://github.com/LiorSkoury-git/RabbitMQ_Grasshopper/tree/Documentation/Tutorial/Examples) folder. 

Most of them build on top of the consumer-producer pattern. It has two main components: A producer that sends messages to a RabbitMQ queue and a consumer that receives the messages and processes them.

These are all the patterns covered in the example files:

- Consumer-producer
- Competing consumers
- Publisher-subscriber
- Request-reply
- Basic routing
- Topic routing

They are covered in detail in the complementary [readme](Examples/readme.md) file.

