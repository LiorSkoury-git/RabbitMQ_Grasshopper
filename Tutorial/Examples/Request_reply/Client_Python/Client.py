import pika
import uuid
import random


def on_reply_message_received(ch, method, properties, body):
    print(f"Received reply: {body}")


# Set connection parameters. If connecting to a real server localhost should be replaced by the server´s address.
connection_parameters = pika.ConnectionParameters('localhost')

# Instantiate a connection unsing the connection_parameters previously defined
connection = pika.BlockingConnection(connection_parameters)

# Instantiate a channel
channel = connection.channel()

# Instantiate the queue for replies.
reply_queue = channel.queue_declare(queue='', exclusive=True)

# Start consuming repli.
channel.basic_consume(queue=reply_queue.method.queue, auto_ack=True,
                      on_message_callback=on_reply_message_received)

# Instantiate the queue for requests.
channel.queue_declare(queue='request-queue')

# Set the correlation id to reference requests when replying.
cor_id = str(uuid.uuid4())
body = str(random.randint(1, 6))

print("Client started")

# Send a request
channel.basic_publish('', routing_key='request-queue', properties=pika.BasicProperties(
    reply_to=reply_queue.method.queue,
    correlation_id=cor_id
), body='4')

print(f"Sent area request {cor_id} for diameter {body}")

channel.start_consuming()
