BULK INSERT Client
FROM 'C:\Users\edlba\Downloads\clients.csv'
WITH (
FIRSTROW=2,
FIELDTERMINATOR = ',',
ROWTERMINATOR = '0x0a'
)
GO