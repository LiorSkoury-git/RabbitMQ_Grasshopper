import pika
import time
import random

# Set connection parameters. If connecting to a real server localhost should be replaced by the server´s address.
connection_parameters = pika.ConnectionParameters('localhost')

# Instantiate a connection unsing the connection_parameters previously defined.
connection = pika.BlockingConnection(connection_parameters)

# Instantiate a channel.
channel = connection.channel()

# Instantiate a queue.
channel.queue_declare(queue='CCQueue')

# Define an id for the message to send.
messageId = 1

# Continuously send messages whit a random delay between them.
while (True):

    # Compose the message body.
    message = f"This is message No. {messageId}"

    # Publish the message.
    channel.basic_publish(exchange='', routing_key='CCQueue', body=message)

    print(f"sent message: {message}")

    # Calculate delay.
    time.sleep(random.randint(1, 4))

    # Update the id for the next message.
    messageId += 1
