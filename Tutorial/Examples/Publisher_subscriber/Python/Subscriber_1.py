import pika


def on_message_received(channe, method, properties, body):
    print(f'[Subscriber 1] Received message: {body}')


def main(connection):

    # Instantiate a channel.
    channel = connection.channel()

    # Instantiate an exchange.
    channel.exchange_declare(exchange='pubsub', exchange_type='fanout')

    # Instantiate a queue.
    queue = channel.queue_declare(queue='', exclusive=True)
    channel.queue_bind(exchange='pubsub', queue=queue.method.queue)

    # Publish the message to the pubsub exchange.
    channel.basic_consume(queue=queue.method.queue, auto_ack=True,
                          on_message_callback=on_message_received)

    print('[Subscriber 1] Started consuming messages')

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
