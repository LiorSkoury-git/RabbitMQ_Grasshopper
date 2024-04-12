import pika


def on_message_received(channe, method, properties, body):
    print(f'Received message: {body}')


# Set connection parameters. If connecting to a real server localhost should be replaced by the server´s address.
connection_parameters = pika.ConnectionParameters('localhost')

# Instantiate a connection unsing the connection_parameters previously defined
connection = pika.BlockingConnection(connection_parameters)

# Instantiate a channel
channel = connection.channel()

# Instantiate a queue
channel.queue_declare(queue='Transforms')

# Publish the message to the letterbox queue
channel.basic_consume(queue='Transforms', auto_ack=True,
                      on_message_callback=on_message_received)

print('Started consuming messages')

channel.start_consuming()
