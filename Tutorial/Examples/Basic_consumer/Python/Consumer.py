import pika

# Callback method to handle received messages.


def on_message_received(channe, method, properties, body):
    print(f'Received message: {body}')


def main(connection):
    # Instantiate a channel.
    channel = connection.channel()

    # Instantiate a queue.
    channel.queue_declare(queue='Transforms')

    # Publish the message to the 'Transforms' queue.
    channel.basic_consume(queue='Transforms', auto_ack=True,
                          on_message_callback=on_message_received)

    print('Started consuming messages')

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
