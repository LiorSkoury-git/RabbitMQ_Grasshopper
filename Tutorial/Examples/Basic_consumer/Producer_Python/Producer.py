import pika
import random


# Set connection parameters. If connecting to a real server localhost should be replaced by the server´s address.
connection_parameters = pika.ConnectionParameters('localhost')

# Instantiate a connection unsing the connection_parameters previously defined
connection = pika.BlockingConnection(connection_parameters)

# Instantiate a channel
channel = connection.channel()

# Instantiate a queue
channel.queue_declare(queue='Transforms')

# Define the message to send
x = random.randint(1, 6)
y = random.randint(1, 6)
z = random.randint(1, 6)
message = f"{x},{y},{z}"


# Publish the message to the letterbox queue
channel.basic_publish(exchange='', routing_key='Transforms', body=message)

# Print the sent message
print(f'Sent message: {message}')

# Close connection
connection.close()
