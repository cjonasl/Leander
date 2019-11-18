ALTER PROCEDURE [dbo].[ClientList]
@ShowClientOnStop bit
AS
	SELECT ClientName as 'StoreName',
		ClientAddress as 'StoreAddress',
		ClientPostcode as 'StorePostcode',
		ClientPhone2 as 'StoreTelephone',
		ClientNotes as 'ClientNotes',
		ClientId as 'StoreId',
		ClientNotes as 'StoreNotes',CAST(ClientBookingType as BIT) as 'ClientBookingType',
		ISNULL(clientbookingdelaydays,3) as 'clientbookingdelaydays',
		ClientOnStopFg
	from Client
	Where 
	(ClientOnStopFg=0 or @ShowClientOnStop = 1)
	and Clientdisabled=0 
	and clientid <>542
GO