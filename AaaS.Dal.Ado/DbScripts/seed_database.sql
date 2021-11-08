insert into client (name, api_key) values ('client1', 'customkey1');
insert into client (name, api_key) values ('client2', 'customkey2');

insert into LogType (name) values ('Error')
insert into telemetry (creation_time, name, client_id, creator_id) values (GETDATE(), 'log1', 1, '09b87c32-d36a-4050-861a-244f86f754a5')
insert into log (telemetry_id, type_id, message) values (1, 1, 'message from log 1')