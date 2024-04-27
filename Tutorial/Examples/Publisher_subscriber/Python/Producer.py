import pika
import sys
import random
from pika.exchange_type import ExchangeType


def main(x: float = None, y: float = None, z: float = None, d: float = None):
    # Set connection parameters. When connecting to a real server, localhost should be replaced by the server´s address.
    connection_parameters = pika.ConnectionParameters('localhost')

    # Instantiate a connection unsing the connection_parameters previously defined.
    connection = pika.BlockingConnection(connection_parameters)

    # Instantiate a channel.
    channel = connection.channel()

    # Instantiate an exchange.
    channel.exchange_declare(
        exchange='pubsub', exchange_type=ExchangeType.fanout)

    # Define the message to send.
    message = f"{x},{y},{z},{d}"

    # Publish the message.
    channel.basic_publish(exchange='pubsub', routing_key='', body=message)

    print(f"Sent message: {message}")

    connection.close()


if __name__ == '__main__':
    # Access command-line arguments.
    args = sys.argv
    try:
        x = args[1]
        y = args[2]
        z = args[3]
        d = args[4]
    except:
        x = random.randint(1, 6)
        y = random.randint(1, 6)
        z = random.randint(1, 6)
        d = random.randint(2, 5)
        print('Invalid arguments. Random values asigned')
    # Run script.
    main(x, y, z, d)
