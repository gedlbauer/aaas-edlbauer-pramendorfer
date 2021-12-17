INSERT INTO client (name, api_key) values ('client1', 'customkey1');
INSERT INTO client (name, api_key) values ('client2', 'customkey2');
INSERT INTO client (name, api_key) values ('client3', 'customkey3');

-- Logs
INSERT INTO LogType (name) VALUES ('Error')
INSERT INTO LogType (name) VALUES ('Message')
INSERT INTO LogType (name) VALUES ('Warning')

INSERT INTO telemetry (creation_time, name, client_id, creator_id) VALUES (GETDATE(), 'log1', 1, '09b87c32-d36a-4050-861a-244f86f754a5')
INSERT INTO log (telemetry_id, type_id, message) VALUES (1, 1, 'message from log 1')
INSERT INTO telemetry (creation_time, name, client_id, creator_id) VALUES (GETDATE(), 'log2', 2, '09b87c32-d36a-4050-861a-244f86f754a6')
INSERT INTO log (telemetry_id, type_id, message) VALUES (2, 1, 'message from log 2')

-- Metrics TimeMeasurements
INSERT INTO telemetry (creation_time, name, client_id, creator_id) VALUES (GETDATE(), 'metric1', 1, '09b87c32-d36a-4050-861a-244f86f754a7')
INSERT INTO telemetry (creation_time, name, client_id, creator_id) VALUES (GETDATE(), 'metric2', 2, '09b87c32-d36a-4050-861a-244f86f754a8')
INSERT INTO metric (telemetry_id, value) VALUES (3, 10)
INSERT INTO metric (telemetry_id, value) VALUES (4, 20)

-- TimeMeasurements
INSERT INTO telemetry (creation_time, name, client_id, creator_id) VALUES (GETDATE(), 'time1', 1, '09b87c32-d36a-4050-861a-244f86f754a7')
INSERT INTO telemetry (creation_time, name, client_id, creator_id) VALUES (GETDATE(), 'time2', 2, '09b87c32-d36a-4050-861a-244f86f754a8')
INSERT INTO telemetry (creation_time, name, client_id, creator_id) VALUES (GETDATE(), 'time2', 2, '09b87c32-d36a-4050-861a-244f86f754a8')
INSERT INTO TimeMeasurement (telemetry_id, start_time, end_time) VALUES (5, GETDATE()-1, GETDATE())
INSERT INTO TimeMeasurement (telemetry_id, start_time, end_time) VALUES (6, GETDATE()-2, GETDATE())
INSERT INTO TimeMeasurement (telemetry_id, start_time, end_time) VALUES (7, GETDATE()-1, GETDATE())

begin --Actions
INSERT INTO Object(type) VALUES ('AaaS.Dal.Tests.TestObjects.SimpleAction, AaaS.Dal.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null')
INSERT INTO Action(object_id, client_id, name) VALUES(1, 1, 'Simple1')
INSERT INTO ObjectProperty(object_id, name, type, value) VALUES (1, 'Email', 'System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e', '"testmail@test.com"')
INSERT INTO ObjectProperty(object_id, name, type, value) VALUES (1, 'TemplateText', 'System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e', '"Das ist eine Testmail"')
INSERT INTO ObjectProperty(object_id, name, type, value) VALUES (1, 'Value', 'System.Int32, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e', '12')

INSERT INTO Object(type) VALUES ('AaaS.Dal.Tests.TestObjects.SimpleAction, AaaS.Dal.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null')
INSERT INTO Action(object_id, client_id, name) VALUES(2, 2, 'Simple2')
INSERT INTO ObjectProperty(object_id, name, type, value) VALUES (2, 'Email', 'System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e', '"testmail@test.com"')
INSERT INTO ObjectProperty(object_id, name, type, value) VALUES (2, 'TemplateText', 'System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e', '"Das ist eine Testmail"')
INSERT INTO ObjectProperty(object_id, name, type, value) VALUES (2, 'Value', 'System.Int32, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e', '12')
end

begin --Detectors
INSERT INTO Object(type) VALUES ('AaaS.Dal.Tests.TestObjects.SimpleDetector, AaaS.Dal.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null')
INSERT INTO Detector(object_id, client_id, telemetry_name, action_id, check_interval) VALUES (3, 1, 'TestTelemetry1', 1, 1000)
end
