import pika
import time
import random


# Callback method to process received messages.
def on_message_received(ch, method, properties, body):
    # Set a random time that simulates the processing of the mesage.
    processing_time = random.randint(1, 6)
    # Print the received message.
    print(f'[Consumer 2] Received message: {body}')
    # Simulate message processing.
    time.sleep(processing_time)
    # Acknowledge the received message after processing it.
    ch.basic_ack(delivery_tag=method.delivery_tag)


def main(connection):

    # Instantiate a queue.
    channel = connection.channel()

    # Queue declaration.
    channel.queue_declare(queue='CCQueue')

    # Set the number of messages to be retreived by the consumer.
    channel.basic_qos(prefetch_count=1)

    # Set up the consuminng behavior.
    channel.basic_consume(
        queue='CCQueue', on_message_callback=on_message_received)

    # Start the consumer.
    print('[Consumer 2] Started Consuming')
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
