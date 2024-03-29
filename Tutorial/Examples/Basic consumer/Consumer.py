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

# Close connection
# connection.close()

# Expected message
# {"Rhino.Geometry.Transform":"R0=(0.707106781186548,-0.707106781186547,0,1.4142135623731), R1=(0.707106781186547,0.707106781186548,0,1.41421356237309), R2=(0,0,1,0), R3=(0,0,0,1)"}
