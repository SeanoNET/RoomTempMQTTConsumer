# RoomTempMQTTConsumer
A MQTT client that consumes and saves metric data sent from the MXChip AZ3166 IoT Devkit

## Getting Started

These instructions will get your clone of RoomTempMQTTConsumer up and running on your local machine for development.

- Download and install [.NET Core 2.2+](https://dotnet.microsoft.com/download) 
- `cd /RoomTempMQTTConsumer/RoomTempMQTTConsumer`
- `dotnet restore`
- `dotnet build`
- Create your `appsettings.json` file see [Configuration](#configuration) 
- `dotnet run --project RoomTempMQTTConsumer/RoomTempMQTTConsumer.csproj`


### Configuration

The `MqttClient` and `DataRepository` configuration values are stored in `appsettings.json`

| Section| Name|Description|
|---|---|---|
| MqttClient |  | Mqtt client settings |
|  | ClientId | The MQTT device client id|
|  | MqttServerIp | The MQTT broker server IP|
|  | MqttServerPort | The MQTT broker server port|
|  | MqttSubscribeTopic | The MQTT topic if using the [RoomTempDevice-MQTT](https://github.com/SeanoNET/RoomTempDevice-MQTT) this will be the same topic set in `topic`|
| DataRepository |  | MSSQL Server data repository settings |
|  | DataSource | MSSQL connection string |
|  | TableName | MSSQL table name to save the metric data see [Creating the SQL Table](#creating-the-sql-table)|

Example:
```JSON
{
  "MqttClient": {
    "ClientId": "metric-consumer",
    "MqttServerIp": "localhost",
    "MqttServerPort": "1883",
    "MqttSubscribeTopic": "home/room/temp-mon/data"
  },
  "DataRepository": {
    "DataSource": "Server=(local);Database=MQTT;Trusted_Connection=True;",
    "TableName": "TempMonData"
  }
}
```
## Running locally in Docker

Install [Docker](https://docs.docker.com/get-docker/) and [Docker Compose](https://docs.docker.com/compose/install/)

### Building the image

Build the image with

`docker build -t roomtempmqttconsumer:latest .`

### Running in Docker Compose

create `docker-compose.yml`

```
version: '3'

services:

  consumer:
    build: .
    image: seanonet/roomtempmqttconsumer:latest
    environment:
      DataSource: Server=dbdata;Database=MQTT;User Id=sa;Password=St0ngPassword1!;
      ClientId: metric-consumer
      MqttServerIp: mqtt
      MqttServerPort: 1883
      MqttSubscribeTopic: home/room/temp-mon/data
    depends_on:
        - mqtt
        - dbdata
  mqtt:
    image: eclipse-mosquitto:1.6
    ports:
        - 1883:1883
        - 9001:9001
    volumes:
        - mosquitto:/mosquitto/data
        - mosquitto:/mosquitto/log eclipse-mosquitto
  dbdata:
    image: mcr.microsoft.com/mssql/server:2019-latest
    ports:
        - 1433:1433
    environment:
        SA_PASSWORD: St0ngPassword1!
        ACCEPT_EULA: Y
        MSSQL_PID: Express
    volumes:
        - dbsql:/var/opt/mssql
  dashboard:
    image: seanonet/roomtempdashboard:latest
    ports:
        - 5000:5000
    environment:
        DataSource: Server=dbdata;Database=MQTT;User Id=sa;Password=St0ngPassword1!;
        ASPNETCORE_URLS: http://0.0.0.0:5000

volumes:
    mosquitto:
    dbsql:
```

Start stack with `docker-compose up --build`

## Running on Swarm

Init swarm with `docker swarm init` and create a `docker-compose.yml` see [Running in Docker Compose](#running-in-docker-compose)

`docker stack deploy --compose-file docker-compose.yml roomtempstack`


## MQTT device

To send data to this consumer see [RoomTempDevice-MQTT](https://github.com/SeanoNET/RoomTempDevice-MQTT)