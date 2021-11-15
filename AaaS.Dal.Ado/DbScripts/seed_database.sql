insert into client (name, api_key) values ('client1', 'customkey1');
insert into client (name, api_key) values ('client2', 'customkey2');
insert into client (name, api_key) values ('client3', 'customkey3');

insert into LogType (name) values ('Error')

insert into telemetry (creation_time, name, client_id, creator_id) values (GETDATE(), 'log1', 1, '09b87c32-d36a-4050-861a-244f86f754a5')
insert into log (telemetry_id, type_id, message) values (1, 1, 'message from log 1')

insert into telemetry (creation_time, name, client_id, creator_id) values (GETDATE(), 'log2', 2, '09b87c32-d36a-4050-861a-244f86f754a6')
insert into log (telemetry_id, type_id, message) values (2, 1, 'message from log 2')

begin --Actions
INSERT INTO Object(type) VALUES ('AaaS.Core.SimpleAction, AaaS.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null')
INSERT INTO Action(object_id) VALUES(1)
INSERT INTO ObjectProperty(object_id, name, type, value) VALUES (1, 'Email', 'System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e', '"testmail@test.com"')
INSERT INTO ObjectProperty(object_id, name, type, value) VALUES (1, 'TemplateText', 'System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e', '"Das ist eine Testmail"')
INSERT INTO ObjectProperty(object_id, name, type, value) VALUES (1, 'Value', 'System.Int32, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e', '12')

INSERT INTO Object(type) VALUES ('AaaS.Core.SimpleAction, AaaS.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null')
INSERT INTO Action(object_id) VALUES(2)
INSERT INTO ObjectProperty(object_id, name, type, value) VALUES (2, 'Email', 'System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e', '"testmail@test.com"')
INSERT INTO ObjectProperty(object_id, name, type, value) VALUES (2, 'TemplateText', 'System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e', '"Das ist eine Testmail"')
INSERT INTO ObjectProperty(object_id, name, type, value) VALUES (2, 'Value', 'System.Int32, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e', '12')
end

begin --Detectors
INSERT INTO Object(type) VALUES ('AaaS.Core.SimpleDetector, AaaS.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null')
INSERT INTO Detector(object_id, client_id, telemetry_name, action_id, check_interval) VALUES (3, 1, 'TestTelemetry1', 1, 1000)
end
