BULK INSERT Client
FROM 'C:\Users\edlba\source\repos\SWK\vz-aaas-g2-edlbauer-g2-pramendorfer\AaaS.Dal.Ado\DbScripts\SeedData\clients.csv'
WITH (
FIRSTROW=2,
FIELDTERMINATOR = ',',
ROWTERMINATOR = '0x0a'
)
GO