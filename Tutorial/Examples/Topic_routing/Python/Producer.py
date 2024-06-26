import pika
import random
import sys
from pika.exchange_type import ExchangeType


def main(l: float = None):
    # Set connection parameters. When connecting to a real server, localhost should be replaced by the server´s address.
    connection_parameters = pika.ConnectionParameters('localhost')

    # Instantiate a connection unsing the connection_parameters previously defined.
    connection = pika.BlockingConnection(connection_parameters)

    # Instantiate a channel.
    channel = connection.channel()

    # Instantiate an exchang.e
    channel.exchange_declare(
        exchange='topic', exchange_type=ExchangeType.topic)

    # Define the message to send.
    message = f"{l}"

    # Select the routing key to use when sending the message.
    routing_keys = ['square.perimeter', 'square.area']
    key = routing_keys[random.randint(0, len(routing_keys)-1)]

    # Publish the message to the exchange.
    channel.basic_publish(exchange='topic', routing_key=key, body=message)

    # Print the sent message.
    print(f'Sent message: {message} with the key {key}')

    # Close connection.
    connection.close()


if __name__ == '__main__':
    # Access command-line arguments.
    args = sys.argv
    try:
        l = args[1]
    except:
        l = random.randint(1, 6)
        print('Invalid arguments. Random values asigned')
    # Run script.
    main(l)
