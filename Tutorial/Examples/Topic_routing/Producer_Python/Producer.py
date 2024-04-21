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
channel.exchange_declare(exchange='topic', exchange_type=ExchangeType.topic)

# Define the message to send
l = random.randint(1, 6)
message = f"{l}"

# Select the routing key to use when sending the message.
routing_keys = ['square.perimeter', 'square.area']
key = routing_keys[random.randint(0, len(routing_keys)-1)]

# Publish the message to the letterbox queue
channel.basic_publish(exchange='topic', routing_key=key, body=message)

# Print the sent message
print(f'Sent message: {message} with the key {key}')

# Close connection
connection.close()
