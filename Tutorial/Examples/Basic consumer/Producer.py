import pika


# Set connection parameters. If connecting to a real server localhost should be replaced by the server´s address.
connection_parameters = pika.ConnectionParameters('localhost')

# Instantiate a connection unsing the connection_parameters previously defined
connection = pika.BlockingConnection(connection_parameters)

# Instantiate a channel
channel = connection.channel()

# Instantiate a queue
channel.queue_declare(queue='Transforms')

# Define the message to send
message = """{"Rhino.Geometry.Transform": "R0=(1,-0.707106781186547,0,1.4142135623731), R1=(0.707106781186547,0.707106781186548,0,1.41421356237309), R2=(0,0,1,0), R3=(0,0,0,1)"}"""


# Publish the message to the letterbox queue
channel.basic_publish(exchange='', routing_key='Transforms', body=message)

# Print the sent message
print(f'sent message: {message}')

# Close connection
connection.close()
