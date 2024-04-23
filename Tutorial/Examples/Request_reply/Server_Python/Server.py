import pika
import math


def area_from_diam(diam: float):
    return math.pi*diam**2/4


def on_request_message_received(ch, method, properties, body):
    print(f"Area request with id:{properties.correlation_id}")
    ch.basic_publish('', routing_key=properties.reply_to,
                     body=f"The requested area for {properties.correlation_id} is {area_from_diam(float(body.decode('utf-8')))}")


def main():

    # Set connection parameters. If connecting to a real server localhost should be replaced by the server´s address.
    connection_parameters = pika.ConnectionParameters('localhost')

    # Instantiate a connection unsing the connection_parameters previously defined
    connection = pika.BlockingConnection(connection_parameters)

    # Instantiate a channel
    channel = connection.channel()

    # Instantiate the queue for requests.
    channel.queue_declare(queue='request-queue')

    # Start consuming requests.
    channel.basic_consume(queue='request-queue', auto_ack=True,
                          on_message_callback=on_request_message_received)

    print("Server started")

    channel.start_consuming()


if __name__ == '__main__':
    # Run script.
    main()
