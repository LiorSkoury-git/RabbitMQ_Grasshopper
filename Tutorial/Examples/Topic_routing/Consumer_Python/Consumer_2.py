import pika
from pika.exchange_type import ExchangeType


def on_message_received(channe, method, properties, body):
    print(f'[Area consumer] Received message: {body}')


# Set connection parameters. If connecting to a real server localhost should be replaced by the server´s address.
connection_parameters = pika.ConnectionParameters('localhost')

# Instantiate a connection unsing the connection_parameters previously defined
connection = pika.BlockingConnection(connection_parameters)

# Instantiate a channel
channel = connection.channel()

# Instantiate an exchange and queue.
channel.exchange_declare(exchange='topic', exchange_type=ExchangeType.topic)
queue = channel.queue_declare(queue='', exclusive=True)

# Bind the queue using the "square" and "both" keys.
channel.queue_bind(exchange='topic',
                   queue=queue.method.queue, routing_key='square.area')


# Start consuming messages
channel.basic_consume(queue=queue.method.queue, auto_ack=True,
                      on_message_callback=on_message_received)

print('[Area consumer] Started consuming messages')

channel.start_consuming()
