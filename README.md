# RabbitMQ tryout
The source code for my RabbitMQ tryout.

RabbitMQ is an open-source message broker that makes communication between services very easy. In particular, RabbitMQ uses a Publish/Subscribe pattern with an Advanced Message Queuing Protocol. To get started with RabbitMQ see this link: https://www.rabbitmq.com/#getstarted

This project contains code for RabbitMQ tutorials with their ports to various languages.

# Running RabbitMQ server in Docker

<h2>Step 1</h2>
We're going to use "docker-compose" to configure the container name, the volumes and networks, and the ports that RabbitMQ will use. Doing so ensures that everything is isolated and easy to modify.

Create a folder called "rabbitmq-go"

<h2>Step 2</h2>
Change your current directory to the "rabbitmq-go" and create a new file "docker-compose.yml". Inside that file, add the following:

``` 
version: "3.2"
services:
  rabbitmq:
    image: rabbitmq:3-management-alpine
    container_name: 'rabbitmq'
    ports:
        - 5672:5672
        - 15672:15672
    volumes:
        - ~/.docker-conf/rabbitmq/data/:/var/lib/rabbitmq/
        - ~/.docker-conf/rabbitmq/log/:/var/log/rabbitmq
    networks:
        - rabbitmq_go_net

networks:
  rabbitmq_go_net:
    driver: bridge
```

<h2>Step 3</h2>
Open a terminal, navigate to your rabbitmq-go folder and run "docker-compose" up. Make sure that your Docker daemon is running.
This command will pull the rabbitmq:3-management-alpine image, create the container rabbitmq and start the service and webUI.

<h2>Step 4</h2>
Head over to:

``` http://localhost:15672 ``` 

You should see the RabbitMQ UI. Use guest as username and password.

# Sending and receiving messages

Make sure you are running the .NET Core 6.0 application from this repository and you got the RabbitMQ server up and running.

<h2>Sending messages</h2>
Go to the url: 

``` https://localhost:5001/api/sendmq ```

Set the following params to the GET request:

queue={queue name} <br/>
message={message}

So a send message request can look like this: 

``` https://localhost:5001/api/sendmq?queue=testqueue&message=Hi, it's me! ```

<h2>Sending messages</h2>
Go to the url: 

``` https://localhost:5001/api/receivemq ``` 

Set the following params to the GET request:

queue={queue name}

So a receive message request can look like this:

``` https://localhost:5001/api/sendmq?queue=testqueue ```




