import pika
import random
import sys
from pika.exchange_type import ExchangeType


def main(x: float = None, y: float = None, l: float = None):
    # Set connection parameters. If connecting to a real server localhost should be replaced by the server´s address.
    connection_parameters = pika.ConnectionParameters('localhost')

    # Instantiate a connection unsing the connection_parameters previously defined
    connection = pika.BlockingConnection(connection_parameters)

    # Instantiate a channel
    channel = connection.channel()

    # Instantiate an exchange
    channel.exchange_declare(
        exchange='routing', exchange_type=ExchangeType.direct)

    # Define the message to send
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


if __name__ == '__main__':
    # Access command-line arguments.
    args = sys.argv
    try:
        x = args[1]
        y = args[2]
        l = args[3]
    except:
        x = random.randint(1, 6)
        y = random.randint(1, 6)
        l = random.randint(1, 6)
        print('Invalid arguments. Random values asigned')
    # Run script.
    main(x, y, l)
