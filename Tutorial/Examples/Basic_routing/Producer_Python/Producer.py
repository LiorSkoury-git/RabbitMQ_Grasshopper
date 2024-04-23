import pika
import random
from pika.exchange_type import ExchangeType


# Set connection parameters. If connecting to a real server localhost should be replaced by the server´s address.
connection_parameters = pika.ConnectionParameters('localhost')

# Instantiate a connection unsing the connection_parameters previously defined
connection = pika.BlockingConnection(connection_parameters)

# Instantiate a channel
channel = connection.channel()

# Instantiate an exchange
channel.exchange_declare(exchange='routing', exchange_type=ExchangeType.direct)

# Define the message to send
x = random.randint(1, 6)
y = random.randint(1, 6)
l = random.randint(2, 4)
message = f"{x},{y},{l}"

# Select the routing key to use when sending the message.
routing_keys = ['square', 'circle', 'shape']
key = routing_keys[random.randint(0, len(routing_keys)-1)]

# Publish the message to the letterbox queue
channel.basic_publish(exchange='routing', routing_key=key, body=message)

# Print the sent message
print(f'Sent message: {message} with the key {key}')

# Close connection
connection.close()
