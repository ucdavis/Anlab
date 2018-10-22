CREATE VIEW dbo.ReviewerOrders
AS
SELECT        Id, RequestNum, ClientId, PaymentType, Created, Updated, Status, Paid, DateFinalized, CAST(JSON_VALUE(JsonDetails, '$.GrandTotal') AS DECIMAL(18, 2)) AS GrandTotal
FROM            dbo.Orders
WHERE        (IsDeleted = 0)
GO


