import pika
from pika.exchange_type import ExchangeType
import random

# Set connection parameters. If connecting to a real server localhost should be replaced by the server´s address.
connection_parameters = pika.ConnectionParameters('localhost')

# Instantiate a connection unsing the connection_parameters previously defined.
connection = pika.BlockingConnection(connection_parameters)

# Instantiate a channel.
channel = connection.channel()

# Instantiate an exchange.
channel.exchange_declare(exchange='pubsub', exchange_type=ExchangeType.fanout)

# Define the message to send.
x = random.randint(1, 6)
y = random.randint(1, 6)
z = random.randint(1, 6)
d = random.randint(2, 5)
message = f"{x},{y},{z},{d}"

# Publish the message.
channel.basic_publish(exchange='pubsub', routing_key='', body=message)

print(f"Sent message: {message}")

connection.close()
