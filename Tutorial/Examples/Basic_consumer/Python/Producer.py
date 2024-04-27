import pika
import random
import sys


def main(x: float = None, y: float = None, z: float = None):
    # Set connection parameters. When connecting to a real server, 'localhost' should be replaced by the server´s address.
    connection_parameters = pika.ConnectionParameters('localhost')

    # Instantiate a connection unsing the connection_parameters previously defined.
    connection = pika.BlockingConnection(connection_parameters)

    # Instantiate a channel.
    channel = connection.channel()

    # Instantiate a queue.
    channel.queue_declare(queue='Transforms')

    # Define the default message to send.
    message = f"{x},{y},{z}"

    # Publish the message to the 'Transforms' queue.
    channel.basic_publish(exchange='', routing_key='Transforms', body=message)

    # Print the sent message.
    print(f'Sent message: {message}')

    # Close connection.
    connection.close()


if __name__ == '__main__':
    # Access command-line arguments.
    args = sys.argv
    try:
        x = args[1]
        y = args[2]
        z = args[3]
    except:
        x = random.randint(1, 6)
        y = random.randint(1, 6)
        z = random.randint(1, 6)
        print('Invalid arguments. Random values asigned')
    # Run script.
    main(x, y, z)
