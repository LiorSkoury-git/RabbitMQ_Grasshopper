import pika
from pika.exchange_type import ExchangeType


def on_message_received(channe, method, properties, body):
    print(f'[General consumer] Received message: {body}')


def main(connection):

    # Instantiate a channel.
    channel = connection.channel()

    # Instantiate an exchange and queue.
    channel.exchange_declare(
        exchange='topic', exchange_type=ExchangeType.topic)
    queue = channel.queue_declare(queue='', exclusive=True)

    # Bind the queue using the "square.*" key.
    channel.queue_bind(exchange='topic',
                       queue=queue.method.queue, routing_key='square.*')

    # Start consuming messages.
    channel.basic_consume(queue=queue.method.queue, auto_ack=True,
                          on_message_callback=on_message_received)

    print('[General consumer] Started consuming messages')

    channel.start_consuming()


if __name__ == '__main__':

    # Set connection parameters. When connecting to a real server, 'localhost' should be replaced by the server´s address.
    connection_parameters = pika.ConnectionParameters('localhost')

    # Instantiate a connection unsing the connection_parameters previously defined.
    connection = pika.BlockingConnection(connection_parameters)

    try:
        # Run script.
        main(connection)

    except KeyboardInterrupt:
        # Handle user interruption.
        print('Loop stopped by user')
        connection.close()
