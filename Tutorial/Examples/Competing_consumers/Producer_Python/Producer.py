import pika
import time

# Set connection parameters. If connecting to a real server localhost should be replaced by the server´s address.
connection_parameters = pika.ConnectionParameters('localhost')

# Instantiate a connection unsing the connection_parameters previously defined.
connection = pika.BlockingConnection(connection_parameters)

# Instantiate a channel.
channel = connection.channel()

# Instantiate a queue.
channel.queue_declare(queue='CCQueue')

# Variable steup for message construction.
count = 0
A = [0, 0, 0]
B = [1, 0, 0]
adder = 1

# Continuously send messages whit a random delay between them.
while (True):

    # Compose the message body.
    if count % 2 == 0:
        message = A[:]
    else:
        message = B[:]

    message[1] = count
    message = "{},{},{}".format(message[0], message[1], message[2])

    # Publish the message.
    channel.basic_publish(exchange='', routing_key='CCQueue', body=message)

    print(f"sent message: {message}")

    # Calculate delay.
    time.sleep(0.1)

    # Update the id for the next message.

    if count > 20 or count < 0:
        adder *= -1
    count += adder
