import pika
from pika.exchange_type import ExchangeType


def on_message_received(channe, method, properties, body):
    print(f'[Square consumer] Received message: {body}')


def main():
    # Set connection parameters. If connecting to a real server localhost should be replaced by the server´s address.
    connection_parameters = pika.ConnectionParameters('localhost')

    # Instantiate a connection unsing the connection_parameters previously defined
    connection = pika.BlockingConnection(connection_parameters)

    # Instantiate a channel
    channel = connection.channel()

    # Instantiate an exchange and queue.
    channel.exchange_declare(
        exchange='routing', exchange_type=ExchangeType.direct)
    queue = channel.queue_declare(queue='', exclusive=True)

    # Bind the queue using the "square" and "shape" keys.
    channel.queue_bind(exchange='routing',
                       queue=queue.method.queue, routing_key='square')
    channel.queue_bind(exchange='routing',
                       queue=queue.method.queue, routing_key='shape')

    # Start consuming messages
    channel.basic_consume(queue=queue.method.queue, auto_ack=True,
                          on_message_callback=on_message_received)

    print('[Square consumer] Started consuming messages')

    channel.start_consuming()


if __name__ == '__main__':
    # Run script.
    main()
