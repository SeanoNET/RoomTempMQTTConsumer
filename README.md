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
|  | TableName | MSSQL table name to save the metric data see [Creating SQL Table](#creating-sql-table)|

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
    "TableName": "Test"
  }
}
```


### Creating the SQL Table

The payload [MetricData](RoomTempMQTTConsumer/Entities/MetricData.cs) will be saved into a sql table, create the sql table below in your sql instance

```SQL
CREATE TABLE [dbo].[Test](
	[Temperature] [decimal](18, 2) NULL,
	[Humidity] [decimal](18, 2) NULL,
	[MeasuredAt] [datetime] NULL
) ON [PRIMARY]
GO
```

Notice the table name `Test` is set in the configuration file under `DataRepository` `TableName`

## MQTT device

To send data to this consumer see [RoomTempDevice-MQTT](https://github.com/SeanoNET/RoomTempDevice-MQTT)