CREATE TABLE Telemetry (id int IDENTITY NOT NULL, creation_time datetime NULL, name varchar(255) NULL, client_id int NOT NULL, creator_id varchar(68) NULL, PRIMARY KEY (id));
CREATE TABLE Log (telemetry_id int NOT NULL, message varchar(1000) NULL, type_id int NOT NULL, PRIMARY KEY (telemetry_id));
CREATE TABLE Metric (telemetry_id int NOT NULL, value float NULL, PRIMARY KEY (telemetry_id));
CREATE TABLE TimeMeasurement (telemetry_id int NOT NULL, start_time datetime NULL, end_time datetime NULL, PRIMARY KEY (telemetry_id));
CREATE TABLE Client (id int IDENTITY NOT NULL, api_key varchar(68) NOT NULL UNIQUE, name varchar(255) NULL, PRIMARY KEY (id));
CREATE TABLE LogType (id int IDENTITY NOT NULL, name varchar(255) NULL, PRIMARY KEY (id));
CREATE TABLE Detector (object_id int NOT NULL, client_id int NOT NULL, telemetry_name varchar(255) NULL, action_id int NOT NULL, check_interval int NULL, PRIMARY KEY (object_id));
CREATE TABLE Action (object_id int NOT NULL, client_id int NOT NULL, name varchar(255) NOT NULL, PRIMARY KEY (object_id));
CREATE TABLE ObjectProperty (name varchar(255) NOT NULL, value varchar(500) NULL, type varchar(511) NULL, object_id int NOT NULL, PRIMARY KEY (name, object_id));
CREATE TABLE Object (id int IDENTITY NOT NULL, type varchar(511) NULL, PRIMARY KEY (id));
ALTER TABLE Log ADD CONSTRAINT FK_Log_Telemetry FOREIGN KEY (telemetry_id) REFERENCES Telemetry (id) ON DELETE Cascade;
ALTER TABLE Metric ADD CONSTRAINT FK_Metric_Telemetry FOREIGN KEY (telemetry_id) REFERENCES Telemetry (id) ON DELETE Cascade;
ALTER TABLE TimeMeasurement ADD CONSTRAINT FK_TimeMeasurement_Telemetry FOREIGN KEY (telemetry_id) REFERENCES Telemetry (id) ON DELETE Cascade;
ALTER TABLE Telemetry ADD CONSTRAINT FK_Telemetry_Client FOREIGN KEY (client_id) REFERENCES Client (id);
ALTER TABLE Log ADD CONSTRAINT FK_Log_LogType FOREIGN KEY (type_id) REFERENCES LogType (id);
ALTER TABLE Detector ADD CONSTRAINT FK_Detector_Object FOREIGN KEY (object_id) REFERENCES Object (id);
ALTER TABLE Action ADD CONSTRAINT FK_Action_Object FOREIGN KEY (object_id) REFERENCES Object (id);
ALTER TABLE Detector ADD CONSTRAINT FK_Detector_Action FOREIGN KEY (action_id) REFERENCES Action (object_id);
ALTER TABLE Detector ADD CONSTRAINT FK_Detector_Client FOREIGN KEY (client_id) REFERENCES Client (id);
ALTER TABLE ObjectProperty ADD CONSTRAINT FK_ObjectProperty_Object FOREIGN KEY (object_id) REFERENCES Object (id) ON DELETE Cascade;