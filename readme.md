## Software Frameworks

## Assignment 1: Messaging (Kafka Demo)

## Author: se21m024

<br/>
<br/>

# Summary

This is a containerised ASP .NET 6 web API project to demonstrate a basic Kafka integration based on the requirements defined in https://moodle.technikum-wien.at/pluginfile.php/1418596/mod_resource/content/0/assigment.pdf.
<br/>
<br/>

# Repository

Clone the following repository:
<br/>
https://github.com/se21m024/KafkaAsp
<br/>
<br/>
In the docker-compose.yml file the threshold for declining transactions can be configured. In this case all transactions with an amount of more than 2000 are declined.

![Docker Compose File](./Screenshots/DockerComposeFile.png)
<br/>
<br/>

# Run Project

Navigate to the root folder 'KafkaAsp' of the repository and execute the following command:
<br/>
docker-compose up
<br/>
<br/>
![Run project](./Screenshots/DockerCompose.png)
<br/>
<br/>
The project is then built and started.
![Containers](./Screenshots/DockerContainers.png)
<br/>
<br/>

# Test Project

After the projet is started navigate to http://localhost:3003/swagger/index.html on your local browser to open the Swagger page of the project.
![Swagger1](./Screenshots/Swagger1.png)

To create a new transation, post a transaction via http://localhost:3003/api/CoreBanking/Create.
![Create transaction](./Screenshots/Swagger2.png)

To get a summary about how many transactions have been created and how many of them have been declined, call http://localhost:3003/api/TransactionAnalytics/LaundryCheckSummary.
![Show transaction summary](./Screenshots/Swagger3.png)

The logs of the docker container 'kafka-asp' shows the flow of the messages.
![Live Logs 1](./Screenshots/DemoLog1.png)
![Live Logs 2](./Screenshots/DemoLog2.png)
