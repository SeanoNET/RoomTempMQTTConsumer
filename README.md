# RoomTempMQTTConsumer
![Build Docker CLI](https://github.com/SeanoNET/RoomTempMQTTConsumer/workflows/Build%20Docker%20CLI/badge.svg)

A MQTT client that consumes and saves metric data sent from the MXChip AZ3166 IoT Devkit see [RoomTempDevice-MQTT](https://github.com/SeanoNET/RoomTempDevice-MQTT). For viewing the metrics in a browser see [RoomTempDashboard](https://github.com/SeanoNET/RoomTempDashboard/)

## Getting Started

These instructions will get your clone of RoomTempMQTTConsumer up and running on your local machine for development.

Clone this [repository](https://github.com/SeanoNET/RoomTempMQTTConsumer).

`git clone https://github.com/SeanoNET/RoomTempMQTTConsumer.git`

- Download and install [.NET Core 3.1+](https://dotnet.microsoft.com/download) 
- `cd /RoomTempMQTTConsumer/src`
- `dotnet restore`
- `dotnet build`
- Create `appsettings.json` file see [Configuration](#configuration) 
- `dotnet run`

### Configuration

Create and configure the `MqttClient` and the [Postgres](https://www.postgresql.org/) `DataSource` connection string in `appsettings.json`.

| Name|Description|
|---|---|
| ClientId | The MQTT device client id|
| MqttServerIp | The MQTT broker server IP|
| MqttServerPort | The MQTT broker server port|
| MqttSubscribeTopic | The MQTT topic if using the [RoomTempDevice-MQTT](https://github.com/SeanoNET/RoomTempDevice-MQTT) this will be the same topic set in `topic`|
| DataSource | MSSQL connection string |

`appsettings.json`
```JSON
{
  "ClientId": "metric-consumer",
  "MqttServerIp": "localhost",
  "MqttServerPort": "1883",
  "MqttSubscribeTopic": "home/room/temp-mon/data",
  "DataSource": "Host=dbdata;Database=MQTT;Username=postgres;Password=St0ngPassword1!;"
}
```
## Running locally in Docker

Install [Docker](https://docs.docker.com/get-docker/) and [Docker Compose](https://docs.docker.com/compose/install/)

### Running in Docker Compose

Create `docker-compose.yml`

```
version: '3'

services:

  consumer:
    build: .
    image: roomtempmqttconsumer
    environment:
      DataSource: Host=dbdata;Database=MQTT;Username=postgres;Password=St0ngPassword1!;
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
    image: postgres:12
    ports:
    - 5432:5432
    environment:
        POSTGRES_PASSWORD: St0ngPassword1!
    volumes:
        - dbsql:/var/lib/postgresql/data
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

Start the stack with `docker-compose up --build`

## Running Stack in Production on Swarm

For running in production see [RoomTempStack](https://github.com/SeanoNET/RoomTempStack)


## MQTT device

To send data to this consumer see [RoomTempDevice-MQTT](https://github.com/SeanoNET/RoomTempDevice-MQTT) or you can test with [MQTT Explorer](http://mqtt-explorer.com/)